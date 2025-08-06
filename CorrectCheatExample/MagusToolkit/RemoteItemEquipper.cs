using System;
using System.Linq;
using FishNet.Object;
using MagusToolkit;
using UnityEngine;

// Token: 0x0200001B RID: 27
public class RemoteItemEquipper : NetworkBehaviour
{
	// Token: 0x06000042 RID: 66 RVA: 0x0000396C File Offset: 0x00001B6C
	[ServerRpc]
	public void Server_HijackPickup(int itemId)
	{
		MonoBehaviour monoBehaviour = Object.FindObjectsByType<MonoBehaviour>(0).FirstOrDefault(delegate(MonoBehaviour x)
		{
			bool flag;
			try
			{
				flag = ItemHelper.GetItemId(x) == itemId && x.GetComponent<NetworkObject>() != null;
			}
			catch
			{
				flag = false;
			}
			return flag;
		});
		if (monoBehaviour == null)
		{
			ConfigManager.Logger.LogWarning(string.Format("[HijackPickup] No scene item found with ID {0}", itemId));
			return;
		}
		monoBehaviour.transform.position = base.transform.position + Vector3.up * 0.5f;
		PlayerInventory component = base.GetComponent<PlayerInventory>();
		if (component == null)
		{
			ConfigManager.Logger.LogWarning("[HijackPickup] No PlayerInventory");
			return;
		}
		if (itemId >= 33 && itemId <= 38)
		{
			component.SpecialPickup(monoBehaviour.gameObject);
			return;
		}
		component.Pickup(monoBehaviour.gameObject);
	}
}
