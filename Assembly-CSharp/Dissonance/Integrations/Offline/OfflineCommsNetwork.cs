using System;
using System.Collections.Generic;
using Dissonance.Audio.Playback;
using Dissonance.Extensions;
using Dissonance.Networking;
using JetBrains.Annotations;
using UnityEngine;

namespace Dissonance.Integrations.Offline
{
	// Token: 0x02000246 RID: 582
	public class OfflineCommsNetwork : MonoBehaviour, ICommsNetwork
	{
		// Token: 0x17000262 RID: 610
		// (get) Token: 0x06001705 RID: 5893 RVA: 0x0005FC8F File Offset: 0x0005DE8F
		// (set) Token: 0x06001706 RID: 5894 RVA: 0x0005FC97 File Offset: 0x0005DE97
		public int LoopbackPacketCount { get; private set; }

		// Token: 0x17000263 RID: 611
		// (get) Token: 0x06001707 RID: 5895 RVA: 0x0000CA50 File Offset: 0x0000AC50
		public ConnectionStatus Status
		{
			get
			{
				return ConnectionStatus.Connected;
			}
		}

		// Token: 0x06001708 RID: 5896 RVA: 0x0005FCA0 File Offset: 0x0005DEA0
		public void Initialize(string playerName, Rooms rooms, PlayerChannels playerChannels, RoomChannels roomChannels, CodecSettings codecSettings)
		{
			this._codecSettings = new CodecSettings?(codecSettings);
			this._loopbackChannels.Add(new RemoteChannel("Loopback", ChannelType.Room, new PlaybackOptions(false, 1f, ChannelPriority.Default)));
			roomChannels.OpenedChannel += this.BeginLoopback;
			roomChannels.ClosedChannel += this.EndLoopback;
		}

		// Token: 0x06001709 RID: 5897 RVA: 0x0005FD02 File Offset: 0x0005DF02
		private void BeginLoopback(RoomName channel, ChannelProperties props)
		{
			this._loopbackActive = true;
		}

		// Token: 0x0600170A RID: 5898 RVA: 0x0005FD0B File Offset: 0x0005DF0B
		private void EndLoopback(RoomName channel, ChannelProperties props)
		{
			if (this._sentStartedSpeakingEvent)
			{
				Action<string> playerStoppedSpeaking = this.PlayerStoppedSpeaking;
				if (playerStoppedSpeaking != null)
				{
					playerStoppedSpeaking("Loopback");
				}
			}
			this._loopbackQueue.Clear();
			this._sentStartedSpeakingEvent = false;
			this._loopbackActive = false;
			this._loopbackSequenceNumber = 0U;
		}

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x0600170B RID: 5899 RVA: 0x0000CA50 File Offset: 0x0000AC50
		public NetworkMode Mode
		{
			get
			{
				return NetworkMode.Client;
			}
		}

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x0600170C RID: 5900 RVA: 0x0005FD4C File Offset: 0x0005DF4C
		// (remove) Token: 0x0600170D RID: 5901 RVA: 0x0005FD84 File Offset: 0x0005DF84
		public event Action<string, CodecSettings> PlayerJoined;

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x0600170E RID: 5902 RVA: 0x0005FDBC File Offset: 0x0005DFBC
		// (remove) Token: 0x0600170F RID: 5903 RVA: 0x0005FDF4 File Offset: 0x0005DFF4
		public event Action<VoicePacket> VoicePacketReceived;

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06001710 RID: 5904 RVA: 0x0005FE2C File Offset: 0x0005E02C
		// (remove) Token: 0x06001711 RID: 5905 RVA: 0x0005FE64 File Offset: 0x0005E064
		public event Action<string> PlayerStartedSpeaking;

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06001712 RID: 5906 RVA: 0x0005FE9C File Offset: 0x0005E09C
		// (remove) Token: 0x06001713 RID: 5907 RVA: 0x0005FED4 File Offset: 0x0005E0D4
		public event Action<string> PlayerStoppedSpeaking;

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x06001714 RID: 5908 RVA: 0x0005FF0C File Offset: 0x0005E10C
		// (remove) Token: 0x06001715 RID: 5909 RVA: 0x0005FF44 File Offset: 0x0005E144
		public event Action<NetworkMode> ModeChanged;

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x06001716 RID: 5910 RVA: 0x0005FF7C File Offset: 0x0005E17C
		// (remove) Token: 0x06001717 RID: 5911 RVA: 0x0005FFB4 File Offset: 0x0005E1B4
		public event Action<string> PlayerLeft;

		// Token: 0x1400000E RID: 14
		// (add) Token: 0x06001718 RID: 5912 RVA: 0x0005FFEC File Offset: 0x0005E1EC
		// (remove) Token: 0x06001719 RID: 5913 RVA: 0x00060024 File Offset: 0x0005E224
		public event Action<TextMessage> TextPacketReceived;

		// Token: 0x1400000F RID: 15
		// (add) Token: 0x0600171A RID: 5914 RVA: 0x0006005C File Offset: 0x0005E25C
		// (remove) Token: 0x0600171B RID: 5915 RVA: 0x00060094 File Offset: 0x0005E294
		public event Action<RoomEvent> PlayerEnteredRoom;

		// Token: 0x14000010 RID: 16
		// (add) Token: 0x0600171C RID: 5916 RVA: 0x000600CC File Offset: 0x0005E2CC
		// (remove) Token: 0x0600171D RID: 5917 RVA: 0x00060104 File Offset: 0x0005E304
		public event Action<RoomEvent> PlayerExitedRoom;

		// Token: 0x0600171E RID: 5918 RVA: 0x0006013C File Offset: 0x0005E33C
		public void SendVoice(ArraySegment<byte> data)
		{
			if (!this._loopbackActive)
			{
				return;
			}
			ArraySegment<byte> arraySegment = data.CopyToSegment((this._bufferPool.Count > 0) ? this._bufferPool.Dequeue() : new byte[1024], 0);
			int loopbackPacketCount = this.LoopbackPacketCount;
			this.LoopbackPacketCount = loopbackPacketCount + 1;
			Queue<VoicePacket> loopbackQueue = this._loopbackQueue;
			string text = "Loopback";
			ChannelPriority channelPriority = ChannelPriority.Default;
			float num = 1f;
			bool flag = false;
			ArraySegment<byte> arraySegment2 = arraySegment;
			uint loopbackSequenceNumber = this._loopbackSequenceNumber;
			this._loopbackSequenceNumber = loopbackSequenceNumber + 1U;
			loopbackQueue.Enqueue(new VoicePacket(text, channelPriority, num, flag, arraySegment2, loopbackSequenceNumber, this._loopbackChannels));
		}

		// Token: 0x0600171F RID: 5919 RVA: 0x000021EF File Offset: 0x000003EF
		public void SendText([CanBeNull] string data, ChannelType recipientType, string recipientId)
		{
		}

		// Token: 0x06001720 RID: 5920 RVA: 0x000601C3 File Offset: 0x0005E3C3
		private void Update()
		{
			this.JoinFakePlayer();
			if (this._playerJoined)
			{
				this.PumpLoopback();
			}
		}

		// Token: 0x06001721 RID: 5921 RVA: 0x000601DC File Offset: 0x0005E3DC
		private void JoinFakePlayer()
		{
			if (this._playerJoined)
			{
				return;
			}
			if (this._codecSettings == null)
			{
				return;
			}
			Action<string, CodecSettings> playerJoined = this.PlayerJoined;
			if (playerJoined != null)
			{
				playerJoined("Loopback", this._codecSettings.Value);
			}
			this._playerJoined = true;
		}

		// Token: 0x06001722 RID: 5922 RVA: 0x00060228 File Offset: 0x0005E428
		private void PumpLoopback()
		{
			if (!this._loopbackActive)
			{
				return;
			}
			if (!this._sentStartedSpeakingEvent && this._loopbackQueue.Count < 5)
			{
				return;
			}
			if (!this._sentStartedSpeakingEvent)
			{
				Action<string> playerStartedSpeaking = this.PlayerStartedSpeaking;
				if (playerStartedSpeaking != null)
				{
					playerStartedSpeaking("Loopback");
				}
				this._sentStartedSpeakingEvent = true;
			}
			while (this._loopbackQueue.Count > 0)
			{
				VoicePacket voicePacket = this._loopbackQueue.Dequeue();
				Action<VoicePacket> voicePacketReceived = this.VoicePacketReceived;
				if (voicePacketReceived != null)
				{
					voicePacketReceived(voicePacket);
				}
				this._bufferPool.Enqueue(voicePacket.EncodedAudioFrame.Array);
			}
		}

		// Token: 0x04000D39 RID: 3385
		private bool _loopbackActive;

		// Token: 0x04000D3A RID: 3386
		private bool _sentStartedSpeakingEvent;

		// Token: 0x04000D3B RID: 3387
		private uint _loopbackSequenceNumber;

		// Token: 0x04000D3C RID: 3388
		private readonly List<RemoteChannel> _loopbackChannels = new List<RemoteChannel>();

		// Token: 0x04000D3D RID: 3389
		private readonly Queue<byte[]> _bufferPool = new Queue<byte[]>();

		// Token: 0x04000D3E RID: 3390
		private readonly Queue<VoicePacket> _loopbackQueue = new Queue<VoicePacket>(128);

		// Token: 0x04000D3F RID: 3391
		private bool _playerJoined;

		// Token: 0x04000D40 RID: 3392
		private CodecSettings? _codecSettings;
	}
}
