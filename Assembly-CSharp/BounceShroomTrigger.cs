using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000021 RID: 33
public class BounceShroomTrigger : MonoBehaviour
{
	// Token: 0x0600018C RID: 396 RVA: 0x00007E7C File Offset: 0x0000607C
	private void Start()
	{
		this.mushas = base.GetComponent<AudioSource>();
	}

	// Token: 0x0600018D RID: 397 RVA: 0x00007E8C File Offset: 0x0000608C
	private void OnTriggerEnter(Collider other)
	{
		PlayerMovement playerMovement;
		if (other.gameObject.CompareTag("Player") && Time.time - this.lastTrigger > 1f && other.gameObject.TryGetComponent<PlayerMovement>(out playerMovement) && playerMovement.checkowner())
		{
			this.lastTrigger = Time.time;
			this.mushManager.TriggerMushAnimation(this.mushID);
		}
	}

	// Token: 0x0600018E RID: 398 RVA: 0x00007EF1 File Offset: 0x000060F1
	public void MushAnimations()
	{
		this.mushAni.SetBool("bounce", true);
		base.StartCoroutine(this.turnoffthethign());
	}

	// Token: 0x0600018F RID: 399 RVA: 0x00007F11 File Offset: 0x00006111
	private IEnumerator turnoffthethign()
	{
		yield return null;
		this.mushAni.SetBool("bounce", false);
		yield break;
	}

	// Token: 0x040000BC RID: 188
	public int mushID;

	// Token: 0x040000BD RID: 189
	public BounceMushroomManager mushManager;

	// Token: 0x040000BE RID: 190
	public Animator mushAni;

	// Token: 0x040000BF RID: 191
	public AudioSource mushas;

	// Token: 0x040000C0 RID: 192
	private float lastTrigger;
}
