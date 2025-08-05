using System;
using UnityEngine;

// Token: 0x0200010D RID: 269
public interface ITimedInteractable
{
	// Token: 0x06000ACD RID: 2765
	float GetInteractTimer(GameObject player);

	// Token: 0x06000ACE RID: 2766
	void Interact(GameObject player);

	// Token: 0x06000ACF RID: 2767
	string DisplayInteractUI();
}
