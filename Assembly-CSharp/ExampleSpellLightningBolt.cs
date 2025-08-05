using System;
using System.Collections;
using Recognissimo;
using Recognissimo.Components;
using UnityEngine;

// Token: 0x0200006F RID: 111
public class ExampleSpellLightningBolt : MonoBehaviour, ISpellCommand
{
	// Token: 0x060004BB RID: 1211 RVA: 0x00012CB7 File Offset: 0x00010EB7
	public void ResetVoiceDetect()
	{
		base.GetComponent<SpeechRecognizer>().Vocabulary.Add("thunderbolt");
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x00012CCE File Offset: 0x00010ECE
	public string GetSpellName()
	{
		return this.spellname;
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x00012CD8 File Offset: 0x00010ED8
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

	// Token: 0x060004BE RID: 1214 RVA: 0x00012D7A File Offset: 0x00010F7A
	private IEnumerator restartvoicedetect()
	{
		while (base.GetComponent<SpeechRecognizer>().State != SpeechProcessorState.Inactive)
		{
			yield return null;
		}
		base.GetComponent<SpeechRecognizer>().StartProcessing();
		yield break;
	}

	// Token: 0x04000249 RID: 585
	public PlayerInventory pi;

	// Token: 0x0400024A RID: 586
	public string spellname;

	// Token: 0x0400024B RID: 587
	private int id = 33;
}
