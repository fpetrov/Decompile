using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200004C RID: 76
public class DespawnLackyRB : MonoBehaviour
{
	// Token: 0x06000355 RID: 853 RVA: 0x0000E157 File Offset: 0x0000C357
	private void Start()
	{
		base.GetComponent<AudioSource>().PlayOneShot(this.deths[Random.Range(0, 2)]);
		base.StartCoroutine(this.poofbye());
	}

	// Token: 0x06000356 RID: 854 RVA: 0x0000E17F File Offset: 0x0000C37F
	private IEnumerator poofbye()
	{
		yield return new WaitForSeconds(10f);
		Object.Instantiate<GameObject>(this.poof, base.transform.position, Quaternion.identity);
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x040001AA RID: 426
	public GameObject poof;

	// Token: 0x040001AB RID: 427
	public AudioClip[] deths;
}
