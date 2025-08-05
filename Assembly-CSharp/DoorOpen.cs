using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001ED RID: 493
public class DoorOpen : MonoBehaviour, IInteractableNetworkObj, ITimedInteractable
{
	// Token: 0x060013D5 RID: 5077 RVA: 0x00052E00 File Offset: 0x00051000
	private void Start()
	{
		this.NetItemManager = GameObject.FindGameObjectWithTag("NetItemManager").GetComponent<NetInteractionManager>();
		this.objID = this.NetItemManager.AddToList(base.gameObject);
	}

	// Token: 0x060013D6 RID: 5078 RVA: 0x00052E2E File Offset: 0x0005102E
	public void NetInteract()
	{
		this.NetItemManager.NetObjectInteraction(this.objID);
	}

	// Token: 0x060013D7 RID: 5079 RVA: 0x00052E41 File Offset: 0x00051041
	public void Interact(GameObject player)
	{
		this.NetInteract();
		this.NetItemManager.PlayerOpenDoor(this.objID);
	}

	// Token: 0x060013D8 RID: 5080 RVA: 0x00052E5A File Offset: 0x0005105A
	public float GetInteractTimer(GameObject player)
	{
		return 0.5f;
	}

	// Token: 0x060013D9 RID: 5081 RVA: 0x00052E61 File Offset: 0x00051061
	public string DisplayInteractUI()
	{
		if (!this.opened)
		{
			return "Opene Dore";
		}
		return "Shut Dore";
	}

	// Token: 0x060013DA RID: 5082 RVA: 0x00052E76 File Offset: 0x00051076
	public void initDoor()
	{
		this.TriggerColliderd.enabled = true;
		this.ActualInteraction();
	}

	// Token: 0x060013DB RID: 5083 RVA: 0x00052E8C File Offset: 0x0005108C
	public void ActualInteraction()
	{
		if (this.isRightDoor)
		{
			base.StopCoroutine(this.OpenRightDoor());
			if (!this.opened)
			{
				this._targetRot = Quaternion.Euler(0f, 20f, 0f);
				this.opened = true;
				this.da.pitch = Random.Range(0.75f, 0.95f);
				this.da.PlayOneShot(this.ac);
			}
			else
			{
				this.opened = false;
				this._targetRot = Quaternion.Euler(0f, -90f, 0f);
				this.da.pitch = Random.Range(0.75f, 0.95f);
				this.da.PlayOneShot(this.acc);
			}
			base.StartCoroutine(this.OpenRightDoor());
			return;
		}
		base.StopCoroutine(this.OpenLeftDoor());
		if (!this.opened)
		{
			this._targetRot = Quaternion.Euler(0f, -20f, 0f);
			this.opened = true;
			this.da.pitch = Random.Range(0.75f, 0.95f);
			this.da.PlayOneShot(this.ac);
		}
		else
		{
			this.opened = false;
			this._targetRot = Quaternion.Euler(0f, 90f, 0f);
			this.da.pitch = Random.Range(0.75f, 0.95f);
			this.da.PlayOneShot(this.acc);
		}
		base.StartCoroutine(this.OpenLeftDoor());
	}

	// Token: 0x060013DC RID: 5084 RVA: 0x0005301B File Offset: 0x0005121B
	private IEnumerator OpenRightDoor()
	{
		while (base.transform.localRotation != this._targetRot)
		{
			base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, this._targetRot, this.doorSpeed * Time.deltaTime);
			yield return null;
		}
		yield break;
	}

	// Token: 0x060013DD RID: 5085 RVA: 0x0005302A File Offset: 0x0005122A
	private IEnumerator OpenLeftDoor()
	{
		while (base.transform.localRotation != this._targetRot)
		{
			base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, this._targetRot, this.doorSpeed * Time.deltaTime);
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000BA3 RID: 2979
	private NetInteractionManager NetItemManager;

	// Token: 0x04000BA4 RID: 2980
	private int objID;

	// Token: 0x04000BA5 RID: 2981
	public bool isRightDoor;

	// Token: 0x04000BA6 RID: 2982
	public bool opened = true;

	// Token: 0x04000BA7 RID: 2983
	public float doorSpeed = 5f;

	// Token: 0x04000BA8 RID: 2984
	private Quaternion _targetRot;

	// Token: 0x04000BA9 RID: 2985
	public Collider TriggerColliderd;

	// Token: 0x04000BAA RID: 2986
	public float InteractTimer = 0.5f;

	// Token: 0x04000BAB RID: 2987
	public AudioSource da;

	// Token: 0x04000BAC RID: 2988
	public AudioClip ac;

	// Token: 0x04000BAD RID: 2989
	public AudioClip acc;

	// Token: 0x04000BAE RID: 2990
	public bool OpenedByPlayer;
}
