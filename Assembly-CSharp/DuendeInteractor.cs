using System;
using UnityEngine;

// Token: 0x0200005D RID: 93
public class DuendeInteractor : MonoBehaviour, IInteractable, IInteractableNetworkObj
{
	// Token: 0x060003BA RID: 954 RVA: 0x0000F5CC File Offset: 0x0000D7CC
	public void Interact(GameObject player)
	{
		PlayerInventory playerInventory;
		if (player.TryGetComponent<PlayerInventory>(out playerInventory) && !this.duend.isTrading && Time.time - this.tradeCDTimer > 20f && !this.duend.panic && !playerInventory.isNotHoldingItem())
		{
			this.duend.interactedWith(player);
			this.tradeCDTimer = Time.time;
		}
	}

	// Token: 0x060003BB RID: 955 RVA: 0x0000F630 File Offset: 0x0000D830
	public string DisplayInteractUI(GameObject player)
	{
		if (!this.duend.playedGreetingsSound && Time.time - this.hellocd > 5f && !this.duend.panic)
		{
			this.duend.playHelloSound();
			this.hellocd = Time.time;
		}
		PlayerInventory playerInventory;
		if (player.TryGetComponent<PlayerInventory>(out playerInventory) && !this.duend.isTrading && Time.time - this.tradeCDTimer > 20f && !this.duend.panic && !playerInventory.isNotHoldingItem())
		{
			return "Trade Item";
		}
		return "";
	}

	// Token: 0x060003BC RID: 956 RVA: 0x0000F6CA File Offset: 0x0000D8CA
	public void NetInteract()
	{
		if (!this.interacted)
		{
			this.NetItemManager.KillDuende(this.duend.DuendeID);
		}
	}

	// Token: 0x060003BD RID: 957 RVA: 0x0000F6EA File Offset: 0x0000D8EA
	public void ActualInteraction()
	{
		bool flag = this.interacted;
	}

	// Token: 0x040001EC RID: 492
	public DuendeController duend;

	// Token: 0x040001ED RID: 493
	private float tradeCDTimer;

	// Token: 0x040001EE RID: 494
	public DuendeManager NetItemManager;

	// Token: 0x040001EF RID: 495
	private bool interacted;

	// Token: 0x040001F0 RID: 496
	private float hellocd;
}
