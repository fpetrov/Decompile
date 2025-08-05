using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000050 RID: 80
public class DethRobeRaycaster : MonoBehaviour
{
	// Token: 0x06000368 RID: 872 RVA: 0x0000E2F3 File Offset: 0x0000C4F3
	private void Start()
	{
		base.StartCoroutine(this.DethRobeRoutine());
	}

	// Token: 0x06000369 RID: 873 RVA: 0x0000E302 File Offset: 0x0000C502
	private IEnumerator DethRobeRoutine()
	{
		float timer = 0f;
		RaycastHit hit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out hit, 30f, this.GroundLayer))
		{
			while (base.transform.position != hit.point)
			{
				if (timer >= 30f)
				{
					break;
				}
				base.transform.position = Vector3.Lerp(base.transform.position, hit.point, Time.deltaTime * 2f);
				yield return null;
				timer += Time.deltaTime;
			}
		}
		while (timer < 30f)
		{
			yield return null;
			timer += Time.deltaTime;
		}
		timer = 0f;
		Vector3 lerpTarg = new Vector3(base.transform.position.x, base.transform.position.y - 50f, base.transform.position.z);
		while (timer < 30f)
		{
			timer += Time.deltaTime;
			base.transform.position = Vector3.Lerp(base.transform.position, lerpTarg, Time.deltaTime / 2f);
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x040001B5 RID: 437
	public LayerMask GroundLayer;
}
