using System;
using UnityEngine;

// Token: 0x0200015F RID: 351
public class RockCheckSphere : MonoBehaviour
{
	// Token: 0x06000EF8 RID: 3832 RVA: 0x0003C4A8 File Offset: 0x0003A6A8
	private void Start()
	{
		foreach (Collider collider in Physics.OverlapSphere(base.transform.position, this.radius, this.playerLayer))
		{
			PlayerMovement playerMovement;
			GetShadowWizardController getShadowWizardController;
			if (collider.TryGetComponent<PlayerMovement>(out playerMovement))
			{
				playerMovement.rockHit(this.grpo.owner);
			}
			else if (collider.CompareTag("PlayerNpc") && collider.gameObject.TryGetComponent<GetShadowWizardController>(out getShadowWizardController))
			{
				getShadowWizardController.ShadowWizardAI.GetComponent<ShadowWizardAI>().HitMonsterNotNetworked(100f);
			}
			else if (collider.CompareTag("hitable"))
			{
				collider.GetComponent<IInteractableNetworkObj>().NetInteract();
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
	}

	// Token: 0x06000EF9 RID: 3833 RVA: 0x0003C590 File Offset: 0x0003A790
	private void FixedUpdate()
	{
		if (this.timer < 0.25f)
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, this.radius, this.playerLayer);
			for (int i = 0; i < array.Length; i++)
			{
				PlayerMovement playerMovement;
				if (array[i].TryGetComponent<PlayerMovement>(out playerMovement))
				{
					playerMovement.rockHit(this.grpo.owner);
				}
			}
			this.timer += Time.fixedDeltaTime;
		}
	}

	// Token: 0x04000839 RID: 2105
	public float radius = 5f;

	// Token: 0x0400083A RID: 2106
	public LayerMask playerLayer;

	// Token: 0x0400083B RID: 2107
	public GetRockPlayerOwner grpo;

	// Token: 0x0400083C RID: 2108
	private float timer = 0.2f;
}
