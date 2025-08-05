using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x02000098 RID: 152
public class FrogSpear : SwordController
{
	// Token: 0x06000645 RID: 1605 RVA: 0x00018549 File Offset: 0x00016749
	public override void ItemInit()
	{
		this.mlgsRenderer.SetActive(true);
		this.asource.PlayOneShot(this.aclips[0]);
		this.asource.PlayOneShot(this.aclips[1]);
	}

	// Token: 0x06000646 RID: 1606 RVA: 0x00018549 File Offset: 0x00016749
	public override void ItemInitObs()
	{
		this.mlgsRenderer.SetActive(true);
		this.asource.PlayOneShot(this.aclips[0]);
		this.asource.PlayOneShot(this.aclips[1]);
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x0001857D File Offset: 0x0001677D
	public override string DisplayInteractUI(GameObject player)
	{
		return "Grasp Frog Spear";
	}

	// Token: 0x06000648 RID: 1608 RVA: 0x00018584 File Offset: 0x00016784
	public override int GetItemID()
	{
		return 22;
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x00018588 File Offset: 0x00016788
	public override void Interaction(GameObject player)
	{
		base.StartCoroutine(this.AtkRoutine(player.GetComponent<PlayerInventory>(), player.GetComponent<PlayerMovement>()));
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x000185A3 File Offset: 0x000167A3
	private IEnumerator AtkRoutine(PlayerInventory playerInv, PlayerMovement playerMov)
	{
		playerInv.armsAni.SetBool("attack", true);
		playerInv.bodyAni.SetBool("attack", true);
		yield return new WaitForSeconds(0.1f);
		this.fsanitoggle2();
		this.BladeTrigger.enabled = true;
		this.HitSubject = null;
		this.isHitting = false;
		while (Input.GetKey(KeyCode.Mouse0))
		{
			if (this.isHitting && this.HitSubject != null)
			{
				if (this.HitSubject.CompareTag("Player"))
				{
					GetPlayerGameobject getPlayerGameobject;
					if (this.HitSubject.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
					{
						this.ToungePull(getPlayerGameobject.player);
						break;
					}
					break;
				}
				else if (this.HitSubject.CompareTag("hitable"))
				{
					this.HitSubject.GetComponent<IInteractableNetworkObj>().NetInteract();
					yield return new WaitForSeconds(0.1f);
				}
				else if (!this.HitSubject.CompareTag("Ignorable"))
				{
					break;
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
		this.fsanitoggle();
		this.HitSubject = null;
		yield break;
	}

	// Token: 0x0600064B RID: 1611 RVA: 0x000185C0 File Offset: 0x000167C0
	public override void PlayDropSound()
	{
		this.asource.PlayOneShot(this.aclips[4]);
		this.asource.PlayOneShot(this.aclips[5]);
	}

	// Token: 0x0600064C RID: 1612 RVA: 0x000185E8 File Offset: 0x000167E8
	public override void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			base.transform.position = new Vector3(raycastHit.point.x + 0.5f, raycastHit.point.y, raycastHit.point.z);
		}
		this.HitSubject = null;
		this.asource.PlayOneShot(this.aclips[4]);
		this.asource.PlayOneShot(this.aclips[5]);
		this.mlgsRenderer.SetActive(true);
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x000186B7 File Offset: 0x000168B7
	[ServerRpc(RequireOwnership = false)]
	private void ToungePull(GameObject player)
	{
		this.RpcWriter___Server_ToungePull_1934289915(player);
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x000186C3 File Offset: 0x000168C3
	[ObserversRpc]
	private void Toungepobs(GameObject player)
	{
		this.RpcWriter___Observers_Toungepobs_1934289915(player);
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x000186CF File Offset: 0x000168CF
	[ServerRpc(RequireOwnership = false)]
	private void fsanitoggle()
	{
		this.RpcWriter___Server_fsanitoggle_2166136261();
	}

	// Token: 0x06000650 RID: 1616 RVA: 0x000186D7 File Offset: 0x000168D7
	[ObserversRpc]
	private void fsanitoggleobs()
	{
		this.RpcWriter___Observers_fsanitoggleobs_2166136261();
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x000186DF File Offset: 0x000168DF
	[ServerRpc(RequireOwnership = false)]
	private void fsanitoggle2()
	{
		this.RpcWriter___Server_fsanitoggle2_2166136261();
	}

	// Token: 0x06000652 RID: 1618 RVA: 0x000186E7 File Offset: 0x000168E7
	[ObserversRpc]
	private void fsanitoggleobs2()
	{
		this.RpcWriter___Observers_fsanitoggleobs2_2166136261();
	}

	// Token: 0x06000654 RID: 1620 RVA: 0x000186F0 File Offset: 0x000168F0
	public override void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyFrogSpearAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyFrogSpearAssembly-CSharp.dll_Excuted = true;
		base.NetworkInitialize___Early();
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_ToungePull_1934289915));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_Toungepobs_1934289915));
		base.RegisterServerRpc(6U, new ServerRpcDelegate(this.RpcReader___Server_fsanitoggle_2166136261));
		base.RegisterObserversRpc(7U, new ClientRpcDelegate(this.RpcReader___Observers_fsanitoggleobs_2166136261));
		base.RegisterServerRpc(8U, new ServerRpcDelegate(this.RpcReader___Server_fsanitoggle2_2166136261));
		base.RegisterObserversRpc(9U, new ClientRpcDelegate(this.RpcReader___Observers_fsanitoggleobs2_2166136261));
	}

	// Token: 0x06000655 RID: 1621 RVA: 0x0001879E File Offset: 0x0001699E
	public override void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateFrogSpearAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateFrogSpearAssembly-CSharp.dll_Excuted = true;
		base.NetworkInitialize__Late();
	}

	// Token: 0x06000656 RID: 1622 RVA: 0x000187B7 File Offset: 0x000169B7
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000657 RID: 1623 RVA: 0x000187C8 File Offset: 0x000169C8
	private void RpcWriter___Server_ToungePull_1934289915(GameObject player)
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

	// Token: 0x06000658 RID: 1624 RVA: 0x0001883A File Offset: 0x00016A3A
	private void RpcLogic___ToungePull_1934289915(GameObject player)
	{
		this.Toungepobs(player);
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x00018844 File Offset: 0x00016A44
	private void RpcReader___Server_ToungePull_1934289915(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ToungePull_1934289915(gameObject);
	}

	// Token: 0x0600065A RID: 1626 RVA: 0x00018878 File Offset: 0x00016A78
	private void RpcWriter___Observers_Toungepobs_1934289915(GameObject player)
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

	// Token: 0x0600065B RID: 1627 RVA: 0x000188F9 File Offset: 0x00016AF9
	private void RpcLogic___Toungepobs_1934289915(GameObject player)
	{
		Debug.Log(player);
		player.GetComponent<PlayerInventory>().Drop();
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x0001890C File Offset: 0x00016B0C
	private void RpcReader___Observers_Toungepobs_1934289915(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___Toungepobs_1934289915(gameObject);
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x00018940 File Offset: 0x00016B40
	private void RpcWriter___Server_fsanitoggle_2166136261()
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

	// Token: 0x0600065E RID: 1630 RVA: 0x000189A5 File Offset: 0x00016BA5
	private void RpcLogic___fsanitoggle_2166136261()
	{
		this.fsanitoggleobs();
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x000189B0 File Offset: 0x00016BB0
	private void RpcReader___Server_fsanitoggle_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___fsanitoggle_2166136261();
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x000189D0 File Offset: 0x00016BD0
	private void RpcWriter___Observers_fsanitoggleobs_2166136261()
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

	// Token: 0x06000661 RID: 1633 RVA: 0x00018A44 File Offset: 0x00016C44
	private void RpcLogic___fsanitoggleobs_2166136261()
	{
		this.fsani.SetBool("extendo", false);
		this.asource.PlayOneShot(this.aclips[3]);
	}

	// Token: 0x06000662 RID: 1634 RVA: 0x00018A6C File Offset: 0x00016C6C
	private void RpcReader___Observers_fsanitoggleobs_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___fsanitoggleobs_2166136261();
	}

	// Token: 0x06000663 RID: 1635 RVA: 0x00018A8C File Offset: 0x00016C8C
	private void RpcWriter___Server_fsanitoggle2_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(8U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x00018AF1 File Offset: 0x00016CF1
	private void RpcLogic___fsanitoggle2_2166136261()
	{
		this.fsanitoggleobs2();
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x00018AFC File Offset: 0x00016CFC
	private void RpcReader___Server_fsanitoggle2_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___fsanitoggle2_2166136261();
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x00018B1C File Offset: 0x00016D1C
	private void RpcWriter___Observers_fsanitoggleobs2_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(9U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x00018B90 File Offset: 0x00016D90
	private void RpcLogic___fsanitoggleobs2_2166136261()
	{
		this.fsani.SetBool("extendo", true);
		this.asource.PlayOneShot(this.aclips[2]);
	}

	// Token: 0x06000668 RID: 1640 RVA: 0x00018BB8 File Offset: 0x00016DB8
	private void RpcReader___Observers_fsanitoggleobs2_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___fsanitoggleobs2_2166136261();
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x00018BD8 File Offset: 0x00016DD8
	public override void Awake()
	{
		this.NetworkInitialize___Early();
		base.Awake();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000318 RID: 792
	public Animator fsani;

	// Token: 0x04000319 RID: 793
	public AudioSource asource;

	// Token: 0x0400031A RID: 794
	public AudioClip[] aclips;

	// Token: 0x0400031B RID: 795
	private bool NetworkInitialize___EarlyFrogSpearAssembly-CSharp.dll_Excuted;

	// Token: 0x0400031C RID: 796
	private bool NetworkInitialize__LateFrogSpearAssembly-CSharp.dll_Excuted;
}
