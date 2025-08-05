using System;
using System.Collections;
using System.Linq;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Serializing.Generated;
using FishNet.Transporting;
using Steamworks;
using UnityEngine;

// Token: 0x0200001B RID: 27
public class BootstrapNetworkManager : NetworkBehaviour
{
	// Token: 0x0600010E RID: 270 RVA: 0x00006854 File Offset: 0x00004A54
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.Awake_UserLogic_BootstrapNetworkManager_Assembly-CSharp.dll();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0600010F RID: 271 RVA: 0x00006868 File Offset: 0x00004A68
	public static void ChangeNetworkSceneLeave(string sceneName)
	{
		SceneLoadData sceneLoadData = new SceneLoadData(sceneName);
		NetworkConnection[] array = BootstrapNetworkManager.instance.ServerManager.Clients.Values.ToArray<NetworkConnection>();
		BootstrapNetworkManager.instance.SceneManager.LoadConnectionScenes(array, sceneLoadData);
	}

	// Token: 0x06000110 RID: 272 RVA: 0x000068A8 File Offset: 0x00004AA8
	public static void ChangeNetworkScene(string sceneName)
	{
		SceneLoadData sceneLoadData = new SceneLoadData(sceneName);
		NetworkConnection[] array = BootstrapNetworkManager.instance.ServerManager.Clients.Values.ToArray<NetworkConnection>();
		BootstrapNetworkManager.instance.SceneManager.LoadConnectionScenes(array, sceneLoadData);
		BootstrapNetworkManager.instance.sdfgklnsdfgjksdfgjk();
	}

	// Token: 0x06000111 RID: 273 RVA: 0x000068F1 File Offset: 0x00004AF1
	[ServerRpc(RequireOwnership = false)]
	private void sdfgklnsdfgjksdfgjk()
	{
		this.RpcWriter___Server_sdfgklnsdfgjksdfgjk_2166136261();
	}

	// Token: 0x06000112 RID: 274 RVA: 0x000068F9 File Offset: 0x00004AF9
	[ObserversRpc]
	private void fsdklfjskldgkrg()
	{
		this.RpcWriter___Observers_fsdklfjskldgkrg_2166136261();
	}

	// Token: 0x06000113 RID: 275 RVA: 0x00006904 File Offset: 0x00004B04
	public static void ChangeNetworkSceneTutorial(string sceneName)
	{
		SceneLoadData sceneLoadData = new SceneLoadData(sceneName);
		NetworkConnection[] array = BootstrapNetworkManager.instance.ServerManager.Clients.Values.ToArray<NetworkConnection>();
		BootstrapNetworkManager.instance.SceneManager.LoadConnectionScenes(array, sceneLoadData);
		BootstrapNetworkManager.instance.ServerTutStart();
	}

	// Token: 0x06000114 RID: 276 RVA: 0x0000694D File Offset: 0x00004B4D
	[ServerRpc(RequireOwnership = false)]
	private void ServerTutStart()
	{
		this.RpcWriter___Server_ServerTutStart_2166136261();
	}

	// Token: 0x06000115 RID: 277 RVA: 0x00006955 File Offset: 0x00004B55
	[ObserversRpc]
	private void ObsTutStart()
	{
		this.RpcWriter___Observers_ObsTutStart_2166136261();
	}

	// Token: 0x06000116 RID: 278 RVA: 0x0000695D File Offset: 0x00004B5D
	public static void OnJoinUpdateHats(string PlayerName)
	{
		if (BootstrapNetworkManager.instance != null && BootstrapNetworkManager.instance.gameObject.activeInHierarchy)
		{
			BootstrapNetworkManager.instance.ionknowbruhther(PlayerName);
		}
	}

	// Token: 0x06000117 RID: 279 RVA: 0x00006988 File Offset: 0x00004B88
	private void ionknowbruhther(string PlayerName)
	{
		base.StartCoroutine(this.WaitforClientToStart(PlayerName));
	}

	// Token: 0x06000118 RID: 280 RVA: 0x00006998 File Offset: 0x00004B98
	private IEnumerator WaitforClientToStart(string PlayerName)
	{
		while (!base.IsClientInitialized)
		{
			yield return null;
		}
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
			text = text + " " + ((BootstrapNetworkManager.WizardRank)Mathf.Clamp(num2, 0, 9)).ToString();
		}
		else
		{
			text += " Lacky";
		}
		if (Time.time - this.cooldown > 1f)
		{
			this.cooldown = Time.time;
			this.ServerJoinHatsCall(PlayerName, text, SteamUser.GetSteamID().ToString());
		}
		yield break;
	}

	// Token: 0x06000119 RID: 281 RVA: 0x000069AE File Offset: 0x00004BAE
	[ServerRpc(RequireOwnership = false)]
	private void ServerJoinHatsCall(string PlayerName, string levelandrank, string steamid)
	{
		this.RpcWriter___Server_ServerJoinHatsCall_852232071(PlayerName, levelandrank, steamid);
	}

	// Token: 0x0600011A RID: 282 RVA: 0x000069C2 File Offset: 0x00004BC2
	[ObserversRpc]
	private void ObsCallJoinHats(string PlayerName, string levelandrank, string steamid)
	{
		this.RpcWriter___Observers_ObsCallJoinHats_852232071(PlayerName, levelandrank, steamid);
	}

	// Token: 0x0600011B RID: 283 RVA: 0x000069D6 File Offset: 0x00004BD6
	public static void SyncPlayerNames(string[] PlayerNames, string[] PlayerRanks)
	{
		if (BootstrapNetworkManager.instance != null && BootstrapNetworkManager.instance.gameObject.activeInHierarchy)
		{
			BootstrapNetworkManager.instance.ServerSyncPlayerNames(PlayerNames, PlayerRanks);
		}
	}

	// Token: 0x0600011C RID: 284 RVA: 0x00006A02 File Offset: 0x00004C02
	[ServerRpc(RequireOwnership = false)]
	private void ServerSyncPlayerNames(string[] PlayerNames, string[] PlayerRanks)
	{
		this.RpcWriter___Server_ServerSyncPlayerNames_3821904405(PlayerNames, PlayerRanks);
	}

	// Token: 0x0600011D RID: 285 RVA: 0x00006A12 File Offset: 0x00004C12
	[ObserversRpc]
	private void ObsSyncPlayerNames(string[] PlayerNames, string[] PlayerRanks)
	{
		this.RpcWriter___Observers_ObsSyncPlayerNames_3821904405(PlayerNames, PlayerRanks);
	}

	// Token: 0x0600011E RID: 286 RVA: 0x00006A22 File Offset: 0x00004C22
	public static void KickPlayer(string steamid)
	{
		if (BootstrapNetworkManager.instance != null && BootstrapNetworkManager.instance.gameObject.activeInHierarchy)
		{
			BootstrapNetworkManager.instance.kickPlayerServer(steamid);
		}
	}

	// Token: 0x0600011F RID: 287 RVA: 0x00006A4D File Offset: 0x00004C4D
	[ServerRpc(RequireOwnership = false)]
	private void kickPlayerServer(string steamid)
	{
		this.RpcWriter___Server_kickPlayerServer_3615296227(steamid);
	}

	// Token: 0x06000120 RID: 288 RVA: 0x00006A59 File Offset: 0x00004C59
	[ObserversRpc]
	private void ObsKickPlayer(string steamid)
	{
		this.RpcWriter___Observers_ObsKickPlayer_3615296227(steamid);
	}

	// Token: 0x06000121 RID: 289 RVA: 0x00006A65 File Offset: 0x00004C65
	public static void HostLeftInLobby()
	{
		BootstrapNetworkManager.instance.ServerDisbandLobby();
	}

	// Token: 0x06000122 RID: 290 RVA: 0x00006A71 File Offset: 0x00004C71
	[ServerRpc(RequireOwnership = false)]
	private void ServerDisbandLobby()
	{
		this.RpcWriter___Server_ServerDisbandLobby_2166136261();
	}

	// Token: 0x06000123 RID: 291 RVA: 0x00006A79 File Offset: 0x00004C79
	[ObserversRpc]
	private void ObsDisband()
	{
		this.RpcWriter___Observers_ObsDisband_2166136261();
	}

	// Token: 0x06000125 RID: 293 RVA: 0x00006A84 File Offset: 0x00004C84
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyBootstrapNetworkManagerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyBootstrapNetworkManagerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_sdfgklnsdfgjksdfgjk_2166136261));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_fsdklfjskldgkrg_2166136261));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_ServerTutStart_2166136261));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ObsTutStart_2166136261));
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_ServerJoinHatsCall_852232071));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_ObsCallJoinHats_852232071));
		base.RegisterServerRpc(6U, new ServerRpcDelegate(this.RpcReader___Server_ServerSyncPlayerNames_3821904405));
		base.RegisterObserversRpc(7U, new ClientRpcDelegate(this.RpcReader___Observers_ObsSyncPlayerNames_3821904405));
		base.RegisterServerRpc(8U, new ServerRpcDelegate(this.RpcReader___Server_kickPlayerServer_3615296227));
		base.RegisterObserversRpc(9U, new ClientRpcDelegate(this.RpcReader___Observers_ObsKickPlayer_3615296227));
		base.RegisterServerRpc(10U, new ServerRpcDelegate(this.RpcReader___Server_ServerDisbandLobby_2166136261));
		base.RegisterObserversRpc(11U, new ClientRpcDelegate(this.RpcReader___Observers_ObsDisband_2166136261));
	}

	// Token: 0x06000126 RID: 294 RVA: 0x00006BB6 File Offset: 0x00004DB6
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateBootstrapNetworkManagerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateBootstrapNetworkManagerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000127 RID: 295 RVA: 0x00006BC9 File Offset: 0x00004DC9
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000128 RID: 296 RVA: 0x00006BD8 File Offset: 0x00004DD8
	private void RpcWriter___Server_sdfgklnsdfgjksdfgjk_2166136261()
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

	// Token: 0x06000129 RID: 297 RVA: 0x00006C3D File Offset: 0x00004E3D
	private void RpcLogic___sdfgklnsdfgjksdfgjk_2166136261()
	{
		this.fsdklfjskldgkrg();
	}

	// Token: 0x0600012A RID: 298 RVA: 0x00006C48 File Offset: 0x00004E48
	private void RpcReader___Server_sdfgklnsdfgjksdfgjk_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___sdfgklnsdfgjksdfgjk_2166136261();
	}

	// Token: 0x0600012B RID: 299 RVA: 0x00006C68 File Offset: 0x00004E68
	private void RpcWriter___Observers_fsdklfjskldgkrg_2166136261()
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

	// Token: 0x0600012C RID: 300 RVA: 0x00006CDC File Offset: 0x00004EDC
	private void RpcLogic___fsdklfjskldgkrg_2166136261()
	{
		GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MainMenuManager>().StartGameActual();
	}

	// Token: 0x0600012D RID: 301 RVA: 0x00006CF4 File Offset: 0x00004EF4
	private void RpcReader___Observers_fsdklfjskldgkrg_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___fsdklfjskldgkrg_2166136261();
	}

	// Token: 0x0600012E RID: 302 RVA: 0x00006D14 File Offset: 0x00004F14
	private void RpcWriter___Server_ServerTutStart_2166136261()
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

	// Token: 0x0600012F RID: 303 RVA: 0x00006D79 File Offset: 0x00004F79
	private void RpcLogic___ServerTutStart_2166136261()
	{
		this.ObsTutStart();
	}

	// Token: 0x06000130 RID: 304 RVA: 0x00006D84 File Offset: 0x00004F84
	private void RpcReader___Server_ServerTutStart_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerTutStart_2166136261();
	}

	// Token: 0x06000131 RID: 305 RVA: 0x00006DA4 File Offset: 0x00004FA4
	private void RpcWriter___Observers_ObsTutStart_2166136261()
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

	// Token: 0x06000132 RID: 306 RVA: 0x00006E18 File Offset: 0x00005018
	private void RpcLogic___ObsTutStart_2166136261()
	{
		GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MainMenuManager>().StartGameActualTutorial();
	}

	// Token: 0x06000133 RID: 307 RVA: 0x00006E30 File Offset: 0x00005030
	private void RpcReader___Observers_ObsTutStart_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsTutStart_2166136261();
	}

	// Token: 0x06000134 RID: 308 RVA: 0x00006E50 File Offset: 0x00005050
	private void RpcWriter___Server_ServerJoinHatsCall_852232071(string PlayerName, string levelandrank, string steamid)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteString(PlayerName);
		pooledWriter.WriteString(levelandrank);
		pooledWriter.WriteString(steamid);
		base.SendServerRpc(4U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000135 RID: 309 RVA: 0x00006EDC File Offset: 0x000050DC
	private void RpcLogic___ServerJoinHatsCall_852232071(string PlayerName, string levelandrank, string steamid)
	{
		this.ObsCallJoinHats(PlayerName, levelandrank, steamid);
	}

	// Token: 0x06000136 RID: 310 RVA: 0x00006EE8 File Offset: 0x000050E8
	private void RpcReader___Server_ServerJoinHatsCall_852232071(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		string text = PooledReader0.ReadString();
		string text2 = PooledReader0.ReadString();
		string text3 = PooledReader0.ReadString();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerJoinHatsCall_852232071(text, text2, text3);
	}

	// Token: 0x06000137 RID: 311 RVA: 0x00006F3C File Offset: 0x0000513C
	private void RpcWriter___Observers_ObsCallJoinHats_852232071(string PlayerName, string levelandrank, string steamid)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteString(PlayerName);
		pooledWriter.WriteString(levelandrank);
		pooledWriter.WriteString(steamid);
		base.SendObserversRpc(5U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000138 RID: 312 RVA: 0x00006FD7 File Offset: 0x000051D7
	private void RpcLogic___ObsCallJoinHats_852232071(string PlayerName, string levelandrank, string steamid)
	{
		if (base.HasAuthority)
		{
			GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MainMenuManager>().SyncHats(PlayerName, levelandrank, steamid);
		}
	}

	// Token: 0x06000139 RID: 313 RVA: 0x00006FF8 File Offset: 0x000051F8
	private void RpcReader___Observers_ObsCallJoinHats_852232071(PooledReader PooledReader0, Channel channel)
	{
		string text = PooledReader0.ReadString();
		string text2 = PooledReader0.ReadString();
		string text3 = PooledReader0.ReadString();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsCallJoinHats_852232071(text, text2, text3);
	}

	// Token: 0x0600013A RID: 314 RVA: 0x0000704C File Offset: 0x0000524C
	private void RpcWriter___Server_ServerSyncPlayerNames_3821904405(string[] PlayerNames, string[] PlayerRanks)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.GWrite___System.String[]FishNet.Serializing.Generated(PlayerNames);
		pooledWriter.GWrite___System.String[]FishNet.Serializing.Generated(PlayerRanks);
		base.SendServerRpc(6U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600013B RID: 315 RVA: 0x000070CB File Offset: 0x000052CB
	private void RpcLogic___ServerSyncPlayerNames_3821904405(string[] PlayerNames, string[] PlayerRanks)
	{
		this.ObsSyncPlayerNames(PlayerNames, PlayerRanks);
	}

	// Token: 0x0600013C RID: 316 RVA: 0x000070D8 File Offset: 0x000052D8
	private void RpcReader___Server_ServerSyncPlayerNames_3821904405(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		string[] array = FishNet.Serializing.Generated.GeneratedReaders___Internal.GRead___System.String[]FishNet.Serializing.Generateds(PooledReader0);
		string[] array2 = FishNet.Serializing.Generated.GeneratedReaders___Internal.GRead___System.String[]FishNet.Serializing.Generateds(PooledReader0);
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSyncPlayerNames_3821904405(array, array2);
	}

	// Token: 0x0600013D RID: 317 RVA: 0x0000711C File Offset: 0x0000531C
	private void RpcWriter___Observers_ObsSyncPlayerNames_3821904405(string[] PlayerNames, string[] PlayerRanks)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.GWrite___System.String[]FishNet.Serializing.Generated(PlayerNames);
		pooledWriter.GWrite___System.String[]FishNet.Serializing.Generated(PlayerRanks);
		base.SendObserversRpc(7U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600013E RID: 318 RVA: 0x000071AA File Offset: 0x000053AA
	private void RpcLogic___ObsSyncPlayerNames_3821904405(string[] PlayerNames, string[] PlayerRanks)
	{
		if (!base.HasAuthority)
		{
			GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MainMenuManager>().SyncUpdateNames(PlayerNames, PlayerRanks);
		}
	}

	// Token: 0x0600013F RID: 319 RVA: 0x000071CC File Offset: 0x000053CC
	private void RpcReader___Observers_ObsSyncPlayerNames_3821904405(PooledReader PooledReader0, Channel channel)
	{
		string[] array = FishNet.Serializing.Generated.GeneratedReaders___Internal.GRead___System.String[]FishNet.Serializing.Generateds(PooledReader0);
		string[] array2 = FishNet.Serializing.Generated.GeneratedReaders___Internal.GRead___System.String[]FishNet.Serializing.Generateds(PooledReader0);
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsSyncPlayerNames_3821904405(array, array2);
	}

	// Token: 0x06000140 RID: 320 RVA: 0x00007210 File Offset: 0x00005410
	private void RpcWriter___Server_kickPlayerServer_3615296227(string steamid)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteString(steamid);
		base.SendServerRpc(8U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000141 RID: 321 RVA: 0x00007282 File Offset: 0x00005482
	private void RpcLogic___kickPlayerServer_3615296227(string steamid)
	{
		this.ObsKickPlayer(steamid);
	}

	// Token: 0x06000142 RID: 322 RVA: 0x0000728C File Offset: 0x0000548C
	private void RpcReader___Server_kickPlayerServer_3615296227(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		string text = PooledReader0.ReadString();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___kickPlayerServer_3615296227(text);
	}

	// Token: 0x06000143 RID: 323 RVA: 0x000072C0 File Offset: 0x000054C0
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
		base.SendObserversRpc(9U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000144 RID: 324 RVA: 0x00007341 File Offset: 0x00005541
	private void RpcLogic___ObsKickPlayer_3615296227(string steamid)
	{
		GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MainMenuManager>().ActKickPlayer(steamid);
	}

	// Token: 0x06000145 RID: 325 RVA: 0x00007358 File Offset: 0x00005558
	private void RpcReader___Observers_ObsKickPlayer_3615296227(PooledReader PooledReader0, Channel channel)
	{
		string text = PooledReader0.ReadString();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsKickPlayer_3615296227(text);
	}

	// Token: 0x06000146 RID: 326 RVA: 0x0000738C File Offset: 0x0000558C
	private void RpcWriter___Server_ServerDisbandLobby_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(10U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000147 RID: 327 RVA: 0x000073F1 File Offset: 0x000055F1
	private void RpcLogic___ServerDisbandLobby_2166136261()
	{
		this.ObsDisband();
	}

	// Token: 0x06000148 RID: 328 RVA: 0x000073FC File Offset: 0x000055FC
	private void RpcReader___Server_ServerDisbandLobby_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerDisbandLobby_2166136261();
	}

	// Token: 0x06000149 RID: 329 RVA: 0x0000741C File Offset: 0x0000561C
	private void RpcWriter___Observers_ObsDisband_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(11U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600014A RID: 330 RVA: 0x00007490 File Offset: 0x00005690
	private void RpcLogic___ObsDisband_2166136261()
	{
		GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MainMenuManager>().DisbandLobby();
	}

	// Token: 0x0600014B RID: 331 RVA: 0x000074A8 File Offset: 0x000056A8
	private void RpcReader___Observers_ObsDisband_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsDisband_2166136261();
	}

	// Token: 0x0600014C RID: 332 RVA: 0x000074C8 File Offset: 0x000056C8
	private void Awake_UserLogic_BootstrapNetworkManager_Assembly-CSharp.dll()
	{
		BootstrapNetworkManager.instance = this;
	}

	// Token: 0x04000098 RID: 152
	private static BootstrapNetworkManager instance;

	// Token: 0x04000099 RID: 153
	private float cooldown;

	// Token: 0x0400009A RID: 154
	private bool NetworkInitialize___EarlyBootstrapNetworkManagerAssembly-CSharp.dll_Excuted;

	// Token: 0x0400009B RID: 155
	private bool NetworkInitialize__LateBootstrapNetworkManagerAssembly-CSharp.dll_Excuted;

	// Token: 0x0200001C RID: 28
	private enum WizardRank
	{
		// Token: 0x0400009D RID: 157
		Lackey,
		// Token: 0x0400009E RID: 158
		Sputterer,
		// Token: 0x0400009F RID: 159
		Novice,
		// Token: 0x040000A0 RID: 160
		Apprentice,
		// Token: 0x040000A1 RID: 161
		Savant,
		// Token: 0x040000A2 RID: 162
		Master,
		// Token: 0x040000A3 RID: 163
		Grand_Master,
		// Token: 0x040000A4 RID: 164
		Archmagus,
		// Token: 0x040000A5 RID: 165
		Magus_Prime,
		// Token: 0x040000A6 RID: 166
		Supreme_Archmagus
	}
}
