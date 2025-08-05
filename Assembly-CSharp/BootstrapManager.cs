using System;
using System.Collections;
using System.Collections.Generic;
using Dissonance;
using Dissonance.Integrations.FishNet;
using FishNet.Managing;
using FishySteamworks;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000015 RID: 21
public class BootstrapManager : MonoBehaviour
{
	// Token: 0x17000013 RID: 19
	// (get) Token: 0x060000D5 RID: 213 RVA: 0x00005811 File Offset: 0x00003A11
	// (set) Token: 0x060000D6 RID: 214 RVA: 0x00005818 File Offset: 0x00003A18
	public static BootstrapManager instance { get; private set; }

	// Token: 0x060000D7 RID: 215 RVA: 0x00005820 File Offset: 0x00003A20
	private void Awake()
	{
		if (BootstrapManager.instance == null)
		{
			BootstrapManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x0000584C File Offset: 0x00003A4C
	public void HostPlayAgain()
	{
		this.isPlayAgain = true;
		this.is1v1 = false;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		base.StartCoroutine(this.HostLeaveAndJoinNewLobby());
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x00005875 File Offset: 0x00003A75
	private IEnumerator HostLeaveAndJoinNewLobby()
	{
		this.hasLeaveGameFinished = false;
		BootstrapManager.LeaveLobby2();
		while (!this.hasLeaveGameFinished)
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.4f);
		BootstrapManager.CreateLobby(false);
		GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MainMenuManager>().HostPlayAgain();
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		yield break;
	}

	// Token: 0x060000DA RID: 218 RVA: 0x00005884 File Offset: 0x00003A84
	public void ClientPlayAgain()
	{
		if (this.lobbyIDs.Count > 0)
		{
			this.lobbyIDs.Clear();
		}
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		this.is1v1 = false;
		this.isPlayAgain = true;
		SteamMatchmaking.AddRequestLobbyListStringFilter("HostAddress", this.PreviousHostsID, ELobbyComparison.k_ELobbyComparisonEqual);
		SteamMatchmaking.RequestLobbyList();
		base.StartCoroutine(this.WaitforLobbyIDs());
	}

	// Token: 0x060000DB RID: 219 RVA: 0x000058E8 File Offset: 0x00003AE8
	private IEnumerator WaitforLobbyIDs()
	{
		float timeout = 0f;
		while (this.lobbyIDs.Count == 0 && timeout < 15f)
		{
			timeout += 1f;
			SteamMatchmaking.AddRequestLobbyListStringFilter("HostAddress", this.PreviousHostsID, ELobbyComparison.k_ELobbyComparisonEqual);
			SteamMatchmaking.RequestLobbyList();
			yield return new WaitForSeconds(1f);
		}
		foreach (CSteamID csteamID in this.lobbyIDs)
		{
			if (csteamID != new CSteamID(BootstrapManager.CurrentLobbyID))
			{
				base.StartCoroutine(this.LeaveAndJoinNewLobby(csteamID));
				break;
			}
		}
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		yield break;
	}

	// Token: 0x060000DC RID: 220 RVA: 0x000058F7 File Offset: 0x00003AF7
	private IEnumerator LeaveAndJoinNewLobby(CSteamID lobbyid)
	{
		this.hasLeaveGameFinished = false;
		BootstrapManager.LeaveLobby2();
		while (!this.hasLeaveGameFinished)
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.4f);
		BootstrapManager.JoinByID(lobbyid);
		yield break;
	}

	// Token: 0x060000DD RID: 221 RVA: 0x00005910 File Offset: 0x00003B10
	private void Start()
	{
		this.LobbyCreated = Callback<LobbyCreated_t>.Create(new Callback<LobbyCreated_t>.DispatchDelegate(this.OnLobbyCreated));
		this.JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(new Callback<GameLobbyJoinRequested_t>.DispatchDelegate(this.OnJoinRequest));
		this.LobbyEntered = Callback<LobbyEnter_t>.Create(new Callback<LobbyEnter_t>.DispatchDelegate(this.OnLobbyEntered));
		this.LobbyList = Callback<LobbyMatchList_t>.Create(new Callback<LobbyMatchList_t>.DispatchDelegate(this.OnGetLobbyList));
		this.LobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(new Callback<LobbyDataUpdate_t>.DispatchDelegate(this.OnGetLobbyData));
		this.lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(new Callback<LobbyChatUpdate_t>.DispatchDelegate(this.OnLobbyChatUpdate));
	}

	// Token: 0x060000DE RID: 222 RVA: 0x000059A8 File Offset: 0x00003BA8
	public void GetLobbiesList()
	{
		if (this.lobbyIDs.Count > 0)
		{
			this.lobbyIDs.Clear();
		}
		SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterClose);
		SteamMatchmaking.AddRequestLobbyListResultCountFilter(60);
		string version = Application.version;
		SteamMatchmaking.AddRequestLobbyListStringFilter("Version", version, ELobbyComparison.k_ELobbyComparisonEqual);
		SteamMatchmaking.AddRequestLobbyListStringFilter("closed", "0", ELobbyComparison.k_ELobbyComparisonEqual);
		SteamMatchmaking.RequestLobbyList();
	}

	// Token: 0x060000DF RID: 223 RVA: 0x00005A03 File Offset: 0x00003C03
	public void ChangeLobbySize(int size)
	{
		if (BootstrapManager.CurrentLobbyID != 0UL)
		{
			SteamMatchmaking.SetLobbyMemberLimit(new CSteamID(BootstrapManager.CurrentLobbyID), size);
		}
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x00005A20 File Offset: 0x00003C20
	public void CloseLobby()
	{
		if (BootstrapManager.CurrentLobbyID != 0UL)
		{
			SteamMatchmaking.SetLobbyType(new CSteamID(BootstrapManager.CurrentLobbyID), ELobbyType.k_ELobbyTypePrivate);
			SteamMatchmaking.SetLobbyMemberLimit(new CSteamID(BootstrapManager.CurrentLobbyID), 1);
			SteamMatchmaking.SetLobbyData(new CSteamID(BootstrapManager.CurrentLobbyID), "closed", "1");
			SteamMatchmaking.SetLobbyJoinable(new CSteamID(BootstrapManager.CurrentLobbyID), false);
		}
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x00005A84 File Offset: 0x00003C84
	private void OnGetLobbyList(LobbyMatchList_t result)
	{
		if (FindPublicLobbies.instance.listOfLobbies.Count > 0)
		{
			FindPublicLobbies.instance.DestroyLobbies();
		}
		int num = 0;
		while ((long)num < (long)((ulong)result.m_nLobbiesMatching))
		{
			CSteamID lobbyByIndex = SteamMatchmaking.GetLobbyByIndex(num);
			if ((SteamMatchmaking.GetLobbyMemberLimit(lobbyByIndex) == 2 && this.SearchForDMOnly) || (SteamMatchmaking.GetLobbyMemberLimit(lobbyByIndex) == 8 && !this.SearchForDMOnly))
			{
				this.lobbyIDs.Add(lobbyByIndex);
				SteamMatchmaking.RequestLobbyData(lobbyByIndex);
			}
			num++;
		}
	}

	// Token: 0x060000E2 RID: 226 RVA: 0x00005AFC File Offset: 0x00003CFC
	public void GoToMenu()
	{
		SceneManager.LoadScene(this.menuName, LoadSceneMode.Additive);
	}

	// Token: 0x060000E3 RID: 227 RVA: 0x00005B0A File Offset: 0x00003D0A
	public void ClearLocalPlayers()
	{
		this.LocalPlayers = new List<CSteamID>();
	}

	// Token: 0x060000E4 RID: 228 RVA: 0x00005B18 File Offset: 0x00003D18
	public static void CreateLobby(bool isPrivate)
	{
		if (BootstrapManager.instance.isPlayAgain)
		{
			if (BootstrapManager.instance._fishySteamworks == null)
			{
				BootstrapManager.instance._fishySteamworks = Object.FindFirstObjectByType<global::FishySteamworks.FishySteamworks>();
			}
			if (BootstrapManager.instance._networkManager == null)
			{
				BootstrapManager.instance._networkManager = Object.FindFirstObjectByType<NetworkManager>();
			}
			if (BootstrapManager.instance._networkManager.IsServerStarted)
			{
				BootstrapManager.instance._networkManager.ServerManager.StopConnection(true);
			}
			if (BootstrapManager.instance._networkManager.IsClientStarted)
			{
				BootstrapManager.instance._networkManager.ClientManager.StopConnection();
			}
			Debug.Log("FishySteamworks: " + Object.FindObjectsByType<global::FishySteamworks.FishySteamworks>(FindObjectsSortMode.None).Length.ToString());
			Debug.Log("NetworkManagers: " + Object.FindObjectsByType<NetworkManager>(FindObjectsSortMode.None).Length.ToString());
			Debug.Log("DissonanceComms: " + Object.FindObjectsByType<DissonanceComms>(FindObjectsSortMode.None).Length.ToString());
			Debug.Log("DissonanceFishNetComms: " + Object.FindObjectsByType<DissonanceFishNetComms>(FindObjectsSortMode.None).Length.ToString());
			BootstrapManager.instance.islobbyprivate = true;
			SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 8);
			return;
		}
		if (isPrivate)
		{
			if (BootstrapManager.instance._fishySteamworks == null)
			{
				BootstrapManager.instance._fishySteamworks = Object.FindFirstObjectByType<global::FishySteamworks.FishySteamworks>();
			}
			if (BootstrapManager.instance._networkManager == null)
			{
				BootstrapManager.instance._networkManager = Object.FindFirstObjectByType<NetworkManager>();
			}
			if (BootstrapManager.instance._networkManager.IsServerStarted)
			{
				BootstrapManager.instance._networkManager.ServerManager.StopConnection(true);
			}
			if (BootstrapManager.instance._networkManager.IsClientStarted)
			{
				BootstrapManager.instance._networkManager.ClientManager.StopConnection();
			}
			Debug.Log("FishySteamworks: " + Object.FindObjectsByType<global::FishySteamworks.FishySteamworks>(FindObjectsSortMode.None).Length.ToString());
			Debug.Log("NetworkManagers: " + Object.FindObjectsByType<NetworkManager>(FindObjectsSortMode.None).Length.ToString());
			Debug.Log("DissonanceComms: " + Object.FindObjectsByType<DissonanceComms>(FindObjectsSortMode.None).Length.ToString());
			Debug.Log("DissonanceFishNetComms: " + Object.FindObjectsByType<DissonanceFishNetComms>(FindObjectsSortMode.None).Length.ToString());
			BootstrapManager.instance.islobbyprivate = true;
			SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePrivate, 8);
			return;
		}
		if (BootstrapManager.instance._fishySteamworks == null)
		{
			BootstrapManager.instance._fishySteamworks = Object.FindFirstObjectByType<global::FishySteamworks.FishySteamworks>();
		}
		if (BootstrapManager.instance._networkManager == null)
		{
			BootstrapManager.instance._networkManager = Object.FindFirstObjectByType<NetworkManager>();
		}
		if (BootstrapManager.instance._networkManager.IsServerStarted)
		{
			BootstrapManager.instance._networkManager.ServerManager.StopConnection(true);
		}
		if (BootstrapManager.instance._networkManager.IsClientStarted)
		{
			BootstrapManager.instance._networkManager.ClientManager.StopConnection();
		}
		Debug.Log("FishySteamworks: " + Object.FindObjectsByType<global::FishySteamworks.FishySteamworks>(FindObjectsSortMode.None).Length.ToString());
		Debug.Log("NetworkManagers: " + Object.FindObjectsByType<NetworkManager>(FindObjectsSortMode.None).Length.ToString());
		Debug.Log("DissonanceComms: " + Object.FindObjectsByType<DissonanceComms>(FindObjectsSortMode.None).Length.ToString());
		Debug.Log("DissonanceFishNetComms: " + Object.FindObjectsByType<DissonanceFishNetComms>(FindObjectsSortMode.None).Length.ToString());
		BootstrapManager.instance.islobbyprivate = false;
		SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 8);
	}

	// Token: 0x060000E5 RID: 229 RVA: 0x00005E98 File Offset: 0x00004098
	private void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
	{
		BootstrapManager.CurrentLobbyID = callback.m_ulSteamIDLobby;
		if (SteamMatchmaking.GetLobbyOwner(new CSteamID(BootstrapManager.CurrentLobbyID)) == SteamUser.GetSteamID())
		{
			EChatMemberStateChange rgfChatMemberStateChange = (EChatMemberStateChange)callback.m_rgfChatMemberStateChange;
			if (rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeLeft || rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeDisconnected)
			{
				string friendPersonaName = SteamFriends.GetFriendPersonaName((CSteamID)callback.m_ulSteamIDUserChanged);
				GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MainMenuManager>().RemoveHat(friendPersonaName);
				return;
			}
			if (rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeEntered)
			{
				CSteamID csteamID = (CSteamID)callback.m_ulSteamIDUserChanged;
				Debug.Log("Player Joining:");
				Debug.Log(csteamID);
				Debug.Log(SteamFriends.GetFriendPersonaName(csteamID));
				Debug.Log("Player Joined");
			}
		}
	}

	// Token: 0x060000E6 RID: 230 RVA: 0x00005F3C File Offset: 0x0000413C
	private void OnLobbyCreated(LobbyCreated_t callback)
	{
		Debug.Log("Starting lobby creation: " + callback.m_eResult.ToString());
		if (callback.m_eResult != EResult.k_EResultOK)
		{
			return;
		}
		this.haslobbyStarted = false;
		BootstrapManager.CurrentLobbyID = callback.m_ulSteamIDLobby;
		SteamMatchmaking.SetLobbyData(new CSteamID(BootstrapManager.CurrentLobbyID), "HostAddress", SteamUser.GetSteamID().ToString());
		string version = Application.version;
		SteamMatchmaking.SetLobbyData(new CSteamID(BootstrapManager.CurrentLobbyID), "Version", version);
		string lobbyData = SteamMatchmaking.GetLobbyData(new CSteamID(BootstrapManager.CurrentLobbyID), "Version");
		Debug.Log("Set version: " + version + " | Confirmed: " + lobbyData);
		Debug.Log(version);
		SteamMatchmaking.SetLobbyData(new CSteamID(BootstrapManager.CurrentLobbyID), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
		if (BootstrapManager.instance.islobbyprivate)
		{
			SteamMatchmaking.SetLobbyData(new CSteamID(BootstrapManager.CurrentLobbyID), "closed", "2");
		}
		else
		{
			SteamMatchmaking.SetLobbyData(new CSteamID(BootstrapManager.CurrentLobbyID), "closed", "0");
		}
		this._fishySteamworks.SetClientAddress(SteamUser.GetSteamID().ToString());
		this._fishySteamworks.StartConnection(true);
	}

	// Token: 0x060000E7 RID: 231 RVA: 0x00006091 File Offset: 0x00004291
	private void OnJoinRequest(GameLobbyJoinRequested_t callback)
	{
		SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x0000609F File Offset: 0x0000429F
	private void OnLobbyEntered(LobbyEnter_t callback)
	{
		BootstrapManager.CurrentLobbyID = callback.m_ulSteamIDLobby;
		this.TryingToJoin = true;
		SteamMatchmaking.RequestLobbyData(new CSteamID(BootstrapManager.CurrentLobbyID));
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x000060C4 File Offset: 0x000042C4
	private void OnGetLobbyData(LobbyDataUpdate_t result)
	{
		FindPublicLobbies.instance.DisplayLobbies(this.lobbyIDs, result);
		if (this.TryingToJoin)
		{
			this.TryingToJoin = false;
			string version = Application.version;
			Debug.Log(version);
			Debug.Log(SteamMatchmaking.GetLobbyData(new CSteamID(BootstrapManager.CurrentLobbyID), "Version"));
			Debug.Log(SteamMatchmaking.GetNumLobbyMembers(new CSteamID(BootstrapManager.CurrentLobbyID)));
			string lobbyData = SteamMatchmaking.GetLobbyData(new CSteamID(BootstrapManager.CurrentLobbyID), "closed");
			if (SteamMatchmaking.GetLobbyData(new CSteamID(BootstrapManager.CurrentLobbyID), "Version") != version)
			{
				Debug.Log("wrong version");
				base.StartCoroutine(this.WaitForDelay());
				return;
			}
			if (lobbyData == "1")
			{
				Debug.Log("lobby started");
				base.StartCoroutine(this.WaitForDelay());
				return;
			}
			if (SteamMatchmaking.GetNumLobbyMembers(new CSteamID(BootstrapManager.CurrentLobbyID)) > 8)
			{
				Debug.Log("too many peopl");
				base.StartCoroutine(this.WaitForDelay());
				return;
			}
			MainMenuManager.LobbyEntered(SteamMatchmaking.GetLobbyData(new CSteamID(BootstrapManager.CurrentLobbyID), "name"), this._networkManager.IsServerStarted, BootstrapManager.CurrentLobbyID);
			this._fishySteamworks.SetClientAddress(SteamMatchmaking.GetLobbyData(new CSteamID(BootstrapManager.CurrentLobbyID), "HostAddress"));
			this.PreviousHostsID = SteamMatchmaking.GetLobbyData(new CSteamID(BootstrapManager.CurrentLobbyID), "HostAddress");
			this._fishySteamworks.StartConnection(false);
		}
	}

	// Token: 0x060000EA RID: 234 RVA: 0x00006239 File Offset: 0x00004439
	private IEnumerator WaitForDelay()
	{
		yield return new WaitForSeconds(0.5f);
		MainMenuManager mainMenuManager = Object.FindFirstObjectByType<MainMenuManager>();
		mainMenuManager.toggleVersionError("The lobby is full or has already started!");
		mainMenuManager.KickPlayer(SteamUser.GetSteamID().ToString());
		yield break;
	}

	// Token: 0x060000EB RID: 235 RVA: 0x00006241 File Offset: 0x00004441
	public static void JoinByID(CSteamID steamID)
	{
		if (Time.time - BootstrapManager.instance.joincd > 2f)
		{
			BootstrapManager.instance.joincd = Time.time;
			SteamMatchmaking.JoinLobby(steamID);
		}
	}

	// Token: 0x060000EC RID: 236 RVA: 0x00006270 File Offset: 0x00004470
	public static void LeaveLobby()
	{
		SteamMatchmaking.LeaveLobby(new CSteamID(BootstrapManager.CurrentLobbyID));
		BootstrapManager.CurrentLobbyID = 0UL;
		if (BootstrapManager.instance._fishySteamworks != null && BootstrapManager.instance._networkManager != null)
		{
			if (BootstrapManager.instance._networkManager.IsClientStarted)
			{
				BootstrapManager.instance._fishySteamworks.StopConnection(false);
			}
			if (BootstrapManager.instance._networkManager.IsServerStarted)
			{
				BootstrapManager.instance._fishySteamworks.StopConnection(true);
			}
		}
	}

	// Token: 0x060000ED RID: 237 RVA: 0x000062FB File Offset: 0x000044FB
	public static void LeaveLobby2()
	{
		BootstrapManager.instance.StartCoroutine(BootstrapManager.ChangeSceneAfterCleanup("Bootstrap"));
	}

	// Token: 0x060000EE RID: 238 RVA: 0x00006312 File Offset: 0x00004512
	private static IEnumerator ChangeSceneAfterCleanup(string sceneName)
	{
		Object.FindFirstObjectByType<DissonanceFishNetComms>().stopit();
		yield return null;
		DissonanceComms dissonanceComms = Object.FindFirstObjectByType<DissonanceComms>();
		if (dissonanceComms != null)
		{
			Object.Destroy(dissonanceComms.gameObject);
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("playbackprefab");
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i]);
		}
		yield return null;
		BootstrapManager.instance._fishySteamworks.StopConnection(false);
		if (BootstrapManager.instance._networkManager.IsServerStarted)
		{
			BootstrapManager.instance._fishySteamworks.StopConnection(true);
		}
		SteamMatchmaking.LeaveLobby(new CSteamID(BootstrapManager.CurrentLobbyID));
		BootstrapManager.CurrentLobbyID = 0UL;
		yield return null;
		SceneManager.LoadScene(sceneName);
		BootstrapManager.instance.GoToMenu();
		BootstrapManager.instance._fishySteamworks = Object.FindFirstObjectByType<global::FishySteamworks.FishySteamworks>();
		BootstrapManager.instance._networkManager = Object.FindFirstObjectByType<NetworkManager>();
		if (BootstrapManager.instance._networkManager.IsServerStarted)
		{
			BootstrapManager.instance._networkManager.ServerManager.StopConnection(true);
		}
		if (BootstrapManager.instance._networkManager.IsClientStarted)
		{
			BootstrapManager.instance._networkManager.ClientManager.StopConnection();
		}
		BootstrapManager.instance.hasLeaveGameFinished = true;
		yield break;
	}

	// Token: 0x04000071 RID: 113
	[SerializeField]
	private string menuName = "GameScene";

	// Token: 0x04000072 RID: 114
	[SerializeField]
	private NetworkManager _networkManager;

	// Token: 0x04000073 RID: 115
	[SerializeField]
	private global::FishySteamworks.FishySteamworks _fishySteamworks;

	// Token: 0x04000074 RID: 116
	protected Callback<LobbyCreated_t> LobbyCreated;

	// Token: 0x04000075 RID: 117
	protected Callback<GameLobbyJoinRequested_t> JoinRequest;

	// Token: 0x04000076 RID: 118
	protected Callback<LobbyEnter_t> LobbyEntered;

	// Token: 0x04000077 RID: 119
	protected Callback<LobbyMatchList_t> LobbyList;

	// Token: 0x04000078 RID: 120
	protected Callback<LobbyDataUpdate_t> LobbyDataUpdated;

	// Token: 0x04000079 RID: 121
	protected Callback<LobbyChatUpdate_t> lobbyChatUpdate;

	// Token: 0x0400007A RID: 122
	public List<CSteamID> lobbyIDs = new List<CSteamID>();

	// Token: 0x0400007B RID: 123
	public int currentTeam = 2;

	// Token: 0x0400007C RID: 124
	public static ulong CurrentLobbyID;

	// Token: 0x0400007D RID: 125
	public string PreviousHostsID;

	// Token: 0x0400007E RID: 126
	private bool hasLeaveGameFinished;

	// Token: 0x0400007F RID: 127
	private bool haslobbyStarted;

	// Token: 0x04000080 RID: 128
	public bool isPlayAgain;

	// Token: 0x04000081 RID: 129
	public bool is1v1;

	// Token: 0x04000082 RID: 130
	public bool SearchForDMOnly;

	// Token: 0x04000083 RID: 131
	private List<CSteamID> LocalPlayers = new List<CSteamID>();

	// Token: 0x04000084 RID: 132
	private bool islobbyprivate;

	// Token: 0x04000085 RID: 133
	private bool TryingToJoin;

	// Token: 0x04000086 RID: 134
	private float joincd;

	// Token: 0x04000087 RID: 135
	internal CSteamID[] listOfLobbies = new CSteamID[]
	{
		new CSteamID(76561199814425300UL),
		new CSteamID(76561199800074067UL)
	};
}
