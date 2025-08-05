using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001DB RID: 475
public class WormholeTele : MonoBehaviour
{
	// Token: 0x06001351 RID: 4945 RVA: 0x00051931 File Offset: 0x0004FB31
	private void Start()
	{
		this.holeid = Random.Range(0, 100000000);
		this.resizeWormhole();
	}

	// Token: 0x06001352 RID: 4946 RVA: 0x0005194C File Offset: 0x0004FB4C
	private void Update()
	{
		if (Camera.main != null)
		{
			Vector3 position = Camera.main.transform.position;
			position.y = base.transform.position.y;
			base.transform.LookAt(position);
		}
	}

	// Token: 0x06001353 RID: 4947 RVA: 0x0005199C File Offset: 0x0004FB9C
	private void OnTriggerEnter(Collider other)
	{
		PlayerMovement playerMovement;
		if (other.CompareTag("Player") && other.TryGetComponent<PlayerMovement>(out playerMovement) && this.WormHolePair != null)
		{
			other.transform.position = this.WormHolePair.transform.position;
			base.StartCoroutine(this.TelePlayer(other.gameObject));
			playerMovement.playPortalSound();
			return;
		}
		if (other.CompareTag("Fireball") && this.WormHolePair != null && !other.GetComponent<FireballController>().tpeders.Contains(this.holeid))
		{
			other.GetComponent<FireballController>().addtolist(this.holeid);
			if (this.WormHolePair != null)
			{
				other.GetComponent<FireballController>().addtolist(this.WormHolePair.GetComponent<WormholeTele>().holeid);
			}
			other.GetComponent<FireballController>().hastped = true;
			other.transform.position = base.transform.position;
			base.StartCoroutine(this.telefb(other.gameObject));
		}
	}

	// Token: 0x06001354 RID: 4948 RVA: 0x00051AAB File Offset: 0x0004FCAB
	private IEnumerator telefb(GameObject fb)
	{
		float timer = 0f;
		while (timer < 0.3f)
		{
			yield return new WaitForEndOfFrame();
			fb.transform.position = base.transform.position;
			timer += Time.deltaTime;
			yield return null;
		}
		timer = 0f;
		while (timer < 0.3f)
		{
			yield return new WaitForEndOfFrame();
			fb.transform.position = this.WormHolePair.transform.position;
			timer += Time.deltaTime;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06001355 RID: 4949 RVA: 0x00051AC1 File Offset: 0x0004FCC1
	private IEnumerator TelePlayer(GameObject player)
	{
		this.WormHolePair.GetComponent<Collider>().enabled = false;
		int num;
		for (int i = 0; i < 15; i = num + 1)
		{
			if (this.WormHolePair != null)
			{
				player.transform.position = this.WormHolePair.transform.position;
			}
			yield return null;
			num = i;
		}
		yield return new WaitForSeconds(1f);
		if (this.WormHolePair != null)
		{
			this.WormHolePair.GetComponent<Collider>().enabled = true;
		}
		yield break;
	}

	// Token: 0x06001356 RID: 4950 RVA: 0x00051AD7 File Offset: 0x0004FCD7
	public void resizeWormhole()
	{
		this.wsource.PlayOneShot(this.pclip);
		base.transform.localScale = new Vector3(0f, 0f, 0f);
		base.StartCoroutine(this.ResizeRoutine());
	}

	// Token: 0x06001357 RID: 4951 RVA: 0x00051B16 File Offset: 0x0004FD16
	private IEnumerator ResizeRoutine()
	{
		float timer = 0f;
		while (timer < 1f)
		{
			timer += Time.deltaTime;
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, new Vector3(1f, 1f, 1f), timer);
			yield return null;
		}
		yield break;
	}

	// Token: 0x06001358 RID: 4952 RVA: 0x00051B25 File Offset: 0x0004FD25
	public void DestroyInSeconds(int seconds)
	{
		base.StartCoroutine(this.Destroysoon(seconds));
	}

	// Token: 0x06001359 RID: 4953 RVA: 0x00051B35 File Offset: 0x0004FD35
	private IEnumerator Destroysoon(int seconds)
	{
		yield return new WaitForSeconds((float)seconds);
		float timer = 0f;
		while (timer < 1f)
		{
			timer += Time.deltaTime;
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, new Vector3(0f, 0f, 0f), timer);
			yield return null;
		}
		if (this.mbc != null)
		{
			this.mbc.CallDestroyWormHole(this.isHole);
		}
		else
		{
			this.DestroyWormhole();
		}
		yield break;
	}

	// Token: 0x0600135A RID: 4954 RVA: 0x00051B4B File Offset: 0x0004FD4B
	public void DestroyWormhole()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04000B4D RID: 2893
	public GameObject WormHolePair;

	// Token: 0x04000B4E RID: 2894
	public AudioSource wsource;

	// Token: 0x04000B4F RID: 2895
	public AudioClip pclip;

	// Token: 0x04000B50 RID: 2896
	public MageBookController mbc;

	// Token: 0x04000B51 RID: 2897
	public int holeid;

	// Token: 0x04000B52 RID: 2898
	public bool isHole;
}
