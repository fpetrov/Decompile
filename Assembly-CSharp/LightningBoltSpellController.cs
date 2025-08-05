using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000BD RID: 189
public class LightningBoltSpellController : MonoBehaviour, ISpell
{
	// Token: 0x06000761 RID: 1889 RVA: 0x0001C41F File Offset: 0x0001A61F
	public void PlayerSetup(GameObject ownerobj, Vector3 fwdvector, int Level)
	{
		this.level = Level;
		this.playerOwner = ownerobj;
		base.StartCoroutine(this.SummonLightning(fwdvector, ownerobj));
	}

	// Token: 0x06000762 RID: 1890 RVA: 0x0001C43E File Offset: 0x0001A63E
	private IEnumerator SummonLightning(Vector3 fwdvector, GameObject ownerobj)
	{
		Vector3 vector = new Vector3(ownerobj.transform.position.x, ownerobj.transform.position.y + 2f, ownerobj.transform.position.z);
		RaycastHit raycastHit;
		RaycastHit raycastHit3;
		if (Physics.Raycast(vector, fwdvector, out raycastHit, 100f, this.ground))
		{
			RaycastHit raycastHit2;
			if (Physics.Raycast(new Vector3(raycastHit.point.x, raycastHit.point.y + 40f, raycastHit.point.z), Vector3.down, out raycastHit2, 200f, this.ground))
			{
				Object.Instantiate<GameObject>(this.LightningBoltPrefab, raycastHit2.point, Quaternion.identity).GetComponent<LightningBoltDamage>().DoDmg(ownerobj);
			}
			else if (Physics.Raycast(vector + fwdvector * 30f + Vector3.up * 40f, Vector3.down, out raycastHit2, 200f, this.ground))
			{
				Object.Instantiate<GameObject>(this.LightningBoltPrefab, raycastHit2.point, Quaternion.identity).GetComponent<LightningBoltDamage>().DoDmg(ownerobj);
			}
		}
		else if (Physics.Raycast(vector + fwdvector * 30f + Vector3.up * 40f, Vector3.down, out raycastHit3, 200f, this.ground))
		{
			Object.Instantiate<GameObject>(this.LightningBoltPrefab, raycastHit3.point, Quaternion.identity).GetComponent<LightningBoltDamage>().DoDmg(ownerobj);
		}
		yield return new WaitForSeconds(0.8f);
		vector = new Vector3(ownerobj.transform.position.x, ownerobj.transform.position.y + 2f, ownerobj.transform.position.z);
		fwdvector = Quaternion.AngleAxis(-20f, ownerobj.transform.right) * ownerobj.transform.forward;
		if (this.level > 1)
		{
			RaycastHit raycastHit5;
			if (Physics.Raycast(vector, fwdvector, out raycastHit, 100f, this.ground))
			{
				RaycastHit raycastHit4;
				if (Physics.Raycast(new Vector3(raycastHit.point.x, raycastHit.point.y + 40f, raycastHit.point.z), Vector3.down, out raycastHit4, 200f, this.ground))
				{
					Object.Instantiate<GameObject>(this.LightningBoltPrefab, raycastHit4.point, Quaternion.identity).GetComponent<LightningBoltDamage>().DoDmg(ownerobj);
				}
				else if (Physics.Raycast(vector + fwdvector * 30f + Vector3.up * 40f, Vector3.down, out raycastHit4, 200f, this.ground))
				{
					Object.Instantiate<GameObject>(this.LightningBoltPrefab, raycastHit4.point, Quaternion.identity).GetComponent<LightningBoltDamage>().DoDmg(ownerobj);
				}
			}
			else if (Physics.Raycast(vector + fwdvector * 30f + Vector3.up * 40f, Vector3.down, out raycastHit5, 200f, this.ground))
			{
				Object.Instantiate<GameObject>(this.LightningBoltPrefab, raycastHit5.point, Quaternion.identity).GetComponent<LightningBoltDamage>().DoDmg(ownerobj);
			}
		}
		yield return new WaitForSeconds(0.4f);
		vector = new Vector3(ownerobj.transform.position.x, ownerobj.transform.position.y + 2f, ownerobj.transform.position.z);
		fwdvector = Quaternion.AngleAxis(-20f, ownerobj.transform.right) * ownerobj.transform.forward;
		if (this.level == 3)
		{
			RaycastHit raycastHit7;
			if (Physics.Raycast(vector, fwdvector, out raycastHit, 100f, this.ground))
			{
				RaycastHit raycastHit6;
				if (Physics.Raycast(new Vector3(raycastHit.point.x, raycastHit.point.y + 40f, raycastHit.point.z), Vector3.down, out raycastHit6, 200f, this.ground))
				{
					Object.Instantiate<GameObject>(this.LightningBoltPrefab, raycastHit6.point, Quaternion.identity).GetComponent<LightningBoltDamage>().DoDmg(ownerobj);
				}
				else if (Physics.Raycast(vector + fwdvector * 30f + Vector3.up * 40f, Vector3.down, out raycastHit6, 200f, this.ground))
				{
					Object.Instantiate<GameObject>(this.LightningBoltPrefab, raycastHit6.point, Quaternion.identity).GetComponent<LightningBoltDamage>().DoDmg(ownerobj);
				}
			}
			else if (Physics.Raycast(vector + fwdvector * 30f + Vector3.up * 40f, Vector3.down, out raycastHit7, 200f, this.ground))
			{
				Object.Instantiate<GameObject>(this.LightningBoltPrefab, raycastHit7.point, Quaternion.identity).GetComponent<LightningBoltDamage>().DoDmg(ownerobj);
			}
		}
		yield break;
	}

	// Token: 0x040003BC RID: 956
	private int level;

	// Token: 0x040003BD RID: 957
	private GameObject playerOwner;

	// Token: 0x040003BE RID: 958
	public GameObject LightningBoltPrefab;

	// Token: 0x040003BF RID: 959
	public LayerMask ground;
}
