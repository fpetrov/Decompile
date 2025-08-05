using System;
using UnityEngine;

// Token: 0x020001C7 RID: 455
public class Readme : ScriptableObject
{
	// Token: 0x04000AF4 RID: 2804
	public Texture2D icon;

	// Token: 0x04000AF5 RID: 2805
	public string title;

	// Token: 0x04000AF6 RID: 2806
	public Readme.Section[] sections;

	// Token: 0x04000AF7 RID: 2807
	public bool loadedLayout;

	// Token: 0x020001C8 RID: 456
	[Serializable]
	public class Section
	{
		// Token: 0x04000AF8 RID: 2808
		public string heading;

		// Token: 0x04000AF9 RID: 2809
		public string text;

		// Token: 0x04000AFA RID: 2810
		public string linkText;

		// Token: 0x04000AFB RID: 2811
		public string url;
	}
}
