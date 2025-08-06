using System;
using System.Reflection;
using HarmonyLib;

// Token: 0x0200000D RID: 13
[HarmonyPatch(typeof(PlayerMovement), "UpdateGravity")]
public static class Patch_PlayerMovement_FallDamageBypass
{
	// Token: 0x06000027 RID: 39 RVA: 0x00003008 File Offset: 0x00001208
	[HarmonyPostfix]
	public static void Postfix(PlayerMovement __instance)
	{
		if (!ConfigManager.GodMode.Value || Patch_PlayerMovement_FallDamageBypass.fallTimerField == null)
		{
			return;
		}
		if ((float)Patch_PlayerMovement_FallDamageBypass.fallTimerField.GetValue(__instance) > 1.4f)
		{
			Patch_PlayerMovement_FallDamageBypass.fallTimerField.SetValue(__instance, 0f);
		}
	}

	// Token: 0x04000026 RID: 38
	private static readonly FieldInfo fallTimerField = typeof(PlayerMovement).GetField("FallTimer", BindingFlags.Instance | BindingFlags.NonPublic);
}
