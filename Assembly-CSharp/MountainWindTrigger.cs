using System;
using UnityEngine;

// Token: 0x020000E9 RID: 233
public class MountainWindTrigger : MonoBehaviour
{
	// Token: 0x0600098E RID: 2446 RVA: 0x00024FFC File Offset: 0x000231FC
	private void OnTriggerEnter(Collider other)
	{
		PlayerMovement playerMovement;
		if (other.CompareTag("Player") && other.TryGetComponent<PlayerMovement>(out playerMovement))
		{
			this.mWind.plyrmov = playerMovement;
		}
	}

	// Token: 0x0600098F RID: 2447 RVA: 0x0002502C File Offset: 0x0002322C
	private void OnTriggerExit(Collider other)
	{
		PlayerMovement playerMovement;
		if (other.CompareTag("Player") && other.TryGetComponent<PlayerMovement>(out playerMovement))
		{
			this.mWind.plyrmov = null;
		}
	}

	// Token: 0x04000512 RID: 1298
	public MoutainWind mWind;
}
