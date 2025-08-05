using System;
using System.Collections;
using System.Collections.Generic;

namespace BlueRaja
{
	// Token: 0x02000228 RID: 552
	internal interface IFixedSizePriorityQueue<TItem, in TPriority> : IPriorityQueue<TItem, TPriority>, IEnumerable<TItem>, IEnumerable where TPriority : IComparable<TPriority>
	{
		// Token: 0x060015D1 RID: 5585
		void Resize(int maxNodes);

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x060015D2 RID: 5586
		int MaxSize { get; }

		// Token: 0x060015D3 RID: 5587
		void ResetNode(TItem node);
	}
}
