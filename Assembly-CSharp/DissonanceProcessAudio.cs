using System;
using Dissonance;
using NAudio.Wave;
using UnityEngine;

// Token: 0x02000053 RID: 83
public class DissonanceProcessAudio : BaseMicrophoneSubscriber
{
	// Token: 0x06000377 RID: 887 RVA: 0x0000E600 File Offset: 0x0000C800
	public void OnEnable()
	{
		Object.FindFirstObjectByType<DissonanceComms>().SubscribeToRecordedAudio(this);
	}

	// Token: 0x06000378 RID: 888 RVA: 0x0000E610 File Offset: 0x0000C810
	protected override void ProcessAudio(ArraySegment<float> data)
	{
		float[] array = data.ToArray();
		this.speechSource.ProcessAudio(array);
	}

	// Token: 0x06000379 RID: 889 RVA: 0x0000E631 File Offset: 0x0000C831
	protected override void ResetAudioStream(WaveFormat waveFormat)
	{
		if (waveFormat.Channels != 1)
		{
			throw new NotImplementedException("Stereo support is not implemented yet");
		}
	}

	// Token: 0x040001BF RID: 447
	[SerializeField]
	private DissonanceSpeechSource speechSource;
}
