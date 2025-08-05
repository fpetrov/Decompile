using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001E9 RID: 489
public class DoorInteract : MonoBehaviour, ITimedInteractable
{
	// Token: 0x060013BE RID: 5054 RVA: 0x00052BB0 File Offset: 0x00050DB0
	public void Interact(GameObject player)
	{
		this.EntrancePoint.GetComponent<AudioSource>().PlayOneShot(this.acl);
		player.transform.position = this.EntrancePoint.position;
		player.GetComponent<PlayerMovement>().canRecall = false;
		base.StartCoroutine(this.TelePlayer(player));
	}

	// Token: 0x060013BF RID: 5055 RVA: 0x00052C03 File Offset: 0x00050E03
	private IEnumerator TelePlayer(GameObject player)
	{
		GameObject.FindGameObjectWithTag("Weather").GetComponent<Light>().enabled = false;
		this.tdgmr.enableAll();
		int num;
		for (int i = 0; i < 20; i = num + 1)
		{
			player.transform.position = this.EntrancePoint.position;
			yield return null;
			num = i;
		}
		yield break;
	}

	// Token: 0x060013C0 RID: 5056 RVA: 0x00052C19 File Offset: 0x00050E19
	public float GetInteractTimer(GameObject player)
	{
		return 2f;
	}

	// Token: 0x060013C1 RID: 5057 RVA: 0x00052C20 File Offset: 0x00050E20
	public string DisplayInteractUI()
	{
		return "Enter Mausoleum";
	}

	// Token: 0x04000B95 RID: 2965
	public AudioClip acl;

	// Token: 0x04000B96 RID: 2966
	public Transform EntrancePoint;

	// Token: 0x04000B97 RID: 2967
	public ToggleDungeonMeshRenders tdgmr;
}
