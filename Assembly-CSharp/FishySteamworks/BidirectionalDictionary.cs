using System;
using System.Collections;
using System.Collections.Generic;

namespace FishySteamworks
{
	// Token: 0x02000237 RID: 567
	public class BidirectionalDictionary<T1, T2> : IEnumerable
	{
		// Token: 0x17000257 RID: 599
		// (get) Token: 0x06001644 RID: 5700 RVA: 0x0005CF3C File Offset: 0x0005B13C
		public IEnumerable<T1> FirstTypes
		{
			get
			{
				return this.t1ToT2Dict.Keys;
			}
		}

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x06001645 RID: 5701 RVA: 0x0005CF49 File Offset: 0x0005B149
		public IEnumerable<T2> SecondTypes
		{
			get
			{
				return this.t2ToT1Dict.Keys;
			}
		}

		// Token: 0x06001646 RID: 5702 RVA: 0x0005CF56 File Offset: 0x0005B156
		public IEnumerator GetEnumerator()
		{
			return this.t1ToT2Dict.GetEnumerator();
		}

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x06001647 RID: 5703 RVA: 0x0005CF68 File Offset: 0x0005B168
		public int Count
		{
			get
			{
				return this.t1ToT2Dict.Count;
			}
		}

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x06001648 RID: 5704 RVA: 0x0005CF75 File Offset: 0x0005B175
		public Dictionary<T1, T2> First
		{
			get
			{
				return this.t1ToT2Dict;
			}
		}

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x06001649 RID: 5705 RVA: 0x0005CF7D File Offset: 0x0005B17D
		public Dictionary<T2, T1> Second
		{
			get
			{
				return this.t2ToT1Dict;
			}
		}

		// Token: 0x0600164A RID: 5706 RVA: 0x0005CF85 File Offset: 0x0005B185
		public void Add(T1 key, T2 value)
		{
			if (this.t1ToT2Dict.ContainsKey(key))
			{
				this.Remove(key);
			}
			this.t1ToT2Dict[key] = value;
			this.t2ToT1Dict[value] = key;
		}

		// Token: 0x0600164B RID: 5707 RVA: 0x0005CFB6 File Offset: 0x0005B1B6
		public void Add(T2 key, T1 value)
		{
			if (this.t2ToT1Dict.ContainsKey(key))
			{
				this.Remove(key);
			}
			this.t2ToT1Dict[key] = value;
			this.t1ToT2Dict[value] = key;
		}

		// Token: 0x0600164C RID: 5708 RVA: 0x0005CFE7 File Offset: 0x0005B1E7
		public T2 Get(T1 key)
		{
			return this.t1ToT2Dict[key];
		}

		// Token: 0x0600164D RID: 5709 RVA: 0x0005CFF5 File Offset: 0x0005B1F5
		public T1 Get(T2 key)
		{
			return this.t2ToT1Dict[key];
		}

		// Token: 0x0600164E RID: 5710 RVA: 0x0005D003 File Offset: 0x0005B203
		public bool TryGetValue(T1 key, out T2 value)
		{
			return this.t1ToT2Dict.TryGetValue(key, out value);
		}

		// Token: 0x0600164F RID: 5711 RVA: 0x0005D012 File Offset: 0x0005B212
		public bool TryGetValue(T2 key, out T1 value)
		{
			return this.t2ToT1Dict.TryGetValue(key, out value);
		}

		// Token: 0x06001650 RID: 5712 RVA: 0x0005D021 File Offset: 0x0005B221
		public bool Contains(T1 key)
		{
			return this.t1ToT2Dict.ContainsKey(key);
		}

		// Token: 0x06001651 RID: 5713 RVA: 0x0005D02F File Offset: 0x0005B22F
		public bool Contains(T2 key)
		{
			return this.t2ToT1Dict.ContainsKey(key);
		}

		// Token: 0x06001652 RID: 5714 RVA: 0x0005D040 File Offset: 0x0005B240
		public void Remove(T1 key)
		{
			if (this.Contains(key))
			{
				T2 t = this.t1ToT2Dict[key];
				this.t1ToT2Dict.Remove(key);
				this.t2ToT1Dict.Remove(t);
			}
		}

		// Token: 0x06001653 RID: 5715 RVA: 0x0005D080 File Offset: 0x0005B280
		public void Remove(T2 key)
		{
			if (this.Contains(key))
			{
				T1 t = this.t2ToT1Dict[key];
				this.t1ToT2Dict.Remove(t);
				this.t2ToT1Dict.Remove(key);
			}
		}

		// Token: 0x1700025C RID: 604
		public T1 this[T2 key]
		{
			get
			{
				return this.t2ToT1Dict[key];
			}
			set
			{
				this.Add(key, value);
			}
		}

		// Token: 0x1700025D RID: 605
		public T2 this[T1 key]
		{
			get
			{
				return this.t1ToT2Dict[key];
			}
			set
			{
				this.Add(key, value);
			}
		}

		// Token: 0x04000CE0 RID: 3296
		private Dictionary<T1, T2> t1ToT2Dict = new Dictionary<T1, T2>();

		// Token: 0x04000CE1 RID: 3297
		private Dictionary<T2, T1> t2ToT1Dict = new Dictionary<T2, T1>();
	}
}
