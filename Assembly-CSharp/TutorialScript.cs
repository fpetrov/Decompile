using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001C9 RID: 457
public class TutorialScript : MonoBehaviour
{
	// Token: 0x060012CE RID: 4814 RVA: 0x0004FDDE File Offset: 0x0004DFDE
	public void StartTutorial()
	{
		base.StartCoroutine(this.TutorialLoop());
		this.tgob.Starttheroutines();
	}

	// Token: 0x060012CF RID: 4815 RVA: 0x0004FDF8 File Offset: 0x0004DFF8
	private IEnumerator TutorialLoop()
	{
		for (;;)
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, 50f, this.lyrm);
			this.holes = 0;
			foreach (Collider collider in array)
			{
				PlayerInventory playerInventory;
				if (collider.CompareTag("Fireball"))
				{
					this.tgob.hasCastFireBall = true;
				}
				else if (collider.CompareTag("FrostBolt"))
				{
					this.tgob.hasCastFrostBolt = true;
				}
				else if (collider.CompareTag("wormhole"))
				{
					this.holes++;
					if (this.holes == 2)
					{
						this.tgob.hasCastHole = true;
					}
					this.tgob.hasCastWorm = true;
				}
				else if (collider.CompareTag("Magicmissle"))
				{
					this.tgob.hasCastWard = true;
				}
				else if (collider.CompareTag("item"))
				{
					this.tgob.hasCraftedShite = true;
				}
				else if (collider.CompareTag("Player") && this.pinv == null && collider.transform.TryGetComponent<PlayerInventory>(out playerInventory))
				{
					this.pinv = playerInventory;
				}
				if (this.pinv != null && this.pinv.GetEquippedItemID() == 5)
				{
					this.tgob.hasEquippedTorch = true;
				}
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000AFC RID: 2812
	public TutorialGoblin tgob;

	// Token: 0x04000AFD RID: 2813
	public LayerMask lyrm;

	// Token: 0x04000AFE RID: 2814
	private int holes;

	// Token: 0x04000AFF RID: 2815
	private PlayerInventory pinv;
}
