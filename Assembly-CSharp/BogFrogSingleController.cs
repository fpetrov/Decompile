using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200000D RID: 13
public class BogFrogSingleController : MonoBehaviour
{
	// Token: 0x06000085 RID: 133 RVA: 0x00003C90 File Offset: 0x00001E90
	public int getbfid()
	{
		return this.bogFrogid;
	}

	// Token: 0x06000086 RID: 134 RVA: 0x00003C98 File Offset: 0x00001E98
	private void Start()
	{
		this.frogtoungestartpos = this.frogtounge.localPosition;
	}

	// Token: 0x06000087 RID: 135 RVA: 0x00003CAB File Offset: 0x00001EAB
	private void Update()
	{
		this.eatcdtimer += Time.deltaTime;
	}

	// Token: 0x06000088 RID: 136 RVA: 0x00003CC0 File Offset: 0x00001EC0
	private void FixedUpdate()
	{
		if (this.imEatingtheGuy)
		{
			if (this.GuyToEat != null)
			{
				Vector3 vector = new Vector3(this.GuyToEat.position.x, base.transform.position.y, this.GuyToEat.position.z) - base.transform.position;
				vector.Normalize();
				Quaternion quaternion = Quaternion.LookRotation(vector);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion, Time.fixedDeltaTime * this.rotationSpeed * 4f);
				return;
			}
		}
		else if (this.targets.Count > 0 && this.targets[0] != null)
		{
			this.ClosestTarget = this.targets[0].transform;
			float num = Vector3.Distance(this.targets[0].transform.position, base.transform.position);
			for (int i = 1; i < this.targets.Count; i++)
			{
				if (this.targets[i] != null)
				{
					float num2 = Vector3.Distance(this.targets[i].transform.position, base.transform.position);
					if (num2 < num)
					{
						this.ClosestTarget = this.targets[i].transform;
						num = num2;
					}
				}
			}
			Vector3 vector2 = new Vector3(this.ClosestTarget.position.x, base.transform.position.y, this.ClosestTarget.position.z) - base.transform.position;
			vector2.Normalize();
			Quaternion quaternion2 = Quaternion.LookRotation(vector2);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion2, Time.fixedDeltaTime * this.rotationSpeed);
			if (num < 12f && !this.imEatingtheGuy)
			{
				for (int j = 0; j < this.ClosestTarget.childCount; j++)
				{
					if (this.ClosestTarget.GetChild(j).CompareTag("MainCamera"))
					{
						this.bfc.EatTheGuy(this.bogFrogid, this.ClosestTarget);
					}
				}
			}
		}
	}

	// Token: 0x06000089 RID: 137 RVA: 0x00003F22 File Offset: 0x00002122
	public bool CheckPlayersNear()
	{
		return this.targets.Count > 0;
	}

	// Token: 0x0600008A RID: 138 RVA: 0x00003F32 File Offset: 0x00002132
	public void NewLocationAnis()
	{
		base.StartCoroutine(this.NewLocationAnimations());
	}

	// Token: 0x0600008B RID: 139 RVA: 0x00003F41 File Offset: 0x00002141
	private IEnumerator NewLocationAnimations()
	{
		this.eatcdtimer = 2f;
		this.BogFrogAni.SetBool("riseup", false);
		this.BogFrogAni.SetBool("backdown", true);
		this.frogio.pitch = 1f;
		this.frogio.PlayOneShot(this.frogclips[0]);
		yield return new WaitForSeconds(2f);
		this.frogio.PlayOneShot(this.frogclips[1]);
		this.BogFrogAni.SetBool("backdown", false);
		this.BogFrogAni.SetBool("riseup", true);
		yield return new WaitForSeconds(2f);
		if (!this.BogFrogAni.GetBool("riseup"))
		{
			this.BogFrogAni.SetBool("riseup", true);
		}
		if (this.targets.Count == 0)
		{
			this.frogio.PlayOneShot(this.frogclips[2]);
		}
		yield break;
	}

	// Token: 0x0600008C RID: 140 RVA: 0x00003F50 File Offset: 0x00002150
	public void EatGuyAnis()
	{
		base.StartCoroutine(this.EatGuyAnimations());
	}

	// Token: 0x0600008D RID: 141 RVA: 0x00003F5F File Offset: 0x0000215F
	private IEnumerator EatGuyAnimations()
	{
		this.BogFrogAni.SetBool("yummy", true);
		this.BogFrogAni.SetBool("riseup", false);
		this.frogio.pitch = 0.8f;
		this.frogio.PlayOneShot(this.frogclips[3]);
		foreach (Collider collider in Physics.OverlapSphere(base.transform.position, 30f, this.PlayerLayer))
		{
			PlayerMovement playerMovement;
			if (collider.CompareTag("Player") && collider.gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
			{
				playerMovement.ShakeCam((30f - Vector3.Distance(base.transform.position, collider.transform.position)) / 30f, (30f - Vector3.Distance(base.transform.position, collider.transform.position)) / 30f);
			}
		}
		yield return new WaitForSeconds(0.5f);
		this.frogio.PlayOneShot(this.frogclips[4]);
		this.eatcdtimer = 0f;
		while (this.eatcdtimer < 1f)
		{
			this.frogtounge.position = Vector3.Lerp(this.frogtounge.position, this.GuyToEat.position + Vector3.up, this.eatcdtimer);
			yield return null;
		}
		this.frogtounge.position = this.GuyToEat.position + Vector3.up;
		PlayerMovement playerMovement2;
		if (this.GuyToEat.transform.TryGetComponent<PlayerMovement>(out playerMovement2))
		{
			playerMovement2.FrogToung = this.playerLerpTarg;
			playerMovement2.eatenByFrog = true;
		}
		this.eatcdtimer = 0f;
		this.frogio.PlayOneShot(this.frogclips[5]);
		while (this.eatcdtimer < 1f)
		{
			this.eatcdtimer += Time.deltaTime;
			this.frogtounge.localPosition = Vector3.Lerp(this.frogtounge.localPosition, this.frogtoungestartpos, this.eatcdtimer);
			yield return null;
		}
		Debug.Log("exiting");
		this.BogFrogAni.SetBool("yummy", false);
		this.frogio.PlayOneShot(this.frogclips[6]);
		yield return new WaitForSeconds(15f);
		this.GuyToEat = null;
		this.imEatingtheGuy = false;
		this.bfc.forcegonewspot(this.bogFrogid);
		yield break;
	}

	// Token: 0x0600008E RID: 142 RVA: 0x00003F6E File Offset: 0x0000216E
	public void hitbyspell()
	{
		if (!this.imEatingtheGuy)
		{
			this.bfc.hitbyspell(this.bogFrogid);
		}
	}

	// Token: 0x0600008F RID: 143 RVA: 0x00003F89 File Offset: 0x00002189
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			this.targets.Add(other);
		}
	}

	// Token: 0x06000090 RID: 144 RVA: 0x00003FA4 File Offset: 0x000021A4
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			this.targets.Remove(other);
		}
	}

	// Token: 0x06000091 RID: 145 RVA: 0x00003FC0 File Offset: 0x000021C0
	public void SetBogFrogID(int id)
	{
		this.bogFrogid = id;
	}

	// Token: 0x0400002B RID: 43
	public int FrogPoint;

	// Token: 0x0400002C RID: 44
	public Animator BogFrogAni;

	// Token: 0x0400002D RID: 45
	private List<Collider> targets = new List<Collider>();

	// Token: 0x0400002E RID: 46
	private Transform ClosestTarget;

	// Token: 0x0400002F RID: 47
	public float rotationSpeed;

	// Token: 0x04000030 RID: 48
	public BogFrogController bfc;

	// Token: 0x04000031 RID: 49
	public bool imEatingtheGuy;

	// Token: 0x04000032 RID: 50
	public Transform GuyToEat;

	// Token: 0x04000033 RID: 51
	private int bogFrogid;

	// Token: 0x04000034 RID: 52
	public AudioSource frogio;

	// Token: 0x04000035 RID: 53
	public AudioClip[] frogclips;

	// Token: 0x04000036 RID: 54
	public Transform frogtounge;

	// Token: 0x04000037 RID: 55
	private Vector3 frogtoungestartpos;

	// Token: 0x04000038 RID: 56
	public float eatcdtimer;

	// Token: 0x04000039 RID: 57
	public Transform playerLerpTarg;

	// Token: 0x0400003A RID: 58
	public LayerMask PlayerLayer;
}
