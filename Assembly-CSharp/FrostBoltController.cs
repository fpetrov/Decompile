using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x0200009A RID: 154
public class FrostBoltController : MonoBehaviour
{
	// Token: 0x06000670 RID: 1648 RVA: 0x00018E17 File Offset: 0x00017017
	private void FixedUpdate()
	{
		this.rb.MoveRotation(Quaternion.LookRotation(this.rb.velocity));
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x00018E34 File Offset: 0x00017034
	public void PlayerSetup(GameObject ownerobj, Vector3 fwdVector, int Level)
	{
		this.level = Level;
		this.playerOwner = ownerobj;
		this.rb.AddForce(fwdVector * this.muzzleVelocity, ForceMode.VelocityChange);
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x00018E5C File Offset: 0x0001705C
	public void Setup(Vector3 target, bool maxHeight)
	{
		Vector3 position = base.transform.position;
		this.ShotByAi = true;
		float num;
		if (maxHeight)
		{
			if (FrostBoltController.CalculateHighTrajectory(position, target, this.muzzleVelocity, this.gravity, out num))
			{
				base.transform.LookAt(target);
				base.transform.rotation = Quaternion.Euler(num, base.transform.eulerAngles.y, base.transform.eulerAngles.z);
				if (this.rb != null)
				{
					this.rb.AddForce(base.transform.forward * this.muzzleVelocity, ForceMode.VelocityChange);
					return;
				}
			}
		}
		else if (FrostBoltController.CalculateLowTrajectory(position, target, this.muzzleVelocity, this.gravity, out num))
		{
			base.transform.LookAt(target);
			base.transform.rotation = Quaternion.Euler(num, base.transform.eulerAngles.y, base.transform.eulerAngles.z);
			if (this.rb != null)
			{
				this.rb.AddForce(base.transform.forward * this.muzzleVelocity, ForceMode.VelocityChange);
			}
		}
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x00018F94 File Offset: 0x00017194
	public static bool CalculateLowTrajectory(Vector3 start, Vector3 end, float muzzleVelocity, float gravity, out float angle)
	{
		Vector3 vector = end - start;
		float num = muzzleVelocity * muzzleVelocity;
		float y = vector.y;
		vector.y = 0f;
		float sqrMagnitude = vector.sqrMagnitude;
		float num2 = num * num - gravity * (gravity * sqrMagnitude + 2f * y * num);
		if (num2 < 0f)
		{
			angle = -45f;
			return false;
		}
		float num3 = Mathf.Sqrt(num2);
		float num4 = gravity * Mathf.Sqrt(sqrMagnitude);
		angle = -(Mathf.Atan2(num - num3, num4) * 57.29578f);
		return true;
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x0001901C File Offset: 0x0001721C
	public static bool CalculateHighTrajectory(Vector3 start, Vector3 end, float muzzleVelocity, float gravity, out float angle)
	{
		Vector3 vector = end - start;
		float num = muzzleVelocity * muzzleVelocity;
		float y = vector.y;
		vector.y = 0f;
		float sqrMagnitude = vector.sqrMagnitude;
		float num2 = num * num - gravity * (gravity * sqrMagnitude + 2f * y * num);
		if (num2 < 0f)
		{
			angle = -45f;
			return false;
		}
		float num3 = Mathf.Sqrt(num2);
		float num4 = gravity * Mathf.Sqrt(sqrMagnitude);
		angle = -(Mathf.Atan2(num + num3, num4) * 57.29578f);
		return true;
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x000190A4 File Offset: 0x000172A4
	private void OnTriggerEnter(Collider other)
	{
		GetPlayerGameobject getPlayerGameobject;
		PlayerMovement playerMovement;
		GetShadowWizardController getShadowWizardController;
		if (!other.CompareTag("Ignorable") && !this.collided && (!other.transform.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject) || !(getPlayerGameobject.player == this.playerOwner)) && (this.ShotByAi || !other.TryGetComponent<PlayerMovement>(out playerMovement)) && (!this.ShotByAi || !other.TryGetComponent<GetShadowWizardController>(out getShadowWizardController)))
		{
			this.FrostSource.PlayOneShot(this.FrostHit);
			Debug.Log(other.gameObject);
			this.collided = true;
			Object.Instantiate<GameObject>(this.explosion, base.transform.position, Quaternion.identity);
			PlayerMovement playerMovement2;
			GetPlayerGameobject getPlayerGameobject2;
			GetShadowWizardController getShadowWizardController2;
			if (this.ShotByAi && other.TryGetComponent<PlayerMovement>(out playerMovement2))
			{
				playerMovement2.CallSummonIceBox(this.level, null);
			}
			else if (other.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject2))
			{
				getPlayerGameobject2.player.GetComponent<PlayerMovement>().CallSummonIceBox(this.level, this.playerOwner);
			}
			else if (!this.ShotByAi && other.TryGetComponent<GetShadowWizardController>(out getShadowWizardController2))
			{
				getShadowWizardController2.ShadowWizardAI.GetComponent<ShadowWizardAI>().CallSummonIceBox(this.level);
			}
			else if (other.CompareTag("hitable"))
			{
				other.GetComponent<IInteractableNetworkObj>().NetInteract();
			}
			else if (other.CompareTag("wormhole"))
			{
				other.GetComponent<WormholeTele>().DestroyWormhole();
			}
			base.StartCoroutine(this.DestroyuGameobject());
		}
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x00019211 File Offset: 0x00017411
	private IEnumerator DestroyuGameobject()
	{
		this.hdLightData.gameObject.SetActive(false);
		this.foge.gameObject.SetActive(false);
		this.bolt.enabled = false;
		yield return new WaitForSeconds(1f);
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x04000322 RID: 802
	private bool collided;

	// Token: 0x04000323 RID: 803
	public HDAdditionalLightData hdLightData;

	// Token: 0x04000324 RID: 804
	public LocalVolumetricFog foge;

	// Token: 0x04000325 RID: 805
	public MeshRenderer bolt;

	// Token: 0x04000326 RID: 806
	public float travelTime = 1f;

	// Token: 0x04000327 RID: 807
	public Rigidbody rb;

	// Token: 0x04000328 RID: 808
	public float muzzleVelocity = 80f;

	// Token: 0x04000329 RID: 809
	public float gravity = 9.8f;

	// Token: 0x0400032A RID: 810
	public GameObject explosion;

	// Token: 0x0400032B RID: 811
	private bool ShotByAi;

	// Token: 0x0400032C RID: 812
	public AudioSource FrostSource;

	// Token: 0x0400032D RID: 813
	public AudioClip FrostHit;

	// Token: 0x0400032E RID: 814
	public GameObject playerOwner;

	// Token: 0x0400032F RID: 815
	private int level;
}
