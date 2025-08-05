using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x02000173 RID: 371
[ExecuteInEditMode]
public class ProjectCaustics : MonoBehaviour
{
	// Token: 0x06000F3F RID: 3903 RVA: 0x0003DE74 File Offset: 0x0003C074
	private void Update()
	{
		if (this.waterSurface.GetCausticsBuffer(out this.regionSize) != null && this.decal.GetTexture("_Texture2D") == null)
		{
			this.decal.SetTexture("_Texture2D", this.waterSurface.GetCausticsBuffer(out this.regionSize));
		}
	}

	// Token: 0x040008B1 RID: 2225
	public Material decal;

	// Token: 0x040008B2 RID: 2226
	public WaterSurface waterSurface;

	// Token: 0x040008B3 RID: 2227
	public float regionSize = 20f;
}
