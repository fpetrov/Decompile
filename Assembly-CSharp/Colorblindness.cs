using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x02000164 RID: 356
[VolumeComponentMenu("Post-processing/Custom/Colorblindness")]
[Serializable]
public sealed class Colorblindness : CustomPostProcessVolumeComponent, IPostProcessComponent
{
	// Token: 0x06000F0D RID: 3853 RVA: 0x0003CBC4 File Offset: 0x0003ADC4
	public bool IsActive()
	{
		return this.m_Material != null && this.intensity.value > 0f;
	}

	// Token: 0x17000183 RID: 387
	// (get) Token: 0x06000F0E RID: 3854 RVA: 0x0000CA50 File Offset: 0x0000AC50
	public override CustomPostProcessInjectionPoint injectionPoint
	{
		get
		{
			return CustomPostProcessInjectionPoint.AfterPostProcess;
		}
	}

	// Token: 0x06000F0F RID: 3855 RVA: 0x0003CBE8 File Offset: 0x0003ADE8
	public override void Setup()
	{
		if (Shader.Find("HDRPSamples/ColorblindFilter") != null)
		{
			this.m_Material = new Material(Shader.Find("HDRPSamples/ColorblindFilter"));
			return;
		}
		Debug.LogError("Unable to find shader 'HDRPSamples/ColorblindFilter'. Post Process ColorBlindness is unable to load.");
	}

	// Token: 0x06000F10 RID: 3856 RVA: 0x0003CC1C File Offset: 0x0003AE1C
	public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
	{
		if (this.m_Material == null)
		{
			return;
		}
		this.m_Material.SetFloat("_Intensity", this.intensity.value);
		if (this.Type == Colorblindness.colorblindness_Types.Protanopia)
		{
			this.m_Material.EnableKeyword("_TYPE_PROTANOPIA");
			this.m_Material.DisableKeyword("_TYPE_DEUTERANOPIA");
			this.m_Material.DisableKeyword("_TYPE_TRITANOPIA");
			this.m_Material.DisableKeyword("_TYPE_CONE_MONOCHROMATISM");
			this.m_Material.DisableKeyword("_TYPE_ACHROMATOPSIA");
		}
		if (this.Type == Colorblindness.colorblindness_Types.Deuteranopia)
		{
			this.m_Material.DisableKeyword("_TYPE_PROTANOPIA");
			this.m_Material.EnableKeyword("_TYPE_DEUTERANOPIA");
			this.m_Material.DisableKeyword("_TYPE_TRITANOPIA");
			this.m_Material.DisableKeyword("_TYPE_CONE_MONOCHROMATISM");
			this.m_Material.DisableKeyword("_TYPE_ACHROMATOPSIA");
		}
		if (this.Type == Colorblindness.colorblindness_Types.Tritanopia)
		{
			this.m_Material.DisableKeyword("_TYPE_PROTANOPIA");
			this.m_Material.DisableKeyword("_TYPE_DEUTERANOPIA");
			this.m_Material.EnableKeyword("_TYPE_TRITANOPIA");
			this.m_Material.DisableKeyword("_TYPE_CONE_MONOCHROMATISM");
			this.m_Material.DisableKeyword("_TYPE_ACHROMATOPSIA");
		}
		if (this.Type == Colorblindness.colorblindness_Types.MonoChromatism)
		{
			this.m_Material.DisableKeyword("_TYPE_PROTANOPIA");
			this.m_Material.DisableKeyword("_TYPE_DEUTERANOPIA");
			this.m_Material.DisableKeyword("_TYPE_TRITANOPIA");
			this.m_Material.EnableKeyword("_TYPE_CONE_MONOCHROMATISM");
			this.m_Material.DisableKeyword("_TYPE_ACHROMATOPSIA");
		}
		if (this.Type == Colorblindness.colorblindness_Types.Achromatopsia)
		{
			this.m_Material.DisableKeyword("_TYPE_PROTANOPIA");
			this.m_Material.DisableKeyword("_TYPE_DEUTERANOPIA");
			this.m_Material.DisableKeyword("_TYPE_TRITANOPIA");
			this.m_Material.DisableKeyword("_TYPE_CONE_MONOCHROMATISM");
			this.m_Material.EnableKeyword("_TYPE_ACHROMATOPSIA");
		}
		this.m_Material.SetTexture("_MainTex", source);
		HDUtils.DrawFullScreen(cmd, this.m_Material, destination, null, 0);
	}

	// Token: 0x06000F11 RID: 3857 RVA: 0x0003CE4F File Offset: 0x0003B04F
	public override void Cleanup()
	{
		CoreUtils.Destroy(this.m_Material);
	}

	// Token: 0x04000854 RID: 2132
	[Tooltip("Controls the intensity of the effect.")]
	public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f, false);

	// Token: 0x04000855 RID: 2133
	public Colorblindness.colorblindness_Types_Parameter Type = new Colorblindness.colorblindness_Types_Parameter(Colorblindness.colorblindness_Types.Protanopia, false);

	// Token: 0x04000856 RID: 2134
	private Material m_Material;

	// Token: 0x04000857 RID: 2135
	private const string kShaderName = "HDRPSamples/ColorblindFilter";

	// Token: 0x02000165 RID: 357
	public enum colorblindness_Types
	{
		// Token: 0x04000859 RID: 2137
		Protanopia,
		// Token: 0x0400085A RID: 2138
		Deuteranopia,
		// Token: 0x0400085B RID: 2139
		Tritanopia,
		// Token: 0x0400085C RID: 2140
		MonoChromatism,
		// Token: 0x0400085D RID: 2141
		Achromatopsia
	}

	// Token: 0x02000166 RID: 358
	[Serializable]
	public sealed class colorblindness_Types_Parameter : VolumeParameter<Colorblindness.colorblindness_Types>
	{
		// Token: 0x06000F13 RID: 3859 RVA: 0x0003CE8C File Offset: 0x0003B08C
		public colorblindness_Types_Parameter(Colorblindness.colorblindness_Types value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
