using System;
using FishNet.Managing;
using FishNet.Transporting;
using FishySteamworks.Client;
using FishySteamworks.Server;
using Steamworks;
using UnityEngine;

namespace FishySteamworks
{
	// Token: 0x0200023A RID: 570
	public class FishySteamworks : Transport
	{
		// Token: 0x06001663 RID: 5731 RVA: 0x0005D3C4 File Offset: 0x0005B5C4
		~FishySteamworks()
		{
			this.Shutdown();
		}

		// Token: 0x06001664 RID: 5732 RVA: 0x0005D3F0 File Offset: 0x0005B5F0
		public override void Initialize(NetworkManager networkManager, int transportIndex)
		{
			base.Initialize(networkManager, transportIndex);
			this._client = new ClientSocket();
			this._clientHost = new ClientHostSocket();
			this._server = new ServerSocket();
			this.CreateChannelData();
			this._client.Initialize(this);
			this._clientHost.Initialize(this);
			this._server.Initialize(this);
		}

		// Token: 0x06001665 RID: 5733 RVA: 0x0005D450 File Offset: 0x0005B650
		private void OnDestroy()
		{
			this.Shutdown();
		}

		// Token: 0x06001666 RID: 5734 RVA: 0x0005D458 File Offset: 0x0005B658
		private void Update()
		{
			this._clientHost.CheckSetStarted();
		}

		// Token: 0x06001667 RID: 5735 RVA: 0x0005D465 File Offset: 0x0005B665
		private void CreateChannelData()
		{
			this._mtus = new int[] { 1048576, 1200 };
		}

		// Token: 0x06001668 RID: 5736 RVA: 0x0005D484 File Offset: 0x0005B684
		private bool InitializeRelayNetworkAccess()
		{
			bool flag;
			try
			{
				SteamNetworkingUtils.InitRelayNetworkAccess();
				if (this.IsNetworkAccessAvailable())
				{
					this.LocalUserSteamID = SteamUser.GetSteamID().m_SteamID;
				}
				this._shutdownCalled = false;
				flag = true;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x06001669 RID: 5737 RVA: 0x0005D4D0 File Offset: 0x0005B6D0
		public bool IsNetworkAccessAvailable()
		{
			bool flag;
			try
			{
				InteropHelp.TestIfAvailableClient();
				flag = true;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x0600166A RID: 5738 RVA: 0x0005D4FC File Offset: 0x0005B6FC
		public override string GetConnectionAddress(int connectionId)
		{
			return this._server.GetConnectionAddress(connectionId);
		}

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x0600166B RID: 5739 RVA: 0x0005D50C File Offset: 0x0005B70C
		// (remove) Token: 0x0600166C RID: 5740 RVA: 0x0005D544 File Offset: 0x0005B744
		public override event Action<ClientConnectionStateArgs> OnClientConnectionState;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x0600166D RID: 5741 RVA: 0x0005D57C File Offset: 0x0005B77C
		// (remove) Token: 0x0600166E RID: 5742 RVA: 0x0005D5B4 File Offset: 0x0005B7B4
		public override event Action<ServerConnectionStateArgs> OnServerConnectionState;

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x0600166F RID: 5743 RVA: 0x0005D5EC File Offset: 0x0005B7EC
		// (remove) Token: 0x06001670 RID: 5744 RVA: 0x0005D624 File Offset: 0x0005B824
		public override event Action<RemoteConnectionStateArgs> OnRemoteConnectionState;

		// Token: 0x06001671 RID: 5745 RVA: 0x0005D659 File Offset: 0x0005B859
		public override LocalConnectionState GetConnectionState(bool server)
		{
			if (server)
			{
				return this._server.GetLocalConnectionState();
			}
			return this._client.GetLocalConnectionState();
		}

		// Token: 0x06001672 RID: 5746 RVA: 0x0005D675 File Offset: 0x0005B875
		public override RemoteConnectionState GetConnectionState(int connectionId)
		{
			return this._server.GetConnectionState(connectionId);
		}

		// Token: 0x06001673 RID: 5747 RVA: 0x0005D683 File Offset: 0x0005B883
		public override void HandleClientConnectionState(ClientConnectionStateArgs connectionStateArgs)
		{
			Action<ClientConnectionStateArgs> onClientConnectionState = this.OnClientConnectionState;
			if (onClientConnectionState == null)
			{
				return;
			}
			onClientConnectionState(connectionStateArgs);
		}

		// Token: 0x06001674 RID: 5748 RVA: 0x0005D696 File Offset: 0x0005B896
		public override void HandleServerConnectionState(ServerConnectionStateArgs connectionStateArgs)
		{
			Action<ServerConnectionStateArgs> onServerConnectionState = this.OnServerConnectionState;
			if (onServerConnectionState == null)
			{
				return;
			}
			onServerConnectionState(connectionStateArgs);
		}

		// Token: 0x06001675 RID: 5749 RVA: 0x0005D6A9 File Offset: 0x0005B8A9
		public override void HandleRemoteConnectionState(RemoteConnectionStateArgs connectionStateArgs)
		{
			Action<RemoteConnectionStateArgs> onRemoteConnectionState = this.OnRemoteConnectionState;
			if (onRemoteConnectionState == null)
			{
				return;
			}
			onRemoteConnectionState(connectionStateArgs);
		}

		// Token: 0x06001676 RID: 5750 RVA: 0x0005D6BC File Offset: 0x0005B8BC
		public override void IterateIncoming(bool server)
		{
			if (server)
			{
				this._server.IterateIncoming();
				return;
			}
			this._client.IterateIncoming();
			this._clientHost.IterateIncoming();
		}

		// Token: 0x06001677 RID: 5751 RVA: 0x0005D6E3 File Offset: 0x0005B8E3
		public override void IterateOutgoing(bool server)
		{
			if (server)
			{
				this._server.IterateOutgoing();
				return;
			}
			this._client.IterateOutgoing();
		}

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06001678 RID: 5752 RVA: 0x0005D700 File Offset: 0x0005B900
		// (remove) Token: 0x06001679 RID: 5753 RVA: 0x0005D738 File Offset: 0x0005B938
		public override event Action<ClientReceivedDataArgs> OnClientReceivedData;

		// Token: 0x0600167A RID: 5754 RVA: 0x0005D76D File Offset: 0x0005B96D
		public override void HandleClientReceivedDataArgs(ClientReceivedDataArgs receivedDataArgs)
		{
			Action<ClientReceivedDataArgs> onClientReceivedData = this.OnClientReceivedData;
			if (onClientReceivedData == null)
			{
				return;
			}
			onClientReceivedData(receivedDataArgs);
		}

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x0600167B RID: 5755 RVA: 0x0005D780 File Offset: 0x0005B980
		// (remove) Token: 0x0600167C RID: 5756 RVA: 0x0005D7B8 File Offset: 0x0005B9B8
		public override event Action<ServerReceivedDataArgs> OnServerReceivedData;

		// Token: 0x0600167D RID: 5757 RVA: 0x0005D7ED File Offset: 0x0005B9ED
		public override void HandleServerReceivedDataArgs(ServerReceivedDataArgs receivedDataArgs)
		{
			Action<ServerReceivedDataArgs> onServerReceivedData = this.OnServerReceivedData;
			if (onServerReceivedData == null)
			{
				return;
			}
			onServerReceivedData(receivedDataArgs);
		}

		// Token: 0x0600167E RID: 5758 RVA: 0x0005D800 File Offset: 0x0005BA00
		public override void SendToServer(byte channelId, ArraySegment<byte> segment)
		{
			this._client.SendToServer(channelId, segment);
			this._clientHost.SendToServer(channelId, segment);
		}

		// Token: 0x0600167F RID: 5759 RVA: 0x0005D81C File Offset: 0x0005BA1C
		public override void SendToClient(byte channelId, ArraySegment<byte> segment, int connectionId)
		{
			this._server.SendToClient(channelId, segment, connectionId);
		}

		// Token: 0x06001680 RID: 5760 RVA: 0x0005D82C File Offset: 0x0005BA2C
		public override int GetMaximumClients()
		{
			return this._server.GetMaximumClients();
		}

		// Token: 0x06001681 RID: 5761 RVA: 0x0005D839 File Offset: 0x0005BA39
		public override void SetMaximumClients(int value)
		{
			this._server.SetMaximumClients(value);
		}

		// Token: 0x06001682 RID: 5762 RVA: 0x0005D847 File Offset: 0x0005BA47
		public override void SetClientAddress(string address)
		{
			this._clientAddress = address;
		}

		// Token: 0x06001683 RID: 5763 RVA: 0x0005D850 File Offset: 0x0005BA50
		public override void SetServerBindAddress(string address, IPAddressType addressType)
		{
			this._serverBindAddress = address;
		}

		// Token: 0x06001684 RID: 5764 RVA: 0x0005D859 File Offset: 0x0005BA59
		public override void SetPort(ushort port)
		{
			this._port = port;
		}

		// Token: 0x06001685 RID: 5765 RVA: 0x0005D862 File Offset: 0x0005BA62
		public override bool StartConnection(bool server)
		{
			if (server)
			{
				return this.StartServer();
			}
			return this.StartClient(this._clientAddress);
		}

		// Token: 0x06001686 RID: 5766 RVA: 0x0005D87A File Offset: 0x0005BA7A
		public override bool StopConnection(bool server)
		{
			if (server)
			{
				return this.StopServer();
			}
			return this.StopClient();
		}

		// Token: 0x06001687 RID: 5767 RVA: 0x0005D88C File Offset: 0x0005BA8C
		public override bool StopConnection(int connectionId, bool immediately)
		{
			return this.StopClient(connectionId, immediately);
		}

		// Token: 0x06001688 RID: 5768 RVA: 0x0005D896 File Offset: 0x0005BA96
		public override void Shutdown()
		{
			if (this._shutdownCalled)
			{
				return;
			}
			this._shutdownCalled = true;
			this.StopConnection(false);
			this.StopConnection(true);
		}

		// Token: 0x06001689 RID: 5769 RVA: 0x0005D8B8 File Offset: 0x0005BAB8
		private bool StartServer()
		{
			if (!this.InitializeRelayNetworkAccess())
			{
				base.NetworkManager.LogError("RelayNetworkAccess could not be initialized.");
				return false;
			}
			if (!this.IsNetworkAccessAvailable())
			{
				base.NetworkManager.LogError("Server network access is not available.");
				return false;
			}
			this._server.ResetInvalidSocket();
			if (this._server.GetLocalConnectionState() != LocalConnectionState.Stopped)
			{
				base.NetworkManager.LogError("Server is already running.");
				return false;
			}
			bool flag = this._client.GetLocalConnectionState() > LocalConnectionState.Stopped;
			if (flag)
			{
				this._client.StopConnection();
			}
			bool flag2 = this._server.StartConnection(this._serverBindAddress, this._port, (int)this._maximumClients, this._peerToPeer);
			if (flag2 && flag)
			{
				this.StartConnection(false);
			}
			return flag2;
		}

		// Token: 0x0600168A RID: 5770 RVA: 0x0005D971 File Offset: 0x0005BB71
		private bool StopServer()
		{
			return this._server != null && this._server.StopConnection();
		}

		// Token: 0x0600168B RID: 5771 RVA: 0x0005D988 File Offset: 0x0005BB88
		private bool StartClient(string address)
		{
			if (this._server.GetLocalConnectionState() == LocalConnectionState.Stopped)
			{
				if (this._client.GetLocalConnectionState() != LocalConnectionState.Stopped)
				{
					base.NetworkManager.LogError("Client is already running.");
					return false;
				}
				if (this._clientHost.GetLocalConnectionState() != LocalConnectionState.Stopped)
				{
					this._clientHost.StopConnection();
				}
				if (!this.InitializeRelayNetworkAccess())
				{
					base.NetworkManager.LogError("RelayNetworkAccess could not be initialized.");
					return false;
				}
				if (!this.IsNetworkAccessAvailable())
				{
					base.NetworkManager.LogError("Client network access is not available.");
					return false;
				}
				this._client.StartConnection(address, this._port, this._peerToPeer);
			}
			else
			{
				this._clientHost.StartConnection(this._server);
			}
			return true;
		}

		// Token: 0x0600168C RID: 5772 RVA: 0x0005DA40 File Offset: 0x0005BC40
		private bool StopClient()
		{
			bool flag = false;
			if (this._client != null)
			{
				flag |= this._client.StopConnection();
			}
			if (this._clientHost != null)
			{
				flag |= this._clientHost.StopConnection();
			}
			return flag;
		}

		// Token: 0x0600168D RID: 5773 RVA: 0x0005DA7C File Offset: 0x0005BC7C
		private bool StopClient(int connectionId, bool immediately)
		{
			return this._server.StopConnection(connectionId);
		}

		// Token: 0x0600168E RID: 5774 RVA: 0x0005DA8A File Offset: 0x0005BC8A
		public override int GetMTU(byte channel)
		{
			if ((int)channel >= this._mtus.Length)
			{
				Debug.LogError(string.Format("Channel {0} is out of bounds.", channel));
				return 0;
			}
			return this._mtus[(int)channel];
		}

		// Token: 0x04000CEB RID: 3307
		[NonSerialized]
		public ulong LocalUserSteamID;

		// Token: 0x04000CEC RID: 3308
		[Tooltip("Address server should bind to.")]
		[SerializeField]
		private string _serverBindAddress = string.Empty;

		// Token: 0x04000CED RID: 3309
		[Tooltip("Port to use.")]
		[SerializeField]
		private ushort _port = 7770;

		// Token: 0x04000CEE RID: 3310
		[Tooltip("Maximum number of players which may be connected at once.")]
		[Range(1f, 65535f)]
		[SerializeField]
		private ushort _maximumClients = 9001;

		// Token: 0x04000CEF RID: 3311
		[Tooltip("True if using peer to peer socket.")]
		[SerializeField]
		private bool _peerToPeer;

		// Token: 0x04000CF0 RID: 3312
		[Tooltip("Address client should connect to.")]
		[SerializeField]
		private string _clientAddress = string.Empty;

		// Token: 0x04000CF1 RID: 3313
		private int[] _mtus;

		// Token: 0x04000CF2 RID: 3314
		private ClientSocket _client;

		// Token: 0x04000CF3 RID: 3315
		private ClientHostSocket _clientHost;

		// Token: 0x04000CF4 RID: 3316
		private ServerSocket _server;

		// Token: 0x04000CF5 RID: 3317
		private bool _shutdownCalled = true;

		// Token: 0x04000CF6 RID: 3318
		internal const int CLIENT_HOST_ID = 32767;
	}
}
