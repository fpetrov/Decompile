using System;
using UnityEngine;

// Token: 0x020000A9 RID: 169
public class GolemGuyAdder : MonoBehaviour
{
	// Token: 0x060006DB RID: 1755 RVA: 0x0001A680 File Offset: 0x00018880
	private void OnTriggerEnter(Collider other)
	{
		GetShadowWizardController getShadowWizardController;
		if (other.CompareTag("Player"))
		{
			GetPlayerGameobject getPlayerGameobject;
			if (other.gameObject.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
			{
				this.mgc.potentialTargets.Add(getPlayerGameobject.player.transform);
				return;
			}
			PlayerMovement playerMovement;
			if (other.gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
			{
				this.mgc.potentialTargets.Add(playerMovement.transform);
				return;
			}
		}
		else if (other.CompareTag("PlayerNpc") && other.TryGetComponent<GetShadowWizardController>(out getShadowWizardController))
		{
			this.mgc.potentialTargets.Add(getShadowWizardController.ShadowWizardAI.transform);
		}
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x0001A720 File Offset: 0x00018920
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			GetPlayerGameobject getPlayerGameobject;
			if (other.gameObject.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
			{
				this.mgc.potentialTargets.Remove(getPlayerGameobject.player.transform);
				return;
			}
			PlayerMovement playerMovement;
			if (other.gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
			{
				this.mgc.potentialTargets.Remove(playerMovement.transform);
			}
		}
	}

	// Token: 0x04000368 RID: 872
	public GolemController mgc;
}
