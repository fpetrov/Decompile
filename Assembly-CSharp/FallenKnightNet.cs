using System;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;

// Token: 0x0200007E RID: 126
public class FallenKnightNet : NetworkBehaviour
{
	// Token: 0x06000547 RID: 1351 RVA: 0x00014916 File Offset: 0x00012B16
	public void KillFK()
	{
		this.KillFKServer();
	}

	// Token: 0x06000548 RID: 1352 RVA: 0x0001491E File Offset: 0x00012B1E
	[ServerRpc(RequireOwnership = false)]
	private void KillFKServer()
	{
		this.RpcWriter___Server_KillFKServer_2166136261();
	}

	// Token: 0x06000549 RID: 1353 RVA: 0x00014926 File Offset: 0x00012B26
	[ObserversRpc]
	private void KillFKObservers()
	{
		this.RpcWriter___Observers_KillFKObservers_2166136261();
	}

	// Token: 0x0600054A RID: 1354 RVA: 0x0001492E File Offset: 0x00012B2E
	public void KnightTalk()
	{
		this.ServerKT();
	}

	// Token: 0x0600054B RID: 1355 RVA: 0x00014936 File Offset: 0x00012B36
	[ServerRpc(RequireOwnership = false)]
	private void ServerKT()
	{
		this.RpcWriter___Server_ServerKT_2166136261();
	}

	// Token: 0x0600054C RID: 1356 RVA: 0x0001493E File Offset: 0x00012B3E
	[ObserversRpc]
	private void ObsKT()
	{
		this.RpcWriter___Observers_ObsKT_2166136261();
	}

	// Token: 0x0600054D RID: 1357 RVA: 0x00014946 File Offset: 0x00012B46
	public void KnightDonged()
	{
		this.ServerKnightDonged();
	}

	// Token: 0x0600054E RID: 1358 RVA: 0x0001494E File Offset: 0x00012B4E
	[ServerRpc(RequireOwnership = false)]
	private void ServerKnightDonged()
	{
		this.RpcWriter___Server_ServerKnightDonged_2166136261();
	}

	// Token: 0x0600054F RID: 1359 RVA: 0x00014956 File Offset: 0x00012B56
	[ObserversRpc]
	private void ObsKnightDonged()
	{
		this.RpcWriter___Observers_ObsKnightDonged_2166136261();
	}

	// Token: 0x06000551 RID: 1361 RVA: 0x00014960 File Offset: 0x00012B60
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyFallenKnightNetAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyFallenKnightNetAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_KillFKServer_2166136261));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_KillFKObservers_2166136261));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_ServerKT_2166136261));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ObsKT_2166136261));
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_ServerKnightDonged_2166136261));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_ObsKnightDonged_2166136261));
	}

	// Token: 0x06000552 RID: 1362 RVA: 0x00014A08 File Offset: 0x00012C08
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateFallenKnightNetAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateFallenKnightNetAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000553 RID: 1363 RVA: 0x00014A1B File Offset: 0x00012C1B
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000554 RID: 1364 RVA: 0x00014A2C File Offset: 0x00012C2C
	private void RpcWriter___Server_KillFKServer_2166136261()
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

	// Token: 0x06000555 RID: 1365 RVA: 0x00014A91 File Offset: 0x00012C91
	private void RpcLogic___KillFKServer_2166136261()
	{
		this.KillFKObservers();
	}

	// Token: 0x06000556 RID: 1366 RVA: 0x00014A9C File Offset: 0x00012C9C
	private void RpcReader___Server_KillFKServer_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___KillFKServer_2166136261();
	}

	// Token: 0x06000557 RID: 1367 RVA: 0x00014ABC File Offset: 0x00012CBC
	private void RpcWriter___Observers_KillFKObservers_2166136261()
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

	// Token: 0x06000558 RID: 1368 RVA: 0x00014B30 File Offset: 0x00012D30
	private void RpcLogic___KillFKObservers_2166136261()
	{
		this.fk.ActualInteraction();
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x00014B40 File Offset: 0x00012D40
	private void RpcReader___Observers_KillFKObservers_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___KillFKObservers_2166136261();
	}

	// Token: 0x0600055A RID: 1370 RVA: 0x00014B60 File Offset: 0x00012D60
	private void RpcWriter___Server_ServerKT_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600055B RID: 1371 RVA: 0x00014BC5 File Offset: 0x00012DC5
	private void RpcLogic___ServerKT_2166136261()
	{
		this.ObsKT();
	}

	// Token: 0x0600055C RID: 1372 RVA: 0x00014BD0 File Offset: 0x00012DD0
	private void RpcReader___Server_ServerKT_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerKT_2166136261();
	}

	// Token: 0x0600055D RID: 1373 RVA: 0x00014BF0 File Offset: 0x00012DF0
	private void RpcWriter___Observers_ObsKT_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600055E RID: 1374 RVA: 0x00014C64 File Offset: 0x00012E64
	private void RpcLogic___ObsKT_2166136261()
	{
		this.fk.KnightTalk();
	}

	// Token: 0x0600055F RID: 1375 RVA: 0x00014C74 File Offset: 0x00012E74
	private void RpcReader___Observers_ObsKT_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsKT_2166136261();
	}

	// Token: 0x06000560 RID: 1376 RVA: 0x00014C94 File Offset: 0x00012E94
	private void RpcWriter___Server_ServerKnightDonged_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(4U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000561 RID: 1377 RVA: 0x00014CF9 File Offset: 0x00012EF9
	private void RpcLogic___ServerKnightDonged_2166136261()
	{
		this.ObsKnightDonged();
	}

	// Token: 0x06000562 RID: 1378 RVA: 0x00014D04 File Offset: 0x00012F04
	private void RpcReader___Server_ServerKnightDonged_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerKnightDonged_2166136261();
	}

	// Token: 0x06000563 RID: 1379 RVA: 0x00014D24 File Offset: 0x00012F24
	private void RpcWriter___Observers_ObsKnightDonged_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(5U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000564 RID: 1380 RVA: 0x00014D98 File Offset: 0x00012F98
	private void RpcLogic___ObsKnightDonged_2166136261()
	{
		this.kd.ActualInteraction();
	}

	// Token: 0x06000565 RID: 1381 RVA: 0x00014DA8 File Offset: 0x00012FA8
	private void RpcReader___Observers_ObsKnightDonged_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsKnightDonged_2166136261();
	}

	// Token: 0x06000566 RID: 1382 RVA: 0x00014A1B File Offset: 0x00012C1B
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0400029A RID: 666
	public FallenKnight fk;

	// Token: 0x0400029B RID: 667
	public KnightDong kd;

	// Token: 0x0400029C RID: 668
	private bool NetworkInitialize___EarlyFallenKnightNetAssembly-CSharp.dll_Excuted;

	// Token: 0x0400029D RID: 669
	private bool NetworkInitialize__LateFallenKnightNetAssembly-CSharp.dll_Excuted;
}
