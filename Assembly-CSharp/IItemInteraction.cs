using System;
using UnityEngine;

// Token: 0x02000111 RID: 273
public interface IItemInteraction
{
	// Token: 0x06000AE4 RID: 2788
	void Interaction(GameObject player);

	// Token: 0x06000AE5 RID: 2789
	void Interaction2(GameObject subject);

	// Token: 0x06000AE6 RID: 2790
	void DropItem();

	// Token: 0x06000AE7 RID: 2791
	void SetScale();

	// Token: 0x06000AE8 RID: 2792
	void ItemInit();

	// Token: 0x06000AE9 RID: 2793
	void ItemInitObs();

	// Token: 0x06000AEA RID: 2794
	void HideItem();

	// Token: 0x06000AEB RID: 2795
	int GetItemID();

	// Token: 0x06000AEC RID: 2796
	void PlayDropSound();
}
