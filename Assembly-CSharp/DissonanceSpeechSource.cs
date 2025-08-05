using System;
using Recognissimo;

// Token: 0x02000056 RID: 86
public class DissonanceSpeechSource : SpeechSource
{
	// Token: 0x17000054 RID: 84
	// (get) Token: 0x06000386 RID: 902 RVA: 0x0000E802 File Offset: 0x0000CA02
	public override int SampleRate
	{
		get
		{
			return 48000;
		}
	}

	// Token: 0x06000387 RID: 903 RVA: 0x0000E809 File Offset: 0x0000CA09
	public override void StartProducing()
	{
		this._isProducing = true;
	}

	// Token: 0x06000388 RID: 904 RVA: 0x0000E812 File Offset: 0x0000CA12
	public override void StopProducing()
	{
		this._isProducing = false;
	}

	// Token: 0x06000389 RID: 905 RVA: 0x0000E81B File Offset: 0x0000CA1B
	public void ProcessAudio(float[] data)
	{
		if (!this._isProducing)
		{
			return;
		}
		base.OnSamplesReady(new SamplesReadyEventArgs(data, data.Length));
	}

	// Token: 0x040001C7 RID: 455
	private const int DissonanceSampleRate = 48000;

	// Token: 0x040001C8 RID: 456
	private bool _isProducing;
}
