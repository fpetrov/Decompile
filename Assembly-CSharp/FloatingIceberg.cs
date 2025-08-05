using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x02000170 RID: 368
[ExecuteInEditMode]
public class FloatingIceberg : MonoBehaviour
{
	// Token: 0x06000F34 RID: 3892 RVA: 0x0003D9F9 File Offset: 0x0003BBF9
	private void Start()
	{
		this.Reset();
	}

	// Token: 0x06000F35 RID: 3893 RVA: 0x0003DA04 File Offset: 0x0003BC04
	private void Update()
	{
		float num = 50f;
		float num2 = 68f;
		float num3 = 1f - Mathf.Clamp01((Mathf.Abs(base.transform.position.x) - num) / (num2 - num));
		if (base.transform.position.x > 0f)
		{
			base.GetComponent<Renderer>().sharedMaterial.SetFloat("_Opacity", num3);
		}
		else
		{
			base.transform.localScale = Vector3.one * this.initialScale * num3;
		}
		if (base.transform.position.x < -num2 || this.clickToResetPosition)
		{
			this.Reset();
		}
		if (this.selfRotationSpeed > 0f)
		{
			Vector3 localEulerAngles = base.transform.localEulerAngles;
			localEulerAngles.y += this.selfRotationSpeed;
			base.transform.localEulerAngles = localEulerAngles;
		}
		if (this.targetSurface != null)
		{
			this.searchParameters.startPositionWS = this.searchResult.candidateLocationWS;
			this.searchParameters.targetPositionWS = base.transform.position;
			this.searchParameters.error = 0.01f;
			this.searchParameters.maxIterations = 8;
			this.searchParameters.includeDeformation = this.includeDeformers;
			this.searchParameters.excludeSimulation = false;
			if (this.targetSurface.ProjectPointOnWaterSurface(this.searchParameters, out this.searchResult))
			{
				base.transform.position = this.searchResult.projectedPositionWS + this.searchResult.currentDirectionWS * this.currentSpeedMultiplier;
			}
		}
	}

	// Token: 0x06000F36 RID: 3894 RVA: 0x0003DBB6 File Offset: 0x0003BDB6
	private void Reset()
	{
		base.transform.position = this.initialPosition;
		base.transform.localScale = Vector3.one * this.initialScale;
		this.clickToResetPosition = false;
	}

	// Token: 0x0400089F RID: 2207
	public WaterSurface targetSurface;

	// Token: 0x040008A0 RID: 2208
	public float currentSpeedMultiplier = 1f;

	// Token: 0x040008A1 RID: 2209
	public float selfRotationSpeed;

	// Token: 0x040008A2 RID: 2210
	public Vector3 initialPosition;

	// Token: 0x040008A3 RID: 2211
	public float initialScale = 0.25f;

	// Token: 0x040008A4 RID: 2212
	public bool includeDeformers = true;

	// Token: 0x040008A5 RID: 2213
	public bool clickToResetPosition;

	// Token: 0x040008A6 RID: 2214
	private WaterSearchParameters searchParameters;

	// Token: 0x040008A7 RID: 2215
	private WaterSearchResult searchResult;
}
