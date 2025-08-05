using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x0200008E RID: 142
public class FreezeHitController : MonoBehaviour
{
	// Token: 0x060005D2 RID: 1490 RVA: 0x00016A2C File Offset: 0x00014C2C
	private void Start()
	{
		this.foge = base.transform.GetComponent<LocalVolumetricFog>();
		base.StartCoroutine(this.ExplosionRoutine());
	}

	// Token: 0x060005D3 RID: 1491 RVA: 0x00016A4C File Offset: 0x00014C4C
	private IEnumerator ExplosionRoutine()
	{
		this.foge.parameters.size = this.startSize;
		while (this.timer <= this.lerptime1)
		{
			this.timer += Time.deltaTime;
			this.foge.parameters.size = Vector3.Lerp(this.startSize, this.EndSize, this.timer / this.lerptime1);
			this.hdld.intensity = Mathf.Lerp(0f, 30000f, this.timer / this.lerptime1);
			yield return null;
		}
		this.timer = 0f;
		while (this.timer <= this.lerptime2)
		{
			this.timer += Time.deltaTime;
			this.foge.parameters.size = Vector3.Lerp(this.EndSize, this.FinalSize, this.timer / this.lerptime2);
			this.hdld.intensity = Mathf.Lerp(30000f, 0f, this.timer / this.lerptime2);
			this.foge.parameters.meanFreePath = Mathf.Lerp(this.highFogVal, 100f, this.timer / this.lerptime2);
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x040002E3 RID: 739
	private LocalVolumetricFog foge;

	// Token: 0x040002E4 RID: 740
	private float timer;

	// Token: 0x040002E5 RID: 741
	public HDAdditionalLightData hdld;

	// Token: 0x040002E6 RID: 742
	public float lerptime1 = 1f;

	// Token: 0x040002E7 RID: 743
	public float lerptime2 = 20f;

	// Token: 0x040002E8 RID: 744
	private Vector3 startSize = new Vector3(0f, 0f, 0f);

	// Token: 0x040002E9 RID: 745
	private Vector3 EndSize = new Vector3(10f, 10f, 10f);

	// Token: 0x040002EA RID: 746
	private Vector3 FinalSize = new Vector3(20f, 20f, 20f);

	// Token: 0x040002EB RID: 747
	public float highFogVal = 25f;
}
