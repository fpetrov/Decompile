using System;
using UnityEngine;

// Token: 0x02000145 RID: 325
public class PortcullisController : MonoBehaviour, ITimedInteractable
{
	// Token: 0x06000E16 RID: 3606 RVA: 0x00039C3E File Offset: 0x00037E3E
	public string DisplayInteractUI()
	{
		if (this.pnc.isOpen)
		{
			return "Shut the Portcullis";
		}
		return "Open the Portcullis";
	}

	// Token: 0x06000E17 RID: 3607 RVA: 0x00039C58 File Offset: 0x00037E58
	public float GetInteractTimer(GameObject player)
	{
		PlayerMovement playerMovement;
		if ((player.TryGetComponent<PlayerMovement>(out playerMovement) && playerMovement.playerTeam == this.pncteam) || this.pnc.isOpen)
		{
			return 0.75f;
		}
		return 17.1f;
	}

	// Token: 0x06000E18 RID: 3608 RVA: 0x00039C95 File Offset: 0x00037E95
	public void Interact(GameObject player)
	{
		this.pnc.PortcullisInteract();
	}

	// Token: 0x040007BB RID: 1979
	public PortcullisNetController pnc;

	// Token: 0x040007BC RID: 1980
	public int pncteam;
}
