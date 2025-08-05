using System;
using UnityEngine;

// Token: 0x020001AB RID: 427
public class SpineSyncer : MonoBehaviour
{
	// Token: 0x060011B7 RID: 4535 RVA: 0x0004BD6C File Offset: 0x00049F6C
	private void LateUpdate()
	{
		this.actuallerptarget = Mathf.Lerp(this.actuallerptarget, this.lerptarget, Time.deltaTime * 10f);
		base.transform.localRotation = Quaternion.Euler(this.actuallerptarget, 0f, 0f);
	}

	// Token: 0x04000A42 RID: 2626
	public float lerptarget;

	// Token: 0x04000A43 RID: 2627
	private float actuallerptarget;
}
