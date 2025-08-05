using System;
using System.Collections.Generic;
using Graphs;
using UnityEngine;

// Token: 0x020001F8 RID: 504
public class Delaunay2D
{
	// Token: 0x06001441 RID: 5185 RVA: 0x00054EC1 File Offset: 0x000530C1
	private static bool AlmostEqual(float x, float y)
	{
		return Mathf.Abs(x - y) <= float.Epsilon * Mathf.Abs(x + y) * 2f || Mathf.Abs(x - y) < float.MinValue;
	}

	// Token: 0x06001442 RID: 5186 RVA: 0x00054EF2 File Offset: 0x000530F2
	private static bool AlmostEqual(Vertex left, Vertex right)
	{
		return Delaunay2D.AlmostEqual(left.Position.x, right.Position.x) && Delaunay2D.AlmostEqual(left.Position.y, right.Position.y);
	}

	// Token: 0x1700020C RID: 524
	// (get) Token: 0x06001443 RID: 5187 RVA: 0x00054F2E File Offset: 0x0005312E
	// (set) Token: 0x06001444 RID: 5188 RVA: 0x00054F36 File Offset: 0x00053136
	public List<Vertex> Vertices { get; private set; }

	// Token: 0x1700020D RID: 525
	// (get) Token: 0x06001445 RID: 5189 RVA: 0x00054F3F File Offset: 0x0005313F
	// (set) Token: 0x06001446 RID: 5190 RVA: 0x00054F47 File Offset: 0x00053147
	public List<Delaunay2D.Edge> Edges { get; private set; }

	// Token: 0x1700020E RID: 526
	// (get) Token: 0x06001447 RID: 5191 RVA: 0x00054F50 File Offset: 0x00053150
	// (set) Token: 0x06001448 RID: 5192 RVA: 0x00054F58 File Offset: 0x00053158
	public List<Delaunay2D.Triangle> Triangles { get; private set; }

	// Token: 0x06001449 RID: 5193 RVA: 0x00054F61 File Offset: 0x00053161
	private Delaunay2D()
	{
		this.Edges = new List<Delaunay2D.Edge>();
		this.Triangles = new List<Delaunay2D.Triangle>();
	}

	// Token: 0x0600144A RID: 5194 RVA: 0x00054F7F File Offset: 0x0005317F
	public static Delaunay2D Triangulate(List<Vertex> vertices)
	{
		Delaunay2D delaunay2D = new Delaunay2D();
		delaunay2D.Vertices = new List<Vertex>(vertices);
		delaunay2D.Triangulate();
		return delaunay2D;
	}

	// Token: 0x0600144B RID: 5195 RVA: 0x00054F98 File Offset: 0x00053198
	private void Triangulate()
	{
		float num = this.Vertices[0].Position.x;
		float num2 = this.Vertices[0].Position.y;
		float num3 = num;
		float num4 = num2;
		foreach (Vertex vertex in this.Vertices)
		{
			if (vertex.Position.x < num)
			{
				num = vertex.Position.x;
			}
			if (vertex.Position.x > num3)
			{
				num3 = vertex.Position.x;
			}
			if (vertex.Position.y < num2)
			{
				num2 = vertex.Position.y;
			}
			if (vertex.Position.y > num4)
			{
				num4 = vertex.Position.y;
			}
		}
		float num5 = num3 - num;
		float num6 = num4 - num2;
		float num7 = Mathf.Max(num5, num6) * 2f;
		Vertex p1 = new Vertex(new Vector2(num - 1f, num2 - 1f));
		Vertex p2 = new Vertex(new Vector2(num - 1f, num4 + num7));
		Vertex p3 = new Vertex(new Vector2(num3 + num7, num2 - 1f));
		this.Triangles.Add(new Delaunay2D.Triangle(p1, p2, p3));
		foreach (Vertex vertex2 in this.Vertices)
		{
			List<Delaunay2D.Edge> list = new List<Delaunay2D.Edge>();
			foreach (Delaunay2D.Triangle triangle in this.Triangles)
			{
				if (triangle.CircumCircleContains(vertex2.Position))
				{
					triangle.IsBad = true;
					list.Add(new Delaunay2D.Edge(triangle.A, triangle.B));
					list.Add(new Delaunay2D.Edge(triangle.B, triangle.C));
					list.Add(new Delaunay2D.Edge(triangle.C, triangle.A));
				}
			}
			this.Triangles.RemoveAll((Delaunay2D.Triangle t) => t.IsBad);
			for (int i = 0; i < list.Count; i++)
			{
				for (int j = i + 1; j < list.Count; j++)
				{
					if (Delaunay2D.Edge.AlmostEqual(list[i], list[j]))
					{
						list[i].IsBad = true;
						list[j].IsBad = true;
					}
				}
			}
			list.RemoveAll((Delaunay2D.Edge e) => e.IsBad);
			foreach (Delaunay2D.Edge edge in list)
			{
				this.Triangles.Add(new Delaunay2D.Triangle(edge.U, edge.V, vertex2));
			}
		}
		this.Triangles.RemoveAll((Delaunay2D.Triangle t) => t.ContainsVertex(p1.Position) || t.ContainsVertex(p2.Position) || t.ContainsVertex(p3.Position));
		HashSet<Delaunay2D.Edge> hashSet = new HashSet<Delaunay2D.Edge>();
		foreach (Delaunay2D.Triangle triangle2 in this.Triangles)
		{
			Delaunay2D.Edge edge2 = new Delaunay2D.Edge(triangle2.A, triangle2.B);
			Delaunay2D.Edge edge3 = new Delaunay2D.Edge(triangle2.B, triangle2.C);
			Delaunay2D.Edge edge4 = new Delaunay2D.Edge(triangle2.C, triangle2.A);
			if (hashSet.Add(edge2))
			{
				this.Edges.Add(edge2);
			}
			if (hashSet.Add(edge3))
			{
				this.Edges.Add(edge3);
			}
			if (hashSet.Add(edge4))
			{
				this.Edges.Add(edge4);
			}
		}
	}

	// Token: 0x020001F9 RID: 505
	public class Triangle : IEquatable<Delaunay2D.Triangle>
	{
		// Token: 0x1700020F RID: 527
		// (get) Token: 0x0600144C RID: 5196 RVA: 0x00055458 File Offset: 0x00053658
		// (set) Token: 0x0600144D RID: 5197 RVA: 0x00055460 File Offset: 0x00053660
		public Vertex A { get; set; }

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x0600144E RID: 5198 RVA: 0x00055469 File Offset: 0x00053669
		// (set) Token: 0x0600144F RID: 5199 RVA: 0x00055471 File Offset: 0x00053671
		public Vertex B { get; set; }

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x06001450 RID: 5200 RVA: 0x0005547A File Offset: 0x0005367A
		// (set) Token: 0x06001451 RID: 5201 RVA: 0x00055482 File Offset: 0x00053682
		public Vertex C { get; set; }

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x06001452 RID: 5202 RVA: 0x0005548B File Offset: 0x0005368B
		// (set) Token: 0x06001453 RID: 5203 RVA: 0x00055493 File Offset: 0x00053693
		public bool IsBad { get; set; }

		// Token: 0x06001454 RID: 5204 RVA: 0x0000EB70 File Offset: 0x0000CD70
		public Triangle()
		{
		}

		// Token: 0x06001455 RID: 5205 RVA: 0x0005549C File Offset: 0x0005369C
		public Triangle(Vertex a, Vertex b, Vertex c)
		{
			this.A = a;
			this.B = b;
			this.C = c;
		}

		// Token: 0x06001456 RID: 5206 RVA: 0x000554BC File Offset: 0x000536BC
		public bool ContainsVertex(Vector3 v)
		{
			return Vector3.Distance(v, this.A.Position) < 0.01f || Vector3.Distance(v, this.B.Position) < 0.01f || Vector3.Distance(v, this.C.Position) < 0.01f;
		}

		// Token: 0x06001457 RID: 5207 RVA: 0x00055514 File Offset: 0x00053714
		public bool CircumCircleContains(Vector3 v)
		{
			Vector3 position = this.A.Position;
			Vector3 position2 = this.B.Position;
			Vector3 position3 = this.C.Position;
			float sqrMagnitude = position.sqrMagnitude;
			float sqrMagnitude2 = position2.sqrMagnitude;
			float sqrMagnitude3 = position3.sqrMagnitude;
			float num = (sqrMagnitude * (position3.y - position2.y) + sqrMagnitude2 * (position.y - position3.y) + sqrMagnitude3 * (position2.y - position.y)) / (position.x * (position3.y - position2.y) + position2.x * (position.y - position3.y) + position3.x * (position2.y - position.y));
			float num2 = (sqrMagnitude * (position3.x - position2.x) + sqrMagnitude2 * (position.x - position3.x) + sqrMagnitude3 * (position2.x - position.x)) / (position.y * (position3.x - position2.x) + position2.y * (position.x - position3.x) + position3.y * (position2.x - position.x));
			Vector3 vector = new Vector3(num / 2f, num2 / 2f);
			float num3 = Vector3.SqrMagnitude(position - vector);
			return Vector3.SqrMagnitude(v - vector) <= num3;
		}

		// Token: 0x06001458 RID: 5208 RVA: 0x00055678 File Offset: 0x00053878
		public static bool operator ==(Delaunay2D.Triangle left, Delaunay2D.Triangle right)
		{
			return (left.A == right.A || left.A == right.B || left.A == right.C) && (left.B == right.A || left.B == right.B || left.B == right.C) && (left.C == right.A || left.C == right.B || left.C == right.C);
		}

		// Token: 0x06001459 RID: 5209 RVA: 0x00055707 File Offset: 0x00053907
		public static bool operator !=(Delaunay2D.Triangle left, Delaunay2D.Triangle right)
		{
			return !(left == right);
		}

		// Token: 0x0600145A RID: 5210 RVA: 0x00055714 File Offset: 0x00053914
		public override bool Equals(object obj)
		{
			Delaunay2D.Triangle triangle = obj as Delaunay2D.Triangle;
			return triangle != null && this == triangle;
		}

		// Token: 0x0600145B RID: 5211 RVA: 0x00055734 File Offset: 0x00053934
		public bool Equals(Delaunay2D.Triangle t)
		{
			return this == t;
		}

		// Token: 0x0600145C RID: 5212 RVA: 0x0005573D File Offset: 0x0005393D
		public override int GetHashCode()
		{
			return this.A.GetHashCode() ^ this.B.GetHashCode() ^ this.C.GetHashCode();
		}
	}

	// Token: 0x020001FA RID: 506
	public class Edge
	{
		// Token: 0x17000213 RID: 531
		// (get) Token: 0x0600145D RID: 5213 RVA: 0x00055762 File Offset: 0x00053962
		// (set) Token: 0x0600145E RID: 5214 RVA: 0x0005576A File Offset: 0x0005396A
		public Vertex U { get; set; }

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x0600145F RID: 5215 RVA: 0x00055773 File Offset: 0x00053973
		// (set) Token: 0x06001460 RID: 5216 RVA: 0x0005577B File Offset: 0x0005397B
		public Vertex V { get; set; }

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x06001461 RID: 5217 RVA: 0x00055784 File Offset: 0x00053984
		// (set) Token: 0x06001462 RID: 5218 RVA: 0x0005578C File Offset: 0x0005398C
		public bool IsBad { get; set; }

		// Token: 0x06001463 RID: 5219 RVA: 0x0000EB70 File Offset: 0x0000CD70
		public Edge()
		{
		}

		// Token: 0x06001464 RID: 5220 RVA: 0x00055795 File Offset: 0x00053995
		public Edge(Vertex u, Vertex v)
		{
			this.U = u;
			this.V = v;
		}

		// Token: 0x06001465 RID: 5221 RVA: 0x000557AB File Offset: 0x000539AB
		public static bool operator ==(Delaunay2D.Edge left, Delaunay2D.Edge right)
		{
			return (left.U == right.U || left.U == right.V) && (left.V == right.U || left.V == right.V);
		}

		// Token: 0x06001466 RID: 5222 RVA: 0x000557E9 File Offset: 0x000539E9
		public static bool operator !=(Delaunay2D.Edge left, Delaunay2D.Edge right)
		{
			return !(left == right);
		}

		// Token: 0x06001467 RID: 5223 RVA: 0x000557F8 File Offset: 0x000539F8
		public override bool Equals(object obj)
		{
			Delaunay2D.Edge edge = obj as Delaunay2D.Edge;
			return edge != null && this == edge;
		}

		// Token: 0x06001468 RID: 5224 RVA: 0x00055818 File Offset: 0x00053A18
		public bool Equals(Delaunay2D.Edge e)
		{
			return this == e;
		}

		// Token: 0x06001469 RID: 5225 RVA: 0x00055821 File Offset: 0x00053A21
		public override int GetHashCode()
		{
			return this.U.GetHashCode() ^ this.V.GetHashCode();
		}

		// Token: 0x0600146A RID: 5226 RVA: 0x0005583C File Offset: 0x00053A3C
		public static bool AlmostEqual(Delaunay2D.Edge left, Delaunay2D.Edge right)
		{
			return (Delaunay2D.AlmostEqual(left.U, right.U) && Delaunay2D.AlmostEqual(left.V, right.V)) || (Delaunay2D.AlmostEqual(left.U, right.V) && Delaunay2D.AlmostEqual(left.V, right.U));
		}
	}
}
