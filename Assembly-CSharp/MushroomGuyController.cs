using System;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020000F0 RID: 240
public class MushroomGuyController : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x060009BB RID: 2491 RVA: 0x00025678 File Offset: 0x00023878
	private void Update()
	{
		if (this.isPutDown)
		{
			if (this.mushAgent.isOnNavMesh)
			{
				this.mushAgent.SetDestination(this.target.position);
			}
			if (this.mushAgent.velocity.magnitude > 0.2f)
			{
				this.mushAni.SetBool("run", true);
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
					this.asource.volume = Random.Range(0.05f, 0.1f);
					this.asource.pitch = Random.Range(0.9f, 1f);
					this.asource.PlayOneShot(this.aclips[this.FourstepIndex]);
				}
			}
			else
			{
				this.mushAni.SetBool("run", false);
			}
		}
		else
		{
			this.mushAni.SetBool("run", false);
		}
		this.footstepTimer += Time.deltaTime;
	}

	// Token: 0x060009BC RID: 2492 RVA: 0x000257A2 File Offset: 0x000239A2
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x060009BD RID: 2493 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction(GameObject player)
	{
	}

	// Token: 0x060009BE RID: 2494 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x060009BF RID: 2495 RVA: 0x000257D4 File Offset: 0x000239D4
	public void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			base.transform.position = raycastHit.point;
			this.togglePutDown(true);
			this.rockrender.transform.localPosition = new Vector3(0f, -0.1f, 0f);
			this.rockrender.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
		}
		this.asource.volume = 0.6f;
		this.asource.PlayOneShot(this.aclips[1]);
		this.rockrender.SetActive(true);
	}

	// Token: 0x060009C0 RID: 2496 RVA: 0x000258C3 File Offset: 0x00023AC3
	public void PlayDropSound()
	{
		this.asource.PlayOneShot(this.aclips[1]);
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x000258D8 File Offset: 0x00023AD8
	public void ItemInit()
	{
		this.rockrender.SetActive(true);
		this.rockrender.transform.localPosition = new Vector3(0.53f, 0.197f, -0.578f);
		this.rockrender.transform.localRotation = Quaternion.Euler(98.86f, -90f, -54.53f);
		this.asource.volume = 0.6f;
		this.asource.PlayOneShot(this.aclips[0]);
	}

	// Token: 0x060009C2 RID: 2498 RVA: 0x0002595C File Offset: 0x00023B5C
	public void ItemInitObs()
	{
		this.rockrender.SetActive(true);
		this.rockrender.transform.localPosition = new Vector3(0.53f, 0.197f, -0.578f);
		this.rockrender.transform.localRotation = Quaternion.Euler(98.86f, -90f, -54.53f);
		this.asource.volume = 0.6f;
		this.asource.PlayOneShot(this.aclips[0]);
	}

	// Token: 0x060009C3 RID: 2499 RVA: 0x000259E0 File Offset: 0x00023BE0
	public void HideItem()
	{
		this.rockrender.SetActive(false);
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x000259F0 File Offset: 0x00023BF0
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
		this.rockrender.transform.localPosition = new Vector3(0.53f, 0.197f, -0.578f);
		this.rockrender.transform.localRotation = Quaternion.Euler(98.86f, -90f, -54.53f);
		this.toggleTarget(player);
		this.togglePutDown(false);
	}

	// Token: 0x060009C5 RID: 2501 RVA: 0x00025A64 File Offset: 0x00023C64
	[ServerRpc(RequireOwnership = false)]
	private void toggleTarget(GameObject fartd)
	{
		this.RpcWriter___Server_toggleTarget_1934289915(fartd);
	}

	// Token: 0x060009C6 RID: 2502 RVA: 0x00025A70 File Offset: 0x00023C70
	[ObserversRpc]
	private void toggleTargetObs(GameObject tartd)
	{
		this.RpcWriter___Observers_toggleTargetObs_1934289915(tartd);
	}

	// Token: 0x060009C7 RID: 2503 RVA: 0x00025A7C File Offset: 0x00023C7C
	[ServerRpc(RequireOwnership = false)]
	private void togglePutDown(bool fart)
	{
		this.RpcWriter___Server_togglePutDown_1140765316(fart);
	}

	// Token: 0x060009C8 RID: 2504 RVA: 0x00025A88 File Offset: 0x00023C88
	[ObserversRpc]
	private void togglePutDownObs(bool tart)
	{
		this.RpcWriter___Observers_togglePutDownObs_1140765316(tart);
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x00025A94 File Offset: 0x00023C94
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp Mushroom Man";
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x00025A9B File Offset: 0x00023C9B
	public int GetItemID()
	{
		return 7;
	}

	// Token: 0x060009CB RID: 2507 RVA: 0x00025A9E File Offset: 0x00023C9E
	public void PlayMGSound()
	{
		this.asource.volume = 0.5f;
		this.asource.PlayOneShot(this.aclips[Random.Range(5, this.aclips.Length)]);
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x00025AFC File Offset: 0x00023CFC
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyMushroomGuyControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyMushroomGuyControllerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_toggleTarget_1934289915));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_toggleTargetObs_1934289915));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_togglePutDown_1140765316));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_togglePutDownObs_1140765316));
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x00025B76 File Offset: 0x00023D76
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateMushroomGuyControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateMushroomGuyControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x060009CF RID: 2511 RVA: 0x00025B89 File Offset: 0x00023D89
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x060009D0 RID: 2512 RVA: 0x00025B98 File Offset: 0x00023D98
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
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060009D1 RID: 2513 RVA: 0x00025C0A File Offset: 0x00023E0A
	private void RpcLogic___toggleTarget_1934289915(GameObject fartd)
	{
		this.toggleTargetObs(fartd);
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x00025C14 File Offset: 0x00023E14
	private void RpcReader___Server_toggleTarget_1934289915(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___toggleTarget_1934289915(gameObject);
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x00025C48 File Offset: 0x00023E48
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
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x00025CC9 File Offset: 0x00023EC9
	private void RpcLogic___toggleTargetObs_1934289915(GameObject tartd)
	{
		this.target = tartd.transform;
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x00025CD8 File Offset: 0x00023ED8
	private void RpcReader___Observers_toggleTargetObs_1934289915(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___toggleTargetObs_1934289915(gameObject);
	}

	// Token: 0x060009D6 RID: 2518 RVA: 0x00025D0C File Offset: 0x00023F0C
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
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060009D7 RID: 2519 RVA: 0x00025D7E File Offset: 0x00023F7E
	private void RpcLogic___togglePutDown_1140765316(bool fart)
	{
		this.togglePutDownObs(fart);
	}

	// Token: 0x060009D8 RID: 2520 RVA: 0x00025D88 File Offset: 0x00023F88
	private void RpcReader___Server_togglePutDown_1140765316(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___togglePutDown_1140765316(flag);
	}

	// Token: 0x060009D9 RID: 2521 RVA: 0x00025DBC File Offset: 0x00023FBC
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
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060009DA RID: 2522 RVA: 0x00025E3D File Offset: 0x0002403D
	private void RpcLogic___togglePutDownObs_1140765316(bool tart)
	{
		this.isPutDown = tart;
		this.mushAgent.enabled = tart;
	}

	// Token: 0x060009DB RID: 2523 RVA: 0x00025E54 File Offset: 0x00024054
	private void RpcReader___Observers_togglePutDownObs_1140765316(PooledReader PooledReader0, Channel channel)
	{
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___togglePutDownObs_1140765316(flag);
	}

	// Token: 0x060009DC RID: 2524 RVA: 0x00025B89 File Offset: 0x00023D89
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0400052A RID: 1322
	public GameObject rockrender;

	// Token: 0x0400052B RID: 1323
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x0400052C RID: 1324
	private Transform target;

	// Token: 0x0400052D RID: 1325
	private Transform backupTarget;

	// Token: 0x0400052E RID: 1326
	public NavMeshAgent mushAgent;

	// Token: 0x0400052F RID: 1327
	private bool isPutDown;

	// Token: 0x04000530 RID: 1328
	public int ownerTeamNumber;

	// Token: 0x04000531 RID: 1329
	public Animator mushAni;

	// Token: 0x04000532 RID: 1330
	public AudioSource asource;

	// Token: 0x04000533 RID: 1331
	public AudioClip[] aclips;

	// Token: 0x04000534 RID: 1332
	public float StepInterval;

	// Token: 0x04000535 RID: 1333
	private int FourstepIndex = 2;

	// Token: 0x04000536 RID: 1334
	private float footstepTimer;

	// Token: 0x04000537 RID: 1335
	private bool NetworkInitialize___EarlyMushroomGuyControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x04000538 RID: 1336
	private bool NetworkInitialize__LateMushroomGuyControllerAssembly-CSharp.dll_Excuted;
}
