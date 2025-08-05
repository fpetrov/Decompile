using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200005E RID: 94
public class DuendeManager : NetworkBehaviour
{
	// Token: 0x060003BF RID: 959 RVA: 0x0000F6F4 File Offset: 0x0000D8F4
	public override void OnStartClient()
	{
		base.OnStartClient();
		if (base.HasAuthority)
		{
			base.StartCoroutine(this.WaitforGameStart());
		}
		this.isDuendeDead = new bool[this.Duende.Length];
		for (int i = 0; i < this.isDuendeDead.Length; i++)
		{
			this.isDuendeDead[i] = false;
		}
		base.StartCoroutine(this.WaitforNetItemManager());
	}

	// Token: 0x060003C0 RID: 960 RVA: 0x0000F758 File Offset: 0x0000D958
	private IEnumerator WaitforNetItemManager()
	{
		GameObject NIM = null;
		while (NIM == null)
		{
			NIM = GameObject.FindGameObjectWithTag("NetItemManager");
			yield return null;
		}
		this.plt = NIM.GetComponent<PageLootTable>();
		yield break;
	}

	// Token: 0x060003C1 RID: 961 RVA: 0x0000F767 File Offset: 0x0000D967
	private IEnumerator WaitforGameStart()
	{
		MainMenuManager mmm = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MainMenuManager>();
		while (!mmm.GameHasStarted)
		{
			yield return new WaitForSeconds(0.1f);
		}
		base.StartCoroutine(this.DuendeBehavior());
		yield break;
	}

	// Token: 0x060003C2 RID: 962 RVA: 0x0000F776 File Offset: 0x0000D976
	private IEnumerator DuendeBehavior()
	{
		for (int j = 0; j < this.Duende.Length; j++)
		{
			this.Duende[j].DuendePoint = j;
			this.ServerSetDuendeID(j);
		}
		for (;;)
		{
			int num;
			for (int i = 0; i < this.Duende.Length; i = num + 1)
			{
				if (!this.Duende[i].CheckPlayersNear() && !this.isDuendeDead[i])
				{
					this.ServerNewPosition(i, this.PatrolPoints[this.GetNewUnoccupiedPoint(i)].position);
					yield return new WaitForSeconds((float)Random.Range(1, 10));
				}
				num = i;
			}
			if (Physics.OverlapSphere(base.transform.position, 75f, this.playerLayer).Length == 0)
			{
				this.RespawnDuende();
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x060003C3 RID: 963 RVA: 0x0000F785 File Offset: 0x0000D985
	[ServerRpc(RequireOwnership = false)]
	private void ServerNewPosition(int id, Vector3 newPos)
	{
		this.RpcWriter___Server_ServerNewPosition_215135683(id, newPos);
	}

	// Token: 0x060003C4 RID: 964 RVA: 0x0000F795 File Offset: 0x0000D995
	[ObserversRpc]
	private void ObserversNewPosition(int id, Vector3 newPos)
	{
		this.RpcWriter___Observers_ObserversNewPosition_215135683(id, newPos);
	}

	// Token: 0x060003C5 RID: 965 RVA: 0x0000F7A8 File Offset: 0x0000D9A8
	private int GetNewUnoccupiedPoint(int id)
	{
		this.pointFound = false;
		int num = Random.Range(0, this.PatrolPoints.Length);
		while (!this.pointFound)
		{
			this.pointFound = true;
			foreach (DuendeController duendeController in this.Duende)
			{
				if (num == duendeController.DuendePoint)
				{
					this.pointFound = false;
					num = Random.Range(0, this.PatrolPoints.Length);
				}
			}
		}
		this.Duende[id].DuendePoint = num;
		return num;
	}

	// Token: 0x060003C6 RID: 966 RVA: 0x0000F823 File Offset: 0x0000DA23
	[ServerRpc(RequireOwnership = false)]
	private void ServerSetDuendeID(int id)
	{
		this.RpcWriter___Server_ServerSetDuendeID_3316948804(id);
	}

	// Token: 0x060003C7 RID: 967 RVA: 0x0000F82F File Offset: 0x0000DA2F
	[ObserversRpc]
	private void ObserversSetDuendeID(int id)
	{
		this.RpcWriter___Observers_ObserversSetDuendeID_3316948804(id);
	}

	// Token: 0x060003C8 RID: 968 RVA: 0x0000F83B File Offset: 0x0000DA3B
	public void DuendeTrade(int id)
	{
		this.ServerDuendeTrade(id);
	}

	// Token: 0x060003C9 RID: 969 RVA: 0x0000F844 File Offset: 0x0000DA44
	[ServerRpc(RequireOwnership = false)]
	private void ServerDuendeTrade(int id)
	{
		this.RpcWriter___Server_ServerDuendeTrade_3316948804(id);
	}

	// Token: 0x060003CA RID: 970 RVA: 0x0000F850 File Offset: 0x0000DA50
	[ObserversRpc]
	private void ObserversDuendeTrade(int id)
	{
		this.RpcWriter___Observers_ObserversDuendeTrade_3316948804(id);
	}

	// Token: 0x060003CB RID: 971 RVA: 0x0000F85C File Offset: 0x0000DA5C
	public void TradeItem(int DuendeID)
	{
		if (Random.Range(0, 2) == 1)
		{
			this.ServerCreatePage(DuendeID, Random.Range(0, this.plt.Pages.Length));
			return;
		}
		this.ServerCreateItem(DuendeID, Random.Range(0, this.DuendeTradeItems.Length));
	}

	// Token: 0x060003CC RID: 972 RVA: 0x0000F898 File Offset: 0x0000DA98
	[ServerRpc(RequireOwnership = false)]
	private void ServerCreatePage(int DuendeID, int rand)
	{
		this.RpcWriter___Server_ServerCreatePage_1692629761(DuendeID, rand);
	}

	// Token: 0x060003CD RID: 973 RVA: 0x0000F8B4 File Offset: 0x0000DAB4
	[ServerRpc(RequireOwnership = false)]
	private void ServerCreateItem(int DuendeID, int rand)
	{
		this.RpcWriter___Server_ServerCreateItem_1692629761(DuendeID, rand);
	}

	// Token: 0x060003CE RID: 974 RVA: 0x0000F8D0 File Offset: 0x0000DAD0
	[ObserversRpc]
	private void SetupItem(GameObject obj, int DuendeID)
	{
		this.RpcWriter___Observers_SetupItem_2943392466(obj, DuendeID);
	}

	// Token: 0x060003CF RID: 975 RVA: 0x0000F8EC File Offset: 0x0000DAEC
	[ObserversRpc]
	private void SetupItem2(GameObject obj, int DuendeID)
	{
		this.RpcWriter___Observers_SetupItem2_2943392466(obj, DuendeID);
	}

	// Token: 0x060003D0 RID: 976 RVA: 0x0000F907 File Offset: 0x0000DB07
	public void KillDuende(int DuendeID)
	{
		this.ServerKillDuende(DuendeID);
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x0000F910 File Offset: 0x0000DB10
	[ServerRpc(RequireOwnership = false)]
	private void ServerKillDuende(int DuendeID)
	{
		this.RpcWriter___Server_ServerKillDuende_3316948804(DuendeID);
	}

	// Token: 0x060003D2 RID: 978 RVA: 0x0000F91C File Offset: 0x0000DB1C
	[ObserversRpc]
	private void ObsKillDuende(int DuendeID)
	{
		this.RpcWriter___Observers_ObsKillDuende_3316948804(DuendeID);
	}

	// Token: 0x060003D3 RID: 979 RVA: 0x0000F928 File Offset: 0x0000DB28
	private void RespawnDuende()
	{
		this.serverRespDuend();
	}

	// Token: 0x060003D4 RID: 980 RVA: 0x0000F930 File Offset: 0x0000DB30
	[ServerRpc(RequireOwnership = false)]
	private void serverRespDuend()
	{
		this.RpcWriter___Server_serverRespDuend_2166136261();
	}

	// Token: 0x060003D5 RID: 981 RVA: 0x0000F938 File Offset: 0x0000DB38
	[ObserversRpc]
	private void ObsRespDuend()
	{
		this.RpcWriter___Observers_ObsRespDuend_2166136261();
	}

	// Token: 0x060003D6 RID: 982 RVA: 0x0000F94B File Offset: 0x0000DB4B
	public void FireBallDetected(GameObject other)
	{
		if (base.HasAuthority)
		{
			this.ServerTogglePanic(true);
			this.detectedFireball = other;
			base.StartCoroutine(this.WaitForFireballtoDie());
		}
	}

	// Token: 0x060003D7 RID: 983 RVA: 0x0000F970 File Offset: 0x0000DB70
	private IEnumerator WaitForFireballtoDie()
	{
		while (this.detectedFireball != null)
		{
			for (int i = 0; i < this.Duende.Length; i++)
			{
				if (this.Duende[i].DuendeAgent.velocity.magnitude <= 0.5f && !this.isDuendeDead[i])
				{
					this.ServerNewPosition(i, this.PatrolPoints[this.GetNewUnoccupiedPoint(i)].position);
				}
			}
			yield return new WaitForSeconds(0.2f);
		}
		for (float timer = 0f; timer < 5f; timer += 0.2f)
		{
			for (int j = 0; j < this.Duende.Length; j++)
			{
				if (this.Duende[j].DuendeAgent.velocity.magnitude <= 0.5f && !this.isDuendeDead[j])
				{
					this.ServerNewPosition(j, this.PatrolPoints[this.GetNewUnoccupiedPoint(j)].position);
				}
			}
			yield return new WaitForSeconds(0.2f);
		}
		this.ServerTogglePanic(false);
		yield break;
	}

	// Token: 0x060003D8 RID: 984 RVA: 0x0000F97F File Offset: 0x0000DB7F
	[ServerRpc(RequireOwnership = false)]
	private void ServerTogglePanic(bool tf)
	{
		this.RpcWriter___Server_ServerTogglePanic_1140765316(tf);
	}

	// Token: 0x060003D9 RID: 985 RVA: 0x0000F98C File Offset: 0x0000DB8C
	[ObserversRpc]
	private void ObsTogglePanic(bool tf)
	{
		this.RpcWriter___Observers_ObsTogglePanic_1140765316(tf);
	}

	// Token: 0x060003DA RID: 986 RVA: 0x0000F9A3 File Offset: 0x0000DBA3
	private IEnumerator PanicForFiveSeconds()
	{
		this.ServerTogglePanic(true);
		for (float timer = 0f; timer < 12f; timer += 0.2f)
		{
			for (int i = 0; i < this.Duende.Length; i++)
			{
				if (this.Duende[i].DuendeAgent.velocity.magnitude <= 0.5f && !this.isDuendeDead[i])
				{
					this.ServerNewPosition(i, this.PatrolPoints[this.GetNewUnoccupiedPoint(i)].position);
				}
			}
			yield return new WaitForSeconds(0.2f);
		}
		this.ServerTogglePanic(false);
		yield break;
	}

	// Token: 0x060003DB RID: 987 RVA: 0x0000F9B2 File Offset: 0x0000DBB2
	public void PlayHello(int did)
	{
		this.serverPlayHello(did);
	}

	// Token: 0x060003DC RID: 988 RVA: 0x0000F9BB File Offset: 0x0000DBBB
	[ServerRpc(RequireOwnership = false)]
	private void serverPlayHello(int did)
	{
		this.RpcWriter___Server_serverPlayHello_3316948804(did);
	}

	// Token: 0x060003DD RID: 989 RVA: 0x0000F9C7 File Offset: 0x0000DBC7
	[ObserversRpc]
	private void ObsPlayHello(int did)
	{
		this.RpcWriter___Observers_ObsPlayHello_3316948804(did);
	}

	// Token: 0x060003DF RID: 991 RVA: 0x0000F9D4 File Offset: 0x0000DBD4
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyDuendeManagerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyDuendeManagerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ServerNewPosition_215135683));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversNewPosition_215135683));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_ServerSetDuendeID_3316948804));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversSetDuendeID_3316948804));
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_ServerDuendeTrade_3316948804));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversDuendeTrade_3316948804));
		base.RegisterServerRpc(6U, new ServerRpcDelegate(this.RpcReader___Server_ServerCreatePage_1692629761));
		base.RegisterServerRpc(7U, new ServerRpcDelegate(this.RpcReader___Server_ServerCreateItem_1692629761));
		base.RegisterObserversRpc(8U, new ClientRpcDelegate(this.RpcReader___Observers_SetupItem_2943392466));
		base.RegisterObserversRpc(9U, new ClientRpcDelegate(this.RpcReader___Observers_SetupItem2_2943392466));
		base.RegisterServerRpc(10U, new ServerRpcDelegate(this.RpcReader___Server_ServerKillDuende_3316948804));
		base.RegisterObserversRpc(11U, new ClientRpcDelegate(this.RpcReader___Observers_ObsKillDuende_3316948804));
		base.RegisterServerRpc(12U, new ServerRpcDelegate(this.RpcReader___Server_serverRespDuend_2166136261));
		base.RegisterObserversRpc(13U, new ClientRpcDelegate(this.RpcReader___Observers_ObsRespDuend_2166136261));
		base.RegisterServerRpc(14U, new ServerRpcDelegate(this.RpcReader___Server_ServerTogglePanic_1140765316));
		base.RegisterObserversRpc(15U, new ClientRpcDelegate(this.RpcReader___Observers_ObsTogglePanic_1140765316));
		base.RegisterServerRpc(16U, new ServerRpcDelegate(this.RpcReader___Server_serverPlayHello_3316948804));
		base.RegisterObserversRpc(17U, new ClientRpcDelegate(this.RpcReader___Observers_ObsPlayHello_3316948804));
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x0000FB90 File Offset: 0x0000DD90
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateDuendeManagerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateDuendeManagerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x060003E1 RID: 993 RVA: 0x0000FBA3 File Offset: 0x0000DDA3
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x060003E2 RID: 994 RVA: 0x0000FBB4 File Offset: 0x0000DDB4
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
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060003E3 RID: 995 RVA: 0x0000FC33 File Offset: 0x0000DE33
	private void RpcLogic___ServerNewPosition_215135683(int id, Vector3 newPos)
	{
		this.ObserversNewPosition(id, newPos);
	}

	// Token: 0x060003E4 RID: 996 RVA: 0x0000FC40 File Offset: 0x0000DE40
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

	// Token: 0x060003E5 RID: 997 RVA: 0x0000FC84 File Offset: 0x0000DE84
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
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060003E6 RID: 998 RVA: 0x0000FD12 File Offset: 0x0000DF12
	private void RpcLogic___ObserversNewPosition_215135683(int id, Vector3 newPos)
	{
		this.Duende[id].WalktoPoint(newPos);
	}

	// Token: 0x060003E7 RID: 999 RVA: 0x0000FD24 File Offset: 0x0000DF24
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

	// Token: 0x060003E8 RID: 1000 RVA: 0x0000FD68 File Offset: 0x0000DF68
	private void RpcWriter___Server_ServerSetDuendeID_3316948804(int id)
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

	// Token: 0x060003E9 RID: 1001 RVA: 0x0000FDDA File Offset: 0x0000DFDA
	private void RpcLogic___ServerSetDuendeID_3316948804(int id)
	{
		this.ObserversSetDuendeID(id);
	}

	// Token: 0x060003EA RID: 1002 RVA: 0x0000FDE4 File Offset: 0x0000DFE4
	private void RpcReader___Server_ServerSetDuendeID_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSetDuendeID_3316948804(num);
	}

	// Token: 0x060003EB RID: 1003 RVA: 0x0000FE18 File Offset: 0x0000E018
	private void RpcWriter___Observers_ObserversSetDuendeID_3316948804(int id)
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

	// Token: 0x060003EC RID: 1004 RVA: 0x0000FE99 File Offset: 0x0000E099
	private void RpcLogic___ObserversSetDuendeID_3316948804(int id)
	{
		this.Duende[id].SetDuendeID(id);
	}

	// Token: 0x060003ED RID: 1005 RVA: 0x0000FEAC File Offset: 0x0000E0AC
	private void RpcReader___Observers_ObserversSetDuendeID_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversSetDuendeID_3316948804(num);
	}

	// Token: 0x060003EE RID: 1006 RVA: 0x0000FEE0 File Offset: 0x0000E0E0
	private void RpcWriter___Server_ServerDuendeTrade_3316948804(int id)
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

	// Token: 0x060003EF RID: 1007 RVA: 0x0000FF52 File Offset: 0x0000E152
	private void RpcLogic___ServerDuendeTrade_3316948804(int id)
	{
		this.ObserversDuendeTrade(id);
	}

	// Token: 0x060003F0 RID: 1008 RVA: 0x0000FF5C File Offset: 0x0000E15C
	private void RpcReader___Server_ServerDuendeTrade_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerDuendeTrade_3316948804(num);
	}

	// Token: 0x060003F1 RID: 1009 RVA: 0x0000FF90 File Offset: 0x0000E190
	private void RpcWriter___Observers_ObserversDuendeTrade_3316948804(int id)
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

	// Token: 0x060003F2 RID: 1010 RVA: 0x00010011 File Offset: 0x0000E211
	private void RpcLogic___ObserversDuendeTrade_3316948804(int id)
	{
		this.Duende[id].isTrading = true;
		this.Duende[id].TradeAnis();
	}

	// Token: 0x060003F3 RID: 1011 RVA: 0x00010030 File Offset: 0x0000E230
	private void RpcReader___Observers_ObserversDuendeTrade_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversDuendeTrade_3316948804(num);
	}

	// Token: 0x060003F4 RID: 1012 RVA: 0x00010064 File Offset: 0x0000E264
	private void RpcWriter___Server_ServerCreatePage_1692629761(int DuendeID, int rand)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(DuendeID);
		pooledWriter.WriteInt32(rand);
		base.SendServerRpc(6U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060003F5 RID: 1013 RVA: 0x000100E4 File Offset: 0x0000E2E4
	private void RpcLogic___ServerCreatePage_1692629761(int DuendeID, int rand)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.plt.Pages[rand]);
		base.ServerManager.Spawn(gameObject, null, default(Scene));
		this.SetupItem2(gameObject, DuendeID);
	}

	// Token: 0x060003F6 RID: 1014 RVA: 0x00010124 File Offset: 0x0000E324
	private void RpcReader___Server_ServerCreatePage_1692629761(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		int num2 = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerCreatePage_1692629761(num, num2);
	}

	// Token: 0x060003F7 RID: 1015 RVA: 0x00010168 File Offset: 0x0000E368
	private void RpcWriter___Server_ServerCreateItem_1692629761(int DuendeID, int rand)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(DuendeID);
		pooledWriter.WriteInt32(rand);
		base.SendServerRpc(7U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060003F8 RID: 1016 RVA: 0x000101E8 File Offset: 0x0000E3E8
	private void RpcLogic___ServerCreateItem_1692629761(int DuendeID, int rand)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.DuendeTradeItems[rand]);
		base.ServerManager.Spawn(gameObject, null, default(Scene));
		this.SetupItem(gameObject, DuendeID);
	}

	// Token: 0x060003F9 RID: 1017 RVA: 0x00010224 File Offset: 0x0000E424
	private void RpcReader___Server_ServerCreateItem_1692629761(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		int num2 = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerCreateItem_1692629761(num, num2);
	}

	// Token: 0x060003FA RID: 1018 RVA: 0x00010268 File Offset: 0x0000E468
	private void RpcWriter___Observers_SetupItem_2943392466(GameObject obj, int DuendeID)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(obj);
		pooledWriter.WriteInt32(DuendeID);
		base.SendObserversRpc(8U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060003FB RID: 1019 RVA: 0x000102F8 File Offset: 0x0000E4F8
	private void RpcLogic___SetupItem_2943392466(GameObject obj, int DuendeID)
	{
		obj.transform.localScale = new Vector3(1f, 1f, 1f);
		obj.transform.parent = this.Duende[DuendeID].DuendeHandPoint;
		obj.transform.position = this.Duende[DuendeID].DuendeHandPoint.position;
		obj.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x060003FC RID: 1020 RVA: 0x00010378 File Offset: 0x0000E578
	private void RpcReader___Observers_SetupItem_2943392466(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___SetupItem_2943392466(gameObject, num);
	}

	// Token: 0x060003FD RID: 1021 RVA: 0x000103BC File Offset: 0x0000E5BC
	private void RpcWriter___Observers_SetupItem2_2943392466(GameObject obj, int DuendeID)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(obj);
		pooledWriter.WriteInt32(DuendeID);
		base.SendObserversRpc(9U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060003FE RID: 1022 RVA: 0x0001044C File Offset: 0x0000E64C
	private void RpcLogic___SetupItem2_2943392466(GameObject obj, int DuendeID)
	{
		obj.transform.localScale = new Vector3(1f, 1f, 1f);
		obj.transform.parent = this.Duende[DuendeID].DuendeHandPoint;
		obj.transform.localPosition = new Vector3(-0.079f, -0.057f, -0.1702f);
		obj.transform.localRotation = Quaternion.Euler(-22f, -10f, 90f);
	}

	// Token: 0x060003FF RID: 1023 RVA: 0x000104D0 File Offset: 0x0000E6D0
	private void RpcReader___Observers_SetupItem2_2943392466(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___SetupItem2_2943392466(gameObject, num);
	}

	// Token: 0x06000400 RID: 1024 RVA: 0x00010514 File Offset: 0x0000E714
	private void RpcWriter___Server_ServerKillDuende_3316948804(int DuendeID)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(DuendeID);
		base.SendServerRpc(10U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000401 RID: 1025 RVA: 0x00010586 File Offset: 0x0000E786
	private void RpcLogic___ServerKillDuende_3316948804(int DuendeID)
	{
		this.ObsKillDuende(DuendeID);
	}

	// Token: 0x06000402 RID: 1026 RVA: 0x00010590 File Offset: 0x0000E790
	private void RpcReader___Server_ServerKillDuende_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerKillDuende_3316948804(num);
	}

	// Token: 0x06000403 RID: 1027 RVA: 0x000105C4 File Offset: 0x0000E7C4
	private void RpcWriter___Observers_ObsKillDuende_3316948804(int DuendeID)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(DuendeID);
		base.SendObserversRpc(11U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000404 RID: 1028 RVA: 0x00010645 File Offset: 0x0000E845
	private void RpcLogic___ObsKillDuende_3316948804(int DuendeID)
	{
		if (!this.isDuendeDead[DuendeID])
		{
			this.Duende[DuendeID].KillDuende();
			this.isDuendeDead[DuendeID] = true;
			if (base.HasAuthority)
			{
				base.StartCoroutine(this.PanicForFiveSeconds());
			}
		}
	}

	// Token: 0x06000405 RID: 1029 RVA: 0x0001067C File Offset: 0x0000E87C
	private void RpcReader___Observers_ObsKillDuende_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsKillDuende_3316948804(num);
	}

	// Token: 0x06000406 RID: 1030 RVA: 0x000106B0 File Offset: 0x0000E8B0
	private void RpcWriter___Server_serverRespDuend_2166136261()
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

	// Token: 0x06000407 RID: 1031 RVA: 0x00010715 File Offset: 0x0000E915
	private void RpcLogic___serverRespDuend_2166136261()
	{
		this.ObsRespDuend();
	}

	// Token: 0x06000408 RID: 1032 RVA: 0x00010720 File Offset: 0x0000E920
	private void RpcReader___Server_serverRespDuend_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___serverRespDuend_2166136261();
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x00010740 File Offset: 0x0000E940
	private void RpcWriter___Observers_ObsRespDuend_2166136261()
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

	// Token: 0x0600040A RID: 1034 RVA: 0x000107B4 File Offset: 0x0000E9B4
	private void RpcLogic___ObsRespDuend_2166136261()
	{
		for (int i = 0; i < this.Duende.Length; i++)
		{
			if (this.isDuendeDead[i])
			{
				this.isDuendeDead[i] = false;
				this.Duende[i].RespawnDuende();
			}
		}
	}

	// Token: 0x0600040B RID: 1035 RVA: 0x000107F4 File Offset: 0x0000E9F4
	private void RpcReader___Observers_ObsRespDuend_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsRespDuend_2166136261();
	}

	// Token: 0x0600040C RID: 1036 RVA: 0x00010814 File Offset: 0x0000EA14
	private void RpcWriter___Server_ServerTogglePanic_1140765316(bool tf)
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
		base.SendServerRpc(14U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600040D RID: 1037 RVA: 0x00010886 File Offset: 0x0000EA86
	private void RpcLogic___ServerTogglePanic_1140765316(bool tf)
	{
		this.ObsTogglePanic(tf);
	}

	// Token: 0x0600040E RID: 1038 RVA: 0x00010890 File Offset: 0x0000EA90
	private void RpcReader___Server_ServerTogglePanic_1140765316(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerTogglePanic_1140765316(flag);
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x000108C4 File Offset: 0x0000EAC4
	private void RpcWriter___Observers_ObsTogglePanic_1140765316(bool tf)
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
		base.SendObserversRpc(15U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x00010948 File Offset: 0x0000EB48
	private void RpcLogic___ObsTogglePanic_1140765316(bool tf)
	{
		if (tf)
		{
			this.panicint++;
			DuendeController[] array = this.Duende;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].panic = true;
			}
			return;
		}
		if (!tf)
		{
			this.panicint--;
			if (this.panicint == 0)
			{
				DuendeController[] array = this.Duende;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].panic = false;
				}
			}
		}
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x000109BC File Offset: 0x0000EBBC
	private void RpcReader___Observers_ObsTogglePanic_1140765316(PooledReader PooledReader0, Channel channel)
	{
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsTogglePanic_1140765316(flag);
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x000109F0 File Offset: 0x0000EBF0
	private void RpcWriter___Server_serverPlayHello_3316948804(int did)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(did);
		base.SendServerRpc(16U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x00010A62 File Offset: 0x0000EC62
	private void RpcLogic___serverPlayHello_3316948804(int did)
	{
		this.ObsPlayHello(did);
	}

	// Token: 0x06000414 RID: 1044 RVA: 0x00010A6C File Offset: 0x0000EC6C
	private void RpcReader___Server_serverPlayHello_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___serverPlayHello_3316948804(num);
	}

	// Token: 0x06000415 RID: 1045 RVA: 0x00010AA0 File Offset: 0x0000ECA0
	private void RpcWriter___Observers_ObsPlayHello_3316948804(int did)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(did);
		base.SendObserversRpc(17U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x00010B21 File Offset: 0x0000ED21
	private void RpcLogic___ObsPlayHello_3316948804(int did)
	{
		this.Duende[did].ActualPlayHelloSound();
	}

	// Token: 0x06000417 RID: 1047 RVA: 0x00010B30 File Offset: 0x0000ED30
	private void RpcReader___Observers_ObsPlayHello_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsPlayHello_3316948804(num);
	}

	// Token: 0x06000418 RID: 1048 RVA: 0x0000FBA3 File Offset: 0x0000DDA3
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x040001F1 RID: 497
	public DuendeController[] Duende;

	// Token: 0x040001F2 RID: 498
	public Transform[] PatrolPoints;

	// Token: 0x040001F3 RID: 499
	private bool pointFound;

	// Token: 0x040001F4 RID: 500
	public GameObject[] DuendeTradeItems;

	// Token: 0x040001F5 RID: 501
	private bool[] isDuendeDead;

	// Token: 0x040001F6 RID: 502
	public LayerMask playerLayer;

	// Token: 0x040001F7 RID: 503
	private GameObject detectedFireball;

	// Token: 0x040001F8 RID: 504
	private int panicint;

	// Token: 0x040001F9 RID: 505
	private PageLootTable plt;

	// Token: 0x040001FA RID: 506
	private bool NetworkInitialize___EarlyDuendeManagerAssembly-CSharp.dll_Excuted;

	// Token: 0x040001FB RID: 507
	private bool NetworkInitialize__LateDuendeManagerAssembly-CSharp.dll_Excuted;
}
