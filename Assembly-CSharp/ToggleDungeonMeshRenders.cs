using System;
using UnityEngine;

// Token: 0x020001B6 RID: 438
public class ToggleDungeonMeshRenders : MonoBehaviour
{
	// Token: 0x06001236 RID: 4662 RVA: 0x0004D53C File Offset: 0x0004B73C
	public void DisableAll()
	{
		MeshRenderer[] componentsInChildren = base.GetComponentsInChildren<MeshRenderer>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
	}

	// Token: 0x06001237 RID: 4663 RVA: 0x0004D568 File Offset: 0x0004B768
	public void enableAll()
	{
		foreach (MeshRenderer meshRenderer in base.GetComponentsInChildren<MeshRenderer>(true))
		{
			meshRenderer.enabled = true;
			PotInteract potInteract;
			if (meshRenderer.TryGetComponent<PotInteract>(out potInteract) && potInteract.didinteract())
			{
				meshRenderer.enabled = false;
			}
		}
	}
}
