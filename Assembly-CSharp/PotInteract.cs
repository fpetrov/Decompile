using System;
using UnityEngine;

// Token: 0x020001F3 RID: 499
public class PotInteract : MonoBehaviour, IInteractableNetworkObj
{
	// Token: 0x06001428 RID: 5160 RVA: 0x00053FD1 File Offset: 0x000521D1
	private void Start()
	{
		this.NetItemManager = GameObject.FindGameObjectWithTag("NetItemManager").GetComponent<NetInteractionManager>();
		this.objID = this.NetItemManager.AddToList(base.gameObject);
	}

	// Token: 0x06001429 RID: 5161 RVA: 0x00053FFF File Offset: 0x000521FF
	public void NetInteract()
	{
		if (!this.interacted)
		{
			this.NetItemManager.NetObjectInteraction(this.objID);
		}
	}

	// Token: 0x0600142A RID: 5162 RVA: 0x0005401C File Offset: 0x0005221C
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

	// Token: 0x0600142B RID: 5163 RVA: 0x00054087 File Offset: 0x00052287
	public bool didinteract()
	{
		return this.interacted;
	}

	// Token: 0x04000BBB RID: 3003
	private NetInteractionManager NetItemManager;

	// Token: 0x04000BBC RID: 3004
	private int objID;

	// Token: 0x04000BBD RID: 3005
	public GameObject BrokenPot;

	// Token: 0x04000BBE RID: 3006
	public MeshRenderer PotMesh;

	// Token: 0x04000BBF RID: 3007
	public Collider PotColl;

	// Token: 0x04000BC0 RID: 3008
	public AudioSource PotAudio;

	// Token: 0x04000BC1 RID: 3009
	public AudioClip PotSound;

	// Token: 0x04000BC2 RID: 3010
	private bool interacted;
}
