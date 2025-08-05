using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200002B RID: 43
public class ChestNetController : NetworkBehaviour
{
	// Token: 0x060001C1 RID: 449 RVA: 0x00008667 File Offset: 0x00006867
	public override void OnStartClient()
	{
		base.OnStartClient();
		this.isOpen = false;
		base.StartCoroutine(this.CloseChest());
	}

	// Token: 0x060001C2 RID: 450 RVA: 0x00008683 File Offset: 0x00006883
	public void ChestInteract()
	{
		this.pisever();
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x0000868C File Offset: 0x0000688C
	[ServerRpc(RequireOwnership = false)]
	private void pisever()
	{
		this.RpcWriter___Server_pisever_2166136261();
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x000086A0 File Offset: 0x000068A0
	[ObserversRpc]
	private void piObservers()
	{
		this.RpcWriter___Observers_piObservers_2166136261();
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x000086B3 File Offset: 0x000068B3
	private IEnumerator OpenChest()
	{
		this.pcCol.enabled = false;
		Quaternion startRot = this.Chest.transform.localRotation;
		Quaternion targetRot = Quaternion.Euler(-200f, 90f, -90f);
		float duration = 0.5f;
		float elapsedTime = 0f;
		while (elapsedTime < duration)
		{
			this.Chest.transform.localRotation = Quaternion.Slerp(startRot, targetRot, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		this.pcCol.enabled = true;
		this.Chest.transform.localRotation = targetRot;
		yield break;
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x000086C2 File Offset: 0x000068C2
	private IEnumerator CloseChest()
	{
		this.pcCol.enabled = false;
		Quaternion startRot = this.Chest.transform.localRotation;
		Quaternion targetRot = Quaternion.Euler(-90f, 90f, -90f);
		float duration = 0.5f;
		float elapsedTime = 0f;
		while (elapsedTime < duration)
		{
			this.Chest.transform.localRotation = Quaternion.Slerp(startRot, targetRot, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		this.pcCol.enabled = true;
		this.Chest.transform.localRotation = targetRot;
		yield break;
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x000086D1 File Offset: 0x000068D1
	public void SpawnLacky(int itemid, Vector3 spawnpos)
	{
		if (itemid >= 0)
		{
			this.ServerSpawnLacky(itemid, spawnpos);
		}
	}

	// Token: 0x060001C8 RID: 456 RVA: 0x000086DF File Offset: 0x000068DF
	[ServerRpc(RequireOwnership = false)]
	private void ServerSpawnLacky(int id, Vector3 spawnpos)
	{
		this.RpcWriter___Server_ServerSpawnLacky_215135683(id, spawnpos);
	}

	// Token: 0x060001C9 RID: 457 RVA: 0x000086F0 File Offset: 0x000068F0
	[ObserversRpc]
	private void SpawnLackyObservers(int id, Vector3 spawnpos, int lackyidd)
	{
		this.RpcWriter___Observers_SpawnLackyObservers_4194131050(id, spawnpos, lackyidd);
	}

	// Token: 0x060001CA RID: 458 RVA: 0x00008710 File Offset: 0x00006910
	[ObserversRpc]
	private void levelupPlayer()
	{
		this.RpcWriter___Observers_levelupPlayer_2166136261();
	}

	// Token: 0x060001CB RID: 459 RVA: 0x00008723 File Offset: 0x00006923
	public void LackyDropItem(Vector3 lackypos, int lackytype)
	{
		if (base.HasAuthority)
		{
			this.ServerLackDropItem(lackypos, lackytype);
		}
	}

	// Token: 0x060001CC RID: 460 RVA: 0x00008738 File Offset: 0x00006938
	[ServerRpc(RequireOwnership = false)]
	private void ServerLackDropItem(Vector3 lackypos, int lackytype)
	{
		this.RpcWriter___Server_ServerLackDropItem_953814113(lackypos, lackytype);
	}

	// Token: 0x060001CD RID: 461 RVA: 0x00008753 File Offset: 0x00006953
	public void KillLacky(int lackyid)
	{
		this.ServerKillLacky(lackyid);
	}

	// Token: 0x060001CE RID: 462 RVA: 0x0000875C File Offset: 0x0000695C
	[ServerRpc(RequireOwnership = false)]
	private void ServerKillLacky(int lackyid)
	{
		this.RpcWriter___Server_ServerKillLacky_3316948804(lackyid);
	}

	// Token: 0x060001CF RID: 463 RVA: 0x00008768 File Offset: 0x00006968
	[ObserversRpc]
	private void ObsKillLacky(int lackyid)
	{
		this.RpcWriter___Observers_ObsKillLacky_3316948804(lackyid);
	}

	// Token: 0x060001D0 RID: 464 RVA: 0x0000877F File Offset: 0x0000697F
	public void LockPick(bool onoff)
	{
		this.ChestLockPickSound(onoff);
	}

	// Token: 0x060001D1 RID: 465 RVA: 0x00008788 File Offset: 0x00006988
	[ServerRpc(RequireOwnership = false)]
	private void ChestLockPickSound(bool onoff)
	{
		this.RpcWriter___Server_ChestLockPickSound_1140765316(onoff);
	}

	// Token: 0x060001D2 RID: 466 RVA: 0x00008794 File Offset: 0x00006994
	[ObserversRpc]
	private void ChestLockPickSoundObs(bool onoff)
	{
		this.RpcWriter___Observers_ChestLockPickSoundObs_1140765316(onoff);
	}

	// Token: 0x060001D4 RID: 468 RVA: 0x000087B4 File Offset: 0x000069B4
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyChestNetControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyChestNetControllerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_pisever_2166136261));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_piObservers_2166136261));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_ServerSpawnLacky_215135683));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_SpawnLackyObservers_4194131050));
		base.RegisterObserversRpc(4U, new ClientRpcDelegate(this.RpcReader___Observers_levelupPlayer_2166136261));
		base.RegisterServerRpc(5U, new ServerRpcDelegate(this.RpcReader___Server_ServerLackDropItem_953814113));
		base.RegisterServerRpc(6U, new ServerRpcDelegate(this.RpcReader___Server_ServerKillLacky_3316948804));
		base.RegisterObserversRpc(7U, new ClientRpcDelegate(this.RpcReader___Observers_ObsKillLacky_3316948804));
		base.RegisterServerRpc(8U, new ServerRpcDelegate(this.RpcReader___Server_ChestLockPickSound_1140765316));
		base.RegisterObserversRpc(9U, new ClientRpcDelegate(this.RpcReader___Observers_ChestLockPickSoundObs_1140765316));
	}

	// Token: 0x060001D5 RID: 469 RVA: 0x000088B8 File Offset: 0x00006AB8
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateChestNetControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateChestNetControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x060001D6 RID: 470 RVA: 0x000088CB File Offset: 0x00006ACB
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x060001D7 RID: 471 RVA: 0x000088DC File Offset: 0x00006ADC
	private void RpcWriter___Server_pisever_2166136261()
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

	// Token: 0x060001D8 RID: 472 RVA: 0x00008944 File Offset: 0x00006B44
	private void RpcLogic___pisever_2166136261()
	{
		if (!this.hasBeenOpened)
		{
			this.hasBeenOpened = true;
			GameObject gameObject = Object.Instantiate<GameObject>(this.Items[Random.Range(0, this.Items.Length)]);
			gameObject.transform.position = this.ItemPoints[this.slotnum].transform.position;
			base.ServerManager.Spawn(gameObject, null, default(Scene));
			if (this.slotnum >= 4)
			{
				this.slotnum = 0;
			}
			else
			{
				this.slotnum++;
			}
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.Items[Random.Range(0, this.Items.Length)]);
			gameObject2.transform.position = this.ItemPoints[this.slotnum].transform.position;
			base.ServerManager.Spawn(gameObject2, null, default(Scene));
			if (this.slotnum >= 4)
			{
				this.slotnum = 0;
			}
			else
			{
				this.slotnum++;
			}
			GameObject gameObject3 = Object.Instantiate<GameObject>(this.Items[Random.Range(0, this.Items.Length)]);
			gameObject3.transform.position = this.ItemPoints[this.slotnum].transform.position;
			base.ServerManager.Spawn(gameObject3, null, default(Scene));
			if (this.slotnum >= 4)
			{
				this.slotnum = 0;
			}
			else
			{
				this.slotnum++;
			}
			GameObject gameObject4 = Object.Instantiate<GameObject>(this.Items[Random.Range(0, this.Items.Length)]);
			gameObject4.transform.position = this.ItemPoints[this.slotnum].transform.position;
			base.ServerManager.Spawn(gameObject4, null, default(Scene));
			if (this.slotnum >= 4)
			{
				this.slotnum = 0;
			}
			else
			{
				this.slotnum++;
			}
			GameObject gameObject5 = Object.Instantiate<GameObject>(this.Items[Random.Range(0, this.Items.Length)]);
			gameObject5.transform.position = this.ItemPoints[this.slotnum].transform.position;
			base.ServerManager.Spawn(gameObject5, null, default(Scene));
			if (this.slotnum >= 4)
			{
				this.slotnum = 0;
			}
			else
			{
				this.slotnum++;
			}
			GameObject gameObject6 = Object.Instantiate<GameObject>(this.Items[Random.Range(0, this.Items.Length)]);
			gameObject6.transform.position = this.ItemPoints[this.slotnum].transform.position;
			base.ServerManager.Spawn(gameObject6, null, default(Scene));
			if (this.slotnum >= 4)
			{
				this.slotnum = 0;
			}
			else
			{
				this.slotnum++;
			}
		}
		this.piObservers();
	}

	// Token: 0x060001D9 RID: 473 RVA: 0x00008C28 File Offset: 0x00006E28
	private void RpcReader___Server_pisever_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___pisever_2166136261();
	}

	// Token: 0x060001DA RID: 474 RVA: 0x00008C48 File Offset: 0x00006E48
	private void RpcWriter___Observers_piObservers_2166136261()
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

	// Token: 0x060001DB RID: 475 RVA: 0x00008CBC File Offset: 0x00006EBC
	private void RpcLogic___piObservers_2166136261()
	{
		if (this.isOpen)
		{
			base.StartCoroutine(this.CloseChest());
			this.isOpen = false;
			this.chestSource.PlayOneShot(this.chestClips[1]);
			return;
		}
		base.StartCoroutine(this.OpenChest());
		this.isOpen = true;
		this.chestSource.PlayOneShot(this.chestClips[0]);
	}

	// Token: 0x060001DC RID: 476 RVA: 0x00008D20 File Offset: 0x00006F20
	private void RpcReader___Observers_piObservers_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___piObservers_2166136261();
	}

	// Token: 0x060001DD RID: 477 RVA: 0x00008D40 File Offset: 0x00006F40
	private void RpcWriter___Server_ServerSpawnLacky_215135683(int id, Vector3 spawnpos)
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
		pooledWriter.WriteVector3(spawnpos);
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060001DE RID: 478 RVA: 0x00008DBF File Offset: 0x00006FBF
	private void RpcLogic___ServerSpawnLacky_215135683(int id, Vector3 spawnpos)
	{
		if (Random.Range(0, 3) == 0)
		{
			this.SpawnLackyObservers(id, spawnpos, Random.Range(0, int.MaxValue));
		}
		this.levelupPlayer();
	}

	// Token: 0x060001DF RID: 479 RVA: 0x00008DE4 File Offset: 0x00006FE4
	private void RpcReader___Server_ServerSpawnLacky_215135683(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSpawnLacky_215135683(num, vector);
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x00008E28 File Offset: 0x00007028
	private void RpcWriter___Observers_SpawnLackyObservers_4194131050(int id, Vector3 spawnpos, int lackyidd)
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
		pooledWriter.WriteVector3(spawnpos);
		pooledWriter.WriteInt32(lackyidd);
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x00008EC4 File Offset: 0x000070C4
	private void RpcLogic___SpawnLackyObservers_4194131050(int id, Vector3 spawnpos, int lackyidd)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.Lacky, spawnpos, Quaternion.identity);
		gameObject.GetComponent<LackyController>().SetupLacky(this.LackyDestination.position, id, this);
		gameObject.GetComponent<LackyController>().lackyid = lackyidd;
		this.LackyReferences.Add(gameObject.GetComponent<LackyController>());
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x00008F18 File Offset: 0x00007118
	private void RpcReader___Observers_SpawnLackyObservers_4194131050(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		Vector3 vector = PooledReader0.ReadVector3();
		int num2 = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___SpawnLackyObservers_4194131050(num, vector, num2);
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x00008F6C File Offset: 0x0000716C
	private void RpcWriter___Observers_levelupPlayer_2166136261()
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

	// Token: 0x060001E4 RID: 484 RVA: 0x00008FE0 File Offset: 0x000071E0
	private void RpcLogic___levelupPlayer_2166136261()
	{
		PlayerMovement playerMovement;
		if (Camera.main.transform.parent.TryGetComponent<PlayerMovement>(out playerMovement) && playerMovement.playerTeam == this.TeamNum)
		{
			playerMovement.XPupdate();
		}
	}

	// Token: 0x060001E5 RID: 485 RVA: 0x0000901C File Offset: 0x0000721C
	private void RpcReader___Observers_levelupPlayer_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___levelupPlayer_2166136261();
	}

	// Token: 0x060001E6 RID: 486 RVA: 0x0000903C File Offset: 0x0000723C
	private void RpcWriter___Server_ServerLackDropItem_953814113(Vector3 lackypos, int lackytype)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(lackypos);
		pooledWriter.WriteInt32(lackytype);
		base.SendServerRpc(5U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060001E7 RID: 487 RVA: 0x000090BC File Offset: 0x000072BC
	private void RpcLogic___ServerLackDropItem_953814113(Vector3 lackypos, int lackytype)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.Items[lackytype]);
		gameObject.transform.position = lackypos;
		base.ServerManager.Spawn(gameObject, null, default(Scene));
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x000090FC File Offset: 0x000072FC
	private void RpcReader___Server_ServerLackDropItem_953814113(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerLackDropItem_953814113(vector, num);
	}

	// Token: 0x060001E9 RID: 489 RVA: 0x00009140 File Offset: 0x00007340
	private void RpcWriter___Server_ServerKillLacky_3316948804(int lackyid)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(lackyid);
		base.SendServerRpc(6U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060001EA RID: 490 RVA: 0x000091B2 File Offset: 0x000073B2
	private void RpcLogic___ServerKillLacky_3316948804(int lackyid)
	{
		this.ObsKillLacky(lackyid);
	}

	// Token: 0x060001EB RID: 491 RVA: 0x000091BC File Offset: 0x000073BC
	private void RpcReader___Server_ServerKillLacky_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerKillLacky_3316948804(num);
	}

	// Token: 0x060001EC RID: 492 RVA: 0x000091F0 File Offset: 0x000073F0
	private void RpcWriter___Observers_ObsKillLacky_3316948804(int lackyid)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(lackyid);
		base.SendObserversRpc(7U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060001ED RID: 493 RVA: 0x00009274 File Offset: 0x00007474
	private void RpcLogic___ObsKillLacky_3316948804(int lackyid)
	{
		foreach (LackyController lackyController in this.LackyReferences)
		{
			if (lackyController.lackyid == lackyid)
			{
				lackyController.ActualInteraction();
			}
		}
	}

	// Token: 0x060001EE RID: 494 RVA: 0x000092D0 File Offset: 0x000074D0
	private void RpcReader___Observers_ObsKillLacky_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsKillLacky_3316948804(num);
	}

	// Token: 0x060001EF RID: 495 RVA: 0x00009304 File Offset: 0x00007504
	private void RpcWriter___Server_ChestLockPickSound_1140765316(bool onoff)
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
		base.SendServerRpc(8U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060001F0 RID: 496 RVA: 0x00009376 File Offset: 0x00007576
	private void RpcLogic___ChestLockPickSound_1140765316(bool onoff)
	{
		this.ChestLockPickSoundObs(onoff);
	}

	// Token: 0x060001F1 RID: 497 RVA: 0x00009380 File Offset: 0x00007580
	private void RpcReader___Server_ChestLockPickSound_1140765316(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ChestLockPickSound_1140765316(flag);
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x000093B4 File Offset: 0x000075B4
	private void RpcWriter___Observers_ChestLockPickSoundObs_1140765316(bool onoff)
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
		base.SendObserversRpc(9U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x00009435 File Offset: 0x00007635
	private void RpcLogic___ChestLockPickSoundObs_1140765316(bool onoff)
	{
		if (onoff)
		{
			this.chestSource.Play();
			return;
		}
		this.chestSource.Stop();
	}

	// Token: 0x060001F4 RID: 500 RVA: 0x00009454 File Offset: 0x00007654
	private void RpcReader___Observers_ChestLockPickSoundObs_1140765316(PooledReader PooledReader0, Channel channel)
	{
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ChestLockPickSoundObs_1140765316(flag);
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x000088CB File Offset: 0x00006ACB
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x040000E8 RID: 232
	public Transform Chest;

	// Token: 0x040000E9 RID: 233
	public bool isOpen;

	// Token: 0x040000EA RID: 234
	public Collider pcCol;

	// Token: 0x040000EB RID: 235
	public int TeamNum;

	// Token: 0x040000EC RID: 236
	public Transform[] ItemPoints;

	// Token: 0x040000ED RID: 237
	public GameObject[] Items;

	// Token: 0x040000EE RID: 238
	private int slotnum;

	// Token: 0x040000EF RID: 239
	public AudioSource chestSource;

	// Token: 0x040000F0 RID: 240
	public AudioClip[] chestClips;

	// Token: 0x040000F1 RID: 241
	public bool hasBeenOpened;

	// Token: 0x040000F2 RID: 242
	public Transform LackyDestination;

	// Token: 0x040000F3 RID: 243
	public GameObject Lacky;

	// Token: 0x040000F4 RID: 244
	private List<LackyController> LackyReferences = new List<LackyController>();

	// Token: 0x040000F5 RID: 245
	private bool NetworkInitialize___EarlyChestNetControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x040000F6 RID: 246
	private bool NetworkInitialize__LateChestNetControllerAssembly-CSharp.dll_Excuted;
}
