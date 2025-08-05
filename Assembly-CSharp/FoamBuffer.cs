using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x02000171 RID: 369
[ExecuteInEditMode]
public class FoamBuffer : MonoBehaviour
{
	// Token: 0x06000F38 RID: 3896 RVA: 0x0003DC10 File Offset: 0x0003BE10
	private void Update()
	{
		Vector2 vector;
		base.GetComponent<DecalProjector>().material.SetTexture("_Base_Color", this.waterSurface.GetFoamBuffer(out vector));
	}

	// Token: 0x040008A8 RID: 2216
	public WaterSurface waterSurface;

	// Token: 0x040008A9 RID: 2217
	public float time = 2.5f;
}
