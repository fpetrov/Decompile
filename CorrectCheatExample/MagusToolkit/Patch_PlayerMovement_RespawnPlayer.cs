using System;
using HarmonyLib;
using MageArenaManager;

// Token: 0x02000006 RID: 6
[HarmonyPatch(typeof(PlayerMovement), "RespawnPlayer")]
public class Patch_PlayerMovement_RespawnPlayer
{
	// Token: 0x06000017 RID: 23 RVA: 0x00002C5C File Offset: 0x00000E5C
	private static void Postfix(PlayerMovement __instance)
	{
		if (!__instance.IsOwner)
		{
			return;
		}
		if (ConfigManager.NoClip.Value)
		{
			NoClip component = __instance.GetComponent<NoClip>();
			if (component == null)
			{
				__instance.gameObject.AddComponent<NoClip>();
				ConfigManager.Logger.LogInfo("[MagusToolkit] NoClip component re-attached after respawn.");
				return;
			}
			component.enabled = true;
			ConfigManager.Logger.LogInfo("[MagusToolkit] NoClip component already present, re-enabled.");
		}
	}
}
