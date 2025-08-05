using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x02000208 RID: 520
public class FireballController : MonoBehaviour
{
	// Token: 0x060014CA RID: 5322 RVA: 0x00057485 File Offset: 0x00055685
	public void addtolist(int val)
	{
		this.tpeders.Add(val);
	}

	// Token: 0x060014CB RID: 5323 RVA: 0x00057493 File Offset: 0x00055693
	private void FixedUpdate()
	{
		this.rb.MoveRotation(Quaternion.LookRotation(this.rb.velocity));
	}

	// Token: 0x060014CC RID: 5324 RVA: 0x000574B0 File Offset: 0x000556B0
	public void PlayerSetup(GameObject ownerobj, Vector3 fwdvector, int Level)
	{
		this.level = Level;
		this.playerOwner = ownerobj;
		this.rb.AddForce(fwdvector * this.muzzleVelocity, ForceMode.VelocityChange);
		base.StartCoroutine(this.Shoott());
	}

	// Token: 0x060014CD RID: 5325 RVA: 0x000574E8 File Offset: 0x000556E8
	public void Setup(Vector3 target, bool maxHeight)
	{
		this.shotByAi = true;
		Vector3 position = base.transform.position;
		float num;
		if (maxHeight)
		{
			if (FireballController.CalculateHighTrajectory(position, target, this.muzzleVelocity, this.gravity, out num))
			{
				base.transform.LookAt(target);
				base.transform.rotation = Quaternion.Euler(num, base.transform.eulerAngles.y, base.transform.eulerAngles.z);
				if (this.rb != null)
				{
					this.rb.AddForce(base.transform.forward * this.muzzleVelocity, ForceMode.VelocityChange);
				}
			}
		}
		else if (FireballController.CalculateLowTrajectory(position, target, this.muzzleVelocity / 1.5f, this.gravity / 1.75f, out num))
		{
			this.gravity /= 1.75f;
			base.transform.LookAt(target);
			base.transform.rotation = Quaternion.Euler(num, base.transform.eulerAngles.y, base.transform.eulerAngles.z);
			if (this.rb != null)
			{
				this.rb.AddForce(base.transform.forward * (this.muzzleVelocity / 1.5f), ForceMode.VelocityChange);
			}
		}
		base.StartCoroutine(this.Shoott());
	}

	// Token: 0x060014CE RID: 5326 RVA: 0x00057658 File Offset: 0x00055858
	public static bool CalculateLowTrajectory(Vector3 start, Vector3 end, float muzzleVelocity, float cgravity, out float angle)
	{
		Vector3 vector = end - start;
		float num = muzzleVelocity * muzzleVelocity;
		float y = vector.y;
		vector.y = 0f;
		float sqrMagnitude = vector.sqrMagnitude;
		float num2 = num * num - cgravity * (cgravity * sqrMagnitude + 2f * y * num);
		if (num2 < 0f)
		{
			angle = -45f;
			return false;
		}
		float num3 = Mathf.Sqrt(num2);
		float num4 = cgravity * Mathf.Sqrt(sqrMagnitude);
		angle = -(Mathf.Atan2(num - num3, num4) * 57.29578f);
		return true;
	}

	// Token: 0x060014CF RID: 5327 RVA: 0x000576E0 File Offset: 0x000558E0
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

	// Token: 0x060014D0 RID: 5328 RVA: 0x00057765 File Offset: 0x00055965
	private IEnumerator Shoott()
	{
		this.fbsource.volume = 0f;
		this.fbsource.PlayOneShot(this.fbsounds[0]);
		float timer = 0f;
		while (timer < 0.2f)
		{
			this.fbsource.volume = Mathf.Lerp(0f, 0.2f, timer * 5f);
			timer += Time.deltaTime;
			this.foge.parameters.meanFreePath = Mathf.Lerp(25f, 0.05f, timer / 0.2f);
			this.hdLightData.range = Mathf.Lerp(0.01f, 1.2f, timer / 0.2f);
			this.hdLightData.intensity = Mathf.Lerp(1000f, 40000f, timer / 0.2f);
			yield return null;
		}
		timer = 0f;
		while (timer < 1.5f)
		{
			this.fbsource.volume = Mathf.Lerp(0.2f, 0.6f, timer / 1.5f);
			timer += Time.deltaTime;
			yield return null;
		}
		yield break;
	}

	// Token: 0x060014D1 RID: 5329 RVA: 0x00057774 File Offset: 0x00055974
	private void OnCollisionEnter(Collision other)
	{
		GetPlayerGameobject getPlayerGameobject;
		if (!this.collided && (!this.shotByAi || !other.transform.CompareTag("PlayerNpc")) && (!other.transform.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject) || !(getPlayerGameobject.player == this.playerOwner)) && !(other.gameObject == this.playerOwner))
		{
			this.collided = true;
			this.fbsource.Stop();
			this.fbsource.volume = 1f;
			this.fbsource.PlayOneShot(this.fbsounds[1]);
			GameObject gameObject = Object.Instantiate<GameObject>(this.explosion, base.transform.position, Quaternion.identity);
			gameObject.GetComponent<ExplosionController>().level = this.level;
			gameObject.GetComponent<ExplosionController>().Explode(this.playerOwner);
			base.StartCoroutine(this.DestroyuGameobject());
		}
	}

	// Token: 0x060014D2 RID: 5330 RVA: 0x0005785F File Offset: 0x00055A5F
	private IEnumerator DestroyuGameobject()
	{
		this.hdLightData.gameObject.SetActive(false);
		this.foge.gameObject.SetActive(false);
		yield return new WaitForSeconds(0.5f);
		for (float timer = 0f; timer < 0.5f; timer += Time.deltaTime)
		{
			this.fbsource.volume = Mathf.Lerp(1f, 0f, timer * 2f);
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x04000C29 RID: 3113
	private bool collided;

	// Token: 0x04000C2A RID: 3114
	public HDAdditionalLightData hdLightData;

	// Token: 0x04000C2B RID: 3115
	public LocalVolumetricFog foge;

	// Token: 0x04000C2C RID: 3116
	public AudioSource fbsource;

	// Token: 0x04000C2D RID: 3117
	public AudioClip[] fbsounds;

	// Token: 0x04000C2E RID: 3118
	public float travelTime = 1f;

	// Token: 0x04000C2F RID: 3119
	public Rigidbody rb;

	// Token: 0x04000C30 RID: 3120
	public float muzzleVelocity = 10f;

	// Token: 0x04000C31 RID: 3121
	public float gravity = 9.8f;

	// Token: 0x04000C32 RID: 3122
	public GameObject explosion;

	// Token: 0x04000C33 RID: 3123
	private bool shotByAi;

	// Token: 0x04000C34 RID: 3124
	private float LightLerper;

	// Token: 0x04000C35 RID: 3125
	public GameObject playerOwner;

	// Token: 0x04000C36 RID: 3126
	private int level = 2;

	// Token: 0x04000C37 RID: 3127
	public bool hastped;

	// Token: 0x04000C38 RID: 3128
	public List<int> tpeders = new List<int>();
}
