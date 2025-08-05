using System;
using UnityEngine;

// Token: 0x02000080 RID: 128
public class FireAudioPlayer : MonoBehaviour
{
	// Token: 0x0600056C RID: 1388 RVA: 0x00014F9C File Offset: 0x0001319C
	private void Update()
	{
		if (this.fire.isPlaying)
		{
			this.sauce.volume = Mathf.Clamp01(this.sauce.volume + Time.deltaTime);
			return;
		}
		this.sauce.volume = Mathf.Clamp01(this.sauce.volume - Time.deltaTime * 4f);
	}

	// Token: 0x040002A5 RID: 677
	public ParticleSystem fire;

	// Token: 0x040002A6 RID: 678
	public AudioSource sauce;
}
