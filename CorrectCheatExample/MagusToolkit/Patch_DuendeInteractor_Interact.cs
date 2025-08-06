using System;
using HarmonyLib;
using UnityEngine;

// Token: 0x02000017 RID: 23
[HarmonyPatch(typeof(DuendeInteractor), "Interact")]
public class Patch_DuendeInteractor_Interact
{
	// Token: 0x06000039 RID: 57 RVA: 0x0000368C File Offset: 0x0000188C
	private static bool Prefix(DuendeInteractor __instance, GameObject player)
	{
		if (__instance.duend == null || __instance.duend.isTrading || __instance.duend.panic)
		{
			return false;
		}
		PlayerInventory playerInventory;
		if (!player.TryGetComponent<PlayerInventory>(ref playerInventory))
		{
			return false;
		}
		if (playerInventory.isNotHoldingItem())
		{
			return false;
		}
		if (ConfigManager.InstantTrade.Value || Time.time - Traverse.Create(__instance).Field("tradeCDTimer").GetValue<float>() > 35f)
		{
			__instance.duend.interactedWith(player);
			Traverse.Create(__instance).Field("tradeCDTimer").SetValue(Time.time);
		}
		return false;
	}
}
