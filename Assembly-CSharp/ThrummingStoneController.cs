using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x020001B3 RID: 435
public class ThrummingStoneController : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x06001212 RID: 4626 RVA: 0x0004CECB File Offset: 0x0004B0CB
	private void Start()
	{
		this.spheremat = this.sphererend.material;
		this.lightning = this.mrend.materials[1];
	}

	// Token: 0x06001213 RID: 4627 RVA: 0x0004CEF1 File Offset: 0x0004B0F1
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x06001214 RID: 4628 RVA: 0x0004CF23 File Offset: 0x0004B123
	public void Interaction(GameObject player)
	{
		if (Time.time - this.cd > 10f)
		{
			this.cd = Time.time;
			this.pinv.canSwapItem = false;
			this.ServerMakeSphere();
		}
	}

	// Token: 0x06001215 RID: 4629 RVA: 0x0004CF55 File Offset: 0x0004B155
	[ServerRpc(RequireOwnership = false)]
	private void ServerMakeSphere()
	{
		this.RpcWriter___Server_ServerMakeSphere_2166136261();
	}

	// Token: 0x06001216 RID: 4630 RVA: 0x0004CF5D File Offset: 0x0004B15D
	[ObserversRpc]
	private void ObserverMakeSphere()
	{
		this.RpcWriter___Observers_ObserverMakeSphere_2166136261();
	}

	// Token: 0x06001217 RID: 4631 RVA: 0x0004CF65 File Offset: 0x0004B165
	private IEnumerator spheremaka()
	{
		float timer = 0f;
		this.audisauc.volume = 1f;
		while (timer < 1f)
		{
			timer += Time.deltaTime;
			float num = Mathf.Lerp(0f, 7f, timer);
			this.sphere.transform.localScale = new Vector3(num, num, num);
			this.spheremat.SetFloat("_alpha", Mathf.Lerp(0.8f, 0f, timer));
			this.sphere2.material = this.spheremat;
			yield return null;
		}
		yield return new WaitForSeconds(0.1f);
		this.sphere.transform.localScale = new Vector3(0f, 0f, 0f);
		this.pinv.canSwapItem = true;
		yield break;
	}

	// Token: 0x06001218 RID: 4632 RVA: 0x0004CF74 File Offset: 0x0004B174
	private void Update()
	{
		if (this.audisauc.volume > 0.05f)
		{
			this.audisauc.volume -= Time.deltaTime / 2f;
		}
		this.lightningval -= Time.deltaTime * 1.5f;
		this.lightning.SetFloat("_idk", this.lightningval);
	}

	// Token: 0x06001219 RID: 4633 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x0600121A RID: 4634 RVA: 0x0004CFE0 File Offset: 0x0004B1E0
	public void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			base.transform.position = raycastHit.point;
		}
		this.audisauc.volume = 0f;
		this.audisauc.PlayOneShot(this.dclipagio[1]);
		this.rockrender.SetActive(true);
	}

	// Token: 0x0600121B RID: 4635 RVA: 0x0004D07D File Offset: 0x0004B27D
	public void PlayDropSound()
	{
		this.audisauc.PlayOneShot(this.dclipagio[1]);
	}

	// Token: 0x0600121C RID: 4636 RVA: 0x0004D092 File Offset: 0x0004B292
	public void ItemInit()
	{
		this.rockrender.SetActive(true);
		this.audisauc.volume = 0.3f;
		this.audisauc.PlayOneShot(this.dclipagio[0]);
	}

	// Token: 0x0600121D RID: 4637 RVA: 0x0004D092 File Offset: 0x0004B292
	public void ItemInitObs()
	{
		this.rockrender.SetActive(true);
		this.audisauc.volume = 0.3f;
		this.audisauc.PlayOneShot(this.dclipagio[0]);
	}

	// Token: 0x0600121E RID: 4638 RVA: 0x0004D0C3 File Offset: 0x0004B2C3
	public void HideItem()
	{
		this.audisauc.volume = 0f;
		this.rockrender.SetActive(false);
	}

	// Token: 0x0600121F RID: 4639 RVA: 0x0004D0E1 File Offset: 0x0004B2E1
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
		this.pinv = player.GetComponent<PlayerInventory>();
	}

	// Token: 0x06001220 RID: 4640 RVA: 0x0004D100 File Offset: 0x0004B300
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp Thrumming Stone";
	}

	// Token: 0x06001221 RID: 4641 RVA: 0x0000C2A8 File Offset: 0x0000A4A8
	public int GetItemID()
	{
		return 13;
	}

	// Token: 0x06001223 RID: 4643 RVA: 0x0004D134 File Offset: 0x0004B334
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyThrummingStoneControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyThrummingStoneControllerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ServerMakeSphere_2166136261));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_ObserverMakeSphere_2166136261));
	}

	// Token: 0x06001224 RID: 4644 RVA: 0x0004D180 File Offset: 0x0004B380
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateThrummingStoneControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateThrummingStoneControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06001225 RID: 4645 RVA: 0x0004D193 File Offset: 0x0004B393
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06001226 RID: 4646 RVA: 0x0004D1A4 File Offset: 0x0004B3A4
	private void RpcWriter___Server_ServerMakeSphere_2166136261()
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

	// Token: 0x06001227 RID: 4647 RVA: 0x0004D209 File Offset: 0x0004B409
	private void RpcLogic___ServerMakeSphere_2166136261()
	{
		this.ObserverMakeSphere();
	}

	// Token: 0x06001228 RID: 4648 RVA: 0x0004D214 File Offset: 0x0004B414
	private void RpcReader___Server_ServerMakeSphere_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerMakeSphere_2166136261();
	}

	// Token: 0x06001229 RID: 4649 RVA: 0x0004D234 File Offset: 0x0004B434
	private void RpcWriter___Observers_ObserverMakeSphere_2166136261()
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

	// Token: 0x0600122A RID: 4650 RVA: 0x0004D2A8 File Offset: 0x0004B4A8
	private void RpcLogic___ObserverMakeSphere_2166136261()
	{
		base.StartCoroutine(this.spheremaka());
	}

	// Token: 0x0600122B RID: 4651 RVA: 0x0004D2B8 File Offset: 0x0004B4B8
	private void RpcReader___Observers_ObserverMakeSphere_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserverMakeSphere_2166136261();
	}

	// Token: 0x0600122C RID: 4652 RVA: 0x0004D193 File Offset: 0x0004B393
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000A71 RID: 2673
	public GameObject rockrender;

	// Token: 0x04000A72 RID: 2674
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x04000A73 RID: 2675
	public GameObject sphere;

	// Token: 0x04000A74 RID: 2676
	public AudioSource audisauc;

	// Token: 0x04000A75 RID: 2677
	public AudioClip[] dclipagio;

	// Token: 0x04000A76 RID: 2678
	public MeshRenderer mrend;

	// Token: 0x04000A77 RID: 2679
	public MeshRenderer sphererend;

	// Token: 0x04000A78 RID: 2680
	public MeshRenderer sphere2;

	// Token: 0x04000A79 RID: 2681
	private Material spheremat;

	// Token: 0x04000A7A RID: 2682
	private Material lightning;

	// Token: 0x04000A7B RID: 2683
	private float cd;

	// Token: 0x04000A7C RID: 2684
	private float lightningval = 500000f;

	// Token: 0x04000A7D RID: 2685
	private PlayerInventory pinv;

	// Token: 0x04000A7E RID: 2686
	private bool NetworkInitialize___EarlyThrummingStoneControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x04000A7F RID: 2687
	private bool NetworkInitialize__LateThrummingStoneControllerAssembly-CSharp.dll_Excuted;
}
