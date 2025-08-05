using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000162 RID: 354
public class RockSpellController : MonoBehaviour
{
	// Token: 0x06000F04 RID: 3844 RVA: 0x0003C880 File Offset: 0x0003AA80
	public void StartRockRoutine(GameObject playerOwner, int level)
	{
		this.spelllevel = level;
		if (this.spelllevel < 2)
		{
			this.rcs.radius = 5f;
			this.RockVisual.transform.localScale = new Vector3(452.3f, 353.5f, 297f);
		}
		else if (this.spelllevel >= 5)
		{
			this.rcs.radius = 7f;
			this.RockVisual.transform.localScale = new Vector3(752.3f, 588.03f, 494.02f);
		}
		base.StartCoroutine(this.RockRoutine());
	}

	// Token: 0x06000F05 RID: 3845 RVA: 0x0003C91D File Offset: 0x0003AB1D
	private IEnumerator RockRoutine()
	{
		float timer = 0f;
		Vector3 Startpos = this.RockVisual.transform.localPosition;
		foreach (Collider collider in Physics.OverlapSphere(base.transform.position, 100f, this.PlayerLayer))
		{
			PlayerMovement playerMovement;
			if (collider.CompareTag("Player") && collider.gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
			{
				float num = Mathf.Clamp(Vector3.Distance(base.transform.position, collider.transform.position), 0f, 25f);
				playerMovement.ShakeCam((40f - num) / 10f, (40f - num) / 10f);
			}
		}
		while (timer < 0.165f)
		{
			yield return null;
			timer += Time.deltaTime;
			this.RockVisual.transform.localPosition = Vector3.Lerp(Startpos, this.EndPostion, timer / 0.165f);
		}
		yield return new WaitForSeconds(0.2f);
		this.rockColliders[0].enabled = false;
		this.rockColliders[1].enabled = true;
		this.RockVisual.transform.localPosition = this.EndPostion;
		yield return new WaitForSeconds(25f);
		Vector3 newEndPos = new Vector3(this.EndPostion.x, -100f, this.EndPostion.z);
		this.rocksource.PlayOneShot(this.rockclip);
		while (timer < 15f)
		{
			yield return null;
			timer += Time.deltaTime;
			this.RockVisual.transform.localPosition = Vector3.Lerp(this.EndPostion, newEndPos, timer / 15f);
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x04000844 RID: 2116
	public GameObject RockVisual;

	// Token: 0x04000845 RID: 2117
	public Vector3 EndPostion;

	// Token: 0x04000846 RID: 2118
	private GameObject playerOwner;

	// Token: 0x04000847 RID: 2119
	public AudioSource rocksource;

	// Token: 0x04000848 RID: 2120
	public AudioClip rockclip;

	// Token: 0x04000849 RID: 2121
	public Collider[] rockColliders;

	// Token: 0x0400084A RID: 2122
	public GetRockPlayerOwner grpo;

	// Token: 0x0400084B RID: 2123
	public LayerMask PlayerLayer;

	// Token: 0x0400084C RID: 2124
	public RockCheckSphere rcs;

	// Token: 0x0400084D RID: 2125
	private int spelllevel;
}
