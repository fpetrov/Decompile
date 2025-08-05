using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

// Token: 0x02000065 RID: 101
public class DungeonGenerator : NetworkBehaviour
{
	// Token: 0x06000439 RID: 1081 RVA: 0x000110D2 File Offset: 0x0000F2D2
	public void ResetDung()
	{
		this.HexesPlaced = false;
		this.dungeonPlaced = false;
		this.isDungeonGenerated = false;
		this.LoadScreen.SetActive(true);
	}

	// Token: 0x0600043A RID: 1082 RVA: 0x000110F5 File Offset: 0x0000F2F5
	public override void OnStartClient()
	{
		base.OnStartClient();
	}

	// Token: 0x0600043B RID: 1083 RVA: 0x00011100 File Offset: 0x0000F300
	private void Start()
	{
		for (int i = 0; i < this.OccupiedHexes.Length; i++)
		{
			this.OccupiedHexes[i] = false;
		}
	}

	// Token: 0x0600043C RID: 1084 RVA: 0x00011129 File Offset: 0x0000F329
	public void StartGenRoutine()
	{
		base.StartCoroutine(this.WaitForIsTutorial());
	}

	// Token: 0x0600043D RID: 1085 RVA: 0x00011138 File Offset: 0x0000F338
	public void setMapInfo(int info)
	{
		if (info == 0)
		{
			this.smallmap = false;
			this.isColoseumMap = false;
		}
		else if (info == 1)
		{
			this.smallmap = true;
			this.isColoseumMap = false;
		}
		else if (info == 2)
		{
			this.smallmap = false;
			this.isColoseumMap = true;
		}
		this.hasrecievedmapinfo = true;
	}

	// Token: 0x0600043E RID: 1086 RVA: 0x00011185 File Offset: 0x0000F385
	private IEnumerator WaitForIsTutorial()
	{
		while (this.IsThisTutorial || !this.hasrecievedmapinfo)
		{
			yield return null;
		}
		if (base.HasAuthority)
		{
			this.seedval = Random.Range(0, 1000000);
			if (this.isColoseumMap)
			{
				this.GenerateColosseum();
			}
			else if (this.smallmap)
			{
				base.StartCoroutine(this.GenerateSmallMap());
			}
			else
			{
				base.StartCoroutine(this.GenerateMap());
			}
			base.StartCoroutine(this.WaitBakeNavMesh());
		}
		else
		{
			base.StartCoroutine(this.BackupLoadScreenOff());
			this.ServerRequestSeed();
		}
		yield break;
	}

	// Token: 0x0600043F RID: 1087 RVA: 0x00011194 File Offset: 0x0000F394
	[ServerRpc(RequireOwnership = false)]
	private void ServerRequestSeed()
	{
		this.RpcWriter___Server_ServerRequestSeed_2166136261();
	}

	// Token: 0x06000440 RID: 1088 RVA: 0x0001119C File Offset: 0x0000F39C
	[ObserversRpc]
	private void ObsRequestSeed()
	{
		this.RpcWriter___Observers_ObsRequestSeed_2166136261();
	}

	// Token: 0x06000441 RID: 1089 RVA: 0x000111A4 File Offset: 0x0000F3A4
	private void SeedRequested()
	{
		if (base.HasAuthority)
		{
			this.ServerSyncSeed(this.seedval);
		}
	}

	// Token: 0x06000442 RID: 1090 RVA: 0x000111BA File Offset: 0x0000F3BA
	[ServerRpc(RequireOwnership = false)]
	private void ServerSyncSeed(int seed)
	{
		this.RpcWriter___Server_ServerSyncSeed_3316948804(seed);
	}

	// Token: 0x06000443 RID: 1091 RVA: 0x000111C6 File Offset: 0x0000F3C6
	[ObserversRpc]
	private void ObsSyncSeed(int seed)
	{
		this.RpcWriter___Observers_ObsSyncSeed_3316948804(seed);
	}

	// Token: 0x06000444 RID: 1092 RVA: 0x000111D2 File Offset: 0x0000F3D2
	[ServerRpc(RequireOwnership = false)]
	private void SmallMapRPC()
	{
		this.RpcWriter___Server_SmallMapRPC_2166136261();
	}

	// Token: 0x06000445 RID: 1093 RVA: 0x000111DC File Offset: 0x0000F3DC
	[ObserversRpc]
	private void SmallMapobsRPC()
	{
		this.RpcWriter___Observers_SmallMapobsRPC_2166136261();
	}

	// Token: 0x06000446 RID: 1094 RVA: 0x000111EF File Offset: 0x0000F3EF
	[ServerRpc(RequireOwnership = false)]
	private void LargeMapRPC()
	{
		this.RpcWriter___Server_LargeMapRPC_2166136261();
	}

	// Token: 0x06000447 RID: 1095 RVA: 0x000111F8 File Offset: 0x0000F3F8
	[ObserversRpc]
	private void LargeMapobsRPC()
	{
		this.RpcWriter___Observers_LargeMapobsRPC_2166136261();
	}

	// Token: 0x06000448 RID: 1096 RVA: 0x0001120B File Offset: 0x0000F40B
	private void GenerateColosseum()
	{
		this.ServerSpawnColosseum();
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x00011214 File Offset: 0x0000F414
	[ServerRpc(RequireOwnership = false)]
	private void ServerSpawnColosseum()
	{
		this.RpcWriter___Server_ServerSpawnColosseum_2166136261();
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x00011227 File Offset: 0x0000F427
	[ObserversRpc]
	private void ObsColosseumRPC()
	{
		this.RpcWriter___Observers_ObsColosseumRPC_2166136261();
	}

	// Token: 0x0600044B RID: 1099 RVA: 0x0001122F File Offset: 0x0000F42F
	private IEnumerator GenerateSmallMap()
	{
		int numloothexes = 0;
		this.OccupiedHexes[0] = true;
		this.OccupiedHexes[6] = true;
		this.SmallMapRPC();
		List<int> numbers = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };
		int num3;
		for (int j = numbers.Count - 1; j > 0; j--)
		{
			int num = Random.Range(0, j + 1);
			List<int> list = numbers;
			int num2 = j;
			List<int> list2 = numbers;
			num3 = num;
			int num4 = numbers[num];
			int num5 = numbers[j];
			list[num2] = num4;
			list2[num3] = num5;
		}
		for (int i = 0; i < this.OccupiedHexes.Length; i = num3 + 1)
		{
			if (!this.OccupiedHexes[i])
			{
				if (numbers[i] == 0 || numbers[i] == 3 || numbers[i] == 5)
				{
					num3 = numloothexes;
					numloothexes = num3 + 1;
				}
				if (numloothexes > 2)
				{
					if (numbers[0] != 0 && numbers[0] != 3 && numbers[0] != 5)
					{
						numbers[i] = numbers[0];
					}
					else if (numbers[7] != 0 && numbers[7] != 3 && numbers[7] != 5)
					{
						numbers[i] = numbers[7];
					}
				}
				else if (numloothexes < 2 && i == 5)
				{
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					for (int k = 1; k < 5; k++)
					{
						if (numbers[k] == 0)
						{
							flag3 = true;
						}
						else if (numbers[k] == 3)
						{
							flag = true;
						}
						else if (numbers[k] == 5)
						{
							flag2 = true;
						}
					}
					if (!flag2)
					{
						numbers[i] = 5;
					}
					else if (!flag3)
					{
						numbers[i] = 0;
					}
					else if (!flag)
					{
						numbers[i] = 3;
					}
				}
				if (numbers[i] == 3)
				{
					this.dungeonPlaced = true;
					this.ServerPlaceHex(numbers[i], i);
					while (!this.PrevHexSpawned && !this.isDungeonGenerated)
					{
						yield return null;
					}
					yield return null;
					this.PrevHexSpawned = false;
				}
				else
				{
					this.ServerPlaceHex(numbers[i], i);
					while (!this.PrevHexSpawned)
					{
						yield return null;
					}
					this.PrevHexSpawned = false;
				}
			}
			num3 = i;
		}
		this.HexesPlaced = true;
		for (int l = 0; l < this.OccupiedHexes.Length; l++)
		{
			this.OccupiedHexes[l] = false;
		}
		yield break;
	}

	// Token: 0x0600044C RID: 1100 RVA: 0x0001123E File Offset: 0x0000F43E
	private IEnumerator GenerateMap()
	{
		this.LargeMapRPC();
		for (int j = 0; j < this.ToggledDeformers.Length; j++)
		{
			if (j < 4)
			{
				this.ToggledDeformers[j].SetActive(true);
			}
			else
			{
				this.ToggledDeformers[j].SetActive(false);
			}
		}
		if (Random.Range(0, 3) == 10)
		{
			this.PlaceMountain();
			while (!this.PrevHexSpawned)
			{
				yield return null;
			}
			this.PrevHexSpawned = false;
		}
		List<int> numbers = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };
		int num3;
		for (int k = numbers.Count - 1; k > 0; k--)
		{
			int num = Random.Range(0, k + 1);
			List<int> list = numbers;
			int num2 = k;
			List<int> list2 = numbers;
			num3 = num;
			int num4 = numbers[num];
			int num5 = numbers[k];
			list[num2] = num4;
			list2[num3] = num5;
		}
		for (int i = 0; i < this.OccupiedHexes.Length; i = num3 + 1)
		{
			if (!this.OccupiedHexes[i])
			{
				if (numbers[i] == 3)
				{
					this.dungeonPlaced = true;
					this.ServerPlaceHex(numbers[i], i);
					while (!this.PrevHexSpawned && !this.isDungeonGenerated)
					{
						yield return null;
					}
					this.PrevHexSpawned = false;
				}
				else
				{
					this.ServerPlaceHex(numbers[i], i);
					while (!this.PrevHexSpawned)
					{
						yield return null;
					}
					this.PrevHexSpawned = false;
				}
			}
			num3 = i;
		}
		this.HexesPlaced = true;
		for (int l = 0; l < this.OccupiedHexes.Length; l++)
		{
			this.OccupiedHexes[l] = false;
		}
		yield break;
	}

	// Token: 0x0600044D RID: 1101 RVA: 0x0001124D File Offset: 0x0000F44D
	private IEnumerator WaitBakeNavMesh()
	{
		while (!this.HexesPlaced)
		{
			yield return null;
		}
		yield return null;
		this.ServerBakeNM();
		yield break;
	}

	// Token: 0x0600044E RID: 1102 RVA: 0x0001125C File Offset: 0x0000F45C
	[ServerRpc(RequireOwnership = false)]
	private void ServerBakeNM()
	{
		this.RpcWriter___Server_ServerBakeNM_2166136261();
	}

	// Token: 0x0600044F RID: 1103 RVA: 0x00011264 File Offset: 0x0000F464
	[ObserversRpc]
	private void obsBakeNM()
	{
		this.RpcWriter___Observers_obsBakeNM_2166136261();
	}

	// Token: 0x06000450 RID: 1104 RVA: 0x0001126C File Offset: 0x0000F46C
	private IEnumerator WaitDungeon()
	{
		if (this.dungeonPlaced)
		{
			while (!this.isDungeonGenerated)
			{
				yield return null;
			}
		}
		this.navsurface.BuildNavMesh();
		base.StartCoroutine(this.ResetNavLinks());
		this.LoadScreen.SetActive(false);
		yield break;
	}

	// Token: 0x06000451 RID: 1105 RVA: 0x0001127B File Offset: 0x0000F47B
	private IEnumerator BackupLoadScreenOff()
	{
		yield return new WaitForSeconds(8f);
		this.LoadScreen.SetActive(false);
		yield break;
	}

	// Token: 0x06000452 RID: 1106 RVA: 0x0001128A File Offset: 0x0000F48A
	private IEnumerator ResetNavLinks()
	{
		yield return null;
		GameObject[] navlinks = GameObject.FindGameObjectsWithTag("NavLink");
		GameObject[] array = navlinks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		yield return null;
		array = navlinks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(true);
		}
		yield break;
	}

	// Token: 0x06000453 RID: 1107 RVA: 0x00011294 File Offset: 0x0000F494
	[ServerRpc(RequireOwnership = false)]
	private void ServerPlaceHex(int HexType, int hexindex)
	{
		this.RpcWriter___Server_ServerPlaceHex_1692629761(HexType, hexindex);
	}

	// Token: 0x06000454 RID: 1108 RVA: 0x000112B0 File Offset: 0x0000F4B0
	[ObserversRpc]
	private void updatemap(int ht, int hid)
	{
		this.RpcWriter___Observers_updatemap_1692629761(ht, hid);
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x000112CC File Offset: 0x0000F4CC
	private void PlaceMountain()
	{
		int num = Random.Range(0, 2);
		if (num == 10)
		{
			this.OccupiedHexes[3] = true;
			this.OccupiedHexes[4] = true;
			this.OccupiedHexes[6] = true;
		}
		else if (num == 1)
		{
			this.OccupiedHexes[2] = true;
			this.OccupiedHexes[3] = true;
			this.OccupiedHexes[5] = true;
		}
		this.ServerSpawnMountain(num);
	}

	// Token: 0x06000456 RID: 1110 RVA: 0x0001132C File Offset: 0x0000F52C
	[ServerRpc(RequireOwnership = false)]
	private void ServerSpawnMountain(int transformIndex)
	{
		this.RpcWriter___Server_ServerSpawnMountain_3316948804(transformIndex);
	}

	// Token: 0x06000457 RID: 1111 RVA: 0x00011344 File Offset: 0x0000F544
	public void turndecalred(int id)
	{
		if (this.smallmap)
		{
			if (this.smallmap && id == 7)
			{
				this.mapicons[0].material.color = Color.red;
			}
			else if (this.smallmap && id == 8)
			{
				this.mapicons[6].material.color = Color.red;
			}
		}
		this.mapicons[id].material.color = Color.red;
	}

	// Token: 0x06000458 RID: 1112 RVA: 0x000113B8 File Offset: 0x0000F5B8
	public void turndecalwhite(int id)
	{
		if (this.smallmap && id == 7)
		{
			this.mapicons[0].material.color = Color.white;
		}
		else if (this.smallmap && id == 8)
		{
			this.mapicons[6].material.color = Color.white;
		}
		this.mapicons[id].material.color = Color.white;
	}

	// Token: 0x06000459 RID: 1113 RVA: 0x00011424 File Offset: 0x0000F624
	public void turndecalgray(int id)
	{
		this.mapicons[id].material.color = Color.gray;
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x00011460 File Offset: 0x0000F660
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyDungeonGeneratorAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyDungeonGeneratorAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ServerRequestSeed_2166136261));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_ObsRequestSeed_2166136261));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_ServerSyncSeed_3316948804));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ObsSyncSeed_3316948804));
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_SmallMapRPC_2166136261));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_SmallMapobsRPC_2166136261));
		base.RegisterServerRpc(6U, new ServerRpcDelegate(this.RpcReader___Server_LargeMapRPC_2166136261));
		base.RegisterObserversRpc(7U, new ClientRpcDelegate(this.RpcReader___Observers_LargeMapobsRPC_2166136261));
		base.RegisterServerRpc(8U, new ServerRpcDelegate(this.RpcReader___Server_ServerSpawnColosseum_2166136261));
		base.RegisterObserversRpc(9U, new ClientRpcDelegate(this.RpcReader___Observers_ObsColosseumRPC_2166136261));
		base.RegisterServerRpc(10U, new ServerRpcDelegate(this.RpcReader___Server_ServerBakeNM_2166136261));
		base.RegisterObserversRpc(11U, new ClientRpcDelegate(this.RpcReader___Observers_obsBakeNM_2166136261));
		base.RegisterServerRpc(12U, new ServerRpcDelegate(this.RpcReader___Server_ServerPlaceHex_1692629761));
		base.RegisterObserversRpc(13U, new ClientRpcDelegate(this.RpcReader___Observers_updatemap_1692629761));
		base.RegisterServerRpc(14U, new ServerRpcDelegate(this.RpcReader___Server_ServerSpawnMountain_3316948804));
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x000115D7 File Offset: 0x0000F7D7
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateDungeonGeneratorAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateDungeonGeneratorAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x000115EA File Offset: 0x0000F7EA
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0600045E RID: 1118 RVA: 0x000115F8 File Offset: 0x0000F7F8
	private void RpcWriter___Server_ServerRequestSeed_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600045F RID: 1119 RVA: 0x0001165D File Offset: 0x0000F85D
	private void RpcLogic___ServerRequestSeed_2166136261()
	{
		this.ObsRequestSeed();
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x00011668 File Offset: 0x0000F868
	private void RpcReader___Server_ServerRequestSeed_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerRequestSeed_2166136261();
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x00011688 File Offset: 0x0000F888
	private void RpcWriter___Observers_ObsRequestSeed_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x000116FC File Offset: 0x0000F8FC
	private void RpcLogic___ObsRequestSeed_2166136261()
	{
		this.SeedRequested();
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x00011704 File Offset: 0x0000F904
	private void RpcReader___Observers_ObsRequestSeed_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsRequestSeed_2166136261();
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x00011724 File Offset: 0x0000F924
	private void RpcWriter___Server_ServerSyncSeed_3316948804(int seed)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(seed);
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000465 RID: 1125 RVA: 0x00011796 File Offset: 0x0000F996
	private void RpcLogic___ServerSyncSeed_3316948804(int seed)
	{
		this.ObsSyncSeed(seed);
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x000117A0 File Offset: 0x0000F9A0
	private void RpcReader___Server_ServerSyncSeed_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSyncSeed_3316948804(num);
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x000117D4 File Offset: 0x0000F9D4
	private void RpcWriter___Observers_ObsSyncSeed_3316948804(int seed)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(seed);
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x00011855 File Offset: 0x0000FA55
	private void RpcLogic___ObsSyncSeed_3316948804(int seed)
	{
		this.seedval = seed;
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x00011860 File Offset: 0x0000FA60
	private void RpcReader___Observers_ObsSyncSeed_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsSyncSeed_3316948804(num);
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x00011894 File Offset: 0x0000FA94
	private void RpcWriter___Server_SmallMapRPC_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(4U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x000118F9 File Offset: 0x0000FAF9
	private void RpcLogic___SmallMapRPC_2166136261()
	{
		this.SmallMapobsRPC();
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x00011904 File Offset: 0x0000FB04
	private void RpcReader___Server_SmallMapRPC_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___SmallMapRPC_2166136261();
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x00011924 File Offset: 0x0000FB24
	private void RpcWriter___Observers_SmallMapobsRPC_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(5U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x00011998 File Offset: 0x0000FB98
	private void RpcLogic___SmallMapobsRPC_2166136261()
	{
		this.Smallbeach.gameObject.SetActive(true);
		this.LargeBeach.gameObject.SetActive(false);
		this.castles[0].position = new Vector3(147.1f, 54.41f, 13f);
		this.castles[1].position = new Vector3(3.6f, 54.41f, 252.3f);
		this.mapicons[7].material = this.smallmapicons[2];
		this.mapicons[8].material = this.smallmapicons[2];
		this.mapicons[0].material = this.smallmapicons[0];
		base.GetComponent<PlayerRespawnManager>().isSmallMap = true;
		this.mapicons[6].material = this.smallmapicons[1];
		for (int i = 0; i < this.ToggledDeformers.Length; i++)
		{
			if (i < 4)
			{
				this.ToggledDeformers[i].SetActive(false);
			}
			else
			{
				this.ToggledDeformers[i].SetActive(true);
			}
		}
		this.smallmap = true;
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x00011AA8 File Offset: 0x0000FCA8
	private void RpcReader___Observers_SmallMapobsRPC_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___SmallMapobsRPC_2166136261();
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x00011AC8 File Offset: 0x0000FCC8
	private void RpcWriter___Server_LargeMapRPC_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(6U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x00011B2D File Offset: 0x0000FD2D
	private void RpcLogic___LargeMapRPC_2166136261()
	{
		this.LargeMapobsRPC();
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x00011B38 File Offset: 0x0000FD38
	private void RpcReader___Server_LargeMapRPC_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___LargeMapRPC_2166136261();
	}

	// Token: 0x06000473 RID: 1139 RVA: 0x00011B58 File Offset: 0x0000FD58
	private void RpcWriter___Observers_LargeMapobsRPC_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(7U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x00011BCC File Offset: 0x0000FDCC
	private void RpcLogic___LargeMapobsRPC_2166136261()
	{
		this.Smallbeach.gameObject.SetActive(false);
		this.LargeBeach.gameObject.SetActive(true);
		for (int i = 0; i < this.ToggledDeformers.Length; i++)
		{
			if (i < 4)
			{
				this.ToggledDeformers[i].SetActive(true);
			}
			else
			{
				this.ToggledDeformers[i].SetActive(false);
			}
		}
		base.GetComponent<PlayerRespawnManager>().isSmallMap = false;
		this.smallmap = false;
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x00011C44 File Offset: 0x0000FE44
	private void RpcReader___Observers_LargeMapobsRPC_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___LargeMapobsRPC_2166136261();
	}

	// Token: 0x06000476 RID: 1142 RVA: 0x00011C64 File Offset: 0x0000FE64
	private void RpcWriter___Server_ServerSpawnColosseum_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(8U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000477 RID: 1143 RVA: 0x00011CCC File Offset: 0x0000FECC
	private void RpcLogic___ServerSpawnColosseum_2166136261()
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.coloseum, this.ColSpawnPos.position, Quaternion.identity);
		base.ServerManager.Spawn(gameObject, null, default(Scene));
		this.ObsColosseumRPC();
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x00011D14 File Offset: 0x0000FF14
	private void RpcReader___Server_ServerSpawnColosseum_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSpawnColosseum_2166136261();
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x00011D34 File Offset: 0x0000FF34
	private void RpcWriter___Observers_ObsColosseumRPC_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(9U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x00011DA8 File Offset: 0x0000FFA8
	private void RpcLogic___ObsColosseumRPC_2166136261()
	{
		base.GetComponent<PlayerRespawnManager>().isSmallMap = false;
		base.GetComponent<PlayerRespawnManager>().moveRespawnPointsColosseum();
		this.HexesPlaced = true;
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x00011DC8 File Offset: 0x0000FFC8
	private void RpcReader___Observers_ObsColosseumRPC_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsColosseumRPC_2166136261();
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x00011DE8 File Offset: 0x0000FFE8
	private void RpcWriter___Server_ServerBakeNM_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(10U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x00011E4D File Offset: 0x0001004D
	private void RpcLogic___ServerBakeNM_2166136261()
	{
		this.obsBakeNM();
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x00011E58 File Offset: 0x00010058
	private void RpcReader___Server_ServerBakeNM_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerBakeNM_2166136261();
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x00011E78 File Offset: 0x00010078
	private void RpcWriter___Observers_obsBakeNM_2166136261()
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendObserversRpc(11U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x00011EEC File Offset: 0x000100EC
	private void RpcLogic___obsBakeNM_2166136261()
	{
		base.StartCoroutine(this.WaitDungeon());
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x00011EFC File Offset: 0x000100FC
	private void RpcReader___Observers_obsBakeNM_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___obsBakeNM_2166136261();
	}

	// Token: 0x06000482 RID: 1154 RVA: 0x00011F1C File Offset: 0x0001011C
	private void RpcWriter___Server_ServerPlaceHex_1692629761(int HexType, int hexindex)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(HexType);
		pooledWriter.WriteInt32(hexindex);
		base.SendServerRpc(12U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000483 RID: 1155 RVA: 0x00011F9C File Offset: 0x0001019C
	private void RpcLogic___ServerPlaceHex_1692629761(int HexType, int hexindex)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.Hexes[HexType]);
		gameObject.transform.position = this.HexPoints[hexindex].position;
		base.ServerManager.Spawn(gameObject, null, default(Scene));
		this.PrevHexSpawned = true;
		this.updatemap(HexType, hexindex);
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x00011FF4 File Offset: 0x000101F4
	private void RpcReader___Server_ServerPlaceHex_1692629761(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		int num2 = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerPlaceHex_1692629761(num, num2);
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x00012038 File Offset: 0x00010238
	private void RpcWriter___Observers_updatemap_1692629761(int ht, int hid)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(ht);
		pooledWriter.WriteInt32(hid);
		base.SendObserversRpc(13U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x000120C8 File Offset: 0x000102C8
	private void RpcLogic___updatemap_1692629761(int ht, int hid)
	{
		this.mapicons[hid].material = this.hexmapicons[ht];
		base.GetComponent<ResourceManager>().mapicons[hid] = ht;
		this.mapicons[hid].material.color = Color.white;
		if (ht == 1 || ht == 2 || ht == 4 || ht == 6 || ht == 7)
		{
			this.mapicons[hid].material.color = Color.gray;
		}
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x0001213C File Offset: 0x0001033C
	private void RpcReader___Observers_updatemap_1692629761(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		int num2 = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___updatemap_1692629761(num, num2);
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x00012180 File Offset: 0x00010380
	private void RpcWriter___Server_ServerSpawnMountain_3316948804(int transformIndex)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(transformIndex);
		base.SendServerRpc(14U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x000121F4 File Offset: 0x000103F4
	private void RpcLogic___ServerSpawnMountain_3316948804(int transformIndex)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.Hexes[7]);
		gameObject.transform.position = this.MountainPoints[transformIndex].position;
		base.ServerManager.Spawn(gameObject, null, default(Scene));
		this.PrevHexSpawned = true;
		if (transformIndex == 0)
		{
			this.updatemap(7, 3);
			this.updatemap(7, 4);
			this.updatemap(7, 6);
			return;
		}
		this.updatemap(7, 2);
		this.updatemap(7, 3);
		this.updatemap(7, 5);
	}

	// Token: 0x0600048A RID: 1162 RVA: 0x00012278 File Offset: 0x00010478
	private void RpcReader___Server_ServerSpawnMountain_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSpawnMountain_3316948804(num);
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x000115EA File Offset: 0x0000F7EA
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000212 RID: 530
	public NavMeshSurface navsurface;

	// Token: 0x04000213 RID: 531
	public GameObject[] Hexes;

	// Token: 0x04000214 RID: 532
	private bool[] OccupiedHexes = new bool[7];

	// Token: 0x04000215 RID: 533
	public Transform[] HexPoints;

	// Token: 0x04000216 RID: 534
	public Transform[] MountainPoints;

	// Token: 0x04000217 RID: 535
	private NetworkManager networkManager;

	// Token: 0x04000218 RID: 536
	private bool mountain;

	// Token: 0x04000219 RID: 537
	public int seedval;

	// Token: 0x0400021A RID: 538
	private bool PrevHexSpawned;

	// Token: 0x0400021B RID: 539
	private bool HexesPlaced;

	// Token: 0x0400021C RID: 540
	private bool dungeonPlaced;

	// Token: 0x0400021D RID: 541
	public bool isDungeonGenerated;

	// Token: 0x0400021E RID: 542
	public bool IsThisTutorial = true;

	// Token: 0x0400021F RID: 543
	public LayerMask groundlayer;

	// Token: 0x04000220 RID: 544
	public GameObject LoadScreen;

	// Token: 0x04000221 RID: 545
	public Material[] hexmapicons;

	// Token: 0x04000222 RID: 546
	public Material[] smallmapicons;

	// Token: 0x04000223 RID: 547
	public DecalProjector[] mapicons;

	// Token: 0x04000224 RID: 548
	private bool hasrecievedmapinfo;

	// Token: 0x04000225 RID: 549
	private bool smallmap = true;

	// Token: 0x04000226 RID: 550
	public Transform Smallbeach;

	// Token: 0x04000227 RID: 551
	public Transform LargeBeach;

	// Token: 0x04000228 RID: 552
	public Transform[] castles;

	// Token: 0x04000229 RID: 553
	public GameObject[] ToggledDeformers;

	// Token: 0x0400022A RID: 554
	public bool isColoseumMap;

	// Token: 0x0400022B RID: 555
	public GameObject coloseum;

	// Token: 0x0400022C RID: 556
	public Transform ColSpawnPos;

	// Token: 0x0400022D RID: 557
	private bool NetworkInitialize___EarlyDungeonGeneratorAssembly-CSharp.dll_Excuted;

	// Token: 0x0400022E RID: 558
	private bool NetworkInitialize__LateDungeonGeneratorAssembly-CSharp.dll_Excuted;
}
