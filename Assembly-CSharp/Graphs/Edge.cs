using System;

namespace Graphs
{
	// Token: 0x02000221 RID: 545
	public class Edge : IEquatable<Edge>
	{
		// Token: 0x17000237 RID: 567
		// (get) Token: 0x06001585 RID: 5509 RVA: 0x0005A42D File Offset: 0x0005862D
		// (set) Token: 0x06001586 RID: 5510 RVA: 0x0005A435 File Offset: 0x00058635
		public Vertex U { get; set; }

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x06001587 RID: 5511 RVA: 0x0005A43E File Offset: 0x0005863E
		// (set) Token: 0x06001588 RID: 5512 RVA: 0x0005A446 File Offset: 0x00058646
		public Vertex V { get; set; }

		// Token: 0x06001589 RID: 5513 RVA: 0x0000EB70 File Offset: 0x0000CD70
		public Edge()
		{
		}

		// Token: 0x0600158A RID: 5514 RVA: 0x0005A44F File Offset: 0x0005864F
		public Edge(Vertex u, Vertex v)
		{
			this.U = u;
			this.V = v;
		}

		// Token: 0x0600158B RID: 5515 RVA: 0x0005A465 File Offset: 0x00058665
		public static bool operator ==(Edge left, Edge right)
		{
			return (left.U == right.U || left.U == right.V) && (left.V == right.U || left.V == right.V);
		}

		// Token: 0x0600158C RID: 5516 RVA: 0x0005A4A3 File Offset: 0x000586A3
		public static bool operator !=(Edge left, Edge right)
		{
			return !(left == right);
		}

		// Token: 0x0600158D RID: 5517 RVA: 0x0005A4B0 File Offset: 0x000586B0
		public override bool Equals(object obj)
		{
			Edge edge = obj as Edge;
			return edge != null && this == edge;
		}

		// Token: 0x0600158E RID: 5518 RVA: 0x0005A4D0 File Offset: 0x000586D0
		public bool Equals(Edge e)
		{
			return this == e;
		}

		// Token: 0x0600158F RID: 5519 RVA: 0x0005429D File Offset: 0x0005249D
		public override int GetHashCode()
		{
			return this.U.GetHashCode() ^ this.V.GetHashCode();
		}
	}
}
