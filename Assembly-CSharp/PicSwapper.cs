using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x02000107 RID: 263
public class PicSwapper : MonoBehaviour
{
	// Token: 0x06000A97 RID: 2711 RVA: 0x00027D9F File Offset: 0x00025F9F
	private void Start()
	{
		base.StartCoroutine(this.SwapIcon());
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x00027DAE File Offset: 0x00025FAE
	private IEnumerator SwapIcon()
	{
		while (base.isActiveAndEnabled)
		{
			int num = Random.Range(0, this.texts.Length);
			this.text.material.mainTexture = this.texts[num];
			this.icon.material.mainTexture = this.icons[num];
			yield return new WaitForSeconds(35f);
		}
		yield break;
	}

	// Token: 0x040005A1 RID: 1441
	public DecalProjector text;

	// Token: 0x040005A2 RID: 1442
	public DecalProjector icon;

	// Token: 0x040005A3 RID: 1443
	public Texture2D[] texts;

	// Token: 0x040005A4 RID: 1444
	public Texture2D[] icons;
}
