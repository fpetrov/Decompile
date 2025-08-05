using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020000B4 RID: 180
public class LackyController : MonoBehaviour, IInteractableNetworkObj
{
	// Token: 0x06000711 RID: 1809 RVA: 0x0001B20C File Offset: 0x0001940C
	public void SetupLacky(Vector3 TargetPos, int loottype, ChestNetController cncr)
	{
		Object.Instantiate<GameObject>(this.poof, base.transform.position, Quaternion.identity);
		this.loottypee = loottype;
		this.targetposition = TargetPos;
		this.cnc = cncr;
		this.items[loottype].SetActive(true);
		this.lackyAgent.SetDestination(TargetPos);
	}

	// Token: 0x06000712 RID: 1810 RVA: 0x0001B268 File Offset: 0x00019468
	private void Update()
	{
		if (Vector3.Distance(base.transform.position, this.targetposition) < 3f && !this.hasdroppeditem)
		{
			this.hasdroppeditem = true;
			this.lackyani.SetBool("drop", true);
			this.lackyAgent.isStopped = true;
			this.cnc.LackyDropItem(base.transform.position + base.transform.forward, this.loottypee);
			this.items[this.loottypee].SetActive(false);
			base.StartCoroutine(this.Despawnguy());
		}
	}

	// Token: 0x06000713 RID: 1811 RVA: 0x0001B30A File Offset: 0x0001950A
	public void NetInteract()
	{
		this.cnc.KillLacky(this.lackyid);
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x0001B320 File Offset: 0x00019520
	public void ActualInteraction()
	{
		if (!this.interacted)
		{
			if (!this.hasdroppeditem)
			{
				this.cnc.LackyDropItem(base.transform.position + base.transform.forward, this.loottypee);
				this.items[this.loottypee].SetActive(false);
			}
			this.interacted = true;
			Object.Instantiate<GameObject>(this.LackyRB, base.transform.position, base.transform.rotation);
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x0001B3B0 File Offset: 0x000195B0
	private IEnumerator Despawnguy()
	{
		yield return new WaitForSeconds(1f);
		Object.Instantiate<GameObject>(this.poof, base.transform.position, Quaternion.identity);
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x04000388 RID: 904
	public NavMeshAgent lackyAgent;

	// Token: 0x04000389 RID: 905
	public GameObject[] items;

	// Token: 0x0400038A RID: 906
	public GameObject LackyRB;

	// Token: 0x0400038B RID: 907
	private ChestNetController cnc;

	// Token: 0x0400038C RID: 908
	public Animator lackyani;

	// Token: 0x0400038D RID: 909
	private Vector3 targetposition;

	// Token: 0x0400038E RID: 910
	private bool hasdroppeditem;

	// Token: 0x0400038F RID: 911
	private bool interacted;

	// Token: 0x04000390 RID: 912
	public int lackyid;

	// Token: 0x04000391 RID: 913
	private int loottypee;

	// Token: 0x04000392 RID: 914
	public GameObject poof;
}
