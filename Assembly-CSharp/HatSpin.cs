using System;
using UnityEngine;

// Token: 0x020000AA RID: 170
public class HatSpin : MonoBehaviour
{
	// Token: 0x060006DE RID: 1758 RVA: 0x0001A78C File Offset: 0x0001898C
	private void Update()
	{
		base.transform.localRotation = Quaternion.Euler(base.transform.localRotation.eulerAngles.x, base.transform.localRotation.eulerAngles.y + this.spinamount * Time.deltaTime, 0f);
	}

	// Token: 0x060006DF RID: 1759 RVA: 0x0001A7EB File Offset: 0x000189EB
	public void DestroyMe()
	{
		Debug.Log("ieatgrass");
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04000369 RID: 873
	public float spinamount = 180f;
}
