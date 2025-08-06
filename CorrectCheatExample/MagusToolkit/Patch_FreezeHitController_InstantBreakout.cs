using System;
using System.Collections;
using HarmonyLib;

// Token: 0x0200000A RID: 10
[HarmonyPatch(typeof(FreezeHitController), "ExplosionRoutine")]
public class Patch_FreezeHitController_InstantBreakout
{
	// Token: 0x06000021 RID: 33 RVA: 0x00002E1F File Offset: 0x0000101F
	private static bool Prefix(FreezeHitController __instance, ref IEnumerator __result)
	{
		__result = Patch_FreezeHitController_InstantBreakout.SkipExplosion(__instance);
		return false;
	}

	// Token: 0x06000022 RID: 34 RVA: 0x00002E2A File Offset: 0x0000102A
	private static IEnumerator SkipExplosion(FreezeHitController instance)
	{
		Patch_FreezeHitController_InstantBreakout.<SkipExplosion>d__1 <SkipExplosion>d__ = new Patch_FreezeHitController_InstantBreakout.<SkipExplosion>d__1(0);
		<SkipExplosion>d__.instance = instance;
		return <SkipExplosion>d__;
	}
}
