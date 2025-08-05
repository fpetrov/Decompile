using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000EE RID: 238
public class MushroomBottomBoinger : MonoBehaviour
{
	// Token: 0x060009B1 RID: 2481 RVA: 0x0002559E File Offset: 0x0002379E
	private void OnTriggerEnter(Collider other)
	{
		this.bmm.TriggerBottomBounceMush(this.mushid);
	}

	// Token: 0x060009B2 RID: 2482 RVA: 0x000255B1 File Offset: 0x000237B1
	public void Dotheboing()
	{
		base.StartCoroutine(this.yumproutine());
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x000255C0 File Offset: 0x000237C0
	private IEnumerator yumproutine()
	{
		this.asource.PlayOneShot(this.mushclips[Random.Range(0, this.mushclips.Length)]);
		this.mushani.SetBool("yump", true);
		yield return new WaitForSeconds(0.1f);
		this.mushani.SetBool("yump", false);
		yield break;
	}

	// Token: 0x04000522 RID: 1314
	public AudioSource asource;

	// Token: 0x04000523 RID: 1315
	public AudioClip[] mushclips;

	// Token: 0x04000524 RID: 1316
	public int mushid;

	// Token: 0x04000525 RID: 1317
	public BounceMushroomManager bmm;

	// Token: 0x04000526 RID: 1318
	public Animator mushani;
}
