using System;
using UnityEngine;

// Token: 0x020000F1 RID: 241
public class MushroomManTrigger : MonoBehaviour
{
	// Token: 0x060009DD RID: 2525 RVA: 0x00025E85 File Offset: 0x00024085
	private void OnTriggerEnter(Collider other)
	{
		this.mgc.PlayMGSound();
	}

	// Token: 0x04000539 RID: 1337
	public MushroomGuyController mgc;
}
