using System;
using System.Collections;
using Recognissimo;
using Recognissimo.Components;
using UnityEngine;

// Token: 0x020001A0 RID: 416
public class SpellHolyLight : MonoBehaviour, ISpellCommand
{
	// Token: 0x06001165 RID: 4453 RVA: 0x000021EF File Offset: 0x000003EF
	private void Start()
	{
	}

	// Token: 0x06001166 RID: 4454 RVA: 0x0004B00D File Offset: 0x0004920D
	public void ResetVoiceDetect()
	{
		base.GetComponent<SpeechRecognizer>().Vocabulary.Add("divine");
	}

	// Token: 0x06001167 RID: 4455 RVA: 0x0004B024 File Offset: 0x00049224
	public string GetSpellName()
	{
		return this.spellname;
	}

	// Token: 0x06001168 RID: 4456 RVA: 0x0004B02C File Offset: 0x0004922C
	public void TryCastSpell()
	{
		PlayerInventory playerInventory;
		if (this.pi == null && Camera.main.transform.parent != null && Camera.main.transform.parent.TryGetComponent<PlayerInventory>(out playerInventory))
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

	// Token: 0x06001169 RID: 4457 RVA: 0x0004B0C1 File Offset: 0x000492C1
	private IEnumerator restartvoicedetect()
	{
		while (base.GetComponent<SpeechRecognizer>().State != SpeechProcessorState.Inactive)
		{
			yield return null;
		}
		base.GetComponent<SpeechRecognizer>().StartProcessing();
		yield break;
	}

	// Token: 0x04000A19 RID: 2585
	public PlayerInventory pi;

	// Token: 0x04000A1A RID: 2586
	public string spellname;

	// Token: 0x04000A1B RID: 2587
	private int id = 35;
}
