using System;
using FishNet.Object;

// Token: 0x020000F7 RID: 247
public class NetworkedFireballController : NetworkBehaviour
{
	// Token: 0x06000A09 RID: 2569 RVA: 0x000021EF File Offset: 0x000003EF
	private void Start()
	{
	}

	// Token: 0x06000A0A RID: 2570 RVA: 0x000021EF File Offset: 0x000003EF
	private void Update()
	{
	}

	// Token: 0x06000A0C RID: 2572 RVA: 0x00026608 File Offset: 0x00024808
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyNetworkedFireballControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyNetworkedFireballControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000A0D RID: 2573 RVA: 0x0002661B File Offset: 0x0002481B
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateNetworkedFireballControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateNetworkedFireballControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000A0E RID: 2574 RVA: 0x0002662E File Offset: 0x0002482E
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000A0F RID: 2575 RVA: 0x0002662E File Offset: 0x0002482E
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000549 RID: 1353
	private bool NetworkInitialize___EarlyNetworkedFireballControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x0400054A RID: 1354
	private bool NetworkInitialize__LateNetworkedFireballControllerAssembly-CSharp.dll_Excuted;
}
