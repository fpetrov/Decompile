using System;
using NAudio.Wave;
using Recognissimo;

// Token: 0x02000052 RID: 82
public class DissonanceMicSource : SpeechSource
{
	// Token: 0x06000371 RID: 881 RVA: 0x0000E51A File Offset: 0x0000C71A
	private void Awake()
	{
		this.SampleRate = 16000;
	}

	// Token: 0x06000372 RID: 882 RVA: 0x0000E527 File Offset: 0x0000C727
	public override void StartProducing()
	{
		this._isProducing = true;
	}

	// Token: 0x06000373 RID: 883 RVA: 0x0000E530 File Offset: 0x0000C730
	public override void StopProducing()
	{
		this._isProducing = false;
		base.OnDried();
	}

	// Token: 0x06000374 RID: 884 RVA: 0x0000E540 File Offset: 0x0000C740
	public void initshit(WaveFormat fmt)
	{
		int num = 4000;
		if (this._buffer == null)
		{
			this._buffer = new float[num];
		}
	}

	// Token: 0x06000375 RID: 885 RVA: 0x0000E568 File Offset: 0x0000C768
	public void PushSamples(float[] samples, int offset, int count)
	{
		if (!this._isProducing)
		{
			return;
		}
		int i = offset;
		while (i < offset + count)
		{
			int num = Math.Min(this._buffer.Length - this._bufferPos, offset + count - i);
			Array.Copy(samples, i, this._buffer, this._bufferPos, num);
			this._bufferPos += num;
			i += num;
			if (this._bufferPos == this._buffer.Length)
			{
				base.OnSamplesReady(new SamplesReadyEventArgs(this._buffer, this._buffer.Length));
				this._bufferPos = 0;
			}
		}
	}

	// Token: 0x040001BC RID: 444
	private bool _isProducing;

	// Token: 0x040001BD RID: 445
	private float[] _buffer;

	// Token: 0x040001BE RID: 446
	private int _bufferPos;
}
