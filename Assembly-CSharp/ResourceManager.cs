using System;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

// Token: 0x02000159 RID: 345
public class ResourceManager : NetworkBehaviour
{
	// Token: 0x06000ED0 RID: 3792 RVA: 0x0003BD30 File Offset: 0x00039F30
	public void updatehex(int type, int ct)
	{
		if (ct == -1)
		{
			if (type == -1)
			{
				base.GetComponent<DungeonGenerator>().turndecalgray(7);
				return;
			}
			if (type == -2)
			{
				base.GetComponent<DungeonGenerator>().turndecalgray(8);
				return;
			}
			for (int i = 0; i < this.mapicons.Length; i++)
			{
				int num = this.mapicons[i];
				if (num == 2 && type == 2)
				{
					base.GetComponent<DungeonGenerator>().turndecalgray(i);
				}
				else if (num == 1 && type == 3)
				{
					base.GetComponent<DungeonGenerator>().turndecalgray(i);
				}
				else if (num == 4 && type == 0)
				{
					base.GetComponent<DungeonGenerator>().turndecalgray(i);
				}
				else if (num == 6 && type == 4)
				{
					base.GetComponent<DungeonGenerator>().turndecalgray(i);
				}
				else if (num == 7 && type == 1)
				{
					base.GetComponent<DungeonGenerator>().turndecalgray(i);
				}
			}
			return;
		}
		else
		{
			this.playerteam = Camera.main.transform.parent.GetComponent<PlayerMovement>().playerTeam;
			if (this.playerteam == 0)
			{
				this.playerteam = 1;
			}
			if (type == -1)
			{
				if (ct != this.playerteam)
				{
					base.GetComponent<DungeonGenerator>().turndecalred(7);
					return;
				}
				base.GetComponent<DungeonGenerator>().turndecalwhite(7);
				return;
			}
			else
			{
				if (type != -2)
				{
					for (int j = 0; j < this.mapicons.Length; j++)
					{
						int num2 = this.mapicons[j];
						if (num2 == 2 && type == 2)
						{
							if (ct != this.playerteam)
							{
								base.GetComponent<DungeonGenerator>().turndecalred(j);
							}
							else
							{
								base.GetComponent<DungeonGenerator>().turndecalwhite(j);
							}
						}
						else if (num2 == 1 && type == 3)
						{
							if (ct != this.playerteam)
							{
								base.GetComponent<DungeonGenerator>().turndecalred(j);
							}
							else
							{
								base.GetComponent<DungeonGenerator>().turndecalwhite(j);
							}
						}
						else if (num2 == 4 && type == 0)
						{
							if (ct != this.playerteam)
							{
								base.GetComponent<DungeonGenerator>().turndecalred(j);
							}
							else
							{
								base.GetComponent<DungeonGenerator>().turndecalwhite(j);
							}
						}
						else if (num2 == 6 && type == 4)
						{
							if (ct != this.playerteam)
							{
								base.GetComponent<DungeonGenerator>().turndecalred(j);
							}
							else
							{
								base.GetComponent<DungeonGenerator>().turndecalwhite(j);
							}
						}
						else if (num2 == 7 && type == 1)
						{
							if (ct != this.playerteam)
							{
								base.GetComponent<DungeonGenerator>().turndecalred(j);
							}
							else
							{
								base.GetComponent<DungeonGenerator>().turndecalwhite(j);
							}
						}
					}
					return;
				}
				if (ct != this.playerteam)
				{
					base.GetComponent<DungeonGenerator>().turndecalred(8);
					return;
				}
				base.GetComponent<DungeonGenerator>().turndecalwhite(8);
				return;
			}
		}
	}

	// Token: 0x06000ED2 RID: 3794 RVA: 0x0003BF9A File Offset: 0x0003A19A
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyResourceManagerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyResourceManagerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000ED3 RID: 3795 RVA: 0x0003BFAD File Offset: 0x0003A1AD
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateResourceManagerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateResourceManagerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000ED4 RID: 3796 RVA: 0x0003BFC0 File Offset: 0x0003A1C0
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000ED5 RID: 3797 RVA: 0x0003BFC0 File Offset: 0x0003A1C0
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0400081F RID: 2079
	public List<FlagController> Flags = new List<FlagController>();

	// Token: 0x04000820 RID: 2080
	public int[] mapicons = new int[8];

	// Token: 0x04000821 RID: 2081
	private int playerteam;

	// Token: 0x04000822 RID: 2082
	private bool NetworkInitialize___EarlyResourceManagerAssembly-CSharp.dll_Excuted;

	// Token: 0x04000823 RID: 2083
	private bool NetworkInitialize__LateResourceManagerAssembly-CSharp.dll_Excuted;
}
