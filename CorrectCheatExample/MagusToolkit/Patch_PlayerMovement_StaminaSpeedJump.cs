using System;
using System.Reflection;
using HarmonyLib;

// Token: 0x0200000B RID: 11
[HarmonyPatch(typeof(PlayerMovement), "UpdateMovement")]
public static class Patch_PlayerMovement_StaminaSpeedJump
{
	// Token: 0x06000024 RID: 36 RVA: 0x00002E44 File Offset: 0x00001044
	[HarmonyPrefix]
	public static void Prefix(PlayerMovement __instance)
	{
		if (ConfigManager.InfiniteStam.Value && Patch_PlayerMovement_StaminaSpeedJump.staminaField != null)
		{
			Patch_PlayerMovement_StaminaSpeedJump.staminaField.SetValue(__instance, 9.99f);
		}
		if (ConfigManager.SpeedMod.Value && Patch_PlayerMovement_StaminaSpeedJump.walkingSpeedField != null && Patch_PlayerMovement_StaminaSpeedJump.runningSpeedField != null)
		{
			float num = 2f;
			float num2 = 9f;
			Patch_PlayerMovement_StaminaSpeedJump.walkingSpeedField.SetValue(__instance, num * ConfigManager.SpeedMultiplier.Value);
			Patch_PlayerMovement_StaminaSpeedJump.runningSpeedField.SetValue(__instance, num2 * ConfigManager.SpeedMultiplier.Value);
		}
		if (ConfigManager.JumpMod.Value && Patch_PlayerMovement_StaminaSpeedJump.jumpSpeedField != null)
		{
			float num3 = 8f;
			Patch_PlayerMovement_StaminaSpeedJump.jumpSpeedField.SetValue(__instance, num3 * ConfigManager.JumpMultiplier.Value);
		}
	}

	// Token: 0x04000022 RID: 34
	private static readonly FieldInfo staminaField = typeof(PlayerMovement).GetField("stamina", BindingFlags.Instance | BindingFlags.NonPublic);

	// Token: 0x04000023 RID: 35
	private static readonly FieldInfo walkingSpeedField = typeof(PlayerMovement).GetField("walkingSpeed", BindingFlags.Instance | BindingFlags.Public);

	// Token: 0x04000024 RID: 36
	private static readonly FieldInfo runningSpeedField = typeof(PlayerMovement).GetField("runningSpeed", BindingFlags.Instance | BindingFlags.Public);

	// Token: 0x04000025 RID: 37
	private static readonly FieldInfo jumpSpeedField = typeof(PlayerMovement).GetField("jumpSpeed", BindingFlags.Instance | BindingFlags.NonPublic);
}
