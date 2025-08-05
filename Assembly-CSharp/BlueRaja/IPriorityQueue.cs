using System;
using System.Collections;
using System.Collections.Generic;

namespace BlueRaja
{
	// Token: 0x02000229 RID: 553
	public interface IPriorityQueue<TItem, in TPriority> : IEnumerable<TItem>, IEnumerable where TPriority : IComparable<TPriority>
	{
		// Token: 0x060015D4 RID: 5588
		void Enqueue(TItem node, TPriority priority);

		// Token: 0x060015D5 RID: 5589
		TItem Dequeue();

		// Token: 0x060015D6 RID: 5590
		void Clear();

		// Token: 0x060015D7 RID: 5591
		bool Contains(TItem node);

		// Token: 0x060015D8 RID: 5592
		void Remove(TItem node);

		// Token: 0x060015D9 RID: 5593
		void UpdatePriority(TItem node, TPriority priority);

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x060015DA RID: 5594
		TItem First { get; }

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x060015DB RID: 5595
		int Count { get; }
	}
}
