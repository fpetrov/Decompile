using System;
using UnityEngine;

// Token: 0x020000BB RID: 187
public class LightningBoltDamage : MonoBehaviour
{
	// Token: 0x0600075E RID: 1886 RVA: 0x0001C1F4 File Offset: 0x0001A3F4
	public void DoDmg(GameObject OwnerObj)
	{
		Vector3 vector = base.transform.position + Vector3.up * (this.height / 2f);
		Vector3 vector2 = new Vector3(this.width / 2f, this.height / 2f, this.width / 2f);
		foreach (Collider collider in Physics.OverlapBox(vector, vector2, Quaternion.identity, this.lrm))
		{
			PlayerMovement playerMovement;
			GetShadowWizardController getShadowWizardController;
			if (collider.CompareTag("Player") && collider.gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
			{
				playerMovement.LightningStrike(base.transform.position, OwnerObj);
			}
			else if (collider.CompareTag("PlayerNpc") && collider.gameObject.TryGetComponent<GetShadowWizardController>(out getShadowWizardController))
			{
				getShadowWizardController.ShadowWizardAI.GetComponent<ShadowWizardAI>().LightningHit(base.transform.position);
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
		foreach (Collider collider2 in Physics.OverlapSphere(base.transform.position, 50f, this.lrm))
		{
			PlayerMovement playerMovement2;
			if (collider2.CompareTag("Player") && collider2.gameObject.TryGetComponent<PlayerMovement>(out playerMovement2))
			{
				playerMovement2.ShakeCam((30f - Vector3.Distance(base.transform.position, collider2.transform.position)) / 10f, (50f - Vector3.Distance(base.transform.position, collider2.transform.position)) / 15f);
			}
		}
	}

	// Token: 0x040003B9 RID: 953
	private float height = 50f;

	// Token: 0x040003BA RID: 954
	private float width = 11f;

	// Token: 0x040003BB RID: 955
	public LayerMask lrm;
}
