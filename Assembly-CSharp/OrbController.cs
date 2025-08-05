using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x020000FD RID: 253
public class OrbController : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x06000A2C RID: 2604 RVA: 0x00026A9F File Offset: 0x00024C9F
	public override void OnStartClient()
	{
		base.OnStartClient();
		this.GetPlayers();
	}

	// Token: 0x06000A2D RID: 2605 RVA: 0x00026AAD File Offset: 0x00024CAD
	[ServerRpc(RequireOwnership = false)]
	private void GetPlayers()
	{
		this.RpcWriter___Server_GetPlayers_2166136261();
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x00026AB8 File Offset: 0x00024CB8
	[ObserversRpc]
	private void GetPlayersObs()
	{
		this.RpcWriter___Observers_GetPlayersObs_2166136261();
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x00026ACB File Offset: 0x00024CCB
	private void Start()
	{
		this.OrbMat = this.orb.GetComponent<Renderer>().material;
	}

	// Token: 0x06000A30 RID: 2608 RVA: 0x00026AE3 File Offset: 0x00024CE3
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x06000A31 RID: 2609 RVA: 0x00026B15 File Offset: 0x00024D15
	public void Interaction(GameObject player)
	{
		this.isOrbing = true;
		base.StartCoroutine(this.OrbRoutine());
	}

	// Token: 0x06000A32 RID: 2610 RVA: 0x00026B2B File Offset: 0x00024D2B
	private IEnumerator OrbRoutine()
	{
		this.OrbCam.gameObject.SetActive(true);
		this.OrbCam.SetParent(null);
		this.OrbMatLerpTarget = 0f;
		if (this.orbSyncIndex < this.EligiblePlayers.Count - 1)
		{
			this.orbSyncIndex++;
		}
		else
		{
			this.orbSyncIndex = 0;
		}
		while (Input.GetKey(KeyCode.Mouse0))
		{
			yield return null;
		}
		this.OrbMatLerpTarget = 10f;
		this.OrbCam.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06000A33 RID: 2611 RVA: 0x00026B3C File Offset: 0x00024D3C
	private void Update()
	{
		if (this.EligiblePlayers.Count > 0 && this.EligiblePlayers[this.orbSyncIndex] != null)
		{
			this.OrbMat.SetFloat("_blender", Mathf.Lerp(this.OrbMat.GetFloat("_blender"), this.OrbMatLerpTarget, Time.deltaTime));
			this.OrbCam.position = new Vector3(this.EligiblePlayers[this.orbSyncIndex].transform.position.x, this.EligiblePlayers[this.orbSyncIndex].transform.position.y + 2f, this.EligiblePlayers[this.orbSyncIndex].transform.position.z);
			this.OrbCam.rotation = Camera.main.transform.rotation;
		}
	}

	// Token: 0x06000A34 RID: 2612 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x06000A35 RID: 2613 RVA: 0x00026C38 File Offset: 0x00024E38
	public void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			base.transform.position = raycastHit.point;
		}
		this.asource.PlayOneShot(this.aclips[1]);
		this.rockrender.SetActive(true);
	}

	// Token: 0x06000A36 RID: 2614 RVA: 0x00026CC5 File Offset: 0x00024EC5
	public void PlayDropSound()
	{
		this.asource.PlayOneShot(this.aclips[1]);
	}

	// Token: 0x06000A37 RID: 2615 RVA: 0x00026CDA File Offset: 0x00024EDA
	public void ItemInit()
	{
		this.rockrender.SetActive(true);
		this.asource.PlayOneShot(this.aclips[0]);
	}

	// Token: 0x06000A38 RID: 2616 RVA: 0x00026CDA File Offset: 0x00024EDA
	public void ItemInitObs()
	{
		this.rockrender.SetActive(true);
		this.asource.PlayOneShot(this.aclips[0]);
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x00026CFB File Offset: 0x00024EFB
	public void HideItem()
	{
		this.rockrender.SetActive(false);
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x00026D0C File Offset: 0x00024F0C
	public void Interact(GameObject player)
	{
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject gameObject in this.EligiblePlayers)
		{
			PlayerMovement playerMovement;
			PlayerMovement playerMovement2;
			if (gameObject != null && gameObject.TryGetComponent<PlayerMovement>(out playerMovement) && player.TryGetComponent<PlayerMovement>(out playerMovement2) && playerMovement.playerTeam == playerMovement2.playerTeam)
			{
				list.Add(gameObject);
			}
		}
		foreach (GameObject gameObject2 in list)
		{
			this.EligiblePlayers.Remove(gameObject2);
		}
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x00026DE8 File Offset: 0x00024FE8
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp Orb";
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x00026DEF File Offset: 0x00024FEF
	public int GetItemID()
	{
		return 16;
	}

	// Token: 0x06000A3E RID: 2622 RVA: 0x00026E2C File Offset: 0x0002502C
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyOrbControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyOrbControllerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_GetPlayers_2166136261));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_GetPlayersObs_2166136261));
	}

	// Token: 0x06000A3F RID: 2623 RVA: 0x00026E78 File Offset: 0x00025078
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateOrbControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateOrbControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000A40 RID: 2624 RVA: 0x00026E8B File Offset: 0x0002508B
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x00026E9C File Offset: 0x0002509C
	private void RpcWriter___Server_GetPlayers_2166136261()
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

	// Token: 0x06000A42 RID: 2626 RVA: 0x00026F01 File Offset: 0x00025101
	private void RpcLogic___GetPlayers_2166136261()
	{
		this.GetPlayersObs();
	}

	// Token: 0x06000A43 RID: 2627 RVA: 0x00026F0C File Offset: 0x0002510C
	private void RpcReader___Server_GetPlayers_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___GetPlayers_2166136261();
	}

	// Token: 0x06000A44 RID: 2628 RVA: 0x00026F2C File Offset: 0x0002512C
	private void RpcWriter___Observers_GetPlayersObs_2166136261()
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

	// Token: 0x06000A45 RID: 2629 RVA: 0x00026FA0 File Offset: 0x000251A0
	private void RpcLogic___GetPlayersObs_2166136261()
	{
		this.Players = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject gameObject in this.Players)
		{
			PlayerMovement playerMovement;
			if (gameObject != null && gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
			{
				this.EligiblePlayers.Add(gameObject);
			}
		}
	}

	// Token: 0x06000A46 RID: 2630 RVA: 0x00026FF8 File Offset: 0x000251F8
	private void RpcReader___Observers_GetPlayersObs_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___GetPlayersObs_2166136261();
	}

	// Token: 0x06000A47 RID: 2631 RVA: 0x00026E8B File Offset: 0x0002508B
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0400055C RID: 1372
	public Transform OrbCam;

	// Token: 0x0400055D RID: 1373
	public GameObject rockrender;

	// Token: 0x0400055E RID: 1374
	public GameObject orb;

	// Token: 0x0400055F RID: 1375
	private Material OrbMat;

	// Token: 0x04000560 RID: 1376
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x04000561 RID: 1377
	public LayerMask layermsk;

	// Token: 0x04000562 RID: 1378
	private GameObject[] Players;

	// Token: 0x04000563 RID: 1379
	public List<GameObject> EligiblePlayers = new List<GameObject>();

	// Token: 0x04000564 RID: 1380
	private bool isOrbing;

	// Token: 0x04000565 RID: 1381
	private float OrbMatLerpTarget = 10f;

	// Token: 0x04000566 RID: 1382
	private int orbSyncIndex;

	// Token: 0x04000567 RID: 1383
	private float myteamnum;

	// Token: 0x04000568 RID: 1384
	public AudioSource asource;

	// Token: 0x04000569 RID: 1385
	public AudioClip[] aclips;

	// Token: 0x0400056A RID: 1386
	private bool NetworkInitialize___EarlyOrbControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x0400056B RID: 1387
	private bool NetworkInitialize__LateOrbControllerAssembly-CSharp.dll_Excuted;
}
