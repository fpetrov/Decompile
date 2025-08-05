using System;
using System.Collections;
using System.Collections.Generic;

namespace BlueRaja
{
	// Token: 0x02000222 RID: 546
	public sealed class FastPriorityQueue<T> : IFixedSizePriorityQueue<T, float>, IPriorityQueue<T, float>, IEnumerable<T>, IEnumerable where T : FastPriorityQueueNode
	{
		// Token: 0x06001590 RID: 5520 RVA: 0x0005A4D9 File Offset: 0x000586D9
		public FastPriorityQueue(int maxNodes)
		{
			this._numNodes = 0;
			this._nodes = new T[maxNodes + 1];
		}

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x06001591 RID: 5521 RVA: 0x0005A4F6 File Offset: 0x000586F6
		public int Count
		{
			get
			{
				return this._numNodes;
			}
		}

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x06001592 RID: 5522 RVA: 0x0005A4FE File Offset: 0x000586FE
		public int MaxSize
		{
			get
			{
				return this._nodes.Length - 1;
			}
		}

		// Token: 0x06001593 RID: 5523 RVA: 0x0005A50A File Offset: 0x0005870A
		public void Clear()
		{
			Array.Clear(this._nodes, 1, this._numNodes);
			this._numNodes = 0;
		}

		// Token: 0x06001594 RID: 5524 RVA: 0x0005A525 File Offset: 0x00058725
		public bool Contains(T node)
		{
			return this._nodes[node.QueueIndex] == node;
		}

		// Token: 0x06001595 RID: 5525 RVA: 0x0005A54C File Offset: 0x0005874C
		public void Enqueue(T node, float priority)
		{
			node.Priority = priority;
			this._numNodes++;
			this._nodes[this._numNodes] = node;
			node.QueueIndex = this._numNodes;
			this.CascadeUp(node);
		}

		// Token: 0x06001596 RID: 5526 RVA: 0x0005A5A0 File Offset: 0x000587A0
		private void CascadeUp(T node)
		{
			if (node.QueueIndex <= 1)
			{
				return;
			}
			int i = node.QueueIndex >> 1;
			T t = this._nodes[i];
			if (this.HasHigherOrEqualPriority(t, node))
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
				if (this.HasHigherOrEqualPriority(t2, node))
				{
					break;
				}
				this._nodes[node.QueueIndex] = t2;
				t2.QueueIndex = node.QueueIndex;
				node.QueueIndex = i;
			}
			this._nodes[node.QueueIndex] = node;
		}

		// Token: 0x06001597 RID: 5527 RVA: 0x0005A68C File Offset: 0x0005888C
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

		// Token: 0x06001598 RID: 5528 RVA: 0x0005A8F8 File Offset: 0x00058AF8
		private bool HasHigherPriority(T higher, T lower)
		{
			return higher.Priority < lower.Priority;
		}

		// Token: 0x06001599 RID: 5529 RVA: 0x0005A912 File Offset: 0x00058B12
		private bool HasHigherOrEqualPriority(T higher, T lower)
		{
			return higher.Priority <= lower.Priority;
		}

		// Token: 0x0600159A RID: 5530 RVA: 0x0005A930 File Offset: 0x00058B30
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

		// Token: 0x0600159B RID: 5531 RVA: 0x0005A9CC File Offset: 0x00058BCC
		public void Resize(int maxNodes)
		{
			T[] array = new T[maxNodes + 1];
			int num = Math.Min(maxNodes, this._numNodes);
			Array.Copy(this._nodes, array, num + 1);
			this._nodes = array;
		}

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x0600159C RID: 5532 RVA: 0x0005AA05 File Offset: 0x00058C05
		public T First
		{
			get
			{
				return this._nodes[1];
			}
		}

		// Token: 0x0600159D RID: 5533 RVA: 0x0005AA13 File Offset: 0x00058C13
		public void UpdatePriority(T node, float priority)
		{
			node.Priority = priority;
			this.OnNodeUpdated(node);
		}

		// Token: 0x0600159E RID: 5534 RVA: 0x0005AA28 File Offset: 0x00058C28
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

		// Token: 0x0600159F RID: 5535 RVA: 0x0005AA6C File Offset: 0x00058C6C
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

		// Token: 0x060015A0 RID: 5536 RVA: 0x0005AB23 File Offset: 0x00058D23
		public void ResetNode(T node)
		{
			node.QueueIndex = 0;
		}

		// Token: 0x060015A1 RID: 5537 RVA: 0x0005AB31 File Offset: 0x00058D31
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

		// Token: 0x060015A2 RID: 5538 RVA: 0x0005AB40 File Offset: 0x00058D40
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x060015A3 RID: 5539 RVA: 0x0005AB48 File Offset: 0x00058D48
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

		// Token: 0x04000CA4 RID: 3236
		private int _numNodes;

		// Token: 0x04000CA5 RID: 3237
		private T[] _nodes;
	}
}
