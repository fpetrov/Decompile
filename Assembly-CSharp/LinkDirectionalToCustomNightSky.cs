using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x0200016B RID: 363
[ExecuteAlways]
public class LinkDirectionalToCustomNightSky : MonoBehaviour
{
	// Token: 0x06000F1E RID: 3870 RVA: 0x0003D2CC File Offset: 0x0003B4CC
	private void Update()
	{
		if (this.update)
		{
			HDRenderPipeline hdrenderPipeline = RenderPipelineManager.currentPipeline as HDRenderPipeline;
			if (hdrenderPipeline != null && hdrenderPipeline.GetMainLight() != null)
			{
				this.Dir = hdrenderPipeline.GetMainLight().gameObject.transform.forward;
				this.SkyMat.SetVector("_Moonlight_Forward_Direction", this.Dir);
			}
		}
	}

	// Token: 0x04000881 RID: 2177
	public Material SkyMat;

	// Token: 0x04000882 RID: 2178
	private Vector3 Dir;

	// Token: 0x04000883 RID: 2179
	public bool update = true;
}
