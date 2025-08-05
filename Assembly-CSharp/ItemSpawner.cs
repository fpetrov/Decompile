using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020000AF RID: 175
public class ItemSpawner : NetworkBehaviour
{
	// Token: 0x060006F3 RID: 1779 RVA: 0x0001ABE5 File Offset: 0x00018DE5
	public void StartItemRoutine()
	{
		if (base.HasAuthority)
		{
			base.StartCoroutine(this.SpawnItemRandomly());
		}
	}

	// Token: 0x060006F4 RID: 1780 RVA: 0x0001ABFC File Offset: 0x00018DFC
	private IEnumerator SpawnItemRandomly()
	{
		while (base.isActiveAndEnabled)
		{
			yield return new WaitForSeconds(this.shtimer);
			this.ServerSpawnRaven();
		}
		yield break;
	}

	// Token: 0x060006F5 RID: 1781 RVA: 0x0001AC0C File Offset: 0x00018E0C
	[ServerRpc(RequireOwnership = false)]
	private void ServerSpawnRaven()
	{
		this.RpcWriter___Server_ServerSpawnRaven_2166136261();
	}

	// Token: 0x060006F6 RID: 1782 RVA: 0x0001AC1F File Offset: 0x00018E1F
	[ObserversRpc]
	private void shitimerupd()
	{
		this.RpcWriter___Observers_shitimerupd_2166136261();
	}

	// Token: 0x060006F8 RID: 1784 RVA: 0x0001AC3C File Offset: 0x00018E3C
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyItemSpawnerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyItemSpawnerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ServerSpawnRaven_2166136261));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_shitimerupd_2166136261));
	}

	// Token: 0x060006F9 RID: 1785 RVA: 0x0001AC88 File Offset: 0x00018E88
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateItemSpawnerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateItemSpawnerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x060006FA RID: 1786 RVA: 0x0001AC9B File Offset: 0x00018E9B
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x060006FB RID: 1787 RVA: 0x0001ACAC File Offset: 0x00018EAC
	private void RpcWriter___Server_ServerSpawnRaven_2166136261()
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

	// Token: 0x060006FC RID: 1788 RVA: 0x0001AD14 File Offset: 0x00018F14
	private void RpcLogic___ServerSpawnRaven_2166136261()
	{
		Vector3 vector = this.raycastorgin.position + base.transform.forward * (float)Random.Range(-40, 40) + base.transform.right * (float)Random.Range(-40, 40);
		Vector3 down = Vector3.down;
		RaycastHit raycastHit;
		if (Physics.Raycast(vector, down, out raycastHit, 50f, this.groundlayer))
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.ItemPrefab, raycastHit.point, Quaternion.Euler(-90f, 0f, 0f));
			base.ServerManager.Spawn(gameObject, null, default(Scene));
			this.shitimerupd();
		}
	}

	// Token: 0x060006FD RID: 1789 RVA: 0x0001ADD0 File Offset: 0x00018FD0
	private void RpcReader___Server_ServerSpawnRaven_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSpawnRaven_2166136261();
	}

	// Token: 0x060006FE RID: 1790 RVA: 0x0001ADF0 File Offset: 0x00018FF0
	private void RpcWriter___Observers_shitimerupd_2166136261()
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

	// Token: 0x060006FF RID: 1791 RVA: 0x0001AE64 File Offset: 0x00019064
	private void RpcLogic___shitimerupd_2166136261()
	{
		this.shtimer *= 2f;
	}

	// Token: 0x06000700 RID: 1792 RVA: 0x0001AE78 File Offset: 0x00019078
	private void RpcReader___Observers_shitimerupd_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___shitimerupd_2166136261();
	}

	// Token: 0x06000701 RID: 1793 RVA: 0x0001AC9B File Offset: 0x00018E9B
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000376 RID: 886
	public LayerMask groundlayer;

	// Token: 0x04000377 RID: 887
	public GameObject ItemPrefab;

	// Token: 0x04000378 RID: 888
	public Transform raycastorgin;

	// Token: 0x04000379 RID: 889
	private float shtimer = 9f;

	// Token: 0x0400037A RID: 890
	private bool NetworkInitialize___EarlyItemSpawnerAssembly-CSharp.dll_Excuted;

	// Token: 0x0400037B RID: 891
	private bool NetworkInitialize__LateItemSpawnerAssembly-CSharp.dll_Excuted;
}
