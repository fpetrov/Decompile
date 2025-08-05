using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x020000FF RID: 255
public class PageController : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x06000A4E RID: 2638 RVA: 0x000270F1 File Offset: 0x000252F1
	private void Start()
	{
		this.PageMaterial = this.pagerender.material;
	}

	// Token: 0x06000A4F RID: 2639 RVA: 0x00027104 File Offset: 0x00025304
	public void ReinstatePageEmis()
	{
		base.StartCoroutine(this.LerpEmis(this.PageMaterial, this.PageEmissiveVal, "_emissi"));
	}

	// Token: 0x06000A50 RID: 2640 RVA: 0x00027124 File Offset: 0x00025324
	public void CastSpell(GameObject ownerobj, int level)
	{
		base.StartCoroutine(this.LerpEmis(this.PageMaterial, 0f, "_emissi"));
		this.CastSpellServer(ownerobj, Camera.main.transform.forward, level, this.firePoint.position);
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x00027170 File Offset: 0x00025370
	[ServerRpc(RequireOwnership = false)]
	private void CastSpellServer(GameObject ownerobj, Vector3 fwdVector, int level, Vector3 spawnpos)
	{
		this.RpcWriter___Server_CastSpellServer_3976682022(ownerobj, fwdVector, level, spawnpos);
	}

	// Token: 0x06000A52 RID: 2642 RVA: 0x00027188 File Offset: 0x00025388
	[ObserversRpc]
	private void CastSpellObs(GameObject ownerobj, Vector3 fwdVector, int level, Vector3 spawnpos)
	{
		this.RpcWriter___Observers_CastSpellObs_3976682022(ownerobj, fwdVector, level, spawnpos);
	}

	// Token: 0x06000A53 RID: 2643 RVA: 0x000271AB File Offset: 0x000253AB
	private IEnumerator LerpEmis(Material mat, float targetVal, string paramName)
	{
		float timer = 0f;
		if (mat != null)
		{
			float startval = mat.GetFloat(paramName);
			while (timer < 0.2f && mat != null)
			{
				mat.SetFloat(paramName, Mathf.Lerp(startval, targetVal, timer * 5f));
				yield return null;
				timer += Time.deltaTime;
			}
			if (mat != null)
			{
				mat.SetFloat(paramName, targetVal);
			}
		}
		yield break;
	}

	// Token: 0x06000A54 RID: 2644 RVA: 0x000271C8 File Offset: 0x000253C8
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x06000A55 RID: 2645 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction(GameObject player)
	{
	}

	// Token: 0x06000A56 RID: 2646 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x06000A57 RID: 2647 RVA: 0x000271FC File Offset: 0x000253FC
	public void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.position = raycastHit.point + new Vector3(0f, 0.1f, 0f);
			base.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
		}
		this.asource.volume = 0.2f;
		this.asource.pitch = 1.3f;
		this.asource.PlayOneShot(this.aclips[1]);
		this.pap = null;
	}

	// Token: 0x06000A58 RID: 2648 RVA: 0x000272BD File Offset: 0x000254BD
	public void PlayDropSound()
	{
		this.asource.PlayOneShot(this.aclips[1]);
	}

	// Token: 0x06000A59 RID: 2649 RVA: 0x000272D4 File Offset: 0x000254D4
	public void ItemInit()
	{
		this.asource.pitch = 1f;
		this.asource.volume = 0.5f;
		this.pagerender.gameObject.SetActive(true);
		this.asource.PlayOneShot(this.aclips[0]);
		this.pageani.SetBool("onground", false);
	}

	// Token: 0x06000A5A RID: 2650 RVA: 0x00027338 File Offset: 0x00025538
	public void ItemInitObs()
	{
		this.pagerender.gameObject.SetActive(true);
		this.asource.pitch = 1f;
		this.asource.volume = 0.5f;
		this.asource.PlayOneShot(this.aclips[0]);
		this.pageani.SetBool("onground", false);
	}

	// Token: 0x06000A5B RID: 2651 RVA: 0x0002739A File Offset: 0x0002559A
	public void HideItem()
	{
		this.pagerender.gameObject.SetActive(false);
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x000273AD File Offset: 0x000255AD
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
		this.pap = player.GetComponent<PlayerAudioPlayer>();
		base.StartCoroutine(this.checkrun());
	}

	// Token: 0x06000A5D RID: 2653 RVA: 0x000273D9 File Offset: 0x000255D9
	private IEnumerator checkrun()
	{
		while (this.pap != null)
		{
			if (this.pap.magnitude > 6f)
			{
				this.pageani.SetBool("run", true);
			}
			else
			{
				this.pageani.SetBool("run", false);
			}
			yield return null;
		}
		this.pageani.SetBool("run", false);
		yield break;
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x000273E8 File Offset: 0x000255E8
	public string DisplayInteractUI(GameObject player)
	{
		return this.pickupText;
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x000273F0 File Offset: 0x000255F0
	public int GetItemID()
	{
		return this.ItemID;
	}

	// Token: 0x06000A61 RID: 2657 RVA: 0x00027428 File Offset: 0x00025628
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyPageControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyPageControllerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_CastSpellServer_3976682022));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_CastSpellObs_3976682022));
	}

	// Token: 0x06000A62 RID: 2658 RVA: 0x00027474 File Offset: 0x00025674
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LatePageControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LatePageControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000A63 RID: 2659 RVA: 0x00027487 File Offset: 0x00025687
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000A64 RID: 2660 RVA: 0x00027498 File Offset: 0x00025698
	private void RpcWriter___Server_CastSpellServer_3976682022(GameObject ownerobj, Vector3 fwdVector, int level, Vector3 spawnpos)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(ownerobj);
		pooledWriter.WriteVector3(fwdVector);
		pooledWriter.WriteInt32(level);
		pooledWriter.WriteVector3(spawnpos);
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000A65 RID: 2661 RVA: 0x00027531 File Offset: 0x00025731
	private void RpcLogic___CastSpellServer_3976682022(GameObject ownerobj, Vector3 fwdVector, int level, Vector3 spawnpos)
	{
		if (ownerobj != null)
		{
			this.CastSpellObs(ownerobj, fwdVector, level, spawnpos);
		}
	}

	// Token: 0x06000A66 RID: 2662 RVA: 0x00027548 File Offset: 0x00025748
	private void RpcReader___Server_CastSpellServer_3976682022(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		Vector3 vector = PooledReader0.ReadVector3();
		int num = PooledReader0.ReadInt32();
		Vector3 vector2 = PooledReader0.ReadVector3();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___CastSpellServer_3976682022(gameObject, vector, num, vector2);
	}

	// Token: 0x06000A67 RID: 2663 RVA: 0x000275AC File Offset: 0x000257AC
	private void RpcWriter___Observers_CastSpellObs_3976682022(GameObject ownerobj, Vector3 fwdVector, int level, Vector3 spawnpos)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(ownerobj);
		pooledWriter.WriteVector3(fwdVector);
		pooledWriter.WriteInt32(level);
		pooledWriter.WriteVector3(spawnpos);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000A68 RID: 2664 RVA: 0x00027654 File Offset: 0x00025854
	private void RpcLogic___CastSpellObs_3976682022(GameObject ownerobj, Vector3 fwdVector, int level, Vector3 spawnpos)
	{
		if (ownerobj != null)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.spellprefab, spawnpos, Quaternion.identity);
			ISpell spell;
			if (gameObject.TryGetComponent<ISpell>(out spell))
			{
				spell.PlayerSetup(ownerobj, fwdVector, level);
				return;
			}
			DarkBlastController darkBlastController;
			if (gameObject.TryGetComponent<DarkBlastController>(out darkBlastController))
			{
				darkBlastController.CastDarkBlast(fwdVector, ownerobj);
				ownerobj.GetComponent<PlayerMovement>().applydbrecoil();
			}
		}
	}

	// Token: 0x06000A69 RID: 2665 RVA: 0x000276B0 File Offset: 0x000258B0
	private void RpcReader___Observers_CastSpellObs_3976682022(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		Vector3 vector = PooledReader0.ReadVector3();
		int num = PooledReader0.ReadInt32();
		Vector3 vector2 = PooledReader0.ReadVector3();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___CastSpellObs_3976682022(gameObject, vector, num, vector2);
	}

	// Token: 0x06000A6A RID: 2666 RVA: 0x00027487 File Offset: 0x00025687
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0400056F RID: 1391
	public SkinnedMeshRenderer pagerender;

	// Token: 0x04000570 RID: 1392
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x04000571 RID: 1393
	public AudioSource asource;

	// Token: 0x04000572 RID: 1394
	public AudioClip[] aclips;

	// Token: 0x04000573 RID: 1395
	public string pickupText;

	// Token: 0x04000574 RID: 1396
	public int ItemID;

	// Token: 0x04000575 RID: 1397
	public Animator pageani;

	// Token: 0x04000576 RID: 1398
	private PlayerAudioPlayer pap;

	// Token: 0x04000577 RID: 1399
	public GameObject spellprefab;

	// Token: 0x04000578 RID: 1400
	public float PageCoolDownTimer;

	// Token: 0x04000579 RID: 1401
	public float CoolDown;

	// Token: 0x0400057A RID: 1402
	private Material PageMaterial;

	// Token: 0x0400057B RID: 1403
	public float PageEmissiveVal = 1000f;

	// Token: 0x0400057C RID: 1404
	public Transform firePoint;

	// Token: 0x0400057D RID: 1405
	private bool NetworkInitialize___EarlyPageControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x0400057E RID: 1406
	private bool NetworkInitialize__LatePageControllerAssembly-CSharp.dll_Excuted;
}
