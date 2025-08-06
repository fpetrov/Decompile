using System;
using System.Collections;
using HarmonyLib;
using MagusToolkit;

// Token: 0x02000005 RID: 5
[HarmonyPatch(typeof(PlayerMovement), "StartGame")]
public class Patch_StartGame
{
	// Token: 0x06000014 RID: 20 RVA: 0x00002C22 File Offset: 0x00000E22
	private static void Postfix(PlayerMovement __instance)
	{
		if (!__instance.IsOwner)
		{
			return;
		}
		Globals.LocalPlayer = __instance;
		GameHelper.StartLevelForcer();
		__instance.StartCoroutine(Patch_StartGame.DelayedInitToolkitFeatures(__instance));
	}

	// Token: 0x06000015 RID: 21 RVA: 0x00002C45 File Offset: 0x00000E45
	private static IEnumerator DelayedInitToolkitFeatures(PlayerMovement player)
	{
		Patch_StartGame.<DelayedInitToolkitFeatures>d__1 <DelayedInitToolkitFeatures>d__ = new Patch_StartGame.<DelayedInitToolkitFeatures>d__1(0);
		<DelayedInitToolkitFeatures>d__.player = player;
		return <DelayedInitToolkitFeatures>d__;
	}
}
