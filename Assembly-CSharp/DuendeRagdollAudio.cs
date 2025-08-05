using System;
using UnityEngine;

// Token: 0x02000064 RID: 100
public class DuendeRagdollAudio : MonoBehaviour
{
	// Token: 0x06000437 RID: 1079 RVA: 0x00011078 File Offset: 0x0000F278
	private void Start()
	{
		this.asource.pitch = Random.Range(0.9f, 1.1f);
		this.asource.volume = Random.Range(0.5f, 0.6f);
		this.asource.PlayOneShot(this.clips[Random.Range(0, 1)]);
	}

	// Token: 0x04000210 RID: 528
	public AudioSource asource;

	// Token: 0x04000211 RID: 529
	public AudioClip[] clips;
}
