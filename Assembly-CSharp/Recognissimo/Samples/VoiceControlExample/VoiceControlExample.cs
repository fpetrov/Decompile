using System;
using System.Collections;
using System.Collections.Generic;
using Recognissimo.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Recognissimo.Samples.VoiceControlExample
{
	// Token: 0x02000230 RID: 560
	[AddComponentMenu("")]
	public class VoiceControlExample : MonoBehaviour
	{
		// Token: 0x0600161A RID: 5658 RVA: 0x0005C898 File Offset: 0x0005AA98
		private void Start()
		{
			this.voiceControl.AsapMode = true;
			foreach (string text in this.phrases)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.textPrefab, this.textParent);
				gameObject.SetActive(true);
				Text textComp = gameObject.GetComponent<Text>();
				textComp.text = text;
				textComp.color = VoiceControlExample.Inactive;
				this.voiceControl.Commands.Add(new VoiceControlCommand(text, delegate
				{
					this.Highlight(textComp);
				}));
			}
			this.voiceControl.InitializationFailed.AddListener(delegate(InitializationException e)
			{
				this.ShowError(e.Message);
			});
			this.voiceControl.StartProcessing();
		}

		// Token: 0x0600161B RID: 5659 RVA: 0x0005C98C File Offset: 0x0005AB8C
		private void Highlight(Graphic text)
		{
			if (text.color == VoiceControlExample.Active)
			{
				return;
			}
			base.StartCoroutine(VoiceControlExample.HighlightCoroutine(text));
		}

		// Token: 0x0600161C RID: 5660 RVA: 0x0005C9AE File Offset: 0x0005ABAE
		private static IEnumerator HighlightCoroutine(Graphic text)
		{
			text.color = VoiceControlExample.Active;
			yield return new WaitForSeconds(1f);
			text.color = VoiceControlExample.Inactive;
			yield break;
		}

		// Token: 0x0600161D RID: 5661 RVA: 0x0005C9BD File Offset: 0x0005ABBD
		private void ShowError(string text)
		{
			this.status.gameObject.SetActive(true);
			this.status.color = Color.red;
			this.status.text = text;
		}

		// Token: 0x04000CC4 RID: 3268
		[SerializeField]
		private VoiceControl voiceControl;

		// Token: 0x04000CC5 RID: 3269
		[SerializeField]
		private Text status;

		// Token: 0x04000CC6 RID: 3270
		[SerializeField]
		private List<string> phrases;

		// Token: 0x04000CC7 RID: 3271
		[SerializeField]
		private Transform textParent;

		// Token: 0x04000CC8 RID: 3272
		[SerializeField]
		private GameObject textPrefab;

		// Token: 0x04000CC9 RID: 3273
		private static readonly Color Active = Color.green;

		// Token: 0x04000CCA RID: 3274
		private static readonly Color Inactive = Color.black;
	}
}
