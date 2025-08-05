using System;
using UnityEngine;

// Token: 0x020001D4 RID: 468
public class WeaponHitDetection : MonoBehaviour
{
	// Token: 0x0600132A RID: 4906 RVA: 0x00050E79 File Offset: 0x0004F079
	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Ignorable"))
		{
			Debug.Log(other.gameObject);
			this.swerd.HitSubject = other.gameObject;
			this.swerd.isHitting = true;
		}
	}

	// Token: 0x0600132B RID: 4907 RVA: 0x00050EB0 File Offset: 0x0004F0B0
	private void OnTriggerExit(Collider other)
	{
		if (!other.CompareTag("Ignorable"))
		{
			this.swerd.isHitting = false;
			this.swerd.HitSubject = null;
		}
	}

	// Token: 0x04000B2B RID: 2859
	public SwordController swerd;
}
