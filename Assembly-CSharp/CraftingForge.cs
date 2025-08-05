using System;
using System.Collections;
using System.Linq;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000039 RID: 57
public class CraftingForge : NetworkBehaviour
{
	// Token: 0x06000275 RID: 629 RVA: 0x0000AD48 File Offset: 0x00008F48
	public override void OnStartClient()
	{
		base.OnStartClient();
		if (!base.HasAuthority)
		{
			base.enabled = false;
			return;
		}
		this.SlotItems[0] = null;
		this.SlotItems[1] = null;
		if (this.Soupman != null)
		{
			this.SetShitActiveidk();
		}
		base.StartCoroutine(this.CrafterRoutine());
		if (this.Soupman != null)
		{
			base.StartCoroutine(this.ActivateRenders());
		}
	}

	// Token: 0x06000276 RID: 630 RVA: 0x0000ADB9 File Offset: 0x00008FB9
	private IEnumerator ActivateRenders()
	{
		yield return new WaitForSeconds(5f);
		MeshRenderer[] componentsInChildren = this.Soupman.GetComponentsInChildren<MeshRenderer>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = true;
		}
		yield break;
	}

	// Token: 0x06000277 RID: 631 RVA: 0x0000ADC8 File Offset: 0x00008FC8
	[ServerRpc(RequireOwnership = false)]
	private void SetShitActiveidk()
	{
		this.RpcWriter___Server_SetShitActiveidk_2166136261();
	}

	// Token: 0x06000278 RID: 632 RVA: 0x0000ADDB File Offset: 0x00008FDB
	[ObserversRpc]
	private void doshiteza()
	{
		this.RpcWriter___Observers_doshiteza_2166136261();
	}

	// Token: 0x06000279 RID: 633 RVA: 0x0000ADE3 File Offset: 0x00008FE3
	private IEnumerator CrafterRoutine()
	{
		while (base.isActiveAndEnabled)
		{
			if (this.SlotItems[0] != null && this.SlotItems[1] != null && this.craftingComplete && this.craftableids.Contains(this.SlotItems[0].GetComponent<IItemInteraction>().GetItemID()) && this.craftableids.Contains(this.SlotItems[1].GetComponent<IItemInteraction>().GetItemID()))
			{
				this.craftingComplete = false;
				this.ServerCraft();
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600027A RID: 634 RVA: 0x0000ADF2 File Offset: 0x00008FF2
	public void Craft()
	{
		if (this.SlotItems[0] != null && this.SlotItems[1] != null)
		{
			this.ServerCraft();
		}
	}

	// Token: 0x0600027B RID: 635 RVA: 0x0000AE1A File Offset: 0x0000901A
	[ObserversRpc]
	private void makepoof()
	{
		this.RpcWriter___Observers_makepoof_2166136261();
	}

	// Token: 0x0600027C RID: 636 RVA: 0x0000AE24 File Offset: 0x00009024
	[ServerRpc(RequireOwnership = false)]
	private void ServerCraft()
	{
		this.RpcWriter___Server_ServerCraft_2166136261();
	}

	// Token: 0x0600027D RID: 637 RVA: 0x0000AE37 File Offset: 0x00009037
	[ObserversRpc]
	private void CraftCleanUp()
	{
		this.RpcWriter___Observers_CraftCleanUp_2166136261();
	}

	// Token: 0x0600027F RID: 639 RVA: 0x0000AE74 File Offset: 0x00009074
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyCraftingForgeAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyCraftingForgeAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_SetShitActiveidk_2166136261));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_doshiteza_2166136261));
		base.RegisterObserversRpc(2U, new ClientRpcDelegate(this.RpcReader___Observers_makepoof_2166136261));
		base.RegisterServerRpc(3U, new ServerRpcDelegate(this.RpcReader___Server_ServerCraft_2166136261));
		base.RegisterObserversRpc(4U, new ClientRpcDelegate(this.RpcReader___Observers_CraftCleanUp_2166136261));
	}

	// Token: 0x06000280 RID: 640 RVA: 0x0000AF05 File Offset: 0x00009105
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateCraftingForgeAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateCraftingForgeAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000281 RID: 641 RVA: 0x0000AF18 File Offset: 0x00009118
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000282 RID: 642 RVA: 0x0000AF28 File Offset: 0x00009128
	private void RpcWriter___Server_SetShitActiveidk_2166136261()
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

	// Token: 0x06000283 RID: 643 RVA: 0x0000AF90 File Offset: 0x00009190
	private void RpcLogic___SetShitActiveidk_2166136261()
	{
		base.ServerManager.Spawn(this.CraftInteracter1.gameObject, null, default(Scene));
		base.ServerManager.Spawn(this.CraftInteracter2.gameObject, null, default(Scene));
		base.ServerManager.Spawn(this.Soupman, null, default(Scene));
		base.ServerManager.Spawn(this.MagicMirror, null, default(Scene));
		this.doshiteza();
	}

	// Token: 0x06000284 RID: 644 RVA: 0x0000B01C File Offset: 0x0000921C
	private void RpcReader___Server_SetShitActiveidk_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___SetShitActiveidk_2166136261();
	}

	// Token: 0x06000285 RID: 645 RVA: 0x0000B03C File Offset: 0x0000923C
	private void RpcWriter___Observers_doshiteza_2166136261()
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

	// Token: 0x06000286 RID: 646 RVA: 0x0000B0B0 File Offset: 0x000092B0
	private void RpcLogic___doshiteza_2166136261()
	{
		this.CraftInteracter1.itemPlaced = false;
		this.CraftInteracter2.itemPlaced = false;
		this.SlotItems[0] = null;
		this.SlotItems[1] = null;
	}

	// Token: 0x06000287 RID: 647 RVA: 0x0000B0DC File Offset: 0x000092DC
	private void RpcReader___Observers_doshiteza_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___doshiteza_2166136261();
	}

	// Token: 0x06000288 RID: 648 RVA: 0x0000B0FC File Offset: 0x000092FC
	private void RpcWriter___Observers_makepoof_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(2U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000289 RID: 649 RVA: 0x0000B170 File Offset: 0x00009370
	private void RpcLogic___makepoof_2166136261()
	{
		this.asauce.PlayOneShot(this.poofta);
		Object.Instantiate<GameObject>(this.Poof).transform.position = this.itemSpawnPoint.position;
	}

	// Token: 0x0600028A RID: 650 RVA: 0x0000B1A4 File Offset: 0x000093A4
	private void RpcReader___Observers_makepoof_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___makepoof_2166136261();
	}

	// Token: 0x0600028B RID: 651 RVA: 0x0000B1C4 File Offset: 0x000093C4
	private void RpcWriter___Server_ServerCraft_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(3U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600028C RID: 652 RVA: 0x0000B22C File Offset: 0x0000942C
	private void RpcLogic___ServerCraft_2166136261()
	{
		if (this.SlotItems[0] != null && this.SlotItems[1] != null)
		{
			this.makepoof();
			int num = this.SlotItems[0].GetComponent<IItemInteraction>().GetItemID() * 10 + this.SlotItems[1].GetComponent<IItemInteraction>().GetItemID();
			switch (num)
			{
			case 0:
			{
				base.ServerManager.Despawn(this.SlotItems[0], null);
				base.ServerManager.Despawn(this.SlotItems[1], null);
				GameObject gameObject = Object.Instantiate<GameObject>(this.CraftablePrefabs[0]);
				gameObject.transform.position = this.itemSpawnPoint.position;
				base.ServerManager.Spawn(gameObject, null, default(Scene));
				this.didCraft = true;
				break;
			}
			case 1:
			case 10:
			{
				base.ServerManager.Despawn(this.SlotItems[0], null);
				base.ServerManager.Despawn(this.SlotItems[1], null);
				GameObject gameObject2 = Object.Instantiate<GameObject>(this.CraftablePrefabs[1]);
				gameObject2.transform.position = this.itemSpawnPoint.position;
				base.ServerManager.Spawn(gameObject2, null, default(Scene));
				this.didCraft = true;
				break;
			}
			case 2:
			case 20:
			{
				base.ServerManager.Despawn(this.SlotItems[0], null);
				base.ServerManager.Despawn(this.SlotItems[1], null);
				GameObject gameObject3 = Object.Instantiate<GameObject>(this.CraftablePrefabs[9]);
				gameObject3.transform.position = this.itemSpawnPoint.position;
				base.ServerManager.Spawn(gameObject3, null, default(Scene));
				this.didCraft = true;
				break;
			}
			case 3:
			case 30:
			{
				base.ServerManager.Despawn(this.SlotItems[0], null);
				base.ServerManager.Despawn(this.SlotItems[1], null);
				GameObject gameObject4 = Object.Instantiate<GameObject>(this.CraftablePrefabs[8]);
				gameObject4.transform.position = this.itemSpawnPoint.position;
				base.ServerManager.Spawn(gameObject4, null, default(Scene));
				this.didCraft = true;
				break;
			}
			case 4:
			case 40:
			{
				base.ServerManager.Despawn(this.SlotItems[0], null);
				base.ServerManager.Despawn(this.SlotItems[1], null);
				GameObject gameObject5 = Object.Instantiate<GameObject>(this.CraftablePrefabs[7]);
				gameObject5.transform.position = this.itemSpawnPoint.position;
				base.ServerManager.Spawn(gameObject5, null, default(Scene));
				this.didCraft = true;
				break;
			}
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 15:
			case 16:
			case 17:
			case 18:
			case 19:
			case 25:
			case 26:
			case 27:
			case 28:
			case 29:
			case 35:
			case 36:
			case 37:
			case 38:
			case 39:
				break;
			case 11:
			{
				base.ServerManager.Despawn(this.SlotItems[0], null);
				base.ServerManager.Despawn(this.SlotItems[1], null);
				GameObject gameObject6 = Object.Instantiate<GameObject>(this.CraftablePrefabs[2]);
				gameObject6.transform.position = this.itemSpawnPoint.position;
				base.ServerManager.Spawn(gameObject6, null, default(Scene));
				this.didCraft = true;
				break;
			}
			case 12:
			case 21:
			{
				base.ServerManager.Despawn(this.SlotItems[0], null);
				base.ServerManager.Despawn(this.SlotItems[1], null);
				GameObject gameObject7 = Object.Instantiate<GameObject>(this.CraftablePrefabs[3]);
				gameObject7.transform.position = this.itemSpawnPoint.position;
				base.ServerManager.Spawn(gameObject7, null, default(Scene));
				this.didCraft = true;
				break;
			}
			case 13:
			case 31:
			{
				base.ServerManager.Despawn(this.SlotItems[0], null);
				base.ServerManager.Despawn(this.SlotItems[1], null);
				GameObject gameObject8 = Object.Instantiate<GameObject>(this.CraftablePrefabs[15]);
				gameObject8.transform.position = this.itemSpawnPoint.position;
				base.ServerManager.Spawn(gameObject8, null, default(Scene));
				this.didCraft = true;
				break;
			}
			case 14:
			case 41:
			{
				base.ServerManager.Despawn(this.SlotItems[0], null);
				base.ServerManager.Despawn(this.SlotItems[1], null);
				GameObject gameObject9 = Object.Instantiate<GameObject>(this.CraftablePrefabs[10]);
				gameObject9.transform.position = this.itemSpawnPoint.position;
				base.ServerManager.Spawn(gameObject9, null, default(Scene));
				this.didCraft = true;
				break;
			}
			case 22:
			{
				base.ServerManager.Despawn(this.SlotItems[0], null);
				base.ServerManager.Despawn(this.SlotItems[1], null);
				GameObject gameObject10 = Object.Instantiate<GameObject>(this.CraftablePrefabs[12]);
				gameObject10.transform.position = this.itemSpawnPoint.position;
				base.ServerManager.Spawn(gameObject10, null, default(Scene));
				this.didCraft = true;
				break;
			}
			case 23:
			case 32:
			{
				base.ServerManager.Despawn(this.SlotItems[0], null);
				base.ServerManager.Despawn(this.SlotItems[1], null);
				GameObject gameObject11 = Object.Instantiate<GameObject>(this.CraftablePrefabs[13]);
				gameObject11.transform.position = this.itemSpawnPoint.position;
				base.ServerManager.Spawn(gameObject11, null, default(Scene));
				this.didCraft = true;
				break;
			}
			case 24:
			case 42:
			{
				base.ServerManager.Despawn(this.SlotItems[0], null);
				base.ServerManager.Despawn(this.SlotItems[1], null);
				GameObject gameObject12 = Object.Instantiate<GameObject>(this.CraftablePrefabs[11]);
				gameObject12.transform.position = this.itemSpawnPoint.position;
				base.ServerManager.Spawn(gameObject12, null, default(Scene));
				this.didCraft = true;
				break;
			}
			case 33:
			{
				base.ServerManager.Despawn(this.SlotItems[0], null);
				base.ServerManager.Despawn(this.SlotItems[1], null);
				GameObject gameObject13 = Object.Instantiate<GameObject>(this.CraftablePrefabs[4]);
				gameObject13.transform.position = this.itemSpawnPoint.position;
				base.ServerManager.Spawn(gameObject13, null, default(Scene));
				this.didCraft = true;
				break;
			}
			case 34:
			case 43:
			{
				base.ServerManager.Despawn(this.SlotItems[0], null);
				base.ServerManager.Despawn(this.SlotItems[1], null);
				GameObject gameObject14 = Object.Instantiate<GameObject>(this.CraftablePrefabs[5]);
				gameObject14.transform.position = this.itemSpawnPoint.position;
				base.ServerManager.Spawn(gameObject14, null, default(Scene));
				this.didCraft = true;
				break;
			}
			case 44:
			{
				base.ServerManager.Despawn(this.SlotItems[0], null);
				base.ServerManager.Despawn(this.SlotItems[1], null);
				GameObject gameObject15 = Object.Instantiate<GameObject>(this.CraftablePrefabs[6]);
				gameObject15.transform.position = this.itemSpawnPoint.position;
				base.ServerManager.Spawn(gameObject15, null, default(Scene));
				this.didCraft = true;
				break;
			}
			default:
				if (num == 89 || num == 98)
				{
					base.ServerManager.Despawn(this.SlotItems[0], null);
					base.ServerManager.Despawn(this.SlotItems[1], null);
					GameObject gameObject16 = Object.Instantiate<GameObject>(this.CraftablePrefabs[14]);
					gameObject16.transform.position = this.itemSpawnPoint.position;
					base.ServerManager.Spawn(gameObject16, null, default(Scene));
					this.didCraft = true;
				}
				break;
			}
			if (this.didCraft)
			{
				this.didCraft = false;
				this.SlotItems[0] = null;
				this.SlotItems[1] = null;
				this.CraftCleanUp();
			}
		}
		this.craftingComplete = true;
	}

	// Token: 0x0600028D RID: 653 RVA: 0x0000BBC4 File Offset: 0x00009DC4
	private void RpcReader___Server_ServerCraft_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerCraft_2166136261();
	}

	// Token: 0x0600028E RID: 654 RVA: 0x0000BBE4 File Offset: 0x00009DE4
	private void RpcWriter___Observers_CraftCleanUp_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(4U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600028F RID: 655 RVA: 0x0000BC58 File Offset: 0x00009E58
	private void RpcLogic___CraftCleanUp_2166136261()
	{
		this.SlotItems[0] = null;
		this.SlotItems[1] = null;
		this.CraftInteracter1.clearItem();
		this.CraftInteracter2.clearItem();
	}

	// Token: 0x06000290 RID: 656 RVA: 0x0000BC84 File Offset: 0x00009E84
	private void RpcReader___Observers_CraftCleanUp_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___CraftCleanUp_2166136261();
	}

	// Token: 0x06000291 RID: 657 RVA: 0x0000AF18 File Offset: 0x00009118
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000139 RID: 313
	public Transform worldObjectHolder;

	// Token: 0x0400013A RID: 314
	public CraftInteracter CraftInteracter1;

	// Token: 0x0400013B RID: 315
	public CraftInteracter CraftInteracter2;

	// Token: 0x0400013C RID: 316
	public GameObject[] SlotItems = new GameObject[2];

	// Token: 0x0400013D RID: 317
	public GameObject[] CraftablePrefabs;

	// Token: 0x0400013E RID: 318
	public Transform itemSpawnPoint;

	// Token: 0x0400013F RID: 319
	private bool didCraft;

	// Token: 0x04000140 RID: 320
	private bool craftingComplete = true;

	// Token: 0x04000141 RID: 321
	public GameObject Poof;

	// Token: 0x04000142 RID: 322
	public AudioSource asauce;

	// Token: 0x04000143 RID: 323
	public AudioClip poofta;

	// Token: 0x04000144 RID: 324
	private int[] craftableids = new int[] { 0, 1, 2, 3, 4, 8, 9 };

	// Token: 0x04000145 RID: 325
	public GameObject Soupman;

	// Token: 0x04000146 RID: 326
	public GameObject MagicMirror;

	// Token: 0x04000147 RID: 327
	private bool NetworkInitialize___EarlyCraftingForgeAssembly-CSharp.dll_Excuted;

	// Token: 0x04000148 RID: 328
	private bool NetworkInitialize__LateCraftingForgeAssembly-CSharp.dll_Excuted;
}
