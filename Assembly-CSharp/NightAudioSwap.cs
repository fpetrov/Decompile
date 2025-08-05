using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000F8 RID: 248
public class NightAudioSwap : MonoBehaviour
{
	// Token: 0x06000A10 RID: 2576 RVA: 0x0002663C File Offset: 0x0002483C
	private void Start()
	{
		base.StartCoroutine(this.GetSun());
	}

	// Token: 0x06000A11 RID: 2577 RVA: 0x0002663C File Offset: 0x0002483C
	public void Getdasun()
	{
		base.StartCoroutine(this.GetSun());
	}

	// Token: 0x06000A12 RID: 2578 RVA: 0x0002664B File Offset: 0x0002484B
	private IEnumerator GetSun()
	{
		GameObject Sunpobj = null;
		while (Sunpobj == null)
		{
			Sunpobj = GameObject.FindGameObjectWithTag("Weather");
			yield return null;
		}
		this.nightday = Sunpobj.GetComponent<WeatherCycle>();
		yield break;
	}

	// Token: 0x06000A13 RID: 2579 RVA: 0x0002665C File Offset: 0x0002485C
	private void Update()
	{
		if (this.nightday != null)
		{
			if (this.nightday.isNight && !this.playingnightsound)
			{
				base.StartCoroutine(this.swaptonight());
				this.playingnightsound = true;
				return;
			}
			if (!this.nightday.isNight && this.playingnightsound)
			{
				base.StartCoroutine(this.swaptoday());
				this.playingnightsound = false;
			}
		}
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x000266CA File Offset: 0x000248CA
	private IEnumerator swaptonight()
	{
		float timer;
		for (timer = 0f; timer < 2f; timer += Time.deltaTime)
		{
			this.asau.volume = Mathf.Lerp(this.daytimeval, 0f, timer / 2f);
			yield return null;
		}
		this.asau.clip = this.daynightaud[1];
		this.asau.Play();
		timer = 0f;
		yield return new WaitForSeconds(3f);
		while (timer < 2f)
		{
			this.asau.volume = Mathf.Lerp(0f, 0.25f, timer / 2f);
			yield return null;
			timer += Time.deltaTime;
		}
		yield break;
	}

	// Token: 0x06000A15 RID: 2581 RVA: 0x000266D9 File Offset: 0x000248D9
	private IEnumerator swaptoday()
	{
		float timer;
		for (timer = 0f; timer < 2f; timer += Time.deltaTime)
		{
			this.asau.volume = Mathf.Lerp(0.25f, 0f, timer / 2f);
			yield return null;
		}
		this.asau.clip = this.daynightaud[0];
		this.asau.Play();
		timer = 0f;
		yield return new WaitForSeconds(3f);
		while (timer < 2f)
		{
			this.asau.volume = Mathf.Lerp(0f, this.daytimeval, timer / 2f);
			yield return null;
			timer += Time.deltaTime;
		}
		yield break;
	}

	// Token: 0x0400054B RID: 1355
	private WeatherCycle nightday;

	// Token: 0x0400054C RID: 1356
	public AudioSource asau;

	// Token: 0x0400054D RID: 1357
	public AudioClip[] daynightaud;

	// Token: 0x0400054E RID: 1358
	public float daytimeval = 0.15f;

	// Token: 0x0400054F RID: 1359
	private bool playingnightsound;
}
