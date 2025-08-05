using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using Graphs;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000200 RID: 512
public class Generator2D : NetworkBehaviour
{
	// Token: 0x0600147D RID: 5245 RVA: 0x00055C4C File Offset: 0x00053E4C
	public override void OnStartClient()
	{
		this.gamemanager = GameObject.FindGameObjectWithTag("NetItemManager");
		if (base.HasAuthority)
		{
			Debug.Log("dungeon");
			Debug.Log("dungeon");
			Debug.Log("dungeon");
			Debug.Log("dungeon");
			Debug.Log("dungeon");
			Debug.Log("dungeon");
			Debug.Log("dungeon");
			Debug.Log("dungeon");
			Debug.Log("dungeon");
			Debug.Log("dungeon");
			this.seedval = Random.Range(0, 1000000);
			this.seedSynced = true;
			this.random = new Random(this.seedval);
			this.Generate();
			return;
		}
		base.StartCoroutine(this.AwaitSeedSync());
		this.ServerRequestSeed();
	}

	// Token: 0x0600147E RID: 5246 RVA: 0x00055D1B File Offset: 0x00053F1B
	[ServerRpc(RequireOwnership = false)]
	private void ServerRequestSeed()
	{
		this.RpcWriter___Server_ServerRequestSeed_2166136261();
	}

	// Token: 0x0600147F RID: 5247 RVA: 0x00055D23 File Offset: 0x00053F23
	[ObserversRpc]
	private void ObsRequestSeed()
	{
		this.RpcWriter___Observers_ObsRequestSeed_2166136261();
	}

	// Token: 0x06001480 RID: 5248 RVA: 0x00055D2B File Offset: 0x00053F2B
	private void SeedRequested()
	{
		if (base.HasAuthority)
		{
			this.ServerSyncSeed(this.seedval);
		}
	}

	// Token: 0x06001481 RID: 5249 RVA: 0x00055D41 File Offset: 0x00053F41
	[ServerRpc(RequireOwnership = false)]
	private void ServerSyncSeed(int seed)
	{
		this.RpcWriter___Server_ServerSyncSeed_3316948804(seed);
	}

	// Token: 0x06001482 RID: 5250 RVA: 0x00055D4D File Offset: 0x00053F4D
	[ObserversRpc]
	private void ObsSyncSeed(int seed)
	{
		this.RpcWriter___Observers_ObsSyncSeed_3316948804(seed);
	}

	// Token: 0x06001483 RID: 5251 RVA: 0x00055D59 File Offset: 0x00053F59
	private IEnumerator AwaitSeedSync()
	{
		while (!this.seedSynced)
		{
			yield return null;
		}
		this.random = new Random(this.seedval);
		this.Generate();
		yield break;
	}

	// Token: 0x06001484 RID: 5252 RVA: 0x00055D68 File Offset: 0x00053F68
	private void Generate()
	{
		this.grid = new Grid2D<Generator2D.CellType>(this.size, Vector2Int.zero);
		this.rooms = new List<Generator2D.Room>();
		base.StartCoroutine(this.GenerateRoutine());
	}

	// Token: 0x06001485 RID: 5253 RVA: 0x00055D98 File Offset: 0x00053F98
	private IEnumerator GenerateRoutine()
	{
		this.PlaceRooms();
		while (!this.roomsplaced)
		{
			Debug.Log("room");
			yield return null;
		}
		yield return null;
		this.Triangulate();
		while (!this.triangualted)
		{
			Debug.Log("tri");
			yield return null;
		}
		yield return null;
		this.CreateHallways();
		while (!this.createdhallways)
		{
			Debug.Log("hall");
			yield return null;
		}
		yield return null;
		this.PathfindHallways();
		while (!this.pathfoundhallways)
		{
			Debug.Log("phall");
			yield return null;
		}
		yield return null;
		base.StartCoroutine(this.SetRoomsAndScale());
		yield break;
	}

	// Token: 0x06001486 RID: 5254 RVA: 0x00055DA8 File Offset: 0x00053FA8
	private void PlaceRooms()
	{
		Generator2D.Room room = new Generator2D.Room(new Vector2Int(this.size.x / 2, this.size.y / 2), new Vector2Int(2, 2));
		this.rooms.Add(room);
		this.PlaceRoom(room.bounds.position, room.bounds.size, true);
		foreach (Vector2Int vector2Int in room.bounds.allPositionsWithin)
		{
			this.grid[vector2Int] = Generator2D.CellType.Room;
		}
		for (int i = 0; i < this.roomCount; i++)
		{
			Vector2Int vector2Int2 = new Vector2Int(this.random.Next(4, this.size.x - 4), this.random.Next(4, this.size.y - 4));
			Vector2Int vector2Int3 = new Vector2Int(2, this.random.Next(2, this.roomMaxSize.y + 1));
			bool flag = true;
			Generator2D.Room room2 = new Generator2D.Room(vector2Int2, vector2Int3);
			Generator2D.Room room3 = new Generator2D.Room(vector2Int2 + new Vector2Int(-1, -1), vector2Int3 + new Vector2Int(2, 2));
			using (List<Generator2D.Room>.Enumerator enumerator = this.rooms.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (Generator2D.Room.Intersect(enumerator.Current, room3))
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				this.rooms.Add(room2);
				this.PlaceRoom(room2.bounds.position, room2.bounds.size, false);
				foreach (Vector2Int vector2Int4 in room2.bounds.allPositionsWithin)
				{
					this.grid[vector2Int4] = Generator2D.CellType.Room;
				}
			}
		}
		this.roomsplaced = true;
	}

	// Token: 0x06001487 RID: 5255 RVA: 0x00055FD4 File Offset: 0x000541D4
	private void Triangulate()
	{
		List<Vertex> list = new List<Vertex>();
		foreach (Generator2D.Room room in this.rooms)
		{
			list.Add(new Vertex<Generator2D.Room>(room.bounds.position + room.bounds.size / 2f, room));
		}
		this.delaunay = Delaunay2D.Triangulate(list);
		this.triangualted = true;
	}

	// Token: 0x06001488 RID: 5256 RVA: 0x0005607C File Offset: 0x0005427C
	private void CreateHallways()
	{
		List<Prim.Edge> list = new List<Prim.Edge>();
		foreach (Delaunay2D.Edge edge in this.delaunay.Edges)
		{
			list.Add(new Prim.Edge(edge.U, edge.V));
		}
		List<Prim.Edge> list2 = Prim.MinimumSpanningTree(list, list[0].U);
		this.selectedEdges = new HashSet<Prim.Edge>(list2);
		HashSet<Prim.Edge> hashSet = new HashSet<Prim.Edge>(list);
		hashSet.ExceptWith(this.selectedEdges);
		foreach (Prim.Edge edge2 in hashSet)
		{
			if (this.random.NextDouble() < 0.125)
			{
				this.selectedEdges.Add(edge2);
			}
		}
		this.createdhallways = true;
	}

	// Token: 0x06001489 RID: 5257 RVA: 0x00056180 File Offset: 0x00054380
	private void PathfindHallways()
	{
		DungeonPathfinder2D dungeonPathfinder2D = new DungeonPathfinder2D(this.size);
		foreach (Prim.Edge edge in this.selectedEdges)
		{
			Generator2D.Room item = (edge.U as Vertex<Generator2D.Room>).Item;
			Generator2D.Room item2 = (edge.V as Vertex<Generator2D.Room>).Item;
			Vector2 center = item.bounds.center;
			Vector2 center2 = item2.bounds.center;
			Vector2Int vector2Int = new Vector2Int((int)center.x, (int)center.y);
			Vector2Int endPos = new Vector2Int((int)center2.x, (int)center2.y);
			List<Vector2Int> list = dungeonPathfinder2D.FindPath(vector2Int, endPos, delegate(DungeonPathfinder2D.Node a, DungeonPathfinder2D.Node b)
			{
				DungeonPathfinder2D.PathCost pathCost = default(DungeonPathfinder2D.PathCost);
				pathCost.cost = Vector2Int.Distance(b.Position, endPos);
				if (this.grid[b.Position] == Generator2D.CellType.Room)
				{
					pathCost.cost += 6f;
				}
				else if (this.grid[b.Position] == Generator2D.CellType.None)
				{
					pathCost.cost += 1f;
				}
				else if (this.grid[b.Position] == Generator2D.CellType.Hallway)
				{
					pathCost.cost += 0f;
				}
				pathCost.traversable = true;
				return pathCost;
			});
			if (list == null || list.Count == 0)
			{
				Debug.LogWarning(string.Format("NO PATH from {0} to {1} - Skipping hallway.", vector2Int, endPos));
			}
			else
			{
				Debug.Log(string.Format("Found path from {0} to {1} with {2} steps.", vector2Int, endPos, list.Count));
				for (int i = 0; i < list.Count; i++)
				{
					Vector2Int vector2Int2 = list[i];
					if (this.grid[vector2Int2] == Generator2D.CellType.None)
					{
						this.grid[vector2Int2] = Generator2D.CellType.Hallway;
					}
					if (i > 0)
					{
						Vector2Int vector2Int3 = list[i - 1];
						vector2Int2 - vector2Int3;
					}
				}
				foreach (Vector2Int vector2Int4 in list)
				{
					if (this.grid[vector2Int4] == Generator2D.CellType.Hallway)
					{
						this.PlaceHallway(vector2Int4);
					}
				}
			}
		}
		this.pathfoundhallways = true;
	}

	// Token: 0x0600148A RID: 5258 RVA: 0x0005639C File Offset: 0x0005459C
	private void PlaceRoom(Vector2Int location, Vector2Int size, bool isEntrance)
	{
		if (isEntrance)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.SpawnRoom, new Vector3((float)location.x, 0f, (float)location.y), Quaternion.identity, this.parentScale);
			this.FrontDoor.EntrancePoint = gameObject.GetComponent<GetEntrancePoint>().EntrancePoint;
			return;
		}
		if (size == new Vector2Int(2, 2))
		{
			Object.Instantiate<GameObject>(this.room2x2[this.random.Next(0, this.room2x2.Length)], new Vector3((float)location.x, 0f, (float)location.y), Quaternion.identity, this.parentScale);
			return;
		}
		if (size == new Vector2Int(2, 3))
		{
			Object.Instantiate<GameObject>(this.room2x3[this.random.Next(0, this.room2x3.Length)], new Vector3((float)location.x, 0f, (float)location.y), Quaternion.identity, this.parentScale);
			return;
		}
		if (size == new Vector2Int(3, 2))
		{
			Object.Instantiate<GameObject>(this.room3x2[this.random.Next(0, this.room3x2.Length)], new Vector3((float)location.x, 0f, (float)location.y), Quaternion.identity, this.parentScale);
			return;
		}
		if (size == new Vector2Int(3, 3))
		{
			Object.Instantiate<GameObject>(this.room3x3[this.random.Next(0, this.room3x3.Length)], new Vector3((float)location.x, 0f, (float)location.y), Quaternion.identity, this.parentScale);
		}
	}

	// Token: 0x0600148B RID: 5259 RVA: 0x0005654C File Offset: 0x0005474C
	private void PlaceHallway(Vector2Int location)
	{
		if (!this.IsLocationOccupied(location))
		{
			Object.Instantiate<GameObject>(this.HallwayPrefab, new Vector3((float)location.x, 0f, (float)location.y), Quaternion.identity, this.parentScale).tag = "hallway";
		}
	}

	// Token: 0x0600148C RID: 5260 RVA: 0x0005659C File Offset: 0x0005479C
	private bool IsLocationOccupied(Vector2Int location)
	{
		Vector3 vector = new Vector3((float)location.x + 0.5f, 0f, (float)location.y + 0.5f);
		Vector3 vector2 = new Vector3(0.1f, 0.1f, 0.1f);
		return Physics.OverlapBox(vector, vector2, Quaternion.identity, this.ground).Length != 0;
	}

	// Token: 0x0600148D RID: 5261 RVA: 0x000565FF File Offset: 0x000547FF
	private IEnumerator SetRoomsAndScale()
	{
		yield return new WaitForSeconds(1.5f);
		this.parentScale.localScale = new Vector3(7f, 7f, 7f);
		this.plt = GameObject.FindGameObjectWithTag("NetItemManager").GetComponent<PageLootTable>();
		yield return new WaitForSeconds(0.1f);
		for (int i = 0; i < this.parentScale.childCount; i++)
		{
			RoomEnabler roomEnabler;
			RoomPrefabEnabler roomPrefabEnabler;
			if (this.parentScale.GetChild(i).TryGetComponent<RoomEnabler>(out roomEnabler))
			{
				roomEnabler.CheckBorders();
				int num = this.random.Next(0, 7);
				roomEnabler.RandomizeHallway(this.random.Next(0, 2), this.random.Next(0, 5), this.random.Next(0, 5), num, this.random.Next(0, 14));
				if (base.HasAuthority && (num == 1 || num == 2) && this.numberOfLoots < 8 && Random.Range(0, 4) != 1)
				{
					this.PlaceItemIn(Random.Range(0, this.plt.Pages.Length), roomEnabler.otherShit[num].transform.position);
				}
			}
			else if (this.parentScale.GetChild(i).TryGetComponent<RoomPrefabEnabler>(out roomPrefabEnabler))
			{
				roomPrefabEnabler.CheckBorder();
			}
		}
		for (int j = 1; j < this.parentScale.childCount - 1; j++)
		{
			RoomEnabler roomEnabler2;
			RoomEnabler roomEnabler3;
			RoomEnabler roomEnabler4;
			if (this.parentScale.GetChild(j).TryGetComponent<RoomEnabler>(out roomEnabler2) && this.parentScale.GetChild(j - 1).TryGetComponent<RoomEnabler>(out roomEnabler3) && this.parentScale.GetChild(j + 1).TryGetComponent<RoomEnabler>(out roomEnabler4) && roomEnabler2.isCorner && roomEnabler3.isCorner && roomEnabler4.isCorner)
			{
				roomEnabler2.makeDiagonal();
			}
		}
		this.BakeDungeonNavMesh();
		for (int k = 0; k < this.parentScale.childCount; k++)
		{
			RoomPrefabEnabler roomPrefabEnabler2;
			if (this.parentScale.GetChild(k).TryGetComponent<RoomPrefabEnabler>(out roomPrefabEnabler2))
			{
				roomPrefabEnabler2.CloseDoors();
			}
		}
		this.toggledungeonmeshrenderers.DisableAll();
		this.gamemanager.GetComponent<DungeonGenerator>().isDungeonGenerated = true;
		yield return new WaitForSeconds(10f);
		if (base.HasAuthority)
		{
			this.ServerSpawnSkeleMage();
		}
		yield break;
	}

	// Token: 0x0600148E RID: 5262 RVA: 0x0005660E File Offset: 0x0005480E
	public void PlaceItemIn(int itemid, Vector3 pos)
	{
		this.ServerPlaceItem(itemid, pos);
	}

	// Token: 0x0600148F RID: 5263 RVA: 0x00056618 File Offset: 0x00054818
	[ServerRpc(RequireOwnership = false)]
	private void ServerPlaceItem(int itemid, Vector3 pos)
	{
		this.RpcWriter___Server_ServerPlaceItem_215135683(itemid, pos);
	}

	// Token: 0x06001490 RID: 5264 RVA: 0x00056634 File Offset: 0x00054834
	[ServerRpc(RequireOwnership = false)]
	private void ServerSpawnSkeleMage()
	{
		this.RpcWriter___Server_ServerSpawnSkeleMage_2166136261();
	}

	// Token: 0x06001491 RID: 5265 RVA: 0x000021EF File Offset: 0x000003EF
	private void BakeDungeonNavMesh()
	{
	}

	// Token: 0x06001493 RID: 5267 RVA: 0x00056648 File Offset: 0x00054848
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyGenerator2DAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyGenerator2DAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ServerRequestSeed_2166136261));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_ObsRequestSeed_2166136261));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_ServerSyncSeed_3316948804));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ObsSyncSeed_3316948804));
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_ServerPlaceItem_215135683));
		base.RegisterServerRpc(5U, new ServerRpcDelegate(this.RpcReader___Server_ServerSpawnSkeleMage_2166136261));
	}

	// Token: 0x06001494 RID: 5268 RVA: 0x000566F0 File Offset: 0x000548F0
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateGenerator2DAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateGenerator2DAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06001495 RID: 5269 RVA: 0x00056703 File Offset: 0x00054903
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06001496 RID: 5270 RVA: 0x00056714 File Offset: 0x00054914
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

	// Token: 0x06001497 RID: 5271 RVA: 0x00056779 File Offset: 0x00054979
	private void RpcLogic___ServerRequestSeed_2166136261()
	{
		this.ObsRequestSeed();
	}

	// Token: 0x06001498 RID: 5272 RVA: 0x00056784 File Offset: 0x00054984
	private void RpcReader___Server_ServerRequestSeed_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerRequestSeed_2166136261();
	}

	// Token: 0x06001499 RID: 5273 RVA: 0x000567A4 File Offset: 0x000549A4
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

	// Token: 0x0600149A RID: 5274 RVA: 0x00056818 File Offset: 0x00054A18
	private void RpcLogic___ObsRequestSeed_2166136261()
	{
		this.SeedRequested();
	}

	// Token: 0x0600149B RID: 5275 RVA: 0x00056820 File Offset: 0x00054A20
	private void RpcReader___Observers_ObsRequestSeed_2166136261(PooledReader PooledReader0, Channel channel)
	{
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsRequestSeed_2166136261();
	}

	// Token: 0x0600149C RID: 5276 RVA: 0x00056840 File Offset: 0x00054A40
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

	// Token: 0x0600149D RID: 5277 RVA: 0x000568B2 File Offset: 0x00054AB2
	private void RpcLogic___ServerSyncSeed_3316948804(int seed)
	{
		this.ObsSyncSeed(seed);
	}

	// Token: 0x0600149E RID: 5278 RVA: 0x000568BC File Offset: 0x00054ABC
	private void RpcReader___Server_ServerSyncSeed_3316948804(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSyncSeed_3316948804(num);
	}

	// Token: 0x0600149F RID: 5279 RVA: 0x000568F0 File Offset: 0x00054AF0
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

	// Token: 0x060014A0 RID: 5280 RVA: 0x00056971 File Offset: 0x00054B71
	private void RpcLogic___ObsSyncSeed_3316948804(int seed)
	{
		this.seedval = seed;
		this.seedSynced = true;
	}

	// Token: 0x060014A1 RID: 5281 RVA: 0x00056984 File Offset: 0x00054B84
	private void RpcReader___Observers_ObsSyncSeed_3316948804(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsSyncSeed_3316948804(num);
	}

	// Token: 0x060014A2 RID: 5282 RVA: 0x000569B8 File Offset: 0x00054BB8
	private void RpcWriter___Server_ServerPlaceItem_215135683(int itemid, Vector3 pos)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(itemid);
		pooledWriter.WriteVector3(pos);
		base.SendServerRpc(4U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060014A3 RID: 5283 RVA: 0x00056A38 File Offset: 0x00054C38
	private void RpcLogic___ServerPlaceItem_215135683(int itemid, Vector3 pos)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.plt.Pages[itemid]);
		gameObject.transform.position = pos;
		gameObject.transform.localRotation = Quaternion.Euler(10f, 0f, 180f);
		base.ServerManager.Spawn(gameObject, null, default(Scene));
	}

	// Token: 0x060014A4 RID: 5284 RVA: 0x00056A9C File Offset: 0x00054C9C
	private void RpcReader___Server_ServerPlaceItem_215135683(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerPlaceItem_215135683(num, vector);
	}

	// Token: 0x060014A5 RID: 5285 RVA: 0x00056AE0 File Offset: 0x00054CE0
	private void RpcWriter___Server_ServerSpawnSkeleMage_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(5U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060014A6 RID: 5286 RVA: 0x00056B48 File Offset: 0x00054D48
	private void RpcLogic___ServerSpawnSkeleMage_2166136261()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("ainodes");
		if (array.Length != 0)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.SkeleMage, array[this.random.Next(0, array.Length)].transform.position, Quaternion.identity);
			gameObject.transform.localScale = new Vector3(1.75f, 1.75f, 1.75f);
			base.ServerManager.Spawn(gameObject, null, default(Scene));
			GameObject gameObject2 = null;
			GameObject gameObject3 = null;
			float num = 0f;
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = i + 1; j < array.Length; j++)
				{
					float num2 = Vector3.Distance(array[i].transform.position, array[j].transform.position);
					if (num2 > num)
					{
						num = num2;
						gameObject2 = array[i];
						gameObject3 = array[j];
					}
				}
			}
			GameObject gameObject4 = Object.Instantiate<GameObject>(this.HiltRock, gameObject2.transform.position, Quaternion.identity);
			base.ServerManager.Spawn(gameObject4, null, default(Scene));
			GameObject gameObject5 = Object.Instantiate<GameObject>(this.BladeRock, gameObject3.transform.position, Quaternion.identity);
			base.ServerManager.Spawn(gameObject5, null, default(Scene));
			return;
		}
		Debug.Log("ruh roh raggy");
	}

	// Token: 0x060014A7 RID: 5287 RVA: 0x00056CA8 File Offset: 0x00054EA8
	private void RpcReader___Server_ServerSpawnSkeleMage_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerSpawnSkeleMage_2166136261();
	}

	// Token: 0x060014A8 RID: 5288 RVA: 0x00056703 File Offset: 0x00054903
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000BF5 RID: 3061
	[SerializeField]
	private Vector2Int size;

	// Token: 0x04000BF6 RID: 3062
	[SerializeField]
	private int roomCount;

	// Token: 0x04000BF7 RID: 3063
	[SerializeField]
	private Vector2Int roomMaxSize;

	// Token: 0x04000BF8 RID: 3064
	[SerializeField]
	private Transform parentScale;

	// Token: 0x04000BF9 RID: 3065
	public GameObject HallwayPrefab;

	// Token: 0x04000BFA RID: 3066
	private Random random;

	// Token: 0x04000BFB RID: 3067
	private Grid2D<Generator2D.CellType> grid;

	// Token: 0x04000BFC RID: 3068
	private List<Generator2D.Room> rooms;

	// Token: 0x04000BFD RID: 3069
	private Delaunay2D delaunay;

	// Token: 0x04000BFE RID: 3070
	private HashSet<Prim.Edge> selectedEdges;

	// Token: 0x04000BFF RID: 3071
	public GameObject[] room2x2;

	// Token: 0x04000C00 RID: 3072
	public GameObject[] room2x3;

	// Token: 0x04000C01 RID: 3073
	public GameObject[] room3x2;

	// Token: 0x04000C02 RID: 3074
	public GameObject[] room3x3;

	// Token: 0x04000C03 RID: 3075
	public GameObject SpawnRoom;

	// Token: 0x04000C04 RID: 3076
	public DoorInteract FrontDoor;

	// Token: 0x04000C05 RID: 3077
	public GameObject SkeleMage;

	// Token: 0x04000C06 RID: 3078
	public GameObject BladeRock;

	// Token: 0x04000C07 RID: 3079
	public GameObject HiltRock;

	// Token: 0x04000C08 RID: 3080
	private GameObject gamemanager;

	// Token: 0x04000C09 RID: 3081
	public int seedval;

	// Token: 0x04000C0A RID: 3082
	private bool seedSynced;

	// Token: 0x04000C0B RID: 3083
	private bool hasRetried;

	// Token: 0x04000C0C RID: 3084
	public ToggleDungeonMeshRenders toggledungeonmeshrenderers;

	// Token: 0x04000C0D RID: 3085
	private bool roomsplaced;

	// Token: 0x04000C0E RID: 3086
	private bool triangualted;

	// Token: 0x04000C0F RID: 3087
	private bool createdhallways;

	// Token: 0x04000C10 RID: 3088
	private bool pathfoundhallways;

	// Token: 0x04000C11 RID: 3089
	public LayerMask ground;

	// Token: 0x04000C12 RID: 3090
	private int numberOfLoots;

	// Token: 0x04000C13 RID: 3091
	private PageLootTable plt;

	// Token: 0x04000C14 RID: 3092
	private bool NetworkInitialize___EarlyGenerator2DAssembly-CSharp.dll_Excuted;

	// Token: 0x04000C15 RID: 3093
	private bool NetworkInitialize__LateGenerator2DAssembly-CSharp.dll_Excuted;

	// Token: 0x02000201 RID: 513
	private enum CellType
	{
		// Token: 0x04000C17 RID: 3095
		None,
		// Token: 0x04000C18 RID: 3096
		Room,
		// Token: 0x04000C19 RID: 3097
		Hallway
	}

	// Token: 0x02000202 RID: 514
	private class Room
	{
		// Token: 0x060014A9 RID: 5289 RVA: 0x00056CC8 File Offset: 0x00054EC8
		public Room(Vector2Int location, Vector2Int size)
		{
			this.bounds = new RectInt(location, size);
		}

		// Token: 0x060014AA RID: 5290 RVA: 0x00056CE0 File Offset: 0x00054EE0
		public static bool Intersect(Generator2D.Room a, Generator2D.Room b)
		{
			return a.bounds.position.x < b.bounds.position.x + b.bounds.size.x && a.bounds.position.x + a.bounds.size.x > b.bounds.position.x && a.bounds.position.y < b.bounds.position.y + b.bounds.size.y && a.bounds.position.y + a.bounds.size.y > b.bounds.position.y;
		}

		// Token: 0x04000C1A RID: 3098
		public RectInt bounds;
	}
}
