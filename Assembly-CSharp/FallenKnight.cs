using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200007B RID: 123
public class FallenKnight : MonoBehaviour, IInteractableNetworkObj, IInteractable
{
	// Token: 0x06000533 RID: 1331 RVA: 0x000146C4 File Offset: 0x000128C4
	public void NetInteract()
	{
		if (!this.interacted)
		{
			this.FkN.KillFK();
		}
	}

	// Token: 0x06000534 RID: 1332 RVA: 0x000146DC File Offset: 0x000128DC
	public void ActualInteraction()
	{
		if (!this.interacted)
		{
			this.interacted = true;
			this.KnightAni.SetBool("die", true);
			this.KnightAudio.clip = null;
			this.KnightAudio.Stop();
			base.StartCoroutine(this.OtherKnightDeath());
		}
	}

	// Token: 0x06000535 RID: 1333 RVA: 0x0001472D File Offset: 0x0001292D
	public void Interact(GameObject player)
	{
		if (!this.interacted && !this.hasSpoken)
		{
			this.hasSpoken = true;
			this.FkN.KnightTalk();
		}
	}

	// Token: 0x06000536 RID: 1334 RVA: 0x00014751 File Offset: 0x00012951
	public string DisplayInteractUI(GameObject player)
	{
		if (!this.interacted && !this.hasSpoken)
		{
			return "SpeaK to Knight";
		}
		return "";
	}

	// Token: 0x06000537 RID: 1335 RVA: 0x0001476E File Offset: 0x0001296E
	public void KnightTalk()
	{
		this.KnightAudio.clip = null;
		this.KnightAudio.PlayOneShot(this.KnightSpeak);
		base.StartCoroutine(this.KnightDeath());
	}

	// Token: 0x06000538 RID: 1336 RVA: 0x0001479A File Offset: 0x0001299A
	private IEnumerator KnightDeath()
	{
		this.hasSpoken = true;
		yield return new WaitForSeconds(42f);
		if (!this.interacted)
		{
			this.KnightAni.SetBool("die", true);
			this.KnightAudio.PlayOneShot(this.knightdie[0]);
			yield return new WaitForSeconds(0.15f);
			this.KnightAudio.PlayOneShot(this.knightdie[1]);
		}
		yield break;
	}

	// Token: 0x06000539 RID: 1337 RVA: 0x000147A9 File Offset: 0x000129A9
	private IEnumerator OtherKnightDeath()
	{
		this.KnightAudio.PlayOneShot(this.knightdie[0]);
		yield return new WaitForSeconds(0.15f);
		this.KnightAudio.PlayOneShot(this.knightdie[1]);
		yield break;
	}

	// Token: 0x0400028D RID: 653
	public Animator KnightAni;

	// Token: 0x0400028E RID: 654
	public AudioSource KnightAudio;

	// Token: 0x0400028F RID: 655
	public AudioClip KnightSpeak;

	// Token: 0x04000290 RID: 656
	public AudioClip[] knightdie;

	// Token: 0x04000291 RID: 657
	private bool interacted;

	// Token: 0x04000292 RID: 658
	private bool hasSpoken;

	// Token: 0x04000293 RID: 659
	public FallenKnightNet FkN;
}
