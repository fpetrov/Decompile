using System;
using System.Reflection;
using MagusToolkit;
using UnityEngine;

// Token: 0x02000004 RID: 4
public class LevelForcer : MonoBehaviour
{
	// Token: 0x06000011 RID: 17 RVA: 0x00002B64 File Offset: 0x00000D64
	private void Start()
	{
		this.player = Globals.LocalPlayer;
		if (this.player != null)
		{
			this.levelField = typeof(PlayerMovement).GetField("level", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (this.levelField == null)
			{
				ConfigManager.Logger.LogWarning("PlayerMovement.level field not found.");
			}
		}
	}

	// Token: 0x06000012 RID: 18 RVA: 0x00002BC4 File Offset: 0x00000DC4
	private void Update()
	{
		if (this.player == null || this.levelField == null)
		{
			return;
		}
		if (ConfigManager.ForceLevel.Value)
		{
			this.levelField.SetValue(this.player, ConfigManager.ForcedLevelVal.Value);
		}
	}

	// Token: 0x04000020 RID: 32
	private PlayerMovement player;

	// Token: 0x04000021 RID: 33
	private FieldInfo levelField;
}
