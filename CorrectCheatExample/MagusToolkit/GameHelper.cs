using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MagusToolkit;
using UnityEngine;

// Token: 0x02000003 RID: 3
public static class GameHelper
{
	// Token: 0x06000006 RID: 6 RVA: 0x000024B0 File Offset: 0x000006B0
	public static bool IsInGame()
	{
		foreach (PlayerMovement playerMovement in Object.FindObjectsByType<PlayerMovement>(0))
		{
			if (playerMovement != null && playerMovement.enabled && playerMovement.gameObject.activeInHierarchy)
			{
				Camera componentInChildren = playerMovement.GetComponentInChildren<Camera>();
				if (componentInChildren != null && componentInChildren.enabled)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000007 RID: 7 RVA: 0x00002510 File Offset: 0x00000710
	public static string GetMySteamId()
	{
		MainMenuManager mainMenuManager = Object.FindFirstObjectByType<MainMenuManager>();
		if (mainMenuManager == null || mainMenuManager.kickplayershold == null)
		{
			return null;
		}
		MainMenuManagerNetworked mainMenuManagerNetworked = Object.FindFirstObjectByType<MainMenuManagerNetworked>();
		if (mainMenuManagerNetworked == null)
		{
			return null;
		}
		string text = (string)typeof(MainMenuManagerNetworked).GetField("localplayername", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(mainMenuManagerNetworked);
		string text2;
		if (mainMenuManager.kickplayershold.nametosteamid.TryGetValue(text, out text2))
		{
			return text2;
		}
		return null;
	}

	// Token: 0x06000008 RID: 8 RVA: 0x00002587 File Offset: 0x00000787
	public static IEnumerator DelayedApplyChams(float delaySeconds = 2f)
	{
		GameHelper.<DelayedApplyChams>d__3 <DelayedApplyChams>d__ = new GameHelper.<DelayedApplyChams>d__3(0);
		<DelayedApplyChams>d__.delaySeconds = delaySeconds;
		return <DelayedApplyChams>d__;
	}

	// Token: 0x06000009 RID: 9 RVA: 0x00002598 File Offset: 0x00000798
	public static void DumpAllFields(object obj)
	{
		if (obj == null)
		{
			ConfigManager.Logger.LogWarning("DumpAllFields: Object is null.");
			return;
		}
		Type type = obj.GetType();
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		ConfigManager.Logger.LogWarning("[MagusToolkit] Dumping fields for " + type.Name);
		FieldInfo[] array = fields;
		int i = 0;
		while (i < array.Length)
		{
			FieldInfo fieldInfo = array[i];
			object value;
			try
			{
				value = fieldInfo.GetValue(obj);
			}
			catch (Exception ex)
			{
				ConfigManager.Logger.LogWarning("Field: " + fieldInfo.Name + " threw exception: " + ex.Message);
				goto IL_00AF;
			}
			goto IL_007A;
			IL_00AF:
			i++;
			continue;
			IL_007A:
			string text = ((value != null) ? value.ToString() : "null");
			ConfigManager.Logger.LogWarning("Field: " + fieldInfo.Name + " = " + text);
			goto IL_00AF;
		}
	}

	// Token: 0x0600000A RID: 10 RVA: 0x00002670 File Offset: 0x00000870
	public static void InitChamsMaterials()
	{
		Shader shader = Shader.Find("Hidden/Internal-Colored");
		if (shader == null)
		{
			ConfigManager.Logger.LogWarning("[MagusToolkit] InitChamsMaterials: Hidden/Internal-Colored not found. Trying 'Standard' shader as fallback.");
			shader = Shader.Find("Standard");
		}
		if (shader == null)
		{
			ConfigManager.Logger.LogWarning("[MagusToolkit] InitChamsMaterials: No valid shader found for chams.");
			return;
		}
		GameHelper.enemyCham = new Material(shader);
		GameHelper.enemyCham.color = ConfigManager.GetEnemyChamsColor();
		GameHelper.enemyCham.SetInt("_ZTest", 8);
		GameHelper.allyCham = new Material(shader);
		GameHelper.allyCham.color = ConfigManager.GetAllyChamsColor();
		GameHelper.allyCham.SetInt("_ZTest", 8);
		ConfigManager.Logger.LogWarning("[MagusToolkit] InitChamsMaterials: Using shader '" + shader.name + "' for chams.");
	}

	// Token: 0x0600000B RID: 11 RVA: 0x00002738 File Offset: 0x00000938
	public static int GetPlayerTeam(PlayerMovement player)
	{
		int num;
		try
		{
			FieldInfo fieldInfo = AccessTools.Field(player.GetType(), "playerTeam");
			num = ((fieldInfo != null) ? ((int)fieldInfo.GetValue(player)) : (-1));
		}
		catch
		{
			num = -1;
		}
		return num;
	}

	// Token: 0x0600000C RID: 12 RVA: 0x00002788 File Offset: 0x00000988
	public static void ApplyChams()
	{
		if (GameHelper.enemyCham == null || GameHelper.allyCham == null)
		{
			ConfigManager.Logger.LogWarning("[MagusToolkit] ApplyChams: Materials not initialized. Calling InitChamsMaterials.");
			GameHelper.InitChamsMaterials();
			if (GameHelper.enemyCham == null || GameHelper.allyCham == null)
			{
				ConfigManager.Logger.LogWarning("[MagusToolkit] ApplyChams: Failed to initialize chams materials.");
				return;
			}
		}
		PlayerMovement localPlayer = Globals.LocalPlayer;
		if (localPlayer == null)
		{
			ConfigManager.Logger.LogWarning("[MagusToolkit] ApplyChams: LocalPlayer is null.");
			return;
		}
		int playerTeam = GameHelper.GetPlayerTeam(localPlayer);
		if (playerTeam == -1)
		{
			ConfigManager.Logger.LogWarning("[MagusToolkit] ApplyChams: Could not determine LocalPlayer team.");
			GameHelper.DumpAllFields(localPlayer);
			return;
		}
		PlayerMovement[] array = Object.FindObjectsByType<PlayerMovement>(0);
		ConfigManager.Logger.LogWarning(string.Format("[MagusToolkit] ApplyChams: Found {0} PlayerMovement objects.", array.Length));
		foreach (PlayerMovement playerMovement in array)
		{
			if (!(playerMovement == null) && !(playerMovement == localPlayer))
			{
				int playerTeam2 = GameHelper.GetPlayerTeam(playerMovement);
				if (playerTeam2 == -1)
				{
					ConfigManager.Logger.LogWarning("[MagusToolkit] ApplyChams: Could not determine team for " + playerMovement.name + ". Dumping fields...");
					GameHelper.DumpAllFields(playerMovement);
				}
				else
				{
					bool flag = playerTeam2 == playerTeam;
					ConfigManager.Logger.LogInfo(string.Format("[MagusToolkit] ApplyChams: Player: {0} | Team: {1} | Ally: {2}", playerMovement.playerNameText.text, playerTeam2, flag));
					Material material = (flag ? GameHelper.allyCham : GameHelper.enemyCham);
					Renderer[] componentsInChildren = playerMovement.GetComponentsInChildren<Renderer>(true);
					if (componentsInChildren.Length == 0)
					{
						ConfigManager.Logger.LogWarning("[MagusToolkit] ApplyChams: No renderers found on " + playerMovement.playerNameText.text + ".");
					}
					else
					{
						foreach (Renderer renderer in componentsInChildren)
						{
							if (renderer.enabled && renderer.gameObject.activeInHierarchy)
							{
								string text = renderer.gameObject.name.ToLower();
								if ((renderer is SkinnedMeshRenderer || text.Contains("body") || text.Contains("arm") || text.Contains("hand") || text.Contains("robe") || text.Contains("head") || text.Contains("legs")) && renderer.sharedMaterials.Length != 0)
								{
									if (!GameHelper.originalMaterials.ContainsKey(renderer.gameObject))
									{
										GameHelper.originalMaterials[renderer.gameObject] = renderer.materials;
									}
									if (!(renderer.sharedMaterials[0] == material))
									{
										Material[] array4 = new Material[renderer.sharedMaterials.Length];
										for (int k = 0; k < array4.Length; k++)
										{
											array4[k] = material;
										}
										renderer.materials = array4;
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600000D RID: 13 RVA: 0x00002A70 File Offset: 0x00000C70
	public static void RestoreOriginalMaterials()
	{
		foreach (KeyValuePair<GameObject, Material[]> keyValuePair in GameHelper.originalMaterials)
		{
			if (keyValuePair.Key != null)
			{
				Renderer component = keyValuePair.Key.GetComponent<Renderer>();
				if (component != null)
				{
					component.materials = keyValuePair.Value;
				}
			}
		}
		GameHelper.originalMaterials.Clear();
	}

	// Token: 0x0600000E RID: 14 RVA: 0x00002AF8 File Offset: 0x00000CF8
	public static void RefreshChamsColors()
	{
		if (GameHelper.enemyCham != null)
		{
			GameHelper.enemyCham.color = ConfigManager.GetEnemyChamsColor();
		}
		if (GameHelper.allyCham != null)
		{
			GameHelper.allyCham.color = ConfigManager.GetAllyChamsColor();
		}
	}

	// Token: 0x0600000F RID: 15 RVA: 0x00002B32 File Offset: 0x00000D32
	public static void StartLevelForcer()
	{
		if (Globals.LocalPlayer == null)
		{
			return;
		}
		GameObject gameObject = new GameObject("LevelForcer");
		Object.DontDestroyOnLoad(gameObject);
		gameObject.AddComponent<LevelForcer>();
	}

	// Token: 0x0400001D RID: 29
	private static readonly Dictionary<GameObject, Material[]> originalMaterials = new Dictionary<GameObject, Material[]>();

	// Token: 0x0400001E RID: 30
	private static Material enemyCham;

	// Token: 0x0400001F RID: 31
	private static Material allyCham;
}
