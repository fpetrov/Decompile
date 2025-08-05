using System;
using FishNet.Object;
using UnityEngine;

// Token: 0x0200015E RID: 350
public class Rock : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x06000EE8 RID: 3816 RVA: 0x0003C340 File Offset: 0x0003A540
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x06000EE9 RID: 3817 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction(GameObject player)
	{
	}

	// Token: 0x06000EEA RID: 3818 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x06000EEB RID: 3819 RVA: 0x0003C374 File Offset: 0x0003A574
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
		this.rockrender.enabled = true;
	}

	// Token: 0x06000EEC RID: 3820 RVA: 0x0003C401 File Offset: 0x0003A601
	public void PlayDropSound()
	{
		this.asource.PlayOneShot(this.aclips[1]);
	}

	// Token: 0x06000EED RID: 3821 RVA: 0x0003C416 File Offset: 0x0003A616
	public void ItemInit()
	{
		this.rockrender.enabled = true;
		this.asource.PlayOneShot(this.aclips[0]);
	}

	// Token: 0x06000EEE RID: 3822 RVA: 0x0003C416 File Offset: 0x0003A616
	public void ItemInitObs()
	{
		this.rockrender.enabled = true;
		this.asource.PlayOneShot(this.aclips[0]);
	}

	// Token: 0x06000EEF RID: 3823 RVA: 0x0003C437 File Offset: 0x0003A637
	public void HideItem()
	{
		this.rockrender.enabled = false;
	}

	// Token: 0x06000EF0 RID: 3824 RVA: 0x00007BEE File Offset: 0x00005DEE
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
	}

	// Token: 0x06000EF1 RID: 3825 RVA: 0x0003C445 File Offset: 0x0003A645
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp RocK";
	}

	// Token: 0x06000EF2 RID: 3826 RVA: 0x0003C44C File Offset: 0x0003A64C
	public int GetItemID()
	{
		return 1;
	}

	// Token: 0x06000EF4 RID: 3828 RVA: 0x0003C471 File Offset: 0x0003A671
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyRockAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyRockAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000EF5 RID: 3829 RVA: 0x0003C484 File Offset: 0x0003A684
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateRockAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateRockAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000EF6 RID: 3830 RVA: 0x0003C497 File Offset: 0x0003A697
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000EF7 RID: 3831 RVA: 0x0003C497 File Offset: 0x0003A697
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000833 RID: 2099
	public MeshRenderer rockrender;

	// Token: 0x04000834 RID: 2100
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x04000835 RID: 2101
	public AudioSource asource;

	// Token: 0x04000836 RID: 2102
	public AudioClip[] aclips;

	// Token: 0x04000837 RID: 2103
	private bool NetworkInitialize___EarlyRockAssembly-CSharp.dll_Excuted;

	// Token: 0x04000838 RID: 2104
	private bool NetworkInitialize__LateRockAssembly-CSharp.dll_Excuted;
}
