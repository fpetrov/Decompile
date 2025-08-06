using System;
using System.Collections;
using HarmonyLib;

// Token: 0x02000009 RID: 9
[HarmonyPatch(typeof(PlayerMovement), "FreezeOverlay")]
public class Patch_FreezeOverlay
{
	// Token: 0x0600001E RID: 30 RVA: 0x00002DFD File Offset: 0x00000FFD
	private static bool Prefix(PlayerMovement __instance, float duration, ref IEnumerator __result)
	{
		__result = Patch_FreezeOverlay.InstantFreezeOverlay(__instance);
		return false;
	}

	// Token: 0x0600001F RID: 31 RVA: 0x00002E08 File Offset: 0x00001008
	private static IEnumerator InstantFreezeOverlay(PlayerMovement instance)
	{
		Patch_FreezeOverlay.<InstantFreezeOverlay>d__1 <InstantFreezeOverlay>d__ = new Patch_FreezeOverlay.<InstantFreezeOverlay>d__1(0);
		<InstantFreezeOverlay>d__.instance = instance;
		return <InstantFreezeOverlay>d__;
	}
}
