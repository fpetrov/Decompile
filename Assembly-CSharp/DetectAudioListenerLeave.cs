using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200004E RID: 78
public class DetectAudioListenerLeave : MonoBehaviour
{
	// Token: 0x0600035E RID: 862 RVA: 0x0000E21B File Offset: 0x0000C41B
	private void Start()
	{
		base.StartCoroutine(this.GetMCAudioListener());
	}

	// Token: 0x0600035F RID: 863 RVA: 0x0000E22A File Offset: 0x0000C42A
	private IEnumerator GetMCAudioListener()
	{
		while (this.al == null)
		{
			AudioListener audioListener;
			if (Camera.main.TryGetComponent<AudioListener>(out audioListener))
			{
				this.al = audioListener;
			}
			yield return null;
		}
		this.hasGottenAL = true;
		yield break;
	}

	// Token: 0x06000360 RID: 864 RVA: 0x0000E239 File Offset: 0x0000C439
	private void Update()
	{
		if (this.hasGottenAL && this.al == null && !this.BackupAudioListener.activeSelf)
		{
			this.BackupAudioListener.SetActive(true);
		}
	}

	// Token: 0x040001AF RID: 431
	private AudioListener al;

	// Token: 0x040001B0 RID: 432
	private bool hasGottenAL;

	// Token: 0x040001B1 RID: 433
	public GameObject BackupAudioListener;
}
