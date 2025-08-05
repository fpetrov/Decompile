using System;
using UnityEngine;

// Token: 0x0200018E RID: 398
public class showcheckmark : MonoBehaviour
{
	// Token: 0x060010CA RID: 4298 RVA: 0x00048BFC File Offset: 0x00046DFC
	public void ToggleCheck(bool val)
	{
		this.checkmark.SetActive(val);
	}

	// Token: 0x040009B6 RID: 2486
	public GameObject checkmark;
}
