using System;
using UnityEngine;

namespace Graphs
{
	// Token: 0x0200021F RID: 543
	public class Vertex : IEquatable<Vertex>
	{
		// Token: 0x17000235 RID: 565
		// (get) Token: 0x0600157A RID: 5498 RVA: 0x0005A37A File Offset: 0x0005857A
		// (set) Token: 0x0600157B RID: 5499 RVA: 0x0005A382 File Offset: 0x00058582
		public Vector3 Position { get; private set; }

		// Token: 0x0600157C RID: 5500 RVA: 0x0000EB70 File Offset: 0x0000CD70
		public Vertex()
		{
		}

		// Token: 0x0600157D RID: 5501 RVA: 0x0005A38B File Offset: 0x0005858B
		public Vertex(Vector3 position)
		{
			this.Position = position;
		}

		// Token: 0x0600157E RID: 5502 RVA: 0x0005A39C File Offset: 0x0005859C
		public override bool Equals(object obj)
		{
			Vertex vertex = obj as Vertex;
			return vertex != null && this.Position == vertex.Position;
		}

		// Token: 0x0600157F RID: 5503 RVA: 0x0005A3C6 File Offset: 0x000585C6
		public bool Equals(Vertex other)
		{
			return this.Position == other.Position;
		}

		// Token: 0x06001580 RID: 5504 RVA: 0x0005A3DC File Offset: 0x000585DC
		public override int GetHashCode()
		{
			return this.Position.GetHashCode();
		}
	}
}
