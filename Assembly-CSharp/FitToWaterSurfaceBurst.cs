using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x0200016F RID: 367
[ExecuteInEditMode]
public class FitToWaterSurfaceBurst : MonoBehaviour
{
	// Token: 0x06000F2C RID: 3884 RVA: 0x0003D5B5 File Offset: 0x0003B7B5
	private void Start()
	{
		this.boxCollider = base.GetComponent<BoxCollider>();
		this.Reset();
	}

	// Token: 0x06000F2D RID: 3885 RVA: 0x0003D5CC File Offset: 0x0003B7CC
	private void Reset()
	{
		this.OnDestroy();
		this.targetPositionBuffer = new NativeArray<float3>(this.count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.errorBuffer = new NativeArray<float>(this.count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.candidatePositionBuffer = new NativeArray<float3>(this.count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.projectedPositionWSBuffer = new NativeArray<float3>(this.count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.stepCountBuffer = new NativeArray<int>(this.count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.directionBuffer = new NativeArray<float3>(this.count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.prefabList = new List<GameObject>();
		this.prefabList.Clear();
		for (int i = base.transform.childCount; i > 0; i--)
		{
			FitToWaterSurfaceBurst.SmartDestroy(base.transform.GetChild(0).gameObject);
		}
		for (int j = 0; j < this.count; j++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.prefab);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = this.RandomPointInBounds(base.GetComponent<Collider>().bounds) - base.transform.position;
			gameObject.transform.localEulerAngles = new Vector3(-180f, (float)global::UnityEngine.Random.Range(0, 360), 0f);
			this.prefabList.Add(gameObject);
		}
	}

	// Token: 0x06000F2E RID: 3886 RVA: 0x0003D724 File Offset: 0x0003B924
	private void Update()
	{
		if (this.waterSurface == null)
		{
			return;
		}
		if (!this.targetPositionBuffer.IsCreated)
		{
			this.Reset();
		}
		WaterSimSearchData waterSimSearchData = default(WaterSimSearchData);
		if (!this.waterSurface.FillWaterSearchData(ref waterSimSearchData))
		{
			return;
		}
		for (int i = 0; i < this.prefabList.Count; i++)
		{
			this.targetPositionBuffer[i] = this.prefabList[i].transform.position;
		}
		new WaterSimulationSearchJob
		{
			simSearchData = waterSimSearchData,
			targetPositionWSBuffer = this.targetPositionBuffer,
			startPositionWSBuffer = this.targetPositionBuffer,
			maxIterations = 8,
			error = 0.01f,
			includeDeformation = this.includeDeformation,
			excludeSimulation = false,
			projectedPositionWSBuffer = this.projectedPositionWSBuffer,
			errorBuffer = this.errorBuffer,
			candidateLocationWSBuffer = this.candidatePositionBuffer,
			directionBuffer = this.directionBuffer,
			stepCountBuffer = this.stepCountBuffer
		}.Schedule(this.count, 1, default(JobHandle)).Complete();
		for (int j = 0; j < this.prefabList.Count; j++)
		{
			float3 @float = this.projectedPositionWSBuffer[j];
			this.prefabList[j].transform.position = @float + Time.deltaTime * this.directionBuffer[j] * this.currentSpeedMultiplier;
		}
	}

	// Token: 0x06000F2F RID: 3887 RVA: 0x0003D8CC File Offset: 0x0003BACC
	private Vector3 RandomPointInBounds(Bounds bounds)
	{
		return new Vector3(global::UnityEngine.Random.Range(bounds.min.x, bounds.max.x), global::UnityEngine.Random.Range(bounds.min.y, bounds.max.y), global::UnityEngine.Random.Range(bounds.min.z, bounds.max.z));
	}

	// Token: 0x06000F30 RID: 3888 RVA: 0x0003D938 File Offset: 0x0003BB38
	private void OnDestroy()
	{
		if (this.directionBuffer.IsCreated)
		{
			this.directionBuffer.Dispose();
		}
		if (this.targetPositionBuffer.IsCreated)
		{
			this.targetPositionBuffer.Dispose();
		}
		if (this.errorBuffer.IsCreated)
		{
			this.errorBuffer.Dispose();
		}
		if (this.candidatePositionBuffer.IsCreated)
		{
			this.candidatePositionBuffer.Dispose();
		}
		if (this.stepCountBuffer.IsCreated)
		{
			this.stepCountBuffer.Dispose();
		}
	}

	// Token: 0x06000F31 RID: 3889 RVA: 0x0003D9BD File Offset: 0x0003BBBD
	private void OnDisable()
	{
		this.OnDestroy();
	}

	// Token: 0x06000F32 RID: 3890 RVA: 0x0003D9C5 File Offset: 0x0003BBC5
	public static void SmartDestroy(Object obj)
	{
		if (obj == null)
		{
			return;
		}
		Object.Destroy(obj);
	}

	// Token: 0x04000892 RID: 2194
	public int count = 50;

	// Token: 0x04000893 RID: 2195
	public WaterSurface waterSurface;

	// Token: 0x04000894 RID: 2196
	public bool includeDeformation = true;

	// Token: 0x04000895 RID: 2197
	public float currentSpeedMultiplier = 1f;

	// Token: 0x04000896 RID: 2198
	public GameObject prefab;

	// Token: 0x04000897 RID: 2199
	private List<GameObject> prefabList;

	// Token: 0x04000898 RID: 2200
	private BoxCollider boxCollider;

	// Token: 0x04000899 RID: 2201
	private NativeArray<float3> targetPositionBuffer;

	// Token: 0x0400089A RID: 2202
	private NativeArray<float> errorBuffer;

	// Token: 0x0400089B RID: 2203
	private NativeArray<float3> candidatePositionBuffer;

	// Token: 0x0400089C RID: 2204
	private NativeArray<float3> projectedPositionWSBuffer;

	// Token: 0x0400089D RID: 2205
	private NativeArray<float3> directionBuffer;

	// Token: 0x0400089E RID: 2206
	private NativeArray<int> stepCountBuffer;
}
