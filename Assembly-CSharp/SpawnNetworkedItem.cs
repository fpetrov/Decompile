using System;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200019D RID: 413
public class SpawnNetworkedItem : NetworkBehaviour
{
	// Token: 0x0600114F RID: 4431 RVA: 0x0004AD90 File Offset: 0x00048F90
	public override void OnStartClient()
	{
		base.OnStartClient();
		this.SpawnitemServer();
	}

	// Token: 0x06001150 RID: 4432 RVA: 0x0004ADA0 File Offset: 0x00048FA0
	[ServerRpc(RequireOwnership = false)]
	private void SpawnitemServer()
	{
		this.RpcWriter___Server_SpawnitemServer_2166136261();
	}

	// Token: 0x06001152 RID: 4434 RVA: 0x0004ADB3 File Offset: 0x00048FB3
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlySpawnNetworkedItemAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlySpawnNetworkedItemAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_SpawnitemServer_2166136261));
	}

	// Token: 0x06001153 RID: 4435 RVA: 0x0004ADDD File Offset: 0x00048FDD
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateSpawnNetworkedItemAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateSpawnNetworkedItemAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06001154 RID: 4436 RVA: 0x0004ADF0 File Offset: 0x00048FF0
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06001155 RID: 4437 RVA: 0x0004AE00 File Offset: 0x00049000
	private void RpcWriter___Server_SpawnitemServer_2166136261()
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

	// Token: 0x06001156 RID: 4438 RVA: 0x0004AE68 File Offset: 0x00049068
	private void RpcLogic___SpawnitemServer_2166136261()
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.prefab, base.transform.position, Quaternion.identity);
		base.Spawn(gameObject, null, default(Scene));
	}

	// Token: 0x06001157 RID: 4439 RVA: 0x0004AEA4 File Offset: 0x000490A4
	private void RpcReader___Server_SpawnitemServer_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___SpawnitemServer_2166136261();
	}

	// Token: 0x06001158 RID: 4440 RVA: 0x0004ADF0 File Offset: 0x00048FF0
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000A10 RID: 2576
	public GameObject prefab;

	// Token: 0x04000A11 RID: 2577
	private bool NetworkInitialize___EarlySpawnNetworkedItemAssembly-CSharp.dll_Excuted;

	// Token: 0x04000A12 RID: 2578
	private bool NetworkInitialize__LateSpawnNetworkedItemAssembly-CSharp.dll_Excuted;
}
