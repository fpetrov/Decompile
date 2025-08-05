using System;
using UnityEngine;

// Token: 0x020000F6 RID: 246
public class NetHudCanvasHider : MonoBehaviour
{
	// Token: 0x06000A07 RID: 2567 RVA: 0x000265F1 File Offset: 0x000247F1
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			base.gameObject.SetActive(false);
		}
	}
}
