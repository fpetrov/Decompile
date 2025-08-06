using System;
using System.Collections.Generic;
using System.Linq;
using MageArenaManager;
using MagusToolkit;
using UnityEngine;

// Token: 0x0200001C RID: 28
public class UIMenuIMGUI : MonoBehaviour
{
	// Token: 0x06000044 RID: 68 RVA: 0x00003A46 File Offset: 0x00001C46
	private void Start()
	{
		this.InitStyles();
	}

	// Token: 0x06000045 RID: 69 RVA: 0x00003A50 File Offset: 0x00001C50
	private void Update()
	{
		if (Input.GetKeyDown(282))
		{
			this.isVisible = !this.isVisible;
			if (this.isVisible)
			{
				Cursor.lockState = 0;
				Cursor.visible = true;
				return;
			}
			if (GameHelper.IsInGame())
			{
				Cursor.lockState = 1;
				Cursor.visible = false;
				return;
			}
			Cursor.lockState = 0;
			Cursor.visible = true;
		}
	}

	// Token: 0x06000046 RID: 70 RVA: 0x00003AB0 File Offset: 0x00001CB0
	private void OnGUI()
	{
		if (!this.isVisible)
		{
			return;
		}
		if (this.boxStyle == null)
		{
			this.InitStyles();
		}
		GUI.backgroundColor = this.bg;
		this.windowRect = GUI.Window(0, this.windowRect, new GUI.WindowFunction(this.DrawMenuWindow), GUIContent.none);
	}

	// Token: 0x06000047 RID: 71 RVA: 0x00003B04 File Offset: 0x00001D04
	private void DrawMenuWindow(int windowID)
	{
		GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
		GUI.backgroundColor = this.titleBar;
		GUILayout.Box("MagusToolkit v1.0.0.0", this.titleStyle, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		this.DrawTabColumn();
		this.DrawTabContent();
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUI.DragWindow(new Rect(0f, 0f, this.windowRect.width, 25f));
	}

	// Token: 0x06000048 RID: 72 RVA: 0x00003B8C File Offset: 0x00001D8C
	private void DrawTabColumn()
	{
		GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width((float)this.tabWidth) });
		this.DrawTabButton("PLAYER", UIMenuIMGUI.Tab.PLAYER);
		this.DrawTabButton("VISUALS", UIMenuIMGUI.Tab.VISUALS);
		this.DrawTabButton("LOBBY", UIMenuIMGUI.Tab.LOBBY);
		this.DrawTabButton("ITEMS", UIMenuIMGUI.Tab.ITEMS);
		this.DrawTabButton("SPELLS", UIMenuIMGUI.Tab.SPELLEDIT);
		this.DrawTabButton("ABOUT", UIMenuIMGUI.Tab.ABOUT);
		GUILayout.EndVertical();
	}

	// Token: 0x06000049 RID: 73 RVA: 0x00003C00 File Offset: 0x00001E00
	private void DrawTabButton(string label, UIMenuIMGUI.Tab tab)
	{
		GUIStyle guistyle = ((this.selectedTab == tab) ? this.tabSelectedStyle : this.tabStyle);
		if (GUILayout.Button(label, guistyle, Array.Empty<GUILayoutOption>()))
		{
			this.selectedTab = tab;
		}
	}

	// Token: 0x0600004A RID: 74 RVA: 0x00003C3C File Offset: 0x00001E3C
	private void DrawTabContent()
	{
		GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
		this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, new GUILayoutOption[] { GUILayout.Height(380f) });
		switch (this.selectedTab)
		{
		case UIMenuIMGUI.Tab.PLAYER:
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			string text = (this.showPlayerToggles ? "▼ " : "► ") + "Toggles";
			string text2 = (this.showPlayerSliders ? "▼ " : "► ") + "Modifiers";
			text = ((text.Length > 20) ? (text.Substring(0, 20) + "...") : text);
			text2 = ((text2.Length > 20) ? (text2.Substring(0, 20) + "...") : text2);
			float num = (600f - (float)this.tabWidth) / 2f - 20f;
			GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(num) });
			if (GUILayout.Button(text, this.buttonStyle, new GUILayoutOption[] { GUILayout.Width(num) }))
			{
				this.showPlayerToggles = !this.showPlayerToggles;
			}
			if (this.showPlayerToggles)
			{
				this.DrawToggle("Godmode", () => ConfigManager.GodMode.Value, delegate(bool v)
				{
					ConfigManager.GodMode.Value = v;
				});
				this.DrawToggle("Infinite Stamina", () => ConfigManager.InfiniteStam.Value, delegate(bool v)
				{
					ConfigManager.InfiniteStam.Value = v;
				});
				this.DrawToggle("No Cooldowns", () => ConfigManager.NoCooldowns.Value, delegate(bool v)
				{
					ConfigManager.NoCooldowns.Value = v;
				});
				this.DrawToggle("Instant Doors", () => ConfigManager.InstantDoors.Value, delegate(bool v)
				{
					ConfigManager.InstantDoors.Value = v;
				});
				this.DrawToggle("Instant Recall", () => ConfigManager.InstantRecall.Value, delegate(bool v)
				{
					ConfigManager.InstantRecall.Value = v;
				});
				this.DrawToggle("Instant Respawn", () => ConfigManager.InstantRespawn.Value, delegate(bool v)
				{
					ConfigManager.InstantRespawn.Value = v;
				});
				this.DrawToggle("Disable Wind", () => ConfigManager.DisableWind.Value, delegate(bool v)
				{
					ConfigManager.DisableWind.Value = v;
				});
				this.DrawToggle("Keep Inventory", () => ConfigManager.KeepItems.Value, delegate(bool v)
				{
					ConfigManager.KeepItems.Value = v;
				});
				this.DrawToggle("Instant Trades", () => ConfigManager.InstantTrade.Value, delegate(bool v)
				{
					ConfigManager.InstantTrade.Value = v;
				});
				this.DrawToggle("Page Trades", () => ConfigManager.TradePage.Value, delegate(bool v)
				{
					ConfigManager.TradePage.Value = v;
				});
				this.DrawToggle("Boosted XP", () => ConfigManager.BoostedXP.Value, delegate(bool v)
				{
					ConfigManager.BoostedXP.Value = v;
				});
				this.DrawToggle("Force Level", () => ConfigManager.ForceLevel.Value, delegate(bool v)
				{
					ConfigManager.ForceLevel.Value = v;
				});
				this.DrawToggle("Speed Mod", () => ConfigManager.SpeedMod.Value, delegate(bool v)
				{
					ConfigManager.SpeedMod.Value = v;
				});
				this.DrawToggle("Jump Mod", () => ConfigManager.JumpMod.Value, delegate(bool v)
				{
					ConfigManager.JumpMod.Value = v;
				});
				this.DrawToggle("No Clip", () => ConfigManager.NoClip.Value, delegate(bool v)
				{
					ConfigManager.NoClip.Value = v;
					if (GameHelper.IsInGame())
					{
						PlayerMovement localPlayer = Globals.LocalPlayer;
						if (localPlayer != null)
						{
							if (v)
							{
								if (localPlayer.GetComponent<NoClip>() == null)
								{
									localPlayer.gameObject.AddComponent<NoClip>();
									return;
								}
							}
							else
							{
								NoClip component = localPlayer.GetComponent<NoClip>();
								if (component != null)
								{
									Object.Destroy(component);
								}
							}
						}
					}
				});
			}
			GUILayout.EndVertical();
			GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(num) });
			if (GUILayout.Button(text2, this.buttonStyle, new GUILayoutOption[] { GUILayout.Width(num) }))
			{
				this.showPlayerSliders = !this.showPlayerSliders;
			}
			if (this.showPlayerSliders)
			{
				if (ConfigManager.BoostedXP.Value)
				{
					this.DrawSimpleSlider("XP Multiplier", (float)ConfigManager.XPMultiplier.Value, delegate(float v)
					{
						ConfigManager.XPMultiplier.Value = (int)v;
					}, 1f, 10f, true);
				}
				if (ConfigManager.ForceLevel.Value)
				{
					this.DrawSimpleSlider("Level", (float)ConfigManager.ForcedLevelVal.Value, delegate(float v)
					{
						ConfigManager.ForcedLevelVal.Value = (int)v;
					}, 1f, 100f, true);
				}
				if (ConfigManager.SpeedMod.Value)
				{
					this.DrawSimpleSlider("Speed Multiplier", ConfigManager.SpeedMultiplier.Value, delegate(float v)
					{
						ConfigManager.SpeedMultiplier.Value = v;
					}, 1f, 10f, false);
				}
				if (ConfigManager.JumpMod.Value)
				{
					this.DrawSimpleSlider("Jump Multiplier", ConfigManager.JumpMultiplier.Value, delegate(float v)
					{
						ConfigManager.JumpMultiplier.Value = v;
					}, 1f, 10f, false);
				}
				if (ConfigManager.NoClip.Value)
				{
					this.DrawSimpleSlider("Fly Speed", ConfigManager.FlySpeed.Value, delegate(float v)
					{
						ConfigManager.FlySpeed.Value = v;
					}, 1f, 100f, false);
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			break;
		}
		case UIMenuIMGUI.Tab.VISUALS:
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			string text3 = (this.showVisualToggles ? "▼ " : "► ") + "Toggles";
			string text4 = (this.showVisualColors ? "▼ " : "► ") + "Colors";
			text3 = ((text3.Length > 20) ? (text3.Substring(0, 20) + "...") : text3);
			text4 = ((text4.Length > 20) ? (text4.Substring(0, 20) + "...") : text4);
			float num2 = (600f - (float)this.tabWidth) / 2f - 20f;
			GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(num2) });
			if (GUILayout.Button(text3, this.buttonStyle, new GUILayoutOption[] { GUILayout.Width(num2) }))
			{
				this.showVisualToggles = !this.showVisualToggles;
			}
			if (this.showVisualToggles)
			{
				this.DrawToggle("Chams", () => ConfigManager.ESPChams.Value, delegate(bool v)
				{
					ConfigManager.ESPChams.Value = v;
					if (v)
					{
						GameHelper.ApplyChams();
						return;
					}
					GameHelper.RestoreOriginalMaterials();
				});
			}
			GUILayout.EndVertical();
			GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(num2) });
			if (GUILayout.Button(text4, this.buttonStyle, new GUILayoutOption[] { GUILayout.Width(num2) }))
			{
				this.showVisualColors = !this.showVisualColors;
			}
			if (this.showVisualColors)
			{
				Color enemyChamsColor = ConfigManager.GetEnemyChamsColor();
				Color allyChamsColor = ConfigManager.GetAllyChamsColor();
				GUILayout.Label("Enemy Color", new GUILayoutOption[] { GUILayout.Width(150f) });
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				enemyChamsColor.r = GUILayout.HorizontalSlider(enemyChamsColor.r, 0f, 1f, Array.Empty<GUILayoutOption>());
				GUILayout.Label("R", new GUILayoutOption[] { GUILayout.Width(20f) });
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				enemyChamsColor.g = GUILayout.HorizontalSlider(enemyChamsColor.g, 0f, 1f, Array.Empty<GUILayoutOption>());
				GUILayout.Label("G", new GUILayoutOption[] { GUILayout.Width(20f) });
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				enemyChamsColor.b = GUILayout.HorizontalSlider(enemyChamsColor.b, 0f, 1f, Array.Empty<GUILayoutOption>());
				GUILayout.Label("B", new GUILayoutOption[] { GUILayout.Width(20f) });
				GUILayout.EndHorizontal();
				ConfigManager.SetEnemyChamsColor(enemyChamsColor);
				GameHelper.RefreshChamsColors();
				GUILayout.Space(10f);
				GUILayout.Label("Ally Color", new GUILayoutOption[] { GUILayout.Width(150f) });
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				allyChamsColor.r = GUILayout.HorizontalSlider(allyChamsColor.r, 0f, 1f, Array.Empty<GUILayoutOption>());
				GUILayout.Label("R", new GUILayoutOption[] { GUILayout.Width(20f) });
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				allyChamsColor.g = GUILayout.HorizontalSlider(allyChamsColor.g, 0f, 1f, Array.Empty<GUILayoutOption>());
				GUILayout.Label("G", new GUILayoutOption[] { GUILayout.Width(20f) });
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				allyChamsColor.b = GUILayout.HorizontalSlider(allyChamsColor.b, 0f, 1f, Array.Empty<GUILayoutOption>());
				GUILayout.Label("B", new GUILayoutOption[] { GUILayout.Width(20f) });
				GUILayout.EndHorizontal();
				ConfigManager.SetAllyChamsColor(allyChamsColor);
				GameHelper.RefreshChamsColors();
				GUILayout.Space(10f);
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
				GUILayout.Box(new GUIContent(""), new GUIStyle
				{
					normal = 
					{
						background = UIMenuIMGUI.TextureFromColor(enemyChamsColor)
					}
				}, new GUILayoutOption[]
				{
					GUILayout.Width(20f),
					GUILayout.Height(20f)
				});
				GUILayout.Label("Enemy", new GUILayoutOption[] { GUILayout.Width(60f) });
				GUILayout.EndVertical();
				GUILayout.Space(10f);
				GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
				GUILayout.Box(new GUIContent(""), new GUIStyle
				{
					normal = 
					{
						background = UIMenuIMGUI.TextureFromColor(allyChamsColor)
					}
				}, new GUILayoutOption[]
				{
					GUILayout.Width(20f),
					GUILayout.Height(20f)
				});
				GUILayout.Label("Ally", new GUILayoutOption[] { GUILayout.Width(60f) });
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
				GUILayout.Space(5f);
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			break;
		}
		case UIMenuIMGUI.Tab.LOBBY:
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			string text5 = (this.showLobbyPlayers ? "▼ " : "► ") + "Players";
			string text6 = (this.showPlayerOptions ? "▼ " : "► ") + "Options";
			string text = ((text5.Length > 20) ? (text5.Substring(0, 20) + "...") : text5);
			string text2 = ((text6.Length > 20) ? (text6.Substring(0, 20) + "...") : text6);
			float num3 = (600f - (float)this.tabWidth) / 2f - 20f;
			GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(num3) });
			if (GUILayout.Button(text, this.buttonStyle, new GUILayoutOption[] { GUILayout.Width(num3) }))
			{
				this.showLobbyPlayers = !this.showLobbyPlayers;
			}
			if (this.showLobbyPlayers)
			{
				if (GUILayout.Button("Refresh Players", new GUILayoutOption[] { GUILayout.Height(25f) }))
				{
					UIMenuIMGUI.lobbyPlayerList = Object.FindObjectsByType<PlayerMovement>(0).ToList<PlayerMovement>();
					ConfigManager.Logger.LogInfo(string.Format("[MagusToolkit] Refreshed lobby player list, found {0} players.", UIMenuIMGUI.lobbyPlayerList.Count));
				}
				GUILayout.Space(5f);
				UIMenuIMGUI.scrollPos_LobbyPlayers = GUILayout.BeginScrollView(UIMenuIMGUI.scrollPos_LobbyPlayers, new GUILayoutOption[] { GUILayout.Height(300f) });
				if (UIMenuIMGUI.lobbyPlayerList != null && UIMenuIMGUI.lobbyPlayerList.Count > 0)
				{
					using (List<PlayerMovement>.Enumerator enumerator = UIMenuIMGUI.lobbyPlayerList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							PlayerMovement playerMovement = enumerator.Current;
							if (!(playerMovement == null))
							{
								string text7 = playerMovement.playerNameText.text;
								GUIStyle guistyle = ((UIMenuIMGUI.selectedLobbyPlayer == playerMovement) ? this.selectedButtonStyle : this.buttonStyle);
								if (GUILayout.Button(text7, guistyle, new GUILayoutOption[] { GUILayout.Height(30f) }))
								{
									UIMenuIMGUI.selectedLobbyPlayer = playerMovement;
								}
							}
						}
						goto IL_0E96;
					}
				}
				GUILayout.Label("No players found.", new GUILayoutOption[] { GUILayout.Height(25f) });
				IL_0E96:
				GUILayout.EndScrollView();
			}
			GUILayout.EndVertical();
			GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(num3) });
			if (GUILayout.Button(text2, this.buttonStyle, new GUILayoutOption[] { GUILayout.Width(num3) }))
			{
				this.showPlayerOptions = !this.showPlayerOptions;
			}
			if (this.showPlayerOptions)
			{
				if (UIMenuIMGUI.selectedLobbyPlayer != null)
				{
					GUILayout.Label("Player Name: " + UIMenuIMGUI.selectedLobbyPlayer.playerNameText.text, new GUILayoutOption[] { GUILayout.Height(25f) });
					GUILayout.Space(5f);
				}
				else
				{
					GUILayout.Label("Select a player from the list.", new GUILayoutOption[] { GUILayout.Height(25f) });
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			break;
		}
		case UIMenuIMGUI.Tab.ITEMS:
			GUILayout.Label("Equip Scene Items - Buggy", new GUILayoutOption[] { GUILayout.Height(25f) });
			if (GUILayout.Button("Log All Scene GameObject Names", this.buttonStyle, Array.Empty<GUILayoutOption>()))
			{
				ItemHelper.LogAllSceneItemNames();
			}
			if (GUILayout.Button("Refresh Item Cache", new GUILayoutOption[] { GUILayout.Height(30f) }))
			{
				ItemHelper.RefreshItemSceneCache();
			}
			GUILayout.Space(10f);
			this.scrollPos_Items = GUILayout.BeginScrollView(this.scrollPos_Items, new GUILayoutOption[] { GUILayout.Height(250f) });
			foreach (KeyValuePair<string, GameObject> keyValuePair in ItemHelper.GetCachedItems())
			{
				GameObject value = keyValuePair.Value;
				string text8 = keyValuePair.Key.Replace("(Clone)", "").Trim();
				int num4 = -1;
				foreach (MonoBehaviour monoBehaviour in value.GetComponents<MonoBehaviour>())
				{
					try
					{
						num4 = ItemHelper.GetItemId(monoBehaviour);
						if (num4 != -1)
						{
							break;
						}
					}
					catch
					{
					}
				}
				if (GUILayout.Button(string.Format("Equip {0} (ID {1})", text8, num4), new GUILayoutOption[] { GUILayout.Height(30f) }))
				{
					ConfigManager.Logger.LogInfo(string.Format("[UI] Attempting to equip item '{0}' with ID {1}", text8, num4));
					ItemHelper.EquipItemByID(num4);
				}
			}
			GUILayout.EndScrollView();
			break;
		case UIMenuIMGUI.Tab.SPELLEDIT:
		{
			GUILayout.Label("Spell Editor", this.labelStyle, Array.Empty<GUILayoutOption>());
			string[] array = new string[] { "Fireball", "Freeze", "Magic Missile", "Worm/Hole", "Thunderbolt", "Dark Blast", "Devine Light", "Blink", "Wisp", "Rock" };
			float num5 = 600f - (float)this.tabWidth;
			this.selectedSpellTab = GUILayout.SelectionGrid(this.selectedSpellTab, array, 4, new GUILayoutOption[]
			{
				GUILayout.Height(80f),
				GUILayout.Width(num5 - 50f)
			});
			GUILayout.Space(10f);
			int i = this.selectedSpellTab;
			switch (i)
			{
			case 0:
				this.DrawFireballEditor();
				break;
			case 1:
				this.DrawFreezeEditor();
				break;
			case 2:
				this.DrawMagicMissileEditor();
				break;
			case 3:
				this.DrawWormholeEditor();
				break;
			case 4:
				this.DrawThunderboltEditor();
				break;
			case 5:
				this.DrawDarkBlastEditor();
				break;
			case 6:
				this.DrawDevineLightEditor();
				break;
			case 7:
				this.DrawBlinkEditor();
				break;
			case 8:
				this.DrawWispEditor();
				break;
			case 9:
				this.DrawRockEditor();
				break;
			}
			break;
		}
		case UIMenuIMGUI.Tab.ABOUT:
			GUILayout.Label("MagusToolkit v1.0.0.0", this.boldLabelStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Space(5f);
			GUILayout.Label("Created by OniGremlin", this.labelStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Space(10f);
			GUILayout.Label("MagusToolkit is a modular toolkit for modifying gameplay in Mage Arena.", this.labelStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Label("Features include:", this.labelStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Label("• Player stat modifiers (Godmode, Speed, Jump, XP, etc.)", this.labelStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Label("• Visual enhancements (Chams, Custom Colors)", this.labelStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Label("• Item and Spell editors", this.labelStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Space(10f);
			GUILayout.Label("This mod was created for experimentation, quality of life improvements,", this.labelStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Label("and learning Unity + FishNet modding.", this.labelStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Space(10f);
			GUILayout.Label("Not affiliated with the Mage Arena developers.", this.labelStyle, Array.Empty<GUILayoutOption>());
			break;
		}
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
	}

	// Token: 0x0600004B RID: 75 RVA: 0x00004FB8 File Offset: 0x000031B8
	private void InitStyles()
	{
		this.boxStyle = new GUIStyle(GUI.skin.box);
		this.titleStyle = new GUIStyle(GUI.skin.box)
		{
			fontSize = 14,
			fontStyle = 1,
			alignment = 4,
			normal = 
			{
				textColor = this.textColor,
				background = UIMenuIMGUI.TextureFromColor(this.titleBar)
			}
		};
		this.tabStyle = new GUIStyle(GUI.skin.button)
		{
			normal = 
			{
				textColor = this.textColor,
				background = UIMenuIMGUI.TextureFromColor(this.tabBG)
			},
			hover = 
			{
				background = UIMenuIMGUI.TextureFromColor(this.tabHover)
			}
		};
		this.tabSelectedStyle = new GUIStyle(this.tabStyle)
		{
			normal = 
			{
				background = UIMenuIMGUI.TextureFromColor(this.tabSelected)
			}
		};
		this.toggleStyle = new GUIStyle(GUI.skin.toggle)
		{
			normal = 
			{
				textColor = this.textColor
			},
			onNormal = 
			{
				textColor = this.textColor
			}
		};
		this.labelStyle = new GUIStyle(GUI.skin.label)
		{
			normal = 
			{
				textColor = this.textColor
			}
		};
		this.boldLabelStyle = new GUIStyle(GUI.skin.label)
		{
			fontStyle = 1,
			normal = 
			{
				textColor = this.textColor
			}
		};
		this.buttonStyle = new GUIStyle(GUI.skin.button)
		{
			normal = 
			{
				textColor = this.textColor
			}
		};
		this.selectedButtonStyle = new GUIStyle(this.buttonStyle)
		{
			normal = 
			{
				textColor = Color.white,
				background = this.buttonStyle.normal.background
			},
			fontStyle = 1
		};
	}

	// Token: 0x0600004C RID: 76 RVA: 0x00005198 File Offset: 0x00003398
	private static Color Hex(string hex)
	{
		Color color;
		ColorUtility.TryParseHtmlString("#" + hex, ref color);
		return color;
	}

	// Token: 0x0600004D RID: 77 RVA: 0x000051BC File Offset: 0x000033BC
	private void DrawColorBox(Color color, string label)
	{
		GUIStyle guistyle = new GUIStyle(GUI.skin.box);
		guistyle.normal.background = UIMenuIMGUI.TextureFromColor(color);
		GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
		GUILayout.Box(GUIContent.none, guistyle, new GUILayoutOption[]
		{
			GUILayout.Width(20f),
			GUILayout.Height(20f)
		});
		GUILayout.Label(label, new GUILayoutOption[] { GUILayout.Width(40f) });
		GUILayout.EndVertical();
	}

	// Token: 0x0600004E RID: 78 RVA: 0x0000523D File Offset: 0x0000343D
	private static Texture2D TextureFromColor(Color c)
	{
		Texture2D texture2D = new Texture2D(1, 1);
		texture2D.SetPixel(0, 0, c);
		texture2D.Apply();
		return texture2D;
	}

	// Token: 0x0600004F RID: 79 RVA: 0x00005258 File Offset: 0x00003458
	private void DrawFireballEditor()
	{
		string text = (this.showFireballSliders ? "▼ " : "► ") + "Fireball Settings";
		text = ((text.Length > 20) ? (text.Substring(0, 20) + "...") : text);
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		float num = 600f - (float)this.tabWidth;
		GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(num - 20f) });
		if (GUILayout.Button(text, this.buttonStyle, new GUILayoutOption[] { GUILayout.Width(num - 20f) }))
		{
			this.showFireballSliders = !this.showFireballSliders;
		}
		if (this.showFireballSliders)
		{
			this.DrawToggle("Enabled", () => ConfigManager.FireballEdits.Value, delegate(bool v)
			{
				ConfigManager.FireballEdits.Value = v;
			});
			this.DrawSimpleSlider("Muzzle Velocity", ConfigManager.FireballVelocity.Value, delegate(float v)
			{
				ConfigManager.FireballVelocity.Value = v;
			}, 0f, 1000f, false);
			this.DrawSimpleSlider("Gravity", ConfigManager.FireballGravity.Value, delegate(float v)
			{
				ConfigManager.FireballGravity.Value = v;
			}, 0f, 100f, false);
		}
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}

	// Token: 0x06000050 RID: 80 RVA: 0x000053E5 File Offset: 0x000035E5
	private void DrawFreezeEditor()
	{
		GUILayout.Label("Freeze Settings", this.boldLabelStyle, Array.Empty<GUILayoutOption>());
	}

	// Token: 0x06000051 RID: 81 RVA: 0x000053FC File Offset: 0x000035FC
	private void DrawMagicMissileEditor()
	{
		GUILayout.Label("Magic Missle Settings", this.boldLabelStyle, Array.Empty<GUILayoutOption>());
	}

	// Token: 0x06000052 RID: 82 RVA: 0x00005413 File Offset: 0x00003613
	private void DrawWormholeEditor()
	{
		GUILayout.Label("Worm / Hole Settings", this.boldLabelStyle, Array.Empty<GUILayoutOption>());
	}

	// Token: 0x06000053 RID: 83 RVA: 0x0000542A File Offset: 0x0000362A
	private void DrawThunderboltEditor()
	{
		GUILayout.Label("Thunderbolt Settings", this.boldLabelStyle, Array.Empty<GUILayoutOption>());
	}

	// Token: 0x06000054 RID: 84 RVA: 0x00005441 File Offset: 0x00003641
	private void DrawDarkBlastEditor()
	{
		GUILayout.Label("Dark Blast Settings", this.boldLabelStyle, Array.Empty<GUILayoutOption>());
	}

	// Token: 0x06000055 RID: 85 RVA: 0x00005458 File Offset: 0x00003658
	private void DrawDevineLightEditor()
	{
		GUILayout.Label("Devine Light Settings", this.boldLabelStyle, Array.Empty<GUILayoutOption>());
	}

	// Token: 0x06000056 RID: 86 RVA: 0x00005470 File Offset: 0x00003670
	private void DrawBlinkEditor()
	{
		this.DrawSimpleSlider("Blink Multiplier", ConfigManager.BlinkMultiplier.Value, delegate(float v)
		{
			ConfigManager.BlinkMultiplier.Value = v;
		}, 0f, 10f, false);
	}

	// Token: 0x06000057 RID: 87 RVA: 0x000054BC File Offset: 0x000036BC
	private void DrawWispEditor()
	{
		GUILayout.Label("Wisp Settings", this.boldLabelStyle, Array.Empty<GUILayoutOption>());
	}

	// Token: 0x06000058 RID: 88 RVA: 0x000054D3 File Offset: 0x000036D3
	private void DrawRockEditor()
	{
		GUILayout.Label("Rock Settings", this.boldLabelStyle, Array.Empty<GUILayoutOption>());
	}

	// Token: 0x06000059 RID: 89 RVA: 0x000054EC File Offset: 0x000036EC
	private void DrawSimpleSlider(string label, float value, Action<float> setter, float min = 1f, float max = 10f, bool isInt = false)
	{
		GUILayout.Label(label, this.labelStyle, Array.Empty<GUILayoutOption>());
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		float num = 150f;
		float num2 = 50f;
		float num3 = GUILayout.HorizontalSlider(value, min, max, new GUILayoutOption[] { GUILayout.Width(num) });
		GUILayout.Space(5f);
		GUILayout.Label(isInt ? ((int)num3).ToString() : num3.ToString("F1"), this.labelStyle, new GUILayoutOption[] { GUILayout.Width(num2) });
		GUILayout.EndHorizontal();
		setter(isInt ? ((float)Mathf.RoundToInt(num3)) : num3);
	}

	// Token: 0x0600005A RID: 90 RVA: 0x00005594 File Offset: 0x00003794
	private void DrawToggle(string label, Func<bool> getter, Action<bool> setter)
	{
		bool flag = getter();
		bool flag2 = GUILayout.Toggle(flag, label, this.toggleStyle, Array.Empty<GUILayoutOption>());
		if (flag2 != flag)
		{
			setter(flag2);
		}
	}

	// Token: 0x0600005B RID: 91 RVA: 0x000055C8 File Offset: 0x000037C8
	private void DrawSliderWithToggle(string label, Func<bool> getT, Action<bool> setT, Func<float> getS, Action<float> setS, bool isInt = false)
	{
		bool flag = getT();
		flag = GUILayout.Toggle(flag, label, this.toggleStyle, Array.Empty<GUILayoutOption>());
		setT(flag);
		if (flag)
		{
			float num = GUILayout.HorizontalSlider(getS(), 1f, 10f, Array.Empty<GUILayoutOption>());
			setS(isInt ? ((float)Mathf.RoundToInt(num)) : num);
			GUILayout.Label(isInt ? ((int)num).ToString() : num.ToString("F1"), this.labelStyle, Array.Empty<GUILayoutOption>());
		}
	}

	// Token: 0x04000030 RID: 48
	private Rect windowRect = new Rect(50f, 220f, 600f, 410f);

	// Token: 0x04000031 RID: 49
	private UIMenuIMGUI.Tab selectedTab;

	// Token: 0x04000032 RID: 50
	private bool isVisible;

	// Token: 0x04000033 RID: 51
	private Vector2 scrollPos;

	// Token: 0x04000034 RID: 52
	private Vector2 scrollPos_Items;

	// Token: 0x04000035 RID: 53
	private int tabWidth = 120;

	// Token: 0x04000036 RID: 54
	private static List<PlayerMovement> lobbyPlayerList = new List<PlayerMovement>();

	// Token: 0x04000037 RID: 55
	private static PlayerMovement selectedLobbyPlayer = null;

	// Token: 0x04000038 RID: 56
	private static Vector2 scrollPos_LobbyPlayers = Vector2.zero;

	// Token: 0x04000039 RID: 57
	private int selectedSpellTab;

	// Token: 0x0400003A RID: 58
	private readonly Color bg = UIMenuIMGUI.Hex("0d1b2a");

	// Token: 0x0400003B RID: 59
	private readonly Color titleBar = UIMenuIMGUI.Hex("1b263b");

	// Token: 0x0400003C RID: 60
	private readonly Color tabBG = UIMenuIMGUI.Hex("0d1b2a");

	// Token: 0x0400003D RID: 61
	private readonly Color tabHover = UIMenuIMGUI.Hex("415a77");

	// Token: 0x0400003E RID: 62
	private readonly Color tabSelected = UIMenuIMGUI.Hex("778da9");

	// Token: 0x0400003F RID: 63
	private readonly Color textColor = UIMenuIMGUI.Hex("e0e1dd");

	// Token: 0x04000040 RID: 64
	private readonly Color checkedColor = UIMenuIMGUI.Hex("2f6690");

	// Token: 0x04000041 RID: 65
	private GUIStyle boxStyle;

	// Token: 0x04000042 RID: 66
	private GUIStyle titleStyle;

	// Token: 0x04000043 RID: 67
	private GUIStyle tabStyle;

	// Token: 0x04000044 RID: 68
	private GUIStyle tabSelectedStyle;

	// Token: 0x04000045 RID: 69
	private GUIStyle toggleStyle;

	// Token: 0x04000046 RID: 70
	private GUIStyle labelStyle;

	// Token: 0x04000047 RID: 71
	private GUIStyle boldLabelStyle;

	// Token: 0x04000048 RID: 72
	private GUIStyle sliderStyle;

	// Token: 0x04000049 RID: 73
	private GUIStyle buttonStyle;

	// Token: 0x0400004A RID: 74
	private GUIStyle selectedButtonStyle;

	// Token: 0x0400004B RID: 75
	private bool showPlayerToggles = true;

	// Token: 0x0400004C RID: 76
	private bool showPlayerSliders = true;

	// Token: 0x0400004D RID: 77
	private bool showLobbyPlayers = true;

	// Token: 0x0400004E RID: 78
	private bool showPlayerOptions = true;

	// Token: 0x0400004F RID: 79
	private bool showVisualToggles = true;

	// Token: 0x04000050 RID: 80
	private bool showVisualColors = true;

	// Token: 0x04000051 RID: 81
	private bool showFireballSliders = true;

	// Token: 0x04000052 RID: 82
	private bool showExplosionSliders = true;

	// Token: 0x02000025 RID: 37
	private enum Tab
	{
		// Token: 0x04000068 RID: 104
		PLAYER,
		// Token: 0x04000069 RID: 105
		VISUALS,
		// Token: 0x0400006A RID: 106
		LOBBY,
		// Token: 0x0400006B RID: 107
		ITEMS,
		// Token: 0x0400006C RID: 108
		SPELLEDIT,
		// Token: 0x0400006D RID: 109
		ABOUT
	}
}
