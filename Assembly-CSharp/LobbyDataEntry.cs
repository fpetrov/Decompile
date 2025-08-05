using System;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000C1 RID: 193
public class LobbyDataEntry : MonoBehaviour
{
	// Token: 0x06000774 RID: 1908 RVA: 0x0001CC4C File Offset: 0x0001AE4C
	public void SetLobbyData(string numplayers, int max)
	{
		if (this.lobbyName == "")
		{
			this.lobbyNameText.text = "Empty";
		}
		else
		{
			this.lobbyNameText.text = this.lobbyName;
		}
		this.NumPlayers.text = numplayers + "/" + max.ToString();
	}

	// Token: 0x06000775 RID: 1909 RVA: 0x0001CCAB File Offset: 0x0001AEAB
	public void JoinLobby()
	{
		BootstrapManager.JoinByID(this.lobbyID);
	}

	// Token: 0x040003CC RID: 972
	public CSteamID lobbyID;

	// Token: 0x040003CD RID: 973
	public string lobbyName;

	// Token: 0x040003CE RID: 974
	public Text lobbyNameText;

	// Token: 0x040003CF RID: 975
	public Text NumPlayers;
}
