using System;
using System.Collections;
using FishNet.Object;
using UnityEngine;

// Token: 0x020001B7 RID: 439
public class TorchController : NetworkBehaviour, IInteractable, IItemInteraction
{
	// Token: 0x06001239 RID: 4665 RVA: 0x0004D5AF File Offset: 0x0004B7AF
	private void Start()
	{
		this.torchsource.Play();
	}

	// Token: 0x0600123A RID: 4666 RVA: 0x0004D5BC File Offset: 0x0004B7BC
	public void SetScale()
	{
		base.transform.localScale = this.Scale;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	// Token: 0x0600123B RID: 4667 RVA: 0x0004D5EE File Offset: 0x0004B7EE
	public void Interaction(GameObject player)
	{
		base.StartCoroutine(this.AttackRoutine(player.GetComponent<PlayerInventory>()));
	}

	// Token: 0x0600123C RID: 4668 RVA: 0x0004D603 File Offset: 0x0004B803
	private IEnumerator AttackRoutine(PlayerInventory playerInv)
	{
		playerInv.armsAni.SetBool("attack", true);
		playerInv.bodyAni.SetBool("attack", true);
		this.BladeTrigger.enabled = true;
		while (Input.GetKey(KeyCode.Mouse0))
		{
			if (this.HitSubject != null)
			{
				GetShadowWizardController getShadowWizardController;
				if (this.HitSubject.CompareTag("Player"))
				{
					GetPlayerGameobject getPlayerGameobject;
					if (this.HitSubject.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
					{
						getPlayerGameobject.player.GetComponent<PlayerMovement>().SetOnFire();
						break;
					}
				}
				else if (this.HitSubject.CompareTag("PlayerNpc") && this.HitSubject.TryGetComponent<GetShadowWizardController>(out getShadowWizardController))
				{
					getShadowWizardController.ShadowWizardAI.GetComponent<ShadowWizardAI>().SetOnFire();
				}
				else
				{
					if (this.HitSubject.CompareTag("brazier"))
					{
						this.HitSubject.GetComponent<BrazierInteract>().LightBrazier();
						break;
					}
					if (this.HitSubject.CompareTag("tutbrazier"))
					{
						this.HitSubject.GetComponent<TutorialBrazzier>().LightBrazier();
						break;
					}
					if (this.HitSubject.CompareTag("RockGround"))
					{
						break;
					}
					if (this.HitSubject.CompareTag("hitable"))
					{
						this.HitSubject.GetComponent<IInteractableNetworkObj>().NetInteract();
						break;
					}
				}
			}
			yield return null;
		}
		this.HitSubject = null;
		playerInv.armsAni.SetBool("attack", false);
		playerInv.bodyAni.SetBool("attack", false);
		yield break;
	}

	// Token: 0x0600123D RID: 4669 RVA: 0x000021EF File Offset: 0x000003EF
	public void Interaction2(GameObject subject)
	{
	}

	// Token: 0x0600123E RID: 4670 RVA: 0x0004D61C File Offset: 0x0004B81C
	public void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			base.transform.position = new Vector3(raycastHit.point.x, raycastHit.point.y + 0.1f, raycastHit.point.z);
		}
		this.torchsource.PlayOneShot(this.torchEquip[2]);
		this.torchRenderer.SetActive(true);
	}

	// Token: 0x0600123F RID: 4671 RVA: 0x0004D6D1 File Offset: 0x0004B8D1
	public void PlayDropSound()
	{
		this.torchsource.PlayOneShot(this.torchEquip[2]);
	}

	// Token: 0x06001240 RID: 4672 RVA: 0x0004D6E6 File Offset: 0x0004B8E6
	public void ItemInit()
	{
		this.torchRenderer.SetActive(true);
		this.torchsource.PlayOneShot(this.torchEquip[0]);
		base.StartCoroutine(this.LerpVolume(0.05f));
	}

	// Token: 0x06001241 RID: 4673 RVA: 0x0004D6E6 File Offset: 0x0004B8E6
	public void ItemInitObs()
	{
		this.torchRenderer.SetActive(true);
		this.torchsource.PlayOneShot(this.torchEquip[0]);
		base.StartCoroutine(this.LerpVolume(0.05f));
	}

	// Token: 0x06001242 RID: 4674 RVA: 0x0004D719 File Offset: 0x0004B919
	public void HideItem()
	{
		this.torchRenderer.SetActive(false);
		base.StartCoroutine(this.LerpVolume(0f));
	}

	// Token: 0x06001243 RID: 4675 RVA: 0x0004D739 File Offset: 0x0004B939
	private IEnumerator LerpVolume(float target)
	{
		float timer = 0f;
		float startvol = this.torchsource.volume;
		while (timer < 0.1f)
		{
			timer += Time.deltaTime;
			this.torchsource.volume = Mathf.Lerp(startvol, target, timer * 10f);
			yield return null;
		}
		yield break;
	}

	// Token: 0x06001244 RID: 4676 RVA: 0x00007BEE File Offset: 0x00005DEE
	public void Interact(GameObject player)
	{
		player.GetComponent<PlayerInventory>().Pickup(base.gameObject);
	}

	// Token: 0x06001245 RID: 4677 RVA: 0x0004D74F File Offset: 0x0004B94F
	public string DisplayInteractUI(GameObject player)
	{
		return "Grasp Torch";
	}

	// Token: 0x06001246 RID: 4678 RVA: 0x0004D756 File Offset: 0x0004B956
	public int GetItemID()
	{
		return 5;
	}

	// Token: 0x06001248 RID: 4680 RVA: 0x0004D77B File Offset: 0x0004B97B
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyTorchControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyTorchControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06001249 RID: 4681 RVA: 0x0004D78E File Offset: 0x0004B98E
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateTorchControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateTorchControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x0600124A RID: 4682 RVA: 0x0004D7A1 File Offset: 0x0004B9A1
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0600124B RID: 4683 RVA: 0x0004D7A1 File Offset: 0x0004B9A1
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000A88 RID: 2696
	public GameObject torchRenderer;

	// Token: 0x04000A89 RID: 2697
	private Transform CamMain;

	// Token: 0x04000A8A RID: 2698
	public Collider BladeTrigger;

	// Token: 0x04000A8B RID: 2699
	public GameObject HitSubject;

	// Token: 0x04000A8C RID: 2700
	public LayerMask layermsk;

	// Token: 0x04000A8D RID: 2701
	private Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x04000A8E RID: 2702
	public AudioSource torchsource;

	// Token: 0x04000A8F RID: 2703
	public AudioClip[] torchEquip;

	// Token: 0x04000A90 RID: 2704
	private bool NetworkInitialize___EarlyTorchControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x04000A91 RID: 2705
	private bool NetworkInitialize__LateTorchControllerAssembly-CSharp.dll_Excuted;
}
