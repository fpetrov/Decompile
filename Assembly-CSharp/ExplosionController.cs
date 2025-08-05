using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x02000077 RID: 119
public class ExplosionController : MonoBehaviour
{
	// Token: 0x0600051C RID: 1308 RVA: 0x00013EC8 File Offset: 0x000120C8
	public void Explode(GameObject owner)
	{
		this.level = Mathf.Clamp(this.level, 0, 8);
		foreach (Collider collider in Physics.OverlapSphere(base.transform.position, 8f + (float)this.level * 0.4f, this.PlayerLayer))
		{
			PlayerMovement playerMovement;
			GetShadowWizardController getShadowWizardController;
			if (collider.CompareTag("Player") && collider.gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
			{
				playerMovement.ExplosionHit(base.transform.position, this.SkeleBomb, owner, this.level);
			}
			else if (collider.CompareTag("PlayerNpc") && collider.gameObject.TryGetComponent<GetShadowWizardController>(out getShadowWizardController))
			{
				getShadowWizardController.ShadowWizardAI.GetComponent<ShadowWizardAI>().ExplosionHit(base.transform.position);
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
		foreach (Collider collider2 in Physics.OverlapSphere(base.transform.position, 100f, this.PlayerLayer))
		{
			PlayerMovement playerMovement2;
			if (collider2.CompareTag("Player") && collider2.gameObject.TryGetComponent<PlayerMovement>(out playerMovement2))
			{
				float num = Mathf.Clamp(Vector3.Distance(base.transform.position, collider2.transform.position), 0f, 25f);
				playerMovement2.ShakeCam((33f - num) / 10f, (33f - num) / 10f);
			}
		}
		this.foge = base.transform.GetComponent<LocalVolumetricFog>();
		base.StartCoroutine(this.ExplosionFogRoutine());
		base.StartCoroutine(this.ExplosionLightRoutine());
		for (int j = 0; j < Random.Range(2, 6); j++)
		{
			Object.Instantiate<GameObject>(this.explosionFire, base.transform.position, Quaternion.identity);
		}
	}

	// Token: 0x0600051D RID: 1309 RVA: 0x0001410D File Offset: 0x0001230D
	private IEnumerator ExplosionLightRoutine()
	{
		this.timer = 0f;
		this.hdld.shapeRadius = 0f;
		while (this.timer <= this.lerpTime1)
		{
			this.timer += Time.deltaTime;
			this.hdld.range = Mathf.Lerp(0.01f, 5.7f + (float)(this.level * 2), this.timer / this.lerpTime1);
			yield return null;
		}
		yield return new WaitForSeconds(this.stallTime);
		this.timer = 0f;
		while (this.timer <= this.lerptime2)
		{
			this.timer += Time.deltaTime;
			this.hdld.shapeRadius = Mathf.Lerp(0f, this.highRadiusVal + (float)(this.level * 2), this.timer / this.lerptime2);
			if (this.timer < this.lightRangeTimer)
			{
				this.hdld.range = Mathf.Lerp(5.7f, 35f + (float)(this.level * 2), this.timer / this.lightRangeTimer);
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600051E RID: 1310 RVA: 0x0001411C File Offset: 0x0001231C
	private IEnumerator ExplosionFogRoutine()
	{
		this.foge.parameters.size = this.startSize;
		this.timer2 = 0f;
		while (this.timer2 <= this.lerpTime1fog)
		{
			this.timer2 += Time.deltaTime;
			this.foge.parameters.meanFreePath = Mathf.Lerp(this.highFogVal, this.LowMeanFree, this.timer2 / this.lerpTime1fog);
			yield return null;
		}
		this.timer2 = 0f;
		base.StartCoroutine(this.ExplosionFogSizeRoutine());
		while (this.timer2 <= this.stallTimeFog)
		{
			this.timer2 += Time.deltaTime;
			this.foge.parameters.meanFreePath = Mathf.Lerp(this.LowMeanFree, this.MidMeanFree, this.timer2 / this.stallTimeFog);
			yield return null;
		}
		this.timer2 = 0f;
		while (this.timer2 <= this.lerptime2fog)
		{
			this.timer2 += Time.deltaTime;
			this.foge.parameters.meanFreePath = Mathf.Lerp(this.MidMeanFree, this.highFogVal, this.timer2 / this.lerptime2fog);
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x0600051F RID: 1311 RVA: 0x0001412B File Offset: 0x0001232B
	private IEnumerator ExplosionFogSizeRoutine()
	{
		float timer3 = 0f;
		while (timer3 <= this.lerptime3fog)
		{
			timer3 += Time.deltaTime;
			this.foge.parameters.size = Vector3.Lerp(this.startSize + new Vector3((float)this.level * 1.8f, (float)this.level * 1.8f, (float)this.level * 1.8f), this.EndSize + new Vector3((float)this.level * 1.8f, (float)this.level * 1.8f, (float)this.level * 1.8f), timer3 / this.lerptime3fog);
			yield return null;
		}
		yield break;
	}

	// Token: 0x0400026D RID: 621
	private LocalVolumetricFog foge;

	// Token: 0x0400026E RID: 622
	private float timer;

	// Token: 0x0400026F RID: 623
	public HDAdditionalLightData hdld;

	// Token: 0x04000270 RID: 624
	public float highFogVal = 100f;

	// Token: 0x04000271 RID: 625
	public float highRadiusVal = 100f;

	// Token: 0x04000272 RID: 626
	public float lerpTime1 = 5f;

	// Token: 0x04000273 RID: 627
	public float lerptime2 = 5f;

	// Token: 0x04000274 RID: 628
	public float stallTime = 0.5f;

	// Token: 0x04000275 RID: 629
	public float lightRangeTimer = 5f;

	// Token: 0x04000276 RID: 630
	private float timer2 = 2f;

	// Token: 0x04000277 RID: 631
	public float lerpTime1fog = 5f;

	// Token: 0x04000278 RID: 632
	public float lerptime2fog = 5f;

	// Token: 0x04000279 RID: 633
	public float lerptime3fog = 5f;

	// Token: 0x0400027A RID: 634
	public float stallTimeFog = 0.5f;

	// Token: 0x0400027B RID: 635
	private Vector3 startSize = new Vector3(8f, 9f, 8f);

	// Token: 0x0400027C RID: 636
	public Vector3 EndSize = new Vector3(15f, 100f, 15f);

	// Token: 0x0400027D RID: 637
	public GameObject explosionFire;

	// Token: 0x0400027E RID: 638
	public float LowMeanFree = 0.05f;

	// Token: 0x0400027F RID: 639
	public float MidMeanFree = 0.5f;

	// Token: 0x04000280 RID: 640
	public LayerMask PlayerLayer;

	// Token: 0x04000281 RID: 641
	public bool SkeleBomb;

	// Token: 0x04000282 RID: 642
	public int level = 2;
}
