using System;
using FishNet.Object;
using UnityEngine;

// Token: 0x020000C2 RID: 194
public class LogItem : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x06000777 RID: 1911 RVA: 0x0001CCB8 File Offset: 0x0001AEB8
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction(GameObject player)
	{
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x0001CCEC File Offset: 0x0001AEEC
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

	// Token: 0x0600077B RID: 1915 RVA: 0x0001CD79 File Offset: 0x0001AF79
	public void PlayDropSound()
	{
		this.asource.PlayOneShot(this.aclips[1]);
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x0001CD8E File Offset: 0x0001AF8E
	public void ItemInit()
	{
		this.rockrender.enabled = true;
		this.asource.PlayOneShot(this.aclips[0]);
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x0001CD8E File Offset: 0x0001AF8E
	public void ItemInitObs()
	{
		this.rockrender.enabled = true;
		this.asource.PlayOneShot(this.aclips[0]);
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x0001CDAF File Offset: 0x0001AFAF
	public void HideItem()
	{
		this.rockrender.enabled = false;
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x00007BEE File Offset: 0x00005DEE
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x0001CDBD File Offset: 0x0001AFBD
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp Log";
	}

	// Token: 0x06000781 RID: 1921 RVA: 0x0001CDC4 File Offset: 0x0001AFC4
	public int GetItemID()
	{
		return 0;
	}

	// Token: 0x06000783 RID: 1923 RVA: 0x0001CDE9 File Offset: 0x0001AFE9
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyLogItemAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyLogItemAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000784 RID: 1924 RVA: 0x0001CDFC File Offset: 0x0001AFFC
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateLogItemAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateLogItemAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000785 RID: 1925 RVA: 0x0001CE0F File Offset: 0x0001B00F
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x0001CE0F File Offset: 0x0001B00F
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x040003D0 RID: 976
	public MeshRenderer rockrender;

	// Token: 0x040003D1 RID: 977
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x040003D2 RID: 978
	public AudioSource asource;

	// Token: 0x040003D3 RID: 979
	public AudioClip[] aclips;

	// Token: 0x040003D4 RID: 980
	private bool NetworkInitialize___EarlyLogItemAssembly-CSharp.dll_Excuted;

	// Token: 0x040003D5 RID: 981
	private bool NetworkInitialize__LateLogItemAssembly-CSharp.dll_Excuted;
}
