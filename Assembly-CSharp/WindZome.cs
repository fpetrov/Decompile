using System;
using UnityEngine;

// Token: 0x02000217 RID: 535
public class WindZome : MonoBehaviour
{
	// Token: 0x0600156A RID: 5482 RVA: 0x0005A251 File Offset: 0x00058451
	private void Update()
	{
		this.ApplySettings();
	}

	// Token: 0x0600156B RID: 5483 RVA: 0x0005A25C File Offset: 0x0005845C
	private void ApplySettings()
	{
		if (this.windZone == null)
		{
			this.windZone = base.gameObject.GetComponent<WindZone>();
		}
		if (this.windZone != null)
		{
			Shader.SetGlobalVector("_WINDZONE_Direction", this.windZone.transform.forward);
			Shader.SetGlobalFloat("_WINDZONE_Main", this.windZone.windMain);
			Shader.SetGlobalFloat("_WINDZONE_Pulse_Frequency", this.windZone.windPulseFrequency);
			Shader.SetGlobalFloat("_WINDZONE_Pulse_Magnitude", this.windZone.windPulseMagnitude);
			Shader.SetGlobalFloat("_WINDZONE_Turbulence", this.windZone.windTurbulence);
		}
	}

	// Token: 0x04000C9A RID: 3226
	private WindZone windZone;
}
