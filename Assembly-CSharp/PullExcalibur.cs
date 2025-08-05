using System;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000149 RID: 329
public class PullExcalibur : NetworkBehaviour, ITimedInteractable
{
	// Token: 0x06000E40 RID: 3648 RVA: 0x0003A368 File Offset: 0x00038568
	public string DisplayInteractUI()
	{
		if (this.BladeGone)
		{
			return "";
		}
		if (this.isbladenothilt)
		{
			return "Pull Shattered Blade of Excalibur";
		}
		return "Pull Hilt of Excalibur";
	}

	// Token: 0x06000E41 RID: 3649 RVA: 0x0003A38B File Offset: 0x0003858B
	public float GetInteractTimer(GameObject player)
	{
		return 10f;
	}

	// Token: 0x06000E42 RID: 3650 RVA: 0x0003A394 File Offset: 0x00038594
	public void Interact(GameObject player)
	{
		PlayerInventory playerInventory;
		if (player.TryGetComponent<PlayerInventory>(out playerInventory) && playerInventory.tryFindSlot() != -1)
		{
			this.BladePull(player);
		}
	}

	// Token: 0x06000E43 RID: 3651 RVA: 0x0003A3BC File Offset: 0x000385BC
	[ServerRpc(RequireOwnership = false)]
	private void BladePull(GameObject player)
	{
		this.RpcWriter___Server_BladePull_1934289915(player);
	}

	// Token: 0x06000E44 RID: 3652 RVA: 0x0003A3D4 File Offset: 0x000385D4
	[ObserversRpc]
	private void ObsBladePull(GameObject player, GameObject spawnedobj)
	{
		this.RpcWriter___Observers_ObsBladePull_1401332417(player, spawnedobj);
	}

	// Token: 0x06000E46 RID: 3654 RVA: 0x0003A3F0 File Offset: 0x000385F0
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyPullExcaliburAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyPullExcaliburAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_BladePull_1934289915));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_ObsBladePull_1401332417));
	}

	// Token: 0x06000E47 RID: 3655 RVA: 0x0003A43C File Offset: 0x0003863C
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LatePullExcaliburAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LatePullExcaliburAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000E48 RID: 3656 RVA: 0x0003A44F File Offset: 0x0003864F
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000E49 RID: 3657 RVA: 0x0003A460 File Offset: 0x00038660
	private void RpcWriter___Server_BladePull_1934289915(GameObject player)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(player);
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000E4A RID: 3658 RVA: 0x0003A4D4 File Offset: 0x000386D4
	private void RpcLogic___BladePull_1934289915(GameObject player)
	{
		if (!this.BladeGone)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.ExBlade);
			base.ServerManager.Spawn(gameObject, null, default(Scene));
			this.BladeGone = true;
			this.ObsBladePull(player, gameObject);
		}
	}

	// Token: 0x06000E4B RID: 3659 RVA: 0x0003A51C File Offset: 0x0003871C
	private void RpcReader___Server_BladePull_1934289915(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___BladePull_1934289915(gameObject);
	}

	// Token: 0x06000E4C RID: 3660 RVA: 0x0003A550 File Offset: 0x00038750
	private void RpcWriter___Observers_ObsBladePull_1401332417(GameObject player, GameObject spawnedobj)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(player);
		pooledWriter.WriteGameObject(spawnedobj);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000E4D RID: 3661 RVA: 0x0003A5E0 File Offset: 0x000387E0
	private void RpcLogic___ObsBladePull_1401332417(GameObject player, GameObject spawnedobj)
	{
		PlayerInventory playerInventory;
		if (player.TryGetComponent<PlayerInventory>(out playerInventory) && playerInventory.IsOwner)
		{
			playerInventory.Pickup(spawnedobj);
		}
		this.cllider.enabled = false;
		this.VisualBlade.SetActive(false);
		this.BladeGone = true;
	}

	// Token: 0x06000E4E RID: 3662 RVA: 0x0003A628 File Offset: 0x00038828
	private void RpcReader___Observers_ObsBladePull_1401332417(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		GameObject gameObject2 = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsBladePull_1401332417(gameObject, gameObject2);
	}

	// Token: 0x06000E4F RID: 3663 RVA: 0x0003A44F File Offset: 0x0003864F
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x040007D3 RID: 2003
	public GameObject ExBlade;

	// Token: 0x040007D4 RID: 2004
	private bool BladeGone;

	// Token: 0x040007D5 RID: 2005
	public GameObject VisualBlade;

	// Token: 0x040007D6 RID: 2006
	public Collider cllider;

	// Token: 0x040007D7 RID: 2007
	public bool isbladenothilt;

	// Token: 0x040007D8 RID: 2008
	private bool NetworkInitialize___EarlyPullExcaliburAssembly-CSharp.dll_Excuted;

	// Token: 0x040007D9 RID: 2009
	private bool NetworkInitialize__LatePullExcaliburAssembly-CSharp.dll_Excuted;
}
