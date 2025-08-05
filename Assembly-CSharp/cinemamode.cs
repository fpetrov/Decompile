using System;
using UnityEngine;

// Token: 0x02000032 RID: 50
public class cinemamode : MonoBehaviour
{
	// Token: 0x06000234 RID: 564 RVA: 0x0000A0C0 File Offset: 0x000082C0
	private void Update()
	{
		if (Input.GetKey(KeyCode.D))
		{
			base.transform.position = new Vector3(base.transform.position.x + this.movespeed * Time.deltaTime, base.transform.position.y, base.transform.position.z);
		}
		if (Input.GetKey(KeyCode.A))
		{
			base.transform.position = new Vector3(base.transform.position.x - this.movespeed * Time.deltaTime, base.transform.position.y, base.transform.position.z);
		}
		if (Input.GetKey(KeyCode.W))
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, base.transform.position.z + this.movespeed * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.S))
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, base.transform.position.z - this.movespeed * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.E))
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + this.movespeed * Time.deltaTime, base.transform.position.z);
		}
		if (Input.GetKey(KeyCode.Q))
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y - this.movespeed * Time.deltaTime, base.transform.position.z);
		}
	}

	// Token: 0x0400011D RID: 285
	public float movespeed = 1f;
}
