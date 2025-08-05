using System;
using System.Collections;
using Recognissimo;
using Recognissimo.Components;
using UnityEngine;

// Token: 0x020001A4 RID: 420
public class SpellWisp : MonoBehaviour, ISpellCommand
{
	// Token: 0x0600117C RID: 4476 RVA: 0x000021EF File Offset: 0x000003EF
	private void Start()
	{
	}

	// Token: 0x0600117D RID: 4477 RVA: 0x0004B2B5 File Offset: 0x000494B5
	public void ResetVoiceDetect()
	{
		base.GetComponent<SpeechRecognizer>().Vocabulary.Add("wisp");
	}

	// Token: 0x0600117E RID: 4478 RVA: 0x0004B2CC File Offset: 0x000494CC
	public string GetSpellName()
	{
		return this.spellname;
	}

	// Token: 0x0600117F RID: 4479 RVA: 0x0004B2D4 File Offset: 0x000494D4
	public void TryCastSpell()
	{
		PlayerInventory playerInventory;
		if (this.pi == null && Camera.main != null && Camera.main.transform.parent != null && Camera.main.transform.parent.TryGetComponent<PlayerInventory>(out playerInventory))
		{
			this.pi = playerInventory;
		}
		if (this.pi != null && this.pi.GetEquippedItemID() == this.id)
		{
			this.pi.cPageSpell();
		}
		base.GetComponent<SpeechRecognizer>().StopProcessing();
		base.StartCoroutine(this.restartvoicedetect());
	}

	// Token: 0x06001180 RID: 4480 RVA: 0x0004B376 File Offset: 0x00049576
	private IEnumerator restartvoicedetect()
	{
		while (base.GetComponent<SpeechRecognizer>().State != SpeechProcessorState.Inactive)
		{
			yield return null;
		}
		base.GetComponent<SpeechRecognizer>().StartProcessing();
		yield break;
	}

	// Token: 0x04000A25 RID: 2597
	public PlayerInventory pi;

	// Token: 0x04000A26 RID: 2598
	public string spellname;

	// Token: 0x04000A27 RID: 2599
	private int id = 38;
}
