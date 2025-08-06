using System;
using System.Collections;
using HarmonyLib;
using UnityEngine;

// Token: 0x02000016 RID: 22
[HarmonyPatch(typeof(BlinkSpellController), "PlayerSetup")]
public class Patch_BlinkSpellController_PlayerSetup
{
	// Token: 0x06000037 RID: 55 RVA: 0x000034A8 File Offset: 0x000016A8
	private static bool Prefix(BlinkSpellController __instance, GameObject ownerobj, Vector3 fwdvector, int Level)
	{
		LayerMask ground = __instance.ground;
		float value = ConfigManager.BlinkMultiplier.Value;
		float num = 15f * value;
		float num2 = 5f * value;
		float num3 = 50f;
		Vector3 forward = ownerobj.transform.forward;
		RaycastHit raycastHit;
		Vector3 vector;
		if (Physics.Raycast(ownerobj.transform.position + forward * num + Vector3.up * 10f, Vector3.down, ref raycastHit, num3, ground) && !raycastHit.transform.CompareTag("blockblink"))
		{
			vector = raycastHit.point;
		}
		else if (Physics.Raycast(ownerobj.transform.position + forward * num2 + Vector3.up * 5f, Vector3.down, ref raycastHit, num3, ground) && !raycastHit.transform.CompareTag("blockblink"))
		{
			vector = raycastHit.point;
		}
		else if (Physics.Raycast(ownerobj.transform.position + Vector3.up * 1.3f, fwdvector, ref raycastHit, 20f, ground) && !raycastHit.transform.CompareTag("blockblink"))
		{
			vector = raycastHit.point;
		}
		else
		{
			vector = ownerobj.transform.position;
		}
		GameObject poof = __instance.Poof;
		Object.Instantiate<GameObject>(poof, ownerobj.transform.position, Quaternion.identity);
		if (ownerobj.GetComponent<PlayerMovement>().IsOwner)
		{
			__instance.StartCoroutine((IEnumerator)AccessTools.Method(typeof(BlinkSpellController), "Blink", null, null).Invoke(__instance, new object[] { vector, ownerobj }));
		}
		Object.Instantiate<GameObject>(poof, vector, Quaternion.identity);
		return false;
	}
}
