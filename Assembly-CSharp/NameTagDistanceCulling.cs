using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000F4 RID: 244
public class NameTagDistanceCulling : MonoBehaviour
{
	// Token: 0x060009FE RID: 2558 RVA: 0x000264D8 File Offset: 0x000246D8
	public void StartDistRoutine()
	{
		base.StartCoroutine(this.distroutine());
	}

	// Token: 0x060009FF RID: 2559 RVA: 0x000264E7 File Offset: 0x000246E7
	private IEnumerator distroutine()
	{
		while (base.isActiveAndEnabled)
		{
			if (this.playerani.GetBool("crouch"))
			{
				this.holder.SetActive(false);
			}
			else
			{
				this.holder.SetActive(true);
				float num = Vector3.Distance(Camera.main.transform.position, base.transform.position);
				float num2 = Mathf.Lerp(0.4f, 0f, Mathf.Clamp01(num / this.dist));
				this.hpbarmat.material.SetFloat("_alpha", num2);
				this.cg.alpha = num2;
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000541 RID: 1345
	public float dist;

	// Token: 0x04000542 RID: 1346
	public MeshRenderer hpbarmat;

	// Token: 0x04000543 RID: 1347
	public CanvasGroup cg;

	// Token: 0x04000544 RID: 1348
	public GameObject holder;

	// Token: 0x04000545 RID: 1349
	public Animator playerani;
}
