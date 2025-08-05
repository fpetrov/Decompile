using System;
using UnityEngine;

// Token: 0x020000B3 RID: 179
public class KnightDong : MonoBehaviour, IInteractableNetworkObj
{
	// Token: 0x0600070E RID: 1806 RVA: 0x0001B1EB File Offset: 0x000193EB
	public void NetInteract()
	{
		this.FkN.KnightDonged();
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x0001B1F8 File Offset: 0x000193F8
	public void ActualInteraction()
	{
		this.KnightAudio.PlayOneShot(this.dong);
	}

	// Token: 0x04000385 RID: 901
	public AudioSource KnightAudio;

	// Token: 0x04000386 RID: 902
	public AudioClip dong;

	// Token: 0x04000387 RID: 903
	public FallenKnightNet FkN;
}
