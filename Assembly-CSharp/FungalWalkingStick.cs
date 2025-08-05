using System;
using FishNet.Object;
using UnityEngine;

// Token: 0x0200009C RID: 156
public class FungalWalkingStick : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x0600067E RID: 1662 RVA: 0x000192E5 File Offset: 0x000174E5
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x0600067F RID: 1663 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction(GameObject player)
	{
	}

	// Token: 0x06000680 RID: 1664 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x00019318 File Offset: 0x00017518
	public void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			base.transform.position = raycastHit.point;
		}
		this.asource.PlayOneShot(this.aclip[2]);
		this.asource.PlayOneShot(this.aclip[3]);
		this.rockrender.SetActive(true);
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x000193B8 File Offset: 0x000175B8
	public void PlayDropSound()
	{
		this.asource.PlayOneShot(this.aclip[2]);
		this.asource.PlayOneShot(this.aclip[3]);
	}

	// Token: 0x06000683 RID: 1667 RVA: 0x000193E0 File Offset: 0x000175E0
	public void ItemInit()
	{
		this.rockrender.SetActive(true);
		this.asource.PlayOneShot(this.aclip[0]);
		this.asource.PlayOneShot(this.aclip[1]);
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x000193E0 File Offset: 0x000175E0
	public void ItemInitObs()
	{
		this.rockrender.SetActive(true);
		this.asource.PlayOneShot(this.aclip[0]);
		this.asource.PlayOneShot(this.aclip[1]);
	}

	// Token: 0x06000685 RID: 1669 RVA: 0x00019414 File Offset: 0x00017614
	public void HideItem()
	{
		this.rockrender.SetActive(false);
	}

	// Token: 0x06000686 RID: 1670 RVA: 0x00007BEE File Offset: 0x00005DEE
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
	}

	// Token: 0x06000687 RID: 1671 RVA: 0x00019422 File Offset: 0x00017622
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp Fungal Walking Stick";
	}

	// Token: 0x06000688 RID: 1672 RVA: 0x00019429 File Offset: 0x00017629
	public int GetItemID()
	{
		return 11;
	}

	// Token: 0x0600068A RID: 1674 RVA: 0x0001944F File Offset: 0x0001764F
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyFungalWalkingStickAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyFungalWalkingStickAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x0600068B RID: 1675 RVA: 0x00019462 File Offset: 0x00017662
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateFungalWalkingStickAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateFungalWalkingStickAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x0600068C RID: 1676 RVA: 0x00019475 File Offset: 0x00017675
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0600068D RID: 1677 RVA: 0x00019475 File Offset: 0x00017675
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000333 RID: 819
	public GameObject rockrender;

	// Token: 0x04000334 RID: 820
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x04000335 RID: 821
	public AudioSource asource;

	// Token: 0x04000336 RID: 822
	public AudioClip[] aclip;

	// Token: 0x04000337 RID: 823
	private bool NetworkInitialize___EarlyFungalWalkingStickAssembly-CSharp.dll_Excuted;

	// Token: 0x04000338 RID: 824
	private bool NetworkInitialize__LateFungalWalkingStickAssembly-CSharp.dll_Excuted;
}
