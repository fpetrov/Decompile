using System;
using Dissonance;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

namespace Gameplay.VoiceChat
{
	// Token: 0x02000247 RID: 583
	public class CustomDissonancePlayer : NetworkBehaviour, IDissonancePlayer
	{
		// Token: 0x17000265 RID: 613
		// (get) Token: 0x06001724 RID: 5924 RVA: 0x000602ED File Offset: 0x0005E4ED
		// (set) Token: 0x06001725 RID: 5925 RVA: 0x000602F5 File Offset: 0x0005E4F5
		public bool IsTracking { get; private set; }

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x06001726 RID: 5926 RVA: 0x000602FE File Offset: 0x0005E4FE
		// (set) Token: 0x06001727 RID: 5927 RVA: 0x00060306 File Offset: 0x0005E506
		public string PlayerId { get; private set; }

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x06001728 RID: 5928 RVA: 0x0006030F File Offset: 0x0005E50F
		public Vector3 Position
		{
			get
			{
				return base.transform.position;
			}
		}

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x06001729 RID: 5929 RVA: 0x0006031C File Offset: 0x0005E51C
		public Quaternion Rotation
		{
			get
			{
				return base.transform.rotation;
			}
		}

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x0600172A RID: 5930 RVA: 0x00060329 File Offset: 0x0005E529
		public NetworkPlayerType Type
		{
			get
			{
				if (this._comms == null || this.PlayerId == null)
				{
					return NetworkPlayerType.Unknown;
				}
				if (!this._comms.LocalPlayerName.Equals(this.PlayerId))
				{
					return NetworkPlayerType.Remote;
				}
				return NetworkPlayerType.Local;
			}
		}

		// Token: 0x0600172B RID: 5931 RVA: 0x0006035E File Offset: 0x0005E55E
		public void OnDestroy()
		{
			if (this._comms != null)
			{
				this._comms.LocalPlayerNameChanged -= this.SetPlayerName;
			}
		}

		// Token: 0x0600172C RID: 5932 RVA: 0x00060385 File Offset: 0x0005E585
		public void OnEnable()
		{
			this._comms = Object.FindAnyObjectByType<DissonanceComms>();
			if (this._comms == null)
			{
				Debug.Log("Cannot find DissonanceComms component in scene");
			}
		}

		// Token: 0x0600172D RID: 5933 RVA: 0x000603AA File Offset: 0x0005E5AA
		public void OnDisable()
		{
			if (this.IsTracking)
			{
				this.StopTracking();
			}
		}

		// Token: 0x0600172E RID: 5934 RVA: 0x000603BA File Offset: 0x0005E5BA
		public override void OnOwnershipClient(NetworkConnection prevOwner)
		{
			base.OnOwnershipClient(prevOwner);
			if (this._comms.LocalPlayerName != null)
			{
				this.SetPlayerName(this._comms.LocalPlayerName);
			}
			this._comms.LocalPlayerNameChanged += this.SetPlayerName;
		}

		// Token: 0x0600172F RID: 5935 RVA: 0x000603F8 File Offset: 0x0005E5F8
		private void SetPlayerName(string playerName)
		{
			if (this.IsTracking)
			{
				this.StopTracking();
			}
			this.PlayerId = playerName;
			this.StartTracking();
			if (base.IsOwner)
			{
				this.SetPlayerNameServerRpc(playerName);
			}
		}

		// Token: 0x06001730 RID: 5936 RVA: 0x00060424 File Offset: 0x0005E624
		public override void OnStartClient()
		{
			base.OnStartClient();
			if (!string.IsNullOrEmpty(this.PlayerId))
			{
				this.StartTracking();
			}
		}

		// Token: 0x06001731 RID: 5937 RVA: 0x0006043F File Offset: 0x0005E63F
		[ServerRpc]
		private void SetPlayerNameServerRpc(string playerName)
		{
			this.RpcWriter___Server_SetPlayerNameServerRpc_3615296227(playerName);
		}

		// Token: 0x06001732 RID: 5938 RVA: 0x0006044B File Offset: 0x0005E64B
		[ObserversRpc(BufferLast = true)]
		private void SetPlayerNameObserversRpc(string playerName)
		{
			this.RpcWriter___Observers_SetPlayerNameObserversRpc_3615296227(playerName);
		}

		// Token: 0x06001733 RID: 5939 RVA: 0x00060457 File Offset: 0x0005E657
		private void StartTracking()
		{
			if (this.IsTracking)
			{
				Debug.Log("Attempting to start player tracking, but tracking is already started");
			}
			if (this._comms != null)
			{
				this._comms.TrackPlayerPosition(this);
				this.IsTracking = true;
			}
		}

		// Token: 0x06001734 RID: 5940 RVA: 0x0006048C File Offset: 0x0005E68C
		private void StopTracking()
		{
			if (!this.IsTracking)
			{
				Debug.Log("Attempting to stop player tracking, but tracking is not started");
			}
			if (this._comms != null)
			{
				this._comms.StopTracking(this);
				this.IsTracking = false;
			}
		}

		// Token: 0x06001736 RID: 5942 RVA: 0x000604C4 File Offset: 0x0005E6C4
		public virtual void NetworkInitialize___Early()
		{
			if (this.NetworkInitialize___EarlyGameplay.VoiceChat.CustomDissonancePlayerAssembly-CSharp.dll_Excuted)
			{
				return;
			}
			this.NetworkInitialize___EarlyGameplay.VoiceChat.CustomDissonancePlayerAssembly-CSharp.dll_Excuted = true;
			base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_SetPlayerNameServerRpc_3615296227));
			base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_SetPlayerNameObserversRpc_3615296227));
		}

		// Token: 0x06001737 RID: 5943 RVA: 0x00060510 File Offset: 0x0005E710
		public virtual void NetworkInitialize__Late()
		{
			if (this.NetworkInitialize__LateGameplay.VoiceChat.CustomDissonancePlayerAssembly-CSharp.dll_Excuted)
			{
				return;
			}
			this.NetworkInitialize__LateGameplay.VoiceChat.CustomDissonancePlayerAssembly-CSharp.dll_Excuted = true;
		}

		// Token: 0x06001738 RID: 5944 RVA: 0x00060523 File Offset: 0x0005E723
		public override void NetworkInitializeIfDisabled()
		{
			this.NetworkInitialize___Early();
			this.NetworkInitialize__Late();
		}

		// Token: 0x06001739 RID: 5945 RVA: 0x00060534 File Offset: 0x0005E734
		private void RpcWriter___Server_SetPlayerNameServerRpc_3615296227(string playerName)
		{
			if (!base.IsClientInitialized)
			{
				NetworkManager networkManager = base.NetworkManager;
				networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
				return;
			}
			if (!base.IsOwner)
			{
				NetworkManager networkManager2 = base.NetworkManager;
				networkManager2.LogWarning("Cannot complete action because you are not the owner of this object. .");
				return;
			}
			Channel channel = Channel.Reliable;
			PooledWriter pooledWriter = WriterPool.Retrieve();
			pooledWriter.WriteString(playerName);
			base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
			pooledWriter.Store();
		}

		// Token: 0x0600173A RID: 5946 RVA: 0x000605CB File Offset: 0x0005E7CB
		private void RpcLogic___SetPlayerNameServerRpc_3615296227(string playerName)
		{
			this.PlayerId = playerName;
			this.SetPlayerNameObserversRpc(playerName);
		}

		// Token: 0x0600173B RID: 5947 RVA: 0x000605DC File Offset: 0x0005E7DC
		private void RpcReader___Server_SetPlayerNameServerRpc_3615296227(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
		{
			string text = PooledReader0.ReadString();
			if (!base.IsServerInitialized)
			{
				return;
			}
			if (!base.OwnerMatches(conn))
			{
				return;
			}
			this.RpcLogic___SetPlayerNameServerRpc_3615296227(text);
		}

		// Token: 0x0600173C RID: 5948 RVA: 0x00060620 File Offset: 0x0005E820
		private void RpcWriter___Observers_SetPlayerNameObserversRpc_3615296227(string playerName)
		{
			if (!base.IsServerInitialized)
			{
				NetworkManager networkManager = base.NetworkManager;
				networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
				return;
			}
			Channel channel = Channel.Reliable;
			PooledWriter pooledWriter = WriterPool.Retrieve();
			pooledWriter.WriteString(playerName);
			base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, true, false, false);
			pooledWriter.Store();
		}

		// Token: 0x0600173D RID: 5949 RVA: 0x000606A1 File Offset: 0x0005E8A1
		private void RpcLogic___SetPlayerNameObserversRpc_3615296227(string playerName)
		{
			if (!base.IsOwner)
			{
				this.SetPlayerName(playerName);
			}
		}

		// Token: 0x0600173E RID: 5950 RVA: 0x000606B4 File Offset: 0x0005E8B4
		private void RpcReader___Observers_SetPlayerNameObserversRpc_3615296227(PooledReader PooledReader0, Channel channel)
		{
			string text = PooledReader0.ReadString();
			if (!base.IsClientInitialized)
			{
				return;
			}
			this.RpcLogic___SetPlayerNameObserversRpc_3615296227(text);
		}

		// Token: 0x0600173F RID: 5951 RVA: 0x00060523 File Offset: 0x0005E723
		public virtual void Awake()
		{
			this.NetworkInitialize___Early();
			this.NetworkInitialize__Late();
		}

		// Token: 0x04000D4B RID: 3403
		private DissonanceComms _comms;

		// Token: 0x04000D4E RID: 3406
		private bool NetworkInitialize___EarlyGameplay.VoiceChat.CustomDissonancePlayerAssembly-CSharp.dll_Excuted;

		// Token: 0x04000D4F RID: 3407
		private bool NetworkInitialize__LateGameplay.VoiceChat.CustomDissonancePlayerAssembly-CSharp.dll_Excuted;
	}
}
