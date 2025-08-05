using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x020001D9 RID: 473
public class WispController : MonoBehaviour, ISpell
{
	// Token: 0x06001347 RID: 4935 RVA: 0x0005137C File Offset: 0x0004F57C
	public void PlayerSetup(GameObject ownerobj, Vector3 fwdVector, int level)
	{
		this.ownerob = ownerobj;
		this.wispsource.PlayOneShot(this.wispclips[1]);
		Collider[] array = Physics.OverlapSphere(Vector3.zero, 10000f, this.player);
		float num = 11000f;
		int playerTeam = ownerobj.GetComponent<PlayerMovement>().playerTeam;
		foreach (Collider collider in array)
		{
			GetPlayerGameobject getPlayerGameobject;
			PlayerMovement playerMovement;
			if (collider.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
			{
				if (getPlayerGameobject.player.GetComponent<PlayerMovement>().playerTeam != playerTeam && Vector3.Distance(base.transform.position, collider.transform.position) < num)
				{
					this.target = collider.transform;
				}
			}
			else if (collider.TryGetComponent<PlayerMovement>(out playerMovement) && playerMovement.playerTeam != playerTeam && Vector3.Distance(base.transform.position, collider.transform.position) < num)
			{
				this.target = collider.transform;
			}
		}
		this.inited = true;
	}

	// Token: 0x06001348 RID: 4936 RVA: 0x00051480 File Offset: 0x0004F680
	private void Update()
	{
		if (this.inited && this.target != null)
		{
			if (this.wispagent.isActiveAndEnabled && this.wispagent.isOnNavMesh)
			{
				NavMeshPath navMeshPath = new NavMeshPath();
				if (!this.wispagent.CalculatePath(this.target.position, navMeshPath))
				{
					Vector3 normalized = (this.target.position + Vector3.up - base.transform.position).normalized;
					base.transform.position = Vector3.Lerp(base.transform.position, base.transform.position + normalized, Time.deltaTime * 10f);
				}
				else
				{
					this.wispagent.SetPath(navMeshPath);
				}
			}
			else
			{
				Vector3 normalized2 = (this.target.position + Vector3.up - base.transform.position).normalized;
				base.transform.position = Vector3.Lerp(base.transform.position, base.transform.position + normalized2, Time.deltaTime * 10f);
			}
			Vector3 vector = new Vector3(Mathf.Round(base.transform.position.z * 10f) / 10f, Mathf.Round(base.transform.position.y * 10f) / 10f, Mathf.Round(base.transform.position.z * 10f) / 10f);
			if (vector == this.prevpos)
			{
				this.standstilltimeoutimer += Time.deltaTime;
				if (this.standstilltimeoutimer > 0.5f)
				{
					this.wispagent.enabled = false;
				}
			}
			this.prevpos = vector;
			if (Vector3.Distance(base.transform.position, this.target.position) < 1f)
			{
				this.wispsource.Stop();
				this.wispsource.volume = 0.6f;
				this.wispsource.PlayOneShot(this.wispclips[0]);
				this.particles.Stop();
				base.StartCoroutine(this.lerplight());
				GetPlayerGameobject getPlayerGameobject;
				PlayerMovement playerMovement;
				if (this.target.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
				{
					getPlayerGameobject.player.GetComponent<PlayerMovement>().SummonIceBox(0, this.ownerob);
				}
				else if (this.target.TryGetComponent<PlayerMovement>(out playerMovement))
				{
					playerMovement.SummonIceBox(0, this.ownerob);
				}
				this.inited = false;
				return;
			}
		}
		else if (this.inited && this.target == null)
		{
			this.inited = false;
			base.StartCoroutine(this.lerplight());
			this.particles.Stop();
			this.wispsource.Stop();
			this.wispsource.volume = 0.6f;
		}
	}

	// Token: 0x06001349 RID: 4937 RVA: 0x00051778 File Offset: 0x0004F978
	private IEnumerator lerplight()
	{
		float timer = 0f;
		while (timer < 0.5f)
		{
			this.hdld.range = Mathf.Lerp(10f, 25f, timer * 2f);
			this.hdld.intensity = Mathf.Lerp(40000f, 700000f, timer * 2f);
			timer += Time.deltaTime;
			yield return null;
		}
		this.hdld.intensity = 0f;
		this.volumetrics.SetActive(false);
		yield return new WaitForSeconds(5f);
		timer = 0f;
		while (timer < 0.5f)
		{
			this.wispsource.volume = Mathf.Lerp(0.6f, 0f, timer * 2f);
			timer += Time.deltaTime;
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x04000B3D RID: 2877
	public NavMeshAgent wispagent;

	// Token: 0x04000B3E RID: 2878
	private Transform target;

	// Token: 0x04000B3F RID: 2879
	public LayerMask player;

	// Token: 0x04000B40 RID: 2880
	private bool inited;

	// Token: 0x04000B41 RID: 2881
	private float standstilltimeoutimer;

	// Token: 0x04000B42 RID: 2882
	private Vector3 prevpos = Vector3.zero;

	// Token: 0x04000B43 RID: 2883
	public AudioSource wispsource;

	// Token: 0x04000B44 RID: 2884
	public AudioClip[] wispclips;

	// Token: 0x04000B45 RID: 2885
	public HDAdditionalLightData hdld;

	// Token: 0x04000B46 RID: 2886
	public ParticleSystem particles;

	// Token: 0x04000B47 RID: 2887
	private GameObject ownerob;

	// Token: 0x04000B48 RID: 2888
	public GameObject volumetrics;
}
