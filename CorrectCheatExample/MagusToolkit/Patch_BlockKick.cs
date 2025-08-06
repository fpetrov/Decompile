using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

// Token: 0x02000019 RID: 25
[HarmonyPatch(typeof(MainMenuManager), "ActKickPlayer")]
public class Patch_BlockKick
{
	// Token: 0x0600003C RID: 60 RVA: 0x00003828 File Offset: 0x00001A28
	private static bool Prefix(string steamid)
	{
		MainMenuManagerNetworked mainMenuManagerNetworked = Object.FindFirstObjectByType<MainMenuManagerNetworked>();
		if (mainMenuManagerNetworked == null)
		{
			return true;
		}
		string text = (string)typeof(MainMenuManagerNetworked).GetField("localplayername", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(mainMenuManagerNetworked);
		MainMenuManager mainMenuManager = Object.FindFirstObjectByType<MainMenuManager>();
		if (mainMenuManager == null || mainMenuManager.kickplayershold == null)
		{
			return true;
		}
		string text2;
		if (mainMenuManager.kickplayershold.nametosteamid.TryGetValue(text, out text2) && steamid == text2)
		{
			ConfigManager.Logger.LogWarning(string.Concat(new string[] { "[AntiKick] Kick blocked for ", text, " (", text2, ")" }));
			return false;
		}
		return true;
	}
}
