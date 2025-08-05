using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x02000197 RID: 407
public class SmokeRingController : MonoBehaviour
{
	// Token: 0x06001108 RID: 4360 RVA: 0x00049B1E File Offset: 0x00047D1E
	public void SetupRing(Vector3 FwdVector)
	{
		this.mainfog = base.GetComponent<LocalVolumetricFog>();
		this.holefog.gameObject.SetActive(true);
		base.StartCoroutine(this.RingRoutine(FwdVector));
	}

	// Token: 0x06001109 RID: 4361 RVA: 0x00049B4B File Offset: 0x00047D4B
	private IEnumerator RingRoutine(Vector3 fwdvector)
	{
		float timer = 0f;
		Vector3 startpos = base.transform.position;
		while (timer < 1f)
		{
			this.holefog.parameters.size = Vector3.Lerp(this.holestartsize, this.holeendsize / 2f, timer);
			this.mainfog.parameters.size = Vector3.Lerp(this.mainstartsize, this.mainendsize / 2f, timer);
			base.transform.position = Vector3.Slerp(startpos, startpos + fwdvector, timer);
			timer += Time.deltaTime;
			yield return null;
		}
		timer = 0f;
		startpos = base.transform.position;
		Quaternion startrot = base.transform.rotation;
		while (timer < 10f)
		{
			this.holefog.parameters.size = Vector3.Lerp(this.holeendsize / 2f, this.holeendsize, timer / 10f);
			this.mainfog.parameters.size = Vector3.Lerp(this.mainendsize / 2f, this.mainendsize, timer / 10f);
			base.transform.rotation = Quaternion.Slerp(startrot, Quaternion.Euler(0f, 0f, 0f), timer / 10f);
			base.transform.position = Vector3.Slerp(startpos, startpos + fwdvector / 2f + Vector3.up, timer / 10f);
			timer += Time.deltaTime;
			yield return null;
		}
		timer = 0f;
		startpos = base.transform.position;
		while (timer < 5f)
		{
			this.mainfog.parameters.meanFreePath = Mathf.Lerp(0.05f, 3f, timer / 5f);
			this.holefog.parameters.size = Vector3.Lerp(this.holeendsize, new Vector3(1.8f, 1f, 1.8f), timer / 5f);
			this.mainfog.parameters.size = Vector3.Lerp(this.mainendsize, new Vector3(2f, 0.32f, 2f), timer / 5f);
			base.transform.position = Vector3.Lerp(startpos, startpos + Vector3.up, timer / 5f);
			timer += Time.deltaTime;
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x040009E8 RID: 2536
	public LocalVolumetricFog holefog;

	// Token: 0x040009E9 RID: 2537
	private LocalVolumetricFog mainfog;

	// Token: 0x040009EA RID: 2538
	public Vector3 mainstartsize;

	// Token: 0x040009EB RID: 2539
	public Vector3 mainendsize;

	// Token: 0x040009EC RID: 2540
	public Vector3 holestartsize;

	// Token: 0x040009ED RID: 2541
	public Vector3 holeendsize;
}
