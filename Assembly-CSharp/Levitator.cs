using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x020000B8 RID: 184
public class Levitator : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x06000726 RID: 1830 RVA: 0x0001B52B File Offset: 0x0001972B
	private void Update()
	{
		if (this.LevEnergy < 30f)
		{
			this.LevEnergy += Time.deltaTime;
		}
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x0001B54C File Offset: 0x0001974C
	private void Start()
	{
		this.maincam = Camera.main.transform;
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x0001B55E File Offset: 0x0001975E
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x06000729 RID: 1833 RVA: 0x0001B590 File Offset: 0x00019790
	public void Interaction(GameObject player)
	{
		base.StartCoroutine(this.LevitatorRoutine(player.GetComponent<PlayerInventory>(), player.GetComponent<PlayerMovement>()));
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x0001B5AB File Offset: 0x000197AB
	private IEnumerator LevitatorRoutine(PlayerInventory playerInv, PlayerMovement playerMov)
	{
		int eqindex = playerInv.equippedIndex;
		yield return new WaitForSeconds(0.1f);
		while (Input.GetKey(playerInv.Sholder.leftmouse) && Input.GetAxis("Mouse ScrollWheel") == 0f)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(this.maincam.position, this.maincam.forward, out raycastHit, this.maxDistance, this.layermsk))
			{
				this.HitSubject = raycastHit.transform.gameObject;
				this.isHitting = true;
				this.lineren.enabled = true;
				this.lineren.SetPosition(0, this.linePoint.position);
				this.lineren.SetPosition(1, raycastHit.point);
			}
			else
			{
				this.HitSubject = null;
				this.isHitting = false;
				this.lineren.enabled = false;
			}
			if (this.isHitting && this.HitSubject != null)
			{
				if (!this.HitSubject.CompareTag("Player") || this.LevEnergy <= 1f)
				{
					break;
				}
				GetPlayerGameobject poop;
				if (this.HitSubject.TryGetComponent<GetPlayerGameobject>(out poop))
				{
					this.HitSubject = poop.player;
					if (this.networkedhitsubj != poop.player)
					{
						this.networkedhitsubj = poop.player;
						this.sethitsubj(poop.player);
					}
					this.MakeLevitate(poop.player);
					while (this.LevEnergy > 0f && eqindex == playerInv.equippedIndex && Input.GetKey(playerInv.Sholder.leftmouse))
					{
						this.LevEnergy -= Time.deltaTime * 2f;
						yield return null;
					}
					this.NoMakeLevitate(poop.player);
				}
				else
				{
					PlayerMovement pm;
					if (this.HitSubject.TryGetComponent<PlayerMovement>(out pm))
					{
						this.HitSubject = pm.gameObject;
						if (this.networkedhitsubj != pm.gameObject)
						{
							this.networkedhitsubj = pm.gameObject;
							this.sethitsubj(pm.gameObject);
						}
						this.MakeLevitate(pm.gameObject);
						while (this.LevEnergy > 0f && eqindex == playerInv.equippedIndex && Input.GetKey(playerInv.Sholder.leftmouse))
						{
							this.LevEnergy -= Time.deltaTime * 2f;
							yield return null;
						}
						this.NoMakeLevitate(pm.gameObject);
					}
					pm = null;
				}
				poop = null;
			}
			yield return null;
		}
		playerInv.canSwapItem = true;
		playerInv.canUseItem = true;
		playerMov.canJump = true;
		playerMov.camArmsSyncSpeed = 10f;
		playerMov.mouseSensitivity = 2f;
		playerMov.runningSpeed = 11.5f;
		playerMov.walkingSpeed = 7.5f;
		this.lineren.enabled = false;
		yield break;
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x0001B5C8 File Offset: 0x000197C8
	[ServerRpc(RequireOwnership = false)]
	private void sethitsubj(GameObject hitsubj)
	{
		this.RpcWriter___Server_sethitsubj_1934289915(hitsubj);
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x0001B5D4 File Offset: 0x000197D4
	[ObserversRpc]
	private void obssethitsubj(GameObject hitsubj)
	{
		this.RpcWriter___Observers_obssethitsubj_1934289915(hitsubj);
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x0001B5E0 File Offset: 0x000197E0
	[ServerRpc(RequireOwnership = false)]
	private void MakeLevitate(GameObject poopler)
	{
		this.RpcWriter___Server_MakeLevitate_1934289915(poopler);
	}

	// Token: 0x0600072E RID: 1838 RVA: 0x0001B5EC File Offset: 0x000197EC
	[ObserversRpc]
	private void MakeLevitateObs(GameObject poopler)
	{
		this.RpcWriter___Observers_MakeLevitateObs_1934289915(poopler);
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x0001B5F8 File Offset: 0x000197F8
	private IEnumerator LineRo()
	{
		this.lineren.enabled = true;
		while (this.isLeving && this.HitSubject != null)
		{
			this.lineren.SetPosition(0, this.linePoint.position);
			this.lineren.SetPosition(1, new Vector3(this.HitSubject.transform.position.x, this.HitSubject.transform.position.y + 1f, this.HitSubject.transform.position.z));
			yield return null;
		}
		this.lineren.enabled = false;
		yield break;
	}

	// Token: 0x06000730 RID: 1840 RVA: 0x0001B607 File Offset: 0x00019807
	[ServerRpc(RequireOwnership = false)]
	private void NoMakeLevitate(GameObject poopler)
	{
		this.RpcWriter___Server_NoMakeLevitate_1934289915(poopler);
	}

	// Token: 0x06000731 RID: 1841 RVA: 0x0001B613 File Offset: 0x00019813
	[ObserversRpc]
	private void NoMakeLevitateObs(GameObject poopler)
	{
		this.RpcWriter___Observers_NoMakeLevitateObs_1934289915(poopler);
	}

	// Token: 0x06000732 RID: 1842 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x06000733 RID: 1843 RVA: 0x0001B620 File Offset: 0x00019820
	public void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			base.transform.position = raycastHit.point;
		}
		this.asource.PlayOneShot(this.aclips[1]);
		this.lineren.enabled = false;
		this.rockrender.SetActive(true);
	}

	// Token: 0x06000734 RID: 1844 RVA: 0x0001B6B9 File Offset: 0x000198B9
	public void PlayDropSound()
	{
		this.asource.PlayOneShot(this.aclips[1]);
	}

	// Token: 0x06000735 RID: 1845 RVA: 0x0001B6CE File Offset: 0x000198CE
	public void ItemInit()
	{
		this.rockrender.SetActive(true);
		this.asource.PlayOneShot(this.aclips[0]);
		this.asource.PlayOneShot(this.aclips[2]);
	}

	// Token: 0x06000736 RID: 1846 RVA: 0x0001B702 File Offset: 0x00019902
	public void ItemInitObs()
	{
		this.asource.PlayOneShot(this.aclips[0]);
		this.asource.PlayOneShot(this.aclips[2]);
		this.rockrender.SetActive(true);
	}

	// Token: 0x06000737 RID: 1847 RVA: 0x0001B736 File Offset: 0x00019936
	public void HideItem()
	{
		this.rockrender.SetActive(false);
	}

	// Token: 0x06000738 RID: 1848 RVA: 0x00007BEE File Offset: 0x00005DEE
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
	}

	// Token: 0x06000739 RID: 1849 RVA: 0x0001B744 File Offset: 0x00019944
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp Levitator";
	}

	// Token: 0x0600073A RID: 1850 RVA: 0x0001B74B File Offset: 0x0001994B
	public int GetItemID()
	{
		return 15;
	}

	// Token: 0x0600073C RID: 1852 RVA: 0x0001B77C File Offset: 0x0001997C
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyLevitatorAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyLevitatorAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_sethitsubj_1934289915));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_obssethitsubj_1934289915));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_MakeLevitate_1934289915));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_MakeLevitateObs_1934289915));
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_NoMakeLevitate_1934289915));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_NoMakeLevitateObs_1934289915));
	}

	// Token: 0x0600073D RID: 1853 RVA: 0x0001B824 File Offset: 0x00019A24
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateLevitatorAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateLevitatorAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x0600073E RID: 1854 RVA: 0x0001B837 File Offset: 0x00019A37
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0600073F RID: 1855 RVA: 0x0001B848 File Offset: 0x00019A48
	private void RpcWriter___Server_sethitsubj_1934289915(GameObject hitsubj)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(hitsubj);
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000740 RID: 1856 RVA: 0x0001B8BA File Offset: 0x00019ABA
	private void RpcLogic___sethitsubj_1934289915(GameObject hitsubj)
	{
		this.obssethitsubj(hitsubj);
	}

	// Token: 0x06000741 RID: 1857 RVA: 0x0001B8C4 File Offset: 0x00019AC4
	private void RpcReader___Server_sethitsubj_1934289915(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___sethitsubj_1934289915(gameObject);
	}

	// Token: 0x06000742 RID: 1858 RVA: 0x0001B8F8 File Offset: 0x00019AF8
	private void RpcWriter___Observers_obssethitsubj_1934289915(GameObject hitsubj)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(hitsubj);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000743 RID: 1859 RVA: 0x0001B979 File Offset: 0x00019B79
	private void RpcLogic___obssethitsubj_1934289915(GameObject hitsubj)
	{
		this.HitSubject = hitsubj;
	}

	// Token: 0x06000744 RID: 1860 RVA: 0x0001B984 File Offset: 0x00019B84
	private void RpcReader___Observers_obssethitsubj_1934289915(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___obssethitsubj_1934289915(gameObject);
	}

	// Token: 0x06000745 RID: 1861 RVA: 0x0001B9B8 File Offset: 0x00019BB8
	private void RpcWriter___Server_MakeLevitate_1934289915(GameObject poopler)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(poopler);
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000746 RID: 1862 RVA: 0x0001BA2A File Offset: 0x00019C2A
	private void RpcLogic___MakeLevitate_1934289915(GameObject poopler)
	{
		this.MakeLevitateObs(poopler);
	}

	// Token: 0x06000747 RID: 1863 RVA: 0x0001BA34 File Offset: 0x00019C34
	private void RpcReader___Server_MakeLevitate_1934289915(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___MakeLevitate_1934289915(gameObject);
	}

	// Token: 0x06000748 RID: 1864 RVA: 0x0001BA68 File Offset: 0x00019C68
	private void RpcWriter___Observers_MakeLevitateObs_1934289915(GameObject poopler)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(poopler);
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x0001BAE9 File Offset: 0x00019CE9
	private void RpcLogic___MakeLevitateObs_1934289915(GameObject poopler)
	{
		this.asource.Play();
		this.isLeving = true;
		poopler.GetComponent<PlayerMovement>().isBeingLevitated = true;
		poopler.GetComponent<PlayerMovement>().LevitationTarget = this.LevitationTargett;
		base.StartCoroutine(this.LineRo());
	}

	// Token: 0x0600074A RID: 1866 RVA: 0x0001BB28 File Offset: 0x00019D28
	private void RpcReader___Observers_MakeLevitateObs_1934289915(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___MakeLevitateObs_1934289915(gameObject);
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x0001BB5C File Offset: 0x00019D5C
	private void RpcWriter___Server_NoMakeLevitate_1934289915(GameObject poopler)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(poopler);
		base.SendServerRpc(4U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600074C RID: 1868 RVA: 0x0001BBCE File Offset: 0x00019DCE
	private void RpcLogic___NoMakeLevitate_1934289915(GameObject poopler)
	{
		this.NoMakeLevitateObs(poopler);
	}

	// Token: 0x0600074D RID: 1869 RVA: 0x0001BBD8 File Offset: 0x00019DD8
	private void RpcReader___Server_NoMakeLevitate_1934289915(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___NoMakeLevitate_1934289915(gameObject);
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x0001BC0C File Offset: 0x00019E0C
	private void RpcWriter___Observers_NoMakeLevitateObs_1934289915(GameObject poopler)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(poopler);
		base.SendObserversRpc(5U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600074F RID: 1871 RVA: 0x0001BC8D File Offset: 0x00019E8D
	private void RpcLogic___NoMakeLevitateObs_1934289915(GameObject poopler)
	{
		this.isLeving = false;
		poopler.GetComponent<PlayerMovement>().isBeingLevitated = false;
		poopler.GetComponent<PlayerMovement>().LevitationTarget = null;
		this.asource.Stop();
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x0001BCBC File Offset: 0x00019EBC
	private void RpcReader___Observers_NoMakeLevitateObs_1934289915(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___NoMakeLevitateObs_1934289915(gameObject);
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x0001B837 File Offset: 0x00019A37
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0400039C RID: 924
	private Transform maincam;

	// Token: 0x0400039D RID: 925
	public GameObject rockrender;

	// Token: 0x0400039E RID: 926
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x0400039F RID: 927
	public bool isHitting;

	// Token: 0x040003A0 RID: 928
	public GameObject HitSubject;

	// Token: 0x040003A1 RID: 929
	public Transform Hitpoint;

	// Token: 0x040003A2 RID: 930
	public LayerMask layermsk;

	// Token: 0x040003A3 RID: 931
	public float maxDistance;

	// Token: 0x040003A4 RID: 932
	public Transform LevitationTargett;

	// Token: 0x040003A5 RID: 933
	private float LevEnergy = 30f;

	// Token: 0x040003A6 RID: 934
	public LineRenderer lineren;

	// Token: 0x040003A7 RID: 935
	public Transform linePoint;

	// Token: 0x040003A8 RID: 936
	private bool isLeving;

	// Token: 0x040003A9 RID: 937
	public AudioSource asource;

	// Token: 0x040003AA RID: 938
	public AudioClip[] aclips;

	// Token: 0x040003AB RID: 939
	private GameObject networkedhitsubj;

	// Token: 0x040003AC RID: 940
	private bool NetworkInitialize___EarlyLevitatorAssembly-CSharp.dll_Excuted;

	// Token: 0x040003AD RID: 941
	private bool NetworkInitialize__LateLevitatorAssembly-CSharp.dll_Excuted;
}
