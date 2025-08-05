using System;
using FishNet.Object;
using UnityEngine;

// Token: 0x02000041 RID: 65
public class CrystalShard : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x060002D4 RID: 724 RVA: 0x0000C944 File Offset: 0x0000AB44
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction(GameObject player)
	{
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x0000C978 File Offset: 0x0000AB78
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

	// Token: 0x060002D8 RID: 728 RVA: 0x0000CA05 File Offset: 0x0000AC05
	public void PlayDropSound()
	{
		this.asource.PlayOneShot(this.aclips[1]);
	}

	// Token: 0x060002D9 RID: 729 RVA: 0x0000CA1A File Offset: 0x0000AC1A
	public void ItemInit()
	{
		this.rockrender.SetActive(true);
		this.asource.PlayOneShot(this.aclips[0]);
	}

	// Token: 0x060002DA RID: 730 RVA: 0x0000CA1A File Offset: 0x0000AC1A
	public void ItemInitObs()
	{
		this.rockrender.SetActive(true);
		this.asource.PlayOneShot(this.aclips[0]);
	}

	// Token: 0x060002DB RID: 731 RVA: 0x0000CA3B File Offset: 0x0000AC3B
	public void HideItem()
	{
		this.rockrender.SetActive(false);
	}

	// Token: 0x060002DC RID: 732 RVA: 0x00007BEE File Offset: 0x00005DEE
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
	}

	// Token: 0x060002DD RID: 733 RVA: 0x0000CA49 File Offset: 0x0000AC49
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp Crystal";
	}

	// Token: 0x060002DE RID: 734 RVA: 0x0000CA50 File Offset: 0x0000AC50
	public int GetItemID()
	{
		return 2;
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x0000CA75 File Offset: 0x0000AC75
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyCrystalShardAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyCrystalShardAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x0000CA88 File Offset: 0x0000AC88
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateCrystalShardAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateCrystalShardAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x0000CA9B File Offset: 0x0000AC9B
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x060002E3 RID: 739 RVA: 0x0000CA9B File Offset: 0x0000AC9B
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000167 RID: 359
	public GameObject rockrender;

	// Token: 0x04000168 RID: 360
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x04000169 RID: 361
	public AudioSource asource;

	// Token: 0x0400016A RID: 362
	public AudioClip[] aclips;

	// Token: 0x0400016B RID: 363
	private bool NetworkInitialize___EarlyCrystalShardAssembly-CSharp.dll_Excuted;

	// Token: 0x0400016C RID: 364
	private bool NetworkInitialize__LateCrystalShardAssembly-CSharp.dll_Excuted;
}
