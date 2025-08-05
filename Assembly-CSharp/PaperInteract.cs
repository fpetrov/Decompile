using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000103 RID: 259
public class PaperInteract : MonoBehaviour, IInteractable
{
	// Token: 0x06000A78 RID: 2680 RVA: 0x000278CE File Offset: 0x00025ACE
	private void Start()
	{
		this.paperStartPos = this.paper.transform.localPosition;
	}

	// Token: 0x06000A79 RID: 2681 RVA: 0x000278E6 File Offset: 0x00025AE6
	public string DisplayInteractUI(GameObject player)
	{
		if (!this.isReading)
		{
			return "Read Crafting Manual";
		}
		if (this.hidetexttimer < 3f)
		{
			return "Move to Close";
		}
		return "";
	}

	// Token: 0x06000A7A RID: 2682 RVA: 0x00027910 File Offset: 0x00025B10
	public void Interact(GameObject player)
	{
		if (!this.isReading)
		{
			this.asou.PlayOneShot(this.aclip);
			this.paper.transform.localPosition = this.paperStartPos;
			this.hidetexttimer = 0f;
			this.paper.SetActive(true);
			PlayerMovement component = player.GetComponent<PlayerMovement>();
			component.canMoveCamera = false;
			this.isReading = true;
			base.StartCoroutine(this.ReadRoutine(component));
		}
	}

	// Token: 0x06000A7B RID: 2683 RVA: 0x00027986 File Offset: 0x00025B86
	private IEnumerator ReadRoutine(PlayerMovement pm)
	{
		Vector3 startpos = pm.transform.position;
		while (this.isReading)
		{
			if (Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.Escape) || Vector3.Distance(pm.transform.position, startpos) > 0.2f)
			{
				this.isReading = false;
			}
			this.hidetexttimer += Time.deltaTime;
			yield return null;
		}
		this.asou.PlayOneShot(this.aclip);
		this.paper.SetActive(false);
		this.hidetexttimer = 0f;
		pm.canMoveCamera = true;
		yield break;
	}

	// Token: 0x0400058A RID: 1418
	public GameObject paper;

	// Token: 0x0400058B RID: 1419
	private bool isReading;

	// Token: 0x0400058C RID: 1420
	private float hidetexttimer;

	// Token: 0x0400058D RID: 1421
	private Vector3 paperStartPos;

	// Token: 0x0400058E RID: 1422
	public AudioSource asou;

	// Token: 0x0400058F RID: 1423
	public AudioClip aclip;
}
