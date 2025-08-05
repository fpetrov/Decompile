using System;
using System.Collections.Generic;
using Recognissimo.Components;
using TMPro;
using UnityEngine;

// Token: 0x02000178 RID: 376
public class SetLabelMic : MonoBehaviour
{
	// Token: 0x06000F4E RID: 3918 RVA: 0x0003E200 File Offset: 0x0003C400
	private void Start()
	{
		List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
		foreach (string text in Microphone.devices)
		{
			list.Add(new TMP_Dropdown.OptionData(text));
		}
		this.dropdown.options = list;
	}

	// Token: 0x06000F4F RID: 3919 RVA: 0x0003E243 File Offset: 0x0003C443
	private void OnDisable()
	{
		this.mss.DeviceName = this.dropdown.captionText.text;
	}

	// Token: 0x040008C4 RID: 2244
	public TMP_Dropdown dropdown;

	// Token: 0x040008C5 RID: 2245
	public MicrophoneSpeechSource mss;
}
