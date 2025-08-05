using System;
using System.IO;
using UnityEngine;

// Token: 0x02000088 RID: 136
public class PixelArtDrawingSystem : MonoBehaviour
{
	// Token: 0x060005B8 RID: 1464 RVA: 0x0001661F File Offset: 0x0001481F
	private void Awake()
	{
		this.grid = new Grid<PixelArtDrawingSystem.GridObject>(128, 128, 1f, Vector3.zero, (Grid<PixelArtDrawingSystem.GridObject> g, int x, int y) => new PixelArtDrawingSystem.GridObject(g, x, y));
	}

	// Token: 0x060005B9 RID: 1465 RVA: 0x0001665F File Offset: 0x0001485F
	private void Start()
	{
		this.pixelArtDrawingSystemVisual.SetGrid(this.grid);
	}

	// Token: 0x060005BA RID: 1466 RVA: 0x00016674 File Offset: 0x00014874
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mouseWorldPosition = PixelArtDrawingSystem.GetMouseWorldPosition();
			Debug.Log(this.grid.GetGridObject(mouseWorldPosition).GetColorUV());
			this.grid.GetGridObject(mouseWorldPosition).SetColorUV(this.colorUV);
			Debug.Log(this.grid.GetGridObject(mouseWorldPosition).GetColorUV());
		}
		if (Input.GetKeyDown(KeyCode.T))
		{
			this.colorUV = new Vector2(0f, 1f);
		}
		if (Input.GetKeyDown(KeyCode.Y))
		{
			this.colorUV = new Vector2(0.3f, 1f);
		}
		if (Input.GetKeyDown(KeyCode.U))
		{
			this.colorUV = new Vector2(0f, 0f);
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			this.SaveImage();
		}
	}

	// Token: 0x060005BB RID: 1467 RVA: 0x00016748 File Offset: 0x00014948
	private void SaveImage()
	{
		Texture2D texture2D = new Texture2D(this.grid.GetWidth(), this.grid.GetHeight(), TextureFormat.ARGB32, false);
		for (int i = 0; i < this.grid.GetWidth(); i++)
		{
			for (int j = 0; j < this.grid.GetHeight(); j++)
			{
				PixelArtDrawingSystem.GridObject gridObject = this.grid.GetGridObject(i, j);
				int num = (int)(gridObject.GetColorUV().x * (float)this.colorTexture2D.width);
				int num2 = (int)(gridObject.GetColorUV().y * (float)this.colorTexture2D.height);
				Color pixel = this.colorTexture2D.GetPixel(num, num2);
				texture2D.SetPixel(i, j, pixel);
			}
		}
		texture2D.Apply();
		byte[] array = texture2D.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/draw/pixelArt.png", array);
	}

	// Token: 0x060005BC RID: 1468 RVA: 0x00016824 File Offset: 0x00014A24
	public static Vector3 GetMouseWorldPosition()
	{
		Vector3 mouseWorldPositionWithZ = PixelArtDrawingSystem.GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
		mouseWorldPositionWithZ.z = 0f;
		return mouseWorldPositionWithZ;
	}

	// Token: 0x060005BD RID: 1469 RVA: 0x0001684E File Offset: 0x00014A4E
	public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
	{
		return worldCamera.ScreenToWorldPoint(screenPosition);
	}

	// Token: 0x040002D1 RID: 721
	[SerializeField]
	private PixelArtDrawingSystemVisual pixelArtDrawingSystemVisual;

	// Token: 0x040002D2 RID: 722
	[SerializeField]
	private Texture2D colorTexture2D;

	// Token: 0x040002D3 RID: 723
	private Grid<PixelArtDrawingSystem.GridObject> grid;

	// Token: 0x040002D4 RID: 724
	private Vector2 colorUV = new Vector2(0f, 0f);

	// Token: 0x02000089 RID: 137
	public class GridObject
	{
		// Token: 0x060005BF RID: 1471 RVA: 0x00016874 File Offset: 0x00014A74
		public GridObject(Grid<PixelArtDrawingSystem.GridObject> grid, int x, int y)
		{
			this.grid = grid;
			this.x = x;
			this.y = y;
		}

		// Token: 0x060005C0 RID: 1472 RVA: 0x00016891 File Offset: 0x00014A91
		public void SetColorUV(Vector2 colorUV)
		{
			this.colorUV = colorUV;
			this.grid.TriggerGridObjectChanged(this.x, this.y);
		}

		// Token: 0x060005C1 RID: 1473 RVA: 0x000168B1 File Offset: 0x00014AB1
		public Vector2 GetColorUV()
		{
			return this.colorUV;
		}

		// Token: 0x060005C2 RID: 1474 RVA: 0x000168BC File Offset: 0x00014ABC
		public override string ToString()
		{
			return ((int)this.colorUV.x).ToString();
		}

		// Token: 0x040002D5 RID: 725
		private Grid<PixelArtDrawingSystem.GridObject> grid;

		// Token: 0x040002D6 RID: 726
		private int x;

		// Token: 0x040002D7 RID: 727
		private int y;

		// Token: 0x040002D8 RID: 728
		private Vector2 colorUV;
	}
}
