using System;
using System.Collections;
using System.Collections.Generic;

namespace BlueRaja
{
	// Token: 0x02000225 RID: 549
	public sealed class GenericPriorityQueue<TItem, TPriority> : IFixedSizePriorityQueue<TItem, TPriority>, IPriorityQueue<TItem, TPriority>, IEnumerable<TItem>, IEnumerable where TItem : GenericPriorityQueueNode<TPriority> where TPriority : IComparable<TPriority>
	{
		// Token: 0x060015AF RID: 5551 RVA: 0x0005ACCD File Offset: 0x00058ECD
		public GenericPriorityQueue(int maxNodes)
			: this(maxNodes, Comparer<TPriority>.Default)
		{
		}

		// Token: 0x060015B0 RID: 5552 RVA: 0x0005ACDB File Offset: 0x00058EDB
		public GenericPriorityQueue(int maxNodes, IComparer<TPriority> comparer)
			: this(maxNodes, new Comparison<TPriority>(comparer.Compare))
		{
		}

		// Token: 0x060015B1 RID: 5553 RVA: 0x0005ACF1 File Offset: 0x00058EF1
		public GenericPriorityQueue(int maxNodes, Comparison<TPriority> comparer)
		{
			this._numNodes = 0;
			this._nodes = new TItem[maxNodes + 1];
			this._numNodesEverEnqueued = 0L;
			this._comparer = comparer;
		}

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x060015B2 RID: 5554 RVA: 0x0005AD1D File Offset: 0x00058F1D
		public int Count
		{
			get
			{
				return this._numNodes;
			}
		}

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x060015B3 RID: 5555 RVA: 0x0005AD25 File Offset: 0x00058F25
		public int MaxSize
		{
			get
			{
				return this._nodes.Length - 1;
			}
		}

		// Token: 0x060015B4 RID: 5556 RVA: 0x0005AD31 File Offset: 0x00058F31
		public void Clear()
		{
			Array.Clear(this._nodes, 1, this._numNodes);
			this._numNodes = 0;
		}

		// Token: 0x060015B5 RID: 5557 RVA: 0x0005AD4C File Offset: 0x00058F4C
		public bool Contains(TItem node)
		{
			return this._nodes[node.QueueIndex] == node;
		}

		// Token: 0x060015B6 RID: 5558 RVA: 0x0005AD74 File Offset: 0x00058F74
		public void Enqueue(TItem node, TPriority priority)
		{
			node.Priority = priority;
			this._numNodes++;
			this._nodes[this._numNodes] = node;
			node.QueueIndex = this._numNodes;
			GenericPriorityQueueNode<TPriority> genericPriorityQueueNode = node;
			long numNodesEverEnqueued = this._numNodesEverEnqueued;
			this._numNodesEverEnqueued = numNodesEverEnqueued + 1L;
			genericPriorityQueueNode.InsertionIndex = numNodesEverEnqueued;
			this.CascadeUp(node);
		}

		// Token: 0x060015B7 RID: 5559 RVA: 0x0005ADE4 File Offset: 0x00058FE4
		private void CascadeUp(TItem node)
		{
			if (node.QueueIndex <= 1)
			{
				return;
			}
			int i = node.QueueIndex >> 1;
			TItem titem = this._nodes[i];
			if (this.HasHigherPriority(titem, node))
			{
				return;
			}
			this._nodes[node.QueueIndex] = titem;
			titem.QueueIndex = node.QueueIndex;
			node.QueueIndex = i;
			while (i > 1)
			{
				i >>= 1;
				TItem titem2 = this._nodes[i];
				if (this.HasHigherPriority(titem2, node))
				{
					break;
				}
				this._nodes[node.QueueIndex] = titem2;
				titem2.QueueIndex = node.QueueIndex;
				node.QueueIndex = i;
			}
			this._nodes[node.QueueIndex] = node;
		}

		// Token: 0x060015B8 RID: 5560 RVA: 0x0005AED0 File Offset: 0x000590D0
		private void CascadeDown(TItem node)
		{
			int num = node.QueueIndex;
			int num2 = 2 * num;
			if (num2 > this._numNodes)
			{
				return;
			}
			int num3 = num2 + 1;
			TItem titem = this._nodes[num2];
			if (this.HasHigherPriority(titem, node))
			{
				if (num3 > this._numNodes)
				{
					node.QueueIndex = num2;
					titem.QueueIndex = num;
					this._nodes[num] = titem;
					this._nodes[num2] = node;
					return;
				}
				TItem titem2 = this._nodes[num3];
				if (this.HasHigherPriority(titem, titem2))
				{
					titem.QueueIndex = num;
					this._nodes[num] = titem;
					num = num2;
				}
				else
				{
					titem2.QueueIndex = num;
					this._nodes[num] = titem2;
					num = num3;
				}
			}
			else
			{
				if (num3 > this._numNodes)
				{
					return;
				}
				TItem titem3 = this._nodes[num3];
				if (!this.HasHigherPriority(titem3, node))
				{
					return;
				}
				titem3.QueueIndex = num;
				this._nodes[num] = titem3;
				num = num3;
			}
			for (;;)
			{
				num2 = 2 * num;
				if (num2 > this._numNodes)
				{
					break;
				}
				num3 = num2 + 1;
				titem = this._nodes[num2];
				if (this.HasHigherPriority(titem, node))
				{
					if (num3 > this._numNodes)
					{
						goto Block_9;
					}
					TItem titem4 = this._nodes[num3];
					if (this.HasHigherPriority(titem, titem4))
					{
						titem.QueueIndex = num;
						this._nodes[num] = titem;
						num = num2;
					}
					else
					{
						titem4.QueueIndex = num;
						this._nodes[num] = titem4;
						num = num3;
					}
				}
				else
				{
					if (num3 > this._numNodes)
					{
						goto Block_11;
					}
					TItem titem5 = this._nodes[num3];
					if (!this.HasHigherPriority(titem5, node))
					{
						goto IL_0246;
					}
					titem5.QueueIndex = num;
					this._nodes[num] = titem5;
					num = num3;
				}
			}
			node.QueueIndex = num;
			this._nodes[num] = node;
			return;
			Block_9:
			node.QueueIndex = num2;
			titem.QueueIndex = num;
			this._nodes[num] = titem;
			this._nodes[num2] = node;
			return;
			Block_11:
			node.QueueIndex = num;
			this._nodes[num] = node;
			return;
			IL_0246:
			node.QueueIndex = num;
			this._nodes[num] = node;
		}

		// Token: 0x060015B9 RID: 5561 RVA: 0x0005B13C File Offset: 0x0005933C
		private bool HasHigherPriority(TItem higher, TItem lower)
		{
			int num = this._comparer(higher.Priority, lower.Priority);
			return num < 0 || (num == 0 && higher.InsertionIndex < lower.InsertionIndex);
		}

		// Token: 0x060015BA RID: 5562 RVA: 0x0005B190 File Offset: 0x00059390
		public TItem Dequeue()
		{
			TItem titem = this._nodes[1];
			if (this._numNodes == 1)
			{
				this._nodes[1] = default(TItem);
				this._numNodes = 0;
				return titem;
			}
			TItem titem2 = this._nodes[this._numNodes];
			this._nodes[1] = titem2;
			titem2.QueueIndex = 1;
			this._nodes[this._numNodes] = default(TItem);
			this._numNodes--;
			this.CascadeDown(titem2);
			return titem;
		}

		// Token: 0x060015BB RID: 5563 RVA: 0x0005B22C File Offset: 0x0005942C
		public void Resize(int maxNodes)
		{
			TItem[] array = new TItem[maxNodes + 1];
			int num = Math.Min(maxNodes, this._numNodes);
			Array.Copy(this._nodes, array, num + 1);
			this._nodes = array;
		}

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x060015BC RID: 5564 RVA: 0x0005B265 File Offset: 0x00059465
		public TItem First
		{
			get
			{
				return this._nodes[1];
			}
		}

		// Token: 0x060015BD RID: 5565 RVA: 0x0005B273 File Offset: 0x00059473
		public void UpdatePriority(TItem node, TPriority priority)
		{
			node.Priority = priority;
			this.OnNodeUpdated(node);
		}

		// Token: 0x060015BE RID: 5566 RVA: 0x0005B288 File Offset: 0x00059488
		private void OnNodeUpdated(TItem node)
		{
			int num = node.QueueIndex >> 1;
			if (num > 0 && this.HasHigherPriority(node, this._nodes[num]))
			{
				this.CascadeUp(node);
				return;
			}
			this.CascadeDown(node);
		}

		// Token: 0x060015BF RID: 5567 RVA: 0x0005B2CC File Offset: 0x000594CC
		public void Remove(TItem node)
		{
			if (node.QueueIndex == this._numNodes)
			{
				this._nodes[this._numNodes] = default(TItem);
				this._numNodes--;
				return;
			}
			TItem titem = this._nodes[this._numNodes];
			this._nodes[node.QueueIndex] = titem;
			titem.QueueIndex = node.QueueIndex;
			this._nodes[this._numNodes] = default(TItem);
			this._numNodes--;
			this.OnNodeUpdated(titem);
		}

		// Token: 0x060015C0 RID: 5568 RVA: 0x0005B383 File Offset: 0x00059583
		public void ResetNode(TItem node)
		{
			node.QueueIndex = 0;
		}

		// Token: 0x060015C1 RID: 5569 RVA: 0x0005B391 File Offset: 0x00059591
		public IEnumerator<TItem> GetEnumerator()
		{
			int num;
			for (int i = 1; i <= this._numNodes; i = num + 1)
			{
				yield return this._nodes[i];
				num = i;
			}
			yield break;
		}

		// Token: 0x060015C2 RID: 5570 RVA: 0x0005B3A0 File Offset: 0x000595A0
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x060015C3 RID: 5571 RVA: 0x0005B3A8 File Offset: 0x000595A8
		public bool IsValidQueue()
		{
			for (int i = 1; i < this._nodes.Length; i++)
			{
				if (this._nodes[i] != null)
				{
					int num = 2 * i;
					if (num < this._nodes.Length && this._nodes[num] != null && this.HasHigherPriority(this._nodes[num], this._nodes[i]))
					{
						return false;
					}
					int num2 = num + 1;
					if (num2 < this._nodes.Length && this._nodes[num2] != null && this.HasHigherPriority(this._nodes[num2], this._nodes[i]))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x04000CAC RID: 3244
		private int _numNodes;

		// Token: 0x04000CAD RID: 3245
		private TItem[] _nodes;

		// Token: 0x04000CAE RID: 3246
		private long _numNodesEverEnqueued;

		// Token: 0x04000CAF RID: 3247
		private readonly Comparison<TPriority> _comparer;
	}
}
