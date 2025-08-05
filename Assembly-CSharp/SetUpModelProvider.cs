using System;
using System.Collections.Generic;
using Recognissimo.Components;
using UnityEngine;

// Token: 0x0200017F RID: 383
public class SetUpModelProvider : MonoBehaviour
{
	// Token: 0x06000F95 RID: 3989 RVA: 0x00040028 File Offset: 0x0003E228
	public void Setup()
	{
		SettingsHolder component = GameObject.FindGameObjectWithTag("Settings").GetComponent<SettingsHolder>();
		StreamingAssetsLanguageModelProvider streamingAssetsLanguageModelProvider = base.gameObject.AddComponent<StreamingAssetsLanguageModelProvider>();
		streamingAssetsLanguageModelProvider.language = SystemLanguage.English;
		streamingAssetsLanguageModelProvider.languageModels = new List<StreamingAssetsLanguageModel>
		{
			new StreamingAssetsLanguageModel
			{
				language = SystemLanguage.English,
				path = component.VoiceModelPath
			}
		};
		base.GetComponent<SpeechRecognizer>().LanguageModelProvider = streamingAssetsLanguageModelProvider;
	}
}
