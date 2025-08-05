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

// Token: 0x020001F1 RID: 497
public class NetInteractionManager : NetworkBehaviour
{
	// Token: 0x060013ED RID: 5101 RVA: 0x000531A6 File Offset: 0x000513A6
	public int AddToList(GameObject objToAdd)
	{
		this.Netobjects.Add(objToAdd);
		return this.Netobjects.Count - 1;
	}

	// Token: 0x060013EE RID: 5102 RVA: 0x000531C1 File Offset: 0x000513C1
	public void NetObjectInteraction(int objID)
	{
		this.ServerNetObjectInteraction(objID);
	}

	// Token: 0x060013EF RID: 5103 RVA: 0x000531CA File Offset: 0x000513CA
	[ServerRpc(RequireOwnership = false)]
	private void ServerNetObjectInteraction(int objID)
	{
		this.RpcWriter___Server_ServerNetObjectInteraction_3316948804(objID);
	}

	// Token: 0x060013F0 RID: 5104 RVA: 0x000531D8 File Offset: 0x000513D8
	[ObserversRpc]
	private void ObserverNetObjectInteraction(int objID)
	{
		this.RpcWriter___Observers_ObserverNetObjectInteraction_3316948804(objID);
	}

	// Token: 0x060013F1 RID: 5105 RVA: 0x000531EF File Offset: 0x000513EF
	public void PlayerOpenDoor(int objID)
	{
		this.ServerPlayerOpenDoor(objID);
	}

	// Token: 0x060013F2 RID: 5106 RVA: 0x000531F8 File Offset: 0x000513F8
	[ServerRpc(RequireOwnership = false)]
	private void ServerPlayerOpenDoor(int objID)
	{
		this.RpcWriter___Server_ServerPlayerOpenDoor_3316948804(objID);
	}

	// Token: 0x060013F3 RID: 5107 RVA: 0x00053204 File Offset: 0x00051404
	[ObserversRpc]
	private void ObserverPlayerOpenDoor(int objID)
	{
		this.RpcWriter___Observers_ObserverPlayerOpenDoor_3316948804(objID);
	}

	// Token: 0x060013F4 RID: 5108 RVA: 0x00053210 File Offset: 0x00051410
	public void SkeletonInteraction(int objID, int boneID)
	{
		this.ServerSkeletonNetObjectInteraction(objID, boneID);
	}

	// Token: 0x060013F5 RID: 5109 RVA: 0x0005321A File Offset: 0x0005141A
	[ServerRpc(RequireOwnership = false)]
	private void ServerSkeletonNetObjectInteraction(int objID, int boneID)
	{
		this.RpcWriter___Server_ServerSkeletonNetObjectInteraction_1692629761(objID, boneID);
	}

	// Token: 0x060013F6 RID: 5110 RVA: 0x0005322C File Offset: 0x0005142C
	[ObserversRpc]
	private void ObserverSkeletonNetObjectInteraction(int objID, int boneID)
	{
		this.RpcWriter___Observers_ObserverSkeletonNetObjectInteraction_1692629761(objID, boneID);
	}

	// Token: 0x060013F7 RID: 5111 RVA: 0x00053247 File Offset: 0x00051447
	public void Skeletoninit(int objID, Collider col)
	{
		this.ServerSkeletoninit(objID, col.gameObject);
	}

	// Token: 0x060013F8 RID: 5112 RVA: 0x00053256 File Offset: 0x00051456
	[ServerRpc(RequireOwnership = false)]
	private void ServerSkeletoninit(int objID, GameObject col)
	{
		this.RpcWriter___Server_ServerSkeletoninit_1011425610(objID, col);
	}

	// Token: 0x060013F9 RID: 5113 RVA: 0x00053268 File Offset: 0x00051468
	[ObserversRpc]
	private void ObserverSkeletoninit(int objID, GameObject col)
	{
		this.RpcWriter___Observers_ObserverSkeletoninit_1011425610(objID, col);
	}

	// Token: 0x060013FA RID: 5114 RVA: 0x00053284 File Offset: 0x00051484
	public void Skeletonretarget(int objID)
	{
		SkeletoneController skeletoneController;
		if (this.Netobjects[objID].TryGetComponent<SkeletoneController>(out skeletoneController) && base.HasAuthority)
		{
			Collider[] array = Physics.OverlapSphere(this.Netobjects[objID].transform.position, 20f, this.playerLayer);
			if (array.Length != 0)
			{
				float num = 22f;
				GameObject gameObject = null;
				foreach (Collider collider in array)
				{
					float num2 = Vector3.Distance(this.Netobjects[objID].transform.position, collider.transform.position);
					if (num2 < num)
					{
						num = num2;
						gameObject = collider.gameObject;
					}
				}
				if (gameObject != null)
				{
					GetPlayerGameobject getPlayerGameobject;
					if (gameObject.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
					{
						this.ServerSkeletonsettarget(objID, getPlayerGameobject.player, this.Netobjects[objID].transform.position);
						return;
					}
					this.ServerSkeletonsettarget(objID, gameObject, this.Netobjects[objID].transform.position);
				}
			}
		}
	}

	// Token: 0x060013FB RID: 5115 RVA: 0x0005339B File Offset: 0x0005159B
	[ServerRpc(RequireOwnership = false)]
	private void ServerSkeletonsettarget(int objID, GameObject target, Vector3 skpos)
	{
		this.RpcWriter___Server_ServerSkeletonsettarget_4287051269(objID, target, skpos);
	}

	// Token: 0x060013FC RID: 5116 RVA: 0x000533B0 File Offset: 0x000515B0
	[ObserversRpc]
	private void ObserverSkeletonsettarget(int objID, GameObject targeto, Vector3 skpos)
	{
		this.RpcWriter___Observers_ObserverSkeletonsettarget_4287051269(objID, targeto, skpos);
	}

	// Token: 0x060013FD RID: 5117 RVA: 0x000533CF File Offset: 0x000515CF
	public void SpawnPrefab(Vector3 SpawnPoint, GameObject Prefab)
	{
		if (base.HasAuthority)
		{
			this.SpawnBoulderServer(SpawnPoint, Prefab);
		}
	}

	// Token: 0x060013FE RID: 5118 RVA: 0x000533E4 File Offset: 0x000515E4
	[ServerRpc(RequireOwnership = false)]
	private void SpawnBoulderServer(Vector3 SpawnPoint, GameObject prefab)
	{
		this.RpcWriter___Server_SpawnBoulderServer_208080042(SpawnPoint, prefab);
	}

	// Token: 0x06001400 RID: 5120 RVA: 0x00053414 File Offset: 0x00051614
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyNetInteractionManagerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyNetInteractionManagerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ServerNetObjectInteraction_3316948804));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_ObserverNetObjectInteraction_3316948804));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_ServerPlayerOpenDoor_3316948804));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ObserverPlayerOpenDoor_3316948804));
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_ServerSkeletonNetObjectInteraction_1692629761));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_ObserverSkeletonNetObjectInteraction_1692629761));
		base.RegisterServerRpc(6U, new ServerRpcDelegate(this.RpcReader___Server_ServerSkeletoninit_1011425610));
		base.RegisterObserversRpc(7U, new ClientRpcDelegate(this.RpcReader___Observers_ObserverSkeletoninit_1011425610));
		base.RegisterServerRpc(8U, new ServerRpcDelegate(this.RpcReader___Server_ServerSkeletonsettarget_4287051269));
		base.RegisterObserversRpc(9U, new ClientRpcDelegate(this.RpcReader___Observers_ObserverSkeletonsettarget_4287051269));
		base.RegisterServerRpc(10U, new ServerRpcDelegate(this.RpcReader___Server_SpawnBoulderServer_208080042));
	}

	// Token: 0x06001401 RID: 5121 RVA: 0x0005352F File Offset: 0x0005172F
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateNetInteractionManagerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateNetInteractionManagerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06001402 RID: 5122 RVA: 0x00053542 File Offset: 0x00051742
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06001403 RID: 5123 RVA: 0x00053550 File Offset: 0x00051750
	private void RpcWriter___Server_ServerNetObjectInteraction_3316948804(int objID)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(objID);
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06001404 RID: 5124 RVA: 0x000535C2 File Offset: 0x000517C2
	private void RpcLogic___ServerNetObjectInteraction_3316948804(int objID)
	{
		this.ObserverNetObjectInteraction(objID);
	}

	// Token: 0x06001405 RID: 5125 RVA: 0x000535CC File Offset: 0x000517CC
	private void RpcReader___Server_ServerNetObjectInteraction_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerNetObjectInteraction_3316948804(num);
	}

	// Token: 0x06001406 RID: 5126 RVA: 0x00053600 File Offset: 0x00051800
	private void RpcWriter___Observers_ObserverNetObjectInteraction_3316948804(int objID)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(objID);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06001407 RID: 5127 RVA: 0x00053684 File Offset: 0x00051884
	private void RpcLogic___ObserverNetObjectInteraction_3316948804(int objID)
	{
		IInteractableNetworkObj interactableNetworkObj;
		if (this.Netobjects[objID].TryGetComponent<IInteractableNetworkObj>(out interactableNetworkObj))
		{
			interactableNetworkObj.ActualInteraction();
		}
	}

	// Token: 0x06001408 RID: 5128 RVA: 0x000536AC File Offset: 0x000518AC
	private void RpcReader___Observers_ObserverNetObjectInteraction_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserverNetObjectInteraction_3316948804(num);
	}

	// Token: 0x06001409 RID: 5129 RVA: 0x000536E0 File Offset: 0x000518E0
	private void RpcWriter___Server_ServerPlayerOpenDoor_3316948804(int objID)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(objID);
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600140A RID: 5130 RVA: 0x00053752 File Offset: 0x00051952
	private void RpcLogic___ServerPlayerOpenDoor_3316948804(int objID)
	{
		this.ObserverPlayerOpenDoor(objID);
	}

	// Token: 0x0600140B RID: 5131 RVA: 0x0005375C File Offset: 0x0005195C
	private void RpcReader___Server_ServerPlayerOpenDoor_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerPlayerOpenDoor_3316948804(num);
	}

	// Token: 0x0600140C RID: 5132 RVA: 0x00053790 File Offset: 0x00051990
	private void RpcWriter___Observers_ObserverPlayerOpenDoor_3316948804(int objID)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(objID);
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600140D RID: 5133 RVA: 0x00053811 File Offset: 0x00051A11
	private void RpcLogic___ObserverPlayerOpenDoor_3316948804(int objID)
	{
		this.Netobjects[objID].GetComponent<DoorOpen>().OpenedByPlayer = true;
	}

	// Token: 0x0600140E RID: 5134 RVA: 0x0005382C File Offset: 0x00051A2C
	private void RpcReader___Observers_ObserverPlayerOpenDoor_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserverPlayerOpenDoor_3316948804(num);
	}

	// Token: 0x0600140F RID: 5135 RVA: 0x00053860 File Offset: 0x00051A60
	private void RpcWriter___Server_ServerSkeletonNetObjectInteraction_1692629761(int objID, int boneID)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(objID);
		pooledWriter.WriteInt32(boneID);
		base.SendServerRpc(4U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06001410 RID: 5136 RVA: 0x000538DF File Offset: 0x00051ADF
	private void RpcLogic___ServerSkeletonNetObjectInteraction_1692629761(int objID, int boneID)
	{
		this.ObserverSkeletonNetObjectInteraction(objID, boneID);
	}

	// Token: 0x06001411 RID: 5137 RVA: 0x000538EC File Offset: 0x00051AEC
	private void RpcReader___Server_ServerSkeletonNetObjectInteraction_1692629761(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		int num2 = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSkeletonNetObjectInteraction_1692629761(num, num2);
	}

	// Token: 0x06001412 RID: 5138 RVA: 0x00053930 File Offset: 0x00051B30
	private void RpcWriter___Observers_ObserverSkeletonNetObjectInteraction_1692629761(int objID, int boneID)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(objID);
		pooledWriter.WriteInt32(boneID);
		base.SendObserversRpc(5U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06001413 RID: 5139 RVA: 0x000539C0 File Offset: 0x00051BC0
	private void RpcLogic___ObserverSkeletonNetObjectInteraction_1692629761(int objID, int boneID)
	{
		SkeletoneController skeletoneController;
		if (this.Netobjects[objID].TryGetComponent<SkeletoneController>(out skeletoneController))
		{
			skeletoneController.LastHitPart = boneID;
			skeletoneController.SkeleInteraction();
		}
	}

	// Token: 0x06001414 RID: 5140 RVA: 0x000539F0 File Offset: 0x00051BF0
	private void RpcReader___Observers_ObserverSkeletonNetObjectInteraction_1692629761(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		int num2 = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserverSkeletonNetObjectInteraction_1692629761(num, num2);
	}

	// Token: 0x06001415 RID: 5141 RVA: 0x00053A34 File Offset: 0x00051C34
	private void RpcWriter___Server_ServerSkeletoninit_1011425610(int objID, GameObject col)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(objID);
		pooledWriter.WriteGameObject(col);
		base.SendServerRpc(6U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06001416 RID: 5142 RVA: 0x00053AB3 File Offset: 0x00051CB3
	private void RpcLogic___ServerSkeletoninit_1011425610(int objID, GameObject col)
	{
		this.ObserverSkeletoninit(objID, col);
	}

	// Token: 0x06001417 RID: 5143 RVA: 0x00053AC0 File Offset: 0x00051CC0
	private void RpcReader___Server_ServerSkeletoninit_1011425610(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSkeletoninit_1011425610(num, gameObject);
	}

	// Token: 0x06001418 RID: 5144 RVA: 0x00053B04 File Offset: 0x00051D04
	private void RpcWriter___Observers_ObserverSkeletoninit_1011425610(int objID, GameObject col)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(objID);
		pooledWriter.WriteGameObject(col);
		base.SendObserversRpc(7U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06001419 RID: 5145 RVA: 0x00053B94 File Offset: 0x00051D94
	private void RpcLogic___ObserverSkeletoninit_1011425610(int objID, GameObject col)
	{
		SkeletoneController skeletoneController;
		if (this.Netobjects[objID].TryGetComponent<SkeletoneController>(out skeletoneController))
		{
			skeletoneController.SkeleInit(col);
		}
	}

	// Token: 0x0600141A RID: 5146 RVA: 0x00053BC0 File Offset: 0x00051DC0
	private void RpcReader___Observers_ObserverSkeletoninit_1011425610(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserverSkeletoninit_1011425610(num, gameObject);
	}

	// Token: 0x0600141B RID: 5147 RVA: 0x00053C04 File Offset: 0x00051E04
	private void RpcWriter___Server_ServerSkeletonsettarget_4287051269(int objID, GameObject target, Vector3 skpos)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(objID);
		pooledWriter.WriteGameObject(target);
		pooledWriter.WriteVector3(skpos);
		base.SendServerRpc(8U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600141C RID: 5148 RVA: 0x00053C90 File Offset: 0x00051E90
	private void RpcLogic___ServerSkeletonsettarget_4287051269(int objID, GameObject target, Vector3 skpos)
	{
		this.ObserverSkeletonsettarget(objID, target, skpos);
	}

	// Token: 0x0600141D RID: 5149 RVA: 0x00053C9C File Offset: 0x00051E9C
	private void RpcReader___Server_ServerSkeletonsettarget_4287051269(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		GameObject gameObject = PooledReader0.ReadGameObject();
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSkeletonsettarget_4287051269(num, gameObject, vector);
	}

	// Token: 0x0600141E RID: 5150 RVA: 0x00053CF0 File Offset: 0x00051EF0
	private void RpcWriter___Observers_ObserverSkeletonsettarget_4287051269(int objID, GameObject targeto, Vector3 skpos)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(objID);
		pooledWriter.WriteGameObject(targeto);
		pooledWriter.WriteVector3(skpos);
		base.SendObserversRpc(9U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600141F RID: 5151 RVA: 0x00053D8C File Offset: 0x00051F8C
	private void RpcLogic___ObserverSkeletonsettarget_4287051269(int objID, GameObject targeto, Vector3 skpos)
	{
		SkeletoneController skeletoneController;
		if (this.Netobjects[objID].TryGetComponent<SkeletoneController>(out skeletoneController))
		{
			skeletoneController.RepositionSkeleton(skpos, targeto);
			if (!skeletoneController.inited)
			{
				skeletoneController.SkeleInit(targeto);
				return;
			}
			skeletoneController.target = targeto;
		}
	}

	// Token: 0x06001420 RID: 5152 RVA: 0x00053DD0 File Offset: 0x00051FD0
	private void RpcReader___Observers_ObserverSkeletonsettarget_4287051269(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		GameObject gameObject = PooledReader0.ReadGameObject();
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserverSkeletonsettarget_4287051269(num, gameObject, vector);
	}

	// Token: 0x06001421 RID: 5153 RVA: 0x00053E24 File Offset: 0x00052024
	private void RpcWriter___Server_SpawnBoulderServer_208080042(Vector3 SpawnPoint, GameObject prefab)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(SpawnPoint);
		pooledWriter.WriteGameObject(prefab);
		base.SendServerRpc(10U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06001422 RID: 5154 RVA: 0x00053EA4 File Offset: 0x000520A4
	private void RpcLogic___SpawnBoulderServer_208080042(Vector3 SpawnPoint, GameObject prefab)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(prefab, SpawnPoint, Quaternion.identity);
		base.ServerManager.Spawn(gameObject, null, default(Scene));
	}

	// Token: 0x06001423 RID: 5155 RVA: 0x00053ED4 File Offset: 0x000520D4
	private void RpcReader___Server_SpawnBoulderServer_208080042(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___SpawnBoulderServer_208080042(vector, gameObject);
	}

	// Token: 0x06001424 RID: 5156 RVA: 0x00053542 File Offset: 0x00051742
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000BB5 RID: 2997
	public List<GameObject> Netobjects = new List<GameObject>();

	// Token: 0x04000BB6 RID: 2998
	public LayerMask playerLayer;

	// Token: 0x04000BB7 RID: 2999
	private bool NetworkInitialize___EarlyNetInteractionManagerAssembly-CSharp.dll_Excuted;

	// Token: 0x04000BB8 RID: 3000
	private bool NetworkInitialize__LateNetInteractionManagerAssembly-CSharp.dll_Excuted;
}
