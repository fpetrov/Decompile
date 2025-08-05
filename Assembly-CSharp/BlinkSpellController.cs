using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000007 RID: 7
public class BlinkSpellController : MonoBehaviour, ISpell
{
	// Token: 0x06000031 RID: 49 RVA: 0x00002AF0 File Offset: 0x00000CF0
	public void PlayerSetup(GameObject ownerobj, Vector3 fwdvector, int Level)
	{
		RaycastHit raycastHit;
		Vector3 vector;
		RaycastHit raycastHit2;
		RaycastHit raycastHit3;
		if (Physics.Raycast(ownerobj.transform.position + ownerobj.transform.forward * 15f + Vector3.up * 10f, Vector3.down, out raycastHit, 50f, this.ground) && !raycastHit.transform.CompareTag("blockblink"))
		{
			vector = raycastHit.point;
		}
		else if (Physics.Raycast(ownerobj.transform.position + ownerobj.transform.forward * 5f + Vector3.up * 5f, Vector3.down, out raycastHit2, 50f, this.ground) && !raycastHit2.transform.CompareTag("blockblink"))
		{
			vector = raycastHit2.point;
		}
		else if (Physics.Raycast(new Vector3(ownerobj.transform.position.x, ownerobj.transform.position.y + 1.3f, ownerobj.transform.position.z), fwdvector, out raycastHit3, 20f, this.ground) && !raycastHit3.transform.CompareTag("blockblink"))
		{
			vector = raycastHit3.point;
		}
		else
		{
			vector = ownerobj.transform.position;
		}
		Object.Instantiate<GameObject>(this.Poof, ownerobj.transform.position, Quaternion.identity);
		if (ownerobj.GetComponent<PlayerMovement>().IsOwner)
		{
			base.StartCoroutine(this.Blink(vector, ownerobj));
		}
		Object.Instantiate<GameObject>(this.Poof, vector, Quaternion.identity);
	}

	// Token: 0x06000032 RID: 50 RVA: 0x00002CB0 File Offset: 0x00000EB0
	private IEnumerator Blink(Vector3 Pos, GameObject ownerobj)
	{
		ownerobj.transform.position = Pos;
		yield return null;
		ownerobj.transform.position = Pos;
		yield return null;
		ownerobj.transform.position = Pos;
		yield return null;
		ownerobj.transform.position = Pos;
		yield return null;
		ownerobj.transform.position = Pos;
		yield break;
	}

	// Token: 0x04000016 RID: 22
	public GameObject Poof;

	// Token: 0x04000017 RID: 23
	public LayerMask ground;

	// Token: 0x04000018 RID: 24
	private bool isindung;
}
