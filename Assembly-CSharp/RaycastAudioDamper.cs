using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// Token: 0x02000153 RID: 339
public class RaycastAudioDamper : MonoBehaviour
{
	// Token: 0x06000EAC RID: 3756 RVA: 0x0003B6A3 File Offset: 0x000398A3
	private void Start()
	{
		base.StartCoroutine(this.CaveRoutine());
	}

	// Token: 0x06000EAD RID: 3757 RVA: 0x0003B6B4 File Offset: 0x000398B4
	public void GetClosestPlayerName(Text micText)
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, 0.2f, this.Player);
		float num = 100000f;
		int num2 = -1;
		for (int i = 0; i < array.Length; i++)
		{
			GetPlayerGameobject getPlayerGameobject;
			if (array[i].TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject) && Vector3.Distance(array[i].transform.position, base.transform.position) < num)
			{
				num = Vector3.Distance(array[i].transform.position, base.transform.position);
				num2 = i;
			}
		}
		if (num2 != -1)
		{
			micText.text = array[num2].GetComponent<GetPlayerGameobject>().player.GetComponent<PlayerMovement>().playername;
		}
		if (micText.text == "sampletext")
		{
			base.StartCoroutine(this.tryagain(micText));
		}
	}

	// Token: 0x06000EAE RID: 3758 RVA: 0x0003B785 File Offset: 0x00039985
	private IEnumerator tryagain(Text micText)
	{
		yield return new WaitForSeconds(2f);
		Collider[] array = Physics.OverlapSphere(base.transform.position, 0.2f, this.Player);
		float num = 100000f;
		int num2 = -1;
		for (int i = 0; i < array.Length; i++)
		{
			GetPlayerGameobject getPlayerGameobject;
			if (array[i].TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject) && Vector3.Distance(array[i].transform.position, base.transform.position) < num)
			{
				num = Vector3.Distance(array[i].transform.position, base.transform.position);
				num2 = i;
			}
		}
		if (num2 != -1)
		{
			micText.text = array[num2].GetComponent<GetPlayerGameobject>().player.GetComponent<PlayerMovement>().playername;
		}
		yield break;
	}

	// Token: 0x06000EAF RID: 3759 RVA: 0x0003B79B File Offset: 0x0003999B
	public void ToggleGlobalVoice()
	{
		this.globalvoicetimer += 60f;
		base.StartCoroutine(this.GlobalVoice());
	}

	// Token: 0x06000EB0 RID: 3760 RVA: 0x0003B7BC File Offset: 0x000399BC
	private IEnumerator GlobalVoice()
	{
		base.GetComponent<AudioSource>().maxDistance = 5000f;
		while (this.globalvoicetimer > 0f)
		{
			yield return null;
			this.globalvoicetimer -= Time.deltaTime;
		}
		base.GetComponent<AudioSource>().maxDistance = 150f;
		yield break;
	}

	// Token: 0x06000EB1 RID: 3761 RVA: 0x0003B7CB File Offset: 0x000399CB
	public void ToggleHighpitch()
	{
		this.shrunktimer += 41f;
		base.StartCoroutine(this.highpitch());
	}

	// Token: 0x06000EB2 RID: 3762 RVA: 0x0003B7EC File Offset: 0x000399EC
	private IEnumerator highpitch()
	{
		while (this.shrunktimer > 0f)
		{
			yield return null;
			this.shrunktimer -= Time.deltaTime;
		}
		yield break;
	}

	// Token: 0x06000EB3 RID: 3763 RVA: 0x0003B7FB File Offset: 0x000399FB
	private IEnumerator CaveRoutine()
	{
		while (base.isActiveAndEnabled)
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, 1f, this.Cave);
			if (array.Length == 0)
			{
				this.whichreverbzone = 0;
			}
			else
			{
				foreach (Collider collider in array)
				{
					if (collider.CompareTag("cave"))
					{
						this.whichreverbzone = 1;
					}
					else if (collider.CompareTag("castle"))
					{
						this.whichreverbzone = 2;
					}
					else if (collider.CompareTag("forest"))
					{
						this.whichreverbzone = 3;
					}
					else
					{
						this.whichreverbzone = 0;
					}
				}
			}
			yield return new WaitForSeconds(0.125f);
		}
		yield break;
	}

	// Token: 0x06000EB4 RID: 3764 RVA: 0x0003B80C File Offset: 0x00039A0C
	private void Update()
	{
		if (this.shrunktimer > 0f)
		{
			this.ps.outputAudioMixerGroup = this.highpitched;
			return;
		}
		if (this.globalvoicetimer > 0f)
		{
			this.ps.outputAudioMixerGroup = this.globalloud;
			return;
		}
		Vector3 vector = new Vector3(base.transform.position.x, base.transform.position.y + 1f, base.transform.position.z);
		Vector3 normalized = (Camera.main.transform.position - vector).normalized;
		float num = Vector3.Distance(Camera.main.transform.position, vector);
		Debug.DrawRay(vector, normalized * num, Color.cyan, 1f);
		RaycastHit raycastHit;
		if (!Physics.Raycast(vector, normalized, out raycastHit, num, this.GroundLayer))
		{
			this.triggerlowpass = false;
		}
		else
		{
			this.triggerlowpass = true;
		}
		if (this.triggerlowpass)
		{
			if (this.whichreverbzone == 0)
			{
				this.ps.outputAudioMixerGroup = this.plainsreverb1;
				return;
			}
			if (this.whichreverbzone == 1)
			{
				this.ps.outputAudioMixerGroup = this.cavereverb1;
				return;
			}
			if (this.whichreverbzone == 2)
			{
				this.ps.outputAudioMixerGroup = this.castlereverb1;
				return;
			}
			if (this.whichreverbzone == 3)
			{
				this.ps.outputAudioMixerGroup = this.forestreverb1;
				return;
			}
		}
		else
		{
			if (this.whichreverbzone == 0)
			{
				this.ps.outputAudioMixerGroup = this.plainsreverb;
				return;
			}
			if (this.whichreverbzone == 1)
			{
				this.ps.outputAudioMixerGroup = this.cavereverb;
				return;
			}
			if (this.whichreverbzone == 2)
			{
				this.ps.outputAudioMixerGroup = this.castlereverb;
				return;
			}
			if (this.whichreverbzone == 3)
			{
				this.ps.outputAudioMixerGroup = this.forestreverb;
			}
		}
	}

	// Token: 0x04000800 RID: 2048
	public LayerMask GroundLayer;

	// Token: 0x04000801 RID: 2049
	public LayerMask Cave;

	// Token: 0x04000802 RID: 2050
	public LayerMask Player;

	// Token: 0x04000803 RID: 2051
	public AudioSource ps;

	// Token: 0x04000804 RID: 2052
	public AudioMixerGroup plainsreverb;

	// Token: 0x04000805 RID: 2053
	public AudioMixerGroup cavereverb;

	// Token: 0x04000806 RID: 2054
	public AudioMixerGroup castlereverb;

	// Token: 0x04000807 RID: 2055
	public AudioMixerGroup forestreverb;

	// Token: 0x04000808 RID: 2056
	public AudioMixerGroup plainsreverb1;

	// Token: 0x04000809 RID: 2057
	public AudioMixerGroup cavereverb1;

	// Token: 0x0400080A RID: 2058
	public AudioMixerGroup castlereverb1;

	// Token: 0x0400080B RID: 2059
	public AudioMixerGroup forestreverb1;

	// Token: 0x0400080C RID: 2060
	public AudioMixerGroup highpitched;

	// Token: 0x0400080D RID: 2061
	public AudioMixerGroup globalloud;

	// Token: 0x0400080E RID: 2062
	private int whichreverbzone;

	// Token: 0x0400080F RID: 2063
	private bool triggerlowpass;

	// Token: 0x04000810 RID: 2064
	public float shrunktimer;

	// Token: 0x04000811 RID: 2065
	public float globalvoicetimer;
}
