using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200015A RID: 346
public class RespawnWormhole : MonoBehaviour
{
	// Token: 0x06000ED6 RID: 3798 RVA: 0x0003BFD0 File Offset: 0x0003A1D0
	public void resizeWormhole()
	{
		base.transform.position = this.spawnpos.position;
		base.transform.localScale = new Vector3(0f, 0f, 0f);
		base.StartCoroutine(this.ResizeRoutine());
	}

	// Token: 0x06000ED7 RID: 3799 RVA: 0x0003C01F File Offset: 0x0003A21F
	private IEnumerator ResizeRoutine()
	{
		float timer = 0f;
		while (timer < 1f)
		{
			timer += Time.deltaTime;
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, new Vector3(2f, 2f, 2f), timer);
			base.transform.LookAt(Camera.main.transform);
			yield return null;
		}
		yield return new WaitForSeconds(3f);
		timer = 0f;
		while (timer < 1f)
		{
			timer += Time.deltaTime;
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, new Vector3(0f, 0f, 0f), timer);
			yield return null;
		}
		base.transform.localScale = new Vector3(0f, 0f, 0f);
		yield break;
	}

	// Token: 0x04000824 RID: 2084
	public Transform spawnpos;
}
