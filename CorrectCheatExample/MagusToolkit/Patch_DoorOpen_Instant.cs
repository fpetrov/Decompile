using System;
using HarmonyLib;

// Token: 0x02000011 RID: 17
[HarmonyPatch(typeof(DoorOpen), "GetInteractTimer")]
public static class Patch_DoorOpen_Instant
{
	// Token: 0x0600002E RID: 46 RVA: 0x000032D4 File Offset: 0x000014D4
	private static bool Prefix(ref float __result)
	{
		if (ConfigManager.InstantDoors.Value)
		{
			__result = 0f;
			return false;
		}
		return true;
	}
}
