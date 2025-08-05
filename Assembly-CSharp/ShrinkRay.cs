using System;
using System.Collections;
using Dissonance;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x02000190 RID: 400
public class ShrinkRay : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x060010CE RID: 4302 RVA: 0x00048C0A File Offset: 0x00046E0A
	private void Start()
	{
		this.maincam = Camera.main.transform;
	}

	// Token: 0x060010CF RID: 4303 RVA: 0x00048C1C File Offset: 0x00046E1C
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x060010D0 RID: 4304 RVA: 0x00048C4E File Offset: 0x00046E4E
	public void Interaction(GameObject player)
	{
		base.StartCoroutine(this.ShrinkRoutine());
	}

	// Token: 0x060010D1 RID: 4305 RVA: 0x00048C5D File Offset: 0x00046E5D
	private IEnumerator ShrinkRoutine()
	{
		yield return new WaitForSeconds(0.1f);
		this.Shrinkani.SetBool("pew", true);
		while (Input.GetKey(KeyCode.Mouse0) && Time.time - this.LastShrinkTime > 30f)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(this.maincam.position, this.maincam.forward, out raycastHit, this.maxDistance, this.layermsk))
			{
				this.HitSubject = raycastHit.transform.gameObject;
				this.isHitting = true;
			}
			else
			{
				this.HitSubject = null;
				this.isHitting = false;
			}
			Debug.Log(this.HitSubject);
			GetPlayerGameobject getPlayerGameobject;
			if (this.isHitting && this.HitSubject != null && this.HitSubject.CompareTag("Player") && this.HitSubject.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
			{
				this.MakeShrink(getPlayerGameobject.player);
				break;
			}
			yield return null;
		}
		this.Shrinkani.SetBool("pew", false);
		yield break;
	}

	// Token: 0x060010D2 RID: 4306 RVA: 0x00048C6C File Offset: 0x00046E6C
	[ServerRpc(RequireOwnership = false)]
	private void MakeShrink(GameObject poopler)
	{
		this.RpcWriter___Server_MakeShrink_1934289915(poopler);
	}

	// Token: 0x060010D3 RID: 4307 RVA: 0x00048C78 File Offset: 0x00046E78
	[ObserversRpc]
	private void MakeShrinkObs(GameObject poopler)
	{
		this.RpcWriter___Observers_MakeShrinkObs_1934289915(poopler);
	}

	// Token: 0x060010D4 RID: 4308 RVA: 0x00048C8F File Offset: 0x00046E8F
	private IEnumerator LineRo()
	{
		this.lineren.enabled = true;
		float timer = 0f;
		Vector3 pos2 = this.linePoint.position;
		while (timer < 1f)
		{
			this.lineren.SetPosition(0, this.linePoint.position);
			pos2 = Vector3.Lerp(pos2, new Vector3(this.HitSubject.transform.position.x, this.HitSubject.transform.position.y + 1f, this.HitSubject.transform.position.z), timer);
			this.lineren.SetPosition(1, pos2);
			yield return null;
			timer += Time.deltaTime;
		}
		timer = 0f;
		pos2 = this.linePoint.position;
		while (timer < 1f)
		{
			pos2 = Vector3.Lerp(pos2, new Vector3(this.HitSubject.transform.position.x, this.HitSubject.transform.position.y + 1f, this.HitSubject.transform.position.z), timer);
			this.lineren.SetPosition(0, pos2);
			this.lineren.SetPosition(1, new Vector3(this.HitSubject.transform.position.x, this.HitSubject.transform.position.y + 1f, this.HitSubject.transform.position.z));
			yield return null;
			timer += Time.deltaTime;
		}
		this.lineren.enabled = false;
		yield break;
	}

	// Token: 0x060010D5 RID: 4309 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x060010D6 RID: 4310 RVA: 0x00048CA0 File Offset: 0x00046EA0
	public void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			base.transform.position = raycastHit.point;
		}
		this.rockrender.SetActive(true);
		this.asource.PlayOneShot(this.aclip[2]);
		this.asource.PlayOneShot(this.aclip[3]);
	}

	// Token: 0x060010D7 RID: 4311 RVA: 0x00048D40 File Offset: 0x00046F40
	public void PlayDropSound()
	{
		this.asource.PlayOneShot(this.aclip[2]);
		this.asource.PlayOneShot(this.aclip[3]);
	}

	// Token: 0x060010D8 RID: 4312 RVA: 0x00048D68 File Offset: 0x00046F68
	public void ItemInit()
	{
		this.rockrender.SetActive(true);
		this.asource.PlayOneShot(this.aclip[0]);
		this.asource.PlayOneShot(this.aclip[1]);
	}

	// Token: 0x060010D9 RID: 4313 RVA: 0x00048D68 File Offset: 0x00046F68
	public void ItemInitObs()
	{
		this.rockrender.SetActive(true);
		this.asource.PlayOneShot(this.aclip[0]);
		this.asource.PlayOneShot(this.aclip[1]);
	}

	// Token: 0x060010DA RID: 4314 RVA: 0x00048D9C File Offset: 0x00046F9C
	public void HideItem()
	{
		this.rockrender.SetActive(false);
	}

	// Token: 0x060010DB RID: 4315 RVA: 0x00007BEE File Offset: 0x00005DEE
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
	}

	// Token: 0x060010DC RID: 4316 RVA: 0x00048DAA File Offset: 0x00046FAA
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp Ray of Shrink";
	}

	// Token: 0x060010DD RID: 4317 RVA: 0x00048DB1 File Offset: 0x00046FB1
	public int GetItemID()
	{
		return 14;
	}

	// Token: 0x060010DF RID: 4319 RVA: 0x00048DE4 File Offset: 0x00046FE4
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyShrinkRayAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyShrinkRayAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_MakeShrink_1934289915));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_MakeShrinkObs_1934289915));
	}

	// Token: 0x060010E0 RID: 4320 RVA: 0x00048E30 File Offset: 0x00047030
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateShrinkRayAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateShrinkRayAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x060010E1 RID: 4321 RVA: 0x00048E43 File Offset: 0x00047043
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x060010E2 RID: 4322 RVA: 0x00048E54 File Offset: 0x00047054
	private void RpcWriter___Server_MakeShrink_1934289915(GameObject poopler)
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
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x060010E3 RID: 4323 RVA: 0x00048EC6 File Offset: 0x000470C6
	private void RpcLogic___MakeShrink_1934289915(GameObject poopler)
	{
		this.MakeShrinkObs(poopler);
	}

	// Token: 0x060010E4 RID: 4324 RVA: 0x00048ED0 File Offset: 0x000470D0
	private void RpcReader___Server_MakeShrink_1934289915(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___MakeShrink_1934289915(gameObject);
	}

	// Token: 0x060010E5 RID: 4325 RVA: 0x00048F04 File Offset: 0x00047104
	private void RpcWriter___Observers_MakeShrinkObs_1934289915(GameObject poopler)
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
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x060010E6 RID: 4326 RVA: 0x00048F88 File Offset: 0x00047188
	private void RpcLogic___MakeShrinkObs_1934289915(GameObject poopler)
	{
		PlayerMovement component = poopler.GetComponent<PlayerMovement>();
		component.CallShrinkPlayer();
		this.HitSubject = poopler;
		this.asource.PlayOneShot(this.aclip[4]);
		base.StartCoroutine(this.LineRo());
		if (!component.IsOwner)
		{
			Transform transform = Object.FindFirstObjectByType<DissonanceComms>().transform;
			float num = float.MaxValue;
			Transform transform2 = null;
			foreach (object obj in transform)
			{
				Transform transform3 = (Transform)obj;
				float num2 = Vector3.Distance(transform3.position, poopler.transform.position);
				if (num2 < num)
				{
					num = num2;
					transform2 = transform3;
				}
			}
			RaycastAudioDamper raycastAudioDamper;
			if (transform2 != null && transform2.TryGetComponent<RaycastAudioDamper>(out raycastAudioDamper))
			{
				raycastAudioDamper.ToggleHighpitch();
			}
		}
	}

	// Token: 0x060010E7 RID: 4327 RVA: 0x00049064 File Offset: 0x00047264
	private void RpcReader___Observers_MakeShrinkObs_1934289915(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___MakeShrinkObs_1934289915(gameObject);
	}

	// Token: 0x060010E8 RID: 4328 RVA: 0x00048E43 File Offset: 0x00047043
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x040009B7 RID: 2487
	private Transform maincam;

	// Token: 0x040009B8 RID: 2488
	public GameObject rockrender;

	// Token: 0x040009B9 RID: 2489
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x040009BA RID: 2490
	public bool isHitting;

	// Token: 0x040009BB RID: 2491
	public GameObject HitSubject;

	// Token: 0x040009BC RID: 2492
	public LayerMask layermsk;

	// Token: 0x040009BD RID: 2493
	public float maxDistance;

	// Token: 0x040009BE RID: 2494
	private float LastShrinkTime = -30f;

	// Token: 0x040009BF RID: 2495
	public LineRenderer lineren;

	// Token: 0x040009C0 RID: 2496
	public Transform linePoint;

	// Token: 0x040009C1 RID: 2497
	public Animator Shrinkani;

	// Token: 0x040009C2 RID: 2498
	public AudioSource asource;

	// Token: 0x040009C3 RID: 2499
	public AudioClip[] aclip;

	// Token: 0x040009C4 RID: 2500
	private bool NetworkInitialize___EarlyShrinkRayAssembly-CSharp.dll_Excuted;

	// Token: 0x040009C5 RID: 2501
	private bool NetworkInitialize__LateShrinkRayAssembly-CSharp.dll_Excuted;
}
