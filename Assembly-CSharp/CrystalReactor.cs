using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x0200003E RID: 62
public class CrystalReactor : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x060002A4 RID: 676 RVA: 0x0000BEBE File Offset: 0x0000A0BE
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x060002A5 RID: 677 RVA: 0x0000BEF0 File Offset: 0x0000A0F0
	public void Interaction(GameObject player)
	{
		base.StartCoroutine(this.interrout(player));
		this.ServerToggleLerpo(1);
	}

	// Token: 0x060002A6 RID: 678 RVA: 0x0000BF07 File Offset: 0x0000A107
	private IEnumerator interrout(GameObject player)
	{
		float timer = 0f;
		while (Input.GetKey(KeyCode.Mouse0))
		{
			timer += Time.deltaTime;
			if (timer >= 5f)
			{
				this.ActualInteraction(player);
				break;
			}
			yield return null;
		}
		if (!Input.GetKey(KeyCode.Mouse0))
		{
			this.ServerToggleLerpo(2);
		}
		yield break;
	}

	// Token: 0x060002A7 RID: 679 RVA: 0x0000BF1D File Offset: 0x0000A11D
	[ServerRpc(RequireOwnership = false)]
	private void ServerToggleLerpo(int val)
	{
		this.RpcWriter___Server_ServerToggleLerpo_3316948804(val);
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x0000BF29 File Offset: 0x0000A129
	[ObserversRpc]
	private void ObserverToggleLerpo(int val)
	{
		this.RpcWriter___Observers_ObserverToggleLerpo_3316948804(val);
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x0000BF38 File Offset: 0x0000A138
	private void Update()
	{
		if (this.Lerpo == 1 && this.sphere.transform.localScale.x < 0.75f)
		{
			this.sphere.transform.localScale = new Vector3(this.sphere.transform.localScale.x + Time.deltaTime * this.sphere.transform.localScale.x, this.sphere.transform.localScale.y + Time.deltaTime * this.sphere.transform.localScale.x, this.sphere.transform.localScale.z + Time.deltaTime * this.sphere.transform.localScale.x);
			this.audisauc.volume = this.sphere.transform.localScale.x / 10f;
			return;
		}
		if (this.Lerpo == 2 && this.sphere.transform.localScale.x > 0.05f)
		{
			this.sphere.transform.localScale = new Vector3(this.sphere.transform.localScale.x - Time.deltaTime, this.sphere.transform.localScale.y - Time.deltaTime, this.sphere.transform.localScale.z - Time.deltaTime);
			this.audisauc.volume = this.sphere.transform.localScale.x / 10f;
			if (this.sphere.transform.localScale.x < 0.05f)
			{
				this.sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
				return;
			}
		}
		else if (this.audisauc.volume > 0f && this.Lerpo != 2 && this.Lerpo != 1)
		{
			this.audisauc.volume -= Time.deltaTime / 2f;
		}
	}

	// Token: 0x060002AA RID: 682 RVA: 0x0000C178 File Offset: 0x0000A378
	private void ActualInteraction(GameObject player)
	{
		PlayerInventory playerInventory;
		if (player.TryGetComponent<PlayerInventory>(out playerInventory))
		{
			playerInventory.clearHandItem();
		}
		this.ServerSphere();
	}

	// Token: 0x060002AB RID: 683 RVA: 0x0000C19B File Offset: 0x0000A39B
	[ServerRpc(RequireOwnership = false)]
	private void ServerSphere()
	{
		this.RpcWriter___Server_ServerSphere_2166136261();
	}

	// Token: 0x060002AC RID: 684 RVA: 0x0000C1A3 File Offset: 0x0000A3A3
	[ObserversRpc]
	private void ObserverSphere()
	{
		this.RpcWriter___Observers_ObserverSphere_2166136261();
	}

	// Token: 0x060002AD RID: 685 RVA: 0x0000C1AB File Offset: 0x0000A3AB
	private IEnumerator spheremaka()
	{
		float timer = 0f;
		while (timer < 1f)
		{
			this.audisauc.volume = timer;
			timer += Time.deltaTime;
			float num = Mathf.Lerp(0.05f, 60f, timer / 1f);
			this.sphere.transform.localScale = new Vector3(num, num, num);
			this.reactorLight.range = num;
			yield return null;
		}
		this.sphere.transform.localScale = new Vector3(60f, 60f, 60f);
		this.spherecol.enabled = true;
		timer = 0f;
		while (timer < 25f)
		{
			timer += 0.1f;
			this.reactorLight.intensity = 500000f + Mathf.Sin(Random.Range(0.7f, 1f) * Time.time) * Random.Range(100000f, 500000f);
			yield return new WaitForSeconds(0.1f);
		}
		timer = 0f;
		this.ServerToggleLerpo(2);
		while (timer < 1f)
		{
			this.audisauc.volume = 1f - timer;
			timer += Time.deltaTime;
			float num2 = Mathf.Lerp(60f, 0.05f, timer / 1f);
			this.sphere.transform.localScale = new Vector3(num2, num2, num2);
			this.reactorLight.range = num2;
			yield return null;
		}
		this.reactorLight.range = 0.25f;
		base.transform.GetComponent<Collider>().enabled = true;
		this.audisauc.volume = 0f;
		this.ServerToggleLerpo(0);
		yield break;
	}

	// Token: 0x060002AE RID: 686 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x060002AF RID: 687 RVA: 0x0000C1BC File Offset: 0x0000A3BC
	public void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			base.transform.position = raycastHit.point;
		}
		this.audisauc.volume = 0.3f;
		this.audisauc.PlayOneShot(this.dclipagio[1]);
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x0000C24D File Offset: 0x0000A44D
	public void PlayDropSound()
	{
		this.audisauc.PlayOneShot(this.dclipagio[1]);
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x0000C262 File Offset: 0x0000A462
	public void ItemInit()
	{
		this.rockrender.SetActive(true);
		this.audisauc.volume = 0.3f;
		this.audisauc.PlayOneShot(this.dclipagio[0]);
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x0000C262 File Offset: 0x0000A462
	public void ItemInitObs()
	{
		this.rockrender.SetActive(true);
		this.audisauc.volume = 0.3f;
		this.audisauc.PlayOneShot(this.dclipagio[0]);
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x0000C293 File Offset: 0x0000A493
	public void HideItem()
	{
		this.rockrender.SetActive(false);
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x00007BEE File Offset: 0x00005DEE
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x0000C2A1 File Offset: 0x0000A4A1
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp Crystal Reactor";
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x0000C2A8 File Offset: 0x0000A4A8
	public int GetItemID()
	{
		return 13;
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x0000C2D0 File Offset: 0x0000A4D0
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyCrystalReactorAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyCrystalReactorAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ServerToggleLerpo_3316948804));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_ObserverToggleLerpo_3316948804));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_ServerSphere_2166136261));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ObserverSphere_2166136261));
	}

	// Token: 0x060002B9 RID: 697 RVA: 0x0000C34A File Offset: 0x0000A54A
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateCrystalReactorAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateCrystalReactorAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x060002BA RID: 698 RVA: 0x0000C35D File Offset: 0x0000A55D
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x060002BB RID: 699 RVA: 0x0000C36C File Offset: 0x0000A56C
	private void RpcWriter___Server_ServerToggleLerpo_3316948804(int val)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(val);
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060002BC RID: 700 RVA: 0x0000C3DE File Offset: 0x0000A5DE
	private void RpcLogic___ServerToggleLerpo_3316948804(int val)
	{
		this.ObserverToggleLerpo(val);
	}

	// Token: 0x060002BD RID: 701 RVA: 0x0000C3E8 File Offset: 0x0000A5E8
	private void RpcReader___Server_ServerToggleLerpo_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerToggleLerpo_3316948804(num);
	}

	// Token: 0x060002BE RID: 702 RVA: 0x0000C41C File Offset: 0x0000A61C
	private void RpcWriter___Observers_ObserverToggleLerpo_3316948804(int val)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(val);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060002BF RID: 703 RVA: 0x0000C49D File Offset: 0x0000A69D
	private void RpcLogic___ObserverToggleLerpo_3316948804(int val)
	{
		this.Lerpo = val;
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x0000C4A8 File Offset: 0x0000A6A8
	private void RpcReader___Observers_ObserverToggleLerpo_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserverToggleLerpo_3316948804(num);
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x0000C4DC File Offset: 0x0000A6DC
	private void RpcWriter___Server_ServerSphere_2166136261()
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

	// Token: 0x060002C2 RID: 706 RVA: 0x0000C541 File Offset: 0x0000A741
	private void RpcLogic___ServerSphere_2166136261()
	{
		this.ObserverSphere();
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x0000C54C File Offset: 0x0000A74C
	private void RpcReader___Server_ServerSphere_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSphere_2166136261();
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x0000C56C File Offset: 0x0000A76C
	private void RpcWriter___Observers_ObserverSphere_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x0000C5E0 File Offset: 0x0000A7E0
	private void RpcLogic___ObserverSphere_2166136261()
	{
		base.StartCoroutine(this.spheremaka());
	}

	// Token: 0x060002C6 RID: 710 RVA: 0x0000C5F0 File Offset: 0x0000A7F0
	private void RpcReader___Observers_ObserverSphere_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserverSphere_2166136261();
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x0000C35D File Offset: 0x0000A55D
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000154 RID: 340
	public GameObject rockrender;

	// Token: 0x04000155 RID: 341
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x04000156 RID: 342
	public GameObject sphere;

	// Token: 0x04000157 RID: 343
	public Collider spherecol;

	// Token: 0x04000158 RID: 344
	public HDAdditionalLightData reactorLight;

	// Token: 0x04000159 RID: 345
	private int Lerpo;

	// Token: 0x0400015A RID: 346
	public AudioSource audisauc;

	// Token: 0x0400015B RID: 347
	public AudioClip[] dclipagio;

	// Token: 0x0400015C RID: 348
	private bool NetworkInitialize___EarlyCrystalReactorAssembly-CSharp.dll_Excuted;

	// Token: 0x0400015D RID: 349
	private bool NetworkInitialize__LateCrystalReactorAssembly-CSharp.dll_Excuted;
}
