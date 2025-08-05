using System;
using UnityEngine;

// Token: 0x0200004B RID: 75
public class debugmush : MonoBehaviour
{
	// Token: 0x06000353 RID: 851 RVA: 0x0000E14A File Offset: 0x0000C34A
	private void Update()
	{
		Debug.Log(this.mesh);
	}

	// Token: 0x040001A9 RID: 425
	public Mesh mesh;
}
