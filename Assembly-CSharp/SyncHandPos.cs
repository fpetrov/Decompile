using System;
using UnityEngine;

// Token: 0x020001B2 RID: 434
public class SyncHandPos : MonoBehaviour
{
	// Token: 0x06001210 RID: 4624 RVA: 0x0004CE9D File Offset: 0x0004B09D
	private void LateUpdate()
	{
		base.transform.position = this.HandPos.position;
		base.transform.rotation = this.HandPos.rotation;
	}

	// Token: 0x04000A70 RID: 2672
	public Transform HandPos;
}
