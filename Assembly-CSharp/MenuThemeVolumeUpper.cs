using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000E6 RID: 230
public class MenuThemeVolumeUpper : MonoBehaviour
{
	// Token: 0x0600097D RID: 2429 RVA: 0x00024DEA File Offset: 0x00022FEA
	private void Start()
	{
		base.StartCoroutine(this.UpVol());
	}

	// Token: 0x0600097E RID: 2430 RVA: 0x00024DF9 File Offset: 0x00022FF9
	private IEnumerator UpVol()
	{
		AudioSource asaucee = base.GetComponent<AudioSource>();
		yield return new WaitForSeconds(42f);
		float timer = 0f;
		float startvol = asaucee.volume;
		while ((double)timer < 0.5 && !this.fadeout)
		{
			yield return null;
			timer += Time.deltaTime;
			asaucee.volume = Mathf.Lerp(startvol, 0.2f, timer * 2f);
		}
		yield break;
	}

	// Token: 0x0600097F RID: 2431 RVA: 0x00024E08 File Offset: 0x00023008
	public void FadeOutMenuMusic()
	{
		this.fadeout = true;
		base.StartCoroutine(this.Fadeout());
	}

	// Token: 0x06000980 RID: 2432 RVA: 0x00024E1E File Offset: 0x0002301E
	private IEnumerator Fadeout()
	{
		AudioSource asaucee = base.GetComponent<AudioSource>();
		float timer = 0f;
		float startvol = asaucee.volume;
		while (timer < 4f)
		{
			yield return null;
			timer += Time.deltaTime;
			asaucee.volume = Mathf.Lerp(startvol, 0f, timer / 4f);
		}
		yield break;
	}

	// Token: 0x04000505 RID: 1285
	private bool fadeout;
}
