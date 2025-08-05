using System;
using System.Collections.Generic;
using FishNet.Transporting;
using FishNet.Utility.Performance;
using FishySteamworks.Server;

namespace FishySteamworks.Client
{
	// Token: 0x0200023D RID: 573
	public class ClientHostSocket : CommonSocket
	{
		// Token: 0x060016A7 RID: 5799 RVA: 0x0005E4E2 File Offset: 0x0005C6E2
		internal void CheckSetStarted()
		{
			if (this._server != null && base.GetLocalConnectionState() == LocalConnectionState.Starting && this._server.GetLocalConnectionState() == LocalConnectionState.Started)
			{
				this.SetLocalConnectionState(LocalConnectionState.Started, false);
			}
		}

		// Token: 0x060016A8 RID: 5800 RVA: 0x0005E50B File Offset: 0x0005C70B
		internal bool StartConnection(ServerSocket serverSocket)
		{
			this._server = serverSocket;
			this._server.SetClientHostSocket(this);
			if (this._server.GetLocalConnectionState() != LocalConnectionState.Started)
			{
				return false;
			}
			this.SetLocalConnectionState(LocalConnectionState.Starting, false);
			return true;
		}

		// Token: 0x060016A9 RID: 5801 RVA: 0x0005E539 File Offset: 0x0005C739
		protected override void SetLocalConnectionState(LocalConnectionState connectionState, bool server)
		{
			base.SetLocalConnectionState(connectionState, server);
			if (connectionState == LocalConnectionState.Started)
			{
				this._server.OnClientHostState(true);
				return;
			}
			this._server.OnClientHostState(false);
		}

		// Token: 0x060016AA RID: 5802 RVA: 0x0005E560 File Offset: 0x0005C760
		internal bool StopConnection()
		{
			if (base.GetLocalConnectionState() == LocalConnectionState.Stopped || base.GetLocalConnectionState() == LocalConnectionState.Stopping)
			{
				return false;
			}
			base.ClearQueue(this._incoming);
			this.SetLocalConnectionState(LocalConnectionState.Stopping, false);
			this.SetLocalConnectionState(LocalConnectionState.Stopped, false);
			this._server.SetClientHostSocket(null);
			return true;
		}

		// Token: 0x060016AB RID: 5803 RVA: 0x0005E5A0 File Offset: 0x0005C7A0
		internal void IterateIncoming()
		{
			if (base.GetLocalConnectionState() != LocalConnectionState.Started)
			{
				return;
			}
			while (this._incoming.Count > 0)
			{
				LocalPacket localPacket = this._incoming.Dequeue();
				ArraySegment<byte> arraySegment = new ArraySegment<byte>(localPacket.Data, 0, localPacket.Length);
				this.Transport.HandleClientReceivedDataArgs(new ClientReceivedDataArgs(arraySegment, (Channel)localPacket.Channel, this.Transport.Index));
				ByteArrayPool.Store(localPacket.Data);
			}
		}

		// Token: 0x060016AC RID: 5804 RVA: 0x0005E612 File Offset: 0x0005C812
		internal void ReceivedFromLocalServer(LocalPacket packet)
		{
			this._incoming.Enqueue(packet);
		}

		// Token: 0x060016AD RID: 5805 RVA: 0x0005E620 File Offset: 0x0005C820
		internal void SendToServer(byte channelId, ArraySegment<byte> segment)
		{
			if (base.GetLocalConnectionState() != LocalConnectionState.Started)
			{
				return;
			}
			if (this._server.GetLocalConnectionState() != LocalConnectionState.Started)
			{
				return;
			}
			LocalPacket localPacket = new LocalPacket(segment, channelId);
			this._server.ReceivedFromClientHost(localPacket);
		}

		// Token: 0x04000D0B RID: 3339
		private ServerSocket _server;

		// Token: 0x04000D0C RID: 3340
		private Queue<LocalPacket> _incoming = new Queue<LocalPacket>();
	}
}
