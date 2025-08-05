using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001D0 RID: 464
public class WardController : MonoBehaviour
{
	// Token: 0x06001311 RID: 4881 RVA: 0x00050ADE File Offset: 0x0004ECDE
	private void Start()
	{
		this.WardScale = base.transform.localScale;
	}

	// Token: 0x06001312 RID: 4882 RVA: 0x00050AF1 File Offset: 0x0004ECF1
	public void WardDestroyed()
	{
		base.StartCoroutine(this.WardDestroyedRoutine());
	}

	// Token: 0x06001313 RID: 4883 RVA: 0x00050B00 File Offset: 0x0004ED00
	private IEnumerator WardDestroyedRoutine()
	{
		this.WardRenderer.enabled = false;
		foreach (Rigidbody rigidbody in this.rbs)
		{
			rigidbody.gameObject.SetActive(true);
			rigidbody.isKinematic = false;
		}
		float timer = 0f;
		while (timer < 10f)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		Rigidbody[] array = this.rbs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].useGravity = true;
		}
		Collider[] array2 = this.colls;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].enabled = false;
		}
		timer = 0f;
		while (timer < 12f)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		foreach (Rigidbody rigidbody2 in this.rbs)
		{
			rigidbody2.gameObject.SetActive(false);
			rigidbody2.isKinematic = true;
		}
		foreach (Collider collider in this.colls)
		{
			collider.enabled = true;
			collider.transform.localPosition = Vector3.zero;
			collider.transform.localRotation = Quaternion.identity;
		}
		yield break;
	}

	// Token: 0x06001314 RID: 4884 RVA: 0x00050B0F File Offset: 0x0004ED0F
	public void WardFadeOutOrIn(bool doFadeIn)
	{
		if (doFadeIn)
		{
			base.StartCoroutine(this.FadeIn());
			return;
		}
		base.StartCoroutine(this.FadeOut());
	}

	// Token: 0x06001315 RID: 4885 RVA: 0x00050B2F File Offset: 0x0004ED2F
	private IEnumerator FadeOut()
	{
		float timer = 0f;
		while (timer < 1f)
		{
			timer += Time.deltaTime;
			base.transform.localScale = Vector3.Lerp(this.WardScale, Vector3.zero, timer);
			yield return null;
		}
		this.WardRenderer.enabled = false;
		yield break;
	}

	// Token: 0x06001316 RID: 4886 RVA: 0x00050B3E File Offset: 0x0004ED3E
	private IEnumerator FadeIn()
	{
		float timer = 0f;
		base.transform.localScale = Vector3.zero;
		this.WardRenderer.enabled = true;
		while (timer < 1f)
		{
			timer += Time.deltaTime;
			base.transform.localScale = Vector3.Lerp(Vector3.zero, this.WardScale, timer);
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000B1B RID: 2843
	public Rigidbody[] rbs;

	// Token: 0x04000B1C RID: 2844
	public Collider[] colls;

	// Token: 0x04000B1D RID: 2845
	public MeshRenderer WardRenderer;

	// Token: 0x04000B1E RID: 2846
	private Vector3 WardScale;
}
