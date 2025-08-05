using System;
using UnityEngine;

// Token: 0x020000AB RID: 171
public class HealthBarLerper : MonoBehaviour
{
	// Token: 0x060006E1 RID: 1761 RVA: 0x0001A818 File Offset: 0x00018A18
	private void Update()
	{
		base.transform.localScale = new Vector3(base.transform.localScale.x, Mathf.Lerp(base.transform.localScale.y, this.lerper, Time.deltaTime * 10f), base.transform.localScale.z);
	}

	// Token: 0x0400036A RID: 874
	public float lerper = 10f;
}
