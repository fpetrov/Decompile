using System;
using UnityEngine;

// Token: 0x020001BA RID: 442
public class TorchHitDetection : MonoBehaviour
{
	// Token: 0x06001258 RID: 4696 RVA: 0x0004DA50 File Offset: 0x0004BC50
	private void OnTriggerEnter(Collider other)
	{
		this.toerch.HitSubject = other.gameObject;
	}

	// Token: 0x06001259 RID: 4697 RVA: 0x0004DA63 File Offset: 0x0004BC63
	private void OnTriggerExit(Collider other)
	{
		this.toerch.HitSubject = null;
	}

	// Token: 0x04000A9C RID: 2716
	public TorchController toerch;
}
