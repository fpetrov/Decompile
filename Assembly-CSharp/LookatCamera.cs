using System;
using UnityEngine;

// Token: 0x020000C5 RID: 197
public class LookatCamera : MonoBehaviour
{
	// Token: 0x06000791 RID: 1937 RVA: 0x0001D004 File Offset: 0x0001B204
	private void Update()
	{
		base.transform.LookAt(Camera.main.transform);
	}
}
