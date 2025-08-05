using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x02000009 RID: 9
public class BogFrogController : NetworkBehaviour
{
	// Token: 0x0600003A RID: 58 RVA: 0x00002DE2 File Offset: 0x00000FE2
	public override void OnStartClient()
	{
		base.OnStartClient();
		if (!base.HasAuthority)
		{
			this.ServerSyncFrogIds();
			base.gameObject.GetComponent<BogFrogController>().enabled = false;
			return;
		}
		base.StartCoroutine(this.FrogBehavior());
	}

	// Token: 0x0600003B RID: 59 RVA: 0x00002E17 File Offset: 0x00001017
	[ServerRpc(RequireOwnership = false)]
	private void ServerSyncFrogIds()
	{
		this.RpcWriter___Server_ServerSyncFrogIds_2166136261();
	}

	// Token: 0x0600003C RID: 60 RVA: 0x00002E1F File Offset: 0x0000101F
	[ObserversRpc]
	private void ObsSyncFrogIds()
	{
		this.RpcWriter___Observers_ObsSyncFrogIds_2166136261();
	}

	// Token: 0x0600003D RID: 61 RVA: 0x00002E27 File Offset: 0x00001027
	[ServerRpc(RequireOwnership = false)]
	private void ActualSync(int bfid1, int bfid2)
	{
		this.RpcWriter___Server_ActualSync_1692629761(bfid1, bfid2);
	}

	// Token: 0x0600003E RID: 62 RVA: 0x00002E37 File Offset: 0x00001037
	[ObserversRpc]
	private void ActualSyncObs(int bfid1, int bfid2)
	{
		this.RpcWriter___Observers_ActualSyncObs_1692629761(bfid1, bfid2);
	}

	// Token: 0x0600003F RID: 63 RVA: 0x00002E47 File Offset: 0x00001047
	private IEnumerator FrogBehavior()
	{
		for (int j = 0; j < this.BogFrogs.Length; j++)
		{
			this.BogFrogs[j].FrogPoint = j;
			this.ServerSetBogFrogID(j);
		}
		for (;;)
		{
			int num;
			for (int i = 0; i < this.BogFrogs.Length; i = num + 1)
			{
				if (!this.BogFrogs[i].CheckPlayersNear())
				{
					this.ServerNewlocationanis(i);
					yield return new WaitForSeconds(2f);
					this.ServerNewPosition(i, this.FrogPoints[this.GetNewUnoccupiedPoint(i)].position);
				}
				num = i;
			}
			yield return new WaitForSeconds(20f);
		}
		yield break;
	}

	// Token: 0x06000040 RID: 64 RVA: 0x00002E56 File Offset: 0x00001056
	[ServerRpc(RequireOwnership = false)]
	private void ServerSetBogFrogID(int id)
	{
		this.RpcWriter___Server_ServerSetBogFrogID_3316948804(id);
	}

	// Token: 0x06000041 RID: 65 RVA: 0x00002E62 File Offset: 0x00001062
	[ObserversRpc]
	private void ObserversSetBogFrogID(int id)
	{
		this.RpcWriter___Observers_ObserversSetBogFrogID_3316948804(id);
	}

	// Token: 0x06000042 RID: 66 RVA: 0x00002E6E File Offset: 0x0000106E
	[ServerRpc(RequireOwnership = false)]
	private void ServerNewlocationanis(int id)
	{
		this.RpcWriter___Server_ServerNewlocationanis_3316948804(id);
	}

	// Token: 0x06000043 RID: 67 RVA: 0x00002E7A File Offset: 0x0000107A
	[ObserversRpc]
	private void ObserversNewLocoAnis(int id)
	{
		this.RpcWriter___Observers_ObserversNewLocoAnis_3316948804(id);
	}

	// Token: 0x06000044 RID: 68 RVA: 0x00002E86 File Offset: 0x00001086
	[ServerRpc(RequireOwnership = false)]
	private void ServerNewPosition(int id, Vector3 newPos)
	{
		this.RpcWriter___Server_ServerNewPosition_215135683(id, newPos);
	}

	// Token: 0x06000045 RID: 69 RVA: 0x00002E96 File Offset: 0x00001096
	[ObserversRpc]
	private void ObserversNewPosition(int id, Vector3 newPos)
	{
		this.RpcWriter___Observers_ObserversNewPosition_215135683(id, newPos);
	}

	// Token: 0x06000046 RID: 70 RVA: 0x00002EA8 File Offset: 0x000010A8
	private int GetNewUnoccupiedPoint(int id)
	{
		this.pointFound = false;
		int num = Random.Range(0, this.FrogPoints.Length);
		while (!this.pointFound)
		{
			this.pointFound = true;
			foreach (BogFrogSingleController bogFrogSingleController in this.BogFrogs)
			{
				if (num == bogFrogSingleController.FrogPoint)
				{
					this.pointFound = false;
					num = Random.Range(0, this.FrogPoints.Length);
				}
			}
		}
		this.BogFrogs[id].FrogPoint = num;
		return num;
	}

	// Token: 0x06000047 RID: 71 RVA: 0x00002F24 File Offset: 0x00001124
	public void EatTheGuy(int bogfrogid, Transform Guy)
	{
		if (!this.BogFrogs[bogfrogid].imEatingtheGuy && this.BogFrogs[bogfrogid].eatcdtimer > 6f)
		{
			this.BogFrogs[bogfrogid].GuyToEat = Guy;
			this.BogFrogs[bogfrogid].imEatingtheGuy = true;
			this.ServerBogFrogEat(bogfrogid, Guy);
		}
	}

	// Token: 0x06000048 RID: 72 RVA: 0x00002F78 File Offset: 0x00001178
	[ServerRpc(RequireOwnership = false)]
	private void ServerBogFrogEat(int bogfrogid, Transform Guy)
	{
		this.RpcWriter___Server_ServerBogFrogEat_676113415(bogfrogid, Guy);
	}

	// Token: 0x06000049 RID: 73 RVA: 0x00002F88 File Offset: 0x00001188
	[ObserversRpc]
	private void ObserversBogFrogEat(int bogfrogid, Transform Guy)
	{
		this.RpcWriter___Observers_ObserversBogFrogEat_676113415(bogfrogid, Guy);
	}

	// Token: 0x0600004A RID: 74 RVA: 0x00002F98 File Offset: 0x00001198
	public void forcegonewspot(int bogfrogid)
	{
		if (base.HasAuthority)
		{
			this.ServerNewPosition(bogfrogid, this.FrogPoints[this.GetNewUnoccupiedPoint(bogfrogid)].position);
		}
	}

	// Token: 0x0600004B RID: 75 RVA: 0x00002FBC File Offset: 0x000011BC
	public void hitbyspell(int bfid)
	{
		base.StartCoroutine(this.hitbyspellroutine(bfid));
	}

	// Token: 0x0600004C RID: 76 RVA: 0x00002FCC File Offset: 0x000011CC
	private IEnumerator hitbyspellroutine(int i)
	{
		this.ServerNewlocationanis(i);
		yield return new WaitForSeconds(2f);
		this.ServerNewPosition(i, this.FrogPoints[this.GetNewUnoccupiedPoint(i)].position);
		yield break;
	}

	// Token: 0x0600004E RID: 78 RVA: 0x00002FE4 File Offset: 0x000011E4
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyBogFrogControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyBogFrogControllerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ServerSyncFrogIds_2166136261));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_ObsSyncFrogIds_2166136261));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_ActualSync_1692629761));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ActualSyncObs_1692629761));
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_ServerSetBogFrogID_3316948804));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversSetBogFrogID_3316948804));
		base.RegisterServerRpc(6U, new ServerRpcDelegate(this.RpcReader___Server_ServerNewlocationanis_3316948804));
		base.RegisterObserversRpc(7U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversNewLocoAnis_3316948804));
		base.RegisterServerRpc(8U, new ServerRpcDelegate(this.RpcReader___Server_ServerNewPosition_215135683));
		base.RegisterObserversRpc(9U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversNewPosition_215135683));
		base.RegisterServerRpc(10U, new ServerRpcDelegate(this.RpcReader___Server_ServerBogFrogEat_676113415));
		base.RegisterObserversRpc(11U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversBogFrogEat_676113415));
	}

	// Token: 0x0600004F RID: 79 RVA: 0x00003116 File Offset: 0x00001316
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateBogFrogControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateBogFrogControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000050 RID: 80 RVA: 0x00003129 File Offset: 0x00001329
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000051 RID: 81 RVA: 0x00003138 File Offset: 0x00001338
	private void RpcWriter___Server_ServerSyncFrogIds_2166136261()
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

	// Token: 0x06000052 RID: 82 RVA: 0x0000319D File Offset: 0x0000139D
	private void RpcLogic___ServerSyncFrogIds_2166136261()
	{
		this.ObsSyncFrogIds();
	}

	// Token: 0x06000053 RID: 83 RVA: 0x000031A8 File Offset: 0x000013A8
	private void RpcReader___Server_ServerSyncFrogIds_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSyncFrogIds_2166136261();
	}

	// Token: 0x06000054 RID: 84 RVA: 0x000031C8 File Offset: 0x000013C8
	private void RpcWriter___Observers_ObsSyncFrogIds_2166136261()
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

	// Token: 0x06000055 RID: 85 RVA: 0x0000323C File Offset: 0x0000143C
	private void RpcLogic___ObsSyncFrogIds_2166136261()
	{
		if (base.HasAuthority)
		{
			this.ActualSync(this.BogFrogs[0].getbfid(), this.BogFrogs[1].getbfid());
		}
	}

	// Token: 0x06000056 RID: 86 RVA: 0x00003268 File Offset: 0x00001468
	private void RpcReader___Observers_ObsSyncFrogIds_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsSyncFrogIds_2166136261();
	}

	// Token: 0x06000057 RID: 87 RVA: 0x00003288 File Offset: 0x00001488
	private void RpcWriter___Server_ActualSync_1692629761(int bfid1, int bfid2)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(bfid1);
		pooledWriter.WriteInt32(bfid2);
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000058 RID: 88 RVA: 0x00003307 File Offset: 0x00001507
	private void RpcLogic___ActualSync_1692629761(int bfid1, int bfid2)
	{
		this.ActualSyncObs(bfid1, bfid2);
	}

	// Token: 0x06000059 RID: 89 RVA: 0x00003314 File Offset: 0x00001514
	private void RpcReader___Server_ActualSync_1692629761(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		int num2 = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ActualSync_1692629761(num, num2);
	}

	// Token: 0x0600005A RID: 90 RVA: 0x00003358 File Offset: 0x00001558
	private void RpcWriter___Observers_ActualSyncObs_1692629761(int bfid1, int bfid2)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(bfid1);
		pooledWriter.WriteInt32(bfid2);
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600005B RID: 91 RVA: 0x000033E6 File Offset: 0x000015E6
	private void RpcLogic___ActualSyncObs_1692629761(int bfid1, int bfid2)
	{
		this.BogFrogs[bfid1].SetBogFrogID(bfid1);
		this.BogFrogs[bfid2].SetBogFrogID(bfid2);
	}

	// Token: 0x0600005C RID: 92 RVA: 0x00003404 File Offset: 0x00001604
	private void RpcReader___Observers_ActualSyncObs_1692629761(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		int num2 = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ActualSyncObs_1692629761(num, num2);
	}

	// Token: 0x0600005D RID: 93 RVA: 0x00003448 File Offset: 0x00001648
	private void RpcWriter___Server_ServerSetBogFrogID_3316948804(int id)
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

	// Token: 0x0600005E RID: 94 RVA: 0x000034BA File Offset: 0x000016BA
	private void RpcLogic___ServerSetBogFrogID_3316948804(int id)
	{
		this.ObserversSetBogFrogID(id);
	}

	// Token: 0x0600005F RID: 95 RVA: 0x000034C4 File Offset: 0x000016C4
	private void RpcReader___Server_ServerSetBogFrogID_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSetBogFrogID_3316948804(num);
	}

	// Token: 0x06000060 RID: 96 RVA: 0x000034F8 File Offset: 0x000016F8
	private void RpcWriter___Observers_ObserversSetBogFrogID_3316948804(int id)
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

	// Token: 0x06000061 RID: 97 RVA: 0x00003579 File Offset: 0x00001779
	private void RpcLogic___ObserversSetBogFrogID_3316948804(int id)
	{
		this.BogFrogs[id].SetBogFrogID(id);
	}

	// Token: 0x06000062 RID: 98 RVA: 0x0000358C File Offset: 0x0000178C
	private void RpcReader___Observers_ObserversSetBogFrogID_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversSetBogFrogID_3316948804(num);
	}

	// Token: 0x06000063 RID: 99 RVA: 0x000035C0 File Offset: 0x000017C0
	private void RpcWriter___Server_ServerNewlocationanis_3316948804(int id)
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
		base.SendServerRpc(6U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000064 RID: 100 RVA: 0x00003632 File Offset: 0x00001832
	private void RpcLogic___ServerNewlocationanis_3316948804(int id)
	{
		this.ObserversNewLocoAnis(id);
	}

	// Token: 0x06000065 RID: 101 RVA: 0x0000363C File Offset: 0x0000183C
	private void RpcReader___Server_ServerNewlocationanis_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerNewlocationanis_3316948804(num);
	}

	// Token: 0x06000066 RID: 102 RVA: 0x00003670 File Offset: 0x00001870
	private void RpcWriter___Observers_ObserversNewLocoAnis_3316948804(int id)
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
		base.SendObserversRpc(7U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000067 RID: 103 RVA: 0x000036F1 File Offset: 0x000018F1
	private void RpcLogic___ObserversNewLocoAnis_3316948804(int id)
	{
		this.BogFrogs[id].NewLocationAnis();
	}

	// Token: 0x06000068 RID: 104 RVA: 0x00003700 File Offset: 0x00001900
	private void RpcReader___Observers_ObserversNewLocoAnis_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversNewLocoAnis_3316948804(num);
	}

	// Token: 0x06000069 RID: 105 RVA: 0x00003734 File Offset: 0x00001934
	private void RpcWriter___Server_ServerNewPosition_215135683(int id, Vector3 newPos)
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
		pooledWriter.WriteVector3(newPos);
		base.SendServerRpc(8U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600006A RID: 106 RVA: 0x000037B3 File Offset: 0x000019B3
	private void RpcLogic___ServerNewPosition_215135683(int id, Vector3 newPos)
	{
		this.ObserversNewPosition(id, newPos);
	}

	// Token: 0x0600006B RID: 107 RVA: 0x000037C0 File Offset: 0x000019C0
	private void RpcReader___Server_ServerNewPosition_215135683(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerNewPosition_215135683(num, vector);
	}

	// Token: 0x0600006C RID: 108 RVA: 0x00003804 File Offset: 0x00001A04
	private void RpcWriter___Observers_ObserversNewPosition_215135683(int id, Vector3 newPos)
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
		pooledWriter.WriteVector3(newPos);
		base.SendObserversRpc(9U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600006D RID: 109 RVA: 0x00003892 File Offset: 0x00001A92
	private void RpcLogic___ObserversNewPosition_215135683(int id, Vector3 newPos)
	{
		this.BogFrogs[id].transform.position = newPos;
	}

	// Token: 0x0600006E RID: 110 RVA: 0x000038A8 File Offset: 0x00001AA8
	private void RpcReader___Observers_ObserversNewPosition_215135683(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversNewPosition_215135683(num, vector);
	}

	// Token: 0x0600006F RID: 111 RVA: 0x000038EC File Offset: 0x00001AEC
	private void RpcWriter___Server_ServerBogFrogEat_676113415(int bogfrogid, Transform Guy)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(bogfrogid);
		pooledWriter.WriteTransform(Guy);
		base.SendServerRpc(10U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000070 RID: 112 RVA: 0x0000396B File Offset: 0x00001B6B
	private void RpcLogic___ServerBogFrogEat_676113415(int bogfrogid, Transform Guy)
	{
		this.ObserversBogFrogEat(bogfrogid, Guy);
	}

	// Token: 0x06000071 RID: 113 RVA: 0x00003978 File Offset: 0x00001B78
	private void RpcReader___Server_ServerBogFrogEat_676113415(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		Transform transform = PooledReader0.ReadTransform();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerBogFrogEat_676113415(num, transform);
	}

	// Token: 0x06000072 RID: 114 RVA: 0x000039BC File Offset: 0x00001BBC
	private void RpcWriter___Observers_ObserversBogFrogEat_676113415(int bogfrogid, Transform Guy)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(bogfrogid);
		pooledWriter.WriteTransform(Guy);
		base.SendObserversRpc(11U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000073 RID: 115 RVA: 0x00003A4A File Offset: 0x00001C4A
	private void RpcLogic___ObserversBogFrogEat_676113415(int bogfrogid, Transform Guy)
	{
		this.BogFrogs[bogfrogid].GuyToEat = Guy;
		this.BogFrogs[bogfrogid].imEatingtheGuy = true;
		this.BogFrogs[bogfrogid].EatGuyAnis();
	}

	// Token: 0x06000074 RID: 116 RVA: 0x00003A78 File Offset: 0x00001C78
	private void RpcReader___Observers_ObserversBogFrogEat_676113415(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		Transform transform = PooledReader0.ReadTransform();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversBogFrogEat_676113415(num, transform);
	}

	// Token: 0x06000075 RID: 117 RVA: 0x00003129 File Offset: 0x00001329
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0400001D RID: 29
	public Transform[] FrogPoints;

	// Token: 0x0400001E RID: 30
	public BogFrogSingleController[] BogFrogs;

	// Token: 0x0400001F RID: 31
	private bool pointFound;

	// Token: 0x04000020 RID: 32
	private bool NetworkInitialize___EarlyBogFrogControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x04000021 RID: 33
	private bool NetworkInitialize__LateBogFrogControllerAssembly-CSharp.dll_Excuted;
}
