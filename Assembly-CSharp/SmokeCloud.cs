using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x02000195 RID: 405
public class SmokeCloud : MonoBehaviour
{
	// Token: 0x060010FF RID: 4351 RVA: 0x00049956 File Offset: 0x00047B56
	private void Start()
	{
		this.foge = base.transform.GetComponent<LocalVolumetricFog>();
		base.StartCoroutine(this.FogRoutine());
	}

	// Token: 0x06001100 RID: 4352 RVA: 0x00049976 File Offset: 0x00047B76
	private IEnumerator FogRoutine()
	{
		while (this.timer < 15f)
		{
			this.timer += Time.deltaTime;
			this.foge.parameters.size = Vector3.Lerp(this.foge.parameters.size, this.targetSize, this.timer / 15f);
			yield return null;
		}
		this.timer = 0f;
		while (this.timer < 5f)
		{
			this.timer += Time.deltaTime;
			this.foge.parameters.size = Vector3.Lerp(this.foge.parameters.size, new Vector3(50f, 55f, 50f), this.timer / 15f);
			LocalVolumetricFog localVolumetricFog = this.foge;
			localVolumetricFog.parameters.meanFreePath = localVolumetricFog.parameters.meanFreePath + Time.deltaTime * 10f;
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x040009E2 RID: 2530
	private LocalVolumetricFog foge;

	// Token: 0x040009E3 RID: 2531
	private Vector3 targetSize = new Vector3(35f, 35f, 35f);

	// Token: 0x040009E4 RID: 2532
	private float timer;
}
