using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x020001C1 RID: 449
public class TutorialGoblin : MonoBehaviour
{
	// Token: 0x060012A4 RID: 4772 RVA: 0x0004EBB8 File Offset: 0x0004CDB8
	public void Starttheroutines()
	{
		this.sku.SetActive(true);
		this.sku.GetComponent<Renderer>().material.SetFloat("_AlphaRemapMax", 0.33f);
		this.sku.GetComponentInChildren<HDAdditionalLightData>().intensity = 100f;
		this.inited = true;
		base.StartCoroutine(this.TutorialRoutine());
		base.StartCoroutine(this.detectnearby());
	}

	// Token: 0x060012A5 RID: 4773 RVA: 0x0004EC26 File Offset: 0x0004CE26
	private void LateUpdate()
	{
		if (this.inited && Camera.main != null)
		{
			this.spine03.LookAt(Camera.main.transform);
		}
	}

	// Token: 0x060012A6 RID: 4774 RVA: 0x0004EC52 File Offset: 0x0004CE52
	private IEnumerator detectnearby()
	{
		for (;;)
		{
			foreach (Collider collider in Physics.OverlapSphere(base.transform.position, 5f, this.lyrm))
			{
				if ((collider.CompareTag("Fireball") || collider.CompareTag("FrostBolt") || collider.CompareTag("Player") || collider.CompareTag("Magicmissle")) && !this.isTeleporting)
				{
					this.isTeleporting = true;
					base.StartCoroutine(this.TeleRoutine());
				}
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x060012A7 RID: 4775 RVA: 0x0004EC61 File Offset: 0x0004CE61
	private IEnumerator TeleRoutine()
	{
		this.GobAni.SetBool("snap", true);
		base.transform.GetComponent<AudioSource>().PlayOneShot(this.gclips[1]);
		this.GobPoints[this.currentGobPoint].GetComponent<AudioSource>().PlayOneShot(this.gclips[0]);
		yield return new WaitForSeconds(0.25f);
		Object.Instantiate<GameObject>(this.poof, base.transform.position, Quaternion.identity);
		yield return new WaitForSeconds(0.1f);
		this.GetGobPOint();
		base.transform.position = this.GobPoints[this.currentGobPoint].position;
		base.transform.rotation = this.GobPoints[this.currentGobPoint].rotation;
		base.transform.GetComponent<AudioSource>().PlayOneShot(this.gclips[2]);
		this.GobAni.SetBool("snap", false);
		this.isTeleporting = false;
		yield break;
	}

	// Token: 0x060012A8 RID: 4776 RVA: 0x0004EC70 File Offset: 0x0004CE70
	private void GetGobPOint()
	{
		int num = Random.Range(0, 3);
		if (num != this.currentGobPoint)
		{
			this.currentGobPoint = num;
			return;
		}
		if (num == 2)
		{
			this.currentGobPoint--;
			return;
		}
		this.currentGobPoint++;
	}

	// Token: 0x060012A9 RID: 4777 RVA: 0x0004ECB7 File Offset: 0x0004CEB7
	private IEnumerator TutorialRoutine()
	{
		base.StartCoroutine(this.starttext("Scroll to equip your torch"));
		while (!this.hasEquippedTorch)
		{
			yield return null;
		}
		base.StartCoroutine(this.SwapFadeinText("Hold LMB to light the brazier"));
		this.GobAni.SetBool("yap", true);
		this.SpeakSource.PlayOneShot(this.clipagios[0]);
		float timer = 0f;
		while (!Input.GetKeyDown(KeyCode.Mouse1) && timer < 2f)
		{
			if (this.hasLitBrazier && !this.hasSkipText)
			{
				this.hasSkipText = true;
				base.StartCoroutine(this.SwapFadeinText("[Press RMB to Continue]"));
			}
			timer += Time.deltaTime;
			yield return null;
		}
		this.SpeakSource.Stop();
		this.SpeakSource.PlayOneShot(this.clipagios[1]);
		timer = 0f;
		while (!Input.GetKeyDown(KeyCode.Mouse1) && timer < 11f)
		{
			if (this.hasLitBrazier && !this.hasSkipText)
			{
				this.hasSkipText = true;
				base.StartCoroutine(this.SwapFadeinText("[Press RMB to Continue]"));
			}
			timer += Time.deltaTime;
			yield return null;
		}
		this.SpeakSource.Stop();
		this.SpeakSource.PlayOneShot(this.clipagios[2]);
		timer = 0f;
		while (!Input.GetKeyDown(KeyCode.Mouse1) && timer < 11f)
		{
			if (this.hasLitBrazier && !this.hasSkipText)
			{
				this.hasSkipText = true;
				base.StartCoroutine(this.SwapFadeinText("[Press RMB to Continue]"));
			}
			timer += Time.deltaTime;
			yield return null;
		}
		yield return null;
		this.GobAni.SetBool("yap", false);
		while (!this.hasLitBrazier)
		{
			yield return null;
		}
		this.SpeakSource.Stop();
		this.GobAni.SetBool("yap", true);
		this.SpeakSource.PlayOneShot(this.clipagios[3]);
		base.StartCoroutine(this.SwapFadeinText("There are flag poles in each team's base and throughout the map. Hold Q to view the map. [Press RMB to Continue]"));
		timer = 0f;
		while (!Input.GetKeyDown(KeyCode.Mouse1))
		{
			if (timer > 16.5f)
			{
				this.GobAni.SetBool("yap", false);
			}
			timer += Time.deltaTime;
			yield return null;
		}
		yield return null;
		base.StartCoroutine(this.SwapFadeinText("If the flag pole in your base is captured, you will not be resurrected when you die. Recall to your base by pressing B. [Press RMB to Continue]"));
		this.SpeakSource.Stop();
		this.SpeakSource.PlayOneShot(this.clipagios[4]);
		this.GobAni.SetBool("yap", true);
		timer = 0f;
		while (!Input.GetKeyDown(KeyCode.Mouse1))
		{
			if (timer > 7f)
			{
				this.GobAni.SetBool("yap", false);
			}
			timer += Time.deltaTime;
			yield return null;
		}
		yield return null;
		base.StartCoroutine(this.SwapFadeinText("The orb in the bottom of your screen indicates if you will resurrect upon death. A red skull in the orb means you will die forever. [Press RMB to Continue]"));
		this.SpeakSource.Stop();
		this.SpeakSource.PlayOneShot(this.clipagios[5]);
		this.GobAni.SetBool("yap", true);
		timer = 0f;
		while (!Input.GetKeyDown(KeyCode.Mouse1))
		{
			if (timer > 7f)
			{
				this.GobAni.SetBool("yap", false);
			}
			timer += Time.deltaTime;
			yield return null;
		}
		this.SpeakSource.Stop();
		this.SpeakSource.PlayOneShot(this.clipagios[6]);
		this.GobAni.SetBool("yap", true);
		timer = 0f;
		while (!Input.GetKeyDown(KeyCode.Mouse1))
		{
			if (timer > 6f)
			{
				this.GobAni.SetBool("yap", false);
			}
			timer += Time.deltaTime;
			yield return null;
		}
		yield return null;
		this.SpeakSource.Stop();
		this.SpeakSource.PlayOneShot(this.clipagios[7]);
		this.GobAni.SetBool("yap", true);
		base.StartCoroutine(this.SwapFadeinText("To achieve Victory, control the flag pole in your opponents base and eliminate each of your opponents. [Press RMB to Continue]"));
		timer = 0f;
		while (!Input.GetKeyDown(KeyCode.Mouse1))
		{
			if (timer > 17.6f)
			{
				this.GobAni.SetBool("yap", false);
			}
			timer += Time.deltaTime;
			yield return null;
		}
		yield return null;
		this.SpeakSource.Stop();
		this.SpeakSource.PlayOneShot(this.clipagios[8]);
		this.GobAni.SetBool("yap", true);
		base.StartCoroutine(this.SwapFadeinText("Controlling flagpoles around the map grants you resources and XP. [Press RMB to Continue]"));
		timer = 0f;
		while (!Input.GetKeyDown(KeyCode.Mouse1))
		{
			if (timer > 15f)
			{
				this.GobAni.SetBool("yap", false);
			}
			timer += Time.deltaTime;
			yield return null;
		}
		yield return null;
		if (!this.hasCapturedFlag)
		{
			base.StartCoroutine(this.SwapFadeinText("Stand Next to the flag pole to capture it."));
		}
		this.GobAni.SetBool("yap", false);
		while (!this.hasCapturedFlag)
		{
			yield return null;
		}
		this.GobAni.SetBool("yap", true);
		this.SpeakSource.Stop();
		this.SpeakSource.PlayOneShot(this.clipagios[9]);
		timer = 0f;
		while (!Input.GetKeyDown(KeyCode.Mouse1) && timer < 8f)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		yield return null;
		this.SpeakSource.Stop();
		this.SpeakSource.PlayOneShot(this.clipagios[10]);
		base.StartCoroutine(this.SwapFadeinText("Press E to pickup the Frog and Log and place them on the crafting table to craft a Frog Spear."));
		timer = 0f;
		while (!Input.GetKeyDown(KeyCode.Mouse1) && timer < 12f)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		yield return null;
		this.GobAni.SetBool("yap", false);
		while (!this.hasCraftedShite)
		{
			yield return null;
		}
		this.GobAni.SetBool("yap", true);
		this.SpeakSource.Stop();
		this.SpeakSource.PlayOneShot(this.clipagios[11]);
		base.StartCoroutine(this.SwapFadeinText("Equip your spellbook, press 1 to turn to the page Fireball. Then say the word \"Fireball\" to cast Fireball"));
		timer = 0f;
		while (!this.hasCastFireBall)
		{
			timer += Time.deltaTime;
			if (timer > 33f)
			{
				this.GobAni.SetBool("yap", false);
			}
			yield return null;
		}
		this.GobAni.SetBool("yap", true);
		this.SpeakSource.Stop();
		this.SpeakSource.PlayOneShot(this.clipagios[12]);
		base.StartCoroutine(this.SwapFadeinText("Press 2 to turn to the page Frost Bolt. Then say the word \"Freeze\" to cast Frost Bolt"));
		timer = 0f;
		while (!this.hasCastFrostBolt)
		{
			if (timer > 13f)
			{
				this.GobAni.SetBool("yap", false);
			}
			timer += Time.deltaTime;
			yield return null;
		}
		this.GobAni.SetBool("yap", true);
		this.SpeakSource.Stop();
		this.SpeakSource.PlayOneShot(this.clipagios[13]);
		base.StartCoroutine(this.SwapFadeinText("Press 3 to turn to the page Worm Hole. Then say the word \"Worm\" to cast the first half of Worm Hole"));
		timer = 0f;
		while (!this.hasCastWorm)
		{
			if (timer > 13f)
			{
				this.GobAni.SetBool("yap", false);
			}
			timer += Time.deltaTime;
			yield return null;
		}
		this.GobAni.SetBool("yap", true);
		this.SpeakSource.Stop();
		this.SpeakSource.PlayOneShot(this.clipagios[14]);
		base.StartCoroutine(this.SwapFadeinText("Now move to a new location and say the word \"Hole\" to cast the second half of Worm Hole"));
		timer = 0f;
		while (!this.hasCastHole)
		{
			if (timer > 13f)
			{
				this.GobAni.SetBool("yap", false);
			}
			timer += Time.deltaTime;
			yield return null;
		}
		this.GobAni.SetBool("yap", true);
		this.SpeakSource.Stop();
		this.SpeakSource.PlayOneShot(this.clipagios[15]);
		base.StartCoroutine(this.SwapFadeinText("Press 4 to turn to the page Magic Missle. Then say the words \"Magic Missle\" to cast Magic Missle"));
		timer = 0f;
		while (!this.hasCastWard)
		{
			if (timer > 17f)
			{
				this.GobAni.SetBool("yap", false);
			}
			timer += Time.deltaTime;
			yield return null;
		}
		base.StartCoroutine(this.SwapFadeinText("You have passed the Academy of Sorceries final examination. You may now exit the tutorial."));
		this.GobAni.SetBool("yap", true);
		this.SpeakSource.Stop();
		this.SpeakSource.PlayOneShot(this.clipagios[16]);
		yield return new WaitForSeconds(8f);
		this.GobAni.SetBool("yap", false);
		this.ExitTutorial();
		yield break;
	}

	// Token: 0x060012AA RID: 4778 RVA: 0x0004ECC6 File Offset: 0x0004CEC6
	private IEnumerator starttext(string text)
	{
		this.texts[0].text = text;
		this.texts[1].text = text;
		float timer = 0f;
		while (timer < 1f)
		{
			this.cg.alpha = timer;
			timer += Time.deltaTime;
			yield return null;
		}
		yield break;
	}

	// Token: 0x060012AB RID: 4779 RVA: 0x0004ECDC File Offset: 0x0004CEDC
	private IEnumerator SwapFadeinText(string text)
	{
		float timer;
		for (timer = 1f; timer > 0f; timer -= Time.deltaTime)
		{
			this.cg.alpha = timer;
		}
		this.cg.alpha = 0f;
		timer = 0f;
		this.texts[0].text = text;
		this.texts[1].text = text;
		while (timer < 1f)
		{
			this.cg.alpha = timer;
			timer += Time.deltaTime;
			yield return null;
		}
		yield break;
	}

	// Token: 0x060012AC RID: 4780 RVA: 0x0004ECF2 File Offset: 0x0004CEF2
	private void ExitTutorial()
	{
		this.cg.alpha = 0f;
		this.texts[0].text = "Your spells grow stronger...";
		this.texts[1].text = "Your spells grow stronger...";
		Debug.Log("yippee!");
	}

	// Token: 0x04000AC8 RID: 2760
	public Transform spine03;

	// Token: 0x04000AC9 RID: 2761
	public Animator GobAni;

	// Token: 0x04000ACA RID: 2762
	public Transform[] GobPoints;

	// Token: 0x04000ACB RID: 2763
	private int currentGobPoint;

	// Token: 0x04000ACC RID: 2764
	private bool isTeleporting;

	// Token: 0x04000ACD RID: 2765
	public AudioClip[] gclips;

	// Token: 0x04000ACE RID: 2766
	public LayerMask lyrm;

	// Token: 0x04000ACF RID: 2767
	public GameObject poof;

	// Token: 0x04000AD0 RID: 2768
	public bool hasCastFireBall;

	// Token: 0x04000AD1 RID: 2769
	public bool hasCastFrostBolt;

	// Token: 0x04000AD2 RID: 2770
	public bool hasCastWorm;

	// Token: 0x04000AD3 RID: 2771
	public bool hasCastHole;

	// Token: 0x04000AD4 RID: 2772
	public bool hasCastWard;

	// Token: 0x04000AD5 RID: 2773
	public bool hasCapturedFlag;

	// Token: 0x04000AD6 RID: 2774
	public bool hasCraftedShite;

	// Token: 0x04000AD7 RID: 2775
	public bool hasLitBrazier;

	// Token: 0x04000AD8 RID: 2776
	private bool inited;

	// Token: 0x04000AD9 RID: 2777
	public AudioSource SpeakSource;

	// Token: 0x04000ADA RID: 2778
	public AudioClip[] clipagios;

	// Token: 0x04000ADB RID: 2779
	public bool hasEquippedTorch;

	// Token: 0x04000ADC RID: 2780
	public TMP_Text[] texts;

	// Token: 0x04000ADD RID: 2781
	public CanvasGroup cg;

	// Token: 0x04000ADE RID: 2782
	public GameObject sku;

	// Token: 0x04000ADF RID: 2783
	private bool hasSkipText;
}
