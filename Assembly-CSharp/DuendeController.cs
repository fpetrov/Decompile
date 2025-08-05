using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200005B RID: 91
public class DuendeController : MonoBehaviour
{
	// Token: 0x060003A5 RID: 933 RVA: 0x0000F020 File Offset: 0x0000D220
	private void Update()
	{
		if (this.DuendeAgent.velocity.magnitude > 0.1f)
		{
			this.Duendeani.SetBool("walk", true);
		}
		else
		{
			this.Duendeani.SetBool("walk", false);
		}
		if (this.panic)
		{
			if (!this.playedPanicSound)
			{
				this.asource.pitch = Random.Range(1.15f, 1.2f);
				this.asource.volume = Random.Range(0.8f, 1f);
				this.asource.PlayOneShot(this.dclips[Random.Range(6, 8)]);
				this.playedPanicSound = true;
			}
			this.DuendeAgent.speed = 5f;
			this.Duendeani.SetBool("run", true);
			return;
		}
		if (this.playedPanicSound)
		{
			this.playedPanicSound = false;
		}
		this.Duendeani.SetBool("run", false);
		this.DuendeAgent.speed = 1f;
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x0000F121 File Offset: 0x0000D321
	public void playHelloSound()
	{
		this.dman.PlayHello(this.DuendeID);
	}

	// Token: 0x060003A7 RID: 935 RVA: 0x0000F134 File Offset: 0x0000D334
	public void ActualPlayHelloSound()
	{
		if (!this.playedGreetingsSound)
		{
			this.asource.pitch = Random.Range(1.15f, 1.2f);
			this.asource.volume = Random.Range(0.7f, 0.8f);
			this.asource.PlayOneShot(this.dclips[Random.Range(0, 2)]);
			this.playedGreetingsSound = true;
		}
	}

	// Token: 0x060003A8 RID: 936 RVA: 0x0000F1A0 File Offset: 0x0000D3A0
	private void FixedUpdate()
	{
		this.Duendeani.SetBool("turn", false);
		if (this.targets.Count > 0 && !this.panic && this.targets[0] != null)
		{
			this.ClosestTarget = this.targets[0].transform;
			float num = Vector3.Distance(this.targets[0].transform.position, base.transform.position);
			for (int i = 1; i < this.targets.Count; i++)
			{
				float num2 = Vector3.Distance(this.targets[i].transform.position, base.transform.position);
				if (num2 < num)
				{
					this.ClosestTarget = this.targets[i].transform;
					num = num2;
				}
			}
			this.DuendeAgent.SetDestination(base.transform.position);
			Vector3 vector = this.ClosestTarget.position - base.transform.position;
			vector.y = 0f;
			if (vector != Vector3.zero)
			{
				Quaternion quaternion = Quaternion.LookRotation(vector);
				if (Mathf.Abs(Mathf.DeltaAngle(quaternion.eulerAngles.y, base.transform.eulerAngles.y)) > 1f)
				{
					this.Duendeani.SetBool("turn", true);
				}
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion, Time.deltaTime * 5f);
				return;
			}
		}
		else if (this.playedGreetingsSound && !this.panic)
		{
			this.asource.pitch = Random.Range(1.15f, 1.2f);
			this.asource.volume = Random.Range(0.7f, 0.8f);
			this.asource.PlayOneShot(this.dclips[Random.Range(2, 4)]);
			this.playedGreetingsSound = false;
		}
	}

	// Token: 0x060003A9 RID: 937 RVA: 0x0000F3AC File Offset: 0x0000D5AC
	public void interactedWith(GameObject player)
	{
		PlayerInventory playerInventory;
		if (player.TryGetComponent<PlayerInventory>(out playerInventory))
		{
			playerInventory.tempdisableitemswap();
			playerInventory.GiveDuendeItem(this.dman.gameObject, this.DuendeID);
			this.dman.DuendeTrade(this.DuendeID);
		}
	}

	// Token: 0x060003AA RID: 938 RVA: 0x0000F3F4 File Offset: 0x0000D5F4
	public void TradeAnis()
	{
		this.asource.pitch = Random.Range(1.15f, 1.2f);
		this.asource.volume = Random.Range(0.7f, 0.8f);
		this.asource.PlayOneShot(this.dclips[Random.Range(4, 6)]);
		this.Duendeani.SetBool("hold", true);
	}

	// Token: 0x060003AB RID: 939 RVA: 0x0000F45F File Offset: 0x0000D65F
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			this.targets.Add(other);
		}
	}

	// Token: 0x060003AC RID: 940 RVA: 0x0000F47A File Offset: 0x0000D67A
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			this.targets.Remove(other);
		}
	}

	// Token: 0x060003AD RID: 941 RVA: 0x0000F496 File Offset: 0x0000D696
	public void WalktoPoint(Vector3 walkPoint)
	{
		if (this.DuendeAgent.isOnNavMesh)
		{
			this.DuendeAgent.SetDestination(walkPoint);
		}
	}

	// Token: 0x060003AE RID: 942 RVA: 0x0000F4B2 File Offset: 0x0000D6B2
	public bool CheckPlayersNear()
	{
		return this.targets.Count > 0;
	}

	// Token: 0x060003AF RID: 943 RVA: 0x0000F4C2 File Offset: 0x0000D6C2
	public void SetDuendeID(int id)
	{
		this.DuendeID = id;
		base.StartCoroutine(this.SetDuendePos());
	}

	// Token: 0x060003B0 RID: 944 RVA: 0x0000F4D8 File Offset: 0x0000D6D8
	private IEnumerator SetDuendePos()
	{
		this.DuendeAgent.enabled = false;
		base.transform.localPosition = this.Startpos;
		yield return null;
		this.DuendeAgent.enabled = true;
		yield break;
	}

	// Token: 0x060003B1 RID: 945 RVA: 0x0000F4E7 File Offset: 0x0000D6E7
	public void KillDuende()
	{
		this.RagdollReference = Object.Instantiate<GameObject>(this.DuendeRagDoll, base.transform.position, base.transform.rotation);
		base.gameObject.SetActive(false);
	}

	// Token: 0x060003B2 RID: 946 RVA: 0x0000F51C File Offset: 0x0000D71C
	public void RespawnDuende()
	{
		base.gameObject.SetActive(true);
		Object.Destroy(this.RagdollReference);
	}

	// Token: 0x040001D7 RID: 471
	public Transform MyHouse;

	// Token: 0x040001D8 RID: 472
	public int DuendePoint;

	// Token: 0x040001D9 RID: 473
	public int DuendeID;

	// Token: 0x040001DA RID: 474
	private List<Collider> targets = new List<Collider>();

	// Token: 0x040001DB RID: 475
	private Transform ClosestTarget;

	// Token: 0x040001DC RID: 476
	public Animator Duendeani;

	// Token: 0x040001DD RID: 477
	public NavMeshAgent DuendeAgent;

	// Token: 0x040001DE RID: 478
	public bool isTrading;

	// Token: 0x040001DF RID: 479
	public Transform DuendeHandPoint;

	// Token: 0x040001E0 RID: 480
	public DuendeManager dman;

	// Token: 0x040001E1 RID: 481
	public GameObject DuendeRagDoll;

	// Token: 0x040001E2 RID: 482
	private GameObject RagdollReference;

	// Token: 0x040001E3 RID: 483
	public bool panic;

	// Token: 0x040001E4 RID: 484
	public bool playedGreetingsSound;

	// Token: 0x040001E5 RID: 485
	private bool playedPanicSound;

	// Token: 0x040001E6 RID: 486
	public AudioSource asource;

	// Token: 0x040001E7 RID: 487
	public AudioClip[] dclips;

	// Token: 0x040001E8 RID: 488
	public Vector3 Startpos;
}
