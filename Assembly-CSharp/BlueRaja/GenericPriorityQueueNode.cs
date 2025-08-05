using System;

namespace BlueRaja
{
	// Token: 0x02000227 RID: 551
	public class GenericPriorityQueueNode<TPriority>
	{
		// Token: 0x17000245 RID: 581
		// (get) Token: 0x060015CA RID: 5578 RVA: 0x0005B50B File Offset: 0x0005970B
		// (set) Token: 0x060015CB RID: 5579 RVA: 0x0005B513 File Offset: 0x00059713
		public TPriority Priority { get; protected internal set; }

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x060015CC RID: 5580 RVA: 0x0005B51C File Offset: 0x0005971C
		// (set) Token: 0x060015CD RID: 5581 RVA: 0x0005B524 File Offset: 0x00059724
		public int QueueIndex { get; internal set; }

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x060015CE RID: 5582 RVA: 0x0005B52D File Offset: 0x0005972D
		// (set) Token: 0x060015CF RID: 5583 RVA: 0x0005B535 File Offset: 0x00059735
		public long InsertionIndex { get; internal set; }
	}
}
