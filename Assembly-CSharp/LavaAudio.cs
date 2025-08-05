using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000B6 RID: 182
public class LavaAudio : MonoBehaviour
{
	// Token: 0x0600071D RID: 1821 RVA: 0x0001B44B File Offset: 0x0001964B
	private void Update()
	{
		if (!this.routstarted)
		{
			this.routstarted = true;
			base.StartCoroutine(this.LavaSounds());
		}
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x0001B469 File Offset: 0x00019669
	private IEnumerator LavaSounds()
	{
		for (;;)
		{
			yield return new WaitForSeconds(Random.Range(2f, 20f));
			this.LavaSources.volume = Random.Range(0.01f, 0.15f);
			this.LavaSources.pitch = Random.Range(0.8f, 1.2f);
			this.LavaSources.PlayOneShot(this.LavaClips);
		}
		yield break;
	}

	// Token: 0x04000396 RID: 918
	public AudioSource LavaSources;

	// Token: 0x04000397 RID: 919
	public AudioClip LavaClips;

	// Token: 0x04000398 RID: 920
	private bool routstarted;
}
