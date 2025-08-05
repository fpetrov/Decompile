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

// Token: 0x020000A5 RID: 165
public class GolemController : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x0600069C RID: 1692 RVA: 0x00019574 File Offset: 0x00017774
	private void Update()
	{
		if (this.isAtak && this.isPutDown)
		{
			if (this.mushAgent.isOnNavMesh)
			{
				this.mushAgent.SetDestination(base.transform.position);
			}
		}
		else if (this.isPutDown && this.target != null && this.mushAgent.isOnNavMesh)
		{
			this.mushAgent.SetDestination(this.target.position);
		}
		if (this.mushAgent.enabled && this.mushAgent.velocity.magnitude > 0.2f)
		{
			this.mushAni.SetBool("walk", true);
			if (this.footstepTimer > this.StepInterval)
			{
				if (this.FourstepIndex >= 4)
				{
					this.FourstepIndex = 2;
				}
				else
				{
					this.FourstepIndex++;
				}
				this.footstepTimer = 0f;
				this.asource.volume = Random.Range(0.7f, 1f);
				this.asource.pitch = Random.Range(0.9f, 1f);
				this.asource.PlayOneShot(this.aclips[this.FourstepIndex]);
			}
		}
		else
		{
			this.mushAni.SetBool("walk", false);
		}
		this.footstepTimer += Time.deltaTime;
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x000196DC File Offset: 0x000178DC
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction(GameObject player)
	{
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x00019710 File Offset: 0x00017910
	public void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			base.transform.position = raycastHit.point;
			this.isPutDown = true;
			this.mushAgent.enabled = true;
			PlayerMovement playerMovement;
			if (this.target != null && this.target.gameObject.TryGetComponent<PlayerMovement>(out playerMovement) && playerMovement.checkowner())
			{
				this.ownerTeamNumber = playerMovement.playerTeam;
				base.StartCoroutine(this.targetingRoutine());
				Debug.Log("targeting");
			}
			this.rockrender.transform.localPosition = new Vector3(0f, -0.1f, 0f);
			this.rockrender.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
		}
		this.asource.volume = 0.12f;
		this.asource.PlayOneShot(this.aclips[1]);
		this.rockrender.SetActive(true);
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x00019858 File Offset: 0x00017A58
	public void PlayDropSound()
	{
		this.asource.PlayOneShot(this.aclips[1]);
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x0001986D File Offset: 0x00017A6D
	private IEnumerator targetingRoutine()
	{
		while (this.isPutDown)
		{
			if (this.potentialTargets.Count > 0 && !this.isAtak)
			{
				foreach (Transform transform in this.potentialTargets)
				{
					ShadowWizardAI shadowWizardAI;
					PlayerMovement playerMovement;
					if (transform != null && ((transform.TryGetComponent<ShadowWizardAI>(out shadowWizardAI) && !this.isAtak) || (transform.TryGetComponent<PlayerMovement>(out playerMovement) && playerMovement.playerTeam != this.ownerTeamNumber && !this.isAtak)))
					{
						Vector3 vector = new Vector3(base.transform.position.x, base.transform.position.y + 1f, base.transform.position.z);
						Vector3 vector2 = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
						Vector3 normalized = (vector2 - vector).normalized;
						float num = Vector3.Distance(vector, vector2);
						if (num < 12f && !Physics.Raycast(vector, normalized, num, this.ground))
						{
							this.isAtak = true;
							this.ServerAtack(transform);
							Debug.Log("atak");
							yield return new WaitForSeconds(5f);
							break;
						}
					}
				}
				List<Transform>.Enumerator enumerator = default(List<Transform>.Enumerator);
			}
			yield return new WaitForSeconds(0.2f);
		}
		Debug.Log("exittargetroutine");
		yield break;
		yield break;
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x0001987C File Offset: 0x00017A7C
	private IEnumerator atackRoutine(Transform ataktarg)
	{
		this.isAtak = true;
		this.mushAni.Play("atak");
		this.mushAni.SetBool("atak", true);
		bool casted = false;
		float timer = 0f;
		this.asource.PlayOneShot(this.aclips[0]);
		while (this.isAtak)
		{
			timer += Time.deltaTime;
			if (ataktarg != null)
			{
				Vector3 vector = ataktarg.position - base.transform.position;
				vector.y = 0f;
				Quaternion quaternion = Quaternion.LookRotation(vector);
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, quaternion, Time.deltaTime * 8f);
				if (timer >= 1.4f && !casted)
				{
					this.asource.PlayOneShot(this.aclips[5]);
					if (ataktarg != null && Vector3.Distance(ataktarg.position, base.transform.position) < 14f)
					{
						PlayerMovement playerMovement;
						if (ataktarg.TryGetComponent<PlayerMovement>(out playerMovement))
						{
							base.StartCoroutine(this.dealdmgplayer(playerMovement));
						}
						else
						{
							ShadowWizardAI swai;
							if (ataktarg.TryGetComponent<ShadowWizardAI>(out swai))
							{
								yield return new WaitForSeconds(0.1f);
								swai.HitMonsterNotNetworked(20f);
							}
							swai = null;
						}
					}
					casted = true;
					this.particles.Play();
				}
			}
			if (timer >= 5f)
			{
				this.isAtak = false;
			}
			yield return null;
			this.mushAni.SetBool("atak", false);
		}
		yield break;
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x00019892 File Offset: 0x00017A92
	private IEnumerator dealdmgplayer(PlayerMovement pm)
	{
		yield return new WaitForSeconds(0.25f);
		if (pm != null)
		{
			Vector3 normalized = (pm.transform.position + Vector3.up - base.transform.position + Vector3.up).normalized;
			if (!Physics.Raycast(base.transform.position + Vector3.up, normalized, Vector3.Distance(base.transform.position, pm.transform.position), 8))
			{
				pm.NonRpcDamagePlayer(20f, this.target.gameObject, "golem");
			}
		}
		yield break;
	}

	// Token: 0x060006A5 RID: 1701 RVA: 0x000198A8 File Offset: 0x00017AA8
	[ServerRpc(RequireOwnership = false)]
	private void ServerAtack(Transform atakt)
	{
		this.RpcWriter___Server_ServerAtack_3068987916(atakt);
	}

	// Token: 0x060006A6 RID: 1702 RVA: 0x000198B4 File Offset: 0x00017AB4
	[ObserversRpc]
	private void ObsAtack(Transform ataktarg)
	{
		this.RpcWriter___Observers_ObsAtack_3068987916(ataktarg);
	}

	// Token: 0x060006A7 RID: 1703 RVA: 0x000198C0 File Offset: 0x00017AC0
	public void ItemInit()
	{
		this.rockrender.SetActive(true);
		this.rockrender.transform.localPosition = new Vector3(0.53f, 0.197f, -0.578f);
		this.rockrender.transform.localRotation = Quaternion.Euler(98.86f, -90f, -54.53f);
		this.asource.volume = 0.12f;
		this.asource.PlayOneShot(this.aclips[0]);
	}

	// Token: 0x060006A8 RID: 1704 RVA: 0x00019944 File Offset: 0x00017B44
	public void ItemInitObs()
	{
		this.rockrender.SetActive(true);
		this.rockrender.transform.localPosition = new Vector3(0.53f, 0.197f, -0.578f);
		this.rockrender.transform.localRotation = Quaternion.Euler(98.86f, -90f, -54.53f);
		this.asource.volume = 0.12f;
		this.asource.PlayOneShot(this.aclips[0]);
	}

	// Token: 0x060006A9 RID: 1705 RVA: 0x000199C8 File Offset: 0x00017BC8
	public void HideItem()
	{
		this.rockrender.SetActive(false);
	}

	// Token: 0x060006AA RID: 1706 RVA: 0x000199D8 File Offset: 0x00017BD8
	public void Interact(GameObject player)
	{
		PlayerMovement playerMovement;
		if (this.ownerTeamNumber == -1 || (player.TryGetComponent<PlayerMovement>(out playerMovement) && playerMovement.playerTeam == this.ownerTeamNumber))
		{
			player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
			this.rockrender.transform.localPosition = new Vector3(0.53f, 0.197f, -0.578f);
			this.rockrender.transform.localRotation = Quaternion.Euler(98.86f, -90f, -54.53f);
			this.toggleTarget(player);
			this.togglePutDown(false);
			this.potentialTargets = new List<Transform>();
		}
	}

	// Token: 0x060006AB RID: 1707 RVA: 0x00019A7B File Offset: 0x00017C7B
	[ServerRpc(RequireOwnership = false)]
	private void toggleTarget(GameObject fartd)
	{
		this.RpcWriter___Server_toggleTarget_1934289915(fartd);
	}

	// Token: 0x060006AC RID: 1708 RVA: 0x00019A88 File Offset: 0x00017C88
	[ObserversRpc]
	private void toggleTargetObs(GameObject tartd)
	{
		this.RpcWriter___Observers_toggleTargetObs_1934289915(tartd);
	}

	// Token: 0x060006AD RID: 1709 RVA: 0x00019A9F File Offset: 0x00017C9F
	[ServerRpc(RequireOwnership = false)]
	private void togglePutDown(bool fart)
	{
		this.RpcWriter___Server_togglePutDown_1140765316(fart);
	}

	// Token: 0x060006AE RID: 1710 RVA: 0x00019AAB File Offset: 0x00017CAB
	[ObserversRpc]
	private void togglePutDownObs(bool tart)
	{
		this.RpcWriter___Observers_togglePutDownObs_1140765316(tart);
	}

	// Token: 0x060006AF RID: 1711 RVA: 0x00019AB8 File Offset: 0x00017CB8
	public string DisplayInteractUI(GameObject player)
	{
		PlayerMovement playerMovement;
		if (this.ownerTeamNumber == -1 || (player.TryGetComponent<PlayerMovement>(out playerMovement) && playerMovement.playerTeam == this.ownerTeamNumber))
		{
			return "Grasp Golum";
		}
		return "";
	}

	// Token: 0x060006B0 RID: 1712 RVA: 0x00019AF1 File Offset: 0x00017CF1
	public int GetItemID()
	{
		return 6;
	}

	// Token: 0x060006B2 RID: 1714 RVA: 0x00019B30 File Offset: 0x00017D30
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyGolemControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyGolemControllerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ServerAtack_3068987916));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_ObsAtack_3068987916));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_toggleTarget_1934289915));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_toggleTargetObs_1934289915));
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_togglePutDown_1140765316));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_togglePutDownObs_1140765316));
	}

	// Token: 0x060006B3 RID: 1715 RVA: 0x00019BD8 File Offset: 0x00017DD8
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateGolemControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateGolemControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x060006B4 RID: 1716 RVA: 0x00019BEB File Offset: 0x00017DEB
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x060006B5 RID: 1717 RVA: 0x00019BFC File Offset: 0x00017DFC
	private void RpcWriter___Server_ServerAtack_3068987916(Transform atakt)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteTransform(atakt);
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060006B6 RID: 1718 RVA: 0x00019C6E File Offset: 0x00017E6E
	private void RpcLogic___ServerAtack_3068987916(Transform atakt)
	{
		this.ObsAtack(atakt);
	}

	// Token: 0x060006B7 RID: 1719 RVA: 0x00019C78 File Offset: 0x00017E78
	private void RpcReader___Server_ServerAtack_3068987916(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Transform transform = PooledReader0.ReadTransform();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerAtack_3068987916(transform);
	}

	// Token: 0x060006B8 RID: 1720 RVA: 0x00019CAC File Offset: 0x00017EAC
	private void RpcWriter___Observers_ObsAtack_3068987916(Transform ataktarg)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteTransform(ataktarg);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060006B9 RID: 1721 RVA: 0x00019D2D File Offset: 0x00017F2D
	private void RpcLogic___ObsAtack_3068987916(Transform ataktarg)
	{
		base.StartCoroutine(this.atackRoutine(ataktarg));
	}

	// Token: 0x060006BA RID: 1722 RVA: 0x00019D40 File Offset: 0x00017F40
	private void RpcReader___Observers_ObsAtack_3068987916(PooledReader PooledReader0, Channel channel)
	{
		Transform transform = PooledReader0.ReadTransform();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsAtack_3068987916(transform);
	}

	// Token: 0x060006BB RID: 1723 RVA: 0x00019D74 File Offset: 0x00017F74
	private void RpcWriter___Server_toggleTarget_1934289915(GameObject fartd)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(fartd);
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060006BC RID: 1724 RVA: 0x00019DE6 File Offset: 0x00017FE6
	private void RpcLogic___toggleTarget_1934289915(GameObject fartd)
	{
		this.toggleTargetObs(fartd);
	}

	// Token: 0x060006BD RID: 1725 RVA: 0x00019DF0 File Offset: 0x00017FF0
	private void RpcReader___Server_toggleTarget_1934289915(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___toggleTarget_1934289915(gameObject);
	}

	// Token: 0x060006BE RID: 1726 RVA: 0x00019E24 File Offset: 0x00018024
	private void RpcWriter___Observers_toggleTargetObs_1934289915(GameObject tartd)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(tartd);
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060006BF RID: 1727 RVA: 0x00019EA8 File Offset: 0x000180A8
	private void RpcLogic___toggleTargetObs_1934289915(GameObject tartd)
	{
		this.target = tartd.transform;
		PlayerMovement playerMovement;
		if (tartd.TryGetComponent<PlayerMovement>(out playerMovement))
		{
			this.ownerTeamNumber = playerMovement.playerTeam;
		}
	}

	// Token: 0x060006C0 RID: 1728 RVA: 0x00019ED8 File Offset: 0x000180D8
	private void RpcReader___Observers_toggleTargetObs_1934289915(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___toggleTargetObs_1934289915(gameObject);
	}

	// Token: 0x060006C1 RID: 1729 RVA: 0x00019F0C File Offset: 0x0001810C
	private void RpcWriter___Server_togglePutDown_1140765316(bool fart)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteBoolean(fart);
		base.SendServerRpc(4U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060006C2 RID: 1730 RVA: 0x00019F7E File Offset: 0x0001817E
	private void RpcLogic___togglePutDown_1140765316(bool fart)
	{
		this.isPutDown = fart;
		this.togglePutDownObs(fart);
	}

	// Token: 0x060006C3 RID: 1731 RVA: 0x00019F90 File Offset: 0x00018190
	private void RpcReader___Server_togglePutDown_1140765316(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___togglePutDown_1140765316(flag);
	}

	// Token: 0x060006C4 RID: 1732 RVA: 0x00019FC4 File Offset: 0x000181C4
	private void RpcWriter___Observers_togglePutDownObs_1140765316(bool tart)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteBoolean(tart);
		base.SendObserversRpc(5U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060006C5 RID: 1733 RVA: 0x0001A045 File Offset: 0x00018245
	private void RpcLogic___togglePutDownObs_1140765316(bool tart)
	{
		this.isPutDown = tart;
		this.mushAgent.enabled = tart;
	}

	// Token: 0x060006C6 RID: 1734 RVA: 0x0001A05C File Offset: 0x0001825C
	private void RpcReader___Observers_togglePutDownObs_1140765316(PooledReader PooledReader0, Channel channel)
	{
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___togglePutDownObs_1140765316(flag);
	}

	// Token: 0x060006C7 RID: 1735 RVA: 0x00019BEB File Offset: 0x00017DEB
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000346 RID: 838
	public GameObject rockrender;

	// Token: 0x04000347 RID: 839
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x04000348 RID: 840
	private Transform target;

	// Token: 0x04000349 RID: 841
	public ParticleSystem particles;

	// Token: 0x0400034A RID: 842
	public NavMeshAgent mushAgent;

	// Token: 0x0400034B RID: 843
	private bool isPutDown;

	// Token: 0x0400034C RID: 844
	public List<Transform> potentialTargets = new List<Transform>();

	// Token: 0x0400034D RID: 845
	public int ownerTeamNumber = -1;

	// Token: 0x0400034E RID: 846
	public Animator mushAni;

	// Token: 0x0400034F RID: 847
	private bool isAtak;

	// Token: 0x04000350 RID: 848
	private Transform atakTarget;

	// Token: 0x04000351 RID: 849
	public LayerMask ground;

	// Token: 0x04000352 RID: 850
	public AudioSource asource;

	// Token: 0x04000353 RID: 851
	public AudioClip[] aclips;

	// Token: 0x04000354 RID: 852
	public float StepInterval;

	// Token: 0x04000355 RID: 853
	private int FourstepIndex = 2;

	// Token: 0x04000356 RID: 854
	private float footstepTimer;

	// Token: 0x04000357 RID: 855
	private bool NetworkInitialize___EarlyGolemControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x04000358 RID: 856
	private bool NetworkInitialize__LateGolemControllerAssembly-CSharp.dll_Excuted;
}
