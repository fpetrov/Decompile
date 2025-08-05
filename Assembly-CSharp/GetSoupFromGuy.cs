using System;
using UnityEngine;

// Token: 0x020000A4 RID: 164
public class GetSoupFromGuy : MonoBehaviour, IInteractable
{
	// Token: 0x06000697 RID: 1687 RVA: 0x000194B8 File Offset: 0x000176B8
	public void SoupisReady(int id)
	{
		this.soupid = id;
		this.pickupcol.enabled = true;
	}

	// Token: 0x06000698 RID: 1688 RVA: 0x000194CD File Offset: 0x000176CD
	public void SoupPickedup()
	{
		this.soupid = -1;
		this.pickupcol.enabled = false;
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x000194E4 File Offset: 0x000176E4
	public string DisplayInteractUI(GameObject player)
	{
		if (this.soupid == 2)
		{
			return "Grasp Log Soup";
		}
		if (this.soupid == 1)
		{
			return "Grasp Rock Soup";
		}
		if (this.soupid == 0)
		{
			return "Grasp Crystal Soup";
		}
		if (this.soupid == 3)
		{
			return "Grasp Frog Soup";
		}
		if (this.soupid == 4)
		{
			return "Grasp Mushroom Soup";
		}
		return "";
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x00019540 File Offset: 0x00017740
	public void Interact(GameObject player)
	{
		if (Time.time - this.cd > 2f)
		{
			this.cd = Time.time;
			this.smc.GiveSoup(this.soupid, player);
		}
	}

	// Token: 0x04000342 RID: 834
	public int soupid;

	// Token: 0x04000343 RID: 835
	public Collider pickupcol;

	// Token: 0x04000344 RID: 836
	public SoupManController smc;

	// Token: 0x04000345 RID: 837
	private float cd;
}
