using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Utility.Performance;
using Steamworks;

namespace FishySteamworks
{
	// Token: 0x02000238 RID: 568
	public abstract class CommonSocket
	{
		// Token: 0x06001659 RID: 5721 RVA: 0x0005D0EF File Offset: 0x0005B2EF
		internal LocalConnectionState GetLocalConnectionState()
		{
			return this._connectionState;
		}

		// Token: 0x0600165A RID: 5722 RVA: 0x0005D0F8 File Offset: 0x0005B2F8
		protected virtual void SetLocalConnectionState(LocalConnectionState connectionState, bool server)
		{
			if (connectionState == this._connectionState)
			{
				return;
			}
			this._connectionState = connectionState;
			if (server)
			{
				this.Transport.HandleServerConnectionState(new ServerConnectionStateArgs(connectionState, this.Transport.Index));
				return;
			}
			this.Transport.HandleClientConnectionState(new ClientConnectionStateArgs(connectionState, this.Transport.Index));
		}

		// Token: 0x0600165B RID: 5723 RVA: 0x0005D154 File Offset: 0x0005B354
		internal virtual void Initialize(Transport t)
		{
			this.Transport = t;
			int num = this.Transport.GetMTU(0);
			num = Math.Max(num, this.Transport.GetMTU(1));
			this.InboundBuffer = new byte[num];
		}

		// Token: 0x0600165C RID: 5724 RVA: 0x0005D194 File Offset: 0x0005B394
		protected byte[] GetIPBytes(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return null;
			}
			IPAddress ipaddress;
			if (!IPAddress.TryParse(address, out ipaddress))
			{
				this.Transport.NetworkManager.LogError("Could not parse address " + address + " to IPAddress.");
				return null;
			}
			return ipaddress.GetAddressBytes();
		}

		// Token: 0x0600165D RID: 5725 RVA: 0x0005D1E0 File Offset: 0x0005B3E0
		protected EResult Send(HSteamNetConnection steamConnection, ArraySegment<byte> segment, byte channelId)
		{
			if (segment.Array.Length - 1 <= segment.Offset + segment.Count)
			{
				byte[] array = segment.Array;
				Array.Resize<byte>(ref array, array.Length + 1);
				array[array.Length - 1] = channelId;
			}
			else
			{
				segment.Array[segment.Offset + segment.Count] = channelId;
			}
			segment = new ArraySegment<byte>(segment.Array, segment.Offset, segment.Count + 1);
			GCHandle gchandle = GCHandle.Alloc(segment.Array, GCHandleType.Pinned);
			IntPtr intPtr = gchandle.AddrOfPinnedObject() + segment.Offset;
			int num = ((channelId == 1) ? 0 : 8);
			long num2;
			EResult eresult = SteamNetworkingSockets.SendMessageToConnection(steamConnection, intPtr, (uint)segment.Count, num, out num2);
			if (eresult != EResult.k_EResultOK)
			{
				this.Transport.NetworkManager.LogWarning(string.Format("Send issue: {0}", eresult));
			}
			gchandle.Free();
			return eresult;
		}

		// Token: 0x0600165E RID: 5726 RVA: 0x0005D2CC File Offset: 0x0005B4CC
		internal void ClearQueue(ConcurrentQueue<LocalPacket> queue)
		{
			LocalPacket localPacket;
			while (queue.TryDequeue(out localPacket))
			{
				ByteArrayPool.Store(localPacket.Data);
			}
		}

		// Token: 0x0600165F RID: 5727 RVA: 0x0005D2F0 File Offset: 0x0005B4F0
		internal void ClearQueue(Queue<LocalPacket> queue)
		{
			while (queue.Count > 0)
			{
				ByteArrayPool.Store(queue.Dequeue().Data);
			}
		}

		// Token: 0x06001660 RID: 5728 RVA: 0x0005D310 File Offset: 0x0005B510
		protected void GetMessage(IntPtr ptr, byte[] buffer, out ArraySegment<byte> segment, out byte channel)
		{
			SteamNetworkingMessage_t steamNetworkingMessage_t = Marshal.PtrToStructure<SteamNetworkingMessage_t>(ptr);
			int cbSize = steamNetworkingMessage_t.m_cbSize;
			Marshal.Copy(steamNetworkingMessage_t.m_pData, buffer, 0, cbSize);
			SteamNetworkingMessage_t.Release(ptr);
			channel = buffer[cbSize - 1];
			segment = new ArraySegment<byte>(buffer, 0, cbSize - 1);
		}

		// Token: 0x04000CE2 RID: 3298
		private LocalConnectionState _connectionState;

		// Token: 0x04000CE3 RID: 3299
		protected bool PeerToPeer;

		// Token: 0x04000CE4 RID: 3300
		protected Transport Transport;

		// Token: 0x04000CE5 RID: 3301
		protected IntPtr[] MessagePointers = new IntPtr[256];

		// Token: 0x04000CE6 RID: 3302
		protected byte[] InboundBuffer;

		// Token: 0x04000CE7 RID: 3303
		protected const int MAX_MESSAGES = 256;
	}
}
