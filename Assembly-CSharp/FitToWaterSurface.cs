using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x0200016E RID: 366
[ExecuteInEditMode]
public class FitToWaterSurface : MonoBehaviour
{
	// Token: 0x06000F2A RID: 3882 RVA: 0x0003D50C File Offset: 0x0003B70C
	private void Update()
	{
		if (this.targetSurface != null)
		{
			this.searchParameters.startPositionWS = this.searchResult.candidateLocationWS;
			this.searchParameters.targetPositionWS = base.gameObject.transform.position;
			this.searchParameters.error = 0.01f;
			this.searchParameters.maxIterations = 8;
			if (this.targetSurface.ProjectPointOnWaterSurface(this.searchParameters, out this.searchResult))
			{
				base.gameObject.transform.position = this.searchResult.projectedPositionWS;
			}
		}
	}

	// Token: 0x0400088F RID: 2191
	public WaterSurface targetSurface;

	// Token: 0x04000890 RID: 2192
	private WaterSearchParameters searchParameters;

	// Token: 0x04000891 RID: 2193
	private WaterSearchResult searchResult;
}
