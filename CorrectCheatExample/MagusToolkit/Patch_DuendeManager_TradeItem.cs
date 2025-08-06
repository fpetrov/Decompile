using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

// Token: 0x02000018 RID: 24
[HarmonyPatch(typeof(DuendeManager), "TradeItem")]
public static class Patch_DuendeManager_TradeItem
{
	// Token: 0x0600003B RID: 59 RVA: 0x0000373C File Offset: 0x0000193C
	[HarmonyPrefix]
	public static bool Prefix(DuendeManager __instance, int DuendeID)
	{
		if (!ConfigManager.TradePage.Value)
		{
			return true;
		}
		FieldInfo field = typeof(DuendeManager).GetField("plt", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		object obj = ((field != null) ? field.GetValue(__instance) : null);
		if (obj == null)
		{
			ConfigManager.Logger.LogWarning("Failed to get plt from DuendeManager.");
			return true;
		}
		FieldInfo field2 = obj.GetType().GetField("Pages", BindingFlags.Instance | BindingFlags.Public);
		object[] array = ((field2 != null) ? field2.GetValue(obj) : null) as object[];
		if (array == null || array.Length == 0)
		{
			ConfigManager.Logger.LogWarning("Failed to get Pages from plt.");
			return true;
		}
		int num = Random.Range(0, array.Length);
		MethodInfo method = typeof(DuendeManager).GetMethod("ServerCreatePage", BindingFlags.Instance | BindingFlags.NonPublic);
		if (method == null)
		{
			ConfigManager.Logger.LogWarning("Failed to get ServerCreatePage method.");
			return true;
		}
		method.Invoke(__instance, new object[] { DuendeID, num });
		return false;
	}
}
