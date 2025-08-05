using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x02000090 RID: 144
public class FrogBalloon : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x060005DB RID: 1499 RVA: 0x00016CAD File Offset: 0x00014EAD
	public override void OnStartClient()
	{
		base.OnStartClient();
		this.AniSwitch();
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x00016CBC File Offset: 0x00014EBC
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp Frog Balloon";
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x00016CC4 File Offset: 0x00014EC4
	public void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(-90f, 0f, 90f);
			base.transform.position = raycastHit.point;
		}
		this.AniSwitchServer();
		this.frogrender.SetActive(true);
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x000021EF File Offset: 0x000003EF
	public void PlayDropSound()
	{
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x00016D44 File Offset: 0x00014F44
	public int GetItemID()
	{
		return 23;
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x00016D48 File Offset: 0x00014F48
	public void HideItem()
	{
		this.frogrender.SetActive(false);
	}

	// Token: 0x060005E1 RID: 1505 RVA: 0x00016D56 File Offset: 0x00014F56
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
		this.AniSwitchServer();
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction(GameObject player)
	{
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x00016D6F File Offset: 0x00014F6F
	public void ItemInit()
	{
		this.frogrender.SetActive(true);
		this.AniSwitchServer();
		this.frogs.pitch = Random.Range(0.8f, 0.9f);
		this.frogs.PlayOneShot(this.Frogc);
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x00016DAE File Offset: 0x00014FAE
	public void ItemInitObs()
	{
		this.frogrender.SetActive(true);
		this.frogs.pitch = Random.Range(0.8f, 0.9f);
		this.frogs.PlayOneShot(this.Frogc);
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x00016DE7 File Offset: 0x00014FE7
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x00016E19 File Offset: 0x00015019
	[ServerRpc(RequireOwnership = false)]
	private void AniSwitchServer()
	{
		this.RpcWriter___Server_AniSwitchServer_2166136261();
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x00016E21 File Offset: 0x00015021
	[ObserversRpc]
	private void AniSwitchObs()
	{
		this.RpcWriter___Observers_AniSwitchObs_2166136261();
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x00016E29 File Offset: 0x00015029
	private IEnumerator AniSwitch()
	{
		this.fgbn.SetBool("up", false);
		yield return null;
		this.fgbn.SetBool("up", true);
		yield break;
	}

	// Token: 0x060005EB RID: 1515 RVA: 0x00016E5C File Offset: 0x0001505C
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyFrogBalloonAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyFrogBalloonAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_AniSwitchServer_2166136261));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_AniSwitchObs_2166136261));
	}

	// Token: 0x060005EC RID: 1516 RVA: 0x00016EA8 File Offset: 0x000150A8
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateFrogBalloonAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateFrogBalloonAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x00016EBB File Offset: 0x000150BB
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x00016ECC File Offset: 0x000150CC
	private void RpcWriter___Server_AniSwitchServer_2166136261()
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

	// Token: 0x060005EF RID: 1519 RVA: 0x00016F31 File Offset: 0x00015131
	private void RpcLogic___AniSwitchServer_2166136261()
	{
		this.AniSwitchObs();
	}

	// Token: 0x060005F0 RID: 1520 RVA: 0x00016F3C File Offset: 0x0001513C
	private void RpcReader___Server_AniSwitchServer_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___AniSwitchServer_2166136261();
	}

	// Token: 0x060005F1 RID: 1521 RVA: 0x00016F5C File Offset: 0x0001515C
	private void RpcWriter___Observers_AniSwitchObs_2166136261()
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

	// Token: 0x060005F2 RID: 1522 RVA: 0x00016FD0 File Offset: 0x000151D0
	private void RpcLogic___AniSwitchObs_2166136261()
	{
		base.StartCoroutine(this.AniSwitch());
	}

	// Token: 0x060005F3 RID: 1523 RVA: 0x00016FE0 File Offset: 0x000151E0
	private void RpcReader___Observers_AniSwitchObs_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___AniSwitchObs_2166136261();
	}

	// Token: 0x060005F4 RID: 1524 RVA: 0x00016EBB File Offset: 0x000150BB
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x040002EF RID: 751
	public Animator fgbn;

	// Token: 0x040002F0 RID: 752
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x040002F1 RID: 753
	public GameObject frogrender;

	// Token: 0x040002F2 RID: 754
	public AudioSource frogs;

	// Token: 0x040002F3 RID: 755
	public AudioClip Frogc;

	// Token: 0x040002F4 RID: 756
	private bool NetworkInitialize___EarlyFrogBalloonAssembly-CSharp.dll_Excuted;

	// Token: 0x040002F5 RID: 757
	private bool NetworkInitialize__LateFrogBalloonAssembly-CSharp.dll_Excuted;
}
