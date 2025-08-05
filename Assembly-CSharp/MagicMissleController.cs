using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x020000D1 RID: 209
public class MagicMissleController : MonoBehaviour
{
	// Token: 0x0600084A RID: 2122 RVA: 0x0001FEFC File Offset: 0x0001E0FC
	public void SetUp(GameObject ownerobj, Vector3 fwdvector, GameObject Target, bool aishooter, int level)
	{
		this.spelllevel = level;
		this.spelllevel = Mathf.Clamp(this.spelllevel, 0, 10);
		this.playerOwner = ownerobj;
		this.MissleTarget = Target;
		this.shotByAi = aishooter;
		this.rb.AddForce(fwdvector * this.muzzleVelocity, ForceMode.VelocityChange);
		this.forwardVector = fwdvector;
	}

	// Token: 0x0600084B RID: 2123 RVA: 0x0001FF5A File Offset: 0x0001E15A
	public void AISetup(Vector3 fwdvector, GameObject Target, bool aishooter)
	{
		this.playerOwner = null;
		this.MissleTarget = Target;
		this.shotByAi = aishooter;
		this.rb.AddForce(fwdvector * this.muzzleVelocity, ForceMode.VelocityChange);
		this.forwardVector = fwdvector;
	}

	// Token: 0x0600084C RID: 2124 RVA: 0x0001FF90 File Offset: 0x0001E190
	private void Update()
	{
		if (this.MissleTarget == null)
		{
			this.rb.useGravity = true;
			Collider[] array = Physics.OverlapSphere(base.transform.position, 30f, this.playerLayer);
			float num = float.MaxValue;
			foreach (Collider collider in array)
			{
				Vector3 vector = collider.transform.position - base.transform.position;
				float magnitude = vector.magnitude;
				float num2 = Vector3.Angle(this.forwardVector, vector.normalized);
				PlayerMovement playerMovement;
				GetPlayerGameobject getPlayerGameobject;
				if (num2 <= 90f && (!collider.TryGetComponent<PlayerMovement>(out playerMovement) || this.playerOwner.GetComponent<PlayerMovement>().playerTeam != playerMovement.playerTeam) && (!collider.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject) || getPlayerGameobject.player.GetComponent<PlayerMovement>().playerTeam != this.playerOwner.GetComponent<PlayerMovement>().playerTeam) && (!this.shotByAi || (!collider.CompareTag("PlayerNpc") && !collider.CompareTag("Ignorable"))))
				{
					float num3 = magnitude + num2 * 0.5f;
					if (num3 < num)
					{
						num = num3;
						this.MissleTarget = collider.gameObject;
					}
				}
			}
		}
		else if (!this.collided)
		{
			this.rb.useGravity = false;
			if (this.MissleTarget.CompareTag("Player"))
			{
				this.desiredHomingMissleVelocity = (new Vector3(this.MissleTarget.transform.position.x, this.MissleTarget.transform.position.y + 1f, this.MissleTarget.transform.position.z) - base.transform.position).normalized * this.speedmult;
				this.rb.velocity = Vector3.Lerp(this.rb.velocity, this.desiredHomingMissleVelocity, this.lerpmult * Time.deltaTime);
			}
			else
			{
				this.desiredHomingMissleVelocity = (new Vector3(this.MissleTarget.transform.position.x, this.MissleTarget.transform.position.y + 0.5f, this.MissleTarget.transform.position.z) - base.transform.position).normalized * this.speedmult;
				this.rb.velocity = Vector3.Lerp(this.rb.velocity, this.desiredHomingMissleVelocity, this.lerpmult * Time.deltaTime);
			}
		}
		if (this.timeouttimer > 4f)
		{
			this.timeouttimer = 0f;
			this.mmsource.pitch = Random.Range(0.8f, 1.2f);
			this.mmsource.PlayOneShot(this.mmhit);
			base.StartCoroutine(this.DestroyuGameobject());
		}
		this.timeouttimer += Time.deltaTime;
	}

	// Token: 0x0600084D RID: 2125 RVA: 0x000202AC File Offset: 0x0001E4AC
	private void OnCollisionEnter(Collision other)
	{
		GetPlayerGameobject getPlayerGameobject;
		PlayerMovement playerMovement;
		if (!this.collided && (!this.shotByAi || !other.transform.CompareTag("PlayerNpc")) && (!other.transform.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject) || !(getPlayerGameobject.player == this.playerOwner)) && (!other.transform.TryGetComponent<PlayerMovement>(out playerMovement) || !(this.playerOwner == playerMovement.gameObject)))
		{
			this.mmsource.pitch = Random.Range(0.8f, 1.2f);
			this.mmsource.PlayOneShot(this.mmhit);
			this.collided = true;
			PlayerMovement playerMovement2;
			GetShadowWizardController getShadowWizardController;
			if (other.transform.TryGetComponent<PlayerMovement>(out playerMovement2))
			{
				if (this.shotByAi)
				{
					playerMovement2.NonRpcDamagePlayer(10f, this.playerOwner, "missle");
				}
				else
				{
					playerMovement2.NonRpcDamagePlayer(12.5f + (float)(this.spelllevel / 2), this.playerOwner, "missle");
				}
			}
			else if (other.transform.TryGetComponent<GetShadowWizardController>(out getShadowWizardController))
			{
				getShadowWizardController.ShadowWizardAI.GetComponent<ShadowWizardAI>().HitMonsterNotNetworked((float)(14 + this.spelllevel * 2));
			}
			else if (other.transform.CompareTag("hitable"))
			{
				other.transform.GetComponent<IInteractableNetworkObj>().NetInteract();
			}
			base.StartCoroutine(this.DestroyuGameobject());
		}
	}

	// Token: 0x0600084E RID: 2126 RVA: 0x0002040A File Offset: 0x0001E60A
	private IEnumerator DestroyuGameobject()
	{
		float timer = 0f;
		float startvol = this.mmsource.volume;
		while (timer < 1f)
		{
			this.hdld.range = Mathf.Lerp(0.28f, 40f, timer * 2f);
			this.hdld.intensity = Mathf.Lerp(400000f, 10000f, timer);
			if (timer >= 0.8f)
			{
				this.mmsource.volume = Mathf.Lerp(startvol, 0f, (timer - 0.8f) * 5f);
			}
			timer += Time.deltaTime;
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x04000434 RID: 1076
	private bool collided;

	// Token: 0x04000435 RID: 1077
	private bool shotByAi;

	// Token: 0x04000436 RID: 1078
	public GameObject playerOwner;

	// Token: 0x04000437 RID: 1079
	public AudioSource mmsource;

	// Token: 0x04000438 RID: 1080
	public AudioClip mmhit;

	// Token: 0x04000439 RID: 1081
	private GameObject MissleTarget;

	// Token: 0x0400043A RID: 1082
	public float speedmult = 1f;

	// Token: 0x0400043B RID: 1083
	public float lerpmult = 1f;

	// Token: 0x0400043C RID: 1084
	public LayerMask playerLayer;

	// Token: 0x0400043D RID: 1085
	public Rigidbody rb;

	// Token: 0x0400043E RID: 1086
	public float muzzleVelocity = 20f;

	// Token: 0x0400043F RID: 1087
	private float timeouttimer;

	// Token: 0x04000440 RID: 1088
	public HDAdditionalLightData hdld;

	// Token: 0x04000441 RID: 1089
	private Vector3 forwardVector;

	// Token: 0x04000442 RID: 1090
	private Vector3 desiredHomingMissleVelocity;

	// Token: 0x04000443 RID: 1091
	private int spelllevel = 1;
}
