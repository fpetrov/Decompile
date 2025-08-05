using System;
using UnityEngine;

// Token: 0x0200005A RID: 90
public class PixelArtDrawingSystemVisual : MonoBehaviour
{
	// Token: 0x0600039B RID: 923 RVA: 0x0000EBEE File Offset: 0x0000CDEE
	private void Awake()
	{
		this.mesh = new Mesh();
		base.GetComponent<MeshFilter>().mesh = this.mesh;
	}

	// Token: 0x0600039C RID: 924 RVA: 0x0000EC0C File Offset: 0x0000CE0C
	public void SetGrid(Grid<PixelArtDrawingSystem.GridObject> grid)
	{
		this.grid = grid;
		this.UpdateVisual();
		grid.OnGridObjectChanged += this.Grid_OnGridValueChanged;
	}

	// Token: 0x0600039D RID: 925 RVA: 0x0000EC2D File Offset: 0x0000CE2D
	private void Grid_OnGridValueChanged(object sender, Grid<PixelArtDrawingSystem.GridObject>.OnGridObjectChangedEventArgs e)
	{
		this.updateMesh = true;
	}

	// Token: 0x0600039E RID: 926 RVA: 0x0000EC36 File Offset: 0x0000CE36
	private void LateUpdate()
	{
		if (this.updateMesh)
		{
			this.updateMesh = false;
			this.UpdateVisual();
		}
	}

	// Token: 0x0600039F RID: 927 RVA: 0x0000EC50 File Offset: 0x0000CE50
	private void UpdateVisual()
	{
		Vector3[] array;
		Vector2[] array2;
		int[] array3;
		PixelArtDrawingSystemVisual.CreateEmptyMeshArrays(this.grid.GetWidth() * this.grid.GetHeight(), out array, out array2, out array3);
		for (int i = 0; i < this.grid.GetWidth(); i++)
		{
			for (int j = 0; j < this.grid.GetHeight(); j++)
			{
				int num = i * this.grid.GetHeight() + j;
				Vector3 vector = new Vector3(1f, 1f) * this.grid.GetCellSize();
				PixelArtDrawingSystem.GridObject gridObject = this.grid.GetGridObject(i, j);
				Vector2 colorUV = gridObject.GetColorUV();
				Vector2 colorUV2 = gridObject.GetColorUV();
				PixelArtDrawingSystemVisual.AddToMeshArrays(array, array2, array3, num, this.grid.GetWorldPosition(i, j) + vector * 0.5f, 0f, vector, colorUV, colorUV2);
			}
		}
		this.mesh.vertices = array;
		this.mesh.uv = array2;
		this.mesh.triangles = array3;
	}

	// Token: 0x060003A0 RID: 928 RVA: 0x0000ED63 File Offset: 0x0000CF63
	public static void CreateEmptyMeshArrays(int quadCount, out Vector3[] vertices, out Vector2[] uvs, out int[] triangles)
	{
		vertices = new Vector3[4 * quadCount];
		uvs = new Vector2[4 * quadCount];
		triangles = new int[6 * quadCount];
	}

	// Token: 0x060003A1 RID: 929 RVA: 0x0000ED84 File Offset: 0x0000CF84
	public static void AddToMeshArrays(Vector3[] vertices, Vector2[] uvs, int[] triangles, int index, Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11)
	{
		int num2;
		int num = (num2 = index * 4);
		int num3 = num + 1;
		int num4 = num + 2;
		int num5 = num + 3;
		baseSize *= 0.5f;
		if (baseSize.x != baseSize.y)
		{
			vertices[num2] = pos + PixelArtDrawingSystemVisual.GetQuaternionEuler(rot) * new Vector3(-baseSize.x, baseSize.y);
			vertices[num3] = pos + PixelArtDrawingSystemVisual.GetQuaternionEuler(rot) * new Vector3(-baseSize.x, -baseSize.y);
			vertices[num4] = pos + PixelArtDrawingSystemVisual.GetQuaternionEuler(rot) * new Vector3(baseSize.x, -baseSize.y);
			vertices[num5] = pos + PixelArtDrawingSystemVisual.GetQuaternionEuler(rot) * baseSize;
		}
		else
		{
			vertices[num2] = pos + PixelArtDrawingSystemVisual.GetQuaternionEuler(rot - 270f) * baseSize;
			vertices[num3] = pos + PixelArtDrawingSystemVisual.GetQuaternionEuler(rot - 180f) * baseSize;
			vertices[num4] = pos + PixelArtDrawingSystemVisual.GetQuaternionEuler(rot - 90f) * baseSize;
			vertices[num5] = pos + PixelArtDrawingSystemVisual.GetQuaternionEuler(rot - 0f) * baseSize;
		}
		uvs[num2] = new Vector2(uv00.x, uv11.y);
		uvs[num3] = new Vector2(uv00.x, uv00.y);
		uvs[num4] = new Vector2(uv11.x, uv00.y);
		uvs[num5] = new Vector2(uv11.x, uv11.y);
		int num6 = index * 6;
		triangles[num6] = num2;
		triangles[num6 + 1] = num5;
		triangles[num6 + 2] = num3;
		triangles[num6 + 3] = num3;
		triangles[num6 + 4] = num5;
		triangles[num6 + 5] = num4;
	}

	// Token: 0x060003A2 RID: 930 RVA: 0x0000EF90 File Offset: 0x0000D190
	private static void CacheQuaternionEuler()
	{
		if (PixelArtDrawingSystemVisual.cachedQuaternionEulerArr != null)
		{
			return;
		}
		PixelArtDrawingSystemVisual.cachedQuaternionEulerArr = new Quaternion[360];
		for (int i = 0; i < 360; i++)
		{
			PixelArtDrawingSystemVisual.cachedQuaternionEulerArr[i] = Quaternion.Euler(0f, 0f, (float)i);
		}
	}

	// Token: 0x060003A3 RID: 931 RVA: 0x0000EFE0 File Offset: 0x0000D1E0
	private static Quaternion GetQuaternionEuler(float rotFloat)
	{
		int num = Mathf.RoundToInt(rotFloat);
		num %= 360;
		if (num < 0)
		{
			num += 360;
		}
		if (PixelArtDrawingSystemVisual.cachedQuaternionEulerArr == null)
		{
			PixelArtDrawingSystemVisual.CacheQuaternionEuler();
		}
		return PixelArtDrawingSystemVisual.cachedQuaternionEulerArr[num];
	}

	// Token: 0x040001D3 RID: 467
	private Grid<PixelArtDrawingSystem.GridObject> grid;

	// Token: 0x040001D4 RID: 468
	private Mesh mesh;

	// Token: 0x040001D5 RID: 469
	private bool updateMesh;

	// Token: 0x040001D6 RID: 470
	private static Quaternion[] cachedQuaternionEulerArr;
}
