using System;
using UnityEngine;

// Token: 0x0200000C RID: 12
public class BogFrogInteractor : MonoBehaviour, IInteractableNetworkObj
{
	// Token: 0x06000082 RID: 130 RVA: 0x00003C7C File Offset: 0x00001E7C
	public void ActualInteraction()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06000083 RID: 131 RVA: 0x00003C83 File Offset: 0x00001E83
	public void NetInteract()
	{
		this.bfc.hitbyspell();
	}

	// Token: 0x0400002A RID: 42
	public BogFrogSingleController bfc;
}
