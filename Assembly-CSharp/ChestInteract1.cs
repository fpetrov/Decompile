using System;
using UnityEngine;

// Token: 0x0200002A RID: 42
public class ChestInteract1 : MonoBehaviour, ITimedInteractable
{
	// Token: 0x060001BD RID: 445 RVA: 0x00008632 File Offset: 0x00006832
	public string DisplayInteractUI()
	{
		return "Opene Chest";
	}

	// Token: 0x060001BE RID: 446 RVA: 0x00008639 File Offset: 0x00006839
	public float GetInteractTimer(GameObject player)
	{
		return 1f;
	}

	// Token: 0x060001BF RID: 447 RVA: 0x00008640 File Offset: 0x00006840
	public void Interact(GameObject player)
	{
		if (!this.cnc.isOpen[this.id])
		{
			this.cnc.ChestInteract(this.id);
		}
	}

	// Token: 0x040000E5 RID: 229
	public ChestNetController1 cnc;

	// Token: 0x040000E6 RID: 230
	public int id;

	// Token: 0x040000E7 RID: 231
	public AudioSource csource;
}
