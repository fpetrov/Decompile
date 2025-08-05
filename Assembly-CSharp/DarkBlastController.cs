using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x02000044 RID: 68
public class DarkBlastController : MonoBehaviour
{
	// Token: 0x06000307 RID: 775 RVA: 0x0000D03A File Offset: 0x0000B23A
	public void CastDarkBlast(Vector3 fwdVector, GameObject ownerob)
	{
		this.playerOwner = ownerob;
		base.transform.rotation = Quaternion.LookRotation(fwdVector, base.transform.up);
		base.StartCoroutine(this.lightRout());
		base.StartCoroutine(this.DamageRoutine());
	}

	// Token: 0x06000308 RID: 776 RVA: 0x0000D079 File Offset: 0x0000B279
	private IEnumerator DamageRoutine()
	{
		float num = 70f;
		Vector3 boxHalfExtents = new Vector3(0.9f, 0.9f, num / 2f);
		Vector3 boxCenter = base.transform.position + base.transform.forward.normalized * (num / 2f);
		Quaternion lookrot = Quaternion.LookRotation(base.transform.forward, base.transform.up);
		yield return new WaitForSeconds(0.25f);
		for (float timer = 0f; timer <= 0.8f; timer += 0.1f)
		{
			foreach (Collider collider in Physics.OverlapBox(boxCenter, boxHalfExtents, lookrot, this.playerLayerMask))
			{
				PlayerMovement playerMovement;
				GetShadowWizardController getShadowWizardController;
				if (collider.CompareTag("Player") && collider.gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
				{
					playerMovement.DarkBlastHit(this.playerOwner);
				}
				else if (collider.CompareTag("PlayerNpc") && collider.gameObject.TryGetComponent<GetShadowWizardController>(out getShadowWizardController))
				{
					getShadowWizardController.ShadowWizardAI.GetComponent<ShadowWizardAI>().HitMonsterNotNetworked(15f);
				}
				else if (collider.CompareTag("hitable"))
				{
					collider.GetComponent<IInteractableNetworkObj>().NetInteract();
				}
				else if (collider.CompareTag("tutbrazier"))
				{
					collider.GetComponent<TutorialBrazzier>().LightBrazier();
				}
				else if (collider.CompareTag("brazier"))
				{
					collider.GetComponent<BrazierInteract>().LightBrazier();
				}
				else if (collider.CompareTag("wormhole"))
				{
					collider.GetComponent<WormholeTele>().DestroyWormhole();
				}
			}
			yield return new WaitForSeconds(0.1f);
		}
		yield break;
	}

	// Token: 0x06000309 RID: 777 RVA: 0x0000D088 File Offset: 0x0000B288
	private IEnumerator lightRout()
	{
		float timer;
		for (timer = 0f; timer < 0.25f; timer += Time.deltaTime)
		{
			this.hdldpoint.intensity = Mathf.Lerp(0f, 40000000f, timer * 4f);
			this.lvf.parameters.meanFreePath = Mathf.Lerp(12f, 5f, timer * 4f);
			yield return null;
		}
		this.hdldpoint.intensity = 40000000f;
		this.hdldspot.intensity = 40000000f;
		for (timer = 0f; timer < 0.1f; timer += Time.deltaTime)
		{
			this.hdldspot.range = Mathf.Lerp(0f, 70f, timer * 10f);
			yield return null;
		}
		this.hdldspot.range = 70f;
		yield return new WaitForSeconds(0.5f);
		for (timer = 0f; timer < 0.25f; timer += Time.deltaTime)
		{
			this.hdldpoint.intensity = Mathf.Lerp(40000000f, 0f, timer * 4f);
			this.hdldspot.intensity = Mathf.Lerp(40000000f, 0f, timer * 4f);
			this.lvf.parameters.meanFreePath = Mathf.Lerp(5f, 10f, timer * 4f);
			yield return null;
		}
		this.hdldpoint.intensity = 0f;
		this.hdldspot.intensity = 0f;
		timer = 0f;
		while (timer < 0.1f)
		{
			yield return null;
			timer += Time.deltaTime;
			this.lvf.parameters.meanFreePath = Mathf.Lerp(10f, 50f, timer * 10f);
		}
		timer = 0f;
		while (timer < 1.5f)
		{
			yield return null;
			timer += Time.deltaTime;
			this.asour.volume = Mathf.Lerp(1f, 0f, timer / 1.5f);
		}
		this.asour.volume = 0f;
		this.lvf.parameters.meanFreePath = 200f;
		yield return new WaitForSeconds(3f);
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x0400017B RID: 379
	public HDAdditionalLightData hdldpoint;

	// Token: 0x0400017C RID: 380
	public HDAdditionalLightData hdldspot;

	// Token: 0x0400017D RID: 381
	public LocalVolumetricFog lvf;

	// Token: 0x0400017E RID: 382
	public AudioSource asour;

	// Token: 0x0400017F RID: 383
	public LayerMask playerLayerMask;

	// Token: 0x04000180 RID: 384
	private GameObject playerOwner;

	// Token: 0x04000181 RID: 385
	private Vector3 checkpos;
}
