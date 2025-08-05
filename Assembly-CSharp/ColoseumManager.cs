using System;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000034 RID: 52
public class ColoseumManager : NetworkBehaviour
{
	// Token: 0x06000246 RID: 582 RVA: 0x0000A406 File Offset: 0x00008606
	public override void OnStartClient()
	{
		base.OnStartClient();
		this.inited = true;
	}

	// Token: 0x06000247 RID: 583 RVA: 0x0000A418 File Offset: 0x00008618
	public int AddToList(GameObject obj)
	{
		this.interactableObjects.Add(obj);
		if (base.HasAuthority)
		{
			this.ServerPlaceItem(Random.Range(0, this.LootTable.Count), obj.transform.position);
		}
		return this.interactableObjects.IndexOf(obj);
	}

	// Token: 0x06000248 RID: 584 RVA: 0x0000A468 File Offset: 0x00008668
	[ServerRpc(RequireOwnership = false)]
	private void ServerPlaceItem(int itemid, Vector3 pos)
	{
		this.RpcWriter___Server_ServerPlaceItem_215135683(itemid, pos);
	}

	// Token: 0x06000249 RID: 585 RVA: 0x0000A483 File Offset: 0x00008683
	public void InteractWithObj(int index)
	{
		this.ServerObjInter(index);
	}

	// Token: 0x0600024A RID: 586 RVA: 0x0000A48C File Offset: 0x0000868C
	[ServerRpc(RequireOwnership = false)]
	private void ServerObjInter(int index)
	{
		this.RpcWriter___Server_ServerObjInter_3316948804(index);
	}

	// Token: 0x0600024B RID: 587 RVA: 0x0000A498 File Offset: 0x00008698
	[ObserversRpc]
	private void obsObjInter(int index)
	{
		this.RpcWriter___Observers_obsObjInter_3316948804(index);
	}

	// Token: 0x0600024D RID: 589 RVA: 0x0000A4D0 File Offset: 0x000086D0
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyColoseumManagerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyColoseumManagerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ServerPlaceItem_215135683));
		base.RegisterServerRpc(1U, new ServerRpcDelegate(this.RpcReader___Server_ServerObjInter_3316948804));
		base.RegisterObserversRpc(2U, new ClientRpcDelegate(this.RpcReader___Observers_obsObjInter_3316948804));
	}

	// Token: 0x0600024E RID: 590 RVA: 0x0000A533 File Offset: 0x00008733
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateColoseumManagerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateColoseumManagerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x0600024F RID: 591 RVA: 0x0000A546 File Offset: 0x00008746
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000250 RID: 592 RVA: 0x0000A554 File Offset: 0x00008754
	private void RpcWriter___Server_ServerPlaceItem_215135683(int itemid, Vector3 pos)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(itemid);
		pooledWriter.WriteVector3(pos);
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000251 RID: 593 RVA: 0x0000A5D4 File Offset: 0x000087D4
	private void RpcLogic___ServerPlaceItem_215135683(int itemid, Vector3 pos)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.LootTable[itemid]);
		gameObject.transform.position = pos + Vector3.up / 2f;
		if (itemid == 21 || itemid == 19 || itemid == 15 || itemid == 14 || itemid == 6 || itemid == 9)
		{
			gameObject.transform.localRotation = Quaternion.Euler(-100f, 0f, 180f);
		}
		else if (itemid == 10 || itemid == 12)
		{
			gameObject.transform.localRotation = Quaternion.Euler(180f, 0f, 180f);
		}
		else if (itemid == 4 || itemid == 8 || itemid == 11 || itemid == 13 || itemid == 16)
		{
			gameObject.transform.localRotation = Quaternion.Euler(-64.9f, 31.49f, -209.5f);
		}
		else
		{
			gameObject.transform.localRotation = Quaternion.Euler(10f, 0f, 180f);
		}
		base.ServerManager.Spawn(gameObject, null, default(Scene));
	}

	// Token: 0x06000252 RID: 594 RVA: 0x0000A6EC File Offset: 0x000088EC
	private void RpcReader___Server_ServerPlaceItem_215135683(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerPlaceItem_215135683(num, vector);
	}

	// Token: 0x06000253 RID: 595 RVA: 0x0000A730 File Offset: 0x00008930
	private void RpcWriter___Server_ServerObjInter_3316948804(int index)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(index);
		base.SendServerRpc(1U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000254 RID: 596 RVA: 0x0000A7A2 File Offset: 0x000089A2
	private void RpcLogic___ServerObjInter_3316948804(int index)
	{
		this.obsObjInter(index);
	}

	// Token: 0x06000255 RID: 597 RVA: 0x0000A7AC File Offset: 0x000089AC
	private void RpcReader___Server_ServerObjInter_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerObjInter_3316948804(num);
	}

	// Token: 0x06000256 RID: 598 RVA: 0x0000A7E0 File Offset: 0x000089E0
	private void RpcWriter___Observers_obsObjInter_3316948804(int index)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(index);
		base.SendObserversRpc(2U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000257 RID: 599 RVA: 0x0000A864 File Offset: 0x00008A64
	private void RpcLogic___obsObjInter_3316948804(int index)
	{
		IInteractableNetworkObj interactableNetworkObj;
		if (this.interactableObjects[index].TryGetComponent<IInteractableNetworkObj>(out interactableNetworkObj))
		{
			interactableNetworkObj.ActualInteraction();
		}
	}

	// Token: 0x06000258 RID: 600 RVA: 0x0000A88C File Offset: 0x00008A8C
	private void RpcReader___Observers_obsObjInter_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___obsObjInter_3316948804(num);
	}

	// Token: 0x06000259 RID: 601 RVA: 0x0000A546 File Offset: 0x00008746
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000122 RID: 290
	public List<GameObject> interactableObjects = new List<GameObject>();

	// Token: 0x04000123 RID: 291
	public List<GameObject> LootTable = new List<GameObject>();

	// Token: 0x04000124 RID: 292
	public bool inited;

	// Token: 0x04000125 RID: 293
	private bool NetworkInitialize___EarlyColoseumManagerAssembly-CSharp.dll_Excuted;

	// Token: 0x04000126 RID: 294
	private bool NetworkInitialize__LateColoseumManagerAssembly-CSharp.dll_Excuted;
}
