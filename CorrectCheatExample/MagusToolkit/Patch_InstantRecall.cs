using System;
using System.Collections;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

// Token: 0x02000013 RID: 19
[HarmonyPatch(typeof(PlayerMovement), "RecallRoutine")]
public class Patch_InstantRecall
{
	// Token: 0x06000031 RID: 49 RVA: 0x00003308 File Offset: 0x00001508
	private static bool Prefix(PlayerMovement __instance)
	{
		if (!ConfigManager.InstantRecall.Value)
		{
			return true;
		}
		Type typeFromHandle = typeof(PlayerMovement);
		FieldInfo field = typeFromHandle.GetField("recallCD", BindingFlags.Instance | BindingFlags.NonPublic);
		if (field != null)
		{
			field.SetValue(__instance, -999f);
		}
		FieldInfo field2 = typeFromHandle.GetField("playerHealth", BindingFlags.Instance | BindingFlags.NonPublic);
		if (field2 != null && (float)field2.GetValue(__instance) < 100f)
		{
			field2.SetValue(__instance, 100f);
		}
		MethodInfo method = typeFromHandle.GetMethod("visualrecall", BindingFlags.Instance | BindingFlags.NonPublic);
		if (method != null)
		{
			method.Invoke(__instance, null);
		}
		FieldInfo field3 = typeFromHandle.GetField("portalSource", BindingFlags.Instance | BindingFlags.NonPublic);
		FieldInfo field4 = typeFromHandle.GetField("recall", BindingFlags.Instance | BindingFlags.NonPublic);
		AudioSource audioSource = ((field3 != null) ? field3.GetValue(__instance) : null) as AudioSource;
		if (audioSource != null)
		{
			AudioClip audioClip = ((field4 != null) ? field4.GetValue(__instance) : null) as AudioClip;
			if (audioClip != null)
			{
				audioSource.PlayOneShot(audioClip);
			}
		}
		MethodInfo method2 = typeFromHandle.GetMethod("TelePlayer", BindingFlags.Instance | BindingFlags.NonPublic);
		if (method2 != null)
		{
			__instance.StartCoroutine((IEnumerator)method2.Invoke(__instance, null));
		}
		return false;
	}
}
