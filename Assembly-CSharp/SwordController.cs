using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x020001AE RID: 430
public class SwordController : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x060011DA RID: 4570 RVA: 0x000130D5 File Offset: 0x000112D5
	private void Start()
	{
		this.CamMain = Camera.main.transform;
	}

	// Token: 0x060011DB RID: 4571 RVA: 0x0004C1DD File Offset: 0x0004A3DD
	public virtual void Interaction(GameObject player)
	{
		base.StartCoroutine(this.AttackRoutine(player.GetComponent<PlayerInventory>(), player.GetComponent<PlayerMovement>()));
	}

	// Token: 0x060011DC RID: 4572 RVA: 0x0004C1F8 File Offset: 0x0004A3F8
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x060011DD RID: 4573 RVA: 0x0004C22A File Offset: 0x0004A42A
	private IEnumerator AttackRoutine(PlayerInventory playerInv, PlayerMovement playerMov)
	{
		playerInv.armsAni.SetBool("attack", true);
		playerInv.bodyAni.SetBool("attack", true);
		this.BladeTrigger.enabled = true;
		while (Input.GetKey(KeyCode.Mouse0))
		{
			if (this.isHitting && this.HitSubject != null)
			{
				GetPlayerGameobject getPlayerGameobject;
				if (this.HitSubject.CompareTag("Player") && this.HitSubject.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
				{
					playerInv.canSwapItem = false;
					playerInv.canUseItem = false;
					playerMov.canJump = false;
					getPlayerGameobject.player.GetComponent<PlayerMovement>().DamagePlayer(30f, playerMov.gameObject, "frogsword");
					while (this.HitSubject != null && this.HitSubject.CompareTag("Player"))
					{
						if (!this.hasSummonedBlood)
						{
							this.hasSummonedBlood = true;
							this.ServerMakeBlood(this.HitSubject.GetComponent<Collider>().ClosestPoint(this.Hitpoint.position));
						}
						playerMov.camArmsSyncSpeed = Mathf.Lerp(playerMov.camArmsSyncSpeed, 0f, 20f * Time.deltaTime);
						if (!this.hasZerod)
						{
							playerMov.mouseSensitivity = Mathf.Lerp(playerMov.mouseSensitivity, 0f, 60f * Time.deltaTime);
							playerMov.walkingSpeed = Mathf.Lerp(playerMov.walkingSpeed, 0f, 150f * Time.deltaTime);
							playerMov.runningSpeed = Mathf.Lerp(playerMov.runningSpeed, 0f, 500f * Time.deltaTime);
							if (playerMov.mouseSensitivity < 0.001f)
							{
								this.hasZerod = true;
							}
						}
						else
						{
							playerMov.mouseSensitivity = Mathf.Lerp(playerMov.mouseSensitivity, 2f, 2f * Time.deltaTime);
							playerMov.walkingSpeed = Mathf.Lerp(playerMov.walkingSpeed, 2f, 1f * Time.deltaTime);
							playerMov.runningSpeed = Mathf.Lerp(playerMov.runningSpeed, 4f, 1f * Time.deltaTime);
						}
						yield return null;
					}
					this.hasSummonedBlood = false;
				}
				else if (this.HitSubject.CompareTag("RockGround") || this.HitSubject.CompareTag("wall"))
				{
					RaycastHit raycastHit;
					if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out raycastHit, 30f, this.layermsk))
					{
						this.ServerMakeSparks(raycastHit.point);
						break;
					}
					break;
				}
				else
				{
					if (this.HitSubject.CompareTag("WoodGround"))
					{
						break;
					}
					if (this.HitSubject.CompareTag("hitable"))
					{
						this.HitSubject.GetComponent<IInteractableNetworkObj>().NetInteract();
						yield return new WaitForSeconds(0.1f);
					}
					else
					{
						playerInv.canSwapItem = true;
						playerInv.canUseItem = true;
						this.hasZerod = false;
						playerMov.canJump = true;
						playerMov.camArmsSyncSpeed = Mathf.Lerp(playerMov.camArmsSyncSpeed, 10f, 10f * Time.deltaTime);
						playerMov.mouseSensitivity = Mathf.Lerp(playerMov.mouseSensitivity, 2f, 2f * Time.deltaTime);
						playerMov.walkingSpeed = Mathf.Lerp(playerMov.walkingSpeed, 7.5f, 30f * Time.deltaTime);
						playerMov.runningSpeed = Mathf.Lerp(playerMov.runningSpeed, 11.5f, 30f * Time.deltaTime);
					}
				}
			}
			else
			{
				playerInv.canSwapItem = true;
				playerInv.canUseItem = true;
				playerMov.canJump = true;
				this.hasZerod = false;
				playerMov.camArmsSyncSpeed = Mathf.Lerp(playerMov.camArmsSyncSpeed, 10f, 10f * Time.deltaTime);
				playerMov.mouseSensitivity = Mathf.Lerp(playerMov.mouseSensitivity, 1f, 30f * Time.deltaTime);
				playerMov.walkingSpeed = Mathf.Lerp(playerMov.walkingSpeed, 7.5f, 30f * Time.deltaTime);
				playerMov.runningSpeed = Mathf.Lerp(playerMov.runningSpeed, 11.5f, 30f * Time.deltaTime);
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

	// Token: 0x060011DE RID: 4574 RVA: 0x0004C247 File Offset: 0x0004A447
	[ServerRpc(RequireOwnership = false)]
	private void ServerMakeBlood(Vector3 collisionPoint)
	{
		this.RpcWriter___Server_ServerMakeBlood_4276783012(collisionPoint);
	}

	// Token: 0x060011DF RID: 4575 RVA: 0x0004C253 File Offset: 0x0004A453
	[ObserversRpc]
	private void ObserversMakeBlood(Vector3 collisionPoint)
	{
		this.RpcWriter___Observers_ObserversMakeBlood_4276783012(collisionPoint);
	}

	// Token: 0x060011E0 RID: 4576 RVA: 0x0004C25F File Offset: 0x0004A45F
	private IEnumerator blood(Vector3 collisionPoint)
	{
		GameObject thing = Object.Instantiate<GameObject>(this.Blood, collisionPoint, Quaternion.identity);
		thing.transform.LookAt(this.Hitpoint);
		yield return new WaitForSeconds(10f);
		Object.Destroy(thing);
		yield break;
	}

	// Token: 0x060011E1 RID: 4577 RVA: 0x0004C275 File Offset: 0x0004A475
	[ServerRpc(RequireOwnership = false)]
	private void ServerMakeSparks(Vector3 collisionPoint)
	{
		this.RpcWriter___Server_ServerMakeSparks_4276783012(collisionPoint);
	}

	// Token: 0x060011E2 RID: 4578 RVA: 0x0004C281 File Offset: 0x0004A481
	[ObserversRpc]
	private void ObserversMakeSparks(Vector3 collisionPoint)
	{
		this.RpcWriter___Observers_ObserversMakeSparks_4276783012(collisionPoint);
	}

	// Token: 0x060011E3 RID: 4579 RVA: 0x0004C28D File Offset: 0x0004A48D
	private IEnumerator sparks(Vector3 collisionPoint)
	{
		GameObject thing = Object.Instantiate<GameObject>(this.Sparks, collisionPoint, Quaternion.identity);
		thing.transform.LookAt(this.CamMain);
		yield return new WaitForSeconds(1f);
		Object.Destroy(thing);
		yield break;
	}

	// Token: 0x060011E4 RID: 4580 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x060011E5 RID: 4581 RVA: 0x0004C2A4 File Offset: 0x0004A4A4
	public virtual void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			base.transform.position = raycastHit.point;
		}
		this.SwordControllerAudio.PlayOneShot(this.SwordClips[5]);
	}

	// Token: 0x060011E6 RID: 4582 RVA: 0x0004C325 File Offset: 0x0004A525
	public virtual void PlayDropSound()
	{
		this.SwordControllerAudio.PlayOneShot(this.SwordClips[5]);
	}

	// Token: 0x060011E7 RID: 4583 RVA: 0x0004C33A File Offset: 0x0004A53A
	public virtual void ItemInit()
	{
		this.mlgsRenderer.SetActive(true);
		this.SwordControllerAudio.PlayOneShot(this.SwordClips[0]);
	}

	// Token: 0x060011E8 RID: 4584 RVA: 0x0004C33A File Offset: 0x0004A53A
	public virtual void ItemInitObs()
	{
		this.mlgsRenderer.SetActive(true);
		this.SwordControllerAudio.PlayOneShot(this.SwordClips[0]);
	}

	// Token: 0x060011E9 RID: 4585 RVA: 0x0004C35B File Offset: 0x0004A55B
	public virtual void HideItem()
	{
		this.mlgsRenderer.SetActive(false);
	}

	// Token: 0x060011EA RID: 4586 RVA: 0x00007BEE File Offset: 0x00005DEE
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
	}

	// Token: 0x060011EB RID: 4587 RVA: 0x0004C369 File Offset: 0x0004A569
	public virtual string DisplayInteractUI(GameObject player)
	{
		return "Grasp Ritual Dagger";
	}

	// Token: 0x060011EC RID: 4588 RVA: 0x0004C370 File Offset: 0x0004A570
	public virtual int GetItemID()
	{
		return 17;
	}

	// Token: 0x060011EE RID: 4590 RVA: 0x0004C398 File Offset: 0x0004A598
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlySwordControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlySwordControllerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ServerMakeBlood_4276783012));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversMakeBlood_4276783012));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_ServerMakeSparks_4276783012));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversMakeSparks_4276783012));
	}

	// Token: 0x060011EF RID: 4591 RVA: 0x0004C412 File Offset: 0x0004A612
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateSwordControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateSwordControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x060011F0 RID: 4592 RVA: 0x0004C425 File Offset: 0x0004A625
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x060011F1 RID: 4593 RVA: 0x0004C434 File Offset: 0x0004A634
	private void RpcWriter___Server_ServerMakeBlood_4276783012(Vector3 collisionPoint)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(collisionPoint);
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060011F2 RID: 4594 RVA: 0x0004C4A6 File Offset: 0x0004A6A6
	private void RpcLogic___ServerMakeBlood_4276783012(Vector3 collisionPoint)
	{
		this.ObserversMakeBlood(collisionPoint);
	}

	// Token: 0x060011F3 RID: 4595 RVA: 0x0004C4B0 File Offset: 0x0004A6B0
	private void RpcReader___Server_ServerMakeBlood_4276783012(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerMakeBlood_4276783012(vector);
	}

	// Token: 0x060011F4 RID: 4596 RVA: 0x0004C4E4 File Offset: 0x0004A6E4
	private void RpcWriter___Observers_ObserversMakeBlood_4276783012(Vector3 collisionPoint)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(collisionPoint);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060011F5 RID: 4597 RVA: 0x0004C565 File Offset: 0x0004A765
	private void RpcLogic___ObserversMakeBlood_4276783012(Vector3 collisionPoint)
	{
		base.StartCoroutine(this.blood(collisionPoint));
		this.SwordControllerAudio.PlayOneShot(this.SwordClips[Random.Range(1, 5)]);
	}

	// Token: 0x060011F6 RID: 4598 RVA: 0x0004C590 File Offset: 0x0004A790
	private void RpcReader___Observers_ObserversMakeBlood_4276783012(PooledReader PooledReader0, Channel channel)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversMakeBlood_4276783012(vector);
	}

	// Token: 0x060011F7 RID: 4599 RVA: 0x0004C5C4 File Offset: 0x0004A7C4
	private void RpcWriter___Server_ServerMakeSparks_4276783012(Vector3 collisionPoint)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(collisionPoint);
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060011F8 RID: 4600 RVA: 0x0004C636 File Offset: 0x0004A836
	private void RpcLogic___ServerMakeSparks_4276783012(Vector3 collisionPoint)
	{
		this.ObserversMakeSparks(collisionPoint);
	}

	// Token: 0x060011F9 RID: 4601 RVA: 0x0004C640 File Offset: 0x0004A840
	private void RpcReader___Server_ServerMakeSparks_4276783012(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerMakeSparks_4276783012(vector);
	}

	// Token: 0x060011FA RID: 4602 RVA: 0x0004C674 File Offset: 0x0004A874
	private void RpcWriter___Observers_ObserversMakeSparks_4276783012(Vector3 collisionPoint)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(collisionPoint);
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060011FB RID: 4603 RVA: 0x0004C6F5 File Offset: 0x0004A8F5
	private void RpcLogic___ObserversMakeSparks_4276783012(Vector3 collisionPoint)
	{
		base.StartCoroutine(this.sparks(collisionPoint));
	}

	// Token: 0x060011FC RID: 4604 RVA: 0x0004C708 File Offset: 0x0004A908
	private void RpcReader___Observers_ObserversMakeSparks_4276783012(PooledReader PooledReader0, Channel channel)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversMakeSparks_4276783012(vector);
	}

	// Token: 0x060011FD RID: 4605 RVA: 0x0004C425 File Offset: 0x0004A625
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000A51 RID: 2641
	public GameObject mlgsRenderer;

	// Token: 0x04000A52 RID: 2642
	public Transform CamMain;

	// Token: 0x04000A53 RID: 2643
	public Collider BladeTrigger;

	// Token: 0x04000A54 RID: 2644
	public bool isHitting;

	// Token: 0x04000A55 RID: 2645
	public GameObject HitSubject;

	// Token: 0x04000A56 RID: 2646
	public bool hasZerod;

	// Token: 0x04000A57 RID: 2647
	public GameObject Sparks;

	// Token: 0x04000A58 RID: 2648
	public GameObject Blood;

	// Token: 0x04000A59 RID: 2649
	public Transform Hitpoint;

	// Token: 0x04000A5A RID: 2650
	public bool hasSummonedBlood;

	// Token: 0x04000A5B RID: 2651
	public LayerMask layermsk;

	// Token: 0x04000A5C RID: 2652
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x04000A5D RID: 2653
	public AudioSource SwordControllerAudio;

	// Token: 0x04000A5E RID: 2654
	public AudioClip[] SwordClips;

	// Token: 0x04000A5F RID: 2655
	private bool NetworkInitialize___EarlySwordControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x04000A60 RID: 2656
	private bool NetworkInitialize__LateSwordControllerAssembly-CSharp.dll_Excuted;
}
