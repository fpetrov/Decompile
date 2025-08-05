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

// Token: 0x0200002E RID: 46
public class ChestNetController1 : NetworkBehaviour
{
	// Token: 0x06000202 RID: 514 RVA: 0x000096CA File Offset: 0x000078CA
	public override void OnStartClient()
	{
		if (base.HasAuthority)
		{
			base.StartCoroutine(this.LootRoutine());
		}
		base.StartCoroutine(this.WaitforNetItemManager());
	}

	// Token: 0x06000203 RID: 515 RVA: 0x000096EE File Offset: 0x000078EE
	private IEnumerator WaitforNetItemManager()
	{
		GameObject NIM = null;
		while (NIM == null)
		{
			NIM = GameObject.FindGameObjectWithTag("NetItemManager");
			yield return null;
		}
		this.plt = NIM.GetComponent<PageLootTable>();
		yield break;
	}

	// Token: 0x06000204 RID: 516 RVA: 0x000096FD File Offset: 0x000078FD
	private IEnumerator LootRoutine()
	{
		yield return new WaitForSeconds(5f);
		for (int i = 0; i < 4; i++)
		{
			this.PlaceItemIn(Random.Range(0, this.plt.Pages.Length), i);
		}
		yield break;
	}

	// Token: 0x06000205 RID: 517 RVA: 0x0000970C File Offset: 0x0000790C
	public void ChestInteract(int id)
	{
		this.pisever(id);
	}

	// Token: 0x06000206 RID: 518 RVA: 0x00009715 File Offset: 0x00007915
	[ServerRpc(RequireOwnership = false)]
	private void pisever(int id)
	{
		this.RpcWriter___Server_pisever_3316948804(id);
	}

	// Token: 0x06000207 RID: 519 RVA: 0x00009721 File Offset: 0x00007921
	[ObserversRpc]
	private void piObservers(int id)
	{
		this.RpcWriter___Observers_piObservers_3316948804(id);
	}

	// Token: 0x06000208 RID: 520 RVA: 0x0000972D File Offset: 0x0000792D
	private IEnumerator OpenChest(int id)
	{
		this.pcCol[id].enabled = false;
		Quaternion startRot = this.Chest[id].transform.localRotation;
		Quaternion targetRot = Quaternion.Euler(-200f, 90f, -90f);
		float duration = 0.5f;
		float elapsedTime = 0f;
		while (elapsedTime < duration)
		{
			this.Chest[id].transform.localRotation = Quaternion.Slerp(startRot, targetRot, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		this.Chest[id].transform.localRotation = targetRot;
		yield return new WaitForSeconds(120f);
		startRot = this.Chest[id].transform.localRotation;
		targetRot = Quaternion.Euler(-90f, 90f, -90f);
		duration = 0.5f;
		elapsedTime = 0f;
		this.Chest[id].GetComponent<ChestInteract1>().csource.PlayOneShot(this.chestClips[1]);
		while (elapsedTime < duration)
		{
			this.Chest[id].transform.localRotation = Quaternion.Slerp(startRot, targetRot, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		this.pcCol[id].enabled = true;
		this.Chest[id].transform.localRotation = targetRot;
		this.isOpen[id] = false;
		if (base.HasAuthority)
		{
			this.PlaceItemIn(Random.Range(0, this.plt.Pages.Length), id);
		}
		yield break;
	}

	// Token: 0x06000209 RID: 521 RVA: 0x00009743 File Offset: 0x00007943
	public void PlaceItemIn(int itemid, int id)
	{
		this.ServerPlaceItem(itemid, id);
	}

	// Token: 0x0600020A RID: 522 RVA: 0x00009750 File Offset: 0x00007950
	[ServerRpc(RequireOwnership = false)]
	private void ServerPlaceItem(int itemid, int id)
	{
		this.RpcWriter___Server_ServerPlaceItem_1692629761(itemid, id);
	}

	// Token: 0x0600020B RID: 523 RVA: 0x0000976B File Offset: 0x0000796B
	public void LockPick(bool onoff, int id)
	{
		this.ChestLockPickSound(onoff, id);
	}

	// Token: 0x0600020C RID: 524 RVA: 0x00009775 File Offset: 0x00007975
	[ServerRpc(RequireOwnership = false)]
	private void ChestLockPickSound(bool onoff, int id)
	{
		this.RpcWriter___Server_ChestLockPickSound_3648907713(onoff, id);
	}

	// Token: 0x0600020D RID: 525 RVA: 0x00009785 File Offset: 0x00007985
	[ObserversRpc]
	private void ChestLockPickSoundObs(bool onoff, int id)
	{
		this.RpcWriter___Observers_ChestLockPickSoundObs_3648907713(onoff, id);
	}

	// Token: 0x0600020F RID: 527 RVA: 0x00009798 File Offset: 0x00007998
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyChestNetController1Assembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyChestNetController1Assembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_pisever_3316948804));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_piObservers_3316948804));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_ServerPlaceItem_1692629761));
		base.RegisterServerRpc(3U, new ServerRpcDelegate(this.RpcReader___Server_ChestLockPickSound_3648907713));
		base.RegisterObserversRpc(4U, new ClientRpcDelegate(this.RpcReader___Observers_ChestLockPickSoundObs_3648907713));
	}

	// Token: 0x06000210 RID: 528 RVA: 0x00009829 File Offset: 0x00007A29
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateChestNetController1Assembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateChestNetController1Assembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000211 RID: 529 RVA: 0x0000983C File Offset: 0x00007A3C
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000212 RID: 530 RVA: 0x0000984C File Offset: 0x00007A4C
	private void RpcWriter___Server_pisever_3316948804(int id)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(id);
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000213 RID: 531 RVA: 0x000098BE File Offset: 0x00007ABE
	private void RpcLogic___pisever_3316948804(int id)
	{
		this.piObservers(id);
	}

	// Token: 0x06000214 RID: 532 RVA: 0x000098C8 File Offset: 0x00007AC8
	private void RpcReader___Server_pisever_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___pisever_3316948804(num);
	}

	// Token: 0x06000215 RID: 533 RVA: 0x000098FC File Offset: 0x00007AFC
	private void RpcWriter___Observers_piObservers_3316948804(int id)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(id);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000216 RID: 534 RVA: 0x0000997D File Offset: 0x00007B7D
	private void RpcLogic___piObservers_3316948804(int id)
	{
		base.StartCoroutine(this.OpenChest(id));
		this.Chest[id].GetComponent<ChestInteract1>().csource.PlayOneShot(this.chestClips[0]);
		this.isOpen[id] = true;
	}

	// Token: 0x06000217 RID: 535 RVA: 0x000099B8 File Offset: 0x00007BB8
	private void RpcReader___Observers_piObservers_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___piObservers_3316948804(num);
	}

	// Token: 0x06000218 RID: 536 RVA: 0x000099EC File Offset: 0x00007BEC
	private void RpcWriter___Server_ServerPlaceItem_1692629761(int itemid, int id)
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
		pooledWriter.WriteInt32(id);
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000219 RID: 537 RVA: 0x00009A6C File Offset: 0x00007C6C
	private void RpcLogic___ServerPlaceItem_1692629761(int itemid, int id)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.plt.Pages[itemid]);
		gameObject.transform.position = this.ItemPoint[id].transform.position;
		gameObject.transform.localRotation = Quaternion.Euler(10f, 0f, 180f);
		base.ServerManager.Spawn(gameObject, null, default(Scene));
	}

	// Token: 0x0600021A RID: 538 RVA: 0x00009AE0 File Offset: 0x00007CE0
	private void RpcReader___Server_ServerPlaceItem_1692629761(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		int num2 = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerPlaceItem_1692629761(num, num2);
	}

	// Token: 0x0600021B RID: 539 RVA: 0x00009B24 File Offset: 0x00007D24
	private void RpcWriter___Server_ChestLockPickSound_3648907713(bool onoff, int id)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteBoolean(onoff);
		pooledWriter.WriteInt32(id);
		base.SendServerRpc(3U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600021C RID: 540 RVA: 0x00009BA3 File Offset: 0x00007DA3
	private void RpcLogic___ChestLockPickSound_3648907713(bool onoff, int id)
	{
		this.ChestLockPickSoundObs(onoff, id);
	}

	// Token: 0x0600021D RID: 541 RVA: 0x00009BB0 File Offset: 0x00007DB0
	private void RpcReader___Server_ChestLockPickSound_3648907713(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		bool flag = PooledReader0.ReadBoolean();
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ChestLockPickSound_3648907713(flag, num);
	}

	// Token: 0x0600021E RID: 542 RVA: 0x00009BF4 File Offset: 0x00007DF4
	private void RpcWriter___Observers_ChestLockPickSoundObs_3648907713(bool onoff, int id)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteBoolean(onoff);
		pooledWriter.WriteInt32(id);
		base.SendObserversRpc(4U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600021F RID: 543 RVA: 0x00009C82 File Offset: 0x00007E82
	private void RpcLogic___ChestLockPickSoundObs_3648907713(bool onoff, int id)
	{
		if (onoff)
		{
			this.Chest[id].GetComponent<ChestInteract1>().csource.Play();
			return;
		}
		this.Chest[id].GetComponent<ChestInteract1>().csource.Stop();
	}

	// Token: 0x06000220 RID: 544 RVA: 0x00009CB8 File Offset: 0x00007EB8
	private void RpcReader___Observers_ChestLockPickSoundObs_3648907713(PooledReader PooledReader0, Channel channel)
	{
		bool flag = PooledReader0.ReadBoolean();
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ChestLockPickSoundObs_3648907713(flag, num);
	}

	// Token: 0x06000221 RID: 545 RVA: 0x0000983C File Offset: 0x00007A3C
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000105 RID: 261
	public Transform[] Chest;

	// Token: 0x04000106 RID: 262
	public bool[] isOpen;

	// Token: 0x04000107 RID: 263
	public Collider[] pcCol;

	// Token: 0x04000108 RID: 264
	public Transform[] ItemPoint;

	// Token: 0x04000109 RID: 265
	public GameObject[] Items;

	// Token: 0x0400010A RID: 266
	public AudioClip[] chestClips;

	// Token: 0x0400010B RID: 267
	private PageLootTable plt;

	// Token: 0x0400010C RID: 268
	private bool NetworkInitialize___EarlyChestNetController1Assembly-CSharp.dll_Excuted;

	// Token: 0x0400010D RID: 269
	private bool NetworkInitialize__LateChestNetController1Assembly-CSharp.dll_Excuted;
}
