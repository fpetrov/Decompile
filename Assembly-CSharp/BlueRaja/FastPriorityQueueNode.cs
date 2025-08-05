using System;

namespace BlueRaja
{
	// Token: 0x02000224 RID: 548
	public class FastPriorityQueueNode
	{
		// Token: 0x1700023E RID: 574
		// (get) Token: 0x060015AA RID: 5546 RVA: 0x0005ACAB File Offset: 0x00058EAB
		// (set) Token: 0x060015AB RID: 5547 RVA: 0x0005ACB3 File Offset: 0x00058EB3
		public float Priority { get; protected internal set; }

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x060015AC RID: 5548 RVA: 0x0005ACBC File Offset: 0x00058EBC
		// (set) Token: 0x060015AD RID: 5549 RVA: 0x0005ACC4 File Offset: 0x00058EC4
		public int QueueIndex { get; internal set; }
	}
}
