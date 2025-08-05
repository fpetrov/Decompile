using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x02000003 RID: 3
public class AiMAgeBook : NetworkBehaviour
{
	// Token: 0x06000003 RID: 3 RVA: 0x00002064 File Offset: 0x00000264
	public override void OnStartClient()
	{
		base.OnStartClient();
		this.BookInit();
	}

	// Token: 0x06000004 RID: 4 RVA: 0x00002072 File Offset: 0x00000272
	public void FlipPage(int pagenum)
	{
		this.ServerFlipPage(pagenum);
	}

	// Token: 0x06000005 RID: 5 RVA: 0x0000207B File Offset: 0x0000027B
	[ServerRpc(RequireOwnership = false)]
	private void ServerFlipPage(int pg)
	{
		this.RpcWriter___Server_ServerFlipPage_3316948804(pg);
	}

	// Token: 0x06000006 RID: 6 RVA: 0x00002088 File Offset: 0x00000288
	[ObserversRpc]
	private void ObserversFlipPage(int pgn)
	{
		this.RpcWriter___Observers_ObserversFlipPage_3316948804(pgn);
	}

	// Token: 0x06000007 RID: 7 RVA: 0x0000209F File Offset: 0x0000029F
	private IEnumerator Lerppos(Transform ltarget, float lval)
	{
		float i = 0f;
		float currentValue = ltarget.localPosition.y;
		while (i < 0.05f)
		{
			i += Time.deltaTime;
			float num = i / 0.05f;
			currentValue = Mathf.Lerp(currentValue, lval, num);
			ltarget.localPosition = new Vector3(0f, currentValue, 0f);
			yield return null;
		}
		ltarget.localPosition = new Vector3(0f, lval, 0f);
		yield break;
	}

	// Token: 0x06000008 RID: 8 RVA: 0x000020B8 File Offset: 0x000002B8
	public void Interaction(GameObject player)
	{
		RaycastHit raycastHit;
		GetPlayerGameobject getPlayerGameobject;
		if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out raycastHit, 100f, this.layerMask) && raycastHit.transform.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
		{
			this.ServerMakeIcebox(getPlayerGameobject.player);
		}
	}

	// Token: 0x06000009 RID: 9 RVA: 0x0000211D File Offset: 0x0000031D
	[ServerRpc(RequireOwnership = false)]
	private void ServerMakeIcebox(GameObject player)
	{
		this.RpcWriter___Server_ServerMakeIcebox_1934289915(player);
	}

	// Token: 0x0600000A RID: 10 RVA: 0x00002129 File Offset: 0x00000329
	[ObserversRpc]
	private void ObserversMakeIcebox(GameObject Player)
	{
		this.RpcWriter___Observers_ObserversMakeIcebox_1934289915(Player);
	}

	// Token: 0x0600000B RID: 11 RVA: 0x00002138 File Offset: 0x00000338
	public void BookInit()
	{
		this.pages[3].localPosition = new Vector3(0f, 0.09f, 0f);
		this.pages[2].localPosition = new Vector3(0f, 0.08f, 0f);
		this.pages[1].localPosition = new Vector3(0f, 0.06f, 0f);
		foreach (Animator animator in this.animaes)
		{
			animator.Rebind();
			animator.Update(0f);
			animator.SetBool("eq", true);
		}
		this.asource.PlayOneShot(this.aclip[4]);
	}

	// Token: 0x0600000C RID: 12 RVA: 0x000021EF File Offset: 0x000003EF
	public void KnifeInit()
	{
	}

	// Token: 0x0600000D RID: 13 RVA: 0x000021F4 File Offset: 0x000003F4
	public void SwapItem(bool swapToKnife)
	{
		if (swapToKnife)
		{
			this.bookrender.SetActive(false);
			this.knifeRender.SetActive(true);
			this.KnifeInit();
			return;
		}
		this.bookrender.SetActive(true);
		this.knifeRender.SetActive(false);
		this.BookInit();
	}

	// Token: 0x0600000F RID: 15 RVA: 0x0000224C File Offset: 0x0000044C
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyAiMAgeBookAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyAiMAgeBookAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ServerFlipPage_3316948804));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversFlipPage_3316948804));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_ServerMakeIcebox_1934289915));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversMakeIcebox_1934289915));
	}

	// Token: 0x06000010 RID: 16 RVA: 0x000022C6 File Offset: 0x000004C6
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateAiMAgeBookAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateAiMAgeBookAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000011 RID: 17 RVA: 0x000022D9 File Offset: 0x000004D9
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000012 RID: 18 RVA: 0x000022E8 File Offset: 0x000004E8
	private void RpcWriter___Server_ServerFlipPage_3316948804(int pg)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(pg);
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000013 RID: 19 RVA: 0x0000235A File Offset: 0x0000055A
	private void RpcLogic___ServerFlipPage_3316948804(int pg)
	{
		this.ObserversFlipPage(pg);
	}

	// Token: 0x06000014 RID: 20 RVA: 0x00002364 File Offset: 0x00000564
	private void RpcReader___Server_ServerFlipPage_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerFlipPage_3316948804(num);
	}

	// Token: 0x06000015 RID: 21 RVA: 0x00002398 File Offset: 0x00000598
	private void RpcWriter___Observers_ObserversFlipPage_3316948804(int pgn)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(pgn);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000016 RID: 22 RVA: 0x0000241C File Offset: 0x0000061C
	private void RpcLogic___ObserversFlipPage_3316948804(int pgn)
	{
		this.asource.PlayOneShot(this.aclip[Random.Range(0, 4)]);
		if (pgn == 4)
		{
			this.animaes[1].SetBool("pgflip", false);
			this.animaes[2].SetBool("pgflip", false);
			this.animaes[3].SetBool("pgflip", false);
			this.animaes[4].SetBool("pgflip", false);
			base.StartCoroutine(this.Lerppos(this.pages[3], 0.09f));
			base.StartCoroutine(this.Lerppos(this.pages[2], 0.08f));
			base.StartCoroutine(this.Lerppos(this.pages[1], 0.06f));
			return;
		}
		if (pgn == 3)
		{
			this.animaes[1].SetBool("pgflip", false);
			this.animaes[2].SetBool("pgflip", false);
			this.animaes[3].SetBool("pgflip", false);
			this.animaes[4].SetBool("pgflip", true);
			base.StartCoroutine(this.Lerppos(this.pages[3], 0.04f));
			base.StartCoroutine(this.Lerppos(this.pages[2], 0.08f));
			base.StartCoroutine(this.Lerppos(this.pages[1], 0.04f));
			base.StartCoroutine(this.Lerppos(this.pages[0], 0f));
			return;
		}
		if (pgn == 2)
		{
			this.animaes[1].SetBool("pgflip", false);
			this.animaes[2].SetBool("pgflip", false);
			this.animaes[3].SetBool("pgflip", true);
			this.animaes[4].SetBool("pgflip", true);
			base.StartCoroutine(this.Lerppos(this.pages[3], 0.04f));
			base.StartCoroutine(this.Lerppos(this.pages[2], 0.06f));
			base.StartCoroutine(this.Lerppos(this.pages[1], 0.06f));
			return;
		}
		if (pgn == 1)
		{
			this.animaes[1].SetBool("pgflip", false);
			this.animaes[2].SetBool("pgflip", true);
			this.animaes[3].SetBool("pgflip", true);
			this.animaes[4].SetBool("pgflip", true);
			base.StartCoroutine(this.Lerppos(this.pages[3], 0.04f));
			base.StartCoroutine(this.Lerppos(this.pages[2], 0.06f));
			base.StartCoroutine(this.Lerppos(this.pages[1], 0.08f));
		}
	}

	// Token: 0x06000017 RID: 23 RVA: 0x000026E4 File Offset: 0x000008E4
	private void RpcReader___Observers_ObserversFlipPage_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversFlipPage_3316948804(num);
	}

	// Token: 0x06000018 RID: 24 RVA: 0x00002718 File Offset: 0x00000918
	private void RpcWriter___Server_ServerMakeIcebox_1934289915(GameObject player)
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

	// Token: 0x06000019 RID: 25 RVA: 0x0000278A File Offset: 0x0000098A
	private void RpcLogic___ServerMakeIcebox_1934289915(GameObject player)
	{
		this.ObserversMakeIcebox(player);
	}

	// Token: 0x0600001A RID: 26 RVA: 0x00002794 File Offset: 0x00000994
	private void RpcReader___Server_ServerMakeIcebox_1934289915(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerMakeIcebox_1934289915(gameObject);
	}

	// Token: 0x0600001B RID: 27 RVA: 0x000027C8 File Offset: 0x000009C8
	private void RpcWriter___Observers_ObserversMakeIcebox_1934289915(GameObject Player)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(Player);
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600001C RID: 28 RVA: 0x00002849 File Offset: 0x00000A49
	private void RpcLogic___ObserversMakeIcebox_1934289915(GameObject Player)
	{
		Player.GetComponent<PlayerMovement>().SummonIceBox(0, null);
	}

	// Token: 0x0600001D RID: 29 RVA: 0x00002858 File Offset: 0x00000A58
	private void RpcReader___Observers_ObserversMakeIcebox_1934289915(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversMakeIcebox_1934289915(gameObject);
	}

	// Token: 0x0600001E RID: 30 RVA: 0x000022D9 File Offset: 0x000004D9
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000001 RID: 1
	public Animator[] animaes;

	// Token: 0x04000002 RID: 2
	public Transform[] pages;

	// Token: 0x04000003 RID: 3
	public GameObject bookrender;

	// Token: 0x04000004 RID: 4
	public LayerMask layerMask;

	// Token: 0x04000005 RID: 5
	public GameObject knifeRender;

	// Token: 0x04000006 RID: 6
	public AudioSource asource;

	// Token: 0x04000007 RID: 7
	public AudioClip[] aclip;

	// Token: 0x04000008 RID: 8
	private bool NetworkInitialize___EarlyAiMAgeBookAssembly-CSharp.dll_Excuted;

	// Token: 0x04000009 RID: 9
	private bool NetworkInitialize__LateAiMAgeBookAssembly-CSharp.dll_Excuted;
}
