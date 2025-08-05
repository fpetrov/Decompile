using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x020000C6 RID: 198
public class MageBookController : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x06000793 RID: 1939 RVA: 0x0001D01C File Offset: 0x0001B21C
	private void Start()
	{
		this.FBMat = this.PageRenderers[0].material;
		this.FrbMat = this.PageRenderers[1].material;
		this.WormMat = this.PageRenderers[2].material;
		this.WardAndHoleMat = this.PageRenderers[3].material;
		this.Sholder = GameObject.FindGameObjectWithTag("Settings").GetComponent<SettingsHolder>();
		Animator[] array = this.animaes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].speed = 1.1f;
		}
	}

	// Token: 0x06000794 RID: 1940 RVA: 0x0001D0AC File Offset: 0x0001B2AC
	private IEnumerator LerpEmis(Material mat, float targetVal, string paramName)
	{
		if (mat != null)
		{
			float timer = 0f;
			float startval = mat.GetFloat(paramName);
			while (timer < 0.2f && mat != null)
			{
				mat.SetFloat(paramName, Mathf.Lerp(startval, targetVal, timer * 5f));
				yield return null;
				timer += Time.deltaTime;
			}
			if (mat != null)
			{
				mat.SetFloat(paramName, targetVal);
			}
		}
		yield break;
	}

	// Token: 0x06000795 RID: 1941 RVA: 0x0001D0C9 File Offset: 0x0001B2C9
	public void ReinstateWardEmis()
	{
		base.StartCoroutine(this.LerpEmis(this.WardAndHoleMat, 1000f, "_emissi"));
	}

	// Token: 0x06000796 RID: 1942 RVA: 0x0001D0E8 File Offset: 0x0001B2E8
	public void CastWard(GameObject ownerobj, int level)
	{
		base.StartCoroutine(this.LerpEmis(this.WardAndHoleMat, 0f, "_emissi"));
		Collider[] array = Physics.OverlapSphere(base.transform.position, 60f, this.playerlayer);
		GameObject gameObject = null;
		float num = float.MaxValue;
		foreach (Collider collider in array)
		{
			Vector3 vector = collider.transform.position - base.transform.position;
			float magnitude = vector.magnitude;
			float num2 = Vector3.Angle(Camera.main.transform.forward, vector.normalized);
			PlayerMovement playerMovement;
			GetPlayerGameobject getPlayerGameobject;
			if (num2 <= 90f && !collider.TryGetComponent<PlayerMovement>(out playerMovement) && (!collider.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject) || getPlayerGameobject.player.GetComponent<PlayerMovement>().playerTeam != ownerobj.GetComponent<PlayerMovement>().playerTeam))
			{
				float num3 = magnitude + num2 * 0.5f;
				if (num3 < num)
				{
					num = num3;
					gameObject = collider.gameObject;
				}
			}
		}
		if (!(gameObject != null))
		{
			this.ShootMagicMissleServer(ownerobj, Camera.main.transform.forward, null, level);
			return;
		}
		PlayerMovement playerMovement2;
		if (gameObject.TryGetComponent<PlayerMovement>(out playerMovement2))
		{
			this.ShootMagicMissleServer(ownerobj, Camera.main.transform.forward, playerMovement2.gameObject, level);
			return;
		}
		GetPlayerGameobject getPlayerGameobject2;
		if (gameObject.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject2))
		{
			this.ShootMagicMissleServer(ownerobj, Camera.main.transform.forward, getPlayerGameobject2.player, level);
			return;
		}
		GetShadowWizardController getShadowWizardController;
		if (gameObject.TryGetComponent<GetShadowWizardController>(out getShadowWizardController))
		{
			this.ShootMagicMissleServer(ownerobj, Camera.main.transform.forward, getShadowWizardController.ShadowWizardAI, level);
			return;
		}
		this.ShootMagicMissleServer(ownerobj, Camera.main.transform.forward, null, level);
	}

	// Token: 0x06000797 RID: 1943 RVA: 0x0001D2AA File Offset: 0x0001B4AA
	[ServerRpc(RequireOwnership = false)]
	private void ShootMagicMissleServer(GameObject ownerobj, Vector3 fwdVector, GameObject target, int level)
	{
		this.RpcWriter___Server_ShootMagicMissleServer_2308368969(ownerobj, fwdVector, target, level);
	}

	// Token: 0x06000798 RID: 1944 RVA: 0x0001D2C4 File Offset: 0x0001B4C4
	[ObserversRpc]
	private void ShootMagicMissleOBs(GameObject ownerobj, Vector3 fwdVector, GameObject target, int level)
	{
		this.RpcWriter___Observers_ShootMagicMissleOBs_2308368969(ownerobj, fwdVector, target, level);
	}

	// Token: 0x06000799 RID: 1945 RVA: 0x0001D2E7 File Offset: 0x0001B4E7
	private IEnumerator SpawnMissles(GameObject ownerobj, Vector3 fwdVector, GameObject target, int level)
	{
		yield return new WaitForSeconds(0.1f);
		Object.Instantiate<GameObject>(this.MagicMissle, this.firePoint.position, Quaternion.identity).GetComponent<MagicMissleController>().SetUp(ownerobj, fwdVector, target, false, level);
		yield return new WaitForSeconds(0.1f);
		Object.Instantiate<GameObject>(this.MagicMissle, this.firePoint.position, Quaternion.identity).GetComponent<MagicMissleController>().SetUp(ownerobj, fwdVector, target, false, level);
		yield break;
	}

	// Token: 0x0600079A RID: 1946 RVA: 0x0001D313 File Offset: 0x0001B513
	public void ReinstateWormEmis()
	{
		base.StartCoroutine(this.LerpEmis(this.WormMat, 500f, "_emissi"));
	}

	// Token: 0x0600079B RID: 1947 RVA: 0x0001D332 File Offset: 0x0001B532
	public void FlickerWormEmis()
	{
		base.StartCoroutine(this.FlickerWormEmisrout());
	}

	// Token: 0x0600079C RID: 1948 RVA: 0x0001D341 File Offset: 0x0001B541
	private IEnumerator FlickerWormEmisrout()
	{
		base.StartCoroutine(this.LerpEmis(this.WormMat, 0f, "_emissi"));
		yield return new WaitForSeconds(0.5f);
		base.StartCoroutine(this.LerpEmis(this.WormMat, 500f, "_emissi"));
		yield break;
	}

	// Token: 0x0600079D RID: 1949 RVA: 0x0001D350 File Offset: 0x0001B550
	public void Castworm(int level, Vector3 pos)
	{
		base.StartCoroutine(this.LerpEmis(this.WormMat, 0f, "_emissi"));
		this.WormServer(level, pos);
	}

	// Token: 0x0600079E RID: 1950 RVA: 0x0001D377 File Offset: 0x0001B577
	[ServerRpc(RequireOwnership = false)]
	private void WormServer(int level, Vector3 pos)
	{
		this.RpcWriter___Server_WormServer_215135683(level, pos);
	}

	// Token: 0x0600079F RID: 1951 RVA: 0x0001D388 File Offset: 0x0001B588
	[ObserversRpc]
	private void WormOBs(int level, Vector3 pos)
	{
		this.RpcWriter___Observers_WormOBs_215135683(level, pos);
	}

	// Token: 0x060007A0 RID: 1952 RVA: 0x0001D3A3 File Offset: 0x0001B5A3
	public void ReinstateHoleEmis()
	{
		base.StartCoroutine(this.LerpEmis(this.WardAndHoleMat, 500f, "_emissiback"));
	}

	// Token: 0x060007A1 RID: 1953 RVA: 0x0001D3C2 File Offset: 0x0001B5C2
	public void FlickerHoleEmis()
	{
		base.StartCoroutine(this.FlickerHoleEmisrout());
	}

	// Token: 0x060007A2 RID: 1954 RVA: 0x0001D3D1 File Offset: 0x0001B5D1
	private IEnumerator FlickerHoleEmisrout()
	{
		base.StartCoroutine(this.LerpEmis(this.WardAndHoleMat, 0f, "_emissiback"));
		yield return new WaitForSeconds(0.5f);
		base.StartCoroutine(this.LerpEmis(this.WardAndHoleMat, 500f, "_emissiback"));
		yield break;
	}

	// Token: 0x060007A3 RID: 1955 RVA: 0x0001D3E0 File Offset: 0x0001B5E0
	public void Casthole(int level, Vector3 pos)
	{
		Debug.Log("attempt cast4");
		base.StartCoroutine(this.LerpEmis(this.WardAndHoleMat, 0f, "_emissiback"));
		this.HoleServer(level, pos);
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x0001D411 File Offset: 0x0001B611
	[ServerRpc(RequireOwnership = false)]
	private void HoleServer(int level, Vector3 pos)
	{
		this.RpcWriter___Server_HoleServer_215135683(level, pos);
	}

	// Token: 0x060007A5 RID: 1957 RVA: 0x0001D424 File Offset: 0x0001B624
	[ObserversRpc]
	private void HoleOBs(int level, Vector3 pos)
	{
		this.RpcWriter___Observers_HoleOBs_215135683(level, pos);
	}

	// Token: 0x060007A6 RID: 1958 RVA: 0x0001D43F File Offset: 0x0001B63F
	public void CallDestroyWormHole(bool ishole)
	{
		this.ServerDestroyHole(ishole);
	}

	// Token: 0x060007A7 RID: 1959 RVA: 0x0001D448 File Offset: 0x0001B648
	[ServerRpc(RequireOwnership = false)]
	private void ServerDestroyHole(bool ishole)
	{
		this.RpcWriter___Server_ServerDestroyHole_1140765316(ishole);
	}

	// Token: 0x060007A8 RID: 1960 RVA: 0x0001D454 File Offset: 0x0001B654
	[ObserversRpc]
	private void ObsDestroyHole(bool ishole)
	{
		this.RpcWriter___Observers_ObsDestroyHole_1140765316(ishole);
	}

	// Token: 0x060007A9 RID: 1961 RVA: 0x0001D460 File Offset: 0x0001B660
	public void ReinstateFrostBoltEmis()
	{
		base.StartCoroutine(this.LerpEmis(this.FrbMat, 1000f, "_emissi"));
	}

	// Token: 0x060007AA RID: 1962 RVA: 0x0001D47F File Offset: 0x0001B67F
	public void Frostbolt(GameObject ownerobj, int level)
	{
		base.StartCoroutine(this.LerpEmis(this.FrbMat, 0f, "_emissi"));
		this.ShootFrostboltServer(ownerobj, Camera.main.transform.forward, level);
	}

	// Token: 0x060007AB RID: 1963 RVA: 0x0001D4B5 File Offset: 0x0001B6B5
	[ServerRpc(RequireOwnership = false)]
	private void ShootFrostboltServer(GameObject ownerobj, Vector3 fwdVector, int level)
	{
		this.RpcWriter___Server_ShootFrostboltServer_1366904011(ownerobj, fwdVector, level);
	}

	// Token: 0x060007AC RID: 1964 RVA: 0x0001D4C9 File Offset: 0x0001B6C9
	[ObserversRpc]
	private void ShootFrostboltOBs(GameObject ownerobj, Vector3 fwdVector, int level)
	{
		this.RpcWriter___Observers_ShootFrostboltOBs_1366904011(ownerobj, fwdVector, level);
	}

	// Token: 0x060007AD RID: 1965 RVA: 0x0001D4DD File Offset: 0x0001B6DD
	public void ReinstateFireballEmis()
	{
		base.StartCoroutine(this.LerpEmis(this.FBMat, 1000f, "_emissi"));
	}

	// Token: 0x060007AE RID: 1966 RVA: 0x0001D4FC File Offset: 0x0001B6FC
	public void Fireball(GameObject ownerobj, int level)
	{
		base.StartCoroutine(this.LerpEmis(this.FBMat, 0f, "_emissi"));
		this.ShootFireballServer(ownerobj, Camera.main.transform.forward, level, this.firePoint.position);
	}

	// Token: 0x060007AF RID: 1967 RVA: 0x0001D548 File Offset: 0x0001B748
	[ServerRpc(RequireOwnership = false)]
	private void ShootFireballServer(GameObject ownerobj, Vector3 fwdVector, int level, Vector3 spawnpos)
	{
		this.RpcWriter___Server_ShootFireballServer_3976682022(ownerobj, fwdVector, level, spawnpos);
	}

	// Token: 0x060007B0 RID: 1968 RVA: 0x0001D560 File Offset: 0x0001B760
	[ObserversRpc]
	private void ShootfireballOBs(GameObject ownerobj, Vector3 fwdVector, int level, Vector3 spawnpos)
	{
		this.RpcWriter___Observers_ShootfireballOBs_3976682022(ownerobj, fwdVector, level, spawnpos);
	}

	// Token: 0x060007B1 RID: 1969 RVA: 0x0001D578 File Offset: 0x0001B778
	private void Update()
	{
		if (this.isEquipped)
		{
			this.prevpos = this.firePoint.position;
			int lastPressedPage = this.LastPressedPage;
			if (Input.GetKeyDown(this.Sholder.four))
			{
				this.ServerFlipPage(4, lastPressedPage);
				this.LastPressedPage = 4;
			}
			if (Input.GetKeyDown(this.Sholder.three))
			{
				this.ServerFlipPage(3, lastPressedPage);
				this.LastPressedPage = 3;
			}
			if (Input.GetKeyDown(this.Sholder.two))
			{
				this.ServerFlipPage(2, lastPressedPage);
				this.LastPressedPage = 2;
			}
			if (Input.GetKeyDown(this.Sholder.one))
			{
				this.ServerFlipPage(1, lastPressedPage);
				this.LastPressedPage = 1;
				return;
			}
		}
		else
		{
			if (Input.GetKeyDown(this.Sholder.four))
			{
				this.LastPressedPage = 4;
			}
			if (Input.GetKeyDown(this.Sholder.three))
			{
				this.LastPressedPage = 3;
			}
			if (Input.GetKeyDown(this.Sholder.two))
			{
				this.LastPressedPage = 2;
			}
			if (Input.GetKeyDown(this.Sholder.one))
			{
				this.LastPressedPage = 1;
			}
		}
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x0001D691 File Offset: 0x0001B891
	[ServerRpc(RequireOwnership = false)]
	private void ServerFlipPage(int pgnum, int startpg)
	{
		this.RpcWriter___Server_ServerFlipPage_1692629761(pgnum, startpg);
	}

	// Token: 0x060007B3 RID: 1971 RVA: 0x0001D6A4 File Offset: 0x0001B8A4
	[ObserversRpc]
	private void ObsFlipPage(int pgnum, int startpg)
	{
		this.RpcWriter___Observers_ObsFlipPage_1692629761(pgnum, startpg);
	}

	// Token: 0x060007B4 RID: 1972 RVA: 0x0001D6BF File Offset: 0x0001B8BF
	private IEnumerator Lerppos(Transform ltarget, float lval)
	{
		float i = 0f;
		float currentValue = ltarget.localPosition.y;
		while (i < 0.05f)
		{
			i += Time.deltaTime;
			float num = i / 0.05f;
			currentValue = Mathf.Lerp(currentValue, lval, num);
			ltarget.localPosition = new Vector3(0f, currentValue, 0f);
			yield return null;
		}
		ltarget.localPosition = new Vector3(0f, lval, 0f);
		yield break;
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x0001D6D5 File Offset: 0x0001B8D5
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x060007B6 RID: 1974 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction(GameObject player)
	{
	}

	// Token: 0x060007B7 RID: 1975 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x060007B8 RID: 1976 RVA: 0x0001D708 File Offset: 0x0001B908
	public void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			base.transform.position = new Vector3(raycastHit.point.x, raycastHit.point.y + 0.1f, raycastHit.point.z);
		}
		this.isEquipped = false;
		this.asource.PlayOneShot(this.aclip[8]);
		if (!this.isowner)
		{
			base.StartCoroutine(this.disablecoll());
		}
		else
		{
			this.booklight.SetActive(true);
		}
		foreach (Animator animator in this.animaes)
		{
			animator.Rebind();
			animator.Update(0f);
			animator.SetBool("eq", false);
		}
		this.bookrender.SetActive(true);
	}

	// Token: 0x060007B9 RID: 1977 RVA: 0x0001D81B File Offset: 0x0001BA1B
	private IEnumerator disablecoll()
	{
		yield return null;
		Collider collider;
		if (base.TryGetComponent<Collider>(out collider))
		{
			collider.enabled = false;
		}
		yield break;
	}

	// Token: 0x060007BA RID: 1978 RVA: 0x0001D82A File Offset: 0x0001BA2A
	public void PlayDropSound()
	{
		this.asource.PlayOneShot(this.aclip[8]);
	}

	// Token: 0x060007BB RID: 1979 RVA: 0x0001D840 File Offset: 0x0001BA40
	public void ItemInit()
	{
		this.bookrender.SetActive(true);
		this.pages[3].localPosition = new Vector3(0f, 0.09f, 0f);
		this.pages[2].localPosition = new Vector3(0f, 0.08f, 0f);
		this.pages[1].localPosition = new Vector3(0f, 0.06f, 0f);
		foreach (Animator animator in this.animaes)
		{
			animator.Rebind();
			animator.Update(0f);
			animator.SetBool("eq", true);
		}
		this.isEquipped = true;
		this.asource.PlayOneShot(this.aclip[0]);
		if (!this.isowner)
		{
			this.isowner = true;
			this.pinv.setsbk(base.gameObject);
		}
		this.booklight.SetActive(false);
		this.ServerFlipPage(this.LastPressedPage, 4);
	}

	// Token: 0x060007BC RID: 1980 RVA: 0x0001D944 File Offset: 0x0001BB44
	public void ItemInitObs()
	{
		this.pages[3].localPosition = new Vector3(0f, 0.09f, 0f);
		this.pages[2].localPosition = new Vector3(0f, 0.08f, 0f);
		this.pages[1].localPosition = new Vector3(0f, 0.06f, 0f);
		this.bookrender.SetActive(true);
		foreach (Animator animator in this.animaes)
		{
			animator.Rebind();
			animator.Update(0f);
			animator.SetBool("eq", true);
		}
		this.asource.PlayOneShot(this.aclip[0]);
	}

	// Token: 0x060007BD RID: 1981 RVA: 0x0001DA07 File Offset: 0x0001BC07
	public void HideItem()
	{
		this.bookrender.SetActive(false);
		this.booklight.SetActive(false);
		this.isEquipped = false;
	}

	// Token: 0x060007BE RID: 1982 RVA: 0x0001DA28 File Offset: 0x0001BC28
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
		this.booklight.SetActive(false);
	}

	// Token: 0x060007BF RID: 1983 RVA: 0x0001DA47 File Offset: 0x0001BC47
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp BooK";
	}

	// Token: 0x060007C0 RID: 1984 RVA: 0x0001DA4E File Offset: 0x0001BC4E
	public int GetItemID()
	{
		return 24;
	}

	// Token: 0x060007C2 RID: 1986 RVA: 0x0001DA88 File Offset: 0x0001BC88
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyMageBookControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyMageBookControllerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ShootMagicMissleServer_2308368969));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_ShootMagicMissleOBs_2308368969));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_WormServer_215135683));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_WormOBs_215135683));
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_HoleServer_215135683));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_HoleOBs_215135683));
		base.RegisterServerRpc(6U, new ServerRpcDelegate(this.RpcReader___Server_ServerDestroyHole_1140765316));
		base.RegisterObserversRpc(7U, new ClientRpcDelegate(this.RpcReader___Observers_ObsDestroyHole_1140765316));
		base.RegisterServerRpc(8U, new ServerRpcDelegate(this.RpcReader___Server_ShootFrostboltServer_1366904011));
		base.RegisterObserversRpc(9U, new ClientRpcDelegate(this.RpcReader___Observers_ShootFrostboltOBs_1366904011));
		base.RegisterServerRpc(10U, new ServerRpcDelegate(this.RpcReader___Server_ShootFireballServer_3976682022));
		base.RegisterObserversRpc(11U, new ClientRpcDelegate(this.RpcReader___Observers_ShootfireballOBs_3976682022));
		base.RegisterServerRpc(12U, new ServerRpcDelegate(this.RpcReader___Server_ServerFlipPage_1692629761));
		base.RegisterObserversRpc(13U, new ClientRpcDelegate(this.RpcReader___Observers_ObsFlipPage_1692629761));
	}

	// Token: 0x060007C3 RID: 1987 RVA: 0x0001DBE8 File Offset: 0x0001BDE8
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateMageBookControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateMageBookControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x060007C4 RID: 1988 RVA: 0x0001DBFB File Offset: 0x0001BDFB
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x060007C5 RID: 1989 RVA: 0x0001DC0C File Offset: 0x0001BE0C
	private void RpcWriter___Server_ShootMagicMissleServer_2308368969(GameObject ownerobj, Vector3 fwdVector, GameObject target, int level)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(ownerobj);
		pooledWriter.WriteVector3(fwdVector);
		pooledWriter.WriteGameObject(target);
		pooledWriter.WriteInt32(level);
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060007C6 RID: 1990 RVA: 0x0001DCA5 File Offset: 0x0001BEA5
	private void RpcLogic___ShootMagicMissleServer_2308368969(GameObject ownerobj, Vector3 fwdVector, GameObject target, int level)
	{
		this.ShootMagicMissleOBs(ownerobj, fwdVector, target, level);
	}

	// Token: 0x060007C7 RID: 1991 RVA: 0x0001DCB4 File Offset: 0x0001BEB4
	private void RpcReader___Server_ShootMagicMissleServer_2308368969(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		Vector3 vector = PooledReader0.ReadVector3();
		GameObject gameObject2 = PooledReader0.ReadGameObject();
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ShootMagicMissleServer_2308368969(gameObject, vector, gameObject2, num);
	}

	// Token: 0x060007C8 RID: 1992 RVA: 0x0001DD18 File Offset: 0x0001BF18
	private void RpcWriter___Observers_ShootMagicMissleOBs_2308368969(GameObject ownerobj, Vector3 fwdVector, GameObject target, int level)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(ownerobj);
		pooledWriter.WriteVector3(fwdVector);
		pooledWriter.WriteGameObject(target);
		pooledWriter.WriteInt32(level);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060007C9 RID: 1993 RVA: 0x0001DDC0 File Offset: 0x0001BFC0
	private void RpcLogic___ShootMagicMissleOBs_2308368969(GameObject ownerobj, Vector3 fwdVector, GameObject target, int level)
	{
		this.asource.PlayOneShot(this.aclip[7]);
		Object.Instantiate<GameObject>(this.MagicMissle, this.firePoint.position, Quaternion.identity).GetComponent<MagicMissleController>().SetUp(ownerobj, fwdVector, target, false, level);
		base.StartCoroutine(this.SpawnMissles(ownerobj, fwdVector, target, level));
	}

	// Token: 0x060007CA RID: 1994 RVA: 0x0001DE20 File Offset: 0x0001C020
	private void RpcReader___Observers_ShootMagicMissleOBs_2308368969(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		Vector3 vector = PooledReader0.ReadVector3();
		GameObject gameObject2 = PooledReader0.ReadGameObject();
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ShootMagicMissleOBs_2308368969(gameObject, vector, gameObject2, num);
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x0001DE84 File Offset: 0x0001C084
	private void RpcWriter___Server_WormServer_215135683(int level, Vector3 pos)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(level);
		pooledWriter.WriteVector3(pos);
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x0001DF03 File Offset: 0x0001C103
	private void RpcLogic___WormServer_215135683(int level, Vector3 pos)
	{
		this.WormOBs(level, pos);
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x0001DF10 File Offset: 0x0001C110
	private void RpcReader___Server_WormServer_215135683(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___WormServer_215135683(num, vector);
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x0001DF54 File Offset: 0x0001C154
	private void RpcWriter___Observers_WormOBs_215135683(int level, Vector3 pos)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(level);
		pooledWriter.WriteVector3(pos);
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060007CF RID: 1999 RVA: 0x0001DFE4 File Offset: 0x0001C1E4
	private void RpcLogic___WormOBs_215135683(int level, Vector3 pos)
	{
		if (this.worm == null && this.hole == null)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.wormHole, pos, Quaternion.identity);
			this.worm = gameObject.GetComponent<WormholeTele>();
			this.worm.mbc = this;
			this.worm.isHole = false;
			if (level == 1)
			{
				this.worm.DestroyInSeconds(25);
				return;
			}
			if (level == 2)
			{
				this.worm.DestroyInSeconds(45);
				return;
			}
		}
		else if (this.worm == null && this.hole != null)
		{
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.wormHole, pos, Quaternion.identity);
			this.worm = gameObject2.GetComponent<WormholeTele>();
			this.worm.mbc = this;
			this.worm.isHole = false;
			this.hole.WormHolePair = this.worm.gameObject;
			this.worm.WormHolePair = this.hole.gameObject;
			if (level == 1)
			{
				this.worm.DestroyInSeconds(25);
				return;
			}
			if (level == 2)
			{
				this.worm.DestroyInSeconds(45);
				return;
			}
		}
		else if (this.worm != null && this.hole != null)
		{
			this.worm.transform.position = pos;
		}
	}

	// Token: 0x060007D0 RID: 2000 RVA: 0x0001E140 File Offset: 0x0001C340
	private void RpcReader___Observers_WormOBs_215135683(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___WormOBs_215135683(num, vector);
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x0001E184 File Offset: 0x0001C384
	private void RpcWriter___Server_HoleServer_215135683(int level, Vector3 pos)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(level);
		pooledWriter.WriteVector3(pos);
		base.SendServerRpc(4U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060007D2 RID: 2002 RVA: 0x0001E203 File Offset: 0x0001C403
	private void RpcLogic___HoleServer_215135683(int level, Vector3 pos)
	{
		this.HoleOBs(level, pos);
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x0001E210 File Offset: 0x0001C410
	private void RpcReader___Server_HoleServer_215135683(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___HoleServer_215135683(num, vector);
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x0001E254 File Offset: 0x0001C454
	private void RpcWriter___Observers_HoleOBs_215135683(int level, Vector3 pos)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(level);
		pooledWriter.WriteVector3(pos);
		base.SendObserversRpc(5U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060007D5 RID: 2005 RVA: 0x0001E2E4 File Offset: 0x0001C4E4
	private void RpcLogic___HoleOBs_215135683(int level, Vector3 pos)
	{
		if (this.worm == null && this.hole == null)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.wormHole, pos, Quaternion.identity);
			this.hole = gameObject.GetComponent<WormholeTele>();
			this.hole.mbc = this;
			this.hole.isHole = true;
			if (level == 1)
			{
				this.hole.DestroyInSeconds(25);
				return;
			}
			if (level == 2)
			{
				this.hole.DestroyInSeconds(45);
				return;
			}
		}
		else if (this.worm != null && this.hole == null)
		{
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.wormHole, pos, Quaternion.identity);
			this.hole = gameObject2.GetComponent<WormholeTele>();
			this.hole.mbc = this;
			this.hole.isHole = true;
			this.worm.WormHolePair = this.hole.gameObject;
			this.hole.WormHolePair = this.worm.gameObject;
			if (level == 1)
			{
				this.hole.DestroyInSeconds(25);
				return;
			}
			if (level == 2)
			{
				this.hole.DestroyInSeconds(45);
				return;
			}
		}
		else if (this.worm != null && this.hole != null)
		{
			this.hole.transform.position = pos;
		}
	}

	// Token: 0x060007D6 RID: 2006 RVA: 0x0001E440 File Offset: 0x0001C640
	private void RpcReader___Observers_HoleOBs_215135683(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___HoleOBs_215135683(num, vector);
	}

	// Token: 0x060007D7 RID: 2007 RVA: 0x0001E484 File Offset: 0x0001C684
	private void RpcWriter___Server_ServerDestroyHole_1140765316(bool ishole)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteBoolean(ishole);
		base.SendServerRpc(6U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x0001E4F6 File Offset: 0x0001C6F6
	private void RpcLogic___ServerDestroyHole_1140765316(bool ishole)
	{
		this.ObsDestroyHole(ishole);
	}

	// Token: 0x060007D9 RID: 2009 RVA: 0x0001E500 File Offset: 0x0001C700
	private void RpcReader___Server_ServerDestroyHole_1140765316(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerDestroyHole_1140765316(flag);
	}

	// Token: 0x060007DA RID: 2010 RVA: 0x0001E534 File Offset: 0x0001C734
	private void RpcWriter___Observers_ObsDestroyHole_1140765316(bool ishole)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteBoolean(ishole);
		base.SendObserversRpc(7U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060007DB RID: 2011 RVA: 0x0001E5B5 File Offset: 0x0001C7B5
	private void RpcLogic___ObsDestroyHole_1140765316(bool ishole)
	{
		if (ishole)
		{
			if (this.hole != null)
			{
				this.hole.DestroyWormhole();
				return;
			}
		}
		else if (this.worm != null)
		{
			this.worm.DestroyWormhole();
		}
	}

	// Token: 0x060007DC RID: 2012 RVA: 0x0001E5F0 File Offset: 0x0001C7F0
	private void RpcReader___Observers_ObsDestroyHole_1140765316(PooledReader PooledReader0, Channel channel)
	{
		bool flag = PooledReader0.ReadBoolean();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsDestroyHole_1140765316(flag);
	}

	// Token: 0x060007DD RID: 2013 RVA: 0x0001E624 File Offset: 0x0001C824
	private void RpcWriter___Server_ShootFrostboltServer_1366904011(GameObject ownerobj, Vector3 fwdVector, int level)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(ownerobj);
		pooledWriter.WriteVector3(fwdVector);
		pooledWriter.WriteInt32(level);
		base.SendServerRpc(8U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060007DE RID: 2014 RVA: 0x0001E6B0 File Offset: 0x0001C8B0
	private void RpcLogic___ShootFrostboltServer_1366904011(GameObject ownerobj, Vector3 fwdVector, int level)
	{
		this.ShootFrostboltOBs(ownerobj, fwdVector, level);
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x0001E6BC File Offset: 0x0001C8BC
	private void RpcReader___Server_ShootFrostboltServer_1366904011(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		Vector3 vector = PooledReader0.ReadVector3();
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ShootFrostboltServer_1366904011(gameObject, vector, num);
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x0001E710 File Offset: 0x0001C910
	private void RpcWriter___Observers_ShootFrostboltOBs_1366904011(GameObject ownerobj, Vector3 fwdVector, int level)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(ownerobj);
		pooledWriter.WriteVector3(fwdVector);
		pooledWriter.WriteInt32(level);
		base.SendObserversRpc(9U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x0001E7AB File Offset: 0x0001C9AB
	private void RpcLogic___ShootFrostboltOBs_1366904011(GameObject ownerobj, Vector3 fwdVector, int level)
	{
		Object.Instantiate<GameObject>(this.frostBolt, this.firePoint.position, Quaternion.identity).GetComponent<FrostBoltController>().PlayerSetup(ownerobj, fwdVector, level);
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x0001E7D8 File Offset: 0x0001C9D8
	private void RpcReader___Observers_ShootFrostboltOBs_1366904011(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		Vector3 vector = PooledReader0.ReadVector3();
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ShootFrostboltOBs_1366904011(gameObject, vector, num);
	}

	// Token: 0x060007E3 RID: 2019 RVA: 0x0001E82C File Offset: 0x0001CA2C
	private void RpcWriter___Server_ShootFireballServer_3976682022(GameObject ownerobj, Vector3 fwdVector, int level, Vector3 spawnpos)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(ownerobj);
		pooledWriter.WriteVector3(fwdVector);
		pooledWriter.WriteInt32(level);
		pooledWriter.WriteVector3(spawnpos);
		base.SendServerRpc(10U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x0001E8C5 File Offset: 0x0001CAC5
	private void RpcLogic___ShootFireballServer_3976682022(GameObject ownerobj, Vector3 fwdVector, int level, Vector3 spawnpos)
	{
		if (ownerobj != null)
		{
			this.ShootfireballOBs(ownerobj, fwdVector, level, spawnpos);
		}
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x0001E8DC File Offset: 0x0001CADC
	private void RpcReader___Server_ShootFireballServer_3976682022(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		Vector3 vector = PooledReader0.ReadVector3();
		int num = PooledReader0.ReadInt32();
		Vector3 vector2 = PooledReader0.ReadVector3();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ShootFireballServer_3976682022(gameObject, vector, num, vector2);
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x0001E940 File Offset: 0x0001CB40
	private void RpcWriter___Observers_ShootfireballOBs_3976682022(GameObject ownerobj, Vector3 fwdVector, int level, Vector3 spawnpos)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(ownerobj);
		pooledWriter.WriteVector3(fwdVector);
		pooledWriter.WriteInt32(level);
		pooledWriter.WriteVector3(spawnpos);
		base.SendObserversRpc(11U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x0001E9E8 File Offset: 0x0001CBE8
	private void RpcLogic___ShootfireballOBs_3976682022(GameObject ownerobj, Vector3 fwdVector, int level, Vector3 spawnpos)
	{
		if (ownerobj != null)
		{
			Object.Instantiate<GameObject>(this.fireBall, spawnpos, Quaternion.identity).GetComponent<FireballController>().PlayerSetup(ownerobj, fwdVector, level);
		}
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x0001EA14 File Offset: 0x0001CC14
	private void RpcReader___Observers_ShootfireballOBs_3976682022(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		Vector3 vector = PooledReader0.ReadVector3();
		int num = PooledReader0.ReadInt32();
		Vector3 vector2 = PooledReader0.ReadVector3();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ShootfireballOBs_3976682022(gameObject, vector, num, vector2);
	}

	// Token: 0x060007E9 RID: 2025 RVA: 0x0001EA78 File Offset: 0x0001CC78
	private void RpcWriter___Server_ServerFlipPage_1692629761(int pgnum, int startpg)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(pgnum);
		pooledWriter.WriteInt32(startpg);
		base.SendServerRpc(12U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060007EA RID: 2026 RVA: 0x0001EAF7 File Offset: 0x0001CCF7
	private void RpcLogic___ServerFlipPage_1692629761(int pgnum, int startpg)
	{
		this.ObsFlipPage(pgnum, startpg);
	}

	// Token: 0x060007EB RID: 2027 RVA: 0x0001EB04 File Offset: 0x0001CD04
	private void RpcReader___Server_ServerFlipPage_1692629761(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		int num = PooledReader0.ReadInt32();
		int num2 = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerFlipPage_1692629761(num, num2);
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x0001EB48 File Offset: 0x0001CD48
	private void RpcWriter___Observers_ObsFlipPage_1692629761(int pgnum, int startpg)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteInt32(pgnum);
		pooledWriter.WriteInt32(startpg);
		base.SendObserversRpc(13U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x0001EBD8 File Offset: 0x0001CDD8
	private void RpcLogic___ObsFlipPage_1692629761(int pgnum, int startpg)
	{
		if (startpg < pgnum - 1)
		{
			if (pgnum - startpg == 2)
			{
				this.asource.PlayOneShot(this.aclip[5]);
			}
			else if (pgnum - startpg == 3)
			{
				this.asource.PlayOneShot(this.aclip[6]);
			}
		}
		else if (pgnum != startpg)
		{
			this.asource.PlayOneShot(this.aclip[Random.Range(1, 5)]);
		}
		if (pgnum == 4)
		{
			this.animaes[1].SetBool("pgflip", false);
			this.animaes[2].SetBool("pgflip", false);
			this.animaes[3].SetBool("pgflip", false);
			this.animaes[4].SetBool("pgflip", false);
			base.StartCoroutine(this.Lerppos(this.pages[3], 0.09f));
			base.StartCoroutine(this.Lerppos(this.pages[2], 0.08f));
			base.StartCoroutine(this.Lerppos(this.pages[1], 0.06f));
		}
		if (pgnum == 3)
		{
			this.animaes[1].SetBool("pgflip", false);
			this.animaes[2].SetBool("pgflip", false);
			this.animaes[3].SetBool("pgflip", false);
			this.animaes[4].SetBool("pgflip", true);
			base.StartCoroutine(this.Lerppos(this.pages[3], 0.04f));
			base.StartCoroutine(this.Lerppos(this.pages[2], 0.08f));
			base.StartCoroutine(this.Lerppos(this.pages[1], 0.04f));
			base.StartCoroutine(this.Lerppos(this.pages[0], 0f));
		}
		if (pgnum == 2)
		{
			this.animaes[1].SetBool("pgflip", false);
			this.animaes[2].SetBool("pgflip", false);
			this.animaes[3].SetBool("pgflip", true);
			this.animaes[4].SetBool("pgflip", true);
			base.StartCoroutine(this.Lerppos(this.pages[3], 0.04f));
			base.StartCoroutine(this.Lerppos(this.pages[2], 0.06f));
			base.StartCoroutine(this.Lerppos(this.pages[1], 0.06f));
		}
		if (pgnum == 1)
		{
			this.animaes[1].SetBool("pgflip", false);
			this.animaes[2].SetBool("pgflip", true);
			this.animaes[3].SetBool("pgflip", true);
			this.animaes[4].SetBool("pgflip", true);
			base.StartCoroutine(this.Lerppos(this.pages[3], 0.04f));
			base.StartCoroutine(this.Lerppos(this.pages[2], 0.06f));
			base.StartCoroutine(this.Lerppos(this.pages[1], 0.08f));
		}
	}

	// Token: 0x060007EE RID: 2030 RVA: 0x0001EEDC File Offset: 0x0001D0DC
	private void RpcReader___Observers_ObsFlipPage_1692629761(PooledReader PooledReader0, Channel channel)
	{
		int num = PooledReader0.ReadInt32();
		int num2 = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsFlipPage_1692629761(num, num2);
	}

	// Token: 0x060007EF RID: 2031 RVA: 0x0001DBFB File Offset: 0x0001BDFB
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x040003DE RID: 990
	public Animator[] animaes;

	// Token: 0x040003DF RID: 991
	public Transform[] pages;

	// Token: 0x040003E0 RID: 992
	public GameObject bookrender;

	// Token: 0x040003E1 RID: 993
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x040003E2 RID: 994
	public int LastPressedPage = 4;

	// Token: 0x040003E3 RID: 995
	public LayerMask layerMask;

	// Token: 0x040003E4 RID: 996
	public GameObject fireBall;

	// Token: 0x040003E5 RID: 997
	public GameObject frostBolt;

	// Token: 0x040003E6 RID: 998
	public GameObject wormHole;

	// Token: 0x040003E7 RID: 999
	public GameObject MagicMissle;

	// Token: 0x040003E8 RID: 1000
	private WormholeTele worm;

	// Token: 0x040003E9 RID: 1001
	private WormholeTele hole;

	// Token: 0x040003EA RID: 1002
	public Transform firePoint;

	// Token: 0x040003EB RID: 1003
	public AudioSource asource;

	// Token: 0x040003EC RID: 1004
	public AudioClip[] aclip;

	// Token: 0x040003ED RID: 1005
	public PlayerInventory pinv;

	// Token: 0x040003EE RID: 1006
	private bool isEquipped;

	// Token: 0x040003EF RID: 1007
	private Material FBMat;

	// Token: 0x040003F0 RID: 1008
	private Material FrbMat;

	// Token: 0x040003F1 RID: 1009
	private Material WormMat;

	// Token: 0x040003F2 RID: 1010
	private Material WardAndHoleMat;

	// Token: 0x040003F3 RID: 1011
	public SkinnedMeshRenderer[] PageRenderers;

	// Token: 0x040003F4 RID: 1012
	public LayerMask playerlayer;

	// Token: 0x040003F5 RID: 1013
	private SettingsHolder Sholder;

	// Token: 0x040003F6 RID: 1014
	private bool isowner;

	// Token: 0x040003F7 RID: 1015
	public GameObject booklight;

	// Token: 0x040003F8 RID: 1016
	private Vector3 prevpos = Vector3.zero;

	// Token: 0x040003F9 RID: 1017
	private bool NetworkInitialize___EarlyMageBookControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x040003FA RID: 1018
	private bool NetworkInitialize__LateMageBookControllerAssembly-CSharp.dll_Excuted;
}
