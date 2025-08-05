using System;
using UnityEngine;

// Token: 0x020001F6 RID: 502
public class RoomEnabler : MonoBehaviour
{
	// Token: 0x06001436 RID: 5174 RVA: 0x000542B8 File Offset: 0x000524B8
	public void CheckBorders()
	{
		this.borders[0] = this.IsLocationOccupied(this.posx.position);
		this.borders[1] = this.IsLocationOccupied(this.posz.position);
		this.borders[2] = this.IsLocationOccupied(this.negx.position);
		this.borders[3] = this.IsLocationOccupied(this.negz.position);
		this.isCorner = this.Enabler();
	}

	// Token: 0x06001437 RID: 5175 RVA: 0x00054338 File Offset: 0x00052538
	private bool IsLocationOccupied(Vector3 location)
	{
		Vector3 vector = new Vector3(0.1f, 0.1f, 0.1f);
		int num = 8;
		return Physics.OverlapBox(location, vector, Quaternion.identity, num).Length != 0;
	}

	// Token: 0x06001438 RID: 5176 RVA: 0x00054370 File Offset: 0x00052570
	private bool Enabler()
	{
		if (this.borders[0] && this.borders[1] && this.borders[2] && this.borders[3])
		{
			this.Decor.localRotation = Quaternion.Euler(0f, 45f, 0f);
			this.Decor.SetParent(this.rooms[0].transform);
			this.rooms[0].SetActive(true);
			return false;
		}
		if (this.borders[0] && this.borders[1] && this.borders[2] && !this.borders[3])
		{
			this.Decor.localRotation = Quaternion.Euler(0f, -90f, 0f);
			this.Decor.localPosition = new Vector3(this.Decor.localPosition.x + 1.7f, this.Decor.localPosition.y, this.Decor.localPosition.z);
			this.Decor.SetParent(this.rooms[1].transform);
			this.rooms[1].SetActive(true);
			this.rooms[1].transform.localRotation = Quaternion.Euler(-90f, 90f, -90f);
			return false;
		}
		if (this.borders[0] && this.borders[1] && !this.borders[2] && this.borders[3])
		{
			this.Decor.localRotation = Quaternion.Euler(0f, -90f, 0f);
			this.Decor.localPosition = new Vector3(this.Decor.localPosition.x + 1.7f, this.Decor.localPosition.y, this.Decor.localPosition.z);
			this.Decor.SetParent(this.rooms[1].transform);
			this.rooms[1].SetActive(true);
			this.rooms[1].transform.localRotation = Quaternion.Euler(-90f, 180f, -90f);
			return false;
		}
		if (this.borders[0] && this.borders[1] && !this.borders[2] && !this.borders[3])
		{
			this.Decor.localRotation = Quaternion.Euler(0f, -45f, 0f);
			this.Decor.localPosition = new Vector3(this.Decor.localPosition.x + 0.5f, this.Decor.localPosition.y, this.Decor.localPosition.z - 0.5f);
			this.Decor.SetParent(this.rooms[2].transform);
			this.rooms[2].SetActive(true);
			this.rooms[2].transform.localRotation = Quaternion.Euler(-90f, 90f, -90f);
			this.rooms[3].transform.localRotation = Quaternion.Euler(-90f, 90f, -90f);
			return true;
		}
		if (this.borders[0] && !this.borders[1] && this.borders[2] && this.borders[3])
		{
			this.Decor.localRotation = Quaternion.Euler(0f, -90f, 0f);
			this.Decor.localPosition = new Vector3(this.Decor.localPosition.x + 1.7f, this.Decor.localPosition.y, this.Decor.localPosition.z);
			this.Decor.SetParent(this.rooms[1].transform);
			this.rooms[1].SetActive(true);
			this.rooms[1].transform.localRotation = Quaternion.Euler(-90f, -90f, -90f);
			return false;
		}
		if (this.borders[0] && !this.borders[1] && this.borders[2] && !this.borders[3])
		{
			this.Decor.SetParent(this.rooms[4].transform);
			this.rooms[4].SetActive(true);
			this.rooms[4].transform.localRotation = Quaternion.Euler(-90f, 90f, -90f);
			return false;
		}
		if (this.borders[0] && !this.borders[1] && !this.borders[2] && this.borders[3])
		{
			this.Decor.localRotation = Quaternion.Euler(0f, -45f, 0f);
			this.Decor.localPosition = new Vector3(this.Decor.localPosition.x + 0.5f, this.Decor.localPosition.y, this.Decor.localPosition.z - 0.5f);
			this.Decor.SetParent(this.rooms[2].transform);
			this.rooms[2].SetActive(true);
			this.rooms[2].transform.localRotation = Quaternion.Euler(-90f, 180f, -90f);
			this.rooms[3].transform.localRotation = Quaternion.Euler(-90f, 180f, -90f);
			return true;
		}
		if (this.borders[0] && !this.borders[1] && !this.borders[2] && !this.borders[3])
		{
			return false;
		}
		if (!this.borders[0] && this.borders[1] && this.borders[2] && this.borders[3])
		{
			this.Decor.localPosition = new Vector3(this.Decor.localPosition.x + 1.7f, this.Decor.localPosition.y, this.Decor.localPosition.z);
			this.Decor.localRotation = Quaternion.Euler(0f, -90f, 0f);
			this.Decor.SetParent(this.rooms[1].transform);
			this.rooms[1].SetActive(true);
			return false;
		}
		if (!this.borders[0] && this.borders[1] && this.borders[2] && !this.borders[3])
		{
			this.Decor.localRotation = Quaternion.Euler(0f, -45f, 0f);
			this.Decor.localPosition = new Vector3(this.Decor.localPosition.x + 0.5f, this.Decor.localPosition.y, this.Decor.localPosition.z - 0.5f);
			this.Decor.SetParent(this.rooms[2].transform);
			this.rooms[2].SetActive(true);
			return true;
		}
		if (!this.borders[0] && this.borders[1] && !this.borders[2] && this.borders[3])
		{
			this.Decor.SetParent(this.rooms[4].transform);
			this.rooms[4].SetActive(true);
			return false;
		}
		if (!this.borders[0] && this.borders[1] && !this.borders[2] && !this.borders[3])
		{
			return false;
		}
		if (!this.borders[0] && !this.borders[1] && this.borders[2] && this.borders[3])
		{
			this.Decor.localRotation = Quaternion.Euler(0f, -45f, 0f);
			this.Decor.localPosition = new Vector3(this.Decor.localPosition.x + 0.5f, this.Decor.localPosition.y, this.Decor.localPosition.z - 0.5f);
			this.Decor.SetParent(this.rooms[2].transform);
			this.rooms[2].SetActive(true);
			this.rooms[2].transform.localRotation = Quaternion.Euler(-90f, -90f, -90f);
			this.rooms[3].transform.localRotation = Quaternion.Euler(-90f, -90f, -90f);
			return true;
		}
		if (!this.borders[0] && !this.borders[1] && this.borders[2] && !this.borders[3])
		{
			return false;
		}
		if (!this.borders[0] && !this.borders[1] && !this.borders[2])
		{
			bool flag = this.borders[3];
			return false;
		}
		return false;
	}

	// Token: 0x06001439 RID: 5177 RVA: 0x00054CD0 File Offset: 0x00052ED0
	public void makeDiagonal()
	{
		this.rooms[2].SetActive(false);
		this.rooms[3].SetActive(true);
		Object.Destroy(this.nodepoint);
	}

	// Token: 0x0600143A RID: 5178 RVA: 0x00054CF9 File Offset: 0x00052EF9
	public void RandomizeHallway(int pillar, int leftPillarVal, int RightPillarVal, int otherVal, int puddleval)
	{
		if (pillar == 1)
		{
			this.LeftPillar[leftPillarVal].SetActive(true);
			this.RightPillar[RightPillarVal].SetActive(true);
		}
		this.otherShit[otherVal].SetActive(true);
		this.puddles[puddleval].SetActive(true);
	}

	// Token: 0x04000BC4 RID: 3012
	public Transform posx;

	// Token: 0x04000BC5 RID: 3013
	public Transform negx;

	// Token: 0x04000BC6 RID: 3014
	public Transform posz;

	// Token: 0x04000BC7 RID: 3015
	public Transform negz;

	// Token: 0x04000BC8 RID: 3016
	private bool[] borders = new bool[4];

	// Token: 0x04000BC9 RID: 3017
	public GameObject[] rooms;

	// Token: 0x04000BCA RID: 3018
	public bool isCorner;

	// Token: 0x04000BCB RID: 3019
	public GameObject[] LeftPillar;

	// Token: 0x04000BCC RID: 3020
	public GameObject[] RightPillar;

	// Token: 0x04000BCD RID: 3021
	public GameObject[] otherShit;

	// Token: 0x04000BCE RID: 3022
	public GameObject[] puddles;

	// Token: 0x04000BCF RID: 3023
	public Transform Decor;

	// Token: 0x04000BD0 RID: 3024
	public GameObject nodepoint;
}
