using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

// Token: 0x0200007F RID: 127
public class FindPublicLobbies : MonoBehaviour
{
	// Token: 0x06000567 RID: 1383 RVA: 0x00014DC8 File Offset: 0x00012FC8
	private void Awake()
	{
		if (FindPublicLobbies.instance == null)
		{
			FindPublicLobbies.instance = this;
		}
	}

	// Token: 0x06000568 RID: 1384 RVA: 0x00014DE0 File Offset: 0x00012FE0
	public void DestroyLobbies()
	{
		foreach (GameObject gameObject in this.listOfLobbies)
		{
			Object.Destroy(gameObject);
		}
		this.listOfLobbies.Clear();
		Debug.Log("destroyinglobbylist");
	}

	// Token: 0x06000569 RID: 1385 RVA: 0x00014E48 File Offset: 0x00013048
	public void DisplayLobbies(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result)
	{
		for (int i = 0; i < lobbyIDs.Count; i++)
		{
			if (lobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.lobbyDataItemPrefab);
				gameObject.GetComponent<LobbyDataEntry>().lobbyID = (CSteamID)lobbyIDs[i].m_SteamID;
				gameObject.GetComponent<LobbyDataEntry>().lobbyName = SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDs[i].m_SteamID, "name");
				gameObject.GetComponent<LobbyDataEntry>().SetLobbyData(SteamMatchmaking.GetNumLobbyMembers((CSteamID)lobbyIDs[i].m_SteamID).ToString(), SteamMatchmaking.GetLobbyMemberLimit((CSteamID)lobbyIDs[i].m_SteamID));
				gameObject.transform.SetParent(this.lobbyListContent.transform);
				gameObject.transform.localScale = Vector3.one;
				this.listOfLobbies.Add(gameObject);
			}
		}
		Debug.Log("displayinglobbylist");
	}

	// Token: 0x0600056A RID: 1386 RVA: 0x00014F4C File Offset: 0x0001314C
	public void GetListOfLobbies()
	{
		this.lobbiesButton.SetActive(false);
		this.hostButton.SetActive(false);
		this.lobbiesMenu.SetActive(true);
		BootstrapManager.instance.GetLobbiesList();
		Debug.Log("gettinglobbylist");
	}

	// Token: 0x0400029E RID: 670
	public static FindPublicLobbies instance;

	// Token: 0x0400029F RID: 671
	public GameObject lobbiesMenu;

	// Token: 0x040002A0 RID: 672
	public GameObject lobbyDataItemPrefab;

	// Token: 0x040002A1 RID: 673
	public GameObject lobbyListContent;

	// Token: 0x040002A2 RID: 674
	public GameObject lobbiesButton;

	// Token: 0x040002A3 RID: 675
	public GameObject hostButton;

	// Token: 0x040002A4 RID: 676
	public List<GameObject> listOfLobbies = new List<GameObject>();
}
