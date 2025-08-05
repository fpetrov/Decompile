using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000168 RID: 360
[ExecuteInEditMode]
public class FullscreenSamplesEffectSelection : MonoBehaviour
{
	// Token: 0x06000F18 RID: 3864 RVA: 0x0003CEEC File Offset: 0x0003B0EC
	private void SwitchEffect()
	{
		switch (this.fullscreenEffect)
		{
		case FullscreenSamplesEffectSelection.FullscreenEffectsEnum.None:
			this.infos = "No Effect selected with this script";
			return;
		case FullscreenSamplesEffectSelection.FullscreenEffectsEnum.EdgeDetection:
			this.infos = "Fullscreen Custom Pass using a Shadergraph. \nThe material performs a Robert Cross Edge Detection on the Scene Depth and Normal Buffer. \nThe normal and depth buffer happens before Transparency in the rendering pipeline. \nIt means that transparent objects won't be seen by this effect.\n\nCodeless.";
			return;
		case FullscreenSamplesEffectSelection.FullscreenEffectsEnum.Highlight:
			this.infos = "Here objects are highlighted thanks to two passes. \n\nFirst objects inside the UI Layer are rendered with a single color onto the Custom Color Buffer. \nThis color is changed per object through a C# script that edits Material Property Block. See CustomizeHighlightColor.cs. \n\nThen in a second Pass, a fullscreen shader uses the Custom Color Buffer to creates the visual highlight seen on screen.\n\nCodeless.";
			return;
		case FullscreenSamplesEffectSelection.FullscreenEffectsEnum.Sobel:
			this.infos = "Fullscreen Custom Pass that uses a Shadergraph that performs a Sobel Filter on the Scene Color.\n\n Codeless.";
			return;
		case FullscreenSamplesEffectSelection.FullscreenEffectsEnum.SpeedLines:
			this.infos = "Fullscreen Custom Pass making animated speed lines over the screen with Shadergraph.\n\nCodeless.";
			return;
		case FullscreenSamplesEffectSelection.FullscreenEffectsEnum.NightVision:
			this.infos = "Fullscreen Custom Pass to create a Night Vision filter. \n\nPlease try this effect with Night lighting.\nCodeless.";
			return;
		case FullscreenSamplesEffectSelection.FullscreenEffectsEnum.CustomNightSky:
			this.infos = "A shadergraph is used to render the artistic look of a night sky on a cubemap.\nThe cubemap is then used by the HDRi Sky Override on a Volume Profile.\nA C# script links the directional light to the Moon position.";
			return;
		case FullscreenSamplesEffectSelection.FullscreenEffectsEnum.RainOnCamera:
			this.infos = "The rain animation is created through a shadergraph rendering on a Double Buffered Custom Render Target.\n This creates an animated texture of water droplets.\nThe animated texture is then used in another Shadergraph on a Fullscreen Custom Pass.\nCodeless.\n\n To note : while in editor and outside of runtime, the double buffered Render Target update timing is not consistant.";
			return;
		case FullscreenSamplesEffectSelection.FullscreenEffectsEnum.ColorblindnessFilter:
			this.infos = "Filter that simulates types of Colorblindness.\nThis filter needs to be applied to the final color of the render, after Tonemapping or any other color grading.\nThis is done in Shadergraph by using PostProcessInput of the HDSampleBuffer node, which is only available after Post Process. \nIt means a new Post Process for the Volume Profile needs to be created.\nCustom Post Process are created through C# script, see Colorblindness.cs \n\nThis custom post process needs to be added to the HDRP Global settings \n(Custom Post Process Orders > After Post Process)\n otherwise HDRP won't recognize it.\nThen, Colorblindness will be available as a new Override for Volume Profile under Post-Processing>Custom.";
			return;
		default:
			return;
		}
	}

	// Token: 0x06000F19 RID: 3865 RVA: 0x0003CF96 File Offset: 0x0003B196
	private void OnValidate()
	{
		this.SwitchEffect();
		this.needUpdate = true;
	}

	// Token: 0x06000F1A RID: 3866 RVA: 0x0003CFA8 File Offset: 0x0003B1A8
	private void Update()
	{
		if (this.needUpdate)
		{
			this.RemoveEffect();
			if (this.fullscreenEffect != FullscreenSamplesEffectSelection.FullscreenEffectsEnum.None)
			{
				this.InstantiateNewEffect(this.fullscreenEffect - FullscreenSamplesEffectSelection.FullscreenEffectsEnum.EdgeDetection);
				if (Application.isPlaying)
				{
					this.infoText.text = this.infos;
				}
			}
			if (this.useAttachedDayNightProfile)
			{
				FullscreenSamplesEffectSelection.TimeOfDayEnum timeOfDayEnum = this.timeOfDay;
				if (timeOfDayEnum != FullscreenSamplesEffectSelection.TimeOfDayEnum.Day)
				{
					if (timeOfDayEnum == FullscreenSamplesEffectSelection.TimeOfDayEnum.Night)
					{
						this.sceneVolume.sharedProfile = this.nightVolumeProfile;
						if (this.directionalLight != null)
						{
							this.directionalLight.intensity = 3f;
						}
					}
				}
				else
				{
					this.sceneVolume.sharedProfile = this.dayVolumeProfile;
					if (this.directionalLight != null)
					{
						this.directionalLight.intensity = 10000f;
					}
				}
			}
			this.needUpdate = false;
		}
		if (Application.isFocused)
		{
			this.horizontalAxis = Input.GetAxis("Horizontal");
			this.verticalAxis = Input.GetAxis("Vertical");
			this.spaceToggle = Input.GetKeyDown(KeyCode.Space);
			if (Mathf.Abs(this.horizontalAxis) > 0.25f)
			{
				if (!this.horizontalAxisInUse)
				{
					if (this.horizontalAxis > 0f)
					{
						this.fullscreenEffect++;
						if (this.fullscreenEffect == (FullscreenSamplesEffectSelection.FullscreenEffectsEnum)this.numberOfEffects)
						{
							this.fullscreenEffect = FullscreenSamplesEffectSelection.FullscreenEffectsEnum.EdgeDetection;
						}
					}
					else
					{
						this.fullscreenEffect--;
						if (this.fullscreenEffect == FullscreenSamplesEffectSelection.FullscreenEffectsEnum.None)
						{
							this.fullscreenEffect = (FullscreenSamplesEffectSelection.FullscreenEffectsEnum)(this.numberOfEffects - 1);
						}
					}
					this.SwitchEffect();
					this.needUpdate = true;
					this.horizontalAxisInUse = true;
				}
			}
			else
			{
				this.horizontalAxisInUse = false;
			}
			if (Mathf.Abs(this.verticalAxis) > 0.25f)
			{
				if (!this.verticalAxisInUse)
				{
					this.timeOfDay++;
					if (this.timeOfDay == (FullscreenSamplesEffectSelection.TimeOfDayEnum)2)
					{
						this.timeOfDay = FullscreenSamplesEffectSelection.TimeOfDayEnum.Day;
					}
					this.needUpdate = true;
					this.verticalAxisInUse = true;
				}
			}
			else
			{
				this.verticalAxisInUse = false;
			}
			if (this.spaceToggle)
			{
				this.toggleEffect = !this.toggleEffect;
				this.prefab.SetActive(this.toggleEffect);
			}
		}
	}

	// Token: 0x06000F1B RID: 3867 RVA: 0x0003D1B0 File Offset: 0x0003B3B0
	private void InstantiateNewEffect(int index)
	{
		this.RemoveEffect();
		if (index <= this.effectPrefabs.Length && this.effectPrefabs.Length != 0)
		{
			this.prefabToInstantiate = this.effectPrefabs[index];
		}
		if (this.prefabToInstantiate != null)
		{
			this.prefab = Object.Instantiate(this.prefabToInstantiate, base.transform.position, Quaternion.identity) as GameObject;
			this.prefab.transform.parent = base.gameObject.transform;
		}
	}

	// Token: 0x06000F1C RID: 3868 RVA: 0x0003D234 File Offset: 0x0003B434
	private void RemoveEffect()
	{
		if (base.transform.childCount > 0)
		{
			foreach (object obj in base.transform)
			{
				Object.DestroyImmediate(((Transform)obj).gameObject);
			}
		}
	}

	// Token: 0x0400085F RID: 2143
	public FullscreenSamplesEffectSelection.FullscreenEffectsEnum fullscreenEffect;

	// Token: 0x04000860 RID: 2144
	private int numberOfEffects = Enum.GetValues(typeof(FullscreenSamplesEffectSelection.FullscreenEffectsEnum)).Length;

	// Token: 0x04000861 RID: 2145
	private string prefabPath;

	// Token: 0x04000862 RID: 2146
	public GameObject[] effectPrefabs;

	// Token: 0x04000863 RID: 2147
	private GameObject prefab;

	// Token: 0x04000864 RID: 2148
	private Object prefabToInstantiate;

	// Token: 0x04000865 RID: 2149
	public string infos;

	// Token: 0x04000866 RID: 2150
	private bool needUpdate;

	// Token: 0x04000867 RID: 2151
	public TextMesh infoText;

	// Token: 0x04000868 RID: 2152
	private bool toggleEffect = true;

	// Token: 0x04000869 RID: 2153
	private bool horizontalAxisInUse;

	// Token: 0x0400086A RID: 2154
	private bool verticalAxisInUse;

	// Token: 0x0400086B RID: 2155
	private float horizontalAxis;

	// Token: 0x0400086C RID: 2156
	private float verticalAxis;

	// Token: 0x0400086D RID: 2157
	private bool spaceToggle;

	// Token: 0x0400086E RID: 2158
	public bool useAttachedDayNightProfile;

	// Token: 0x0400086F RID: 2159
	public FullscreenSamplesEffectSelection.TimeOfDayEnum timeOfDay;

	// Token: 0x04000870 RID: 2160
	public Volume sceneVolume;

	// Token: 0x04000871 RID: 2161
	public VolumeProfile dayVolumeProfile;

	// Token: 0x04000872 RID: 2162
	public VolumeProfile nightVolumeProfile;

	// Token: 0x04000873 RID: 2163
	public Light directionalLight;

	// Token: 0x02000169 RID: 361
	public enum FullscreenEffectsEnum
	{
		// Token: 0x04000875 RID: 2165
		None,
		// Token: 0x04000876 RID: 2166
		EdgeDetection,
		// Token: 0x04000877 RID: 2167
		Highlight,
		// Token: 0x04000878 RID: 2168
		Sobel,
		// Token: 0x04000879 RID: 2169
		SpeedLines,
		// Token: 0x0400087A RID: 2170
		NightVision,
		// Token: 0x0400087B RID: 2171
		CustomNightSky,
		// Token: 0x0400087C RID: 2172
		RainOnCamera,
		// Token: 0x0400087D RID: 2173
		ColorblindnessFilter
	}

	// Token: 0x0200016A RID: 362
	public enum TimeOfDayEnum
	{
		// Token: 0x0400087F RID: 2175
		Day,
		// Token: 0x04000880 RID: 2176
		Night
	}
}
