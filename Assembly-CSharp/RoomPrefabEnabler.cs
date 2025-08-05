using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001F7 RID: 503
public class RoomPrefabEnabler : MonoBehaviour
{
	// Token: 0x0600143C RID: 5180 RVA: 0x00054D4D File Offset: 0x00052F4D
	private void Start()
	{
		this.NetItemManager = GameObject.FindGameObjectWithTag("NetItemManager").GetComponent<NetInteractionManager>();
	}

	// Token: 0x0600143D RID: 5181 RVA: 0x00054D64 File Offset: 0x00052F64
	public void CheckBorder()
	{
		int num = 0;
		foreach (Transform transform in this.Checkpoints)
		{
			if (this.IsLocationOccupied(transform.position) && !this.SetDoorIds.Contains(this.checkpointWallIds[num]))
			{
				this.SetDoorIds.Add(this.checkpointWallIds[num]);
			}
			else
			{
				this.Doorways[num].SetActive(false);
				this.Walls[num].SetActive(true);
			}
			num++;
		}
	}

	// Token: 0x0600143E RID: 5182 RVA: 0x00054DE4 File Offset: 0x00052FE4
	public void CloseDoors()
	{
		foreach (DoorOpen doorOpen in this.doors)
		{
			if (doorOpen.isActiveAndEnabled)
			{
				doorOpen.initDoor();
			}
		}
		GameObject[] lateInitObjects = this.LateInitObjects;
		for (int i = 0; i < lateInitObjects.Length; i++)
		{
			lateInitObjects[i].SetActive(true);
		}
		if (this.NetworkedMonsterPrefabsToSpawn.Length != 0)
		{
			for (int j = 0; j < this.NetworkedMonsterPrefabsToSpawn.Length; j++)
			{
				this.NetItemManager.SpawnPrefab(this.NetworkedMonsterSpawnpoint[j].position, this.NetworkedMonsterPrefabsToSpawn[j]);
			}
		}
	}

	// Token: 0x0600143F RID: 5183 RVA: 0x00054E78 File Offset: 0x00053078
	private bool IsLocationOccupied(Vector3 location)
	{
		Vector3 vector = new Vector3(0.3f, 0.3f, 0.3f);
		int num = 8;
		return Physics.OverlapBox(location, vector, Quaternion.identity, num).Length != 0;
	}

	// Token: 0x04000BD1 RID: 3025
	public Transform[] Checkpoints;

	// Token: 0x04000BD2 RID: 3026
	public GameObject[] Walls;

	// Token: 0x04000BD3 RID: 3027
	public GameObject[] Doorways;

	// Token: 0x04000BD4 RID: 3028
	public DoorOpen[] doors;

	// Token: 0x04000BD5 RID: 3029
	public GameObject[] LateInitObjects;

	// Token: 0x04000BD6 RID: 3030
	public GameObject[] NetworkedMonsterPrefabsToSpawn;

	// Token: 0x04000BD7 RID: 3031
	public Transform[] NetworkedMonsterSpawnpoint;

	// Token: 0x04000BD8 RID: 3032
	private NetInteractionManager NetItemManager;

	// Token: 0x04000BD9 RID: 3033
	public int[] checkpointWallIds;

	// Token: 0x04000BDA RID: 3034
	private List<int> SetDoorIds = new List<int>();
}
