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

// Token: 0x02000084 RID: 132
public class FlagController : NetworkBehaviour
{
	// Token: 0x06000579 RID: 1401 RVA: 0x00015348 File Offset: 0x00013548
	public override void OnStartClient()
	{
		base.OnStartClient();
		this.controlTeam = 2;
		this.prevconteam = 2f;
		this.teamsInside = new List<int>();
		this.playersInside = new List<PlayerMovement>();
		this.controlTimer = 0f;
		this.hasbeencappedonce = false;
		base.StartCoroutine(this.WaitforRM());
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x000153A2 File Offset: 0x000135A2
	private IEnumerator WaitforRM()
	{
		GameObject NIM = null;
		while (NIM == null)
		{
			NIM = GameObject.FindGameObjectWithTag("NetItemManager");
			yield return null;
		}
		this.RM = NIM.GetComponent<ResourceManager>();
		this.RM.Flags.Add(base.gameObject.GetComponent<FlagController>());
		if (!base.HasAuthority)
		{
			base.gameObject.GetComponent<FlagController>().enabled = false;
			yield break;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("hex");
		for (int i = 0; i < array.Length; i++)
		{
			ChestNetController chestNetController;
			if (array[i].TryGetComponent<ChestNetController>(out chestNetController))
			{
				if (chestNetController.TeamNum == 0)
				{
					this.Team1Chest = chestNetController;
				}
				else if (chestNetController.TeamNum == 2)
				{
					this.Team2Chest = chestNetController;
				}
			}
		}
		base.StartCoroutine(this.FlagRoutine());
		this.flagvisual.material = this.flagMats[0];
		this.RM.updatehex(this.FlagResourceID, -1);
		yield break;
	}

	// Token: 0x0600057B RID: 1403 RVA: 0x000153B4 File Offset: 0x000135B4
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
				this.playersInside.Add(getPlayerGameobject.player.GetComponent<PlayerMovement>());
			}
		}
	}

	// Token: 0x0600057C RID: 1404 RVA: 0x0001543C File Offset: 0x0001363C
	public void removeplayers(PlayerMovement playmov)
	{
		if (this.teamsInside.Contains(playmov.playerTeam))
		{
			this.teamsInside.Remove(playmov.playerTeam);
		}
		if (this.playersInside.Contains(playmov))
		{
			this.playersInside.Remove(playmov);
			if (this.playersInside.Contains(playmov))
			{
				this.playersInside.Remove(playmov);
				if (this.teamsInside.Contains(playmov.playerTeam))
				{
					this.teamsInside.Remove(playmov.playerTeam);
				}
			}
		}
	}

	// Token: 0x0600057D RID: 1405 RVA: 0x000154CC File Offset: 0x000136CC
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerMovement playerMovement;
			GetPlayerGameobject getPlayerGameobject;
			if (other.gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
			{
				if (this.teamsInside.Contains(playerMovement.playerTeam))
				{
					this.teamsInside.Remove(playerMovement.playerTeam);
				}
				if (this.playersInside.Contains(playerMovement))
				{
					this.playersInside.Remove(playerMovement);
					return;
				}
			}
			else if (other.gameObject.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
			{
				if (this.teamsInside.Contains(getPlayerGameobject.player.GetComponent<PlayerMovement>().playerTeam))
				{
					this.teamsInside.Remove(getPlayerGameobject.player.GetComponent<PlayerMovement>().playerTeam);
				}
				if (this.playersInside.Contains(getPlayerGameobject.player.GetComponent<PlayerMovement>()))
				{
					this.playersInside.Remove(getPlayerGameobject.player.GetComponent<PlayerMovement>());
				}
			}
		}
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x000155B3 File Offset: 0x000137B3
	private void setfm(int val)
	{
		this.setFlagMat(val);
	}

	// Token: 0x0600057F RID: 1407 RVA: 0x000155BC File Offset: 0x000137BC
	[ServerRpc(RequireOwnership = false)]
	private void setFlagMat(int matnum)
	{
		this.RpcWriter___Server_setFlagMat_3316948804(matnum);
	}

	// Token: 0x06000580 RID: 1408 RVA: 0x000155C8 File Offset: 0x000137C8
	[ObserversRpc]
	private void SetFlagMatObs(int matnum)
	{
		this.RpcWriter___Observers_SetFlagMatObs_3316948804(matnum);
	}

	// Token: 0x06000581 RID: 1409 RVA: 0x000155DF File Offset: 0x000137DF
	private IEnumerator playsoundrot(int id)
	{
		if (this.announcer == null)
		{
			this.announcer = GameObject.FindGameObjectWithTag("announcer").GetComponent<AudioSource>();
		}
		while (this.announcer != null && this.announcer.isPlaying)
		{
			yield return null;
		}
		int random = Random.Range(0, 25);
		while (random > 0)
		{
			int num = random;
			random = num - 1;
			yield return null;
		}
		while (this.announcer != null && this.announcer.isPlaying)
		{
			yield return null;
		}
		if (id > 0 && this.announcer != null)
		{
			this.announcer.PlayOneShot(this.Announcerclips[id - 1]);
		}
		yield break;
	}

	// Token: 0x06000582 RID: 1410 RVA: 0x000155F5 File Offset: 0x000137F5
	private IEnumerator FlagRoutine()
	{
		while (base.isActiveAndEnabled)
		{
			yield return null;
			if (this.controlTeam == 0)
			{
				this.flagvisual.material != this.flagMats[1];
				this.t0timer += Time.deltaTime;
				this.t2timer = 0f;
				if (this.t0timer > 30f)
				{
					if (base.HasAuthority && this.LackySpawnPos != null)
					{
						this.Team1Chest.SpawnLacky(this.FlagResourceID, this.LackySpawnPos.position);
					}
					this.t0timer = 0f;
				}
			}
			else if (this.controlTeam == 1)
			{
				this.flagvisual.material != this.flagMats[2];
				this.t2timer += Time.deltaTime;
				this.t0timer = 0f;
				if (this.t2timer > 30f)
				{
					if (base.HasAuthority && this.LackySpawnPos != null)
					{
						this.Team2Chest.SpawnLacky(this.FlagResourceID, this.LackySpawnPos.position);
					}
					this.t2timer = 0f;
				}
			}
			else
			{
				this.t0timer = 0f;
				this.t2timer = 0f;
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
								this.setfm(1);
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
								this.setfm(0);
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
								this.setfm(2);
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
								this.setfm(0);
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
		yield break;
	}

	// Token: 0x06000583 RID: 1411 RVA: 0x00015604 File Offset: 0x00013804
	[ServerRpc(RequireOwnership = false)]
	private void SyncFlagVal(int val)
	{
		this.RpcWriter___Server_SyncFlagVal_3316948804(val);
	}

	// Token: 0x06000584 RID: 1412 RVA: 0x00015610 File Offset: 0x00013810
	[ObserversRpc]
	private void ObsSyncFlagVal(int val)
	{
		this.RpcWriter___Observers_ObsSyncFlagVal_3316948804(val);
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x0001561C File Offset: 0x0001381C
	[ServerRpc(RequireOwnership = false)]
	private void ChangeFlagAniFalse()
	{
		this.RpcWriter___Server_ChangeFlagAniFalse_2166136261();
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x00015624 File Offset: 0x00013824
	[ObserversRpc]
	private void ChangeFlagAniFalseObs()
	{
		this.RpcWriter___Observers_ChangeFlagAniFalseObs_2166136261();
	}

	// Token: 0x06000587 RID: 1415 RVA: 0x0001562C File Offset: 0x0001382C
	[ServerRpc(RequireOwnership = false)]
	private void ChangeFlagAniTrue()
	{
		this.RpcWriter___Server_ChangeFlagAniTrue_2166136261();
	}

	// Token: 0x06000588 RID: 1416 RVA: 0x00015634 File Offset: 0x00013834
	[ObserversRpc]
	private void ChangeFlagAniTrueObs()
	{
		this.RpcWriter___Observers_ChangeFlagAniTrueObs_2166136261();
	}

	// Token: 0x0600058A RID: 1418 RVA: 0x0001566C File Offset: 0x0001386C
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyFlagControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyFlagControllerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_setFlagMat_3316948804));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_SetFlagMatObs_3316948804));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_SyncFlagVal_3316948804));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ObsSyncFlagVal_3316948804));
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_ChangeFlagAniFalse_2166136261));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_ChangeFlagAniFalseObs_2166136261));
		base.RegisterServerRpc(6U, new ServerRpcDelegate(this.RpcReader___Server_ChangeFlagAniTrue_2166136261));
		base.RegisterObserversRpc(7U, new ClientRpcDelegate(this.RpcReader___Observers_ChangeFlagAniTrueObs_2166136261));
	}

	// Token: 0x0600058B RID: 1419 RVA: 0x00015742 File Offset: 0x00013942
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateFlagControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateFlagControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x0600058C RID: 1420 RVA: 0x00015755 File Offset: 0x00013955
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0600058D RID: 1421 RVA: 0x00015764 File Offset: 0x00013964
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

	// Token: 0x0600058E RID: 1422 RVA: 0x000157D6 File Offset: 0x000139D6
	private void RpcLogic___setFlagMat_3316948804(int matnum)
	{
		this.SetFlagMatObs(matnum);
	}

	// Token: 0x0600058F RID: 1423 RVA: 0x000157E0 File Offset: 0x000139E0
	private void RpcReader___Server_setFlagMat_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___setFlagMat_3316948804(num);
	}

	// Token: 0x06000590 RID: 1424 RVA: 0x00015814 File Offset: 0x00013A14
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

	// Token: 0x06000591 RID: 1425 RVA: 0x00015898 File Offset: 0x00013A98
	private void RpcLogic___SetFlagMatObs_3316948804(int matnum)
	{
		this.flagvisual.material = this.flagMats[matnum];
		if (this.flagvisual.material != this.flagMats[matnum] && (matnum == 1 || matnum == 2))
		{
			this.FlagAudio.PlayOneShot(this.flagcap);
			this.RM.updatehex(this.FlagResourceID, matnum);
			foreach (ParticleSystem particleSystem in this.particles)
			{
				particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
				particleSystem.Play();
			}
		}
		if (this.playSoundOnInitialCapture || this.hasbeencappedonce)
		{
			base.StartCoroutine(this.playsoundrot(matnum));
		}
		this.hasbeencappedonce = true;
	}

	// Token: 0x06000592 RID: 1426 RVA: 0x00015948 File Offset: 0x00013B48
	private void RpcReader___Observers_SetFlagMatObs_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___SetFlagMatObs_3316948804(num);
	}

	// Token: 0x06000593 RID: 1427 RVA: 0x0001597C File Offset: 0x00013B7C
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

	// Token: 0x06000594 RID: 1428 RVA: 0x000159EE File Offset: 0x00013BEE
	private void RpcLogic___SyncFlagVal_3316948804(int val)
	{
		this.ObsSyncFlagVal(val);
	}

	// Token: 0x06000595 RID: 1429 RVA: 0x000159F8 File Offset: 0x00013BF8
	private void RpcReader___Server_SyncFlagVal_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___SyncFlagVal_3316948804(num);
	}

	// Token: 0x06000596 RID: 1430 RVA: 0x00015A2C File Offset: 0x00013C2C
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

	// Token: 0x06000597 RID: 1431 RVA: 0x00015AAD File Offset: 0x00013CAD
	private void RpcLogic___ObsSyncFlagVal_3316948804(int val)
	{
		this.controlTeam = val;
	}

	// Token: 0x06000598 RID: 1432 RVA: 0x00015AB8 File Offset: 0x00013CB8
	private void RpcReader___Observers_ObsSyncFlagVal_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsSyncFlagVal_3316948804(num);
	}

	// Token: 0x06000599 RID: 1433 RVA: 0x00015AEC File Offset: 0x00013CEC
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

	// Token: 0x0600059A RID: 1434 RVA: 0x00015B51 File Offset: 0x00013D51
	private void RpcLogic___ChangeFlagAniFalse_2166136261()
	{
		this.ChangeFlagAniFalseObs();
	}

	// Token: 0x0600059B RID: 1435 RVA: 0x00015B5C File Offset: 0x00013D5C
	private void RpcReader___Server_ChangeFlagAniFalse_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ChangeFlagAniFalse_2166136261();
	}

	// Token: 0x0600059C RID: 1436 RVA: 0x00015B7C File Offset: 0x00013D7C
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

	// Token: 0x0600059D RID: 1437 RVA: 0x00015BF0 File Offset: 0x00013DF0
	private void RpcLogic___ChangeFlagAniFalseObs_2166136261()
	{
		this.FlagAni.SetBool("raise", false);
		this.FlagAudio.volume = 0.03f;
	}

	// Token: 0x0600059E RID: 1438 RVA: 0x00015C14 File Offset: 0x00013E14
	private void RpcReader___Observers_ChangeFlagAniFalseObs_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ChangeFlagAniFalseObs_2166136261();
	}

	// Token: 0x0600059F RID: 1439 RVA: 0x00015C34 File Offset: 0x00013E34
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

	// Token: 0x060005A0 RID: 1440 RVA: 0x00015C99 File Offset: 0x00013E99
	private void RpcLogic___ChangeFlagAniTrue_2166136261()
	{
		this.ChangeFlagAniTrueObs();
	}

	// Token: 0x060005A1 RID: 1441 RVA: 0x00015CA4 File Offset: 0x00013EA4
	private void RpcReader___Server_ChangeFlagAniTrue_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ChangeFlagAniTrue_2166136261();
	}

	// Token: 0x060005A2 RID: 1442 RVA: 0x00015CC4 File Offset: 0x00013EC4
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

	// Token: 0x060005A3 RID: 1443 RVA: 0x00015D38 File Offset: 0x00013F38
	private void RpcLogic___ChangeFlagAniTrueObs_2166136261()
	{
		this.FlagAni.SetBool("raise", true);
		this.FlagAudio.volume = 0.15f;
	}

	// Token: 0x060005A4 RID: 1444 RVA: 0x00015D5C File Offset: 0x00013F5C
	private void RpcReader___Observers_ChangeFlagAniTrueObs_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ChangeFlagAniTrueObs_2166136261();
	}

	// Token: 0x060005A5 RID: 1445 RVA: 0x00015755 File Offset: 0x00013955
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x040002AD RID: 685
	public Animator FlagAni;

	// Token: 0x040002AE RID: 686
	public int controlTeam = 2;

	// Token: 0x040002AF RID: 687
	private float t0timer;

	// Token: 0x040002B0 RID: 688
	private float t2timer;

	// Token: 0x040002B1 RID: 689
	public List<int> teamsInside = new List<int>();

	// Token: 0x040002B2 RID: 690
	public List<PlayerMovement> playersInside = new List<PlayerMovement>();

	// Token: 0x040002B3 RID: 691
	public float controlTimer;

	// Token: 0x040002B4 RID: 692
	public int FlagResourceID;

	// Token: 0x040002B5 RID: 693
	private ResourceManager RM;

	// Token: 0x040002B6 RID: 694
	public AudioSource FlagAudio;

	// Token: 0x040002B7 RID: 695
	private ChestNetController Team2Chest;

	// Token: 0x040002B8 RID: 696
	private ChestNetController Team1Chest;

	// Token: 0x040002B9 RID: 697
	public Material[] flagMats;

	// Token: 0x040002BA RID: 698
	public SkinnedMeshRenderer flagvisual;

	// Token: 0x040002BB RID: 699
	public AudioClip flagcap;

	// Token: 0x040002BC RID: 700
	private float prevconteam = 2f;

	// Token: 0x040002BD RID: 701
	public ParticleSystem[] particles;

	// Token: 0x040002BE RID: 702
	public AudioClip[] Announcerclips;

	// Token: 0x040002BF RID: 703
	public bool playSoundOnInitialCapture;

	// Token: 0x040002C0 RID: 704
	private bool hasbeencappedonce;

	// Token: 0x040002C1 RID: 705
	public Transform LackySpawnPos;

	// Token: 0x040002C2 RID: 706
	private AudioSource announcer;

	// Token: 0x040002C3 RID: 707
	private bool NetworkInitialize___EarlyFlagControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x040002C4 RID: 708
	private bool NetworkInitialize__LateFlagControllerAssembly-CSharp.dll_Excuted;
}
