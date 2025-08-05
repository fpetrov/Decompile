using System;
using UnityEngine;

// Token: 0x0200019C RID: 412
public class SoupManInteractor : MonoBehaviour, IInteractable
{
	// Token: 0x0600114C RID: 4428 RVA: 0x0004AC34 File Offset: 0x00048E34
	public void Interact(GameObject player)
	{
		PlayerInventory playerInventory;
		if (player.TryGetComponent<PlayerInventory>(out playerInventory) && !this.smc.isCookingSoup && Time.time - this.cd > 1f)
		{
			this.cd = Time.time;
			int equippedItemID = playerInventory.GetEquippedItemID();
			if (equippedItemID == 0)
			{
				this.smc.CookSoup(2);
				playerInventory.destroyHandItem();
				return;
			}
			if (equippedItemID == 1)
			{
				this.smc.CookSoup(1);
				playerInventory.destroyHandItem();
				return;
			}
			if (equippedItemID == 2)
			{
				this.smc.CookSoup(0);
				playerInventory.destroyHandItem();
				return;
			}
			if (equippedItemID == 3)
			{
				this.smc.CookSoup(3);
				playerInventory.destroyHandItem();
				return;
			}
			if (equippedItemID == 4)
			{
				this.smc.CookSoup(4);
				playerInventory.destroyHandItem();
				return;
			}
			if (!this.hasTalked)
			{
				this.smc.playspeakline();
				this.hasTalked = true;
			}
		}
	}

	// Token: 0x0600114D RID: 4429 RVA: 0x0004AD14 File Offset: 0x00048F14
	public string DisplayInteractUI(GameObject player)
	{
		PlayerInventory playerInventory;
		if (player.TryGetComponent<PlayerInventory>(out playerInventory) && !this.smc.isCookingSoup)
		{
			int equippedItemID = playerInventory.GetEquippedItemID();
			if (equippedItemID == 0)
			{
				return "Make Log Soup";
			}
			if (equippedItemID == 1)
			{
				return "Make Rock Soup";
			}
			if (equippedItemID == 2)
			{
				return "Make Crystal Soup";
			}
			if (equippedItemID == 3)
			{
				return "Make Frog Soup";
			}
			if (equippedItemID == 4)
			{
				return "Make Mushroom Soup";
			}
		}
		if (!this.hasTalked && !this.smc.isCookingSoup)
		{
			return "Speak to Soup Man";
		}
		return "";
	}

	// Token: 0x04000A0D RID: 2573
	public SoupManController smc;

	// Token: 0x04000A0E RID: 2574
	private bool hasTalked;

	// Token: 0x04000A0F RID: 2575
	private float cd;
}
