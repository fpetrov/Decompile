using System;
using System.Collections;
using Recognissimo;
using Recognissimo.Components;
using UnityEngine;

// Token: 0x0200019E RID: 414
public class SpellDarkBlast : MonoBehaviour, ISpellCommand
{
	// Token: 0x06001159 RID: 4441 RVA: 0x000021EF File Offset: 0x000003EF
	private void Start()
	{
	}

	// Token: 0x0600115A RID: 4442 RVA: 0x0004AEC4 File Offset: 0x000490C4
	public void ResetVoiceDetect()
	{
		base.GetComponent<SpeechRecognizer>().Vocabulary.Add("dark blast");
	}

	// Token: 0x0600115B RID: 4443 RVA: 0x0004AEDB File Offset: 0x000490DB
	public string GetSpellName()
	{
		return this.spellname;
	}

	// Token: 0x0600115C RID: 4444 RVA: 0x0004AEE4 File Offset: 0x000490E4
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

	// Token: 0x0600115D RID: 4445 RVA: 0x0004AF79 File Offset: 0x00049179
	private IEnumerator restartvoicedetect()
	{
		while (base.GetComponent<SpeechRecognizer>().State != SpeechProcessorState.Inactive)
		{
			yield return null;
		}
		base.GetComponent<SpeechRecognizer>().StartProcessing();
		yield break;
	}

	// Token: 0x04000A13 RID: 2579
	public PlayerInventory pi;

	// Token: 0x04000A14 RID: 2580
	public string spellname;

	// Token: 0x04000A15 RID: 2581
	private int id = 37;
}
