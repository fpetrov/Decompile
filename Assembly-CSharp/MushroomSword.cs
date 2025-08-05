using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x020000F2 RID: 242
public class MushroomSword : SwordController
{
	// Token: 0x060009DF RID: 2527 RVA: 0x00025E92 File Offset: 0x00024092
	public override string DisplayInteractUI(GameObject player)
	{
		return "Grasp Mushroom Swerd";
	}

	// Token: 0x060009E0 RID: 2528 RVA: 0x00025E99 File Offset: 0x00024099
	public override int GetItemID()
	{
		return 18;
	}

	// Token: 0x060009E1 RID: 2529 RVA: 0x00025E9D File Offset: 0x0002409D
	public override void Interaction(GameObject player)
	{
		base.StartCoroutine(this.MushRoutine(player.GetComponent<PlayerInventory>(), player.GetComponent<PlayerMovement>()));
	}

	// Token: 0x060009E2 RID: 2530 RVA: 0x00025EB8 File Offset: 0x000240B8
	private IEnumerator MushRoutine(PlayerInventory playerInv, PlayerMovement playerMov)
	{
		playerInv.armsAni.SetBool("attack", true);
		playerInv.bodyAni.SetBool("attack", true);
		this.BladeTrigger.enabled = true;
		while (Input.GetKey(KeyCode.Mouse0))
		{
			if (this.isHitting && this.HitSubject != null)
			{
				if (this.HitSubject.CompareTag("Player"))
				{
					GetPlayerGameobject getPlayerGameobject;
					if (this.HitSubject.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
					{
						this.ApplyKnockbackserver(getPlayerGameobject.player);
						break;
					}
				}
				else
				{
					if (this.HitSubject.layer == 3)
					{
						playerMov.BoingWall();
						this.playboingserver();
						break;
					}
					if (this.HitSubject.CompareTag("hitable"))
					{
						this.HitSubject.GetComponent<IInteractableNetworkObj>().NetInteract();
						yield return new WaitForSeconds(0.1f);
						break;
					}
					GetShadowWizardController getShadowWizardController;
					if (this.HitSubject.CompareTag("PlayerNpc") && this.HitSubject.TryGetComponent<GetShadowWizardController>(out getShadowWizardController))
					{
						getShadowWizardController.ShadowWizardAI.GetComponent<ShadowWizardAI>().MushSwordHit(base.transform);
						break;
					}
				}
			}
			yield return null;
		}
		playerInv.canSwapItem = true;
		playerInv.canUseItem = true;
		playerMov.canJump = true;
		playerMov.camArmsSyncSpeed = 10f;
		playerMov.mouseSensitivity = 2f;
		playerMov.runningSpeed = 11.5f;
		playerMov.walkingSpeed = 7.5f;
		playerInv.armsAni.SetBool("attack", false);
		playerInv.bodyAni.SetBool("attack", false);
		yield break;
	}

	// Token: 0x060009E3 RID: 2531 RVA: 0x00025ED5 File Offset: 0x000240D5
	[ServerRpc(RequireOwnership = false)]
	private void ApplyKnockbackserver(GameObject player)
	{
		this.RpcWriter___Server_ApplyKnockbackserver_1934289915(player);
	}

	// Token: 0x060009E4 RID: 2532 RVA: 0x00025EE1 File Offset: 0x000240E1
	[ObserversRpc]
	private void ApplyKnockbackObs(GameObject player)
	{
		this.RpcWriter___Observers_ApplyKnockbackObs_1934289915(player);
	}

	// Token: 0x060009E5 RID: 2533 RVA: 0x00025EED File Offset: 0x000240ED
	[ServerRpc(RequireOwnership = false)]
	private void playboingserver()
	{
		this.RpcWriter___Server_playboingserver_2166136261();
	}

	// Token: 0x060009E6 RID: 2534 RVA: 0x00025EF5 File Offset: 0x000240F5
	[ObserversRpc]
	private void playboingObs()
	{
		this.RpcWriter___Observers_playboingObs_2166136261();
	}

	// Token: 0x060009E8 RID: 2536 RVA: 0x00025F00 File Offset: 0x00024100
	public override void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyMushroomSwordAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyMushroomSwordAssembly-CSharp.dll_Excuted = true;
		base.NetworkInitialize___Early();
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_ApplyKnockbackserver_1934289915));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_ApplyKnockbackObs_1934289915));
		base.RegisterServerRpc(6U, new ServerRpcDelegate(this.RpcReader___Server_playboingserver_2166136261));
		base.RegisterObserversRpc(7U, new ClientRpcDelegate(this.RpcReader___Observers_playboingObs_2166136261));
	}

	// Token: 0x060009E9 RID: 2537 RVA: 0x00025F80 File Offset: 0x00024180
	public override void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateMushroomSwordAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateMushroomSwordAssembly-CSharp.dll_Excuted = true;
		base.NetworkInitialize__Late();
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x00025F99 File Offset: 0x00024199
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x00025FA8 File Offset: 0x000241A8
	private void RpcWriter___Server_ApplyKnockbackserver_1934289915(GameObject player)
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
		base.SendServerRpc(4U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060009EC RID: 2540 RVA: 0x0002601A File Offset: 0x0002421A
	private void RpcLogic___ApplyKnockbackserver_1934289915(GameObject player)
	{
		this.ApplyKnockbackObs(player);
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x00026024 File Offset: 0x00024224
	private void RpcReader___Server_ApplyKnockbackserver_1934289915(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ApplyKnockbackserver_1934289915(gameObject);
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x00026058 File Offset: 0x00024258
	private void RpcWriter___Observers_ApplyKnockbackObs_1934289915(GameObject player)
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
		base.SendObserversRpc(5U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x000260D9 File Offset: 0x000242D9
	private void RpcLogic___ApplyKnockbackObs_1934289915(GameObject player)
	{
		player.GetComponent<PlayerMovement>().ApplyKnockback(this.Hitpoint.gameObject);
		this.SwordControllerAudio.PlayOneShot(this.SwordClips[Random.Range(1, this.SwordClips.Length - 1)]);
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x00026114 File Offset: 0x00024314
	private void RpcReader___Observers_ApplyKnockbackObs_1934289915(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ApplyKnockbackObs_1934289915(gameObject);
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x00026148 File Offset: 0x00024348
	private void RpcWriter___Server_playboingserver_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(6U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060009F2 RID: 2546 RVA: 0x000261AD File Offset: 0x000243AD
	private void RpcLogic___playboingserver_2166136261()
	{
		this.playboingObs();
	}

	// Token: 0x060009F3 RID: 2547 RVA: 0x000261B8 File Offset: 0x000243B8
	private void RpcReader___Server_playboingserver_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___playboingserver_2166136261();
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x000261D8 File Offset: 0x000243D8
	private void RpcWriter___Observers_playboingObs_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(7U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x0002624C File Offset: 0x0002444C
	private void RpcLogic___playboingObs_2166136261()
	{
		this.SwordControllerAudio.PlayOneShot(this.SwordClips[Random.Range(1, this.SwordClips.Length - 1)]);
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x00026270 File Offset: 0x00024470
	private void RpcReader___Observers_playboingObs_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___playboingObs_2166136261();
	}

	// Token: 0x060009F7 RID: 2551 RVA: 0x00026290 File Offset: 0x00024490
	public override void Awake()
	{
		this.NetworkInitialize___Early();
		base.Awake();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0400053A RID: 1338
	private bool NetworkInitialize___EarlyMushroomSwordAssembly-CSharp.dll_Excuted;

	// Token: 0x0400053B RID: 1339
	private bool NetworkInitialize__LateMushroomSwordAssembly-CSharp.dll_Excuted;
}
