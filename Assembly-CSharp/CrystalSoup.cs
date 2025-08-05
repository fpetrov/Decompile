using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x02000042 RID: 66
public class CrystalSoup : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x060002E4 RID: 740 RVA: 0x0000CAA9 File Offset: 0x0000ACA9
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x060002E5 RID: 741 RVA: 0x0000CADB File Offset: 0x0000ACDB
	public void Interaction(GameObject player)
	{
		if (!this.stewdrank)
		{
			this.stewdrank = true;
			player.GetComponent<PlayerMovement>().DrinkStew(this.stewid);
			player.GetComponent<PlayerInventory>().SwapItemImg(player.GetComponent<PlayerInventory>().equippedIndex, 32);
			this.DrinkStewServer();
		}
	}

	// Token: 0x060002E6 RID: 742 RVA: 0x0000CB1B File Offset: 0x0000AD1B
	[ServerRpc(RequireOwnership = false)]
	private void DrinkStewServer()
	{
		this.RpcWriter___Server_DrinkStewServer_2166136261();
	}

	// Token: 0x060002E7 RID: 743 RVA: 0x0000CB24 File Offset: 0x0000AD24
	[ObserversRpc]
	private void DrinkStewObservers()
	{
		this.RpcWriter___Observers_DrinkStewObservers_2166136261();
	}

	// Token: 0x060002E8 RID: 744 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x0000CB38 File Offset: 0x0000AD38
	public void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(298f, 37f, 146f);
			base.transform.position = raycastHit.point;
		}
		this.asource.PlayOneShot(this.aclips[1]);
		if (this.stewdrank)
		{
			base.StartCoroutine(this.destroysoup());
		}
		this.rockrender.SetActive(true);
	}

	// Token: 0x060002EA RID: 746 RVA: 0x0000CBDA File Offset: 0x0000ADDA
	private IEnumerator destroysoup()
	{
		yield return new WaitForSeconds(0.25f);
		this.serverdestroysoup();
		yield break;
	}

	// Token: 0x060002EB RID: 747 RVA: 0x0000CBEC File Offset: 0x0000ADEC
	[ServerRpc(RequireOwnership = false)]
	private void serverdestroysoup()
	{
		this.RpcWriter___Server_serverdestroysoup_2166136261();
	}

	// Token: 0x060002EC RID: 748 RVA: 0x0000CBFF File Offset: 0x0000ADFF
	public void PlayDropSound()
	{
		this.asource.PlayOneShot(this.aclips[1]);
	}

	// Token: 0x060002ED RID: 749 RVA: 0x0000CC14 File Offset: 0x0000AE14
	public void ItemInit()
	{
		this.rockrender.SetActive(true);
		this.asource.PlayOneShot(this.aclips[0]);
	}

	// Token: 0x060002EE RID: 750 RVA: 0x0000CC14 File Offset: 0x0000AE14
	public void ItemInitObs()
	{
		this.rockrender.SetActive(true);
		this.asource.PlayOneShot(this.aclips[0]);
	}

	// Token: 0x060002EF RID: 751 RVA: 0x0000CC35 File Offset: 0x0000AE35
	public void HideItem()
	{
		this.rockrender.SetActive(false);
	}

	// Token: 0x060002F0 RID: 752 RVA: 0x0000CC43 File Offset: 0x0000AE43
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
		if (this.stewdrank)
		{
			player.GetComponent<PlayerInventory>().SwapItemImg(player.GetComponent<PlayerInventory>().equippedIndex, 32);
		}
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x0000CC78 File Offset: 0x0000AE78
	public string DisplayInteractUI(GameObject player)
	{
		if (this.stewdrank)
		{
			return "Grasp Bowl";
		}
		if (this.stewid == 0)
		{
			return "Grasp Crystal Soup";
		}
		if (this.stewid == 1)
		{
			return "Grasp Log Soup";
		}
		if (this.stewid == 2)
		{
			return "Grasp Mushroom Soup";
		}
		if (this.stewid == 3)
		{
			return "Grasp Frog Soup";
		}
		if (this.stewid == 4)
		{
			return "Grasp Rock Soup";
		}
		return "";
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x0000CCE2 File Offset: 0x0000AEE2
	public int GetItemID()
	{
		return this.itemid;
	}

	// Token: 0x060002F4 RID: 756 RVA: 0x0000CD14 File Offset: 0x0000AF14
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyCrystalSoupAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyCrystalSoupAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_DrinkStewServer_2166136261));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_DrinkStewObservers_2166136261));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_serverdestroysoup_2166136261));
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x0000CD77 File Offset: 0x0000AF77
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateCrystalSoupAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateCrystalSoupAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x0000CD8A File Offset: 0x0000AF8A
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x060002F7 RID: 759 RVA: 0x0000CD98 File Offset: 0x0000AF98
	private void RpcWriter___Server_DrinkStewServer_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060002F8 RID: 760 RVA: 0x0000CDFD File Offset: 0x0000AFFD
	private void RpcLogic___DrinkStewServer_2166136261()
	{
		this.DrinkStewObservers();
	}

	// Token: 0x060002F9 RID: 761 RVA: 0x0000CE08 File Offset: 0x0000B008
	private void RpcReader___Server_DrinkStewServer_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___DrinkStewServer_2166136261();
	}

	// Token: 0x060002FA RID: 762 RVA: 0x0000CE28 File Offset: 0x0000B028
	private void RpcWriter___Observers_DrinkStewObservers_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060002FB RID: 763 RVA: 0x0000CE9C File Offset: 0x0000B09C
	private void RpcLogic___DrinkStewObservers_2166136261()
	{
		this.asource.PlayOneShot(this.aclips[2]);
		this.rockrender.transform.GetChild(0).gameObject.SetActive(false);
		Material[] materials = this.mrend.materials;
		materials[1] = this.blankmat;
		this.mrend.materials = materials;
		this.stewdrank = true;
	}

	// Token: 0x060002FC RID: 764 RVA: 0x0000CF00 File Offset: 0x0000B100
	private void RpcReader___Observers_DrinkStewObservers_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___DrinkStewObservers_2166136261();
	}

	// Token: 0x060002FD RID: 765 RVA: 0x0000CF20 File Offset: 0x0000B120
	private void RpcWriter___Server_serverdestroysoup_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060002FE RID: 766 RVA: 0x0000CF88 File Offset: 0x0000B188
	private void RpcLogic___serverdestroysoup_2166136261()
	{
		base.ServerManager.Despawn(base.gameObject, null);
	}

	// Token: 0x060002FF RID: 767 RVA: 0x0000CFB0 File Offset: 0x0000B1B0
	private void RpcReader___Server_serverdestroysoup_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___serverdestroysoup_2166136261();
	}

	// Token: 0x06000300 RID: 768 RVA: 0x0000CD8A File Offset: 0x0000AF8A
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0400016D RID: 365
	public GameObject rockrender;

	// Token: 0x0400016E RID: 366
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x0400016F RID: 367
	public AudioSource asource;

	// Token: 0x04000170 RID: 368
	public AudioClip[] aclips;

	// Token: 0x04000171 RID: 369
	public int stewid;

	// Token: 0x04000172 RID: 370
	public int itemid = 27;

	// Token: 0x04000173 RID: 371
	private bool stewdrank;

	// Token: 0x04000174 RID: 372
	public Material blankmat;

	// Token: 0x04000175 RID: 373
	public MeshRenderer mrend;

	// Token: 0x04000176 RID: 374
	private bool NetworkInitialize___EarlyCrystalSoupAssembly-CSharp.dll_Excuted;

	// Token: 0x04000177 RID: 375
	private bool NetworkInitialize__LateCrystalSoupAssembly-CSharp.dll_Excuted;
}
