using System;
using UnityEngine;

namespace DrawPixels_Done
{
	// Token: 0x02000245 RID: 581
	public class DrawPixelsVisual : MonoBehaviour
	{
		// Token: 0x060016FA RID: 5882 RVA: 0x0005F86B File Offset: 0x0005DA6B
		private void Awake()
		{
			this.mesh = new Mesh();
			base.GetComponent<MeshFilter>().mesh = this.mesh;
		}

		// Token: 0x060016FB RID: 5883 RVA: 0x0005F889 File Offset: 0x0005DA89
		private void Start()
		{
			this.SetGrid(this.drawPixels.GetGrid());
		}

		// Token: 0x060016FC RID: 5884 RVA: 0x0005F89C File Offset: 0x0005DA9C
		private void SetGrid(Grid<DrawPixels.PixelGridObject> grid)
		{
			this.grid = grid;
			this.UpdateVisual();
			grid.OnGridObjectChanged += this.Grid_OnGridValueChanged;
		}

		// Token: 0x060016FD RID: 5885 RVA: 0x0005F8BD File Offset: 0x0005DABD
		private void Grid_OnGridValueChanged(object sender, Grid<DrawPixels.PixelGridObject>.OnGridObjectChangedEventArgs e)
		{
			this.updateMesh = true;
		}

		// Token: 0x060016FE RID: 5886 RVA: 0x0005F8C6 File Offset: 0x0005DAC6
		private void LateUpdate()
		{
			if (this.updateMesh)
			{
				this.updateMesh = false;
				this.UpdateVisual();
			}
		}

		// Token: 0x060016FF RID: 5887 RVA: 0x0005F8E0 File Offset: 0x0005DAE0
		private void UpdateVisual()
		{
			Vector3[] array;
			Vector2[] array2;
			int[] array3;
			DrawPixelsVisual.CreateEmptyMeshArrays(this.grid.GetWidth() * this.grid.GetHeight(), out array, out array2, out array3);
			for (int i = 0; i < this.grid.GetWidth(); i++)
			{
				for (int j = 0; j < this.grid.GetHeight(); j++)
				{
					int num = i * this.grid.GetHeight() + j;
					DrawPixels.PixelGridObject gridObject = this.grid.GetGridObject(i, j);
					Vector3 vector = new Vector3(1f, 1f) * this.grid.GetCellSize();
					Vector2 colorUV = gridObject.GetColorUV();
					Vector2 colorUV2 = gridObject.GetColorUV();
					DrawPixelsVisual.AddToMeshArrays(array, array2, array3, num, this.grid.GetWorldPosition(i, j) + vector * 0.5f, 0f, vector, colorUV, colorUV2);
				}
			}
			this.mesh.vertices = array;
			this.mesh.uv = array2;
			this.mesh.triangles = array3;
		}

		// Token: 0x06001700 RID: 5888 RVA: 0x0000ED63 File Offset: 0x0000CF63
		public static void CreateEmptyMeshArrays(int quadCount, out Vector3[] vertices, out Vector2[] uvs, out int[] triangles)
		{
			vertices = new Vector3[4 * quadCount];
			uvs = new Vector2[4 * quadCount];
			triangles = new int[6 * quadCount];
		}

		// Token: 0x06001701 RID: 5889 RVA: 0x0005F9F4 File Offset: 0x0005DBF4
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
				vertices[num2] = pos + DrawPixelsVisual.GetQuaternionEuler(rot) * new Vector3(-baseSize.x, baseSize.y);
				vertices[num3] = pos + DrawPixelsVisual.GetQuaternionEuler(rot) * new Vector3(-baseSize.x, -baseSize.y);
				vertices[num4] = pos + DrawPixelsVisual.GetQuaternionEuler(rot) * new Vector3(baseSize.x, -baseSize.y);
				vertices[num5] = pos + DrawPixelsVisual.GetQuaternionEuler(rot) * baseSize;
			}
			else
			{
				vertices[num2] = pos + DrawPixelsVisual.GetQuaternionEuler(rot - 270f) * baseSize;
				vertices[num3] = pos + DrawPixelsVisual.GetQuaternionEuler(rot - 180f) * baseSize;
				vertices[num4] = pos + DrawPixelsVisual.GetQuaternionEuler(rot - 90f) * baseSize;
				vertices[num5] = pos + DrawPixelsVisual.GetQuaternionEuler(rot - 0f) * baseSize;
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

		// Token: 0x06001702 RID: 5890 RVA: 0x0005FC00 File Offset: 0x0005DE00
		private static void CacheQuaternionEuler()
		{
			if (DrawPixelsVisual.cachedQuaternionEulerArr != null)
			{
				return;
			}
			DrawPixelsVisual.cachedQuaternionEulerArr = new Quaternion[360];
			for (int i = 0; i < 360; i++)
			{
				DrawPixelsVisual.cachedQuaternionEulerArr[i] = Quaternion.Euler(0f, 0f, (float)i);
			}
		}

		// Token: 0x06001703 RID: 5891 RVA: 0x0005FC50 File Offset: 0x0005DE50
		private static Quaternion GetQuaternionEuler(float rotFloat)
		{
			int num = Mathf.RoundToInt(rotFloat);
			num %= 360;
			if (num < 0)
			{
				num += 360;
			}
			if (DrawPixelsVisual.cachedQuaternionEulerArr == null)
			{
				DrawPixelsVisual.CacheQuaternionEuler();
			}
			return DrawPixelsVisual.cachedQuaternionEulerArr[num];
		}

		// Token: 0x04000D34 RID: 3380
		[SerializeField]
		private DrawPixels drawPixels;

		// Token: 0x04000D35 RID: 3381
		private Grid<DrawPixels.PixelGridObject> grid;

		// Token: 0x04000D36 RID: 3382
		private Mesh mesh;

		// Token: 0x04000D37 RID: 3383
		private bool updateMesh;

		// Token: 0x04000D38 RID: 3384
		private static Quaternion[] cachedQuaternionEulerArr;
	}
}
