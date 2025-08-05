using System;
using UnityEngine;

// Token: 0x0200006D RID: 109
public class EnableMeshRenderers : MonoBehaviour
{
	// Token: 0x060004B6 RID: 1206 RVA: 0x00012C6C File Offset: 0x00010E6C
	public void enableRenderers()
	{
		MeshRenderer[] componentsInChildren = base.GetComponentsInChildren<MeshRenderer>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = true;
		}
		SkinnedMeshRenderer[] componentsInChildren2 = base.GetComponentsInChildren<SkinnedMeshRenderer>(true);
		for (int i = 0; i < componentsInChildren2.Length; i++)
		{
			componentsInChildren2[i].enabled = true;
		}
	}
}
