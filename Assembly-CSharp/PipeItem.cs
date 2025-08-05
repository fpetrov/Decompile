using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x02000109 RID: 265
public class PipeItem : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x06000AA0 RID: 2720 RVA: 0x00027E69 File Offset: 0x00026069
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x06000AA1 RID: 2721 RVA: 0x00027E9C File Offset: 0x0002609C
	public void Interaction(GameObject player)
	{
		if (Time.time - this.cdtimer > 3f)
		{
			this.cdtimer = Time.time;
			PlayerMovement component = player.GetComponent<PlayerMovement>();
			player.GetComponent<PlayerInventory>().canSwapItem = false;
			component.SmokePipe();
			component.isSmoking = true;
			base.StartCoroutine(this.SmokePipe(component));
		}
	}

	// Token: 0x06000AA2 RID: 2722 RVA: 0x00027EF5 File Offset: 0x000260F5
	private IEnumerator SmokePipe(PlayerMovement pm)
	{
		this.serverplaysmokeani(pm.gameObject);
		yield return new WaitForSeconds(0.4f);
		pm.gameObject.GetComponent<PlayerInventory>().canSwapItem = true;
		this.asouce.PlayOneShot(this.smokefx[0]);
		yield return new WaitForSeconds(0.8f);
		pm.isSmoking = false;
		this.asouce.PlayOneShot(this.smokefx[1]);
		this.summonSmokeRing(Camera.main.transform.position, Camera.main.transform.forward);
		yield break;
	}

	// Token: 0x06000AA3 RID: 2723 RVA: 0x00027F0B File Offset: 0x0002610B
	[ServerRpc(RequireOwnership = false)]
	private void serverplaysmokeani(GameObject ani)
	{
		this.RpcWriter___Server_serverplaysmokeani_1934289915(ani);
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x00027F18 File Offset: 0x00026118
	[ObserversRpc]
	private void obsplaysmokeani(GameObject ani)
	{
		this.RpcWriter___Observers_obsplaysmokeani_1934289915(ani);
	}

	// Token: 0x06000AA5 RID: 2725 RVA: 0x00027F2F File Offset: 0x0002612F
	[ServerRpc(RequireOwnership = false)]
	private void summonSmokeRing(Vector3 spawnpos, Vector3 fwdvector)
	{
		this.RpcWriter___Server_summonSmokeRing_2936446947(spawnpos, fwdvector);
	}

	// Token: 0x06000AA6 RID: 2726 RVA: 0x00027F3F File Offset: 0x0002613F
	[ObserversRpc]
	private void ObsSummonSmokeRing(Vector3 spawnpos, Vector3 fwdvector)
	{
		this.RpcWriter___Observers_ObsSummonSmokeRing_2936446947(spawnpos, fwdvector);
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x00027F50 File Offset: 0x00026150
	public void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			base.transform.position = raycastHit.point;
		}
		this.asouce.PlayOneShot(this.smokefx[2]);
		this.piperender.SetActive(true);
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x00027FDD File Offset: 0x000261DD
	public void PlayDropSound()
	{
		this.asouce.PlayOneShot(this.smokefx[2]);
	}

	// Token: 0x06000AAA RID: 2730 RVA: 0x00027FF2 File Offset: 0x000261F2
	public void ItemInit()
	{
		this.piperender.SetActive(true);
	}

	// Token: 0x06000AAB RID: 2731 RVA: 0x00027FF2 File Offset: 0x000261F2
	public void ItemInitObs()
	{
		this.piperender.SetActive(true);
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x00028000 File Offset: 0x00026200
	public void HideItem()
	{
		this.piperender.SetActive(false);
	}

	// Token: 0x06000AAD RID: 2733 RVA: 0x00007BEE File Offset: 0x00005DEE
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
	}

	// Token: 0x06000AAE RID: 2734 RVA: 0x0002800E File Offset: 0x0002620E
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp Weed of the Pipe";
	}

	// Token: 0x06000AAF RID: 2735 RVA: 0x00028015 File Offset: 0x00026215
	public int GetItemID()
	{
		return 36;
	}

	// Token: 0x06000AB1 RID: 2737 RVA: 0x00028048 File Offset: 0x00026248
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyPipeItemAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyPipeItemAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_serverplaysmokeani_1934289915));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_obsplaysmokeani_1934289915));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_summonSmokeRing_2936446947));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ObsSummonSmokeRing_2936446947));
	}

	// Token: 0x06000AB2 RID: 2738 RVA: 0x000280C2 File Offset: 0x000262C2
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LatePipeItemAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LatePipeItemAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000AB3 RID: 2739 RVA: 0x000280D5 File Offset: 0x000262D5
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000AB4 RID: 2740 RVA: 0x000280E4 File Offset: 0x000262E4
	private void RpcWriter___Server_serverplaysmokeani_1934289915(GameObject ani)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(ani);
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000AB5 RID: 2741 RVA: 0x00028156 File Offset: 0x00026356
	private void RpcLogic___serverplaysmokeani_1934289915(GameObject ani)
	{
		this.obsplaysmokeani(ani);
	}

	// Token: 0x06000AB6 RID: 2742 RVA: 0x00028160 File Offset: 0x00026360
	private void RpcReader___Server_serverplaysmokeani_1934289915(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___serverplaysmokeani_1934289915(gameObject);
	}

	// Token: 0x06000AB7 RID: 2743 RVA: 0x00028194 File Offset: 0x00026394
	private void RpcWriter___Observers_obsplaysmokeani_1934289915(GameObject ani)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(ani);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x00028218 File Offset: 0x00026418
	private void RpcLogic___obsplaysmokeani_1934289915(GameObject ani)
	{
		int layerIndex = ani.GetComponent<PlayerMovement>().BodyAni.GetLayerIndex("Base Layer");
		ani.GetComponent<PlayerMovement>().BodyAni.Play("metarig|Weedofthepipe", layerIndex, 0f);
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x00028258 File Offset: 0x00026458
	private void RpcReader___Observers_obsplaysmokeani_1934289915(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___obsplaysmokeani_1934289915(gameObject);
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x0002828C File Offset: 0x0002648C
	private void RpcWriter___Server_summonSmokeRing_2936446947(Vector3 spawnpos, Vector3 fwdvector)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(spawnpos);
		pooledWriter.WriteVector3(fwdvector);
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x0002830B File Offset: 0x0002650B
	private void RpcLogic___summonSmokeRing_2936446947(Vector3 spawnpos, Vector3 fwdvector)
	{
		this.ObsSummonSmokeRing(spawnpos, fwdvector);
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x00028318 File Offset: 0x00026518
	private void RpcReader___Server_summonSmokeRing_2936446947(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		Vector3 vector2 = PooledReader0.ReadVector3();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___summonSmokeRing_2936446947(vector, vector2);
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x0002835C File Offset: 0x0002655C
	private void RpcWriter___Observers_ObsSummonSmokeRing_2936446947(Vector3 spawnpos, Vector3 fwdvector)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(spawnpos);
		pooledWriter.WriteVector3(fwdvector);
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000ABE RID: 2750 RVA: 0x000283EA File Offset: 0x000265EA
	private void RpcLogic___ObsSummonSmokeRing_2936446947(Vector3 spawnpos, Vector3 fwdvector)
	{
		Object.Instantiate<GameObject>(this.smokering, spawnpos, Quaternion.LookRotation(fwdvector) * Quaternion.Euler(90f, 0f, 0f)).GetComponent<SmokeRingController>().SetupRing(fwdvector);
	}

	// Token: 0x06000ABF RID: 2751 RVA: 0x00028424 File Offset: 0x00026624
	private void RpcReader___Observers_ObsSummonSmokeRing_2936446947(PooledReader PooledReader0, Channel channel)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		Vector3 vector2 = PooledReader0.ReadVector3();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsSummonSmokeRing_2936446947(vector, vector2);
	}

	// Token: 0x06000AC0 RID: 2752 RVA: 0x000280D5 File Offset: 0x000262D5
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x040005A8 RID: 1448
	public GameObject piperender;

	// Token: 0x040005A9 RID: 1449
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x040005AA RID: 1450
	public float maxDistance = 55f;

	// Token: 0x040005AB RID: 1451
	public GameObject smokering;

	// Token: 0x040005AC RID: 1452
	private float cdtimer;

	// Token: 0x040005AD RID: 1453
	public AudioClip[] smokefx;

	// Token: 0x040005AE RID: 1454
	public AudioSource asouce;

	// Token: 0x040005AF RID: 1455
	private bool NetworkInitialize___EarlyPipeItemAssembly-CSharp.dll_Excuted;

	// Token: 0x040005B0 RID: 1456
	private bool NetworkInitialize__LatePipeItemAssembly-CSharp.dll_Excuted;
}
