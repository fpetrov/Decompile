using System;
using BepInEx;
using HarmonyLib;

// Token: 0x0200001A RID: 26
[BepInPlugin("com.onigremlin.magustoolkit", "MagusToolkit", "1.0.0")]
public class MageArenaMod : BaseUnityPlugin
{
	// Token: 0x0600003E RID: 62 RVA: 0x000038E5 File Offset: 0x00001AE5
	private void Awake()
	{
		base.Logger.LogInfo("Mod Initialized");
		base.gameObject.AddComponent<UIMenuIMGUI>();
	}

	// Token: 0x0600003F RID: 63 RVA: 0x00003904 File Offset: 0x00001B04
	private void OnEnable()
	{
		base.Logger.LogInfo("[MagusToolkit] OnEnable called");
		ConfigManager.Init(base.Config, base.Logger);
		new Harmony("com.onigremlin.magustoolkit").PatchAll();
		base.Logger.LogInfo("Harmony patches applied.");
	}

	// Token: 0x06000040 RID: 64 RVA: 0x00003951 File Offset: 0x00001B51
	private void OnDisable()
	{
		base.Logger.LogInfo("[MagusToolkit] OnDisable called");
	}
}
