using System;
using UnityEngine;

// Token: 0x0200016C RID: 364
[ExecuteInEditMode]
public class SamplesShowcase : MonoBehaviour
{
	// Token: 0x06000F20 RID: 3872 RVA: 0x000021EF File Offset: 0x000003EF
	private void Start()
	{
	}

	// Token: 0x06000F21 RID: 3873 RVA: 0x0003D342 File Offset: 0x0003B542
	private void OnValidate()
	{
		this.needUpdate = true;
	}

	// Token: 0x06000F22 RID: 3874 RVA: 0x0003D34C File Offset: 0x0003B54C
	private void Update()
	{
		if (Application.isFocused && Input.GetKeyDown(KeyCode.Space))
		{
			this.SwitchEffect();
		}
		if (this.needUpdate && this.currentIndex != this.prefabIndex)
		{
			this.CleanChildren();
			this.InstantiateSample(this.currentIndex);
			this.needUpdate = false;
		}
	}

	// Token: 0x06000F23 RID: 3875 RVA: 0x0003D39E File Offset: 0x0003B59E
	private void SwitchEffect()
	{
		this.currentIndex++;
		this.currentIndex = ((this.currentIndex > this.samplesPrefabs.Length - 1) ? 0 : this.currentIndex);
		this.needUpdate = true;
	}

	// Token: 0x06000F24 RID: 3876 RVA: 0x0003D3D8 File Offset: 0x0003B5D8
	private void InstantiateSample(int index)
	{
		if (this.currentIndex <= this.samplesPrefabs.Length && this.samplesPrefabs.Length != 0)
		{
			this.currentPrefab = this.samplesPrefabs[index];
		}
		if (this.currentPrefab != null)
		{
			this.instantiatedPrefab = Object.Instantiate(this.currentPrefab, base.transform.position, Quaternion.identity) as GameObject;
			this.instantiatedPrefab.transform.parent = base.gameObject.transform;
			this.prefabIndex = this.currentIndex;
		}
	}

	// Token: 0x06000F25 RID: 3877 RVA: 0x0003D468 File Offset: 0x0003B668
	private void CleanChildren()
	{
		if (base.transform.childCount > 0)
		{
			foreach (object obj in base.transform)
			{
				Object.DestroyImmediate(((Transform)obj).gameObject);
			}
		}
	}

	// Token: 0x04000884 RID: 2180
	public string headline = "Headline Goes Here";

	// Token: 0x04000885 RID: 2181
	public Color headlineColor = Color.white;

	// Token: 0x04000886 RID: 2182
	public Color linkColor = Color.white;

	// Token: 0x04000887 RID: 2183
	public TextAsset SamplesDescriptions;

	// Token: 0x04000888 RID: 2184
	public GameObject[] samplesPrefabs;

	// Token: 0x04000889 RID: 2185
	public int currentIndex;

	// Token: 0x0400088A RID: 2186
	private Object currentPrefab;

	// Token: 0x0400088B RID: 2187
	private int prefabIndex;

	// Token: 0x0400088C RID: 2188
	public GameObject instantiatedPrefab;

	// Token: 0x0400088D RID: 2189
	private bool needUpdate;
}
