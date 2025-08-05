using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace DrawPixels_Done
{
	// Token: 0x02000244 RID: 580
	public class DrawPixelsUI : MonoBehaviour
	{
		// Token: 0x060016EC RID: 5868 RVA: 0x0005F420 File Offset: 0x0005D620
		private void Awake()
		{
			base.transform.Find("SmallBtn").GetComponent<Button>().onClick.AddListener(delegate
			{
				this.dp1.Instance.SetCursorSize(DrawPixels.CursorSize.Small);
				this.dp2.Instance.SetCursorSize(DrawPixels.CursorSize.Small);
			});
			base.transform.Find("MediumBtn").GetComponent<Button>().onClick.AddListener(delegate
			{
				this.dp1.Instance.SetCursorSize(DrawPixels.CursorSize.Medium);
				this.dp2.Instance.SetCursorSize(DrawPixels.CursorSize.Medium);
			});
			base.transform.Find("LargeBtn").GetComponent<Button>().onClick.AddListener(delegate
			{
				this.dp1.Instance.SetCursorSize(DrawPixels.CursorSize.Large);
				this.dp2.Instance.SetCursorSize(DrawPixels.CursorSize.Large);
			});
			base.transform.Find("SaveBtn").GetComponent<Button>().onClick.AddListener(delegate
			{
				this.dp1.Instance.SaveImage(delegate(Texture2D texture2D)
				{
					base.transform.Find("RawImage").GetComponent<RawImage>().texture = texture2D;
					byte[] array = texture2D.EncodeToPNG();
					File.WriteAllBytes(Application.dataPath + this.FilePath, array);
				});
			});
			this.selectedColorImage = base.transform.Find("SelectedColor").GetComponent<Image>();
		}

		// Token: 0x060016ED RID: 5869 RVA: 0x0005F4F4 File Offset: 0x0005D6F4
		public void SaveFlags()
		{
			this.dp1.Instance.SaveImage(delegate(Texture2D texture2D)
			{
				base.transform.Find("RawImage").GetComponent<RawImage>().texture = texture2D;
				byte[] array = texture2D.EncodeToPNG();
				File.WriteAllBytes(Application.dataPath + this.FilePath, array);
				byte[] array2 = File.ReadAllBytes(Application.dataPath + this.FilePath);
				Texture2D texture2D2 = new Texture2D(2, 2);
				texture2D2.LoadImage(array2);
				texture2D2.Apply();
				this.flagmats[0].mainTexture = texture2D2;
			});
			this.dp2.Instance.SaveImage(delegate(Texture2D texture2D)
			{
				base.transform.Find("RawImage").GetComponent<RawImage>().texture = texture2D;
				byte[] array3 = texture2D.EncodeToPNG();
				File.WriteAllBytes(Application.dataPath + this.FilePath2, array3);
				byte[] array4 = File.ReadAllBytes(Application.dataPath + this.FilePath2);
				Texture2D texture2D3 = new Texture2D(2, 2);
				texture2D3.LoadImage(array4);
				texture2D3.Apply();
				this.flagmats[1].mainTexture = texture2D3;
			});
		}

		// Token: 0x060016EE RID: 5870 RVA: 0x0005F52E File Offset: 0x0005D72E
		private void Start()
		{
			this.dp1.Instance.OnColorChanged += this.DrawPixels_OnColorChanged;
			this.UpdateSelectedColor();
		}

		// Token: 0x060016EF RID: 5871 RVA: 0x0005F552 File Offset: 0x0005D752
		private void DrawPixels_OnColorChanged(object sender, EventArgs e)
		{
			this.UpdateSelectedColor();
		}

		// Token: 0x060016F0 RID: 5872 RVA: 0x0005F55C File Offset: 0x0005D75C
		private void UpdateSelectedColor()
		{
			Vector2 colorUV = this.dp1.Instance.GetColorUV();
			colorUV.x *= (float)this.colorsTexture.width;
			colorUV.y *= (float)this.colorsTexture.height;
			this.selectedColorImage.color = this.colorsTexture.GetPixel((int)colorUV.x, (int)colorUV.y);
		}

		// Token: 0x060016F1 RID: 5873 RVA: 0x0005F5CC File Offset: 0x0005D7CC
		private void SaveImageCamera(int width, int height)
		{
			RenderTexture renderTexture = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
			this.saveCamera.targetTexture = renderTexture;
			this.saveCamera.enabled = true;
			this.saveCamera.Render();
			RenderTexture.active = renderTexture;
			Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
			Rect rect = new Rect(0f, 0f, (float)width, (float)height);
			texture2D.ReadPixels(rect, 0, 0);
			texture2D.Apply();
			this.saveCamera.enabled = false;
			base.transform.Find("RawImage").GetComponent<RawImage>().texture = renderTexture;
			byte[] array = texture2D.EncodeToPNG();
			File.WriteAllBytes(Application.dataPath + "/Resources/PixelImage.png", array);
		}

		// Token: 0x04000D2C RID: 3372
		[SerializeField]
		private Texture2D colorsTexture;

		// Token: 0x04000D2D RID: 3373
		[SerializeField]
		private Camera saveCamera;

		// Token: 0x04000D2E RID: 3374
		private Image selectedColorImage;

		// Token: 0x04000D2F RID: 3375
		public string FilePath = "/Resources/PixelImage.png";

		// Token: 0x04000D30 RID: 3376
		public string FilePath2 = "/Resources/PixelImage1.png";

		// Token: 0x04000D31 RID: 3377
		public DrawPixels dp1;

		// Token: 0x04000D32 RID: 3378
		public DrawPixels dp2;

		// Token: 0x04000D33 RID: 3379
		public Material[] flagmats;
	}
}
