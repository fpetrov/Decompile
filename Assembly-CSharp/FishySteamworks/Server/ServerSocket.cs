using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FishNet.Managing;
using FishNet.Transporting;
using FishySteamworks.Client;
using Steamworks;

namespace FishySteamworks.Server
{
	// Token: 0x0200023B RID: 571
	public class ServerSocket : CommonSocket
	{
		// Token: 0x06001690 RID: 5776 RVA: 0x0005DAF1 File Offset: 0x0005BCF1
		internal RemoteConnectionState GetConnectionState(int connectionId)
		{
			if (this._steamConnections.Second.ContainsKey(connectionId))
			{
				return RemoteConnectionState.Started;
			}
			return RemoteConnectionState.Stopped;
		}

		// Token: 0x06001691 RID: 5777 RVA: 0x0005DB09 File Offset: 0x0005BD09
		internal void ResetInvalidSocket()
		{
			if (this._socket == HSteamListenSocket.Invalid)
			{
				base.SetLocalConnectionState(LocalConnectionState.Stopped, true);
			}
		}

		// Token: 0x06001692 RID: 5778 RVA: 0x0005DB28 File Offset: 0x0005BD28
		internal bool StartConnection(string address, ushort port, int maximumClients, bool peerToPeer)
		{
			try
			{
				if (this._onRemoteConnectionStateCallback == null)
				{
					this._onRemoteConnectionStateCallback = Callback<SteamNetConnectionStatusChangedCallback_t>.Create(new Callback<SteamNetConnectionStatusChangedCallback_t>.DispatchDelegate(this.OnRemoteConnectionState));
				}
				this.PeerToPeer = peerToPeer;
				byte[] array = ((!peerToPeer) ? base.GetIPBytes(address) : null);
				this.PeerToPeer = peerToPeer;
				this.SetMaximumClients(maximumClients);
				this._nextConnectionId = 0;
				this._cachedConnectionIds.Clear();
				this._iteratingConnections = false;
				base.SetLocalConnectionState(LocalConnectionState.Starting, true);
				SteamNetworkingConfigValue_t[] array2 = new SteamNetworkingConfigValue_t[0];
				if (this.PeerToPeer)
				{
					this._socket = SteamNetworkingSockets.CreateListenSocketP2P(0, array2.Length, array2);
				}
				else
				{
					SteamNetworkingIPAddr steamNetworkingIPAddr = default(SteamNetworkingIPAddr);
					steamNetworkingIPAddr.Clear();
					if (array != null)
					{
						steamNetworkingIPAddr.SetIPv6(array, port);
					}
					this._socket = SteamNetworkingSockets.CreateListenSocketIP(ref steamNetworkingIPAddr, 0, array2);
				}
			}
			catch
			{
				base.SetLocalConnectionState(LocalConnectionState.Stopped, true);
				return false;
			}
			if (this._socket == HSteamListenSocket.Invalid)
			{
				base.SetLocalConnectionState(LocalConnectionState.Stopped, true);
				return false;
			}
			base.SetLocalConnectionState(LocalConnectionState.Started, true);
			return true;
		}

		// Token: 0x06001693 RID: 5779 RVA: 0x0005DC2C File Offset: 0x0005BE2C
		internal bool StopConnection()
		{
			if (this._socket != HSteamListenSocket.Invalid)
			{
				SteamNetworkingSockets.CloseListenSocket(this._socket);
				if (this._onRemoteConnectionStateCallback != null)
				{
					this._onRemoteConnectionStateCallback.Dispose();
					this._onRemoteConnectionStateCallback = null;
				}
				this._socket = HSteamListenSocket.Invalid;
			}
			this._pendingConnectionChanges.Clear();
			if (base.GetLocalConnectionState() == LocalConnectionState.Stopped)
			{
				return false;
			}
			base.SetLocalConnectionState(LocalConnectionState.Stopping, true);
			base.SetLocalConnectionState(LocalConnectionState.Stopped, true);
			return true;
		}

		// Token: 0x06001694 RID: 5780 RVA: 0x0005DCA4 File Offset: 0x0005BEA4
		internal bool StopConnection(int connectionId)
		{
			if (connectionId == 32767)
			{
				if (this._clientHost != null)
				{
					this._clientHost.StopConnection();
					return true;
				}
				return false;
			}
			else
			{
				HSteamNetConnection hsteamNetConnection;
				if (this._steamConnections.Second.TryGetValue(connectionId, out hsteamNetConnection))
				{
					return this.StopConnection(connectionId, hsteamNetConnection);
				}
				this.Transport.NetworkManager.LogError(string.Format("Steam connection not found for connectionId {0}.", connectionId));
				return false;
			}
		}

		// Token: 0x06001695 RID: 5781 RVA: 0x0005DD10 File Offset: 0x0005BF10
		private bool StopConnection(int connectionId, HSteamNetConnection socket)
		{
			SteamNetworkingSockets.CloseConnection(socket, 0, string.Empty, false);
			if (!this._iteratingConnections)
			{
				this.RemoveConnection(connectionId);
			}
			else
			{
				this._pendingConnectionChanges.Add(new ServerSocket.ConnectionChange(connectionId));
			}
			return true;
		}

		// Token: 0x06001696 RID: 5782 RVA: 0x0005DD44 File Offset: 0x0005BF44
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void OnRemoteConnectionState(SteamNetConnectionStatusChangedCallback_t args)
		{
			ulong steamID = args.m_info.m_identityRemote.GetSteamID64();
			if (args.m_info.m_eState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting)
			{
				if (this._steamConnections.Count >= this.GetMaximumClients())
				{
					this.Transport.NetworkManager.Log(string.Format("Incoming connection {0} was rejected because would exceed the maximum connection count.", steamID));
					SteamNetworkingSockets.CloseConnection(args.m_hConn, 0, "Max Connection Count", false);
					return;
				}
				EResult eresult = SteamNetworkingSockets.AcceptConnection(args.m_hConn);
				if (eresult == EResult.k_EResultOK)
				{
					this.Transport.NetworkManager.Log(string.Format("Accepting connection {0}", steamID));
					return;
				}
				this.Transport.NetworkManager.Log(string.Format("Connection {0} could not be accepted: {1}", steamID, eresult.ToString()));
				return;
			}
			else
			{
				if (args.m_info.m_eState != ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected)
				{
					if (args.m_info.m_eState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer || args.m_info.m_eState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally)
					{
						int num;
						if (this._steamConnections.TryGetValue(args.m_hConn, out num))
						{
							this.StopConnection(num, args.m_hConn);
							return;
						}
					}
					else
					{
						this.Transport.NetworkManager.Log(string.Format("Connection {0} state changed: {1}", steamID, args.m_info.m_eState.ToString()));
					}
					return;
				}
				int num2;
				if (this._cachedConnectionIds.Count <= 0)
				{
					int nextConnectionId = this._nextConnectionId;
					this._nextConnectionId = nextConnectionId + 1;
					num2 = nextConnectionId;
				}
				else
				{
					num2 = this._cachedConnectionIds.Dequeue();
				}
				int num3 = num2;
				if (!this._iteratingConnections)
				{
					this.AddConnection(num3, args.m_hConn, args.m_info.m_identityRemote.GetSteamID());
					return;
				}
				this._pendingConnectionChanges.Add(new ServerSocket.ConnectionChange(num3, args.m_hConn, args.m_info.m_identityRemote.GetSteamID()));
				return;
			}
		}

		// Token: 0x06001697 RID: 5783 RVA: 0x0005DF20 File Offset: 0x0005C120
		private void AddConnection(int connectionId, HSteamNetConnection steamConnection, CSteamID steamId)
		{
			this._steamConnections.Add(steamConnection, connectionId);
			this._steamIds.Add(steamId, connectionId);
			this.Transport.NetworkManager.Log(string.Format("Client with SteamID {0} connected. Assigning connection id {1}", steamId.m_SteamID, connectionId));
			this.Transport.HandleRemoteConnectionState(new RemoteConnectionStateArgs(RemoteConnectionState.Started, connectionId, this.Transport.Index));
		}

		// Token: 0x06001698 RID: 5784 RVA: 0x0005DF90 File Offset: 0x0005C190
		private void RemoveConnection(int connectionId)
		{
			this._steamConnections.Remove(connectionId);
			this._steamIds.Remove(connectionId);
			this.Transport.NetworkManager.Log(string.Format("Client with ConnectionID {0} disconnected.", connectionId));
			this.Transport.HandleRemoteConnectionState(new RemoteConnectionStateArgs(RemoteConnectionState.Stopped, connectionId, this.Transport.Index));
			this._cachedConnectionIds.Enqueue(connectionId);
		}

		// Token: 0x06001699 RID: 5785 RVA: 0x0005E000 File Offset: 0x0005C200
		internal void IterateOutgoing()
		{
			if (base.GetLocalConnectionState() != LocalConnectionState.Started)
			{
				return;
			}
			this._iteratingConnections = true;
			foreach (HSteamNetConnection hsteamNetConnection in this._steamConnections.FirstTypes)
			{
				SteamNetworkingSockets.FlushMessagesOnConnection(hsteamNetConnection);
			}
			this._iteratingConnections = false;
			this.ProcessPendingConnectionChanges();
		}

		// Token: 0x0600169A RID: 5786 RVA: 0x0005E070 File Offset: 0x0005C270
		internal void IterateIncoming()
		{
			if (base.GetLocalConnectionState() == LocalConnectionState.Stopped || base.GetLocalConnectionState() == LocalConnectionState.Stopping)
			{
				return;
			}
			this._iteratingConnections = true;
			while (this._clientHostIncoming.Count > 0)
			{
				LocalPacket localPacket = this._clientHostIncoming.Dequeue();
				ArraySegment<byte> arraySegment = new ArraySegment<byte>(localPacket.Data, 0, localPacket.Length);
				this.Transport.HandleServerReceivedDataArgs(new ServerReceivedDataArgs(arraySegment, (Channel)localPacket.Channel, 32767, this.Transport.Index));
			}
			foreach (KeyValuePair<HSteamNetConnection, int> keyValuePair in this._steamConnections.First)
			{
				HSteamNetConnection key = keyValuePair.Key;
				int value = keyValuePair.Value;
				int num = SteamNetworkingSockets.ReceiveMessagesOnConnection(key, this.MessagePointers, 256);
				if (num > 0)
				{
					for (int i = 0; i < num; i++)
					{
						ArraySegment<byte> arraySegment2;
						byte b;
						base.GetMessage(this.MessagePointers[i], this.InboundBuffer, out arraySegment2, out b);
						this.Transport.HandleServerReceivedDataArgs(new ServerReceivedDataArgs(arraySegment2, (Channel)b, value, this.Transport.Index));
					}
				}
			}
			this._iteratingConnections = false;
			this.ProcessPendingConnectionChanges();
		}

		// Token: 0x0600169B RID: 5787 RVA: 0x0005E1B4 File Offset: 0x0005C3B4
		private void ProcessPendingConnectionChanges()
		{
			foreach (ServerSocket.ConnectionChange connectionChange in this._pendingConnectionChanges)
			{
				if (connectionChange.IsConnect)
				{
					this.AddConnection(connectionChange.ConnectionId, connectionChange.SteamConnection, connectionChange.SteamId);
				}
				else
				{
					this.RemoveConnection(connectionChange.ConnectionId);
				}
			}
			this._pendingConnectionChanges.Clear();
		}

		// Token: 0x0600169C RID: 5788 RVA: 0x0005E23C File Offset: 0x0005C43C
		internal void SendToClient(byte channelId, ArraySegment<byte> segment, int connectionId)
		{
			if (base.GetLocalConnectionState() != LocalConnectionState.Started)
			{
				return;
			}
			if (connectionId == 32767)
			{
				if (this._clientHost != null)
				{
					LocalPacket localPacket = new LocalPacket(segment, channelId);
					this._clientHost.ReceivedFromLocalServer(localPacket);
				}
				return;
			}
			HSteamNetConnection hsteamNetConnection;
			if (this._steamConnections.TryGetValue(connectionId, out hsteamNetConnection))
			{
				EResult eresult = base.Send(hsteamNetConnection, segment, channelId);
				if (eresult == EResult.k_EResultNoConnection || eresult == EResult.k_EResultInvalidParam)
				{
					this.Transport.NetworkManager.Log(string.Format("Connection to {0} was lost.", connectionId));
					this.StopConnection(connectionId, hsteamNetConnection);
					return;
				}
				if (eresult != EResult.k_EResultOK)
				{
					this.Transport.NetworkManager.LogError("Could not send: " + eresult.ToString());
					return;
				}
			}
			else
			{
				this.Transport.NetworkManager.LogError(string.Format("ConnectionId {0} does not exist, data will not be sent.", connectionId));
			}
		}

		// Token: 0x0600169D RID: 5789 RVA: 0x0005E314 File Offset: 0x0005C514
		internal string GetConnectionAddress(int connectionId)
		{
			CSteamID csteamID;
			if (this._steamIds.TryGetValue(connectionId, out csteamID))
			{
				return csteamID.ToString();
			}
			this.Transport.NetworkManager.LogError(string.Format("ConnectionId {0} is invalid; address cannot be returned.", connectionId));
			return string.Empty;
		}

		// Token: 0x0600169E RID: 5790 RVA: 0x0005E364 File Offset: 0x0005C564
		internal void SetMaximumClients(int value)
		{
			this._maximumClients = Math.Min(value, 32766);
		}

		// Token: 0x0600169F RID: 5791 RVA: 0x0005E377 File Offset: 0x0005C577
		internal int GetMaximumClients()
		{
			return this._maximumClients;
		}

		// Token: 0x060016A0 RID: 5792 RVA: 0x0005E37F File Offset: 0x0005C57F
		internal void SetClientHostSocket(ClientHostSocket socket)
		{
			this._clientHost = socket;
		}

		// Token: 0x060016A1 RID: 5793 RVA: 0x0005E388 File Offset: 0x0005C588
		internal void OnClientHostState(bool started)
		{
			FishySteamworks fishySteamworks = (FishySteamworks)this.Transport;
			CSteamID csteamID = new CSteamID(fishySteamworks.LocalUserSteamID);
			if (!started && this._clientHostStarted)
			{
				base.ClearQueue(this._clientHostIncoming);
				this.Transport.HandleRemoteConnectionState(new RemoteConnectionStateArgs(RemoteConnectionState.Stopped, 32767, this.Transport.Index));
				this._steamIds.Remove(csteamID);
			}
			else if (started)
			{
				this._steamIds[csteamID] = 32767;
				this.Transport.HandleRemoteConnectionState(new RemoteConnectionStateArgs(RemoteConnectionState.Started, 32767, this.Transport.Index));
			}
			this._clientHostStarted = started;
		}

		// Token: 0x060016A2 RID: 5794 RVA: 0x0005E430 File Offset: 0x0005C630
		internal void ReceivedFromClientHost(LocalPacket packet)
		{
			if (!this._clientHostStarted)
			{
				return;
			}
			this._clientHostIncoming.Enqueue(packet);
		}

		// Token: 0x04000CFC RID: 3324
		private BidirectionalDictionary<HSteamNetConnection, int> _steamConnections = new BidirectionalDictionary<HSteamNetConnection, int>();

		// Token: 0x04000CFD RID: 3325
		private BidirectionalDictionary<CSteamID, int> _steamIds = new BidirectionalDictionary<CSteamID, int>();

		// Token: 0x04000CFE RID: 3326
		private int _maximumClients;

		// Token: 0x04000CFF RID: 3327
		private int _nextConnectionId;

		// Token: 0x04000D00 RID: 3328
		private HSteamListenSocket _socket = new HSteamListenSocket(0U);

		// Token: 0x04000D01 RID: 3329
		private Queue<LocalPacket> _clientHostIncoming = new Queue<LocalPacket>();

		// Token: 0x04000D02 RID: 3330
		private bool _clientHostStarted;

		// Token: 0x04000D03 RID: 3331
		private Callback<SteamNetConnectionStatusChangedCallback_t> _onRemoteConnectionStateCallback;

		// Token: 0x04000D04 RID: 3332
		private Queue<int> _cachedConnectionIds = new Queue<int>();

		// Token: 0x04000D05 RID: 3333
		private ClientHostSocket _clientHost;

		// Token: 0x04000D06 RID: 3334
		private bool _iteratingConnections;

		// Token: 0x04000D07 RID: 3335
		private List<ServerSocket.ConnectionChange> _pendingConnectionChanges = new List<ServerSocket.ConnectionChange>();

		// Token: 0x0200023C RID: 572
		public struct ConnectionChange
		{
			// Token: 0x1700025E RID: 606
			// (get) Token: 0x060016A4 RID: 5796 RVA: 0x0005E49E File Offset: 0x0005C69E
			public bool IsConnect
			{
				get
				{
					return this.SteamId.IsValid();
				}
			}

			// Token: 0x060016A5 RID: 5797 RVA: 0x0005E4AB File Offset: 0x0005C6AB
			public ConnectionChange(int id)
			{
				this.ConnectionId = id;
				this.SteamId = CSteamID.Nil;
				this.SteamConnection = default(HSteamNetConnection);
			}

			// Token: 0x060016A6 RID: 5798 RVA: 0x0005E4CB File Offset: 0x0005C6CB
			public ConnectionChange(int id, HSteamNetConnection steamConnection, CSteamID steamId)
			{
				this.ConnectionId = id;
				this.SteamConnection = steamConnection;
				this.SteamId = steamId;
			}

			// Token: 0x04000D08 RID: 3336
			public int ConnectionId;

			// Token: 0x04000D09 RID: 3337
			public HSteamNetConnection SteamConnection;

			// Token: 0x04000D0A RID: 3338
			public CSteamID SteamId;
		}
	}
}
