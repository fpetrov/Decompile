using System;
using UnityEngine;

// Token: 0x02000081 RID: 129
public class FireballDectetion : MonoBehaviour
{
	// Token: 0x0600056E RID: 1390 RVA: 0x00014FFF File Offset: 0x000131FF
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Fireball"))
		{
			this.dm.FireBallDetected(other.gameObject);
		}
	}

	// Token: 0x040002A7 RID: 679
	public DuendeManager dm;
}
