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
using UnityEngine.AI;

// Token: 0x02000180 RID: 384
public class ShadowWizardAI : NetworkBehaviour, IHitableMonster
{
	// Token: 0x06000F97 RID: 3991 RVA: 0x00040095 File Offset: 0x0003E295
	public override void OnStartClient()
	{
		base.OnStartClient();
		if (!base.HasAuthority)
		{
			this.navAgent.enabled = false;
			base.enabled = false;
			return;
		}
	}

	// Token: 0x06000F98 RID: 3992 RVA: 0x000400BC File Offset: 0x0003E2BC
	public void AgentSetup()
	{
		if (base.HasAuthority)
		{
			this.navAgent.enabled = false;
			this.PotentialTargets = new List<Transform>();
			this.CurrentPatrolPoint = base.transform;
			this.Hexes = GameObject.FindGameObjectsWithTag("hex");
			this.hexAiNodes = new Transform[this.Hexes.Length][];
			for (int i = 0; i < this.Hexes.Length; i++)
			{
				this.hexAiNodes[i] = this.Hexes[i].GetComponent<GetHexAiNodes>().HexAiNodes;
			}
			base.StartCoroutine(this.WaitforGameStart());
			base.transform.position = this.SpawnPos;
			base.StartCoroutine(this.backtospawn());
			this.state = 0;
			return;
		}
		this.Hexes = GameObject.FindGameObjectsWithTag("hex");
		for (int j = 0; j < this.Hexes.Length; j++)
		{
			if (this.Hexes[j].name == "Blackcastle(Clone)" && !this.hasSetNewSpawnPos)
			{
				GetRespPortal getRespPortal;
				if (this.Hexes[j].TryGetComponent<GetRespPortal>(out getRespPortal))
				{
					this.respport = getRespPortal;
				}
				this.hasSetNewSpawnPos = true;
			}
		}
	}

	// Token: 0x06000F99 RID: 3993 RVA: 0x000401DD File Offset: 0x0003E3DD
	private IEnumerator backtospawn()
	{
		yield return null;
		base.transform.position = this.SpawnPos;
		yield return null;
		base.transform.position = this.SpawnPos;
		yield return null;
		base.transform.position = this.SpawnPos;
		yield return null;
		base.transform.position = this.SpawnPos;
		yield return null;
		base.transform.position = this.SpawnPos;
		yield return null;
		base.transform.position = this.SpawnPos;
		yield return null;
		base.transform.position = this.SpawnPos;
		yield break;
	}

	// Token: 0x06000F9A RID: 3994 RVA: 0x000401EC File Offset: 0x0003E3EC
	private IEnumerator WaitforGameStart()
	{
		this.inited = false;
		MainMenuManager mmm = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MainMenuManager>();
		while (!mmm.GameHasStarted)
		{
			base.transform.position = this.SpawnPos;
			yield return new WaitForSeconds(0.1f);
		}
		this.navAgent.enabled = true;
		if (this.isCastleDefender)
		{
			this.inited = true;
		}
		this.syncani();
		yield break;
	}

	// Token: 0x06000F9B RID: 3995 RVA: 0x000401FB File Offset: 0x0003E3FB
	[ServerRpc(RequireOwnership = false)]
	private void syncani()
	{
		this.RpcWriter___Server_syncani_2166136261();
	}

	// Token: 0x06000F9C RID: 3996 RVA: 0x00040203 File Offset: 0x0003E403
	[ObserversRpc]
	private void obssyncanis()
	{
		this.RpcWriter___Observers_obssyncanis_2166136261();
	}

	// Token: 0x06000F9D RID: 3997 RVA: 0x0004020C File Offset: 0x0003E40C
	private void Update()
	{
		if (this.inited && !this.isFrozen && !this.isDead)
		{
			this.DoAiInterval();
			return;
		}
		if (this.inited && this.FireTimer > 0f)
		{
			this.FireTimer = 0f;
			this.ToggleParticles(false);
		}
	}

	// Token: 0x06000F9E RID: 3998 RVA: 0x0004025F File Offset: 0x0003E45F
	private void LateUpdate()
	{
		if (this.knockBackTimer > 0f)
		{
			this.knockBackTimer -= Time.deltaTime;
			this.navAgent.SetDestination(this.KnockBackTargetPos);
		}
	}

	// Token: 0x06000F9F RID: 3999 RVA: 0x00040294 File Offset: 0x0003E494
	private void DoAiInterval()
	{
		if (this.FireTimer > 0f)
		{
			this.FireTimer -= Time.deltaTime;
			this.hp -= Time.deltaTime * 5f;
			if (this.FireTimer <= 0f)
			{
				this.ToggleParticles(false);
			}
		}
		if (this.PRM.DeadPlayers != null)
		{
			foreach (GameObject gameObject in this.PRM.DeadPlayers)
			{
				if (gameObject != null && this.CurrentTarget == gameObject.transform)
				{
					this.PotentialTargets.Remove(gameObject.transform);
					foreach (Collider collider in gameObject.GetComponent<PlayerMovement>().hitboxcols)
					{
						this.PotentialTargets.Remove(collider.transform);
					}
					this.state = 2;
				}
			}
		}
		if (this.hp <= 0f)
		{
			this.isDead = true;
			base.StartCoroutine(this.RespawnRout());
			this.swizcols[0].enabled = false;
			this.swizcols[1].enabled = false;
			Vector3 position = base.transform.position;
			this.ServerDeathRobes(position);
			base.transform.position = this.SpawnPos;
			this.navAgent.SetDestination(base.transform.position);
		}
		if (this.sneakOrWalk != 1)
		{
			if (this.canRun)
			{
				this.stamina -= Time.deltaTime;
				if (this.stamina < 0f)
				{
					this.canRun = false;
				}
			}
			else
			{
				this.stamina += Time.deltaTime * 2f;
				if (this.stamina > 9.5f)
				{
					this.canRun = true;
				}
				else if (this.stamina > 4f && Time.time - this.runCheckTimer > 1f)
				{
					this.runCheckTimer = Time.time;
					if (Random.Range(0, 3) == 0)
					{
						this.canRun = true;
					}
				}
			}
		}
		if (this.navAgent.velocity != Vector3.zero && !this.LookingAtPlayer && this.state != 6)
		{
			Quaternion quaternion = Quaternion.LookRotation(this.navAgent.velocity);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, quaternion, Time.deltaTime);
		}
		if (this.navAgent.isOnOffMeshLink)
		{
			if (this.state != 6 && this.state != 8)
			{
				this.prevState = this.state;
			}
			this.state = 6;
		}
		if (this.state != 6 && this.state != 8 && this.FireballDetected != null)
		{
			Vector3 normalized = new Vector3(0f, this.FireballDetected.transform.forward.y, this.FireballDetected.transform.forward.z).normalized;
			Vector3 normalized2 = new Vector3(0f, this.Spine003.forward.y, -this.Spine003.forward.z).normalized;
			base.transform.position - this.FireballDetected.transform.position;
			Vector3 vector = base.transform.position - this.FireballDetected.transform.position;
			Vector3 normalized3 = new Vector3(this.FireballDetected.transform.forward.x, 0f, this.FireballDetected.transform.forward.z).normalized;
			Vector3 normalized4 = new Vector3(vector.x, 0f, vector.z).normalized;
			if (Vector3.Angle(normalized3, normalized4) < 35f && this.state != 6 && this.state != 8)
			{
				this.prevState = this.state;
				this.DodgeDirection = Random.Range(0, 3);
				this.fbdodgetimer = 2f;
				this.state = 8;
			}
		}
		switch (this.state)
		{
		case 0:
			if (!this.isCastleDefender)
			{
				this.isinblackcastle = false;
				this.canRun = false;
				int num = this.targetHex;
				this.targetHex = Random.Range(0, this.Hexes.Length);
				if (this.targetHex == num)
				{
					if (this.targetHex == this.Hexes.Length - 1)
					{
						this.targetHex--;
					}
					else
					{
						this.targetHex++;
					}
				}
				this.CurrentPatrolPoint = this.hexAiNodes[this.targetHex][0];
				this.sneakOrWalk = Random.Range(0, 2);
				if (this.sneakOrWalk == 0)
				{
					this.navAgent.speed = 4f;
					this.ToggleSneakAni(false);
				}
				else if (this.sneakOrWalk == 1)
				{
					this.navAgent.speed = 3f;
					this.ToggleSneakAni(true);
				}
				this.meetAtCenter = Random.Range(0, 2);
				this.stayAtCenterTimer = 0f;
				this.state = 1;
				if (this.navAgent.isOnNavMesh)
				{
					this.navAgent.SetDestination(this.CurrentPatrolPoint.position);
				}
			}
			else
			{
				for (int j = 0; j < this.Hexes.Length; j++)
				{
					if (this.Hexes[j].name == "Blackcastle(Clone)")
					{
						if (!this.hasSetNewSpawnPos)
						{
							GetRespPortal getRespPortal;
							if (this.Hexes[j].TryGetComponent<GetRespPortal>(out getRespPortal))
							{
								this.respport = getRespPortal;
							}
							this.SpawnPos = this.hexAiNodes[j][0].position;
							this.hasSetNewSpawnPos = true;
							base.StartCoroutine(this.StartgameRout());
							this.isinblackcastle = true;
						}
						this.targetHex = j;
						this.CurrentPatrolPoint = this.hexAiNodes[j][0];
						this.meetAtCenter = Random.Range(0, 2);
						this.sneakOrWalk = 1;
						this.navAgent.speed = 8f;
						this.ToggleSneakAni(false);
						this.stayAtCenterTimer = 0f;
						this.state = 1;
						if (this.navAgent.isOnNavMesh)
						{
							this.navAgent.SetDestination(this.CurrentPatrolPoint.position);
						}
					}
				}
				if (this.state != 1)
				{
					this.isCastleDefender = false;
					this.inited = false;
				}
			}
			break;
		case 1:
			this.canRun = false;
			if (this.meetAtCenter == 0)
			{
				if (Vector3.Distance(base.transform.position, this.CurrentPatrolPoint.position) < 4f)
				{
					this.navAgent.speed = 4f;
					this.PreviousPatrolPoint = this.CurrentPatrolPoint;
					this.state = 2;
					this.randomSwitchNodeValue = 0f;
					this.switchHexesTimer = (float)Random.Range(20, 45);
					this.switchSearchNodeTimer = 0f;
				}
			}
			else if (Vector3.Distance(base.transform.position, this.CurrentPatrolPoint.position) < 12f)
			{
				this.navAgent.speed = 4f;
				this.PreviousPatrolPoint = this.CurrentPatrolPoint;
				this.state = 2;
				this.randomSwitchNodeValue = 0f;
				this.switchHexesTimer = (float)Random.Range(20, 45);
				this.switchSearchNodeTimer = 0f;
			}
			if (this.LookingAtPlayer)
			{
				this.LookAtTargetServer(null, false);
				this.LookingAtPlayer = false;
			}
			this.LOSCheck();
			break;
		case 2:
		{
			this.canRun = false;
			this.switchSearchNodeTimer += Time.deltaTime;
			this.switchHexesTimer -= Time.deltaTime;
			if (this.LookingAtPlayer)
			{
				this.LookAtTargetServer(null, false);
				this.LookingAtPlayer = false;
			}
			if (this.switchHexesTimer < 0f)
			{
				this.state = 0;
			}
			int num2 = this.meetAtCenter;
			if (this.switchSearchNodeTimer > this.randomSwitchNodeValue)
			{
				this.switchSearchNodeTimer = 0f;
				if (Random.Range(0, 3) == 0)
				{
					this.CurrentPatrolPoint = base.transform;
					this.randomSwitchNodeValue = Random.Range(2f, 3f);
				}
				else
				{
					this.PreviousPatrolPoint = this.CurrentPatrolPoint;
					this.CurrentPatrolPoint = this.hexAiNodes[this.targetHex][Random.Range(0, this.hexAiNodes[this.targetHex].Length)];
					if (Random.Range(0, 11) <= 8)
					{
						this.randomSwitchNodeValue = (float)Random.Range(2, 3);
					}
					else
					{
						this.canRun = true;
						this.randomSwitchNodeValue = (float)Random.Range(4, 8);
					}
				}
			}
			if (this.CurrentPatrolPoint == null)
			{
				this.CurrentPatrolPoint = this.hexAiNodes[this.targetHex][Random.Range(0, this.hexAiNodes[this.targetHex].Length)];
			}
			if (this.PreviousPatrolPoint == null)
			{
				this.PreviousPatrolPoint = this.CurrentPatrolPoint;
			}
			if (this.navAgent.isOnNavMesh)
			{
				this.navAgent.SetDestination(Vector3.Lerp(this.PreviousPatrolPoint.position, this.CurrentPatrolPoint.position, Mathf.Clamp01(this.switchSearchNodeTimer)));
			}
			this.LOSCheck();
			this.TryLookLeftToRight();
			break;
		}
		case 3:
			if (this.sneakOrWalk == 1)
			{
				this.navAgent.SetDestination(this.CurrentTarget.position);
				Vector3 vector2;
				if (this.adjustedLOSRayCast(this.CurrentTarget, false, out vector2) && Vector3.Angle(vector2, this.CurrentTarget.forward) < 40f)
				{
					this.sneakOrWalk = 0;
					this.ToggleSneakAni(false);
				}
			}
			else
			{
				if (this.TargetLOSCheck())
				{
					if (Time.time - this.freezeCdTimer > 35f && Vector3.Distance(base.transform.position, this.CurrentTarget.position) < 20f && Random.Range(0, 3) == 0)
					{
						if (this.equippedKnife)
						{
							this.equippedKnife = false;
							this.ServerEquipBook();
						}
						this.mgbk.FlipPage(2);
						this.fbGiveUpTimer = 0f;
						this.switchSearchNodeTimer = 0f;
						this.state = 7;
						this.hasPlayedCastSound = false;
						this.gottenDirection = false;
						break;
					}
					if (Time.time - this.fireBallCdTimer > 10f && this.canCastFB && Vector3.Distance(base.transform.position, this.CurrentTarget.position) < 50f && Random.Range(0, 3) == 0)
					{
						if (this.equippedKnife)
						{
							this.equippedKnife = false;
							this.ServerEquipBook();
						}
						this.mgbk.FlipPage(1);
						this.fbGiveUpTimer = 0f;
						this.switchSearchNodeTimer = 0f;
						this.state = 5;
						this.hasPlayedCastSound = false;
						this.gottenDirection = false;
						break;
					}
					if (Time.time - this.mmCdTimer > 15f && Vector3.Distance(base.transform.position, this.CurrentTarget.position) < 50f && Random.Range(0, 3) == 0)
					{
						if (this.equippedKnife)
						{
							this.equippedKnife = false;
							this.ServerEquipBook();
						}
						this.mgbk.FlipPage(4);
						this.fbGiveUpTimer = 0f;
						this.switchSearchNodeTimer = 0f;
						this.state = 9;
						this.hasPlayedCastSound = false;
						this.gottenDirection = false;
						break;
					}
					this.gottenDirection = false;
					NavMeshPath navMeshPath = new NavMeshPath();
					this.navAgent.CalculatePath(this.CurrentTarget.position, navMeshPath);
					if (navMeshPath.status != NavMeshPathStatus.PathComplete)
					{
						if (this.switchSearchNodeTimer > 2f)
						{
							this.navAgent.SetDestination(base.transform.position);
							Vector3 vector3 = this.CurrentTarget.position - base.transform.position;
							vector3.y = 0f;
							if (vector3 != Vector3.zero)
							{
								base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(vector3), Time.deltaTime);
							}
							if (!this.LookingAtPlayer)
							{
								this.LookAtTargetServer(this.CurrentTarget, true);
								this.LookingAtPlayer = true;
							}
						}
						if (this.switchSearchNodeTimer > 25f)
						{
							this.gottenDirection = false;
							this.state = 4;
							this.canCastFB = false;
						}
					}
					else
					{
						this.navAgent.SetPath(navMeshPath);
						this.switchSearchNodeTimer = 0f;
						if (Vector3.Distance(base.transform.position, this.CurrentTarget.position) < 6f)
						{
							if (!this.equippedKnife)
							{
								this.equippedKnife = true;
								this.ServerEquipKnife();
								this.PlaySoundServer(Random.Range(9, 11));
							}
							if (this.equippedKnife && Vector3.Distance(base.transform.position, this.CurrentTarget.position) < 2.5f)
							{
								if (!this.LookingAtPlayer)
								{
									this.LookAtTargetServer(this.CurrentTarget, true);
									this.LookingAtPlayer = true;
								}
								Vector3 vector4 = this.CurrentTarget.position - base.transform.position;
								vector4.y = 0f;
								if (vector4 != Vector3.zero)
								{
									base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(vector4), Time.deltaTime * 15f);
								}
								if (Time.time - this.knifeHitTimer > this.randomizedKnifeHitCd)
								{
									this.knifeHitTimer = Time.time;
									this.randomizedKnifeHitCd = Random.Range(0.1f, 0.6f);
									GetPlayerGameobject getPlayerGameobject;
									PlayerMovement playerMovement;
									if (this.CurrentTarget.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
									{
										this.ServerMakeBlood(getPlayerGameobject.transform);
										this.DmgPlayerServer(getPlayerGameobject.player);
									}
									else if (this.CurrentTarget.TryGetComponent<PlayerMovement>(out playerMovement))
									{
										this.ServerMakeBlood(playerMovement.transform);
										playerMovement.DamagePlayer(6f, null, "frogsword");
									}
								}
							}
						}
						if (this.LookingAtPlayer)
						{
							this.LookAtTargetServer(null, false);
							this.LookingAtPlayer = false;
						}
					}
					this.LastSeenPlayerPos = this.CurrentTarget.position;
					this.canCastFB = true;
				}
				else if (!this.gottenDirection)
				{
					if (this.LookingAtPlayer)
					{
						this.LookAtTargetServer(null, false);
						this.LookingAtPlayer = false;
					}
					this.switchSearchNodeTimer = 0f;
					this.gottenDirection = true;
					Transform transform = this.Hexes[0].transform;
					this.LastSeenPlayerHex = 0;
					for (int k = 0; k < this.Hexes.Length; k++)
					{
						if (Vector3.Distance(this.CurrentTarget.position, this.Hexes[k].transform.position) < Vector3.Distance(this.CurrentTarget.position, transform.position))
						{
							this.LastSeenPlayerHex = k;
							transform = this.Hexes[k].transform;
						}
					}
					transform = this.hexAiNodes[this.LastSeenPlayerHex][1];
					float num3 = Vector3.Distance(this.LastSeenPlayerPos, this.hexAiNodes[this.LastSeenPlayerHex][1].position);
					for (int l = 2; l < this.hexAiNodes[this.LastSeenPlayerHex].Length; l++)
					{
						float num4;
						if ((num4 = Vector3.Distance(this.LastSeenPlayerPos, this.hexAiNodes[this.LastSeenPlayerHex][l].position)) < num3)
						{
							num3 = num4;
							transform = this.hexAiNodes[this.LastSeenPlayerHex][l];
						}
					}
					this.gottenDirection = true;
					this.navAgent.SetDestination(transform.position);
				}
				else if ((this.gottenDirection && this.switchSearchNodeTimer > 10f) || this.switchSearchNodeTimer > 30f)
				{
					if (this.LookingAtPlayer)
					{
						this.LookAtTargetServer(null, false);
						this.LookingAtPlayer = false;
					}
					this.gottenDirection = false;
					this.state = 4;
					this.canCastFB = false;
				}
				this.switchSearchNodeTimer += Time.deltaTime;
			}
			break;
		case 4:
			this.canRun = false;
			this.targetHex = this.LastSeenPlayerHex;
			this.CurrentPatrolPoint = this.hexAiNodes[this.targetHex][0];
			this.sneakOrWalk = 0;
			if (this.sneakOrWalk == 0)
			{
				this.navAgent.speed = 4f;
				this.ToggleSneakAni(false);
			}
			this.meetAtCenter = 1;
			this.stayAtCenterTimer = 0f;
			this.state = 1;
			this.navAgent.SetDestination(this.CurrentPatrolPoint.position);
			break;
		case 5:
		{
			Vector3 vector5 = this.CurrentTarget.position - base.transform.position;
			vector5.y = 0f;
			if (vector5 != Vector3.zero)
			{
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(vector5), Time.deltaTime);
			}
			if (!this.LookingAtPlayer)
			{
				this.LookAtTargetServer(this.CurrentTarget, true);
				this.LookingAtPlayer = true;
			}
			if (this.fbTimer > 0.2f)
			{
				this.fbTimer = 0f;
				float num5;
				this.FireballTargeting(out num5);
				this.switchSearchNodeTimer += num5;
			}
			if (this.switchSearchNodeTimer > 1.5f && !this.hasPlayedCastSound)
			{
				this.hasPlayedCastSound = true;
				this.PlaySoundServer(Random.Range(4, 8));
			}
			if (this.switchSearchNodeTimer > 2f)
			{
				Vector3 vector6 = new Vector3(this.CurrentTarget.position.x, this.CurrentTarget.position.y + 2f, this.CurrentTarget.position.z);
				Vector3 vector7 = vector6 - new Vector3(this.LastSeenPlayerPos.x, this.LastSeenPlayerPos.y + 2f, this.LastSeenPlayerPos.z);
				float num6 = Vector3.Distance(vector6, base.transform.position);
				Vector3 vector8 = vector6 + vector7 * num6;
				this.ShootFireball(vector8);
				this.state = 3;
				this.switchSearchNodeTimer = 0f;
				this.fireBallCdTimer = Time.time;
				this.canCastFB = false;
			}
			if (this.fbGiveUpTimer > 6f)
			{
				this.switchSearchNodeTimer = 0f;
				this.state = 3;
				this.fireBallCdTimer = Time.time - 3f;
				this.canCastFB = false;
			}
			this.fbTimer += Time.deltaTime;
			this.fbGiveUpTimer += Time.deltaTime;
			this.LastSeenPlayerPos = this.CurrentTarget.position;
			break;
		}
		case 6:
			if (!this.beganJump)
			{
				this.ServerSwapJumpAni(true);
				this.navLinkTimer = 0f;
				this.jumpData = this.navAgent.currentOffMeshLinkData;
				Vector3 position2 = this.navAgent.transform.position;
				Vector3 endPos = this.jumpData.endPos;
				this.navAgent.updatePosition = false;
				this.navAgent.isStopped = true;
				this.beganJump = true;
				this.jumpDistance = Vector3.Distance(this.jumpData.startPos, this.jumpData.endPos);
				this.LookAtPositionServer(this.jumpData.endPos, true);
			}
			if (this.Spine003.forward.sqrMagnitude > 0f)
			{
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(base.transform.rotation.eulerAngles.x, this.Spine003.rotation.eulerAngles.y, base.transform.rotation.eulerAngles.z), Time.deltaTime);
			}
			base.transform.position = Vector3.Lerp(this.jumpData.startPos, this.jumpData.endPos, this.navLinkTimer / this.jumpDistance) + Vector3.up * Mathf.Sin(3.1415927f * this.navLinkTimer / this.jumpDistance) * (this.jumpDistance / 1.5f);
			if (this.navLinkTimer / this.jumpDistance >= 1f)
			{
				this.LookAtPositionServer(Vector3.zero, false);
				this.ServerSwapJumpAni(false);
				base.transform.position = this.jumpData.endPos;
				this.beganJump = false;
				this.navAgent.updatePosition = true;
				this.navAgent.isStopped = false;
				this.navAgent.CompleteOffMeshLink();
				this.state = this.prevState;
			}
			this.navLinkTimer += Time.deltaTime * 9f;
			break;
		case 7:
		{
			Vector3 vector9 = this.CurrentTarget.position - base.transform.position;
			vector9.y = 0f;
			if (vector9 != Vector3.zero)
			{
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(vector9), Time.deltaTime);
			}
			if (!this.LookingAtPlayer)
			{
				this.LookAtTargetServer(this.CurrentTarget, true);
				this.LookingAtPlayer = true;
			}
			if (this.fbTimer > 0.2f)
			{
				this.fbTimer = 0f;
				float num7;
				this.FireballTargeting(out num7);
				this.switchSearchNodeTimer += num7;
			}
			if (this.switchSearchNodeTimer > 1.5f && !this.hasPlayedCastSound)
			{
				this.hasPlayedCastSound = true;
				this.PlaySoundServer(8);
			}
			if (this.switchSearchNodeTimer > 2f)
			{
				Vector3 vector10 = new Vector3(this.CurrentTarget.position.x, this.CurrentTarget.position.y + 2f, this.CurrentTarget.position.z);
				Vector3 vector11 = vector10 - new Vector3(this.LastSeenPlayerPos.x, this.LastSeenPlayerPos.y + 2f, this.LastSeenPlayerPos.z);
				float num8 = Vector3.Distance(vector10, base.transform.position) / 2f;
				Vector3 vector12 = vector10 + vector11 * num8;
				this.ShootFrostBolt(vector12);
				this.state = 3;
				this.switchSearchNodeTimer = 0f;
				this.freezeCdTimer = Time.time;
				this.canCastFB = false;
			}
			if (this.fbGiveUpTimer > 15f)
			{
				this.switchSearchNodeTimer = 0f;
				this.state = 3;
				this.freezeCdTimer = Time.time - 3f;
				this.canCastFB = false;
			}
			this.fbTimer += Time.deltaTime;
			this.fbGiveUpTimer += Time.deltaTime;
			this.LastSeenPlayerPos = this.CurrentTarget.position;
			break;
		}
		case 8:
			this.fbdodgetimer += Time.deltaTime;
			if (this.fbdodgetimer > 2f)
			{
				this.FleeFireball();
				this.fbdodgetimer = 0f;
			}
			if (this.FireballDetected == null)
			{
				this.fbdodgetimer = 0f;
				this.state = this.prevState;
			}
			break;
		case 9:
		{
			Vector3 vector13 = this.CurrentTarget.position - base.transform.position;
			vector13.y = 0f;
			if (vector13 != Vector3.zero)
			{
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(vector13), Time.deltaTime);
			}
			if (!this.LookingAtPlayer)
			{
				this.LookAtTargetServer(this.CurrentTarget, true);
				this.LookingAtPlayer = true;
			}
			if (this.fbTimer > 0.2f)
			{
				this.fbTimer = 0f;
				float num9;
				this.FireballTargeting(out num9);
				this.switchSearchNodeTimer += num9;
			}
			if (this.switchSearchNodeTimer > 1.5f && !this.hasPlayedCastSound)
			{
				this.hasPlayedCastSound = true;
				this.PlaySoundServer(Random.Range(15, 17));
			}
			if (this.switchSearchNodeTimer > 2f)
			{
				this.ShootMagicMissle();
				this.state = 3;
				this.switchSearchNodeTimer = 0f;
				this.mmCdTimer = Time.time;
				this.canCastFB = false;
			}
			if (this.fbGiveUpTimer > 15f)
			{
				this.switchSearchNodeTimer = 0f;
				this.state = 3;
				this.mmCdTimer = Time.time - 3f;
				this.canCastFB = false;
			}
			this.fbTimer += Time.deltaTime;
			this.fbGiveUpTimer += Time.deltaTime;
			this.LastSeenPlayerPos = this.CurrentTarget.position;
			break;
		}
		}
		if (this.sneakOrWalk != 1)
		{
			if (this.canRun)
			{
				this.navAgent.speed = 8.5f;
				return;
			}
			this.navAgent.speed = 4f;
		}
	}

	// Token: 0x06000FA0 RID: 4000 RVA: 0x00041B9C File Offset: 0x0003FD9C
	[ServerRpc(RequireOwnership = false)]
	private void ServerSwapJumpAni(bool tf)
	{
		this.RpcWriter___Server_ServerSwapJumpAni_1140765316(tf);
	}

	// Token: 0x06000FA1 RID: 4001 RVA: 0x00041BA8 File Offset: 0x0003FDA8
	[ObserversRpc]
	private void ObsSwapJumpAni(bool tf)
	{
		this.RpcWriter___Observers_ObsSwapJumpAni_1140765316(tf);
	}

	// Token: 0x06000FA2 RID: 4002 RVA: 0x00041BB4 File Offset: 0x0003FDB4
	private void FireballTargeting(out float adder)
	{
		if (this.isinblackcastle)
		{
			Vector3 vector = new Vector3(this.CurrentTarget.position.x, this.CurrentTarget.position.y + 1.2f, this.CurrentTarget.position.z);
			Vector3 vector2 = vector - new Vector3(this.LastSeenPlayerPos.x, this.LastSeenPlayerPos.y + 1.2f, this.LastSeenPlayerPos.z);
			float num = Vector3.Distance(vector, base.transform.position);
			Vector3 vector3 = vector + vector2 * num - this.Spine003.position;
			Vector3 vector4 = this.CurrentTarget.position;
			adder = 0f;
			if (!Physics.BoxCast(this.Spine003.position, new Vector3(0.5f, 0.5f, 0.5f), vector3, Quaternion.identity, vector3.magnitude, this.GroundLayer) && !Physics.Raycast(this.Spine003.position - this.Spine003.forward, vector3, vector3.magnitude, this.GroundLayer))
			{
				vector4 = base.transform.position;
				adder = 0.2f;
			}
			this.navAgent.SetDestination(vector4);
			return;
		}
		Vector3 vector5 = new Vector3(this.CurrentTarget.position.x, this.CurrentTarget.position.y + 2f, this.CurrentTarget.position.z);
		Vector3 vector6 = vector5 - new Vector3(this.LastSeenPlayerPos.x, this.LastSeenPlayerPos.y + 2f, this.LastSeenPlayerPos.z);
		float num2 = Vector3.Distance(vector5, base.transform.position);
		Vector3 vector7 = vector5 + vector6 * num2;
		Vector3 vector8 = vector7 - this.Spine003.position;
		adder = 0f;
		Vector3 vector9 = this.CurrentTarget.position;
		if (!Physics.BoxCast(this.Spine003.position, new Vector3(0.5f, 0.5f, 0.5f), vector8, Quaternion.identity, vector8.magnitude, this.GroundLayer) && !Physics.Raycast(this.Spine003.position - this.Spine003.forward, vector8, vector8.magnitude, this.GroundLayer))
		{
			vector9 = base.transform.position;
			adder = 0.2f;
		}
		else if (this.goingLeft)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 10f, this.Spine003.position.z) + this.Spine003.forward - this.Spine003.right * 15f, Vector3.down, out raycastHit, 20f, this.GroundLayer))
			{
				Vector3 vector10 = new Vector3(raycastHit.point.x, raycastHit.point.y + 1f, raycastHit.point.z);
				Vector3 vector11 = vector7 - vector10;
				if (!Physics.BoxCast(vector10, new Vector3(0.5f, 0.5f, 0.5f), vector11, Quaternion.identity, vector11.magnitude, this.GroundLayer))
				{
					this.goingLeft = true;
					vector9 = raycastHit.point;
				}
				else if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 10f, this.Spine003.position.z) + this.Spine003.forward + this.Spine003.right * 15f, Vector3.down, out raycastHit, 20f, this.GroundLayer))
				{
					vector10 = new Vector3(raycastHit.point.x, raycastHit.point.y + 1f, raycastHit.point.z);
					vector11 = vector7 - vector10;
					if (!Physics.BoxCast(vector10, new Vector3(0.5f, 0.5f, 0.5f), vector11, Quaternion.identity, vector11.magnitude, this.GroundLayer))
					{
						this.goingLeft = false;
						vector9 = raycastHit.point;
					}
					else if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 5f, this.Spine003.position.z) + this.Spine003.forward, Vector3.down, out raycastHit, 15f, this.GroundLayer))
					{
						vector10 = new Vector3(raycastHit.point.x, raycastHit.point.y + 1f, raycastHit.point.z);
						vector11 = vector7 - vector10;
						if (!Physics.BoxCast(vector10, new Vector3(0.5f, 0.5f, 0.5f), vector11, Quaternion.identity, vector11.magnitude, this.GroundLayer))
						{
							vector9 = raycastHit.point;
						}
					}
				}
				else if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 5f, this.Spine003.position.z) + this.Spine003.forward, Vector3.down, out raycastHit, 15f, this.GroundLayer))
				{
					vector10 = new Vector3(raycastHit.point.x, raycastHit.point.y + 1f, raycastHit.point.z);
					vector11 = vector7 - vector10;
					if (!Physics.BoxCast(vector10, new Vector3(0.5f, 0.5f, 0.5f), vector11, Quaternion.identity, vector11.magnitude, this.GroundLayer))
					{
						vector9 = raycastHit.point;
					}
				}
			}
			else if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 10f, this.Spine003.position.z) + this.Spine003.forward + this.Spine003.right * 15f, Vector3.down, out raycastHit, 20f, this.GroundLayer))
			{
				Vector3 vector12 = new Vector3(raycastHit.point.x, raycastHit.point.y + 1f, raycastHit.point.z);
				Vector3 vector13 = vector7 - vector12;
				if (!Physics.BoxCast(vector12, new Vector3(0.5f, 0.5f, 0.5f), vector13, Quaternion.identity, vector13.magnitude, this.GroundLayer))
				{
					this.goingLeft = false;
					vector9 = raycastHit.point;
				}
				else if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 5f, this.Spine003.position.z) + this.Spine003.forward, Vector3.down, out raycastHit, 15f, this.GroundLayer))
				{
					vector12 = new Vector3(raycastHit.point.x, raycastHit.point.y + 1f, raycastHit.point.z);
					vector13 = vector7 - vector12;
					if (!Physics.BoxCast(vector12, new Vector3(0.5f, 0.5f, 0.5f), vector13, Quaternion.identity, vector13.magnitude, this.GroundLayer))
					{
						vector9 = raycastHit.point;
					}
				}
			}
			else if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 5f, this.Spine003.position.z) + this.Spine003.forward, Vector3.down, out raycastHit, 15f, this.GroundLayer))
			{
				Vector3 vector14 = new Vector3(raycastHit.point.x, raycastHit.point.y + 1f, raycastHit.point.z);
				Vector3 vector15 = vector7 - vector14;
				if (!Physics.BoxCast(vector14, new Vector3(0.5f, 0.5f, 0.5f), vector15, Quaternion.identity, vector15.magnitude, this.GroundLayer))
				{
					vector9 = raycastHit.point;
				}
			}
		}
		else if (!this.goingLeft)
		{
			RaycastHit raycastHit2;
			if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 10f, this.Spine003.position.z) + this.Spine003.forward + this.Spine003.right * 15f, Vector3.down, out raycastHit2, 20f, this.GroundLayer))
			{
				Vector3 vector16 = new Vector3(raycastHit2.point.x, raycastHit2.point.y + 1f, raycastHit2.point.z);
				Vector3 vector17 = vector7 - vector16;
				if (!Physics.BoxCast(vector16, new Vector3(0.5f, 0.5f, 0.5f), vector17, Quaternion.identity, vector17.magnitude, this.GroundLayer))
				{
					this.goingLeft = false;
					vector9 = raycastHit2.point;
				}
				else if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 10f, this.Spine003.position.z) + this.Spine003.forward - this.Spine003.right * 15f, Vector3.down, out raycastHit2, 20f, this.GroundLayer))
				{
					vector16 = new Vector3(raycastHit2.point.x, raycastHit2.point.y + 1f, raycastHit2.point.z);
					vector17 = vector7 - vector16;
					if (!Physics.BoxCast(vector16, new Vector3(0.5f, 0.5f, 0.5f), vector17, Quaternion.identity, vector17.magnitude, this.GroundLayer))
					{
						this.goingLeft = true;
						vector9 = raycastHit2.point;
					}
					else if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 5f, this.Spine003.position.z) + this.Spine003.forward, Vector3.down, out raycastHit2, 15f, this.GroundLayer))
					{
						vector16 = new Vector3(raycastHit2.point.x, raycastHit2.point.y + 1f, raycastHit2.point.z);
						vector17 = vector7 - vector16;
						if (!Physics.BoxCast(vector16, new Vector3(0.5f, 0.5f, 0.5f), vector17, Quaternion.identity, vector17.magnitude, this.GroundLayer))
						{
							vector9 = raycastHit2.point;
						}
					}
				}
				else if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 5f, this.Spine003.position.z) + this.Spine003.forward, Vector3.down, out raycastHit2, 15f, this.GroundLayer))
				{
					vector16 = new Vector3(raycastHit2.point.x, raycastHit2.point.y + 1f, raycastHit2.point.z);
					vector17 = vector7 - vector16;
					if (!Physics.BoxCast(vector16, new Vector3(0.5f, 0.5f, 0.5f), vector17, Quaternion.identity, vector17.magnitude, this.GroundLayer))
					{
						vector9 = raycastHit2.point;
					}
				}
			}
			else if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 10f, this.Spine003.position.z) + this.Spine003.forward - this.Spine003.right * 15f, Vector3.down, out raycastHit2, 20f, this.GroundLayer))
			{
				Vector3 vector18 = new Vector3(raycastHit2.point.x, raycastHit2.point.y + 1f, raycastHit2.point.z);
				Vector3 vector19 = vector7 - vector18;
				if (!Physics.BoxCast(vector18, new Vector3(0.5f, 0.5f, 0.5f), vector19, Quaternion.identity, vector19.magnitude, this.GroundLayer))
				{
					this.goingLeft = true;
					vector9 = raycastHit2.point;
				}
				else if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 5f, this.Spine003.position.z) + this.Spine003.forward, Vector3.down, out raycastHit2, 15f, this.GroundLayer))
				{
					vector18 = new Vector3(raycastHit2.point.x, raycastHit2.point.y + 1f, raycastHit2.point.z);
					vector19 = vector7 - vector18;
					if (!Physics.BoxCast(vector18, new Vector3(0.5f, 0.5f, 0.5f), vector19, Quaternion.identity, vector19.magnitude, this.GroundLayer))
					{
						vector9 = raycastHit2.point;
					}
				}
			}
			else if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 5f, this.Spine003.position.z) + this.Spine003.forward, Vector3.down, out raycastHit2, 15f, this.GroundLayer))
			{
				Vector3 vector20 = new Vector3(raycastHit2.point.x, raycastHit2.point.y + 1f, raycastHit2.point.z);
				Vector3 vector21 = vector7 - vector20;
				if (!Physics.BoxCast(vector20, new Vector3(0.5f, 0.5f, 0.5f), vector21, Quaternion.identity, vector21.magnitude, this.GroundLayer))
				{
					vector9 = raycastHit2.point;
				}
			}
		}
		this.navAgent.SetDestination(vector9);
	}

	// Token: 0x06000FA3 RID: 4003 RVA: 0x00042C38 File Offset: 0x00040E38
	private void FleeFireball()
	{
		if (this.DodgeDirection == 0)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 10f, this.Spine003.position.z) + this.Spine003.forward - this.Spine003.right * 15f, Vector3.down, out raycastHit, 20f, this.GroundLayer))
			{
				Vector3 vector = raycastHit.point;
				this.navAgent.SetDestination(vector);
				return;
			}
			if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 10f, this.Spine003.position.z) + this.Spine003.forward - this.Spine003.right * 5f, Vector3.down, out raycastHit, 20f, this.GroundLayer))
			{
				Vector3 vector = raycastHit.point;
				this.navAgent.SetDestination(vector);
				return;
			}
			if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 10f, this.Spine003.position.z) + this.Spine003.forward + this.Spine003.right * 15f, Vector3.down, out raycastHit, 20f, this.GroundLayer))
			{
				Vector3 vector = raycastHit.point;
				this.navAgent.SetDestination(vector);
				this.DodgeDirection = 1;
				return;
			}
			if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 10f, this.Spine003.position.z) + -this.Spine003.forward * 10f, Vector3.down, out raycastHit, 20f, this.GroundLayer))
			{
				Vector3 vector = raycastHit.point;
				this.navAgent.SetDestination(vector);
				this.DodgeDirection = 2;
				return;
			}
		}
		else if (this.DodgeDirection == 1)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 10f, this.Spine003.position.z) + this.Spine003.forward + this.Spine003.right * 15f, Vector3.down, out raycastHit, 20f, this.GroundLayer))
			{
				Vector3 vector = raycastHit.point;
				this.navAgent.SetDestination(vector);
				return;
			}
			if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 10f, this.Spine003.position.z) + this.Spine003.forward + this.Spine003.right * 5f, Vector3.down, out raycastHit, 20f, this.GroundLayer))
			{
				Vector3 vector = raycastHit.point;
				this.navAgent.SetDestination(vector);
				return;
			}
			if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 10f, this.Spine003.position.z) + this.Spine003.forward - this.Spine003.right * 15f, Vector3.down, out raycastHit, 20f, this.GroundLayer))
			{
				Vector3 vector = raycastHit.point;
				this.navAgent.SetDestination(vector);
				this.DodgeDirection = 0;
				return;
			}
			if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 10f, this.Spine003.position.z) + -this.Spine003.forward * 10f, Vector3.down, out raycastHit, 20f, this.GroundLayer))
			{
				Vector3 vector = raycastHit.point;
				this.navAgent.SetDestination(vector);
				this.DodgeDirection = 2;
				return;
			}
		}
		else if (this.DodgeDirection == 2)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 10f, this.Spine003.position.z) + -this.Spine003.forward * 15f, Vector3.down, out raycastHit, 20f, this.GroundLayer))
			{
				Vector3 vector = raycastHit.point;
				this.navAgent.SetDestination(vector);
				return;
			}
			if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 10f, this.Spine003.position.z) + -this.Spine003.forward * 5f, Vector3.down, out raycastHit, 20f, this.GroundLayer))
			{
				Vector3 vector = raycastHit.point;
				this.navAgent.SetDestination(vector);
				return;
			}
			if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 10f, this.Spine003.position.z) + this.Spine003.forward - this.Spine003.right * 15f, Vector3.down, out raycastHit, 20f, this.GroundLayer))
			{
				Vector3 vector = raycastHit.point;
				this.navAgent.SetDestination(vector);
				this.DodgeDirection = 0;
				return;
			}
			if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 10f, this.Spine003.position.z) + this.Spine003.forward + this.Spine003.right * 15f, Vector3.down, out raycastHit, 20f, this.GroundLayer))
			{
				Vector3 vector = raycastHit.point;
				this.navAgent.SetDestination(vector);
				this.DodgeDirection = 1;
			}
		}
	}

	// Token: 0x06000FA4 RID: 4004 RVA: 0x00043398 File Offset: 0x00041598
	private bool TargetLOSCheck()
	{
		Vector3 vector;
		return this.adjustedLOSRayCast(this.CurrentTarget, true, out vector);
	}

	// Token: 0x06000FA5 RID: 4005 RVA: 0x000433B4 File Offset: 0x000415B4
	private void LOSCheck()
	{
		if (this.PotentialTargets.Count > 0)
		{
			foreach (Transform transform in this.PotentialTargets)
			{
				Vector3 vector;
				if (this.adjustedLOSRayCast(transform, true, out vector))
				{
					if (Vector3.Angle(vector, this.Spine003.forward) < 45f)
					{
						this.switchSearchNodeTimer = 0f;
						this.state = 3;
						this.CurrentTarget = transform;
						this.PlaySoundServer(Random.Range(0, 4));
						break;
					}
					if (vector.magnitude < 20f)
					{
						GetPlayerGameobject getPlayerGameobject;
						PlayerAudioPlayer playerAudioPlayer;
						if (transform.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
						{
							if (getPlayerGameobject.player.GetComponent<PlayerAudioPlayer>().magnitude > 1f)
							{
								this.switchSearchNodeTimer = 0f;
								this.state = 3;
								this.CurrentTarget = transform;
								this.PlaySoundServer(Random.Range(0, 4));
								break;
							}
						}
						else if (transform.TryGetComponent<PlayerAudioPlayer>(out playerAudioPlayer) && playerAudioPlayer.magnitude > 1f)
						{
							this.switchSearchNodeTimer = 0f;
							this.state = 3;
							this.CurrentTarget = transform;
							this.PlaySoundServer(Random.Range(0, 4));
							break;
						}
					}
				}
			}
		}
	}

	// Token: 0x06000FA6 RID: 4006 RVA: 0x00043510 File Offset: 0x00041710
	private bool adjustedLOSRayCast(Transform target, bool towardsTarget, out Vector3 direction)
	{
		if (!(target != null))
		{
			direction = Vector3.zero;
			return false;
		}
		if (towardsTarget)
		{
			Vector3 vector = new Vector3(this.Spine003.position.x, this.Spine003.position.y + 0.5f, this.Spine003.position.z);
			Vector3 vector2 = new Vector3(target.position.x, target.position.y + 2f, target.position.z);
			direction = vector2 - vector;
			return !Physics.Raycast(vector, direction, direction.magnitude, this.GroundLayer) && direction.magnitude < 70f;
		}
		Vector3 vector3 = new Vector3(this.Spine003.position.x, this.Spine003.position.y + 0.5f, this.Spine003.position.z);
		Vector3 vector4 = new Vector3(target.position.x, target.position.y + 2f, target.position.z);
		direction = vector3 - vector4;
		return !Physics.Raycast(vector4, direction, direction.magnitude, this.GroundLayer);
	}

	// Token: 0x06000FA7 RID: 4007 RVA: 0x0004367B File Offset: 0x0004187B
	private void TryLookLeftToRight()
	{
		if (Time.time - this.lookLeftRightCheckTimer > 10f)
		{
			this.lookLeftRightCheckTimer = Time.time;
			if (Random.Range(0, 3) != 0)
			{
				this.LookLeftToRightServer(2);
			}
		}
	}

	// Token: 0x06000FA8 RID: 4008 RVA: 0x000436AB File Offset: 0x000418AB
	private void ShootFireball(Vector3 target)
	{
		this.ShootFireballServer(target, false);
		this.fireBallCdTimer = Time.time;
		this.mgbk.FlipPage(1);
	}

	// Token: 0x06000FA9 RID: 4009 RVA: 0x000436CC File Offset: 0x000418CC
	[ServerRpc(RequireOwnership = false)]
	private void ShootFireballServer(Vector3 target, bool maxHeight)
	{
		this.RpcWriter___Server_ShootFireballServer_1082256137(target, maxHeight);
	}

	// Token: 0x06000FAA RID: 4010 RVA: 0x000436DC File Offset: 0x000418DC
	[ObserversRpc]
	private void ShootfireballOBs(Vector3 target, bool maxHeight)
	{
		this.RpcWriter___Observers_ShootfireballOBs_1082256137(target, maxHeight);
	}

	// Token: 0x06000FAB RID: 4011 RVA: 0x000436EC File Offset: 0x000418EC
	private void ShootFrostBolt(Vector3 target)
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(target, Vector3.down, out raycastHit, 20f, this.GroundLayer))
		{
			this.ShootFrostBoltServer(new Vector3(raycastHit.point.x, raycastHit.point.y + 1.25f, raycastHit.point.z), false);
		}
		this.freezeCdTimer = Time.time;
	}

	// Token: 0x06000FAC RID: 4012 RVA: 0x00043759 File Offset: 0x00041959
	[ServerRpc(RequireOwnership = false)]
	private void ShootFrostBoltServer(Vector3 target, bool maxHeight)
	{
		this.RpcWriter___Server_ShootFrostBoltServer_1082256137(target, maxHeight);
	}

	// Token: 0x06000FAD RID: 4013 RVA: 0x00043769 File Offset: 0x00041969
	[ObserversRpc]
	private void ShootFrostBoltOBs(Vector3 target, bool maxHeight)
	{
		this.RpcWriter___Observers_ShootFrostBoltOBs_1082256137(target, maxHeight);
	}

	// Token: 0x06000FAE RID: 4014 RVA: 0x0004377C File Offset: 0x0004197C
	private void ShootMagicMissle()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, 60f, this.PlayerLayer);
		GameObject gameObject = null;
		float num = float.MaxValue;
		foreach (Collider collider in array)
		{
			Vector3 vector = collider.transform.position - base.transform.position;
			float magnitude = vector.magnitude;
			float num2 = Vector3.Angle(Camera.main.transform.forward, vector.normalized);
			float num3 = magnitude + num2 * 0.5f;
			if (num3 < num)
			{
				num = num3;
				gameObject = collider.gameObject;
			}
		}
		if (gameObject != null)
		{
			PlayerMovement playerMovement;
			GetPlayerGameobject getPlayerGameobject;
			if (gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
			{
				this.ShootMagicMissleServer(this.Spine003.forward, playerMovement.gameObject);
			}
			else if (gameObject.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
			{
				this.ShootMagicMissleServer(this.Spine003.forward, getPlayerGameobject.player);
			}
			else
			{
				this.ShootMagicMissleServer(this.Spine003.forward, null);
			}
		}
		else
		{
			this.ShootMagicMissleServer(this.Spine003.forward, null);
		}
		this.mmCdTimer = Time.time;
	}

	// Token: 0x06000FAF RID: 4015 RVA: 0x000438A4 File Offset: 0x00041AA4
	[ServerRpc(RequireOwnership = false)]
	private void ShootMagicMissleServer(Vector3 fwd, GameObject target)
	{
		this.RpcWriter___Server_ShootMagicMissleServer_208080042(fwd, target);
	}

	// Token: 0x06000FB0 RID: 4016 RVA: 0x000438B4 File Offset: 0x00041AB4
	[ObserversRpc]
	private void ShootMagicMissleOBs(Vector3 fwd, GameObject target)
	{
		this.RpcWriter___Observers_ShootMagicMissleOBs_208080042(fwd, target);
	}

	// Token: 0x06000FB1 RID: 4017 RVA: 0x000438CF File Offset: 0x00041ACF
	private IEnumerator SpawnMissles(Vector3 fwd, GameObject target)
	{
		yield return new WaitForSeconds(0.1f);
		Object.Instantiate<GameObject>(this.MagicMissle, this.Spine003.position, Quaternion.identity).GetComponent<MagicMissleController>().AISetup(fwd, target, true);
		yield return new WaitForSeconds(0.1f);
		Object.Instantiate<GameObject>(this.MagicMissle, this.Spine003.position, Quaternion.identity).GetComponent<MagicMissleController>().AISetup(fwd, target, true);
		yield break;
	}

	// Token: 0x06000FB2 RID: 4018 RVA: 0x000438EC File Offset: 0x00041AEC
	[ServerRpc(RequireOwnership = false)]
	private void ToggleSneakAni(bool toggle)
	{
		this.RpcWriter___Server_ToggleSneakAni_1140765316(toggle);
	}

	// Token: 0x06000FB3 RID: 4019 RVA: 0x000438F8 File Offset: 0x00041AF8
	[ObserversRpc]
	private void ToggleSneakAniObs(bool toggle)
	{
		this.RpcWriter___Observers_ToggleSneakAniObs_1140765316(toggle);
	}

	// Token: 0x06000FB4 RID: 4020 RVA: 0x00043904 File Offset: 0x00041B04
	[ServerRpc(RequireOwnership = false)]
	private void LookLeftToRightServer(int lookType)
	{
		this.RpcWriter___Server_LookLeftToRightServer_3316948804(lookType);
	}

	// Token: 0x06000FB5 RID: 4021 RVA: 0x00043910 File Offset: 0x00041B10
	[ObserversRpc]
	private void LookLeftToRightObservers(int lookType)
	{
		this.RpcWriter___Observers_LookLeftToRightObservers_3316948804(lookType);
	}

	// Token: 0x06000FB6 RID: 4022 RVA: 0x0004391C File Offset: 0x00041B1C
	[ServerRpc(RequireOwnership = false)]
	private void LookAtTargetServer(Transform target, bool yn)
	{
		this.RpcWriter___Server_LookAtTargetServer_1329964289(target, yn);
	}

	// Token: 0x06000FB7 RID: 4023 RVA: 0x0004392C File Offset: 0x00041B2C
	[ObserversRpc]
	private void LookAtTargetObs(Transform target, bool yn)
	{
		this.RpcWriter___Observers_LookAtTargetObs_1329964289(target, yn);
	}

	// Token: 0x06000FB8 RID: 4024 RVA: 0x0004393C File Offset: 0x00041B3C
	[ServerRpc(RequireOwnership = false)]
	private void LookAtPositionServer(Vector3 target, bool yn)
	{
		this.RpcWriter___Server_LookAtPositionServer_1082256137(target, yn);
	}

	// Token: 0x06000FB9 RID: 4025 RVA: 0x0004394C File Offset: 0x00041B4C
	[ObserversRpc]
	private void LookAtPositionObs(Vector3 target, bool yn)
	{
		this.RpcWriter___Observers_LookAtPositionObs_1082256137(target, yn);
	}

	// Token: 0x06000FBA RID: 4026 RVA: 0x0004395C File Offset: 0x00041B5C
	[ServerRpc(RequireOwnership = false)]
	private void ServerEquipKnife()
	{
		this.RpcWriter___Server_ServerEquipKnife_2166136261();
	}

	// Token: 0x06000FBB RID: 4027 RVA: 0x00043964 File Offset: 0x00041B64
	[ObserversRpc]
	private void ObsEquipKnife()
	{
		this.RpcWriter___Observers_ObsEquipKnife_2166136261();
	}

	// Token: 0x06000FBC RID: 4028 RVA: 0x0004396C File Offset: 0x00041B6C
	[ServerRpc(RequireOwnership = false)]
	private void ServerEquipBook()
	{
		this.RpcWriter___Server_ServerEquipBook_2166136261();
	}

	// Token: 0x06000FBD RID: 4029 RVA: 0x00043974 File Offset: 0x00041B74
	[ObserversRpc]
	private void ObsEquipBook()
	{
		this.RpcWriter___Observers_ObsEquipBook_2166136261();
	}

	// Token: 0x06000FBE RID: 4030 RVA: 0x0004397C File Offset: 0x00041B7C
	[ServerRpc(RequireOwnership = false)]
	private void PlaySoundServer(int sid)
	{
		this.RpcWriter___Server_PlaySoundServer_3316948804(sid);
	}

	// Token: 0x06000FBF RID: 4031 RVA: 0x00043988 File Offset: 0x00041B88
	[ObserversRpc]
	private void PlaySoundObs(int sid)
	{
		this.RpcWriter___Observers_PlaySoundObs_3316948804(sid);
	}

	// Token: 0x06000FC0 RID: 4032 RVA: 0x00043994 File Offset: 0x00041B94
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			GetPlayerGameobject getPlayerGameobject;
			if (other.gameObject.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
			{
				this.PotentialTargets.Add(getPlayerGameobject.player.transform);
				return;
			}
			PlayerMovement playerMovement;
			if (other.gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
			{
				this.PotentialTargets.Add(playerMovement.transform);
				return;
			}
		}
		else if (other.CompareTag("Fireball"))
		{
			this.FireballDetected = other.transform;
		}
	}

	// Token: 0x06000FC1 RID: 4033 RVA: 0x00043A10 File Offset: 0x00041C10
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			GetPlayerGameobject getPlayerGameobject;
			if (other.gameObject.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
			{
				this.PotentialTargets.Remove(getPlayerGameobject.player.transform);
				return;
			}
			PlayerMovement playerMovement;
			if (other.gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
			{
				this.PotentialTargets.Remove(playerMovement.transform);
			}
		}
	}

	// Token: 0x06000FC2 RID: 4034 RVA: 0x00043A72 File Offset: 0x00041C72
	public void CallSummonIceBox(int lvl)
	{
		this.ServerCallSummonIceBox(lvl);
	}

	// Token: 0x06000FC3 RID: 4035 RVA: 0x00043A7B File Offset: 0x00041C7B
	[ServerRpc(RequireOwnership = false)]
	private void ServerCallSummonIceBox(int lvl)
	{
		this.RpcWriter___Server_ServerCallSummonIceBox_3316948804(lvl);
	}

	// Token: 0x06000FC4 RID: 4036 RVA: 0x00043A88 File Offset: 0x00041C88
	[ObserversRpc]
	private void ObserversCallSummonIceBox(int lvl)
	{
		this.RpcWriter___Observers_ObserversCallSummonIceBox_3316948804(lvl);
	}

	// Token: 0x06000FC5 RID: 4037 RVA: 0x00043AA0 File Offset: 0x00041CA0
	public void SummonIceBox(int lvl)
	{
		if (base.HasAuthority)
		{
			this.freezebreak = false;
			this.ServerSummonIceBox(base.transform.position);
			GameObject gameObject = Object.Instantiate<GameObject>(this.icebox, new Vector3(base.transform.position.x, base.transform.position.y + 1f, base.transform.position.z), Quaternion.Euler(-90f, base.transform.rotation.y, 0f));
			Material material = gameObject.GetComponent<Renderer>().material;
			base.StartCoroutine(this.lerpAlphaVal(material, gameObject));
			this.isFrozen = true;
			this.AniController.isFrozen = true;
			this.hp -= 15f;
		}
	}

	// Token: 0x06000FC6 RID: 4038 RVA: 0x00043B76 File Offset: 0x00041D76
	[ServerRpc(RequireOwnership = false)]
	private void ServerSummonIceBox(Vector3 posi)
	{
		this.RpcWriter___Server_ServerSummonIceBox_4276783012(posi);
	}

	// Token: 0x06000FC7 RID: 4039 RVA: 0x00043B84 File Offset: 0x00041D84
	[ObserversRpc]
	private void ObserversSummonIceBox(Vector3 posi)
	{
		this.RpcWriter___Observers_ObserversSummonIceBox_4276783012(posi);
	}

	// Token: 0x06000FC8 RID: 4040 RVA: 0x00043B9B File Offset: 0x00041D9B
	private IEnumerator lerpAlphaVal(Material mat, GameObject icebox)
	{
		float timer = 0f;
		while (timer < 0.5f)
		{
			timer += Time.deltaTime;
			mat.SetFloat("_AlphaRemapMax", Mathf.Lerp(0f, 0.75f, timer * 2f));
			icebox.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + 1f, base.transform.position.z);
			yield return null;
		}
		timer = 0f;
		while (timer < 5f && !this.freezebreak)
		{
			yield return null;
			timer += Time.deltaTime;
			icebox.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + 1f, base.transform.position.z);
		}
		if (!this.freezebreak)
		{
			timer = 0f;
			while (timer < 0.5f)
			{
				timer += Time.deltaTime;
				mat.SetFloat("_AlphaRemapMax", Mathf.Lerp(0.75f, 0f, timer * 2f));
				icebox.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + 1f, base.transform.position.z);
				yield return null;
			}
			mat.SetFloat("_AlphaRemapMax", 0f);
		}
		else
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.brokenCube, icebox.transform.position, icebox.transform.rotation);
			MeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].material = mat;
			}
			mat.SetFloat("_AlphaRemapMax", 0.75f);
			base.StartCoroutine(this.lerpBrokenMata(mat, gameObject));
		}
		this.isFrozen = false;
		this.AniController.isFrozen = false;
		Object.Destroy(icebox);
		yield break;
	}

	// Token: 0x06000FC9 RID: 4041 RVA: 0x00043BB8 File Offset: 0x00041DB8
	private IEnumerator lerpBrokenMata(Material mat, GameObject icebox)
	{
		yield return new WaitForSeconds(2f);
		float timer = 0f;
		while (timer < 0.5f)
		{
			timer += Time.deltaTime;
			mat.SetFloat("_AlphaRemapMax", Mathf.Lerp(0.75f, 0f, timer * 2f));
			yield return null;
		}
		Object.Destroy(icebox);
		yield break;
	}

	// Token: 0x06000FCA RID: 4042 RVA: 0x00043BD0 File Offset: 0x00041DD0
	public void ExplosionHit(Vector3 ExplosionPosition)
	{
		if (base.HasAuthority)
		{
			if (!this.isFrozen)
			{
				this.hp -= Mathf.Clamp(100f - Vector3.Distance(base.transform.position, ExplosionPosition) * 17f, 5f, 100f);
				this.SetOnFire();
				this.PlaySoundServer(Random.Range(11, 15));
				Vector3 vector = base.transform.position - ExplosionPosition;
				vector.y = 0f;
				RaycastHit raycastHit;
				NavMeshHit navMeshHit;
				if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 20f, this.Spine003.position.z) + vector * (11f - Vector3.Distance(base.transform.position, ExplosionPosition)), Vector3.down, out raycastHit, 40f, this.GroundLayer) && NavMesh.SamplePosition(raycastHit.point, out navMeshHit, 5f, -1))
				{
					GameObject gameObject = Object.Instantiate<GameObject>(this.GeneratedNavLink);
					gameObject.transform.GetChild(0).position = base.transform.position;
					gameObject.transform.GetChild(1).position = navMeshHit.position;
					this.knockBackTimer = 0.5f;
					this.KnockBackTargetPos = navMeshHit.position;
					base.StartCoroutine(this.DestroyMeshLink(gameObject));
					return;
				}
			}
			else
			{
				this.freezebreak = true;
				this.serverbreakfreeze();
			}
		}
	}

	// Token: 0x06000FCB RID: 4043 RVA: 0x00043D67 File Offset: 0x00041F67
	[ServerRpc(RequireOwnership = false)]
	private void serverbreakfreeze()
	{
		this.RpcWriter___Server_serverbreakfreeze_2166136261();
	}

	// Token: 0x06000FCC RID: 4044 RVA: 0x00043D6F File Offset: 0x00041F6F
	[ObserversRpc]
	private void obsbreakfreeze()
	{
		this.RpcWriter___Observers_obsbreakfreeze_2166136261();
	}

	// Token: 0x06000FCD RID: 4045 RVA: 0x00043D78 File Offset: 0x00041F78
	public void LightningHit(Vector3 lightningPos)
	{
		if (base.HasAuthority)
		{
			this.hp -= Mathf.Clamp(100f - Vector3.Distance(base.transform.position, lightningPos) * 17f, 5f, 100f);
			this.SetOnFire();
			this.PlaySoundServer(Random.Range(11, 15));
		}
	}

	// Token: 0x06000FCE RID: 4046 RVA: 0x00043DDB File Offset: 0x00041FDB
	[ServerRpc(RequireOwnership = false)]
	private void ServerDeathRobes(Vector3 pos)
	{
		this.RpcWriter___Server_ServerDeathRobes_4276783012(pos);
	}

	// Token: 0x06000FCF RID: 4047 RVA: 0x00043DE7 File Offset: 0x00041FE7
	[ObserversRpc]
	private void ObsDeathRobes(Vector3 pos)
	{
		this.RpcWriter___Observers_ObsDeathRobes_4276783012(pos);
	}

	// Token: 0x06000FD0 RID: 4048 RVA: 0x00043DF3 File Offset: 0x00041FF3
	[ServerRpc(RequireOwnership = false)]
	private void tiggerrespportal()
	{
		this.RpcWriter___Server_tiggerrespportal_2166136261();
	}

	// Token: 0x06000FD1 RID: 4049 RVA: 0x00043DFB File Offset: 0x00041FFB
	[ObserversRpc]
	private void triggerespobs()
	{
		this.RpcWriter___Observers_triggerespobs_2166136261();
	}

	// Token: 0x06000FD2 RID: 4050 RVA: 0x00043E03 File Offset: 0x00042003
	private IEnumerator RespawnRout()
	{
		yield return new WaitForSeconds(60f);
		this.tiggerrespportal();
		this.hp = 100f;
		this.Bringbackbro();
		this.navAgent.enabled = false;
		float timer = 0f;
		while (timer < 0.2f)
		{
			base.transform.position = this.SpawnPos;
			timer += Time.deltaTime;
			yield return null;
		}
		this.swizcols[0].enabled = true;
		this.swizcols[1].enabled = true;
		this.navAgent.enabled = true;
		this.CurrentTarget = null;
		this.state = 0;
		this.isDead = false;
		yield break;
	}

	// Token: 0x06000FD3 RID: 4051 RVA: 0x00043E12 File Offset: 0x00042012
	private IEnumerator StartgameRout()
	{
		this.hp = 50f;
		this.Bringbackbro();
		this.navAgent.enabled = false;
		float timer = 0f;
		while (timer < 0.2f)
		{
			base.transform.position = this.SpawnPos + this.spawnOffset;
			timer += Time.deltaTime;
			yield return null;
		}
		this.navAgent.enabled = true;
		this.CurrentTarget = null;
		this.state = 0;
		this.isDead = false;
		yield break;
	}

	// Token: 0x06000FD4 RID: 4052 RVA: 0x00043E21 File Offset: 0x00042021
	[ServerRpc(RequireOwnership = false)]
	private void Bringbackbro()
	{
		this.RpcWriter___Server_Bringbackbro_2166136261();
	}

	// Token: 0x06000FD5 RID: 4053 RVA: 0x00043E29 File Offset: 0x00042029
	[ObserversRpc]
	private void Brindbackbroobs()
	{
		this.RpcWriter___Observers_Brindbackbroobs_2166136261();
	}

	// Token: 0x06000FD6 RID: 4054 RVA: 0x0003AD41 File Offset: 0x00038F41
	public bool CheckOwner()
	{
		return base.HasAuthority;
	}

	// Token: 0x06000FD7 RID: 4055 RVA: 0x00043E34 File Offset: 0x00042034
	public void HitByDart()
	{
		if (Time.time - this.dartCDTimer > 1f)
		{
			this.hp -= 10f;
			this.dartCDTimer = Time.time;
			this.PlaySoundServer(Random.Range(11, 15));
		}
	}

	// Token: 0x06000FD8 RID: 4056 RVA: 0x00043E80 File Offset: 0x00042080
	[ServerRpc(RequireOwnership = false)]
	private void ServerMakeBlood(Transform prnt)
	{
		this.RpcWriter___Server_ServerMakeBlood_3068987916(prnt);
	}

	// Token: 0x06000FD9 RID: 4057 RVA: 0x00043E8C File Offset: 0x0004208C
	[ObserversRpc]
	private void ObserversMakeBlood(Transform prnt)
	{
		this.RpcWriter___Observers_ObserversMakeBlood_3068987916(prnt);
	}

	// Token: 0x06000FDA RID: 4058 RVA: 0x00043E98 File Offset: 0x00042098
	private IEnumerator blood(Transform prnt)
	{
		GameObject thing = Object.Instantiate<GameObject>(this.Blood, new Vector3(prnt.transform.position.x, prnt.transform.position.y + 1f, prnt.transform.position.z), Quaternion.identity, prnt.transform);
		thing.transform.LookAt(new Vector3(base.transform.position.x, base.transform.position.y + 2f, base.transform.position.z));
		yield return new WaitForSeconds(10f);
		Object.Destroy(thing);
		yield break;
	}

	// Token: 0x06000FDB RID: 4059 RVA: 0x00043EAE File Offset: 0x000420AE
	[ServerRpc(RequireOwnership = false)]
	private void DmgPlayerServer(GameObject Player)
	{
		this.RpcWriter___Server_DmgPlayerServer_1934289915(Player);
	}

	// Token: 0x06000FDC RID: 4060 RVA: 0x00043EBC File Offset: 0x000420BC
	[ObserversRpc]
	private void DmgPlayerObservers(GameObject Player)
	{
		this.RpcWriter___Observers_DmgPlayerObservers_1934289915(Player);
	}

	// Token: 0x06000FDD RID: 4061 RVA: 0x00043ED3 File Offset: 0x000420D3
	public void MushSwordHit(Transform Hitter)
	{
		this.ServerMushSword(Hitter);
	}

	// Token: 0x06000FDE RID: 4062 RVA: 0x00043EDC File Offset: 0x000420DC
	[ServerRpc(RequireOwnership = false)]
	private void ServerMushSword(Transform Hitter)
	{
		this.RpcWriter___Server_ServerMushSword_3068987916(Hitter);
	}

	// Token: 0x06000FDF RID: 4063 RVA: 0x00043EE8 File Offset: 0x000420E8
	[ObserversRpc]
	private void ObsMushSword(Transform Hitter)
	{
		this.RpcWriter___Observers_ObsMushSword_3068987916(Hitter);
	}

	// Token: 0x06000FE0 RID: 4064 RVA: 0x00043EFF File Offset: 0x000420FF
	private IEnumerator DestroyMeshLink(GameObject meshlink)
	{
		yield return new WaitForSeconds(15f);
		Object.Destroy(meshlink);
		yield break;
	}

	// Token: 0x06000FE1 RID: 4065 RVA: 0x00043F0E File Offset: 0x0004210E
	public void HitMonster(float Damage)
	{
		this.ServerMonsterHit(Damage);
	}

	// Token: 0x06000FE2 RID: 4066 RVA: 0x00043F17 File Offset: 0x00042117
	[ServerRpc(RequireOwnership = false)]
	private void ServerMonsterHit(float damage)
	{
		this.RpcWriter___Server_ServerMonsterHit_431000436(damage);
	}

	// Token: 0x06000FE3 RID: 4067 RVA: 0x00043F24 File Offset: 0x00042124
	[ObserversRpc]
	private void obsMonsterHit(float damage)
	{
		this.RpcWriter___Observers_obsMonsterHit_431000436(damage);
	}

	// Token: 0x06000FE4 RID: 4068 RVA: 0x00043F3C File Offset: 0x0004213C
	public void HitMonsterNotNetworked(float Damage)
	{
		if (base.HasAuthority)
		{
			this.hp -= Damage;
		}
		this.VoiceAudio.pitch = 0.75f;
		this.VoiceAudio.volume = 0.5f;
		this.VoiceAudio.PlayOneShot(this.VoiceClips[Random.Range(11, 15)]);
	}

	// Token: 0x06000FE5 RID: 4069 RVA: 0x00043F9A File Offset: 0x0004219A
	public void SetOnFire()
	{
		this.ToggleParticles(true);
	}

	// Token: 0x06000FE6 RID: 4070 RVA: 0x00043FA3 File Offset: 0x000421A3
	[ServerRpc(RequireOwnership = false)]
	private void ToggleParticles(bool val)
	{
		this.RpcWriter___Server_ToggleParticles_1140765316(val);
	}

	// Token: 0x06000FE7 RID: 4071 RVA: 0x00043FAF File Offset: 0x000421AF
	[ObserversRpc]
	private void ObsToggleParticles(bool val)
	{
		this.RpcWriter___Observers_ObsToggleParticles_1140765316(val);
	}

	// Token: 0x06000FE9 RID: 4073 RVA: 0x00044010 File Offset: 0x00042210
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyShadowWizardAIAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyShadowWizardAIAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_syncani_2166136261));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_obssyncanis_2166136261));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_ServerSwapJumpAni_1140765316));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ObsSwapJumpAni_1140765316));
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_ShootFireballServer_1082256137));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_ShootfireballOBs_1082256137));
		base.RegisterServerRpc(6U, new ServerRpcDelegate(this.RpcReader___Server_ShootFrostBoltServer_1082256137));
		base.RegisterObserversRpc(7U, new ClientRpcDelegate(this.RpcReader___Observers_ShootFrostBoltOBs_1082256137));
		base.RegisterServerRpc(8U, new ServerRpcDelegate(this.RpcReader___Server_ShootMagicMissleServer_208080042));
		base.RegisterObserversRpc(9U, new ClientRpcDelegate(this.RpcReader___Observers_ShootMagicMissleOBs_208080042));
		base.RegisterServerRpc(10U, new ServerRpcDelegate(this.RpcReader___Server_ToggleSneakAni_1140765316));
		base.RegisterObserversRpc(11U, new ClientRpcDelegate(this.RpcReader___Observers_ToggleSneakAniObs_1140765316));
		base.RegisterServerRpc(12U, new ServerRpcDelegate(this.RpcReader___Server_LookLeftToRightServer_3316948804));
		base.RegisterObserversRpc(13U, new ClientRpcDelegate(this.RpcReader___Observers_LookLeftToRightObservers_3316948804));
		base.RegisterServerRpc(14U, new ServerRpcDelegate(this.RpcReader___Server_LookAtTargetServer_1329964289));
		base.RegisterObserversRpc(15U, new ClientRpcDelegate(this.RpcReader___Observers_LookAtTargetObs_1329964289));
		base.RegisterServerRpc(16U, new ServerRpcDelegate(this.RpcReader___Server_LookAtPositionServer_1082256137));
		base.RegisterObserversRpc(17U, new ClientRpcDelegate(this.RpcReader___Observers_LookAtPositionObs_1082256137));
		base.RegisterServerRpc(18U, new ServerRpcDelegate(this.RpcReader___Server_ServerEquipKnife_2166136261));
		base.RegisterObserversRpc(19U, new ClientRpcDelegate(this.RpcReader___Observers_ObsEquipKnife_2166136261));
		base.RegisterServerRpc(20U, new ServerRpcDelegate(this.RpcReader___Server_ServerEquipBook_2166136261));
		base.RegisterObserversRpc(21U, new ClientRpcDelegate(this.RpcReader___Observers_ObsEquipBook_2166136261));
		base.RegisterServerRpc(22U, new ServerRpcDelegate(this.RpcReader___Server_PlaySoundServer_3316948804));
		base.RegisterObserversRpc(23U, new ClientRpcDelegate(this.RpcReader___Observers_PlaySoundObs_3316948804));
		base.RegisterServerRpc(24U, new ServerRpcDelegate(this.RpcReader___Server_ServerCallSummonIceBox_3316948804));
		base.RegisterObserversRpc(25U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversCallSummonIceBox_3316948804));
		base.RegisterServerRpc(26U, new ServerRpcDelegate(this.RpcReader___Server_ServerSummonIceBox_4276783012));
		base.RegisterObserversRpc(27U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversSummonIceBox_4276783012));
		base.RegisterServerRpc(28U, new ServerRpcDelegate(this.RpcReader___Server_serverbreakfreeze_2166136261));
		base.RegisterObserversRpc(29U, new ClientRpcDelegate(this.RpcReader___Observers_obsbreakfreeze_2166136261));
		base.RegisterServerRpc(30U, new ServerRpcDelegate(this.RpcReader___Server_ServerDeathRobes_4276783012));
		base.RegisterObserversRpc(31U, new ClientRpcDelegate(this.RpcReader___Observers_ObsDeathRobes_4276783012));
		base.RegisterServerRpc(32U, new ServerRpcDelegate(this.RpcReader___Server_tiggerrespportal_2166136261));
		base.RegisterObserversRpc(33U, new ClientRpcDelegate(this.RpcReader___Observers_triggerespobs_2166136261));
		base.RegisterServerRpc(34U, new ServerRpcDelegate(this.RpcReader___Server_Bringbackbro_2166136261));
		base.RegisterObserversRpc(35U, new ClientRpcDelegate(this.RpcReader___Observers_Brindbackbroobs_2166136261));
		base.RegisterServerRpc(36U, new ServerRpcDelegate(this.RpcReader___Server_ServerMakeBlood_3068987916));
		base.RegisterObserversRpc(37U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversMakeBlood_3068987916));
		base.RegisterServerRpc(38U, new ServerRpcDelegate(this.RpcReader___Server_DmgPlayerServer_1934289915));
		base.RegisterObserversRpc(39U, new ClientRpcDelegate(this.RpcReader___Observers_DmgPlayerObservers_1934289915));
		base.RegisterServerRpc(40U, new ServerRpcDelegate(this.RpcReader___Server_ServerMushSword_3068987916));
		base.RegisterObserversRpc(41U, new ClientRpcDelegate(this.RpcReader___Observers_ObsMushSword_3068987916));
		base.RegisterServerRpc(42U, new ServerRpcDelegate(this.RpcReader___Server_ServerMonsterHit_431000436));
		base.RegisterObserversRpc(43U, new ClientRpcDelegate(this.RpcReader___Observers_obsMonsterHit_431000436));
		base.RegisterServerRpc(44U, new ServerRpcDelegate(this.RpcReader___Server_ToggleParticles_1140765316));
		base.RegisterObserversRpc(45U, new ClientRpcDelegate(this.RpcReader___Observers_ObsToggleParticles_1140765316));
	}

	// Token: 0x06000FEA RID: 4074 RVA: 0x00044450 File Offset: 0x00042650
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateShadowWizardAIAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateShadowWizardAIAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000FEB RID: 4075 RVA: 0x00044463 File Offset: 0x00042663
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000FEC RID: 4076 RVA: 0x00044474 File Offset: 0x00042674
	private void RpcWriter___Server_syncani_2166136261()
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

	// Token: 0x06000FED RID: 4077 RVA: 0x000444D9 File Offset: 0x000426D9
	private void RpcLogic___syncani_2166136261()
	{
		this.obssyncanis();
	}

	// Token: 0x06000FEE RID: 4078 RVA: 0x000444E4 File Offset: 0x000426E4
	private void RpcReader___Server_syncani_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___syncani_2166136261();
	}

	// Token: 0x06000FEF RID: 4079 RVA: 0x00044504 File Offset: 0x00042704
	private void RpcWriter___Observers_obssyncanis_2166136261()
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

	// Token: 0x06000FF0 RID: 4080 RVA: 0x00044578 File Offset: 0x00042778
	private void RpcLogic___obssyncanis_2166136261()
	{
		base.GetComponent<EnableMeshRenderers>().enableRenderers();
		this.AniController.SwapItemAnis(false);
	}

	// Token: 0x06000FF1 RID: 4081 RVA: 0x00044594 File Offset: 0x00042794
	private void RpcReader___Observers_obssyncanis_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___obssyncanis_2166136261();
	}

	// Token: 0x06000FF2 RID: 4082 RVA: 0x000445B4 File Offset: 0x000427B4
	private void RpcWriter___Server_ServerSwapJumpAni_1140765316(bool tf)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteBoolean(tf);
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000FF3 RID: 4083 RVA: 0x00044626 File Offset: 0x00042826
	private void RpcLogic___ServerSwapJumpAni_1140765316(bool tf)
	{
		this.ObsSwapJumpAni(tf);
	}

	// Token: 0x06000FF4 RID: 4084 RVA: 0x00044630 File Offset: 0x00042830
	private void RpcReader___Server_ServerSwapJumpAni_1140765316(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSwapJumpAni_1140765316(flag);
	}

	// Token: 0x06000FF5 RID: 4085 RVA: 0x00044664 File Offset: 0x00042864
	private void RpcWriter___Observers_ObsSwapJumpAni_1140765316(bool tf)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteBoolean(tf);
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000FF6 RID: 4086 RVA: 0x000446E5 File Offset: 0x000428E5
	private void RpcLogic___ObsSwapJumpAni_1140765316(bool tf)
	{
		this.AniController.jumpBool = tf;
	}

	// Token: 0x06000FF7 RID: 4087 RVA: 0x000446F4 File Offset: 0x000428F4
	private void RpcReader___Observers_ObsSwapJumpAni_1140765316(PooledReader PooledReader0, Channel channel)
	{
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsSwapJumpAni_1140765316(flag);
	}

	// Token: 0x06000FF8 RID: 4088 RVA: 0x00044728 File Offset: 0x00042928
	private void RpcWriter___Server_ShootFireballServer_1082256137(Vector3 target, bool maxHeight)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(target);
		pooledWriter.WriteBoolean(maxHeight);
		base.SendServerRpc(4U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000FF9 RID: 4089 RVA: 0x000447A7 File Offset: 0x000429A7
	private void RpcLogic___ShootFireballServer_1082256137(Vector3 target, bool maxHeight)
	{
		this.ShootfireballOBs(target, maxHeight);
	}

	// Token: 0x06000FFA RID: 4090 RVA: 0x000447B4 File Offset: 0x000429B4
	private void RpcReader___Server_ShootFireballServer_1082256137(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ShootFireballServer_1082256137(vector, flag);
	}

	// Token: 0x06000FFB RID: 4091 RVA: 0x000447F8 File Offset: 0x000429F8
	private void RpcWriter___Observers_ShootfireballOBs_1082256137(Vector3 target, bool maxHeight)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(target);
		pooledWriter.WriteBoolean(maxHeight);
		base.SendObserversRpc(5U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000FFC RID: 4092 RVA: 0x00044886 File Offset: 0x00042A86
	private void RpcLogic___ShootfireballOBs_1082256137(Vector3 target, bool maxHeight)
	{
		Object.Instantiate<GameObject>(this.fireBall, this.Spine003.position, Quaternion.identity).GetComponent<FireballController>().Setup(target, maxHeight);
	}

	// Token: 0x06000FFD RID: 4093 RVA: 0x000448B0 File Offset: 0x00042AB0
	private void RpcReader___Observers_ShootfireballOBs_1082256137(PooledReader PooledReader0, Channel channel)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ShootfireballOBs_1082256137(vector, flag);
	}

	// Token: 0x06000FFE RID: 4094 RVA: 0x000448F4 File Offset: 0x00042AF4
	private void RpcWriter___Server_ShootFrostBoltServer_1082256137(Vector3 target, bool maxHeight)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(target);
		pooledWriter.WriteBoolean(maxHeight);
		base.SendServerRpc(6U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000FFF RID: 4095 RVA: 0x00044973 File Offset: 0x00042B73
	private void RpcLogic___ShootFrostBoltServer_1082256137(Vector3 target, bool maxHeight)
	{
		this.ShootFrostBoltOBs(target, maxHeight);
	}

	// Token: 0x06001000 RID: 4096 RVA: 0x00044980 File Offset: 0x00042B80
	private void RpcReader___Server_ShootFrostBoltServer_1082256137(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ShootFrostBoltServer_1082256137(vector, flag);
	}

	// Token: 0x06001001 RID: 4097 RVA: 0x000449C4 File Offset: 0x00042BC4
	private void RpcWriter___Observers_ShootFrostBoltOBs_1082256137(Vector3 target, bool maxHeight)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(target);
		pooledWriter.WriteBoolean(maxHeight);
		base.SendObserversRpc(7U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06001002 RID: 4098 RVA: 0x00044A52 File Offset: 0x00042C52
	private void RpcLogic___ShootFrostBoltOBs_1082256137(Vector3 target, bool maxHeight)
	{
		Object.Instantiate<GameObject>(this.FrostBolt, this.Spine003.position, Quaternion.identity).GetComponent<FrostBoltController>().Setup(target, maxHeight);
	}

	// Token: 0x06001003 RID: 4099 RVA: 0x00044A7C File Offset: 0x00042C7C
	private void RpcReader___Observers_ShootFrostBoltOBs_1082256137(PooledReader PooledReader0, Channel channel)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ShootFrostBoltOBs_1082256137(vector, flag);
	}

	// Token: 0x06001004 RID: 4100 RVA: 0x00044AC0 File Offset: 0x00042CC0
	private void RpcWriter___Server_ShootMagicMissleServer_208080042(Vector3 fwd, GameObject target)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(fwd);
		pooledWriter.WriteGameObject(target);
		base.SendServerRpc(8U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06001005 RID: 4101 RVA: 0x00044B3F File Offset: 0x00042D3F
	private void RpcLogic___ShootMagicMissleServer_208080042(Vector3 fwd, GameObject target)
	{
		this.ShootMagicMissleOBs(fwd, target);
	}

	// Token: 0x06001006 RID: 4102 RVA: 0x00044B4C File Offset: 0x00042D4C
	private void RpcReader___Server_ShootMagicMissleServer_208080042(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ShootMagicMissleServer_208080042(vector, gameObject);
	}

	// Token: 0x06001007 RID: 4103 RVA: 0x00044B90 File Offset: 0x00042D90
	private void RpcWriter___Observers_ShootMagicMissleOBs_208080042(Vector3 fwd, GameObject target)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(fwd);
		pooledWriter.WriteGameObject(target);
		base.SendObserversRpc(9U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06001008 RID: 4104 RVA: 0x00044C20 File Offset: 0x00042E20
	private void RpcLogic___ShootMagicMissleOBs_208080042(Vector3 fwd, GameObject target)
	{
		this.VoiceAudio.pitch = 1f;
		this.VoiceAudio.volume = 0.25f;
		this.VoiceAudio.PlayOneShot(this.VoiceClips[17]);
		Object.Instantiate<GameObject>(this.MagicMissle, this.Spine003.position, Quaternion.identity).GetComponent<MagicMissleController>().AISetup(fwd, target, true);
		base.StartCoroutine(this.SpawnMissles(fwd, target));
	}

	// Token: 0x06001009 RID: 4105 RVA: 0x00044C98 File Offset: 0x00042E98
	private void RpcReader___Observers_ShootMagicMissleOBs_208080042(PooledReader PooledReader0, Channel channel)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ShootMagicMissleOBs_208080042(vector, gameObject);
	}

	// Token: 0x0600100A RID: 4106 RVA: 0x00044CDC File Offset: 0x00042EDC
	private void RpcWriter___Server_ToggleSneakAni_1140765316(bool toggle)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteBoolean(toggle);
		base.SendServerRpc(10U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600100B RID: 4107 RVA: 0x00044D4E File Offset: 0x00042F4E
	private void RpcLogic___ToggleSneakAni_1140765316(bool toggle)
	{
		this.ToggleSneakAniObs(toggle);
	}

	// Token: 0x0600100C RID: 4108 RVA: 0x00044D58 File Offset: 0x00042F58
	private void RpcReader___Server_ToggleSneakAni_1140765316(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ToggleSneakAni_1140765316(flag);
	}

	// Token: 0x0600100D RID: 4109 RVA: 0x00044D8C File Offset: 0x00042F8C
	private void RpcWriter___Observers_ToggleSneakAniObs_1140765316(bool toggle)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteBoolean(toggle);
		base.SendObserversRpc(11U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600100E RID: 4110 RVA: 0x00044E0D File Offset: 0x0004300D
	private void RpcLogic___ToggleSneakAniObs_1140765316(bool toggle)
	{
		this.AniController.toggleSneak(toggle);
	}

	// Token: 0x0600100F RID: 4111 RVA: 0x00044E1C File Offset: 0x0004301C
	private void RpcReader___Observers_ToggleSneakAniObs_1140765316(PooledReader PooledReader0, Channel channel)
	{
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ToggleSneakAniObs_1140765316(flag);
	}

	// Token: 0x06001010 RID: 4112 RVA: 0x00044E50 File Offset: 0x00043050
	private void RpcWriter___Server_LookLeftToRightServer_3316948804(int lookType)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(lookType);
		base.SendServerRpc(12U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06001011 RID: 4113 RVA: 0x00044EC2 File Offset: 0x000430C2
	private void RpcLogic___LookLeftToRightServer_3316948804(int lookType)
	{
		this.LookLeftToRightObservers(lookType);
	}

	// Token: 0x06001012 RID: 4114 RVA: 0x00044ECC File Offset: 0x000430CC
	private void RpcReader___Server_LookLeftToRightServer_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___LookLeftToRightServer_3316948804(num);
	}

	// Token: 0x06001013 RID: 4115 RVA: 0x00044F00 File Offset: 0x00043100
	private void RpcWriter___Observers_LookLeftToRightObservers_3316948804(int lookType)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(lookType);
		base.SendObserversRpc(13U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06001014 RID: 4116 RVA: 0x00044F81 File Offset: 0x00043181
	private void RpcLogic___LookLeftToRightObservers_3316948804(int lookType)
	{
		this.AniController.LookLeftToRight = lookType;
		this.AniController.LTRphase = 0f;
		this.AniController.ltrTimer = 0f;
	}

	// Token: 0x06001015 RID: 4117 RVA: 0x00044FB0 File Offset: 0x000431B0
	private void RpcReader___Observers_LookLeftToRightObservers_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___LookLeftToRightObservers_3316948804(num);
	}

	// Token: 0x06001016 RID: 4118 RVA: 0x00044FE4 File Offset: 0x000431E4
	private void RpcWriter___Server_LookAtTargetServer_1329964289(Transform target, bool yn)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteTransform(target);
		pooledWriter.WriteBoolean(yn);
		base.SendServerRpc(14U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06001017 RID: 4119 RVA: 0x00045063 File Offset: 0x00043263
	private void RpcLogic___LookAtTargetServer_1329964289(Transform target, bool yn)
	{
		this.LookAtTargetObs(target, yn);
	}

	// Token: 0x06001018 RID: 4120 RVA: 0x00045070 File Offset: 0x00043270
	private void RpcReader___Server_LookAtTargetServer_1329964289(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Transform transform = PooledReader0.ReadTransform();
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___LookAtTargetServer_1329964289(transform, flag);
	}

	// Token: 0x06001019 RID: 4121 RVA: 0x000450B4 File Offset: 0x000432B4
	private void RpcWriter___Observers_LookAtTargetObs_1329964289(Transform target, bool yn)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteTransform(target);
		pooledWriter.WriteBoolean(yn);
		base.SendObserversRpc(15U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600101A RID: 4122 RVA: 0x00045142 File Offset: 0x00043342
	private void RpcLogic___LookAtTargetObs_1329964289(Transform target, bool yn)
	{
		this.AniController.PlayerToLookAt = target;
		this.AniController.LookAtPlayer = yn;
	}

	// Token: 0x0600101B RID: 4123 RVA: 0x0004515C File Offset: 0x0004335C
	private void RpcReader___Observers_LookAtTargetObs_1329964289(PooledReader PooledReader0, Channel channel)
	{
		Transform transform = PooledReader0.ReadTransform();
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___LookAtTargetObs_1329964289(transform, flag);
	}

	// Token: 0x0600101C RID: 4124 RVA: 0x000451A0 File Offset: 0x000433A0
	private void RpcWriter___Server_LookAtPositionServer_1082256137(Vector3 target, bool yn)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(target);
		pooledWriter.WriteBoolean(yn);
		base.SendServerRpc(16U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600101D RID: 4125 RVA: 0x0004521F File Offset: 0x0004341F
	private void RpcLogic___LookAtPositionServer_1082256137(Vector3 target, bool yn)
	{
		this.LookAtPositionObs(target, yn);
	}

	// Token: 0x0600101E RID: 4126 RVA: 0x0004522C File Offset: 0x0004342C
	private void RpcReader___Server_LookAtPositionServer_1082256137(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___LookAtPositionServer_1082256137(vector, flag);
	}

	// Token: 0x0600101F RID: 4127 RVA: 0x00045270 File Offset: 0x00043470
	private void RpcWriter___Observers_LookAtPositionObs_1082256137(Vector3 target, bool yn)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(target);
		pooledWriter.WriteBoolean(yn);
		base.SendObserversRpc(17U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06001020 RID: 4128 RVA: 0x000452FE File Offset: 0x000434FE
	private void RpcLogic___LookAtPositionObs_1082256137(Vector3 target, bool yn)
	{
		this.AniController.PositionToLookAt = target;
		this.AniController.LookAtPosition = yn;
	}

	// Token: 0x06001021 RID: 4129 RVA: 0x00045318 File Offset: 0x00043518
	private void RpcReader___Observers_LookAtPositionObs_1082256137(PooledReader PooledReader0, Channel channel)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___LookAtPositionObs_1082256137(vector, flag);
	}

	// Token: 0x06001022 RID: 4130 RVA: 0x0004535C File Offset: 0x0004355C
	private void RpcWriter___Server_ServerEquipKnife_2166136261()
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

	// Token: 0x06001023 RID: 4131 RVA: 0x000453C1 File Offset: 0x000435C1
	private void RpcLogic___ServerEquipKnife_2166136261()
	{
		this.ObsEquipKnife();
	}

	// Token: 0x06001024 RID: 4132 RVA: 0x000453CC File Offset: 0x000435CC
	private void RpcReader___Server_ServerEquipKnife_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerEquipKnife_2166136261();
	}

	// Token: 0x06001025 RID: 4133 RVA: 0x000453EC File Offset: 0x000435EC
	private void RpcWriter___Observers_ObsEquipKnife_2166136261()
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

	// Token: 0x06001026 RID: 4134 RVA: 0x00045460 File Offset: 0x00043660
	private void RpcLogic___ObsEquipKnife_2166136261()
	{
		this.AniController.SwapItemAnis(true);
		this.AniController.ToggleAttackAni(true);
	}

	// Token: 0x06001027 RID: 4135 RVA: 0x0004547C File Offset: 0x0004367C
	private void RpcReader___Observers_ObsEquipKnife_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsEquipKnife_2166136261();
	}

	// Token: 0x06001028 RID: 4136 RVA: 0x0004549C File Offset: 0x0004369C
	private void RpcWriter___Server_ServerEquipBook_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(20U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06001029 RID: 4137 RVA: 0x00045501 File Offset: 0x00043701
	private void RpcLogic___ServerEquipBook_2166136261()
	{
		this.ObsEquipBook();
	}

	// Token: 0x0600102A RID: 4138 RVA: 0x0004550C File Offset: 0x0004370C
	private void RpcReader___Server_ServerEquipBook_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerEquipBook_2166136261();
	}

	// Token: 0x0600102B RID: 4139 RVA: 0x0004552C File Offset: 0x0004372C
	private void RpcWriter___Observers_ObsEquipBook_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(21U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600102C RID: 4140 RVA: 0x000455A0 File Offset: 0x000437A0
	private void RpcLogic___ObsEquipBook_2166136261()
	{
		this.AniController.SwapItemAnis(false);
		this.AniController.ToggleAttackAni(false);
	}

	// Token: 0x0600102D RID: 4141 RVA: 0x000455BC File Offset: 0x000437BC
	private void RpcReader___Observers_ObsEquipBook_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsEquipBook_2166136261();
	}

	// Token: 0x0600102E RID: 4142 RVA: 0x000455DC File Offset: 0x000437DC
	private void RpcWriter___Server_PlaySoundServer_3316948804(int sid)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(sid);
		base.SendServerRpc(22U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600102F RID: 4143 RVA: 0x0004564E File Offset: 0x0004384E
	private void RpcLogic___PlaySoundServer_3316948804(int sid)
	{
		this.PlaySoundObs(sid);
	}

	// Token: 0x06001030 RID: 4144 RVA: 0x00045658 File Offset: 0x00043858
	private void RpcReader___Server_PlaySoundServer_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___PlaySoundServer_3316948804(num);
	}

	// Token: 0x06001031 RID: 4145 RVA: 0x0004568C File Offset: 0x0004388C
	private void RpcWriter___Observers_PlaySoundObs_3316948804(int sid)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(sid);
		base.SendObserversRpc(23U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06001032 RID: 4146 RVA: 0x0004570D File Offset: 0x0004390D
	private void RpcLogic___PlaySoundObs_3316948804(int sid)
	{
		this.VoiceAudio.pitch = 0.75f;
		this.VoiceAudio.volume = 1f;
		this.VoiceAudio.PlayOneShot(this.VoiceClips[sid]);
	}

	// Token: 0x06001033 RID: 4147 RVA: 0x00045744 File Offset: 0x00043944
	private void RpcReader___Observers_PlaySoundObs_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___PlaySoundObs_3316948804(num);
	}

	// Token: 0x06001034 RID: 4148 RVA: 0x00045778 File Offset: 0x00043978
	private void RpcWriter___Server_ServerCallSummonIceBox_3316948804(int lvl)
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
		base.SendServerRpc(24U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06001035 RID: 4149 RVA: 0x000457EA File Offset: 0x000439EA
	private void RpcLogic___ServerCallSummonIceBox_3316948804(int lvl)
	{
		this.ObserversCallSummonIceBox(lvl);
	}

	// Token: 0x06001036 RID: 4150 RVA: 0x000457F4 File Offset: 0x000439F4
	private void RpcReader___Server_ServerCallSummonIceBox_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerCallSummonIceBox_3316948804(num);
	}

	// Token: 0x06001037 RID: 4151 RVA: 0x00045828 File Offset: 0x00043A28
	private void RpcWriter___Observers_ObserversCallSummonIceBox_3316948804(int lvl)
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
		base.SendObserversRpc(25U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06001038 RID: 4152 RVA: 0x000458AC File Offset: 0x00043AAC
	private void RpcLogic___ObserversCallSummonIceBox_3316948804(int lvl)
	{
		this.SummonIceBox(lvl);
		this.VoiceAudio.pitch = 0.75f;
		this.VoiceAudio.volume = 1f;
		this.VoiceAudio.PlayOneShot(this.VoiceClips[Random.Range(11, 15)]);
	}

	// Token: 0x06001039 RID: 4153 RVA: 0x000458FC File Offset: 0x00043AFC
	private void RpcReader___Observers_ObserversCallSummonIceBox_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversCallSummonIceBox_3316948804(num);
	}

	// Token: 0x0600103A RID: 4154 RVA: 0x00045930 File Offset: 0x00043B30
	private void RpcWriter___Server_ServerSummonIceBox_4276783012(Vector3 posi)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(posi);
		base.SendServerRpc(26U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600103B RID: 4155 RVA: 0x000459A2 File Offset: 0x00043BA2
	private void RpcLogic___ServerSummonIceBox_4276783012(Vector3 posi)
	{
		this.ObserversSummonIceBox(posi);
	}

	// Token: 0x0600103C RID: 4156 RVA: 0x000459AC File Offset: 0x00043BAC
	private void RpcReader___Server_ServerSummonIceBox_4276783012(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSummonIceBox_4276783012(vector);
	}

	// Token: 0x0600103D RID: 4157 RVA: 0x000459E0 File Offset: 0x00043BE0
	private void RpcWriter___Observers_ObserversSummonIceBox_4276783012(Vector3 posi)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(posi);
		base.SendObserversRpc(27U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600103E RID: 4158 RVA: 0x00045A64 File Offset: 0x00043C64
	private void RpcLogic___ObserversSummonIceBox_4276783012(Vector3 posi)
	{
		if (!base.HasAuthority)
		{
			this.freezebreak = false;
			GameObject gameObject = Object.Instantiate<GameObject>(this.icebox, new Vector3(posi.x, posi.y + 1f, posi.z), Quaternion.Euler(-90f, base.transform.rotation.y, 0f));
			Material material = gameObject.GetComponent<Renderer>().material;
			base.StartCoroutine(this.lerpAlphaVal(material, gameObject));
		}
	}

	// Token: 0x0600103F RID: 4159 RVA: 0x00045AE4 File Offset: 0x00043CE4
	private void RpcReader___Observers_ObserversSummonIceBox_4276783012(PooledReader PooledReader0, Channel channel)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversSummonIceBox_4276783012(vector);
	}

	// Token: 0x06001040 RID: 4160 RVA: 0x00045B18 File Offset: 0x00043D18
	private void RpcWriter___Server_serverbreakfreeze_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(28U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06001041 RID: 4161 RVA: 0x00045B7D File Offset: 0x00043D7D
	private void RpcLogic___serverbreakfreeze_2166136261()
	{
		this.obsbreakfreeze();
	}

	// Token: 0x06001042 RID: 4162 RVA: 0x00045B88 File Offset: 0x00043D88
	private void RpcReader___Server_serverbreakfreeze_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___serverbreakfreeze_2166136261();
	}

	// Token: 0x06001043 RID: 4163 RVA: 0x00045BA8 File Offset: 0x00043DA8
	private void RpcWriter___Observers_obsbreakfreeze_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(29U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06001044 RID: 4164 RVA: 0x00045C1C File Offset: 0x00043E1C
	private void RpcLogic___obsbreakfreeze_2166136261()
	{
		this.freezebreak = true;
	}

	// Token: 0x06001045 RID: 4165 RVA: 0x00045C28 File Offset: 0x00043E28
	private void RpcReader___Observers_obsbreakfreeze_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___obsbreakfreeze_2166136261();
	}

	// Token: 0x06001046 RID: 4166 RVA: 0x00045C48 File Offset: 0x00043E48
	private void RpcWriter___Server_ServerDeathRobes_4276783012(Vector3 pos)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(pos);
		base.SendServerRpc(30U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06001047 RID: 4167 RVA: 0x00045CBA File Offset: 0x00043EBA
	private void RpcLogic___ServerDeathRobes_4276783012(Vector3 pos)
	{
		this.ObsDeathRobes(pos);
	}

	// Token: 0x06001048 RID: 4168 RVA: 0x00045CC4 File Offset: 0x00043EC4
	private void RpcReader___Server_ServerDeathRobes_4276783012(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerDeathRobes_4276783012(vector);
	}

	// Token: 0x06001049 RID: 4169 RVA: 0x00045CF8 File Offset: 0x00043EF8
	private void RpcWriter___Observers_ObsDeathRobes_4276783012(Vector3 pos)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(pos);
		base.SendObserversRpc(31U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600104A RID: 4170 RVA: 0x00045D79 File Offset: 0x00043F79
	private void RpcLogic___ObsDeathRobes_4276783012(Vector3 pos)
	{
		Object.Instantiate<GameObject>(this.ShadowMageDeth, pos, base.transform.rotation);
		this.VisualObject.SetActive(false);
	}

	// Token: 0x0600104B RID: 4171 RVA: 0x00045DA0 File Offset: 0x00043FA0
	private void RpcReader___Observers_ObsDeathRobes_4276783012(PooledReader PooledReader0, Channel channel)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsDeathRobes_4276783012(vector);
	}

	// Token: 0x0600104C RID: 4172 RVA: 0x00045DD4 File Offset: 0x00043FD4
	private void RpcWriter___Server_tiggerrespportal_2166136261()
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

	// Token: 0x0600104D RID: 4173 RVA: 0x00045E39 File Offset: 0x00044039
	private void RpcLogic___tiggerrespportal_2166136261()
	{
		this.triggerespobs();
	}

	// Token: 0x0600104E RID: 4174 RVA: 0x00045E44 File Offset: 0x00044044
	private void RpcReader___Server_tiggerrespportal_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___tiggerrespportal_2166136261();
	}

	// Token: 0x0600104F RID: 4175 RVA: 0x00045E64 File Offset: 0x00044064
	private void RpcWriter___Observers_triggerespobs_2166136261()
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

	// Token: 0x06001050 RID: 4176 RVA: 0x00045ED8 File Offset: 0x000440D8
	private void RpcLogic___triggerespobs_2166136261()
	{
		if (this.respport != null)
		{
			this.respport.triggerPortal();
		}
	}

	// Token: 0x06001051 RID: 4177 RVA: 0x00045EF4 File Offset: 0x000440F4
	private void RpcReader___Observers_triggerespobs_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___triggerespobs_2166136261();
	}

	// Token: 0x06001052 RID: 4178 RVA: 0x00045F14 File Offset: 0x00044114
	private void RpcWriter___Server_Bringbackbro_2166136261()
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

	// Token: 0x06001053 RID: 4179 RVA: 0x00045F79 File Offset: 0x00044179
	private void RpcLogic___Bringbackbro_2166136261()
	{
		this.Brindbackbroobs();
	}

	// Token: 0x06001054 RID: 4180 RVA: 0x00045F84 File Offset: 0x00044184
	private void RpcReader___Server_Bringbackbro_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___Bringbackbro_2166136261();
	}

	// Token: 0x06001055 RID: 4181 RVA: 0x00045FA4 File Offset: 0x000441A4
	private void RpcWriter___Observers_Brindbackbroobs_2166136261()
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

	// Token: 0x06001056 RID: 4182 RVA: 0x00046018 File Offset: 0x00044218
	private void RpcLogic___Brindbackbroobs_2166136261()
	{
		this.VisualObject.SetActive(true);
		this.syncani();
	}

	// Token: 0x06001057 RID: 4183 RVA: 0x0004602C File Offset: 0x0004422C
	private void RpcReader___Observers_Brindbackbroobs_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___Brindbackbroobs_2166136261();
	}

	// Token: 0x06001058 RID: 4184 RVA: 0x0004604C File Offset: 0x0004424C
	private void RpcWriter___Server_ServerMakeBlood_3068987916(Transform prnt)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteTransform(prnt);
		base.SendServerRpc(36U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06001059 RID: 4185 RVA: 0x000460BE File Offset: 0x000442BE
	private void RpcLogic___ServerMakeBlood_3068987916(Transform prnt)
	{
		this.ObserversMakeBlood(prnt);
	}

	// Token: 0x0600105A RID: 4186 RVA: 0x000460C8 File Offset: 0x000442C8
	private void RpcReader___Server_ServerMakeBlood_3068987916(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Transform transform = PooledReader0.ReadTransform();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerMakeBlood_3068987916(transform);
	}

	// Token: 0x0600105B RID: 4187 RVA: 0x000460FC File Offset: 0x000442FC
	private void RpcWriter___Observers_ObserversMakeBlood_3068987916(Transform prnt)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteTransform(prnt);
		base.SendObserversRpc(37U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600105C RID: 4188 RVA: 0x0004617D File Offset: 0x0004437D
	private void RpcLogic___ObserversMakeBlood_3068987916(Transform prnt)
	{
		base.StartCoroutine(this.blood(prnt));
	}

	// Token: 0x0600105D RID: 4189 RVA: 0x00046190 File Offset: 0x00044390
	private void RpcReader___Observers_ObserversMakeBlood_3068987916(PooledReader PooledReader0, Channel channel)
	{
		Transform transform = PooledReader0.ReadTransform();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversMakeBlood_3068987916(transform);
	}

	// Token: 0x0600105E RID: 4190 RVA: 0x000461C4 File Offset: 0x000443C4
	private void RpcWriter___Server_DmgPlayerServer_1934289915(GameObject Player)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(Player);
		base.SendServerRpc(38U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600105F RID: 4191 RVA: 0x00046236 File Offset: 0x00044436
	private void RpcLogic___DmgPlayerServer_1934289915(GameObject Player)
	{
		this.DmgPlayerObservers(Player);
	}

	// Token: 0x06001060 RID: 4192 RVA: 0x00046240 File Offset: 0x00044440
	private void RpcReader___Server_DmgPlayerServer_1934289915(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___DmgPlayerServer_1934289915(gameObject);
	}

	// Token: 0x06001061 RID: 4193 RVA: 0x00046274 File Offset: 0x00044474
	private void RpcWriter___Observers_DmgPlayerObservers_1934289915(GameObject Player)
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
		base.SendObserversRpc(39U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06001062 RID: 4194 RVA: 0x000462F8 File Offset: 0x000444F8
	private void RpcLogic___DmgPlayerObservers_1934289915(GameObject Player)
	{
		PlayerMovement playerMovement;
		if (Player.TryGetComponent<PlayerMovement>(out playerMovement) && playerMovement.checkowner())
		{
			playerMovement.DamagePlayer(5f, null, "frogsword");
		}
	}

	// Token: 0x06001063 RID: 4195 RVA: 0x00046328 File Offset: 0x00044528
	private void RpcReader___Observers_DmgPlayerObservers_1934289915(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___DmgPlayerObservers_1934289915(gameObject);
	}

	// Token: 0x06001064 RID: 4196 RVA: 0x0004635C File Offset: 0x0004455C
	private void RpcWriter___Server_ServerMushSword_3068987916(Transform Hitter)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteTransform(Hitter);
		base.SendServerRpc(40U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06001065 RID: 4197 RVA: 0x000463CE File Offset: 0x000445CE
	private void RpcLogic___ServerMushSword_3068987916(Transform Hitter)
	{
		this.ObsMushSword(Hitter);
	}

	// Token: 0x06001066 RID: 4198 RVA: 0x000463D8 File Offset: 0x000445D8
	private void RpcReader___Server_ServerMushSword_3068987916(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Transform transform = PooledReader0.ReadTransform();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerMushSword_3068987916(transform);
	}

	// Token: 0x06001067 RID: 4199 RVA: 0x0004640C File Offset: 0x0004460C
	private void RpcWriter___Observers_ObsMushSword_3068987916(Transform Hitter)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteTransform(Hitter);
		base.SendObserversRpc(41U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06001068 RID: 4200 RVA: 0x00046490 File Offset: 0x00044690
	private void RpcLogic___ObsMushSword_3068987916(Transform Hitter)
	{
		if (base.HasAuthority)
		{
			Vector3 vector = base.transform.position - Hitter.transform.position;
			vector.y = 0f;
			RaycastHit raycastHit;
			NavMeshHit navMeshHit;
			if (Physics.Raycast(new Vector3(this.Spine003.position.x, this.Spine003.position.y + 20f, this.Spine003.position.z) + vector * 15f, Vector3.down, out raycastHit, 40f, this.GroundLayer) && NavMesh.SamplePosition(raycastHit.point, out navMeshHit, 5f, -1))
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.GeneratedNavLink);
				gameObject.transform.GetChild(0).position = base.transform.position;
				gameObject.transform.GetChild(1).position = navMeshHit.position;
				this.knockBackTimer = 0.5f;
				this.KnockBackTargetPos = navMeshHit.position;
				base.StartCoroutine(this.DestroyMeshLink(gameObject));
			}
			this.PlaySoundServer(Random.Range(11, 15));
		}
	}

	// Token: 0x06001069 RID: 4201 RVA: 0x000465C4 File Offset: 0x000447C4
	private void RpcReader___Observers_ObsMushSword_3068987916(PooledReader PooledReader0, Channel channel)
	{
		Transform transform = PooledReader0.ReadTransform();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsMushSword_3068987916(transform);
	}

	// Token: 0x0600106A RID: 4202 RVA: 0x000465F8 File Offset: 0x000447F8
	private void RpcWriter___Server_ServerMonsterHit_431000436(float damage)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteSingle(damage);
		base.SendServerRpc(42U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600106B RID: 4203 RVA: 0x0004666A File Offset: 0x0004486A
	private void RpcLogic___ServerMonsterHit_431000436(float damage)
	{
		this.obsMonsterHit(damage);
	}

	// Token: 0x0600106C RID: 4204 RVA: 0x00046674 File Offset: 0x00044874
	private void RpcReader___Server_ServerMonsterHit_431000436(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		float num = PooledReader0.ReadSingle();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerMonsterHit_431000436(num);
	}

	// Token: 0x0600106D RID: 4205 RVA: 0x000466A8 File Offset: 0x000448A8
	private void RpcWriter___Observers_obsMonsterHit_431000436(float damage)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteSingle(damage);
		base.SendObserversRpc(43U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600106E RID: 4206 RVA: 0x0004672C File Offset: 0x0004492C
	private void RpcLogic___obsMonsterHit_431000436(float damage)
	{
		if (base.HasAuthority)
		{
			this.hp -= damage;
		}
		this.VoiceAudio.pitch = 0.75f;
		this.VoiceAudio.volume = 0.5f;
		this.VoiceAudio.PlayOneShot(this.VoiceClips[Random.Range(11, 15)]);
	}

	// Token: 0x0600106F RID: 4207 RVA: 0x0004678C File Offset: 0x0004498C
	private void RpcReader___Observers_obsMonsterHit_431000436(PooledReader PooledReader0, Channel channel)
	{
		float num = PooledReader0.ReadSingle();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___obsMonsterHit_431000436(num);
	}

	// Token: 0x06001070 RID: 4208 RVA: 0x000467C0 File Offset: 0x000449C0
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
		base.SendServerRpc(44U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06001071 RID: 4209 RVA: 0x00046832 File Offset: 0x00044A32
	private void RpcLogic___ToggleParticles_1140765316(bool val)
	{
		this.ObsToggleParticles(val);
	}

	// Token: 0x06001072 RID: 4210 RVA: 0x0004683C File Offset: 0x00044A3C
	private void RpcReader___Server_ToggleParticles_1140765316(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ToggleParticles_1140765316(flag);
	}

	// Token: 0x06001073 RID: 4211 RVA: 0x00046870 File Offset: 0x00044A70
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
		base.SendObserversRpc(45U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06001074 RID: 4212 RVA: 0x000468F1 File Offset: 0x00044AF1
	private void RpcLogic___ObsToggleParticles_1140765316(bool val)
	{
		if (base.HasAuthority && val)
		{
			this.FireTimer += 10f;
		}
		if (val)
		{
			this.fireParticles.Play();
			return;
		}
		this.fireParticles.Stop();
	}

	// Token: 0x06001075 RID: 4213 RVA: 0x0004692C File Offset: 0x00044B2C
	private void RpcReader___Observers_ObsToggleParticles_1140765316(PooledReader PooledReader0, Channel channel)
	{
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsToggleParticles_1140765316(flag);
	}

	// Token: 0x06001076 RID: 4214 RVA: 0x00044463 File Offset: 0x00042663
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000917 RID: 2327
	public int state;

	// Token: 0x04000918 RID: 2328
	public List<Transform> PotentialTargets = new List<Transform>();

	// Token: 0x04000919 RID: 2329
	private Transform CurrentTarget;

	// Token: 0x0400091A RID: 2330
	private int sneakOrWalk;

	// Token: 0x0400091B RID: 2331
	private int meetAtCenter;

	// Token: 0x0400091C RID: 2332
	private float stayAtCenterTimer;

	// Token: 0x0400091D RID: 2333
	public NavMeshAgent navAgent;

	// Token: 0x0400091E RID: 2334
	public LayerMask GroundLayer;

	// Token: 0x0400091F RID: 2335
	public LayerMask PlayerLayer;

	// Token: 0x04000920 RID: 2336
	public ShadowWizardAnimationController AniController;

	// Token: 0x04000921 RID: 2337
	private float stamina = 10f;

	// Token: 0x04000922 RID: 2338
	private bool canRun = true;

	// Token: 0x04000923 RID: 2339
	private float runCheckTimer;

	// Token: 0x04000924 RID: 2340
	public Transform Spine003;

	// Token: 0x04000925 RID: 2341
	private float lookLeftRightCheckTimer;

	// Token: 0x04000926 RID: 2342
	public AiMAgeBook mgbk;

	// Token: 0x04000927 RID: 2343
	private float fireBallCdTimer;

	// Token: 0x04000928 RID: 2344
	public GameObject fireBall;

	// Token: 0x04000929 RID: 2345
	public GameObject FrostBolt;

	// Token: 0x0400092A RID: 2346
	public GameObject MagicMissle;

	// Token: 0x0400092B RID: 2347
	public GameObject[] Hexes;

	// Token: 0x0400092C RID: 2348
	private Transform[][] hexAiNodes;

	// Token: 0x0400092D RID: 2349
	private Transform PreviousPatrolPoint;

	// Token: 0x0400092E RID: 2350
	private Transform CurrentPatrolPoint;

	// Token: 0x0400092F RID: 2351
	private int targetHex = -1;

	// Token: 0x04000930 RID: 2352
	private float switchHexesTimer;

	// Token: 0x04000931 RID: 2353
	private float switchSearchNodeTimer;

	// Token: 0x04000932 RID: 2354
	private float randomSwitchNodeValue;

	// Token: 0x04000933 RID: 2355
	public bool inited;

	// Token: 0x04000934 RID: 2356
	private bool gottenDirection;

	// Token: 0x04000935 RID: 2357
	private Vector3 LastSeenPlayerPos;

	// Token: 0x04000936 RID: 2358
	private int LastSeenPlayerHex;

	// Token: 0x04000937 RID: 2359
	private NavMeshPath currentPath;

	// Token: 0x04000938 RID: 2360
	private bool LookingAtPlayer;

	// Token: 0x04000939 RID: 2361
	private bool canCastFB;

	// Token: 0x0400093A RID: 2362
	private float fbTimer;

	// Token: 0x0400093B RID: 2363
	private float fbGiveUpTimer;

	// Token: 0x0400093C RID: 2364
	private bool goingLeft;

	// Token: 0x0400093D RID: 2365
	private float navLinkTimer;

	// Token: 0x0400093E RID: 2366
	private bool beganJump;

	// Token: 0x0400093F RID: 2367
	private int prevState;

	// Token: 0x04000940 RID: 2368
	private float jumpDistance;

	// Token: 0x04000941 RID: 2369
	private OffMeshLinkData jumpData;

	// Token: 0x04000942 RID: 2370
	private float freezeCdTimer;

	// Token: 0x04000943 RID: 2371
	private float mmCdTimer;

	// Token: 0x04000944 RID: 2372
	private Transform FireballDetected;

	// Token: 0x04000945 RID: 2373
	private int DodgeDirection;

	// Token: 0x04000946 RID: 2374
	private float fbdodgetimer;

	// Token: 0x04000947 RID: 2375
	private bool equippedKnife;

	// Token: 0x04000948 RID: 2376
	public AudioSource VoiceAudio;

	// Token: 0x04000949 RID: 2377
	public AudioClip[] VoiceClips;

	// Token: 0x0400094A RID: 2378
	private float hp = 50f;

	// Token: 0x0400094B RID: 2379
	private bool isDead;

	// Token: 0x0400094C RID: 2380
	public GameObject VisualObject;

	// Token: 0x0400094D RID: 2381
	public GameObject ShadowMageDeth;

	// Token: 0x0400094E RID: 2382
	private bool isFrozen;

	// Token: 0x0400094F RID: 2383
	public GameObject icebox;

	// Token: 0x04000950 RID: 2384
	private float dartCDTimer;

	// Token: 0x04000951 RID: 2385
	private float knifeHitTimer;

	// Token: 0x04000952 RID: 2386
	private float randomizedKnifeHitCd = 0.3f;

	// Token: 0x04000953 RID: 2387
	public GameObject Blood;

	// Token: 0x04000954 RID: 2388
	public GameObject GeneratedNavLink;

	// Token: 0x04000955 RID: 2389
	private Vector3 KnockBackTargetPos;

	// Token: 0x04000956 RID: 2390
	private float knockBackTimer;

	// Token: 0x04000957 RID: 2391
	private bool hasPlayedCastSound;

	// Token: 0x04000958 RID: 2392
	public Vector3 SpawnPos;

	// Token: 0x04000959 RID: 2393
	private bool hasSetNewSpawnPos;

	// Token: 0x0400095A RID: 2394
	public ParticleSystem fireParticles;

	// Token: 0x0400095B RID: 2395
	private float FireTimer;

	// Token: 0x0400095C RID: 2396
	public bool isCastleDefender;

	// Token: 0x0400095D RID: 2397
	public Collider[] swizcols;

	// Token: 0x0400095E RID: 2398
	private GetRespPortal respport;

	// Token: 0x0400095F RID: 2399
	public Vector3 spawnOffset;

	// Token: 0x04000960 RID: 2400
	private bool isinblackcastle = true;

	// Token: 0x04000961 RID: 2401
	public PlayerRespawnManager PRM;

	// Token: 0x04000962 RID: 2402
	private bool freezebreak;

	// Token: 0x04000963 RID: 2403
	public GameObject brokenCube;

	// Token: 0x04000964 RID: 2404
	private bool NetworkInitialize___EarlyShadowWizardAIAssembly-CSharp.dll_Excuted;

	// Token: 0x04000965 RID: 2405
	private bool NetworkInitialize__LateShadowWizardAIAssembly-CSharp.dll_Excuted;
}
