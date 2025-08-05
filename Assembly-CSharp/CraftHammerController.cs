using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000037 RID: 55
public class CraftHammerController : SwordController
{
	// Token: 0x06000265 RID: 613 RVA: 0x0000A9F3 File Offset: 0x00008BF3
	public override string DisplayInteractUI(GameObject player)
	{
		return "Grasp Hammer of Craft";
	}

	// Token: 0x06000266 RID: 614 RVA: 0x0000A9FA File Offset: 0x00008BFA
	public override int GetItemID()
	{
		return 25;
	}

	// Token: 0x06000267 RID: 615 RVA: 0x0000A9FE File Offset: 0x00008BFE
	public override void Interaction(GameObject player)
	{
		base.StartCoroutine(this.CraftRoutine(player.GetComponent<PlayerInventory>(), player.GetComponent<PlayerMovement>()));
	}

	// Token: 0x06000268 RID: 616 RVA: 0x0000AA19 File Offset: 0x00008C19
	private IEnumerator CraftRoutine(PlayerInventory playerInv, PlayerMovement playerMov)
	{
		playerInv.armsAni.SetBool("attack", true);
		playerInv.bodyAni.SetBool("attack", true);
		this.BladeTrigger.enabled = true;
		while (Input.GetKey(KeyCode.Mouse0))
		{
			if (this.isHitting && this.HitSubject != null && !this.HitSubject.CompareTag("Player"))
			{
				if (this.HitSubject.CompareTag("RockGround"))
				{
					RaycastHit raycastHit;
					if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out raycastHit, 30f, this.layermsk))
					{
						break;
					}
					break;
				}
				else if (this.HitSubject.CompareTag("hitable"))
				{
					this.HitSubject.GetComponent<IInteractableNetworkObj>().NetInteract();
					yield return new WaitForSeconds(0.1f);
				}
				else if (this.HitSubject.CompareTag("CraftingTable"))
				{
					this.HitSubject.GetComponent<CraftTableToCastleHex>().cforge.Craft();
					break;
				}
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
		playerInv.armsAni.SetBool("attack", false);
		playerInv.bodyAni.SetBool("attack", false);
		yield break;
	}

	// Token: 0x06000269 RID: 617 RVA: 0x0000AA38 File Offset: 0x00008C38
	public override void DropItem()
	{
		LayerMask layerMask = 192;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 100f, ~layerMask))
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			base.transform.position = raycastHit.point;
		}
		this.SwordControllerAudio.PlayOneShot(this.SwordClips[1]);
	}

	// Token: 0x0600026B RID: 619 RVA: 0x0000AAC1 File Offset: 0x00008CC1
	public override void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyCraftHammerControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyCraftHammerControllerAssembly-CSharp.dll_Excuted = true;
		base.NetworkInitialize___Early();
	}

	// Token: 0x0600026C RID: 620 RVA: 0x0000AADA File Offset: 0x00008CDA
	public override void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateCraftHammerControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateCraftHammerControllerAssembly-CSharp.dll_Excuted = true;
		base.NetworkInitialize__Late();
	}

	// Token: 0x0600026D RID: 621 RVA: 0x0000AAF3 File Offset: 0x00008CF3
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0600026E RID: 622 RVA: 0x0000AB01 File Offset: 0x00008D01
	public override void Awake()
	{
		this.NetworkInitialize___Early();
		base.Awake();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000132 RID: 306
	private bool NetworkInitialize___EarlyCraftHammerControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x04000133 RID: 307
	private bool NetworkInitialize__LateCraftHammerControllerAssembly-CSharp.dll_Excuted;
}
