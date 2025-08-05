using System;
using UnityEngine;

// Token: 0x020000E5 RID: 229
public class MapCGFader : MonoBehaviour
{
	// Token: 0x06000978 RID: 2424 RVA: 0x00024D35 File Offset: 0x00022F35
	private void Start()
	{
		this.cg = base.GetComponent<CanvasGroup>();
	}

	// Token: 0x06000979 RID: 2425 RVA: 0x00024D43 File Offset: 0x00022F43
	public void FadeinShit()
	{
		this.fadeIn = true;
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x00024D4C File Offset: 0x00022F4C
	public void Fadeoutshit()
	{
		this.fadeIn = false;
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x00024D58 File Offset: 0x00022F58
	private void Update()
	{
		if (this.fadeIn)
		{
			this.cg.alpha = Mathf.Clamp01(this.elapsed / 0.25f);
			if (this.elapsed < 1f)
			{
				this.elapsed += Time.deltaTime;
				return;
			}
		}
		else if (this.elapsed > 0f)
		{
			this.elapsed -= Time.deltaTime * 4f;
			this.cg.alpha = Mathf.Clamp01(this.elapsed / 0.25f);
		}
	}

	// Token: 0x04000502 RID: 1282
	private CanvasGroup cg;

	// Token: 0x04000503 RID: 1283
	private bool fadeIn;

	// Token: 0x04000504 RID: 1284
	private float elapsed;
}
