using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000B2 RID: 178
public class KillFeedMessage : MonoBehaviour
{
	// Token: 0x0600070C RID: 1804 RVA: 0x0001B05C File Offset: 0x0001925C
	public void setitup(string name, Texture2D death, string killername)
	{
		Vector2 anchoredPosition = this.deth.rectTransform.anchoredPosition;
		this.deth.texture = death;
		this.namee.text = name;
		this.KillerText.text = killername;
		float preferredWidth = this.namee.preferredWidth;
		Vector2 sizeDelta = this.namee.rectTransform.sizeDelta;
		sizeDelta.x = preferredWidth;
		this.namee.rectTransform.sizeDelta = sizeDelta;
		float num = -(preferredWidth / 2f) - 23f;
		this.namee.rectTransform.anchoredPosition = new Vector2(num, this.namee.rectTransform.anchoredPosition.y);
		this.deth.rectTransform.anchoredPosition = new Vector2(-preferredWidth - 50f, this.deth.rectTransform.anchoredPosition.y);
		float preferredWidth2 = this.KillerText.preferredWidth;
		Vector2 sizeDelta2 = this.KillerText.rectTransform.sizeDelta;
		sizeDelta2.x = preferredWidth2;
		this.KillerText.rectTransform.sizeDelta = sizeDelta2;
		float num2 = -(preferredWidth2 / 2f) - 23f;
		this.KillerText.rectTransform.anchoredPosition = new Vector2(-preferredWidth + num2 - 54f, this.KillerText.rectTransform.anchoredPosition.y);
		Vector2 sizeDelta3 = this.blackbox.sizeDelta;
		sizeDelta3.x += preferredWidth + preferredWidth2 + 16f;
		this.blackbox.sizeDelta = sizeDelta3;
	}

	// Token: 0x04000381 RID: 897
	public RectTransform blackbox;

	// Token: 0x04000382 RID: 898
	public Text namee;

	// Token: 0x04000383 RID: 899
	public RawImage deth;

	// Token: 0x04000384 RID: 900
	public Text KillerText;
}
