using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000BF RID: 191
public class LightningLookAt : MonoBehaviour
{
	// Token: 0x0600076A RID: 1898 RVA: 0x0001CAD0 File Offset: 0x0001ACD0
	private void Start()
	{
		this.asourc.pitch = Random.Range(0.8f, 1.2f);
		this.asourc.volume = Random.Range(0.68f, 0.75f);
		this.asourc.PlayOneShot(this.acl);
		base.StartCoroutine(this.lrout());
	}

	// Token: 0x0600076B RID: 1899 RVA: 0x0001CB2F File Offset: 0x0001AD2F
	private IEnumerator lrout()
	{
		for (float timer = 0f; timer < 0.5f; timer += Time.deltaTime)
		{
			yield return null;
		}
		this.bolt.SetActive(false);
		yield return new WaitForSeconds(30f);
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x0600076C RID: 1900 RVA: 0x0001CB40 File Offset: 0x0001AD40
	private void Update()
	{
		Vector3 position = Camera.main.transform.position;
		position.y = base.transform.position.y;
		base.transform.LookAt(position);
	}

	// Token: 0x040003C5 RID: 965
	public AudioSource asourc;

	// Token: 0x040003C6 RID: 966
	public AudioClip acl;

	// Token: 0x040003C7 RID: 967
	public GameObject bolt;
}
