using System;
using UnityEngine;

// Token: 0x02000029 RID: 41
public class ChestInteract : MonoBehaviour, ITimedInteractable
{
	// Token: 0x060001B9 RID: 441 RVA: 0x000085CC File Offset: 0x000067CC
	public string DisplayInteractUI()
	{
		if (this.cnc.isOpen)
		{
			return "Shut Chest";
		}
		return "Open Chest";
	}

	// Token: 0x060001BA RID: 442 RVA: 0x000085E8 File Offset: 0x000067E8
	public float GetInteractTimer(GameObject player)
	{
		PlayerMovement playerMovement;
		if ((player.TryGetComponent<PlayerMovement>(out playerMovement) && playerMovement.playerTeam == this.cncTeam) || this.cnc.isOpen)
		{
			return 1f;
		}
		return 15.1f;
	}

	// Token: 0x060001BB RID: 443 RVA: 0x00008625 File Offset: 0x00006825
	public void Interact(GameObject player)
	{
		this.cnc.ChestInteract();
	}

	// Token: 0x040000E3 RID: 227
	public ChestNetController cnc;

	// Token: 0x040000E4 RID: 228
	public int cncTeam;
}
