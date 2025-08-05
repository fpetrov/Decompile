using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x02000146 RID: 326
public class PortcullisNetController : NetworkBehaviour
{
	// Token: 0x06000E1A RID: 3610 RVA: 0x00039CA2 File Offset: 0x00037EA2
	public override void OnStartClient()
	{
		base.OnStartClient();
		base.StartCoroutine(this.ClosePortcullis());
	}

	// Token: 0x06000E1B RID: 3611 RVA: 0x00039CB7 File Offset: 0x00037EB7
	public void PortcullisInteract()
	{
		this.pisever();
	}

	// Token: 0x06000E1C RID: 3612 RVA: 0x00039CBF File Offset: 0x00037EBF
	[ServerRpc(RequireOwnership = false)]
	private void pisever()
	{
		this.RpcWriter___Server_pisever_2166136261();
	}

	// Token: 0x06000E1D RID: 3613 RVA: 0x00039CC7 File Offset: 0x00037EC7
	[ObserversRpc]
	private void piObservers()
	{
		this.RpcWriter___Observers_piObservers_2166136261();
	}

	// Token: 0x06000E1E RID: 3614 RVA: 0x00039CCF File Offset: 0x00037ECF
	private IEnumerator OpenPortcullis()
	{
		this.asor.Stop();
		yield return null;
		this.isOpen = true;
		this.pcCol.enabled = false;
		this.navlink.SetActive(true);
		Vector3 startPosition = this.Portcullis.transform.localPosition;
		Vector3 targetPosition = new Vector3(0.03630615f, 0.1074f, 0.2051495f);
		float duration = 0.85f;
		float elapsedTime = 0f;
		this.asor.pitch = Random.Range(0.95f, 1.05f);
		this.asor.PlayOneShot(this.clis[0]);
		while (elapsedTime < duration)
		{
			this.Portcullis.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		this.pcCol.enabled = true;
		this.Portcullis.transform.localPosition = targetPosition;
		yield break;
	}

	// Token: 0x06000E1F RID: 3615 RVA: 0x00039CDE File Offset: 0x00037EDE
	private IEnumerator ClosePortcullis()
	{
		this.isOpen = false;
		this.pcCol.enabled = false;
		this.navlink.SetActive(false);
		Vector3 startPosition = this.Portcullis.transform.localPosition;
		Vector3 targetPosition = new Vector3(0.03630615f, 0.056f, 0.2051495f);
		float duration = 0.85f;
		float elapsedTime = 0f;
		this.asor.pitch = Random.Range(0.95f, 1.05f);
		this.asor.PlayOneShot(this.clis[1]);
		while (elapsedTime < duration)
		{
			this.Portcullis.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		this.pcCol.enabled = true;
		this.Portcullis.transform.localPosition = targetPosition;
		yield break;
	}

	// Token: 0x06000E20 RID: 3616 RVA: 0x00039CED File Offset: 0x00037EED
	public void LockPick(bool onoff)
	{
		this.ChestLockPickSound(onoff);
	}

	// Token: 0x06000E21 RID: 3617 RVA: 0x00039CF6 File Offset: 0x00037EF6
	[ServerRpc(RequireOwnership = false)]
	private void ChestLockPickSound(bool onoff)
	{
		this.RpcWriter___Server_ChestLockPickSound_1140765316(onoff);
	}

	// Token: 0x06000E22 RID: 3618 RVA: 0x00039D02 File Offset: 0x00037F02
	[ObserversRpc]
	private void ChestLockPickSoundObs(bool onoff)
	{
		this.RpcWriter___Observers_ChestLockPickSoundObs_1140765316(onoff);
	}

	// Token: 0x06000E24 RID: 3620 RVA: 0x00039D10 File Offset: 0x00037F10
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyPortcullisNetControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyPortcullisNetControllerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_pisever_2166136261));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_piObservers_2166136261));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_ChestLockPickSound_1140765316));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ChestLockPickSoundObs_1140765316));
	}

	// Token: 0x06000E25 RID: 3621 RVA: 0x00039D8A File Offset: 0x00037F8A
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LatePortcullisNetControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LatePortcullisNetControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000E26 RID: 3622 RVA: 0x00039D9D File Offset: 0x00037F9D
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000E27 RID: 3623 RVA: 0x00039DAC File Offset: 0x00037FAC
	private void RpcWriter___Server_pisever_2166136261()
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

	// Token: 0x06000E28 RID: 3624 RVA: 0x00039E11 File Offset: 0x00038011
	private void RpcLogic___pisever_2166136261()
	{
		this.piObservers();
	}

	// Token: 0x06000E29 RID: 3625 RVA: 0x00039E1C File Offset: 0x0003801C
	private void RpcReader___Server_pisever_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___pisever_2166136261();
	}

	// Token: 0x06000E2A RID: 3626 RVA: 0x00039E3C File Offset: 0x0003803C
	private void RpcWriter___Observers_piObservers_2166136261()
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

	// Token: 0x06000E2B RID: 3627 RVA: 0x00039EB0 File Offset: 0x000380B0
	private void RpcLogic___piObservers_2166136261()
	{
		if (this.isOpen)
		{
			base.StartCoroutine(this.ClosePortcullis());
			return;
		}
		base.StartCoroutine(this.OpenPortcullis());
	}

	// Token: 0x06000E2C RID: 3628 RVA: 0x00039ED8 File Offset: 0x000380D8
	private void RpcReader___Observers_piObservers_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___piObservers_2166136261();
	}

	// Token: 0x06000E2D RID: 3629 RVA: 0x00039EF8 File Offset: 0x000380F8
	private void RpcWriter___Server_ChestLockPickSound_1140765316(bool onoff)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteBoolean(onoff);
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000E2E RID: 3630 RVA: 0x00039F6A File Offset: 0x0003816A
	private void RpcLogic___ChestLockPickSound_1140765316(bool onoff)
	{
		this.ChestLockPickSoundObs(onoff);
	}

	// Token: 0x06000E2F RID: 3631 RVA: 0x00039F74 File Offset: 0x00038174
	private void RpcReader___Server_ChestLockPickSound_1140765316(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ChestLockPickSound_1140765316(flag);
	}

	// Token: 0x06000E30 RID: 3632 RVA: 0x00039FA8 File Offset: 0x000381A8
	private void RpcWriter___Observers_ChestLockPickSoundObs_1140765316(bool onoff)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteBoolean(onoff);
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000E31 RID: 3633 RVA: 0x0003A029 File Offset: 0x00038229
	private void RpcLogic___ChestLockPickSoundObs_1140765316(bool onoff)
	{
		if (onoff)
		{
			this.asor.Play();
			return;
		}
		this.asor.Stop();
	}

	// Token: 0x06000E32 RID: 3634 RVA: 0x0003A048 File Offset: 0x00038248
	private void RpcReader___Observers_ChestLockPickSoundObs_1140765316(PooledReader PooledReader0, Channel channel)
	{
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ChestLockPickSoundObs_1140765316(flag);
	}

	// Token: 0x06000E33 RID: 3635 RVA: 0x00039D9D File Offset: 0x00037F9D
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x040007BD RID: 1981
	public Transform Portcullis;

	// Token: 0x040007BE RID: 1982
	public bool isOpen;

	// Token: 0x040007BF RID: 1983
	public Collider pcCol;

	// Token: 0x040007C0 RID: 1984
	public GameObject navlink;

	// Token: 0x040007C1 RID: 1985
	public AudioSource asor;

	// Token: 0x040007C2 RID: 1986
	public AudioClip[] clis;

	// Token: 0x040007C3 RID: 1987
	private bool NetworkInitialize___EarlyPortcullisNetControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x040007C4 RID: 1988
	private bool NetworkInitialize__LatePortcullisNetControllerAssembly-CSharp.dll_Excuted;
}
