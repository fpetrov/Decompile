using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000035 RID: 53
public class ColoseumPots : MonoBehaviour, IInteractableNetworkObj
{
	// Token: 0x0600025A RID: 602 RVA: 0x0000A8BD File Offset: 0x00008ABD
	private void Start()
	{
		base.StartCoroutine(this.WaitforClientStarted());
	}

	// Token: 0x0600025B RID: 603 RVA: 0x0000A8CC File Offset: 0x00008ACC
	private IEnumerator WaitforClientStarted()
	{
		while (this.NetItemManager == null || !this.NetItemManager.inited)
		{
			yield return null;
		}
		this.objID = this.NetItemManager.AddToList(base.gameObject);
		yield break;
	}

	// Token: 0x0600025C RID: 604 RVA: 0x0000A8DB File Offset: 0x00008ADB
	public void NetInteract()
	{
		if (!this.interacted)
		{
			this.NetItemManager.InteractWithObj(this.objID);
		}
	}

	// Token: 0x0600025D RID: 605 RVA: 0x0000A8F8 File Offset: 0x00008AF8
	public void ActualInteraction()
	{
		if (!this.interacted)
		{
			this.interacted = true;
			this.PotAudio.pitch = Random.Range(0.75f, 1.35f);
			this.PotAudio.PlayOneShot(this.PotSound);
			this.PotMesh.enabled = false;
			this.PotColl.enabled = false;
			this.BrokenPot.SetActive(true);
		}
	}

	// Token: 0x04000127 RID: 295
	public ColoseumManager NetItemManager;

	// Token: 0x04000128 RID: 296
	private int objID;

	// Token: 0x04000129 RID: 297
	public GameObject BrokenPot;

	// Token: 0x0400012A RID: 298
	public MeshRenderer PotMesh;

	// Token: 0x0400012B RID: 299
	public Collider PotColl;

	// Token: 0x0400012C RID: 300
	public AudioSource PotAudio;

	// Token: 0x0400012D RID: 301
	public AudioClip PotSound;

	// Token: 0x0400012E RID: 302
	private bool interacted;
}
