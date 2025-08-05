using System;
using UnityEngine;

// Token: 0x02000014 RID: 20
public class BookController : MonoBehaviour
{
	// Token: 0x060000D3 RID: 211 RVA: 0x00005748 File Offset: 0x00003948
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			this.ani1.SetBool("eq", true);
			this.ani2.SetBool("eq", true);
			this.ani3.SetBool("eq", true);
			this.ani4.SetBool("eq", true);
			this.ani5.SetBool("eq", true);
		}
		if (Input.GetKeyDown(KeyCode.Q))
		{
			this.ani1.SetBool("eq", false);
			this.ani2.SetBool("eq", false);
			this.ani3.SetBool("eq", false);
			this.ani4.SetBool("eq", false);
			this.ani5.SetBool("eq", false);
		}
	}

	// Token: 0x0400006B RID: 107
	public Animator ani1;

	// Token: 0x0400006C RID: 108
	public Animator ani2;

	// Token: 0x0400006D RID: 109
	public Animator ani3;

	// Token: 0x0400006E RID: 110
	public Animator ani4;

	// Token: 0x0400006F RID: 111
	public Animator ani5;
}
