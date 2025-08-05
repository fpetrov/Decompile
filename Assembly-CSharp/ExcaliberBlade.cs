using System;
using FishNet.Object;
using UnityEngine;

// Token: 0x02000071 RID: 113
public class ExcaliberBlade : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x060004C6 RID: 1222 RVA: 0x00012E0D File Offset: 0x0001100D
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x060004C7 RID: 1223 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction(GameObject player)
	{
	}

	// Token: 0x060004C8 RID: 1224 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x060004C9 RID: 1225 RVA: 0x00012E40 File Offset: 0x00011040
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

	// Token: 0x060004CA RID: 1226 RVA: 0x00012ECD File Offset: 0x000110CD
	public void PlayDropSound()
	{
		this.asource.PlayOneShot(this.aclips[1]);
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x00012EE2 File Offset: 0x000110E2
	public void ItemInit()
	{
		this.frogrender.SetActive(true);
		this.asource.PlayOneShot(this.aclips[0]);
	}

	// Token: 0x060004CC RID: 1228 RVA: 0x00012EE2 File Offset: 0x000110E2
	public void ItemInitObs()
	{
		this.frogrender.SetActive(true);
		this.asource.PlayOneShot(this.aclips[0]);
	}

	// Token: 0x060004CD RID: 1229 RVA: 0x00012F03 File Offset: 0x00011103
	public void HideItem()
	{
		this.frogrender.SetActive(false);
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x00007BEE File Offset: 0x00005DEE
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
	}

	// Token: 0x060004CF RID: 1231 RVA: 0x00012F11 File Offset: 0x00011111
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp Blade of Excaliber";
	}

	// Token: 0x060004D0 RID: 1232 RVA: 0x00012F18 File Offset: 0x00011118
	public int GetItemID()
	{
		return 9;
	}

	// Token: 0x060004D2 RID: 1234 RVA: 0x00012F3E File Offset: 0x0001113E
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyExcaliberBladeAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyExcaliberBladeAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x00012F51 File Offset: 0x00011151
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateExcaliberBladeAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateExcaliberBladeAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x060004D4 RID: 1236 RVA: 0x00012F64 File Offset: 0x00011164
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x060004D5 RID: 1237 RVA: 0x00012F64 File Offset: 0x00011164
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0400024F RID: 591
	public GameObject frogrender;

	// Token: 0x04000250 RID: 592
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x04000251 RID: 593
	public AudioSource asource;

	// Token: 0x04000252 RID: 594
	public AudioClip[] aclips;

	// Token: 0x04000253 RID: 595
	private bool NetworkInitialize___EarlyExcaliberBladeAssembly-CSharp.dll_Excuted;

	// Token: 0x04000254 RID: 596
	private bool NetworkInitialize__LateExcaliberBladeAssembly-CSharp.dll_Excuted;
}
