using System;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;

// Token: 0x02000002 RID: 2
public static class ConfigManager
{
	// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
	public static void Init(ConfigFile config, ManualLogSource logger)
	{
		ConfigManager.Logger = logger;
		ConfigManager.EnemyChamsColorHex = config.Bind<string>("Visuals", "EnemyChamsColor", "#FF0000", "Chams color for enemies (e.g., #FF0000 = Red)");
		ConfigManager.AllyChamsColorHex = config.Bind<string>("Visuals", "AllyChamsColor", "#00FF00", "Chams color for allies (e.g., #00FF00 = Green)");
		ConfigManager.GodMode = config.Bind<bool>("Cheats", "GodMode", true, "Enable god mode");
		ConfigManager.InfiniteStam = config.Bind<bool>("Cheats", "InfiniteStam", true, "Enable infinite stamina");
		ConfigManager.BoostedXP = config.Bind<bool>("Cheats", "BoostedXP", true, "Enable boosted XP");
		ConfigManager.ForceLevel = config.Bind<bool>("Cheats", "ForceLevel", true, "Force your level in game");
		ConfigManager.NoCooldowns = config.Bind<bool>("Cheats", "NoCooldowns", true, "Enable no spell cooldowns");
		ConfigManager.SpeedMod = config.Bind<bool>("Cheats", "SpeedMod", true, "Enable speed modifier");
		ConfigManager.JumpMod = config.Bind<bool>("Cheats", "JumpMod", true, "Enable jump modifier");
		ConfigManager.InstantDoors = config.Bind<bool>("Cheats", "InstantDoors", true, "Enable opening doors instantly");
		ConfigManager.InstantRecall = config.Bind<bool>("Cheats", "InstantRecall", true, "Enable recall instantly");
		ConfigManager.InstantRespawn = config.Bind<bool>("Cheats", "InstantRespawn", true, "Enable respawn instantly");
		ConfigManager.DisableWind = config.Bind<bool>("Cheats", "DisableWind", true, "Disable mountain wind");
		ConfigManager.KeepItems = config.Bind<bool>("Cheats", "KeepItems", true, "Keep your inventory when you die");
		ConfigManager.ESPChams = config.Bind<bool>("Cheats", "ESPChams", true, "Toggle chams");
		ConfigManager.InstantTrade = config.Bind<bool>("Cheats", "InstantTrade", true, "Toggle instant trades");
		ConfigManager.TradePage = config.Bind<bool>("Cheats", "TradePage", true, "Toggle trades always get a page");
		ConfigManager.NoClip = config.Bind<bool>("Cheats", "NoClip", false, "Enable noclip");
		ConfigManager.XPMultiplier = config.Bind<int>("Cheats", "XPMultiplier", 3, new ConfigDescription("XP multiplier value", new AcceptableValueRange<int>(1, 10), Array.Empty<object>()));
		ConfigManager.ForcedLevelVal = config.Bind<int>("Cheats", "ForcedLevelVal", 10, new ConfigDescription("Level to force", new AcceptableValueRange<int>(1, 100), Array.Empty<object>()));
		ConfigManager.SpeedMultiplier = config.Bind<float>("Cheats", "SpeedMultiplier", 3f, new ConfigDescription("Speed multiplier value", new AcceptableValueRange<float>(1f, 10f), Array.Empty<object>()));
		ConfigManager.JumpMultiplier = config.Bind<float>("Cheats", "JumpMultiplier", 1f, new ConfigDescription("Jump multiplier value", new AcceptableValueRange<float>(1f, 10f), Array.Empty<object>()));
		ConfigManager.FlySpeed = config.Bind<float>("Cheats", "FlySpeed", 20f, new ConfigDescription("Fly/Noclip speed", new AcceptableValueRange<float>(1f, 100f), Array.Empty<object>()));
		ConfigManager.FireballEdits = config.Bind<bool>("Fireball", "FireballEdits", false, "Toggle fireball edits");
		ConfigManager.FireballVelocity = config.Bind<float>("Fireball", "MuzzleVelocity", 20f, new ConfigDescription("Initial speed of fireball", new AcceptableValueRange<float>(0f, 1000f), Array.Empty<object>()));
		ConfigManager.FireballGravity = config.Bind<float>("Fireball", "Gravity", 9.8f, new ConfigDescription("Gravity on fireball arc", new AcceptableValueRange<float>(0f, 100f), Array.Empty<object>()));
		ConfigManager.BlinkMultiplier = config.Bind<float>("Blink", "Multiplier", 1f, new ConfigDescription("Blink multiplier value", new AcceptableValueRange<float>(0f, 10f), Array.Empty<object>()));
		ConfigManager.Logger.LogInfo("[MagusToolkit][ConfigManager] Config Loaded.");
	}

	// Token: 0x06000002 RID: 2 RVA: 0x00002428 File Offset: 0x00000628
	public static Color GetEnemyChamsColor()
	{
		Color color;
		if (ColorUtility.TryParseHtmlString(ConfigManager.EnemyChamsColorHex.Value, ref color))
		{
			return color;
		}
		return Color.red;
	}

	// Token: 0x06000003 RID: 3 RVA: 0x00002450 File Offset: 0x00000650
	public static Color GetAllyChamsColor()
	{
		Color color;
		if (ColorUtility.TryParseHtmlString(ConfigManager.AllyChamsColorHex.Value, ref color))
		{
			return color;
		}
		return Color.green;
	}

	// Token: 0x06000004 RID: 4 RVA: 0x00002477 File Offset: 0x00000677
	public static void SetEnemyChamsColor(Color color)
	{
		ConfigManager.EnemyChamsColorHex.Value = "#" + ColorUtility.ToHtmlStringRGB(color);
	}

	// Token: 0x06000005 RID: 5 RVA: 0x00002493 File Offset: 0x00000693
	public static void SetAllyChamsColor(Color color)
	{
		ConfigManager.AllyChamsColorHex.Value = "#" + ColorUtility.ToHtmlStringRGB(color);
	}

	// Token: 0x04000001 RID: 1
	public static ManualLogSource Logger;

	// Token: 0x04000002 RID: 2
	public static ConfigEntry<bool> ESPChams;

	// Token: 0x04000003 RID: 3
	public static ConfigEntry<string> EnemyChamsColorHex;

	// Token: 0x04000004 RID: 4
	public static ConfigEntry<string> AllyChamsColorHex;

	// Token: 0x04000005 RID: 5
	public static ConfigEntry<bool> GodMode;

	// Token: 0x04000006 RID: 6
	public static ConfigEntry<bool> InfiniteStam;

	// Token: 0x04000007 RID: 7
	public static ConfigEntry<bool> BoostedXP;

	// Token: 0x04000008 RID: 8
	public static ConfigEntry<bool> ForceLevel;

	// Token: 0x04000009 RID: 9
	public static ConfigEntry<bool> NoCooldowns;

	// Token: 0x0400000A RID: 10
	public static ConfigEntry<bool> SpeedMod;

	// Token: 0x0400000B RID: 11
	public static ConfigEntry<bool> JumpMod;

	// Token: 0x0400000C RID: 12
	public static ConfigEntry<bool> InstantDoors;

	// Token: 0x0400000D RID: 13
	public static ConfigEntry<bool> InstantRecall;

	// Token: 0x0400000E RID: 14
	public static ConfigEntry<bool> InstantRespawn;

	// Token: 0x0400000F RID: 15
	public static ConfigEntry<bool> DisableWind;

	// Token: 0x04000010 RID: 16
	public static ConfigEntry<bool> KeepItems;

	// Token: 0x04000011 RID: 17
	public static ConfigEntry<bool> InstantTrade;

	// Token: 0x04000012 RID: 18
	public static ConfigEntry<bool> TradePage;

	// Token: 0x04000013 RID: 19
	public static ConfigEntry<bool> NoClip;

	// Token: 0x04000014 RID: 20
	public static ConfigEntry<int> XPMultiplier;

	// Token: 0x04000015 RID: 21
	public static ConfigEntry<float> SpeedMultiplier;

	// Token: 0x04000016 RID: 22
	public static ConfigEntry<float> JumpMultiplier;

	// Token: 0x04000017 RID: 23
	public static ConfigEntry<int> ForcedLevelVal;

	// Token: 0x04000018 RID: 24
	public static ConfigEntry<float> FlySpeed;

	// Token: 0x04000019 RID: 25
	public static ConfigEntry<bool> FireballEdits;

	// Token: 0x0400001A RID: 26
	public static ConfigEntry<float> FireballVelocity;

	// Token: 0x0400001B RID: 27
	public static ConfigEntry<float> FireballGravity;

	// Token: 0x0400001C RID: 28
	public static ConfigEntry<float> BlinkMultiplier;
}
