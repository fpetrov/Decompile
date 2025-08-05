using System;
using Recognissimo.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Recognissimo.Samples.VoiceActivityDetectorExample
{
	// Token: 0x02000233 RID: 563
	[AddComponentMenu("")]
	public class VoiceActivityDetectorExample : MonoBehaviour
	{
		// Token: 0x06001629 RID: 5673 RVA: 0x0005CAA4 File Offset: 0x0005ACA4
		private void Start()
		{
			this.activityDetector.TimeoutMs = 0;
			this.activityDetector.Spoke.AddListener(delegate
			{
				this.status.text = "<color=green>Speech</color>";
			});
			this.activityDetector.Silenced.AddListener(delegate
			{
				this.status.text = "<color=red>Silence</color>";
			});
			this.activityDetector.InitializationFailed.AddListener(delegate(InitializationException e)
			{
				this.status.text = e.Message;
			});
			this.activityDetector.StartProcessing();
		}

		// Token: 0x04000CD0 RID: 3280
		[SerializeField]
		private VoiceActivityDetector activityDetector;

		// Token: 0x04000CD1 RID: 3281
		[SerializeField]
		private Text status;
	}
}
