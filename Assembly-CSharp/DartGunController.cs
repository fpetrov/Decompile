using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x02000049 RID: 73
public class DartGunController : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x06000321 RID: 801 RVA: 0x0000D8A1 File Offset: 0x0000BAA1
	private void Update()
	{
		this.cdtimer += Time.deltaTime;
	}

	// Token: 0x06000322 RID: 802 RVA: 0x0000D8B5 File Offset: 0x0000BAB5
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x06000323 RID: 803 RVA: 0x0000D8E8 File Offset: 0x0000BAE8
	public void Interaction(GameObject player)
	{
		if (this.cdtimer > 1f)
		{
			this.cdtimer = 0f;
			Vector3 normalized = this.dartStart.forward.normalized;
			this.ServerShootDart(normalized, new Vector3(this.dartStart.position.x, this.dartStart.position.y, this.dartStart.position.z));
		}
	}

	// Token: 0x06000324 RID: 804 RVA: 0x0000D95D File Offset: 0x0000BB5D
	[ServerRpc(RequireOwnership = false)]
	private void ServerShootDart(Vector3 Shootdir, Vector3 Startpos)
	{
		this.RpcWriter___Server_ServerShootDart_2936446947(Shootdir, Startpos);
	}

	// Token: 0x06000325 RID: 805 RVA: 0x0000D970 File Offset: 0x0000BB70
	[ObserversRpc]
	private void obsshootdart(Vector3 Shootdir, Vector3 Startpos)
	{
		this.RpcWriter___Observers_obsshootdart_2936446947(Shootdir, Startpos);
	}

	// Token: 0x06000326 RID: 806 RVA: 0x0000D98B File Offset: 0x0000BB8B
	public void applydogshit(GameObject player)
	{
		this.serverapplydogshit(player);
	}

	// Token: 0x06000327 RID: 807 RVA: 0x0000D994 File Offset: 0x0000BB94
	[ServerRpc(RequireOwnership = false)]
	private void serverapplydogshit(GameObject player)
	{
		this.RpcWriter___Server_serverapplydogshit_1934289915(player);
	}

	// Token: 0x06000328 RID: 808 RVA: 0x0000D9A0 File Offset: 0x0000BBA0
	[ObserversRpc]
	private void observersapplydogshit(GameObject player)
	{
		this.RpcWriter___Observers_observersapplydogshit_1934289915(player);
	}

	// Token: 0x06000329 RID: 809 RVA: 0x0000D9AC File Offset: 0x0000BBAC
	public void applydogshitai(GameObject ai)
	{
		this.serverapplydogshitai(ai);
	}

	// Token: 0x0600032A RID: 810 RVA: 0x0000D9B5 File Offset: 0x0000BBB5
	[ServerRpc(RequireOwnership = false)]
	private void serverapplydogshitai(GameObject ai)
	{
		this.RpcWriter___Server_serverapplydogshitai_1934289915(ai);
	}

	// Token: 0x0600032B RID: 811 RVA: 0x0000D9C4 File Offset: 0x0000BBC4
	[ObserversRpc]
	private void observersapplydogshitai(GameObject ai)
	{
		this.RpcWriter___Observers_observersapplydogshitai_1934289915(ai);
	}

	// Token: 0x0600032C RID: 812 RVA: 0x0000D9DB File Offset: 0x0000BBDB
	private IEnumerator Anitogle()
	{
		this.DartAni.SetBool("pew", true);
		yield return new WaitForSeconds(0.1f);
		this.DartAni.SetBool("pew", false);
		yield break;
	}

	// Token: 0x0600032D RID: 813 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x0600032E RID: 814 RVA: 0x0000D9EC File Offset: 0x0000BBEC
	public void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			base.transform.position = raycastHit.point;
		}
		this.Dartaud.PlayOneShot(this.squelchs[1]);
		this.rockrender.SetActive(true);
	}

	// Token: 0x0600032F RID: 815 RVA: 0x0000DA79 File Offset: 0x0000BC79
	public void PlayDropSound()
	{
		this.Dartaud.PlayOneShot(this.squelchs[1]);
	}

	// Token: 0x06000330 RID: 816 RVA: 0x0000DA8E File Offset: 0x0000BC8E
	public void ItemInit()
	{
		this.rockrender.SetActive(true);
		this.Dartaud.PlayOneShot(this.squelchs[0]);
	}

	// Token: 0x06000331 RID: 817 RVA: 0x0000DA8E File Offset: 0x0000BC8E
	public void ItemInitObs()
	{
		this.rockrender.SetActive(true);
		this.Dartaud.PlayOneShot(this.squelchs[0]);
	}

	// Token: 0x06000332 RID: 818 RVA: 0x0000DAAF File Offset: 0x0000BCAF
	public void HideItem()
	{
		this.rockrender.SetActive(false);
	}

	// Token: 0x06000333 RID: 819 RVA: 0x00007BEE File Offset: 0x00005DEE
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
	}

	// Token: 0x06000334 RID: 820 RVA: 0x0000DABD File Offset: 0x0000BCBD
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp Dart Gun";
	}

	// Token: 0x06000335 RID: 821 RVA: 0x0000DAC4 File Offset: 0x0000BCC4
	public int GetItemID()
	{
		return 19;
	}

	// Token: 0x06000337 RID: 823 RVA: 0x0000DAF8 File Offset: 0x0000BCF8
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyDartGunControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyDartGunControllerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ServerShootDart_2936446947));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_obsshootdart_2936446947));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_serverapplydogshit_1934289915));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_observersapplydogshit_1934289915));
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_serverapplydogshitai_1934289915));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_observersapplydogshitai_1934289915));
	}

	// Token: 0x06000338 RID: 824 RVA: 0x0000DBA0 File Offset: 0x0000BDA0
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateDartGunControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateDartGunControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000339 RID: 825 RVA: 0x0000DBB3 File Offset: 0x0000BDB3
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0600033A RID: 826 RVA: 0x0000DBC4 File Offset: 0x0000BDC4
	private void RpcWriter___Server_ServerShootDart_2936446947(Vector3 Shootdir, Vector3 Startpos)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(Shootdir);
		pooledWriter.WriteVector3(Startpos);
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600033B RID: 827 RVA: 0x0000DC43 File Offset: 0x0000BE43
	private void RpcLogic___ServerShootDart_2936446947(Vector3 Shootdir, Vector3 Startpos)
	{
		this.obsshootdart(Shootdir, Startpos);
	}

	// Token: 0x0600033C RID: 828 RVA: 0x0000DC50 File Offset: 0x0000BE50
	private void RpcReader___Server_ServerShootDart_2936446947(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		Vector3 vector2 = PooledReader0.ReadVector3();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerShootDart_2936446947(vector, vector2);
	}

	// Token: 0x0600033D RID: 829 RVA: 0x0000DC94 File Offset: 0x0000BE94
	private void RpcWriter___Observers_obsshootdart_2936446947(Vector3 Shootdir, Vector3 Startpos)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(Shootdir);
		pooledWriter.WriteVector3(Startpos);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600033E RID: 830 RVA: 0x0000DD24 File Offset: 0x0000BF24
	private void RpcLogic___obsshootdart_2936446947(Vector3 Shootdir, Vector3 Startpos)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.dart, Startpos, Quaternion.identity);
		this.Dartaud.PlayOneShot(this.dartclipshoot);
		gameObject.GetComponent<DartController>().Setup(Shootdir);
		gameObject.GetComponent<DartController>().dgc = base.transform.GetComponent<DartGunController>();
		base.StartCoroutine(this.Anitogle());
	}

	// Token: 0x0600033F RID: 831 RVA: 0x0000DD84 File Offset: 0x0000BF84
	private void RpcReader___Observers_obsshootdart_2936446947(PooledReader PooledReader0, Channel channel)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		Vector3 vector2 = PooledReader0.ReadVector3();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___obsshootdart_2936446947(vector, vector2);
	}

	// Token: 0x06000340 RID: 832 RVA: 0x0000DDC8 File Offset: 0x0000BFC8
	private void RpcWriter___Server_serverapplydogshit_1934289915(GameObject player)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(player);
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000341 RID: 833 RVA: 0x0000DE3A File Offset: 0x0000C03A
	private void RpcLogic___serverapplydogshit_1934289915(GameObject player)
	{
		this.observersapplydogshit(player);
	}

	// Token: 0x06000342 RID: 834 RVA: 0x0000DE44 File Offset: 0x0000C044
	private void RpcReader___Server_serverapplydogshit_1934289915(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___serverapplydogshit_1934289915(gameObject);
	}

	// Token: 0x06000343 RID: 835 RVA: 0x0000DE78 File Offset: 0x0000C078
	private void RpcWriter___Observers_observersapplydogshit_1934289915(GameObject player)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(player);
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000344 RID: 836 RVA: 0x0000DEF9 File Offset: 0x0000C0F9
	private void RpcLogic___observersapplydogshit_1934289915(GameObject player)
	{
		player.GetComponent<PlayerMovement>().HitByDart();
	}

	// Token: 0x06000345 RID: 837 RVA: 0x0000DF08 File Offset: 0x0000C108
	private void RpcReader___Observers_observersapplydogshit_1934289915(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___observersapplydogshit_1934289915(gameObject);
	}

	// Token: 0x06000346 RID: 838 RVA: 0x0000DF3C File Offset: 0x0000C13C
	private void RpcWriter___Server_serverapplydogshitai_1934289915(GameObject ai)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(ai);
		base.SendServerRpc(4U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000347 RID: 839 RVA: 0x0000DFAE File Offset: 0x0000C1AE
	private void RpcLogic___serverapplydogshitai_1934289915(GameObject ai)
	{
		this.observersapplydogshitai(ai);
	}

	// Token: 0x06000348 RID: 840 RVA: 0x0000DFB8 File Offset: 0x0000C1B8
	private void RpcReader___Server_serverapplydogshitai_1934289915(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___serverapplydogshitai_1934289915(gameObject);
	}

	// Token: 0x06000349 RID: 841 RVA: 0x0000DFEC File Offset: 0x0000C1EC
	private void RpcWriter___Observers_observersapplydogshitai_1934289915(GameObject ai)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(ai);
		base.SendObserversRpc(5U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600034A RID: 842 RVA: 0x0000E070 File Offset: 0x0000C270
	private void RpcLogic___observersapplydogshitai_1934289915(GameObject ai)
	{
		ShadowWizardAI component;
		if ((component = ai.GetComponent<ShadowWizardAI>()).CheckOwner())
		{
			component.HitByDart();
		}
	}

	// Token: 0x0600034B RID: 843 RVA: 0x0000E094 File Offset: 0x0000C294
	private void RpcReader___Observers_observersapplydogshitai_1934289915(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___observersapplydogshitai_1934289915(gameObject);
	}

	// Token: 0x0600034C RID: 844 RVA: 0x0000DBB3 File Offset: 0x0000BDB3
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000199 RID: 409
	public GameObject rockrender;

	// Token: 0x0400019A RID: 410
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x0400019B RID: 411
	public float maxDistance = 55f;

	// Token: 0x0400019C RID: 412
	public LayerMask layermsk;

	// Token: 0x0400019D RID: 413
	public GameObject dart;

	// Token: 0x0400019E RID: 414
	private float cdtimer;

	// Token: 0x0400019F RID: 415
	public Animator DartAni;

	// Token: 0x040001A0 RID: 416
	public Transform dartStart;

	// Token: 0x040001A1 RID: 417
	public AudioSource Dartaud;

	// Token: 0x040001A2 RID: 418
	public AudioClip dartclipshoot;

	// Token: 0x040001A3 RID: 419
	public AudioClip[] squelchs;

	// Token: 0x040001A4 RID: 420
	private bool NetworkInitialize___EarlyDartGunControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x040001A5 RID: 421
	private bool NetworkInitialize__LateDartGunControllerAssembly-CSharp.dll_Excuted;
}
