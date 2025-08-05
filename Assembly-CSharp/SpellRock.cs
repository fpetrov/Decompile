using System;
using System.Collections;
using Recognissimo;
using Recognissimo.Components;
using UnityEngine;

// Token: 0x020001A2 RID: 418
public class SpellRock : MonoBehaviour, ISpellCommand
{
	// Token: 0x06001171 RID: 4465 RVA: 0x0004B155 File Offset: 0x00049355
	public void ResetVoiceDetect()
	{
		base.GetComponent<SpeechRecognizer>().Vocabulary.Add("rock");
	}

	// Token: 0x06001172 RID: 4466 RVA: 0x0004B16C File Offset: 0x0004936C
	public string GetSpellName()
	{
		return this.spellname;
	}

	// Token: 0x06001173 RID: 4467 RVA: 0x0004B174 File Offset: 0x00049374
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

	// Token: 0x06001174 RID: 4468 RVA: 0x0004B216 File Offset: 0x00049416
	private IEnumerator restartvoicedetect()
	{
		while (base.GetComponent<SpeechRecognizer>().State != SpeechProcessorState.Inactive)
		{
			yield return null;
		}
		base.GetComponent<SpeechRecognizer>().StartProcessing();
		yield break;
	}

	// Token: 0x04000A1F RID: 2591
	public PlayerInventory pi;

	// Token: 0x04000A20 RID: 2592
	public string spellname = "rock";

	// Token: 0x04000A21 RID: 2593
	private int id = 39;
}
