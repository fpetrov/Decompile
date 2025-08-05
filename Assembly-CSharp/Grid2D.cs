using System;
using UnityEngine;

// Token: 0x02000207 RID: 519
public class Grid2D<T>
{
	// Token: 0x1700021F RID: 543
	// (get) Token: 0x060014BF RID: 5311 RVA: 0x0005736F File Offset: 0x0005556F
	// (set) Token: 0x060014C0 RID: 5312 RVA: 0x00057377 File Offset: 0x00055577
	public Vector2Int Size { get; private set; }

	// Token: 0x17000220 RID: 544
	// (get) Token: 0x060014C1 RID: 5313 RVA: 0x00057380 File Offset: 0x00055580
	// (set) Token: 0x060014C2 RID: 5314 RVA: 0x00057388 File Offset: 0x00055588
	public Vector2Int Offset { get; set; }

	// Token: 0x060014C3 RID: 5315 RVA: 0x00057391 File Offset: 0x00055591
	public Grid2D(Vector2Int size, Vector2Int offset)
	{
		this.Size = size;
		this.Offset = offset;
		this.data = new T[size.x * size.y];
	}

	// Token: 0x060014C4 RID: 5316 RVA: 0x000573C4 File Offset: 0x000555C4
	public int GetIndex(Vector2Int pos)
	{
		return pos.x + this.Size.x * pos.y;
	}

	// Token: 0x060014C5 RID: 5317 RVA: 0x000573F0 File Offset: 0x000555F0
	public bool InBounds(Vector2Int pos)
	{
		return new RectInt(Vector2Int.zero, this.Size).Contains(pos + this.Offset);
	}

	// Token: 0x17000221 RID: 545
	public T this[int x, int y]
	{
		get
		{
			return this[new Vector2Int(x, y)];
		}
		set
		{
			this[new Vector2Int(x, y)] = value;
		}
	}

	// Token: 0x17000222 RID: 546
	public T this[Vector2Int pos]
	{
		get
		{
			pos += this.Offset;
			return this.data[this.GetIndex(pos)];
		}
		set
		{
			pos += this.Offset;
			this.data[this.GetIndex(pos)] = value;
		}
	}

	// Token: 0x04000C26 RID: 3110
	private T[] data;
}
