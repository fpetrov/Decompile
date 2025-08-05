using System;
using System.Collections;
using System.Collections.Generic;

namespace BlueRaja
{
	// Token: 0x0200022A RID: 554
	public class SimplePriorityQueue<TItem, TPriority> : IPriorityQueue<TItem, TPriority>, IEnumerable<TItem>, IEnumerable where TPriority : IComparable<TPriority>
	{
		// Token: 0x060015DC RID: 5596 RVA: 0x0005B53E File Offset: 0x0005973E
		public SimplePriorityQueue()
			: this(Comparer<TPriority>.Default, EqualityComparer<TItem>.Default)
		{
		}

		// Token: 0x060015DD RID: 5597 RVA: 0x0005B550 File Offset: 0x00059750
		public SimplePriorityQueue(IComparer<TPriority> priorityComparer)
			: this(new Comparison<TPriority>(priorityComparer.Compare), EqualityComparer<TItem>.Default)
		{
		}

		// Token: 0x060015DE RID: 5598 RVA: 0x0005B56A File Offset: 0x0005976A
		public SimplePriorityQueue(Comparison<TPriority> priorityComparer)
			: this(priorityComparer, EqualityComparer<TItem>.Default)
		{
		}

		// Token: 0x060015DF RID: 5599 RVA: 0x0005B578 File Offset: 0x00059778
		public SimplePriorityQueue(IEqualityComparer<TItem> itemEquality)
			: this(Comparer<TPriority>.Default, itemEquality)
		{
		}

		// Token: 0x060015E0 RID: 5600 RVA: 0x0005B586 File Offset: 0x00059786
		public SimplePriorityQueue(IComparer<TPriority> priorityComparer, IEqualityComparer<TItem> itemEquality)
			: this(new Comparison<TPriority>(priorityComparer.Compare), itemEquality)
		{
		}

		// Token: 0x060015E1 RID: 5601 RVA: 0x0005B59C File Offset: 0x0005979C
		public SimplePriorityQueue(Comparison<TPriority> priorityComparer, IEqualityComparer<TItem> itemEquality)
		{
			this._queue = new GenericPriorityQueue<SimplePriorityQueue<TItem, TPriority>.SimpleNode, TPriority>(10, priorityComparer);
			this._itemToNodesCache = new Dictionary<TItem, IList<SimplePriorityQueue<TItem, TPriority>.SimpleNode>>(itemEquality);
			this._nullNodesCache = new List<SimplePriorityQueue<TItem, TPriority>.SimpleNode>();
		}

		// Token: 0x060015E2 RID: 5602 RVA: 0x0005B5CC File Offset: 0x000597CC
		private SimplePriorityQueue<TItem, TPriority>.SimpleNode GetExistingNode(TItem item)
		{
			if (item == null)
			{
				if (this._nullNodesCache.Count <= 0)
				{
					return null;
				}
				return this._nullNodesCache[0];
			}
			else
			{
				IList<SimplePriorityQueue<TItem, TPriority>.SimpleNode> list;
				if (!this._itemToNodesCache.TryGetValue(item, out list))
				{
					return null;
				}
				return list[0];
			}
		}

		// Token: 0x060015E3 RID: 5603 RVA: 0x0005B618 File Offset: 0x00059818
		private void AddToNodeCache(SimplePriorityQueue<TItem, TPriority>.SimpleNode node)
		{
			if (node.Data == null)
			{
				this._nullNodesCache.Add(node);
				return;
			}
			IList<SimplePriorityQueue<TItem, TPriority>.SimpleNode> list;
			if (!this._itemToNodesCache.TryGetValue(node.Data, out list))
			{
				list = new List<SimplePriorityQueue<TItem, TPriority>.SimpleNode>();
				this._itemToNodesCache[node.Data] = list;
			}
			list.Add(node);
		}

		// Token: 0x060015E4 RID: 5604 RVA: 0x0005B674 File Offset: 0x00059874
		private void RemoveFromNodeCache(SimplePriorityQueue<TItem, TPriority>.SimpleNode node)
		{
			if (node.Data == null)
			{
				this._nullNodesCache.Remove(node);
				return;
			}
			IList<SimplePriorityQueue<TItem, TPriority>.SimpleNode> list;
			if (!this._itemToNodesCache.TryGetValue(node.Data, out list))
			{
				return;
			}
			list.Remove(node);
			if (list.Count == 0)
			{
				this._itemToNodesCache.Remove(node.Data);
			}
		}

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x060015E5 RID: 5605 RVA: 0x0005B6D4 File Offset: 0x000598D4
		public int Count
		{
			get
			{
				GenericPriorityQueue<SimplePriorityQueue<TItem, TPriority>.SimpleNode, TPriority> queue = this._queue;
				int count;
				lock (queue)
				{
					count = this._queue.Count;
				}
				return count;
			}
		}

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x060015E6 RID: 5606 RVA: 0x0005B71C File Offset: 0x0005991C
		public TItem First
		{
			get
			{
				GenericPriorityQueue<SimplePriorityQueue<TItem, TPriority>.SimpleNode, TPriority> queue = this._queue;
				TItem data;
				lock (queue)
				{
					if (this._queue.Count <= 0)
					{
						throw new InvalidOperationException("Cannot call .First on an empty queue");
					}
					data = this._queue.First.Data;
				}
				return data;
			}
		}

		// Token: 0x060015E7 RID: 5607 RVA: 0x0005B784 File Offset: 0x00059984
		public void Clear()
		{
			GenericPriorityQueue<SimplePriorityQueue<TItem, TPriority>.SimpleNode, TPriority> queue = this._queue;
			lock (queue)
			{
				this._queue.Clear();
				this._itemToNodesCache.Clear();
				this._nullNodesCache.Clear();
			}
		}

		// Token: 0x060015E8 RID: 5608 RVA: 0x0005B7E0 File Offset: 0x000599E0
		public bool Contains(TItem item)
		{
			GenericPriorityQueue<SimplePriorityQueue<TItem, TPriority>.SimpleNode, TPriority> queue = this._queue;
			bool flag2;
			lock (queue)
			{
				flag2 = ((item == null) ? (this._nullNodesCache.Count > 0) : this._itemToNodesCache.ContainsKey(item));
			}
			return flag2;
		}

		// Token: 0x060015E9 RID: 5609 RVA: 0x0005B840 File Offset: 0x00059A40
		public TItem Dequeue()
		{
			GenericPriorityQueue<SimplePriorityQueue<TItem, TPriority>.SimpleNode, TPriority> queue = this._queue;
			TItem data;
			lock (queue)
			{
				if (this._queue.Count <= 0)
				{
					throw new InvalidOperationException("Cannot call Dequeue() on an empty queue");
				}
				SimplePriorityQueue<TItem, TPriority>.SimpleNode simpleNode = this._queue.Dequeue();
				this.RemoveFromNodeCache(simpleNode);
				data = simpleNode.Data;
			}
			return data;
		}

		// Token: 0x060015EA RID: 5610 RVA: 0x0005B8B0 File Offset: 0x00059AB0
		private SimplePriorityQueue<TItem, TPriority>.SimpleNode EnqueueNoLockOrCache(TItem item, TPriority priority)
		{
			SimplePriorityQueue<TItem, TPriority>.SimpleNode simpleNode = new SimplePriorityQueue<TItem, TPriority>.SimpleNode(item);
			if (this._queue.Count == this._queue.MaxSize)
			{
				this._queue.Resize(this._queue.MaxSize * 2 + 1);
			}
			this._queue.Enqueue(simpleNode, priority);
			return simpleNode;
		}

		// Token: 0x060015EB RID: 5611 RVA: 0x0005B904 File Offset: 0x00059B04
		public void Enqueue(TItem item, TPriority priority)
		{
			GenericPriorityQueue<SimplePriorityQueue<TItem, TPriority>.SimpleNode, TPriority> queue = this._queue;
			lock (queue)
			{
				IList<SimplePriorityQueue<TItem, TPriority>.SimpleNode> list;
				if (item == null)
				{
					list = this._nullNodesCache;
				}
				else if (!this._itemToNodesCache.TryGetValue(item, out list))
				{
					list = new List<SimplePriorityQueue<TItem, TPriority>.SimpleNode>();
					this._itemToNodesCache[item] = list;
				}
				SimplePriorityQueue<TItem, TPriority>.SimpleNode simpleNode = this.EnqueueNoLockOrCache(item, priority);
				list.Add(simpleNode);
			}
		}

		// Token: 0x060015EC RID: 5612 RVA: 0x0005B984 File Offset: 0x00059B84
		public bool EnqueueWithoutDuplicates(TItem item, TPriority priority)
		{
			GenericPriorityQueue<SimplePriorityQueue<TItem, TPriority>.SimpleNode, TPriority> queue = this._queue;
			bool flag2;
			lock (queue)
			{
				IList<SimplePriorityQueue<TItem, TPriority>.SimpleNode> list;
				if (item == null)
				{
					if (this._nullNodesCache.Count > 0)
					{
						return false;
					}
					list = this._nullNodesCache;
				}
				else
				{
					if (this._itemToNodesCache.ContainsKey(item))
					{
						return false;
					}
					list = new List<SimplePriorityQueue<TItem, TPriority>.SimpleNode>();
					this._itemToNodesCache[item] = list;
				}
				SimplePriorityQueue<TItem, TPriority>.SimpleNode simpleNode = this.EnqueueNoLockOrCache(item, priority);
				list.Add(simpleNode);
				flag2 = true;
			}
			return flag2;
		}

		// Token: 0x060015ED RID: 5613 RVA: 0x0005BA20 File Offset: 0x00059C20
		public void Remove(TItem item)
		{
			GenericPriorityQueue<SimplePriorityQueue<TItem, TPriority>.SimpleNode, TPriority> queue = this._queue;
			lock (queue)
			{
				SimplePriorityQueue<TItem, TPriority>.SimpleNode simpleNode;
				IList<SimplePriorityQueue<TItem, TPriority>.SimpleNode> nullNodesCache;
				if (item == null)
				{
					if (this._nullNodesCache.Count == 0)
					{
						string text = "Cannot call Remove() on a node which is not enqueued: ";
						TItem titem = item;
						throw new InvalidOperationException(text + ((titem != null) ? titem.ToString() : null));
					}
					simpleNode = this._nullNodesCache[0];
					nullNodesCache = this._nullNodesCache;
				}
				else
				{
					if (!this._itemToNodesCache.TryGetValue(item, out nullNodesCache))
					{
						string text2 = "Cannot call Remove() on a node which is not enqueued: ";
						TItem titem = item;
						throw new InvalidOperationException(text2 + ((titem != null) ? titem.ToString() : null));
					}
					simpleNode = nullNodesCache[0];
					if (nullNodesCache.Count == 1)
					{
						this._itemToNodesCache.Remove(item);
					}
				}
				this._queue.Remove(simpleNode);
				nullNodesCache.Remove(simpleNode);
			}
		}

		// Token: 0x060015EE RID: 5614 RVA: 0x0005BB20 File Offset: 0x00059D20
		public void UpdatePriority(TItem item, TPriority priority)
		{
			GenericPriorityQueue<SimplePriorityQueue<TItem, TPriority>.SimpleNode, TPriority> queue = this._queue;
			lock (queue)
			{
				SimplePriorityQueue<TItem, TPriority>.SimpleNode existingNode = this.GetExistingNode(item);
				if (existingNode == null)
				{
					string text = "Cannot call UpdatePriority() on a node which is not enqueued: ";
					TItem titem = item;
					throw new InvalidOperationException(text + ((titem != null) ? titem.ToString() : null));
				}
				this._queue.UpdatePriority(existingNode, priority);
			}
		}

		// Token: 0x060015EF RID: 5615 RVA: 0x0005BB9C File Offset: 0x00059D9C
		public TPriority GetPriority(TItem item)
		{
			GenericPriorityQueue<SimplePriorityQueue<TItem, TPriority>.SimpleNode, TPriority> queue = this._queue;
			TPriority priority;
			lock (queue)
			{
				SimplePriorityQueue<TItem, TPriority>.SimpleNode existingNode = this.GetExistingNode(item);
				if (existingNode == null)
				{
					string text = "Cannot call GetPriority() on a node which is not enqueued: ";
					TItem titem = item;
					throw new InvalidOperationException(text + ((titem != null) ? titem.ToString() : null));
				}
				priority = existingNode.Priority;
			}
			return priority;
		}

		// Token: 0x060015F0 RID: 5616 RVA: 0x0005BC14 File Offset: 0x00059E14
		public bool TryFirst(out TItem first)
		{
			if (this._queue.Count > 0)
			{
				GenericPriorityQueue<SimplePriorityQueue<TItem, TPriority>.SimpleNode, TPriority> queue = this._queue;
				lock (queue)
				{
					if (this._queue.Count > 0)
					{
						first = this._queue.First.Data;
						return true;
					}
				}
			}
			first = default(TItem);
			return false;
		}

		// Token: 0x060015F1 RID: 5617 RVA: 0x0005BC90 File Offset: 0x00059E90
		public bool TryDequeue(out TItem first)
		{
			if (this._queue.Count > 0)
			{
				GenericPriorityQueue<SimplePriorityQueue<TItem, TPriority>.SimpleNode, TPriority> queue = this._queue;
				lock (queue)
				{
					if (this._queue.Count > 0)
					{
						SimplePriorityQueue<TItem, TPriority>.SimpleNode simpleNode = this._queue.Dequeue();
						first = simpleNode.Data;
						this.RemoveFromNodeCache(simpleNode);
						return true;
					}
				}
			}
			first = default(TItem);
			return false;
		}

		// Token: 0x060015F2 RID: 5618 RVA: 0x0005BD14 File Offset: 0x00059F14
		public bool TryRemove(TItem item)
		{
			GenericPriorityQueue<SimplePriorityQueue<TItem, TPriority>.SimpleNode, TPriority> queue = this._queue;
			bool flag2;
			lock (queue)
			{
				SimplePriorityQueue<TItem, TPriority>.SimpleNode simpleNode;
				IList<SimplePriorityQueue<TItem, TPriority>.SimpleNode> nullNodesCache;
				if (item == null)
				{
					if (this._nullNodesCache.Count == 0)
					{
						return false;
					}
					simpleNode = this._nullNodesCache[0];
					nullNodesCache = this._nullNodesCache;
				}
				else
				{
					if (!this._itemToNodesCache.TryGetValue(item, out nullNodesCache))
					{
						return false;
					}
					simpleNode = nullNodesCache[0];
					if (nullNodesCache.Count == 1)
					{
						this._itemToNodesCache.Remove(item);
					}
				}
				this._queue.Remove(simpleNode);
				nullNodesCache.Remove(simpleNode);
				flag2 = true;
			}
			return flag2;
		}

		// Token: 0x060015F3 RID: 5619 RVA: 0x0005BDCC File Offset: 0x00059FCC
		public bool TryUpdatePriority(TItem item, TPriority priority)
		{
			GenericPriorityQueue<SimplePriorityQueue<TItem, TPriority>.SimpleNode, TPriority> queue = this._queue;
			bool flag2;
			lock (queue)
			{
				SimplePriorityQueue<TItem, TPriority>.SimpleNode existingNode = this.GetExistingNode(item);
				if (existingNode == null)
				{
					flag2 = false;
				}
				else
				{
					this._queue.UpdatePriority(existingNode, priority);
					flag2 = true;
				}
			}
			return flag2;
		}

		// Token: 0x060015F4 RID: 5620 RVA: 0x0005BE28 File Offset: 0x0005A028
		public bool TryGetPriority(TItem item, out TPriority priority)
		{
			GenericPriorityQueue<SimplePriorityQueue<TItem, TPriority>.SimpleNode, TPriority> queue = this._queue;
			bool flag2;
			lock (queue)
			{
				SimplePriorityQueue<TItem, TPriority>.SimpleNode existingNode = this.GetExistingNode(item);
				if (existingNode == null)
				{
					priority = default(TPriority);
					flag2 = false;
				}
				else
				{
					priority = existingNode.Priority;
					flag2 = true;
				}
			}
			return flag2;
		}

		// Token: 0x060015F5 RID: 5621 RVA: 0x0005BE88 File Offset: 0x0005A088
		public IEnumerator<TItem> GetEnumerator()
		{
			List<TItem> list = new List<TItem>();
			GenericPriorityQueue<SimplePriorityQueue<TItem, TPriority>.SimpleNode, TPriority> queue = this._queue;
			lock (queue)
			{
				foreach (SimplePriorityQueue<TItem, TPriority>.SimpleNode simpleNode in this._queue)
				{
					list.Add(simpleNode.Data);
				}
			}
			return list.GetEnumerator();
		}

		// Token: 0x060015F6 RID: 5622 RVA: 0x0005BF14 File Offset: 0x0005A114
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x060015F7 RID: 5623 RVA: 0x0005BF1C File Offset: 0x0005A11C
		public bool IsValidQueue()
		{
			GenericPriorityQueue<SimplePriorityQueue<TItem, TPriority>.SimpleNode, TPriority> queue = this._queue;
			bool flag2;
			lock (queue)
			{
				foreach (IList<SimplePriorityQueue<TItem, TPriority>.SimpleNode> list in this._itemToNodesCache.Values)
				{
					foreach (SimplePriorityQueue<TItem, TPriority>.SimpleNode simpleNode in list)
					{
						if (!this._queue.Contains(simpleNode))
						{
							return false;
						}
					}
				}
				foreach (SimplePriorityQueue<TItem, TPriority>.SimpleNode simpleNode2 in this._queue)
				{
					if (this.GetExistingNode(simpleNode2.Data) == null)
					{
						return false;
					}
				}
				flag2 = this._queue.IsValidQueue();
			}
			return flag2;
		}

		// Token: 0x04000CB7 RID: 3255
		private const int INITIAL_QUEUE_SIZE = 10;

		// Token: 0x04000CB8 RID: 3256
		private readonly GenericPriorityQueue<SimplePriorityQueue<TItem, TPriority>.SimpleNode, TPriority> _queue;

		// Token: 0x04000CB9 RID: 3257
		private readonly Dictionary<TItem, IList<SimplePriorityQueue<TItem, TPriority>.SimpleNode>> _itemToNodesCache;

		// Token: 0x04000CBA RID: 3258
		private readonly IList<SimplePriorityQueue<TItem, TPriority>.SimpleNode> _nullNodesCache;

		// Token: 0x0200022B RID: 555
		private class SimpleNode : GenericPriorityQueueNode<TPriority>
		{
			// Token: 0x1700024D RID: 589
			// (get) Token: 0x060015F8 RID: 5624 RVA: 0x0005C038 File Offset: 0x0005A238
			// (set) Token: 0x060015F9 RID: 5625 RVA: 0x0005C040 File Offset: 0x0005A240
			public TItem Data { get; private set; }

			// Token: 0x060015FA RID: 5626 RVA: 0x0005C049 File Offset: 0x0005A249
			public SimpleNode(TItem data)
			{
				this.Data = data;
			}
		}
	}
}
