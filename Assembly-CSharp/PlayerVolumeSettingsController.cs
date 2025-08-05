using System;
using Dissonance;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000140 RID: 320
public class PlayerVolumeSettingsController : MonoBehaviour
{
	// Token: 0x06000DFB RID: 3579 RVA: 0x00039560 File Offset: 0x00037760
	public void SetPlayerVolumeNames()
	{
		Transform transform = Object.FindFirstObjectByType<DissonanceComms>().transform;
		for (int i = 0; i < 7; i++)
		{
			this.names[i].gameObject.SetActive(true);
		}
		for (int j = 0; j < transform.childCount; j++)
		{
			if (j < 7)
			{
				Transform child = transform.GetChild(j);
				this.PlayerSources[j] = child.GetComponent<AudioSource>();
				child.GetComponent<RaycastAudioDamper>().GetClosestPlayerName(this.names[j]);
			}
		}
		for (int k = 0; k < 7; k++)
		{
			if (k >= transform.childCount)
			{
				this.names[k].gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06000DFC RID: 3580 RVA: 0x00039603 File Offset: 0x00037803
	public void UpdateVolumeVal(float value)
	{
		if (this.PlayerSources[this.setterindex] != null)
		{
			this.PlayerSources[this.setterindex].volume = value;
		}
	}

	// Token: 0x06000DFD RID: 3581 RVA: 0x0003962D File Offset: 0x0003782D
	public void setterIndex(int index)
	{
		this.setterindex = index;
	}

	// Token: 0x040007A0 RID: 1952
	public Text[] names;

	// Token: 0x040007A1 RID: 1953
	public AudioSource[] PlayerSources;

	// Token: 0x040007A2 RID: 1954
	private int setterindex;
}
