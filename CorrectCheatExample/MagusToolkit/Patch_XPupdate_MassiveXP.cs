using System;
using HarmonyLib;
using UnityEngine;

// Token: 0x02000008 RID: 8
[HarmonyPatch(typeof(PlayerMovement), "XPupdate")]
public class Patch_XPupdate_MassiveXP
{
	// Token: 0x0600001B RID: 27 RVA: 0x00002D34 File Offset: 0x00000F34
	private static bool Prefix(PlayerMovement __instance)
	{
		if (!ConfigManager.BoostedXP.Value)
		{
			return true;
		}
		__instance.xp *= (float)ConfigManager.XPMultiplier.Value;
		while (__instance.xp >= Patch_XPupdate_MassiveXP.GetRequiredXP(__instance.level))
		{
			__instance.level++;
			PlayerInteract component = Camera.main.GetComponent<PlayerInteract>();
			component.leveluptxt("His power level is over 9000...");
			component.SetLevelNum(__instance.level);
			ConfigManager.Logger.LogInfo(string.Format("[MagusToolkit] New level: {0}", __instance.level));
			__instance.xp -= Patch_XPupdate_MassiveXP.GetRequiredXP(__instance.level - 1);
		}
		return false;
	}

	// Token: 0x0600001C RID: 28 RVA: 0x00002DE4 File Offset: 0x00000FE4
	private static float GetRequiredXP(int level)
	{
		return 3f * (float)Mathf.Clamp(level, 0, 2);
	}
}
