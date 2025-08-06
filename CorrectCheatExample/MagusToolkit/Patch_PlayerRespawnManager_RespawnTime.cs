using System;
using System.Reflection;
using HarmonyLib;

// Token: 0x02000007 RID: 7
[HarmonyPatch(typeof(PlayerRespawnManager), "Awake")]
public class Patch_PlayerRespawnManager_RespawnTime
{
	// Token: 0x06000019 RID: 25 RVA: 0x00002CC8 File Offset: 0x00000EC8
	private static void Postfix(PlayerRespawnManager __instance)
	{
		if (!ConfigManager.InstantRespawn.Value)
		{
			return;
		}
		FieldInfo fieldInfo = AccessTools.Field(__instance.GetType(), "Respawntime");
		if (fieldInfo != null)
		{
			fieldInfo.SetValue(__instance, 0f);
			ConfigManager.Logger.LogInfo("[MagusToolkit] InstantRespawn enabled – Respawntime set to 0f.");
			return;
		}
		ConfigManager.Logger.LogWarning("[MagusToolkit] Could not find 'Respawntime' field in PlayerRespawnManager.");
	}
}
