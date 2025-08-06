using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

// Token: 0x0200000E RID: 14
[HarmonyPatch(typeof(PageController), "CastSpell")]
public static class Patch_PageController_SkipCooldown
{
	// Token: 0x06000029 RID: 41 RVA: 0x00003078 File Offset: 0x00001278
	[HarmonyPostfix]
	public static void Postfix(PageController __instance)
	{
		if (Patch_PageController_SkipCooldown.cooldownField == null || Patch_PageController_SkipCooldown.timerField == null || Patch_PageController_SkipCooldown.pageMatField == null || Patch_PageController_SkipCooldown.emissiveValField == null)
		{
			return;
		}
		Patch_PageController_SkipCooldown.timerField.SetValue(__instance, (float)Patch_PageController_SkipCooldown.cooldownField.GetValue(__instance));
		Material material = (Material)Patch_PageController_SkipCooldown.pageMatField.GetValue(__instance);
		float num = (float)Patch_PageController_SkipCooldown.emissiveValField.GetValue(__instance);
		if (material != null)
		{
			material.SetFloat("_emissi", num);
		}
	}

	// Token: 0x04000027 RID: 39
	private static readonly FieldInfo cooldownField = typeof(PageController).GetField("CoolDown", BindingFlags.Instance | BindingFlags.Public);

	// Token: 0x04000028 RID: 40
	private static readonly FieldInfo timerField = typeof(PageController).GetField("PageCoolDownTimer", BindingFlags.Instance | BindingFlags.Public);

	// Token: 0x04000029 RID: 41
	private static readonly FieldInfo pageMatField = typeof(PageController).GetField("PageMaterial", BindingFlags.Instance | BindingFlags.NonPublic);

	// Token: 0x0400002A RID: 42
	private static readonly FieldInfo emissiveValField = typeof(PageController).GetField("PageEmissiveVal", BindingFlags.Instance | BindingFlags.Public);
}
