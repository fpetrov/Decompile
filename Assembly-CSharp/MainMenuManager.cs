using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Dissonance;
using Dissonance.Integrations.FishNet;
using DrawPixels_Done;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

// Token: 0x020000D3 RID: 211
public class MainMenuManager : MonoBehaviour
{
	// Token: 0x06000856 RID: 2134 RVA: 0x00020574 File Offset: 0x0001E774
	private void Awake()
	{
		MainMenuManager.instance = this;
	}

	// Token: 0x06000857 RID: 2135 RVA: 0x0002057C File Offset: 0x0001E77C
	private void Start()
	{
		Screen.SetResolution(1920, 1080, true);
		this.m_LobbyChatMsg = Callback<LobbyChatMsg_t>.Create(new Callback<LobbyChatMsg_t>.DispatchDelegate(this.OnLobbyChatMsg));
		SettingsHolder.Instance.SetSettings(this.Settings);
		this.OpenMainMenu();
		this.playerNames = new string[8];
		this.playerLevelandRanks = new string[8];
		this.chatInputFieldText.onSubmit.AddListener(new UnityAction<string>(this.OnSubmit));
		for (int i = 0; i < this.playerNames.Length; i++)
		{
			this.playerNames[i] = null;
			this.hats[i].SetActive(false);
			this.texts[i * 2].text = "";
			this.texts[i * 2 + 1].text = "";
		}
		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			if (assembly.FullName.Contains("BepInEx") || assembly.FullName.Contains("Harmony") || assembly.FullName.Contains("Melon"))
			{
				SteamUserStats.SetStat("modding", 1);
			}
		}
	}

	// Token: 0x06000858 RID: 2136 RVA: 0x000206AD File Offset: 0x0001E8AD
	private void OnDestroy()
	{
		if (this.m_LobbyChatMsg != null)
		{
			this.m_LobbyChatMsg.Dispose();
			this.m_LobbyChatMsg = null;
		}
	}

	// Token: 0x06000859 RID: 2137 RVA: 0x000206CC File Offset: 0x0001E8CC
	public void SendLobbyChatMessage()
	{
		if (Time.time - this.chatcd > 0.1f && !string.IsNullOrWhiteSpace(this.chatInputFieldText.text))
		{
			this.chatcd = Time.time;
			byte[] bytes = Encoding.UTF8.GetBytes(this.chatInputFieldText.text);
			SteamMatchmaking.SendLobbyChatMsg(new CSteamID(BootstrapManager.CurrentLobbyID), bytes, bytes.Length);
			this.chatInputFieldText.text = "";
		}
	}

	// Token: 0x0600085A RID: 2138 RVA: 0x00020743 File Offset: 0x0001E943
	private void OnSubmit(string text)
	{
		this.SendLobbyChatMessage();
	}

	// Token: 0x0600085B RID: 2139 RVA: 0x0002074C File Offset: 0x0001E94C
	private void OnLobbyChatMsg(LobbyChatMsg_t callback)
	{
		byte[] array = new byte[4096];
		CSteamID csteamID;
		EChatEntryType echatEntryType;
		int lobbyChatEntry = SteamMatchmaking.GetLobbyChatEntry((CSteamID)callback.m_ulSteamIDLobby, (int)callback.m_iChatID, out csteamID, array, array.Length, out echatEntryType);
		string @string = Encoding.UTF8.GetString(array, 0, lobbyChatEntry);
		Debug.Log("[" + SteamFriends.GetFriendPersonaName(csteamID) + "] says: " + @string);
		GameObject gameObject = Object.Instantiate<GameObject>(this.ChatMessagePrefab);
		gameObject.GetComponent<SetChatMessage>().msgtext.text = "[" + SteamFriends.GetFriendPersonaName(csteamID) + "]: " + @string;
		gameObject.transform.SetParent(this.ChatListContent.transform);
		gameObject.transform.localScale = Vector3.one;
		Canvas.ForceUpdateCanvases();
		this.scrollRect.verticalNormalizedPosition = 0f;
		this.ChatMessages.Add(gameObject);
	}

	// Token: 0x0600085C RID: 2140 RVA: 0x000021EF File Offset: 0x000003EF
	public void SendMessage()
	{
	}

	// Token: 0x0600085D RID: 2141 RVA: 0x0002082D File Offset: 0x0001EA2D
	public void openkickplayersmenu()
	{
		this.kickplayersmenu.SetActive(true);
	}

	// Token: 0x0600085E RID: 2142 RVA: 0x0002083B File Offset: 0x0001EA3B
	public void closekickplayersmenu()
	{
		this.kickplayersmenu.SetActive(false);
	}

	// Token: 0x0600085F RID: 2143 RVA: 0x00020849 File Offset: 0x0001EA49
	public void OpenAccentMenu()
	{
		this.AccentMenu.SetActive(true);
	}

	// Token: 0x06000860 RID: 2144 RVA: 0x00020857 File Offset: 0x0001EA57
	public void CloseAccentMenu()
	{
		this.AccentMenu.SetActive(false);
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x00020868 File Offset: 0x0001EA68
	public void SetAccentBritish()
	{
		GameObject[] accentsCheckMarks = this.AccentsCheckMarks;
		for (int i = 0; i < accentsCheckMarks.Length; i++)
		{
			accentsCheckMarks[i].SetActive(false);
		}
		this.AccentsCheckMarks[1].SetActive(true);
		SettingsHolder.Instance.SetVoiceModelPath("LanguageModels/vosk-model-small-en-gb-0.15");
	}

	// Token: 0x06000862 RID: 2146 RVA: 0x000208B0 File Offset: 0x0001EAB0
	public void SetAccentIndian()
	{
		GameObject[] accentsCheckMarks = this.AccentsCheckMarks;
		for (int i = 0; i < accentsCheckMarks.Length; i++)
		{
			accentsCheckMarks[i].SetActive(false);
		}
		this.AccentsCheckMarks[2].SetActive(true);
		SettingsHolder.Instance.SetVoiceModelPath("LanguageModels/vosk-model-small-en-in-0.4");
	}

	// Token: 0x06000863 RID: 2147 RVA: 0x000208F8 File Offset: 0x0001EAF8
	public void SetAccentDefault()
	{
		GameObject[] accentsCheckMarks = this.AccentsCheckMarks;
		for (int i = 0; i < accentsCheckMarks.Length; i++)
		{
			accentsCheckMarks[i].SetActive(false);
		}
		this.AccentsCheckMarks[0].SetActive(true);
		SettingsHolder.Instance.SetVoiceModelPath("LanguageModels/vosk-model-small-en-us-0.15");
	}

	// Token: 0x06000864 RID: 2148 RVA: 0x00020940 File Offset: 0x0001EB40
	private void Update()
	{
		if (this.GameHasStarted && Input.GetKeyDown(KeyCode.Escape))
		{
			this.ToggleInGameMenu();
		}
	}

	// Token: 0x06000865 RID: 2149 RVA: 0x00020959 File Offset: 0x0001EB59
	public void SearchLobbyTypeDM()
	{
		BootstrapManager.instance.SearchForDMOnly = true;
		this.Filterdmctfchecks[1].SetActive(true);
		this.Filterdmctfchecks[0].SetActive(false);
	}

	// Token: 0x06000866 RID: 2150 RVA: 0x00020982 File Offset: 0x0001EB82
	public void SearchLobbyTypeCTF()
	{
		BootstrapManager.instance.SearchForDMOnly = false;
		this.Filterdmctfchecks[0].SetActive(true);
		this.Filterdmctfchecks[1].SetActive(false);
	}

	// Token: 0x06000867 RID: 2151 RVA: 0x000209AB File Offset: 0x0001EBAB
	public void Coloseum()
	{
		this.checkmarks[0].SetActive(false);
		this.checkmarks[1].SetActive(false);
		this.checkmarks[2].SetActive(true);
		this.mapinfo = 2;
		BootstrapManager.instance.ChangeLobbySize(2);
	}

	// Token: 0x06000868 RID: 2152 RVA: 0x000209E9 File Offset: 0x0001EBE9
	public void SmallMap()
	{
		this.checkmarks[0].SetActive(true);
		this.checkmarks[1].SetActive(false);
		this.checkmarks[2].SetActive(false);
		this.mapinfo = 1;
		BootstrapManager.instance.ChangeLobbySize(8);
	}

	// Token: 0x06000869 RID: 2153 RVA: 0x00020A27 File Offset: 0x0001EC27
	public void LargeMap()
	{
		this.checkmarks[0].SetActive(false);
		this.checkmarks[1].SetActive(true);
		this.checkmarks[2].SetActive(false);
		this.mapinfo = 0;
		BootstrapManager.instance.ChangeLobbySize(8);
	}

	// Token: 0x0600086A RID: 2154 RVA: 0x00020A65 File Offset: 0x0001EC65
	public void ShowCredits()
	{
		this.Credits.SetActive(true);
	}

	// Token: 0x0600086B RID: 2155 RVA: 0x00020A73 File Offset: 0x0001EC73
	public void StopCredits()
	{
		this.Credits.SetActive(false);
	}

	// Token: 0x0600086C RID: 2156 RVA: 0x00020A84 File Offset: 0x0001EC84
	public void ChangeTeamText(int team, int index, string name, string lvlandrank)
	{
		if (team == 0)
		{
			this.team1[index].text = this.ClampString(name, 10);
			lvlandrank = lvlandrank.Replace("_", " ");
			this.team1rankandleveltext[index].text = lvlandrank;
			if (lvlandrank.Contains("Lackey"))
			{
				this.team1rankandleveltext[index].color = this.Rank1;
			}
			else if (lvlandrank.Contains("Sputterer"))
			{
				this.team1rankandleveltext[index].color = this.Rank2;
			}
			else if (lvlandrank.Contains("Novice"))
			{
				this.team1rankandleveltext[index].color = this.Rank3;
			}
			else if (lvlandrank.Contains("Apprentice"))
			{
				this.team1rankandleveltext[index].color = this.Rank4;
			}
			else if (lvlandrank.Contains("Savant"))
			{
				this.team1rankandleveltext[index].color = this.Rank5;
			}
			else if (lvlandrank.Contains("Master"))
			{
				this.team1rankandleveltext[index].color = this.Rank6;
			}
			else if (lvlandrank.Contains("Grand"))
			{
				this.team1rankandleveltext[index].color = this.Rank7;
			}
			else if (lvlandrank.Contains("Supreme"))
			{
				this.team1rankandleveltext[index].color = this.Rank10;
			}
			else if (lvlandrank.Contains("Archmagus"))
			{
				this.team1rankandleveltext[index].color = this.Rank8;
			}
			else if (lvlandrank.Contains("Prime"))
			{
				this.team1rankandleveltext[index].color = this.Rank9;
			}
			GameObject gameObject = Object.Instantiate<GameObject>(this.Sorcerer, this.hats[index * 2].transform.position, Quaternion.identity);
			this.bodies[index * 2] = gameObject;
			return;
		}
		this.team2[index].text = this.ClampString(name, 10);
		lvlandrank = lvlandrank.Replace("_", " ");
		this.team2rankandleveltext[index].text = lvlandrank;
		if (lvlandrank.Contains("Lackey"))
		{
			this.team2rankandleveltext[index].color = this.Rank1;
		}
		else if (lvlandrank.Contains("Sputterer"))
		{
			this.team2rankandleveltext[index].color = this.Rank2;
		}
		else if (lvlandrank.Contains("Novice"))
		{
			this.team2rankandleveltext[index].color = this.Rank3;
		}
		else if (lvlandrank.Contains("Apprentice"))
		{
			this.team2rankandleveltext[index].color = this.Rank4;
		}
		else if (lvlandrank.Contains("Savant"))
		{
			this.team2rankandleveltext[index].color = this.Rank5;
		}
		else if (lvlandrank.Contains("Master"))
		{
			this.team2rankandleveltext[index].color = this.Rank6;
		}
		else if (lvlandrank.Contains("Grand"))
		{
			this.team2rankandleveltext[index].color = this.Rank7;
		}
		else if (lvlandrank.Contains("Supreme"))
		{
			this.team2rankandleveltext[index].color = this.Rank10;
		}
		else if (lvlandrank.Contains("Archmagus"))
		{
			this.team2rankandleveltext[index].color = this.Rank8;
		}
		else if (lvlandrank.Contains("Prime"))
		{
			this.team2rankandleveltext[index].color = this.Rank9;
		}
		GameObject gameObject2 = Object.Instantiate<GameObject>(this.Warlock, this.hats[index * 2 + 1].transform.position, Quaternion.identity);
		this.bodies[index * 2 + 1] = gameObject2;
	}

	// Token: 0x0600086D RID: 2157 RVA: 0x00020E48 File Offset: 0x0001F048
	private string ClampString(string input, int maxLength)
	{
		if (input.Length <= maxLength)
		{
			return input;
		}
		return input.Substring(0, maxLength);
	}

	// Token: 0x0600086E RID: 2158 RVA: 0x00020E60 File Offset: 0x0001F060
	public void DestroySpinningGuy(int team, int index)
	{
		if (team == 0)
		{
			this.team1[index].text = "";
			this.team1rankandleveltext[index].text = "";
			HatSpin component = this.bodies[index * 2].GetComponent<HatSpin>();
			this.bodies[index * 2] = null;
			component.DestroyMe();
			return;
		}
		this.team2[index].text = "";
		this.team2rankandleveltext[index].text = "";
		HatSpin component2 = this.bodies[index * 2 + 1].GetComponent<HatSpin>();
		this.bodies[index * 2 + 1] = null;
		component2.DestroyMe();
	}

	// Token: 0x0600086F RID: 2159 RVA: 0x00020EFC File Offset: 0x0001F0FC
	public void PlayTutorial()
	{
		if (Time.time - this.buttonPressCoolDown > 0.5f)
		{
			this.buttonPressCoolDown = Time.time;
			this.CloseAccentMenu();
			this.StopCredits();
			this.isTutorial = true;
			BootstrapManager.CreateLobby(true);
			this.TextChatHolder.SetActive(false);
		}
	}

	// Token: 0x06000870 RID: 2160 RVA: 0x00020F4C File Offset: 0x0001F14C
	public void MakePrivate()
	{
		this.isLobbyPrivate = true;
		this.isPrivateCheckMark.SetActive(this.isLobbyPrivate);
		this.isPublicCheckMark.SetActive(!this.isLobbyPrivate);
	}

	// Token: 0x06000871 RID: 2161 RVA: 0x00020F7A File Offset: 0x0001F17A
	public void MakePublic()
	{
		this.isLobbyPrivate = false;
		this.isPrivateCheckMark.SetActive(this.isLobbyPrivate);
		this.isPublicCheckMark.SetActive(!this.isLobbyPrivate);
	}

	// Token: 0x06000872 RID: 2162 RVA: 0x00020FA8 File Offset: 0x0001F1A8
	public void ShowLobbyMenu()
	{
		this.CloseAccentMenu();
		this.StopCredits();
		this.CreateLobbyb.SetActive(false);
		this.JoinLobbyb.SetActive(false);
		this.PlayTutorialb.SetActive(false);
		this.quitbutton.SetActive(false);
		this.creditsbutton.SetActive(false);
		this.accentbutton.SetActive(false);
		this.CreateLobbyMenu.SetActive(true);
		BootstrapManager.instance.isPlayAgain = false;
	}

	// Token: 0x06000873 RID: 2163 RVA: 0x00021020 File Offset: 0x0001F220
	public void HideLobbyMenu()
	{
		this.CreateLobbyb.SetActive(true);
		this.JoinLobbyb.SetActive(true);
		this.PlayTutorialb.SetActive(true);
		this.quitbutton.SetActive(true);
		this.creditsbutton.SetActive(true);
		this.accentbutton.SetActive(true);
		this.CreateLobbyMenu.SetActive(false);
	}

	// Token: 0x06000874 RID: 2164 RVA: 0x00021084 File Offset: 0x0001F284
	public void CreateLobbySorc()
	{
		if (Time.time - this.buttonPressCoolDown > 0.5f)
		{
			this.LobbyCodeCover.SetActive(true);
			this.buttonPressCoolDown = Time.time;
			this.isTutorial = false;
			this.ishost = true;
			GameObject[] array = this.kickplayermenubuttons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
			BootstrapManager.instance.isPlayAgain = false;
			BootstrapManager.instance.is1v1 = false;
			this.Mapchoseholder.SetActive(true);
			BootstrapManager.instance.ClearLocalPlayers();
			BootstrapManager.CreateLobby(this.isLobbyPrivate);
		}
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x0002111E File Offset: 0x0001F31E
	public void OpenMainMenu()
	{
		this.CloseAllScreens();
		this.menuScreen.SetActive(true);
		Camera.main.transform.GetComponent<AudioReverbZone>().enabled = true;
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x00021148 File Offset: 0x0001F348
	public void OpenLobby()
	{
		this.CloseAllScreens();
		this.lobbyScreen.SetActive(true);
		this.startstartGameButton.gameObject.SetActive(false);
		if (this.isTutorial)
		{
			base.StartCoroutine(this.Idontevenknowman());
			return;
		}
		for (int i = 0; i < this.playerNames.Length; i++)
		{
			this.playerNames[i] = null;
			this.hats[i].SetActive(false);
			this.texts[i * 2].text = "";
			this.texts[i * 2 + 1].text = "";
		}
		base.StartCoroutine(this.IseriouslyDontKnowAnymore());
	}

	// Token: 0x06000877 RID: 2167 RVA: 0x000211EF File Offset: 0x0001F3EF
	private IEnumerator IseriouslyDontKnowAnymore()
	{
		GameObject networkManager = null;
		while (networkManager == null)
		{
			networkManager = GameObject.Find("BootstrapNetworkManager");
			yield return null;
		}
		while (string.IsNullOrEmpty(SteamFriends.GetPersonaName().ToString()))
		{
			yield return null;
		}
		BootstrapNetworkManager.OnJoinUpdateHats(SteamFriends.GetPersonaName().ToString());
		yield break;
	}

	// Token: 0x06000878 RID: 2168 RVA: 0x000211F8 File Offset: 0x0001F3F8
	public void RemoveHat(string PlayerName)
	{
		if (!this.networksceneswapped)
		{
			if (this.kickplayershold.nametosteamid.ContainsKey(PlayerName))
			{
				this.kickplayershold.nametosteamid.Remove(PlayerName);
			}
			for (int i = 0; i < this.playerNames.Length; i++)
			{
				if (this.playerNames[i] == PlayerName)
				{
					this.playerNames[i] = null;
					this.playerLevelandRanks[i] = null;
					break;
				}
			}
			if (this.playerNames != null)
			{
				this.SyncUpdateNames(this.playerNames, this.playerLevelandRanks);
				BootstrapNetworkManager.SyncPlayerNames(this.playerNames, this.playerLevelandRanks);
				return;
			}
		}
		else
		{
			this.mmmn.LeftInLobby(PlayerName);
			if (this.kickplayershold.nametosteamid.ContainsKey(PlayerName))
			{
				this.kickplayershold.nametosteamid.Remove(PlayerName);
			}
		}
	}

	// Token: 0x06000879 RID: 2169 RVA: 0x000212C9 File Offset: 0x0001F4C9
	public void KickPlayer(string steamid)
	{
		if (this.GameHasStarted)
		{
			this.mmmn.KickPlayer(steamid);
			return;
		}
		BootstrapNetworkManager.KickPlayer(steamid);
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x000212E8 File Offset: 0x0001F4E8
	public void ActKickPlayer(string steamid)
	{
		if (steamid == SteamUser.GetSteamID().ToString())
		{
			if (this.GameHasStarted)
			{
				this.networksceneswapped = false;
				this.LeaveGame();
				return;
			}
			this.networksceneswapped = false;
			this.LeaveLobby();
		}
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x00021334 File Offset: 0x0001F534
	public void SyncHats(string PlayerName, string PlayerRank, string steamid)
	{
		this.kickplayershold.AddToDict(PlayerName, steamid);
		for (int i = 0; i < this.playerNames.Length; i++)
		{
			if (this.playerNames[i] == null)
			{
				this.playerNames[i] = PlayerName;
				this.playerLevelandRanks[i] = PlayerRank;
				break;
			}
		}
		this.SyncUpdateNames(this.playerNames, this.playerLevelandRanks);
		BootstrapNetworkManager.SyncPlayerNames(this.playerNames, this.playerLevelandRanks);
	}

	// Token: 0x0600087C RID: 2172 RVA: 0x000213A4 File Offset: 0x0001F5A4
	public void SyncUpdateNames(string[] PlayerNames, string[] PlayerRanks)
	{
		this.playerNames = PlayerNames;
		for (int i = 0; i < this.playerNames.Length; i++)
		{
			if (this.playerNames[i] == null)
			{
				this.hats[i].SetActive(false);
				this.rankandleveltext[i].text = "";
				this.texts[i * 2].text = "";
				this.texts[i * 2 + 1].text = "";
			}
			else
			{
				this.hats[i].SetActive(true);
				if (PlayerRanks[i] != null)
				{
					PlayerRanks[i] = PlayerRanks[i].Replace("_", " ");
					if (PlayerRanks[i].Contains("Lackey"))
					{
						this.rankandleveltext[i].color = this.Rank1;
					}
					else if (PlayerRanks[i].Contains("Sputterer"))
					{
						this.rankandleveltext[i].color = this.Rank2;
					}
					else if (PlayerRanks[i].Contains("Novice"))
					{
						this.rankandleveltext[i].color = this.Rank3;
					}
					else if (PlayerRanks[i].Contains("Apprentice"))
					{
						this.rankandleveltext[i].color = this.Rank4;
					}
					else if (PlayerRanks[i].Contains("Savant"))
					{
						this.rankandleveltext[i].color = this.Rank5;
					}
					else if (PlayerRanks[i].Contains("Master"))
					{
						this.rankandleveltext[i].color = this.Rank6;
					}
					else if (PlayerRanks[i].Contains("Grand"))
					{
						this.rankandleveltext[i].color = this.Rank7;
					}
					else if (PlayerRanks[i].Contains("Supreme"))
					{
						this.rankandleveltext[i].color = this.Rank10;
					}
					else if (PlayerRanks[i].Contains("Archmagus"))
					{
						this.rankandleveltext[i].color = this.Rank8;
					}
					else if (PlayerRanks[i].Contains("Prime"))
					{
						this.rankandleveltext[i].color = this.Rank9;
					}
					this.rankandleveltext[i].text = PlayerRanks[i];
				}
				this.texts[i * 2].text = this.playerNames[i];
				this.texts[i * 2 + 1].text = this.playerNames[i];
			}
		}
	}

	// Token: 0x0600087D RID: 2173 RVA: 0x0002160D File Offset: 0x0001F80D
	private IEnumerator Idontevenknowman()
	{
		while (!this.tutorialStarted)
		{
			yield return null;
			this.StartGameTutorial();
		}
		yield break;
	}

	// Token: 0x0600087E RID: 2174 RVA: 0x0002161C File Offset: 0x0001F81C
	public static void LobbyEntered(string lobbyName, bool isHost, ulong lobbyid)
	{
		MainMenuManager.instance.getlobid(lobbyid);
		MainMenuManager.instance.lobbyTitle.text = lobbyName;
		MainMenuManager.instance.startGameButton.gameObject.SetActive(isHost);
		MainMenuManager.instance.lobbyIDText.text = BootstrapManager.CurrentLobbyID.ToString();
		MainMenuManager.instance.OpenLobby();
		MainMenuManager.instance.TextChatHolder.SetActive(true);
		MainMenuManager.instance.Mapchoseholder.SetActive(isHost);
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x0002169C File Offset: 0x0001F89C
	private void getlobid(ulong lobbyid)
	{
		this.lobbyId = lobbyid;
		this.mmmn.transform.GetComponent<PlayerRespawnManager>().lobbyid = lobbyid;
	}

	// Token: 0x06000880 RID: 2176 RVA: 0x000216BB File Offset: 0x0001F8BB
	private void CloseAllScreens()
	{
		this.menuScreen.SetActive(false);
		this.lobbyScreen.SetActive(false);
	}

	// Token: 0x06000881 RID: 2177 RVA: 0x000216D8 File Offset: 0x0001F8D8
	public void JoinLobby()
	{
		this.CloseAccentMenu();
		this.StopCredits();
		BootstrapManager.instance.is1v1 = false;
		BootstrapManager.instance.isPlayAgain = false;
		this.CreateLobbyb.SetActive(false);
		this.JoinLobbyb.SetActive(false);
		this.PlayTutorialb.SetActive(false);
		this.quitbutton.SetActive(false);
		this.creditsbutton.SetActive(false);
		this.accentbutton.SetActive(false);
		this.lobbyInput.gameObject.SetActive(true);
		this.LobbyCodeCover.SetActive(true);
		this.isTutorial = false;
	}

	// Token: 0x06000882 RID: 2178 RVA: 0x00021774 File Offset: 0x0001F974
	public void ActuallyJoinLobby()
	{
		if (Time.time - this.actjoincd > 2f)
		{
			this.actjoincd = Time.time;
			this.ishost = false;
			BootstrapManager.JoinByID(new CSteamID(Convert.ToUInt64(this.lobbyInput.text)));
			Bloom bloom;
			if (this.volume.profile.TryGet<Bloom>(out bloom))
			{
				bloom.active = false;
			}
			Fog fog;
			if (this.volume.profile.TryGet<Fog>(out fog))
			{
				fog.active = false;
			}
		}
	}

	// Token: 0x06000883 RID: 2179 RVA: 0x000217F6 File Offset: 0x0001F9F6
	public void toggleVersionError(string text)
	{
		this.errorText.text = text;
		base.StartCoroutine(this.toggleErrorOff());
	}

	// Token: 0x06000884 RID: 2180 RVA: 0x00021811 File Offset: 0x0001FA11
	private IEnumerator toggleErrorOff()
	{
		this.versionerrormsg.SetActive(true);
		yield return new WaitForSeconds(6f);
		this.versionerrormsg.SetActive(false);
		yield break;
	}

	// Token: 0x06000885 RID: 2181 RVA: 0x00021820 File Offset: 0x0001FA20
	public void CopyLobbyID()
	{
		TextEditor textEditor = new TextEditor();
		textEditor.text = this.lobbyIDText.text;
		textEditor.SelectAll();
		textEditor.Copy();
	}

	// Token: 0x06000886 RID: 2182 RVA: 0x00021844 File Offset: 0x0001FA44
	public void BackFromJoinLobby()
	{
		this.CreateLobbyb.SetActive(true);
		this.JoinLobbyb.SetActive(true);
		this.PlayTutorialb.SetActive(true);
		this.lobbyInput.gameObject.SetActive(false);
		this.quitbutton.SetActive(true);
		this.creditsbutton.SetActive(true);
		this.accentbutton.SetActive(true);
	}

	// Token: 0x06000887 RID: 2183 RVA: 0x000218AC File Offset: 0x0001FAAC
	public void LeaveLobby()
	{
		if (Time.time - this.leavecd > 0.5f)
		{
			this.leavecd = Time.time;
			if (this.ishost)
			{
				BootstrapManager.instance.CloseLobby();
				base.StartCoroutine(this.HostLeave());
				return;
			}
			DissonanceFishNetComms dissonanceFishNetComms = Object.FindFirstObjectByType<DissonanceFishNetComms>();
			if (dissonanceFishNetComms != null)
			{
				dissonanceFishNetComms.Stop();
			}
			this.ishost = false;
			BootstrapManager.LeaveLobby();
			this.kickplayershold.nametosteamid.Clear();
			this.CreateLobbyb.SetActive(true);
			this.JoinLobbyb.SetActive(true);
			this.PlayTutorialb.SetActive(true);
			this.lobbyInput.gameObject.SetActive(false);
			this.quitbutton.SetActive(true);
			this.creditsbutton.SetActive(true);
			this.accentbutton.SetActive(true);
			this.HideLobbyMenu();
			for (int i = 0; i < this.playerNames.Length; i++)
			{
				this.hats[i].SetActive(false);
				this.texts[i * 2].text = "";
				this.texts[i * 2 + 1].text = "";
			}
			this.OpenMainMenu();
			MainMenuManager.instance.TextChatHolder.SetActive(false);
			foreach (GameObject gameObject in this.ChatMessages)
			{
				Object.Destroy(gameObject);
			}
		}
	}

	// Token: 0x06000888 RID: 2184 RVA: 0x00021A30 File Offset: 0x0001FC30
	private IEnumerator HostLeave()
	{
		yield return new WaitForSeconds(1.5f);
		BootstrapNetworkManager.HostLeftInLobby();
		yield break;
	}

	// Token: 0x06000889 RID: 2185 RVA: 0x00021A38 File Offset: 0x0001FC38
	public void DisbandLobby()
	{
		DissonanceFishNetComms dissonanceFishNetComms = Object.FindFirstObjectByType<DissonanceFishNetComms>();
		if (dissonanceFishNetComms != null)
		{
			dissonanceFishNetComms.Stop();
		}
		this.ishost = false;
		BootstrapManager.LeaveLobby();
		this.kickplayershold.nametosteamid.Clear();
		this.CreateLobbyb.SetActive(true);
		this.JoinLobbyb.SetActive(true);
		this.PlayTutorialb.SetActive(true);
		this.lobbyInput.gameObject.SetActive(false);
		this.quitbutton.SetActive(true);
		this.creditsbutton.SetActive(true);
		this.accentbutton.SetActive(true);
		this.HideLobbyMenu();
		for (int i = 0; i < this.playerNames.Length; i++)
		{
			this.hats[i].SetActive(false);
			this.texts[i * 2].text = "";
			this.texts[i * 2 + 1].text = "";
		}
		this.OpenMainMenu();
		MainMenuManager.instance.TextChatHolder.SetActive(false);
		foreach (GameObject gameObject in this.ChatMessages)
		{
			Object.Destroy(gameObject);
		}
	}

	// Token: 0x0600088A RID: 2186 RVA: 0x00021B7C File Offset: 0x0001FD7C
	public void ShowStartLobby()
	{
		this.LobbyConfirm.SetActive(true);
	}

	// Token: 0x0600088B RID: 2187 RVA: 0x00021B8A File Offset: 0x0001FD8A
	public void BackFromConfirmLobby()
	{
		this.LobbyConfirm.SetActive(false);
	}

	// Token: 0x0600088C RID: 2188 RVA: 0x00021B98 File Offset: 0x0001FD98
	public void StartGame()
	{
		if (Time.time - this.startgamecooldown > 5f)
		{
			this.startgamecooldown = Time.time;
			this.menuScreen.SetActive(false);
			this.lobbyScreen.SetActive(false);
			this.InGameLobby.SetActive(true);
			this.startstartGameButton.gameObject.SetActive(true);
			BootstrapManager.instance.CloseLobby();
			base.StartCoroutine(this.WaitForLobbyDataClosed());
		}
	}

	// Token: 0x0600088D RID: 2189 RVA: 0x00021C0F File Offset: 0x0001FE0F
	private IEnumerator WaitForLobbyDataClosed()
	{
		yield return new WaitForSeconds(1.5f);
		BootstrapNetworkManager.ChangeNetworkScene("GameScene");
		this.networksceneswapped = true;
		yield break;
	}

	// Token: 0x0600088E RID: 2190 RVA: 0x00021C1E File Offset: 0x0001FE1E
	public void RefreshLobbyList()
	{
		BootstrapManager.instance.GetLobbiesList();
	}

	// Token: 0x0600088F RID: 2191 RVA: 0x00021C2C File Offset: 0x0001FE2C
	public void movespawnscolosseum()
	{
		if (this.mapinfo == 2)
		{
			this.spawnPoints[0].transform.position = new Vector3(888.9f, 65.58f, 105.3f);
			this.spawnPoints[0].spawnpos.position = new Vector3(888.9f, 65.58f, 105.3f);
			this.spawnPoints[2].transform.position = new Vector3(863.09f, 65.58f, 146.3f);
			this.spawnPoints[2].spawnpos.position = new Vector3(863.09f, 65.58f, 146.3f);
		}
	}

	// Token: 0x06000890 RID: 2192 RVA: 0x00021CDD File Offset: 0x0001FEDD
	public void StartGameTutorial()
	{
		this.menuScreen.SetActive(false);
		this.lobbyScreen.SetActive(false);
		this.InGameLobby.SetActive(false);
		this.TextChatHolder.SetActive(false);
		BootstrapNetworkManager.ChangeNetworkSceneTutorial("GameScene");
	}

	// Token: 0x06000891 RID: 2193 RVA: 0x00021D1C File Offset: 0x0001FF1C
	public void StartGameActual()
	{
		this.Menumusiccontroller.FadeOutMenuMusic();
		this.mmmn.transform.GetComponent<DungeonGenerator>().IsThisTutorial = false;
		this.menuScreen.SetActive(false);
		this.lobbyScreen.SetActive(false);
		this.InGameLobby.SetActive(true);
		for (int i = 0; i < this.playerNames.Length; i++)
		{
			this.hats[i].SetActive(false);
			this.texts[i * 2].text = "";
			this.texts[i * 2 + 1].text = "";
		}
		base.StartCoroutine(this.WaitforClient());
	}

	// Token: 0x06000892 RID: 2194 RVA: 0x00021DC6 File Offset: 0x0001FFC6
	private IEnumerator WaitforClient()
	{
		while (!this.mmmn.NetworkObject.IsClientInitialized)
		{
			yield return null;
		}
		int num = this.mapinfo;
		this.mmmn.RequestMapInfo();
		this.mmmn.transform.GetComponent<DungeonGenerator>().StartGenRoutine();
		yield break;
	}

	// Token: 0x06000893 RID: 2195 RVA: 0x00021DD8 File Offset: 0x0001FFD8
	public void StartGameActualTutorial()
	{
		if (!this.tutorialStarted)
		{
			this.tutorialStarted = true;
			this.pm.teamSpawns = this.TutorialSpawnPoint;
			this.pm.StartGame();
			this.TextChatHolder.SetActive(false);
			this.drp1.transform.parent.gameObject.SetActive(false);
			this.drp2.transform.parent.gameObject.SetActive(false);
			this.lobbyScreen.SetActive(false);
			this.InGameLobby.SetActive(false);
			this.invUI.SetActive(true);
			this.GameHasStarted = true;
			this.InGameMenuHolder.SetActive(true);
			Camera.main.targetTexture = this.CDS;
			this.cdsgo.SetActive(true);
			Camera.main.orthographic = false;
			Camera.main.fieldOfView = 60f;
			Camera.main.cullingMask = this.normal;
			Bloom bloom;
			if (this.volume.profile.TryGet<Bloom>(out bloom))
			{
				bloom.active = true;
			}
			Fog fog;
			if (this.volume.profile.TryGet<Fog>(out fog))
			{
				fog.active = true;
			}
			this.pm.GetComponent<PlayerInventory>().InitInv();
			this.pm.GetComponent<PlayerInventory>().SetSpawnItems();
			this.pm.istutorial = true;
			this.tutorialsc.StartTutorial();
			this.pm.SetName(SteamFriends.GetPersonaName().ToString());
			base.StartCoroutine(this.WaittoSyncVolumes());
		}
	}

	// Token: 0x06000894 RID: 2196 RVA: 0x00021F66 File Offset: 0x00020166
	public void ActuallyStartGame()
	{
		if (Time.time - this.cooldown > 2f)
		{
			this.cooldown = Time.time;
			this.mmmn.ActuallyStartGame();
		}
	}

	// Token: 0x06000895 RID: 2197 RVA: 0x00021F94 File Offset: 0x00020194
	public void ActuallyStartGameActually()
	{
		this.InGameLobby.SetActive(false);
		GameObject[] array = GameObject.FindGameObjectsWithTag("hex");
		MainMenuManager.instance.TextChatHolder.SetActive(false);
		this.Crosshair.SetActive(true);
		GameObject[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			ItemSpawner itemSpawner;
			if (array2[i].TryGetComponent<ItemSpawner>(out itemSpawner))
			{
				itemSpawner.StartItemRoutine();
			}
		}
		this.cncs[0].gameObject.GetComponent<NightAudioSwap>().Getdasun();
		this.cncs[1].gameObject.GetComponent<NightAudioSwap>().Getdasun();
		this.MainMenuVisuals.SetActive(false);
		this.dpui.SaveFlags();
		this.sun.SetActive(true);
		this.sun.GetComponent<WeatherCycle>().StartSun();
		if (this.mapinfo == 2)
		{
			this.sun.GetComponent<WeatherCycle>().setrot(170f);
		}
		this.pm.teamSpawns = this.spawnPoints;
		this.pm.StartGame();
		this.pm.xpbar = this.xpbar;
		this.pm.setxpbar();
		this.FlagEdit.SetActive(false);
		this.Flagdraw2.SetActive(false);
		this.Flagdraw1.SetActive(false);
		this.drpvis.SetActive(false);
		this.drpvis2.SetActive(false);
		this.drp1.transform.parent.gameObject.SetActive(false);
		this.drp2.transform.parent.gameObject.SetActive(false);
		this.lobbyScreen.SetActive(false);
		this.invUI.SetActive(true);
		this.GameHasStarted = true;
		this.InGameMenuHolder.SetActive(true);
		Camera.main.targetTexture = this.CDS;
		this.cdsgo.SetActive(true);
		Camera.main.orthographic = false;
		Camera.main.fieldOfView = 60f;
		Camera.main.cullingMask = this.normal;
		Bloom bloom;
		if (this.volume.profile.TryGet<Bloom>(out bloom))
		{
			bloom.active = true;
		}
		Fog fog;
		if (this.volume.profile.TryGet<Fog>(out fog))
		{
			fog.active = true;
		}
		this.pm.GetComponent<PlayerInventory>().InitInv();
		this.pm.GetComponent<PlayerInventory>().SetSpawnItems();
		for (int j = 0; j < this.bodies.Length; j++)
		{
			if (this.bodies[j] != null)
			{
				Object.Destroy(this.bodies[j]);
				this.bodies[j] = null;
			}
		}
		for (int k = 0; k < this.team1.Length; k++)
		{
			this.team1[k].text = "";
			this.team2[k].text = "";
		}
		this.cfcn.StartFlagsShit();
		this.pm.SetName(SteamFriends.GetPersonaName().ToString());
		Camera.main.transform.GetComponent<AudioReverbZone>().enabled = false;
		if (this.mapinfo == 2)
		{
			base.StartCoroutine(this.playtrumpetcoli());
		}
		else
		{
			base.StartCoroutine(this.playtrumpet());
		}
		ShadowWizardAI[] array3 = this.swais;
		for (int i = 0; i < array3.Length; i++)
		{
			array3[i].AgentSetup();
		}
		this.mmmn.transform.GetComponent<PlayerRespawnManager>().unmuteaud();
		base.StartCoroutine(this.WaittoSyncVolumes());
		if (this.mapinfo == 2)
		{
			this.pm.canMove = false;
			this.mmmn.transform.GetComponent<PlayerRespawnManager>().startcoliroutine();
		}
		this.emrs[0].enableRenderers();
		this.emrs[1].enableRenderers();
		if (this.mapinfo != 2 && !this.isTutorial)
		{
			base.StartCoroutine(this.fadeinctfoverlay());
			return;
		}
		if (this.mapinfo == 2)
		{
			base.StartCoroutine(this.fadeindmoverlay());
		}
	}

	// Token: 0x06000896 RID: 2198 RVA: 0x0002237C File Offset: 0x0002057C
	private IEnumerator fadeinctfoverlay()
	{
		for (float timer = 0f; timer < 1f; timer += Time.deltaTime)
		{
			this.ctfoverlay.alpha = Mathf.Lerp(0f, 1f, timer);
			yield return null;
		}
		yield return new WaitForSeconds(3f);
		for (float timer = 0f; timer < 3f; timer += Time.deltaTime)
		{
			this.ctfoverlay.alpha = Mathf.Lerp(1f, 0f, timer / 3f);
			yield return null;
		}
		Object.Destroy(this.ctfoverlay.gameObject);
		Object.Destroy(this.dmoverlay.gameObject);
		yield break;
	}

	// Token: 0x06000897 RID: 2199 RVA: 0x0002238B File Offset: 0x0002058B
	private IEnumerator fadeindmoverlay()
	{
		for (float timer = 0f; timer < 1f; timer += Time.deltaTime)
		{
			this.dmoverlay.alpha = Mathf.Lerp(0f, 1f, timer);
			yield return null;
		}
		yield return new WaitForSeconds(4f);
		for (float timer = 0f; timer < 3f; timer += Time.deltaTime)
		{
			this.dmoverlay.alpha = Mathf.Lerp(1f, 0f, timer / 3f);
			yield return null;
		}
		Object.Destroy(this.ctfoverlay.gameObject);
		Object.Destroy(this.dmoverlay.gameObject);
		yield break;
	}

	// Token: 0x06000898 RID: 2200 RVA: 0x0002239A File Offset: 0x0002059A
	private IEnumerator WaittoSyncVolumes()
	{
		yield return new WaitForSeconds(2f);
		if (this.InGameLobby.activeSelf)
		{
			this.InGameLobby.SetActive(false);
		}
		this.pvsc.SetPlayerVolumeNames();
		yield break;
	}

	// Token: 0x06000899 RID: 2201 RVA: 0x000223A9 File Offset: 0x000205A9
	private IEnumerator playtrumpet()
	{
		this.Starttrumpet.bypassReverbZones = false;
		yield return new WaitForSeconds(3f);
		this.Starttrumpet.Play();
		yield return new WaitForSeconds(8.2f);
		this.Starttrumpet.pitch = 0.85f;
		this.Starttrumpet.volume = 0.2f;
		this.Starttrumpet.PlayOneShot(this.anouncerstart[0]);
		yield return new WaitForSeconds(9f);
		this.Starttrumpet.PlayOneShot(this.anouncerstart[1]);
		yield break;
	}

	// Token: 0x0600089A RID: 2202 RVA: 0x000223B8 File Offset: 0x000205B8
	private IEnumerator playtrumpetcoli()
	{
		this.Starttrumpet.bypassReverbZones = false;
		yield return new WaitForSeconds(0.5f);
		this.Starttrumpet.pitch = 0.85f;
		this.Starttrumpet.volume = 0.2f;
		this.Starttrumpet.PlayOneShot(this.anouncerstart[0]);
		yield return new WaitForSeconds(9f);
		this.Starttrumpet.PlayOneShot(this.anouncerstart[1]);
		yield break;
	}

	// Token: 0x0600089B RID: 2203 RVA: 0x000223C7 File Offset: 0x000205C7
	public void JoinLowestPlayerCountTeam()
	{
		if (this.hasSwappedteam)
		{
			this.hasSwappedteam = false;
			this.mmmn.JoinLowestPlayerTeam(SteamFriends.GetPersonaName().ToString());
		}
	}

	// Token: 0x0600089C RID: 2204 RVA: 0x000223ED File Offset: 0x000205ED
	public void JoinTeam1()
	{
		if (this.hasSwappedteam)
		{
			this.hasSwappedteam = false;
			this.mmmn.JoinTeam(SteamFriends.GetPersonaName().ToString(), 0);
		}
	}

	// Token: 0x0600089D RID: 2205 RVA: 0x00022414 File Offset: 0x00020614
	public void JoinTeam2()
	{
		if (this.hasSwappedteam)
		{
			this.hasSwappedteam = false;
			this.mmmn.JoinTeam(SteamFriends.GetPersonaName().ToString(), 2);
		}
	}

	// Token: 0x0600089E RID: 2206 RVA: 0x0002243C File Offset: 0x0002063C
	public void SaveFlag()
	{
		if (this.pm.playerTeam == 0 && Time.time - this.savecooldown > 15f)
		{
			this.savecooldown = Time.time;
			this.drp1.SaveFlagPermanently();
			return;
		}
		if (this.pm.playerTeam == 2 && Time.time - this.savecooldown > 15f)
		{
			this.savecooldown = Time.time;
			this.drp2.SaveFlagPermanently();
		}
	}

	// Token: 0x0600089F RID: 2207 RVA: 0x000224B8 File Offset: 0x000206B8
	public void LoadFlag()
	{
		if (this.pm.playerTeam == 0 && Time.time - this.loadcooldown > 15f)
		{
			this.loadcooldown = Time.time;
			this.drp1.loadflag();
			return;
		}
		if (this.pm.playerTeam == 2 && Time.time - this.loadcooldown > 15f)
		{
			this.loadcooldown = Time.time;
			this.drp2.loadflag();
		}
	}

	// Token: 0x060008A0 RID: 2208 RVA: 0x00022534 File Offset: 0x00020734
	public void EditTeamFlag()
	{
		if (this.pm.playerTeam == 0)
		{
			this.menuScreen.SetActive(false);
			this.lobbyScreen.SetActive(false);
			this.InGameLobby.SetActive(false);
			this.FlagEdit.SetActive(true);
			this.Flagdraw1.SetActive(true);
			this.Flagdraw2.SetActive(false);
			this.drp1.StartEditing();
			this.drpvis.SetActive(true);
			this.drpvis2.SetActive(false);
			this.flagposlerper1.LerpFlag(true);
			this.flagposlerperpole.LerpFlag(true);
			return;
		}
		if (this.pm.playerTeam == 2)
		{
			this.menuScreen.SetActive(false);
			this.lobbyScreen.SetActive(false);
			this.InGameLobby.SetActive(false);
			this.FlagEdit.SetActive(true);
			this.Flagdraw2.SetActive(true);
			this.Flagdraw1.SetActive(false);
			this.drp2.StartEditing();
			this.drpvis.SetActive(false);
			this.drpvis2.SetActive(true);
			this.flagposlerper2.LerpFlag(true);
			this.flagposlerperpole.LerpFlag(true);
		}
	}

	// Token: 0x060008A1 RID: 2209 RVA: 0x0002266C File Offset: 0x0002086C
	public void BackFromFlagEditor()
	{
		if (this.flagposlerper1.gameObject.activeSelf)
		{
			this.flagposlerper1.LerpFlag(false);
		}
		else
		{
			this.flagposlerper2.LerpFlag(false);
		}
		this.flagposlerperpole.LerpFlag(false);
		base.StartCoroutine(this.backroutine());
	}

	// Token: 0x060008A2 RID: 2210 RVA: 0x000226BE File Offset: 0x000208BE
	private IEnumerator backroutine()
	{
		yield return new WaitForSeconds(0.2f);
		this.FlagEdit.SetActive(false);
		this.InGameLobby.SetActive(true);
		this.Flagdraw2.SetActive(false);
		this.Flagdraw1.SetActive(false);
		this.drpvis.SetActive(false);
		this.drpvis2.SetActive(false);
		yield break;
	}

	// Token: 0x060008A3 RID: 2211 RVA: 0x000226D0 File Offset: 0x000208D0
	public void ToggleInGameMenu()
	{
		if (this.InGameMenu.activeSelf && this.GameHasStarted)
		{
			this.CloseSettings();
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else
		{
			Cursor.lockState = CursorLockMode.Confined;
			Cursor.visible = true;
		}
		this.InGameMenu.SetActive(!this.InGameMenu.activeSelf);
	}

	// Token: 0x060008A4 RID: 2212 RVA: 0x0002272B File Offset: 0x0002092B
	public void PlayAgain()
	{
		this.kickplayershold.nametosteamid.Clear();
		if (this.ishost)
		{
			BootstrapManager.instance.HostPlayAgain();
			return;
		}
		BootstrapManager.instance.ClientPlayAgain();
	}

	// Token: 0x060008A5 RID: 2213 RVA: 0x0002275A File Offset: 0x0002095A
	public void HostPlayAgain()
	{
		this.ishost = true;
		this.Mapchoseholder.SetActive(true);
	}

	// Token: 0x060008A6 RID: 2214 RVA: 0x00022770 File Offset: 0x00020970
	public void LeaveGame()
	{
		if (Time.time - this.LeaveGameCooldown > 0.5f)
		{
			this.LeaveGameCooldown = Time.time;
			MainMenuManager.instance.TextChatHolder.SetActive(false);
			this.kickplayershold.nametosteamid.Clear();
			if (this.ishost)
			{
				this.mmmn.leavedagam();
				this.ishost = false;
				return;
			}
			BootstrapManager.LeaveLobby2();
		}
	}

	// Token: 0x060008A7 RID: 2215 RVA: 0x000227DB File Offset: 0x000209DB
	public void ActLeaveGame()
	{
		BootstrapManager.LeaveLobby2();
	}

	// Token: 0x060008A8 RID: 2216 RVA: 0x000227E4 File Offset: 0x000209E4
	public void SetPlayerVolumeNames()
	{
		Transform transform = Object.FindFirstObjectByType<DissonanceComms>().transform;
		float num = float.MaxValue;
		Transform transform2 = null;
		foreach (object obj in transform)
		{
			Transform transform3 = (Transform)obj;
			float num2 = Vector3.Distance(transform3.position, base.transform.position);
			if (num2 < num)
			{
				num = num2;
				transform2 = transform3;
			}
		}
		RaycastAudioDamper raycastAudioDamper;
		if (transform2 != null && transform2.TryGetComponent<RaycastAudioDamper>(out raycastAudioDamper))
		{
			raycastAudioDamper.ToggleGlobalVoice();
		}
	}

	// Token: 0x060008A9 RID: 2217 RVA: 0x00022884 File Offset: 0x00020A84
	public void OpenSettings()
	{
		this.Settings.SetActive(true);
	}

	// Token: 0x060008AA RID: 2218 RVA: 0x00022892 File Offset: 0x00020A92
	public void CloseSettings()
	{
		this.Settings.SetActive(false);
	}

	// Token: 0x060008AB RID: 2219 RVA: 0x000228A0 File Offset: 0x00020AA0
	public void QuitGame()
	{
		Application.Quit();
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x000228A7 File Offset: 0x00020AA7
	public void ResetMic()
	{
		Debug.Log("dont call this");
	}

	// Token: 0x060008AD RID: 2221 RVA: 0x000228B3 File Offset: 0x00020AB3
	public void turnoffinv()
	{
		this.invUI.SetActive(false);
	}

	// Token: 0x060008AE RID: 2222 RVA: 0x000228C1 File Offset: 0x00020AC1
	public void callinitwizards()
	{
		this.mmmn.callinitSwizards();
	}

	// Token: 0x060008AF RID: 2223 RVA: 0x000228D0 File Offset: 0x00020AD0
	public void initSwizards()
	{
		ShadowWizardAI[] array = this.swais;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].inited = true;
		}
	}

	// Token: 0x04000449 RID: 1097
	private static MainMenuManager instance;

	// Token: 0x0400044A RID: 1098
	[SerializeField]
	private GameObject menuScreen;

	// Token: 0x0400044B RID: 1099
	[SerializeField]
	private GameObject lobbyScreen;

	// Token: 0x0400044C RID: 1100
	[SerializeField]
	private GameObject InGameLobby;

	// Token: 0x0400044D RID: 1101
	[SerializeField]
	private GameObject FlagEdit;

	// Token: 0x0400044E RID: 1102
	[SerializeField]
	private GameObject invUI;

	// Token: 0x0400044F RID: 1103
	[SerializeField]
	private GameObject cdsgo;

	// Token: 0x04000450 RID: 1104
	[SerializeField]
	private TMP_InputField lobbyInput;

	// Token: 0x04000451 RID: 1105
	[SerializeField]
	private TextMeshProUGUI lobbyTitle;

	// Token: 0x04000452 RID: 1106
	[SerializeField]
	private TextMeshProUGUI lobbyIDText;

	// Token: 0x04000453 RID: 1107
	[SerializeField]
	private Button startGameButton;

	// Token: 0x04000454 RID: 1108
	[SerializeField]
	private Button startstartGameButton;

	// Token: 0x04000455 RID: 1109
	public PlayerMovement pm;

	// Token: 0x04000456 RID: 1110
	public bool GameHasStarted;

	// Token: 0x04000457 RID: 1111
	public LayerMask normal;

	// Token: 0x04000458 RID: 1112
	public LayerMask drawmode;

	// Token: 0x04000459 RID: 1113
	public RenderTexture CDS;

	// Token: 0x0400045A RID: 1114
	public Volume volume;

	// Token: 0x0400045B RID: 1115
	public DrawPixelsUI dpui;

	// Token: 0x0400045C RID: 1116
	public DrawPixels drp1;

	// Token: 0x0400045D RID: 1117
	public DrawPixels drp2;

	// Token: 0x0400045E RID: 1118
	public GameObject drpvis;

	// Token: 0x0400045F RID: 1119
	public GameObject drpvis2;

	// Token: 0x04000460 RID: 1120
	public FlagPositionLerper flagposlerper1;

	// Token: 0x04000461 RID: 1121
	public FlagPositionLerper flagposlerper2;

	// Token: 0x04000462 RID: 1122
	public FlagPositionLerper flagposlerperpole;

	// Token: 0x04000463 RID: 1123
	public GameObject Flagdraw1;

	// Token: 0x04000464 RID: 1124
	public GameObject Flagdraw2;

	// Token: 0x04000465 RID: 1125
	public MainMenuManagerNetworked mmmn;

	// Token: 0x04000466 RID: 1126
	public RespawnWormhole[] spawnPoints;

	// Token: 0x04000467 RID: 1127
	public RespawnWormhole[] TutorialSpawnPoint;

	// Token: 0x04000468 RID: 1128
	public bool isTutorial;

	// Token: 0x04000469 RID: 1129
	private bool tutorialStarted;

	// Token: 0x0400046A RID: 1130
	public GameObject sun;

	// Token: 0x0400046B RID: 1131
	public GameObject InGameMenuHolder;

	// Token: 0x0400046C RID: 1132
	public GameObject InGameMenu;

	// Token: 0x0400046D RID: 1133
	public GameObject CreateLobbyb;

	// Token: 0x0400046E RID: 1134
	public GameObject JoinLobbyb;

	// Token: 0x0400046F RID: 1135
	public GameObject PlayTutorialb;

	// Token: 0x04000470 RID: 1136
	public Texture2D blank;

	// Token: 0x04000471 RID: 1137
	private string[] playerNames;

	// Token: 0x04000472 RID: 1138
	private string[] playerLevelandRanks;

	// Token: 0x04000473 RID: 1139
	public GameObject[] hats;

	// Token: 0x04000474 RID: 1140
	public Text[] texts;

	// Token: 0x04000475 RID: 1141
	public Text[] rankandleveltext;

	// Token: 0x04000476 RID: 1142
	public Text[] team1;

	// Token: 0x04000477 RID: 1143
	public Text[] team2;

	// Token: 0x04000478 RID: 1144
	public Text[] team1rankandleveltext;

	// Token: 0x04000479 RID: 1145
	public Text[] team2rankandleveltext;

	// Token: 0x0400047A RID: 1146
	public GameObject Sorcerer;

	// Token: 0x0400047B RID: 1147
	public GameObject Warlock;

	// Token: 0x0400047C RID: 1148
	private GameObject[] bodies = new GameObject[8];

	// Token: 0x0400047D RID: 1149
	public GameObject MainMenuVisuals;

	// Token: 0x0400047E RID: 1150
	public ChestNetController[] cncs;

	// Token: 0x0400047F RID: 1151
	public CastleFlagCapturedNotifier cfcn;

	// Token: 0x04000480 RID: 1152
	public GameObject Settings;

	// Token: 0x04000481 RID: 1153
	private string farfuckssakes;

	// Token: 0x04000482 RID: 1154
	public AudioSource Starttrumpet;

	// Token: 0x04000483 RID: 1155
	public TutorialScript tutorialsc;

	// Token: 0x04000484 RID: 1156
	public AudioClip[] anouncerstart;

	// Token: 0x04000485 RID: 1157
	public GameObject Credits;

	// Token: 0x04000486 RID: 1158
	public ShadowWizardAI[] swais;

	// Token: 0x04000487 RID: 1159
	public GameObject quitbutton;

	// Token: 0x04000488 RID: 1160
	public GameObject creditsbutton;

	// Token: 0x04000489 RID: 1161
	public ulong lobbyId;

	// Token: 0x0400048A RID: 1162
	public DecalProjector xpbar;

	// Token: 0x0400048B RID: 1163
	public int mapinfo = 1;

	// Token: 0x0400048C RID: 1164
	public GameObject[] checkmarks;

	// Token: 0x0400048D RID: 1165
	public GameObject Mapchoseholder;

	// Token: 0x0400048E RID: 1166
	private bool isLobbyPrivate = true;

	// Token: 0x0400048F RID: 1167
	public GameObject CreateLobbyMenu;

	// Token: 0x04000490 RID: 1168
	public GameObject isPrivateCheckMark;

	// Token: 0x04000491 RID: 1169
	public GameObject isPublicCheckMark;

	// Token: 0x04000492 RID: 1170
	public GameObject accentbutton;

	// Token: 0x04000493 RID: 1171
	public GameObject AccentMenu;

	// Token: 0x04000494 RID: 1172
	public GameObject[] AccentsCheckMarks;

	// Token: 0x04000495 RID: 1173
	public GameObject Crosshair;

	// Token: 0x04000496 RID: 1174
	private bool ishost;

	// Token: 0x04000497 RID: 1175
	public EnableMeshRenderers[] emrs;

	// Token: 0x04000498 RID: 1176
	public PlayerVolumeSettingsController pvsc;

	// Token: 0x04000499 RID: 1177
	public MenuThemeVolumeUpper Menumusiccontroller;

	// Token: 0x0400049A RID: 1178
	public CanvasGroup ctfoverlay;

	// Token: 0x0400049B RID: 1179
	public CanvasGroup dmoverlay;

	// Token: 0x0400049C RID: 1180
	public Color Rank1;

	// Token: 0x0400049D RID: 1181
	public Color Rank2;

	// Token: 0x0400049E RID: 1182
	public Color Rank3;

	// Token: 0x0400049F RID: 1183
	public Color Rank4;

	// Token: 0x040004A0 RID: 1184
	public Color Rank5;

	// Token: 0x040004A1 RID: 1185
	public Color Rank6;

	// Token: 0x040004A2 RID: 1186
	public Color Rank7;

	// Token: 0x040004A3 RID: 1187
	public Color Rank8;

	// Token: 0x040004A4 RID: 1188
	public Color Rank9;

	// Token: 0x040004A5 RID: 1189
	public Color Rank10;

	// Token: 0x040004A6 RID: 1190
	public GameObject[] Filterdmctfchecks;

	// Token: 0x040004A7 RID: 1191
	private float buttonPressCoolDown = -1f;

	// Token: 0x040004A8 RID: 1192
	public KickPlayersHolder kickplayershold;

	// Token: 0x040004A9 RID: 1193
	public GameObject kickplayersmenu;

	// Token: 0x040004AA RID: 1194
	private bool networksceneswapped;

	// Token: 0x040004AB RID: 1195
	public GameObject[] kickplayermenubuttons;

	// Token: 0x040004AC RID: 1196
	public GameObject TextChatHolder;

	// Token: 0x040004AD RID: 1197
	public TMP_InputField chatInputFieldText;

	// Token: 0x040004AE RID: 1198
	public GameObject ChatMessagePrefab;

	// Token: 0x040004AF RID: 1199
	public GameObject ChatListContent;

	// Token: 0x040004B0 RID: 1200
	private List<GameObject> ChatMessages = new List<GameObject>();

	// Token: 0x040004B1 RID: 1201
	protected Callback<LobbyChatMsg_t> m_LobbyChatMsg;

	// Token: 0x040004B2 RID: 1202
	public ScrollRect scrollRect;

	// Token: 0x040004B3 RID: 1203
	public GameObject LobbyCodeCover;

	// Token: 0x040004B4 RID: 1204
	private float chatcd;

	// Token: 0x040004B5 RID: 1205
	private float actjoincd;

	// Token: 0x040004B6 RID: 1206
	public GameObject versionerrormsg;

	// Token: 0x040004B7 RID: 1207
	public Text errorText;

	// Token: 0x040004B8 RID: 1208
	private float leavecd;

	// Token: 0x040004B9 RID: 1209
	public GameObject LobbyConfirm;

	// Token: 0x040004BA RID: 1210
	private float startgamecooldown;

	// Token: 0x040004BB RID: 1211
	private float cooldown;

	// Token: 0x040004BC RID: 1212
	public bool hasSwappedteam = true;

	// Token: 0x040004BD RID: 1213
	private float savecooldown = -30f;

	// Token: 0x040004BE RID: 1214
	private float loadcooldown = -30f;

	// Token: 0x040004BF RID: 1215
	private float LeaveGameCooldown;
}
