using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000212 RID: 530
public class SkeletoneController : MonoBehaviour, IInteractableNetworkObj
{
	// Token: 0x06001545 RID: 5445 RVA: 0x00059967 File Offset: 0x00057B67
	private void Start()
	{
		this.NetItemManager = GameObject.FindGameObjectWithTag("NetItemManager").GetComponent<NetInteractionManager>();
		this.objID = this.NetItemManager.AddToList(base.gameObject);
	}

	// Token: 0x06001546 RID: 5446 RVA: 0x00059998 File Offset: 0x00057B98
	private void OnTriggerEnter(Collider other)
	{
		PlayerMovement playerMovement;
		if (other.CompareTag("Player") && other.TryGetComponent<PlayerMovement>(out playerMovement) && !this.inited)
		{
			this.NetItemManager.Skeletoninit(this.objID, other);
		}
	}

	// Token: 0x06001547 RID: 5447 RVA: 0x000021EF File Offset: 0x000003EF
	public void NetInteract()
	{
	}

	// Token: 0x06001548 RID: 5448 RVA: 0x000021EF File Offset: 0x000003EF
	public void ActualInteraction()
	{
	}

	// Token: 0x06001549 RID: 5449 RVA: 0x000599D6 File Offset: 0x00057BD6
	public void SkeleInit(GameObject col)
	{
		if (!this.inited)
		{
			this.asource.PlayOneShot(this.aclips[Random.Range(9, 12)]);
			this.target = col;
			this.inited = true;
			base.StartCoroutine(this.SkeleBehavior());
		}
	}

	// Token: 0x0600154A RID: 5450 RVA: 0x00059A16 File Offset: 0x00057C16
	private IEnumerator SkeleBehavior()
	{
		this.SkeleAni.SetBool("awoken", true);
		yield return new WaitForSeconds(3.2f);
		float cd = Time.time;
		while (this.alive)
		{
			this.SkeleAni.SetBool("idle", true);
			if (this.skeleAgent.velocity.magnitude > 0.1f)
			{
				this.SkeleAni.SetBool("idle", false);
				if (Time.time - this.StepTimer > this.stepvalue)
				{
					this.StepTimer = Time.time;
					this.asource.pitch = Random.Range(0.8f, 1.2f);
					this.asource.volume = Random.Range(0.125f, 0.35f);
					this.asource.PlayOneShot(this.aclips[Random.Range(3, 7)]);
				}
			}
			if (this.skeleAgent.isActiveAndEnabled && this.skeleAgent.isOnNavMesh)
			{
				this.skeleAgent.SetDestination(this.target.transform.position);
			}
			else if (!this.skeleAgent.isOnNavMesh)
			{
				this.skeleAgent.enabled = false;
				yield return null;
				this.skeleAgent.enabled = true;
			}
			if (Time.time - cd > 3f)
			{
				cd = Time.time + 2f;
				this.NetItemManager.Skeletonretarget(this.objID);
			}
			yield return null;
			if (Time.time - cd > 3f)
			{
				if (Vector3.Distance(base.transform.position, this.target.transform.position) < 1.6f)
				{
					if (this.skeleAgent.isActiveAndEnabled)
					{
						this.skeleAgent.SetDestination(base.transform.position);
					}
					this.SkeleAni.SetBool("attack", true);
					this.asource.PlayOneShot(this.aclips[Random.Range(7, 9)]);
					PlayerMovement playerMovement;
					if (this.target.TryGetComponent<PlayerMovement>(out playerMovement))
					{
						playerMovement.DamagePlayer(15f, null, "skeleton");
					}
					yield return new WaitForSeconds(1f);
					this.SkeleAni.SetBool("attack", false);
					cd = Time.time;
				}
				else
				{
					this.SkeleAni.SetBool("attack", false);
				}
			}
		}
		yield break;
	}

	// Token: 0x0600154B RID: 5451 RVA: 0x00059A25 File Offset: 0x00057C25
	public void SkeletoneNetInteract(int BoneID)
	{
		this.NetItemManager.SkeletonInteraction(this.objID, BoneID);
	}

	// Token: 0x0600154C RID: 5452 RVA: 0x00059A3C File Offset: 0x00057C3C
	public void SkeleInteraction()
	{
		if (this.LastHitPart == 1 && !this.arm1)
		{
			if (this.arm2)
			{
				this.SkeleAni.SetBool("legonly", true);
			}
			this.arm1 = true;
			this.Bones[this.LastHitPart].ActualInteraction();
			base.StartCoroutine(this.BoneHitSounds());
		}
		else if (this.LastHitPart == 0 && !this.arm2)
		{
			this.SkeleAni.SetBool("nright", true);
			if (this.arm1)
			{
				this.SkeleAni.SetBool("legonly", true);
			}
			this.arm2 = true;
			this.Bones[this.LastHitPart].ActualInteraction();
			base.StartCoroutine(this.BoneHitSounds());
		}
		else if (this.LastHitPart == 2 && !this.head)
		{
			this.head = true;
			this.Bones[this.LastHitPart].ActualInteraction();
			base.StartCoroutine(this.BoneHitSounds());
		}
		else if (this.LastHitPart == 3 && !this.torso)
		{
			this.torso = true;
			this.Bones[this.LastHitPart].ActualInteraction();
			if (!this.arm1)
			{
				this.SkeletoneNetInteract(0);
			}
			if (!this.arm2)
			{
				this.SkeletoneNetInteract(1);
			}
			if (!this.head)
			{
				this.SkeletoneNetInteract(2);
			}
			base.StartCoroutine(this.BoneHitSounds());
			base.StartCoroutine(this.DelayTorso());
		}
		else if (this.LastHitPart == 5 && this.head && this.arm1 && this.arm2 && this.torso && !this.leg2 && this.delaytorso)
		{
			this.hip.SetActive(false);
			this.Bones[this.LastHitPart].ActualInteraction();
			base.StartCoroutine(this.BoneHitSounds());
			if (this.leg)
			{
				base.enabled = false;
			}
			this.leg2 = true;
		}
		else if (this.LastHitPart == 4 && this.head && this.arm1 && this.arm2 && this.torso && !this.leg && this.delaytorso)
		{
			this.leg = true;
			this.Bones[this.LastHitPart].ActualInteraction();
			base.StartCoroutine(this.BoneHitSounds());
			if (this.leg2)
			{
				base.enabled = false;
			}
		}
		if (this.arm1 && this.arm2 && this.head && this.torso && this.leg2 && this.leg)
		{
			this.alive = false;
		}
	}

	// Token: 0x0600154D RID: 5453 RVA: 0x00059CD6 File Offset: 0x00057ED6
	private IEnumerator DelayTorso()
	{
		yield return new WaitForSeconds(0.2f);
		this.delaytorso = true;
		yield break;
	}

	// Token: 0x0600154E RID: 5454 RVA: 0x00059CE5 File Offset: 0x00057EE5
	private IEnumerator BoneHitSounds()
	{
		this.asource.pitch = Random.Range(0.8f, 1.2f);
		this.asource.volume = Random.Range(0.125f, 0.35f);
		this.asource.PlayOneShot(this.aclips[0]);
		yield return new WaitForSeconds(0.4f);
		this.asource.pitch = Random.Range(0.8f, 1.2f);
		this.asource.volume = Random.Range(0.125f, 0.35f);
		this.asource.PlayOneShot(this.aclips[Random.Range(1, 3)]);
		yield break;
	}

	// Token: 0x0600154F RID: 5455 RVA: 0x00059CF4 File Offset: 0x00057EF4
	public void RepositionSkeleton(Vector3 skpos, GameObject targeto)
	{
		base.StartCoroutine(this.ReposSkelRoutine(skpos, targeto));
		this.target = targeto;
	}

	// Token: 0x06001550 RID: 5456 RVA: 0x00059D0C File Offset: 0x00057F0C
	private IEnumerator ReposSkelRoutine(Vector3 skpos, GameObject targeto)
	{
		this.skeleAgent.enabled = false;
		base.transform.position = skpos;
		yield return null;
		this.skeleAgent.enabled = true;
		this.skeleAgent.SetDestination(targeto.transform.position);
		yield break;
	}

	// Token: 0x04000C76 RID: 3190
	private NetInteractionManager NetItemManager;

	// Token: 0x04000C77 RID: 3191
	private int objID;

	// Token: 0x04000C78 RID: 3192
	public int LastHitPart;

	// Token: 0x04000C79 RID: 3193
	public SkeletoneBodyPart[] Bones;

	// Token: 0x04000C7A RID: 3194
	public Animator SkeleAni;

	// Token: 0x04000C7B RID: 3195
	public bool inited;

	// Token: 0x04000C7C RID: 3196
	public NavMeshAgent skeleAgent;

	// Token: 0x04000C7D RID: 3197
	private bool alive = true;

	// Token: 0x04000C7E RID: 3198
	private bool head;

	// Token: 0x04000C7F RID: 3199
	private bool arm1;

	// Token: 0x04000C80 RID: 3200
	private bool arm2;

	// Token: 0x04000C81 RID: 3201
	private bool torso;

	// Token: 0x04000C82 RID: 3202
	private bool leg;

	// Token: 0x04000C83 RID: 3203
	private bool leg2;

	// Token: 0x04000C84 RID: 3204
	public GameObject hip;

	// Token: 0x04000C85 RID: 3205
	public GameObject target;

	// Token: 0x04000C86 RID: 3206
	public AudioSource asource;

	// Token: 0x04000C87 RID: 3207
	public AudioClip[] aclips;

	// Token: 0x04000C88 RID: 3208
	private float StepTimer;

	// Token: 0x04000C89 RID: 3209
	public float stepvalue = 0.1f;

	// Token: 0x04000C8A RID: 3210
	private bool delaytorso;
}
