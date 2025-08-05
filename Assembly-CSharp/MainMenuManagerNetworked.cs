using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using Steamworks;
using UnityEngine;

// Token: 0x020000E0 RID: 224
public class MainMenuManagerNetworked : NetworkBehaviour
{
	// Token: 0x060008F9 RID: 2297 RVA: 0x00023258 File Offset: 0x00021458
	public void ResetLocalTeam()
	{
		this.currentLocalTeam = 99;
		this.team1players = new string[4];
		this.team2players = new string[4];
		this.tutorialStarted = false;
		this.hasgamestarted = false;
		this.allownormalswaps = true;
	}

	// Token: 0x060008FA RID: 2298 RVA: 0x0002328F File Offset: 0x0002148F
	private void Start()
	{
		this.team1players = new string[4];
		this.team2players = new string[4];
	}

	// Token: 0x060008FB RID: 2299 RVA: 0x000232AC File Offset: 0x000214AC
	public void JoinLowestPlayerTeam(string playername)
	{
		this.localplayername = playername;
		bool flag = false;
		if (this.currentLocalTeam == 99)
		{
			flag = true;
		}
		this.ServerJoinLowestPlayerTeam(playername, flag);
	}

	// Token: 0x060008FC RID: 2300 RVA: 0x000232D8 File Offset: 0x000214D8
	[ServerRpc(RequireOwnership = false)]
	private void ServerJoinLowestPlayerTeam(string playername, bool actjoin)
	{
		this.RpcWriter___Server_ServerJoinLowestPlayerTeam_310431262(playername, actjoin);
	}

	// Token: 0x060008FD RID: 2301 RVA: 0x000232F4 File Offset: 0x000214F4
	[ObserversRpc]
	private void ObserversJoinLowestTeam(string playername, int teamtojoin)
	{
		this.RpcWriter___Observers_ObserversJoinLowestTeam_3643459082(playername, teamtojoin);
	}

	// Token: 0x060008FE RID: 2302 RVA: 0x0002330F File Offset: 0x0002150F
	private IEnumerator Waitframe()
	{
		yield return new WaitForSeconds(0.4f);
		if (!this.hasgamestarted)
		{
			this.hasgamestarted = true;
			this.mmm.ActuallyStartGameActually();
		}
		yield break;
	}

	// Token: 0x060008FF RID: 2303 RVA: 0x00023320 File Offset: 0x00021520
	public void JoinTeam(string playername, int teamtojoin)
	{
		this.localplayername = playername;
		if (teamtojoin != this.currentLocalTeam)
		{
			string text = "lvl ";
			int num;
			if (SteamUserStats.GetStat("level", out num))
			{
				text += num.ToString();
			}
			else
			{
				text += 0.ToString();
			}
			int num2;
			if (SteamUserStats.GetStat("rank", out num2))
			{
				text = text + " " + ((MainMenuManagerNetworked.WizardRank)Mathf.Clamp(num2, 0, 9)).ToString();
			}
			else
			{
				text += " Lacky";
			}
			this.ServerJoinTeam(playername, teamtojoin, text);
			return;
		}
		this.mmm.hasSwappedteam = true;
	}

	// Token: 0x06000900 RID: 2304 RVA: 0x000233C8 File Offset: 0x000215C8
	[ServerRpc(RequireOwnership = false)]
	private void ServerJoinTeam(string playername, int teamtojoin, string lvlandrank)
	{
		this.RpcWriter___Server_ServerJoinTeam_193317442(playername, teamtojoin, lvlandrank);
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x000233E8 File Offset: 0x000215E8
	[ObserversRpc]
	private void ObserversJoinTeam(string playername, int teamtojoin, int index, string lvlandrank)
	{
		this.RpcWriter___Observers_ObserversJoinTeam_964249301(playername, teamtojoin, index, lvlandrank);
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x0002340B File Offset: 0x0002160B
	[ObserversRpc]
	private void TeamWasFull(string playername)
	{
		this.RpcWriter___Observers_TeamWasFull_3615296227(playername);
	}

	// Token: 0x06000903 RID: 2307 RVA: 0x00023417 File Offset: 0x00021617
	[ObserversRpc]
	private void ObsRemoveFromTeam(int team, int index)
	{
		this.RpcWriter___Observers_ObsRemoveFromTeam_1692629761(team, index);
	}

	// Token: 0x06000904 RID: 2308 RVA: 0x00023427 File Offset: 0x00021627
	public void LeftInLobby(string playername)
	{
		this.ServerLeftInLobby(this.currentLocalTeam, playername);
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x00023438 File Offset: 0x00021638
	[ServerRpc(RequireOwnership = false)]
	private void ServerLeftInLobby(int currentteam, string playername)
	{
		this.RpcWriter___Server_ServerLeftInLobby_2801973956(currentteam, playername);
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x00023453 File Offset: 0x00021653
	public void StartGame()
	{
		this.ServerStartGame();
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x0002345B File Offset: 0x0002165B
	[ServerRpc(RequireOwnership = false)]
	private void ServerStartGame()
	{
		this.RpcWriter___Server_ServerStartGame_2166136261();
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x00023463 File Offset: 0x00021663
	[ObserversRpc]
	private void ObserversStartGame()
	{
		this.RpcWriter___Observers_ObserversStartGame_2166136261();
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x0002346B File Offset: 0x0002166B
	public void ActuallyStartGame()
	{
		this.ServerActuallyStart();
		this.allownormalswaps = false;
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x0002347A File Offset: 0x0002167A
	[ServerRpc(RequireOwnership = false)]
	private void ServerActuallyStart()
	{
		this.RpcWriter___Server_ServerActuallyStart_2166136261();
	}

	// Token: 0x0600090B RID: 2315 RVA: 0x00023482 File Offset: 0x00021682
	[ObserversRpc]
	private void ObsActuallyStart()
	{
		this.RpcWriter___Observers_ObsActuallyStart_2166136261();
	}

	// Token: 0x0600090C RID: 2316 RVA: 0x0002348A File Offset: 0x0002168A
	public void leavedagam()
	{
		this.ServerLeave();
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x00023492 File Offset: 0x00021692
	[ServerRpc(RequireOwnership = false)]
	private void ServerLeave()
	{
		this.RpcWriter___Server_ServerLeave_2166136261();
	}

	// Token: 0x0600090E RID: 2318 RVA: 0x0002349A File Offset: 0x0002169A
	[ObserversRpc]
	private void ObsLeave()
	{
		this.RpcWriter___Observers_ObsLeave_2166136261();
	}

	// Token: 0x0600090F RID: 2319 RVA: 0x000021EF File Offset: 0x000003EF
	public void disconnecteveryone()
	{
	}

	// Token: 0x06000910 RID: 2320 RVA: 0x000234A2 File Offset: 0x000216A2
	public void callinitSwizards()
	{
		this.callinitserver();
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x000234AA File Offset: 0x000216AA
	[ServerRpc(RequireOwnership = false)]
	private void callinitserver()
	{
		this.RpcWriter___Server_callinitserver_2166136261();
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x000234B2 File Offset: 0x000216B2
	[ObserversRpc]
	private void Callinitobservers()
	{
		this.RpcWriter___Observers_Callinitobservers_2166136261();
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x000234BA File Offset: 0x000216BA
	public void RequestMapInfo()
	{
		this.serverrequestmapinfo();
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x000234C2 File Offset: 0x000216C2
	[ServerRpc(RequireOwnership = false)]
	private void serverrequestmapinfo()
	{
		this.RpcWriter___Server_serverrequestmapinfo_2166136261();
	}

	// Token: 0x06000915 RID: 2325 RVA: 0x000234CA File Offset: 0x000216CA
	[ObserversRpc]
	private void obsRequestMapInfo()
	{
		this.RpcWriter___Observers_obsRequestMapInfo_2166136261();
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x000234D2 File Offset: 0x000216D2
	private void GiveMapInfo()
	{
		if (base.HasAuthority)
		{
			this.serverGivemapinfo(this.mmm.mapinfo);
		}
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x000234ED File Offset: 0x000216ED
	[ServerRpc(RequireOwnership = false)]
	private void serverGivemapinfo(int info)
	{
		this.RpcWriter___Server_serverGivemapinfo_3316948804(info);
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x000234F9 File Offset: 0x000216F9
	[ObserversRpc]
	private void obsGiveMapInfo(int info)
	{
		this.RpcWriter___Observers_obsGiveMapInfo_3316948804(info);
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x00023505 File Offset: 0x00021705
	public void moveSpawnPos()
	{
		this.ServerMoveSpawnPos();
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x0002350D File Offset: 0x0002170D
	[ServerRpc(RequireOwnership = false)]
	private void ServerMoveSpawnPos()
	{
		this.RpcWriter___Server_ServerMoveSpawnPos_2166136261();
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x00023515 File Offset: 0x00021715
	[ObserversRpc]
	private void obsmovespawn()
	{
		this.RpcWriter___Observers_obsmovespawn_2166136261();
	}

	// Token: 0x0600091C RID: 2332 RVA: 0x0002351D File Offset: 0x0002171D
	public void KickPlayer(string steamid)
	{
		this.kickPlayerServer(steamid);
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x00023526 File Offset: 0x00021726
	[ServerRpc(RequireOwnership = true)]
	private void kickPlayerServer(string steamid)
	{
		this.RpcWriter___Server_kickPlayerServer_3615296227(steamid);
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x00023532 File Offset: 0x00021732
	[ObserversRpc]
	private void ObsKickPlayer(string steamid)
	{
		this.RpcWriter___Observers_ObsKickPlayer_3615296227(steamid);
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x00023560 File Offset: 0x00021760
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyMainMenuManagerNetworkedAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyMainMenuManagerNetworkedAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ServerJoinLowestPlayerTeam_310431262));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversJoinLowestTeam_3643459082));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_ServerJoinTeam_193317442));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversJoinTeam_964249301));
		base.RegisterObserversRpc(4U, new ClientRpcDelegate(this.RpcReader___Observers_TeamWasFull_3615296227));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_ObsRemoveFromTeam_1692629761));
		base.RegisterServerRpc(6U, new ServerRpcDelegate(this.RpcReader___Server_ServerLeftInLobby_2801973956));
		base.RegisterServerRpc(7U, new ServerRpcDelegate(this.RpcReader___Server_ServerStartGame_2166136261));
		base.RegisterObserversRpc(8U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversStartGame_2166136261));
		base.RegisterServerRpc(9U, new ServerRpcDelegate(this.RpcReader___Server_ServerActuallyStart_2166136261));
		base.RegisterObserversRpc(10U, new ClientRpcDelegate(this.RpcReader___Observers_ObsActuallyStart_2166136261));
		base.RegisterServerRpc(11U, new ServerRpcDelegate(this.RpcReader___Server_ServerLeave_2166136261));
		base.RegisterObserversRpc(12U, new ClientRpcDelegate(this.RpcReader___Observers_ObsLeave_2166136261));
		base.RegisterServerRpc(13U, new ServerRpcDelegate(this.RpcReader___Server_callinitserver_2166136261));
		base.RegisterObserversRpc(14U, new ClientRpcDelegate(this.RpcReader___Observers_Callinitobservers_2166136261));
		base.RegisterServerRpc(15U, new ServerRpcDelegate(this.RpcReader___Server_serverrequestmapinfo_2166136261));
		base.RegisterObserversRpc(16U, new ClientRpcDelegate(this.RpcReader___Observers_obsRequestMapInfo_2166136261));
		base.RegisterServerRpc(17U, new ServerRpcDelegate(this.RpcReader___Server_serverGivemapinfo_3316948804));
		base.RegisterObserversRpc(18U, new ClientRpcDelegate(this.RpcReader___Observers_obsGiveMapInfo_3316948804));
		base.RegisterServerRpc(19U, new ServerRpcDelegate(this.RpcReader___Server_ServerMoveSpawnPos_2166136261));
		base.RegisterObserversRpc(20U, new ClientRpcDelegate(this.RpcReader___Observers_obsmovespawn_2166136261));
		base.RegisterServerRpc(21U, new ServerRpcDelegate(this.RpcReader___Server_kickPlayerServer_3615296227));
		base.RegisterObserversRpc(22U, new ClientRpcDelegate(this.RpcReader___Observers_ObsKickPlayer_3615296227));
	}

	// Token: 0x06000921 RID: 2337 RVA: 0x0002378F File Offset: 0x0002198F
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateMainMenuManagerNetworkedAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateMainMenuManagerNetworkedAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000922 RID: 2338 RVA: 0x000237A2 File Offset: 0x000219A2
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000923 RID: 2339 RVA: 0x000237B0 File Offset: 0x000219B0
	private void RpcWriter___Server_ServerJoinLowestPlayerTeam_310431262(string playername, bool actjoin)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteString(playername);
		pooledWriter.WriteBoolean(actjoin);
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000924 RID: 2340 RVA: 0x00023830 File Offset: 0x00021A30
	private void RpcLogic___ServerJoinLowestPlayerTeam_310431262(string playername, bool actjoin)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < this.team1players.Length; i++)
		{
			if (this.team1players[i] != null)
			{
				num++;
			}
		}
		for (int j = 0; j < this.team2players.Length; j++)
		{
			if (this.team2players[j] != null)
			{
				num2++;
			}
		}
		Debug.Log(num);
		Debug.Log(num2);
		if (num <= num2)
		{
			if (actjoin)
			{
				for (int k = 0; k < this.team1players.Length; k++)
				{
					if (this.team1players[k] == null)
					{
						this.team1players[k] = playername;
						break;
					}
				}
			}
			this.ObserversJoinLowestTeam(playername, 0);
			return;
		}
		if (actjoin)
		{
			for (int l = 0; l < this.team2players.Length; l++)
			{
				if (this.team2players[l] == null)
				{
					this.team2players[l] = playername;
					break;
				}
			}
		}
		this.ObserversJoinLowestTeam(playername, 2);
	}

	// Token: 0x06000925 RID: 2341 RVA: 0x00023910 File Offset: 0x00021B10
	private void RpcReader___Server_ServerJoinLowestPlayerTeam_310431262(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		string text = PooledReader0.ReadString();
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerJoinLowestPlayerTeam_310431262(text, flag);
	}

	// Token: 0x06000926 RID: 2342 RVA: 0x00023954 File Offset: 0x00021B54
	private void RpcWriter___Observers_ObserversJoinLowestTeam_3643459082(string playername, int teamtojoin)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteString(playername);
		pooledWriter.WriteInt32(teamtojoin);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000927 RID: 2343 RVA: 0x000239E4 File Offset: 0x00021BE4
	private void RpcLogic___ObserversJoinLowestTeam_3643459082(string playername, int teamtojoin)
	{
		Debug.Log(this.currentLocalTeam);
		if (playername == this.localplayername && this.currentLocalTeam == 99)
		{
			this.mmm.hasSwappedteam = true;
			this.currentLocalTeam = teamtojoin;
			this.mmm.pm.playerTeam = teamtojoin;
			BootstrapManager.instance.currentTeam = teamtojoin;
		}
		base.StartCoroutine(this.Waitframe());
	}

	// Token: 0x06000928 RID: 2344 RVA: 0x00023A58 File Offset: 0x00021C58
	private void RpcReader___Observers_ObserversJoinLowestTeam_3643459082(PooledReader PooledReader0, Channel channel)
	{
		string text = PooledReader0.ReadString();
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversJoinLowestTeam_3643459082(text, num);
	}

	// Token: 0x06000929 RID: 2345 RVA: 0x00023A9C File Offset: 0x00021C9C
	private void RpcWriter___Server_ServerJoinTeam_193317442(string playername, int teamtojoin, string lvlandrank)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteString(playername);
		pooledWriter.WriteInt32(teamtojoin);
		pooledWriter.WriteString(lvlandrank);
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600092A RID: 2346 RVA: 0x00023B28 File Offset: 0x00021D28
	private void RpcLogic___ServerJoinTeam_193317442(string playername, int teamtojoin, string lvlandrank)
	{
		if (this.allownormalswaps)
		{
			if (teamtojoin == 0)
			{
				bool flag = false;
				for (int i = 0; i < this.team1players.Length; i++)
				{
					if (this.team1players[i] == null)
					{
						this.team1players[i] = playername;
						this.ObserversJoinTeam(playername, teamtojoin, i, lvlandrank);
						for (int j = 0; j < this.team2players.Length; j++)
						{
							if (this.team2players[j] == playername)
							{
								this.team2players[j] = null;
								this.ObsRemoveFromTeam(2, j);
							}
						}
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this.TeamWasFull(playername);
					return;
				}
			}
			else
			{
				bool flag2 = false;
				for (int k = 0; k < this.team2players.Length; k++)
				{
					if (this.team2players[k] == null)
					{
						this.team2players[k] = playername;
						this.ObserversJoinTeam(playername, teamtojoin, k, lvlandrank);
						for (int l = 0; l < this.team1players.Length; l++)
						{
							if (this.team1players[l] == playername)
							{
								this.team1players[l] = null;
								this.ObsRemoveFromTeam(0, l);
							}
						}
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					this.TeamWasFull(playername);
				}
			}
		}
	}

	// Token: 0x0600092B RID: 2347 RVA: 0x00023C40 File Offset: 0x00021E40
	private void RpcReader___Server_ServerJoinTeam_193317442(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		string text = PooledReader0.ReadString();
		int num = PooledReader0.ReadInt32();
		string text2 = PooledReader0.ReadString();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerJoinTeam_193317442(text, num, text2);
	}

	// Token: 0x0600092C RID: 2348 RVA: 0x00023C94 File Offset: 0x00021E94
	private void RpcWriter___Observers_ObserversJoinTeam_964249301(string playername, int teamtojoin, int index, string lvlandrank)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteString(playername);
		pooledWriter.WriteInt32(teamtojoin);
		pooledWriter.WriteInt32(index);
		pooledWriter.WriteString(lvlandrank);
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600092D RID: 2349 RVA: 0x00023D3C File Offset: 0x00021F3C
	private void RpcLogic___ObserversJoinTeam_964249301(string playername, int teamtojoin, int index, string lvlandrank)
	{
		if (playername == this.localplayername)
		{
			this.mmm.hasSwappedteam = true;
			this.currentLocalTeam = teamtojoin;
			this.mmm.pm.playerTeam = teamtojoin;
			BootstrapManager.instance.currentTeam = teamtojoin;
		}
		this.mmm.ChangeTeamText(teamtojoin, index, playername, lvlandrank);
	}

	// Token: 0x0600092E RID: 2350 RVA: 0x00023D98 File Offset: 0x00021F98
	private void RpcReader___Observers_ObserversJoinTeam_964249301(PooledReader PooledReader0, Channel channel)
	{
		string text = PooledReader0.ReadString();
		int num = PooledReader0.ReadInt32();
		int num2 = PooledReader0.ReadInt32();
		string text2 = PooledReader0.ReadString();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversJoinTeam_964249301(text, num, num2, text2);
	}

	// Token: 0x0600092F RID: 2351 RVA: 0x00023DFC File Offset: 0x00021FFC
	private void RpcWriter___Observers_TeamWasFull_3615296227(string playername)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteString(playername);
		base.SendObserversRpc(4U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000930 RID: 2352 RVA: 0x00023E7D File Offset: 0x0002207D
	private void RpcLogic___TeamWasFull_3615296227(string playername)
	{
		if (playername == this.localplayername)
		{
			this.mmm.hasSwappedteam = true;
		}
	}

	// Token: 0x06000931 RID: 2353 RVA: 0x00023E9C File Offset: 0x0002209C
	private void RpcReader___Observers_TeamWasFull_3615296227(PooledReader PooledReader0, Channel channel)
	{
		string text = PooledReader0.ReadString();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___TeamWasFull_3615296227(text);
	}

	// Token: 0x06000932 RID: 2354 RVA: 0x00023ED0 File Offset: 0x000220D0
	private void RpcWriter___Observers_ObsRemoveFromTeam_1692629761(int team, int index)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(team);
		pooledWriter.WriteInt32(index);
		base.SendObserversRpc(5U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000933 RID: 2355 RVA: 0x00023F5E File Offset: 0x0002215E
	private void RpcLogic___ObsRemoveFromTeam_1692629761(int team, int index)
	{
		this.mmm.DestroySpinningGuy(team, index);
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x00023F70 File Offset: 0x00022170
	private void RpcReader___Observers_ObsRemoveFromTeam_1692629761(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		int num2 = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsRemoveFromTeam_1692629761(num, num2);
	}

	// Token: 0x06000935 RID: 2357 RVA: 0x00023FB4 File Offset: 0x000221B4
	private void RpcWriter___Server_ServerLeftInLobby_2801973956(int currentteam, string playername)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(currentteam);
		pooledWriter.WriteString(playername);
		base.SendServerRpc(6U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000936 RID: 2358 RVA: 0x00024034 File Offset: 0x00022234
	private void RpcLogic___ServerLeftInLobby_2801973956(int currentteam, string playername)
	{
		if (this.allownormalswaps)
		{
			for (int i = 0; i < this.team1players.Length; i++)
			{
				if (this.team1players[i] == playername)
				{
					this.team1players[i] = null;
					this.ObsRemoveFromTeam(0, i);
				}
			}
			for (int j = 0; j < this.team2players.Length; j++)
			{
				if (this.team2players[j] == playername)
				{
					this.team2players[j] = null;
					this.ObsRemoveFromTeam(2, j);
				}
			}
		}
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x000240B4 File Offset: 0x000222B4
	private void RpcReader___Server_ServerLeftInLobby_2801973956(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		string text = PooledReader0.ReadString();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerLeftInLobby_2801973956(num, text);
	}

	// Token: 0x06000938 RID: 2360 RVA: 0x000240F8 File Offset: 0x000222F8
	private void RpcWriter___Server_ServerStartGame_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(7U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000939 RID: 2361 RVA: 0x0002415D File Offset: 0x0002235D
	private void RpcLogic___ServerStartGame_2166136261()
	{
		this.ObserversStartGame();
	}

	// Token: 0x0600093A RID: 2362 RVA: 0x00024168 File Offset: 0x00022368
	private void RpcReader___Server_ServerStartGame_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerStartGame_2166136261();
	}

	// Token: 0x0600093B RID: 2363 RVA: 0x00024188 File Offset: 0x00022388
	private void RpcWriter___Observers_ObserversStartGame_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(8U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600093C RID: 2364 RVA: 0x000241FC File Offset: 0x000223FC
	private void RpcLogic___ObserversStartGame_2166136261()
	{
		this.mmm.StartGameActual();
	}

	// Token: 0x0600093D RID: 2365 RVA: 0x0002420C File Offset: 0x0002240C
	private void RpcReader___Observers_ObserversStartGame_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversStartGame_2166136261();
	}

	// Token: 0x0600093E RID: 2366 RVA: 0x0002422C File Offset: 0x0002242C
	private void RpcWriter___Server_ServerActuallyStart_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(9U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600093F RID: 2367 RVA: 0x00024291 File Offset: 0x00022491
	private void RpcLogic___ServerActuallyStart_2166136261()
	{
		this.ObsActuallyStart();
	}

	// Token: 0x06000940 RID: 2368 RVA: 0x0002429C File Offset: 0x0002249C
	private void RpcReader___Server_ServerActuallyStart_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerActuallyStart_2166136261();
	}

	// Token: 0x06000941 RID: 2369 RVA: 0x000242BC File Offset: 0x000224BC
	private void RpcWriter___Observers_ObsActuallyStart_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(10U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000942 RID: 2370 RVA: 0x00024330 File Offset: 0x00022530
	private void RpcLogic___ObsActuallyStart_2166136261()
	{
		this.mmm.JoinLowestPlayerCountTeam();
	}

	// Token: 0x06000943 RID: 2371 RVA: 0x00024340 File Offset: 0x00022540
	private void RpcReader___Observers_ObsActuallyStart_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsActuallyStart_2166136261();
	}

	// Token: 0x06000944 RID: 2372 RVA: 0x00024360 File Offset: 0x00022560
	private void RpcWriter___Server_ServerLeave_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(11U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000945 RID: 2373 RVA: 0x000243C5 File Offset: 0x000225C5
	private void RpcLogic___ServerLeave_2166136261()
	{
		this.ObsLeave();
	}

	// Token: 0x06000946 RID: 2374 RVA: 0x000243D0 File Offset: 0x000225D0
	private void RpcReader___Server_ServerLeave_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerLeave_2166136261();
	}

	// Token: 0x06000947 RID: 2375 RVA: 0x000243F0 File Offset: 0x000225F0
	private void RpcWriter___Observers_ObsLeave_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(12U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000948 RID: 2376 RVA: 0x00024464 File Offset: 0x00022664
	private void RpcLogic___ObsLeave_2166136261()
	{
		this.mmm.ActLeaveGame();
	}

	// Token: 0x06000949 RID: 2377 RVA: 0x00024474 File Offset: 0x00022674
	private void RpcReader___Observers_ObsLeave_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsLeave_2166136261();
	}

	// Token: 0x0600094A RID: 2378 RVA: 0x00024494 File Offset: 0x00022694
	private void RpcWriter___Server_callinitserver_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(13U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600094B RID: 2379 RVA: 0x000244F9 File Offset: 0x000226F9
	private void RpcLogic___callinitserver_2166136261()
	{
		this.Callinitobservers();
	}

	// Token: 0x0600094C RID: 2380 RVA: 0x00024504 File Offset: 0x00022704
	private void RpcReader___Server_callinitserver_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___callinitserver_2166136261();
	}

	// Token: 0x0600094D RID: 2381 RVA: 0x00024524 File Offset: 0x00022724
	private void RpcWriter___Observers_Callinitobservers_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(14U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600094E RID: 2382 RVA: 0x00024598 File Offset: 0x00022798
	private void RpcLogic___Callinitobservers_2166136261()
	{
		this.mmm.initSwizards();
	}

	// Token: 0x0600094F RID: 2383 RVA: 0x000245A8 File Offset: 0x000227A8
	private void RpcReader___Observers_Callinitobservers_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___Callinitobservers_2166136261();
	}

	// Token: 0x06000950 RID: 2384 RVA: 0x000245C8 File Offset: 0x000227C8
	private void RpcWriter___Server_serverrequestmapinfo_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(15U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000951 RID: 2385 RVA: 0x0002462D File Offset: 0x0002282D
	private void RpcLogic___serverrequestmapinfo_2166136261()
	{
		this.obsRequestMapInfo();
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x00024638 File Offset: 0x00022838
	private void RpcReader___Server_serverrequestmapinfo_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___serverrequestmapinfo_2166136261();
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x00024658 File Offset: 0x00022858
	private void RpcWriter___Observers_obsRequestMapInfo_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(16U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000954 RID: 2388 RVA: 0x000246CC File Offset: 0x000228CC
	private void RpcLogic___obsRequestMapInfo_2166136261()
	{
		this.GiveMapInfo();
	}

	// Token: 0x06000955 RID: 2389 RVA: 0x000246D4 File Offset: 0x000228D4
	private void RpcReader___Observers_obsRequestMapInfo_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___obsRequestMapInfo_2166136261();
	}

	// Token: 0x06000956 RID: 2390 RVA: 0x000246F4 File Offset: 0x000228F4
	private void RpcWriter___Server_serverGivemapinfo_3316948804(int info)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(info);
		base.SendServerRpc(17U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000957 RID: 2391 RVA: 0x00024766 File Offset: 0x00022966
	private void RpcLogic___serverGivemapinfo_3316948804(int info)
	{
		this.obsGiveMapInfo(info);
	}

	// Token: 0x06000958 RID: 2392 RVA: 0x00024770 File Offset: 0x00022970
	private void RpcReader___Server_serverGivemapinfo_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___serverGivemapinfo_3316948804(num);
	}

	// Token: 0x06000959 RID: 2393 RVA: 0x000247A4 File Offset: 0x000229A4
	private void RpcWriter___Observers_obsGiveMapInfo_3316948804(int info)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(info);
		base.SendObserversRpc(18U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600095A RID: 2394 RVA: 0x00024825 File Offset: 0x00022A25
	private void RpcLogic___obsGiveMapInfo_3316948804(int info)
	{
		this.mmm.mapinfo = info;
		if (info == 2)
		{
			this.moveSpawnPos();
		}
		base.GetComponent<DungeonGenerator>().setMapInfo(info);
	}

	// Token: 0x0600095B RID: 2395 RVA: 0x0002484C File Offset: 0x00022A4C
	private void RpcReader___Observers_obsGiveMapInfo_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___obsGiveMapInfo_3316948804(num);
	}

	// Token: 0x0600095C RID: 2396 RVA: 0x00024880 File Offset: 0x00022A80
	private void RpcWriter___Server_ServerMoveSpawnPos_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(19U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600095D RID: 2397 RVA: 0x000248E5 File Offset: 0x00022AE5
	private void RpcLogic___ServerMoveSpawnPos_2166136261()
	{
		this.obsmovespawn();
	}

	// Token: 0x0600095E RID: 2398 RVA: 0x000248F0 File Offset: 0x00022AF0
	private void RpcReader___Server_ServerMoveSpawnPos_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerMoveSpawnPos_2166136261();
	}

	// Token: 0x0600095F RID: 2399 RVA: 0x00024910 File Offset: 0x00022B10
	private void RpcWriter___Observers_obsmovespawn_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(20U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000960 RID: 2400 RVA: 0x00024984 File Offset: 0x00022B84
	private void RpcLogic___obsmovespawn_2166136261()
	{
		this.mmm.movespawnscolosseum();
	}

	// Token: 0x06000961 RID: 2401 RVA: 0x00024994 File Offset: 0x00022B94
	private void RpcReader___Observers_obsmovespawn_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___obsmovespawn_2166136261();
	}

	// Token: 0x06000962 RID: 2402 RVA: 0x000249B4 File Offset: 0x00022BB4
	private void RpcWriter___Server_kickPlayerServer_3615296227(string steamid)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		if (!base.IsOwner)
		{
			NetworkManager networkManager2 = base.NetworkManager;
			networkManager2.LogWarning("Cannot complete action because you are not the owner of this object. .");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteString(steamid);
		base.SendServerRpc(21U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000963 RID: 2403 RVA: 0x00024A4B File Offset: 0x00022C4B
	private void RpcLogic___kickPlayerServer_3615296227(string steamid)
	{
		this.ObsKickPlayer(steamid);
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x00024A54 File Offset: 0x00022C54
	private void RpcReader___Server_kickPlayerServer_3615296227(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		string text = PooledReader0.ReadString();
		if (!base.IsServerInitialized)
		{
			return;
		}
		if (!base.OwnerMatches(conn))
		{
			return;
		}
		this.RpcLogic___kickPlayerServer_3615296227(text);
	}

	// Token: 0x06000965 RID: 2405 RVA: 0x00024A98 File Offset: 0x00022C98
	private void RpcWriter___Observers_ObsKickPlayer_3615296227(string steamid)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteString(steamid);
		base.SendObserversRpc(22U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000966 RID: 2406 RVA: 0x00024B19 File Offset: 0x00022D19
	private void RpcLogic___ObsKickPlayer_3615296227(string steamid)
	{
		this.mmm.ActKickPlayer(steamid);
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x00024B28 File Offset: 0x00022D28
	private void RpcReader___Observers_ObsKickPlayer_3615296227(PooledReader PooledReader0, Channel channel)
	{
		string text = PooledReader0.ReadString();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsKickPlayer_3615296227(text);
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x000237A2 File Offset: 0x000219A2
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x040004E5 RID: 1253
	public MainMenuManager mmm;

	// Token: 0x040004E6 RID: 1254
	private bool tutorialStarted;

	// Token: 0x040004E7 RID: 1255
	private string[] team1players;

	// Token: 0x040004E8 RID: 1256
	private string[] team2players;

	// Token: 0x040004E9 RID: 1257
	private string localplayername = "lksdhgdcvnufnweruinuiwnvuiwernfisdfgklsdfgkdfgkljdfgkljdfklgjkldfkljgkljdflkfjgkldfjkljdfgkljdflkjkldfjgaopkdowqkcmcm0qo";

	// Token: 0x040004EA RID: 1258
	private int currentLocalTeam = 99;

	// Token: 0x040004EB RID: 1259
	private bool hasgamestarted;

	// Token: 0x040004EC RID: 1260
	private bool allownormalswaps = true;

	// Token: 0x040004ED RID: 1261
	private bool NetworkInitialize___EarlyMainMenuManagerNetworkedAssembly-CSharp.dll_Excuted;

	// Token: 0x040004EE RID: 1262
	private bool NetworkInitialize__LateMainMenuManagerNetworkedAssembly-CSharp.dll_Excuted;

	// Token: 0x020000E1 RID: 225
	private enum WizardRank
	{
		// Token: 0x040004F0 RID: 1264
		Lackey,
		// Token: 0x040004F1 RID: 1265
		Sputterer,
		// Token: 0x040004F2 RID: 1266
		Novice,
		// Token: 0x040004F3 RID: 1267
		Apprentice,
		// Token: 0x040004F4 RID: 1268
		Savant,
		// Token: 0x040004F5 RID: 1269
		Master,
		// Token: 0x040004F6 RID: 1270
		Grand_Master,
		// Token: 0x040004F7 RID: 1271
		Archmagus,
		// Token: 0x040004F8 RID: 1272
		Magus_Prime,
		// Token: 0x040004F9 RID: 1273
		Supreme_Archmagus
	}
}
