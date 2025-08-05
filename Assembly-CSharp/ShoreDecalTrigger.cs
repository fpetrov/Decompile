using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x02000176 RID: 374
[ExecuteInEditMode]
public class ShoreDecalTrigger : MonoBehaviour
{
	// Token: 0x06000F49 RID: 3913 RVA: 0x0003E0A4 File Offset: 0x0003C2A4
	private void Update()
	{
		if (this.waterSurface != null)
		{
			this.searchParameters.startPositionWS = this.searchResult.candidateLocationWS;
			this.searchParameters.targetPositionWS = base.gameObject.transform.position;
			this.searchParameters.error = 0.01f;
			this.searchParameters.maxIterations = 8;
			this.searchParameters.includeDeformation = true;
			this.searchParameters.excludeSimulation = true;
			if (this.waterSurface.ProjectPointOnWaterSurface(this.searchParameters, out this.searchResult) && this.searchResult.projectedPositionWS.y > base.transform.position.y && !this.decalIsInstantiated)
			{
				this.InstantiateDecal();
			}
		}
	}

	// Token: 0x06000F4A RID: 3914 RVA: 0x0003E175 File Offset: 0x0003C375
	public void setIsInstantiated(bool b)
	{
		this.decalIsInstantiated = b;
	}

	// Token: 0x06000F4B RID: 3915 RVA: 0x0003E180 File Offset: 0x0003C380
	private void InstantiateDecal()
	{
		this.setIsInstantiated(true);
		if (this.decalGameObject != null)
		{
			this.decalGameObject.transform.parent = base.transform;
			this.decalGameObject.transform.localPosition = Vector3.zero;
			this.decalGameObject.transform.localEulerAngles = new Vector3(90f, 0f, -90f);
			this.decalGameObject.SetActive(true);
		}
	}

	// Token: 0x040008BE RID: 2238
	public WaterSurface waterSurface;

	// Token: 0x040008BF RID: 2239
	public GameObject decalGameObject;

	// Token: 0x040008C0 RID: 2240
	private bool decalIsInstantiated;

	// Token: 0x040008C1 RID: 2241
	private WaterSearchParameters searchParameters;

	// Token: 0x040008C2 RID: 2242
	private WaterSearchResult searchResult;
}
