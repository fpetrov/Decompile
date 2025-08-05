using System;
using UnityEngine;

// Token: 0x02000057 RID: 87
public class Grid<TGridObject>
{
	// Token: 0x14000001 RID: 1
	// (add) Token: 0x0600038B RID: 907 RVA: 0x0000E838 File Offset: 0x0000CA38
	// (remove) Token: 0x0600038C RID: 908 RVA: 0x0000E870 File Offset: 0x0000CA70
	public event EventHandler<Grid<TGridObject>.OnGridObjectChangedEventArgs> OnGridObjectChanged;

	// Token: 0x0600038D RID: 909 RVA: 0x0000E8A8 File Offset: 0x0000CAA8
	public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
	{
		Grid<TGridObject> <>4__this = this;
		this.width = width;
		this.height = height;
		this.cellSize = cellSize;
		this.originPosition = originPosition;
		this.gridArray = new TGridObject[width, height];
		for (int i = 0; i < this.gridArray.GetLength(0); i++)
		{
			for (int j = 0; j < this.gridArray.GetLength(1); j++)
			{
				this.gridArray[i, j] = createGridObject(this, i, j);
			}
		}
		if (false)
		{
			TextMesh[,] debugTextArray = new TextMesh[width, height];
			for (int k = 0; k < this.gridArray.GetLength(0); k++)
			{
				for (int l = 0; l < this.gridArray.GetLength(1); l++)
				{
					Debug.DrawLine(this.GetWorldPosition(k, l), this.GetWorldPosition(k, l + 1), Color.white, 100f);
					Debug.DrawLine(this.GetWorldPosition(k, l), this.GetWorldPosition(k + 1, l), Color.white, 100f);
				}
			}
			Debug.DrawLine(this.GetWorldPosition(0, height), this.GetWorldPosition(width, height), Color.white, 100f);
			Debug.DrawLine(this.GetWorldPosition(width, 0), this.GetWorldPosition(width, height), Color.white, 100f);
			this.OnGridObjectChanged += delegate(object sender, Grid<TGridObject>.OnGridObjectChangedEventArgs eventArgs)
			{
				TextMesh textMesh = debugTextArray[eventArgs.x, eventArgs.y];
				ref TGridObject ptr2;
				ref TGridObject ptr = (ptr2 = ref <>4__this.gridArray[eventArgs.x, eventArgs.y]);
				TGridObject tgridObject = default(TGridObject);
				string text;
				if (tgridObject == null)
				{
					tgridObject = ptr;
					ptr2 = ref tgridObject;
					if (tgridObject == null)
					{
						text = null;
						goto IL_0064;
					}
				}
				text = ptr2.ToString();
				IL_0064:
				textMesh.text = text;
			};
		}
	}

	// Token: 0x0600038E RID: 910 RVA: 0x0000EA12 File Offset: 0x0000CC12
	public int GetWidth()
	{
		return this.width;
	}

	// Token: 0x0600038F RID: 911 RVA: 0x0000EA1A File Offset: 0x0000CC1A
	public int GetHeight()
	{
		return this.height;
	}

	// Token: 0x06000390 RID: 912 RVA: 0x0000EA22 File Offset: 0x0000CC22
	public float GetCellSize()
	{
		return this.cellSize;
	}

	// Token: 0x06000391 RID: 913 RVA: 0x0000EA2A File Offset: 0x0000CC2A
	public Vector3 GetWorldPosition(int x, int y)
	{
		return new Vector3((float)x, (float)y) * this.cellSize + this.originPosition;
	}

	// Token: 0x06000392 RID: 914 RVA: 0x0000EA4B File Offset: 0x0000CC4B
	public void GetXY(Vector3 worldPosition, out int x, out int y)
	{
		x = Mathf.FloorToInt((worldPosition - this.originPosition).x / this.cellSize);
		y = Mathf.FloorToInt((worldPosition - this.originPosition).y / this.cellSize);
	}

	// Token: 0x06000393 RID: 915 RVA: 0x0000EA8B File Offset: 0x0000CC8B
	public void SetGridObject(int x, int y, TGridObject value)
	{
		if (x >= 0 && y >= 0 && x < this.width && y < this.height)
		{
			this.gridArray[x, y] = value;
			this.TriggerGridObjectChanged(x, y);
		}
	}

	// Token: 0x06000394 RID: 916 RVA: 0x0000EABD File Offset: 0x0000CCBD
	public void TriggerGridObjectChanged(int x, int y)
	{
		EventHandler<Grid<TGridObject>.OnGridObjectChangedEventArgs> onGridObjectChanged = this.OnGridObjectChanged;
		if (onGridObjectChanged == null)
		{
			return;
		}
		onGridObjectChanged(this, new Grid<TGridObject>.OnGridObjectChangedEventArgs
		{
			x = x,
			y = y
		});
	}

	// Token: 0x06000395 RID: 917 RVA: 0x0000EAE4 File Offset: 0x0000CCE4
	public void SetGridObject(Vector3 worldPosition, TGridObject value)
	{
		int num;
		int num2;
		this.GetXY(worldPosition, out num, out num2);
		this.SetGridObject(num, num2, value);
	}

	// Token: 0x06000396 RID: 918 RVA: 0x0000EB08 File Offset: 0x0000CD08
	public TGridObject GetGridObject(int x, int y)
	{
		if (x >= 0 && y >= 0 && x < this.width && y < this.height)
		{
			return this.gridArray[x, y];
		}
		return default(TGridObject);
	}

	// Token: 0x06000397 RID: 919 RVA: 0x0000EB48 File Offset: 0x0000CD48
	public TGridObject GetGridObject(Vector3 worldPosition)
	{
		int num;
		int num2;
		this.GetXY(worldPosition, out num, out num2);
		return this.GetGridObject(num, num2);
	}

	// Token: 0x040001CA RID: 458
	private int width;

	// Token: 0x040001CB RID: 459
	private int height;

	// Token: 0x040001CC RID: 460
	private float cellSize;

	// Token: 0x040001CD RID: 461
	private Vector3 originPosition;

	// Token: 0x040001CE RID: 462
	private TGridObject[,] gridArray;

	// Token: 0x02000058 RID: 88
	public class OnGridObjectChangedEventArgs : EventArgs
	{
		// Token: 0x040001CF RID: 463
		public int x;

		// Token: 0x040001D0 RID: 464
		public int y;
	}
}
