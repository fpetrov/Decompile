using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x02000073 RID: 115
public class ExcaliburController : SwordController
{
	// Token: 0x060004E6 RID: 1254 RVA: 0x000130D5 File Offset: 0x000112D5
	private void Start()
	{
		this.CamMain = Camera.main.transform;
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x000130E7 File Offset: 0x000112E7
	public override string DisplayInteractUI(GameObject player)
	{
		return "Grasp Excalibur Reforged";
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x000130EE File Offset: 0x000112EE
	public override int GetItemID()
	{
		return 26;
	}

	// Token: 0x060004E9 RID: 1257 RVA: 0x000130F2 File Offset: 0x000112F2
	public override void Interaction(GameObject player)
	{
		base.StartCoroutine(this.AttackRoutine(player.GetComponent<PlayerInventory>(), player.GetComponent<PlayerMovement>()));
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x0001310D File Offset: 0x0001130D
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
					getPlayerGameobject.player.GetComponent<PlayerMovement>().ExcaliburDamagePlayer(105f, playerMov.gameObject);
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
					GetShadowWizardController getShadowWizardController;
					if (this.HitSubject.CompareTag("PlayerNpc") && this.HitSubject.TryGetComponent<GetShadowWizardController>(out getShadowWizardController))
					{
						getShadowWizardController.ShadowWizardAI.gameObject.GetComponent<IHitableMonster>().HitMonster(100f);
						this.MakeLightning(this.HitSubject.transform.position);
						break;
					}
					if (this.HitSubject.CompareTag("WoodGround"))
					{
						break;
					}
					if (this.HitSubject.CompareTag("hitable"))
					{
						this.HitSubject.GetComponent<IInteractableNetworkObj>().NetInteract();
						yield return new WaitForSeconds(0.25f);
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

	// Token: 0x060004EB RID: 1259 RVA: 0x0001312A File Offset: 0x0001132A
	[ServerRpc(RequireOwnership = false)]
	private void ServerMakeBlood(Vector3 collisionPoint)
	{
		this.RpcWriter___Server_ServerMakeBlood_4276783012(collisionPoint);
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x00013136 File Offset: 0x00011336
	[ObserversRpc]
	private void ObserversMakeBlood(Vector3 collisionPoint)
	{
		this.RpcWriter___Observers_ObserversMakeBlood_4276783012(collisionPoint);
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x00013142 File Offset: 0x00011342
	private IEnumerator blood(Vector3 collisionPoint)
	{
		GameObject thing = Object.Instantiate<GameObject>(this.Blood, collisionPoint, Quaternion.identity);
		thing.transform.LookAt(this.Hitpoint);
		yield return new WaitForSeconds(10f);
		Object.Destroy(thing);
		yield break;
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x00013158 File Offset: 0x00011358
	[ServerRpc(RequireOwnership = false)]
	private void ServerMakeSparks(Vector3 collisionPoint)
	{
		this.RpcWriter___Server_ServerMakeSparks_4276783012(collisionPoint);
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x00013164 File Offset: 0x00011364
	[ObserversRpc]
	private void ObserversMakeSparks(Vector3 collisionPoint)
	{
		this.RpcWriter___Observers_ObserversMakeSparks_4276783012(collisionPoint);
	}

	// Token: 0x060004F0 RID: 1264 RVA: 0x00013170 File Offset: 0x00011370
	private IEnumerator sparks(Vector3 collisionPoint)
	{
		GameObject thing = Object.Instantiate<GameObject>(this.Sparks, collisionPoint, Quaternion.identity);
		thing.transform.LookAt(this.CamMain);
		yield return new WaitForSeconds(1f);
		Object.Destroy(thing);
		yield break;
	}

	// Token: 0x060004F1 RID: 1265 RVA: 0x00013186 File Offset: 0x00011386
	[ServerRpc(RequireOwnership = false)]
	private void MakeLightning(Vector3 pos)
	{
		this.RpcWriter___Server_MakeLightning_4276783012(pos);
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x00013192 File Offset: 0x00011392
	[ObserversRpc]
	private void MakeLightningObs(Vector3 pos)
	{
		this.RpcWriter___Observers_MakeLightningObs_4276783012(pos);
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x000131A0 File Offset: 0x000113A0
	public override void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyExcaliburControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyExcaliburControllerAssembly-CSharp.dll_Excuted = true;
		base.NetworkInitialize___Early();
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_ServerMakeBlood_4276783012));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversMakeBlood_4276783012));
		base.RegisterServerRpc(6U, new ServerRpcDelegate(this.RpcReader___Server_ServerMakeSparks_4276783012));
		base.RegisterObserversRpc(7U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversMakeSparks_4276783012));
		base.RegisterServerRpc(8U, new ServerRpcDelegate(this.RpcReader___Server_MakeLightning_4276783012));
		base.RegisterObserversRpc(9U, new ClientRpcDelegate(this.RpcReader___Observers_MakeLightningObs_4276783012));
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x0001324E File Offset: 0x0001144E
	public override void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateExcaliburControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateExcaliburControllerAssembly-CSharp.dll_Excuted = true;
		base.NetworkInitialize__Late();
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x00013267 File Offset: 0x00011467
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x00013278 File Offset: 0x00011478
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

	// Token: 0x060004F8 RID: 1272 RVA: 0x000132EA File Offset: 0x000114EA
	private void RpcLogic___ServerMakeBlood_4276783012(Vector3 collisionPoint)
	{
		this.ObserversMakeBlood(collisionPoint);
	}

	// Token: 0x060004F9 RID: 1273 RVA: 0x000132F4 File Offset: 0x000114F4
	private void RpcReader___Server_ServerMakeBlood_4276783012(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerMakeBlood_4276783012(vector);
	}

	// Token: 0x060004FA RID: 1274 RVA: 0x00013328 File Offset: 0x00011528
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

	// Token: 0x060004FB RID: 1275 RVA: 0x000133A9 File Offset: 0x000115A9
	private void RpcLogic___ObserversMakeBlood_4276783012(Vector3 collisionPoint)
	{
		base.StartCoroutine(this.blood(collisionPoint));
		this.SwordControllerAudio.PlayOneShot(this.SwordClips[Random.Range(1, 5)]);
	}

	// Token: 0x060004FC RID: 1276 RVA: 0x000133D4 File Offset: 0x000115D4
	private void RpcReader___Observers_ObserversMakeBlood_4276783012(PooledReader PooledReader0, Channel channel)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversMakeBlood_4276783012(vector);
	}

	// Token: 0x060004FD RID: 1277 RVA: 0x00013408 File Offset: 0x00011608
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

	// Token: 0x060004FE RID: 1278 RVA: 0x0001347A File Offset: 0x0001167A
	private void RpcLogic___ServerMakeSparks_4276783012(Vector3 collisionPoint)
	{
		this.ObserversMakeSparks(collisionPoint);
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x00013484 File Offset: 0x00011684
	private void RpcReader___Server_ServerMakeSparks_4276783012(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerMakeSparks_4276783012(vector);
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x000134B8 File Offset: 0x000116B8
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

	// Token: 0x06000501 RID: 1281 RVA: 0x00013539 File Offset: 0x00011739
	private void RpcLogic___ObserversMakeSparks_4276783012(Vector3 collisionPoint)
	{
		base.StartCoroutine(this.sparks(collisionPoint));
	}

	// Token: 0x06000502 RID: 1282 RVA: 0x0001354C File Offset: 0x0001174C
	private void RpcReader___Observers_ObserversMakeSparks_4276783012(PooledReader PooledReader0, Channel channel)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversMakeSparks_4276783012(vector);
	}

	// Token: 0x06000503 RID: 1283 RVA: 0x00013580 File Offset: 0x00011780
	private void RpcWriter___Server_MakeLightning_4276783012(Vector3 pos)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(pos);
		base.SendServerRpc(8U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000504 RID: 1284 RVA: 0x000135F2 File Offset: 0x000117F2
	private void RpcLogic___MakeLightning_4276783012(Vector3 pos)
	{
		this.MakeLightningObs(pos);
	}

	// Token: 0x06000505 RID: 1285 RVA: 0x000135FC File Offset: 0x000117FC
	private void RpcReader___Server_MakeLightning_4276783012(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___MakeLightning_4276783012(vector);
	}

	// Token: 0x06000506 RID: 1286 RVA: 0x00013630 File Offset: 0x00011830
	private void RpcWriter___Observers_MakeLightningObs_4276783012(Vector3 pos)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(pos);
		base.SendObserversRpc(9U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x000136B1 File Offset: 0x000118B1
	private void RpcLogic___MakeLightningObs_4276783012(Vector3 pos)
	{
		Object.Instantiate<GameObject>(this.LightningBolt, pos, Quaternion.identity);
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x000136C8 File Offset: 0x000118C8
	private void RpcReader___Observers_MakeLightningObs_4276783012(PooledReader PooledReader0, Channel channel)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___MakeLightningObs_4276783012(vector);
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x000136F9 File Offset: 0x000118F9
	public override void Awake()
	{
		this.NetworkInitialize___Early();
		base.Awake();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0400025B RID: 603
	public GameObject LightningBolt;

	// Token: 0x0400025C RID: 604
	private bool NetworkInitialize___EarlyExcaliburControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x0400025D RID: 605
	private bool NetworkInitialize__LateExcaliburControllerAssembly-CSharp.dll_Excuted;
}
