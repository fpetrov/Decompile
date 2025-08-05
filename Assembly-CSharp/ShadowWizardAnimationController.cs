using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200018A RID: 394
public class ShadowWizardAnimationController : MonoBehaviour
{
	// Token: 0x060010AD RID: 4269 RVA: 0x00047429 File Offset: 0x00045629
	private void Start()
	{
		this.prevFramPos = base.transform.position;
	}

	// Token: 0x060010AE RID: 4270 RVA: 0x0004743C File Offset: 0x0004563C
	private void Update()
	{
		this.ShadowAni.SetBool("jump", this.jumpBool);
		this.magnitude = Mathf.Lerp(this.magnitude, ((base.transform.position - this.prevFramPos) / Time.deltaTime).magnitude, Time.deltaTime * 5f);
		this.magnitude = Mathf.Clamp(this.magnitude, 0f, 20f);
		this.prevFramPos = base.transform.position;
		if (this.magnitude > 1f && this.magnitude < 5.5f)
		{
			this.ShadowAni.SetBool("walk", true);
			this.ShadowAni.SetBool("run", false);
		}
		else if (this.magnitude > 5f)
		{
			this.ShadowAni.SetBool("run", true);
		}
		else
		{
			this.ShadowAni.SetBool("walk", false);
			this.ShadowAni.SetBool("run", false);
		}
		if (this.ShadowAni.GetBool("crouch") && !this.ShadowAni.GetBool("run"))
		{
			this.VisualTransform.localPosition = Vector3.Lerp(this.VisualTransform.localPosition, new Vector3(0f, -0.6f, 0f), Time.deltaTime * 5f);
		}
		else if (base.transform.position.y != -0.1f)
		{
			this.VisualTransform.localPosition = Vector3.Lerp(this.VisualTransform.localPosition, new Vector3(0f, -0.1f, 0f), Time.deltaTime * 5f);
		}
		RaycastHit raycastHit;
		if (this.PlayerSource.isActiveAndEnabled && Physics.Raycast(new Vector3(base.transform.position.x, base.transform.position.y + 0.2f, base.transform.position.z), Vector3.down, out raycastHit, 0.4f, this.GroundLayer))
		{
			this.footstepTimer += this.magnitude * Time.deltaTime;
			if (raycastHit.transform.CompareTag("WaterGround"))
			{
				if (this.magnitude > 6f && this.magnitude < 10f)
				{
					if (this.footstepTimer > this.runStepInterval)
					{
						if (this.PreviousStepIndex > 19 || (this.PreviousStepIndex <= 15 && this.PlayerSource.isActiveAndEnabled))
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

	// Token: 0x060010AF RID: 4271 RVA: 0x0004846B File Offset: 0x0004666B
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

	// Token: 0x060010B0 RID: 4272 RVA: 0x000484AC File Offset: 0x000466AC
	private void LateUpdate()
	{
		if (!this.isFrozen)
		{
			if (this.LookAtPosition)
			{
				Quaternion quaternion = Quaternion.LookRotation(this.PositionToLookAt - this.SpineRotate.position);
				this.SpineRotate.rotation = Quaternion.Lerp(this.prevSpineRotate, quaternion, Time.deltaTime * 2f);
				this.LookAtPlayerToggleOffTimer = 0f;
			}
			else if (this.LookAtPlayer && this.PlayerToLookAt != null)
			{
				Quaternion quaternion2 = Quaternion.LookRotation(new Vector3(this.PlayerToLookAt.position.x, this.PlayerToLookAt.position.y + 1.2f, this.PlayerToLookAt.position.z) - this.SpineRotate.position);
				this.SpineRotate.rotation = Quaternion.Lerp(this.prevSpineRotate, quaternion2, Time.deltaTime * 2f);
				this.LookAtPlayerToggleOffTimer = 0f;
			}
			else if (!this.LookAtPlayer && this.LookAtPlayerToggleOffTimer < 1f)
			{
				this.LookAtPlayerToggleOffTimer += Time.deltaTime;
				Quaternion quaternion3 = Quaternion.LookRotation(base.transform.forward);
				this.SpineRotate.rotation = Quaternion.Lerp(this.prevSpineRotate, quaternion3, Time.deltaTime);
			}
			else if (this.LookLeftToRight == 2)
			{
				if (this.LTRphase == 0f)
				{
					this.ltrTimer += Time.deltaTime / 2f;
					this.ltrTimer = Mathf.Clamp01(this.ltrTimer);
					this.SpineRotate.localRotation = Quaternion.Slerp(Quaternion.Euler(0f, 0f, 0f), Quaternion.Euler(0f, -35f, 0f), this.ltrTimer * 4f);
					if (this.ltrTimer == 1f)
					{
						this.LTRphase = 1f;
						this.ltrTimer = 0f;
					}
				}
				else if (this.LTRphase == 1f)
				{
					this.ltrTimer += Time.deltaTime / 4f;
					this.ltrTimer = Mathf.Clamp01(this.ltrTimer);
					this.SpineRotate.localRotation = Quaternion.Slerp(Quaternion.Euler(0f, -35f, 0f), Quaternion.Euler(0f, 35f, 0f), this.ltrTimer * 4f);
					if (this.ltrTimer == 1f)
					{
						this.LTRphase = 2f;
						this.ltrTimer = 0f;
					}
				}
				else if (this.LTRphase == 2f)
				{
					this.ltrTimer += Time.deltaTime / 2f;
					this.ltrTimer = Mathf.Clamp01(this.ltrTimer);
					this.SpineRotate.localRotation = Quaternion.Slerp(Quaternion.Euler(0f, 35f, 0f), Quaternion.Euler(0f, 0f, 0f), this.ltrTimer * 4f);
					if (this.ltrTimer == 1f)
					{
						this.LookLeftToRight = 3;
						this.ltrTimer = 0f;
						this.LTRphase = 0f;
					}
				}
			}
			else if (this.ShadowAni.GetBool("run"))
			{
				this.SpineRotate.localRotation = Quaternion.Lerp(this.SpineRotate.localRotation, Quaternion.Euler(Mathf.Sin(Time.time * 12f) * 10f + 10f, 0f, 0f), Time.deltaTime * 25f);
			}
			this.prevSpineRotate = this.SpineRotate.rotation;
		}
	}

	// Token: 0x060010B1 RID: 4273 RVA: 0x0004888E File Offset: 0x00046A8E
	public void toggleSneak(bool setter)
	{
		this.ShadowAni.SetBool("crouch", setter);
	}

	// Token: 0x060010B2 RID: 4274 RVA: 0x000488A1 File Offset: 0x00046AA1
	private IEnumerator LayerMaskSwapOne()
	{
		if (this.ShadowAni.GetLayerWeight(1) != 1f)
		{
			float timer = 0f;
			while (timer < 0.09f)
			{
				timer += Time.deltaTime;
				this.ShadowAni.SetLayerWeight(1, timer * 12.5f);
				yield return null;
			}
			this.ShadowAni.SetLayerWeight(1, 1f);
		}
		yield break;
	}

	// Token: 0x060010B3 RID: 4275 RVA: 0x000488B0 File Offset: 0x00046AB0
	private IEnumerator LayerMaskSwapZero()
	{
		if (this.ShadowAni.GetLayerWeight(1) != 0f)
		{
			float timer = 0f;
			while (timer < 0.09f)
			{
				timer += Time.deltaTime;
				this.ShadowAni.SetLayerWeight(1, 1f - timer * 12.5f);
				yield return null;
			}
			this.ShadowAni.SetLayerWeight(1, 0f);
		}
		yield break;
	}

	// Token: 0x060010B4 RID: 4276 RVA: 0x000488BF File Offset: 0x00046ABF
	public void SwapItemAnis(bool swapToKnife)
	{
		base.StartCoroutine(this.SwapItemRoutine(swapToKnife));
	}

	// Token: 0x060010B5 RID: 4277 RVA: 0x000488CF File Offset: 0x00046ACF
	private IEnumerator SwapItemRoutine(bool swapToKnife)
	{
		float timer = 0f;
		if (this.ShadowAni.GetLayerWeight(1) != 0f)
		{
			while (timer < 0.09f)
			{
				timer += Time.deltaTime;
				this.ShadowAni.SetLayerWeight(1, 1f - timer * 12.5f);
				yield return null;
			}
			this.ShadowAni.SetLayerWeight(1, 0f);
		}
		timer = 0f;
		this.mgitems.SwapItem(swapToKnife);
		if (this.ShadowAni.GetLayerWeight(1) != 1f)
		{
			while (timer < 0.09f)
			{
				timer += Time.deltaTime;
				this.ShadowAni.SetLayerWeight(1, timer * 12.5f);
				yield return null;
			}
			this.ShadowAni.SetLayerWeight(1, 1f);
		}
		yield break;
	}

	// Token: 0x060010B6 RID: 4278 RVA: 0x000488E5 File Offset: 0x00046AE5
	public void ToggleAttackAni(bool tf)
	{
		this.ShadowAni.SetBool("attack", tf);
	}

	// Token: 0x0400098D RID: 2445
	public Animator ShadowAni;

	// Token: 0x0400098E RID: 2446
	public NavMeshAgent navAgent;

	// Token: 0x0400098F RID: 2447
	public Transform VisualTransform;

	// Token: 0x04000990 RID: 2448
	public Transform SpineRotate;

	// Token: 0x04000991 RID: 2449
	public LayerMask GroundLayer;

	// Token: 0x04000992 RID: 2450
	public int LookLeftToRight;

	// Token: 0x04000993 RID: 2451
	public float LTRphase;

	// Token: 0x04000994 RID: 2452
	public float ltrTimer;

	// Token: 0x04000995 RID: 2453
	public AudioClip[] FootSteps;

	// Token: 0x04000996 RID: 2454
	public AudioSource PlayerSource;

	// Token: 0x04000997 RID: 2455
	private float footstepTimer;

	// Token: 0x04000998 RID: 2456
	public float walkStepInterval;

	// Token: 0x04000999 RID: 2457
	public float runStepInterval;

	// Token: 0x0400099A RID: 2458
	public float crouchStepInterval;

	// Token: 0x0400099B RID: 2459
	private int FourstepIndex;

	// Token: 0x0400099C RID: 2460
	private int PreviousStepIndex;

	// Token: 0x0400099D RID: 2461
	public bool LookAtPlayer;

	// Token: 0x0400099E RID: 2462
	private float LookAtPlayerToggleOffTimer;

	// Token: 0x0400099F RID: 2463
	private Quaternion prevSpineRotate = Quaternion.identity;

	// Token: 0x040009A0 RID: 2464
	public Transform PlayerToLookAt;

	// Token: 0x040009A1 RID: 2465
	public bool LookAtPosition;

	// Token: 0x040009A2 RID: 2466
	public Vector3 PositionToLookAt;

	// Token: 0x040009A3 RID: 2467
	public bool jumpBool;

	// Token: 0x040009A4 RID: 2468
	private Vector3 prevFramPos;

	// Token: 0x040009A5 RID: 2469
	public AiMAgeBook mgitems;

	// Token: 0x040009A6 RID: 2470
	private float magnitude;

	// Token: 0x040009A7 RID: 2471
	public AudioClip SplashSound;

	// Token: 0x040009A8 RID: 2472
	public bool isFrozen;
}
