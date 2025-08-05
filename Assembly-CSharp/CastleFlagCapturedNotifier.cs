using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x02000023 RID: 35
public class CastleFlagCapturedNotifier : MonoBehaviour
{
	// Token: 0x06000197 RID: 407 RVA: 0x00007F8C File Offset: 0x0000618C
	public void StartFlagsShit()
	{
		this.playerteam = Camera.main.transform.parent.gameObject.GetComponent<PlayerMovement>().playerTeam;
		this.Angelwings.SetActive(false);
		this.skull.SetActive(false);
		base.StartCoroutine(this.DeathcheckRoutine());
	}

	// Token: 0x06000198 RID: 408 RVA: 0x00007FE2 File Offset: 0x000061E2
	private IEnumerator DeathcheckRoutine()
	{
		base.StartCoroutine(this.TurnOnSkull(false));
		yield return new WaitForSeconds(1f);
		while (base.isActiveAndEnabled)
		{
			if (this.playerteam == 0)
			{
				if (this.fcon.controlTeam == 1 && this.isSkullon != 1)
				{
					this.isSkullon = 1;
					base.StartCoroutine(this.TurnOnSkull(true));
				}
				else if (this.fcon.controlTeam == 0 && this.isSkullon != 0)
				{
					this.isSkullon = 0;
					base.StartCoroutine(this.TurnOffSkull());
				}
			}
			else if (this.playerteam == 2)
			{
				if (this.fcon2.controlTeam == 0 && this.isSkullon != 1)
				{
					this.isSkullon = 1;
					base.StartCoroutine(this.TurnOnSkull(true));
				}
				else if (this.fcon2.controlTeam == 1 && this.isSkullon != 0)
				{
					this.isSkullon = 0;
					base.StartCoroutine(this.TurnOffSkull());
				}
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000199 RID: 409 RVA: 0x00007FF1 File Offset: 0x000061F1
	public void externalSkulltoggle()
	{
		base.StartCoroutine(this.TurnOnSkull(true));
	}

	// Token: 0x0600019A RID: 410 RVA: 0x00008001 File Offset: 0x00006201
	public void externalWingstoggle()
	{
		base.StartCoroutine(this.TurnOffSkull());
	}

	// Token: 0x0600019B RID: 411 RVA: 0x00008010 File Offset: 0x00006210
	private IEnumerator TurnOnSkull(bool playsound)
	{
		if (!playsound)
		{
			this.skull.GetComponent<AudioSource>().volume = 0f;
		}
		else
		{
			this.skull.GetComponent<AudioSource>().volume = 0.6f;
		}
		float timer = 0f;
		this.skull.SetActive(true);
		Material skullmat = this.skull.GetComponent<Renderer>().material;
		Material skullmat2 = this.Angelwings.GetComponent<Renderer>().materials[0];
		Material skullmat3 = this.Angelwings.GetComponent<Renderer>().materials[1];
		while (timer <= 1f)
		{
			skullmat.SetFloat("_AlphaRemapMax", timer / 3f);
			this.hdld.intensity = Mathf.Lerp(0f, 100f, timer);
			skullmat2.SetFloat("_AlphaRemapMax", 0f - timer / 3f);
			this.hdld2.intensity = Mathf.Lerp(100f, 0f, timer);
			skullmat3.SetFloat("_AlphaRemapMax", 0f - timer / 3f);
			timer += Time.deltaTime;
			yield return null;
		}
		this.Angelwings.SetActive(false);
		yield break;
	}

	// Token: 0x0600019C RID: 412 RVA: 0x00008026 File Offset: 0x00006226
	private IEnumerator TurnOffSkull()
	{
		float timer = 0f;
		this.Angelwings.SetActive(true);
		Material skullmat = this.skull.GetComponent<Renderer>().material;
		Material skullmat2 = this.Angelwings.GetComponent<Renderer>().materials[0];
		Material skullmat3 = this.Angelwings.GetComponent<Renderer>().materials[1];
		while (timer <= 1f)
		{
			skullmat.SetFloat("_AlphaRemapMax", 0f - timer / 3f);
			this.hdld.intensity = Mathf.Lerp(100f, 0f, timer);
			skullmat2.SetFloat("_AlphaRemapMax", timer / 3f);
			this.hdld2.intensity = Mathf.Lerp(0f, 100f, timer);
			skullmat3.SetFloat("_AlphaRemapMax", timer / 3f);
			timer += Time.deltaTime;
			yield return null;
		}
		this.skull.SetActive(false);
		yield break;
	}

	// Token: 0x040000C4 RID: 196
	public FlagController fcon;

	// Token: 0x040000C5 RID: 197
	public FlagController fcon2;

	// Token: 0x040000C6 RID: 198
	public HDAdditionalLightData hdld;

	// Token: 0x040000C7 RID: 199
	public GameObject skull;

	// Token: 0x040000C8 RID: 200
	public HDAdditionalLightData hdld2;

	// Token: 0x040000C9 RID: 201
	public GameObject Angelwings;

	// Token: 0x040000CA RID: 202
	private int playerteam;

	// Token: 0x040000CB RID: 203
	private int isSkullon = 2;
}
