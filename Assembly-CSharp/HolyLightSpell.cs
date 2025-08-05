using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x020000AC RID: 172
public class HolyLightSpell : MonoBehaviour, ISpell
{
	// Token: 0x060006E3 RID: 1763 RVA: 0x0001A890 File Offset: 0x00018A90
	public void PlayerSetup(GameObject ownerobj, Vector3 fwdVector, int level)
	{
		this.thelight.transform.position = ownerobj.transform.position + new Vector3(0f, 40f, 0f);
		Collider[] array = Physics.OverlapSphere(base.transform.position, 40f, this.playerLayer);
		for (int i = 0; i < array.Length; i++)
		{
			PlayerMovement playerMovement;
			if (array[i].TryGetComponent<PlayerMovement>(out playerMovement))
			{
				base.StartCoroutine(this.waitheal(playerMovement));
			}
		}
		base.StartCoroutine(this.Lightroutine());
	}

	// Token: 0x060006E4 RID: 1764 RVA: 0x0001A927 File Offset: 0x00018B27
	private IEnumerator waitheal(PlayerMovement pm)
	{
		yield return new WaitForSeconds(0.25f);
		pm.nonnetworkedheal(50f);
		yield break;
	}

	// Token: 0x060006E5 RID: 1765 RVA: 0x0001A936 File Offset: 0x00018B36
	private IEnumerator Lightroutine()
	{
		float timer = 0f;
		HDAdditionalLightData hdld = this.thelight.GetComponent<HDAdditionalLightData>();
		while (timer < 1f)
		{
			hdld.range = Mathf.Lerp(1f, 100f, timer);
			hdld.SetSpotAngle(Mathf.Lerp(10f, 30f, timer), 0f);
			timer += Time.deltaTime;
			yield return null;
		}
		yield return new WaitForSeconds(0.25f);
		timer = 0f;
		float startintesity = hdld.intensity;
		while (timer < 2f)
		{
			hdld.SetSpotAngle(Mathf.Lerp(30f, 179f, timer / 3f), 0f);
			hdld.intensity = Mathf.Lerp(startintesity, 1500000f, timer / 2f);
			timer += Time.deltaTime;
			yield return null;
		}
		timer = 0f;
		while (timer < 5f)
		{
			hdld.intensity = Mathf.Lerp(hdld.intensity, 0f, timer / 5f);
			timer += Time.deltaTime;
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x0400036B RID: 875
	public Transform thelight;

	// Token: 0x0400036C RID: 876
	public LayerMask playerLayer;
}
