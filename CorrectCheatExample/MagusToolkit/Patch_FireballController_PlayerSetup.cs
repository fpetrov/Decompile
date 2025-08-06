using System;
using HarmonyLib;

// Token: 0x02000015 RID: 21
[HarmonyPatch(typeof(FireballController), "PlayerSetup")]
public class Patch_FireballController_PlayerSetup
{
	// Token: 0x06000035 RID: 53 RVA: 0x0000346E File Offset: 0x0000166E
	private static void Prefix(FireballController __instance)
	{
		if (!ConfigManager.FireballEdits.Value)
		{
			return;
		}
		__instance.muzzleVelocity = ConfigManager.FireballVelocity.Value;
		__instance.gravity = ConfigManager.FireballGravity.Value;
	}
}
