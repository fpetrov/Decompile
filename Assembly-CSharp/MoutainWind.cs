using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x020000EA RID: 234
public class MoutainWind : NetworkBehaviour
{
	// Token: 0x06000991 RID: 2449 RVA: 0x0002505C File Offset: 0x0002325C
	public override void OnStartClient()
	{
		base.OnStartClient();
		if (base.HasAuthority)
		{
			base.StartCoroutine(this.WindRoutine());
		}
	}

	// Token: 0x06000992 RID: 2450 RVA: 0x0002507C File Offset: 0x0002327C
	private void Update()
	{
		if (this.plyrmov != null && (this.plyrmov.isDead || Vector3.Distance(this.plyrmov.transform.position, base.transform.position) > 50f))
		{
			this.plyrmov = null;
		}
	}

	// Token: 0x06000993 RID: 2451 RVA: 0x000250D2 File Offset: 0x000232D2
	private IEnumerator WindRoutine()
	{
		while (base.isActiveAndEnabled)
		{
			if (this.plyrmov != null && (this.plyrmov.isDead || Vector3.Distance(this.plyrmov.transform.position, base.transform.position) > 50f))
			{
				this.plyrmov = null;
			}
			float num = (float)Random.Range(6, 9);
			float num2 = Random.Range(2.3f, 2.85f);
			int num3 = Random.Range(0, 3);
			this.ServerWind(num, num2, num3);
			yield return new WaitForSeconds((float)Random.Range(13, 20));
		}
		yield break;
	}

	// Token: 0x06000994 RID: 2452 RVA: 0x000250E1 File Offset: 0x000232E1
	[ServerRpc(RequireOwnership = false)]
	private void ServerWind(float time, float strength, int direction)
	{
		this.RpcWriter___Server_ServerWind_2484002398(time, strength, direction);
	}

	// Token: 0x06000995 RID: 2453 RVA: 0x000250F5 File Offset: 0x000232F5
	[ObserversRpc]
	private void ObsWind(float time, float strength, int direction)
	{
		this.RpcWriter___Observers_ObsWind_2484002398(time, strength, direction);
	}

	// Token: 0x06000996 RID: 2454 RVA: 0x00025109 File Offset: 0x00023309
	private IEnumerator ActualWind(float time, float strength, int direction)
	{
		this.MountainSource.volume = strength / 6f;
		this.MountainSource.PlayOneShot(this.Gust);
		while (time > 0f)
		{
			if (this.plyrmov != null)
			{
				this.plyrmov.MountainWind(this.Points[direction], strength);
			}
			time -= Time.deltaTime;
			yield return null;
		}
		while (this.MountainSource.volume > 0f)
		{
			this.MountainSource.volume -= Time.deltaTime;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000998 RID: 2456 RVA: 0x00025130 File Offset: 0x00023330
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyMoutainWindAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyMoutainWindAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ServerWind_2484002398));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_ObsWind_2484002398));
	}

	// Token: 0x06000999 RID: 2457 RVA: 0x0002517C File Offset: 0x0002337C
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateMoutainWindAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateMoutainWindAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x0600099A RID: 2458 RVA: 0x0002518F File Offset: 0x0002338F
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0600099B RID: 2459 RVA: 0x000251A0 File Offset: 0x000233A0
	private void RpcWriter___Server_ServerWind_2484002398(float time, float strength, int direction)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteSingle(time);
		pooledWriter.WriteSingle(strength);
		pooledWriter.WriteInt32(direction);
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x0002522C File Offset: 0x0002342C
	private void RpcLogic___ServerWind_2484002398(float time, float strength, int direction)
	{
		this.ObsWind(time, strength, direction);
	}

	// Token: 0x0600099D RID: 2461 RVA: 0x00025238 File Offset: 0x00023438
	private void RpcReader___Server_ServerWind_2484002398(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		float num = PooledReader0.ReadSingle();
		float num2 = PooledReader0.ReadSingle();
		int num3 = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerWind_2484002398(num, num2, num3);
	}

	// Token: 0x0600099E RID: 2462 RVA: 0x0002528C File Offset: 0x0002348C
	private void RpcWriter___Observers_ObsWind_2484002398(float time, float strength, int direction)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteSingle(time);
		pooledWriter.WriteSingle(strength);
		pooledWriter.WriteInt32(direction);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x00025327 File Offset: 0x00023527
	private void RpcLogic___ObsWind_2484002398(float time, float strength, int direction)
	{
		base.StartCoroutine(this.ActualWind(time, strength, direction));
	}

	// Token: 0x060009A0 RID: 2464 RVA: 0x0002533C File Offset: 0x0002353C
	private void RpcReader___Observers_ObsWind_2484002398(PooledReader PooledReader0, Channel channel)
	{
		float num = PooledReader0.ReadSingle();
		float num2 = PooledReader0.ReadSingle();
		int num3 = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsWind_2484002398(num, num2, num3);
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x0002518F File Offset: 0x0002338F
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000513 RID: 1299
	public PlayerMovement plyrmov;

	// Token: 0x04000514 RID: 1300
	public Transform[] Points;

	// Token: 0x04000515 RID: 1301
	public AudioSource MountainSource;

	// Token: 0x04000516 RID: 1302
	public AudioClip Gust;

	// Token: 0x04000517 RID: 1303
	private bool NetworkInitialize___EarlyMoutainWindAssembly-CSharp.dll_Excuted;

	// Token: 0x04000518 RID: 1304
	private bool NetworkInitialize__LateMoutainWindAssembly-CSharp.dll_Excuted;
}
