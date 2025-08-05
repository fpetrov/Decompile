using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x020001E7 RID: 487
public class BrazzerInteract : MonoBehaviour
{
	// Token: 0x060013B2 RID: 5042 RVA: 0x00052A55 File Offset: 0x00050C55
	private void Start()
	{
		this.hdLightData = this.BrazzerLight.GetComponent<HDAdditionalLightData>();
	}

	// Token: 0x060013B3 RID: 5043 RVA: 0x00052A68 File Offset: 0x00050C68
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			this.playersInBounds++;
			this.UpdateShadowCasting();
		}
	}

	// Token: 0x060013B4 RID: 5044 RVA: 0x00052A8B File Offset: 0x00050C8B
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			this.playersInBounds--;
			this.UpdateShadowCasting();
		}
	}

	// Token: 0x060013B5 RID: 5045 RVA: 0x00052AAE File Offset: 0x00050CAE
	private void UpdateShadowCasting()
	{
		if (this.playersInBounds < 1)
		{
			this.hdLightData.EnableShadows(false);
			return;
		}
		this.hdLightData.EnableShadows(true);
		if (!this.isFlickering)
		{
			base.StartCoroutine(this.FlickerLight());
		}
	}

	// Token: 0x060013B6 RID: 5046 RVA: 0x00052AE7 File Offset: 0x00050CE7
	private IEnumerator FlickerLight()
	{
		this.isFlickering = true;
		while (this.playersInBounds > 0)
		{
			this.hdLightData.intensity = 500000f + Mathf.Sin(Random.Range(0.1f, 1f) * Time.time) * Random.Range(10000f, 25000f);
			yield return new WaitForSeconds(0.1f);
		}
		this.isFlickering = false;
		yield break;
	}

	// Token: 0x04000B8E RID: 2958
	private int playersInBounds;

	// Token: 0x04000B8F RID: 2959
	public Light BrazzerLight;

	// Token: 0x04000B90 RID: 2960
	private HDAdditionalLightData hdLightData;

	// Token: 0x04000B91 RID: 2961
	private bool isFlickering;
}
