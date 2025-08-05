using System;
using System.Collections;
using Recognissimo;
using Recognissimo.Components;
using UnityEngine;

// Token: 0x02000005 RID: 5
public class PoofSpell : MonoBehaviour, ISpellCommand
{
	// Token: 0x06000025 RID: 37 RVA: 0x000021EF File Offset: 0x000003EF
	private void Start()
	{
	}

	// Token: 0x06000026 RID: 38 RVA: 0x0000298D File Offset: 0x00000B8D
	public void ResetVoiceDetect()
	{
		base.GetComponent<SpeechRecognizer>().Vocabulary.Add("blink");
		base.GetComponent<SpeechRecognizer>().Vocabulary.Add("blank");
	}

	// Token: 0x06000027 RID: 39 RVA: 0x000029B9 File Offset: 0x00000BB9
	public string GetSpellName()
	{
		return this.spellname;
	}

	// Token: 0x06000028 RID: 40 RVA: 0x000029C4 File Offset: 0x00000BC4
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

	// Token: 0x06000029 RID: 41 RVA: 0x00002A59 File Offset: 0x00000C59
	private IEnumerator restartvoicedetect()
	{
		while (base.GetComponent<SpeechRecognizer>().State != SpeechProcessorState.Inactive)
		{
			yield return null;
		}
		base.GetComponent<SpeechRecognizer>().StartProcessing();
		yield break;
	}

	// Token: 0x04000010 RID: 16
	public PlayerInventory pi;

	// Token: 0x04000011 RID: 17
	public string spellname;

	// Token: 0x04000012 RID: 18
	private int id = 34;
}
