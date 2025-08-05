using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FishNet.Serializing.Generated
{
	// Token: 0x02000250 RID: 592
	[StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
	public static class GeneratedReaders___Internal
	{
		// Token: 0x06001743 RID: 5955 RVA: 0x00060712 File Offset: 0x0005E912
		[RuntimeInitializeOnLoadMethod]
		private static void InitializeOnce()
		{
			GenericReader<string[]>.SetRead(new Func<Reader, string[]>(GeneratedReaders___Internal.GRead___System.String[]FishNet.Serializing.Generateds));
		}

		// Token: 0x06001744 RID: 5956 RVA: 0x00060728 File Offset: 0x0005E928
		public static string[] GRead___System.String[]FishNet.Serializing.Generateds(Reader reader)
		{
			return reader.ReadArrayAllocated<string>();
		}
	}
}
