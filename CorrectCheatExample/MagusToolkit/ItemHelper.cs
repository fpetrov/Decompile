using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MagusToolkit
{
	// Token: 0x0200001F RID: 31
	internal static class ItemHelper
	{
		// Token: 0x06000068 RID: 104 RVA: 0x00005AD4 File Offset: 0x00003CD4
		public static Dictionary<string, GameObject> GetCachedItems()
		{
			return ItemHelper.CachedItemObjects;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00005ADC File Offset: 0x00003CDC
		public static void LogAllSceneItemNames()
		{
			List<string> list = (from n in (from mb in Object.FindObjectsByType<MonoBehaviour>(0)
					where mb != null && mb.gameObject != null
					select mb.gameObject.name).Distinct<string>()
				orderby n
				select n).ToList<string>();
			ConfigManager.Logger.LogInfo(string.Format("[ItemHelper] Logging {0} unique GameObject names in scene:", list.Count));
			foreach (string text in list)
			{
				ConfigManager.Logger.LogInfo(" - " + text);
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00005BD4 File Offset: 0x00003DD4
		public static void RefreshItemSceneCache()
		{
			ItemHelper.CachedItemObjects.Clear();
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				if (sceneAt.isLoaded)
				{
					list.AddRange(sceneAt.GetRootGameObjects());
				}
			}
			foreach (GameObject gameObject in list)
			{
				Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>(true);
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					GameObject gameObject2 = componentsInChildren[j].gameObject;
					if (ItemHelper.KnownItemNames.Contains(gameObject2.name) && !ItemHelper.CachedItemObjects.ContainsKey(gameObject2.name))
					{
						ItemHelper.CachedItemObjects[gameObject2.name] = gameObject2;
						ConfigManager.Logger.LogInfo("[ItemCache] Found item in scene: " + gameObject2.name);
					}
				}
			}
			foreach (ChestNetController chestNetController in Object.FindObjectsByType<ChestNetController>(0))
			{
				if (chestNetController.Items != null)
				{
					foreach (GameObject gameObject3 in chestNetController.Items)
					{
						if (gameObject3 != null && !ItemHelper.CachedItemObjects.ContainsKey(gameObject3.name))
						{
							ItemHelper.CachedItemObjects[gameObject3.name] = gameObject3;
							ConfigManager.Logger.LogInfo("[ItemCache] Found chest item: " + gameObject3.name);
						}
					}
				}
			}
			foreach (CraftingForge craftingForge in Object.FindObjectsByType<CraftingForge>(0))
			{
				if (craftingForge.CraftablePrefabs != null)
				{
					foreach (GameObject gameObject4 in craftingForge.CraftablePrefabs)
					{
						if (gameObject4 != null && !ItemHelper.CachedItemObjects.ContainsKey(gameObject4.name))
						{
							ItemHelper.CachedItemObjects[gameObject4.name] = gameObject4;
							ConfigManager.Logger.LogInfo("[ItemCache] Found forge craftable: " + gameObject4.name);
						}
					}
				}
			}
			foreach (PageLootTable pageLootTable in Object.FindObjectsByType<PageLootTable>(0))
			{
				if (pageLootTable.Pages != null)
				{
					foreach (GameObject gameObject5 in pageLootTable.Pages)
					{
						if (gameObject5 != null && !ItemHelper.CachedItemObjects.ContainsKey(gameObject5.name))
						{
							ItemHelper.CachedItemObjects[gameObject5.name] = gameObject5;
							ConfigManager.Logger.LogInfo("[ItemCache] Found spell page: " + gameObject5.name);
						}
					}
				}
			}
			ItemSpawner[] array5 = Object.FindObjectsByType<ItemSpawner>(0);
			for (int j = 0; j < array5.Length; j++)
			{
				GameObject itemPrefab = array5[j].ItemPrefab;
				if (itemPrefab != null && !ItemHelper.CachedItemObjects.ContainsKey(itemPrefab.name))
				{
					ItemHelper.CachedItemObjects[itemPrefab.name] = itemPrefab;
					ConfigManager.Logger.LogInfo("[ItemCache] Found spawner item: " + itemPrefab.name);
				}
			}
			ConfigManager.Logger.LogInfo(string.Format("[ItemCache] Total unique cached items: {0}", ItemHelper.CachedItemObjects.Count));
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00005F4C File Offset: 0x0000414C
		public static int GetItemId(MonoBehaviour component)
		{
			if (component.GetType().GetInterface("IItemInteraction") != null)
			{
				MethodInfo method = component.GetType().GetMethod("GetItemID");
				if (method != null)
				{
					return (int)method.Invoke(component, null);
				}
			}
			FieldInfo field = component.GetType().GetField("id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field != null)
			{
				return (int)field.GetValue(component);
			}
			return -1;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00005FC4 File Offset: 0x000041C4
		public static void EquipItemByID(int itemId)
		{
			if (!Globals.LocalPlayer)
			{
				ConfigManager.Logger.LogWarning("[EquipItemByID] LocalPlayer is null.");
				return;
			}
			RemoteItemEquipper component = Globals.LocalPlayer.GetComponent<RemoteItemEquipper>();
			if (component == null)
			{
				ConfigManager.Logger.LogWarning("[EquipItemByID] RemoteItemEquipper not found on LocalPlayer.");
				return;
			}
			ConfigManager.Logger.LogInfo(string.Format("[EquipItemByID] Attempting hijack for item ID {0}.", itemId));
			component.Server_HijackPickup(itemId);
		}

		// Token: 0x04000058 RID: 88
		private static Dictionary<string, GameObject> CachedItemObjects = new Dictionary<string, GameObject>();

		// Token: 0x04000059 RID: 89
		private static readonly List<string> KnownItemNames = new List<string>
		{
			"MushroomWalkingStick(Clone)", "Log(Clone)", "Torch(Clone)", "Crystal(Clone)", "Frog(Clone)", "frog(Clone)", "Rock(Clone)", "RocK(Clone)", "PageBlink(Clone)", "PageHolyLight(Clone)",
			"PageThunderBolt(Clone)", "PageWisp(Clone)", "PageDarkBlast(Clone)", "PageRock(Clone)", "MageBook(Clone)", "BounceMush(Clone)", "WeedofThePipe(Clone)", "MushroomSword(Clone)", "FrogBlade(Clone)", "ExcaliberBlade(Clone)",
			"ExcaliberHilt(Clone)", "Excaliber(Clone)", "SporeFrog(Clone)", "FrogSpear(Clone)", "Frogballoon(Clone)", "SpikedRoots(Clone)", "StaffOfLevitation(Clone)", "ShrinkRay(Clone)", "WalkingStick(Clone)"
		};
	}
}
