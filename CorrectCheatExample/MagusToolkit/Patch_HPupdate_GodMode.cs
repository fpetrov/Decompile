using System;
using System.Reflection;
using HarmonyLib;

// Token: 0x0200000C RID: 12
[HarmonyPatch(typeof(PlayerMovement), "HPupdate")]
public static class Patch_HPupdate_GodMode
{
	// Token: 0x06000026 RID: 38 RVA: 0x00002FA0 File Offset: 0x000011A0
	public static bool Prefix(PlayerMovement __instance, float val)
	{
		if (ConfigManager.GodMode.Value)
		{
			typeof(PlayerMovement).GetField("playerHealth", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(__instance, 100f);
			typeof(PlayerMovement).GetField("prevhp", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(__instance, 100f);
			return false;
		}
		return true;
	}
}
