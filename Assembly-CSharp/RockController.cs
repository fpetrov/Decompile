using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000160 RID: 352
public class RockController : MonoBehaviour, ISpell
{
	// Token: 0x06000EFB RID: 3835 RVA: 0x0003C628 File Offset: 0x0003A828
	public void PlayerSetup(GameObject ownerobj, Vector3 fwdvector, int Level)
	{
		this.level = Level;
		this.playerOwner = ownerobj;
		base.StartCoroutine(this.Cleanup());
		RaycastHit raycastHit;
		RaycastHit raycastHit3;
		if (Physics.Raycast(base.transform.position, fwdvector, out raycastHit, 100f, this.ground))
		{
			RaycastHit raycastHit2;
			if (Physics.Raycast(new Vector3(raycastHit.point.x, raycastHit.point.y + 40f, raycastHit.point.z), Vector3.down, out raycastHit2, 200f, this.ground))
			{
				Object.Instantiate<GameObject>(this.RockPrefab, raycastHit2.point, Quaternion.identity).GetComponent<RockSpellController>().StartRockRoutine(this.playerOwner, this.level);
				return;
			}
			if (Physics.Raycast(base.transform.position + ownerobj.transform.forward * 50f + Vector3.up * 40f, Vector3.down, out raycastHit2, 200f, this.ground))
			{
				Object.Instantiate<GameObject>(this.RockPrefab, raycastHit2.point, Quaternion.identity).GetComponent<RockSpellController>().StartRockRoutine(this.playerOwner, this.level);
				return;
			}
		}
		else if (Physics.Raycast(base.transform.position + ownerobj.transform.forward * 50f + Vector3.up * 40f, Vector3.down, out raycastHit3, 200f, this.ground))
		{
			Object.Instantiate<GameObject>(this.RockPrefab, raycastHit3.point, Quaternion.identity).GetComponent<RockSpellController>().StartRockRoutine(this.playerOwner, this.level);
		}
	}

	// Token: 0x06000EFC RID: 3836 RVA: 0x0003C7FF File Offset: 0x0003A9FF
	private IEnumerator Cleanup()
	{
		yield return new WaitForSeconds(10f);
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x0400083D RID: 2109
	private int level;

	// Token: 0x0400083E RID: 2110
	private GameObject playerOwner;

	// Token: 0x0400083F RID: 2111
	public GameObject RockPrefab;

	// Token: 0x04000840 RID: 2112
	public LayerMask ground;
}
