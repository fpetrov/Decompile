using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001E4 RID: 484
public class BrazierInteract : MonoBehaviour, IInteractableNetworkObj
{
	// Token: 0x0600139E RID: 5022 RVA: 0x000527BC File Offset: 0x000509BC
	private void Start()
	{
		this.NetItemManager = GameObject.FindGameObjectWithTag("NetItemManager").GetComponent<NetInteractionManager>();
		this.objID = this.NetItemManager.AddToList(base.gameObject);
	}

	// Token: 0x0600139F RID: 5023 RVA: 0x000527EA File Offset: 0x000509EA
	public void NetInteract()
	{
		this.NetItemManager.NetObjectInteraction(this.objID);
	}

	// Token: 0x060013A0 RID: 5024 RVA: 0x000527FD File Offset: 0x000509FD
	public void LightBrazier()
	{
		this.lightOrExtinguish = 0;
		if (!this.lit)
		{
			this.NetInteract();
		}
	}

	// Token: 0x060013A1 RID: 5025 RVA: 0x00052814 File Offset: 0x00050A14
	public void ExtinguishBrazier()
	{
		this.lightOrExtinguish = 1;
		this.NetInteract();
	}

	// Token: 0x060013A2 RID: 5026 RVA: 0x00052824 File Offset: 0x00050A24
	public void ActualInteraction()
	{
		if (this.lightOrExtinguish == 0)
		{
			this.lit = true;
			this.fireLight.enabled = true;
			this.fireParticle.SetActive(true);
			base.StartCoroutine(this.LightSounds());
			return;
		}
		this.lit = false;
		this.fireLight.enabled = false;
		this.fireParticle.SetActive(false);
	}

	// Token: 0x060013A3 RID: 5027 RVA: 0x00052885 File Offset: 0x00050A85
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

	// Token: 0x060013A4 RID: 5028 RVA: 0x00052894 File Offset: 0x00050A94
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

	// Token: 0x04000B7C RID: 2940
	private NetInteractionManager NetItemManager;

	// Token: 0x04000B7D RID: 2941
	private int objID;

	// Token: 0x04000B7E RID: 2942
	public Light fireLight;

	// Token: 0x04000B7F RID: 2943
	public GameObject fireParticle;

	// Token: 0x04000B80 RID: 2944
	private int lightOrExtinguish;

	// Token: 0x04000B81 RID: 2945
	public AudioSource BrazierSource;

	// Token: 0x04000B82 RID: 2946
	public AudioClip[] BrazierClips;

	// Token: 0x04000B83 RID: 2947
	public bool lit;
}
