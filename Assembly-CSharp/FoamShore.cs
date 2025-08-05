using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x02000172 RID: 370
[ExecuteInEditMode]
public class FoamShore : MonoBehaviour
{
	// Token: 0x06000F3A RID: 3898 RVA: 0x0003DC54 File Offset: 0x0003BE54
	public void OnEnable()
	{
		this.startTime = Time.realtimeSinceStartup;
		this.aliveTime = Random.Range(this.minAliveTime, this.maxAliveTime);
		this.m_DecalProjectorComponent = base.GetComponent<DecalProjector>();
		this.m_DecalProjectorComponent.material = new Material(this.m_DecalProjectorComponent.material);
		this.Reset();
	}

	// Token: 0x06000F3B RID: 3899 RVA: 0x0003DCB0 File Offset: 0x0003BEB0
	private void Start()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000F3C RID: 3900 RVA: 0x0003DCC0 File Offset: 0x0003BEC0
	private void Update()
	{
		float num = (Time.realtimeSinceStartup - this.startTime) % this.aliveTime / this.aliveTime;
		float num2 = 1f - Mathf.Abs(4f * Mathf.Pow(num - 0.5f, 2f));
		float num3 = (((double)num <= 0.5) ? num2 : 1f);
		float num4 = (((double)num <= 0.5) ? 0f : (1f - num2));
		Vector3 size = this.m_DecalProjectorComponent.size;
		size.y = Mathf.Lerp(0f, this.maxSize, num2);
		this.m_DecalProjectorComponent.size = size;
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x = Mathf.Lerp(0f, this.maxPosition, num2);
		base.transform.localPosition = localPosition;
		this.m_DecalProjectorComponent.material.SetFloat("_Opacity", num3);
		this.m_DecalProjectorComponent.material.SetFloat("_NormalizedAliveTime", num);
		this.m_DecalProjectorComponent.material.SetFloat("_ContrastNormalized", num4);
		if ((double)num > 0.99)
		{
			base.transform.parent.GetComponent<ShoreDecalTrigger>().setIsInstantiated(false);
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000F3D RID: 3901 RVA: 0x0003DE12 File Offset: 0x0003C012
	public void Reset()
	{
		Random.Range(0, 2);
		Random.Range(0, 2);
		this.m_DecalProjectorComponent.material.SetFloat("_Opacity", 0f);
	}

	// Token: 0x040008AA RID: 2218
	public float minAliveTime = 6f;

	// Token: 0x040008AB RID: 2219
	public float maxAliveTime = 8f;

	// Token: 0x040008AC RID: 2220
	public float maxPosition = 5f;

	// Token: 0x040008AD RID: 2221
	public float maxSize = 15f;

	// Token: 0x040008AE RID: 2222
	private float aliveTime;

	// Token: 0x040008AF RID: 2223
	private float startTime;

	// Token: 0x040008B0 RID: 2224
	private DecalProjector m_DecalProjectorComponent;
}
