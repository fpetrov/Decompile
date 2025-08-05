using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200014C RID: 332
public class RavenController : MonoBehaviour
{
	// Token: 0x06000E59 RID: 3673 RVA: 0x0003A720 File Offset: 0x00038920
	private void Start()
	{
		this.RavenSpawnPoint = GameObject.FindGameObjectWithTag("RavenPoint").transform.position;
		this.RavenSpawnPoint = new Vector3(this.RavenSpawnPoint.x + (float)Random.Range(-500, 500), this.RavenSpawnPoint.y, this.RavenSpawnPoint.z + (float)Random.Range(-500, 500));
		base.StartCoroutine(this.RavenRoutine());
		if (this.rspawn.checkAuthority())
		{
			base.StartCoroutine(this.RavenSounds());
		}
	}

	// Token: 0x06000E5A RID: 3674 RVA: 0x0003A7BC File Offset: 0x000389BC
	private IEnumerator RavenSounds()
	{
		while (base.isActiveAndEnabled && this.rspawn != null)
		{
			this.rspawn.TriggerSound(this.RavenID);
			yield return new WaitForSeconds((float)Random.Range(7, 30));
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x06000E5B RID: 3675 RVA: 0x0003A7CB File Offset: 0x000389CB
	private IEnumerator RavenRoutine()
	{
		while (base.transform.position != this.LandPoint && !this.flyaway && base.isActiveAndEnabled)
		{
			base.transform.position = Vector3.Lerp(this.RavenSpawnPoint, this.LandPoint, this.timer / (Vector3.Distance(this.RavenSpawnPoint, this.LandPoint) / 20f));
			this.timer += Time.deltaTime;
			base.transform.LookAt(this.LandPoint);
			yield return null;
		}
		base.transform.rotation = Quaternion.Euler(0f, (float)Random.Range(0, 360), 0f);
		this.RavenAni.SetBool("idle", true);
		yield break;
	}

	// Token: 0x06000E5C RID: 3676 RVA: 0x0003A7DC File Offset: 0x000389DC
	private void OnTriggerEnter(Collider other)
	{
		if (this.rspawn != null && this.rspawn.isActiveAndEnabled)
		{
			PlayerInventory playerInventory;
			if (other.CompareTag("Player") && !this.flyaway && other.gameObject.TryGetComponent<PlayerInventory>(out playerInventory))
			{
				this.rspawn.TriggerFlyAway(this.RavenID);
				return;
			}
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000E5D RID: 3677 RVA: 0x0003A845 File Offset: 0x00038A45
	public void StartFlyAwayr()
	{
		base.StartCoroutine(this.RavenFlyAway());
	}

	// Token: 0x06000E5E RID: 3678 RVA: 0x0003A854 File Offset: 0x00038A54
	private IEnumerator RavenFlyAway()
	{
		this.flyaway = true;
		this.timer = 0f;
		this.RavenAni.SetBool("fly", true);
		this.ravso.pitch = Random.Range(0.95f, 1.05f);
		this.ravso.PlayOneShot(this.ravcl[Random.Range(0, this.ravcl.Length)]);
		while (base.transform.position != this.RavenSpawnPoint && base.isActiveAndEnabled)
		{
			base.transform.position = Vector3.Lerp(this.LandPoint, this.RavenSpawnPoint, this.timer / (Vector3.Distance(this.RavenSpawnPoint, this.LandPoint) / 20f));
			this.timer += Time.deltaTime;
			base.transform.LookAt(this.RavenSpawnPoint);
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x06000E5F RID: 3679 RVA: 0x0003A863 File Offset: 0x00038A63
	public void PlaySound()
	{
		this.ravso.pitch = Random.Range(0.95f, 1.05f);
		this.ravso.PlayOneShot(this.ravcl[Random.Range(0, this.ravcl.Length)]);
	}

	// Token: 0x040007DF RID: 2015
	public Vector3 LandPoint;

	// Token: 0x040007E0 RID: 2016
	public int RavenID;

	// Token: 0x040007E1 RID: 2017
	public Animator RavenAni;

	// Token: 0x040007E2 RID: 2018
	private float timer;

	// Token: 0x040007E3 RID: 2019
	public Vector3 RavenSpawnPoint;

	// Token: 0x040007E4 RID: 2020
	public RavenSpawner rspawn;

	// Token: 0x040007E5 RID: 2021
	private bool flyaway;

	// Token: 0x040007E6 RID: 2022
	public AudioSource ravso;

	// Token: 0x040007E7 RID: 2023
	public AudioClip[] ravcl;
}
