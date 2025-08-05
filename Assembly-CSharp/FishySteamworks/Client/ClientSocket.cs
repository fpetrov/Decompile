using System;
using System.Diagnostics;
using System.Threading;
using FishNet.Managing;
using FishNet.Transporting;
using Steamworks;
using UnityEngine;

namespace FishySteamworks.Client
{
	// Token: 0x0200023E RID: 574
	public class ClientSocket : CommonSocket
	{
		// Token: 0x060016AF RID: 5807 RVA: 0x0005E670 File Offset: 0x0005C870
		private void CheckTimeout()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			do
			{
				if ((float)(stopwatch.ElapsedMilliseconds / 1000L) > this._connectTimeout)
				{
					this.StopConnection();
				}
				Thread.Sleep(50);
			}
			while (base.GetLocalConnectionState() == LocalConnectionState.Starting);
			stopwatch.Stop();
			this._timeoutThread.Abort();
		}

		// Token: 0x060016B0 RID: 5808 RVA: 0x0005E6C8 File Offset: 0x0005C8C8
		internal bool StartConnection(string address, ushort port, bool peerToPeer)
		{
			try
			{
				if (this._onLocalConnectionStateCallback == null)
				{
					this._onLocalConnectionStateCallback = Callback<SteamNetConnectionStatusChangedCallback_t>.Create(new Callback<SteamNetConnectionStatusChangedCallback_t>.DispatchDelegate(this.OnLocalConnectionState));
				}
				this.PeerToPeer = peerToPeer;
				byte[] array = ((!peerToPeer) ? base.GetIPBytes(address) : null);
				if (!peerToPeer && array == null)
				{
					base.SetLocalConnectionState(LocalConnectionState.Stopped, false);
					return false;
				}
				base.SetLocalConnectionState(LocalConnectionState.Starting, false);
				this._connectTimeout = Time.unscaledTime + 8000f;
				this._timeoutThread = new Thread(new ThreadStart(this.CheckTimeout));
				this._timeoutThread.Start();
				this._hostSteamID = new CSteamID(ulong.Parse(address));
				SteamNetworkingIdentity steamNetworkingIdentity = default(SteamNetworkingIdentity);
				steamNetworkingIdentity.SetSteamID(this._hostSteamID);
				SteamNetworkingConfigValue_t[] array2 = new SteamNetworkingConfigValue_t[0];
				if (this.PeerToPeer)
				{
					this._socket = SteamNetworkingSockets.ConnectP2P(ref steamNetworkingIdentity, 0, array2.Length, array2);
				}
				else
				{
					SteamNetworkingIPAddr steamNetworkingIPAddr = default(SteamNetworkingIPAddr);
					steamNetworkingIPAddr.Clear();
					steamNetworkingIPAddr.SetIPv6(array, port);
					this._socket = SteamNetworkingSockets.ConnectByIPAddress(ref steamNetworkingIPAddr, 0, array2);
				}
			}
			catch
			{
				base.SetLocalConnectionState(LocalConnectionState.Stopped, false);
				return false;
			}
			return true;
		}

		// Token: 0x060016B1 RID: 5809 RVA: 0x0005E7EC File Offset: 0x0005C9EC
		private void OnLocalConnectionState(SteamNetConnectionStatusChangedCallback_t args)
		{
			if (args.m_info.m_eState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected)
			{
				base.SetLocalConnectionState(LocalConnectionState.Started, false);
				return;
			}
			if (args.m_info.m_eState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer || args.m_info.m_eState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally)
			{
				this.Transport.NetworkManager.Log("Connection was closed by peer, " + args.m_info.m_szEndDebug);
				this.StopConnection();
				return;
			}
			this.Transport.NetworkManager.Log("Connection state changed: " + args.m_info.m_eState.ToString() + " - " + args.m_info.m_szEndDebug);
		}

		// Token: 0x060016B2 RID: 5810 RVA: 0x0005E89C File Offset: 0x0005CA9C
		internal bool StopConnection()
		{
			if (this._timeoutThread != null && this._timeoutThread.IsAlive)
			{
				this._timeoutThread.Abort();
			}
			if (this._socket != HSteamNetConnection.Invalid)
			{
				if (this._onLocalConnectionStateCallback != null)
				{
					this._onLocalConnectionStateCallback.Dispose();
					this._onLocalConnectionStateCallback = null;
				}
				SteamNetworkingSockets.CloseConnection(this._socket, 0, string.Empty, false);
				this._socket = HSteamNetConnection.Invalid;
			}
			if (base.GetLocalConnectionState() == LocalConnectionState.Stopped || base.GetLocalConnectionState() == LocalConnectionState.Stopping)
			{
				return false;
			}
			base.SetLocalConnectionState(LocalConnectionState.Stopping, false);
			base.SetLocalConnectionState(LocalConnectionState.Stopped, false);
			return true;
		}

		// Token: 0x060016B3 RID: 5811 RVA: 0x0005E938 File Offset: 0x0005CB38
		internal void IterateIncoming()
		{
			if (base.GetLocalConnectionState() != LocalConnectionState.Started)
			{
				return;
			}
			int num = SteamNetworkingSockets.ReceiveMessagesOnConnection(this._socket, this.MessagePointers, 256);
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					ArraySegment<byte> arraySegment;
					byte b;
					base.GetMessage(this.MessagePointers[i], this.InboundBuffer, out arraySegment, out b);
					this.Transport.HandleClientReceivedDataArgs(new ClientReceivedDataArgs(arraySegment, (Channel)b, this.Transport.Index));
				}
			}
		}

		// Token: 0x060016B4 RID: 5812 RVA: 0x0005E9AC File Offset: 0x0005CBAC
		internal void SendToServer(byte channelId, ArraySegment<byte> segment)
		{
			if (base.GetLocalConnectionState() != LocalConnectionState.Started)
			{
				return;
			}
			EResult eresult = base.Send(this._socket, segment, channelId);
			if (eresult == EResult.k_EResultNoConnection || eresult == EResult.k_EResultInvalidParam)
			{
				this.Transport.NetworkManager.Log("Connection to server was lost.");
				this.StopConnection();
				return;
			}
			if (eresult != EResult.k_EResultOK)
			{
				this.Transport.NetworkManager.LogError("Could not send: " + eresult.ToString());
			}
		}

		// Token: 0x060016B5 RID: 5813 RVA: 0x0005EA22 File Offset: 0x0005CC22
		internal void IterateOutgoing()
		{
			if (base.GetLocalConnectionState() != LocalConnectionState.Started)
			{
				return;
			}
			SteamNetworkingSockets.FlushMessagesOnConnection(this._socket);
		}

		// Token: 0x04000D0D RID: 3341
		private Callback<SteamNetConnectionStatusChangedCallback_t> _onLocalConnectionStateCallback;

		// Token: 0x04000D0E RID: 3342
		private CSteamID _hostSteamID = CSteamID.Nil;

		// Token: 0x04000D0F RID: 3343
		private HSteamNetConnection _socket;

		// Token: 0x04000D10 RID: 3344
		private Thread _timeoutThread;

		// Token: 0x04000D11 RID: 3345
		private float _connectTimeout = -1f;

		// Token: 0x04000D12 RID: 3346
		private const float CONNECT_TIMEOUT_DURATION = 8000f;
	}
}
