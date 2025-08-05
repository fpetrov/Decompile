using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FishNet.Serializing.Generated
{
	// Token: 0x0200024F RID: 591
	[StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
	public static class GeneratedWriters___Internal
	{
		// Token: 0x06001741 RID: 5953 RVA: 0x000606E5 File Offset: 0x0005E8E5
		[RuntimeInitializeOnLoadMethod]
		private static void InitializeOnce()
		{
			GenericWriter<string[]>.SetWrite(new Action<Writer, string[]>(GeneratedWriters___Internal.GWrite___System.String[]FishNet.Serializing.Generated));
		}

		// Token: 0x06001742 RID: 5954 RVA: 0x000606F8 File Offset: 0x0005E8F8
		public static void GWrite___System.String[]FishNet.Serializing.Generated(this Writer writer, string[] value)
		{
			writer.WriteArray<string>(value);
		}
	}
}
