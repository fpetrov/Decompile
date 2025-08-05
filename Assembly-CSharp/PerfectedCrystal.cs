using System;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x02000106 RID: 262
public class PerfectedCrystal : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x06000A85 RID: 2693 RVA: 0x00027B56 File Offset: 0x00025D56
	private void Start()
	{
		this.hdLightData = this.CrystalLight.GetComponent<HDAdditionalLightData>();
		this.asource = base.transform.GetComponent<AudioSource>();
	}

	// Token: 0x06000A86 RID: 2694 RVA: 0x00027B7C File Offset: 0x00025D7C
	private void Update()
	{
		if (this.lightval < 90000000f && !this.equipped)
		{
			this.lightval += Time.deltaTime * 3000000f;
		}
		else if (this.equipped)
		{
			this.lightval -= Time.deltaTime * 30000000f;
			if (this.lightval < 0f)
			{
				this.lightval = 0f;
			}
		}
		if (this.equipped)
		{
			this.asource.volume = Mathf.Lerp(this.asource.volume, this.lightval / 10000000f, Time.deltaTime * 2f);
		}
		else
		{
			this.asource.volume = Mathf.Lerp(this.asource.volume, 0f, Time.deltaTime * 20f);
		}
		this.hdLightData.intensity = this.lightval;
	}

	// Token: 0x06000A87 RID: 2695 RVA: 0x00027C68 File Offset: 0x00025E68
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x06000A88 RID: 2696 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction(GameObject player)
	{
	}

	// Token: 0x06000A89 RID: 2697 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x00027C9C File Offset: 0x00025E9C
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

	// Token: 0x06000A8B RID: 2699 RVA: 0x000021EF File Offset: 0x000003EF
	public void PlayDropSound()
	{
	}

	// Token: 0x06000A8C RID: 2700 RVA: 0x00027D0A File Offset: 0x00025F0A
	public void ItemInit()
	{
		this.rockrender.SetActive(true);
		this.equipped = true;
	}

	// Token: 0x06000A8D RID: 2701 RVA: 0x00027D0A File Offset: 0x00025F0A
	public void ItemInitObs()
	{
		this.rockrender.SetActive(true);
		this.equipped = true;
	}

	// Token: 0x06000A8E RID: 2702 RVA: 0x00027D1F File Offset: 0x00025F1F
	public void HideItem()
	{
		this.rockrender.SetActive(false);
		this.equipped = false;
	}

	// Token: 0x06000A8F RID: 2703 RVA: 0x00007BEE File Offset: 0x00005DEE
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
	}

	// Token: 0x06000A90 RID: 2704 RVA: 0x00027D34 File Offset: 0x00025F34
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp Perfected Orb";
	}

	// Token: 0x06000A91 RID: 2705 RVA: 0x00027D3B File Offset: 0x00025F3B
	public int GetItemID()
	{
		return 3452;
	}

	// Token: 0x06000A93 RID: 2707 RVA: 0x00027D6B File Offset: 0x00025F6B
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyPerfectedCrystalAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyPerfectedCrystalAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000A94 RID: 2708 RVA: 0x00027D7E File Offset: 0x00025F7E
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LatePerfectedCrystalAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LatePerfectedCrystalAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000A95 RID: 2709 RVA: 0x00027D91 File Offset: 0x00025F91
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000A96 RID: 2710 RVA: 0x00027D91 File Offset: 0x00025F91
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000598 RID: 1432
	public GameObject rockrender;

	// Token: 0x04000599 RID: 1433
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x0400059A RID: 1434
	public Light CrystalLight;

	// Token: 0x0400059B RID: 1435
	private HDAdditionalLightData hdLightData;

	// Token: 0x0400059C RID: 1436
	private float lightval;

	// Token: 0x0400059D RID: 1437
	private bool equipped = true;

	// Token: 0x0400059E RID: 1438
	private AudioSource asource;

	// Token: 0x0400059F RID: 1439
	private bool NetworkInitialize___EarlyPerfectedCrystalAssembly-CSharp.dll_Excuted;

	// Token: 0x040005A0 RID: 1440
	private bool NetworkInitialize__LatePerfectedCrystalAssembly-CSharp.dll_Excuted;
}
