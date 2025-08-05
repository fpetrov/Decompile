using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

// Token: 0x020001A6 RID: 422
public class SpikedRootController : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x06001188 RID: 4488 RVA: 0x0004B409 File Offset: 0x00049609
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x06001189 RID: 4489 RVA: 0x0004B43C File Offset: 0x0004963C
	public void Interaction(GameObject player)
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out raycastHit, this.maxDistance, this.layermsk) && this.charges > 0f)
		{
			this.charges -= 1f;
			this.ServerMakeSpikes(raycastHit.point);
			base.StartCoroutine(this.ReplenishCharge());
		}
	}

	// Token: 0x0600118A RID: 4490 RVA: 0x0004B4BA File Offset: 0x000496BA
	private IEnumerator ReplenishCharge()
	{
		base.StartCoroutine(this.lerptochargecount());
		yield return new WaitForSeconds(30f);
		this.charges += 1f;
		base.StartCoroutine(this.lerptochargecount());
		Mathf.Clamp(this.charges, 0f, 3f);
		yield break;
	}

	// Token: 0x0600118B RID: 4491 RVA: 0x0004B4C9 File Offset: 0x000496C9
	private IEnumerator lerptochargecount()
	{
		this.lerpingcharges = false;
		yield return null;
		this.lerpingcharges = true;
		float timer = 0f;
		while (timer < 1f && this.lerpingcharges)
		{
			timer += Time.deltaTime;
			if (this.charges == 0f)
			{
				this.vinething.localScale = Vector3.Lerp(this.vinething.localScale, new Vector3(10f, 10f, 10f), timer);
			}
			else if (this.charges == 1f)
			{
				this.vinething.localScale = Vector3.Lerp(this.vinething.localScale, new Vector3(30f, 30f, 30f), timer);
			}
			else if (this.charges == 2f)
			{
				this.vinething.localScale = Vector3.Lerp(this.vinething.localScale, new Vector3(45f, 45f, 45f), timer);
			}
			else if (this.charges == 3f)
			{
				this.vinething.localScale = Vector3.Lerp(this.vinething.localScale, new Vector3(59.75f, 59.75f, 59.75f), timer);
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600118C RID: 4492 RVA: 0x0004B4D8 File Offset: 0x000496D8
	[ServerRpc(RequireOwnership = false)]
	private void ServerMakeSpikes(Vector3 centerpoint)
	{
		this.RpcWriter___Server_ServerMakeSpikes_4276783012(centerpoint);
	}

	// Token: 0x0600118D RID: 4493 RVA: 0x0004B4E4 File Offset: 0x000496E4
	[ObserversRpc]
	private void obsmakespikes(Vector3 centerpoint)
	{
		this.RpcWriter___Observers_obsmakespikes_4276783012(centerpoint);
	}

	// Token: 0x0600118E RID: 4494 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x0600118F RID: 4495 RVA: 0x0004B4FC File Offset: 0x000496FC
	public void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			base.transform.position = raycastHit.point;
		}
		this.asourc.PlayOneShot(this.clis[1]);
		this.rockrender.SetActive(true);
	}

	// Token: 0x06001190 RID: 4496 RVA: 0x0004B589 File Offset: 0x00049789
	public void PlayDropSound()
	{
		this.asourc.PlayOneShot(this.clis[1]);
	}

	// Token: 0x06001191 RID: 4497 RVA: 0x0004B59E File Offset: 0x0004979E
	public void ItemInit()
	{
		this.rockrender.SetActive(true);
		this.asourc.PlayOneShot(this.clis[0]);
	}

	// Token: 0x06001192 RID: 4498 RVA: 0x0004B59E File Offset: 0x0004979E
	public void ItemInitObs()
	{
		this.rockrender.SetActive(true);
		this.asourc.PlayOneShot(this.clis[0]);
	}

	// Token: 0x06001193 RID: 4499 RVA: 0x0004B5BF File Offset: 0x000497BF
	public void HideItem()
	{
		this.rockrender.SetActive(false);
	}

	// Token: 0x06001194 RID: 4500 RVA: 0x00007BEE File Offset: 0x00005DEE
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
	}

	// Token: 0x06001195 RID: 4501 RVA: 0x0004B5CD File Offset: 0x000497CD
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp Silverseed Bramble";
	}

	// Token: 0x06001196 RID: 4502 RVA: 0x0004B5D4 File Offset: 0x000497D4
	public int GetItemID()
	{
		return 20;
	}

	// Token: 0x06001198 RID: 4504 RVA: 0x0004B610 File Offset: 0x00049810
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlySpikedRootControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlySpikedRootControllerAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_ServerMakeSpikes_4276783012));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_obsmakespikes_4276783012));
	}

	// Token: 0x06001199 RID: 4505 RVA: 0x0004B65C File Offset: 0x0004985C
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateSpikedRootControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateSpikedRootControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x0600119A RID: 4506 RVA: 0x0004B66F File Offset: 0x0004986F
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0600119B RID: 4507 RVA: 0x0004B680 File Offset: 0x00049880
	private void RpcWriter___Server_ServerMakeSpikes_4276783012(Vector3 centerpoint)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(centerpoint);
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x0600119C RID: 4508 RVA: 0x0004B6F2 File Offset: 0x000498F2
	private void RpcLogic___ServerMakeSpikes_4276783012(Vector3 centerpoint)
	{
		this.obsmakespikes(centerpoint);
	}

	// Token: 0x0600119D RID: 4509 RVA: 0x0004B6FC File Offset: 0x000498FC
	private void RpcReader___Server_ServerMakeSpikes_4276783012(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerMakeSpikes_4276783012(vector);
	}

	// Token: 0x0600119E RID: 4510 RVA: 0x0004B730 File Offset: 0x00049930
	private void RpcWriter___Observers_obsmakespikes_4276783012(Vector3 centerpoint)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteVector3(centerpoint);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x0600119F RID: 4511 RVA: 0x0004B7B4 File Offset: 0x000499B4
	private void RpcLogic___obsmakespikes_4276783012(Vector3 centerpoint)
	{
		for (int i = -2; i < 3; i++)
		{
			for (int j = -2; j < 3; j++)
			{
				float num;
				if (Mathf.Abs(i) == 2)
				{
					num = (float)j / 2f;
				}
				else if (Mathf.Abs(i) == 1)
				{
					num = (float)j / 1.33f;
				}
				else
				{
					num = (float)j / 1f;
				}
				RaycastHit raycastHit;
				if (Physics.Raycast(new Vector3(centerpoint.x + num + Random.Range(-0.2f, 0.2f), centerpoint.y + 8f, centerpoint.z + (float)i + Random.Range(-0.2f, 0.2f)), Vector3.down, out raycastHit, 25f, this.layermsk))
				{
					if (j == 0 && i == 0)
					{
						Object.Instantiate<GameObject>(this.spikes[this.spikes.Length - 1], raycastHit.point, Quaternion.Euler(0f, (float)Random.Range(0, 360), 0f));
					}
					else
					{
						Object.Instantiate<GameObject>(this.spikes[Random.Range(0, this.spikes.Length - 1)], raycastHit.point, Quaternion.Euler(0f, (float)Random.Range(0, 360), 0f));
					}
				}
			}
		}
	}

	// Token: 0x060011A0 RID: 4512 RVA: 0x0004B8FC File Offset: 0x00049AFC
	private void RpcReader___Observers_obsmakespikes_4276783012(PooledReader PooledReader0, Channel channel)
	{
		Vector3 vector = PooledReader0.ReadVector3();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___obsmakespikes_4276783012(vector);
	}

	// Token: 0x060011A1 RID: 4513 RVA: 0x0004B66F File Offset: 0x0004986F
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000A2B RID: 2603
	public GameObject rockrender;

	// Token: 0x04000A2C RID: 2604
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x04000A2D RID: 2605
	public float maxDistance = 15f;

	// Token: 0x04000A2E RID: 2606
	public LayerMask layermsk;

	// Token: 0x04000A2F RID: 2607
	public GameObject[] spikes;

	// Token: 0x04000A30 RID: 2608
	public AudioSource asourc;

	// Token: 0x04000A31 RID: 2609
	public AudioClip[] clis;

	// Token: 0x04000A32 RID: 2610
	private float charges = 3f;

	// Token: 0x04000A33 RID: 2611
	private bool lerpingcharges;

	// Token: 0x04000A34 RID: 2612
	public Transform vinething;

	// Token: 0x04000A35 RID: 2613
	private bool NetworkInitialize___EarlySpikedRootControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x04000A36 RID: 2614
	private bool NetworkInitialize__LateSpikedRootControllerAssembly-CSharp.dll_Excuted;
}
