using System;
using UnityEngine;

// Token: 0x02000174 RID: 372
[ExecuteInEditMode]
public class SampleShowcase : MonoBehaviour
{
	// Token: 0x06000F41 RID: 3905 RVA: 0x000021EF File Offset: 0x000003EF
	private void Start()
	{
	}

	// Token: 0x06000F42 RID: 3906 RVA: 0x0003DEE6 File Offset: 0x0003C0E6
	private void OnValidate()
	{
		this.needUpdate = true;
	}

	// Token: 0x06000F43 RID: 3907 RVA: 0x0003DEF0 File Offset: 0x0003C0F0
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

	// Token: 0x06000F44 RID: 3908 RVA: 0x0003DF42 File Offset: 0x0003C142
	private void SwitchEffect()
	{
		this.currentIndex++;
		this.currentIndex = ((this.currentIndex > this.samplesPrefabs.Length - 1) ? 0 : this.currentIndex);
		this.needUpdate = true;
	}

	// Token: 0x06000F45 RID: 3909 RVA: 0x0003DF7C File Offset: 0x0003C17C
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

	// Token: 0x06000F46 RID: 3910 RVA: 0x0003E00C File Offset: 0x0003C20C
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

	// Token: 0x040008B4 RID: 2228
	public string headline = "Headline Goes Here";

	// Token: 0x040008B5 RID: 2229
	public Color headlineColor = Color.white;

	// Token: 0x040008B6 RID: 2230
	public Color linkColor = Color.white;

	// Token: 0x040008B7 RID: 2231
	public TextAsset SamplesDescriptions;

	// Token: 0x040008B8 RID: 2232
	public GameObject[] samplesPrefabs;

	// Token: 0x040008B9 RID: 2233
	public int currentIndex;

	// Token: 0x040008BA RID: 2234
	private Object currentPrefab;

	// Token: 0x040008BB RID: 2235
	private int prefabIndex;

	// Token: 0x040008BC RID: 2236
	public GameObject instantiatedPrefab;

	// Token: 0x040008BD RID: 2237
	private bool needUpdate;
}
