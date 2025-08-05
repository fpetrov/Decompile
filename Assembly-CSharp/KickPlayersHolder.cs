using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000B1 RID: 177
public class KickPlayersHolder : MonoBehaviour
{
	// Token: 0x06000708 RID: 1800 RVA: 0x0001AF0D File Offset: 0x0001910D
	public void AddToDict(string name, string steamid)
	{
		if (!this.nametosteamid.ContainsKey(name))
		{
			this.nametosteamid.Add(name, steamid);
			this.SetNames();
		}
	}

	// Token: 0x06000709 RID: 1801 RVA: 0x0001AF30 File Offset: 0x00019130
	public void kickplayer(Text name)
	{
		if (this.nametosteamid.ContainsKey(name.text))
		{
			base.GetComponent<MainMenuManager>().KickPlayer(this.nametosteamid[name.text]);
			this.nametosteamid.Remove(name.text);
			this.SetNames();
		}
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x0001AF84 File Offset: 0x00019184
	private void SetNames()
	{
		int num = 0;
		using (Dictionary<string, string>.Enumerator enumerator = this.nametosteamid.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, string> keyValuePair = enumerator.Current;
				if (num < 8)
				{
					Debug.Log(keyValuePair.Key);
					this.nametexts[num].text = keyValuePair.Key;
					this.nametexts[num].transform.parent.gameObject.SetActive(true);
					num++;
				}
			}
			goto IL_0097;
		}
		IL_0076:
		this.nametexts[num].transform.parent.gameObject.SetActive(false);
		num++;
		IL_0097:
		if (num >= 8)
		{
			return;
		}
		goto IL_0076;
	}

	// Token: 0x0400037F RID: 895
	public Dictionary<string, string> nametosteamid = new Dictionary<string, string>();

	// Token: 0x04000380 RID: 896
	public Text[] nametexts = new Text[8];
}
