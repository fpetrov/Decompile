using System;
using FishNet.Object;
using UnityEngine;

// Token: 0x020001CF RID: 463
public class WalkingStickController : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x06001301 RID: 4865 RVA: 0x00050978 File Offset: 0x0004EB78
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x06001302 RID: 4866 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction(GameObject player)
	{
	}

	// Token: 0x06001303 RID: 4867 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x06001304 RID: 4868 RVA: 0x000509AC File Offset: 0x0004EBAC
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
		this.rockrender.SetActive(true);
	}

	// Token: 0x06001305 RID: 4869 RVA: 0x00050A39 File Offset: 0x0004EC39
	public void PlayDropSound()
	{
		this.asource.PlayOneShot(this.aclips[1]);
	}

	// Token: 0x06001306 RID: 4870 RVA: 0x00050A4E File Offset: 0x0004EC4E
	public void ItemInit()
	{
		this.rockrender.SetActive(true);
		this.asource.PlayOneShot(this.aclips[0]);
	}

	// Token: 0x06001307 RID: 4871 RVA: 0x00050A4E File Offset: 0x0004EC4E
	public void ItemInitObs()
	{
		this.rockrender.SetActive(true);
		this.asource.PlayOneShot(this.aclips[0]);
	}

	// Token: 0x06001308 RID: 4872 RVA: 0x00050A6F File Offset: 0x0004EC6F
	public void HideItem()
	{
		this.rockrender.SetActive(false);
	}

	// Token: 0x06001309 RID: 4873 RVA: 0x00007BEE File Offset: 0x00005DEE
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
	}

	// Token: 0x0600130A RID: 4874 RVA: 0x00050A7D File Offset: 0x0004EC7D
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp Walking Stick";
	}

	// Token: 0x0600130B RID: 4875 RVA: 0x00050A84 File Offset: 0x0004EC84
	public int GetItemID()
	{
		return 12;
	}

	// Token: 0x0600130D RID: 4877 RVA: 0x00050AAA File Offset: 0x0004ECAA
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyWalkingStickControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyWalkingStickControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x0600130E RID: 4878 RVA: 0x00050ABD File Offset: 0x0004ECBD
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateWalkingStickControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateWalkingStickControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x0600130F RID: 4879 RVA: 0x00050AD0 File Offset: 0x0004ECD0
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06001310 RID: 4880 RVA: 0x00050AD0 File Offset: 0x0004ECD0
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000B15 RID: 2837
	public GameObject rockrender;

	// Token: 0x04000B16 RID: 2838
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x04000B17 RID: 2839
	public AudioSource asource;

	// Token: 0x04000B18 RID: 2840
	public AudioClip[] aclips;

	// Token: 0x04000B19 RID: 2841
	private bool NetworkInitialize___EarlyWalkingStickControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x04000B1A RID: 2842
	private bool NetworkInitialize__LateWalkingStickControllerAssembly-CSharp.dll_Excuted;
}
