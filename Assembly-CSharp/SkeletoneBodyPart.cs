using System;
using UnityEngine;

// Token: 0x02000211 RID: 529
public class SkeletoneBodyPart : MonoBehaviour, IInteractableNetworkObj
{
	// Token: 0x06001542 RID: 5442 RVA: 0x00059900 File Offset: 0x00057B00
	public void NetInteract()
	{
		this.SkeleCon.SkeletoneNetInteract(this.partID);
	}

	// Token: 0x06001543 RID: 5443 RVA: 0x00059914 File Offset: 0x00057B14
	public void ActualInteraction()
	{
		this.ReplaceBone.transform.SetParent(this.BoneHolder);
		this.ReplaceBone.SetActive(true);
		this.Skelerb.isKinematic = false;
		this.Skelerb.useGravity = true;
		base.gameObject.SetActive(false);
	}

	// Token: 0x04000C71 RID: 3185
	public SkeletoneController SkeleCon;

	// Token: 0x04000C72 RID: 3186
	public int partID;

	// Token: 0x04000C73 RID: 3187
	public Rigidbody Skelerb;

	// Token: 0x04000C74 RID: 3188
	public GameObject ReplaceBone;

	// Token: 0x04000C75 RID: 3189
	public Transform BoneHolder;
}
