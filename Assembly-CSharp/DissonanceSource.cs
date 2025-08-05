using System;
using System.Collections;
using Dissonance;
using NAudio.Wave;
using UnityEngine;

// Token: 0x02000054 RID: 84
public class DissonanceSource : BaseMicrophoneSubscriber
{
	// Token: 0x0600037B RID: 891 RVA: 0x0000E64F File Offset: 0x0000C84F
	private void OnEnable()
	{
		base.StartCoroutine(this.trygetcomms());
	}

	// Token: 0x0600037C RID: 892 RVA: 0x0000E65E File Offset: 0x0000C85E
	private IEnumerator trygetcomms()
	{
		while (this._comms == null)
		{
			this._comms = Object.FindFirstObjectByType<DissonanceComms>();
			yield return null;
		}
		this._comms.SubscribeToRecordedAudio(this);
		yield break;
	}

	// Token: 0x0600037D RID: 893 RVA: 0x0000E66D File Offset: 0x0000C86D
	protected override void ResetAudioStream(WaveFormat fmt)
	{
		if (fmt.Channels != 1)
		{
			Debug.LogWarning(string.Format("Recognizer needs mono; got {0} ch", fmt.Channels));
		}
		this.recognissimoSink.initshit(fmt);
		this.mchz = (float)fmt.SampleRate;
	}

	// Token: 0x0600037E RID: 894 RVA: 0x0000E6AC File Offset: 0x0000C8AC
	protected override void ProcessAudio(ArraySegment<float> data)
	{
		if (this.recognissimoSink == null || this._comms == null || this.mchz == -1f)
		{
			return;
		}
		int num = (int)((float)data.Count / (this.mchz / 16000f));
		if (this.down16kBuffer.Length < num)
		{
			this.down16kBuffer = new float[num];
		}
		int i = 0;
		int num2 = 0;
		while (i < num)
		{
			this.down16kBuffer[i] = data.Array[data.Offset + num2];
			i++;
			num2 += (int)(this.mchz / 16000f);
		}
		this.recognissimoSink.PushSamples(this.down16kBuffer, 0, num);
	}

	// Token: 0x040001C0 RID: 448
	[SerializeField]
	private DissonanceMicSource recognissimoSink;

	// Token: 0x040001C1 RID: 449
	private DissonanceComms _comms;

	// Token: 0x040001C2 RID: 450
	private float[] down16kBuffer = new float[320];

	// Token: 0x040001C3 RID: 451
	private float mchz = -1f;
}
