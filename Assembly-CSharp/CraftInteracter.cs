using System;
using UnityEngine;

// Token: 0x0200003C RID: 60
public class CraftInteracter : MonoBehaviour, IInteractable
{
	// Token: 0x0600029E RID: 670 RVA: 0x0000BE10 File Offset: 0x0000A010
	public void Interact(GameObject player)
	{
		if (Time.time - this.cooldown > 0.25f)
		{
			this.cooldown = Time.time;
			player.GetComponent<PlayerInventory>().tempdisableitemswap();
			if (!this.itemPlaced)
			{
				player.GetComponent<PlayerInventory>().PlaceOnCraftingTable(base.gameObject);
				return;
			}
			player.GetComponent<PlayerInventory>().PickupCraftingTable(base.gameObject);
		}
	}

	// Token: 0x0600029F RID: 671 RVA: 0x0000BE71 File Offset: 0x0000A071
	public void SetSlot(GameObject obj)
	{
		this.craftingForge.SlotItems[this.slotID] = obj;
		this.itemPlaced = true;
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x0000BE8D File Offset: 0x0000A08D
	public void clearItem()
	{
		this.itemPlaced = false;
	}

	// Token: 0x060002A1 RID: 673 RVA: 0x0000BE96 File Offset: 0x0000A096
	public string DisplayInteractUI(GameObject player)
	{
		if (!this.itemPlaced)
		{
			return "Place Item";
		}
		return "Grasp Item";
	}

	// Token: 0x0400014F RID: 335
	public CraftingForge craftingForge;

	// Token: 0x04000150 RID: 336
	public int slotID;

	// Token: 0x04000151 RID: 337
	public bool itemPlaced;

	// Token: 0x04000152 RID: 338
	private float cooldown = -1f;
}
