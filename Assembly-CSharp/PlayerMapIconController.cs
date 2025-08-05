using System;
using TMPro;
using UnityEngine;

// Token: 0x02000121 RID: 289
public class PlayerMapIconController : MonoBehaviour
{
	// Token: 0x06000BCD RID: 3021 RVA: 0x0002DC84 File Offset: 0x0002BE84
	private void Update()
	{
		if (this.playerpos != null)
		{
			Vector3 vector = new Vector3(this.Remap(this.playerpos.position.z, -250f, 480f, this.valuerange[0], this.valuerange[1]), this.Remap(this.playerpos.position.x, -200f, 340f, this.valuerange[2], this.valuerange[3]), 0f);
			base.transform.localPosition = vector;
		}
	}

	// Token: 0x06000BCE RID: 3022 RVA: 0x0002DD18 File Offset: 0x0002BF18
	private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
	{
		fromMax -= fromMin;
		value -= fromMin;
		float num = value / fromMax;
		return Mathf.Lerp(toMin, toMax, num);
	}

	// Token: 0x06000BCF RID: 3023 RVA: 0x0002DD3D File Offset: 0x0002BF3D
	public void setname(string name)
	{
		this.text.text = name;
	}

	// Token: 0x0400063A RID: 1594
	public Transform playerpos;

	// Token: 0x0400063B RID: 1595
	public TMP_Text text;

	// Token: 0x0400063C RID: 1596
	public float[] valuerange = new float[] { -430f, 810f, 440f, -450f };
}
