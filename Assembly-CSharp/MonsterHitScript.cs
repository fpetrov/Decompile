using System;
using UnityEngine;

// Token: 0x0200020B RID: 523
public class MonsterHitScript : MonoBehaviour, IInteractableNetworkObj
{
	// Token: 0x060014E0 RID: 5344 RVA: 0x00003C7C File Offset: 0x00001E7C
	public void ActualInteraction()
	{
		throw new NotImplementedException();
	}

	// Token: 0x060014E1 RID: 5345 RVA: 0x00057B6D File Offset: 0x00055D6D
	public void HitTheMonster(float Damage)
	{
		Debug.Log("e");
		this.monsterObject.GetComponent<IHitableMonster>().HitMonster(Damage);
	}

	// Token: 0x060014E2 RID: 5346 RVA: 0x00057B8A File Offset: 0x00055D8A
	public void NetInteract()
	{
		this.HitTheMonster(50f);
	}

	// Token: 0x04000C41 RID: 3137
	public GameObject monsterObject;
}
