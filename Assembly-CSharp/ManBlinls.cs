using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000E3 RID: 227
public class ManBlinls : MonoBehaviour
{
	// Token: 0x0600096F RID: 2415 RVA: 0x00024BD6 File Offset: 0x00022DD6
	private void Start()
	{
		this.mrend = base.GetComponent<SkinnedMeshRenderer>();
		this.mata = this.mrend.material;
		base.StartCoroutine(this.ManBlink());
	}

	// Token: 0x06000970 RID: 2416 RVA: 0x00024C02 File Offset: 0x00022E02
	private IEnumerator ManBlink()
	{
		while (base.isActiveAndEnabled)
		{
			yield return new WaitForSeconds(Random.Range(0.1f, 3f));
			this.mata.mainTextureOffset = new Vector2(0.5f, 0f);
			this.mata.SetTextureOffset("_EmissiveColorMap", new Vector2(0.5f, 0f));
			yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
			this.mata.mainTextureOffset = new Vector2(1f, 0f);
			this.mata.SetTextureOffset("_EmissiveColorMap", new Vector2(1f, 0f));
		}
		yield break;
	}

	// Token: 0x040004FD RID: 1277
	private SkinnedMeshRenderer mrend;

	// Token: 0x040004FE RID: 1278
	private Material mata;
}
