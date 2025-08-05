using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x020001D5 RID: 469
public class WeatherCycle : MonoBehaviour
{
	// Token: 0x0600132D RID: 4909 RVA: 0x00050ED7 File Offset: 0x0004F0D7
	private void Start()
	{
		this.hdldmoon = this.Moon.GetComponent<HDAdditionalLightData>();
		this.hdldsun = this.Sun.GetComponent<HDAdditionalLightData>();
	}

	// Token: 0x0600132E RID: 4910 RVA: 0x00050EFB File Offset: 0x0004F0FB
	public void setrot(float roto)
	{
		this.rot = roto;
	}

	// Token: 0x0600132F RID: 4911 RVA: 0x00050F04 File Offset: 0x0004F104
	public void ResetSuns()
	{
		this.hdldmoon.EnableShadows(false);
		this.hdldsun.EnableShadows(true);
		this.hdldsun.intensity = 20000f;
		this.hdldmoon.intensity = 0f;
		base.gameObject.SetActive(false);
		this.rot = 40f;
		this.isNight = false;
	}

	// Token: 0x06001330 RID: 4912 RVA: 0x00050F67 File Offset: 0x0004F167
	public void StartSun()
	{
		base.transform.rotation = Quaternion.Euler(135.8f, 40.2f, 0f);
		base.StartCoroutine(this.SunMove());
	}

	// Token: 0x06001331 RID: 4913 RVA: 0x00050F95 File Offset: 0x0004F195
	private IEnumerator SunMove()
	{
		for (;;)
		{
			this.rot += Time.deltaTime / this.sunMulti;
			base.transform.rotation = Quaternion.Euler(this.rot, base.transform.rotation.y, base.transform.rotation.z);
			if ((int)this.rot <= 10 && this.toggle)
			{
				this.toggle = false;
				base.StartCoroutine(this.MoonOff());
			}
			if ((int)this.rot >= 150 && !this.toggle)
			{
				this.toggle = true;
				base.StartCoroutine(this.MoonOn());
			}
			if ((int)this.rot >= 360)
			{
				this.rot = 0f;
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x06001332 RID: 4914 RVA: 0x00050FA4 File Offset: 0x0004F1A4
	private IEnumerator MoonOn()
	{
		this.sunMulti = 2f;
		while (this.rot < 170f)
		{
			yield return null;
		}
		while (this.hdldsun.intensity > 0f)
		{
			this.hdldsun.intensity -= 4000f * Time.deltaTime;
			yield return null;
		}
		this.hdldsun.EnableShadows(false);
		this.hdldmoon.EnableShadows(true);
		while (this.hdldmoon.intensity < 900f)
		{
			this.hdldmoon.intensity += 20f * Time.deltaTime;
			Mathf.Clamp(this.hdldmoon.intensity, 0f, 700f);
			yield return null;
		}
		this.toggle = true;
		this.isNight = true;
		yield break;
	}

	// Token: 0x06001333 RID: 4915 RVA: 0x00050FB3 File Offset: 0x0004F1B3
	private IEnumerator MoonOff()
	{
		this.sunMulti = 3.5f;
		while (this.hdldmoon.intensity > 0f)
		{
			this.hdldmoon.intensity -= 20f * Time.deltaTime;
			yield return null;
		}
		this.hdldmoon.EnableShadows(false);
		this.hdldsun.EnableShadows(true);
		while (this.hdldsun.intensity < 25000f)
		{
			this.hdldsun.intensity += 4000f * Time.deltaTime;
			Mathf.Clamp(this.hdldsun.intensity, 0f, 25000f);
			yield return null;
		}
		this.toggle = false;
		this.isNight = false;
		yield break;
	}

	// Token: 0x04000B2C RID: 2860
	private float sunMulti = 3f;

	// Token: 0x04000B2D RID: 2861
	private float rot = 40f;

	// Token: 0x04000B2E RID: 2862
	public Light Sun;

	// Token: 0x04000B2F RID: 2863
	public Light Moon;

	// Token: 0x04000B30 RID: 2864
	private HDAdditionalLightData hdldsun;

	// Token: 0x04000B31 RID: 2865
	private HDAdditionalLightData hdldmoon;

	// Token: 0x04000B32 RID: 2866
	private bool toggle;

	// Token: 0x04000B33 RID: 2867
	public bool isNight;
}
