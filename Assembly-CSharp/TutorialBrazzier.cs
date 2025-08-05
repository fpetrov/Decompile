using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001BB RID: 443
public class TutorialBrazzier : MonoBehaviour, IInteractableNetworkObj
{
	// Token: 0x0600125B RID: 4699 RVA: 0x0004DA71 File Offset: 0x0004BC71
	public void NetInteract()
	{
		this.ActualInteraction();
	}

	// Token: 0x0600125C RID: 4700 RVA: 0x0004DA79 File Offset: 0x0004BC79
	public void LightBrazier()
	{
		this.lightOrExtinguish = 0;
		if (!this.lit)
		{
			this.NetInteract();
		}
	}

	// Token: 0x0600125D RID: 4701 RVA: 0x0004DA90 File Offset: 0x0004BC90
	public void ExtinguishBrazier()
	{
		this.lightOrExtinguish = 1;
		this.NetInteract();
	}

	// Token: 0x0600125E RID: 4702 RVA: 0x0004DAA0 File Offset: 0x0004BCA0
	public void ActualInteraction()
	{
		if (this.lightOrExtinguish == 0)
		{
			this.lit = true;
			this.fireLight.enabled = true;
			this.fireParticle.SetActive(true);
			base.StartCoroutine(this.LightSounds());
			this.tgob.hasLitBrazier = true;
			return;
		}
		this.lit = false;
		this.fireLight.enabled = false;
		this.fireParticle.SetActive(false);
	}

	// Token: 0x0600125F RID: 4703 RVA: 0x0004DB0D File Offset: 0x0004BD0D
	private IEnumerator LightSounds()
	{
		this.BrazierSource.volume = 0.5f;
		this.BrazierSource.PlayOneShot(this.BrazierClips[0]);
		yield return new WaitForSeconds(0.8f);
		base.StartCoroutine(this.Audiofade(0.1f, 0.5f, 2f));
		this.BrazierSource.clip = this.BrazierClips[1];
		this.BrazierSource.Play();
		yield break;
	}

	// Token: 0x06001260 RID: 4704 RVA: 0x0004DB1C File Offset: 0x0004BD1C
	private IEnumerator Audiofade(float StartVal, float EndVal, float duration)
	{
		float currentTime = 0f;
		while (currentTime < duration)
		{
			currentTime += Time.deltaTime;
			this.BrazierSource.volume = Mathf.Lerp(StartVal, EndVal, currentTime / duration);
			yield return null;
		}
		this.BrazierSource.volume = EndVal;
		if (EndVal == 0f)
		{
			this.BrazierSource.Stop();
		}
		yield break;
	}

	// Token: 0x04000A9D RID: 2717
	public Light fireLight;

	// Token: 0x04000A9E RID: 2718
	public GameObject fireParticle;

	// Token: 0x04000A9F RID: 2719
	private int lightOrExtinguish;

	// Token: 0x04000AA0 RID: 2720
	public AudioSource BrazierSource;

	// Token: 0x04000AA1 RID: 2721
	public AudioClip[] BrazierClips;

	// Token: 0x04000AA2 RID: 2722
	public bool lit;

	// Token: 0x04000AA3 RID: 2723
	public TutorialGoblin tgob;
}
