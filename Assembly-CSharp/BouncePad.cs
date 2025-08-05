using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x0200001F RID: 31
public class BouncePad : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x0600016B RID: 363 RVA: 0x00007A25 File Offset: 0x00005C25
	private void Start()
	{
		this.audsm = base.transform.GetComponent<AudioSource>();
	}

	// Token: 0x0600016C RID: 364 RVA: 0x00007A38 File Offset: 0x00005C38
	private void OnTriggerEnter(Collider other)
	{
		PlayerMovement playerMovement;
		if (other.gameObject.CompareTag("Player") && Time.time - this.lastTrigger > 1f && other.gameObject.TryGetComponent<PlayerMovement>(out playerMovement) && playerMovement.checkowner())
		{
			this.lastTrigger = Time.time;
			this.Serveryump();
		}
	}

	// Token: 0x0600016D RID: 365 RVA: 0x00007A92 File Offset: 0x00005C92
	[ServerRpc(RequireOwnership = false)]
	private void Serveryump()
	{
		this.RpcWriter___Server_Serveryump_2166136261();
	}

	// Token: 0x0600016E RID: 366 RVA: 0x00007A9A File Offset: 0x00005C9A
	[ObserversRpc]
	private void obsyump()
	{
		this.RpcWriter___Observers_obsyump_2166136261();
	}

	// Token: 0x0600016F RID: 367 RVA: 0x00007AA2 File Offset: 0x00005CA2
	private IEnumerator yumproutine()
	{
		this.audsm.PlayOneShot(this.mushclips[Random.Range(0, this.mushclips.Length - 2)]);
		this.ShrAni.SetBool("yump", true);
		yield return new WaitForSeconds(0.1f);
		this.ShrAni.SetBool("yump", false);
		yield break;
	}

	// Token: 0x06000170 RID: 368 RVA: 0x00007AB1 File Offset: 0x00005CB1
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
	}

	// Token: 0x06000171 RID: 369 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction(GameObject player)
	{
	}

	// Token: 0x06000172 RID: 370 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x06000173 RID: 371 RVA: 0x00007AE4 File Offset: 0x00005CE4
	public void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
			base.transform.position = raycastHit.point;
			this.BounceCol.enabled = true;
		}
		this.audsm.PlayOneShot(this.mushclips[5]);
		this.rockrender.SetActive(true);
	}

	// Token: 0x06000174 RID: 372 RVA: 0x00007B7D File Offset: 0x00005D7D
	public void PlayDropSound()
	{
		this.audsm.PlayOneShot(this.mushclips[5]);
	}

	// Token: 0x06000175 RID: 373 RVA: 0x00007B92 File Offset: 0x00005D92
	public void ItemInit()
	{
		this.rockrender.SetActive(true);
		this.BounceCol.enabled = false;
		this.audsm.PlayOneShot(this.mushclips[4]);
	}

	// Token: 0x06000176 RID: 374 RVA: 0x00007BBF File Offset: 0x00005DBF
	public void ItemInitObs()
	{
		this.rockrender.SetActive(true);
		this.audsm.PlayOneShot(this.mushclips[4]);
	}

	// Token: 0x06000177 RID: 375 RVA: 0x00007BE0 File Offset: 0x00005DE0
	public void HideItem()
	{
		this.rockrender.SetActive(false);
	}

	// Token: 0x06000178 RID: 376 RVA: 0x00007BEE File Offset: 0x00005DEE
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
	}

	// Token: 0x06000179 RID: 377 RVA: 0x00007C01 File Offset: 0x00005E01
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp Bounce Mush";
	}

	// Token: 0x0600017A RID: 378 RVA: 0x00007C08 File Offset: 0x00005E08
	public int GetItemID()
	{
		return 4;
	}

	// Token: 0x0600017C RID: 380 RVA: 0x00007C30 File Offset: 0x00005E30
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyBouncePadAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyBouncePadAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_Serveryump_2166136261));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_obsyump_2166136261));
	}

	// Token: 0x0600017D RID: 381 RVA: 0x00007C7C File Offset: 0x00005E7C
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateBouncePadAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateBouncePadAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x0600017E RID: 382 RVA: 0x00007C8F File Offset: 0x00005E8F
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0600017F RID: 383 RVA: 0x00007CA0 File Offset: 0x00005EA0
	private void RpcWriter___Server_Serveryump_2166136261()
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

	// Token: 0x06000180 RID: 384 RVA: 0x00007D05 File Offset: 0x00005F05
	private void RpcLogic___Serveryump_2166136261()
	{
		this.obsyump();
	}

	// Token: 0x06000181 RID: 385 RVA: 0x00007D10 File Offset: 0x00005F10
	private void RpcReader___Server_Serveryump_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___Serveryump_2166136261();
	}

	// Token: 0x06000182 RID: 386 RVA: 0x00007D30 File Offset: 0x00005F30
	private void RpcWriter___Observers_obsyump_2166136261()
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

	// Token: 0x06000183 RID: 387 RVA: 0x00007DA4 File Offset: 0x00005FA4
	private void RpcLogic___obsyump_2166136261()
	{
		base.StartCoroutine(this.yumproutine());
	}

	// Token: 0x06000184 RID: 388 RVA: 0x00007DB4 File Offset: 0x00005FB4
	private void RpcReader___Observers_obsyump_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___obsyump_2166136261();
	}

	// Token: 0x06000185 RID: 389 RVA: 0x00007C8F File Offset: 0x00005E8F
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x040000B0 RID: 176
	public GameObject rockrender;

	// Token: 0x040000B1 RID: 177
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x040000B2 RID: 178
	public Collider BounceCol;

	// Token: 0x040000B3 RID: 179
	public Animator ShrAni;

	// Token: 0x040000B4 RID: 180
	private AudioSource audsm;

	// Token: 0x040000B5 RID: 181
	public AudioClip[] mushclips;

	// Token: 0x040000B6 RID: 182
	private float lastTrigger;

	// Token: 0x040000B7 RID: 183
	private bool NetworkInitialize___EarlyBouncePadAssembly-CSharp.dll_Excuted;

	// Token: 0x040000B8 RID: 184
	private bool NetworkInitialize__LateBouncePadAssembly-CSharp.dll_Excuted;
}
