using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001E3 RID: 483
public class AINodes : MonoBehaviour
{
	// Token: 0x06001399 RID: 5017 RVA: 0x00052759 File Offset: 0x00050959
	public void AddPosition(Vector3 Pos)
	{
		this.NodePositions.Add(Pos);
	}

	// Token: 0x0600139A RID: 5018 RVA: 0x00052767 File Offset: 0x00050967
	public void RemovePosition(Vector3 Pos)
	{
		this.NodePositions.Remove(Pos);
	}

	// Token: 0x0600139B RID: 5019 RVA: 0x00052776 File Offset: 0x00050976
	public int GetNodeCount()
	{
		return this.NodePositions.Count;
	}

	// Token: 0x0600139C RID: 5020 RVA: 0x00052783 File Offset: 0x00050983
	public Vector3 GetPositionAtIndex(int ind)
	{
		if (ind < this.NodePositions.Count && ind >= 0)
		{
			return this.NodePositions[ind];
		}
		return Vector3.zero;
	}

	// Token: 0x04000B7B RID: 2939
	public List<Vector3> NodePositions = new List<Vector3>();
}
