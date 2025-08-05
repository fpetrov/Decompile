using System;
using UnityEngine;

// Token: 0x020000FC RID: 252
public class OceanTriggershit : MonoBehaviour
{
	// Token: 0x06000A29 RID: 2601 RVA: 0x00026A58 File Offset: 0x00024C58
	private void OnTriggerEnter(Collider other)
	{
		PlayerMovement playerMovement;
		if (other.TryGetComponent<PlayerMovement>(out playerMovement))
		{
			playerMovement.playerAudio.oceanint = 1;
		}
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x00026A7C File Offset: 0x00024C7C
	private void OnTriggerExit(Collider other)
	{
		PlayerMovement playerMovement;
		if (other.TryGetComponent<PlayerMovement>(out playerMovement))
		{
			playerMovement.playerAudio.oceanint = 0;
		}
	}
}
