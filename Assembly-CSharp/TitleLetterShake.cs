using System;
using UnityEngine;

// Token: 0x020001B5 RID: 437
public class TitleLetterShake : MonoBehaviour
{
	// Token: 0x06001233 RID: 4659 RVA: 0x0004D434 File Offset: 0x0004B634
	private void Start()
	{
		this.StartPos = base.transform.position;
		this.Lerptarg = this.StartPos + new Vector3(0f, Random.Range(-1f, 1f), 0f);
		this.lerpDuration = Random.Range(0.5f, 4f);
	}

	// Token: 0x06001234 RID: 4660 RVA: 0x0004D498 File Offset: 0x0004B698
	private void Update()
	{
		this.lerpTimer += Time.deltaTime;
		if (this.lerpTimer > this.lerpDuration)
		{
			this.lerpTimer = 0f;
			this.Lerptarg = this.StartPos + new Vector3(0f, Random.Range(-1f, 1f), 0f);
			this.lerpDuration = Random.Range(0.5f, 4f);
		}
		base.transform.position = Vector3.Slerp(base.transform.position, this.Lerptarg, this.lerpTimer);
	}

	// Token: 0x04000A84 RID: 2692
	private Vector3 StartPos;

	// Token: 0x04000A85 RID: 2693
	private Vector3 Lerptarg;

	// Token: 0x04000A86 RID: 2694
	private float lerpDuration;

	// Token: 0x04000A87 RID: 2695
	private float lerpTimer;
}
