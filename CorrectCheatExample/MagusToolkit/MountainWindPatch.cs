using System;
using HarmonyLib;
using UnityEngine;

// Token: 0x02000012 RID: 18
[HarmonyPatch(typeof(PlayerMovement), "MountainWind")]
public class MountainWindPatch
{
	// Token: 0x0600002F RID: 47 RVA: 0x000032EC File Offset: 0x000014EC
	private static bool Prefix(ref Transform LerpTarg, ref float strength)
	{
		return !ConfigManager.DisableWind.Value;
	}
}
