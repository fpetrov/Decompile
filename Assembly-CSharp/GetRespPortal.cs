using System;
using UnityEngine;

// Token: 0x020000A1 RID: 161
public class GetRespPortal : MonoBehaviour
{
	// Token: 0x06000693 RID: 1683 RVA: 0x0001948B File Offset: 0x0001768B
	public void triggerPortal()
	{
		this.portalasource.PlayOneShot(this.portalclips[Random.Range(0, this.portalclips.Length)]);
		this.shwcashole.resizeWormhole();
	}

	// Token: 0x0400033D RID: 829
	public RespawnWormhole shwcashole;

	// Token: 0x0400033E RID: 830
	public AudioClip[] portalclips;

	// Token: 0x0400033F RID: 831
	public AudioSource portalasource;
}
