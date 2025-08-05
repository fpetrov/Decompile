using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000179 RID: 377
public class SetSenseText : MonoBehaviour
{
	// Token: 0x06000F51 RID: 3921 RVA: 0x0003E260 File Offset: 0x0003C460
	public void SetText()
	{
		this.text.text = this.slider.value.ToString();
	}

	// Token: 0x040008C6 RID: 2246
	public TMP_Text text;

	// Token: 0x040008C7 RID: 2247
	public Slider slider;
}
