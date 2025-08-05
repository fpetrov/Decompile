using System;
using System.Collections;
using System.Collections.Generic;

namespace BlueRaja
{
	// Token: 0x0200022D RID: 557
	public sealed class StablePriorityQueue<T> : IFixedSizePriorityQueue<T, float>, IPriorityQueue<T, float>, IEnumerable<T>, IEnumerable where T : StablePriorityQueueNode
	{
		// Token: 0x060015FE RID: 5630 RVA: 0x0005C072 File Offset: 0x0005A272
		public StablePriorityQueue(int maxNodes)
		{
			this._numNodes = 0;
			this._nodes = new T[maxNodes + 1];
			this._numNodesEverEnqueued = 0L;
		}

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x060015FF RID: 5631 RVA: 0x0005C097 File Offset: 0x0005A297
		public int Count
		{
			get
			{
				return this._numNodes;
			}
		}

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x06001600 RID: 5632 RVA: 0x0005C09F File Offset: 0x0005A29F
		public int MaxSize
		{
			get
			{
				return this._nodes.Length - 1;
			}
		}

		// Token: 0x06001601 RID: 5633 RVA: 0x0005C0AB File Offset: 0x0005A2AB
		public void Clear()
		{
			Array.Clear(this._nodes, 1, this._numNodes);
			this._numNodes = 0;
		}

		// Token: 0x06001602 RID: 5634 RVA: 0x0005C0C6 File Offset: 0x0005A2C6
		public bool Contains(T node)
		{
			return this._nodes[node.QueueIndex] == node;
		}

		// Token: 0x06001603 RID: 5635 RVA: 0x0005C0EC File Offset: 0x0005A2EC
		public void Enqueue(T node, float priority)
		{
			node.Priority = priority;
			this._numNodes++;
			this._nodes[this._numNodes] = node;
			node.QueueIndex = this._numNodes;
			StablePriorityQueueNode stablePriorityQueueNode = node;
			long numNodesEverEnqueued = this._numNodesEverEnqueued;
			this._numNodesEverEnqueued = numNodesEverEnqueued + 1L;
			stablePriorityQueueNode.InsertionIndex = numNodesEverEnqueued;
			this.CascadeUp(node);
		}

		// Token: 0x06001604 RID: 5636 RVA: 0x0005C15C File Offset: 0x0005A35C
		private void CascadeUp(T node)
		{
			if (node.QueueIndex <= 1)
			{
				return;
			}
			int i = node.QueueIndex >> 1;
			T t = this._nodes[i];
			if (this.HasHigherPriority(t, node))
			{
				return;
			}
			this._nodes[node.QueueIndex] = t;
			t.QueueIndex = node.QueueIndex;
			node.QueueIndex = i;
			while (i > 1)
			{
				i >>= 1;
				T t2 = this._nodes[i];
				if (this.HasHigherPriority(t2, node))
				{
					break;
				}
				this._nodes[node.QueueIndex] = t2;
				t2.QueueIndex = node.QueueIndex;
				node.QueueIndex = i;
			}
			this._nodes[node.QueueIndex] = node;
		}

		// Token: 0x06001605 RID: 5637 RVA: 0x0005C248 File Offset: 0x0005A448
		private void CascadeDown(T node)
		{
			int num = node.QueueIndex;
			int num2 = 2 * num;
			if (num2 > this._numNodes)
			{
				return;
			}
			int num3 = num2 + 1;
			T t = this._nodes[num2];
			if (this.HasHigherPriority(t, node))
			{
				if (num3 > this._numNodes)
				{
					node.QueueIndex = num2;
					t.QueueIndex = num;
					this._nodes[num] = t;
					this._nodes[num2] = node;
					return;
				}
				T t2 = this._nodes[num3];
				if (this.HasHigherPriority(t, t2))
				{
					t.QueueIndex = num;
					this._nodes[num] = t;
					num = num2;
				}
				else
				{
					t2.QueueIndex = num;
					this._nodes[num] = t2;
					num = num3;
				}
			}
			else
			{
				if (num3 > this._numNodes)
				{
					return;
				}
				T t3 = this._nodes[num3];
				if (!this.HasHigherPriority(t3, node))
				{
					return;
				}
				t3.QueueIndex = num;
				this._nodes[num] = t3;
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
				t = this._nodes[num2];
				if (this.HasHigherPriority(t, node))
				{
					if (num3 > this._numNodes)
					{
						goto Block_9;
					}
					T t4 = this._nodes[num3];
					if (this.HasHigherPriority(t, t4))
					{
						t.QueueIndex = num;
						this._nodes[num] = t;
						num = num2;
					}
					else
					{
						t4.QueueIndex = num;
						this._nodes[num] = t4;
						num = num3;
					}
				}
				else
				{
					if (num3 > this._numNodes)
					{
						goto Block_11;
					}
					T t5 = this._nodes[num3];
					if (!this.HasHigherPriority(t5, node))
					{
						goto IL_0246;
					}
					t5.QueueIndex = num;
					this._nodes[num] = t5;
					num = num3;
				}
			}
			node.QueueIndex = num;
			this._nodes[num] = node;
			return;
			Block_9:
			node.QueueIndex = num2;
			t.QueueIndex = num;
			this._nodes[num] = t;
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

		// Token: 0x06001606 RID: 5638 RVA: 0x0005C4B4 File Offset: 0x0005A6B4
		private bool HasHigherPriority(T higher, T lower)
		{
			return higher.Priority < lower.Priority || (higher.Priority == lower.Priority && higher.InsertionIndex < lower.InsertionIndex);
		}

		// Token: 0x06001607 RID: 5639 RVA: 0x0005C510 File Offset: 0x0005A710
		public T Dequeue()
		{
			T t = this._nodes[1];
			if (this._numNodes == 1)
			{
				this._nodes[1] = default(T);
				this._numNodes = 0;
				return t;
			}
			T t2 = this._nodes[this._numNodes];
			this._nodes[1] = t2;
			t2.QueueIndex = 1;
			this._nodes[this._numNodes] = default(T);
			this._numNodes--;
			this.CascadeDown(t2);
			return t;
		}

		// Token: 0x06001608 RID: 5640 RVA: 0x0005C5AC File Offset: 0x0005A7AC
		public void Resize(int maxNodes)
		{
			T[] array = new T[maxNodes + 1];
			int num = Math.Min(maxNodes, this._numNodes);
			Array.Copy(this._nodes, array, num + 1);
			this._nodes = array;
		}

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x06001609 RID: 5641 RVA: 0x0005C5E5 File Offset: 0x0005A7E5
		public T First
		{
			get
			{
				return this._nodes[1];
			}
		}

		// Token: 0x0600160A RID: 5642 RVA: 0x0005C5F3 File Offset: 0x0005A7F3
		public void UpdatePriority(T node, float priority)
		{
			node.Priority = priority;
			this.OnNodeUpdated(node);
		}

		// Token: 0x0600160B RID: 5643 RVA: 0x0005C608 File Offset: 0x0005A808
		private void OnNodeUpdated(T node)
		{
			int num = node.QueueIndex >> 1;
			if (num > 0 && this.HasHigherPriority(node, this._nodes[num]))
			{
				this.CascadeUp(node);
				return;
			}
			this.CascadeDown(node);
		}

		// Token: 0x0600160C RID: 5644 RVA: 0x0005C64C File Offset: 0x0005A84C
		public void Remove(T node)
		{
			if (node.QueueIndex == this._numNodes)
			{
				this._nodes[this._numNodes] = default(T);
				this._numNodes--;
				return;
			}
			T t = this._nodes[this._numNodes];
			this._nodes[node.QueueIndex] = t;
			t.QueueIndex = node.QueueIndex;
			this._nodes[this._numNodes] = default(T);
			this._numNodes--;
			this.OnNodeUpdated(t);
		}

		// Token: 0x0600160D RID: 5645 RVA: 0x0005AB23 File Offset: 0x00058D23
		public void ResetNode(T node)
		{
			node.QueueIndex = 0;
		}

		// Token: 0x0600160E RID: 5646 RVA: 0x0005C703 File Offset: 0x0005A903
		public IEnumerator<T> GetEnumerator()
		{
			int num;
			for (int i = 1; i <= this._numNodes; i = num + 1)
			{
				yield return this._nodes[i];
				num = i;
			}
			yield break;
		}

		// Token: 0x0600160F RID: 5647 RVA: 0x0005C712 File Offset: 0x0005A912
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x06001610 RID: 5648 RVA: 0x0005C71C File Offset: 0x0005A91C
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

		// Token: 0x04000CBC RID: 3260
		private int _numNodes;

		// Token: 0x04000CBD RID: 3261
		private T[] _nodes;

		// Token: 0x04000CBE RID: 3262
		private long _numNodesEverEnqueued;
	}
}
