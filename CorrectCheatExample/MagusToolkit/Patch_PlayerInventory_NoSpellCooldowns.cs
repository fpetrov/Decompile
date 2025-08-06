using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

// Token: 0x0200000F RID: 15
[HarmonyPatch(typeof(PlayerInventory), "Update")]
public static class Patch_PlayerInventory_NoSpellCooldowns
{
	// Token: 0x0600002B RID: 43 RVA: 0x00003190 File Offset: 0x00001390
	[HarmonyPostfix]
	public static void Postfix(PlayerInventory __instance)
	{
		if (!ConfigManager.NoCooldowns.Value)
		{
			return;
		}
		float num = Time.time - 999f;
		FieldInfo fieldInfo = Patch_PlayerInventory_NoSpellCooldowns.fbcdField;
		if (fieldInfo != null)
		{
			fieldInfo.SetValue(__instance, num);
		}
		FieldInfo fieldInfo2 = Patch_PlayerInventory_NoSpellCooldowns.frostcdField;
		if (fieldInfo2 != null)
		{
			fieldInfo2.SetValue(__instance, num);
		}
		FieldInfo fieldInfo3 = Patch_PlayerInventory_NoSpellCooldowns.wormcdField;
		if (fieldInfo3 != null)
		{
			fieldInfo3.SetValue(__instance, num);
		}
		FieldInfo fieldInfo4 = Patch_PlayerInventory_NoSpellCooldowns.holecdField;
		if (fieldInfo4 != null)
		{
			fieldInfo4.SetValue(__instance, num);
		}
		FieldInfo fieldInfo5 = Patch_PlayerInventory_NoSpellCooldowns.wardcdField;
		if (fieldInfo5 == null)
		{
			return;
		}
		fieldInfo5.SetValue(__instance, num);
	}

	// Token: 0x0400002B RID: 43
	private static readonly FieldInfo fbcdField = typeof(PlayerInventory).GetField("fbcd", BindingFlags.Instance | BindingFlags.NonPublic);

	// Token: 0x0400002C RID: 44
	private static readonly FieldInfo frostcdField = typeof(PlayerInventory).GetField("frostcd", BindingFlags.Instance | BindingFlags.NonPublic);

	// Token: 0x0400002D RID: 45
	private static readonly FieldInfo wormcdField = typeof(PlayerInventory).GetField("wormcd", BindingFlags.Instance | BindingFlags.NonPublic);

	// Token: 0x0400002E RID: 46
	private static readonly FieldInfo holecdField = typeof(PlayerInventory).GetField("holecd", BindingFlags.Instance | BindingFlags.NonPublic);

	// Token: 0x0400002F RID: 47
	private static readonly FieldInfo wardcdField = typeof(PlayerInventory).GetField("wardcd", BindingFlags.Instance | BindingFlags.NonPublic);
}
