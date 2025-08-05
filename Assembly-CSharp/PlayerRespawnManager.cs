using System;
using System.Collections;
using System.Collections.Generic;
using Dissonance;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000130 RID: 304
public class PlayerRespawnManager : NetworkBehaviour
{
	// Token: 0x06000D39 RID: 3385 RVA: 0x00035950 File Offset: 0x00033B50
	public void moveRespawnPointsColosseum()
	{
		this.iscolosseum = true;
		this.Anouncersource.transform.position = new Vector3(876f, 204.39f, 125f);
		this.RespawnPoints[0].position = new Vector3(903.03f, 65.59f, 121.4f);
		this.RespawnPoints[1].position = new Vector3(888.9f, 65.59f, 105.3f);
		this.RespawnPoints[2].position = new Vector3(859f, 65.59f, 102.4f);
		this.RespawnPoints[3].position = new Vector3(850.9f, 65.59f, 128.9f);
		this.RespawnPoints[4].position = new Vector3(863.1f, 65.59f, 146.3f);
		this.RespawnPoints[5].position = new Vector3(894.13f, 65.59f, 147.72f);
		Material material = this.covershite.material;
		this.covershite.gameObject.SetActive(true);
		material.SetFloat("_AlphaRemapMax", 0.55f);
	}

	// Token: 0x06000D3A RID: 3386 RVA: 0x00035A79 File Offset: 0x00033C79
	public void startcoliroutine()
	{
		base.StartCoroutine(this.WaitForGameToStartColosseum());
		base.StartCoroutine(this.WaitforPmov());
	}

	// Token: 0x06000D3B RID: 3387 RVA: 0x00035A95 File Offset: 0x00033C95
	private IEnumerator WaitforPmov()
	{
		while (this.pmv == null)
		{
			yield return null;
		}
		this.pmv.GetComponent<PlayerInventory>().initedInv = false;
		yield break;
	}

	// Token: 0x06000D3C RID: 3388 RVA: 0x00035AA4 File Offset: 0x00033CA4
	private IEnumerator WaitForGameToStartColosseum()
	{
		float timer = 0f;
		float starttime = 13f;
		GameObject[] array;
		while (timer < 13f)
		{
			array = this.digit1;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			this.digit2.SetActive(false);
			this.digit1[Mathf.Clamp((int)(starttime - timer + 1f) % 10, 0, 9)].SetActive(true);
			if ((int)(starttime - timer + 1f) / 10 != 0)
			{
				this.digit2.SetActive(true);
			}
			yield return null;
			timer += Time.deltaTime;
		}
		this.T1Flag.GetComponent<CastleFlagCapturedNotifier>().externalWingstoggle();
		if (base.HasAuthority)
		{
			yield return new WaitForSeconds(0.1f);
		}
		array = this.digit1;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		this.digit2.SetActive(false);
		this.pmv.canMove = true;
		this.pmv.canRecall = false;
		this.pmv.GetComponent<PlayerInventory>().initedInv = true;
		yield return new WaitForSeconds(1.5f);
		this.Anouncersource.volume = 1f;
		this.Anouncersource.PlayOneShot(this.starthorn);
		base.StartCoroutine(this.FadeOutVignette());
		yield return new WaitForSeconds(1f);
		timer = 0f;
		while (timer < 1f)
		{
			yield return null;
			timer += Time.deltaTime;
			this.Anouncersource.volume = Mathf.Lerp(1f, 0.2f, timer);
		}
		this.Anouncersource.volume = 0.2f;
		yield break;
	}

	// Token: 0x06000D3D RID: 3389 RVA: 0x00035AB3 File Offset: 0x00033CB3
	public void summonDeathMessage(string name, string causeofdeath, string killer)
	{
		this.summondm(name, causeofdeath, killer);
	}

	// Token: 0x06000D3E RID: 3390 RVA: 0x00035ABE File Offset: 0x00033CBE
	[ServerRpc(RequireOwnership = false)]
	private void summondm(string name, string causeofdeath, string killer)
	{
		this.RpcWriter___Server_summondm_852232071(name, causeofdeath, killer);
	}

	// Token: 0x06000D3F RID: 3391 RVA: 0x00035AD4 File Offset: 0x00033CD4
	[ObserversRpc]
	private void SpawnDethMsg(string name, string causeofdeath, string killer)
	{
		this.RpcWriter___Observers_SpawnDethMsg_852232071(name, causeofdeath, killer);
	}

	// Token: 0x06000D40 RID: 3392 RVA: 0x00035AF3 File Offset: 0x00033CF3
	private IEnumerator shit(GameObject deth)
	{
		CanvasGroup cg = deth.GetComponent<CanvasGroup>();
		cg.alpha = 0f;
		float timer = 0f;
		while (timer < 10f)
		{
			if (timer < 0.2f)
			{
				cg.alpha = Mathf.Lerp(0f, 1f, timer * 5f);
			}
			else if (timer > 9f)
			{
				cg.alpha = Mathf.Lerp(1f, 0f, 1f - (10f - timer));
			}
			int num = this.dethmsgs.IndexOf(deth);
			if (num >= 0 && num < 8)
			{
				deth.transform.localPosition = Vector3.Lerp(deth.transform.localPosition, new Vector3(deth.transform.localPosition.x, this.positions[num], deth.transform.localPosition.z), Time.deltaTime * 10f);
			}
			timer += Time.deltaTime;
			yield return null;
		}
		this.dethmsgs.Remove(deth);
		Object.Destroy(deth);
		yield break;
	}

	// Token: 0x06000D41 RID: 3393 RVA: 0x00035B09 File Offset: 0x00033D09
	public void ResetRSM()
	{
		this.DeadPlayers.Clear();
	}

	// Token: 0x06000D42 RID: 3394 RVA: 0x00035B18 File Offset: 0x00033D18
	public override void OnStartClient()
	{
		this.playerAvatars = new Dictionary<string, Texture2D>();
		if (base.HasAuthority)
		{
			base.StartCoroutine(this.FCUpdater());
		}
		this.covershite.gameObject.SetActive(false);
		this.Respawningin.SetActive(false);
		this.skull.SetActive(false);
		this.youdied.enabled = false;
		this.dethicons = new Dictionary<string, Texture2D>();
		for (int i = 0; i < Mathf.Min(this.deathnames.Length, this.deathicon.Length); i++)
		{
			this.dethicons[this.deathnames[i]] = this.deathicon[i];
		}
	}

	// Token: 0x06000D43 RID: 3395 RVA: 0x00035BC0 File Offset: 0x00033DC0
	public void unmuteaud()
	{
		this.vbt.IsMuted = false;
	}

	// Token: 0x06000D44 RID: 3396 RVA: 0x00035BCE File Offset: 0x00033DCE
	private IEnumerator FCUpdater()
	{
		while (base.isActiveAndEnabled)
		{
			if (this.T1Flag.controlTeam != this.T1FlagOwner)
			{
				this.UpdateFlagControllers(0, this.T1Flag.controlTeam);
				this.T1FlagOwner = this.T1Flag.controlTeam;
			}
			else if (this.T2Flag.controlTeam != this.T2FlagOwner)
			{
				this.UpdateFlagControllers(1, this.T2Flag.controlTeam);
				this.T2FlagOwner = this.T2Flag.controlTeam;
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000D45 RID: 3397 RVA: 0x00035BDD File Offset: 0x00033DDD
	[ServerRpc(RequireOwnership = false)]
	private void UpdateFlagControllers(int flagid, int val)
	{
		this.RpcWriter___Server_UpdateFlagControllers_1692629761(flagid, val);
	}

	// Token: 0x06000D46 RID: 3398 RVA: 0x00035BED File Offset: 0x00033DED
	[ObserversRpc]
	private void UpdateFlagControllersObs(int flagid, int val)
	{
		this.RpcWriter___Observers_UpdateFlagControllersObs_1692629761(flagid, val);
	}

	// Token: 0x06000D47 RID: 3399 RVA: 0x00035C00 File Offset: 0x00033E00
	public void IJustDied(PlayerMovement pm)
	{
		this.pmv = pm;
		this.RespawnTimer = 0f;
		this.Players = GameObject.FindGameObjectsWithTag("Player");
		base.StartCoroutine(this.FadeInShite());
		if (!this.iscolosseum)
		{
			base.StartCoroutine(this.SpectateRoutine());
		}
		else if (pm.level == 1)
		{
			pm.XPupdate();
			pm.XPupdate();
			pm.XPupdate();
		}
		else if (pm.level == 2)
		{
			pm.XPupdate();
			pm.XPupdate();
			pm.XPupdate();
			pm.XPupdate();
			pm.XPupdate();
			pm.XPupdate();
		}
		this.AddToDeadList(pm.gameObject, pm.playerTeam);
		this.vbt.IsMuted = true;
	}

	// Token: 0x06000D48 RID: 3400 RVA: 0x00035CBB File Offset: 0x00033EBB
	private IEnumerator SpectateRoutine()
	{
		Transform specpoint = this.pmv.SpectatePoint;
		bool hasclicked = false;
		while (!this.CanRespawn || (this.RespawnTimer < this.Respawntime - 0.5f && !this.gameover))
		{
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				hasclicked = true;
				for (int i = 0; i < this.Players.Length; i++)
				{
					if (this.CurrentPlayerIndex < this.Players.Length - 1)
					{
						this.CurrentPlayerIndex++;
						PlayerMovement playerMovement;
						if (this.Players[this.CurrentPlayerIndex] != null && this.Players[this.CurrentPlayerIndex].TryGetComponent<PlayerMovement>(out playerMovement) && playerMovement.playerTeam == this.pmv.playerTeam)
						{
							specpoint = playerMovement.SpectatePoint;
							break;
						}
					}
					else
					{
						this.CurrentPlayerIndex = 0;
						PlayerMovement playerMovement2;
						if (this.Players[this.CurrentPlayerIndex] != null && this.Players[this.CurrentPlayerIndex].TryGetComponent<PlayerMovement>(out playerMovement2) && playerMovement2.playerTeam == this.pmv.playerTeam)
						{
							specpoint = playerMovement2.SpectatePoint;
							break;
						}
					}
				}
			}
			yield return new WaitForEndOfFrame();
			if (hasclicked)
			{
				float num = Input.GetAxis("Mouse X") * this.sensitivity;
				float num2 = Input.GetAxis("Mouse Y") * this.sensitivity;
				this.pitch -= num2 / 2f;
				this.pitch = Mathf.Clamp(this.pitch, -60f, 60f);
				this.currentangle += num;
				float num3 = this.currentangle * 0.017453292f;
				Vector3 vector = new Vector3(Mathf.Cos(num3) * 5f, 0f, Mathf.Sin(num3) * 5f);
				Camera.main.transform.position = specpoint.position + Vector3.up * 3f + vector;
				Camera.main.transform.LookAt(specpoint.position);
				Camera.main.transform.localRotation *= Quaternion.Euler(this.pitch, 0f, 0f);
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000D49 RID: 3401 RVA: 0x00035CCA File Offset: 0x00033ECA
	[ServerRpc(RequireOwnership = false)]
	public void AddToDeadList(GameObject DeadGuy, int pteam)
	{
		this.RpcWriter___Server_AddToDeadList_2943392466(DeadGuy, pteam);
	}

	// Token: 0x06000D4A RID: 3402 RVA: 0x00035CDC File Offset: 0x00033EDC
	[ObserversRpc]
	public void AddToDeadListObs(GameObject DeadGuy, int pteam)
	{
		this.RpcWriter___Observers_AddToDeadListObs_2943392466(DeadGuy, pteam);
	}

	// Token: 0x06000D4B RID: 3403 RVA: 0x00035CF8 File Offset: 0x00033EF8
	private void PlayAnnouncerSound(int pteam)
	{
		int num = 0;
		int num2 = 0;
		foreach (GameObject gameObject in this.DeadPlayers)
		{
			if (gameObject != null)
			{
				if (gameObject.GetComponent<PlayerMovement>().playerTeam == 0)
				{
					num2++;
				}
				else if (gameObject.GetComponent<PlayerMovement>().playerTeam == 2)
				{
					num++;
				}
			}
		}
		if (pteam == 0)
		{
			if (num2 == 1)
			{
				this.ServerPAsound(0);
			}
			else if (num2 == 2)
			{
				this.ServerPAsound(1);
			}
			else if (num2 == 3)
			{
				this.ServerPAsound(2);
			}
			else if (num2 == 4)
			{
				this.ServerPAsound(3);
			}
		}
		else if (num == 1)
		{
			this.ServerPAsound(4);
		}
		else if (num == 2)
		{
			this.ServerPAsound(5);
		}
		else if (num == 3)
		{
			this.ServerPAsound(6);
		}
		else if (num == 4)
		{
			this.ServerPAsound(7);
		}
		if (this.iscolosseum)
		{
			this.DeadPlayers.Clear();
		}
	}

	// Token: 0x06000D4C RID: 3404 RVA: 0x00035DF4 File Offset: 0x00033FF4
	[ServerRpc(RequireOwnership = false)]
	private void ServerPAsound(int val)
	{
		this.RpcWriter___Server_ServerPAsound_3316948804(val);
	}

	// Token: 0x06000D4D RID: 3405 RVA: 0x00035E00 File Offset: 0x00034000
	[ObserversRpc]
	private void ObsPASound(int val)
	{
		this.RpcWriter___Observers_ObsPASound_3316948804(val);
	}

	// Token: 0x06000D4E RID: 3406 RVA: 0x00035E0C File Offset: 0x0003400C
	private IEnumerator PlayAnnouncerSoundRoutine(int clipid)
	{
		yield return new WaitForSeconds(this.ASoundTimer);
		if (!this.gameover)
		{
			this.Anouncersource.PlayOneShot(this.AnouncerDeathClips[clipid]);
			this.ASoundTimer += 3.5f;
			float timer = 3.5f;
			while (timer > 0f)
			{
				this.ASoundTimer -= Time.deltaTime;
				timer -= Time.deltaTime;
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x06000D4F RID: 3407 RVA: 0x00035E22 File Offset: 0x00034022
	private IEnumerator ColiRespawnRoutine()
	{
		if (this.lives == 1)
		{
			this.T1Flag.GetComponent<CastleFlagCapturedNotifier>().externalSkulltoggle();
		}
		float timer = 0f;
		GameObject[] array;
		while (timer < 3f)
		{
			yield return null;
			timer += Time.deltaTime;
			array = this.digit1;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			this.digit1[Mathf.Clamp((int)(3f - timer + 1f), 1, 3)].SetActive(true);
		}
		array = this.digit1;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		base.StartCoroutine(this.FadeOutVignette());
		base.StartCoroutine(this.TelePlayer(this.pmv.gameObject, Random.Range(0, this.RespawnPoints.Length)));
		this.pmv.RespawnPlayer();
		this.pmv.EnableCols();
		this.vbt.IsMuted = false;
		yield break;
	}

	// Token: 0x06000D50 RID: 3408 RVA: 0x00035E31 File Offset: 0x00034031
	private IEnumerator RespawnRoutine()
	{
		float timer = 0f;
		while ((!this.CanRespawn || this.RespawnTimer < this.Respawntime) && !this.gameover)
		{
			if (!this.gameover)
			{
				if (this.pmv.playerTeam == 0)
				{
					if (this.T1FlagOwner == 0 || this.T1FlagOwner == 2)
					{
						this.CanRespawn = true;
					}
					else
					{
						this.CanRespawn = false;
					}
				}
				else if (this.pmv.playerTeam == 2)
				{
					if (this.T2FlagOwner == 1 || this.T2FlagOwner == 2)
					{
						this.CanRespawn = true;
					}
					else
					{
						this.CanRespawn = false;
					}
				}
				if (!this.CanRespawn)
				{
					this.Respawningin.SetActive(true);
					this.skull.SetActive(true);
					GameObject[] array = this.digit1;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].SetActive(false);
					}
				}
				else
				{
					this.Respawningin.SetActive(true);
					this.skull.SetActive(false);
					GameObject[] array = this.digit1;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].SetActive(false);
					}
					this.digit1[Mathf.Clamp((int)(this.Respawntime - this.RespawnTimer + 1f), 1, 3)].SetActive(true);
				}
				this.RespawnTimer += Time.deltaTime / 1.25f;
			}
			if (Time.time - timer > 5f)
			{
				timer = Time.time;
				this.Servcheckend(this.pmv.playerTeam);
			}
			yield return null;
		}
		if (!this.gameover)
		{
			this.Respawningin.SetActive(false);
			this.skull.SetActive(false);
			GameObject[] array = this.digit1;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			base.StartCoroutine(this.FadeOutVignette());
			this.RemoveFromDeadList(this.pmv.gameObject);
			if (this.isSmallMap)
			{
				int num;
				if (this.pmv.playerTeam == 0)
				{
					num = Random.Range(2, 4);
				}
				else
				{
					num = Random.Range(0, 2);
				}
				base.StartCoroutine(this.TelePlayer(this.pmv.gameObject, num));
			}
			else
			{
				int num2;
				if (this.pmv.playerTeam == 0)
				{
					num2 = Random.Range(2, 5);
					if (num2 == 4)
					{
						num2 = 5;
					}
				}
				else
				{
					num2 = Random.Range(0, 3);
					if (num2 == 2)
					{
						num2 = 4;
					}
				}
				base.StartCoroutine(this.TelePlayer(this.pmv.gameObject, num2));
			}
			this.pmv.RespawnPlayer();
			this.pmv.EnableCols();
			this.vbt.IsMuted = false;
		}
		yield break;
	}

	// Token: 0x06000D51 RID: 3409 RVA: 0x00035E40 File Offset: 0x00034040
	[ServerRpc(RequireOwnership = false)]
	public void Servcheckend(int pteam)
	{
		this.RpcWriter___Server_Servcheckend_3316948804(pteam);
	}

	// Token: 0x06000D52 RID: 3410 RVA: 0x00035E4C File Offset: 0x0003404C
	[ObserversRpc]
	public void Obscheckend(int pteam)
	{
		this.RpcWriter___Observers_Obscheckend_3316948804(pteam);
	}

	// Token: 0x06000D53 RID: 3411 RVA: 0x00035E58 File Offset: 0x00034058
	[ServerRpc(RequireOwnership = false)]
	public void RemoveFromDeadList(GameObject AliveGuy)
	{
		this.RpcWriter___Server_RemoveFromDeadList_1934289915(AliveGuy);
	}

	// Token: 0x06000D54 RID: 3412 RVA: 0x00035E64 File Offset: 0x00034064
	[ObserversRpc]
	public void RemoveFromDeadListObs(GameObject AliveGuy)
	{
		this.RpcWriter___Observers_RemoveFromDeadListObs_1934289915(AliveGuy);
	}

	// Token: 0x06000D55 RID: 3413 RVA: 0x00035E70 File Offset: 0x00034070
	private IEnumerator TelePlayer(GameObject player, int respawnID)
	{
		this.ResizeHole(respawnID, GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MainMenuManager>().pm.gameObject);
		int num;
		for (int i = 0; i < 15; i = num + 1)
		{
			player.transform.position = this.RespawnPoints[respawnID].transform.position;
			yield return null;
			num = i;
		}
		yield return new WaitForSeconds(1f);
		yield break;
	}

	// Token: 0x06000D56 RID: 3414 RVA: 0x00035E8D File Offset: 0x0003408D
	[ServerRpc(RequireOwnership = false)]
	private void ResizeHole(int id, GameObject pobj)
	{
		this.RpcWriter___Server_ResizeHole_1011425610(id, pobj);
	}

	// Token: 0x06000D57 RID: 3415 RVA: 0x00035E9D File Offset: 0x0003409D
	[ObserversRpc]
	private void ResizeHoleObs(int id, GameObject pobj)
	{
		this.RpcWriter___Observers_ResizeHoleObs_1011425610(id, pobj);
	}

	// Token: 0x06000D58 RID: 3416 RVA: 0x00035EAD File Offset: 0x000340AD
	private IEnumerator FadeInShite()
	{
		if (this.iscolosseum)
		{
			this.lives--;
			if (this.lives == 0)
			{
				this.ServerEndGame(this.pmv.playerTeam);
				yield break;
			}
		}
		Material mata = this.covershite.material;
		this.covershite.gameObject.SetActive(true);
		float timer = 0f;
		while (timer < 1f && !this.gameover)
		{
			mata.SetFloat("_AlphaRemapMax", Mathf.Lerp(0f, 0.9f, timer));
			timer += Time.deltaTime;
			yield return null;
		}
		if (!this.gameover)
		{
			this.youdied.enabled = true;
		}
		timer = 0f;
		Material diedmat = this.youdied.material;
		while (timer < 2f && !this.gameover)
		{
			diedmat.SetFloat("_AlphaRemapMax", Mathf.Lerp(0f, 1f, timer / 2f));
			timer += Time.deltaTime;
			yield return null;
		}
		yield return new WaitForSeconds(1f);
		timer = 0f;
		while (timer <= 2f && !this.gameover)
		{
			diedmat.SetFloat("_AlphaRemapMax", Mathf.Lerp(1f, 0f, timer / 2f));
			timer += Time.deltaTime;
			yield return null;
		}
		this.youdied.enabled = false;
		if (this.iscolosseum)
		{
			base.StartCoroutine(this.ColiRespawnRoutine());
		}
		else
		{
			base.StartCoroutine(this.RespawnRoutine());
		}
		timer = 0f;
		while (timer < 1f && !this.gameover)
		{
			mata.SetFloat("_AlphaRemapMax", Mathf.Lerp(0.9f, 0.55f, timer));
			timer += Time.deltaTime;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000D59 RID: 3417 RVA: 0x00035EBC File Offset: 0x000340BC
	private IEnumerator FadeOutVignette()
	{
		Material mata = this.covershite.material;
		float timer = 0f;
		while (timer < 1f && !this.gameover)
		{
			mata.SetFloat("_AlphaRemapMax", Mathf.Lerp(0.55f, 0f, timer));
			timer += Time.deltaTime;
			yield return null;
		}
		this.covershite.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06000D5A RID: 3418 RVA: 0x00035ECC File Offset: 0x000340CC
	private void CheckIfGameShouldEnd(int playerteam)
	{
		if (playerteam == 0)
		{
			if (this.T1FlagOwner == 1)
			{
				this.Players = GameObject.FindGameObjectsWithTag("Player");
				bool flag = true;
				foreach (GameObject gameObject in this.Players)
				{
					PlayerMovement playerMovement;
					if (gameObject.TryGetComponent<PlayerMovement>(out playerMovement) && playerteam == playerMovement.playerTeam && !this.DeadPlayers.Contains(gameObject))
					{
						flag = false;
					}
				}
				if (flag)
				{
					this.ServerEndGame(playerteam);
					return;
				}
			}
		}
		else if (playerteam == 2 && this.T2FlagOwner == 0)
		{
			this.Players = GameObject.FindGameObjectsWithTag("Player");
			bool flag2 = true;
			foreach (GameObject gameObject2 in this.Players)
			{
				PlayerMovement playerMovement2;
				if (gameObject2.TryGetComponent<PlayerMovement>(out playerMovement2) && playerteam == playerMovement2.playerTeam && !this.DeadPlayers.Contains(gameObject2))
				{
					flag2 = false;
				}
			}
			if (flag2)
			{
				this.ServerEndGame(playerteam);
			}
		}
	}

	// Token: 0x06000D5B RID: 3419 RVA: 0x00035FAD File Offset: 0x000341AD
	[ServerRpc(RequireOwnership = false)]
	private void ServerEndGame(int losingteam)
	{
		this.RpcWriter___Server_ServerEndGame_3316948804(losingteam);
	}

	// Token: 0x06000D5C RID: 3420 RVA: 0x00035FB9 File Offset: 0x000341B9
	[ObserversRpc]
	private void ObsEndGame(int losingteam)
	{
		this.RpcWriter___Observers_ObsEndGame_3316948804(losingteam);
	}

	// Token: 0x06000D5D RID: 3421 RVA: 0x00035FC5 File Offset: 0x000341C5
	private IEnumerator DisableAnnouncerSource(int losingteam)
	{
		this.Anouncersource.enabled = false;
		yield return null;
		this.Anouncersource.enabled = true;
		this.Anouncersource.volume = 0.5f;
		this.Anouncersource.pitch = 0.95f;
		this.ASoundTimer += 100f;
		this.EndGame(losingteam);
		yield break;
	}

	// Token: 0x06000D5E RID: 3422 RVA: 0x00035FDC File Offset: 0x000341DC
	private void EndGame(int losingteam)
	{
		if (!this.gameover)
		{
			base.StartCoroutine(this.GetPlayerPfps());
			GameObject[] array = GameObject.FindGameObjectsWithTag("playbackprefab");
			for (int i = 0; i < array.Length; i++)
			{
				array[i].GetComponent<AudioSource>().maxDistance = 100000f;
			}
			this.gameover = true;
			base.StartCoroutine(this.FadeInVignette(losingteam));
			this.Respawningin.SetActive(false);
			array = this.digit1;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			this.skull.SetActive(false);
			if (losingteam == 0)
			{
				this.Anouncersource.PlayOneShot(this.gameclips[2]);
			}
			else
			{
				this.Anouncersource.PlayOneShot(this.gameclips[3]);
			}
			int num;
			SteamUserStats.GetStat("level", out num);
			SteamUserStats.SetStat("level", num + 1);
			if (this.pmv.playerTeam == losingteam)
			{
				int num2;
				SteamUserStats.GetStat("rank", out num2);
				SteamUserStats.SetStat("rank", num2 - 1);
				this.defeat.SetActive(true);
				this.asauce.volume = 0.3f;
				this.asauce.PlayOneShot(this.gameclips[0]);
				base.StartCoroutine(this.WaitforBanishClip(true));
			}
			else
			{
				int num3;
				SteamUserStats.GetStat("rank", out num3);
				SteamUserStats.SetStat("rank", num3 + 1);
				this.victory.SetActive(true);
				this.asauce.volume = 0.3f;
				this.asauce.pitch = 1f;
				this.asauce.PlayOneShot(this.gameclips[1]);
				base.StartCoroutine(this.WaitforBanishClip(false));
			}
			GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MainMenuManager>().turnoffinv();
		}
	}

	// Token: 0x06000D5F RID: 3423 RVA: 0x000361A2 File Offset: 0x000343A2
	private IEnumerator WaitforBanishClip(bool lost)
	{
		yield return new WaitForSeconds(4f);
		if (lost)
		{
			this.Anouncersource.PlayOneShot(this.gameclips[5]);
		}
		else
		{
			this.Anouncersource.PlayOneShot(this.gameclips[4]);
		}
		yield break;
	}

	// Token: 0x06000D60 RID: 3424 RVA: 0x000361B8 File Offset: 0x000343B8
	private IEnumerator FadeInVignette(int losingteam)
	{
		base.StartCoroutine(this.GetPlayerPfps());
		float timer = 0f;
		Material mata = this.covershite.material;
		this.covershite.gameObject.SetActive(true);
		this.scoreb.SetActive(true);
		CanvasGroup CG = this.scoreb.GetComponent<CanvasGroup>();
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;
		GameObject[] array;
		while (timer < 1f)
		{
			timer += Time.deltaTime;
			mata.SetFloat("_AlphaRemapMax", Mathf.Lerp(0f, 1f, timer));
			yield return null;
			this.Respawningin.SetActive(false);
			array = this.digit1;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			this.skull.SetActive(false);
		}
		if (this.Players == null)
		{
			this.Players = GameObject.FindGameObjectsWithTag("Player");
		}
		foreach (GameObject gameObject in this.Players)
		{
			PlayerMovement playerMovement;
			if (gameObject != null && gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
			{
				if (playerMovement.playerTeam == 0)
				{
					Transform child = this.scoreb.transform.GetChild(this.sorcerersset);
					child.GetChild(1).GetComponent<Text>().text = playerMovement.playername;
					if (this.playerAvatars.ContainsKey(playerMovement.playername))
					{
						child.GetChild(0).GetComponent<RawImage>().texture = this.playerAvatars[playerMovement.playername];
					}
					if (playerMovement.checkowner())
					{
						playerMovement.canMove = false;
						playerMovement.canJump = false;
						playerMovement.canMoveCamera = false;
						playerMovement.GetComponent<PlayerInventory>().canSwapItem = false;
						this.syncstats(this.sorcerersset);
					}
					if (this.killindex == this.sorcerersset)
					{
						child.GetChild(7).GetComponent<Text>().text = "Archmage";
					}
					else if (this.deathsindex == this.sorcerersset)
					{
						child.GetChild(7).GetComponent<Text>().text = "Lackey";
					}
					else if (this.distindex == this.sorcerersset)
					{
						child.GetChild(7).GetComponent<Text>().text = "Wanderer";
					}
					else if (this.soupindex == this.sorcerersset)
					{
						child.GetChild(7).GetComponent<Text>().text = "Glutton";
					}
					else if (this.spellsindex == this.sorcerersset)
					{
						child.GetChild(7).GetComponent<Text>().text = "Spellslinger";
					}
					else if (this.pipeindex == this.sorcerersset)
					{
						child.GetChild(7).GetComponent<Text>().text = "Dopey";
					}
					this.sorcerersset++;
				}
				else
				{
					Transform child2 = this.scoreb.transform.GetChild(this.warlocksset);
					child2.GetChild(1).GetComponent<Text>().text = playerMovement.playername;
					if (this.playerAvatars.ContainsKey(playerMovement.playername))
					{
						child2.GetChild(0).GetComponent<RawImage>().texture = this.playerAvatars[playerMovement.playername];
					}
					if (playerMovement.checkowner())
					{
						this.syncstats(this.warlocksset);
					}
					if (this.killindex == this.warlocksset)
					{
						child2.GetChild(7).GetComponent<Text>().text = "Archmage";
					}
					else if (this.deathsindex == this.warlocksset)
					{
						child2.GetChild(7).GetComponent<Text>().text = "Lackey";
					}
					else if (this.distindex == this.warlocksset)
					{
						child2.GetChild(7).GetComponent<Text>().text = "Wanderer";
					}
					else if (this.soupindex == this.warlocksset)
					{
						child2.GetChild(7).GetComponent<Text>().text = "Glutton";
					}
					else if (this.spellsindex == this.warlocksset)
					{
						child2.GetChild(7).GetComponent<Text>().text = "Spellslinger";
					}
					else if (this.pipeindex == this.warlocksset)
					{
						child2.GetChild(7).GetComponent<Text>().text = "Dopey";
					}
					this.warlocksset++;
				}
			}
		}
		yield return new WaitForSeconds(2.5f);
		this.sorcerersset = 0;
		this.warlocksset = 4;
		array = this.Players;
		for (int i = 0; i < array.Length; i++)
		{
			PlayerMovement playerMovement2;
			if (array[i].TryGetComponent<PlayerMovement>(out playerMovement2))
			{
				if (playerMovement2.playerTeam == 0)
				{
					Transform child3 = this.scoreb.transform.GetChild(this.sorcerersset);
					if (this.killindex == this.sorcerersset)
					{
						child3.GetChild(7).GetComponent<Text>().text = "Archmage";
					}
					else if (this.deathsindex == this.sorcerersset)
					{
						child3.GetChild(7).GetComponent<Text>().text = "Lackey";
					}
					else if (this.distindex == this.sorcerersset)
					{
						child3.GetChild(7).GetComponent<Text>().text = "Wanderer";
					}
					else if (this.soupindex == this.sorcerersset)
					{
						child3.GetChild(7).GetComponent<Text>().text = "Glutton";
					}
					else if (this.spellsindex == this.sorcerersset)
					{
						child3.GetChild(7).GetComponent<Text>().text = "Spellslinger";
					}
					else if (this.pipeindex == this.sorcerersset)
					{
						child3.GetChild(7).GetComponent<Text>().text = "Dopey";
					}
					this.sorcerersset++;
				}
				else
				{
					Transform child4 = this.scoreb.transform.GetChild(this.warlocksset);
					if (this.killindex == this.warlocksset)
					{
						child4.GetChild(7).GetComponent<Text>().text = "Archmage";
					}
					else if (this.deathsindex == this.warlocksset)
					{
						child4.GetChild(7).GetComponent<Text>().text = "Lackey";
					}
					else if (this.distindex == this.warlocksset)
					{
						child4.GetChild(7).GetComponent<Text>().text = "Wanderer";
					}
					else if (this.soupindex == this.warlocksset)
					{
						child4.GetChild(7).GetComponent<Text>().text = "Glutton";
					}
					else if (this.spellsindex == this.warlocksset)
					{
						child4.GetChild(7).GetComponent<Text>().text = "Spellslinger";
					}
					else if (this.pipeindex == this.warlocksset)
					{
						child4.GetChild(7).GetComponent<Text>().text = "Dopey";
					}
					this.warlocksset++;
				}
			}
		}
		for (timer = 0f; timer < 1f; timer += Time.deltaTime)
		{
			CG.alpha = Mathf.Lerp(0f, 1f, timer);
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000D61 RID: 3425 RVA: 0x000361C8 File Offset: 0x000343C8
	public void syncstats(int index)
	{
		this.SyncStatsRpc(index, this.pmv.kills, this.pmv.deaths, this.pmv.spellscasted, this.pmv.distanceTraveled, this.pmv.soupsdrank, this.pmv.pipessmoked);
	}

	// Token: 0x06000D62 RID: 3426 RVA: 0x0003621E File Offset: 0x0003441E
	[ServerRpc(RequireOwnership = false)]
	private void SyncStatsRpc(int index, int kills, int deaths, float spellscast, float distance, float soups, float pipes)
	{
		this.RpcWriter___Server_SyncStatsRpc_3934838300(index, kills, deaths, spellscast, distance, soups, pipes);
	}

	// Token: 0x06000D63 RID: 3427 RVA: 0x00036244 File Offset: 0x00034444
	[ObserversRpc]
	private void ObsSyncStats(int index, int kills, int deaths, float spellscast, float distance, float soups, float pipes)
	{
		this.RpcWriter___Observers_ObsSyncStats_3934838300(index, kills, deaths, spellscast, distance, soups, pipes);
	}

	// Token: 0x06000D64 RID: 3428 RVA: 0x00036273 File Offset: 0x00034473
	private IEnumerator GetPlayerPfps()
	{
		List<CSteamID> playerSteamIds = new List<CSteamID>();
		bool hasCompleted = false;
		CSteamID csteamID = (CSteamID)this.lobbyid;
		int numLobbyMembers = SteamMatchmaking.GetNumLobbyMembers(csteamID);
		for (int i = 0; i < numLobbyMembers; i++)
		{
			CSteamID lobbyMemberByIndex = SteamMatchmaking.GetLobbyMemberByIndex(csteamID, i);
			playerSteamIds.Add(lobbyMemberByIndex);
		}
		while (!hasCompleted)
		{
			foreach (CSteamID csteamID2 in playerSteamIds)
			{
				int largeFriendAvatar = SteamFriends.GetLargeFriendAvatar(csteamID2);
				if (largeFriendAvatar <= 0)
				{
					hasCompleted = false;
					break;
				}
				uint num;
				uint num2;
				SteamUtils.GetImageSize(largeFriendAvatar, out num, out num2);
				byte[] array = new byte[num * num2 * 4U];
				SteamUtils.GetImageRGBA(largeFriendAvatar, array, array.Length);
				Texture2D texture2D = new Texture2D((int)num, (int)num2, TextureFormat.RGBA32, false);
				texture2D.LoadRawTextureData(array);
				texture2D.Apply();
				string friendPersonaName = SteamFriends.GetFriendPersonaName(csteamID2);
				if (!this.playerAvatars.ContainsKey(friendPersonaName))
				{
					this.playerAvatars.Add(friendPersonaName, texture2D);
				}
				hasCompleted = true;
			}
			yield return new WaitForSeconds(0.5f);
		}
		yield break;
	}

	// Token: 0x06000D65 RID: 3429 RVA: 0x00036284 File Offset: 0x00034484
	public void GameLeft()
	{
		this.defeat.SetActive(false);
		this.victory.SetActive(false);
		this.scoreb.SetActive(false);
		this.covershite.gameObject.SetActive(false);
		this.Respawningin.SetActive(false);
		this.skull.SetActive(false);
		GameObject[] array = this.digit1;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		this.gameover = false;
		this.vbt.IsMuted = true;
	}

	// Token: 0x06000D67 RID: 3431 RVA: 0x00036418 File Offset: 0x00034618
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyPlayerRespawnManagerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyPlayerRespawnManagerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_summondm_852232071));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_SpawnDethMsg_852232071));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_UpdateFlagControllers_1692629761));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_UpdateFlagControllersObs_1692629761));
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_AddToDeadList_2943392466));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_AddToDeadListObs_2943392466));
		base.RegisterServerRpc(6U, new ServerRpcDelegate(this.RpcReader___Server_ServerPAsound_3316948804));
		base.RegisterObserversRpc(7U, new ClientRpcDelegate(this.RpcReader___Observers_ObsPASound_3316948804));
		base.RegisterServerRpc(8U, new ServerRpcDelegate(this.RpcReader___Server_Servcheckend_3316948804));
		base.RegisterObserversRpc(9U, new ClientRpcDelegate(this.RpcReader___Observers_Obscheckend_3316948804));
		base.RegisterServerRpc(10U, new ServerRpcDelegate(this.RpcReader___Server_RemoveFromDeadList_1934289915));
		base.RegisterObserversRpc(11U, new ClientRpcDelegate(this.RpcReader___Observers_RemoveFromDeadListObs_1934289915));
		base.RegisterServerRpc(12U, new ServerRpcDelegate(this.RpcReader___Server_ResizeHole_1011425610));
		base.RegisterObserversRpc(13U, new ClientRpcDelegate(this.RpcReader___Observers_ResizeHoleObs_1011425610));
		base.RegisterServerRpc(14U, new ServerRpcDelegate(this.RpcReader___Server_ServerEndGame_3316948804));
		base.RegisterObserversRpc(15U, new ClientRpcDelegate(this.RpcReader___Observers_ObsEndGame_3316948804));
		base.RegisterServerRpc(16U, new ServerRpcDelegate(this.RpcReader___Server_SyncStatsRpc_3934838300));
		base.RegisterObserversRpc(17U, new ClientRpcDelegate(this.RpcReader___Observers_ObsSyncStats_3934838300));
	}

	// Token: 0x06000D68 RID: 3432 RVA: 0x000365D4 File Offset: 0x000347D4
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LatePlayerRespawnManagerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LatePlayerRespawnManagerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000D69 RID: 3433 RVA: 0x000365E7 File Offset: 0x000347E7
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000D6A RID: 3434 RVA: 0x000365F8 File Offset: 0x000347F8
	private void RpcWriter___Server_summondm_852232071(string name, string causeofdeath, string killer)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteString(name);
		pooledWriter.WriteString(causeofdeath);
		pooledWriter.WriteString(killer);
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000D6B RID: 3435 RVA: 0x00036684 File Offset: 0x00034884
	private void RpcLogic___summondm_852232071(string name, string causeofdeath, string killer)
	{
		this.SpawnDethMsg(name, causeofdeath, killer);
	}

	// Token: 0x06000D6C RID: 3436 RVA: 0x00036690 File Offset: 0x00034890
	private void RpcReader___Server_summondm_852232071(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		string text = PooledReader0.ReadString();
		string text2 = PooledReader0.ReadString();
		string text3 = PooledReader0.ReadString();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___summondm_852232071(text, text2, text3);
	}

	// Token: 0x06000D6D RID: 3437 RVA: 0x000366E4 File Offset: 0x000348E4
	private void RpcWriter___Observers_SpawnDethMsg_852232071(string name, string causeofdeath, string killer)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteString(name);
		pooledWriter.WriteString(causeofdeath);
		pooledWriter.WriteString(killer);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000D6E RID: 3438 RVA: 0x00036780 File Offset: 0x00034980
	private void RpcLogic___SpawnDethMsg_852232071(string name, string causeofdeath, string killer)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.dethmsg, this.parentcanv);
		gameObject.transform.localPosition = new Vector3(909f, 520f, 0f);
		this.dethmsgs.Insert(0, gameObject);
		if (this.dethicons.ContainsKey(causeofdeath))
		{
			Debug.Log(causeofdeath);
			gameObject.GetComponent<KillFeedMessage>().setitup(name, this.dethicons[causeofdeath], killer);
		}
		else
		{
			gameObject.GetComponent<KillFeedMessage>().setitup(name, this.dethicons["none"], killer);
		}
		base.StartCoroutine(this.shit(gameObject));
	}

	// Token: 0x06000D6F RID: 3439 RVA: 0x00036828 File Offset: 0x00034A28
	private void RpcReader___Observers_SpawnDethMsg_852232071(PooledReader PooledReader0, Channel channel)
	{
		string text = PooledReader0.ReadString();
		string text2 = PooledReader0.ReadString();
		string text3 = PooledReader0.ReadString();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___SpawnDethMsg_852232071(text, text2, text3);
	}

	// Token: 0x06000D70 RID: 3440 RVA: 0x0003687C File Offset: 0x00034A7C
	private void RpcWriter___Server_UpdateFlagControllers_1692629761(int flagid, int val)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(flagid);
		pooledWriter.WriteInt32(val);
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000D71 RID: 3441 RVA: 0x000368FB File Offset: 0x00034AFB
	private void RpcLogic___UpdateFlagControllers_1692629761(int flagid, int val)
	{
		this.UpdateFlagControllersObs(flagid, val);
	}

	// Token: 0x06000D72 RID: 3442 RVA: 0x00036908 File Offset: 0x00034B08
	private void RpcReader___Server_UpdateFlagControllers_1692629761(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		int num2 = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___UpdateFlagControllers_1692629761(num, num2);
	}

	// Token: 0x06000D73 RID: 3443 RVA: 0x0003694C File Offset: 0x00034B4C
	private void RpcWriter___Observers_UpdateFlagControllersObs_1692629761(int flagid, int val)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(flagid);
		pooledWriter.WriteInt32(val);
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000D74 RID: 3444 RVA: 0x000369DA File Offset: 0x00034BDA
	private void RpcLogic___UpdateFlagControllersObs_1692629761(int flagid, int val)
	{
		if (flagid == 0)
		{
			this.T1FlagOwner = val;
			return;
		}
		if (flagid == 1)
		{
			this.T2FlagOwner = val;
		}
	}

	// Token: 0x06000D75 RID: 3445 RVA: 0x000369F4 File Offset: 0x00034BF4
	private void RpcReader___Observers_UpdateFlagControllersObs_1692629761(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		int num2 = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___UpdateFlagControllersObs_1692629761(num, num2);
	}

	// Token: 0x06000D76 RID: 3446 RVA: 0x00036A38 File Offset: 0x00034C38
	private void RpcWriter___Server_AddToDeadList_2943392466(GameObject DeadGuy, int pteam)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(DeadGuy);
		pooledWriter.WriteInt32(pteam);
		base.SendServerRpc(4U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000D77 RID: 3447 RVA: 0x00036AB7 File Offset: 0x00034CB7
	public void RpcLogic___AddToDeadList_2943392466(GameObject DeadGuy, int pteam)
	{
		this.AddToDeadListObs(DeadGuy, pteam);
	}

	// Token: 0x06000D78 RID: 3448 RVA: 0x00036AC4 File Offset: 0x00034CC4
	private void RpcReader___Server_AddToDeadList_2943392466(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___AddToDeadList_2943392466(gameObject, num);
	}

	// Token: 0x06000D79 RID: 3449 RVA: 0x00036B08 File Offset: 0x00034D08
	private void RpcWriter___Observers_AddToDeadListObs_2943392466(GameObject DeadGuy, int pteam)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(DeadGuy);
		pooledWriter.WriteInt32(pteam);
		base.SendObserversRpc(5U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000D7A RID: 3450 RVA: 0x00036B98 File Offset: 0x00034D98
	public void RpcLogic___AddToDeadListObs_2943392466(GameObject DeadGuy, int pteam)
	{
		if (base.HasAuthority)
		{
			this.DeadPlayers.Add(DeadGuy);
			this.PlayAnnouncerSound(pteam);
			if (!this.iscolosseum)
			{
				this.CheckIfGameShouldEnd(pteam);
			}
			GameObject[] array = GameObject.FindGameObjectsWithTag("hex");
			for (int i = 0; i < array.Length; i++)
			{
				FlagController flagController;
				PlayerMovement playerMovement;
				if (array[i].TryGetComponent<FlagController>(out flagController) && DeadGuy.TryGetComponent<PlayerMovement>(out playerMovement))
				{
					flagController.removeplayers(playerMovement);
				}
			}
		}
	}

	// Token: 0x06000D7B RID: 3451 RVA: 0x00036C08 File Offset: 0x00034E08
	private void RpcReader___Observers_AddToDeadListObs_2943392466(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___AddToDeadListObs_2943392466(gameObject, num);
	}

	// Token: 0x06000D7C RID: 3452 RVA: 0x00036C4C File Offset: 0x00034E4C
	private void RpcWriter___Server_ServerPAsound_3316948804(int val)
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
		base.SendServerRpc(6U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000D7D RID: 3453 RVA: 0x00036CBE File Offset: 0x00034EBE
	private void RpcLogic___ServerPAsound_3316948804(int val)
	{
		this.ObsPASound(val);
	}

	// Token: 0x06000D7E RID: 3454 RVA: 0x00036CC8 File Offset: 0x00034EC8
	private void RpcReader___Server_ServerPAsound_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerPAsound_3316948804(num);
	}

	// Token: 0x06000D7F RID: 3455 RVA: 0x00036CFC File Offset: 0x00034EFC
	private void RpcWriter___Observers_ObsPASound_3316948804(int val)
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
		base.SendObserversRpc(7U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000D80 RID: 3456 RVA: 0x00036D7D File Offset: 0x00034F7D
	private void RpcLogic___ObsPASound_3316948804(int val)
	{
		base.StartCoroutine(this.PlayAnnouncerSoundRoutine(val));
	}

	// Token: 0x06000D81 RID: 3457 RVA: 0x00036D90 File Offset: 0x00034F90
	private void RpcReader___Observers_ObsPASound_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsPASound_3316948804(num);
	}

	// Token: 0x06000D82 RID: 3458 RVA: 0x00036DC4 File Offset: 0x00034FC4
	private void RpcWriter___Server_Servcheckend_3316948804(int pteam)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(pteam);
		base.SendServerRpc(8U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000D83 RID: 3459 RVA: 0x00036E36 File Offset: 0x00035036
	public void RpcLogic___Servcheckend_3316948804(int pteam)
	{
		this.Obscheckend(pteam);
	}

	// Token: 0x06000D84 RID: 3460 RVA: 0x00036E40 File Offset: 0x00035040
	private void RpcReader___Server_Servcheckend_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___Servcheckend_3316948804(num);
	}

	// Token: 0x06000D85 RID: 3461 RVA: 0x00036E74 File Offset: 0x00035074
	private void RpcWriter___Observers_Obscheckend_3316948804(int pteam)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(pteam);
		base.SendObserversRpc(9U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000D86 RID: 3462 RVA: 0x00036EF5 File Offset: 0x000350F5
	public void RpcLogic___Obscheckend_3316948804(int pteam)
	{
		if (base.HasAuthority)
		{
			this.CheckIfGameShouldEnd(pteam);
		}
	}

	// Token: 0x06000D87 RID: 3463 RVA: 0x00036F08 File Offset: 0x00035108
	private void RpcReader___Observers_Obscheckend_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___Obscheckend_3316948804(num);
	}

	// Token: 0x06000D88 RID: 3464 RVA: 0x00036F3C File Offset: 0x0003513C
	private void RpcWriter___Server_RemoveFromDeadList_1934289915(GameObject AliveGuy)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(AliveGuy);
		base.SendServerRpc(10U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000D89 RID: 3465 RVA: 0x00036FAE File Offset: 0x000351AE
	public void RpcLogic___RemoveFromDeadList_1934289915(GameObject AliveGuy)
	{
		this.RemoveFromDeadListObs(AliveGuy);
	}

	// Token: 0x06000D8A RID: 3466 RVA: 0x00036FB8 File Offset: 0x000351B8
	private void RpcReader___Server_RemoveFromDeadList_1934289915(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___RemoveFromDeadList_1934289915(gameObject);
	}

	// Token: 0x06000D8B RID: 3467 RVA: 0x00036FEC File Offset: 0x000351EC
	private void RpcWriter___Observers_RemoveFromDeadListObs_1934289915(GameObject AliveGuy)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(AliveGuy);
		base.SendObserversRpc(11U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000D8C RID: 3468 RVA: 0x0003706D File Offset: 0x0003526D
	public void RpcLogic___RemoveFromDeadListObs_1934289915(GameObject AliveGuy)
	{
		if (base.HasAuthority && AliveGuy != null)
		{
			this.DeadPlayers.Remove(AliveGuy);
		}
	}

	// Token: 0x06000D8D RID: 3469 RVA: 0x00037090 File Offset: 0x00035290
	private void RpcReader___Observers_RemoveFromDeadListObs_1934289915(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___RemoveFromDeadListObs_1934289915(gameObject);
	}

	// Token: 0x06000D8E RID: 3470 RVA: 0x000370C4 File Offset: 0x000352C4
	private void RpcWriter___Server_ResizeHole_1011425610(int id, GameObject pobj)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(id);
		pooledWriter.WriteGameObject(pobj);
		base.SendServerRpc(12U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000D8F RID: 3471 RVA: 0x00037143 File Offset: 0x00035343
	private void RpcLogic___ResizeHole_1011425610(int id, GameObject pobj)
	{
		this.ResizeHoleObs(id, pobj);
	}

	// Token: 0x06000D90 RID: 3472 RVA: 0x00037150 File Offset: 0x00035350
	private void RpcReader___Server_ResizeHole_1011425610(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ResizeHole_1011425610(num, gameObject);
	}

	// Token: 0x06000D91 RID: 3473 RVA: 0x00037194 File Offset: 0x00035394
	private void RpcWriter___Observers_ResizeHoleObs_1011425610(int id, GameObject pobj)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(id);
		pooledWriter.WriteGameObject(pobj);
		base.SendObserversRpc(13U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000D92 RID: 3474 RVA: 0x00037222 File Offset: 0x00035422
	private void RpcLogic___ResizeHoleObs_1011425610(int id, GameObject pobj)
	{
		this.RespawnPoints[id].GetComponent<RespawnWormhole>().resizeWormhole();
		pobj.GetComponent<PlayerMovement>().playPortalSound();
	}

	// Token: 0x06000D93 RID: 3475 RVA: 0x00037244 File Offset: 0x00035444
	private void RpcReader___Observers_ResizeHoleObs_1011425610(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ResizeHoleObs_1011425610(num, gameObject);
	}

	// Token: 0x06000D94 RID: 3476 RVA: 0x00037288 File Offset: 0x00035488
	private void RpcWriter___Server_ServerEndGame_3316948804(int losingteam)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(losingteam);
		base.SendServerRpc(14U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000D95 RID: 3477 RVA: 0x000372FA File Offset: 0x000354FA
	private void RpcLogic___ServerEndGame_3316948804(int losingteam)
	{
		this.ObsEndGame(losingteam);
	}

	// Token: 0x06000D96 RID: 3478 RVA: 0x00037304 File Offset: 0x00035504
	private void RpcReader___Server_ServerEndGame_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerEndGame_3316948804(num);
	}

	// Token: 0x06000D97 RID: 3479 RVA: 0x00037338 File Offset: 0x00035538
	private void RpcWriter___Observers_ObsEndGame_3316948804(int losingteam)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(losingteam);
		base.SendObserversRpc(15U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000D98 RID: 3480 RVA: 0x000373B9 File Offset: 0x000355B9
	private void RpcLogic___ObsEndGame_3316948804(int losingteam)
	{
		base.StartCoroutine(this.DisableAnnouncerSource(losingteam));
	}

	// Token: 0x06000D99 RID: 3481 RVA: 0x000373CC File Offset: 0x000355CC
	private void RpcReader___Observers_ObsEndGame_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsEndGame_3316948804(num);
	}

	// Token: 0x06000D9A RID: 3482 RVA: 0x00037400 File Offset: 0x00035600
	private void RpcWriter___Server_SyncStatsRpc_3934838300(int index, int kills, int deaths, float spellscast, float distance, float soups, float pipes)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(index);
		pooledWriter.WriteInt32(kills);
		pooledWriter.WriteInt32(deaths);
		pooledWriter.WriteSingle(spellscast);
		pooledWriter.WriteSingle(distance);
		pooledWriter.WriteSingle(soups);
		pooledWriter.WriteSingle(pipes);
		base.SendServerRpc(16U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000D9B RID: 3483 RVA: 0x000374C0 File Offset: 0x000356C0
	private void RpcLogic___SyncStatsRpc_3934838300(int index, int kills, int deaths, float spellscast, float distance, float soups, float pipes)
	{
		this.ObsSyncStats(index, kills, deaths, spellscast, distance, soups, pipes);
	}

	// Token: 0x06000D9C RID: 3484 RVA: 0x000374D4 File Offset: 0x000356D4
	private void RpcReader___Server_SyncStatsRpc_3934838300(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		int num2 = PooledReader0.ReadInt32();
		int num3 = PooledReader0.ReadInt32();
		float num4 = PooledReader0.ReadSingle();
		float num5 = PooledReader0.ReadSingle();
		float num6 = PooledReader0.ReadSingle();
		float num7 = PooledReader0.ReadSingle();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___SyncStatsRpc_3934838300(num, num2, num3, num4, num5, num6, num7);
	}

	// Token: 0x06000D9D RID: 3485 RVA: 0x0003756C File Offset: 0x0003576C
	private void RpcWriter___Observers_ObsSyncStats_3934838300(int index, int kills, int deaths, float spellscast, float distance, float soups, float pipes)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(index);
		pooledWriter.WriteInt32(kills);
		pooledWriter.WriteInt32(deaths);
		pooledWriter.WriteSingle(spellscast);
		pooledWriter.WriteSingle(distance);
		pooledWriter.WriteSingle(soups);
		pooledWriter.WriteSingle(pipes);
		base.SendObserversRpc(17U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000D9E RID: 3486 RVA: 0x0003763C File Offset: 0x0003583C
	private void RpcLogic___ObsSyncStats_3934838300(int index, int kills, int deaths, float spellscast, float distance, float soups, float pipes)
	{
		Transform child = this.scoreb.transform.GetChild(index);
		child.GetChild(2).GetComponent<Text>().text = kills.ToString();
		child.GetChild(3).GetComponent<Text>().text = deaths.ToString();
		child.GetChild(4).GetComponent<Text>().text = spellscast.ToString();
		if (distance > 999999f)
		{
			distance = 999999f;
		}
		distance = (float)((int)distance);
		child.GetChild(5).GetComponent<Text>().text = distance.ToString() + "m";
		child.GetChild(6).GetComponent<Text>().text = soups.ToString();
		if (kills > 0 && kills > this.largestkills)
		{
			this.largestkills = kills;
			this.killindex = index;
		}
		if (deaths > 0 && (float)(kills / deaths) < this.worstkd)
		{
			this.worstkd = (float)(kills / deaths);
			this.deathsindex = index;
		}
		if (spellscast > this.largestspells)
		{
			this.largestspells = spellscast;
			this.spellsindex = index;
		}
		if (distance > this.largestdist)
		{
			this.largestdist = distance;
			this.distindex = index;
		}
		if (soups > 2f && soups > this.mostsoups)
		{
			this.mostsoups = soups;
			this.soupindex = index;
		}
		if (pipes > 0f && pipes > this.mostpipes)
		{
			this.mostpipes = pipes;
			this.pipeindex = index;
		}
	}

	// Token: 0x06000D9F RID: 3487 RVA: 0x000377A4 File Offset: 0x000359A4
	private void RpcReader___Observers_ObsSyncStats_3934838300(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		int num2 = PooledReader0.ReadInt32();
		int num3 = PooledReader0.ReadInt32();
		float num4 = PooledReader0.ReadSingle();
		float num5 = PooledReader0.ReadSingle();
		float num6 = PooledReader0.ReadSingle();
		float num7 = PooledReader0.ReadSingle();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsSyncStats_3934838300(num, num2, num3, num4, num5, num6, num7);
	}

	// Token: 0x06000DA0 RID: 3488 RVA: 0x000365E7 File Offset: 0x000347E7
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000715 RID: 1813
	public Transform[] RespawnPoints;

	// Token: 0x04000716 RID: 1814
	private bool CanRespawn = true;

	// Token: 0x04000717 RID: 1815
	public MeshRenderer youdied;

	// Token: 0x04000718 RID: 1816
	private GameObject[] Players;

	// Token: 0x04000719 RID: 1817
	private Transform PlayerToSpectate;

	// Token: 0x0400071A RID: 1818
	private int CurrentPlayerIndex;

	// Token: 0x0400071B RID: 1819
	private float RespawnTimer;

	// Token: 0x0400071C RID: 1820
	private float Respawntime = 3f;

	// Token: 0x0400071D RID: 1821
	public PlayerMovement pmv;

	// Token: 0x0400071E RID: 1822
	public List<GameObject> DeadPlayers = new List<GameObject>();

	// Token: 0x0400071F RID: 1823
	public FlagController T1Flag;

	// Token: 0x04000720 RID: 1824
	public FlagController T2Flag;

	// Token: 0x04000721 RID: 1825
	private int T1FlagOwner = -1;

	// Token: 0x04000722 RID: 1826
	private int T2FlagOwner = -1;

	// Token: 0x04000723 RID: 1827
	public GameObject[] digit1;

	// Token: 0x04000724 RID: 1828
	public GameObject digit2;

	// Token: 0x04000725 RID: 1829
	public GameObject skull;

	// Token: 0x04000726 RID: 1830
	public GameObject victory;

	// Token: 0x04000727 RID: 1831
	public GameObject defeat;

	// Token: 0x04000728 RID: 1832
	private bool gameover;

	// Token: 0x04000729 RID: 1833
	public Material happysorc;

	// Token: 0x0400072A RID: 1834
	public Material blank;

	// Token: 0x0400072B RID: 1835
	public GameObject Respawningin;

	// Token: 0x0400072C RID: 1836
	public GameObject scoreb;

	// Token: 0x0400072D RID: 1837
	public MeshRenderer covershite;

	// Token: 0x0400072E RID: 1838
	public GameObject[] PlayerScores;

	// Token: 0x0400072F RID: 1839
	private Vector3 scoreboardendpos = new Vector3(0f, 0.0002f, 0.137f);

	// Token: 0x04000730 RID: 1840
	public AudioSource asauce;

	// Token: 0x04000731 RID: 1841
	public AudioClip[] gameclips;

	// Token: 0x04000732 RID: 1842
	public VoiceBroadcastTrigger vbt;

	// Token: 0x04000733 RID: 1843
	public AudioSource Anouncersource;

	// Token: 0x04000734 RID: 1844
	public AudioClip[] AnouncerDeathClips;

	// Token: 0x04000735 RID: 1845
	private float ASoundTimer;

	// Token: 0x04000736 RID: 1846
	public ulong lobbyid;

	// Token: 0x04000737 RID: 1847
	private Dictionary<string, Texture2D> playerAvatars;

	// Token: 0x04000738 RID: 1848
	public Transform parentcanv;

	// Token: 0x04000739 RID: 1849
	public GameObject dethmsg;

	// Token: 0x0400073A RID: 1850
	private List<GameObject> dethmsgs = new List<GameObject>();

	// Token: 0x0400073B RID: 1851
	public string[] deathnames;

	// Token: 0x0400073C RID: 1852
	public Texture2D[] deathicon;

	// Token: 0x0400073D RID: 1853
	private Dictionary<string, Texture2D> dethicons;

	// Token: 0x0400073E RID: 1854
	private float[] positions = new float[] { 449f, 388.1f, 328.7f, 271.7f, 212.6f, 151.6f, 90.3f, 31.2f };

	// Token: 0x0400073F RID: 1855
	public float sensitivity = 2f;

	// Token: 0x04000740 RID: 1856
	public float clampAngle = 80f;

	// Token: 0x04000741 RID: 1857
	private float rotY;

	// Token: 0x04000742 RID: 1858
	private float rotX;

	// Token: 0x04000743 RID: 1859
	public bool isSmallMap;

	// Token: 0x04000744 RID: 1860
	private bool iscolosseum;

	// Token: 0x04000745 RID: 1861
	private int lives = 4;

	// Token: 0x04000746 RID: 1862
	public AudioClip starthorn;

	// Token: 0x04000747 RID: 1863
	private float currentangle;

	// Token: 0x04000748 RID: 1864
	private float pitch;

	// Token: 0x04000749 RID: 1865
	private int warlocksset = 4;

	// Token: 0x0400074A RID: 1866
	private int sorcerersset;

	// Token: 0x0400074B RID: 1867
	private int largestkills = -1;

	// Token: 0x0400074C RID: 1868
	private int killindex = -1;

	// Token: 0x0400074D RID: 1869
	private float worstkd = 10000000f;

	// Token: 0x0400074E RID: 1870
	private int deathsindex = -1;

	// Token: 0x0400074F RID: 1871
	private float largestspells = -1f;

	// Token: 0x04000750 RID: 1872
	private int spellsindex = -1;

	// Token: 0x04000751 RID: 1873
	private float largestdist = -1f;

	// Token: 0x04000752 RID: 1874
	private int distindex = -1;

	// Token: 0x04000753 RID: 1875
	private float mostsoups = -1f;

	// Token: 0x04000754 RID: 1876
	private int soupindex = -1;

	// Token: 0x04000755 RID: 1877
	private float mostpipes = -1f;

	// Token: 0x04000756 RID: 1878
	private int pipeindex = -1;

	// Token: 0x04000757 RID: 1879
	private bool NetworkInitialize___EarlyPlayerRespawnManagerAssembly-CSharp.dll_Excuted;

	// Token: 0x04000758 RID: 1880
	private bool NetworkInitialize__LatePlayerRespawnManagerAssembly-CSharp.dll_Excuted;
}
