using System;
using System.Collections;
using System.Collections.Generic;
using Dissonance;
using FishNet.Object;
using Recognissimo;
using Recognissimo.Components;
using UnityEngine;

// Token: 0x020001CB RID: 459
public class VoiceControlListener : NetworkBehaviour
{
	// Token: 0x060012D7 RID: 4823 RVA: 0x0004FFBB File Offset: 0x0004E1BB
	public override void OnStartClient()
	{
		base.OnStartClient();
		base.StartCoroutine(this.waitgetplayer());
		this.SpellPages = new List<ISpellCommand>();
	}

	// Token: 0x060012D8 RID: 4824 RVA: 0x0004FFDB File Offset: 0x0004E1DB
	public bool isspellspagesinited()
	{
		return this.SpellPages == null;
	}

	// Token: 0x060012D9 RID: 4825 RVA: 0x0004FFE6 File Offset: 0x0004E1E6
	private IEnumerator waitgetplayer()
	{
		while (this.pi == null)
		{
			PlayerInventory playerInventory;
			if (Camera.main.transform.parent != null && Camera.main.transform.parent.TryGetComponent<PlayerInventory>(out playerInventory))
			{
				this.pi = playerInventory;
			}
			yield return null;
		}
		base.GetComponent<SetUpModelProvider>().Setup();
		yield return null;
		this.sr = base.GetComponent<SpeechRecognizer>();
		this.SpellPages = new List<ISpellCommand>();
		MonoBehaviour[] components = base.gameObject.GetComponents<MonoBehaviour>();
		for (int i = 0; i < components.Length; i++)
		{
			ISpellCommand spellCommand = components[i] as ISpellCommand;
			if (spellCommand != null)
			{
				this.SpellPages.Add(spellCommand);
			}
		}
		foreach (ISpellCommand spellCommand2 in this.SpellPages)
		{
			spellCommand2.ResetVoiceDetect();
		}
		this.sr.Vocabulary.Add("mirror");
		this.sr.Vocabulary.Add("fireball");
		this.sr.Vocabulary.Add("freeze");
		this.sr.Vocabulary.Add("ease");
		this.sr.Vocabulary.Add("frees");
		this.sr.Vocabulary.Add("worm");
		this.sr.Vocabulary.Add("hole");
		this.sr.Vocabulary.Add("magic missle");
		this.sr.Vocabulary.Add("[unk]");
		this.sr.ResultReady.AddListener(delegate(Result res)
		{
			this.tryresult(res.text);
		});
		yield return new WaitForSeconds(1f);
		base.GetComponent<SpeechRecognizer>().StartProcessing();
		while (base.isActiveAndEnabled)
		{
			yield return new WaitForSeconds(30f);
			if (!this.vbt.IsTransmitting)
			{
				this.sr.StopProcessing();
				base.StartCoroutine(this.restartsr());
			}
		}
		yield break;
	}

	// Token: 0x060012DA RID: 4826 RVA: 0x0004FFF5 File Offset: 0x0004E1F5
	public void AddSpellToList(ISpellCommand ispc)
	{
		this.SpellPages.Add(ispc);
	}

	// Token: 0x060012DB RID: 4827 RVA: 0x00050004 File Offset: 0x0004E204
	public void tryresult(string res)
	{
		if (res != null)
		{
			if (res.Contains("fire") || res.Contains("ball"))
			{
				this.CastFireball();
			}
			else if (res.Contains("freeze") || res.Contains("ease") || res.Contains("frees"))
			{
				this.CastFrostBolt();
			}
			else if (res.Contains("worm"))
			{
				this.CastWorm();
			}
			else if (res.Contains("hole"))
			{
				this.CastHole();
			}
			else if (res.Contains("missle") || res.Contains("magic"))
			{
				this.CastMagicMissle();
			}
			else if (res.Contains("mirror"))
			{
				this.ActivateMirror();
			}
			if (res.Contains("blank"))
			{
				using (List<ISpellCommand>.Enumerator enumerator = this.SpellPages.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ISpellCommand spellCommand = enumerator.Current;
						if (spellCommand != null && spellCommand.GetSpellName() == "blink")
						{
							spellCommand.TryCastSpell();
						}
					}
					return;
				}
			}
			if (res.Contains("dark"))
			{
				using (List<ISpellCommand>.Enumerator enumerator = this.SpellPages.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ISpellCommand spellCommand2 = enumerator.Current;
						if (spellCommand2 != null && spellCommand2.GetSpellName() == "blast")
						{
							spellCommand2.TryCastSpell();
						}
					}
					return;
				}
			}
			using (List<ISpellCommand>.Enumerator enumerator = this.SpellPages.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ISpellCommand spellCommand3 = enumerator.Current;
					if (spellCommand3 != null && res.Contains(spellCommand3.GetSpellName()))
					{
						spellCommand3.TryCastSpell();
					}
				}
				return;
			}
		}
		this.sr.StopProcessing();
		base.StartCoroutine(this.restartsr());
	}

	// Token: 0x060012DC RID: 4828 RVA: 0x00050204 File Offset: 0x0004E404
	public void ActivateMirror()
	{
		if (this.pi != null)
		{
			this.pi.TryActivateMirror(this.magicMirrors, this.vbt);
		}
		this.sr.StopProcessing();
		base.StartCoroutine(this.restartsr());
	}

	// Token: 0x060012DD RID: 4829 RVA: 0x00050243 File Offset: 0x0004E443
	public void CastFireball()
	{
		if (this.pi != null)
		{
			this.pi.cFireball();
		}
		this.sr.StopProcessing();
		base.StartCoroutine(this.restartsr());
	}

	// Token: 0x060012DE RID: 4830 RVA: 0x00050276 File Offset: 0x0004E476
	public void CastFrostBolt()
	{
		if (this.pi != null)
		{
			this.pi.cFrostbolt();
		}
		this.sr.StopProcessing();
		base.StartCoroutine(this.restartsr());
	}

	// Token: 0x060012DF RID: 4831 RVA: 0x000502A9 File Offset: 0x0004E4A9
	public void CastWorm()
	{
		if (this.pi != null)
		{
			this.pi.cCastworm();
		}
		this.sr.StopProcessing();
		base.StartCoroutine(this.restartsr());
	}

	// Token: 0x060012E0 RID: 4832 RVA: 0x000502DC File Offset: 0x0004E4DC
	public void CastHole()
	{
		if (this.pi != null)
		{
			this.pi.cCasthole();
		}
		this.sr.StopProcessing();
		base.StartCoroutine(this.restartsr());
	}

	// Token: 0x060012E1 RID: 4833 RVA: 0x0005030F File Offset: 0x0004E50F
	public void CastMagicMissle()
	{
		if (this.pi != null)
		{
			this.pi.cCastWard();
		}
		this.sr.StopProcessing();
		base.StartCoroutine(this.restartsr());
	}

	// Token: 0x060012E2 RID: 4834 RVA: 0x00050342 File Offset: 0x0004E542
	private IEnumerator restartsr()
	{
		while (this.sr.State != SpeechProcessorState.Inactive)
		{
			yield return null;
		}
		this.sr.StartProcessing();
		yield break;
	}

	// Token: 0x060012E3 RID: 4835 RVA: 0x00050351 File Offset: 0x0004E551
	public void resetmic()
	{
		if (Time.time - this.resetmiccooldown > 10f)
		{
			this.resetmiccooldown = Time.time;
			base.StartCoroutine(this.resetmiclong());
		}
	}

	// Token: 0x060012E4 RID: 4836 RVA: 0x0005037E File Offset: 0x0004E57E
	private IEnumerator resetmiclong()
	{
		this.sr.StopProcessing();
		yield return new WaitForSeconds(0.5f);
		Object.Destroy(this.sr);
		yield return new WaitForSeconds(0.5f);
		this.sr = base.gameObject.AddComponent<SpeechRecognizer>();
		this.sr.LanguageModelProvider = base.GetComponent<StreamingAssetsLanguageModelProvider>();
		this.sr.SpeechSource = base.GetComponent<DissonanceSpeechSource>();
		this.sr.Vocabulary = new List<string>();
		this.SpellPages = new List<ISpellCommand>();
		MonoBehaviour[] components = base.gameObject.GetComponents<MonoBehaviour>();
		for (int i = 0; i < components.Length; i++)
		{
			ISpellCommand spellCommand = components[i] as ISpellCommand;
			if (spellCommand != null)
			{
				this.SpellPages.Add(spellCommand);
			}
		}
		this.sr.Vocabulary.Add("mirror");
		this.sr.Vocabulary.Add("fire ball");
		this.sr.Vocabulary.Add("freeze");
		this.sr.Vocabulary.Add("frees");
		this.sr.Vocabulary.Add("ease");
		this.sr.Vocabulary.Add("worm");
		this.sr.Vocabulary.Add("hole");
		this.sr.Vocabulary.Add("magic missle");
		this.sr.Vocabulary.Add("[unk]");
		foreach (ISpellCommand spellCommand2 in this.SpellPages)
		{
			spellCommand2.ResetVoiceDetect();
		}
		this.sr.ResultReady.AddListener(delegate(Result res)
		{
			this.tryresult(res.text);
		});
		yield return new WaitForSeconds(0.1f);
		this.sr.StartProcessing();
		yield break;
	}

	// Token: 0x060012E5 RID: 4837 RVA: 0x000021EF File Offset: 0x000003EF
	public void startit()
	{
	}

	// Token: 0x060012E6 RID: 4838 RVA: 0x000021EF File Offset: 0x000003EF
	public void stopit()
	{
	}

	// Token: 0x060012E7 RID: 4839 RVA: 0x000021EF File Offset: 0x000003EF
	public void damnit()
	{
	}

	// Token: 0x060012EB RID: 4843 RVA: 0x0005039B File Offset: 0x0004E59B
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyVoiceControlListenerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyVoiceControlListenerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x060012EC RID: 4844 RVA: 0x000503AE File Offset: 0x0004E5AE
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateVoiceControlListenerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateVoiceControlListenerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x060012ED RID: 4845 RVA: 0x000503C1 File Offset: 0x0004E5C1
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x060012EE RID: 4846 RVA: 0x000503C1 File Offset: 0x0004E5C1
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000B03 RID: 2819
	public PlayerInventory pi;

	// Token: 0x04000B04 RID: 2820
	private SpeechRecognizer sr;

	// Token: 0x04000B05 RID: 2821
	public VoiceBroadcastTrigger vbt;

	// Token: 0x04000B06 RID: 2822
	public List<ISpellCommand> SpellPages;

	// Token: 0x04000B07 RID: 2823
	public MagicMirrorController[] magicMirrors;

	// Token: 0x04000B08 RID: 2824
	public bool istutorial;

	// Token: 0x04000B09 RID: 2825
	private float resetmiccooldown;

	// Token: 0x04000B0A RID: 2826
	private bool NetworkInitialize___EarlyVoiceControlListenerAssembly-CSharp.dll_Excuted;

	// Token: 0x04000B0B RID: 2827
	private bool NetworkInitialize__LateVoiceControlListenerAssembly-CSharp.dll_Excuted;
}
