using System;
using FishNet.Object;
using UnityEngine;

// Token: 0x02000033 RID: 51
public class CleetedWalkingStickController : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x06000236 RID: 566 RVA: 0x0000A2E4 File Offset: 0x000084E4
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x06000237 RID: 567 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction(GameObject player)
	{
	}

	// Token: 0x06000238 RID: 568 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x06000239 RID: 569 RVA: 0x0000A318 File Offset: 0x00008518
	public void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			base.transform.position = raycastHit.point;
		}
	}

	// Token: 0x0600023A RID: 570 RVA: 0x000021EF File Offset: 0x000003EF
	public void PlayDropSound()
	{
	}

	// Token: 0x0600023B RID: 571 RVA: 0x0000A386 File Offset: 0x00008586
	public void ItemInit()
	{
		this.rockrender.SetActive(true);
	}

	// Token: 0x0600023C RID: 572 RVA: 0x0000A386 File Offset: 0x00008586
	public void ItemInitObs()
	{
		this.rockrender.SetActive(true);
	}

	// Token: 0x0600023D RID: 573 RVA: 0x0000A394 File Offset: 0x00008594
	public void HideItem()
	{
		this.rockrender.SetActive(false);
	}

	// Token: 0x0600023E RID: 574 RVA: 0x00007BEE File Offset: 0x00005DEE
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
	}

	// Token: 0x0600023F RID: 575 RVA: 0x0000A3A2 File Offset: 0x000085A2
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp Cleeted Walking Stick";
	}

	// Token: 0x06000240 RID: 576 RVA: 0x0000A3A9 File Offset: 0x000085A9
	public int GetItemID()
	{
		return 1093;
	}

	// Token: 0x06000242 RID: 578 RVA: 0x0000A3D2 File Offset: 0x000085D2
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyCleetedWalkingStickControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyCleetedWalkingStickControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000243 RID: 579 RVA: 0x0000A3E5 File Offset: 0x000085E5
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateCleetedWalkingStickControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateCleetedWalkingStickControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000244 RID: 580 RVA: 0x0000A3F8 File Offset: 0x000085F8
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000245 RID: 581 RVA: 0x0000A3F8 File Offset: 0x000085F8
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0400011E RID: 286
	public GameObject rockrender;

	// Token: 0x0400011F RID: 287
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x04000120 RID: 288
	private bool NetworkInitialize___EarlyCleetedWalkingStickControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x04000121 RID: 289
	private bool NetworkInitialize__LateCleetedWalkingStickControllerAssembly-CSharp.dll_Excuted;
}
