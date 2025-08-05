using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000082 RID: 130
public class FireballFireController : MonoBehaviour
{
	// Token: 0x06000570 RID: 1392 RVA: 0x00015020 File Offset: 0x00013220
	private void Start()
	{
		base.transform.position = new Vector3(base.transform.position.x + Random.Range(-1.1f, 2.1f), base.transform.position.y + 1f, base.transform.position.z + Random.Range(-1.3f, 1.3f));
		base.transform.rotation = Quaternion.Euler(0f, (float)Random.Range(0, 270), 0f);
		base.transform.localScale *= Random.Range(0.2f, 1f);
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 5f, 8))
		{
			base.transform.position = new Vector3(raycastHit.point.x + (float)Random.Range(-1, 3), raycastHit.point.y - 0.2f, raycastHit.point.z + (float)Random.Range(-2, 2));
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		Material material = base.transform.GetComponent<Renderer>().material;
		base.StartCoroutine(this.lerpAlphaVal(material));
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x00015176 File Offset: 0x00013376
	private IEnumerator lerpAlphaVal(Material mat)
	{
		float alpha = 0f;
		while (alpha < 1f)
		{
			alpha += 0.01f;
			mat.SetFloat("_alphamulti", alpha);
			yield return null;
		}
		alpha = 0f;
		while (alpha < 13f)
		{
			alpha += Time.deltaTime;
			base.transform.localScale = new Vector3(base.transform.localScale.x, Mathf.Clamp(base.transform.localScale.y - Time.deltaTime * 15f, 0f, 250f), base.transform.localScale.z);
			yield return null;
		}
		alpha = 1f;
		while (alpha > 0f)
		{
			alpha -= Time.deltaTime / 2f;
			mat.SetFloat("_alphamulti", alpha);
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield break;
	}
}
