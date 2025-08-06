using System;
using HarmonyLib;

// Token: 0x02000010 RID: 16
[HarmonyPatch(typeof(PortcullisController), "GetInteractTimer")]
public static class Patch_Portcullis_NoInteractTimer
{
	// Token: 0x0600002D RID: 45 RVA: 0x000032BC File Offset: 0x000014BC
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
