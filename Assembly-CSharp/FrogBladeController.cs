using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x02000092 RID: 146
public class FrogBladeController : SwordController
{
	// Token: 0x060005FB RID: 1531 RVA: 0x000130D5 File Offset: 0x000112D5
	private void Start()
	{
		this.CamMain = Camera.main.transform;
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x0001707D File Offset: 0x0001527D
	public override string DisplayInteractUI(GameObject player)
	{
		return "Grasp Frogleather Blade";
	}

	// Token: 0x060005FD RID: 1533 RVA: 0x00017084 File Offset: 0x00015284
	public override int GetItemID()
	{
		return 10;
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x00017088 File Offset: 0x00015288
	public override void Interaction(GameObject player)
	{
		base.StartCoroutine(this.AttackRoutine(player.GetComponent<PlayerInventory>(), player.GetComponent<PlayerMovement>()));
	}

	// Token: 0x060005FF RID: 1535 RVA: 0x000170A3 File Offset: 0x000152A3
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
				GetShadowWizardController getShadowWizardController;
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
				else if (this.HitSubject.CompareTag("PlayerNpc") && this.HitSubject.TryGetComponent<GetShadowWizardController>(out getShadowWizardController))
				{
					playerInv.canSwapItem = false;
					playerInv.canUseItem = false;
					playerMov.canJump = false;
					getShadowWizardController.ShadowWizardAI.GetComponent<IHitableMonster>().HitMonster(100f);
					while (this.HitSubject != null && this.HitSubject.CompareTag("PlayerNpc"))
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
					else if (this.HitSubject.CompareTag("PlayerNpc"))
					{
						playerInv.canSwapItem = false;
						playerInv.canUseItem = false;
						playerMov.canJump = false;
						this.HitSubject.GetComponent<MonsterHitScript>().HitTheMonster(35f);
						while (this.HitSubject != null && this.HitSubject.CompareTag("PlayerNpc"))
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

	// Token: 0x06000600 RID: 1536 RVA: 0x000170C0 File Offset: 0x000152C0
	[ServerRpc(RequireOwnership = false)]
	private void ServerMakeBlood(Vector3 collisionPoint)
	{
		this.RpcWriter___Server_ServerMakeBlood_4276783012(collisionPoint);
	}

	// Token: 0x06000601 RID: 1537 RVA: 0x000170CC File Offset: 0x000152CC
	[ObserversRpc]
	private void ObserversMakeBlood(Vector3 collisionPoint)
	{
		this.RpcWriter___Observers_ObserversMakeBlood_4276783012(collisionPoint);
	}

	// Token: 0x06000602 RID: 1538 RVA: 0x000170D8 File Offset: 0x000152D8
	private IEnumerator blood(Vector3 collisionPoint)
	{
		GameObject thing = Object.Instantiate<GameObject>(this.Blood, collisionPoint, Quaternion.identity);
		thing.transform.LookAt(this.Hitpoint);
		yield return new WaitForSeconds(10f);
		Object.Destroy(thing);
		yield break;
	}

	// Token: 0x06000603 RID: 1539 RVA: 0x000170EE File Offset: 0x000152EE
	[ServerRpc(RequireOwnership = false)]
	private void ServerMakeSparks(Vector3 collisionPoint)
	{
		this.RpcWriter___Server_ServerMakeSparks_4276783012(collisionPoint);
	}

	// Token: 0x06000604 RID: 1540 RVA: 0x000170FA File Offset: 0x000152FA
	[ObserversRpc]
	private void ObserversMakeSparks(Vector3 collisionPoint)
	{
		this.RpcWriter___Observers_ObserversMakeSparks_4276783012(collisionPoint);
	}

	// Token: 0x06000605 RID: 1541 RVA: 0x00017106 File Offset: 0x00015306
	private IEnumerator sparks(Vector3 collisionPoint)
	{
		GameObject thing = Object.Instantiate<GameObject>(this.Sparks, collisionPoint, Quaternion.identity);
		thing.transform.LookAt(this.CamMain);
		yield return new WaitForSeconds(1f);
		Object.Destroy(thing);
		yield break;
	}

	// Token: 0x06000606 RID: 1542 RVA: 0x0001711C File Offset: 0x0001531C
	public override void ItemInit()
	{
		this.mlgsRenderer.SetActive(true);
		this.BladeAni.SetBool("squeeze", true);
		base.StartCoroutine(this.bladeani());
		this.SwordControllerAudio.PlayOneShot(this.SwordClips[0]);
		this.SwordControllerAudio.PlayOneShot(this.SwordClips[1]);
	}

	// Token: 0x06000607 RID: 1543 RVA: 0x0001717C File Offset: 0x0001537C
	public override void ItemInitObs()
	{
		this.mlgsRenderer.SetActive(true);
		this.BladeAni.SetBool("squeeze", true);
		base.StartCoroutine(this.bladeani());
		this.SwordControllerAudio.PlayOneShot(this.SwordClips[0]);
		this.SwordControllerAudio.PlayOneShot(this.SwordClips[1]);
	}

	// Token: 0x06000608 RID: 1544 RVA: 0x000171DC File Offset: 0x000153DC
	public override void HideItem()
	{
		this.mlgsRenderer.SetActive(false);
		this.ActualBlade.localPosition = new Vector3(this.ActualBlade.localPosition.x, 1.3f, this.ActualBlade.localPosition.z);
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x0001722C File Offset: 0x0001542C
	public override void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			base.transform.position = raycastHit.point;
		}
		this.ActualBlade.localPosition = new Vector3(this.ActualBlade.localPosition.x, 1.3f, this.ActualBlade.localPosition.z);
		this.SwordControllerAudio.PlayOneShot(this.SwordClips[6]);
		this.SwordControllerAudio.PlayOneShot(this.SwordClips[7]);
		this.mlgsRenderer.SetActive(true);
	}

	// Token: 0x0600060A RID: 1546 RVA: 0x00017301 File Offset: 0x00015501
	public override void PlayDropSound()
	{
		this.SwordControllerAudio.PlayOneShot(this.SwordClips[6]);
		this.SwordControllerAudio.PlayOneShot(this.SwordClips[7]);
	}

	// Token: 0x0600060B RID: 1547 RVA: 0x00017329 File Offset: 0x00015529
	private IEnumerator bladeani()
	{
		yield return null;
		this.BladeAni.SetBool("squeeze", false);
		Vector3 startpos = this.ActualBlade.localPosition;
		Vector3 endPos = new Vector3(this.ActualBlade.localPosition.x, 3f, this.ActualBlade.localPosition.z);
		float timer = 0f;
		while ((double)timer < 0.25)
		{
			this.ActualBlade.localPosition = Vector3.Lerp(startpos, endPos, timer * 4f);
			timer += Time.deltaTime;
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x00017338 File Offset: 0x00015538
	public override void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyFrogBladeControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyFrogBladeControllerAssembly-CSharp.dll_Excuted = true;
		base.NetworkInitialize___Early();
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_ServerMakeBlood_4276783012));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversMakeBlood_4276783012));
		base.RegisterServerRpc(6U, new ServerRpcDelegate(this.RpcReader___Server_ServerMakeSparks_4276783012));
		base.RegisterObserversRpc(7U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversMakeSparks_4276783012));
	}

	// Token: 0x0600060E RID: 1550 RVA: 0x000173B8 File Offset: 0x000155B8
	public override void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateFrogBladeControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateFrogBladeControllerAssembly-CSharp.dll_Excuted = true;
		base.NetworkInitialize__Late();
	}

	// Token: 0x0600060F RID: 1551 RVA: 0x000173D1 File Offset: 0x000155D1
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x000173E0 File Offset: 0x000155E0
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
		base.SendServerRpc(4U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000611 RID: 1553 RVA: 0x00017452 File Offset: 0x00015652
	private void RpcLogic___ServerMakeBlood_4276783012(Vector3 collisionPoint)
	{
		this.ObserversMakeBlood(collisionPoint);
	}

	// Token: 0x06000612 RID: 1554 RVA: 0x0001745C File Offset: 0x0001565C
	private void RpcReader___Server_ServerMakeBlood_4276783012(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerMakeBlood_4276783012(vector);
	}

	// Token: 0x06000613 RID: 1555 RVA: 0x00017490 File Offset: 0x00015690
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
		base.SendObserversRpc(5U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000614 RID: 1556 RVA: 0x00017511 File Offset: 0x00015711
	private void RpcLogic___ObserversMakeBlood_4276783012(Vector3 collisionPoint)
	{
		base.StartCoroutine(this.blood(collisionPoint));
		this.SwordControllerAudio.PlayOneShot(this.SwordClips[Random.Range(2, 6)]);
	}

	// Token: 0x06000615 RID: 1557 RVA: 0x0001753C File Offset: 0x0001573C
	private void RpcReader___Observers_ObserversMakeBlood_4276783012(PooledReader PooledReader0, Channel channel)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversMakeBlood_4276783012(vector);
	}

	// Token: 0x06000616 RID: 1558 RVA: 0x00017570 File Offset: 0x00015770
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
		base.SendServerRpc(6U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000617 RID: 1559 RVA: 0x000175E2 File Offset: 0x000157E2
	private void RpcLogic___ServerMakeSparks_4276783012(Vector3 collisionPoint)
	{
		this.ObserversMakeSparks(collisionPoint);
	}

	// Token: 0x06000618 RID: 1560 RVA: 0x000175EC File Offset: 0x000157EC
	private void RpcReader___Server_ServerMakeSparks_4276783012(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerMakeSparks_4276783012(vector);
	}

	// Token: 0x06000619 RID: 1561 RVA: 0x00017620 File Offset: 0x00015820
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
		base.SendObserversRpc(7U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600061A RID: 1562 RVA: 0x000176A1 File Offset: 0x000158A1
	private void RpcLogic___ObserversMakeSparks_4276783012(Vector3 collisionPoint)
	{
		base.StartCoroutine(this.sparks(collisionPoint));
	}

	// Token: 0x0600061B RID: 1563 RVA: 0x000176B4 File Offset: 0x000158B4
	private void RpcReader___Observers_ObserversMakeSparks_4276783012(PooledReader PooledReader0, Channel channel)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversMakeSparks_4276783012(vector);
	}

	// Token: 0x0600061C RID: 1564 RVA: 0x000176E5 File Offset: 0x000158E5
	public override void Awake()
	{
		this.NetworkInitialize___Early();
		base.Awake();
		this.NetworkInitialize__Late();
	}

	// Token: 0x040002F9 RID: 761
	public Animator BladeAni;

	// Token: 0x040002FA RID: 762
	public Transform ActualBlade;

	// Token: 0x040002FB RID: 763
	private bool NetworkInitialize___EarlyFrogBladeControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x040002FC RID: 764
	private bool NetworkInitialize__LateFrogBladeControllerAssembly-CSharp.dll_Excuted;
}
