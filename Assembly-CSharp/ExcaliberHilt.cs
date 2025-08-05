using System;
using FishNet.Object;
using UnityEngine;

// Token: 0x02000072 RID: 114
public class ExcaliberHilt : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x060004D6 RID: 1238 RVA: 0x00012F72 File Offset: 0x00011172
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction(GameObject player)
	{
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x00012FA4 File Offset: 0x000111A4
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

	// Token: 0x060004DA RID: 1242 RVA: 0x00013031 File Offset: 0x00011231
	public void PlayDropSound()
	{
		this.asource.PlayOneShot(this.aclips[1]);
	}

	// Token: 0x060004DB RID: 1243 RVA: 0x00013046 File Offset: 0x00011246
	public void ItemInit()
	{
		this.frogrender.SetActive(true);
		this.asource.PlayOneShot(this.aclips[0]);
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x00013046 File Offset: 0x00011246
	public void ItemInitObs()
	{
		this.frogrender.SetActive(true);
		this.asource.PlayOneShot(this.aclips[0]);
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x00013067 File Offset: 0x00011267
	public void HideItem()
	{
		this.frogrender.SetActive(false);
	}

	// Token: 0x060004DE RID: 1246 RVA: 0x00007BEE File Offset: 0x00005DEE
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
	}

	// Token: 0x060004DF RID: 1247 RVA: 0x00013075 File Offset: 0x00011275
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp Hilt of Excaliber";
	}

	// Token: 0x060004E0 RID: 1248 RVA: 0x0001307C File Offset: 0x0001127C
	public int GetItemID()
	{
		return 8;
	}

	// Token: 0x060004E2 RID: 1250 RVA: 0x000130A1 File Offset: 0x000112A1
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyExcaliberHiltAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyExcaliberHiltAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x000130B4 File Offset: 0x000112B4
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateExcaliberHiltAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateExcaliberHiltAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x060004E4 RID: 1252 RVA: 0x000130C7 File Offset: 0x000112C7
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x060004E5 RID: 1253 RVA: 0x000130C7 File Offset: 0x000112C7
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000255 RID: 597
	public GameObject frogrender;

	// Token: 0x04000256 RID: 598
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x04000257 RID: 599
	public AudioSource asource;

	// Token: 0x04000258 RID: 600
	public AudioClip[] aclips;

	// Token: 0x04000259 RID: 601
	private bool NetworkInitialize___EarlyExcaliberHiltAssembly-CSharp.dll_Excuted;

	// Token: 0x0400025A RID: 602
	private bool NetworkInitialize__LateExcaliberHiltAssembly-CSharp.dll_Excuted;
}
