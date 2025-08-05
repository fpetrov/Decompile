using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x020001BE RID: 446
public class TutorialFlagController : NetworkBehaviour
{
	// Token: 0x0600126E RID: 4718 RVA: 0x0004DCDD File Offset: 0x0004BEDD
	public override void OnStartClient()
	{
		base.OnStartClient();
		base.StartCoroutine(this.FlagRoutine());
	}

	// Token: 0x0600126F RID: 4719 RVA: 0x0004DCF4 File Offset: 0x0004BEF4
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerMovement playerMovement;
			if (other.gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
			{
				this.teamsInside.Add(playerMovement.playerTeam);
				this.playersInside.Add(playerMovement);
				return;
			}
			GetPlayerGameobject getPlayerGameobject;
			if (other.gameObject.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
			{
				this.teamsInside.Add(getPlayerGameobject.player.GetComponent<PlayerMovement>().playerTeam);
				this.playersInside.Add(playerMovement);
			}
		}
	}

	// Token: 0x06001270 RID: 4720 RVA: 0x0004DD74 File Offset: 0x0004BF74
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerMovement playerMovement;
			if (other.gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
			{
				this.teamsInside.Remove(playerMovement.playerTeam);
				this.playersInside.Remove(playerMovement);
				return;
			}
			GetPlayerGameobject getPlayerGameobject;
			if (other.gameObject.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
			{
				this.teamsInside.Remove(getPlayerGameobject.player.GetComponent<PlayerMovement>().playerTeam);
				this.playersInside.Remove(playerMovement);
			}
		}
	}

	// Token: 0x06001271 RID: 4721 RVA: 0x0004DDF5 File Offset: 0x0004BFF5
	[ServerRpc(RequireOwnership = false)]
	private void setFlagMat(int matnum)
	{
		this.RpcWriter___Server_setFlagMat_3316948804(matnum);
	}

	// Token: 0x06001272 RID: 4722 RVA: 0x0004DE04 File Offset: 0x0004C004
	[ObserversRpc]
	private void SetFlagMatObs(int matnum)
	{
		this.RpcWriter___Observers_SetFlagMatObs_3316948804(matnum);
	}

	// Token: 0x06001273 RID: 4723 RVA: 0x0004DE1B File Offset: 0x0004C01B
	private IEnumerator FlagRoutine()
	{
		for (;;)
		{
			yield return null;
			if (this.controlTeam != 2)
			{
				break;
			}
			if (this.teamsInside.Count > 0)
			{
				int num = 0;
				int num2 = 0;
				foreach (int num3 in this.teamsInside)
				{
					if (num3 == 0)
					{
						num++;
					}
					else if (num3 == 2)
					{
						num2++;
					}
				}
				if (num <= 0 || num2 <= 0)
				{
					if (num > 0 && this.controlTeam == 2)
					{
						if (!this.FlagAni.GetBool("raise"))
						{
							this.FlagAni.SetBool("raise", true);
							this.ChangeFlagAniTrue();
						}
						if (this.controlTimer > -5f)
						{
							this.controlTimer -= Time.deltaTime;
						}
						else
						{
							this.controlTeam = 0;
							if (this.flagvisual.material != this.flagMats[1])
							{
								this.flagvisual.material = this.flagMats[1];
								this.setFlagMat(1);
							}
						}
					}
					else if (num > 0 && this.controlTeam == 1)
					{
						if (this.FlagAni.GetBool("raise"))
						{
							this.FlagAni.SetBool("raise", false);
							this.ChangeFlagAniFalse();
						}
						if (this.controlTimer > 0f)
						{
							this.controlTimer -= Time.deltaTime;
						}
						else
						{
							this.controlTeam = 2;
							if (this.flagvisual.material != this.flagMats[0])
							{
								this.flagvisual.material = this.flagMats[0];
								this.setFlagMat(0);
							}
						}
					}
					else if (num2 > 0 && this.controlTeam == 2)
					{
						if (!this.FlagAni.GetBool("raise"))
						{
							this.FlagAni.SetBool("raise", true);
							this.ChangeFlagAniTrue();
						}
						if (this.controlTimer < 5f)
						{
							this.controlTimer += Time.deltaTime;
						}
						else
						{
							this.controlTeam = 1;
							if (this.flagvisual.material != this.flagMats[2])
							{
								this.flagvisual.material = this.flagMats[2];
								this.setFlagMat(2);
							}
						}
					}
					else if (num2 > 0 && this.controlTeam == 0)
					{
						if (this.FlagAni.GetBool("raise"))
						{
							this.FlagAni.SetBool("raise", false);
							this.ChangeFlagAniFalse();
						}
						if (this.controlTimer < 0f)
						{
							this.controlTimer += Time.deltaTime;
						}
						else
						{
							this.controlTeam = 2;
							if (this.flagvisual.material != this.flagMats[0])
							{
								this.flagvisual.material = this.flagMats[0];
								this.setFlagMat(0);
							}
						}
					}
				}
			}
			else if (this.teamsInside.Count == 0 && this.controlTeam == 2)
			{
				if (this.controlTimer < 0f)
				{
					this.controlTimer += Time.deltaTime;
				}
				else if (this.controlTimer > 0f)
				{
					this.controlTimer -= Time.deltaTime;
				}
				if (this.FlagAni.GetBool("raise"))
				{
					this.FlagAni.SetBool("raise", false);
					this.ChangeFlagAniFalse();
				}
			}
			else if (this.teamsInside.Count == 0 && this.controlTeam == 1)
			{
				if (this.controlTimer < 5f)
				{
					this.controlTimer += Time.deltaTime;
				}
				if (!this.FlagAni.GetBool("raise"))
				{
					this.FlagAni.SetBool("raise", true);
					this.ChangeFlagAniTrue();
				}
			}
			else if (this.teamsInside.Count == 0 && this.controlTeam == 0)
			{
				if (this.controlTimer > -5f)
				{
					this.controlTimer -= Time.deltaTime;
				}
				if (!this.FlagAni.GetBool("raise"))
				{
					this.FlagAni.SetBool("raise", true);
					this.ChangeFlagAniTrue();
				}
			}
			if (this.prevconteam != (float)this.controlTeam)
			{
				this.SyncFlagVal(this.controlTeam);
			}
			this.prevconteam = (float)this.controlTeam;
		}
		this.ts.hasCapturedFlag = true;
		base.StartCoroutine(this.TurnOffSkull());
		this.FlagAudio.volume = 0f;
		Debug.Log("appaapsdpasd");
		yield break;
	}

	// Token: 0x06001274 RID: 4724 RVA: 0x0004DE2A File Offset: 0x0004C02A
	private IEnumerator TurnOffSkull()
	{
		float timer = 0f;
		this.Angelwings.SetActive(true);
		this.skull.SetActive(false);
		Material skullmat = this.Angelwings.GetComponent<Renderer>().materials[0];
		Material skullmat2 = this.Angelwings.GetComponent<Renderer>().materials[1];
		while (timer <= 1f)
		{
			skullmat.SetFloat("_AlphaRemapMax", timer / 3f);
			this.hdld2.intensity = Mathf.Lerp(0f, 100f, timer);
			skullmat2.SetFloat("_AlphaRemapMax", timer / 3f);
			timer += Time.deltaTime;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06001275 RID: 4725 RVA: 0x0004DE39 File Offset: 0x0004C039
	[ServerRpc(RequireOwnership = false)]
	private void SyncFlagVal(int val)
	{
		this.RpcWriter___Server_SyncFlagVal_3316948804(val);
	}

	// Token: 0x06001276 RID: 4726 RVA: 0x0004DE45 File Offset: 0x0004C045
	[ObserversRpc]
	private void ObsSyncFlagVal(int val)
	{
		this.RpcWriter___Observers_ObsSyncFlagVal_3316948804(val);
	}

	// Token: 0x06001277 RID: 4727 RVA: 0x0004DE51 File Offset: 0x0004C051
	[ServerRpc(RequireOwnership = false)]
	private void ChangeFlagAniFalse()
	{
		this.RpcWriter___Server_ChangeFlagAniFalse_2166136261();
	}

	// Token: 0x06001278 RID: 4728 RVA: 0x0004DE59 File Offset: 0x0004C059
	[ObserversRpc]
	private void ChangeFlagAniFalseObs()
	{
		this.RpcWriter___Observers_ChangeFlagAniFalseObs_2166136261();
	}

	// Token: 0x06001279 RID: 4729 RVA: 0x0004DE61 File Offset: 0x0004C061
	[ServerRpc(RequireOwnership = false)]
	private void ChangeFlagAniTrue()
	{
		this.RpcWriter___Server_ChangeFlagAniTrue_2166136261();
	}

	// Token: 0x0600127A RID: 4730 RVA: 0x0004DE69 File Offset: 0x0004C069
	[ObserversRpc]
	private void ChangeFlagAniTrueObs()
	{
		this.RpcWriter___Observers_ChangeFlagAniTrueObs_2166136261();
	}

	// Token: 0x0600127C RID: 4732 RVA: 0x0004DEA4 File Offset: 0x0004C0A4
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyTutorialFlagControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyTutorialFlagControllerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_setFlagMat_3316948804));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_SetFlagMatObs_3316948804));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_SyncFlagVal_3316948804));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ObsSyncFlagVal_3316948804));
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_ChangeFlagAniFalse_2166136261));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_ChangeFlagAniFalseObs_2166136261));
		base.RegisterServerRpc(6U, new ServerRpcDelegate(this.RpcReader___Server_ChangeFlagAniTrue_2166136261));
		base.RegisterObserversRpc(7U, new ClientRpcDelegate(this.RpcReader___Observers_ChangeFlagAniTrueObs_2166136261));
	}

	// Token: 0x0600127D RID: 4733 RVA: 0x0004DF7A File Offset: 0x0004C17A
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateTutorialFlagControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateTutorialFlagControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x0600127E RID: 4734 RVA: 0x0004DF8D File Offset: 0x0004C18D
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0600127F RID: 4735 RVA: 0x0004DF9C File Offset: 0x0004C19C
	private void RpcWriter___Server_setFlagMat_3316948804(int matnum)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(matnum);
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06001280 RID: 4736 RVA: 0x0004E00E File Offset: 0x0004C20E
	private void RpcLogic___setFlagMat_3316948804(int matnum)
	{
		this.SetFlagMatObs(matnum);
	}

	// Token: 0x06001281 RID: 4737 RVA: 0x0004E018 File Offset: 0x0004C218
	private void RpcReader___Server_setFlagMat_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___setFlagMat_3316948804(num);
	}

	// Token: 0x06001282 RID: 4738 RVA: 0x0004E04C File Offset: 0x0004C24C
	private void RpcWriter___Observers_SetFlagMatObs_3316948804(int matnum)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(matnum);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06001283 RID: 4739 RVA: 0x0004E0D0 File Offset: 0x0004C2D0
	private void RpcLogic___SetFlagMatObs_3316948804(int matnum)
	{
		this.flagvisual.material = this.flagMats[matnum];
		if (this.flagvisual.material != this.flagMats[matnum] && (matnum == 1 || matnum == 2))
		{
			this.FlagAudio.PlayOneShot(this.flagcap);
			foreach (ParticleSystem particleSystem in this.particles)
			{
				particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
				particleSystem.Play();
			}
		}
	}

	// Token: 0x06001284 RID: 4740 RVA: 0x0004E148 File Offset: 0x0004C348
	private void RpcReader___Observers_SetFlagMatObs_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___SetFlagMatObs_3316948804(num);
	}

	// Token: 0x06001285 RID: 4741 RVA: 0x0004E17C File Offset: 0x0004C37C
	private void RpcWriter___Server_SyncFlagVal_3316948804(int val)
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
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06001286 RID: 4742 RVA: 0x0004E1EE File Offset: 0x0004C3EE
	private void RpcLogic___SyncFlagVal_3316948804(int val)
	{
		this.ObsSyncFlagVal(val);
	}

	// Token: 0x06001287 RID: 4743 RVA: 0x0004E1F8 File Offset: 0x0004C3F8
	private void RpcReader___Server_SyncFlagVal_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___SyncFlagVal_3316948804(num);
	}

	// Token: 0x06001288 RID: 4744 RVA: 0x0004E22C File Offset: 0x0004C42C
	private void RpcWriter___Observers_ObsSyncFlagVal_3316948804(int val)
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
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06001289 RID: 4745 RVA: 0x0004E2AD File Offset: 0x0004C4AD
	private void RpcLogic___ObsSyncFlagVal_3316948804(int val)
	{
		this.controlTeam = val;
	}

	// Token: 0x0600128A RID: 4746 RVA: 0x0004E2B8 File Offset: 0x0004C4B8
	private void RpcReader___Observers_ObsSyncFlagVal_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsSyncFlagVal_3316948804(num);
	}

	// Token: 0x0600128B RID: 4747 RVA: 0x0004E2EC File Offset: 0x0004C4EC
	private void RpcWriter___Server_ChangeFlagAniFalse_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(4U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600128C RID: 4748 RVA: 0x0004E351 File Offset: 0x0004C551
	private void RpcLogic___ChangeFlagAniFalse_2166136261()
	{
		this.ChangeFlagAniFalseObs();
	}

	// Token: 0x0600128D RID: 4749 RVA: 0x0004E35C File Offset: 0x0004C55C
	private void RpcReader___Server_ChangeFlagAniFalse_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ChangeFlagAniFalse_2166136261();
	}

	// Token: 0x0600128E RID: 4750 RVA: 0x0004E37C File Offset: 0x0004C57C
	private void RpcWriter___Observers_ChangeFlagAniFalseObs_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(5U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600128F RID: 4751 RVA: 0x0004E3F0 File Offset: 0x0004C5F0
	private void RpcLogic___ChangeFlagAniFalseObs_2166136261()
	{
		this.FlagAni.SetBool("raise", false);
		this.FlagAudio.volume = 0f;
	}

	// Token: 0x06001290 RID: 4752 RVA: 0x0004E414 File Offset: 0x0004C614
	private void RpcReader___Observers_ChangeFlagAniFalseObs_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ChangeFlagAniFalseObs_2166136261();
	}

	// Token: 0x06001291 RID: 4753 RVA: 0x0004E434 File Offset: 0x0004C634
	private void RpcWriter___Server_ChangeFlagAniTrue_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(6U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06001292 RID: 4754 RVA: 0x0004E499 File Offset: 0x0004C699
	private void RpcLogic___ChangeFlagAniTrue_2166136261()
	{
		this.ChangeFlagAniTrueObs();
	}

	// Token: 0x06001293 RID: 4755 RVA: 0x0004E4A4 File Offset: 0x0004C6A4
	private void RpcReader___Server_ChangeFlagAniTrue_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ChangeFlagAniTrue_2166136261();
	}

	// Token: 0x06001294 RID: 4756 RVA: 0x0004E4C4 File Offset: 0x0004C6C4
	private void RpcWriter___Observers_ChangeFlagAniTrueObs_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(7U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06001295 RID: 4757 RVA: 0x0004E538 File Offset: 0x0004C738
	private void RpcLogic___ChangeFlagAniTrueObs_2166136261()
	{
		this.FlagAni.SetBool("raise", true);
		this.FlagAudio.volume = 0.15f;
	}

	// Token: 0x06001296 RID: 4758 RVA: 0x0004E55C File Offset: 0x0004C75C
	private void RpcReader___Observers_ChangeFlagAniTrueObs_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ChangeFlagAniTrueObs_2166136261();
	}

	// Token: 0x06001297 RID: 4759 RVA: 0x0004DF8D File Offset: 0x0004C18D
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000AAE RID: 2734
	public Animator FlagAni;

	// Token: 0x04000AAF RID: 2735
	public int controlTeam = 2;

	// Token: 0x04000AB0 RID: 2736
	private List<int> teamsInside = new List<int>();

	// Token: 0x04000AB1 RID: 2737
	private List<PlayerMovement> playersInside = new List<PlayerMovement>();

	// Token: 0x04000AB2 RID: 2738
	public float controlTimer;

	// Token: 0x04000AB3 RID: 2739
	public AudioSource FlagAudio;

	// Token: 0x04000AB4 RID: 2740
	public Material[] flagMats;

	// Token: 0x04000AB5 RID: 2741
	public SkinnedMeshRenderer flagvisual;

	// Token: 0x04000AB6 RID: 2742
	public AudioClip flagcap;

	// Token: 0x04000AB7 RID: 2743
	private float prevconteam = 2f;

	// Token: 0x04000AB8 RID: 2744
	public ParticleSystem[] particles;

	// Token: 0x04000AB9 RID: 2745
	public TutorialGoblin ts;

	// Token: 0x04000ABA RID: 2746
	public HDAdditionalLightData hdld2;

	// Token: 0x04000ABB RID: 2747
	public GameObject Angelwings;

	// Token: 0x04000ABC RID: 2748
	public GameObject skull;

	// Token: 0x04000ABD RID: 2749
	private bool NetworkInitialize___EarlyTutorialFlagControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x04000ABE RID: 2750
	private bool NetworkInitialize__LateTutorialFlagControllerAssembly-CSharp.dll_Excuted;
}
