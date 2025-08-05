using System;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x0200001E RID: 30
public class BounceMushroomManager : NetworkBehaviour
{
	// Token: 0x06000153 RID: 339 RVA: 0x000075EC File Offset: 0x000057EC
	public override void OnStartClient()
	{
		base.OnStartClient();
		if (!base.IsOwner)
		{
			base.gameObject.GetComponent<BounceMushroomManager>().enabled = false;
		}
		for (int i = 0; i < this.Mushrooms.Length; i++)
		{
			this.Mushrooms[i].mushID = i;
		}
	}

	// Token: 0x06000154 RID: 340 RVA: 0x00007639 File Offset: 0x00005839
	public void TriggerBottomBounceMush(int id)
	{
		this.ServerTriggerBottomMush(id);
	}

	// Token: 0x06000155 RID: 341 RVA: 0x00007642 File Offset: 0x00005842
	[ServerRpc(RequireOwnership = false)]
	private void ServerTriggerBottomMush(int mushID)
	{
		this.RpcWriter___Server_ServerTriggerBottomMush_3316948804(mushID);
	}

	// Token: 0x06000156 RID: 342 RVA: 0x0000764E File Offset: 0x0000584E
	[ObserversRpc]
	private void obsTriggerBottomMush(int mushID)
	{
		this.RpcWriter___Observers_obsTriggerBottomMush_3316948804(mushID);
	}

	// Token: 0x06000157 RID: 343 RVA: 0x0000765A File Offset: 0x0000585A
	public void TriggerMushAnimation(int mushID)
	{
		this.ServerTriggerMush(mushID);
	}

	// Token: 0x06000158 RID: 344 RVA: 0x00007663 File Offset: 0x00005863
	[ServerRpc(RequireOwnership = false)]
	private void ServerTriggerMush(int mushID)
	{
		this.RpcWriter___Server_ServerTriggerMush_3316948804(mushID);
	}

	// Token: 0x06000159 RID: 345 RVA: 0x0000766F File Offset: 0x0000586F
	[ObserversRpc]
	private void obsTriggerMush(int mushID)
	{
		this.RpcWriter___Observers_obsTriggerMush_3316948804(mushID);
	}

	// Token: 0x0600015B RID: 347 RVA: 0x0000767C File Offset: 0x0000587C
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyBounceMushroomManagerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyBounceMushroomManagerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ServerTriggerBottomMush_3316948804));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_obsTriggerBottomMush_3316948804));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_ServerTriggerMush_3316948804));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_obsTriggerMush_3316948804));
	}

	// Token: 0x0600015C RID: 348 RVA: 0x000076F6 File Offset: 0x000058F6
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateBounceMushroomManagerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateBounceMushroomManagerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x0600015D RID: 349 RVA: 0x00007709 File Offset: 0x00005909
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0600015E RID: 350 RVA: 0x00007718 File Offset: 0x00005918
	private void RpcWriter___Server_ServerTriggerBottomMush_3316948804(int mushID)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(mushID);
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600015F RID: 351 RVA: 0x0000778A File Offset: 0x0000598A
	private void RpcLogic___ServerTriggerBottomMush_3316948804(int mushID)
	{
		this.obsTriggerBottomMush(mushID);
	}

	// Token: 0x06000160 RID: 352 RVA: 0x00007794 File Offset: 0x00005994
	private void RpcReader___Server_ServerTriggerBottomMush_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerTriggerBottomMush_3316948804(num);
	}

	// Token: 0x06000161 RID: 353 RVA: 0x000077C8 File Offset: 0x000059C8
	private void RpcWriter___Observers_obsTriggerBottomMush_3316948804(int mushID)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(mushID);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000162 RID: 354 RVA: 0x00007849 File Offset: 0x00005A49
	private void RpcLogic___obsTriggerBottomMush_3316948804(int mushID)
	{
		this.bottomboingers[mushID].Dotheboing();
	}

	// Token: 0x06000163 RID: 355 RVA: 0x00007858 File Offset: 0x00005A58
	private void RpcReader___Observers_obsTriggerBottomMush_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___obsTriggerBottomMush_3316948804(num);
	}

	// Token: 0x06000164 RID: 356 RVA: 0x0000788C File Offset: 0x00005A8C
	private void RpcWriter___Server_ServerTriggerMush_3316948804(int mushID)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(mushID);
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000165 RID: 357 RVA: 0x000078FE File Offset: 0x00005AFE
	private void RpcLogic___ServerTriggerMush_3316948804(int mushID)
	{
		this.obsTriggerMush(mushID);
	}

	// Token: 0x06000166 RID: 358 RVA: 0x00007908 File Offset: 0x00005B08
	private void RpcReader___Server_ServerTriggerMush_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerTriggerMush_3316948804(num);
	}

	// Token: 0x06000167 RID: 359 RVA: 0x0000793C File Offset: 0x00005B3C
	private void RpcWriter___Observers_obsTriggerMush_3316948804(int mushID)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(mushID);
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000168 RID: 360 RVA: 0x000079BD File Offset: 0x00005BBD
	private void RpcLogic___obsTriggerMush_3316948804(int mushID)
	{
		this.Mushrooms[mushID].MushAnimations();
		this.Mushrooms[mushID].mushas.PlayOneShot(this.mushclips[Random.Range(0, this.mushclips.Length)]);
	}

	// Token: 0x06000169 RID: 361 RVA: 0x000079F4 File Offset: 0x00005BF4
	private void RpcReader___Observers_obsTriggerMush_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___obsTriggerMush_3316948804(num);
	}

	// Token: 0x0600016A RID: 362 RVA: 0x00007709 File Offset: 0x00005909
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x040000AB RID: 171
	public BounceShroomTrigger[] Mushrooms;

	// Token: 0x040000AC RID: 172
	public AudioClip[] mushclips;

	// Token: 0x040000AD RID: 173
	public MushroomBottomBoinger[] bottomboingers;

	// Token: 0x040000AE RID: 174
	private bool NetworkInitialize___EarlyBounceMushroomManagerAssembly-CSharp.dll_Excuted;

	// Token: 0x040000AF RID: 175
	private bool NetworkInitialize__LateBounceMushroomManagerAssembly-CSharp.dll_Excuted;
}
