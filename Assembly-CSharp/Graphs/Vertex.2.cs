using System;
using UnityEngine;

namespace Graphs
{
	// Token: 0x02000220 RID: 544
	public class Vertex<T> : Vertex
	{
		// Token: 0x17000236 RID: 566
		// (get) Token: 0x06001581 RID: 5505 RVA: 0x0005A3FD File Offset: 0x000585FD
		// (set) Token: 0x06001582 RID: 5506 RVA: 0x0005A405 File Offset: 0x00058605
		public T Item { get; private set; }

		// Token: 0x06001583 RID: 5507 RVA: 0x0005A40E File Offset: 0x0005860E
		public Vertex(T item)
		{
			this.Item = item;
		}

		// Token: 0x06001584 RID: 5508 RVA: 0x0005A41D File Offset: 0x0005861D
		public Vertex(Vector3 position, T item)
			: base(position)
		{
			this.Item = item;
		}
	}
}
