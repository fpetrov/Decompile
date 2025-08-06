using System;
using HarmonyLib;

// Token: 0x02000014 RID: 20
[HarmonyPatch(typeof(PlayerInventory), "PlayerDied")]
public class Patch_PlayerInventory_PlayerDied
{
	// Token: 0x06000033 RID: 51 RVA: 0x00003430 File Offset: 0x00001630
	private static bool Prefix(PlayerInventory __instance)
	{
		if (!ConfigManager.KeepItems.Value)
		{
			return true;
		}
		PlayerMovement component = __instance.GetComponent<PlayerMovement>();
		return !(component != null) || !component.IsOwner;
	}
}
