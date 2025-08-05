using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x020001AC RID: 428
public class SporeFrog : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x060011B9 RID: 4537 RVA: 0x0004BDBB File Offset: 0x00049FBB
	private void Update()
	{
		if (this.cdtimer < 21f)
		{
			this.cdtimer += Time.deltaTime;
		}
	}

	// Token: 0x060011BA RID: 4538 RVA: 0x0004BDDC File Offset: 0x00049FDC
	private void Start()
	{
		this.asource = base.transform.GetComponent<AudioSource>();
	}

	// Token: 0x060011BB RID: 4539 RVA: 0x0004BDEF File Offset: 0x00049FEF
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x060011BC RID: 4540 RVA: 0x0004BE21 File Offset: 0x0004A021
	public void Interaction(GameObject player)
	{
		if (this.cdtimer > 15f)
		{
			this.SpawnSmoke();
			this.cdtimer = 0f;
		}
	}

	// Token: 0x060011BD RID: 4541 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x060011BE RID: 4542 RVA: 0x0004BE41 File Offset: 0x0004A041
	[ServerRpc(RequireOwnership = false)]
	private void SpawnSmoke()
	{
		this.RpcWriter___Server_SpawnSmoke_2166136261();
	}

	// Token: 0x060011BF RID: 4543 RVA: 0x0004BE4C File Offset: 0x0004A04C
	[ObserversRpc]
	private void obsSpawnSmoke()
	{
		this.RpcWriter___Observers_obsSpawnSmoke_2166136261();
	}

	// Token: 0x060011C0 RID: 4544 RVA: 0x0004BE5F File Offset: 0x0004A05F
	private IEnumerator TurnOffSmoke()
	{
		yield return new WaitForSeconds(0.1f);
		this.FrogAni.SetBool("squeeze", false);
		yield break;
	}

	// Token: 0x060011C1 RID: 4545 RVA: 0x0004BE70 File Offset: 0x0004A070
	public void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			base.transform.position = raycastHit.point;
		}
		this.asource.PlayOneShot(this.squelchs[1]);
		this.rockrender.SetActive(true);
	}

	// Token: 0x060011C2 RID: 4546 RVA: 0x0004BEFD File Offset: 0x0004A0FD
	public void PlayDropSound()
	{
		this.asource.PlayOneShot(this.squelchs[1]);
	}

	// Token: 0x060011C3 RID: 4547 RVA: 0x0004BF12 File Offset: 0x0004A112
	public void ItemInit()
	{
		this.rockrender.SetActive(true);
		this.asource.PlayOneShot(this.squelchs[0]);
	}

	// Token: 0x060011C4 RID: 4548 RVA: 0x0004BF12 File Offset: 0x0004A112
	public void ItemInitObs()
	{
		this.rockrender.SetActive(true);
		this.asource.PlayOneShot(this.squelchs[0]);
	}

	// Token: 0x060011C5 RID: 4549 RVA: 0x0004BF33 File Offset: 0x0004A133
	public void HideItem()
	{
		this.rockrender.SetActive(false);
	}

	// Token: 0x060011C6 RID: 4550 RVA: 0x00007BEE File Offset: 0x00005DEE
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
	}

	// Token: 0x060011C7 RID: 4551 RVA: 0x0004BF41 File Offset: 0x0004A141
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp Spore Frog";
	}

	// Token: 0x060011C8 RID: 4552 RVA: 0x0004BF48 File Offset: 0x0004A148
	public int GetItemID()
	{
		return 21;
	}

	// Token: 0x060011CA RID: 4554 RVA: 0x0004BF7C File Offset: 0x0004A17C
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlySporeFrogAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlySporeFrogAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_SpawnSmoke_2166136261));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_obsSpawnSmoke_2166136261));
	}

	// Token: 0x060011CB RID: 4555 RVA: 0x0004BFC8 File Offset: 0x0004A1C8
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateSporeFrogAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateSporeFrogAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x060011CC RID: 4556 RVA: 0x0004BFDB File Offset: 0x0004A1DB
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x060011CD RID: 4557 RVA: 0x0004BFEC File Offset: 0x0004A1EC
	private void RpcWriter___Server_SpawnSmoke_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060011CE RID: 4558 RVA: 0x0004C051 File Offset: 0x0004A251
	private void RpcLogic___SpawnSmoke_2166136261()
	{
		this.obsSpawnSmoke();
	}

	// Token: 0x060011CF RID: 4559 RVA: 0x0004C05C File Offset: 0x0004A25C
	private void RpcReader___Server_SpawnSmoke_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___SpawnSmoke_2166136261();
	}

	// Token: 0x060011D0 RID: 4560 RVA: 0x0004C07C File Offset: 0x0004A27C
	private void RpcWriter___Observers_obsSpawnSmoke_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060011D1 RID: 4561 RVA: 0x0004C0F0 File Offset: 0x0004A2F0
	private void RpcLogic___obsSpawnSmoke_2166136261()
	{
		Object.Instantiate<GameObject>(this.SmokeCloud, base.transform.position, Quaternion.identity);
		this.FrogAni.SetBool("squeeze", true);
		this.asource.PlayOneShot(this.aclip);
		base.StartCoroutine(this.TurnOffSmoke());
	}

	// Token: 0x060011D2 RID: 4562 RVA: 0x0004C148 File Offset: 0x0004A348
	private void RpcReader___Observers_obsSpawnSmoke_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___obsSpawnSmoke_2166136261();
	}

	// Token: 0x060011D3 RID: 4563 RVA: 0x0004BFDB File Offset: 0x0004A1DB
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000A44 RID: 2628
	public GameObject rockrender;

	// Token: 0x04000A45 RID: 2629
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x04000A46 RID: 2630
	private AudioSource asource;

	// Token: 0x04000A47 RID: 2631
	public GameObject SmokeCloud;

	// Token: 0x04000A48 RID: 2632
	public Animator FrogAni;

	// Token: 0x04000A49 RID: 2633
	public AudioClip aclip;

	// Token: 0x04000A4A RID: 2634
	private float cdtimer = 20f;

	// Token: 0x04000A4B RID: 2635
	public AudioClip[] squelchs;

	// Token: 0x04000A4C RID: 2636
	private bool NetworkInitialize___EarlySporeFrogAssembly-CSharp.dll_Excuted;

	// Token: 0x04000A4D RID: 2637
	private bool NetworkInitialize__LateSporeFrogAssembly-CSharp.dll_Excuted;
}
