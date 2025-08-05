using System;
using UnityEngine;

// Token: 0x0200010C RID: 268
public interface IInteractable
{
	// Token: 0x06000ACB RID: 2763
	void Interact(GameObject player);

	// Token: 0x06000ACC RID: 2764
	string DisplayInteractUI(GameObject player);
}
