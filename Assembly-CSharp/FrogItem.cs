using System;
using FishNet.Object;
using UnityEngine;

// Token: 0x02000097 RID: 151
public class FrogItem : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x06000635 RID: 1589 RVA: 0x000183E5 File Offset: 0x000165E5
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x06000636 RID: 1590 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction(GameObject player)
	{
	}

	// Token: 0x06000637 RID: 1591 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x06000638 RID: 1592 RVA: 0x00018418 File Offset: 0x00016618
	public void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			base.transform.position = raycastHit.point;
		}
		this.asource.PlayOneShot(this.aclips[1]);
		this.frogrender.SetActive(true);
	}

	// Token: 0x06000639 RID: 1593 RVA: 0x000184A5 File Offset: 0x000166A5
	public void PlayDropSound()
	{
		this.asource.PlayOneShot(this.aclips[1]);
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x000184BA File Offset: 0x000166BA
	public void ItemInit()
	{
		this.frogrender.SetActive(true);
		this.asource.PlayOneShot(this.aclips[0]);
	}

	// Token: 0x0600063B RID: 1595 RVA: 0x000184BA File Offset: 0x000166BA
	public void ItemInitObs()
	{
		this.frogrender.SetActive(true);
		this.asource.PlayOneShot(this.aclips[0]);
	}

	// Token: 0x0600063C RID: 1596 RVA: 0x000184DB File Offset: 0x000166DB
	public void HideItem()
	{
		this.frogrender.SetActive(false);
	}

	// Token: 0x0600063D RID: 1597 RVA: 0x00007BEE File Offset: 0x00005DEE
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
	}

	// Token: 0x0600063E RID: 1598 RVA: 0x000184E9 File Offset: 0x000166E9
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp Toad";
	}

	// Token: 0x0600063F RID: 1599 RVA: 0x000184F0 File Offset: 0x000166F0
	public int GetItemID()
	{
		return 3;
	}

	// Token: 0x06000641 RID: 1601 RVA: 0x00018515 File Offset: 0x00016715
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyFrogItemAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyFrogItemAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000642 RID: 1602 RVA: 0x00018528 File Offset: 0x00016728
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateFrogItemAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateFrogItemAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000643 RID: 1603 RVA: 0x0001853B File Offset: 0x0001673B
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000644 RID: 1604 RVA: 0x0001853B File Offset: 0x0001673B
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000312 RID: 786
	public GameObject frogrender;

	// Token: 0x04000313 RID: 787
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x04000314 RID: 788
	public AudioSource asource;

	// Token: 0x04000315 RID: 789
	public AudioClip[] aclips;

	// Token: 0x04000316 RID: 790
	private bool NetworkInitialize___EarlyFrogItemAssembly-CSharp.dll_Excuted;

	// Token: 0x04000317 RID: 791
	private bool NetworkInitialize__LateFrogItemAssembly-CSharp.dll_Excuted;
}
