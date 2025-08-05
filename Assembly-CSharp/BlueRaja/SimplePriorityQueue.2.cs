using System;
using System.Collections.Generic;

namespace BlueRaja
{
	// Token: 0x0200022C RID: 556
	public class SimplePriorityQueue<TItem> : SimplePriorityQueue<TItem, float>
	{
		// Token: 0x060015FB RID: 5627 RVA: 0x0005C058 File Offset: 0x0005A258
		public SimplePriorityQueue()
		{
		}

		// Token: 0x060015FC RID: 5628 RVA: 0x0005C060 File Offset: 0x0005A260
		public SimplePriorityQueue(IComparer<float> comparer)
			: base(comparer)
		{
		}

		// Token: 0x060015FD RID: 5629 RVA: 0x0005C069 File Offset: 0x0005A269
		public SimplePriorityQueue(Comparison<float> comparer)
			: base(comparer)
		{
		}
	}
}
