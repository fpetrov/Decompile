using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x02000141 RID: 321
public class PoofController : MonoBehaviour
{
	// Token: 0x06000DFF RID: 3583 RVA: 0x00039636 File Offset: 0x00037836
	private void Start()
	{
		base.StartCoroutine(this.Shoott());
		base.StartCoroutine(this.Light());
		base.StartCoroutine(this.ExplosionFogSizeRoutine());
	}

	// Token: 0x06000E00 RID: 3584 RVA: 0x0003965F File Offset: 0x0003785F
	private IEnumerator ExplosionFogSizeRoutine()
	{
		float timer3 = 0f;
		while (timer3 <= this.sizetimer)
		{
			timer3 += Time.deltaTime;
			this.foge.parameters.size = Vector3.Lerp(new Vector3(2f, 1f, 2f), this.targetSize1, timer3 / this.sizetimer);
			yield return null;
		}
		timer3 = 0f;
		while (timer3 < this.sizetimer2)
		{
			timer3 += Time.deltaTime;
			this.foge.parameters.size = Vector3.Lerp(this.targetSize1, this.targetSize2, timer3 / this.sizetimer2);
			yield return null;
		}
		timer3 = 0f;
		while (timer3 < 10f)
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + Time.deltaTime * timer3, base.transform.position.z);
			timer3 += Time.deltaTime;
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x06000E01 RID: 3585 RVA: 0x0003966E File Offset: 0x0003786E
	private IEnumerator Shoott()
	{
		float timer = 0f;
		while (timer < this.thicknesstimer)
		{
			timer += Time.deltaTime;
			this.foge.parameters.meanFreePath = Mathf.Lerp(25f, 0.05f, timer / this.thicknesstimer);
			yield return null;
		}
		timer = 0f;
		yield return new WaitForSeconds(this.thicknesswaittimer);
		while (timer < this.thicknesstimer2)
		{
			timer += Time.deltaTime;
			this.foge.parameters.meanFreePath = Mathf.Lerp(0.05f, this.intitialThicknessval, timer / this.thicknesstimer2);
			yield return null;
		}
		timer = 0f;
		while (timer < this.thicknesstimer2)
		{
			timer += Time.deltaTime;
			this.foge.parameters.meanFreePath = Mathf.Lerp(this.intitialThicknessval, 25f, timer / this.thicknesstimer2);
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000E02 RID: 3586 RVA: 0x0003967D File Offset: 0x0003787D
	private IEnumerator Light()
	{
		this.hdLightData.gameObject.SetActive(true);
		float timer = 0f;
		while (timer < this.lightimer)
		{
			timer += Time.deltaTime;
			this.hdLightData.range = Mathf.Lerp(0.01f, 10f, timer / this.lightimer);
			this.hdLightData.intensity = Mathf.Lerp(1000f, 40000f, timer / this.lightimer);
			yield return null;
		}
		timer = 0f;
		while (timer < this.lightimer2)
		{
			timer += Time.deltaTime;
			this.hdLightData.intensity = Mathf.Lerp(40000f, 1000f, timer / this.lightimer2);
			yield return null;
		}
		this.hdLightData.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x040007A3 RID: 1955
	public HDAdditionalLightData hdLightData;

	// Token: 0x040007A4 RID: 1956
	public LocalVolumetricFog foge;

	// Token: 0x040007A5 RID: 1957
	public float sizetimer = 0.2f;

	// Token: 0x040007A6 RID: 1958
	public float sizetimer2 = 0.2f;

	// Token: 0x040007A7 RID: 1959
	public float lightimer = 0.1f;

	// Token: 0x040007A8 RID: 1960
	public float lightimer2 = 5f;

	// Token: 0x040007A9 RID: 1961
	public float thicknesstimer = 0.4f;

	// Token: 0x040007AA RID: 1962
	public float thicknesstimer2 = 5f;

	// Token: 0x040007AB RID: 1963
	public float thicknesswaittimer = 2f;

	// Token: 0x040007AC RID: 1964
	public float intitialThicknessval;

	// Token: 0x040007AD RID: 1965
	public Vector3 targetSize1 = new Vector3(4f, 10f, 4f);

	// Token: 0x040007AE RID: 1966
	public Vector3 targetSize2 = new Vector3(25f, 50f, 25f);
}
