using System;
using UnityEngine;

// Token: 0x0200010B RID: 267
public class PlayerAudioPlayer : MonoBehaviour
{
	// Token: 0x06000AC7 RID: 2759 RVA: 0x00028574 File Offset: 0x00026774
	private void Update()
	{
		Debug.DrawRay(new Vector3(base.transform.position.x, base.transform.position.y + 0.2f, base.transform.position.z), Vector3.down * 0.4f, Color.red);
		RaycastHit raycastHit;
		if (Physics.Raycast(new Vector3(base.transform.position.x, base.transform.position.y + 0.2f, base.transform.position.z), Vector3.down, out raycastHit, 0.4f, this.groundLayer))
		{
			this.magnitude = Mathf.Lerp(this.magnitude, ((base.transform.position - this.prevFramPos) / Time.deltaTime).magnitude, Time.deltaTime * 10f);
			this.magnitude = Mathf.Clamp(this.magnitude, 0f, 20f);
			this.footstepTimer += this.magnitude * Time.deltaTime;
			this.prevFramPos = base.transform.position;
			if (raycastHit.transform.CompareTag("WaterGround") || this.oceanint > 0)
			{
				if (this.magnitude > 6f && this.magnitude < 10f)
				{
					if (this.footstepTimer > this.runStepInterval)
					{
						if (this.PreviousStepIndex > 19 || this.PreviousStepIndex <= 15)
						{
							this.PlayerSource.PlayOneShot(this.SplashSound);
						}
						this.GetNewIndex(16, 19, this.FourstepIndex);
						this.PlayerSource.volume = Random.Range(0.1f, 0.2f);
						this.footstepTimer = 0f;
						this.PlayerSource.pitch = Random.Range(1f, 1.2f);
						this.PlayerSource.PlayOneShot(this.FootSteps[this.FourstepIndex]);
					}
				}
				else if (this.magnitude > 0.3f && this.footstepTimer > this.walkStepInterval)
				{
					if (this.PreviousStepIndex > 19 || this.PreviousStepIndex <= 15)
					{
						this.PlayerSource.PlayOneShot(this.SplashSound);
					}
					this.GetNewIndex(16, 19, this.FourstepIndex);
					this.PlayerSource.volume = Random.Range(0.05f, 0.15f);
					this.footstepTimer = 0f;
					this.PlayerSource.pitch = Random.Range(0.85f, 1.05f);
					this.PlayerSource.PlayOneShot(this.FootSteps[this.FourstepIndex]);
				}
			}
			else if (raycastHit.transform.CompareTag("DirtGround"))
			{
				if (this.magnitude > 6f && this.magnitude < 10f)
				{
					if (this.footstepTimer > this.runStepInterval)
					{
						this.GetNewIndex(0, 3, this.FourstepIndex);
						this.footstepTimer = 0f;
						this.PlayerSource.volume = Random.Range(0.1f, 0.2f);
						this.PlayerSource.pitch = Random.Range(0.8f, 1.1f);
						this.PlayerSource.PlayOneShot(this.FootSteps[this.FourstepIndex]);
					}
				}
				else if (this.magnitude > 0.3f && this.footstepTimer > this.walkStepInterval)
				{
					this.GetNewIndex(0, 3, this.FourstepIndex);
					this.PlayerSource.volume = Random.Range(0.05f, 0.15f);
					this.footstepTimer = 0f;
					this.PlayerSource.pitch = Random.Range(0.7f, 1.2f);
					this.PlayerSource.PlayOneShot(this.FootSteps[this.FourstepIndex]);
				}
			}
			else if (raycastHit.transform.CompareTag("LeavesGround"))
			{
				if (this.magnitude > 6f && this.magnitude < 10f)
				{
					if (this.footstepTimer > this.runStepInterval)
					{
						this.GetNewIndex(4, 7, this.FourstepIndex);
						this.footstepTimer = 0f;
						this.PlayerSource.volume = Random.Range(0.7f, 1f);
						this.PlayerSource.pitch = Random.Range(0.9f, 1f);
						this.PlayerSource.PlayOneShot(this.FootSteps[this.FourstepIndex]);
					}
				}
				else if (this.magnitude > 0.3f && this.footstepTimer > this.walkStepInterval)
				{
					this.GetNewIndex(4, 7, this.FourstepIndex);
					this.PlayerSource.volume = Random.Range(0.5f, 0.7f);
					this.footstepTimer = 0f;
					this.PlayerSource.pitch = Random.Range(0.8f, 1f);
					this.PlayerSource.PlayOneShot(this.FootSteps[this.FourstepIndex]);
				}
			}
			else if (raycastHit.transform.CompareTag("RockGround") || raycastHit.transform.CompareTag("wall") || raycastHit.transform.CompareTag("hallway"))
			{
				if (this.magnitude > 6f && this.magnitude < 10f)
				{
					if (this.footstepTimer > this.runStepInterval)
					{
						this.GetNewIndex(8, 11, this.FourstepIndex);
						this.footstepTimer = 0f;
						this.PlayerSource.volume = Random.Range(0.1f, 0.2f);
						this.PlayerSource.pitch = Random.Range(0.8f, 1f);
						this.PlayerSource.PlayOneShot(this.FootSteps[this.FourstepIndex]);
					}
				}
				else if (this.magnitude > 0.3f && this.footstepTimer > this.walkStepInterval)
				{
					this.GetNewIndex(8, 11, this.FourstepIndex);
					this.PlayerSource.volume = Random.Range(0.05f, 0.15f);
					this.footstepTimer = 0f;
					this.PlayerSource.pitch = Random.Range(0.7f, 1f);
					this.PlayerSource.PlayOneShot(this.FootSteps[this.FourstepIndex]);
				}
			}
			else if (raycastHit.transform.CompareTag("GrassGround"))
			{
				if (this.magnitude > 6f && this.magnitude < 10f)
				{
					if (this.footstepTimer > this.runStepInterval)
					{
						this.GetNewIndex(20, 23, this.FourstepIndex);
						this.footstepTimer = 0f;
						this.PlayerSource.volume = Random.Range(0.05f, 0.1f);
						this.PlayerSource.pitch = Random.Range(0.8f, 1f);
						this.PlayerSource.PlayOneShot(this.FootSteps[this.FourstepIndex]);
					}
				}
				else if (this.magnitude > 0.3f && this.footstepTimer > this.walkStepInterval)
				{
					this.GetNewIndex(20, 23, this.FourstepIndex);
					this.PlayerSource.volume = Random.Range(0.03f, 0.075f);
					this.footstepTimer = 0f;
					this.PlayerSource.pitch = Random.Range(0.7f, 1f);
					this.PlayerSource.PlayOneShot(this.FootSteps[this.FourstepIndex]);
				}
			}
			else if (raycastHit.transform.CompareTag("ForestFloor"))
			{
				if (this.magnitude > 6f && this.magnitude < 10f)
				{
					if (this.footstepTimer > this.runStepInterval)
					{
						this.GetNewIndex(20, 23, this.FourstepIndex);
						this.footstepTimer = 0f;
						this.PlayerSource.volume = Random.Range(0.05f, 0.1f);
						this.PlayerSource.pitch = Random.Range(0.8f, 1f);
						this.PlayerSource.PlayOneShot(this.FootSteps[this.FourstepIndex]);
						if (Random.Range(0, 3) != 0)
						{
							this.PlayerSource.PlayOneShot(this.FootSteps[Random.Range(40, 48)]);
						}
					}
				}
				else if (this.magnitude > 0.3f && this.footstepTimer > this.walkStepInterval)
				{
					this.GetNewIndex(20, 23, this.FourstepIndex);
					this.PlayerSource.volume = Random.Range(0.03f, 0.075f);
					this.footstepTimer = 0f;
					this.PlayerSource.pitch = Random.Range(0.7f, 1f);
					this.PlayerSource.PlayOneShot(this.FootSteps[this.FourstepIndex]);
					if (Random.Range(0, 3) != 0)
					{
						this.PlayerSource.PlayOneShot(this.FootSteps[Random.Range(40, 48)]);
					}
				}
			}
			else if (raycastHit.transform.CompareTag("SoftDirtGround"))
			{
				if (this.magnitude > 6f && this.magnitude < 10f)
				{
					if (this.footstepTimer > this.runStepInterval)
					{
						this.GetNewIndex(24, 27, this.FourstepIndex);
						this.footstepTimer = 0f;
						this.PlayerSource.volume = Random.Range(0.08f, 0.105f);
						this.PlayerSource.pitch = Random.Range(0.8f, 1f);
						this.PlayerSource.PlayOneShot(this.FootSteps[this.FourstepIndex]);
					}
				}
				else if (this.magnitude > 0.3f && this.footstepTimer > this.walkStepInterval)
				{
					this.GetNewIndex(24, 27, this.FourstepIndex);
					this.PlayerSource.volume = Random.Range(0.05f, 0.09f);
					this.footstepTimer = 0f;
					this.PlayerSource.pitch = Random.Range(0.7f, 1f);
					this.PlayerSource.PlayOneShot(this.FootSteps[this.FourstepIndex]);
				}
			}
			else if (raycastHit.transform.CompareTag("SinkBog") || raycastHit.transform.CompareTag("LavaGround"))
			{
				if (this.magnitude > 0.25f && this.footstepTimer > this.walkStepInterval + 0.2f)
				{
					this.GetNewIndex(12, 15, this.FourstepIndex);
					this.PlayerSource.volume = Random.Range(0.05f, 0.15f);
					this.footstepTimer = 0f;
					this.PlayerSource.pitch = Random.Range(0.8f, 1f);
					this.PlayerSource.PlayOneShot(this.FootSteps[this.FourstepIndex]);
				}
			}
			else if (raycastHit.transform.CompareTag("CrystalGround"))
			{
				if (this.magnitude > 6f && this.magnitude < 10f)
				{
					if (this.footstepTimer > this.runStepInterval)
					{
						this.GetNewIndex(28, 31, this.FourstepIndex);
						this.footstepTimer = 0f;
						this.PlayerSource.volume = Random.Range(0.1f, 0.15f);
						this.PlayerSource.pitch = Random.Range(0.9f, 1f);
						this.PlayerSource.PlayOneShot(this.FootSteps[this.FourstepIndex]);
					}
				}
				else if (this.magnitude > 0.3f && this.footstepTimer > this.walkStepInterval)
				{
					this.GetNewIndex(28, 31, this.FourstepIndex);
					this.PlayerSource.volume = Random.Range(0.05f, 0.1f);
					this.footstepTimer = 0f;
					this.PlayerSource.pitch = Random.Range(0.85f, 1f);
					this.PlayerSource.PlayOneShot(this.FootSteps[this.FourstepIndex]);
				}
			}
			else if (raycastHit.transform.CompareTag("SandGround"))
			{
				if (this.magnitude > 6f && this.magnitude < 10f)
				{
					if (this.footstepTimer > this.runStepInterval)
					{
						this.GetNewIndex(32, 35, this.FourstepIndex);
						this.footstepTimer = 0f;
						this.PlayerSource.volume = Random.Range(0.15f, 0.2f);
						this.PlayerSource.pitch = Random.Range(0.9f, 1f);
						this.PlayerSource.PlayOneShot(this.FootSteps[this.FourstepIndex]);
					}
				}
				else if (this.magnitude > 0.3f && this.footstepTimer > this.walkStepInterval)
				{
					this.GetNewIndex(32, 35, this.FourstepIndex);
					this.PlayerSource.volume = Random.Range(0.13f, 0.175f);
					this.footstepTimer = 0f;
					this.PlayerSource.pitch = Random.Range(0.8f, 1f);
					this.PlayerSource.PlayOneShot(this.FootSteps[this.FourstepIndex]);
				}
			}
			else if (raycastHit.transform.CompareTag("MudGround"))
			{
				if (this.magnitude > 6f && this.magnitude < 10f)
				{
					if (this.footstepTimer > this.runStepInterval)
					{
						this.GetNewIndex(36, 39, this.FourstepIndex);
						this.footstepTimer = 0f;
						this.PlayerSource.volume = Random.Range(0.13f, 0.17f);
						this.PlayerSource.pitch = Random.Range(0.7f, 0.8f);
						this.PlayerSource.PlayOneShot(this.FootSteps[this.FourstepIndex]);
					}
				}
				else if (this.magnitude > 0.3f && this.footstepTimer > this.walkStepInterval)
				{
					this.GetNewIndex(36, 39, this.FourstepIndex);
					this.PlayerSource.volume = Random.Range(0.1f, 0.15f);
					this.footstepTimer = 0f;
					this.PlayerSource.pitch = Random.Range(0.6f, 0.7f);
					this.PlayerSource.PlayOneShot(this.FootSteps[this.FourstepIndex]);
				}
			}
			this.PreviousStepIndex = this.FourstepIndex;
		}
	}

	// Token: 0x06000AC8 RID: 2760 RVA: 0x0002949C File Offset: 0x0002769C
	private void GetNewIndex(int MinVal, int MaxVal, int CurrentVal)
	{
		this.FourstepIndex = Random.Range(MinVal, MaxVal + 1);
		if (this.FourstepIndex == CurrentVal)
		{
			if (this.FourstepIndex < MaxVal)
			{
				this.FourstepIndex++;
				return;
			}
			this.FourstepIndex--;
		}
	}

	// Token: 0x06000AC9 RID: 2761 RVA: 0x000294DC File Offset: 0x000276DC
	public void PlayBoing()
	{
		this.PlayerSource.volume = Random.Range(0.075f, 0.125f);
		this.PlayerSource.pitch = Random.Range(0.85f, 1f);
		this.PlayerSource.PlayOneShot(this.StaffBoings[Random.Range(0, this.StaffBoings.Length)]);
	}

	// Token: 0x040005B5 RID: 1461
	public AudioClip[] FootSteps;

	// Token: 0x040005B6 RID: 1462
	public AudioClip SplashSound;

	// Token: 0x040005B7 RID: 1463
	public LayerMask groundLayer;

	// Token: 0x040005B8 RID: 1464
	public AudioSource PlayerSource;

	// Token: 0x040005B9 RID: 1465
	private float footstepTimer;

	// Token: 0x040005BA RID: 1466
	public float walkStepInterval;

	// Token: 0x040005BB RID: 1467
	public float runStepInterval;

	// Token: 0x040005BC RID: 1468
	private int FourstepIndex;

	// Token: 0x040005BD RID: 1469
	private int PreviousStepIndex;

	// Token: 0x040005BE RID: 1470
	public AudioClip[] StaffBoings;

	// Token: 0x040005BF RID: 1471
	private Vector3 prevFramPos = Vector3.zero;

	// Token: 0x040005C0 RID: 1472
	public float magnitude;

	// Token: 0x040005C1 RID: 1473
	public int oceanint;
}
