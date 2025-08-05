using System;
using System.Collections.Generic;
using BlueRaja;
using UnityEngine;

// Token: 0x020001FD RID: 509
public class DungeonPathfinder2D
{
	// Token: 0x06001471 RID: 5233 RVA: 0x000558F0 File Offset: 0x00053AF0
	public DungeonPathfinder2D(Vector2Int size)
	{
		this.grid = new Grid2D<DungeonPathfinder2D.Node>(size, Vector2Int.zero);
		this.queue = new SimplePriorityQueue<DungeonPathfinder2D.Node, float>();
		this.closed = new HashSet<DungeonPathfinder2D.Node>();
		this.stack = new Stack<Vector2Int>();
		for (int i = 0; i < size.x; i++)
		{
			for (int j = 0; j < size.y; j++)
			{
				this.grid[i, j] = new DungeonPathfinder2D.Node(new Vector2Int(i, j));
			}
		}
	}

	// Token: 0x06001472 RID: 5234 RVA: 0x00055974 File Offset: 0x00053B74
	private void ResetNodes()
	{
		Vector2Int size = this.grid.Size;
		for (int i = 0; i < size.x; i++)
		{
			for (int j = 0; j < size.y; j++)
			{
				DungeonPathfinder2D.Node node = this.grid[i, j];
				node.Previous = null;
				node.Cost = float.PositiveInfinity;
			}
		}
	}

	// Token: 0x06001473 RID: 5235 RVA: 0x000559D0 File Offset: 0x00053BD0
	public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, Func<DungeonPathfinder2D.Node, DungeonPathfinder2D.Node, DungeonPathfinder2D.PathCost> costFunction)
	{
		this.ResetNodes();
		this.queue.Clear();
		this.closed.Clear();
		this.queue = new SimplePriorityQueue<DungeonPathfinder2D.Node, float>();
		this.closed = new HashSet<DungeonPathfinder2D.Node>();
		this.grid[start].Cost = 0f;
		this.queue.Enqueue(this.grid[start], 0f);
		while (this.queue.Count > 0)
		{
			DungeonPathfinder2D.Node node = this.queue.Dequeue();
			this.closed.Add(node);
			if (node.Position == end)
			{
				return this.ReconstructPath(node);
			}
			foreach (Vector2Int vector2Int in DungeonPathfinder2D.neighbors)
			{
				if (this.grid.InBounds(node.Position + vector2Int))
				{
					DungeonPathfinder2D.Node node2 = this.grid[node.Position + vector2Int];
					if (!this.closed.Contains(node2))
					{
						DungeonPathfinder2D.PathCost pathCost = costFunction(node, node2);
						if (pathCost.traversable)
						{
							float num = node.Cost + pathCost.cost;
							if (num < node2.Cost)
							{
								node2.Previous = node;
								node2.Cost = num;
								float num2;
								if (this.queue.TryGetPriority(node, out num2))
								{
									this.queue.UpdatePriority(node, num);
								}
								else
								{
									this.queue.Enqueue(node2, node2.Cost);
								}
							}
						}
					}
				}
			}
		}
		return null;
	}

	// Token: 0x06001474 RID: 5236 RVA: 0x00055B64 File Offset: 0x00053D64
	private List<Vector2Int> ReconstructPath(DungeonPathfinder2D.Node node)
	{
		List<Vector2Int> list = new List<Vector2Int>();
		while (node != null)
		{
			this.stack.Push(node.Position);
			node = node.Previous;
		}
		while (this.stack.Count > 0)
		{
			list.Add(this.stack.Pop());
		}
		return list;
	}

	// Token: 0x04000BEB RID: 3051
	private static readonly Vector2Int[] neighbors = new Vector2Int[]
	{
		new Vector2Int(1, 0),
		new Vector2Int(-1, 0),
		new Vector2Int(0, 1),
		new Vector2Int(0, -1)
	};

	// Token: 0x04000BEC RID: 3052
	private Grid2D<DungeonPathfinder2D.Node> grid;

	// Token: 0x04000BED RID: 3053
	private SimplePriorityQueue<DungeonPathfinder2D.Node, float> queue;

	// Token: 0x04000BEE RID: 3054
	private HashSet<DungeonPathfinder2D.Node> closed;

	// Token: 0x04000BEF RID: 3055
	private Stack<Vector2Int> stack;

	// Token: 0x020001FE RID: 510
	public class Node
	{
		// Token: 0x17000216 RID: 534
		// (get) Token: 0x06001476 RID: 5238 RVA: 0x00055C08 File Offset: 0x00053E08
		// (set) Token: 0x06001477 RID: 5239 RVA: 0x00055C10 File Offset: 0x00053E10
		public Vector2Int Position { get; private set; }

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x06001478 RID: 5240 RVA: 0x00055C19 File Offset: 0x00053E19
		// (set) Token: 0x06001479 RID: 5241 RVA: 0x00055C21 File Offset: 0x00053E21
		public DungeonPathfinder2D.Node Previous { get; set; }

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x0600147A RID: 5242 RVA: 0x00055C2A File Offset: 0x00053E2A
		// (set) Token: 0x0600147B RID: 5243 RVA: 0x00055C32 File Offset: 0x00053E32
		public float Cost { get; set; }

		// Token: 0x0600147C RID: 5244 RVA: 0x00055C3B File Offset: 0x00053E3B
		public Node(Vector2Int position)
		{
			this.Position = position;
		}
	}

	// Token: 0x020001FF RID: 511
	public struct PathCost
	{
		// Token: 0x04000BF3 RID: 3059
		public bool traversable;

		// Token: 0x04000BF4 RID: 3060
		public float cost;
	}
}
