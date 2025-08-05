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

// Token: 0x02000199 RID: 409
public class SoupManController : NetworkBehaviour
{
	// Token: 0x06001111 RID: 4369 RVA: 0x00049F2A File Offset: 0x0004812A
	private void Start()
	{
		this.normalRot = this.spinebone.rotation;
		this.prevrot = this.spinebone.rotation;
	}

	// Token: 0x06001112 RID: 4370 RVA: 0x00049F4E File Offset: 0x0004814E
	public void playspeakline()
	{
		this.Serverplayline();
	}

	// Token: 0x06001113 RID: 4371 RVA: 0x00049F56 File Offset: 0x00048156
	[ServerRpc(RequireOwnership = false)]
	private void Serverplayline()
	{
		this.RpcWriter___Server_Serverplayline_2166136261();
	}

	// Token: 0x06001114 RID: 4372 RVA: 0x00049F5E File Offset: 0x0004815E
	[ObserversRpc]
	private void obsplayline()
	{
		this.RpcWriter___Observers_obsplayline_2166136261();
	}

	// Token: 0x06001115 RID: 4373 RVA: 0x00049F66 File Offset: 0x00048166
	public void CookSoup(int id)
	{
		this.ServerCookSoup(id);
	}

	// Token: 0x06001116 RID: 4374 RVA: 0x00049F6F File Offset: 0x0004816F
	[ServerRpc(RequireOwnership = false)]
	private void ServerCookSoup(int id)
	{
		this.RpcWriter___Server_ServerCookSoup_3316948804(id);
	}

	// Token: 0x06001117 RID: 4375 RVA: 0x00049F7B File Offset: 0x0004817B
	[ObserversRpc]
	private void ObsMakeSoup(int id)
	{
		this.RpcWriter___Observers_ObsMakeSoup_3316948804(id);
	}

	// Token: 0x06001118 RID: 4376 RVA: 0x00049F87 File Offset: 0x00048187
	private IEnumerator waittoplaysound()
	{
		yield return new WaitForSeconds(2f);
		this.soupsoiurce.PlayOneShot(this.makeosup);
		yield break;
	}

	// Token: 0x06001119 RID: 4377 RVA: 0x00049F96 File Offset: 0x00048196
	private IEnumerator SoupRoutine(int id)
	{
		yield return new WaitForSeconds(5f);
		this.ServerSoupRoutine(id);
		yield break;
	}

	// Token: 0x0600111A RID: 4378 RVA: 0x00049FAC File Offset: 0x000481AC
	[ServerRpc(RequireOwnership = false)]
	private void ServerSoupRoutine(int id)
	{
		this.RpcWriter___Server_ServerSoupRoutine_3316948804(id);
	}

	// Token: 0x0600111B RID: 4379 RVA: 0x00049FB8 File Offset: 0x000481B8
	[ObserversRpc]
	private void ObsSoupRoutine(int id)
	{
		this.RpcWriter___Observers_ObsSoupRoutine_3316948804(id);
	}

	// Token: 0x0600111C RID: 4380 RVA: 0x00049FCF File Offset: 0x000481CF
	public void GiveSoup(int id, GameObject player)
	{
		this.GiveSoupServer(id, player);
	}

	// Token: 0x0600111D RID: 4381 RVA: 0x00049FDC File Offset: 0x000481DC
	[ServerRpc(RequireOwnership = false)]
	private void GiveSoupServer(int id, GameObject player)
	{
		this.RpcWriter___Server_GiveSoupServer_1011425610(id, player);
	}

	// Token: 0x0600111E RID: 4382 RVA: 0x00049FF8 File Offset: 0x000481F8
	[ObserversRpc]
	private void GiveSoupObs(GameObject soup, GameObject player, int id)
	{
		this.RpcWriter___Observers_GiveSoupObs_1289298000(soup, player, id);
	}

	// Token: 0x0600111F RID: 4383 RVA: 0x000021EF File Offset: 0x000003EF
	private void Update()
	{
	}

	// Token: 0x06001120 RID: 4384 RVA: 0x0004A018 File Offset: 0x00048218
	private void LateUpdate()
	{
		if (this.PotentialTargets.Count > 0 && !this.isCookingSoup)
		{
			using (List<Transform>.Enumerator enumerator = this.PotentialTargets.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Transform transform = enumerator.Current;
					if (transform != null)
					{
						Vector3 forward = base.transform.forward;
						Vector3 normalized = (transform.position - base.transform.position).normalized;
						if (Vector3.Angle(forward, normalized) < 95f)
						{
							Vector3 vector = -(this.spinebone.position - transform.position).normalized;
							Vector3 vector2 = Vector3.up;
							Vector3 normalized2 = Vector3.Cross(vector2, vector).normalized;
							vector2 = Vector3.Cross(vector, normalized2);
							this.spinebone.rotation = Quaternion.Lerp(this.prevrot, Quaternion.LookRotation(normalized2, vector2), Time.deltaTime);
							this.prevrot = this.spinebone.rotation;
							this.hasRecentered = false;
							break;
						}
						if (!this.hasRecentered)
						{
							this.spinebone.rotation = Quaternion.Lerp(this.prevrot, this.normalRot, Time.deltaTime * 5f);
							this.prevrot = this.spinebone.rotation;
							if (Mathf.Acos(Mathf.Clamp(Quaternion.Dot(this.prevrot, this.normalRot), -1f, 1f)) * 2f * 57.29578f < 5f)
							{
								this.hasRecentered = true;
							}
						}
					}
				}
				return;
			}
		}
		if (!this.hasRecentered)
		{
			this.spinebone.rotation = Quaternion.Lerp(this.prevrot, this.normalRot, Time.deltaTime * 5f);
			this.prevrot = this.spinebone.rotation;
			if (Mathf.Acos(Mathf.Clamp(Quaternion.Dot(this.prevrot, this.normalRot), -1f, 1f)) * 2f * 57.29578f < 5f)
			{
				this.hasRecentered = true;
			}
		}
	}

	// Token: 0x06001121 RID: 4385 RVA: 0x0004A264 File Offset: 0x00048464
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
				return;
			}
		}
		else
		{
			other.CompareTag("Fireball");
		}
	}

	// Token: 0x06001122 RID: 4386 RVA: 0x0004A2D4 File Offset: 0x000484D4
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

	// Token: 0x06001124 RID: 4388 RVA: 0x0004A34C File Offset: 0x0004854C
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlySoupManControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlySoupManControllerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_Serverplayline_2166136261));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_obsplayline_2166136261));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_ServerCookSoup_3316948804));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ObsMakeSoup_3316948804));
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_ServerSoupRoutine_3316948804));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_ObsSoupRoutine_3316948804));
		base.RegisterServerRpc(6U, new ServerRpcDelegate(this.RpcReader___Server_GiveSoupServer_1011425610));
		base.RegisterObserversRpc(7U, new ClientRpcDelegate(this.RpcReader___Observers_GiveSoupObs_1289298000));
	}

	// Token: 0x06001125 RID: 4389 RVA: 0x0004A422 File Offset: 0x00048622
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateSoupManControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateSoupManControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06001126 RID: 4390 RVA: 0x0004A435 File Offset: 0x00048635
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06001127 RID: 4391 RVA: 0x0004A444 File Offset: 0x00048644
	private void RpcWriter___Server_Serverplayline_2166136261()
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

	// Token: 0x06001128 RID: 4392 RVA: 0x0004A4A9 File Offset: 0x000486A9
	private void RpcLogic___Serverplayline_2166136261()
	{
		this.obsplayline();
	}

	// Token: 0x06001129 RID: 4393 RVA: 0x0004A4B4 File Offset: 0x000486B4
	private void RpcReader___Server_Serverplayline_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___Serverplayline_2166136261();
	}

	// Token: 0x0600112A RID: 4394 RVA: 0x0004A4D4 File Offset: 0x000486D4
	private void RpcWriter___Observers_obsplayline_2166136261()
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

	// Token: 0x0600112B RID: 4395 RVA: 0x0004A548 File Offset: 0x00048748
	private void RpcLogic___obsplayline_2166136261()
	{
		this.soupsoiurce.PlayOneShot(this.acli);
	}

	// Token: 0x0600112C RID: 4396 RVA: 0x0004A55C File Offset: 0x0004875C
	private void RpcReader___Observers_obsplayline_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___obsplayline_2166136261();
	}

	// Token: 0x0600112D RID: 4397 RVA: 0x0004A57C File Offset: 0x0004877C
	private void RpcWriter___Server_ServerCookSoup_3316948804(int id)
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

	// Token: 0x0600112E RID: 4398 RVA: 0x0004A5EE File Offset: 0x000487EE
	private void RpcLogic___ServerCookSoup_3316948804(int id)
	{
		if (!this.isCookingSoup)
		{
			this.isCookingSoup = true;
			this.ObsMakeSoup(id);
		}
	}

	// Token: 0x0600112F RID: 4399 RVA: 0x0004A608 File Offset: 0x00048808
	private void RpcReader___Server_ServerCookSoup_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerCookSoup_3316948804(num);
	}

	// Token: 0x06001130 RID: 4400 RVA: 0x0004A63C File Offset: 0x0004883C
	private void RpcWriter___Observers_ObsMakeSoup_3316948804(int id)
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

	// Token: 0x06001131 RID: 4401 RVA: 0x0004A6BD File Offset: 0x000488BD
	private void RpcLogic___ObsMakeSoup_3316948804(int id)
	{
		if (base.HasAuthority)
		{
			base.StartCoroutine(this.SoupRoutine(id));
		}
		this.SoupmanAni.SetBool("cook", true);
		this.isCookingSoup = true;
		base.StartCoroutine(this.waittoplaysound());
	}

	// Token: 0x06001132 RID: 4402 RVA: 0x0004A6FC File Offset: 0x000488FC
	private void RpcReader___Observers_ObsMakeSoup_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsMakeSoup_3316948804(num);
	}

	// Token: 0x06001133 RID: 4403 RVA: 0x0004A730 File Offset: 0x00048930
	private void RpcWriter___Server_ServerSoupRoutine_3316948804(int id)
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
		base.SendServerRpc(4U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06001134 RID: 4404 RVA: 0x0004A7A2 File Offset: 0x000489A2
	private void RpcLogic___ServerSoupRoutine_3316948804(int id)
	{
		this.ObsSoupRoutine(id);
	}

	// Token: 0x06001135 RID: 4405 RVA: 0x0004A7AC File Offset: 0x000489AC
	private void RpcReader___Server_ServerSoupRoutine_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSoupRoutine_3316948804(num);
	}

	// Token: 0x06001136 RID: 4406 RVA: 0x0004A7E0 File Offset: 0x000489E0
	private void RpcWriter___Observers_ObsSoupRoutine_3316948804(int id)
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
		base.SendObserversRpc(5U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06001137 RID: 4407 RVA: 0x0004A864 File Offset: 0x00048A64
	private void RpcLogic___ObsSoupRoutine_3316948804(int id)
	{
		this.SoupItems[id].SetActive(true);
		Material[] materials = this.bowl.materials;
		materials[1] = this.matas[id];
		this.bowl.materials = materials;
		this.gsfp.SoupisReady(id);
	}

	// Token: 0x06001138 RID: 4408 RVA: 0x0004A8B0 File Offset: 0x00048AB0
	private void RpcReader___Observers_ObsSoupRoutine_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsSoupRoutine_3316948804(num);
	}

	// Token: 0x06001139 RID: 4409 RVA: 0x0004A8E4 File Offset: 0x00048AE4
	private void RpcWriter___Server_GiveSoupServer_1011425610(int id, GameObject player)
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
		pooledWriter.WriteGameObject(player);
		base.SendServerRpc(6U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600113A RID: 4410 RVA: 0x0004A964 File Offset: 0x00048B64
	private void RpcLogic___GiveSoupServer_1011425610(int id, GameObject player)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.SoupPrefabs[id]);
		gameObject.transform.position = new Vector3(0f, 0f, 0f);
		base.ServerManager.Spawn(gameObject, null, default(Scene));
		this.GiveSoupObs(gameObject, player, id);
	}

	// Token: 0x0600113B RID: 4411 RVA: 0x0004A9C0 File Offset: 0x00048BC0
	private void RpcReader___Server_GiveSoupServer_1011425610(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___GiveSoupServer_1011425610(num, gameObject);
	}

	// Token: 0x0600113C RID: 4412 RVA: 0x0004AA04 File Offset: 0x00048C04
	private void RpcWriter___Observers_GiveSoupObs_1289298000(GameObject soup, GameObject player, int id)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(soup);
		pooledWriter.WriteGameObject(player);
		pooledWriter.WriteInt32(id);
		base.SendObserversRpc(7U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600113D RID: 4413 RVA: 0x0004AAA0 File Offset: 0x00048CA0
	private void RpcLogic___GiveSoupObs_1289298000(GameObject soup, GameObject player, int id)
	{
		PlayerInventory playerInventory;
		if (player.TryGetComponent<PlayerInventory>(out playerInventory) && playerInventory.IsOwner)
		{
			playerInventory.Pickup(soup);
		}
		this.isCookingSoup = false;
		this.gsfp.SoupPickedup();
		this.SoupItems[id].SetActive(false);
		this.SoupmanAni.SetBool("cook", false);
	}

	// Token: 0x0600113E RID: 4414 RVA: 0x0004AAF8 File Offset: 0x00048CF8
	private void RpcReader___Observers_GiveSoupObs_1289298000(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		GameObject gameObject2 = PooledReader0.ReadGameObject();
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___GiveSoupObs_1289298000(gameObject, gameObject2, num);
	}

	// Token: 0x0600113F RID: 4415 RVA: 0x0004A435 File Offset: 0x00048635
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x040009F5 RID: 2549
	public List<Transform> PotentialTargets = new List<Transform>();

	// Token: 0x040009F6 RID: 2550
	public Transform spinebone;

	// Token: 0x040009F7 RID: 2551
	public Animator SoupmanAni;

	// Token: 0x040009F8 RID: 2552
	public bool isCookingSoup;

	// Token: 0x040009F9 RID: 2553
	public GameObject[] SoupItems;

	// Token: 0x040009FA RID: 2554
	public GameObject[] SoupPrefabs;

	// Token: 0x040009FB RID: 2555
	public Material[] matas;

	// Token: 0x040009FC RID: 2556
	public MeshRenderer bowl;

	// Token: 0x040009FD RID: 2557
	public GetSoupFromGuy gsfp;

	// Token: 0x040009FE RID: 2558
	private Quaternion prevrot;

	// Token: 0x040009FF RID: 2559
	private Quaternion normalRot;

	// Token: 0x04000A00 RID: 2560
	private bool hasRecentered;

	// Token: 0x04000A01 RID: 2561
	public AudioSource soupsoiurce;

	// Token: 0x04000A02 RID: 2562
	public AudioClip acli;

	// Token: 0x04000A03 RID: 2563
	public AudioClip makeosup;

	// Token: 0x04000A04 RID: 2564
	private bool NetworkInitialize___EarlySoupManControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x04000A05 RID: 2565
	private bool NetworkInitialize__LateSoupManControllerAssembly-CSharp.dll_Excuted;
}
