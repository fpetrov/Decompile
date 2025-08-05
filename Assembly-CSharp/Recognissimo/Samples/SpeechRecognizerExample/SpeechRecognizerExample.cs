using System;
using System.Collections.Generic;
using System.Linq;
using Recognissimo.Components;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Recognissimo.Samples.SpeechRecognizerExample
{
	// Token: 0x02000234 RID: 564
	[AddComponentMenu("")]
	public class SpeechRecognizerExample : MonoBehaviour
	{
		// Token: 0x0600162E RID: 5678 RVA: 0x0005CB54 File Offset: 0x0005AD54
		private void OnEnable()
		{
			if (this.languageModelProvider.languageModels.Count == 0)
			{
				throw new InvalidOperationException("No language models.");
			}
			this._availableLanguages = this.languageModelProvider.languageModels.Select((StreamingAssetsLanguageModel languageModel) => languageModel.language).ToList<SystemLanguage>();
			this.languageModelProvider.language = SpeechRecognizerExample.GetPreferredLanguage(this._availableLanguages);
			this.InitializeLanguageDropdown();
			this.UpdateStatus("");
			this.recognizer.Started.AddListener(delegate
			{
				this._recognizedText.Clear();
				this.UpdateStatus("");
			});
			this.recognizer.Finished.AddListener(delegate
			{
				Debug.Log("Finished");
			});
			this.recognizer.PartialResultReady.AddListener(new UnityAction<PartialResult>(this.OnPartialResult));
			this.recognizer.ResultReady.AddListener(new UnityAction<Result>(this.OnResult));
			this.recognizer.InitializationFailed.AddListener(new UnityAction<InitializationException>(this.OnError));
			this.recognizer.RuntimeFailed.AddListener(new UnityAction<RuntimeException>(this.OnError));
			this.startButton.onClick.AddListener(delegate
			{
				this.UpdateStatus("Loading...");
				this.recognizer.StartProcessing();
			});
		}

		// Token: 0x0600162F RID: 5679 RVA: 0x0005CCB6 File Offset: 0x0005AEB6
		private static SystemLanguage GetPreferredLanguage(IList<SystemLanguage> availableLanguages)
		{
			if (availableLanguages.Count == 0)
			{
				throw new InvalidOperationException("No available languages.");
			}
			if (availableLanguages.Contains(Application.systemLanguage))
			{
				return Application.systemLanguage;
			}
			if (availableLanguages.Contains(SystemLanguage.English))
			{
				return SystemLanguage.English;
			}
			return availableLanguages[0];
		}

		// Token: 0x06001630 RID: 5680 RVA: 0x0005CCF4 File Offset: 0x0005AEF4
		private void InitializeLanguageDropdown()
		{
			this.languageDropdown.options = this._availableLanguages.Select((SystemLanguage language) => new Dropdown.OptionData
			{
				text = language.ToString()
			}).ToList<Dropdown.OptionData>();
			this.languageDropdown.value = this.languageDropdown.options.FindIndex((Dropdown.OptionData option) => option.text == this.languageModelProvider.language.ToString());
			this.languageDropdown.onValueChanged.AddListener(delegate(int index)
			{
				string text = this.languageDropdown.options[index].text;
				SystemLanguage systemLanguage = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), text);
				this.languageModelProvider.language = systemLanguage;
			});
		}

		// Token: 0x06001631 RID: 5681 RVA: 0x0005CD7E File Offset: 0x0005AF7E
		private void UpdateStatus(string text)
		{
			this.status.text = text;
		}

		// Token: 0x06001632 RID: 5682 RVA: 0x0005CD8C File Offset: 0x0005AF8C
		private void OnPartialResult(PartialResult partial)
		{
			this._recognizedText.Append(partial);
			this.UpdateStatus(this._recognizedText.CurrentText);
		}

		// Token: 0x06001633 RID: 5683 RVA: 0x0005CDAB File Offset: 0x0005AFAB
		private void OnResult(Result result)
		{
			this._recognizedText.Append(result);
			this.UpdateStatus(this._recognizedText.CurrentText);
		}

		// Token: 0x06001634 RID: 5684 RVA: 0x0005CDCA File Offset: 0x0005AFCA
		private void OnError(SpeechProcessorException exception)
		{
			this.UpdateStatus("<color=red>" + exception.Message + "</color>");
		}

		// Token: 0x04000CD2 RID: 3282
		private const string LoadingMessage = "Loading...";

		// Token: 0x04000CD3 RID: 3283
		[SerializeField]
		private SpeechRecognizer recognizer;

		// Token: 0x04000CD4 RID: 3284
		[SerializeField]
		private StreamingAssetsLanguageModelProvider languageModelProvider;

		// Token: 0x04000CD5 RID: 3285
		[SerializeField]
		private Dropdown languageDropdown;

		// Token: 0x04000CD6 RID: 3286
		[SerializeField]
		private Button startButton;

		// Token: 0x04000CD7 RID: 3287
		[SerializeField]
		private InputField status;

		// Token: 0x04000CD8 RID: 3288
		private readonly SpeechRecognizerExample.RecognizedText _recognizedText = new SpeechRecognizerExample.RecognizedText();

		// Token: 0x04000CD9 RID: 3289
		private List<SystemLanguage> _availableLanguages;

		// Token: 0x02000235 RID: 565
		private class RecognizedText
		{
			// Token: 0x17000256 RID: 598
			// (get) Token: 0x0600163A RID: 5690 RVA: 0x0005CE96 File Offset: 0x0005B096
			public string CurrentText
			{
				get
				{
					return this._stableText + " <color=grey>" + this._changingText + "</color>";
				}
			}

			// Token: 0x0600163B RID: 5691 RVA: 0x0005CEB3 File Offset: 0x0005B0B3
			public void Append(Result result)
			{
				this._changingText = "";
				this._stableText = this._stableText + " " + result.text;
			}

			// Token: 0x0600163C RID: 5692 RVA: 0x0005CEDC File Offset: 0x0005B0DC
			public void Append(PartialResult partialResult)
			{
				this._changingText = partialResult.partial;
			}

			// Token: 0x0600163D RID: 5693 RVA: 0x0005CEEA File Offset: 0x0005B0EA
			public void Clear()
			{
				this._changingText = "";
				this._stableText = "";
			}

			// Token: 0x04000CDA RID: 3290
			private string _changingText;

			// Token: 0x04000CDB RID: 3291
			private string _stableText;
		}
	}
}
