using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001A9 RID: 425
public class SpikeGrower : MonoBehaviour
{
	// Token: 0x060011AE RID: 4526 RVA: 0x0004BBAD File Offset: 0x00049DAD
	private void Start()
	{
		base.StartCoroutine(this.MoveSpike());
	}

	// Token: 0x060011AF RID: 4527 RVA: 0x0004BBBC File Offset: 0x00049DBC
	private IEnumerator MoveSpike()
	{
		float timer = 0f;
		while (base.transform.localPosition.y < 0f)
		{
			base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, new Vector3(base.transform.localPosition.x, 0.2f, base.transform.localPosition.z), Time.deltaTime);
			yield return null;
		}
		while (timer < 60f)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		while (base.transform.localPosition.y > -1f)
		{
			base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, new Vector3(base.transform.localPosition.x, -1.2f, base.transform.localPosition.z), Time.deltaTime);
			yield return null;
		}
		Object.Destroy(base.transform.parent.gameObject);
		yield break;
	}
}
