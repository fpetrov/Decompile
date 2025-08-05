using System;
using System.Collections;
using Dissonance;
using UnityEngine;

// Token: 0x0200014A RID: 330
public class PushtoTalk : MonoBehaviour
{
	// Token: 0x06000E50 RID: 3664 RVA: 0x0003A66A File Offset: 0x0003886A
	private void Start()
	{
		this.dc = base.GetComponent<DissonanceComms>();
		base.StartCoroutine(this.trygetsettings());
	}

	// Token: 0x06000E51 RID: 3665 RVA: 0x0003A685 File Offset: 0x00038885
	private IEnumerator trygetsettings()
	{
		while (this.Sholder == null)
		{
			this.Sholder = GameObject.FindGameObjectWithTag("Settings").GetComponent<SettingsHolder>();
			yield return new WaitForSeconds(0.5f);
		}
		yield break;
	}

	// Token: 0x040007DA RID: 2010
	private SettingsHolder Sholder;

	// Token: 0x040007DB RID: 2011
	private DissonanceComms dc;
}
