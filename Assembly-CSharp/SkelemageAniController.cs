using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000193 RID: 403
public class SkelemageAniController : MonoBehaviour
{
	// Token: 0x060010F5 RID: 4341 RVA: 0x0004947C File Offset: 0x0004767C
	public void CastAnis()
	{
		base.StartCoroutine(this.CastAnisr());
	}

	// Token: 0x060010F6 RID: 4342 RVA: 0x0004948B File Offset: 0x0004768B
	private IEnumerator CastAnisr()
	{
		this.MageAni.SetBool("cast", true);
		this.MageAni.speed = 1f;
		yield return new WaitForSeconds(1.65f);
		this.MageAni.SetBool("cast", false);
		yield break;
	}

	// Token: 0x060010F7 RID: 4343 RVA: 0x0004949C File Offset: 0x0004769C
	private void Update()
	{
		if (!this.isdead)
		{
			this.MageAni.SetBool("idle", false);
			Vector3 normalized = (Camera.main.transform.position - base.transform.position).normalized;
			if (!Physics.Raycast(base.transform.position, normalized, Vector3.Distance(Camera.main.transform.position, base.transform.position), this.GroundLayer))
			{
				this.sourcethingy.outputAudioMixerGroup = this.amg2;
			}
			else
			{
				this.sourcethingy.outputAudioMixerGroup = this.amg;
			}
			this.magnitude = Mathf.Lerp(this.magnitude, ((base.transform.position - this.prevFramPos) / Time.deltaTime).magnitude, Time.deltaTime * 2f);
			this.prevFramPos = base.transform.position;
			if (this.magnitude > 3f)
			{
				if (Time.time - this.laststemptime > this.runsteptimer)
				{
					this.sourcethingy.PlayOneShot(this.footstepclips[Random.Range(0, this.footstepclips.Length)]);
					this.laststemptime = Time.time;
					this.sourcethingy.volume = Random.Range(0.1f, 0.15f);
					this.sourcethingy.pitch = Random.Range(0.8f, 1f);
					foreach (Collider collider in Physics.OverlapSphere(base.transform.position, 20f, this.PlayerLayer))
					{
						PlayerMovement playerMovement;
						if (collider.CompareTag("Player") && collider.gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
						{
							playerMovement.ShakeCam((20f - Vector3.Distance(base.transform.position, collider.transform.position)) / 30f, (20f - Vector3.Distance(base.transform.position, collider.transform.position)) / 30f);
						}
					}
				}
				if (Time.time - this.lastdragtime > this.runsteptimer * 2f)
				{
					this.sourcethingy.PlayOneShot(this.capeclips[Random.Range(0, this.capeclips.Length)]);
					this.lastdragtime = Time.time;
				}
				this.MageAni.speed = 2f;
			}
			else if (this.magnitude > 0.1f)
			{
				if (Time.time - this.laststemptime > this.walksteptimer)
				{
					this.sourcethingy.PlayOneShot(this.footstepclips[Random.Range(0, this.footstepclips.Length)]);
					this.laststemptime = Time.time;
					this.sourcethingy.volume = Random.Range(0.05f, 0.08f);
					this.sourcethingy.pitch = Random.Range(0.7f, 1f);
				}
				if (Time.time - this.lastdragtime > this.runsteptimer * 2f)
				{
					this.sourcethingy.PlayOneShot(this.capeclips[Random.Range(0, this.capeclips.Length)]);
					this.lastdragtime = Time.time;
				}
				this.MageAni.speed = 1f;
			}
			else if (!this.MageAni.GetBool("cast"))
			{
				this.MageAni.SetBool("idle", true);
			}
			if (this.castingFireball)
			{
				Vector3 vector = this.target.position - base.transform.position;
				vector.y = 0f;
				if (vector != Vector3.zero)
				{
					base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(vector), Time.deltaTime);
				}
				this.MageAni.speed = 1f;
			}
		}
	}

	// Token: 0x040009CE RID: 2510
	public Animator MageAni;

	// Token: 0x040009CF RID: 2511
	public Transform target;

	// Token: 0x040009D0 RID: 2512
	public bool castingFireball;

	// Token: 0x040009D1 RID: 2513
	public AudioSource sourcethingy;

	// Token: 0x040009D2 RID: 2514
	public AudioMixerGroup amg;

	// Token: 0x040009D3 RID: 2515
	public AudioMixerGroup amg2;

	// Token: 0x040009D4 RID: 2516
	public AudioClip[] footstepclips;

	// Token: 0x040009D5 RID: 2517
	public AudioClip[] capeclips;

	// Token: 0x040009D6 RID: 2518
	private float laststemptime;

	// Token: 0x040009D7 RID: 2519
	private float lastdragtime;

	// Token: 0x040009D8 RID: 2520
	public float walksteptimer = 0.6f;

	// Token: 0x040009D9 RID: 2521
	public float runsteptimer = 0.4f;

	// Token: 0x040009DA RID: 2522
	public LayerMask GroundLayer;

	// Token: 0x040009DB RID: 2523
	private float magnitude;

	// Token: 0x040009DC RID: 2524
	private Vector3 prevFramPos;

	// Token: 0x040009DD RID: 2525
	public LayerMask PlayerLayer;

	// Token: 0x040009DE RID: 2526
	public bool isdead;
}
