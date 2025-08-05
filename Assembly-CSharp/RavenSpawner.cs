using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x02000150 RID: 336
public class RavenSpawner : NetworkBehaviour
{
	// Token: 0x06000E73 RID: 3699 RVA: 0x0003ABA0 File Offset: 0x00038DA0
	public override void OnStartClient()
	{
		base.OnStartClient();
		this.Ravens = new GameObject[3];
		if (base.HasAuthority)
		{
			this.canspawnravens = true;
			base.StartCoroutine(this.RavenRoutine());
			base.StartCoroutine(this.GetSun());
		}
		this.ravenSpawnPoint = GameObject.FindGameObjectWithTag("RavenPoint").transform;
	}

	// Token: 0x06000E74 RID: 3700 RVA: 0x0003ABFD File Offset: 0x00038DFD
	private IEnumerator GetSun()
	{
		GameObject Sunpobj = null;
		while (Sunpobj == null)
		{
			Sunpobj = GameObject.FindGameObjectWithTag("Weather");
			yield return null;
		}
		this.nightday = Sunpobj.GetComponent<WeatherCycle>();
		yield break;
	}

	// Token: 0x06000E75 RID: 3701 RVA: 0x0003AC0C File Offset: 0x00038E0C
	private IEnumerator RavenRoutine()
	{
		while (base.isActiveAndEnabled && this.canspawnravens)
		{
			if (base.NetworkManager.IsClientStarted)
			{
				yield return new WaitForSeconds((float)Random.Range(30, 60));
				if (this.canspawnravens && base.NetworkManager.IsClientStarted)
				{
					this.ServerSpawnRaven();
				}
			}
		}
		yield break;
	}

	// Token: 0x06000E76 RID: 3702 RVA: 0x0003AC1C File Offset: 0x00038E1C
	[ServerRpc(RequireOwnership = false)]
	private void ServerSpawnRaven()
	{
		this.RpcWriter___Server_ServerSpawnRaven_2166136261();
	}

	// Token: 0x06000E77 RID: 3703 RVA: 0x0003AC30 File Offset: 0x00038E30
	[ObserversRpc]
	private void ObserverSpawnRaven(Vector3 LandPoint)
	{
		this.RpcWriter___Observers_ObserverSpawnRaven_4276783012(LandPoint);
	}

	// Token: 0x06000E78 RID: 3704 RVA: 0x0003AC48 File Offset: 0x00038E48
	private void SpawnTheRavenIDKWHYTHIS(int i, Vector3 LandPoint)
	{
		this.Ravens[i] = Object.Instantiate<GameObject>(this.RavenPrefab, this.ravenSpawnPoint.position, Quaternion.identity);
		this.Ravens[i].GetComponent<RavenController>().LandPoint = LandPoint;
		this.Ravens[i].GetComponent<RavenController>().RavenID = i;
		this.Ravens[i].GetComponent<RavenController>().rspawn = base.transform.GetComponent<RavenSpawner>();
	}

	// Token: 0x06000E79 RID: 3705 RVA: 0x0003ACBB File Offset: 0x00038EBB
	public void TriggerFlyAway(int ravenID)
	{
		this.ServerTriggerFlyAway(ravenID);
	}

	// Token: 0x06000E7A RID: 3706 RVA: 0x0003ACC4 File Offset: 0x00038EC4
	[ServerRpc(RequireOwnership = false)]
	private void ServerTriggerFlyAway(int RId)
	{
		this.RpcWriter___Server_ServerTriggerFlyAway_3316948804(RId);
	}

	// Token: 0x06000E7B RID: 3707 RVA: 0x0003ACD0 File Offset: 0x00038ED0
	[ObserversRpc]
	private void ObsTriggerFlyAway(int ravid)
	{
		this.RpcWriter___Observers_ObsTriggerFlyAway_3316948804(ravid);
	}

	// Token: 0x06000E7C RID: 3708 RVA: 0x0003ACDC File Offset: 0x00038EDC
	public void TriggerSound(int ravenID)
	{
		if (this.Ravens[ravenID] && !this.nightday.isNight && this.canspawnravens && base.isActiveAndEnabled && base.NetworkManager.IsClientStarted)
		{
			this.ServerTriggerSound(ravenID);
		}
	}

	// Token: 0x06000E7D RID: 3709 RVA: 0x0003AD29 File Offset: 0x00038F29
	[ServerRpc(RequireOwnership = false)]
	private void ServerTriggerSound(int RId)
	{
		this.RpcWriter___Server_ServerTriggerSound_3316948804(RId);
	}

	// Token: 0x06000E7E RID: 3710 RVA: 0x0003AD35 File Offset: 0x00038F35
	[ObserversRpc]
	private void ObsTriggerSound(int ravid)
	{
		this.RpcWriter___Observers_ObsTriggerSound_3316948804(ravid);
	}

	// Token: 0x06000E7F RID: 3711 RVA: 0x0003AD41 File Offset: 0x00038F41
	public bool checkAuthority()
	{
		return base.HasAuthority;
	}

	// Token: 0x06000E80 RID: 3712 RVA: 0x0003AD4C File Offset: 0x00038F4C
	public void DestroyAllRavens()
	{
		this.canspawnravens = false;
		for (int i = 0; i < this.Ravens.Length; i++)
		{
			if (this.Ravens[i] != null)
			{
				Object.Destroy(this.Ravens[i]);
			}
		}
	}

	// Token: 0x06000E81 RID: 3713 RVA: 0x0003AD90 File Offset: 0x00038F90
	[ServerRpc(RequireOwnership = false)]
	private void Destrotyravens()
	{
		this.RpcWriter___Server_Destrotyravens_2166136261();
	}

	// Token: 0x06000E82 RID: 3714 RVA: 0x0003AD98 File Offset: 0x00038F98
	[ObserversRpc]
	private void DestoyrabensAgain()
	{
		this.RpcWriter___Observers_DestoyrabensAgain_2166136261();
	}

	// Token: 0x06000E84 RID: 3716 RVA: 0x0003ADAC File Offset: 0x00038FAC
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyRavenSpawnerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyRavenSpawnerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ServerSpawnRaven_2166136261));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_ObserverSpawnRaven_4276783012));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_ServerTriggerFlyAway_3316948804));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ObsTriggerFlyAway_3316948804));
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_ServerTriggerSound_3316948804));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_ObsTriggerSound_3316948804));
		base.RegisterServerRpc(6U, new ServerRpcDelegate(this.RpcReader___Server_Destrotyravens_2166136261));
		base.RegisterObserversRpc(7U, new ClientRpcDelegate(this.RpcReader___Observers_DestoyrabensAgain_2166136261));
	}

	// Token: 0x06000E85 RID: 3717 RVA: 0x0003AE82 File Offset: 0x00039082
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateRavenSpawnerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateRavenSpawnerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000E86 RID: 3718 RVA: 0x0003AE95 File Offset: 0x00039095
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000E87 RID: 3719 RVA: 0x0003AEA4 File Offset: 0x000390A4
	private void RpcWriter___Server_ServerSpawnRaven_2166136261()
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

	// Token: 0x06000E88 RID: 3720 RVA: 0x0003AF0C File Offset: 0x0003910C
	private void RpcLogic___ServerSpawnRaven_2166136261()
	{
		Vector3 vector = new Vector3(base.transform.position.x, base.transform.position.y + 50f, base.transform.position.z) + base.transform.forward * (float)Random.Range(-40, 40) + base.transform.right * (float)Random.Range(-40, 40);
		Vector3 down = Vector3.down;
		RaycastHit raycastHit;
		if (this.nightday != null && Physics.Raycast(vector, down, out raycastHit, 70f, this.groundlayer) && !this.nightday.isNight)
		{
			this.ObserverSpawnRaven(raycastHit.point);
		}
	}

	// Token: 0x06000E89 RID: 3721 RVA: 0x0003AFDC File Offset: 0x000391DC
	private void RpcReader___Server_ServerSpawnRaven_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSpawnRaven_2166136261();
	}

	// Token: 0x06000E8A RID: 3722 RVA: 0x0003AFFC File Offset: 0x000391FC
	private void RpcWriter___Observers_ObserverSpawnRaven_4276783012(Vector3 LandPoint)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(LandPoint);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000E8B RID: 3723 RVA: 0x0003B080 File Offset: 0x00039280
	private void RpcLogic___ObserverSpawnRaven_4276783012(Vector3 LandPoint)
	{
		for (int i = 0; i < 3; i++)
		{
			if (this.Ravens[i] == null)
			{
				this.SpawnTheRavenIDKWHYTHIS(i, LandPoint);
				return;
			}
		}
	}

	// Token: 0x06000E8C RID: 3724 RVA: 0x0003B0B4 File Offset: 0x000392B4
	private void RpcReader___Observers_ObserverSpawnRaven_4276783012(PooledReader PooledReader0, Channel channel)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserverSpawnRaven_4276783012(vector);
	}

	// Token: 0x06000E8D RID: 3725 RVA: 0x0003B0E8 File Offset: 0x000392E8
	private void RpcWriter___Server_ServerTriggerFlyAway_3316948804(int RId)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(RId);
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000E8E RID: 3726 RVA: 0x0003B15A File Offset: 0x0003935A
	private void RpcLogic___ServerTriggerFlyAway_3316948804(int RId)
	{
		this.ObsTriggerFlyAway(RId);
	}

	// Token: 0x06000E8F RID: 3727 RVA: 0x0003B164 File Offset: 0x00039364
	private void RpcReader___Server_ServerTriggerFlyAway_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerTriggerFlyAway_3316948804(num);
	}

	// Token: 0x06000E90 RID: 3728 RVA: 0x0003B198 File Offset: 0x00039398
	private void RpcWriter___Observers_ObsTriggerFlyAway_3316948804(int ravid)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(ravid);
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000E91 RID: 3729 RVA: 0x0003B219 File Offset: 0x00039419
	private void RpcLogic___ObsTriggerFlyAway_3316948804(int ravid)
	{
		if (this.Ravens[ravid] != null)
		{
			this.Ravens[ravid].GetComponent<RavenController>().StartFlyAwayr();
			this.Ravens[ravid] = null;
		}
	}

	// Token: 0x06000E92 RID: 3730 RVA: 0x0003B248 File Offset: 0x00039448
	private void RpcReader___Observers_ObsTriggerFlyAway_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsTriggerFlyAway_3316948804(num);
	}

	// Token: 0x06000E93 RID: 3731 RVA: 0x0003B27C File Offset: 0x0003947C
	private void RpcWriter___Server_ServerTriggerSound_3316948804(int RId)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(RId);
		base.SendServerRpc(4U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000E94 RID: 3732 RVA: 0x0003B2EE File Offset: 0x000394EE
	private void RpcLogic___ServerTriggerSound_3316948804(int RId)
	{
		this.ObsTriggerSound(RId);
	}

	// Token: 0x06000E95 RID: 3733 RVA: 0x0003B2F8 File Offset: 0x000394F8
	private void RpcReader___Server_ServerTriggerSound_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerTriggerSound_3316948804(num);
	}

	// Token: 0x06000E96 RID: 3734 RVA: 0x0003B32C File Offset: 0x0003952C
	private void RpcWriter___Observers_ObsTriggerSound_3316948804(int ravid)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(ravid);
		base.SendObserversRpc(5U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000E97 RID: 3735 RVA: 0x0003B3AD File Offset: 0x000395AD
	private void RpcLogic___ObsTriggerSound_3316948804(int ravid)
	{
		if (this.Ravens[ravid] != null)
		{
			this.Ravens[ravid].GetComponent<RavenController>().PlaySound();
		}
	}

	// Token: 0x06000E98 RID: 3736 RVA: 0x0003B3D4 File Offset: 0x000395D4
	private void RpcReader___Observers_ObsTriggerSound_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsTriggerSound_3316948804(num);
	}

	// Token: 0x06000E99 RID: 3737 RVA: 0x0003B408 File Offset: 0x00039608
	private void RpcWriter___Server_Destrotyravens_2166136261()
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

	// Token: 0x06000E9A RID: 3738 RVA: 0x0003B46D File Offset: 0x0003966D
	private void RpcLogic___Destrotyravens_2166136261()
	{
		this.DestoyrabensAgain();
	}

	// Token: 0x06000E9B RID: 3739 RVA: 0x0003B478 File Offset: 0x00039678
	private void RpcReader___Server_Destrotyravens_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___Destrotyravens_2166136261();
	}

	// Token: 0x06000E9C RID: 3740 RVA: 0x0003B498 File Offset: 0x00039698
	private void RpcWriter___Observers_DestoyrabensAgain_2166136261()
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

	// Token: 0x06000E9D RID: 3741 RVA: 0x0003B50C File Offset: 0x0003970C
	private void RpcLogic___DestoyrabensAgain_2166136261()
	{
		for (int i = 0; i < this.Ravens.Length; i++)
		{
			if (this.Ravens[i] != null)
			{
				Object.Destroy(this.Ravens[i]);
			}
		}
	}

	// Token: 0x06000E9E RID: 3742 RVA: 0x0003B54C File Offset: 0x0003974C
	private void RpcReader___Observers_DestoyrabensAgain_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___DestoyrabensAgain_2166136261();
	}

	// Token: 0x06000E9F RID: 3743 RVA: 0x0003AE95 File Offset: 0x00039095
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x040007F1 RID: 2033
	public GameObject[] Ravens;

	// Token: 0x040007F2 RID: 2034
	public GameObject RavenPrefab;

	// Token: 0x040007F3 RID: 2035
	public LayerMask groundlayer;

	// Token: 0x040007F4 RID: 2036
	public Transform ravenSpawnPoint;

	// Token: 0x040007F5 RID: 2037
	public WeatherCycle nightday;

	// Token: 0x040007F6 RID: 2038
	private bool canspawnravens;

	// Token: 0x040007F7 RID: 2039
	private bool NetworkInitialize___EarlyRavenSpawnerAssembly-CSharp.dll_Excuted;

	// Token: 0x040007F8 RID: 2040
	private bool NetworkInitialize__LateRavenSpawnerAssembly-CSharp.dll_Excuted;
}
