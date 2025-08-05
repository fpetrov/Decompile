using System;
using UnityEngine;

// Token: 0x02000013 RID: 19
public class BoneDogTargets : MonoBehaviour
{
	// Token: 0x060000D0 RID: 208 RVA: 0x00005670 File Offset: 0x00003870
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			GetPlayerGameobject getPlayerGameobject;
			if (other.gameObject.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
			{
				this.bdc.PotentialTargets.Add(getPlayerGameobject.player.transform);
				return;
			}
			PlayerMovement playerMovement;
			if (other.gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
			{
				this.bdc.PotentialTargets.Add(playerMovement.transform);
			}
		}
	}

	// Token: 0x060000D1 RID: 209 RVA: 0x000056DC File Offset: 0x000038DC
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			GetPlayerGameobject getPlayerGameobject;
			if (other.gameObject.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
			{
				this.bdc.PotentialTargets.Remove(getPlayerGameobject.player.transform);
				return;
			}
			PlayerMovement playerMovement;
			if (other.gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
			{
				this.bdc.PotentialTargets.Remove(playerMovement.transform);
			}
		}
	}

	// Token: 0x0400006A RID: 106
	public BoneDogController bdc;
}
