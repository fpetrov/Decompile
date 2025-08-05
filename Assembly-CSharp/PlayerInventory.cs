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
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x02000112 RID: 274
public class PlayerInventory : NetworkBehaviour
{
	// Token: 0x06000AED RID: 2797 RVA: 0x0002A116 File Offset: 0x00028316
	public void setsbk(GameObject spellbook)
	{
		this.myspellbook = spellbook;
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x0002A120 File Offset: 0x00028320
	public override void OnStartClient()
	{
		base.OnStartClient();
		this.worldObjectHolder = GameObject.FindGameObjectWithTag("WorldObjects").transform;
		if (!base.IsOwner)
		{
			this.pickupPosition.SetParent(this.AlternatePickupParent);
			this.pickupPosition.localPosition = new Vector3(0.13f, 0.52f, 0.13f);
			this.pickupPosition.localRotation = Quaternion.Euler(-14f, -8f, 0f);
			base.enabled = false;
			return;
		}
		this.Sholder = GameObject.FindGameObjectWithTag("Settings").GetComponent<SettingsHolder>();
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x0002A1BC File Offset: 0x000283BC
	public void InitInv()
	{
		for (int i = 0; i < 5; i++)
		{
			this.UIImages[this.equippedIndex] = GameObject.FindGameObjectWithTag("invui").transform.GetChild(i).GetComponent<Image>();
			this.equippedIndex++;
		}
		this.equippedIndex = 0;
		this.initedInv = true;
		this.pi = base.GetComponentInChildren<PlayerInteract>();
	}

	// Token: 0x06000AF0 RID: 2800 RVA: 0x0002A224 File Offset: 0x00028424
	public void cPageSpell()
	{
		PlayerMovement component = base.GetComponent<PlayerMovement>();
		PageController pageController;
		if (this.equippedItems[this.equippedIndex] != null && this.equippedItems[this.equippedIndex].TryGetComponent<PageController>(out pageController) && Time.time - pageController.PageCoolDownTimer > Mathf.Clamp(pageController.CoolDown - (float)component.level, 3f, 120f) && !component.isDead && this.initedInv)
		{
			pageController.PageCoolDownTimer = Time.time - (float)component.crystalCDReduction;
			pageController.CastSpell(base.gameObject, component.level);
			base.StartCoroutine(this.ReinstatePageEmissive(pageController));
			component.spellscasted += 1f;
		}
	}

	// Token: 0x06000AF1 RID: 2801 RVA: 0x0002A2E9 File Offset: 0x000284E9
	private IEnumerator ReinstatePageEmissive(PageController page)
	{
		PlayerMovement component = base.GetComponent<PlayerMovement>();
		yield return new WaitForSeconds(page.CoolDown - (float)component.crystalCDReduction - (float)component.level);
		if (page != null)
		{
			page.ReinstatePageEmis();
		}
		yield break;
	}

	// Token: 0x06000AF2 RID: 2802 RVA: 0x0002A300 File Offset: 0x00028500
	public void cFireball()
	{
		PlayerMovement component = base.GetComponent<PlayerMovement>();
		MageBookController mageBookController;
		if (this.equippedItems[this.equippedIndex] != null && this.equippedItems[this.equippedIndex].TryGetComponent<MageBookController>(out mageBookController) && mageBookController.LastPressedPage == 1 && Time.time - this.fbcd > 6f && !component.isDead && this.initedInv)
		{
			this.fbcd = Time.time - (float)component.crystalCDReduction;
			mageBookController.Fireball(base.gameObject, component.level);
			component.ShakeCam(0.25f, 0.25f);
			component.applyrecoil();
			component.Fireballoverlay();
			component.spellscasted += 1f;
			base.StartCoroutine(this.ReinstateFBEmissive(mageBookController));
		}
	}

	// Token: 0x06000AF3 RID: 2803 RVA: 0x0002A3D5 File Offset: 0x000285D5
	private IEnumerator ReinstateFBEmissive(MageBookController mgbk)
	{
		PlayerMovement component = base.GetComponent<PlayerMovement>();
		yield return new WaitForSeconds((float)(6 - component.crystalCDReduction));
		mgbk.ReinstateFireballEmis();
		yield break;
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x0002A3EC File Offset: 0x000285EC
	public void cFrostbolt()
	{
		PlayerMovement component = base.GetComponent<PlayerMovement>();
		MageBookController mageBookController;
		if (this.equippedItems[this.equippedIndex] != null && this.equippedItems[this.equippedIndex].TryGetComponent<MageBookController>(out mageBookController) && mageBookController.LastPressedPage == 2 && Time.time - this.frostcd > 8f && !component.isDead && this.initedInv)
		{
			this.frostcd = Time.time - (float)component.crystalCDReduction;
			mageBookController.Frostbolt(base.gameObject, component.level);
			base.StartCoroutine(this.ReinstateFrbEmissive(mageBookController));
			component.spellscasted += 1f;
		}
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x0002A49F File Offset: 0x0002869F
	private IEnumerator ReinstateFrbEmissive(MageBookController mgbk)
	{
		PlayerMovement component = base.GetComponent<PlayerMovement>();
		yield return new WaitForSeconds((float)(8 - component.crystalCDReduction));
		mgbk.ReinstateFrostBoltEmis();
		yield break;
	}

	// Token: 0x06000AF6 RID: 2806 RVA: 0x0002A4B8 File Offset: 0x000286B8
	public void cCastworm()
	{
		PlayerMovement component = base.GetComponent<PlayerMovement>();
		MageBookController mageBookController;
		if (this.equippedItems[this.equippedIndex] != null && this.equippedItems[this.equippedIndex].TryGetComponent<MageBookController>(out mageBookController) && mageBookController.LastPressedPage == 3 && Time.time - this.wormcd > 25f && !component.isDead && this.initedInv)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(mageBookController.firePoint.position + base.transform.forward, Vector3.down, out raycastHit, 15f, this.ground))
			{
				Vector3 vector = new Vector3(raycastHit.point.x, raycastHit.point.y + 3f, raycastHit.point.z);
				if (!Physics.CheckSphere(vector, 0.5f, this.forcefield))
				{
					this.wormcd = Time.time - (float)component.crystalCDReduction;
					mageBookController.Castworm(component.level, vector);
					base.StartCoroutine(this.ReinstateWormEmissive(mageBookController));
					component.spellscasted += 1f;
					return;
				}
				Camera.main.GetComponent<PlayerInteract>().leveluptxt("A strong magical presence dispells your worm...");
				mageBookController.FlickerWormEmis();
				return;
			}
			else
			{
				Camera.main.GetComponent<PlayerInteract>().leveluptxt("A strong magical presence dispells your worm...");
				mageBookController.FlickerWormEmis();
			}
		}
	}

	// Token: 0x06000AF7 RID: 2807 RVA: 0x0002A630 File Offset: 0x00028830
	private IEnumerator ReinstateWormEmissive(MageBookController mgbk)
	{
		PlayerMovement component = base.GetComponent<PlayerMovement>();
		yield return new WaitForSeconds((float)(45 - component.crystalCDReduction));
		mgbk.ReinstateWormEmis();
		yield break;
	}

	// Token: 0x06000AF8 RID: 2808 RVA: 0x0002A648 File Offset: 0x00028848
	public void cCasthole()
	{
		PlayerMovement component = base.GetComponent<PlayerMovement>();
		MageBookController mageBookController;
		if (this.equippedItems[this.equippedIndex] != null && this.equippedItems[this.equippedIndex].TryGetComponent<MageBookController>(out mageBookController) && mageBookController.LastPressedPage == 3 && Time.time - this.holecd > 25f && !component.isDead && this.initedInv)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(mageBookController.firePoint.position + base.transform.forward, Vector3.down, out raycastHit, 15f, this.ground))
			{
				Vector3 vector = new Vector3(raycastHit.point.x, raycastHit.point.y + 3f, raycastHit.point.z);
				if (!Physics.CheckSphere(vector, 0.5f, this.forcefield))
				{
					this.holecd = Time.time - (float)component.crystalCDReduction;
					mageBookController.Casthole(component.level, vector);
					base.StartCoroutine(this.ReinstateHoleEmissive(mageBookController));
					component.spellscasted += 1f;
					return;
				}
				Camera.main.GetComponent<PlayerInteract>().leveluptxt("A strong magical presence dispells your hole...");
				mageBookController.FlickerHoleEmis();
				return;
			}
			else
			{
				Camera.main.GetComponent<PlayerInteract>().leveluptxt("A strong magical presence dispells your hole...");
				mageBookController.FlickerHoleEmis();
			}
		}
	}

	// Token: 0x06000AF9 RID: 2809 RVA: 0x0002A7C0 File Offset: 0x000289C0
	private IEnumerator ReinstateHoleEmissive(MageBookController mgbk)
	{
		PlayerMovement component = base.GetComponent<PlayerMovement>();
		yield return new WaitForSeconds((float)(45 - component.crystalCDReduction));
		mgbk.ReinstateHoleEmis();
		yield break;
	}

	// Token: 0x06000AFA RID: 2810 RVA: 0x0002A7D8 File Offset: 0x000289D8
	public void cCastWard()
	{
		PlayerMovement component = base.GetComponent<PlayerMovement>();
		MageBookController mageBookController;
		if (this.equippedItems[this.equippedIndex] != null && this.equippedItems[this.equippedIndex].TryGetComponent<MageBookController>(out mageBookController) && mageBookController.LastPressedPage == 4 && Time.time - this.wardcd > 6f && !component.isDead && this.initedInv)
		{
			this.wardcd = Time.time - (float)component.crystalCDReduction;
			mageBookController.CastWard(base.gameObject, component.level);
			base.StartCoroutine(this.ReinstateWardEmissive(mageBookController));
			component.spellscasted += 1f;
		}
	}

	// Token: 0x06000AFB RID: 2811 RVA: 0x0002A88B File Offset: 0x00028A8B
	private IEnumerator ReinstateWardEmissive(MageBookController mgbk)
	{
		PlayerMovement component = base.GetComponent<PlayerMovement>();
		yield return new WaitForSeconds((float)(6 - component.crystalCDReduction));
		mgbk.ReinstateWardEmis();
		yield break;
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x0002A8A1 File Offset: 0x00028AA1
	public void SetSpawnItems()
	{
		this.SpawnSpawnItems();
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x0002A8AC File Offset: 0x00028AAC
	[ServerRpc(RequireOwnership = false)]
	private void SpawnSpawnItems()
	{
		this.RpcWriter___Server_SpawnSpawnItems_2166136261();
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x0002A8BF File Offset: 0x00028ABF
	[ObserversRpc]
	private void GetSpawnedItems(GameObject i1, GameObject i2)
	{
		this.RpcWriter___Observers_GetSpawnedItems_1401332417(i1, i2);
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x0002A8D0 File Offset: 0x00028AD0
	private void Update()
	{
		if (this.initedInv)
		{
			base.gameObject.GetComponent<PlayerMovement>().HoldingStickid = 0;
			IItemInteraction itemInteraction;
			if (this.equippedItems[this.equippedIndex] != null && this.equippedItems[this.equippedIndex].TryGetComponent<IItemInteraction>(out itemInteraction))
			{
				if (itemInteraction.GetItemID() == 12)
				{
					base.gameObject.GetComponent<PlayerMovement>().HoldingStickid = 12;
				}
				else if (itemInteraction.GetItemID() == 11)
				{
					base.gameObject.GetComponent<PlayerMovement>().HoldingStickid = 11;
				}
				else if (this.equippedItems[this.equippedIndex].GetComponent<IItemInteraction>().GetItemID() == 23)
				{
					base.gameObject.GetComponent<PlayerMovement>().HoldingStickid = 23;
					if (base.gameObject.GetComponent<PlayerMovement>().velocity.y < 15f)
					{
						PlayerMovement component = base.gameObject.GetComponent<PlayerMovement>();
						component.velocity.y = component.velocity.y + Time.deltaTime * 17f;
					}
				}
			}
			if (Input.GetKeyDown(this.Sholder.drop))
			{
				this.Drop();
			}
			if (Input.GetKeyDown(this.Sholder.leftmouse) && this.equippedItems[this.equippedIndex] != null && this.canUseItem)
			{
				IItemInteraction itemInteraction2;
				if (this.equippedItems[this.equippedIndex].TryGetComponent<IItemInteraction>(out itemInteraction2))
				{
					itemInteraction2.Interaction(base.gameObject);
					return;
				}
			}
			else if (Input.GetAxis("Mouse ScrollWheel") < 0f && Time.time - this.cooldown > 0.1f && this.canSwapItem)
			{
				base.StartCoroutine(this.activateHotbar());
				this.cooldown = Time.time;
				if (this.equippedItems[this.equippedIndex] != null)
				{
					if (this.equippedIndex < 4)
					{
						if (this.equippedItems[this.equippedIndex + 1] != null)
						{
							this.equippedItems[this.equippedIndex].GetComponent<IItemInteraction>().HideItem();
						}
					}
					else if (this.equippedItems[0] != null)
					{
						this.equippedItems[this.equippedIndex].GetComponent<IItemInteraction>().HideItem();
					}
					this.HideObject(this.equippedItems[this.equippedIndex]);
				}
				if (this.equippedIndex < 4)
				{
					this.equippedIndex++;
				}
				else
				{
					this.equippedIndex = 0;
				}
				if (this.equippedItems[this.equippedIndex] != null)
				{
					base.StartCoroutine(this.LayerMaskSwapOne());
					this.equippedItems[this.equippedIndex].GetComponent<IItemInteraction>().ItemInit();
					this.SetObjectInHandServer(this.equippedItems[this.equippedIndex]);
					return;
				}
				base.StartCoroutine(this.LayerMaskSwapZero());
				return;
			}
			else if (Input.GetAxis("Mouse ScrollWheel") > 0f && Time.time - this.cooldown > 0.1f && this.canSwapItem)
			{
				base.StartCoroutine(this.activateHotbar());
				this.cooldown = Time.time;
				if (this.equippedItems[this.equippedIndex] != null)
				{
					if (this.equippedIndex > 0)
					{
						if (this.equippedItems[this.equippedIndex - 1] != null)
						{
							this.equippedItems[this.equippedIndex].GetComponent<IItemInteraction>().HideItem();
						}
					}
					else if (this.equippedItems[4] != null)
					{
						this.equippedItems[this.equippedIndex].GetComponent<IItemInteraction>().HideItem();
					}
					this.HideObject(this.equippedItems[this.equippedIndex]);
				}
				if (this.equippedIndex > 0)
				{
					this.equippedIndex--;
				}
				else
				{
					this.equippedIndex = 4;
				}
				if (this.equippedItems[this.equippedIndex] != null)
				{
					base.StartCoroutine(this.LayerMaskSwapOne());
					this.equippedItems[this.equippedIndex].GetComponent<IItemInteraction>().ItemInit();
					this.SetObjectInHandServer(this.equippedItems[this.equippedIndex]);
					return;
				}
				base.StartCoroutine(this.LayerMaskSwapZero());
				return;
			}
			else if (Input.GetKeyDown(this.Sholder.item1) || Input.GetKeyDown(this.Sholder.item2) || Input.GetKeyDown(this.Sholder.item3) || Input.GetKeyDown(this.Sholder.item4) || (Input.GetKeyDown(this.Sholder.item5) && Time.time - this.cooldown > 0.1f && this.canSwapItem))
			{
				base.StartCoroutine(this.activateHotbar());
				this.cooldown = Time.time;
				int num = 0;
				if (Input.GetKeyDown(this.Sholder.item2))
				{
					num = 1;
				}
				else if (Input.GetKeyDown(this.Sholder.item3))
				{
					num = 2;
				}
				else if (Input.GetKeyDown(this.Sholder.item4))
				{
					num = 3;
				}
				else if (Input.GetKeyDown(this.Sholder.item5))
				{
					num = 4;
				}
				if (num != this.equippedIndex)
				{
					if (this.equippedItems[this.equippedIndex] != null)
					{
						this.equippedItems[this.equippedIndex].GetComponent<IItemInteraction>().HideItem();
						this.HideObject(this.equippedItems[this.equippedIndex]);
					}
					this.equippedIndex = num;
					if (this.equippedItems[this.equippedIndex] != null)
					{
						base.StartCoroutine(this.LayerMaskSwapOne());
						this.equippedItems[this.equippedIndex].GetComponent<IItemInteraction>().ItemInit();
						this.SetObjectInHandServer(this.equippedItems[this.equippedIndex]);
						return;
					}
					base.StartCoroutine(this.LayerMaskSwapZero());
					return;
				}
			}
			else if (Time.time - this.cooldown > 4f)
			{
				this.cooldown = Time.time;
				base.StartCoroutine(this.deactivateHotbar());
			}
		}
	}

	// Token: 0x06000B00 RID: 2816 RVA: 0x0002AEAC File Offset: 0x000290AC
	public void SetSpawnSlotToTwo()
	{
		base.StartCoroutine(this.activateHotbar());
		this.cooldown = Time.time;
		int num = 1;
		if (num != this.equippedIndex)
		{
			if (this.equippedItems[this.equippedIndex] != null)
			{
				this.equippedItems[this.equippedIndex].GetComponent<IItemInteraction>().HideItem();
				this.HideObject(this.equippedItems[this.equippedIndex]);
			}
			this.equippedIndex = num;
			if (this.equippedItems[this.equippedIndex] != null)
			{
				base.StartCoroutine(this.LayerMaskSwapOne());
				this.equippedItems[this.equippedIndex].GetComponent<IItemInteraction>().ItemInit();
				this.SetObjectInHandServer(this.equippedItems[this.equippedIndex]);
				return;
			}
			base.StartCoroutine(this.LayerMaskSwapZero());
		}
	}

	// Token: 0x06000B01 RID: 2817 RVA: 0x0002AF7F File Offset: 0x0002917F
	public void SwapItemImg(int slotid, int itemid)
	{
		if (itemid >= 1000)
		{
			itemid = 35;
		}
		this.UIImages[slotid].transform.GetChild(0).GetComponent<Image>().sprite = this.ItemIcons[itemid];
	}

	// Token: 0x06000B02 RID: 2818 RVA: 0x0002AFB2 File Offset: 0x000291B2
	private void ClearItemImg(int slotid)
	{
		this.UIImages[slotid].transform.GetChild(0).GetComponent<Image>().sprite = this.blank;
	}

	// Token: 0x06000B03 RID: 2819 RVA: 0x0002AFD7 File Offset: 0x000291D7
	private IEnumerator LayerMaskSwapOne()
	{
		if (this.armsAni.GetLayerWeight(1) != 1f)
		{
			float timer = 0f;
			while (timer < 0.09f)
			{
				timer += Time.deltaTime;
				this.armsAni.SetLayerWeight(1, timer * 12.5f);
				yield return null;
			}
			this.armsAni.SetLayerWeight(1, 1f);
			this.bodyAni.SetLayerWeight(1, 1f);
		}
		yield break;
	}

	// Token: 0x06000B04 RID: 2820 RVA: 0x0002AFE6 File Offset: 0x000291E6
	private IEnumerator LayerMaskSwapZero()
	{
		if (this.armsAni.GetLayerWeight(1) != 0f)
		{
			float timer = 0f;
			while (timer < 0.09f)
			{
				timer += Time.deltaTime;
				this.armsAni.SetLayerWeight(1, 1f - timer * 12.5f);
				yield return null;
			}
			this.armsAni.SetLayerWeight(1, 0f);
			this.bodyAni.SetLayerWeight(1, 0f);
		}
		yield break;
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x0002AFF5 File Offset: 0x000291F5
	private IEnumerator activateHotbar()
	{
		Color color = new Color(0.39215687f, 0.39215687f, 0.39215687f, 0.9f);
		foreach (Image image in this.UIImages)
		{
			image.rectTransform.sizeDelta = new Vector2(133f, 110f);
			image.color = color;
			image.transform.GetChild(0).GetComponent<Image>().rectTransform.sizeDelta = new Vector2(120f, 100f);
			image.transform.GetChild(0).GetComponent<Image>().color = new Color(0.78431374f, 0.78431374f, 0.78431374f, 0.9f);
		}
		float elapsedTime = 0f;
		while (elapsedTime < 0.09f)
		{
			this.UIImages[this.equippedIndex].rectTransform.sizeDelta = Vector2.Lerp(new Vector2(133f, 110f), new Vector2(160f, 133f), elapsedTime / 0.09f);
			this.UIImages[this.equippedIndex].transform.GetChild(0).GetComponent<Image>().rectTransform.sizeDelta = Vector2.Lerp(new Vector2(120f, 100f), new Vector2(145.45f, 120.9f), elapsedTime / 0.09f);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		this.UIImages[this.equippedIndex].rectTransform.sizeDelta = new Vector2(160f, 133f);
		yield break;
	}

	// Token: 0x06000B06 RID: 2822 RVA: 0x0002B004 File Offset: 0x00029204
	private IEnumerator deactivateHotbar()
	{
		Color color = new Color(0.39215687f, 0.39215687f, 0.39215687f, 0.4f);
		foreach (Image image in this.UIImages)
		{
			image.color = color;
			image.transform.GetChild(0).GetComponent<Image>().color = color;
		}
		float elapsedTime = 0f;
		while (elapsedTime < 0.09f)
		{
			this.UIImages[this.equippedIndex].rectTransform.sizeDelta = Vector2.Lerp(new Vector2(160f, 133f), new Vector2(133f, 110f), elapsedTime / 0.09f);
			this.UIImages[this.equippedIndex].transform.GetChild(0).GetComponent<Image>().rectTransform.sizeDelta = Vector2.Lerp(new Vector2(145.45f, 120.9f), new Vector2(120f, 100f), elapsedTime / 0.09f);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		this.UIImages[this.equippedIndex].rectTransform.sizeDelta = new Vector2(133f, 110f);
		yield break;
	}

	// Token: 0x06000B07 RID: 2823 RVA: 0x0002B014 File Offset: 0x00029214
	public void PickupToItemSlot(GameObject itemObject, int slot)
	{
		base.StartCoroutine(this.TempDisableItemSwaprout());
		if (slot == 0)
		{
			this.equippedItems[slot] = itemObject;
			IItemInteraction component = this.equippedItems[slot].GetComponent<IItemInteraction>();
			component.ItemInit();
			this.SetObjectInHandServer(this.equippedItems[slot]);
			base.StartCoroutine(this.LayerMaskSwapOne());
			this.SwapItemImg(slot, component.GetItemID());
			base.StartCoroutine(this.activateHotbar());
			return;
		}
		if (slot == 4)
		{
			this.equippedItems[slot] = itemObject;
			this.SwapItemImg(slot, this.equippedItems[slot].GetComponent<IItemInteraction>().GetItemID());
			base.StartCoroutine(this.activateHotbar());
			this.HideObject(itemObject);
		}
	}

	// Token: 0x06000B08 RID: 2824 RVA: 0x0002B0C0 File Offset: 0x000292C0
	public void Pickup(GameObject itemObject)
	{
		base.StartCoroutine(this.TempDisableItemSwaprout());
		if (this.equippedItems[this.equippedIndex] == null)
		{
			this.equippedItems[this.equippedIndex] = itemObject;
			IItemInteraction component = this.equippedItems[this.equippedIndex].GetComponent<IItemInteraction>();
			component.ItemInit();
			this.SetObjectInHandServer(this.equippedItems[this.equippedIndex]);
			base.StartCoroutine(this.LayerMaskSwapOne());
			this.SwapItemImg(this.equippedIndex, component.GetItemID());
			base.StartCoroutine(this.activateHotbar());
			return;
		}
		int num;
		if ((num = this.tryFindSlot()) != -1)
		{
			this.equippedItems[num] = itemObject;
			this.SwapItemImg(num, this.equippedItems[num].GetComponent<IItemInteraction>().GetItemID());
			base.StartCoroutine(this.activateHotbar());
			this.HideObject(itemObject);
		}
	}

	// Token: 0x06000B09 RID: 2825 RVA: 0x0002B19C File Offset: 0x0002939C
	public void SpecialPickup(GameObject itemObject)
	{
		base.StartCoroutine(this.TempDisableItemSwaprout());
		if (this.equippedItems[this.equippedIndex] == null)
		{
			this.equippedItems[this.equippedIndex] = itemObject;
			IItemInteraction component = this.equippedItems[this.equippedIndex].GetComponent<IItemInteraction>();
			this.SwapItemImg(this.equippedIndex, component.GetItemID());
			this.equippedItems[this.equippedIndex].GetComponent<MageBookController>().HideItem();
			return;
		}
		int num;
		if ((num = this.tryFindSlot()) != -1)
		{
			this.equippedItems[num] = itemObject;
			this.SwapItemImg(num, this.equippedItems[num].GetComponent<IItemInteraction>().GetItemID());
			this.equippedItems[num].GetComponent<MageBookController>().HideItem();
		}
	}

	// Token: 0x06000B0A RID: 2826 RVA: 0x0002B258 File Offset: 0x00029458
	public int tryFindSlot()
	{
		for (int i = 0; i < this.equippedItems.Length; i++)
		{
			if (this.equippedItems[i] == null)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000B0B RID: 2827 RVA: 0x0002B28B File Offset: 0x0002948B
	[ServerRpc(RequireOwnership = false)]
	private void HideObject(GameObject objToHide)
	{
		this.RpcWriter___Server_HideObject_1934289915(objToHide);
	}

	// Token: 0x06000B0C RID: 2828 RVA: 0x0002B298 File Offset: 0x00029498
	[ObserversRpc]
	private void HideObjectObserver(GameObject objToHide)
	{
		this.RpcWriter___Observers_HideObjectObserver_1934289915(objToHide);
	}

	// Token: 0x06000B0D RID: 2829 RVA: 0x0002B2AF File Offset: 0x000294AF
	[ServerRpc(RequireOwnership = false)]
	private void SetObjectInHandServer(GameObject obj)
	{
		this.RpcWriter___Server_SetObjectInHandServer_1934289915(obj);
	}

	// Token: 0x06000B0E RID: 2830 RVA: 0x0002B2BC File Offset: 0x000294BC
	[ObserversRpc]
	private void SetObjectInHandObserver(GameObject obj)
	{
		this.RpcWriter___Observers_SetObjectInHandObserver_1934289915(obj);
	}

	// Token: 0x06000B0F RID: 2831 RVA: 0x0002B2D4 File Offset: 0x000294D4
	public void Drop()
	{
		if (this.equippedItems[this.equippedIndex] != null)
		{
			base.StartCoroutine(this.LayerMaskSwapZero());
			this.DropObjectServer(this.equippedItems[this.equippedIndex]);
			this.equippedItems[this.equippedIndex] = null;
			this.ClearItemImg(this.equippedIndex);
		}
	}

	// Token: 0x06000B10 RID: 2832 RVA: 0x0002B330 File Offset: 0x00029530
	public void PlaceOnCraftingTable(GameObject CrIn)
	{
		if (this.equippedItems[this.equippedIndex] != null)
		{
			this.TryPlaceOnCraftingTableServer(this.equippedItems[this.equippedIndex], CrIn);
		}
	}

	// Token: 0x06000B11 RID: 2833 RVA: 0x0002B35C File Offset: 0x0002955C
	[ServerRpc(RequireOwnership = false)]
	private void TryPlaceOnCraftingTableServer(GameObject obj, GameObject CrIn)
	{
		this.RpcWriter___Server_TryPlaceOnCraftingTableServer_1401332417(obj, CrIn);
	}

	// Token: 0x06000B12 RID: 2834 RVA: 0x0002B378 File Offset: 0x00029578
	[ObserversRpc]
	private void PlaceOnCraftingTableObserver(GameObject obj, GameObject CrIn)
	{
		this.RpcWriter___Observers_PlaceOnCraftingTableObserver_1401332417(obj, CrIn);
	}

	// Token: 0x06000B13 RID: 2835 RVA: 0x0002B394 File Offset: 0x00029594
	public void PickupCraftingTable(GameObject CrIn)
	{
		if (this.equippedItems[this.equippedIndex] == null)
		{
			CraftInteracter component = CrIn.GetComponent<CraftInteracter>();
			this.TryPickupCraftingTableServer(component.craftingForge.SlotItems[component.slotID], CrIn);
		}
	}

	// Token: 0x06000B14 RID: 2836 RVA: 0x0002B3D8 File Offset: 0x000295D8
	[ServerRpc(RequireOwnership = false)]
	private void TryPickupCraftingTableServer(GameObject obj, GameObject CrIn)
	{
		this.RpcWriter___Server_TryPickupCraftingTableServer_1401332417(obj, CrIn);
	}

	// Token: 0x06000B15 RID: 2837 RVA: 0x0002B3F4 File Offset: 0x000295F4
	private void PickupCraftingTableAct(GameObject obj)
	{
		this.equippedItems[this.equippedIndex] = obj;
		this.equippedItems[this.equippedIndex].GetComponent<IItemInteraction>().ItemInit();
		this.SwapItemImg(this.equippedIndex, this.equippedItems[this.equippedIndex].GetComponent<IItemInteraction>().GetItemID());
		this.SetObjectInHandServer(this.equippedItems[this.equippedIndex]);
		base.StartCoroutine(this.LayerMaskSwapOne());
	}

	// Token: 0x06000B16 RID: 2838 RVA: 0x0002B469 File Offset: 0x00029669
	[ObserversRpc]
	private void SetCraftingForgeNoItem(GameObject CrIn, GameObject obj)
	{
		this.RpcWriter___Observers_SetCraftingForgeNoItem_1401332417(CrIn, obj);
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x0002B47C File Offset: 0x0002967C
	public void GiveDuendeItem(GameObject DuendeHandPoint, int DuendeID)
	{
		if (this.equippedItems[this.equippedIndex] != null)
		{
			this.GiveDuendeServer(this.equippedItems[this.equippedIndex], DuendeHandPoint.gameObject, DuendeID);
			this.equippedItems[this.equippedIndex] = null;
			this.ClearItemImg(this.equippedIndex);
			base.StartCoroutine(this.LayerMaskSwapZero());
		}
	}

	// Token: 0x06000B18 RID: 2840 RVA: 0x0002B4DF File Offset: 0x000296DF
	[ServerRpc(RequireOwnership = false)]
	private void GiveDuendeServer(GameObject obj, GameObject DuendeHandPoint, int DuendeID)
	{
		this.RpcWriter___Server_GiveDuendeServer_1289298000(obj, DuendeHandPoint, DuendeID);
	}

	// Token: 0x06000B19 RID: 2841 RVA: 0x0002B4F4 File Offset: 0x000296F4
	[ObserversRpc]
	private void GiveDuendeObserver(GameObject obj, GameObject DuendeHandPoint, int DuendeID)
	{
		this.RpcWriter___Observers_GiveDuendeObserver_1289298000(obj, DuendeHandPoint, DuendeID);
	}

	// Token: 0x06000B1A RID: 2842 RVA: 0x0002B513 File Offset: 0x00029713
	private IEnumerator DestroyItem(GameObject item, GameObject DuendeHandPoint, int DuendeID)
	{
		yield return new WaitForSeconds(1f);
		this.ServerDestroyItem(item);
		yield return new WaitForSeconds(2f);
		DuendeHandPoint.GetComponent<DuendeManager>().TradeItem(DuendeID);
		base.StartCoroutine(this.DuendeHoldItem(DuendeHandPoint.GetComponent<DuendeManager>().Duende[DuendeID], DuendeID, DuendeHandPoint.GetComponent<DuendeManager>()));
		yield break;
	}

	// Token: 0x06000B1B RID: 2843 RVA: 0x0002B538 File Offset: 0x00029738
	[ServerRpc(RequireOwnership = false)]
	private void ServerDestroyItem(GameObject item)
	{
		this.RpcWriter___Server_ServerDestroyItem_1934289915(item);
	}

	// Token: 0x06000B1C RID: 2844 RVA: 0x0002B54F File Offset: 0x0002974F
	private IEnumerator DuendeHoldItem(DuendeController Duendec, int DuendeID, DuendeManager DuendeManager)
	{
		yield return new WaitForSeconds(1f);
		while (Duendec.CheckPlayersNear() && Duendec.DuendeHandPoint.childCount > 0)
		{
			yield return null;
		}
		this.ServerDuendeAniToggle(DuendeManager.gameObject, DuendeID);
		yield break;
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x0002B573 File Offset: 0x00029773
	[ServerRpc(RequireOwnership = false)]
	private void ServerDuendeAniToggle(GameObject DuendeHandPoint, int DuendeID)
	{
		this.RpcWriter___Server_ServerDuendeAniToggle_2943392466(DuendeHandPoint, DuendeID);
	}

	// Token: 0x06000B1E RID: 2846 RVA: 0x0002B583 File Offset: 0x00029783
	[ObserversRpc]
	private void ObserversDuendeAniToggle(GameObject DuendeHandPoint, int DuendeID)
	{
		this.RpcWriter___Observers_ObserversDuendeAniToggle_2943392466(DuendeHandPoint, DuendeID);
	}

	// Token: 0x06000B1F RID: 2847 RVA: 0x0002B593 File Offset: 0x00029793
	[ServerRpc(RequireOwnership = false)]
	private void DropObjectServer(GameObject obj)
	{
		this.RpcWriter___Server_DropObjectServer_1934289915(obj);
	}

	// Token: 0x06000B20 RID: 2848 RVA: 0x0002B5A0 File Offset: 0x000297A0
	[ObserversRpc]
	private void DropObjectsObserver(GameObject obj)
	{
		this.RpcWriter___Observers_DropObjectsObserver_1934289915(obj);
	}

	// Token: 0x06000B21 RID: 2849 RVA: 0x0002B5B8 File Offset: 0x000297B8
	public void DropAllItems()
	{
		this.equippedIndex = 0;
		for (int i = 0; i < this.equippedItems.Length; i++)
		{
			this.Drop();
			this.equippedIndex++;
		}
		this.equippedIndex = 0;
	}

	// Token: 0x06000B22 RID: 2850 RVA: 0x0002B5FA File Offset: 0x000297FA
	public bool isNotHoldingItem()
	{
		return this.equippedItems[this.equippedIndex] == null;
	}

	// Token: 0x06000B23 RID: 2851 RVA: 0x0002B60F File Offset: 0x0002980F
	public void clearHandItem()
	{
		base.StartCoroutine(this.LayerMaskSwapZero());
		this.ServerClearHand(this.equippedItems[this.equippedIndex]);
		this.ClearItemImg(this.equippedIndex);
		this.equippedItems[this.equippedIndex] = null;
	}

	// Token: 0x06000B24 RID: 2852 RVA: 0x0002B64B File Offset: 0x0002984B
	[ServerRpc(RequireOwnership = false)]
	private void ServerClearHand(GameObject obj)
	{
		this.RpcWriter___Server_ServerClearHand_1934289915(obj);
	}

	// Token: 0x06000B25 RID: 2853 RVA: 0x0002B658 File Offset: 0x00029858
	[ObserversRpc]
	private void ObsClearHand(GameObject obj)
	{
		this.RpcWriter___Observers_ObsClearHand_1934289915(obj);
	}

	// Token: 0x06000B26 RID: 2854 RVA: 0x0002B66F File Offset: 0x0002986F
	public void destroyHandItem()
	{
		base.StartCoroutine(this.LayerMaskSwapZero());
		this.ServerDestroyHand(this.equippedItems[this.equippedIndex]);
		this.equippedItems[this.equippedIndex] = null;
		this.ClearItemImg(this.equippedIndex);
	}

	// Token: 0x06000B27 RID: 2855 RVA: 0x0002B6AC File Offset: 0x000298AC
	[ServerRpc(RequireOwnership = false)]
	private void ServerDestroyHand(GameObject obj)
	{
		this.RpcWriter___Server_ServerDestroyHand_1934289915(obj);
	}

	// Token: 0x06000B28 RID: 2856 RVA: 0x0002B6C4 File Offset: 0x000298C4
	public void PlayerDied()
	{
		this.canUseItem = false;
		this.canSwapItem = false;
		bool flag = false;
		int num = this.equippedIndex;
		this.equippedIndex = 0;
		for (int i = 0; i < this.equippedItems.Length; i++)
		{
			IItemInteraction itemInteraction;
			if (this.equippedItems[i] != null && this.equippedItems[i].TryGetComponent<IItemInteraction>(out itemInteraction) && itemInteraction.GetItemID() == 24)
			{
				flag = true;
			}
			IItemInteraction itemInteraction2;
			if (this.equippedItems[i] != null && this.equippedItems[i].TryGetComponent<IItemInteraction>(out itemInteraction2) && itemInteraction2.GetItemID() != 24 && itemInteraction2.GetItemID() != 5 && itemInteraction2.GetItemID() != 26)
			{
				this.Drop();
			}
			this.equippedIndex++;
		}
		this.equippedIndex = num;
		this.LayerMaskSwapZero();
		if (this.equippedItems[this.equippedIndex] != null)
		{
			this.HideObject(this.equippedItems[this.equippedIndex]);
		}
		if (!flag)
		{
			base.StartCoroutine(this.DelaySpellbookRespawn());
		}
	}

	// Token: 0x06000B29 RID: 2857 RVA: 0x0002B7D0 File Offset: 0x000299D0
	private IEnumerator DelaySpellbookRespawn()
	{
		yield return new WaitForSeconds(3f);
		if (this.myspellbook != null)
		{
			this.SpecialPickup(this.myspellbook);
		}
		else
		{
			this.RespawnSB();
		}
		yield break;
	}

	// Token: 0x06000B2A RID: 2858 RVA: 0x0002B7E0 File Offset: 0x000299E0
	[ServerRpc(RequireOwnership = false)]
	private void RespawnSB()
	{
		this.RpcWriter___Server_RespawnSB_2166136261();
	}

	// Token: 0x06000B2B RID: 2859 RVA: 0x0002B7F3 File Offset: 0x000299F3
	[ObserversRpc]
	private void GetSpawnedItems(GameObject i1)
	{
		this.RpcWriter___Observers_GetSpawnedItems_1934289915(i1);
	}

	// Token: 0x06000B2C RID: 2860 RVA: 0x0002B7FF File Offset: 0x000299FF
	public void PlayerRevived()
	{
		this.canUseItem = true;
		this.canSwapItem = true;
	}

	// Token: 0x06000B2D RID: 2861 RVA: 0x0002B80F File Offset: 0x00029A0F
	public int GetEquippedItemID()
	{
		if (this.equippedItems[this.equippedIndex] != null)
		{
			return this.equippedItems[this.equippedIndex].GetComponent<IItemInteraction>().GetItemID();
		}
		return -1;
	}

	// Token: 0x06000B2E RID: 2862 RVA: 0x0002B840 File Offset: 0x00029A40
	public void ClearItemImages()
	{
		for (int i = 0; i < this.equippedItems.Length; i++)
		{
			this.ClearItemImg(i);
		}
	}

	// Token: 0x06000B2F RID: 2863 RVA: 0x0002B868 File Offset: 0x00029A68
	public void TryActivateMirror(MagicMirrorController[] mmc, VoiceBroadcastTrigger vbt)
	{
		if (Vector3.Distance(base.transform.position, mmc[0].transform.position) < 10f)
		{
			mmc[0].ActivateMirror(vbt, base.transform);
			return;
		}
		if (Vector3.Distance(base.transform.position, mmc[1].transform.position) < 10f)
		{
			mmc[1].ActivateMirror(vbt, base.transform);
		}
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x0002B8DC File Offset: 0x00029ADC
	public void tempdisableitemswap()
	{
		base.StartCoroutine(this.TempDisableItemSwaprout());
	}

	// Token: 0x06000B31 RID: 2865 RVA: 0x0002B8EB File Offset: 0x00029AEB
	private IEnumerator TempDisableItemSwaprout()
	{
		this.canSwapItem = false;
		yield return new WaitForSeconds(0.25f);
		this.canSwapItem = true;
		yield break;
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x0002B980 File Offset: 0x00029B80
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyPlayerInventoryAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyPlayerInventoryAssembly-CSharp.dll_Excuted = true;
		base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_SpawnSpawnItems_2166136261));
		base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_GetSpawnedItems_1401332417));
		base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_HideObject_1934289915));
		base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_HideObjectObserver_1934289915));
		base.RegisterServerRpc(4U, new ServerRpcDelegate(this.RpcReader___Server_SetObjectInHandServer_1934289915));
		base.RegisterObserversRpc(5U, new ClientRpcDelegate(this.RpcReader___Observers_SetObjectInHandObserver_1934289915));
		base.RegisterServerRpc(6U, new ServerRpcDelegate(this.RpcReader___Server_TryPlaceOnCraftingTableServer_1401332417));
		base.RegisterObserversRpc(7U, new ClientRpcDelegate(this.RpcReader___Observers_PlaceOnCraftingTableObserver_1401332417));
		base.RegisterServerRpc(8U, new ServerRpcDelegate(this.RpcReader___Server_TryPickupCraftingTableServer_1401332417));
		base.RegisterObserversRpc(9U, new ClientRpcDelegate(this.RpcReader___Observers_SetCraftingForgeNoItem_1401332417));
		base.RegisterServerRpc(10U, new ServerRpcDelegate(this.RpcReader___Server_GiveDuendeServer_1289298000));
		base.RegisterObserversRpc(11U, new ClientRpcDelegate(this.RpcReader___Observers_GiveDuendeObserver_1289298000));
		base.RegisterServerRpc(12U, new ServerRpcDelegate(this.RpcReader___Server_ServerDestroyItem_1934289915));
		base.RegisterServerRpc(13U, new ServerRpcDelegate(this.RpcReader___Server_ServerDuendeAniToggle_2943392466));
		base.RegisterObserversRpc(14U, new ClientRpcDelegate(this.RpcReader___Observers_ObserversDuendeAniToggle_2943392466));
		base.RegisterServerRpc(15U, new ServerRpcDelegate(this.RpcReader___Server_DropObjectServer_1934289915));
		base.RegisterObserversRpc(16U, new ClientRpcDelegate(this.RpcReader___Observers_DropObjectsObserver_1934289915));
		base.RegisterServerRpc(17U, new ServerRpcDelegate(this.RpcReader___Server_ServerClearHand_1934289915));
		base.RegisterObserversRpc(18U, new ClientRpcDelegate(this.RpcReader___Observers_ObsClearHand_1934289915));
		base.RegisterServerRpc(19U, new ServerRpcDelegate(this.RpcReader___Server_ServerDestroyHand_1934289915));
		base.RegisterServerRpc(20U, new ServerRpcDelegate(this.RpcReader___Server_RespawnSB_2166136261));
		base.RegisterObserversRpc(21U, new ClientRpcDelegate(this.RpcReader___Observers_GetSpawnedItems_1934289915));
	}

	// Token: 0x06000B34 RID: 2868 RVA: 0x0002BB98 File Offset: 0x00029D98
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LatePlayerInventoryAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LatePlayerInventoryAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x06000B35 RID: 2869 RVA: 0x0002BBAB File Offset: 0x00029DAB
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x0002BBBC File Offset: 0x00029DBC
	private void RpcWriter___Server_SpawnSpawnItems_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000B37 RID: 2871 RVA: 0x0002BC24 File Offset: 0x00029E24
	private void RpcLogic___SpawnSpawnItems_2166136261()
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.Torch);
		base.ServerManager.Spawn(gameObject, null, default(Scene));
		GameObject gameObject2 = Object.Instantiate<GameObject>(this.MageBook);
		base.ServerManager.Spawn(gameObject2, null, default(Scene));
		this.GetSpawnedItems(gameObject, gameObject2);
	}

	// Token: 0x06000B38 RID: 2872 RVA: 0x0002BC80 File Offset: 0x00029E80
	private void RpcReader___Server_SpawnSpawnItems_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___SpawnSpawnItems_2166136261();
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x0002BCA0 File Offset: 0x00029EA0
	private void RpcWriter___Observers_GetSpawnedItems_1401332417(GameObject i1, GameObject i2)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(i1);
		pooledWriter.WriteGameObject(i2);
		base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x0002BD2E File Offset: 0x00029F2E
	private void RpcLogic___GetSpawnedItems_1401332417(GameObject i1, GameObject i2)
	{
		if (base.HasAuthority)
		{
			i2.GetComponent<MageBookController>().pinv = this;
			this.PickupToItemSlot(i1, 4);
			this.PickupToItemSlot(i2, 0);
		}
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x0002BD54 File Offset: 0x00029F54
	private void RpcReader___Observers_GetSpawnedItems_1401332417(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		GameObject gameObject2 = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___GetSpawnedItems_1401332417(gameObject, gameObject2);
	}

	// Token: 0x06000B3C RID: 2876 RVA: 0x0002BD98 File Offset: 0x00029F98
	private void RpcWriter___Server_HideObject_1934289915(GameObject objToHide)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(objToHide);
		base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000B3D RID: 2877 RVA: 0x0002BE0A File Offset: 0x0002A00A
	private void RpcLogic___HideObject_1934289915(GameObject objToHide)
	{
		this.HideObjectObserver(objToHide);
	}

	// Token: 0x06000B3E RID: 2878 RVA: 0x0002BE14 File Offset: 0x0002A014
	private void RpcReader___Server_HideObject_1934289915(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___HideObject_1934289915(gameObject);
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x0002BE48 File Offset: 0x0002A048
	private void RpcWriter___Observers_HideObjectObserver_1934289915(GameObject objToHide)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(objToHide);
		base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x0002BECC File Offset: 0x0002A0CC
	private void RpcLogic___HideObjectObserver_1934289915(GameObject objToHide)
	{
		objToHide.transform.SetParent(this.pickupPosition);
		objToHide.transform.localPosition = new Vector3(0f, 0f, 0f);
		objToHide.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		objToHide.GetComponent<Collider>().enabled = false;
		objToHide.GetComponent<IItemInteraction>().HideItem();
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x0002BF40 File Offset: 0x0002A140
	private void RpcReader___Observers_HideObjectObserver_1934289915(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___HideObjectObserver_1934289915(gameObject);
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x0002BF74 File Offset: 0x0002A174
	private void RpcWriter___Server_SetObjectInHandServer_1934289915(GameObject obj)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(obj);
		base.SendServerRpc(4U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000B43 RID: 2883 RVA: 0x0002BFE6 File Offset: 0x0002A1E6
	private void RpcLogic___SetObjectInHandServer_1934289915(GameObject obj)
	{
		this.SetObjectInHandObserver(obj);
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x0002BFF0 File Offset: 0x0002A1F0
	private void RpcReader___Server_SetObjectInHandServer_1934289915(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___SetObjectInHandServer_1934289915(gameObject);
	}

	// Token: 0x06000B45 RID: 2885 RVA: 0x0002C024 File Offset: 0x0002A224
	private void RpcWriter___Observers_SetObjectInHandObserver_1934289915(GameObject obj)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(obj);
		base.SendObserversRpc(5U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x0002C0A8 File Offset: 0x0002A2A8
	private void RpcLogic___SetObjectInHandObserver_1934289915(GameObject obj)
	{
		obj.transform.SetParent(this.pickupPosition);
		obj.transform.localPosition = new Vector3(0f, 0f, 0f);
		obj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		obj.GetComponent<Collider>().enabled = false;
		if (!base.IsOwner)
		{
			obj.GetComponent<IItemInteraction>().ItemInitObs();
		}
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x0002C124 File Offset: 0x0002A324
	private void RpcReader___Observers_SetObjectInHandObserver_1934289915(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___SetObjectInHandObserver_1934289915(gameObject);
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x0002C158 File Offset: 0x0002A358
	private void RpcWriter___Server_TryPlaceOnCraftingTableServer_1401332417(GameObject obj, GameObject CrIn)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(obj);
		pooledWriter.WriteGameObject(CrIn);
		base.SendServerRpc(6U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x0002C1D8 File Offset: 0x0002A3D8
	private void RpcLogic___TryPlaceOnCraftingTableServer_1401332417(GameObject obj, GameObject CrIn)
	{
		CraftInteracter component = CrIn.GetComponent<CraftInteracter>();
		if (component.craftingForge.SlotItems[component.slotID] == null)
		{
			component.craftingForge.SlotItems[component.slotID] = obj;
			this.PlaceOnCraftingTableObserver(obj, CrIn);
		}
	}

	// Token: 0x06000B4A RID: 2890 RVA: 0x0002C224 File Offset: 0x0002A424
	private void RpcReader___Server_TryPlaceOnCraftingTableServer_1401332417(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		GameObject gameObject2 = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___TryPlaceOnCraftingTableServer_1401332417(gameObject, gameObject2);
	}

	// Token: 0x06000B4B RID: 2891 RVA: 0x0002C268 File Offset: 0x0002A468
	private void RpcWriter___Observers_PlaceOnCraftingTableObserver_1401332417(GameObject obj, GameObject CrIn)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(obj);
		pooledWriter.WriteGameObject(CrIn);
		base.SendObserversRpc(7U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000B4C RID: 2892 RVA: 0x0002C2F8 File Offset: 0x0002A4F8
	private void RpcLogic___PlaceOnCraftingTableObserver_1401332417(GameObject obj, GameObject CrIn)
	{
		if (base.IsOwner)
		{
			this.equippedItems[this.equippedIndex] = null;
			this.ClearItemImg(this.equippedIndex);
			base.StartCoroutine(this.LayerMaskSwapZero());
		}
		obj.transform.parent = this.worldObjectHolder;
		obj.transform.position = CrIn.transform.position + Vector3.up * 0.1f;
		IItemInteraction itemInteraction;
		IItemInteraction itemInteraction2;
		if (obj.TryGetComponent<IItemInteraction>(out itemInteraction) && (itemInteraction.GetItemID() == 8 || itemInteraction.GetItemID() == 9))
		{
			obj.transform.localRotation = Quaternion.Euler(-176f, 3f, 83f);
			itemInteraction.PlayDropSound();
		}
		else if (obj.TryGetComponent<IItemInteraction>(out itemInteraction2) && itemInteraction2.GetItemID() == 25)
		{
			obj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			itemInteraction.PlayDropSound();
		}
		else
		{
			obj.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
			itemInteraction.PlayDropSound();
		}
		CraftInteracter craftInteracter;
		if (CrIn.TryGetComponent<CraftInteracter>(out craftInteracter))
		{
			craftInteracter.SetSlot(obj);
		}
	}

	// Token: 0x06000B4D RID: 2893 RVA: 0x0002C424 File Offset: 0x0002A624
	private void RpcReader___Observers_PlaceOnCraftingTableObserver_1401332417(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		GameObject gameObject2 = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___PlaceOnCraftingTableObserver_1401332417(gameObject, gameObject2);
	}

	// Token: 0x06000B4E RID: 2894 RVA: 0x0002C468 File Offset: 0x0002A668
	private void RpcWriter___Server_TryPickupCraftingTableServer_1401332417(GameObject obj, GameObject CrIn)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(obj);
		pooledWriter.WriteGameObject(CrIn);
		base.SendServerRpc(8U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000B4F RID: 2895 RVA: 0x0002C4E8 File Offset: 0x0002A6E8
	private void RpcLogic___TryPickupCraftingTableServer_1401332417(GameObject obj, GameObject CrIn)
	{
		CraftInteracter component = CrIn.GetComponent<CraftInteracter>();
		if (component.craftingForge.SlotItems[component.slotID] != null && this.equippedItems[this.equippedIndex] == null)
		{
			component.craftingForge.SlotItems[component.slotID] = null;
			this.SetCraftingForgeNoItem(CrIn, obj);
		}
	}

	// Token: 0x06000B50 RID: 2896 RVA: 0x0002C548 File Offset: 0x0002A748
	private void RpcReader___Server_TryPickupCraftingTableServer_1401332417(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		GameObject gameObject2 = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___TryPickupCraftingTableServer_1401332417(gameObject, gameObject2);
	}

	// Token: 0x06000B51 RID: 2897 RVA: 0x0002C58C File Offset: 0x0002A78C
	private void RpcWriter___Observers_SetCraftingForgeNoItem_1401332417(GameObject CrIn, GameObject obj)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(CrIn);
		pooledWriter.WriteGameObject(obj);
		base.SendObserversRpc(9U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000B52 RID: 2898 RVA: 0x0002C61A File Offset: 0x0002A81A
	private void RpcLogic___SetCraftingForgeNoItem_1401332417(GameObject CrIn, GameObject obj)
	{
		if (base.IsOwner)
		{
			this.PickupCraftingTableAct(obj);
		}
		CrIn.GetComponent<CraftInteracter>().clearItem();
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x0002C638 File Offset: 0x0002A838
	private void RpcReader___Observers_SetCraftingForgeNoItem_1401332417(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		GameObject gameObject2 = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___SetCraftingForgeNoItem_1401332417(gameObject, gameObject2);
	}

	// Token: 0x06000B54 RID: 2900 RVA: 0x0002C67C File Offset: 0x0002A87C
	private void RpcWriter___Server_GiveDuendeServer_1289298000(GameObject obj, GameObject DuendeHandPoint, int DuendeID)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(obj);
		pooledWriter.WriteGameObject(DuendeHandPoint);
		pooledWriter.WriteInt32(DuendeID);
		base.SendServerRpc(10U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000B55 RID: 2901 RVA: 0x0002C708 File Offset: 0x0002A908
	private void RpcLogic___GiveDuendeServer_1289298000(GameObject obj, GameObject DuendeHandPoint, int DuendeID)
	{
		this.GiveDuendeObserver(obj, DuendeHandPoint, DuendeID);
	}

	// Token: 0x06000B56 RID: 2902 RVA: 0x0002C714 File Offset: 0x0002A914
	private void RpcReader___Server_GiveDuendeServer_1289298000(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		GameObject gameObject2 = PooledReader0.ReadGameObject();
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___GiveDuendeServer_1289298000(gameObject, gameObject2, num);
	}

	// Token: 0x06000B57 RID: 2903 RVA: 0x0002C768 File Offset: 0x0002A968
	private void RpcWriter___Observers_GiveDuendeObserver_1289298000(GameObject obj, GameObject DuendeHandPoint, int DuendeID)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(obj);
		pooledWriter.WriteGameObject(DuendeHandPoint);
		pooledWriter.WriteInt32(DuendeID);
		base.SendObserversRpc(11U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000B58 RID: 2904 RVA: 0x0002C804 File Offset: 0x0002AA04
	private void RpcLogic___GiveDuendeObserver_1289298000(GameObject obj, GameObject DuendeHandPoint, int DuendeID)
	{
		obj.transform.parent = this.worldObjectHolder;
		obj.transform.localScale = new Vector3(1f, 1f, 1f);
		obj.transform.parent = DuendeHandPoint.GetComponent<DuendeManager>().Duende[DuendeID].DuendeHandPoint;
		obj.transform.position = DuendeHandPoint.GetComponent<DuendeManager>().Duende[DuendeID].DuendeHandPoint.position;
		obj.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
		if (base.IsOwner)
		{
			base.StartCoroutine(this.DestroyItem(obj, DuendeHandPoint, DuendeID));
		}
	}

	// Token: 0x06000B59 RID: 2905 RVA: 0x0002C8B8 File Offset: 0x0002AAB8
	private void RpcReader___Observers_GiveDuendeObserver_1289298000(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		GameObject gameObject2 = PooledReader0.ReadGameObject();
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___GiveDuendeObserver_1289298000(gameObject, gameObject2, num);
	}

	// Token: 0x06000B5A RID: 2906 RVA: 0x0002C90C File Offset: 0x0002AB0C
	private void RpcWriter___Server_ServerDestroyItem_1934289915(GameObject item)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(item);
		base.SendServerRpc(12U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000B5B RID: 2907 RVA: 0x0002C980 File Offset: 0x0002AB80
	private void RpcLogic___ServerDestroyItem_1934289915(GameObject item)
	{
		item.transform.parent = this.worldObjectHolder;
		base.ServerManager.Despawn(item, null);
	}

	// Token: 0x06000B5C RID: 2908 RVA: 0x0002C9B4 File Offset: 0x0002ABB4
	private void RpcReader___Server_ServerDestroyItem_1934289915(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerDestroyItem_1934289915(gameObject);
	}

	// Token: 0x06000B5D RID: 2909 RVA: 0x0002C9E8 File Offset: 0x0002ABE8
	private void RpcWriter___Server_ServerDuendeAniToggle_2943392466(GameObject DuendeHandPoint, int DuendeID)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(DuendeHandPoint);
		pooledWriter.WriteInt32(DuendeID);
		base.SendServerRpc(13U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000B5E RID: 2910 RVA: 0x0002CA67 File Offset: 0x0002AC67
	private void RpcLogic___ServerDuendeAniToggle_2943392466(GameObject DuendeHandPoint, int DuendeID)
	{
		this.ObserversDuendeAniToggle(DuendeHandPoint, DuendeID);
	}

	// Token: 0x06000B5F RID: 2911 RVA: 0x0002CA74 File Offset: 0x0002AC74
	private void RpcReader___Server_ServerDuendeAniToggle_2943392466(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		int num = PooledReader0.ReadInt32();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerDuendeAniToggle_2943392466(gameObject, num);
	}

	// Token: 0x06000B60 RID: 2912 RVA: 0x0002CAB8 File Offset: 0x0002ACB8
	private void RpcWriter___Observers_ObserversDuendeAniToggle_2943392466(GameObject DuendeHandPoint, int DuendeID)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(DuendeHandPoint);
		pooledWriter.WriteInt32(DuendeID);
		base.SendObserversRpc(14U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000B61 RID: 2913 RVA: 0x0002CB46 File Offset: 0x0002AD46
	private void RpcLogic___ObserversDuendeAniToggle_2943392466(GameObject DuendeHandPoint, int DuendeID)
	{
		DuendeHandPoint.GetComponent<DuendeManager>().Duende[DuendeID].Duendeani.SetBool("hold", false);
		DuendeHandPoint.GetComponent<DuendeManager>().Duende[DuendeID].isTrading = false;
	}

	// Token: 0x06000B62 RID: 2914 RVA: 0x0002CB78 File Offset: 0x0002AD78
	private void RpcReader___Observers_ObserversDuendeAniToggle_2943392466(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		int num = PooledReader0.ReadInt32();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObserversDuendeAniToggle_2943392466(gameObject, num);
	}

	// Token: 0x06000B63 RID: 2915 RVA: 0x0002CBBC File Offset: 0x0002ADBC
	private void RpcWriter___Server_DropObjectServer_1934289915(GameObject obj)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(obj);
		base.SendServerRpc(15U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000B64 RID: 2916 RVA: 0x0002CC2E File Offset: 0x0002AE2E
	private void RpcLogic___DropObjectServer_1934289915(GameObject obj)
	{
		this.DropObjectsObserver(obj);
	}

	// Token: 0x06000B65 RID: 2917 RVA: 0x0002CC38 File Offset: 0x0002AE38
	private void RpcReader___Server_DropObjectServer_1934289915(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___DropObjectServer_1934289915(gameObject);
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x0002CC6C File Offset: 0x0002AE6C
	private void RpcWriter___Observers_DropObjectsObserver_1934289915(GameObject obj)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(obj);
		base.SendObserversRpc(16U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x0002CCF0 File Offset: 0x0002AEF0
	private void RpcLogic___DropObjectsObserver_1934289915(GameObject obj)
	{
		obj.transform.parent = this.worldObjectHolder;
		IItemInteraction itemInteraction;
		if (obj.TryGetComponent<IItemInteraction>(out itemInteraction))
		{
			itemInteraction.SetScale();
			itemInteraction.DropItem();
		}
		Collider collider;
		if (obj.TryGetComponent<Collider>(out collider))
		{
			collider.enabled = true;
		}
	}

	// Token: 0x06000B68 RID: 2920 RVA: 0x0002CD38 File Offset: 0x0002AF38
	private void RpcReader___Observers_DropObjectsObserver_1934289915(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___DropObjectsObserver_1934289915(gameObject);
	}

	// Token: 0x06000B69 RID: 2921 RVA: 0x0002CD6C File Offset: 0x0002AF6C
	private void RpcWriter___Server_ServerClearHand_1934289915(GameObject obj)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(obj);
		base.SendServerRpc(17U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000B6A RID: 2922 RVA: 0x0002CDDE File Offset: 0x0002AFDE
	private void RpcLogic___ServerClearHand_1934289915(GameObject obj)
	{
		this.ObsClearHand(obj);
	}

	// Token: 0x06000B6B RID: 2923 RVA: 0x0002CDE8 File Offset: 0x0002AFE8
	private void RpcReader___Server_ServerClearHand_1934289915(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerClearHand_1934289915(gameObject);
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x0002CE1C File Offset: 0x0002B01C
	private void RpcWriter___Observers_ObsClearHand_1934289915(GameObject obj)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(obj);
		base.SendObserversRpc(18U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x0002CEA0 File Offset: 0x0002B0A0
	private void RpcLogic___ObsClearHand_1934289915(GameObject obj)
	{
		obj.transform.parent = this.worldObjectHolder;
		IItemInteraction itemInteraction;
		if (obj.TryGetComponent<IItemInteraction>(out itemInteraction))
		{
			itemInteraction.SetScale();
		}
	}

	// Token: 0x06000B6E RID: 2926 RVA: 0x0002CED0 File Offset: 0x0002B0D0
	private void RpcReader___Observers_ObsClearHand_1934289915(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___ObsClearHand_1934289915(gameObject);
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x0002CF04 File Offset: 0x0002B104
	private void RpcWriter___Server_ServerDestroyHand_1934289915(GameObject obj)
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(obj);
		base.SendServerRpc(19U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x0002CF78 File Offset: 0x0002B178
	private void RpcLogic___ServerDestroyHand_1934289915(GameObject obj)
	{
		base.ServerManager.Despawn(obj, null);
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x0002CF9C File Offset: 0x0002B19C
	private void RpcReader___Server_ServerDestroyHand_1934289915(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___ServerDestroyHand_1934289915(gameObject);
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x0002CFD0 File Offset: 0x0002B1D0
	private void RpcWriter___Server_RespawnSB_2166136261()
	{
		if (!base.IsClientInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		base.SendServerRpc(20U, pooledWriter, channel, DataOrderType.Default);
		pooledWriter.Store();
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x0002D038 File Offset: 0x0002B238
	private void RpcLogic___RespawnSB_2166136261()
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.MageBook);
		base.ServerManager.Spawn(gameObject, null, default(Scene));
		gameObject.GetComponent<MageBookController>().pinv = this;
		this.GetSpawnedItems(gameObject);
	}

	// Token: 0x06000B74 RID: 2932 RVA: 0x0002D07C File Offset: 0x0002B27C
	private void RpcReader___Server_RespawnSB_2166136261(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
	{
		if (!base.IsServerInitialized)
		{
			return;
		}
		this.RpcLogic___RespawnSB_2166136261();
	}

	// Token: 0x06000B75 RID: 2933 RVA: 0x0002D09C File Offset: 0x0002B29C
	private void RpcWriter___Observers_GetSpawnedItems_1934289915(GameObject i1)
	{
		if (!base.IsServerInitialized)
		{
			NetworkManager networkManager = base.NetworkManager;
			networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
			return;
		}
		Channel channel = Channel.Reliable;
		PooledWriter pooledWriter = WriterPool.Retrieve();
		pooledWriter.WriteGameObject(i1);
		base.SendObserversRpc(21U, pooledWriter, channel, DataOrderType.Default, false, false, false);
		pooledWriter.Store();
	}

	// Token: 0x06000B76 RID: 2934 RVA: 0x0002D11D File Offset: 0x0002B31D
	private void RpcLogic___GetSpawnedItems_1934289915(GameObject i1)
	{
		if (base.HasAuthority)
		{
			i1.GetComponent<MageBookController>().pinv = this;
			this.Pickup(i1);
			i1.GetComponent<MageBookController>().HideItem();
		}
	}

	// Token: 0x06000B77 RID: 2935 RVA: 0x0002D148 File Offset: 0x0002B348
	private void RpcReader___Observers_GetSpawnedItems_1934289915(PooledReader PooledReader0, Channel channel)
	{
		GameObject gameObject = PooledReader0.ReadGameObject();
		if (!base.IsClientInitialized)
		{
			return;
		}
		this.RpcLogic___GetSpawnedItems_1934289915(gameObject);
	}

	// Token: 0x06000B78 RID: 2936 RVA: 0x0002BBAB File Offset: 0x00029DAB
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x040005E2 RID: 1506
	private GameObject[] equippedItems = new GameObject[5];

	// Token: 0x040005E3 RID: 1507
	private Image[] UIImages = new Image[5];

	// Token: 0x040005E4 RID: 1508
	public Transform pickupPosition;

	// Token: 0x040005E5 RID: 1509
	private Transform worldObjectHolder;

	// Token: 0x040005E6 RID: 1510
	public int equippedIndex;

	// Token: 0x040005E7 RID: 1511
	private float cooldown = 0.1f;

	// Token: 0x040005E8 RID: 1512
	[SerializeField]
	private KeyCode dropButton = KeyCode.G;

	// Token: 0x040005E9 RID: 1513
	public Animator armsAni;

	// Token: 0x040005EA RID: 1514
	public Animator bodyAni;

	// Token: 0x040005EB RID: 1515
	public bool canSwapItem = true;

	// Token: 0x040005EC RID: 1516
	public bool canUseItem = true;

	// Token: 0x040005ED RID: 1517
	public Transform AlternatePickupParent;

	// Token: 0x040005EE RID: 1518
	public Sprite[] ItemIcons;

	// Token: 0x040005EF RID: 1519
	public Sprite blank;

	// Token: 0x040005F0 RID: 1520
	public GameObject Torch;

	// Token: 0x040005F1 RID: 1521
	public GameObject MageBook;

	// Token: 0x040005F2 RID: 1522
	public bool initedInv;

	// Token: 0x040005F3 RID: 1523
	private float fbcd = -50f;

	// Token: 0x040005F4 RID: 1524
	private float frostcd = -50f;

	// Token: 0x040005F5 RID: 1525
	private float wormcd = -50f;

	// Token: 0x040005F6 RID: 1526
	private float holecd = -50f;

	// Token: 0x040005F7 RID: 1527
	private float wardcd = -50f;

	// Token: 0x040005F8 RID: 1528
	public SettingsHolder Sholder;

	// Token: 0x040005F9 RID: 1529
	public PlayerInteract pi;

	// Token: 0x040005FA RID: 1530
	public GameObject LightningBolt;

	// Token: 0x040005FB RID: 1531
	private GameObject myspellbook;

	// Token: 0x040005FC RID: 1532
	public LayerMask ground;

	// Token: 0x040005FD RID: 1533
	public LayerMask forcefield;

	// Token: 0x040005FE RID: 1534
	private bool NetworkInitialize___EarlyPlayerInventoryAssembly-CSharp.dll_Excuted;

	// Token: 0x040005FF RID: 1535
	private bool NetworkInitialize__LatePlayerInventoryAssembly-CSharp.dll_Excuted;
}
