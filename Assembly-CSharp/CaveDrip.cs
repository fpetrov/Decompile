using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000027 RID: 39
public class CaveDrip : MonoBehaviour
{
	// Token: 0x060001B0 RID: 432 RVA: 0x000084F6 File Offset: 0x000066F6
	private void Start()
	{
		base.StartCoroutine(this.DripSounds());
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x00008505 File Offset: 0x00006705
	private IEnumerator DripSounds()
	{
		for (;;)
		{
			this.asour.pitch = Random.Range(0.8f, 1.2f);
			this.asour.volume = Random.Range(0.03f, 0.1f);
			this.asour.PlayOneShot(this.drips[Random.Range(0, this.drips.Length)]);
			yield return new WaitForSeconds(2f);
		}
		yield break;
	}

	// Token: 0x040000DE RID: 222
	public AudioSource asour;

	// Token: 0x040000DF RID: 223
	public AudioClip[] drips;
}
