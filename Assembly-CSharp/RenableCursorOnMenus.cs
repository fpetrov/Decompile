using System;
using UnityEngine;

// Token: 0x02000158 RID: 344
public class RenableCursorOnMenus : MonoBehaviour
{
	// Token: 0x06000ECE RID: 3790 RVA: 0x0003BD21 File Offset: 0x00039F21
	private void OnEnable()
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}
}
