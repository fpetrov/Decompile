using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200008B RID: 139
public class FlagPositionLerper : MonoBehaviour
{
	// Token: 0x060005C6 RID: 1478 RVA: 0x000168F3 File Offset: 0x00014AF3
	public void LerpFlag(bool upordown)
	{
		this.UporDown = upordown;
		base.StartCoroutine(this.LerpFlagRoutine());
	}

	// Token: 0x060005C7 RID: 1479 RVA: 0x00016909 File Offset: 0x00014B09
	private IEnumerator LerpFlagRoutine()
	{
		float timer = 0f;
		float timer2 = 0f;
		while (timer < 0.25f && timer2 < 0.25f)
		{
			if (this.UporDown)
			{
				timer += Time.deltaTime;
				base.transform.localPosition = Vector3.Lerp(this.InitialPostion, this.TargetPosition, timer * 4f);
			}
			else
			{
				timer2 += Time.deltaTime;
				base.transform.localPosition = Vector3.Lerp(this.TargetPosition, this.InitialPostion, timer2 * 4f);
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x040002DB RID: 731
	public Vector3 TargetPosition;

	// Token: 0x040002DC RID: 732
	public Vector3 InitialPostion;

	// Token: 0x040002DD RID: 733
	private bool UporDown;
}
