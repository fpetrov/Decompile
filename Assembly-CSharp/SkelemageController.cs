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
using UnityEngine.AI;

// Token: 0x0200020D RID: 525
public class SkelemageController : NetworkBehaviour, IHitableMonster
{
	// Token: 0x060014E5 RID: 5349 RVA: 0x00057B98 File Offset: 0x00055D98
	public override void OnStartClient()
	{
		base.OnStartClient();
		if (!base.HasAuthority)
		{
			this.navAgent.enabled = false;
			base.enabled = false;
			return;
		}
		this.inited = true;
		this.CurrentPatrolPoint = base.transform;
		this.AiNodes = GameObject.FindGameObjectsWithTag("ainodes");
		this.PRM = GameObject.FindGameObjectWithTag("NetItemManager").GetComponent<PlayerRespawnManager>();
	}

	// Token: 0x060014E6 RID: 5350 RVA: 0x00057BFF File Offset: 0x00055DFF
	private void Update()
	{
		if (this.inited && !this.isDead)
		{
			this.DoAiInterval();
		}
	}

	// Token: 0x060014E7 RID: 5351 RVA: 0x00057C18 File Offset: 0x00055E18
	private void DoAiInterval()
	{
		foreach (GameObject gameObject in this.PRM.DeadPlayers)
		{
			if (gameObject != null && this.CurrentTarget == gameObject.transform)
			{
				this.PotentialTargets.Remove(gameObject.transform);
				foreach (Collider collider in gameObject.GetComponent<PlayerMovement>().hitboxcols)
				{
					this.PotentialTargets.Remove(collider.transform);
				}
				this.state = 2;
			}
		}
		if (this.hp <= 0f)
		{
			this.navAgent.SetDestination(base.transform.position);
			this.isDead = true;
			base.StartCoroutine(this.makestandstill());
			this.ServerDeathAni();
		}
		if (this.canRun)
		{
			this.stamina -= Time.deltaTime;
			if (this.stamina < 0f)
			{
				this.canRun = false;
			}
		}
		else
		{
			this.stamina += Time.deltaTime * 2f;
			if (this.stamina > 9.5f)
			{
				this.canRun = true;
			}
			else if (this.stamina > 4f && Time.time - this.runCheckTimer > 1f)
			{
				this.runCheckTimer = Time.time;
				if (Random.Range(0, 3) == 0)
				{
					this.canRun = true;
				}
			}
		}
		if (this.navAgent.velocity != Vector3.zero && !this.LookingAtPlayer)
		{
			Quaternion quaternion = Quaternion.LookRotation(this.navAgent.velocity);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, quaternion, Time.deltaTime);
		}
		switch (this.state)
		{
		case 2:
			this.canRun = false;
			this.switchSearchNodeTimer += Time.deltaTime;
			if (this.switchSearchNodeTimer > this.randomSwitchNodeValue)
			{
				this.switchSearchNodeTimer = 0f;
				if (Random.Range(0, 3) == 0)
				{
					this.CurrentPatrolPoint = base.transform;
					this.randomSwitchNodeValue = Random.Range(2f, 3f);
				}
				else
				{
					this.PreviousPatrolPoint = this.CurrentPatrolPoint;
					this.CurrentPatrolPoint = this.AiNodes[Random.Range(0, this.AiNodes.Length)].transform;
					if (Random.Range(0, 11) <= 4)
					{
						this.randomSwitchNodeValue = (float)Random.Range(2, 7);
					}
					else
					{
						this.randomSwitchNodeValue = (float)Random.Range(9, 13);
					}
				}
			}
			if (this.PreviousPatrolPoint != null && !this.navAgent.isOnOffMeshLink)
			{
				this.navAgent.SetDestination(Vector3.Lerp(this.PreviousPatrolPoint.position, this.CurrentPatrolPoint.position, Mathf.Clamp01(this.switchSearchNodeTimer)));
			}
			this.LOSCheck();
			break;
		case 3:
			if (this.TargetLOSCheck())
			{
				if (Time.time - this.fireBallCdTimer > 4f && this.canCastFB && Vector3.Distance(base.transform.position, this.CurrentTarget.position) < 20f)
				{
					Vector3 vector = new Vector3(this.CurrentTarget.position.x, this.CurrentTarget.position.y + 2f, this.CurrentTarget.position.z);
					Vector3 vector2 = vector - new Vector3(this.LastSeenPlayerPos.x, this.LastSeenPlayerPos.y + 2f, this.LastSeenPlayerPos.z);
					float num = Vector3.Distance(vector, base.transform.position);
					Vector3 vector3 = vector + vector2 * num - this.Spine003.position;
					if (!Physics.BoxCast(this.Spine003.position, new Vector3(0.5f, 0.5f, 0.5f), vector3, Quaternion.identity, vector3.magnitude, this.GroundLayer) && !Physics.Raycast(this.Spine003.position - this.Spine003.forward, vector3, vector3.magnitude, this.GroundLayer))
					{
						this.fbGiveUpTimer = 0f;
						this.fbTimer = 0f;
						this.switchSearchNodeTimer = 0f;
						this.state = 5;
						this.gottenDirection = false;
						this.PlaySkeleSound(0);
						this.PlaySkeleSound(1);
						this.ToggleFireballParticles();
						this.ToggleLookRotation(true, this.CurrentTarget);
						this.ToggleCastAni();
					}
				}
				this.gottenDirection = false;
				NavMeshPath navMeshPath = new NavMeshPath();
				this.navAgent.CalculatePath(this.CurrentTarget.position, navMeshPath);
				if (navMeshPath.status != NavMeshPathStatus.PathComplete)
				{
					if (this.switchSearchNodeTimer > 2f && !this.navAgent.isOnOffMeshLink)
					{
						this.navAgent.SetDestination(base.transform.position);
						Vector3 vector4 = this.CurrentTarget.position - base.transform.position;
						vector4.y = 0f;
						if (vector4 != Vector3.zero)
						{
							base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(vector4), Time.deltaTime);
						}
					}
					if (this.switchSearchNodeTimer > 25f)
					{
						this.gottenDirection = false;
						this.state = 2;
						this.canCastFB = false;
					}
				}
				else
				{
					if (!this.navAgent.isOnOffMeshLink)
					{
						this.navAgent.SetPath(navMeshPath);
					}
					this.switchSearchNodeTimer = 0f;
				}
				this.LastSeenPlayerPos = this.CurrentTarget.position;
				this.canCastFB = true;
			}
			else
			{
				if (Vector3.Distance(base.transform.position, this.LastSeenPlayerPos) > 3f)
				{
					this.navAgent.SetDestination(this.LastSeenPlayerPos);
				}
				else
				{
					this.navAgent.SetDestination(this.CurrentTarget.position);
					this.navAgent.speed = 2f;
				}
				if (this.switchSearchNodeTimer > 25f)
				{
					this.gottenDirection = false;
					this.state = 2;
					this.canCastFB = false;
				}
			}
			this.switchSearchNodeTimer += Time.deltaTime;
			break;
		case 5:
		{
			float num2;
			this.FireballTargeting(out num2);
			if (this.fbTimer > 1.4f)
			{
				Vector3 vector5 = new Vector3(this.CurrentTarget.position.x, this.CurrentTarget.position.y + 2f, this.CurrentTarget.position.z);
				Vector3 vector6 = vector5 - new Vector3(this.LastSeenPlayerPos.x, this.LastSeenPlayerPos.y + 2f, this.LastSeenPlayerPos.z);
				float num3 = Vector3.Distance(vector5, base.transform.position);
				Vector3 vector7 = vector5 + vector6 * num3;
				this.ShootFireball(vector7);
				this.state = 3;
				this.switchSearchNodeTimer = 0f;
				this.fireBallCdTimer = Time.time;
				this.canCastFB = false;
				this.ToggleLookRotation(false, null);
			}
			this.fbTimer += Time.deltaTime;
			this.LastSeenPlayerPos = this.CurrentTarget.position;
			break;
		}
		}
		if (this.canRun)
		{
			this.navAgent.speed = 4f;
			return;
		}
		this.navAgent.speed = 2f;
	}

	// Token: 0x060014E8 RID: 5352 RVA: 0x000583D4 File Offset: 0x000565D4
	private void FireballTargeting(out float adder)
	{
		Vector3 vector = new Vector3(this.CurrentTarget.position.x, this.CurrentTarget.position.y + 1.2f, this.CurrentTarget.position.z);
		Vector3 vector2 = vector - new Vector3(this.LastSeenPlayerPos.x, this.LastSeenPlayerPos.y + 1.2f, this.LastSeenPlayerPos.z);
		float num = Vector3.Distance(vector, base.transform.position);
		Vector3 vector3 = vector + vector2 * num - this.Spine003.position;
		Vector3 vector4 = this.CurrentTarget.position;
		adder = 0f;
		if (!Physics.BoxCast(this.Spine003.position, new Vector3(0.5f, 0.5f, 0.5f), vector3, Quaternion.identity, vector3.magnitude, this.GroundLayer) && !Physics.Raycast(this.Spine003.position - this.Spine003.forward, vector3, vector3.magnitude, this.GroundLayer))
		{
			vector4 = base.transform.position;
			adder = 0.2f;
		}
		this.navAgent.SetDestination(vector4);
		if (this.hp <= 0f)
		{
			this.isDead = true;
			this.navAgent.SetDestination(base.transform.position);
		}
	}

	// Token: 0x060014E9 RID: 5353 RVA: 0x00058550 File Offset: 0x00056750
	private bool TargetLOSCheck()
	{
		Vector3 vector;
		return this.adjustedLOSRayCast(this.CurrentTarget, true, out vector);
	}

	// Token: 0x060014EA RID: 5354 RVA: 0x0005856C File Offset: 0x0005676C
	private void LOSCheck()
	{
		if (this.PotentialTargets.Count > 0)
		{
			foreach (Transform transform in this.PotentialTargets)
			{
				Vector3 vector;
				if (this.adjustedLOSRayCast(transform, true, out vector) && Vector3.Angle(vector, this.Spine003.forward) < 45f)
				{
					this.switchSearchNodeTimer = 0f;
					this.state = 3;
					this.CurrentTarget = transform;
					break;
				}
			}
		}
	}

	// Token: 0x060014EB RID: 5355 RVA: 0x00058608 File Offset: 0x00056808
	private bool adjustedLOSRayCast(Transform target, bool towardsTarget, out Vector3 direction)
	{
		if (towardsTarget)
		{
			Vector3 vector = new Vector3(this.Spine003.position.x, this.Spine003.position.y + 0.5f, this.Spine003.position.z);
			Vector3 vector2 = new Vector3(target.position.x, target.position.y + 2f, target.position.z);
			direction = vector2 - vector;
			return !Physics.Raycast(vector, direction, direction.magnitude, this.GroundLayer) && direction.magnitude < 70f;
		}
		Vector3 vector3 = new Vector3(this.Spine003.position.x, this.Spine003.position.y + 0.5f, this.Spine003.position.z);
		Vector3 vector4 = new Vector3(target.position.x, target.position.y + 2f, target.position.z);
		direction = vector3 - vector4;
		return !Physics.Raycast(vector4, direction, direction.magnitude, this.GroundLayer);
	}

	// Token: 0x060014EC RID: 5356 RVA: 0x0005875C File Offset: 0x0005695C
	private void ShootFireball(Vector3 target)
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(target, Vector3.down, out raycastHit, 10f, this.GroundLayer))
		{
			this.ShootFireballServer(new Vector3(raycastHit.point.x, raycastHit.point.y + 0.5f, raycastHit.point.z), false);
		}
		this.fireBallCdTimer = Time.time;
	}

	// Token: 0x060014ED RID: 5357 RVA: 0x000587C9 File Offset: 0x000569C9
	[ServerRpc(RequireOwnership = false)]
	private void ShootFireballServer(Vector3 target, bool maxHeight)
	{
		this.RpcWriter___Server_ShootFireballServer_1082256137(target, maxHeight);
	}

	// Token: 0x060014EE RID: 5358 RVA: 0x000587D9 File Offset: 0x000569D9
	[ObserversRpc]
	private void ShootfireballOBs(Vector3 target, bool maxHeight)
	{
		this.RpcWriter___Observers_ShootfireballOBs_1082256137(target, maxHeight);
	}

	// Token: 0x060014EF RID: 5359 RVA: 0x000587E9 File Offset: 0x000569E9
	[ServerRpc(RequireOwnership = false)]
	private void ToggleCastAni()
	{
		this.RpcWriter___Server_ToggleCastAni_2166136261();
	}

	// Token: 0x060014F0 RID: 5360 RVA: 0x000587F1 File Offset: 0x000569F1
	[ObserversRpc]
	private void ToggleCastAniObs()
	{
		this.RpcWriter___Observers_ToggleCastAniObs_2166136261();
	}

	// Token: 0x060014F1 RID: 5361 RVA: 0x000587F9 File Offset: 0x000569F9
	[ServerRpc(RequireOwnership = false)]
	private void ToggleFireballParticles()
	{
		this.RpcWriter___Server_ToggleFireballParticles_2166136261();
	}

	// Token: 0x060014F2 RID: 5362 RVA: 0x00058801 File Offset: 0x00056A01
	[ObserversRpc]
	private void ToggleFireballParticlesObservers()
	{
		this.RpcWriter___Observers_ToggleFireballParticlesObservers_2166136261();
	}

	// Token: 0x060014F3 RID: 5363 RVA: 0x0005880C File Offset: 0x00056A0C
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			GetPlayerGameobject getPlayerGameobject;
			if (other.gameObject.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
			{
				this.PotentialTargets.Add(getPlayerGameobject.player.transform);
				return;
			}
			PlayerMovement playerMovement;
			if (other.gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
			{
				this.PotentialTargets.Add(playerMovement.transform);
			}
		}
	}

	// Token: 0x060014F4 RID: 5364 RVA: 0x0005886C File Offset: 0x00056A6C
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			GetPlayerGameobject getPlayerGameobject;
			if (other.gameObject.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
			{
				this.PotentialTargets.Remove(getPlayerGameobject.player.transform);
				return;
			}
			PlayerMovement playerMovement;
			if (other.gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
			{
				this.PotentialTargets.Remove(playerMovement.transform);
			}
		}
	}

	// Token: 0x060014F5 RID: 5365 RVA: 0x000588CE File Offset: 0x00056ACE
	[ServerRpc(RequireOwnership = false)]
	private void PlaySkeleSound(int soundID)
	{
		this.RpcWriter___Server_PlaySkeleSound_3316948804(soundID);
	}

	// Token: 0x060014F6 RID: 5366 RVA: 0x000588DC File Offset: 0x00056ADC
	[ObserversRpc]
	private void PlaySkeleObs(int soundID)
	{
		this.RpcWriter___Observers_PlaySkeleObs_3316948804(soundID);
	}

	// Token: 0x060014F7 RID: 5367 RVA: 0x000588F3 File Offset: 0x00056AF3
	[ServerRpc(RequireOwnership = false)]
	private void ToggleLookRotation(bool tf, Transform target)
	{
		this.RpcWriter___Server_ToggleLookRotation_3024699591(tf, target);
	}

	// Token: 0x060014F8 RID: 5368 RVA: 0x00058903 File Offset: 0x00056B03
	[ObserversRpc]
	private void obsToggleLookroto(bool tf, Transform target)
	{
		this.RpcWriter___Observers_obsToggleLookroto_3024699591(tf, target);
	}

	// Token: 0x060014F9 RID: 5369 RVA: 0x00058913 File Offset: 0x00056B13
	public void HitMonster(float Damage)
	{
		this.ServerMonsterHit(Damage);
	}

	// Token: 0x060014FA RID: 5370 RVA: 0x0005891C File Offset: 0x00056B1C
	[ServerRpc(RequireOwnership = false)]
	private void ServerMonsterHit(float damage)
	{
		this.RpcWriter___Server_ServerMonsterHit_431000436(damage);
	}

	// Token: 0x060014FB RID: 5371 RVA: 0x00058928 File Offset: 0x00056B28
	[ObserversRpc]
	private void obsMonsterHit(float damage)
	{
		this.RpcWriter___Observers_obsMonsterHit_431000436(damage);
	}

	// Token: 0x060014FC RID: 5372 RVA: 0x00058934 File Offset: 0x00056B34
	private IEnumerator HitMonster()
	{
		if (Time.time - this.soundcd > 0.1f)
		{
			this.soundcd = Time.time;
			this.SkeleAudio.pitch = Random.Range(0.91f, 0.95f);
			this.PlaySkeleSound(2);
		}
		this.AniController.MageAni.SetBool("hit", true);
		yield return new WaitForSeconds(0.2f);
		this.AniController.MageAni.SetBool("hit", false);
		yield break;
	}

	// Token: 0x060014FD RID: 5373 RVA: 0x00058943 File Offset: 0x00056B43
	[ServerRpc(RequireOwnership = false)]
	private void ServerDeathAni()
	{
		this.RpcWriter___Server_ServerDeathAni_2166136261();
	}

	// Token: 0x060014FE RID: 5374 RVA: 0x0005894C File Offset: 0x00056B4C
	[ObserversRpc]
	private void ObsDeathAni()
	{
		this.RpcWriter___Observers_ObsDeathAni_2166136261();
	}

	// Token: 0x060014FF RID: 5375 RVA: 0x0005895F File Offset: 0x00056B5F
	private IEnumerator makestandstill()
	{
		this.navAgent.SetDestination(base.transform.position);
		yield return null;
		this.navAgent.SetDestination(base.transform.position);
		yield return null;
		this.navAgent.SetDestination(base.transform.position);
		yield return null;
		this.navAgent.SetDestination(base.transform.position);
		yield return null;
		this.navAgent.SetDestination(base.transform.position);
		yield return null;
		this.navAgent.SetDestination(base.transform.position);
		yield break;
	}

	// Token: 0x06001500 RID: 5376 RVA: 0x0005896E File Offset: 0x00056B6E
	private IEnumerator WaitforPoof()
	{
		yield return new WaitForSeconds(4.5f);
		Object.Instantiate<GameObject>(this.Skelepoof, base.transform.position, Quaternion.identity);
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x06001502 RID: 5378 RVA: 0x000589B4 File Offset: 0x00056BB4
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlySkelemageControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlySkelemageControllerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ShootFireballServer_1082256137));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_ShootfireballOBs_1082256137));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_ToggleCastAni_2166136261));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ToggleCastAniObs_2166136261));
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_ToggleFireballParticles_2166136261));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_ToggleFireballParticlesObservers_2166136261));
		base.RegisterServerRpc(6U, new ServerRpcDelegate(this.RpcReader___Server_PlaySkeleSound_3316948804));
		base.RegisterObserversRpc(7U, new ClientRpcDelegate(this.RpcReader___Observers_PlaySkeleObs_3316948804));
		base.RegisterServerRpc(8U, new ServerRpcDelegate(this.RpcReader___Server_ToggleLookRotation_3024699591));
		base.RegisterObserversRpc(9U, new ClientRpcDelegate(this.RpcReader___Observers_obsToggleLookroto_3024699591));
		base.RegisterServerRpc(10U, new ServerRpcDelegate(this.RpcReader___Server_ServerMonsterHit_431000436));
		base.RegisterObserversRpc(11U, new ClientRpcDelegate(this.RpcReader___Observers_obsMonsterHit_431000436));
		base.RegisterServerRpc(12U, new ServerRpcDelegate(this.RpcReader___Server_ServerDeathAni_2166136261));
		base.RegisterObserversRpc(13U, new ClientRpcDelegate(this.RpcReader___Observers_ObsDeathAni_2166136261));
	}

	// Token: 0x06001503 RID: 5379 RVA: 0x00058B14 File Offset: 0x00056D14
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateSkelemageControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateSkelemageControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06001504 RID: 5380 RVA: 0x00058B27 File Offset: 0x00056D27
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06001505 RID: 5381 RVA: 0x00058B38 File Offset: 0x00056D38
	private void RpcWriter___Server_ShootFireballServer_1082256137(Vector3 target, bool maxHeight)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(target);
		pooledWriter.WriteBoolean(maxHeight);
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06001506 RID: 5382 RVA: 0x00058BB7 File Offset: 0x00056DB7
	private void RpcLogic___ShootFireballServer_1082256137(Vector3 target, bool maxHeight)
	{
		this.ShootfireballOBs(target, maxHeight);
	}

	// Token: 0x06001507 RID: 5383 RVA: 0x00058BC4 File Offset: 0x00056DC4
	private void RpcReader___Server_ShootFireballServer_1082256137(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ShootFireballServer_1082256137(vector, flag);
	}

	// Token: 0x06001508 RID: 5384 RVA: 0x00058C08 File Offset: 0x00056E08
	private void RpcWriter___Observers_ShootfireballOBs_1082256137(Vector3 target, bool maxHeight)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(target);
		pooledWriter.WriteBoolean(maxHeight);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06001509 RID: 5385 RVA: 0x00058C96 File Offset: 0x00056E96
	private void RpcLogic___ShootfireballOBs_1082256137(Vector3 target, bool maxHeight)
	{
		Object.Instantiate<GameObject>(this.fireBall, this.Spine003.position + base.transform.forward, Quaternion.identity).GetComponent<FireballController>().Setup(target, maxHeight);
	}

	// Token: 0x0600150A RID: 5386 RVA: 0x00058CD0 File Offset: 0x00056ED0
	private void RpcReader___Observers_ShootfireballOBs_1082256137(PooledReader PooledReader0, Channel channel)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ShootfireballOBs_1082256137(vector, flag);
	}

	// Token: 0x0600150B RID: 5387 RVA: 0x00058D14 File Offset: 0x00056F14
	private void RpcWriter___Server_ToggleCastAni_2166136261()
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

	// Token: 0x0600150C RID: 5388 RVA: 0x00058D79 File Offset: 0x00056F79
	private void RpcLogic___ToggleCastAni_2166136261()
	{
		this.ToggleCastAniObs();
	}

	// Token: 0x0600150D RID: 5389 RVA: 0x00058D84 File Offset: 0x00056F84
	private void RpcReader___Server_ToggleCastAni_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ToggleCastAni_2166136261();
	}

	// Token: 0x0600150E RID: 5390 RVA: 0x00058DA4 File Offset: 0x00056FA4
	private void RpcWriter___Observers_ToggleCastAniObs_2166136261()
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

	// Token: 0x0600150F RID: 5391 RVA: 0x00058E18 File Offset: 0x00057018
	private void RpcLogic___ToggleCastAniObs_2166136261()
	{
		this.AniController.CastAnis();
	}

	// Token: 0x06001510 RID: 5392 RVA: 0x00058E28 File Offset: 0x00057028
	private void RpcReader___Observers_ToggleCastAniObs_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ToggleCastAniObs_2166136261();
	}

	// Token: 0x06001511 RID: 5393 RVA: 0x00058E48 File Offset: 0x00057048
	private void RpcWriter___Server_ToggleFireballParticles_2166136261()
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

	// Token: 0x06001512 RID: 5394 RVA: 0x00058EAD File Offset: 0x000570AD
	private void RpcLogic___ToggleFireballParticles_2166136261()
	{
		this.ToggleFireballParticlesObservers();
	}

	// Token: 0x06001513 RID: 5395 RVA: 0x00058EB8 File Offset: 0x000570B8
	private void RpcReader___Server_ToggleFireballParticles_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ToggleFireballParticles_2166136261();
	}

	// Token: 0x06001514 RID: 5396 RVA: 0x00058ED8 File Offset: 0x000570D8
	private void RpcWriter___Observers_ToggleFireballParticlesObservers_2166136261()
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

	// Token: 0x06001515 RID: 5397 RVA: 0x00058F4C File Offset: 0x0005714C
	private void RpcLogic___ToggleFireballParticlesObservers_2166136261()
	{
		this.particlePoint.Play();
	}

	// Token: 0x06001516 RID: 5398 RVA: 0x00058F5C File Offset: 0x0005715C
	private void RpcReader___Observers_ToggleFireballParticlesObservers_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ToggleFireballParticlesObservers_2166136261();
	}

	// Token: 0x06001517 RID: 5399 RVA: 0x00058F7C File Offset: 0x0005717C
	private void RpcWriter___Server_PlaySkeleSound_3316948804(int soundID)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(soundID);
		base.SendServerRpc(6U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06001518 RID: 5400 RVA: 0x00058FEE File Offset: 0x000571EE
	private void RpcLogic___PlaySkeleSound_3316948804(int soundID)
	{
		this.PlaySkeleObs(soundID);
	}

	// Token: 0x06001519 RID: 5401 RVA: 0x00058FF8 File Offset: 0x000571F8
	private void RpcReader___Server_PlaySkeleSound_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___PlaySkeleSound_3316948804(num);
	}

	// Token: 0x0600151A RID: 5402 RVA: 0x0005902C File Offset: 0x0005722C
	private void RpcWriter___Observers_PlaySkeleObs_3316948804(int soundID)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(soundID);
		base.SendObserversRpc(7U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600151B RID: 5403 RVA: 0x000590B0 File Offset: 0x000572B0
	private void RpcLogic___PlaySkeleObs_3316948804(int soundID)
	{
		if (soundID == 3)
		{
			this.SkeleAudio.volume = 0.7f;
			return;
		}
		if (soundID == 0)
		{
			this.SkeleAudio.volume = 1f;
			this.SkeleAudio.pitch = Random.Range(0.91f, 0.95f);
			this.SkeleAudio.PlayOneShot(this.SkeleClips[soundID]);
			return;
		}
		this.SkeleAudio.pitch = Random.Range(0.91f, 0.95f);
		this.SkeleAudio.PlayOneShot(this.SkeleClips[soundID]);
	}

	// Token: 0x0600151C RID: 5404 RVA: 0x00059140 File Offset: 0x00057340
	private void RpcReader___Observers_PlaySkeleObs_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___PlaySkeleObs_3316948804(num);
	}

	// Token: 0x0600151D RID: 5405 RVA: 0x00059174 File Offset: 0x00057374
	private void RpcWriter___Server_ToggleLookRotation_3024699591(bool tf, Transform target)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteBoolean(tf);
		pooledWriter.WriteTransform(target);
		base.SendServerRpc(8U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600151E RID: 5406 RVA: 0x000591F3 File Offset: 0x000573F3
	private void RpcLogic___ToggleLookRotation_3024699591(bool tf, Transform target)
	{
		this.obsToggleLookroto(tf, target);
	}

	// Token: 0x0600151F RID: 5407 RVA: 0x00059200 File Offset: 0x00057400
	private void RpcReader___Server_ToggleLookRotation_3024699591(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		bool flag = PooledReader0.ReadBoolean();
		Transform transform = PooledReader0.ReadTransform();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ToggleLookRotation_3024699591(flag, transform);
	}

	// Token: 0x06001520 RID: 5408 RVA: 0x00059244 File Offset: 0x00057444
	private void RpcWriter___Observers_obsToggleLookroto_3024699591(bool tf, Transform target)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteBoolean(tf);
		pooledWriter.WriteTransform(target);
		base.SendObserversRpc(9U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06001521 RID: 5409 RVA: 0x000592D2 File Offset: 0x000574D2
	private void RpcLogic___obsToggleLookroto_3024699591(bool tf, Transform target)
	{
		this.AniController.castingFireball = tf;
		this.AniController.target = target;
	}

	// Token: 0x06001522 RID: 5410 RVA: 0x000592EC File Offset: 0x000574EC
	private void RpcReader___Observers_obsToggleLookroto_3024699591(PooledReader PooledReader0, Channel channel)
	{
		bool flag = PooledReader0.ReadBoolean();
		Transform transform = PooledReader0.ReadTransform();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___obsToggleLookroto_3024699591(flag, transform);
	}

	// Token: 0x06001523 RID: 5411 RVA: 0x00059330 File Offset: 0x00057530
	private void RpcWriter___Server_ServerMonsterHit_431000436(float damage)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteSingle(damage);
		base.SendServerRpc(10U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06001524 RID: 5412 RVA: 0x000593A2 File Offset: 0x000575A2
	private void RpcLogic___ServerMonsterHit_431000436(float damage)
	{
		this.obsMonsterHit(damage);
	}

	// Token: 0x06001525 RID: 5413 RVA: 0x000593AC File Offset: 0x000575AC
	private void RpcReader___Server_ServerMonsterHit_431000436(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		float num = PooledReader0.ReadSingle();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerMonsterHit_431000436(num);
	}

	// Token: 0x06001526 RID: 5414 RVA: 0x000593E0 File Offset: 0x000575E0
	private void RpcWriter___Observers_obsMonsterHit_431000436(float damage)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteSingle(damage);
		base.SendObserversRpc(11U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06001527 RID: 5415 RVA: 0x00059461 File Offset: 0x00057661
	private void RpcLogic___obsMonsterHit_431000436(float damage)
	{
		if (base.HasAuthority)
		{
			this.hp -= damage;
		}
		base.StartCoroutine(this.HitMonster());
	}

	// Token: 0x06001528 RID: 5416 RVA: 0x00059488 File Offset: 0x00057688
	private void RpcReader___Observers_obsMonsterHit_431000436(PooledReader PooledReader0, Channel channel)
	{
		float num = PooledReader0.ReadSingle();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___obsMonsterHit_431000436(num);
	}

	// Token: 0x06001529 RID: 5417 RVA: 0x000594BC File Offset: 0x000576BC
	private void RpcWriter___Server_ServerDeathAni_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(12U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600152A RID: 5418 RVA: 0x00059521 File Offset: 0x00057721
	private void RpcLogic___ServerDeathAni_2166136261()
	{
		this.ObsDeathAni();
	}

	// Token: 0x0600152B RID: 5419 RVA: 0x0005952C File Offset: 0x0005772C
	private void RpcReader___Server_ServerDeathAni_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerDeathAni_2166136261();
	}

	// Token: 0x0600152C RID: 5420 RVA: 0x0005954C File Offset: 0x0005774C
	private void RpcWriter___Observers_ObsDeathAni_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(13U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600152D RID: 5421 RVA: 0x000595C0 File Offset: 0x000577C0
	private void RpcLogic___ObsDeathAni_2166136261()
	{
		this.AniController.isdead = true;
		this.AniController.MageAni.SetBool("dead", true);
		this.SkeleAudio.pitch = Random.Range(0.91f, 0.95f);
		this.SkeleAudio.PlayOneShot(this.SkeleClips[3]);
		base.StartCoroutine(this.WaitforPoof());
	}

	// Token: 0x0600152E RID: 5422 RVA: 0x0005962C File Offset: 0x0005782C
	private void RpcReader___Observers_ObsDeathAni_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsDeathAni_2166136261();
	}

	// Token: 0x0600152F RID: 5423 RVA: 0x00058B27 File Offset: 0x00056D27
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000C42 RID: 3138
	public int state = 2;

	// Token: 0x04000C43 RID: 3139
	public List<Transform> PotentialTargets = new List<Transform>();

	// Token: 0x04000C44 RID: 3140
	private Transform CurrentTarget;

	// Token: 0x04000C45 RID: 3141
	public NavMeshAgent navAgent;

	// Token: 0x04000C46 RID: 3142
	public LayerMask GroundLayer;

	// Token: 0x04000C47 RID: 3143
	private float stamina = 10f;

	// Token: 0x04000C48 RID: 3144
	private bool canRun = true;

	// Token: 0x04000C49 RID: 3145
	private float runCheckTimer;

	// Token: 0x04000C4A RID: 3146
	public Transform Spine003;

	// Token: 0x04000C4B RID: 3147
	private float fireBallCdTimer;

	// Token: 0x04000C4C RID: 3148
	public GameObject fireBall;

	// Token: 0x04000C4D RID: 3149
	public GameObject[] AiNodes;

	// Token: 0x04000C4E RID: 3150
	private Transform PreviousPatrolPoint;

	// Token: 0x04000C4F RID: 3151
	private Transform CurrentPatrolPoint;

	// Token: 0x04000C50 RID: 3152
	private float switchSearchNodeTimer;

	// Token: 0x04000C51 RID: 3153
	private float randomSwitchNodeValue;

	// Token: 0x04000C52 RID: 3154
	private bool inited;

	// Token: 0x04000C53 RID: 3155
	private bool gottenDirection;

	// Token: 0x04000C54 RID: 3156
	private Vector3 LastSeenPlayerPos;

	// Token: 0x04000C55 RID: 3157
	private int LastSeenPlayerHex;

	// Token: 0x04000C56 RID: 3158
	private NavMeshPath currentPath;

	// Token: 0x04000C57 RID: 3159
	private bool LookingAtPlayer;

	// Token: 0x04000C58 RID: 3160
	private bool canCastFB;

	// Token: 0x04000C59 RID: 3161
	private float fbTimer;

	// Token: 0x04000C5A RID: 3162
	private float fbGiveUpTimer;

	// Token: 0x04000C5B RID: 3163
	private bool goingLeft;

	// Token: 0x04000C5C RID: 3164
	public SkelemageAniController AniController;

	// Token: 0x04000C5D RID: 3165
	public AudioSource SkeleAudio;

	// Token: 0x04000C5E RID: 3166
	public AudioClip[] SkeleClips;

	// Token: 0x04000C5F RID: 3167
	public ParticleSystem particlePoint;

	// Token: 0x04000C60 RID: 3168
	private float playFbSoundCD;

	// Token: 0x04000C61 RID: 3169
	private float hp = 500f;

	// Token: 0x04000C62 RID: 3170
	private bool isDead;

	// Token: 0x04000C63 RID: 3171
	private PlayerRespawnManager PRM;

	// Token: 0x04000C64 RID: 3172
	public GameObject Skelepoof;

	// Token: 0x04000C65 RID: 3173
	private float soundcd;

	// Token: 0x04000C66 RID: 3174
	private bool NetworkInitialize___EarlySkelemageControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x04000C67 RID: 3175
	private bool NetworkInitialize__LateSkelemageControllerAssembly-CSharp.dll_Excuted;
}
