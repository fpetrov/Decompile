using System;
using System.Collections.Generic;
using Graphs;
using UnityEngine;

// Token: 0x020001F4 RID: 500
public static class Prim
{
	// Token: 0x0600142D RID: 5165 RVA: 0x00054090 File Offset: 0x00052290
	public static List<Prim.Edge> MinimumSpanningTree(List<Prim.Edge> edges, Vertex start)
	{
		HashSet<Vertex> hashSet = new HashSet<Vertex>();
		HashSet<Vertex> hashSet2 = new HashSet<Vertex>();
		foreach (Prim.Edge edge in edges)
		{
			hashSet.Add(edge.U);
			hashSet.Add(edge.V);
		}
		hashSet2.Add(start);
		List<Prim.Edge> list = new List<Prim.Edge>();
		while (hashSet.Count > 0)
		{
			bool flag = false;
			Prim.Edge edge2 = null;
			float num = float.PositiveInfinity;
			foreach (Prim.Edge edge3 in edges)
			{
				int num2 = 0;
				if (!hashSet2.Contains(edge3.U))
				{
					num2++;
				}
				if (!hashSet2.Contains(edge3.V))
				{
					num2++;
				}
				if (num2 == 1 && edge3.Distance < num)
				{
					edge2 = edge3;
					flag = true;
					num = edge3.Distance;
				}
			}
			if (!flag)
			{
				break;
			}
			list.Add(edge2);
			hashSet.Remove(edge2.U);
			hashSet.Remove(edge2.V);
			hashSet2.Add(edge2.U);
			hashSet2.Add(edge2.V);
		}
		return list;
	}

	// Token: 0x020001F5 RID: 501
	public class Edge : global::Graphs.Edge
	{
		// Token: 0x1700020B RID: 523
		// (get) Token: 0x0600142E RID: 5166 RVA: 0x000541F8 File Offset: 0x000523F8
		// (set) Token: 0x0600142F RID: 5167 RVA: 0x00054200 File Offset: 0x00052400
		public float Distance { get; private set; }

		// Token: 0x06001430 RID: 5168 RVA: 0x00054209 File Offset: 0x00052409
		public Edge(Vertex u, Vertex v)
			: base(u, v)
		{
			this.Distance = Vector3.Distance(u.Position, v.Position);
		}

		// Token: 0x06001431 RID: 5169 RVA: 0x0005422A File Offset: 0x0005242A
		public static bool operator ==(Prim.Edge left, Prim.Edge right)
		{
			return (left.U == right.U && left.V == right.V) || (left.U == right.V && left.V == right.U);
		}

		// Token: 0x06001432 RID: 5170 RVA: 0x00054268 File Offset: 0x00052468
		public static bool operator !=(Prim.Edge left, Prim.Edge right)
		{
			return !(left == right);
		}

		// Token: 0x06001433 RID: 5171 RVA: 0x00054274 File Offset: 0x00052474
		public override bool Equals(object obj)
		{
			Prim.Edge edge = obj as Prim.Edge;
			return edge != null && this == edge;
		}

		// Token: 0x06001434 RID: 5172 RVA: 0x00054294 File Offset: 0x00052494
		public bool Equals(Prim.Edge e)
		{
			return this == e;
		}

		// Token: 0x06001435 RID: 5173 RVA: 0x0005429D File Offset: 0x0005249D
		public override int GetHashCode()
		{
			return base.U.GetHashCode() ^ base.V.GetHashCode();
		}
	}
}
