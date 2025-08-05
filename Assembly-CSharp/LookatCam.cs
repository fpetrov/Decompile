using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000C3 RID: 195
public class LookatCam : MonoBehaviour, IInteractable
{
	// Token: 0x06000787 RID: 1927 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interact(GameObject player)
	{
	}

	// Token: 0x06000788 RID: 1928 RVA: 0x0001CE1D File Offset: 0x0001B01D
	public string DisplayInteractUI(GameObject player)
	{
		if (Time.time - this.cdtimer > 30f)
		{
			this.cdtimer = Time.time;
			base.StartCoroutine(this.shitezaRoutine());
		}
		return "";
	}

	// Token: 0x06000789 RID: 1929 RVA: 0x0001CE4F File Offset: 0x0001B04F
	private IEnumerator shitezaRoutine()
	{
		Material mata = this.shitren.material;
		float timer = 0f;
		while (timer < 1f)
		{
			mata.SetFloat("_AlphaRemapMax", Mathf.Lerp(0.3f, 1f, timer));
			this.cg.alpha = Mathf.Lerp(0.3f, 1f, timer);
			timer += Time.deltaTime;
			yield return null;
		}
		yield return new WaitForSeconds(3f);
		timer = 0f;
		while (timer < 1f)
		{
			mata.SetFloat("_AlphaRemapMax", Mathf.Lerp(1f, 0.3f, timer));
			this.cg.alpha = Mathf.Lerp(1f, 0.3f, timer);
			timer += Time.deltaTime;
			yield return null;
		}
		yield break;
	}

	// Token: 0x040003D6 RID: 982
	private float cdtimer = -30f;

	// Token: 0x040003D7 RID: 983
	public CanvasGroup cg;

	// Token: 0x040003D8 RID: 984
	public MeshRenderer shitren;
}
