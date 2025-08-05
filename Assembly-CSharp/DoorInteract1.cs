using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001EB RID: 491
public class DoorInteract1 : MonoBehaviour, ITimedInteractable
{
	// Token: 0x060013C9 RID: 5065 RVA: 0x00052CE1 File Offset: 0x00050EE1
	private void Start()
	{
		this.EntrancePoint = GameObject.FindGameObjectWithTag("poopiebutt").transform;
	}

	// Token: 0x060013CA RID: 5066 RVA: 0x00052CF8 File Offset: 0x00050EF8
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerMovement>().canRecall = true;
		player.transform.position = this.EntrancePoint.position;
		base.StartCoroutine(this.TelePlayer(player));
	}

	// Token: 0x060013CB RID: 5067 RVA: 0x00052D2A File Offset: 0x00050F2A
	private IEnumerator TelePlayer(GameObject player)
	{
		GameObject.FindGameObjectWithTag("Weather").GetComponent<Light>().enabled = true;
		Object.FindFirstObjectByType<ToggleDungeonMeshRenders>().DisableAll();
		int num;
		for (int i = 0; i < 60; i = num + 1)
		{
			player.transform.position = this.EntrancePoint.position;
			yield return null;
			num = i;
		}
		yield break;
	}

	// Token: 0x060013CC RID: 5068 RVA: 0x00052C19 File Offset: 0x00050E19
	public float GetInteractTimer(GameObject player)
	{
		return 2f;
	}

	// Token: 0x060013CD RID: 5069 RVA: 0x00052D40 File Offset: 0x00050F40
	public string DisplayInteractUI()
	{
		return "Exit Mausoleum";
	}

	// Token: 0x04000B9D RID: 2973
	private Transform EntrancePoint;
}
