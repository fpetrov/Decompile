using System;
using FishNet.Utility.Performance;

namespace FishySteamworks
{
	// Token: 0x02000239 RID: 569
	internal struct LocalPacket
	{
		// Token: 0x06001662 RID: 5730 RVA: 0x0005D370 File Offset: 0x0005B570
		public LocalPacket(ArraySegment<byte> data, byte channel)
		{
			this.Data = ByteArrayPool.Retrieve(data.Count);
			this.Length = data.Count;
			Buffer.BlockCopy(data.Array, data.Offset, this.Data, 0, this.Length);
			this.Channel = channel;
		}

		// Token: 0x04000CE8 RID: 3304
		public byte[] Data;

		// Token: 0x04000CE9 RID: 3305
		public int Length;

		// Token: 0x04000CEA RID: 3306
		public byte Channel;
	}
}
