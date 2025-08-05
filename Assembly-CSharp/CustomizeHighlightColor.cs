using System;
using UnityEngine;

// Token: 0x02000167 RID: 359
[RequireComponent(typeof(Renderer))]
public class CustomizeHighlightColor : MonoBehaviour
{
	// Token: 0x06000F14 RID: 3860 RVA: 0x0003CE96 File Offset: 0x0003B096
	private void Start()
	{
		this.SetColor();
	}

	// Token: 0x06000F15 RID: 3861 RVA: 0x0003CE96 File Offset: 0x0003B096
	private void OnValidate()
	{
		this.SetColor();
	}

	// Token: 0x06000F16 RID: 3862 RVA: 0x0003CEA0 File Offset: 0x0003B0A0
	private void SetColor()
	{
		Renderer component = base.GetComponent<Renderer>();
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		component.GetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetColor("_SelectionColor", this.selectionColor);
		component.SetPropertyBlock(materialPropertyBlock);
	}

	// Token: 0x0400085E RID: 2142
	public Color selectionColor = Color.white;
}
