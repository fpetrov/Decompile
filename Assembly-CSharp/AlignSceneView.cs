using System;
using UnityEngine;

// Token: 0x0200016D RID: 365
[ExecuteInEditMode]
public class AlignSceneView : MonoBehaviour
{
	// Token: 0x06000F27 RID: 3879 RVA: 0x0003D4FD File Offset: 0x0003B6FD
	private void Awake()
	{
		AlignSceneView.AlignCamera(base.transform);
	}

	// Token: 0x06000F28 RID: 3880 RVA: 0x000021EF File Offset: 0x000003EF
	private static void AlignCamera(Transform target)
	{
	}

	// Token: 0x0400088E RID: 2190
	private Transform target;
}
