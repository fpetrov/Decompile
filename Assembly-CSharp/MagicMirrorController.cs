using System;
using System.Collections;
using Dissonance;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x020000CD RID: 205
public class MagicMirrorController : NetworkBehaviour, IInteractable
{
	// Token: 0x06000814 RID: 2068 RVA: 0x0001F3E0 File Offset: 0x0001D5E0
	private void Start()
	{
		Material[] materials = this.MirrorGlass.materials;
		materials[1].color = Color.black;
		this.MirrorGlass.materials = materials;
	}

	// Token: 0x06000815 RID: 2069 RVA: 0x0001F412 File Offset: 0x0001D612
	public string DisplayInteractUI(GameObject player)
	{
		return "Mirror mirror...";
	}

	// Token: 0x06000816 RID: 2070 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interact(GameObject player)
	{
	}

	// Token: 0x06000817 RID: 2071 RVA: 0x0001F419 File Offset: 0x0001D619
	public void ActivateMirror(VoiceBroadcastTrigger vbt, Transform player)
	{
		this.ServerStartMirror();
		if (!this.islocalMirrorInited && !this.isMirrorInited)
		{
			this.islocalMirrorInited = true;
			base.StartCoroutine(this.ActivatedMirror(vbt, player));
		}
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x0001F447 File Offset: 0x0001D647
	private IEnumerator ActivatedMirror(VoiceBroadcastTrigger vbt, Transform player)
	{
		float LeaveTimer = 0f;
		bool hasresponded = true;
		float animationTimer = 0f;
		yield return new WaitForSeconds(2f);
		while (LeaveTimer < 10f || animationTimer > 0f)
		{
			if (animationTimer > 0f)
			{
				animationTimer -= Time.deltaTime;
			}
			else if (vbt.IsTransmitting)
			{
				hasresponded = false;
			}
			else if (!hasresponded)
			{
				hasresponded = true;
				int num = Random.Range(0, 5);
				if (num == 0 || num == 1)
				{
					this.ServerPlayYesOrNo(true, false);
					animationTimer += this.MirrorManAni.GetCurrentAnimatorClipInfo(0)[0].clip.length;
				}
				else if (num == 2 || num == 3)
				{
					this.ServerPlayYesOrNo(false, false);
					animationTimer += this.MirrorManAni.GetCurrentAnimatorClipInfo(0)[0].clip.length;
				}
				else
				{
					this.ServerPlayYesOrNo(false, true);
					animationTimer += this.MirrorManAni.GetCurrentAnimatorClipInfo(0)[0].clip.length;
				}
			}
			else
			{
				LeaveTimer += Time.deltaTime;
			}
			if (Vector3.Distance(base.transform.position, player.position) > 15f)
			{
				LeaveTimer = 10f;
			}
			yield return null;
		}
		this.ServerMirrorManLeave();
		this.islocalMirrorInited = false;
		yield break;
	}

	// Token: 0x06000819 RID: 2073 RVA: 0x0001F464 File Offset: 0x0001D664
	[ServerRpc(RequireOwnership = false)]
	private void ServerMirrorManLeave()
	{
		this.RpcWriter___Server_ServerMirrorManLeave_2166136261();
	}

	// Token: 0x0600081A RID: 2074 RVA: 0x0001F46C File Offset: 0x0001D66C
	[ObserversRpc]
	private void ObsMirrorManLeave()
	{
		this.RpcWriter___Observers_ObsMirrorManLeave_2166136261();
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x0001F474 File Offset: 0x0001D674
	private IEnumerator FadeOutMan()
	{
		float timer = 0f;
		Material[] matas = this.MirrorGlass.materials;
		while (timer < 1f)
		{
			matas[1].color = Color.Lerp(Color.gray, Color.black, timer);
			this.MirrorGlass.materials = matas;
			yield return null;
			timer += Time.deltaTime;
		}
		matas[1].color = Color.black;
		this.MirrorGlass.materials = matas;
		this.MirrorCam.gameObject.SetActive(false);
		yield return new WaitForSeconds(2f);
		this.isMirrorInited = false;
		this.mirrorcol.enabled = true;
		yield break;
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x0001F483 File Offset: 0x0001D683
	[ServerRpc(RequireOwnership = false)]
	private void ServerStartMirror()
	{
		this.RpcWriter___Server_ServerStartMirror_2166136261();
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x0001F48C File Offset: 0x0001D68C
	[ObserversRpc]
	private void ObsStartMirror()
	{
		this.RpcWriter___Observers_ObsStartMirror_2166136261();
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x0001F49F File Offset: 0x0001D69F
	private IEnumerator FadeInMan()
	{
		float timer = 0f;
		Material[] matas = this.MirrorGlass.materials;
		while (timer < 1f)
		{
			matas[1].color = Color.Lerp(Color.black, Color.gray, timer);
			this.MirrorGlass.materials = matas;
			yield return null;
			timer += Time.deltaTime;
		}
		matas[1].color = Color.gray;
		this.MirrorGlass.materials = matas;
		yield break;
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x0001F4AE File Offset: 0x0001D6AE
	[ServerRpc(RequireOwnership = false)]
	private void ServerPlayYesOrNo(bool isYes, bool ew)
	{
		this.RpcWriter___Server_ServerPlayYesOrNo_2937493993(isYes, ew);
	}

	// Token: 0x06000820 RID: 2080 RVA: 0x0001F4C0 File Offset: 0x0001D6C0
	[ObserversRpc]
	private void ObsPlayYN(bool isYes, bool ew)
	{
		this.RpcWriter___Observers_ObsPlayYN_2937493993(isYes, ew);
	}

	// Token: 0x06000822 RID: 2082 RVA: 0x0001F4DC File Offset: 0x0001D6DC
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyMagicMirrorControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyMagicMirrorControllerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ServerMirrorManLeave_2166136261));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_ObsMirrorManLeave_2166136261));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_ServerStartMirror_2166136261));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ObsStartMirror_2166136261));
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_ServerPlayYesOrNo_2937493993));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_ObsPlayYN_2937493993));
	}

	// Token: 0x06000823 RID: 2083 RVA: 0x0001F584 File Offset: 0x0001D784
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateMagicMirrorControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateMagicMirrorControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x0001F597 File Offset: 0x0001D797
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x0001F5A8 File Offset: 0x0001D7A8
	private void RpcWriter___Server_ServerMirrorManLeave_2166136261()
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

	// Token: 0x06000826 RID: 2086 RVA: 0x0001F60D File Offset: 0x0001D80D
	private void RpcLogic___ServerMirrorManLeave_2166136261()
	{
		this.ObsMirrorManLeave();
	}

	// Token: 0x06000827 RID: 2087 RVA: 0x0001F618 File Offset: 0x0001D818
	private void RpcReader___Server_ServerMirrorManLeave_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerMirrorManLeave_2166136261();
	}

	// Token: 0x06000828 RID: 2088 RVA: 0x0001F638 File Offset: 0x0001D838
	private void RpcWriter___Observers_ObsMirrorManLeave_2166136261()
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

	// Token: 0x06000829 RID: 2089 RVA: 0x0001F6AC File Offset: 0x0001D8AC
	private void RpcLogic___ObsMirrorManLeave_2166136261()
	{
		this.MirrorManAni.Play("leave", 0, 0f);
		base.StartCoroutine(this.FadeOutMan());
		this.islocalMirrorInited = false;
	}

	// Token: 0x0600082A RID: 2090 RVA: 0x0001F6D8 File Offset: 0x0001D8D8
	private void RpcReader___Observers_ObsMirrorManLeave_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsMirrorManLeave_2166136261();
	}

	// Token: 0x0600082B RID: 2091 RVA: 0x0001F6F8 File Offset: 0x0001D8F8
	private void RpcWriter___Server_ServerStartMirror_2166136261()
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

	// Token: 0x0600082C RID: 2092 RVA: 0x0001F75D File Offset: 0x0001D95D
	private void RpcLogic___ServerStartMirror_2166136261()
	{
		this.ObsStartMirror();
	}

	// Token: 0x0600082D RID: 2093 RVA: 0x0001F768 File Offset: 0x0001D968
	private void RpcReader___Server_ServerStartMirror_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerStartMirror_2166136261();
	}

	// Token: 0x0600082E RID: 2094 RVA: 0x0001F788 File Offset: 0x0001D988
	private void RpcWriter___Observers_ObsStartMirror_2166136261()
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

	// Token: 0x0600082F RID: 2095 RVA: 0x0001F7FC File Offset: 0x0001D9FC
	private void RpcLogic___ObsStartMirror_2166136261()
	{
		if (!this.isMirrorInited)
		{
			this.islocalMirrorInited = true;
			this.mirrorcol.enabled = false;
			this.isMirrorInited = true;
			this.MirrorCam.gameObject.SetActive(true);
			this.MirrorManAni.Play("Intro", 0, 0f);
			this.asauce.PlayOneShot(this.mirrorclips[0]);
			base.StartCoroutine(this.FadeInMan());
		}
	}

	// Token: 0x06000830 RID: 2096 RVA: 0x0001F874 File Offset: 0x0001DA74
	private void RpcReader___Observers_ObsStartMirror_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsStartMirror_2166136261();
	}

	// Token: 0x06000831 RID: 2097 RVA: 0x0001F894 File Offset: 0x0001DA94
	private void RpcWriter___Server_ServerPlayYesOrNo_2937493993(bool isYes, bool ew)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteBoolean(isYes);
		pooledWriter.WriteBoolean(ew);
		base.SendServerRpc(4U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000832 RID: 2098 RVA: 0x0001F913 File Offset: 0x0001DB13
	private void RpcLogic___ServerPlayYesOrNo_2937493993(bool isYes, bool ew)
	{
		this.ObsPlayYN(isYes, ew);
	}

	// Token: 0x06000833 RID: 2099 RVA: 0x0001F920 File Offset: 0x0001DB20
	private void RpcReader___Server_ServerPlayYesOrNo_2937493993(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		bool flag = PooledReader0.ReadBoolean();
		bool flag2 = PooledReader0.ReadBoolean();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerPlayYesOrNo_2937493993(flag, flag2);
	}

	// Token: 0x06000834 RID: 2100 RVA: 0x0001F964 File Offset: 0x0001DB64
	private void RpcWriter___Observers_ObsPlayYN_2937493993(bool isYes, bool ew)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteBoolean(isYes);
		pooledWriter.WriteBoolean(ew);
		base.SendObserversRpc(5U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000835 RID: 2101 RVA: 0x0001F9F4 File Offset: 0x0001DBF4
	private void RpcLogic___ObsPlayYN_2937493993(bool isYes, bool ew)
	{
		if (isYes)
		{
			this.MirrorManAni.Play("yes", 0, 0f);
			this.asauce.PlayOneShot(this.mirrorclips[1]);
			return;
		}
		this.MirrorManAni.Play("no", 0, 0f);
		if (ew)
		{
			this.asauce.PlayOneShot(this.mirrorclips[3]);
			return;
		}
		this.asauce.PlayOneShot(this.mirrorclips[2]);
	}

	// Token: 0x06000836 RID: 2102 RVA: 0x0001FA70 File Offset: 0x0001DC70
	private void RpcReader___Observers_ObsPlayYN_2937493993(PooledReader PooledReader0, Channel channel)
	{
		bool flag = PooledReader0.ReadBoolean();
		bool flag2 = PooledReader0.ReadBoolean();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsPlayYN_2937493993(flag, flag2);
	}

	// Token: 0x06000837 RID: 2103 RVA: 0x0001F597 File Offset: 0x0001D797
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000418 RID: 1048
	public Animator MirrorManAni;

	// Token: 0x04000419 RID: 1049
	public MeshRenderer MirrorGlass;

	// Token: 0x0400041A RID: 1050
	public Transform MirrorCam;

	// Token: 0x0400041B RID: 1051
	public Collider mirrorcol;

	// Token: 0x0400041C RID: 1052
	public AudioClip[] mirrorclips;

	// Token: 0x0400041D RID: 1053
	public AudioSource asauce;

	// Token: 0x0400041E RID: 1054
	private bool isMirrorInited;

	// Token: 0x0400041F RID: 1055
	private bool islocalMirrorInited;

	// Token: 0x04000420 RID: 1056
	private bool NetworkInitialize___EarlyMagicMirrorControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x04000421 RID: 1057
	private bool NetworkInitialize__LateMagicMirrorControllerAssembly-CSharp.dll_Excuted;
}
