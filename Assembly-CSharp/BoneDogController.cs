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

// Token: 0x02000010 RID: 16
public class BoneDogController : NetworkBehaviour
{
	// Token: 0x0600009F RID: 159 RVA: 0x0000444F File Offset: 0x0000264F
	public override void OnStartClient()
	{
		base.OnStartClient();
		if (base.HasAuthority)
		{
			this.DogAgent.enabled = false;
			base.StartCoroutine(this.WaitforGameStart());
			return;
		}
		this.DogAgent.enabled = false;
	}

	// Token: 0x060000A0 RID: 160 RVA: 0x00004485 File Offset: 0x00002685
	private IEnumerator WaitforGameStart()
	{
		this.BoneDog.position = this.bonedogstartpos.position;
		MainMenuManager mmm = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MainMenuManager>();
		while (!mmm.GameHasStarted)
		{
			yield return new WaitForSeconds(0.1f);
		}
		this.DogAgent.enabled = true;
		this.inited = true;
		yield break;
	}

	// Token: 0x060000A1 RID: 161 RVA: 0x00004494 File Offset: 0x00002694
	private void Update()
	{
		this.magnitude = Mathf.Lerp(this.magnitude, ((this.BoneDog.transform.position - this.prevFramPos) / Time.deltaTime).magnitude, Time.deltaTime * 2f);
		if (this.state == 0 || this.state == 1)
		{
			if (this.magnitude < 1f)
			{
				this.BoneDogAni.SetBool("idle", true);
			}
			else
			{
				this.BoneDogAni.SetBool("idle", false);
			}
		}
		this.footstepTimer += Time.deltaTime;
		if (this.magnitude > 3f && this.footstepTimer > this.runStepInterval)
		{
			if (this.FourstepIndex >= 8 || this.FourstepIndex <= 4)
			{
				this.FourstepIndex = 5;
			}
			else
			{
				this.FourstepIndex++;
			}
			this.footstepTimer = 0f;
			this.FootStepSource.pitch = Random.Range(0.6f, 0.8f);
			this.FootStepSource.PlayOneShot(this.BoneDogClips[this.FourstepIndex]);
			this.FootStepSource.volume = Random.Range(0.9f, 1f);
			foreach (Collider collider in Physics.OverlapSphere(this.BoneDog.position, 30f, this.PlayerLayer))
			{
				PlayerMovement playerMovement;
				if (collider.CompareTag("Player") && collider.gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
				{
					playerMovement.ShakeCam((30f - Vector3.Distance(this.BoneDog.position, collider.transform.position)) / 15f, (30f - Vector3.Distance(this.BoneDog.position, collider.transform.position)) / 15f);
				}
			}
		}
		else if (this.magnitude > 0.3f && this.footstepTimer > this.walkStepInterval)
		{
			if (this.FourstepIndex >= 8 || this.FourstepIndex <= 4)
			{
				this.FourstepIndex = 5;
			}
			else
			{
				this.FourstepIndex++;
			}
			this.footstepTimer = 0f;
			this.FootStepSource.pitch = Random.Range(0.8f, 1f);
			this.FootStepSource.PlayOneShot(this.BoneDogClips[this.FourstepIndex]);
			this.FootStepSource.volume = Random.Range(0.5f, 0.6f);
		}
		PlayerMovement playerMovement2;
		if (this.CurrentTarget != null && Vector3.Distance(this.VisualObject.position, this.CurrentTarget.position) < 2.6f && this.CurrentTarget.TryGetComponent<PlayerMovement>(out playerMovement2))
		{
			playerMovement2.NonRpcDamagePlayer(70f * Time.deltaTime, null, "dog");
		}
		if (base.HasAuthority && this.inited)
		{
			this.AIBehavior();
		}
		this.prevFramPos = this.BoneDog.transform.position;
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x000047B4 File Offset: 0x000029B4
	private void LateUpdate()
	{
		if (this.LookAtTarget && this.CurrentTarget != null)
		{
			Vector3 vector = this.CurrentTarget.position - this.NeckBone.position;
			Quaternion quaternion = Quaternion.FromToRotation(this.NeckBone.up, vector.normalized) * this.NeckBone.rotation;
			this.NeckBone.rotation = quaternion;
		}
	}

	// Token: 0x060000A3 RID: 163 RVA: 0x00004828 File Offset: 0x00002A28
	private void AIBehavior()
	{
		int num = this.state;
		if (num == 0)
		{
			if (this.currentNode == -1 || Vector3.Distance(this.BoneDog.position, this.patrolpoints[this.currentNode].position) < 4f)
			{
				this.DogAgent.speed = 2f;
				if (this.currentNode < this.patrolpoints.Length - 1)
				{
					this.currentNode++;
				}
				else
				{
					this.currentNode = 0;
				}
				this.DogAgent.SetDestination(this.patrolpoints[this.currentNode].position);
				if (Random.Range(0, 2) == 0)
				{
					this.PlaySound(Random.Range(3, 5));
				}
			}
			if (this.dropaggrocd > 0f)
			{
				this.dropaggrocd -= Time.deltaTime;
			}
			else if (this.PotentialTargets.Count > 0 && this.SoundTimer > 4f)
			{
				this.LOSCheck();
			}
			this.SoundTimer += 1f;
			return;
		}
		if (num != 1)
		{
			return;
		}
		this.DogAgent.speed = 8.5f;
		if (this.CurrentTarget != null)
		{
			this.DogAgent.SetDestination(this.CurrentTarget.position);
		}
		Vector3 vector;
		if (!this.adjustedLOSRayCast(this.CurrentTarget, true, out vector) || Vector3.Angle(vector, this.BoneDog.forward) >= 90f || this.DogAgent.velocity.magnitude < 0.2f)
		{
			this.GiveUpChaseTimer += Time.deltaTime;
		}
		else if (this.lastBarkPlayed == 0f && this.SoundTimer > 5.5f)
		{
			this.SoundTimer = 0f;
			this.lastBarkPlayed = 1f;
			this.PlaySound(2);
		}
		else if (this.lastBarkPlayed == 1f && this.SoundTimer > 3.5f)
		{
			this.SoundTimer = 0f;
			this.lastBarkPlayed = 0f;
			this.PlaySound(1);
		}
		if (this.lastGrowlPlayed == 0f && this.SoundTimer2 > 6f)
		{
			this.SoundTimer2 = 0f;
			this.lastGrowlPlayed = 1f;
			this.PlaySound(3);
		}
		else if (this.lastGrowlPlayed == 1f && this.SoundTimer2 > 7f)
		{
			this.SoundTimer2 = 0f;
			this.lastGrowlPlayed = 0f;
			this.PlaySound(4);
		}
		if (this.GiveUpChaseTimer > 15f)
		{
			this.dropaggrocd = 3f;
			this.state = 0;
			this.ToggleLookAtTarget(false, null);
			this.currentNode = -1;
			this.SoundTimer = 0f;
			this.ServerRunAniToggle(false);
		}
		this.SoundTimer += Time.deltaTime;
		this.SoundTimer2 += Time.deltaTime;
	}

	// Token: 0x060000A4 RID: 164 RVA: 0x00004B0B File Offset: 0x00002D0B
	[ServerRpc(RequireOwnership = false)]
	private void ServerRunAniToggle(bool val)
	{
		this.RpcWriter___Server_ServerRunAniToggle_1140765316(val);
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x00004B17 File Offset: 0x00002D17
	[ObserversRpc]
	private void ObsRunAniToggle(bool val)
	{
		this.RpcWriter___Observers_ObsRunAniToggle_1140765316(val);
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x00004B24 File Offset: 0x00002D24
	private void LOSCheck()
	{
		foreach (Transform transform in this.PotentialTargets)
		{
			if (Vector3.Distance(this.VisualObject.transform.position, transform.position) < 9f)
			{
				this.PlaySound(0);
				this.PlaySound(1);
				this.currentNode = -1;
				this.GiveUpChaseTimer = 0f;
				this.state = 1;
				this.CurrentTarget = transform;
				this.ToggleLookAtTarget(true, this.CurrentTarget);
				this.SoundTimer = 0f;
				this.SoundTimer2 = 0f;
				this.ServerRunAniToggle(true);
				break;
			}
			Vector3 vector;
			if (this.adjustedLOSRayCast(transform, true, out vector) && Vector3.Angle(vector, this.BoneDog.forward) < 45f)
			{
				this.PlaySound(0);
				this.PlaySound(1);
				this.currentNode = -1;
				this.GiveUpChaseTimer = 0f;
				this.state = 1;
				this.CurrentTarget = transform;
				this.ToggleLookAtTarget(true, this.CurrentTarget);
				this.SoundTimer = 0f;
				this.SoundTimer2 = 0f;
				this.ServerRunAniToggle(true);
				break;
			}
		}
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x00004C80 File Offset: 0x00002E80
	private bool adjustedLOSRayCast(Transform target, bool towardsTarget, out Vector3 direction)
	{
		PlayerMovement playerMovement;
		GetPlayerGameobject getPlayerGameobject;
		if ((target.TryGetComponent<PlayerMovement>(out playerMovement) && playerMovement.isDead) || (target.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject) && getPlayerGameobject.player.GetComponent<PlayerMovement>().isDead))
		{
			direction = new Vector3(0f, 0f, 0f);
			return false;
		}
		if (towardsTarget)
		{
			Vector3 vector = new Vector3(this.BoneDog.position.x + 1f, this.BoneDog.position.y + 1.5f, this.BoneDog.position.z);
			Vector3 vector2 = new Vector3(target.position.x, target.position.y + 2f, target.position.z);
			direction = vector2 - vector;
			return !Physics.Raycast(vector, direction, direction.magnitude, this.GroundLayer) && direction.magnitude < 70f;
		}
		Vector3 vector3 = new Vector3(this.BoneDog.position.x + 1f, this.BoneDog.position.y + 1.5f, this.BoneDog.position.z);
		Vector3 vector4 = new Vector3(target.position.x, target.position.y + 2f, target.position.z);
		direction = vector3 - vector4;
		return !Physics.Raycast(vector4, direction, direction.magnitude, this.GroundLayer);
	}

	// Token: 0x060000A8 RID: 168 RVA: 0x00004E2B File Offset: 0x0000302B
	[ServerRpc(RequireOwnership = false)]
	private void PlaySound(int id)
	{
		this.RpcWriter___Server_PlaySound_3316948804(id);
	}

	// Token: 0x060000A9 RID: 169 RVA: 0x00004E38 File Offset: 0x00003038
	[ObserversRpc]
	private void ObsPlaySound(int id)
	{
		this.RpcWriter___Observers_ObsPlaySound_3316948804(id);
	}

	// Token: 0x060000AA RID: 170 RVA: 0x00004E4F File Offset: 0x0000304F
	private IEnumerator ApplyThugShake(PlayerMovement pm, float value)
	{
		for (float timer = 0f; timer < 1f; timer += Time.deltaTime)
		{
			pm.ShakeCam(value, value);
			yield return null;
		}
		yield break;
	}

	// Token: 0x060000AB RID: 171 RVA: 0x00004E65 File Offset: 0x00003065
	[ServerRpc(RequireOwnership = false)]
	private void ToggleLookAtTarget(bool tf, Transform targ)
	{
		this.RpcWriter___Server_ToggleLookAtTarget_3024699591(tf, targ);
	}

	// Token: 0x060000AC RID: 172 RVA: 0x00004E75 File Offset: 0x00003075
	[ObserversRpc]
	private void ToggleLookAtObs(bool tf, Transform targ)
	{
		this.RpcWriter___Observers_ToggleLookAtObs_3024699591(tf, targ);
	}

	// Token: 0x060000AE RID: 174 RVA: 0x00004EA0 File Offset: 0x000030A0
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyBoneDogControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyBoneDogControllerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ServerRunAniToggle_1140765316));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_ObsRunAniToggle_1140765316));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_PlaySound_3316948804));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ObsPlaySound_3316948804));
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_ToggleLookAtTarget_3024699591));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_ToggleLookAtObs_3024699591));
	}

	// Token: 0x060000AF RID: 175 RVA: 0x00004F48 File Offset: 0x00003148
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateBoneDogControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateBoneDogControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x060000B0 RID: 176 RVA: 0x00004F5B File Offset: 0x0000315B
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x060000B1 RID: 177 RVA: 0x00004F6C File Offset: 0x0000316C
	private void RpcWriter___Server_ServerRunAniToggle_1140765316(bool val)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteBoolean(val);
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060000B2 RID: 178 RVA: 0x00004FDE File Offset: 0x000031DE
	private void RpcLogic___ServerRunAniToggle_1140765316(bool val)
	{
		this.ObsRunAniToggle(val);
	}

	// Token: 0x060000B3 RID: 179 RVA: 0x00004FE8 File Offset: 0x000031E8
	private void RpcReader___Server_ServerRunAniToggle_1140765316(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerRunAniToggle_1140765316(flag);
	}

	// Token: 0x060000B4 RID: 180 RVA: 0x0000501C File Offset: 0x0000321C
	private void RpcWriter___Observers_ObsRunAniToggle_1140765316(bool val)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteBoolean(val);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060000B5 RID: 181 RVA: 0x0000509D File Offset: 0x0000329D
	private void RpcLogic___ObsRunAniToggle_1140765316(bool val)
	{
		this.BoneDogAni.SetBool("run", val);
	}

	// Token: 0x060000B6 RID: 182 RVA: 0x000050B0 File Offset: 0x000032B0
	private void RpcReader___Observers_ObsRunAniToggle_1140765316(PooledReader PooledReader0, Channel channel)
	{
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsRunAniToggle_1140765316(flag);
	}

	// Token: 0x060000B7 RID: 183 RVA: 0x000050E4 File Offset: 0x000032E4
	private void RpcWriter___Server_PlaySound_3316948804(int id)
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
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060000B8 RID: 184 RVA: 0x00005156 File Offset: 0x00003356
	private void RpcLogic___PlaySound_3316948804(int id)
	{
		this.ObsPlaySound(id);
	}

	// Token: 0x060000B9 RID: 185 RVA: 0x00005160 File Offset: 0x00003360
	private void RpcReader___Server_PlaySound_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___PlaySound_3316948804(num);
	}

	// Token: 0x060000BA RID: 186 RVA: 0x00005194 File Offset: 0x00003394
	private void RpcWriter___Observers_ObsPlaySound_3316948804(int id)
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
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060000BB RID: 187 RVA: 0x00005218 File Offset: 0x00003418
	private void RpcLogic___ObsPlaySound_3316948804(int id)
	{
		this.BoneDogAudio.pitch = Random.Range(0.9f, 1f);
		this.BoneDogAudio.PlayOneShot(this.BoneDogClips[id]);
		if (id == 0)
		{
			foreach (Collider collider in Physics.OverlapSphere(base.transform.position, 40f, this.PlayerLayer))
			{
				PlayerMovement playerMovement;
				if (collider.CompareTag("Player") && collider.gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
				{
					playerMovement.ShakeCam((40f - Vector3.Distance(base.transform.position, collider.transform.position)) / 5f, (40f - Vector3.Distance(base.transform.position, collider.transform.position)) / 5f);
					base.StartCoroutine(this.ApplyThugShake(playerMovement, (40f - Vector3.Distance(base.transform.position, collider.transform.position)) / 5f));
				}
			}
		}
	}

	// Token: 0x060000BC RID: 188 RVA: 0x0000533C File Offset: 0x0000353C
	private void RpcReader___Observers_ObsPlaySound_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsPlaySound_3316948804(num);
	}

	// Token: 0x060000BD RID: 189 RVA: 0x00005370 File Offset: 0x00003570
	private void RpcWriter___Server_ToggleLookAtTarget_3024699591(bool tf, Transform targ)
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
		pooledWriter.WriteTransform(targ);
		base.SendServerRpc(4U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060000BE RID: 190 RVA: 0x000053EF File Offset: 0x000035EF
	private void RpcLogic___ToggleLookAtTarget_3024699591(bool tf, Transform targ)
	{
		this.ToggleLookAtObs(tf, targ);
	}

	// Token: 0x060000BF RID: 191 RVA: 0x000053FC File Offset: 0x000035FC
	private void RpcReader___Server_ToggleLookAtTarget_3024699591(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		bool flag = PooledReader0.ReadBoolean();
		Transform transform = PooledReader0.ReadTransform();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ToggleLookAtTarget_3024699591(flag, transform);
	}

	// Token: 0x060000C0 RID: 192 RVA: 0x00005440 File Offset: 0x00003640
	private void RpcWriter___Observers_ToggleLookAtObs_3024699591(bool tf, Transform targ)
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
		pooledWriter.WriteTransform(targ);
		base.SendObserversRpc(5U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060000C1 RID: 193 RVA: 0x000054CE File Offset: 0x000036CE
	private void RpcLogic___ToggleLookAtObs_3024699591(bool tf, Transform targ)
	{
		this.LookAtTarget = tf;
		this.CurrentTarget = targ;
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x000054E0 File Offset: 0x000036E0
	private void RpcReader___Observers_ToggleLookAtObs_3024699591(PooledReader PooledReader0, Channel channel)
	{
		bool flag = PooledReader0.ReadBoolean();
		Transform transform = PooledReader0.ReadTransform();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ToggleLookAtObs_3024699591(flag, transform);
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x00004F5B File Offset: 0x0000315B
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000041 RID: 65
	public List<Transform> PotentialTargets = new List<Transform>();

	// Token: 0x04000042 RID: 66
	private Transform CurrentTarget;

	// Token: 0x04000043 RID: 67
	public Animator BoneDogAni;

	// Token: 0x04000044 RID: 68
	public NavMeshAgent DogAgent;

	// Token: 0x04000045 RID: 69
	public Transform[] patrolpoints;

	// Token: 0x04000046 RID: 70
	private int currentNode = -1;

	// Token: 0x04000047 RID: 71
	private int state;

	// Token: 0x04000048 RID: 72
	public Transform BoneDog;

	// Token: 0x04000049 RID: 73
	public LayerMask GroundLayer;

	// Token: 0x0400004A RID: 74
	public LayerMask PlayerLayer;

	// Token: 0x0400004B RID: 75
	private float GiveUpChaseTimer;

	// Token: 0x0400004C RID: 76
	public Transform NeckBone;

	// Token: 0x0400004D RID: 77
	public AudioSource BoneDogAudio;

	// Token: 0x0400004E RID: 78
	public AudioClip[] BoneDogClips;

	// Token: 0x0400004F RID: 79
	public Transform VisualObject;

	// Token: 0x04000050 RID: 80
	public AudioSource FootStepSource;

	// Token: 0x04000051 RID: 81
	private float SoundTimer;

	// Token: 0x04000052 RID: 82
	private float SoundTimer2;

	// Token: 0x04000053 RID: 83
	private float lastBarkPlayed;

	// Token: 0x04000054 RID: 84
	private float lastGrowlPlayed;

	// Token: 0x04000055 RID: 85
	private float footstepTimer;

	// Token: 0x04000056 RID: 86
	public float walkStepInterval;

	// Token: 0x04000057 RID: 87
	public float runStepInterval;

	// Token: 0x04000058 RID: 88
	private int FourstepIndex;

	// Token: 0x04000059 RID: 89
	private bool LookAtTarget;

	// Token: 0x0400005A RID: 90
	private bool inited;

	// Token: 0x0400005B RID: 91
	private Vector3 prevFramPos;

	// Token: 0x0400005C RID: 92
	private float magnitude;

	// Token: 0x0400005D RID: 93
	private float dropaggrocd;

	// Token: 0x0400005E RID: 94
	public Transform bonedogstartpos;

	// Token: 0x0400005F RID: 95
	private bool NetworkInitialize___EarlyBoneDogControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x04000060 RID: 96
	private bool NetworkInitialize__LateBoneDogControllerAssembly-CSharp.dll_Excuted;
}
