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
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

// Token: 0x02000122 RID: 290
public class PlayerMovement : NetworkBehaviour
{
	// Token: 0x06000BD1 RID: 3025 RVA: 0x0002DD6C File Offset: 0x0002BF6C
	public override void OnStartClient()
	{
		base.OnStartClient();
		if (base.IsOwner)
		{
			GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MainMenuManager>().pm = this;
			base.StartCoroutine(this.GetNetManager());
			this.playerCamera = Camera.main;
			this.Sholder = GameObject.FindGameObjectWithTag("Settings").GetComponent<SettingsHolder>();
			this.Sholder.LoadKeybinds();
			this.frogtoungee.transform.parent = Camera.main.transform;
			this.frogtoungee.transform.localPosition = new Vector3(0.0058f, -0.1501f, 0.2233f);
			this.frogtoungee.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
			return;
		}
		this.SyncHandPos.enabled = false;
		Object.Destroy(this.PlayerArms);
		base.gameObject.GetComponent<PlayerMovement>().enabled = false;
		this.ntdc.StartDistRoutine();
	}

	// Token: 0x06000BD2 RID: 3026 RVA: 0x0002DE6D File Offset: 0x0002C06D
	public void SmokePipe()
	{
		this.DrinkStew(5);
	}

	// Token: 0x06000BD3 RID: 3027 RVA: 0x0002DE76 File Offset: 0x0002C076
	public void SetName(string namee)
	{
		this.ServerSetName(namee);
		this.playername = namee;
		this.playerNameText.transform.parent.parent.gameObject.SetActive(false);
	}

	// Token: 0x06000BD4 RID: 3028 RVA: 0x0002DEA6 File Offset: 0x0002C0A6
	[ServerRpc(RequireOwnership = false)]
	private void ServerSetName(string namee)
	{
		this.RpcWriter___Server_ServerSetName_3615296227(namee);
	}

	// Token: 0x06000BD5 RID: 3029 RVA: 0x0002DEB4 File Offset: 0x0002C0B4
	[ObserversRpc]
	private void ObsSetName(string namee)
	{
		this.RpcWriter___Observers_ObsSetName_3615296227(namee);
	}

	// Token: 0x06000BD6 RID: 3030 RVA: 0x00020E48 File Offset: 0x0001F048
	private string ClampString(string input, int maxLength)
	{
		if (input.Length <= maxLength)
		{
			return input;
		}
		return input.Substring(0, maxLength);
	}

	// Token: 0x06000BD7 RID: 3031 RVA: 0x0002DECB File Offset: 0x0002C0CB
	private IEnumerator GetNetManager()
	{
		yield return null;
		while (this.prm == null)
		{
			GameObject gameObject;
			PlayerRespawnManager playerRespawnManager;
			if ((gameObject = GameObject.FindGameObjectWithTag("NetItemManager")) && gameObject.TryGetComponent<PlayerRespawnManager>(out playerRespawnManager))
			{
				this.prm = playerRespawnManager;
			}
			yield return null;
		}
		this.prm.pmv = this;
		yield break;
	}

	// Token: 0x06000BD8 RID: 3032 RVA: 0x0002DEDC File Offset: 0x0002C0DC
	public void StartGame()
	{
		this.Nausteadoverlay = (FullScreenCustomPass)GameObject.FindGameObjectWithTag("NausOverlay").GetComponent<CustomPassVolume>().customPasses[0];
		this.Hurtoverlay = (FullScreenCustomPass)GameObject.FindGameObjectWithTag("HurtOverlay").GetComponent<CustomPassVolume>().customPasses[0];
		this.Freezeoverlay = (FullScreenCustomPass)GameObject.FindGameObjectWithTag("FreezeOverlay").GetComponent<CustomPassVolume>().customPasses[0];
		this.Fireoverlay = (FullScreenCustomPass)GameObject.FindGameObjectWithTag("FireOverlay").GetComponent<CustomPassVolume>().customPasses[0];
		this.UIBox = GameObject.FindGameObjectWithTag("uibox");
		this.UIani = this.UIBox.transform.GetChild(0).GetComponent<Animator>();
		this.characterController = base.GetComponent<CharacterController>();
		this.stamcover = GameObject.FindGameObjectWithTag("stamcover").transform;
		this.hpcover = GameObject.FindGameObjectWithTag("hpcover").transform;
		this.inited = true;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		if (base.IsOwner)
		{
			this.playerCamera.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + this.cameraYOffset, base.transform.position.z);
			this.playerCamera.transform.SetParent(base.transform);
			this.InitialCamPos = Camera.main.transform.localPosition;
			this.setTeamPlayer(this.playerTeam);
			if (this.playerTeam == 0)
			{
				Camera.main.GetComponent<PlayerInteract>().uiHand.GetComponent<RawImage>().texture = this.handsw[0];
			}
			else
			{
				Camera.main.GetComponent<PlayerInteract>().uiHand.GetComponent<RawImage>().texture = this.handsw[1];
			}
			this.canMove = true;
			Collider[] array = this.hitboxcols;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			base.StartCoroutine(this.TelePlayer());
			this.ServerSwitchTeam(this.playerTeam);
			this.vbt = Object.FindFirstObjectByType<VoiceBroadcastTrigger>();
		}
	}

	// Token: 0x06000BD9 RID: 3033 RVA: 0x0002E115 File Offset: 0x0002C315
	public void setxpbar()
	{
		this.xp = 0.001f;
		this.xpbar.ResizeAroundPivot(new Vector3(this.xp / 3f * 0.124f, 0.009f, 0.01f));
		this.level = 1;
	}

	// Token: 0x06000BDA RID: 3034 RVA: 0x0002E158 File Offset: 0x0002C358
	public void ResetCam()
	{
		this.playerCamera.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + this.cameraYOffset, base.transform.position.z);
		this.playerCamera.transform.SetParent(base.transform);
	}

	// Token: 0x06000BDB RID: 3035 RVA: 0x0002E1C7 File Offset: 0x0002C3C7
	private IEnumerator TelePlayer()
	{
		float timer = Time.time;
		this.teamSpawns[this.playerTeam].resizeWormhole();
		GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MainMenuManager>().pm.playPortalSound();
		while (Time.time - timer < 0.4f)
		{
			base.transform.position = this.teamSpawns[this.playerTeam].transform.position;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000BDC RID: 3036 RVA: 0x0002E1D6 File Offset: 0x0002C3D6
	[ServerRpc(RequireOwnership = false)]
	private void ServerReqTeam()
	{
		this.RpcWriter___Server_ServerReqTeam_2166136261();
	}

	// Token: 0x06000BDD RID: 3037 RVA: 0x0002E1DE File Offset: 0x0002C3DE
	[ObserversRpc]
	private void ObsReqTeam()
	{
		this.RpcWriter___Observers_ObsReqTeam_2166136261();
	}

	// Token: 0x06000BDE RID: 3038 RVA: 0x0002E1E6 File Offset: 0x0002C3E6
	public void SwitchTeam()
	{
		if (base.IsOwner)
		{
			this.ServerSwitchTeam(this.playerTeam);
		}
	}

	// Token: 0x06000BDF RID: 3039 RVA: 0x0002E1FC File Offset: 0x0002C3FC
	[ServerRpc(RequireOwnership = false)]
	private void ServerSwitchTeam(int tn)
	{
		this.RpcWriter___Server_ServerSwitchTeam_3316948804(tn);
	}

	// Token: 0x06000BE0 RID: 3040 RVA: 0x0002E208 File Offset: 0x0002C408
	[ObserversRpc]
	private void ObsSwitchTeam(int tn)
	{
		this.RpcWriter___Observers_ObsSwitchTeam_3316948804(tn);
	}

	// Token: 0x06000BE1 RID: 3041 RVA: 0x0002E214 File Offset: 0x0002C414
	private void setTeamPlayer(int teamNum)
	{
		for (int i = 0; i < this.wizardBody.Length; i++)
		{
			if (i == teamNum)
			{
				this.wizardBody[teamNum].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
				Material[] materials = this.armrender.materials;
				materials[0] = this.armsMats[i];
				this.armrender.materials = materials;
			}
			else
			{
				this.wizardBody[i].gameObject.SetActive(false);
			}
		}
		if (teamNum == 0)
		{
			this.Healthbar2.GetComponent<MeshRenderer>().material = this.sorchp;
			return;
		}
		this.Healthbar2.GetComponent<MeshRenderer>().material = this.warlockhp;
	}

	// Token: 0x06000BE2 RID: 3042 RVA: 0x0002E2B0 File Offset: 0x0002C4B0
	private void setTeamObservers(int teamNum)
	{
		for (int i = 0; i < this.wizardBody.Length; i++)
		{
			if (i != teamNum)
			{
				this.wizardBody[i].gameObject.SetActive(false);
			}
		}
		if (teamNum == 0)
		{
			this.Healthbar2.GetComponent<MeshRenderer>().material = this.sorchp;
			return;
		}
		this.Healthbar2.GetComponent<MeshRenderer>().material = this.warlockhp;
	}

	// Token: 0x06000BE3 RID: 3043 RVA: 0x0002E317 File Offset: 0x0002C517
	[ServerRpc(RequireOwnership = false)]
	private void FrogRpc()
	{
		this.RpcWriter___Server_FrogRpc_2166136261();
	}

	// Token: 0x06000BE4 RID: 3044 RVA: 0x0002E31F File Offset: 0x0002C51F
	[ObserversRpc]
	private void FrogObsSync()
	{
		this.RpcWriter___Observers_FrogObsSync_2166136261();
	}

	// Token: 0x06000BE5 RID: 3045 RVA: 0x0002E328 File Offset: 0x0002C528
	public void XPupdate()
	{
		this.xp += 1f;
		if (this.xp >= (float)(3 * Mathf.Clamp(this.level, 0, 2)))
		{
			this.portalSource.PlayOneShot(this.levelup);
			this.level++;
			Camera.main.GetComponent<PlayerInteract>().leveluptxt("Your spells grow stronger...");
			Camera.main.GetComponent<PlayerInteract>().SetLevelNum(this.level);
			Debug.Log(this.level);
			this.xp = 0.001f;
		}
		this.xpbar.ResizeAroundPivot(new Vector3(this.xp / (float)(3 * Mathf.Clamp(this.level, 0, 2)) * 0.124f, 0.009f, 0.01f));
	}

	// Token: 0x06000BE6 RID: 3046 RVA: 0x0002E3F9 File Offset: 0x0002C5F9
	[ServerRpc(RequireOwnership = false)]
	private void togglevbt()
	{
		this.RpcWriter___Server_togglevbt_2166136261();
	}

	// Token: 0x06000BE7 RID: 3047 RVA: 0x0002E401 File Offset: 0x0002C601
	[ObserversRpc]
	private void obstogglevbt()
	{
		this.RpcWriter___Observers_obstogglevbt_2166136261();
	}

	// Token: 0x06000BE8 RID: 3048 RVA: 0x0002E40C File Offset: 0x0002C60C
	private void Update()
	{
		if (this.inited)
		{
			this.HPupdate(this.playerHealth);
			if (this.eatenByFrog)
			{
				if (!this.sentFrogRpc)
				{
					this.playerHealth = 0f;
					this.causeofdeath = "frog";
					this.FrogRpc();
					this.sentFrogRpc = true;
					this.prm.IJustDied(this);
					if (this.LastPersonToHitMe != null)
					{
						this.prm.summonDeathMessage(this.playername, this.causeofdeath, this.LastPersonToHitMe.playername);
					}
					else
					{
						this.prm.summonDeathMessage(this.playername, this.causeofdeath, "");
					}
					this.PlayerArms.SetActive(false);
					base.transform.GetComponent<PlayerInventory>().PlayerDied();
					this.canMoveCamera = false;
					this.frogeatencameratimer = 0f;
					this.deaths++;
				}
				if (this.frogeatencameratimer < 4f)
				{
					base.transform.LookAt(this.FrogToung.parent.parent);
					this.characterController.height = 0.5f;
					this.playerCamera.transform.position = Vector3.Lerp(this.playerCamera.transform.position, base.transform.position, Time.deltaTime * 3f);
					base.transform.position = Vector3.Lerp(base.transform.position, this.FrogToung.transform.position, Time.deltaTime * 10f);
					this.frogeatencameratimer += Time.deltaTime;
				}
				if (this.areParticlesActive)
				{
					this.areParticlesActive = false;
					this.ToggleParticles(false);
				}
			}
			else if (!this.isDead)
			{
				if (base.transform.position.y < -500f)
				{
					this.playerHealth = -50f;
					this.causeofdeath = "fall";
				}
				if (this.vbt._isVadSpeaking && !this.isvbton)
				{
					if (this.toggleontimer > 0.25f)
					{
						this.togglevbt();
						this.isvbton = true;
					}
					else
					{
						this.toggleontimer += Time.deltaTime;
					}
				}
				else if (!this.vbt._isVadSpeaking && this.isvbton)
				{
					this.toggleontimer = 0f;
					this.togglevbt();
					this.isvbton = false;
				}
				if (base.GetComponent<PlayerAudioPlayer>().oceanint > 0 && base.transform.position.y < 49f)
				{
					this.causeofdeath = "drown";
					this.playerHealth -= 10f * Time.deltaTime;
				}
				if (Input.GetKeyDown(this.Sholder.recall) && Time.time - this.recallCD > 30f && this.canRecall)
				{
					base.StartCoroutine(this.RecallRoutine());
				}
				if (this.fireTimer > 15f)
				{
					this.fireTimer = 15f;
				}
				if (this.fireTimer > 0f)
				{
					this.causeofdeath = "fire";
					this.fireTimer -= Time.deltaTime;
					this.playerHealth -= Time.deltaTime * 4f;
					this.hurtyOverlayVal += Time.deltaTime * 4f / 100f;
					Mathf.Clamp(this.hurtyOverlayVal, 0f, 0.5f);
					if (!this.areParticlesActive)
					{
						this.areParticlesActive = true;
						this.ToggleParticles(true);
					}
				}
				else if (this.areParticlesActive)
				{
					this.areParticlesActive = false;
					this.ToggleParticles(false);
				}
				if (this.playerHealth < 100f)
				{
					this.playerHealth += Time.deltaTime / 3f;
				}
				if (this.hurtyOverlayVal > 0f)
				{
					this.hurtyOverlayVal = Mathf.Clamp(this.hurtyOverlayVal, 0f, 0.16f);
					if (this.playerHealth > 60f)
					{
						this.Hurtoverlay.fullscreenPassMaterial.SetFloat("_x", this.hurtyOverlayVal);
						this.hurtyOverlayVal -= Time.deltaTime / 4f;
					}
					else if (this.playerHealth > 20f)
					{
						this.Hurtoverlay.fullscreenPassMaterial.SetFloat("_x", this.hurtyOverlayVal);
						this.hurtyOverlayVal -= Time.deltaTime / 8f;
						if (this.hurtyOverlayVal < 0.12f)
						{
							this.hurtyOverlayVal = 0.12f;
						}
					}
					else
					{
						this.Hurtoverlay.fullscreenPassMaterial.SetFloat("_x", this.hurtyOverlayVal);
					}
					this.hurtyOverlayVal = Mathf.Clamp(this.hurtyOverlayVal, 0f, 0.16f);
				}
				else
				{
					this.Hurtoverlay.fullscreenPassMaterial.SetFloat("_x", 0f);
				}
				if (Input.GetKeyDown(this.Sholder.Crouch))
				{
					this.isCrouch = !this.isCrouch;
				}
				if (this.NauseatedTimer > 0f)
				{
					this.NauseatedTimer -= Time.deltaTime;
				}
				if (this.externalxmoveinput > 0.1f)
				{
					this.externalxmoveinput -= Time.deltaTime * 10f;
				}
				else if (this.externalxmoveinput < -0.1f)
				{
					this.externalxmoveinput += Time.deltaTime * 10f;
				}
				else if (Mathf.Abs(this.externalxmoveinput) > 0.01f)
				{
					this.externalxmoveinput = 0f;
				}
				if (this.externalzmoveinput > 0.1f)
				{
					this.externalzmoveinput -= Time.deltaTime * 10f;
				}
				else if (this.externalzmoveinput < -0.1f)
				{
					this.externalzmoveinput += Time.deltaTime * 10f;
				}
				else if (Mathf.Abs(this.externalzmoveinput) > 0.01f)
				{
					this.externalzmoveinput = 0f;
				}
				if (this.isBeingLevitated)
				{
					Vector3 vector = this.LevitationTarget.position - base.transform.position;
					if (vector.magnitude > this.runningSpeed / 1.3f / (1f / Time.deltaTime))
					{
						vector = vector.normalized * (this.runningSpeed / 1.3f);
						this.characterController.Move(vector * Time.deltaTime);
					}
				}
				else if (this.canMove)
				{
					Debug.DrawRay(base.transform.position, Vector3.down * 1.25f, Color.red);
					RaycastHit raycastHit;
					if (Physics.Raycast(new Vector3(base.transform.position.x, base.transform.position.y + 0.2f, base.transform.position.z), Vector3.down, out raycastHit, 0.6f, this.groundLayer))
					{
						if (raycastHit.transform.CompareTag("SinkBog"))
						{
							this.isInBog = true;
							this.lasttimewasinbog = Time.time;
							if (this.stewspeedboost >= 0f)
							{
								this.walkingSpeed = 1f;
								this.runningSpeed = 1f + this.stewspeedboost;
							}
							this.jumpSpeed = 7f + this.stewjumpbonus;
						}
						else if (raycastHit.transform.CompareTag("RockSpell"))
						{
							this.rockHit(raycastHit.transform.GetComponent<GetRockPlayerOwner>().owner);
						}
						else if (raycastHit.transform.CompareTag("BounceMush"))
						{
							this.MushroomJump();
							this.isOnMushroom = true;
							this.BlockFallDmgFrames = 5;
						}
						else if (raycastHit.transform.CompareTag("Spike"))
						{
							this.causeofdeath = "spikes";
							this.playerHealth -= Time.deltaTime * 170f;
							this.hurtyOverlayVal += Time.deltaTime * 5f;
							if (Time.time - this.hurtsoundcd > 0.1f)
							{
								this.portalSource.PlayOneShot(this.falldmg);
								this.hurtsoundcd = Time.time;
							}
							Mathf.Clamp(this.hurtyOverlayVal, 0f, 0.5f);
						}
						else if (raycastHit.transform.CompareTag("WaterGround"))
						{
							this.isInWater = true;
							this.fireTimer = 0f;
						}
						else if (raycastHit.transform.CompareTag("CrystalGround"))
						{
							this.OnCrystalGround = true;
						}
						else if (raycastHit.transform.CompareTag("LavaGround"))
						{
							this.walkingSpeed = 1f;
							this.runningSpeed = 1f + this.stewspeedboost;
							this.fireTimer += Time.deltaTime * 8f;
							this.playerHealth -= 8f * Time.deltaTime;
						}
						else
						{
							this.OnCrystalGround = false;
							this.isInWater = false;
							this.isInBog = false;
							this.isOnMushroom = false;
							this.jumpSpeed = 8f + this.stewjumpbonus;
							this.walkingSpeed = 4f;
							this.runningSpeed = 9f + this.stewspeedboost;
						}
					}
					else
					{
						this.jumpSpeed = 8f + this.stewjumpbonus;
						this.isInBog = false;
						this.walkingSpeed = 4f;
						this.runningSpeed = 9f + this.stewspeedboost;
					}
					if (this.HoldingStickid == 12 && !this.isInBog)
					{
						this.runningSpeed = 11f + this.stewspeedboost;
						this.walkingSpeed = 5.5f;
					}
					else if (this.HoldingStickid == 11)
					{
						if (!this.isInBog)
						{
							this.runningSpeed = 11f + this.stewspeedboost;
							this.walkingSpeed = 5f;
						}
						this.jumpSpeed = 12f + this.stewjumpbonus;
					}
					this.UpdateGravity();
					if (this.characterController.isGrounded && !this.isOnMushroom && this.BlockFallDmgFrames > 0)
					{
						this.BlockFallDmgFrames--;
					}
					if (!this.characterController.isGrounded)
					{
						this.timeSinceGrounded += Time.deltaTime;
					}
					else
					{
						this.timeSinceGrounded = 0f;
					}
					this.UpdateMovement();
				}
				if (this.isInBog)
				{
					this.isCrouch = false;
				}
				this.BodyAni.SetBool("crouch", false);
				if (this.isCrouch)
				{
					this.BodyAni.SetBool("crouch", true);
					this.characterController.height = Mathf.Lerp(this.characterController.height, 1.3f, Time.deltaTime * 5f);
					this.UIBox.transform.localPosition = Vector3.Lerp(this.UIBox.transform.localPosition, new Vector3(this.UIBox.transform.localPosition.x, this.UIBox.transform.localPosition.y, 0.145f), Time.deltaTime * 5f);
				}
				else if (this.characterController.height < 2.3f)
				{
					this.characterController.Move(new Vector3(0f, 0f, 0.1f) * Time.deltaTime);
					this.characterController.height += Time.deltaTime * 5f;
					if (this.characterController.height > 2.3f)
					{
						this.characterController.height = 2.3f;
					}
				}
				if (this.playerHealth <= 0f && !this.isDead)
				{
					this.isDead = true;
					this.deaths++;
					this.DisableHBCols();
					this.ServerDeathRobes(this.playerTeam);
					this.prm.IJustDied(this);
					if (this.LastPersonToHitMe != null)
					{
						if (this.LastPersonToHitMe.playerTeam != this.playerTeam)
						{
							this.LastPersonToHitMe.AddKill();
						}
						this.prm.summonDeathMessage(this.playername, this.causeofdeath, this.LastPersonToHitMe.playername);
					}
					else
					{
						this.prm.summonDeathMessage(this.playername, this.causeofdeath, "");
					}
					this.PlayerArms.SetActive(false);
					base.transform.GetComponent<PlayerInventory>().PlayerDied();
					if (this.areParticlesActive)
					{
						this.areParticlesActive = false;
						this.ToggleParticles(false);
					}
				}
			}
			else
			{
				this.canMoveCamera = false;
				this.fireTimer = 0f;
				if (this.hurtyOverlayVal > 0f)
				{
					this.hurtyOverlayVal -= Time.deltaTime / 8f;
					this.hurtyOverlayVal = Mathf.Clamp(this.hurtyOverlayVal, 0f, 0.5f);
					this.Hurtoverlay.fullscreenPassMaterial.SetFloat("_x", this.hurtyOverlayVal);
				}
			}
			if (this.canMoveCamera && this.playerCamera != null)
			{
				float axis = Input.GetAxis("Mouse X");
				float axis2 = Input.GetAxis("Mouse Y");
				Vector2 vector2 = new Vector2(axis, axis2);
				this.look.x = this.look.x + vector2.x * this.mouseSensitivity * this.Sholder.sense;
				this.look.y = this.look.y + vector2.y * this.mouseSensitivity * this.Sholder.sense;
				this.look.y = Mathf.Clamp(this.look.y, -65f, 80f);
				float num = 0f;
				float num2 = 0f;
				float num3 = 0f;
				if (this.shaketime > 0f)
				{
					this.shaketime -= Time.deltaTime;
					this.shakemagnitude -= this.shakefalloff;
					this.shakefalloff += Time.deltaTime / 12f;
					this.shakemagnitude = Mathf.Clamp(this.shakemagnitude, 0f, 200f);
					num = Random.Range(-1f, 1f) * this.shakemagnitude;
					num2 = Random.Range(-1f, 1f) * this.shakemagnitude;
					num3 = Random.Range(-1f, 1f) * this.shakemagnitude * 1f;
				}
				this.playerCamera.transform.localRotation = Quaternion.Euler(-this.look.y + num, num2, num3);
				this.PlayerArms.transform.localRotation = Quaternion.Slerp(this.PlayerArms.transform.localRotation, Quaternion.Euler(-this.look.y, 0f, 0f), Time.deltaTime * this.camArmsSyncSpeed);
				base.transform.localRotation = Quaternion.Euler(0f, this.look.x, 0f);
				if (vector2.x < -2f)
				{
					this.UIani.SetBool("left", true);
					this.UIani.SetBool("right", false);
				}
				else if (vector2.x > 2f)
				{
					this.UIani.SetBool("right", true);
					this.UIani.SetBool("left", false);
				}
				else
				{
					this.UIani.SetBool("right", false);
					this.UIani.SetBool("left", false);
				}
			}
			else
			{
				bool flag = this.isDead;
			}
			if (this.playerHealth < 0f)
			{
				this.playerHealth = 0f;
			}
			this.stamcover.localScale = Vector3.Lerp(this.stamcover.localScale, new Vector3(this.stamcover.localScale.x, 7.91f - this.stamina / 1.264f), Time.deltaTime * 5f);
			this.hpcover.localScale = Vector3.Lerp(this.hpcover.localScale, new Vector3(this.hpcover.localScale.x, 10f - this.playerHealth / 10f), Time.deltaTime * 5f);
		}
	}

	// Token: 0x06000BE9 RID: 3049 RVA: 0x0002F480 File Offset: 0x0002D680
	public void rockHit(GameObject owner)
	{
		if (Time.time - this.rockhitcd > 0.3f)
		{
			this.rockhitcd = Time.time;
			this.NonRpcDamagePlayer(25f, owner, "fireball");
		}
		this.velocity.y = 30f;
		this.hitbymushroom = true;
		base.StartCoroutine(this.WhyisthisnotWorking());
	}

	// Token: 0x06000BEA RID: 3050 RVA: 0x0002F4E0 File Offset: 0x0002D6E0
	private IEnumerator WhyisthisnotWorking()
	{
		yield return new WaitForEndOfFrame();
		this.velocity.y = 30f;
		yield break;
	}

	// Token: 0x06000BEB RID: 3051 RVA: 0x0002F4EF File Offset: 0x0002D6EF
	private void DisableHBCols()
	{
		this.DisableHitCols();
	}

	// Token: 0x06000BEC RID: 3052 RVA: 0x0002F4F7 File Offset: 0x0002D6F7
	[ServerRpc(RequireOwnership = false)]
	private void DisableHitCols()
	{
		this.RpcWriter___Server_DisableHitCols_2166136261();
	}

	// Token: 0x06000BED RID: 3053 RVA: 0x0002F500 File Offset: 0x0002D700
	[ObserversRpc]
	private void ObsDisableHBCols()
	{
		this.RpcWriter___Observers_ObsDisableHBCols_2166136261();
	}

	// Token: 0x06000BEE RID: 3054 RVA: 0x0002F513 File Offset: 0x0002D713
	public void EnableCols()
	{
		this.EnableHBCols();
	}

	// Token: 0x06000BEF RID: 3055 RVA: 0x0002F51B File Offset: 0x0002D71B
	[ServerRpc(RequireOwnership = false)]
	private void EnableHBCols()
	{
		this.RpcWriter___Server_EnableHBCols_2166136261();
	}

	// Token: 0x06000BF0 RID: 3056 RVA: 0x0002F524 File Offset: 0x0002D724
	[ObserversRpc]
	private void ObsEnableHBCols()
	{
		this.RpcWriter___Observers_ObsEnableHBCols_2166136261();
	}

	// Token: 0x06000BF1 RID: 3057 RVA: 0x0002F538 File Offset: 0x0002D738
	private void LateUpdate()
	{
		this.spinesynctimer += Time.deltaTime;
		if (this.spinesynctimer > 0.1f)
		{
			this.spinesync.lerptarget = -this.look.y;
			this.ServerSyncSpine(-this.look.y);
		}
	}

	// Token: 0x06000BF2 RID: 3058 RVA: 0x0002F58D File Offset: 0x0002D78D
	[ServerRpc(RequireOwnership = false)]
	private void ServerSyncSpine(float val)
	{
		this.RpcWriter___Server_ServerSyncSpine_431000436(val);
	}

	// Token: 0x06000BF3 RID: 3059 RVA: 0x0002F599 File Offset: 0x0002D799
	[ObserversRpc]
	private void ObsSyncSpine(float val)
	{
		this.RpcWriter___Observers_ObsSyncSpine_431000436(val);
	}

	// Token: 0x06000BF4 RID: 3060 RVA: 0x0002F5A8 File Offset: 0x0002D7A8
	private void UpdateGravity()
	{
		if (this.isInBog)
		{
			if (this.velocity.y > 0f)
			{
				this.velocity.y = this.velocity.y - Time.deltaTime;
			}
			else if (this.velocity.y < 0f)
			{
				this.velocity.y = this.velocity.y + Time.deltaTime;
			}
		}
		else if (!this.hitbymushroom)
		{
			Vector3 vector = Physics.gravity * this.mass * Time.deltaTime;
			this.velocity.y = (this.characterController.isGrounded ? (-1f) : (this.velocity.y + vector.y));
		}
		else if (this.hitbymushroom)
		{
			this.hitbymushroom = false;
		}
		this.prevYVel = this.characterController.velocity.y;
		if (!this.characterController.isGrounded && this.velocity.y < 0f && this.stewjumpbonus < 1f)
		{
			this.FallTimer += Time.deltaTime;
			if (this.HoldingStickid == 23 || this.isInBog || Time.time - this.lasttimewasinbog < 1f)
			{
				this.FallTimer = 0f;
				return;
			}
		}
		else
		{
			if (this.FallTimer > 1.4f)
			{
				if (!this.isInWater && this.BlockFallDmgFrames <= 0 && this.HoldingStickid != 23 && this.HoldingStickid != 11)
				{
					float num = Mathf.Abs(this.FallTimer * this.FallTimer * this.FallTimer * 10f);
					this.causeofdeath = "fall";
					this.playerHealth -= num;
					this.hurtyOverlayVal += num / 100f;
					this.portalSource.volume = num / 100f;
					this.portalSource.PlayOneShot(this.falldmg);
				}
				this.FallTimer = 0f;
				return;
			}
			this.FallTimer = 0f;
		}
	}

	// Token: 0x06000BF5 RID: 3061 RVA: 0x0002F7C0 File Offset: 0x0002D9C0
	private void UpdateMovement()
	{
		Vector2 zero = Vector2.zero;
		if (Input.GetKey(this.Sholder.w2))
		{
			zero.y += 1f;
		}
		if (Input.GetKey(this.Sholder.s2))
		{
			zero.y -= 1f;
		}
		if (Input.GetKey(this.Sholder.a2))
		{
			zero.x -= 1f;
		}
		if (Input.GetKey(this.Sholder.d2))
		{
			zero.x += 1f;
		}
		if (zero.y < 0f)
		{
			this.BodyAni.SetBool("backwards", true);
			zero.y *= 0.8f;
		}
		else
		{
			this.BodyAni.SetBool("backwards", false);
		}
		Vector3 vector = default(Vector3);
		vector += base.transform.forward * zero.y;
		vector += base.transform.right * zero.x;
		vector = Vector3.ClampMagnitude(vector, 1f);
		if (Mathf.Abs(zero.y) > 0.05f)
		{
			this.ArmsAni.SetBool("walk", true);
			this.BodyAni.SetBool("walk", true);
			this.BodyAni.SetBool("strafe", false);
		}
		else if (Mathf.Abs(zero.x) > 0.05f)
		{
			this.BodyAni.SetBool("strafe", true);
			this.ArmsAni.SetBool("walk", true);
		}
		else
		{
			this.BodyAni.SetBool("strafe", false);
			this.ArmsAni.SetBool("walk", false);
			this.BodyAni.SetBool("walk", false);
		}
		if (this.stamina > 2f)
		{
			this.canSprint = true;
		}
		else if (this.stamina <= 0.1f)
		{
			this.canSprint = false;
		}
		if (Input.GetKey(this.Sholder.sprint) && this.canSprint)
		{
			if (Mathf.Abs(zero.y) > 0.1f || Mathf.Abs(zero.x) > 0.1f)
			{
				this.ArmsAni.SetBool("run", true);
				this.BodyAni.SetBool("run", true);
				this.UIani.SetBool("walk", true);
			}
			else
			{
				this.ArmsAni.SetBool("run", false);
				this.BodyAni.SetBool("run", false);
				this.UIani.SetBool("walk", false);
			}
			this.currentSpeed = Mathf.Lerp(this.currentSpeed, this.runningSpeed, Time.deltaTime * 15f);
			vector *= this.currentSpeed + Mathf.Clamp(this.bhopbonus, 0f, 2f);
			this.isCrouch = false;
		}
		else
		{
			this.ArmsAni.SetBool("run", false);
			this.BodyAni.SetBool("run", false);
			this.UIani.SetBool("walk", false);
			if (Mathf.Abs(zero.y) > 0.05f || Mathf.Abs(zero.x) > 0.05f)
			{
				this.currentSpeed = Mathf.Lerp(this.currentSpeed, this.walkingSpeed, Time.deltaTime * 3f);
			}
			else
			{
				this.currentSpeed = Mathf.Lerp(this.currentSpeed, 0f, Time.deltaTime * 3f);
			}
			vector *= this.currentSpeed;
		}
		if (this.NauseatedTimer > 0.1f)
		{
			if (this.nausoverlaylerper < 0.2f)
			{
				this.nausoverlaylerper += Time.deltaTime;
			}
			vector.x = -vector.x;
			vector.z = -vector.z;
			this.Nausteadoverlay.fullscreenPassMaterial.SetFloat("_x", this.nausoverlaylerper);
		}
		else
		{
			if (this.nausoverlaylerper > 0f)
			{
				this.nausoverlaylerper -= Time.deltaTime;
				if (this.nausoverlaylerper < 0f)
				{
					this.nausoverlaylerper = 0f;
				}
			}
			this.Nausteadoverlay.fullscreenPassMaterial.SetFloat("_x", this.nausoverlaylerper);
		}
		if (Input.GetKey(this.Sholder.sprint) && (double)this.stamina > -0.1)
		{
			this.UpdateSlopeSliding(20f, 2f);
			this.velocity.x = Mathf.Lerp(this.velocity.x, vector.x + this.externalxmoveinput, Time.deltaTime * 1.2f);
			this.velocity.z = Mathf.Lerp(this.velocity.z, vector.z + this.externalzmoveinput, Time.deltaTime * 1.2f);
		}
		if (Mathf.Abs(zero.y) > 0.05f || Mathf.Abs(zero.x) > 0.05f)
		{
			this.UpdateSlopeSliding(20f, 1f);
			this.velocity.x = Mathf.Lerp(this.velocity.x, vector.x + this.externalxmoveinput, Time.deltaTime * 3.6f);
			this.velocity.z = Mathf.Lerp(this.velocity.z, vector.z + this.externalzmoveinput, Time.deltaTime * 3.6f);
		}
		else
		{
			this.UpdateSlopeSliding(60f, 1f);
			this.velocity.x = Mathf.Lerp(this.velocity.x, vector.x + this.externalxmoveinput, Time.deltaTime * this.acceleration);
			this.velocity.z = Mathf.Lerp(this.velocity.z, vector.z + this.externalzmoveinput, Time.deltaTime * this.acceleration);
		}
		if (this.characterController.isGrounded)
		{
			this.ArmsAni.SetBool("jump", false);
			this.BodyAni.SetBool("jump", false);
			this.UIani.SetBool("run", false);
		}
		int num = 0;
		if (Input.GetKeyDown(this.Sholder.jump))
		{
			this.numberofpressesbeforejump++;
			if (this.numberofpressesbeforejump > 1)
			{
				this.bhopr = false;
			}
			num++;
			this.jumpinputted = Time.time;
		}
		else if (Time.time - this.jumpinputted < 0.2f)
		{
			if (this.numberofpressesbeforejump == 1)
			{
				this.bhopr = true;
			}
			num++;
		}
		if (Time.time - this.lasttimewasinbog < 0.3f && this.canJump)
		{
			if (Input.GetKeyDown(this.Sholder.jump))
			{
				this.velocity.y = this.velocity.y + 3f;
			}
		}
		else if (num > 0 && this.characterController.isGrounded && this.canJump && this.stamina > 0.3f)
		{
			if (this.bhopr)
			{
				this.bhopbonus += 0.2f;
			}
			else
			{
				this.bhopbonus = 0f;
			}
			this.numberofpressesbeforejump = 0;
			this.isCrouch = false;
			this.stamina -= 0.2f / this.stewstaminabonus;
			this.timeOfLastJump = Time.time;
			this.velocity.y = this.velocity.y + this.jumpSpeed;
			this.ArmsAni.SetBool("jump", true);
			this.BodyAni.SetBool("jump", true);
			this.UIani.SetBool("run", true);
			if (this.HoldingStickid == 11 || this.stewjumpbonus > 0f)
			{
				this.ServerBoing();
			}
		}
		else if (this.characterController.isGrounded)
		{
			this.bhopbonus = 0f;
		}
		float magnitude = new Vector3(this.characterController.velocity.x, 0f, this.characterController.velocity.z).magnitude;
		this.elapsedTime += Time.deltaTime;
		float num2 = 0f;
		this.characterController.Move(this.velocity * Time.deltaTime);
		this.distanceTraveled += this.velocity.magnitude * Time.deltaTime;
		if (Mathf.Abs(this.characterController.velocity.y - this.prevYVel) > 2f || this.camyoffsetframes > 0f)
		{
			if (this.camyoffsetframes == 0f && this.timeSinceGrounded > 0.2f)
			{
				this.camyoffsetframes = 4f;
			}
			if (!this.isInBog)
			{
				num2 = this.camjumpyoffset;
			}
			Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60f, Time.deltaTime);
		}
		else if (magnitude > 6f && this.characterController.isGrounded)
		{
			if (this.stamina > -0.1f)
			{
				this.stamina -= Time.deltaTime / 1.6f * this.stewstaminabonus;
			}
			num2 = Mathf.Sin(this.elapsedTime * this.headbobfrequency * 22f) * this.headbobamplitude * 14f;
			this.UIBox.transform.localPosition = Vector3.Lerp(this.UIBox.transform.localPosition, new Vector3(this.UIBox.transform.localPosition.x, this.UIBox.transform.localPosition.y, 0.144f), Time.deltaTime * 4f);
			Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 61f, Time.deltaTime);
		}
		else if (magnitude > 2f && this.characterController.isGrounded)
		{
			if (this.stamina < 10f)
			{
				this.stamina += Time.deltaTime * 2f;
			}
			num2 = Mathf.Sin(this.elapsedTime * this.headbobfrequency * 12f) * this.headbobamplitude * 4f;
			this.UIBox.transform.localPosition = Vector3.Lerp(this.UIBox.transform.localPosition, new Vector3(this.UIBox.transform.localPosition.x, this.UIBox.transform.localPosition.y, 0.141f), Time.deltaTime * 3f);
			Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60f, Time.deltaTime);
		}
		else
		{
			if (this.stamina < 10f && this.characterController.isGrounded)
			{
				this.stamina += Time.deltaTime * 4f;
			}
			num2 = 0f;
			this.UIBox.transform.localPosition = Vector3.Lerp(this.UIBox.transform.localPosition, new Vector3(this.UIBox.transform.localPosition.x, this.UIBox.transform.localPosition.y, 0.141f), Time.deltaTime * 3f);
			Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60f, Time.deltaTime);
		}
		if (this.isCrouch)
		{
			num2 -= 0.1f;
		}
		Vector3 vector2 = this.InitialCamPos + new Vector3(0f, num2, 0f);
		this.playerCamera.transform.localPosition = Vector3.Lerp(this.playerCamera.transform.localPosition, vector2, Time.deltaTime * 4f);
		if (this.isSmoking)
		{
			this.PlayerArms.transform.localPosition = Vector3.Lerp(this.PlayerArms.transform.localPosition, new Vector3(-0.14f, 1.98f, -0.37f), Time.deltaTime * 4f);
			this.ArmsAni.SetBool("jump", false);
			this.ArmsAni.SetBool("attack", false);
			this.ArmsAni.SetBool("walk", false);
			this.ArmsAni.SetBool("run", false);
		}
		else if (!this.ArmsAni.GetBool("attack"))
		{
			this.PlayerArms.transform.localPosition = Vector3.Lerp(this.PlayerArms.transform.localPosition, new Vector3(0f, num2 + this.armsYOffset, -0.1f), Time.deltaTime * 4f);
		}
		else if (this.camyoffsetframes > 0f)
		{
			this.PlayerArms.transform.localPosition = Vector3.Lerp(this.PlayerArms.transform.localPosition, new Vector3(0f, num2 + this.armsYOffset, -0.1f), Time.deltaTime * 4f);
		}
		else
		{
			this.PlayerArms.transform.localPosition = Vector3.Lerp(this.PlayerArms.transform.localPosition, new Vector3(0f, num2 + this.armsYOffset, -0.1f), Time.deltaTime * 3f);
		}
		if (this.camyoffsetframes > 0f)
		{
			this.camyoffsetframes -= 1f;
		}
	}

	// Token: 0x06000BF6 RID: 3062 RVA: 0x000305A7 File Offset: 0x0002E7A7
	[ServerRpc(RequireOwnership = false)]
	private void ServerBoing()
	{
		this.RpcWriter___Server_ServerBoing_2166136261();
	}

	// Token: 0x06000BF7 RID: 3063 RVA: 0x000305AF File Offset: 0x0002E7AF
	[ObserversRpc]
	private void ObsBoing()
	{
		this.RpcWriter___Observers_ObsBoing_2166136261();
	}

	// Token: 0x06000BF8 RID: 3064 RVA: 0x000305B8 File Offset: 0x0002E7B8
	private void UpdateSlopeSliding(float minAngle, float multiplier)
	{
		RaycastHit raycastHit;
		if (this.characterController.isGrounded && !this.OnCrystalGround && Physics.SphereCast(new Vector3(base.transform.position.x, base.transform.position.y + 1f, base.transform.position.z), this.characterController.radius - 0.01f, Vector3.down, out raycastHit, 1.5f, this.groundLayerMask, QueryTriggerInteraction.Ignore) && Vector3.Angle(Vector3.up, raycastHit.normal) > minAngle)
		{
			Vector3 normal = raycastHit.normal;
			float num = 1f - normal.y;
			this.slopex = Mathf.Lerp(this.slopex, num * normal.x * multiplier, Time.deltaTime);
			this.slopez = Mathf.Lerp(this.slopez, num * normal.z * multiplier, Time.deltaTime);
			this.velocity.x = this.velocity.x + this.slopex;
			this.velocity.z = this.velocity.z + this.slopez;
		}
	}

	// Token: 0x06000BF9 RID: 3065 RVA: 0x000306E1 File Offset: 0x0002E8E1
	public void MushroomJump()
	{
		this.hitbymushroom = true;
		this.velocity.y = 30f;
	}

	// Token: 0x06000BFA RID: 3066 RVA: 0x000306FA File Offset: 0x0002E8FA
	public bool checkowner()
	{
		return base.IsOwner;
	}

	// Token: 0x06000BFB RID: 3067 RVA: 0x00030704 File Offset: 0x0002E904
	public void ApplyKnockback(GameObject HitPoint)
	{
		if (Time.time - this.knockbackcd > 1f)
		{
			this.knockbackcd = Time.time;
			Vector3 vector = base.transform.position - HitPoint.transform.position;
			if (this.externalxmoveinput < 20f && this.externalzmoveinput < 20f)
			{
				this.externalxmoveinput += vector.x * 15f;
				this.externalzmoveinput += vector.z * 15f;
			}
			this.velocity.y = 35f;
			this.hitbymushroom = true;
		}
	}

	// Token: 0x06000BFC RID: 3068 RVA: 0x000307B4 File Offset: 0x0002E9B4
	public void BoingWall()
	{
		if (Time.time - this.knockbackcd > 1f)
		{
			this.knockbackcd = Time.time;
			this.knockbackcd = Time.time;
			if (this.externalxmoveinput < 20f && this.externalzmoveinput < 20f)
			{
				this.externalxmoveinput += -Camera.main.transform.forward.x * 20f;
				this.externalzmoveinput += -Camera.main.transform.forward.z * 20f;
			}
			this.velocity.y = 18f;
			this.hitbymushroom = true;
		}
	}

	// Token: 0x06000BFD RID: 3069 RVA: 0x00030870 File Offset: 0x0002EA70
	public void MountainWind(Transform LerpTarg, float strength)
	{
		Vector3 normalized = (LerpTarg.transform.position - base.transform.position).normalized;
		this.externalxmoveinput = normalized.x * strength * 2f;
		this.externalzmoveinput = normalized.z * strength * 2f;
	}

	// Token: 0x06000BFE RID: 3070 RVA: 0x000308C9 File Offset: 0x0002EAC9
	public void CallShrinkPlayer()
	{
		this.ServerCallShrinkPlayer();
	}

	// Token: 0x06000BFF RID: 3071 RVA: 0x000308D1 File Offset: 0x0002EAD1
	[ServerRpc(RequireOwnership = false)]
	private void ServerCallShrinkPlayer()
	{
		this.RpcWriter___Server_ServerCallShrinkPlayer_2166136261();
	}

	// Token: 0x06000C00 RID: 3072 RVA: 0x000308D9 File Offset: 0x0002EAD9
	[ObserversRpc]
	private void ObserversCallShrinkPlayer()
	{
		this.RpcWriter___Observers_ObserversCallShrinkPlayer_2166136261();
	}

	// Token: 0x06000C01 RID: 3073 RVA: 0x000308E1 File Offset: 0x0002EAE1
	public void ShrinkPlayer()
	{
		if (base.IsOwner)
		{
			base.StartCoroutine(this.ShrinkRoutine());
		}
	}

	// Token: 0x06000C02 RID: 3074 RVA: 0x000308F8 File Offset: 0x0002EAF8
	private IEnumerator ShrinkRoutine()
	{
		float ShrinkTimer = 0f;
		this.UnShrinkTimer += 40f;
		Vector3 startscale = base.transform.localScale;
		while (ShrinkTimer < 1f)
		{
			ShrinkTimer += Time.deltaTime;
			base.transform.localScale = Vector3.Lerp(startscale, new Vector3(0.5f, 0.5f, 0.5f), ShrinkTimer);
			yield return null;
		}
		while (this.UnShrinkTimer > 0f)
		{
			this.UnShrinkTimer -= Time.deltaTime;
			yield return null;
		}
		ShrinkTimer = 0f;
		startscale = base.transform.localScale;
		while (ShrinkTimer < 1f)
		{
			ShrinkTimer += Time.deltaTime;
			base.transform.localScale = Vector3.Lerp(startscale, new Vector3(1f, 1f, 1f), ShrinkTimer);
			yield return null;
		}
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		yield break;
	}

	// Token: 0x06000C03 RID: 3075 RVA: 0x00030907 File Offset: 0x0002EB07
	public void Fireballoverlay()
	{
		base.StartCoroutine(this.Fireballoverlayrout());
	}

	// Token: 0x06000C04 RID: 3076 RVA: 0x00030916 File Offset: 0x0002EB16
	private IEnumerator Fireballoverlayrout()
	{
		float timer = 0f;
		while ((double)timer < 0.1)
		{
			this.Fireoverlay.fullscreenPassMaterial.SetFloat("_x", timer * 2f);
			timer += Time.deltaTime;
			yield return null;
		}
		timer = 0f;
		while (timer < 1f)
		{
			float num = Mathf.Clamp01(0.2f - timer / 5f);
			this.Fireoverlay.fullscreenPassMaterial.SetFloat("_x", num);
			timer += Time.deltaTime;
			yield return null;
		}
		this.Fireoverlay.fullscreenPassMaterial.SetFloat("_x", 0f);
		yield break;
	}

	// Token: 0x06000C05 RID: 3077 RVA: 0x00030925 File Offset: 0x0002EB25
	public void CallSummonIceBox(int lvl, GameObject playerownner)
	{
		this.ServerCallSummonIceBox(lvl, playerownner);
	}

	// Token: 0x06000C06 RID: 3078 RVA: 0x0003092F File Offset: 0x0002EB2F
	[ServerRpc(RequireOwnership = false)]
	private void ServerCallSummonIceBox(int lvl, GameObject playerownner)
	{
		this.RpcWriter___Server_ServerCallSummonIceBox_1011425610(lvl, playerownner);
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x0003093F File Offset: 0x0002EB3F
	[ObserversRpc]
	private void ObserversCallSummonIceBox(int lvl, GameObject playerownner)
	{
		this.RpcWriter___Observers_ObserversCallSummonIceBox_1011425610(lvl, playerownner);
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x00030950 File Offset: 0x0002EB50
	public void SummonIceBox(int lvl, GameObject playerownner)
	{
		this.breakoutFireball = false;
		if (base.IsOwner && Time.time - this.frostboltcdtimer > 1f)
		{
			this.frostboltcdtimer = Time.time;
			if (lvl == 0)
			{
				this.ArmsAni.speed = 0.5f;
				this.BodyAni.speed = 0.5f;
				base.StartCoroutine(this.FreezeOverlay(4f));
				if (this.stewspeedboost > -6f)
				{
					this.stewspeedboost -= 3f;
					base.StartCoroutine(this.SubtractBonus(4));
				}
				this.playerHealth -= 10f;
				return;
			}
			this.isFrozen = true;
			this.ServerSummonIceBox(lvl);
			this.canMove = false;
			this.canJump = false;
			this.canMoveCamera = false;
			base.gameObject.GetComponent<PlayerInventory>().initedInv = false;
			this.ArmsAni.speed = 0f;
			this.BodyAni.speed = 0f;
			float num = (float)lvl;
			base.StartCoroutine(this.FreezeOverlay(num / 2f));
			this.NonRpcDamagePlayer((float)(15 + lvl * 5), playerownner, "frostbolt");
			if (lvl == 2)
			{
				if (this.stewspeedboost > -3f)
				{
					this.stewspeedboost -= 3f;
					base.StartCoroutine(this.SubtractBonus(4));
					return;
				}
			}
			else if (lvl == 3 && this.stewspeedboost > -6f)
			{
				this.stewspeedboost -= 3f;
				base.StartCoroutine(this.SubtractBonus(4));
				this.stewspeedboost -= 3f;
				base.StartCoroutine(this.SubtractBonus(4));
			}
		}
	}

	// Token: 0x06000C09 RID: 3081 RVA: 0x00030B06 File Offset: 0x0002ED06
	[ServerRpc(RequireOwnership = false)]
	private void ServerSummonIceBox(int lvl)
	{
		this.RpcWriter___Server_ServerSummonIceBox_3316948804(lvl);
	}

	// Token: 0x06000C0A RID: 3082 RVA: 0x00030B14 File Offset: 0x0002ED14
	[ObserversRpc]
	private void ObserversSummonIceBox(int lvl)
	{
		this.RpcWriter___Observers_ObserversSummonIceBox_3316948804(lvl);
	}

	// Token: 0x06000C0B RID: 3083 RVA: 0x00030B2B File Offset: 0x0002ED2B
	private IEnumerator lerpAlphaVal(Material mat, GameObject icebox, float duration)
	{
		float timer = 0f;
		while (timer < 0.25f)
		{
			mat.SetFloat("_AlphaRemapMax", Mathf.Lerp(0f, 0.75f, timer * 4f));
			timer += Time.deltaTime;
			yield return null;
		}
		timer = 0f;
		while (timer < duration && !this.breakoutFireball)
		{
			yield return null;
			timer += Time.deltaTime;
		}
		if (this.breakoutFireball)
		{
			this.breakoutFireball = false;
			GameObject brokenIce = Object.Instantiate<GameObject>(this.BrokenIcePrefab, icebox.transform.position, icebox.transform.rotation);
			Object.Destroy(icebox);
			if (base.IsOwner)
			{
				this.isFrozen = false;
				this.canMove = true;
				this.canJump = true;
				this.canMoveCamera = true;
				base.gameObject.GetComponent<PlayerInventory>().initedInv = true;
				this.ArmsAni.speed = 1f;
				this.BodyAni.speed = 1f;
			}
			timer = 0f;
			MeshRenderer[] componentsInChildren = brokenIce.GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].material = mat;
			}
			yield return new WaitForSeconds(3f);
			while (timer < 5f)
			{
				mat.SetFloat("_AlphaRemapMax", Mathf.Lerp(0.75f, 0f, timer / 5f));
				timer += Time.deltaTime;
				yield return null;
			}
			mat.SetFloat("_AlphaRemapMax", 0f);
			Object.Destroy(brokenIce);
			brokenIce = null;
		}
		else
		{
			timer = 0f;
			while (timer < 0.25f)
			{
				mat.SetFloat("_AlphaRemapMax", Mathf.Lerp(0.75f, 0f, timer * 4f));
				timer += Time.deltaTime;
				yield return null;
			}
			mat.SetFloat("_AlphaRemapMax", 0f);
			Object.Destroy(icebox);
			if (base.IsOwner)
			{
				this.isFrozen = false;
				this.canMove = true;
				this.canJump = true;
				this.canMoveCamera = true;
				base.gameObject.GetComponent<PlayerInventory>().initedInv = true;
				this.ArmsAni.speed = 1f;
				this.BodyAni.speed = 1f;
			}
		}
		yield break;
	}

	// Token: 0x06000C0C RID: 3084 RVA: 0x00030B4F File Offset: 0x0002ED4F
	private IEnumerator FreezeOverlay(float duration)
	{
		float timer = 0f;
		while (timer < 0.2f)
		{
			this.Freezeoverlay.fullscreenPassMaterial.SetFloat("_x", Mathf.Lerp(0f, 0.5f, timer * 5f));
			timer += Time.deltaTime;
			yield return null;
		}
		yield return new WaitForSeconds(duration);
		timer = 0f;
		while (timer < 0.2f)
		{
			this.Freezeoverlay.fullscreenPassMaterial.SetFloat("_x", Mathf.Lerp(0.5f, 0f, timer * 5f));
			timer += Time.deltaTime;
			yield return null;
		}
		if (base.IsOwner)
		{
			this.ArmsAni.speed = 1f;
			this.BodyAni.speed = 1f;
		}
		this.Freezeoverlay.fullscreenPassMaterial.SetFloat("_x", 0f);
		yield break;
	}

	// Token: 0x06000C0D RID: 3085 RVA: 0x00030B65 File Offset: 0x0002ED65
	[ServerRpc(RequireOwnership = false)]
	private void ServerDeathRobes(int TeamNum)
	{
		this.RpcWriter___Server_ServerDeathRobes_3316948804(TeamNum);
	}

	// Token: 0x06000C0E RID: 3086 RVA: 0x00030B74 File Offset: 0x0002ED74
	[ObserversRpc]
	private void ObsDeathRobes(int TeamNum)
	{
		this.RpcWriter___Observers_ObsDeathRobes_3316948804(TeamNum);
	}

	// Token: 0x06000C0F RID: 3087 RVA: 0x00030B8B File Offset: 0x0002ED8B
	public void DamagePlayer(float dmg, GameObject sauce, string type)
	{
		this.playerdmg(dmg, sauce, type);
	}

	// Token: 0x06000C10 RID: 3088 RVA: 0x00030B96 File Offset: 0x0002ED96
	[ServerRpc(RequireOwnership = false)]
	private void playerdmg(float dmg, GameObject sauce, string type)
	{
		this.RpcWriter___Server_playerdmg_313507570(dmg, sauce, type);
	}

	// Token: 0x06000C11 RID: 3089 RVA: 0x00030BAC File Offset: 0x0002EDAC
	[ObserversRpc]
	private void playerdmgobserver(float dmg, GameObject sauce, string type)
	{
		this.RpcWriter___Observers_playerdmgobserver_313507570(dmg, sauce, type);
	}

	// Token: 0x06000C12 RID: 3090 RVA: 0x00030BCB File Offset: 0x0002EDCB
	public void ExcaliburDamagePlayer(float dmg, GameObject sauce)
	{
		this.Excaliburplayerdmg(dmg, sauce);
	}

	// Token: 0x06000C13 RID: 3091 RVA: 0x00030BD5 File Offset: 0x0002EDD5
	[ServerRpc(RequireOwnership = false)]
	private void Excaliburplayerdmg(float dmg, GameObject sauce)
	{
		this.RpcWriter___Server_Excaliburplayerdmg_2976230906(dmg, sauce);
	}

	// Token: 0x06000C14 RID: 3092 RVA: 0x00030BE8 File Offset: 0x0002EDE8
	[ObserversRpc]
	private void Excaliburplayerdmgobserver(float dmg, GameObject sauce)
	{
		this.RpcWriter___Observers_Excaliburplayerdmgobserver_2976230906(dmg, sauce);
	}

	// Token: 0x06000C15 RID: 3093 RVA: 0x00030C03 File Offset: 0x0002EE03
	[ServerRpc(RequireOwnership = false)]
	private void MakeLightning()
	{
		this.RpcWriter___Server_MakeLightning_2166136261();
	}

	// Token: 0x06000C16 RID: 3094 RVA: 0x00030C0B File Offset: 0x0002EE0B
	[ObserversRpc]
	private void MakeLightningObs()
	{
		this.RpcWriter___Observers_MakeLightningObs_2166136261();
	}

	// Token: 0x06000C17 RID: 3095 RVA: 0x00030C14 File Offset: 0x0002EE14
	public void NonRpcDamagePlayer(float dmg, GameObject sauce, string type)
	{
		if (base.IsOwner && this.playerHealth > 0f)
		{
			this.causeofdeath = type;
			this.playerHealth -= dmg;
			if (!this.isDead)
			{
				this.hurtyOverlayVal += dmg / 100f;
			}
			PlayerMovement playerMovement;
			if (sauce != null && sauce.TryGetComponent<PlayerMovement>(out playerMovement))
			{
				this.LastPersonToHitMe = playerMovement;
			}
			Mathf.Clamp(this.hurtyOverlayVal, 0f, 0.5f);
		}
	}

	// Token: 0x06000C18 RID: 3096 RVA: 0x00030C97 File Offset: 0x0002EE97
	public void AddKill()
	{
		this.ServerAddKill();
	}

	// Token: 0x06000C19 RID: 3097 RVA: 0x00030C9F File Offset: 0x0002EE9F
	[ServerRpc(RequireOwnership = false)]
	private void ServerAddKill()
	{
		this.RpcWriter___Server_ServerAddKill_2166136261();
	}

	// Token: 0x06000C1A RID: 3098 RVA: 0x00030CA7 File Offset: 0x0002EEA7
	[ObserversRpc]
	private void ObsAddKill()
	{
		this.RpcWriter___Observers_ObsAddKill_2166136261();
	}

	// Token: 0x06000C1B RID: 3099 RVA: 0x00030CAF File Offset: 0x0002EEAF
	[ServerRpc(RequireOwnership = false)]
	private void toggleBreakout()
	{
		this.RpcWriter___Server_toggleBreakout_2166136261();
	}

	// Token: 0x06000C1C RID: 3100 RVA: 0x00030CB7 File Offset: 0x0002EEB7
	[ObserversRpc]
	private void toggleBreakoutobs()
	{
		this.RpcWriter___Observers_toggleBreakoutobs_2166136261();
	}

	// Token: 0x06000C1D RID: 3101 RVA: 0x00030CC0 File Offset: 0x0002EEC0
	public void DarkBlastHit(GameObject dbowner)
	{
		this.playerHealth -= 12f;
		this.fireTimer += 1f;
		this.causeofdeath = "darkblast";
		PlayerMovement playerMovement;
		if (dbowner.TryGetComponent<PlayerMovement>(out playerMovement))
		{
			this.LastPersonToHitMe = playerMovement;
		}
	}

	// Token: 0x06000C1E RID: 3102 RVA: 0x00030D10 File Offset: 0x0002EF10
	public void ExplosionHit(Vector3 ExplosionPosition, bool skelebomb, GameObject fbowner, int lvl)
	{
		lvl = Mathf.Clamp(lvl, 0, 8);
		if (this.isFrozen)
		{
			this.toggleBreakout();
			return;
		}
		float num = Vector3.Distance(base.transform.position, ExplosionPosition);
		float num2 = Mathf.Clamp(65f - num * 10f, 5f, 100f) + (float)lvl;
		bool flag = false;
		PlayerMovement playerMovement;
		if (fbowner != null && fbowner.TryGetComponent<PlayerMovement>(out playerMovement) && playerMovement.checkowner())
		{
			flag = true;
			num2 /= 1.5f;
		}
		this.playerHealth -= num2;
		this.hurtyOverlayVal += num2 / 100f;
		Mathf.Clamp(this.hurtyOverlayVal, 0f, 0.5f);
		this.causeofdeath = "fireball";
		PlayerMovement playerMovement2;
		if (fbowner != null && fbowner.TryGetComponent<PlayerMovement>(out playerMovement2))
		{
			this.LastPersonToHitMe = playerMovement2;
		}
		if (!skelebomb && this.playerHealth > 0f)
		{
			Vector3 normalized = (base.transform.position - ExplosionPosition).normalized;
			if (flag)
			{
				this.fireTimer += 1.5f;
			}
			else
			{
				this.fireTimer += Mathf.Clamp(8f - num, 0f, 10f) + (float)(lvl / 2);
			}
			this.externalxmoveinput += normalized.x * 3f * ((Mathf.Clamp(10f - num, 0.5f, 10f) + (float)lvl / 1.3f) / 2f);
			this.externalzmoveinput += normalized.z * 3f * ((Mathf.Clamp(10f - num, 0.5f, 10f) + (float)lvl / 1.3f) / 2f);
			this.velocity.y = (float)(10 + Mathf.Clamp(lvl, 0, 7)) + (Mathf.Abs(base.transform.position.y - ExplosionPosition.y) + 1f);
			this.hitbymushroom = true;
		}
	}

	// Token: 0x06000C1F RID: 3103 RVA: 0x00030F28 File Offset: 0x0002F128
	public void LightningStrike(Vector3 ExplosionPosition, GameObject spellowner)
	{
		float num = Vector3.Distance(base.transform.position, ExplosionPosition);
		this.fireTimer += 9f - num;
		float num2 = Mathf.Clamp(62f - num * 5f, 5f, 100f);
		this.causeofdeath = "fireball";
		this.playerHealth -= num2;
		this.hurtyOverlayVal += num2 / 100f;
		Mathf.Clamp(this.hurtyOverlayVal, 0f, 0.5f);
		PlayerMovement playerMovement;
		if (this.playerHealth <= 0f && spellowner != null && spellowner.TryGetComponent<PlayerMovement>(out playerMovement))
		{
			this.LastPersonToHitMe = playerMovement;
		}
	}

	// Token: 0x06000C20 RID: 3104 RVA: 0x00030FE4 File Offset: 0x0002F1E4
	public void HitByDart()
	{
		if (Time.time - this.dartCDTimer > 1f)
		{
			this.causeofdeath = "dart";
			this.playerHealth -= 15f;
			this.hurtyOverlayVal += 0f;
			this.NauseatedTimer += 15f;
			this.dartCDTimer = Time.time;
		}
	}

	// Token: 0x06000C21 RID: 3105 RVA: 0x00031050 File Offset: 0x0002F250
	public void RespawnPlayer()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("hex");
		for (int i = 0; i < array.Length; i++)
		{
			Generator2D generator2D;
			if (array[i].TryGetComponent<Generator2D>(out generator2D))
			{
				generator2D.FrontDoor.tdgmr.DisableAll();
			}
		}
		GameObject.FindGameObjectWithTag("Weather").GetComponent<Light>().enabled = true;
		base.GetComponent<PlayerInventory>().SetSpawnSlotToTwo();
		this.canRecall = true;
		this.LastPersonToHitMe = null;
		this.externalxmoveinput = 0f;
		this.externalzmoveinput = 0f;
		this.characterController.height = 2.3f;
		this.playerHealth = 100f;
		this.isDead = false;
		this.PlayerArms.SetActive(true);
		base.transform.GetComponent<PlayerInventory>().PlayerRevived();
		this.eatenByFrog = false;
		this.FrogToung = null;
		this.canMoveCamera = true;
		this.isBeingLevitated = false;
		this.NauseatedTimer = 0f;
		this.fireTimer = 0f;
		this.FallTimer = 0f;
		this.UndoFrogRpc();
		this.EnableBody();
		this.ResetCam();
	}

	// Token: 0x06000C22 RID: 3106 RVA: 0x00031163 File Offset: 0x0002F363
	[ServerRpc(RequireOwnership = false)]
	private void EnableBody()
	{
		this.RpcWriter___Server_EnableBody_2166136261();
	}

	// Token: 0x06000C23 RID: 3107 RVA: 0x0003116C File Offset: 0x0002F36C
	[ObserversRpc]
	private void ObsEnableBody()
	{
		this.RpcWriter___Observers_ObsEnableBody_2166136261();
	}

	// Token: 0x06000C24 RID: 3108 RVA: 0x0003117F File Offset: 0x0002F37F
	[ServerRpc(RequireOwnership = false)]
	private void UndoFrogRpc()
	{
		this.RpcWriter___Server_UndoFrogRpc_2166136261();
	}

	// Token: 0x06000C25 RID: 3109 RVA: 0x00031187 File Offset: 0x0002F387
	[ObserversRpc]
	private void UndoFrogObsSync()
	{
		this.RpcWriter___Observers_UndoFrogObsSync_2166136261();
	}

	// Token: 0x06000C26 RID: 3110 RVA: 0x0003118F File Offset: 0x0002F38F
	public void SetOnFire()
	{
		this.SetOnFireServer();
	}

	// Token: 0x06000C27 RID: 3111 RVA: 0x00031197 File Offset: 0x0002F397
	[ServerRpc(RequireOwnership = false)]
	private void SetOnFireServer()
	{
		this.RpcWriter___Server_SetOnFireServer_2166136261();
	}

	// Token: 0x06000C28 RID: 3112 RVA: 0x0003119F File Offset: 0x0002F39F
	[ObserversRpc]
	private void SetOnFireObs()
	{
		this.RpcWriter___Observers_SetOnFireObs_2166136261();
	}

	// Token: 0x06000C29 RID: 3113 RVA: 0x000311A7 File Offset: 0x0002F3A7
	[ServerRpc(RequireOwnership = false)]
	private void ToggleParticles(bool val)
	{
		this.RpcWriter___Server_ToggleParticles_1140765316(val);
	}

	// Token: 0x06000C2A RID: 3114 RVA: 0x000311B3 File Offset: 0x0002F3B3
	[ObserversRpc]
	private void ObsToggleParticles(bool val)
	{
		this.RpcWriter___Observers_ObsToggleParticles_1140765316(val);
	}

	// Token: 0x06000C2B RID: 3115 RVA: 0x000311BF File Offset: 0x0002F3BF
	public void playPortalSound()
	{
		this.playPortalSoundServer();
	}

	// Token: 0x06000C2C RID: 3116 RVA: 0x000311C7 File Offset: 0x0002F3C7
	[ServerRpc(RequireOwnership = false)]
	private void playPortalSoundServer()
	{
		this.RpcWriter___Server_playPortalSoundServer_2166136261();
	}

	// Token: 0x06000C2D RID: 3117 RVA: 0x000311D0 File Offset: 0x0002F3D0
	[ObserversRpc]
	private void playPortalSoundObs()
	{
		this.RpcWriter___Observers_playPortalSoundObs_2166136261();
	}

	// Token: 0x06000C2E RID: 3118 RVA: 0x000311E4 File Offset: 0x0002F3E4
	public void DrinkStew(int stewid)
	{
		if (stewid == 0)
		{
			Camera.main.GetComponent<PlayerInteract>().leveluptxt("A surge of magical energy passes through you...");
			this.stewstaminabonus += 1f;
			this.stewspeedboost += 2f;
			this.soupsdrank += 1f;
			if (this.crystalCDReduction < 4)
			{
				this.level++;
				this.crystalCDReduction++;
				base.StartCoroutine(this.SubtractBonus(5));
			}
			base.StartCoroutine(this.SubtractBonus(0));
			return;
		}
		if (stewid == 1)
		{
			Camera.main.GetComponent<PlayerInteract>().leveluptxt("Your skin hardens like tree bark...");
			this.playerHealth += 50f;
			this.soupsdrank += 1f;
			return;
		}
		if (stewid == 2)
		{
			Camera.main.GetComponent<PlayerInteract>().leveluptxt("Your joints feel like rubber...");
			this.stewjumpbonus += 5f;
			this.soupsdrank += 1f;
			base.StartCoroutine(this.SubtractBonus(2));
			return;
		}
		if (stewid == 3)
		{
			Camera.main.GetComponent<PlayerInteract>().leveluptxt("Your tounge gains a mind of its own...");
			this.numberoflicks++;
			this.soupsdrank += 1f;
			base.StartCoroutine(this.FrogStewRoutine());
			return;
		}
		if (stewid == 4)
		{
			Camera.main.GetComponent<PlayerInteract>().leveluptxt("Your voice bellows from the mountain tops...");
			this.ServerToggleLoud();
			this.soupsdrank += 1f;
			return;
		}
		if (stewid == 5)
		{
			if (this.crystalCDReduction < 4)
			{
				this.level++;
				this.crystalCDReduction++;
				base.StartCoroutine(this.SubtractBonus(5));
			}
			this.pipessmoked += 1f;
		}
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x000313CB File Offset: 0x0002F5CB
	[ServerRpc(RequireOwnership = false)]
	private void ServerToggleLoud()
	{
		this.RpcWriter___Server_ServerToggleLoud_2166136261();
	}

	// Token: 0x06000C30 RID: 3120 RVA: 0x000313D4 File Offset: 0x0002F5D4
	[ObserversRpc]
	private void ToggleLoudEchoVoice()
	{
		this.RpcWriter___Observers_ToggleLoudEchoVoice_2166136261();
	}

	// Token: 0x06000C31 RID: 3121 RVA: 0x000313E7 File Offset: 0x0002F5E7
	private IEnumerator SubtractBonus(int stewid)
	{
		if (stewid == 4)
		{
			yield return new WaitForSeconds(4f);
		}
		else if (stewid == 5)
		{
			yield return new WaitForSeconds(15f);
		}
		else
		{
			yield return new WaitForSeconds(120f);
		}
		if (stewid == 0)
		{
			this.stewstaminabonus -= 1f;
			this.stewspeedboost -= 2f;
		}
		else if (stewid != 1)
		{
			if (stewid == 2)
			{
				this.stewjumpbonus -= 5f;
			}
			else if (stewid == 3)
			{
				this.stewspeedboost -= 1.5f;
			}
			else if (stewid == 4)
			{
				this.stewspeedboost += 3f;
			}
			else if (stewid == 5)
			{
				this.level--;
				this.crystalCDReduction--;
			}
		}
		yield break;
	}

	// Token: 0x06000C32 RID: 3122 RVA: 0x000313FD File Offset: 0x0002F5FD
	private IEnumerator FrogStewRoutine()
	{
		bool hashit = false;
		while (this.numberoflicks > 0)
		{
			this.portalSource.PlayOneShot(this.frogtoungesounds[0]);
			while (!hashit)
			{
				RaycastHit raycastHit;
				GetPlayerGameobject getPlayerGameobject;
				if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out raycastHit, 15f, this.playerLayer) && raycastHit.transform.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
				{
					this.FrogToungeAniServer(getPlayerGameobject.player);
					hashit = true;
					this.numberoflicks--;
				}
				yield return null;
			}
			hashit = false;
			yield return new WaitForSeconds(15f);
		}
		yield break;
	}

	// Token: 0x06000C33 RID: 3123 RVA: 0x0003140C File Offset: 0x0002F60C
	[ServerRpc(RequireOwnership = false)]
	private void FrogToungeAniServer(GameObject target)
	{
		this.RpcWriter___Server_FrogToungeAniServer_1934289915(target);
	}

	// Token: 0x06000C34 RID: 3124 RVA: 0x00031418 File Offset: 0x0002F618
	[ObserversRpc]
	private void FrogToungeAniObs(GameObject target)
	{
		this.RpcWriter___Observers_FrogToungeAniObs_1934289915(target);
	}

	// Token: 0x06000C35 RID: 3125 RVA: 0x00031424 File Offset: 0x0002F624
	private IEnumerator ToungeAniRoutine(GameObject target)
	{
		float timer = 0f;
		this.frogtoungee.SetActive(true);
		this.portalSource.PlayOneShot(this.frogtoungesounds[1]);
		while (timer < 0.34f)
		{
			timer += Time.deltaTime;
			this.frogbone.transform.position = Vector3.Lerp(this.frogbone.transform.position, new Vector3(target.transform.position.x, target.transform.position.y + 1.25f, target.transform.position.z), timer * 3f);
			yield return null;
		}
		target.GetComponent<PlayerMovement>().ApplyFrogForce(base.gameObject);
		target.GetComponent<PlayerInventory>().Drop();
		timer = 0f;
		this.portalSource.PlayOneShot(this.frogtoungesounds[2]);
		while (timer < 0.34f)
		{
			timer += Time.deltaTime;
			this.frogbone.transform.localPosition = Vector3.Lerp(this.frogbone.transform.localPosition, Vector3.zero, timer * 3f);
			yield return null;
		}
		this.frogtoungee.SetActive(false);
		yield break;
	}

	// Token: 0x06000C36 RID: 3126 RVA: 0x0003143C File Offset: 0x0002F63C
	public void ApplyFrogForce(GameObject FrogPlayerPos)
	{
		Vector3 vector = FrogPlayerPos.transform.position - base.transform.position;
		float num = Vector3.Distance(FrogPlayerPos.transform.position, base.transform.position);
		this.externalxmoveinput += num / Mathf.Clamp(0f, 20f, (num - vector.x) * 8f);
		this.externalzmoveinput += num / Mathf.Clamp(0f, 20f, (num - vector.z) * 8f);
		this.velocity.y = this.velocity.y + 5f;
		this.hitbymushroom = true;
	}

	// Token: 0x06000C37 RID: 3127 RVA: 0x000314F4 File Offset: 0x0002F6F4
	public void applyrecoil()
	{
		Vector3 vector = Camera.main.transform.forward * -1f;
		this.externalxmoveinput += vector.x * 7f;
		this.externalzmoveinput += vector.z * 7f;
		this.velocity.y = this.velocity.y + vector.y * 5f;
	}

	// Token: 0x06000C38 RID: 3128 RVA: 0x00031568 File Offset: 0x0002F768
	public void applydbrecoil()
	{
		if (base.IsOwner)
		{
			base.StartCoroutine(this.waitdbdelay());
		}
	}

	// Token: 0x06000C39 RID: 3129 RVA: 0x0003157F File Offset: 0x0002F77F
	private IEnumerator waitdbdelay()
	{
		yield return new WaitForSeconds(0.15f);
		Vector3 vector = Camera.main.transform.forward * -1f;
		this.externalxmoveinput += vector.x * 10f;
		this.externalzmoveinput += vector.z * 10f;
		this.velocity.y = this.velocity.y + vector.y * 5f;
		yield break;
	}

	// Token: 0x06000C3A RID: 3130 RVA: 0x0003158E File Offset: 0x0002F78E
	public void ShakeCam(float magnitude, float durationn)
	{
		this.shakemagnitude = magnitude;
		this.shaketime = durationn;
		this.shakefalloff = 0f;
	}

	// Token: 0x06000C3B RID: 3131 RVA: 0x000315A9 File Offset: 0x0002F7A9
	private void HPupdate(float val)
	{
		if (base.HasAuthority && Time.time - this.hpupdatecd > 1f && this.prevhp != val)
		{
			this.hpupdatecd = Time.time;
			this.prevhp = val;
			this.UpdateHP(val);
		}
	}

	// Token: 0x06000C3C RID: 3132 RVA: 0x000315E8 File Offset: 0x0002F7E8
	[ServerRpc(RequireOwnership = false)]
	private void UpdateHP(float newval)
	{
		this.RpcWriter___Server_UpdateHP_431000436(newval);
	}

	// Token: 0x06000C3D RID: 3133 RVA: 0x000315F4 File Offset: 0x0002F7F4
	[ObserversRpc]
	private void UpdateHpObs(float newval)
	{
		this.RpcWriter___Observers_UpdateHpObs_431000436(newval);
	}

	// Token: 0x06000C3E RID: 3134 RVA: 0x00031600 File Offset: 0x0002F800
	private IEnumerator RecallRoutine()
	{
		float timer = 0f;
		bool hasnotmoved = true;
		Vector3 startpos = base.transform.position;
		float starthp = this.playerHealth;
		this.visualrecall();
		this.portalSource.PlayOneShot(this.recall);
		while (timer < 5.1f)
		{
			if (Input.GetKey(this.Sholder.w2) || Input.GetKey(this.Sholder.a2) || Input.GetKey(this.Sholder.s2) || Input.GetKey(this.Sholder.d2) || Input.GetKey(KeyCode.Space))
			{
				hasnotmoved = false;
				break;
			}
			if (Vector3.Distance(base.transform.position, startpos) > 0.25f || this.playerHealth < starthp)
			{
				hasnotmoved = false;
				break;
			}
			this.recallmesh.material.SetVector("_tiling", new Vector2(1f, Mathf.Lerp(-1.5f, -1.05f, timer / 5f)));
			yield return null;
			timer += Time.deltaTime;
		}
		this.portalSource.Stop();
		if (Vector3.Distance(base.transform.position, startpos) > 0.25f)
		{
			hasnotmoved = false;
		}
		this.recallmesh.material.SetVector("_tiling", new Vector2(1f, -1.5f));
		if (hasnotmoved && !this.istutorial && this.canMove && this.playerHealth >= starthp)
		{
			this.recallCD = Time.time;
			if (this.playerHealth < 100f)
			{
				this.playerHealth = 100f;
			}
			base.StartCoroutine(this.TelePlayer());
		}
		yield break;
	}

	// Token: 0x06000C3F RID: 3135 RVA: 0x0003160F File Offset: 0x0002F80F
	[ServerRpc(RequireOwnership = false)]
	private void visualrecall()
	{
		this.RpcWriter___Server_visualrecall_2166136261();
	}

	// Token: 0x06000C40 RID: 3136 RVA: 0x00031617 File Offset: 0x0002F817
	[ObserversRpc]
	private void obsrecallvisuals()
	{
		this.RpcWriter___Observers_obsrecallvisuals_2166136261();
	}

	// Token: 0x06000C41 RID: 3137 RVA: 0x0003161F File Offset: 0x0002F81F
	private IEnumerator recallvisuals()
	{
		float timer = 0f;
		Vector3 startpos = base.transform.position;
		while (timer < 4.9f && Vector3.Distance(base.transform.position, startpos) <= 0.25f)
		{
			this.recallmesh.material.SetVector("_tiling", new Vector2(1f, Mathf.Lerp(-1.5f, -1.05f, timer / 4.9f)));
			yield return null;
			timer += Time.deltaTime;
		}
		this.recallmesh.material.SetVector("_tiling", new Vector2(1f, -1.5f));
		yield break;
	}

	// Token: 0x06000C42 RID: 3138 RVA: 0x00031630 File Offset: 0x0002F830
	public override void OnStopClient()
	{
		base.OnStopClient();
		if (this.Hurtoverlay != null)
		{
			this.Hurtoverlay.fullscreenPassMaterial.SetFloat("_x", 0f);
		}
		if (this.Freezeoverlay != null)
		{
			this.Freezeoverlay.fullscreenPassMaterial.SetFloat("_x", 0f);
		}
		if (this.Nausteadoverlay != null)
		{
			this.Nausteadoverlay.fullscreenPassMaterial.SetFloat("_x", 0f);
		}
	}

	// Token: 0x06000C43 RID: 3139 RVA: 0x000316A9 File Offset: 0x0002F8A9
	public void nonnetworkedheal(float heal)
	{
		if (this.playerHealth < 100f)
		{
			this.playerHealth += heal;
			if (this.playerHealth > 100f)
			{
				this.playerHealth = 100f;
			}
		}
	}

	// Token: 0x06000C45 RID: 3141 RVA: 0x000317E0 File Offset: 0x0002F9E0
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyPlayerMovementAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyPlayerMovementAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ServerSetName_3615296227));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_ObsSetName_3615296227));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_ServerReqTeam_2166136261));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ObsReqTeam_2166136261));
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_ServerSwitchTeam_3316948804));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_ObsSwitchTeam_3316948804));
		base.RegisterServerRpc(6U, new ServerRpcDelegate(this.RpcReader___Server_FrogRpc_2166136261));
		base.RegisterObserversRpc(7U, new ClientRpcDelegate(this.RpcReader___Observers_FrogObsSync_2166136261));
		base.RegisterServerRpc(8U, new ServerRpcDelegate(this.RpcReader___Server_togglevbt_2166136261));
		base.RegisterObserversRpc(9U, new ClientRpcDelegate(this.RpcReader___Observers_obstogglevbt_2166136261));
		base.RegisterServerRpc(10U, new ServerRpcDelegate(this.RpcReader___Server_DisableHitCols_2166136261));
		base.RegisterObserversRpc(11U, new ClientRpcDelegate(this.RpcReader___Observers_ObsDisableHBCols_2166136261));
		base.RegisterServerRpc(12U, new ServerRpcDelegate(this.RpcReader___Server_EnableHBCols_2166136261));
		base.RegisterObserversRpc(13U, new ClientRpcDelegate(this.RpcReader___Observers_ObsEnableHBCols_2166136261));
		base.RegisterServerRpc(14U, new ServerRpcDelegate(this.RpcReader___Server_ServerSyncSpine_431000436));
		base.RegisterObserversRpc(15U, new ClientRpcDelegate(this.RpcReader___Observers_ObsSyncSpine_431000436));
		base.RegisterServerRpc(16U, new ServerRpcDelegate(this.RpcReader___Server_ServerBoing_2166136261));
		base.RegisterObserversRpc(17U, new ClientRpcDelegate(this.RpcReader___Observers_ObsBoing_2166136261));
		base.RegisterServerRpc(18U, new ServerRpcDelegate(this.RpcReader___Server_ServerCallShrinkPlayer_2166136261));
		base.RegisterObserversRpc(19U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversCallShrinkPlayer_2166136261));
		base.RegisterServerRpc(20U, new ServerRpcDelegate(this.RpcReader___Server_ServerCallSummonIceBox_1011425610));
		base.RegisterObserversRpc(21U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversCallSummonIceBox_1011425610));
		base.RegisterServerRpc(22U, new ServerRpcDelegate(this.RpcReader___Server_ServerSummonIceBox_3316948804));
		base.RegisterObserversRpc(23U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversSummonIceBox_3316948804));
		base.RegisterServerRpc(24U, new ServerRpcDelegate(this.RpcReader___Server_ServerDeathRobes_3316948804));
		base.RegisterObserversRpc(25U, new ClientRpcDelegate(this.RpcReader___Observers_ObsDeathRobes_3316948804));
		base.RegisterServerRpc(26U, new ServerRpcDelegate(this.RpcReader___Server_playerdmg_313507570));
		base.RegisterObserversRpc(27U, new ClientRpcDelegate(this.RpcReader___Observers_playerdmgobserver_313507570));
		base.RegisterServerRpc(28U, new ServerRpcDelegate(this.RpcReader___Server_Excaliburplayerdmg_2976230906));
		base.RegisterObserversRpc(29U, new ClientRpcDelegate(this.RpcReader___Observers_Excaliburplayerdmgobserver_2976230906));
		base.RegisterServerRpc(30U, new ServerRpcDelegate(this.RpcReader___Server_MakeLightning_2166136261));
		base.RegisterObserversRpc(31U, new ClientRpcDelegate(this.RpcReader___Observers_MakeLightningObs_2166136261));
		base.RegisterServerRpc(32U, new ServerRpcDelegate(this.RpcReader___Server_ServerAddKill_2166136261));
		base.RegisterObserversRpc(33U, new ClientRpcDelegate(this.RpcReader___Observers_ObsAddKill_2166136261));
		base.RegisterServerRpc(34U, new ServerRpcDelegate(this.RpcReader___Server_toggleBreakout_2166136261));
		base.RegisterObserversRpc(35U, new ClientRpcDelegate(this.RpcReader___Observers_toggleBreakoutobs_2166136261));
		base.RegisterServerRpc(36U, new ServerRpcDelegate(this.RpcReader___Server_EnableBody_2166136261));
		base.RegisterObserversRpc(37U, new ClientRpcDelegate(this.RpcReader___Observers_ObsEnableBody_2166136261));
		base.RegisterServerRpc(38U, new ServerRpcDelegate(this.RpcReader___Server_UndoFrogRpc_2166136261));
		base.RegisterObserversRpc(39U, new ClientRpcDelegate(this.RpcReader___Observers_UndoFrogObsSync_2166136261));
		base.RegisterServerRpc(40U, new ServerRpcDelegate(this.RpcReader___Server_SetOnFireServer_2166136261));
		base.RegisterObserversRpc(41U, new ClientRpcDelegate(this.RpcReader___Observers_SetOnFireObs_2166136261));
		base.RegisterServerRpc(42U, new ServerRpcDelegate(this.RpcReader___Server_ToggleParticles_1140765316));
		base.RegisterObserversRpc(43U, new ClientRpcDelegate(this.RpcReader___Observers_ObsToggleParticles_1140765316));
		base.RegisterServerRpc(44U, new ServerRpcDelegate(this.RpcReader___Server_playPortalSoundServer_2166136261));
		base.RegisterObserversRpc(45U, new ClientRpcDelegate(this.RpcReader___Observers_playPortalSoundObs_2166136261));
		base.RegisterServerRpc(46U, new ServerRpcDelegate(this.RpcReader___Server_ServerToggleLoud_2166136261));
		base.RegisterObserversRpc(47U, new ClientRpcDelegate(this.RpcReader___Observers_ToggleLoudEchoVoice_2166136261));
		base.RegisterServerRpc(48U, new ServerRpcDelegate(this.RpcReader___Server_FrogToungeAniServer_1934289915));
		base.RegisterObserversRpc(49U, new ClientRpcDelegate(this.RpcReader___Observers_FrogToungeAniObs_1934289915));
		base.RegisterServerRpc(50U, new ServerRpcDelegate(this.RpcReader___Server_UpdateHP_431000436));
		base.RegisterObserversRpc(51U, new ClientRpcDelegate(this.RpcReader___Observers_UpdateHpObs_431000436));
		base.RegisterServerRpc(52U, new ServerRpcDelegate(this.RpcReader___Server_visualrecall_2166136261));
		base.RegisterObserversRpc(53U, new ClientRpcDelegate(this.RpcReader___Observers_obsrecallvisuals_2166136261));
	}

	// Token: 0x06000C46 RID: 3142 RVA: 0x00031CD8 File Offset: 0x0002FED8
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LatePlayerMovementAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LatePlayerMovementAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000C47 RID: 3143 RVA: 0x00031CEB File Offset: 0x0002FEEB
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000C48 RID: 3144 RVA: 0x00031CFC File Offset: 0x0002FEFC
	private void RpcWriter___Server_ServerSetName_3615296227(string namee)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteString(namee);
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000C49 RID: 3145 RVA: 0x00031D6E File Offset: 0x0002FF6E
	private void RpcLogic___ServerSetName_3615296227(string namee)
	{
		this.ObsSetName(namee);
	}

	// Token: 0x06000C4A RID: 3146 RVA: 0x00031D78 File Offset: 0x0002FF78
	private void RpcReader___Server_ServerSetName_3615296227(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		string text = PooledReader0.ReadString();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSetName_3615296227(text);
	}

	// Token: 0x06000C4B RID: 3147 RVA: 0x00031DAC File Offset: 0x0002FFAC
	private void RpcWriter___Observers_ObsSetName_3615296227(string namee)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteString(namee);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000C4C RID: 3148 RVA: 0x00031E30 File Offset: 0x00030030
	private void RpcLogic___ObsSetName_3615296227(string namee)
	{
		this.playerNameText.text = this.ClampString(namee, 10);
		this.playername = namee;
		PlayerMovement playerMovement;
		if (Camera.main.transform.parent.TryGetComponent<PlayerMovement>(out playerMovement) && this.playerTeam != playerMovement.playerTeam)
		{
			this.playerNameText.color = this.RedName;
			return;
		}
		Transform map = GameObject.FindGameObjectWithTag("map").GetComponent<GetMap>().map;
		GameObject gameObject = Object.Instantiate<GameObject>(this.playermapicon, map);
		gameObject.GetComponent<PlayerMapIconController>().setname(namee);
		gameObject.GetComponent<PlayerMapIconController>().playerpos = base.transform;
	}

	// Token: 0x06000C4D RID: 3149 RVA: 0x00031ED0 File Offset: 0x000300D0
	private void RpcReader___Observers_ObsSetName_3615296227(PooledReader PooledReader0, Channel channel)
	{
		string text = PooledReader0.ReadString();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsSetName_3615296227(text);
	}

	// Token: 0x06000C4E RID: 3150 RVA: 0x00031F04 File Offset: 0x00030104
	private void RpcWriter___Server_ServerReqTeam_2166136261()
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

	// Token: 0x06000C4F RID: 3151 RVA: 0x00031F69 File Offset: 0x00030169
	private void RpcLogic___ServerReqTeam_2166136261()
	{
		this.ObsReqTeam();
	}

	// Token: 0x06000C50 RID: 3152 RVA: 0x00031F74 File Offset: 0x00030174
	private void RpcReader___Server_ServerReqTeam_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerReqTeam_2166136261();
	}

	// Token: 0x06000C51 RID: 3153 RVA: 0x00031F94 File Offset: 0x00030194
	private void RpcWriter___Observers_ObsReqTeam_2166136261()
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

	// Token: 0x06000C52 RID: 3154 RVA: 0x00032008 File Offset: 0x00030208
	private void RpcLogic___ObsReqTeam_2166136261()
	{
		this.SwitchTeam();
	}

	// Token: 0x06000C53 RID: 3155 RVA: 0x00032010 File Offset: 0x00030210
	private void RpcReader___Observers_ObsReqTeam_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsReqTeam_2166136261();
	}

	// Token: 0x06000C54 RID: 3156 RVA: 0x00032030 File Offset: 0x00030230
	private void RpcWriter___Server_ServerSwitchTeam_3316948804(int tn)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(tn);
		base.SendServerRpc(4U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000C55 RID: 3157 RVA: 0x000320A2 File Offset: 0x000302A2
	private void RpcLogic___ServerSwitchTeam_3316948804(int tn)
	{
		this.ObsSwitchTeam(tn);
	}

	// Token: 0x06000C56 RID: 3158 RVA: 0x000320AC File Offset: 0x000302AC
	private void RpcReader___Server_ServerSwitchTeam_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSwitchTeam_3316948804(num);
	}

	// Token: 0x06000C57 RID: 3159 RVA: 0x000320E0 File Offset: 0x000302E0
	private void RpcWriter___Observers_ObsSwitchTeam_3316948804(int tn)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(tn);
		base.SendObserversRpc(5U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000C58 RID: 3160 RVA: 0x00032161 File Offset: 0x00030361
	private void RpcLogic___ObsSwitchTeam_3316948804(int tn)
	{
		this.playerTeam = tn;
		this.setTeamObservers(this.playerTeam);
	}

	// Token: 0x06000C59 RID: 3161 RVA: 0x00032178 File Offset: 0x00030378
	private void RpcReader___Observers_ObsSwitchTeam_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsSwitchTeam_3316948804(num);
	}

	// Token: 0x06000C5A RID: 3162 RVA: 0x000321AC File Offset: 0x000303AC
	private void RpcWriter___Server_FrogRpc_2166136261()
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

	// Token: 0x06000C5B RID: 3163 RVA: 0x00032211 File Offset: 0x00030411
	private void RpcLogic___FrogRpc_2166136261()
	{
		this.FrogObsSync();
	}

	// Token: 0x06000C5C RID: 3164 RVA: 0x0003221C File Offset: 0x0003041C
	private void RpcReader___Server_FrogRpc_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___FrogRpc_2166136261();
	}

	// Token: 0x06000C5D RID: 3165 RVA: 0x0003223C File Offset: 0x0003043C
	private void RpcWriter___Observers_FrogObsSync_2166136261()
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

	// Token: 0x06000C5E RID: 3166 RVA: 0x000322B0 File Offset: 0x000304B0
	private void RpcLogic___FrogObsSync_2166136261()
	{
		this.WizardTrio.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
	}

	// Token: 0x06000C5F RID: 3167 RVA: 0x000322D8 File Offset: 0x000304D8
	private void RpcReader___Observers_FrogObsSync_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___FrogObsSync_2166136261();
	}

	// Token: 0x06000C60 RID: 3168 RVA: 0x000322F8 File Offset: 0x000304F8
	private void RpcWriter___Server_togglevbt_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(8U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000C61 RID: 3169 RVA: 0x0003235D File Offset: 0x0003055D
	private void RpcLogic___togglevbt_2166136261()
	{
		this.obstogglevbt();
	}

	// Token: 0x06000C62 RID: 3170 RVA: 0x00032368 File Offset: 0x00030568
	private void RpcReader___Server_togglevbt_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___togglevbt_2166136261();
	}

	// Token: 0x06000C63 RID: 3171 RVA: 0x00032388 File Offset: 0x00030588
	private void RpcWriter___Observers_obstogglevbt_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(9U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000C64 RID: 3172 RVA: 0x000323FC File Offset: 0x000305FC
	private void RpcLogic___obstogglevbt_2166136261()
	{
		this.speakericon.SetActive(!this.speakericon.activeSelf);
	}

	// Token: 0x06000C65 RID: 3173 RVA: 0x00032418 File Offset: 0x00030618
	private void RpcReader___Observers_obstogglevbt_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___obstogglevbt_2166136261();
	}

	// Token: 0x06000C66 RID: 3174 RVA: 0x00032438 File Offset: 0x00030638
	private void RpcWriter___Server_DisableHitCols_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(10U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000C67 RID: 3175 RVA: 0x0003249D File Offset: 0x0003069D
	private void RpcLogic___DisableHitCols_2166136261()
	{
		this.ObsDisableHBCols();
	}

	// Token: 0x06000C68 RID: 3176 RVA: 0x000324A8 File Offset: 0x000306A8
	private void RpcReader___Server_DisableHitCols_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___DisableHitCols_2166136261();
	}

	// Token: 0x06000C69 RID: 3177 RVA: 0x000324C8 File Offset: 0x000306C8
	private void RpcWriter___Observers_ObsDisableHBCols_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(11U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000C6A RID: 3178 RVA: 0x0003253C File Offset: 0x0003073C
	private void RpcLogic___ObsDisableHBCols_2166136261()
	{
		if (this.characterController != null && !this.eatenByFrog)
		{
			this.characterController.enabled = false;
		}
		Collider[] array = this.hitboxcols;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
	}

	// Token: 0x06000C6B RID: 3179 RVA: 0x0003258C File Offset: 0x0003078C
	private void RpcReader___Observers_ObsDisableHBCols_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsDisableHBCols_2166136261();
	}

	// Token: 0x06000C6C RID: 3180 RVA: 0x000325AC File Offset: 0x000307AC
	private void RpcWriter___Server_EnableHBCols_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(12U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000C6D RID: 3181 RVA: 0x00032611 File Offset: 0x00030811
	private void RpcLogic___EnableHBCols_2166136261()
	{
		this.ObsEnableHBCols();
	}

	// Token: 0x06000C6E RID: 3182 RVA: 0x0003261C File Offset: 0x0003081C
	private void RpcReader___Server_EnableHBCols_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___EnableHBCols_2166136261();
	}

	// Token: 0x06000C6F RID: 3183 RVA: 0x0003263C File Offset: 0x0003083C
	private void RpcWriter___Observers_ObsEnableHBCols_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(13U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000C70 RID: 3184 RVA: 0x000326B0 File Offset: 0x000308B0
	private void RpcLogic___ObsEnableHBCols_2166136261()
	{
		if (base.IsOwner)
		{
			if (this.characterController != null)
			{
				this.characterController.enabled = true;
				return;
			}
		}
		else
		{
			Collider[] array = this.hitboxcols;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
		}
	}

	// Token: 0x06000C71 RID: 3185 RVA: 0x00032700 File Offset: 0x00030900
	private void RpcReader___Observers_ObsEnableHBCols_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsEnableHBCols_2166136261();
	}

	// Token: 0x06000C72 RID: 3186 RVA: 0x00032720 File Offset: 0x00030920
	private void RpcWriter___Server_ServerSyncSpine_431000436(float val)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteSingle(val);
		base.SendServerRpc(14U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000C73 RID: 3187 RVA: 0x00032792 File Offset: 0x00030992
	private void RpcLogic___ServerSyncSpine_431000436(float val)
	{
		this.ObsSyncSpine(val);
	}

	// Token: 0x06000C74 RID: 3188 RVA: 0x0003279C File Offset: 0x0003099C
	private void RpcReader___Server_ServerSyncSpine_431000436(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		float num = PooledReader0.ReadSingle();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSyncSpine_431000436(num);
	}

	// Token: 0x06000C75 RID: 3189 RVA: 0x000327D0 File Offset: 0x000309D0
	private void RpcWriter___Observers_ObsSyncSpine_431000436(float val)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteSingle(val);
		base.SendObserversRpc(15U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000C76 RID: 3190 RVA: 0x00032851 File Offset: 0x00030A51
	private void RpcLogic___ObsSyncSpine_431000436(float val)
	{
		this.spinesync.lerptarget = val;
	}

	// Token: 0x06000C77 RID: 3191 RVA: 0x00032860 File Offset: 0x00030A60
	private void RpcReader___Observers_ObsSyncSpine_431000436(PooledReader PooledReader0, Channel channel)
	{
		float num = PooledReader0.ReadSingle();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsSyncSpine_431000436(num);
	}

	// Token: 0x06000C78 RID: 3192 RVA: 0x00032894 File Offset: 0x00030A94
	private void RpcWriter___Server_ServerBoing_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(16U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000C79 RID: 3193 RVA: 0x000328F9 File Offset: 0x00030AF9
	private void RpcLogic___ServerBoing_2166136261()
	{
		this.ObsBoing();
	}

	// Token: 0x06000C7A RID: 3194 RVA: 0x00032904 File Offset: 0x00030B04
	private void RpcReader___Server_ServerBoing_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerBoing_2166136261();
	}

	// Token: 0x06000C7B RID: 3195 RVA: 0x00032924 File Offset: 0x00030B24
	private void RpcWriter___Observers_ObsBoing_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(17U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000C7C RID: 3196 RVA: 0x00032998 File Offset: 0x00030B98
	private void RpcLogic___ObsBoing_2166136261()
	{
		this.playerAudio.PlayBoing();
	}

	// Token: 0x06000C7D RID: 3197 RVA: 0x000329A8 File Offset: 0x00030BA8
	private void RpcReader___Observers_ObsBoing_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsBoing_2166136261();
	}

	// Token: 0x06000C7E RID: 3198 RVA: 0x000329C8 File Offset: 0x00030BC8
	private void RpcWriter___Server_ServerCallShrinkPlayer_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(18U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000C7F RID: 3199 RVA: 0x00032A2D File Offset: 0x00030C2D
	private void RpcLogic___ServerCallShrinkPlayer_2166136261()
	{
		this.ObserversCallShrinkPlayer();
	}

	// Token: 0x06000C80 RID: 3200 RVA: 0x00032A38 File Offset: 0x00030C38
	private void RpcReader___Server_ServerCallShrinkPlayer_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerCallShrinkPlayer_2166136261();
	}

	// Token: 0x06000C81 RID: 3201 RVA: 0x00032A58 File Offset: 0x00030C58
	private void RpcWriter___Observers_ObserversCallShrinkPlayer_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(19U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000C82 RID: 3202 RVA: 0x00032ACC File Offset: 0x00030CCC
	private void RpcLogic___ObserversCallShrinkPlayer_2166136261()
	{
		this.ShrinkPlayer();
	}

	// Token: 0x06000C83 RID: 3203 RVA: 0x00032AD4 File Offset: 0x00030CD4
	private void RpcReader___Observers_ObserversCallShrinkPlayer_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversCallShrinkPlayer_2166136261();
	}

	// Token: 0x06000C84 RID: 3204 RVA: 0x00032AF4 File Offset: 0x00030CF4
	private void RpcWriter___Server_ServerCallSummonIceBox_1011425610(int lvl, GameObject playerownner)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(lvl);
		pooledWriter.WriteGameObject(playerownner);
		base.SendServerRpc(20U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000C85 RID: 3205 RVA: 0x00032B73 File Offset: 0x00030D73
	private void RpcLogic___ServerCallSummonIceBox_1011425610(int lvl, GameObject playerownner)
	{
		this.ObserversCallSummonIceBox(lvl, playerownner);
	}

	// Token: 0x06000C86 RID: 3206 RVA: 0x00032B80 File Offset: 0x00030D80
	private void RpcReader___Server_ServerCallSummonIceBox_1011425610(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerCallSummonIceBox_1011425610(num, gameObject);
	}

	// Token: 0x06000C87 RID: 3207 RVA: 0x00032BC4 File Offset: 0x00030DC4
	private void RpcWriter___Observers_ObserversCallSummonIceBox_1011425610(int lvl, GameObject playerownner)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(lvl);
		pooledWriter.WriteGameObject(playerownner);
		base.SendObserversRpc(21U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000C88 RID: 3208 RVA: 0x00032C52 File Offset: 0x00030E52
	private void RpcLogic___ObserversCallSummonIceBox_1011425610(int lvl, GameObject playerownner)
	{
		this.SummonIceBox(lvl, playerownner);
	}

	// Token: 0x06000C89 RID: 3209 RVA: 0x00032C5C File Offset: 0x00030E5C
	private void RpcReader___Observers_ObserversCallSummonIceBox_1011425610(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversCallSummonIceBox_1011425610(num, gameObject);
	}

	// Token: 0x06000C8A RID: 3210 RVA: 0x00032CA0 File Offset: 0x00030EA0
	private void RpcWriter___Server_ServerSummonIceBox_3316948804(int lvl)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(lvl);
		base.SendServerRpc(22U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000C8B RID: 3211 RVA: 0x00032D12 File Offset: 0x00030F12
	private void RpcLogic___ServerSummonIceBox_3316948804(int lvl)
	{
		this.ObserversSummonIceBox(lvl);
	}

	// Token: 0x06000C8C RID: 3212 RVA: 0x00032D1C File Offset: 0x00030F1C
	private void RpcReader___Server_ServerSummonIceBox_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSummonIceBox_3316948804(num);
	}

	// Token: 0x06000C8D RID: 3213 RVA: 0x00032D50 File Offset: 0x00030F50
	private void RpcWriter___Observers_ObserversSummonIceBox_3316948804(int lvl)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(lvl);
		base.SendObserversRpc(23U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000C8E RID: 3214 RVA: 0x00032DD4 File Offset: 0x00030FD4
	private void RpcLogic___ObserversSummonIceBox_3316948804(int lvl)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.icebox, new Vector3(base.transform.position.x, base.transform.position.y + 1.5f, base.transform.position.z), Quaternion.Euler(-90f, base.transform.rotation.y, 0f));
		Material material = gameObject.GetComponent<Renderer>().material;
		base.StartCoroutine(this.lerpAlphaVal(material, gameObject, (float)Mathf.Clamp(lvl, 2, 4)));
	}

	// Token: 0x06000C8F RID: 3215 RVA: 0x00032E6C File Offset: 0x0003106C
	private void RpcReader___Observers_ObserversSummonIceBox_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversSummonIceBox_3316948804(num);
	}

	// Token: 0x06000C90 RID: 3216 RVA: 0x00032EA0 File Offset: 0x000310A0
	private void RpcWriter___Server_ServerDeathRobes_3316948804(int TeamNum)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(TeamNum);
		base.SendServerRpc(24U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000C91 RID: 3217 RVA: 0x00032F12 File Offset: 0x00031112
	private void RpcLogic___ServerDeathRobes_3316948804(int TeamNum)
	{
		this.ObsDeathRobes(TeamNum);
	}

	// Token: 0x06000C92 RID: 3218 RVA: 0x00032F1C File Offset: 0x0003111C
	private void RpcReader___Server_ServerDeathRobes_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerDeathRobes_3316948804(num);
	}

	// Token: 0x06000C93 RID: 3219 RVA: 0x00032F50 File Offset: 0x00031150
	private void RpcWriter___Observers_ObsDeathRobes_3316948804(int TeamNum)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(TeamNum);
		base.SendObserversRpc(25U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000C94 RID: 3220 RVA: 0x00032FD4 File Offset: 0x000311D4
	private void RpcLogic___ObsDeathRobes_3316948804(int TeamNum)
	{
		if (TeamNum == 0)
		{
			Object.Instantiate<GameObject>(this.SorcererDeth, base.transform.position, base.transform.rotation);
			Object.Instantiate<GameObject>(this.SorcererDeth2, base.transform.position, base.transform.rotation);
		}
		else if (TeamNum == 1)
		{
			Object.Instantiate<GameObject>(this.ShadowMageDeth, base.transform.position, base.transform.rotation);
		}
		else
		{
			Object.Instantiate<GameObject>(this.WarlockDeth, base.transform.position, base.transform.rotation);
		}
		this.wizardBody[TeamNum].enabled = false;
		this.portalSource.PlayOneShot(this.playerDeth);
		this.isDead = true;
		this.playerNameText.transform.parent.parent.gameObject.SetActive(false);
	}

	// Token: 0x06000C95 RID: 3221 RVA: 0x000330BC File Offset: 0x000312BC
	private void RpcReader___Observers_ObsDeathRobes_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsDeathRobes_3316948804(num);
	}

	// Token: 0x06000C96 RID: 3222 RVA: 0x000330F0 File Offset: 0x000312F0
	private void RpcWriter___Server_playerdmg_313507570(float dmg, GameObject sauce, string type)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteSingle(dmg);
		pooledWriter.WriteGameObject(sauce);
		pooledWriter.WriteString(type);
		base.SendServerRpc(26U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000C97 RID: 3223 RVA: 0x0003317C File Offset: 0x0003137C
	private void RpcLogic___playerdmg_313507570(float dmg, GameObject sauce, string type)
	{
		this.playerdmgobserver(dmg, sauce, type);
	}

	// Token: 0x06000C98 RID: 3224 RVA: 0x00033188 File Offset: 0x00031388
	private void RpcReader___Server_playerdmg_313507570(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		float num = PooledReader0.ReadSingle();
		GameObject gameObject = PooledReader0.ReadGameObject();
		string text = PooledReader0.ReadString();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___playerdmg_313507570(num, gameObject, text);
	}

	// Token: 0x06000C99 RID: 3225 RVA: 0x000331DC File Offset: 0x000313DC
	private void RpcWriter___Observers_playerdmgobserver_313507570(float dmg, GameObject sauce, string type)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteSingle(dmg);
		pooledWriter.WriteGameObject(sauce);
		pooledWriter.WriteString(type);
		base.SendObserversRpc(27U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000C9A RID: 3226 RVA: 0x00033278 File Offset: 0x00031478
	private void RpcLogic___playerdmgobserver_313507570(float dmg, GameObject sauce, string type)
	{
		if (base.IsOwner)
		{
			this.causeofdeath = type;
			this.playerHealth -= dmg;
			PlayerMovement playerMovement;
			if (sauce != null && sauce.TryGetComponent<PlayerMovement>(out playerMovement))
			{
				this.LastPersonToHitMe = playerMovement;
			}
			if (!this.isDead)
			{
				this.hurtyOverlayVal += dmg / 100f;
			}
			Mathf.Clamp(this.hurtyOverlayVal, 0f, 0.5f);
		}
	}

	// Token: 0x06000C9B RID: 3227 RVA: 0x000332F0 File Offset: 0x000314F0
	private void RpcReader___Observers_playerdmgobserver_313507570(PooledReader PooledReader0, Channel channel)
	{
		float num = PooledReader0.ReadSingle();
		GameObject gameObject = PooledReader0.ReadGameObject();
		string text = PooledReader0.ReadString();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___playerdmgobserver_313507570(num, gameObject, text);
	}

	// Token: 0x06000C9C RID: 3228 RVA: 0x00033344 File Offset: 0x00031544
	private void RpcWriter___Server_Excaliburplayerdmg_2976230906(float dmg, GameObject sauce)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteSingle(dmg);
		pooledWriter.WriteGameObject(sauce);
		base.SendServerRpc(28U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000C9D RID: 3229 RVA: 0x000333C3 File Offset: 0x000315C3
	private void RpcLogic___Excaliburplayerdmg_2976230906(float dmg, GameObject sauce)
	{
		this.Excaliburplayerdmgobserver(dmg, sauce);
	}

	// Token: 0x06000C9E RID: 3230 RVA: 0x000333D0 File Offset: 0x000315D0
	private void RpcReader___Server_Excaliburplayerdmg_2976230906(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		float num = PooledReader0.ReadSingle();
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___Excaliburplayerdmg_2976230906(num, gameObject);
	}

	// Token: 0x06000C9F RID: 3231 RVA: 0x00033414 File Offset: 0x00031614
	private void RpcWriter___Observers_Excaliburplayerdmgobserver_2976230906(float dmg, GameObject sauce)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteSingle(dmg);
		pooledWriter.WriteGameObject(sauce);
		base.SendObserversRpc(29U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000CA0 RID: 3232 RVA: 0x000334A4 File Offset: 0x000316A4
	private void RpcLogic___Excaliburplayerdmgobserver_2976230906(float dmg, GameObject sauce)
	{
		if (base.IsOwner)
		{
			this.playerHealth -= dmg;
			PlayerMovement playerMovement;
			if (this.playerHealth <= 0f && sauce != null && sauce.TryGetComponent<PlayerMovement>(out playerMovement))
			{
				this.LastPersonToHitMe = playerMovement;
			}
			if (this.playerHealth < 0f)
			{
				this.causeofdeath = "excalibur";
				this.MakeLightning();
			}
			if (!this.isDead)
			{
				this.hurtyOverlayVal += dmg / 100f;
			}
			Mathf.Clamp(this.hurtyOverlayVal, 0f, 0.5f);
		}
	}

	// Token: 0x06000CA1 RID: 3233 RVA: 0x00033544 File Offset: 0x00031744
	private void RpcReader___Observers_Excaliburplayerdmgobserver_2976230906(PooledReader PooledReader0, Channel channel)
	{
		float num = PooledReader0.ReadSingle();
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___Excaliburplayerdmgobserver_2976230906(num, gameObject);
	}

	// Token: 0x06000CA2 RID: 3234 RVA: 0x00033588 File Offset: 0x00031788
	private void RpcWriter___Server_MakeLightning_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(30U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000CA3 RID: 3235 RVA: 0x000335ED File Offset: 0x000317ED
	private void RpcLogic___MakeLightning_2166136261()
	{
		this.MakeLightningObs();
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x000335F8 File Offset: 0x000317F8
	private void RpcReader___Server_MakeLightning_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___MakeLightning_2166136261();
	}

	// Token: 0x06000CA5 RID: 3237 RVA: 0x00033618 File Offset: 0x00031818
	private void RpcWriter___Observers_MakeLightningObs_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(31U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000CA6 RID: 3238 RVA: 0x0003368C File Offset: 0x0003188C
	private void RpcLogic___MakeLightningObs_2166136261()
	{
		Object.Instantiate<GameObject>(this.LightningBolt, base.transform.position, Quaternion.identity);
	}

	// Token: 0x06000CA7 RID: 3239 RVA: 0x000336AC File Offset: 0x000318AC
	private void RpcReader___Observers_MakeLightningObs_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___MakeLightningObs_2166136261();
	}

	// Token: 0x06000CA8 RID: 3240 RVA: 0x000336CC File Offset: 0x000318CC
	private void RpcWriter___Server_ServerAddKill_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(32U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000CA9 RID: 3241 RVA: 0x00033731 File Offset: 0x00031931
	private void RpcLogic___ServerAddKill_2166136261()
	{
		this.ObsAddKill();
	}

	// Token: 0x06000CAA RID: 3242 RVA: 0x0003373C File Offset: 0x0003193C
	private void RpcReader___Server_ServerAddKill_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerAddKill_2166136261();
	}

	// Token: 0x06000CAB RID: 3243 RVA: 0x0003375C File Offset: 0x0003195C
	private void RpcWriter___Observers_ObsAddKill_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(33U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000CAC RID: 3244 RVA: 0x000337D0 File Offset: 0x000319D0
	private void RpcLogic___ObsAddKill_2166136261()
	{
		this.kills++;
	}

	// Token: 0x06000CAD RID: 3245 RVA: 0x000337E0 File Offset: 0x000319E0
	private void RpcReader___Observers_ObsAddKill_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsAddKill_2166136261();
	}

	// Token: 0x06000CAE RID: 3246 RVA: 0x00033800 File Offset: 0x00031A00
	private void RpcWriter___Server_toggleBreakout_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(34U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x00033865 File Offset: 0x00031A65
	private void RpcLogic___toggleBreakout_2166136261()
	{
		this.toggleBreakoutobs();
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x00033870 File Offset: 0x00031A70
	private void RpcReader___Server_toggleBreakout_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___toggleBreakout_2166136261();
	}

	// Token: 0x06000CB1 RID: 3249 RVA: 0x00033890 File Offset: 0x00031A90
	private void RpcWriter___Observers_toggleBreakoutobs_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(35U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000CB2 RID: 3250 RVA: 0x00033904 File Offset: 0x00031B04
	private void RpcLogic___toggleBreakoutobs_2166136261()
	{
		this.breakoutFireball = true;
	}

	// Token: 0x06000CB3 RID: 3251 RVA: 0x00033910 File Offset: 0x00031B10
	private void RpcReader___Observers_toggleBreakoutobs_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___toggleBreakoutobs_2166136261();
	}

	// Token: 0x06000CB4 RID: 3252 RVA: 0x00033930 File Offset: 0x00031B30
	private void RpcWriter___Server_EnableBody_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(36U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000CB5 RID: 3253 RVA: 0x00033995 File Offset: 0x00031B95
	private void RpcLogic___EnableBody_2166136261()
	{
		this.ObsEnableBody();
	}

	// Token: 0x06000CB6 RID: 3254 RVA: 0x000339A0 File Offset: 0x00031BA0
	private void RpcReader___Server_EnableBody_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___EnableBody_2166136261();
	}

	// Token: 0x06000CB7 RID: 3255 RVA: 0x000339C0 File Offset: 0x00031BC0
	private void RpcWriter___Observers_ObsEnableBody_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(37U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000CB8 RID: 3256 RVA: 0x00033A34 File Offset: 0x00031C34
	private void RpcLogic___ObsEnableBody_2166136261()
	{
		PlayerAudioPlayer playerAudioPlayer;
		if (base.TryGetComponent<PlayerAudioPlayer>(out playerAudioPlayer))
		{
			playerAudioPlayer.oceanint = 0;
		}
		this.isDead = false;
		this.wizardBody[this.playerTeam].enabled = true;
		if (!base.HasAuthority)
		{
			this.playerNameText.transform.parent.parent.gameObject.SetActive(true);
		}
	}

	// Token: 0x06000CB9 RID: 3257 RVA: 0x00033A94 File Offset: 0x00031C94
	private void RpcReader___Observers_ObsEnableBody_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsEnableBody_2166136261();
	}

	// Token: 0x06000CBA RID: 3258 RVA: 0x00033AB4 File Offset: 0x00031CB4
	private void RpcWriter___Server_UndoFrogRpc_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(38U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000CBB RID: 3259 RVA: 0x00033B19 File Offset: 0x00031D19
	private void RpcLogic___UndoFrogRpc_2166136261()
	{
		this.UndoFrogObsSync();
	}

	// Token: 0x06000CBC RID: 3260 RVA: 0x00033B24 File Offset: 0x00031D24
	private void RpcReader___Server_UndoFrogRpc_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___UndoFrogRpc_2166136261();
	}

	// Token: 0x06000CBD RID: 3261 RVA: 0x00033B44 File Offset: 0x00031D44
	private void RpcWriter___Observers_UndoFrogObsSync_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(39U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000CBE RID: 3262 RVA: 0x00033BB8 File Offset: 0x00031DB8
	private void RpcLogic___UndoFrogObsSync_2166136261()
	{
		this.WizardTrio.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		this.sentFrogRpc = false;
	}

	// Token: 0x06000CBF RID: 3263 RVA: 0x00033BE8 File Offset: 0x00031DE8
	private void RpcReader___Observers_UndoFrogObsSync_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___UndoFrogObsSync_2166136261();
	}

	// Token: 0x06000CC0 RID: 3264 RVA: 0x00033C08 File Offset: 0x00031E08
	private void RpcWriter___Server_SetOnFireServer_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(40U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000CC1 RID: 3265 RVA: 0x00033C6D File Offset: 0x00031E6D
	private void RpcLogic___SetOnFireServer_2166136261()
	{
		this.SetOnFireObs();
	}

	// Token: 0x06000CC2 RID: 3266 RVA: 0x00033C78 File Offset: 0x00031E78
	private void RpcReader___Server_SetOnFireServer_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___SetOnFireServer_2166136261();
	}

	// Token: 0x06000CC3 RID: 3267 RVA: 0x00033C98 File Offset: 0x00031E98
	private void RpcWriter___Observers_SetOnFireObs_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(41U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000CC4 RID: 3268 RVA: 0x00033D0C File Offset: 0x00031F0C
	private void RpcLogic___SetOnFireObs_2166136261()
	{
		if (base.IsOwner)
		{
			this.fireTimer += 8f;
		}
	}

	// Token: 0x06000CC5 RID: 3269 RVA: 0x00033D28 File Offset: 0x00031F28
	private void RpcReader___Observers_SetOnFireObs_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___SetOnFireObs_2166136261();
	}

	// Token: 0x06000CC6 RID: 3270 RVA: 0x00033D48 File Offset: 0x00031F48
	private void RpcWriter___Server_ToggleParticles_1140765316(bool val)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteBoolean(val);
		base.SendServerRpc(42U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000CC7 RID: 3271 RVA: 0x00033DBA File Offset: 0x00031FBA
	private void RpcLogic___ToggleParticles_1140765316(bool val)
	{
		this.ObsToggleParticles(val);
	}

	// Token: 0x06000CC8 RID: 3272 RVA: 0x00033DC4 File Offset: 0x00031FC4
	private void RpcReader___Server_ToggleParticles_1140765316(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ToggleParticles_1140765316(flag);
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x00033DF8 File Offset: 0x00031FF8
	private void RpcWriter___Observers_ObsToggleParticles_1140765316(bool val)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteBoolean(val);
		base.SendObserversRpc(43U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000CCA RID: 3274 RVA: 0x00033E79 File Offset: 0x00032079
	private void RpcLogic___ObsToggleParticles_1140765316(bool val)
	{
		if (val)
		{
			this.fireParticles.Play();
			return;
		}
		this.fireParticles.Stop();
	}

	// Token: 0x06000CCB RID: 3275 RVA: 0x00033E98 File Offset: 0x00032098
	private void RpcReader___Observers_ObsToggleParticles_1140765316(PooledReader PooledReader0, Channel channel)
	{
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsToggleParticles_1140765316(flag);
	}

	// Token: 0x06000CCC RID: 3276 RVA: 0x00033ECC File Offset: 0x000320CC
	private void RpcWriter___Server_playPortalSoundServer_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(44U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000CCD RID: 3277 RVA: 0x00033F31 File Offset: 0x00032131
	private void RpcLogic___playPortalSoundServer_2166136261()
	{
		this.playPortalSoundObs();
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x00033F3C File Offset: 0x0003213C
	private void RpcReader___Server_playPortalSoundServer_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___playPortalSoundServer_2166136261();
	}

	// Token: 0x06000CCF RID: 3279 RVA: 0x00033F5C File Offset: 0x0003215C
	private void RpcWriter___Observers_playPortalSoundObs_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(45U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000CD0 RID: 3280 RVA: 0x00033FD0 File Offset: 0x000321D0
	private void RpcLogic___playPortalSoundObs_2166136261()
	{
		this.portalSource.pitch = Random.Range(1.1f, 1.4f);
		this.portalSource.volume = Random.Range(0.3f, 0.4f);
		this.portalSource.PlayOneShot(this.portalClips[Random.Range(0, this.portalClips.Length)]);
	}

	// Token: 0x06000CD1 RID: 3281 RVA: 0x00034034 File Offset: 0x00032234
	private void RpcReader___Observers_playPortalSoundObs_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___playPortalSoundObs_2166136261();
	}

	// Token: 0x06000CD2 RID: 3282 RVA: 0x00034054 File Offset: 0x00032254
	private void RpcWriter___Server_ServerToggleLoud_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(46U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000CD3 RID: 3283 RVA: 0x000340B9 File Offset: 0x000322B9
	private void RpcLogic___ServerToggleLoud_2166136261()
	{
		this.ToggleLoudEchoVoice();
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x000340C4 File Offset: 0x000322C4
	private void RpcReader___Server_ServerToggleLoud_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerToggleLoud_2166136261();
	}

	// Token: 0x06000CD5 RID: 3285 RVA: 0x000340E4 File Offset: 0x000322E4
	private void RpcWriter___Observers_ToggleLoudEchoVoice_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(47U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000CD6 RID: 3286 RVA: 0x00034158 File Offset: 0x00032358
	private void RpcLogic___ToggleLoudEchoVoice_2166136261()
	{
		if (!base.IsOwner)
		{
			Transform transform = Object.FindFirstObjectByType<DissonanceComms>().transform;
			float num = float.MaxValue;
			Transform transform2 = null;
			foreach (object obj in transform)
			{
				Transform transform3 = (Transform)obj;
				float num2 = Vector3.Distance(transform3.position, base.transform.position);
				if (num2 < num)
				{
					num = num2;
					transform2 = transform3;
				}
			}
			RaycastAudioDamper raycastAudioDamper;
			if (transform2 != null && transform2.TryGetComponent<RaycastAudioDamper>(out raycastAudioDamper))
			{
				raycastAudioDamper.ToggleGlobalVoice();
			}
		}
	}

	// Token: 0x06000CD7 RID: 3287 RVA: 0x00034204 File Offset: 0x00032404
	private void RpcReader___Observers_ToggleLoudEchoVoice_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ToggleLoudEchoVoice_2166136261();
	}

	// Token: 0x06000CD8 RID: 3288 RVA: 0x00034224 File Offset: 0x00032424
	private void RpcWriter___Server_FrogToungeAniServer_1934289915(GameObject target)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(target);
		base.SendServerRpc(48U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000CD9 RID: 3289 RVA: 0x00034296 File Offset: 0x00032496
	private void RpcLogic___FrogToungeAniServer_1934289915(GameObject target)
	{
		this.FrogToungeAniObs(target);
	}

	// Token: 0x06000CDA RID: 3290 RVA: 0x000342A0 File Offset: 0x000324A0
	private void RpcReader___Server_FrogToungeAniServer_1934289915(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___FrogToungeAniServer_1934289915(gameObject);
	}

	// Token: 0x06000CDB RID: 3291 RVA: 0x000342D4 File Offset: 0x000324D4
	private void RpcWriter___Observers_FrogToungeAniObs_1934289915(GameObject target)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(target);
		base.SendObserversRpc(49U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000CDC RID: 3292 RVA: 0x00034355 File Offset: 0x00032555
	private void RpcLogic___FrogToungeAniObs_1934289915(GameObject target)
	{
		base.StartCoroutine(this.ToungeAniRoutine(target));
	}

	// Token: 0x06000CDD RID: 3293 RVA: 0x00034368 File Offset: 0x00032568
	private void RpcReader___Observers_FrogToungeAniObs_1934289915(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___FrogToungeAniObs_1934289915(gameObject);
	}

	// Token: 0x06000CDE RID: 3294 RVA: 0x0003439C File Offset: 0x0003259C
	private void RpcWriter___Server_UpdateHP_431000436(float newval)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteSingle(newval);
		base.SendServerRpc(50U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000CDF RID: 3295 RVA: 0x0003440E File Offset: 0x0003260E
	private void RpcLogic___UpdateHP_431000436(float newval)
	{
		this.UpdateHpObs(newval);
	}

	// Token: 0x06000CE0 RID: 3296 RVA: 0x00034418 File Offset: 0x00032618
	private void RpcReader___Server_UpdateHP_431000436(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		float num = PooledReader0.ReadSingle();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___UpdateHP_431000436(num);
	}

	// Token: 0x06000CE1 RID: 3297 RVA: 0x0003444C File Offset: 0x0003264C
	private void RpcWriter___Observers_UpdateHpObs_431000436(float newval)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteSingle(newval);
		base.SendObserversRpc(51U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000CE2 RID: 3298 RVA: 0x000344CD File Offset: 0x000326CD
	private void RpcLogic___UpdateHpObs_431000436(float newval)
	{
		this.Healthbar2.lerper = newval / 10f;
	}

	// Token: 0x06000CE3 RID: 3299 RVA: 0x000344E4 File Offset: 0x000326E4
	private void RpcReader___Observers_UpdateHpObs_431000436(PooledReader PooledReader0, Channel channel)
	{
		float num = PooledReader0.ReadSingle();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___UpdateHpObs_431000436(num);
	}

	// Token: 0x06000CE4 RID: 3300 RVA: 0x00034518 File Offset: 0x00032718
	private void RpcWriter___Server_visualrecall_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(52U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000CE5 RID: 3301 RVA: 0x0003457D File Offset: 0x0003277D
	private void RpcLogic___visualrecall_2166136261()
	{
		this.obsrecallvisuals();
	}

	// Token: 0x06000CE6 RID: 3302 RVA: 0x00034588 File Offset: 0x00032788
	private void RpcReader___Server_visualrecall_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___visualrecall_2166136261();
	}

	// Token: 0x06000CE7 RID: 3303 RVA: 0x000345A8 File Offset: 0x000327A8
	private void RpcWriter___Observers_obsrecallvisuals_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(53U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000CE8 RID: 3304 RVA: 0x0003461C File Offset: 0x0003281C
	private void RpcLogic___obsrecallvisuals_2166136261()
	{
		if (!base.HasAuthority)
		{
			this.portalSource.PlayOneShot(this.recall);
			base.StartCoroutine(this.recallvisuals());
		}
	}

	// Token: 0x06000CE9 RID: 3305 RVA: 0x00034644 File Offset: 0x00032844
	private void RpcReader___Observers_obsrecallvisuals_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___obsrecallvisuals_2166136261();
	}

	// Token: 0x06000CEA RID: 3306 RVA: 0x00031CEB File Offset: 0x0002FEEB
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0400063D RID: 1597
	[Header("Base setup")]
	public float walkingSpeed = 2f;

	// Token: 0x0400063E RID: 1598
	public float runningSpeed = 9f;

	// Token: 0x0400063F RID: 1599
	public float currentSpeed = 2f;

	// Token: 0x04000640 RID: 1600
	public float mass = 20f;

	// Token: 0x04000641 RID: 1601
	public float mouseSensitivity = 2f;

	// Token: 0x04000642 RID: 1602
	public float acceleration;

	// Token: 0x04000643 RID: 1603
	private float jumpSpeed = 8f;

	// Token: 0x04000644 RID: 1604
	private CharacterController characterController;

	// Token: 0x04000645 RID: 1605
	public Vector3 velocity = Vector3.zero;

	// Token: 0x04000646 RID: 1606
	public bool canMove;

	// Token: 0x04000647 RID: 1607
	[SerializeField]
	private float cameraYOffset = 0.4f;

	// Token: 0x04000648 RID: 1608
	private Camera playerCamera;

	// Token: 0x04000649 RID: 1609
	private LayerMask groundLayerMask = 8;

	// Token: 0x0400064A RID: 1610
	private Vector2 look;

	// Token: 0x0400064B RID: 1611
	private float timeSinceGrounded;

	// Token: 0x0400064C RID: 1612
	private float timeOfLastJump;

	// Token: 0x0400064D RID: 1613
	public Animator ArmsAni;

	// Token: 0x0400064E RID: 1614
	public Animator BodyAni;

	// Token: 0x0400064F RID: 1615
	public GameObject PlayerArms;

	// Token: 0x04000650 RID: 1616
	public float camArmsSyncSpeed = 10f;

	// Token: 0x04000651 RID: 1617
	public float RotationSyncSpeed = 50f;

	// Token: 0x04000652 RID: 1618
	public bool canJump = true;

	// Token: 0x04000653 RID: 1619
	public LayerMask groundLayer;

	// Token: 0x04000654 RID: 1620
	private bool isInBog;

	// Token: 0x04000655 RID: 1621
	private Animator UIani;

	// Token: 0x04000656 RID: 1622
	private GameObject UIBox;

	// Token: 0x04000657 RID: 1623
	private float elapsedTime;

	// Token: 0x04000658 RID: 1624
	public float headbobfrequency;

	// Token: 0x04000659 RID: 1625
	public float headbobamplitude;

	// Token: 0x0400065A RID: 1626
	private Vector3 InitialCamPos;

	// Token: 0x0400065B RID: 1627
	private Transform hpcover;

	// Token: 0x0400065C RID: 1628
	private Transform stamcover;

	// Token: 0x0400065D RID: 1629
	private float stamina = 10f;

	// Token: 0x0400065E RID: 1630
	private float prevYVel;

	// Token: 0x0400065F RID: 1631
	public float camjumpyoffset;

	// Token: 0x04000660 RID: 1632
	public float camyoffsetframes = 5f;

	// Token: 0x04000661 RID: 1633
	public int playerTeam;

	// Token: 0x04000662 RID: 1634
	public SkinnedMeshRenderer[] wizardBody;

	// Token: 0x04000663 RID: 1635
	public Material[] armsMats;

	// Token: 0x04000664 RID: 1636
	public SkinnedMeshRenderer armrender;

	// Token: 0x04000665 RID: 1637
	public Transform WizardTrio;

	// Token: 0x04000666 RID: 1638
	public PlayerAudioPlayer playerAudio;

	// Token: 0x04000667 RID: 1639
	private float spinesynctimer;

	// Token: 0x04000668 RID: 1640
	public SpineSyncer spinesync;

	// Token: 0x04000669 RID: 1641
	public Collider[] hitboxcols;

	// Token: 0x0400066A RID: 1642
	public bool isBeingLevitated;

	// Token: 0x0400066B RID: 1643
	public Transform LevitationTarget;

	// Token: 0x0400066C RID: 1644
	private float playerHealth = 100f;

	// Token: 0x0400066D RID: 1645
	public bool isDead;

	// Token: 0x0400066E RID: 1646
	public float NauseatedTimer;

	// Token: 0x0400066F RID: 1647
	private FullScreenCustomPass Nausteadoverlay;

	// Token: 0x04000670 RID: 1648
	private float nausoverlaylerper;

	// Token: 0x04000671 RID: 1649
	private FullScreenCustomPass Freezeoverlay;

	// Token: 0x04000672 RID: 1650
	private FullScreenCustomPass Fireoverlay;

	// Token: 0x04000673 RID: 1651
	private FullScreenCustomPass Hurtoverlay;

	// Token: 0x04000674 RID: 1652
	private float hurtyOverlayVal;

	// Token: 0x04000675 RID: 1653
	private float externalxmoveinput;

	// Token: 0x04000676 RID: 1654
	private float externalzmoveinput;

	// Token: 0x04000677 RID: 1655
	private bool hitbymushroom;

	// Token: 0x04000678 RID: 1656
	public int HoldingStickid;

	// Token: 0x04000679 RID: 1657
	public GameObject icebox;

	// Token: 0x0400067A RID: 1658
	private bool isCrouch;

	// Token: 0x0400067B RID: 1659
	public float armsYOffset;

	// Token: 0x0400067C RID: 1660
	public SyncHandPos SyncHandPos;

	// Token: 0x0400067D RID: 1661
	private float UnShrinkTimer;

	// Token: 0x0400067E RID: 1662
	private float FallTimer;

	// Token: 0x0400067F RID: 1663
	private bool isInWater;

	// Token: 0x04000680 RID: 1664
	private bool isOnMushroom;

	// Token: 0x04000681 RID: 1665
	public GameObject SorcererDeth;

	// Token: 0x04000682 RID: 1666
	public GameObject SorcererDeth2;

	// Token: 0x04000683 RID: 1667
	public GameObject ShadowMageDeth;

	// Token: 0x04000684 RID: 1668
	public GameObject WarlockDeth;

	// Token: 0x04000685 RID: 1669
	public WardController playerWard;

	// Token: 0x04000686 RID: 1670
	private bool OnCrystalGround;

	// Token: 0x04000687 RID: 1671
	private float dartCDTimer;

	// Token: 0x04000688 RID: 1672
	private float slopex;

	// Token: 0x04000689 RID: 1673
	private float slopez;

	// Token: 0x0400068A RID: 1674
	public bool eatenByFrog;

	// Token: 0x0400068B RID: 1675
	public Transform FrogToung;

	// Token: 0x0400068C RID: 1676
	private bool sentFrogRpc;

	// Token: 0x0400068D RID: 1677
	private PlayerRespawnManager prm;

	// Token: 0x0400068E RID: 1678
	public bool canMoveCamera = true;

	// Token: 0x0400068F RID: 1679
	public float fireTimer;

	// Token: 0x04000690 RID: 1680
	private bool areParticlesActive;

	// Token: 0x04000691 RID: 1681
	public ParticleSystem fireParticles;

	// Token: 0x04000692 RID: 1682
	private bool canSprint = true;

	// Token: 0x04000693 RID: 1683
	public AudioClip[] portalClips;

	// Token: 0x04000694 RID: 1684
	public AudioSource portalSource;

	// Token: 0x04000695 RID: 1685
	private bool inited;

	// Token: 0x04000696 RID: 1686
	public RespawnWormhole[] teamSpawns;

	// Token: 0x04000697 RID: 1687
	public Transform SpectatePoint;

	// Token: 0x04000698 RID: 1688
	private int BlockFallDmgFrames;

	// Token: 0x04000699 RID: 1689
	private float stewjumpbonus;

	// Token: 0x0400069A RID: 1690
	private float stewstaminabonus = 1f;

	// Token: 0x0400069B RID: 1691
	private float stewspeedboost;

	// Token: 0x0400069C RID: 1692
	private SettingsHolder Sholder;

	// Token: 0x0400069D RID: 1693
	public AudioClip playerDeth;

	// Token: 0x0400069E RID: 1694
	public int kills;

	// Token: 0x0400069F RID: 1695
	public int deaths;

	// Token: 0x040006A0 RID: 1696
	public Text playerNameText;

	// Token: 0x040006A1 RID: 1697
	public string playername;

	// Token: 0x040006A2 RID: 1698
	public GameObject LightningBolt;

	// Token: 0x040006A3 RID: 1699
	private float shaketime;

	// Token: 0x040006A4 RID: 1700
	private float shakemagnitude;

	// Token: 0x040006A5 RID: 1701
	private float shakefalloff;

	// Token: 0x040006A6 RID: 1702
	private float frogeatencameratimer;

	// Token: 0x040006A7 RID: 1703
	public AudioClip falldmg;

	// Token: 0x040006A8 RID: 1704
	public HealthBarLerper Healthbar2;

	// Token: 0x040006A9 RID: 1705
	private float hpupdatecd;

	// Token: 0x040006AA RID: 1706
	private float prevhp = 100f;

	// Token: 0x040006AB RID: 1707
	private float hurtsoundcd;

	// Token: 0x040006AC RID: 1708
	public GameObject playermapicon;

	// Token: 0x040006AD RID: 1709
	public float xp = 0.001f;

	// Token: 0x040006AE RID: 1710
	public int level = 1;

	// Token: 0x040006AF RID: 1711
	public DecalProjector xpbar;

	// Token: 0x040006B0 RID: 1712
	public MeshRenderer recallmesh;

	// Token: 0x040006B1 RID: 1713
	public AudioClip recall;

	// Token: 0x040006B2 RID: 1714
	private float recallCD = -30f;

	// Token: 0x040006B3 RID: 1715
	public bool istutorial;

	// Token: 0x040006B4 RID: 1716
	private string causeofdeath;

	// Token: 0x040006B5 RID: 1717
	private float lasttimewasinbog;

	// Token: 0x040006B6 RID: 1718
	private float jumpinputted;

	// Token: 0x040006B7 RID: 1719
	public Texture2D[] handsw;

	// Token: 0x040006B8 RID: 1720
	public GameObject speakericon;

	// Token: 0x040006B9 RID: 1721
	private VoiceBroadcastTrigger vbt;

	// Token: 0x040006BA RID: 1722
	private bool isvbton;

	// Token: 0x040006BB RID: 1723
	private float toggleontimer;

	// Token: 0x040006BC RID: 1724
	public GameObject frogtoungee;

	// Token: 0x040006BD RID: 1725
	public Transform frogbone;

	// Token: 0x040006BE RID: 1726
	private int numberoflicks;

	// Token: 0x040006BF RID: 1727
	public LayerMask playerLayer;

	// Token: 0x040006C0 RID: 1728
	public AudioClip[] frogtoungesounds;

	// Token: 0x040006C1 RID: 1729
	public NameTagDistanceCulling ntdc;

	// Token: 0x040006C2 RID: 1730
	public bool isSmoking;

	// Token: 0x040006C3 RID: 1731
	private bool isFrozen;

	// Token: 0x040006C4 RID: 1732
	private bool breakoutFireball;

	// Token: 0x040006C5 RID: 1733
	public GameObject BrokenIcePrefab;

	// Token: 0x040006C6 RID: 1734
	public PlayerMovement LastPersonToHitMe;

	// Token: 0x040006C7 RID: 1735
	public float distanceTraveled;

	// Token: 0x040006C8 RID: 1736
	public float spellscasted;

	// Token: 0x040006C9 RID: 1737
	public float soupsdrank;

	// Token: 0x040006CA RID: 1738
	public float pipessmoked;

	// Token: 0x040006CB RID: 1739
	public bool canRecall = true;

	// Token: 0x040006CC RID: 1740
	public Color RedName;

	// Token: 0x040006CD RID: 1741
	public Material sorchp;

	// Token: 0x040006CE RID: 1742
	public Material warlockhp;

	// Token: 0x040006CF RID: 1743
	public AudioClip levelup;

	// Token: 0x040006D0 RID: 1744
	private float rockhitcd;

	// Token: 0x040006D1 RID: 1745
	private int numberofpressesbeforejump;

	// Token: 0x040006D2 RID: 1746
	private bool bhopr;

	// Token: 0x040006D3 RID: 1747
	private float bhopbonus;

	// Token: 0x040006D4 RID: 1748
	private float knockbackcd;

	// Token: 0x040006D5 RID: 1749
	private float frostboltcdtimer;

	// Token: 0x040006D6 RID: 1750
	public int crystalCDReduction;

	// Token: 0x040006D7 RID: 1751
	private bool NetworkInitialize___EarlyPlayerMovementAssembly-CSharp.dll_Excuted;

	// Token: 0x040006D8 RID: 1752
	private bool NetworkInitialize__LatePlayerMovementAssembly-CSharp.dll_Excuted;
}
