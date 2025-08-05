using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200015C RID: 348
public class RiverPitchRandomizer : MonoBehaviour
{
	// Token: 0x06000EDF RID: 3807 RVA: 0x0003C1C7 File Offset: 0x0003A3C7
	private void Start()
	{
		base.StartCoroutine(this.RandPitch());
	}

	// Token: 0x06000EE0 RID: 3808 RVA: 0x0003C1D6 File Offset: 0x0003A3D6
	private IEnumerator RandPitch()
	{
		for (;;)
		{
			foreach (AudioSource aso in this.RiverSources)
			{
				this.timer = 0f;
				float newpitch = Random.Range(0.8f, 1.1f);
				float startpitch = aso.pitch;
				while (this.timer < 1f)
				{
					aso.pitch = Mathf.Lerp(startpitch, newpitch, this.timer);
					this.timer += Time.deltaTime;
					yield return null;
				}
				aso = null;
			}
			AudioSource[] array = null;
			yield return new WaitForSeconds((float)Random.Range(10, 30));
		}
		yield break;
	}

	// Token: 0x04000829 RID: 2089
	public AudioSource[] RiverSources;

	// Token: 0x0400082A RID: 2090
	private float timer;
}
