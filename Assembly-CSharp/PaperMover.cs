using System;
using UnityEngine;

// Token: 0x02000105 RID: 261
public class PaperMover : MonoBehaviour
{
	// Token: 0x06000A83 RID: 2691 RVA: 0x00027A98 File Offset: 0x00025C98
	private void Update()
	{
		float axis = Input.GetAxis("Mouse X");
		float axis2 = Input.GetAxis("Mouse Y");
		Input.GetAxis("Mouse ScrollWheel");
		Vector3 vector = new Vector3(axis, axis2, 0f) * this.sensitivity;
		if (vector.magnitude > 0f)
		{
			this.asou.volume = 1f;
		}
		else
		{
			this.asou.volume = Mathf.Lerp(this.asou.volume, 0f, Time.deltaTime);
		}
		base.transform.localPosition += vector;
	}

	// Token: 0x04000595 RID: 1429
	public float sensitivity = 0.1f;

	// Token: 0x04000596 RID: 1430
	public float scrollSensitivity = 1f;

	// Token: 0x04000597 RID: 1431
	public AudioSource asou;
}
