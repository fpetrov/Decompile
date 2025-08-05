using System;
using UnityEngine;

// Token: 0x020001F2 RID: 498
public class NpcOpenDoor : MonoBehaviour
{
	// Token: 0x06001425 RID: 5157 RVA: 0x00053F18 File Offset: 0x00052118
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("PlayerNpc") || other.CompareTag("hitable"))
		{
			foreach (DoorOpen doorOpen in this.door)
			{
				if (!doorOpen.opened)
				{
					doorOpen.NetInteract();
				}
			}
		}
	}

	// Token: 0x06001426 RID: 5158 RVA: 0x00053F68 File Offset: 0x00052168
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("PlayerNpc") || other.CompareTag("hitable"))
		{
			foreach (DoorOpen doorOpen in this.door)
			{
				if (doorOpen.opened && !doorOpen.OpenedByPlayer)
				{
					doorOpen.NetInteract();
				}
			}
		}
	}

	// Token: 0x04000BB9 RID: 3001
	public DoorOpen[] door;

	// Token: 0x04000BBA RID: 3002
	public float dooropentime = 3f;
}
