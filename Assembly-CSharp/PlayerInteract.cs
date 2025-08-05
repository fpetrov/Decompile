using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

// Token: 0x0200010E RID: 270
public class PlayerInteract : MonoBehaviour
{
	// Token: 0x06000AD0 RID: 2768 RVA: 0x00029550 File Offset: 0x00027750
	private void Start()
	{
		this.Sholder = GameObject.FindGameObjectWithTag("Settings").GetComponent<SettingsHolder>();
		base.StartCoroutine(this.Interactor());
	}

	// Token: 0x06000AD1 RID: 2769 RVA: 0x00029574 File Offset: 0x00027774
	public void leveluptxt(string text)
	{
		base.StartCoroutine(this.fadeinlvluptext(text));
	}

	// Token: 0x06000AD2 RID: 2770 RVA: 0x00029584 File Offset: 0x00027784
	private IEnumerator fadeinlvluptext(string text)
	{
		float num = this.waitqueue;
		this.waitqueue += 4f;
		yield return new WaitForSeconds(num);
		this.lvltextactual.text = text;
		this.lvltextactual2.text = text;
		float timer = 0f;
		while (timer < 1f)
		{
			this.lvluptext.alpha = timer;
			timer += Time.deltaTime;
			yield return null;
		}
		this.waitqueue -= 1f;
		yield return new WaitForSeconds(1f);
		this.waitqueue -= 1f;
		yield return new WaitForSeconds(1f);
		this.waitqueue -= 1f;
		yield return new WaitForSeconds(1f);
		this.waitqueue -= 1f;
		timer = 1f;
		while (timer > 0f)
		{
			this.lvluptext.alpha = timer;
			timer -= Time.deltaTime;
			yield return null;
		}
		this.lvluptext.alpha = 0f;
		yield break;
	}

	// Token: 0x06000AD3 RID: 2771 RVA: 0x0002959A File Offset: 0x0002779A
	public void SetLevelNum(int lvl)
	{
		if (lvl < 12)
		{
			this.lvlnum.material = this.levelNumberMats[lvl - 1];
			return;
		}
		this.lvlnum.material = this.levelNumberMats[11];
	}

	// Token: 0x06000AD4 RID: 2772 RVA: 0x000295CC File Offset: 0x000277CC
	private void Update()
	{
		if (Input.GetKey(this.Sholder.map))
		{
			if (this.hasPlayedshite)
			{
				float num = Mathf.Clamp01(this.mapani.GetCurrentAnimatorStateInfo(0).normalizedTime);
				this.mapani.Play("open", 0, 1f - num);
				this.hasPlayedshite = false;
			}
			this.map.SetActive(true);
			if (!this.map.GetComponent<AudioSource>().isPlaying)
			{
				this.map.GetComponent<AudioSource>().pitch = 0.6f;
				this.map.GetComponent<AudioSource>().Play();
			}
			if (this.mapani.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.75f)
			{
				this.mapcanvas.SetActive(true);
				this.mapcanvas.GetComponent<MapCGFader>().FadeinShit();
				this.map.GetComponent<AudioSource>().Pause();
				return;
			}
		}
		else
		{
			if (this.map.activeSelf && !this.hasPlayedshite)
			{
				float num2 = Mathf.Clamp01(this.mapani.GetCurrentAnimatorStateInfo(0).normalizedTime);
				this.hasPlayedshite = true;
				this.mapani.Play("close", 0, 1f - num2);
				this.mapcanvas.GetComponent<MapCGFader>().Fadeoutshit();
				this.map.GetComponent<AudioSource>().pitch = 0.5f;
				this.map.GetComponent<AudioSource>().Play();
				return;
			}
			if (this.map.activeSelf && this.mapani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
			{
				this.map.SetActive(false);
				this.map.GetComponent<AudioSource>().Pause();
			}
		}
	}

	// Token: 0x06000AD5 RID: 2773 RVA: 0x00029785 File Offset: 0x00027985
	private IEnumerator Interactor()
	{
		while (this.player == null)
		{
			if (base.gameObject.transform.parent != null)
			{
				this.player = base.gameObject.transform.parent.gameObject;
			}
			yield return null;
		}
		while (base.isActiveAndEnabled)
		{
			RaycastHit hitInfo;
			if (Physics.Raycast(new Ray(this.InteractorSource.position, this.InteractorSource.forward), out hitInfo, 100f, this.layers) && !this.player.GetComponent<PlayerMovement>().isDead)
			{
				if (hitInfo.distance < this.InteractRange)
				{
					IInteractable interactable;
					if (hitInfo.collider.gameObject.TryGetComponent<IInteractable>(out interactable))
					{
						this.InteractUI.gameObject.SetActive(true);
						this.InteractUI.GetComponent<Text>().text = interactable.DisplayInteractUI(this.player);
						this.InteractUI2.GetComponent<Text>().text = interactable.DisplayInteractUI(this.player);
						if (this.InteractUI.GetComponent<Text>().text != "" && this.InteractUI.GetComponent<Text>().text != "Mirror mirror...")
						{
							this.uiHand.SetActive(true);
						}
						else
						{
							this.uiHand.SetActive(false);
						}
						if (Input.GetKeyDown(this.Sholder.interact))
						{
							interactable.Interact(this.player);
						}
					}
					else
					{
						ITimedInteractable timedInteractObj;
						if (hitInfo.collider.gameObject.TryGetComponent<ITimedInteractable>(out timedInteractObj))
						{
							this.InteractUI.gameObject.SetActive(true);
							this.InteractUI.GetComponent<Text>().text = timedInteractObj.DisplayInteractUI();
							this.InteractUI2.GetComponent<Text>().text = timedInteractObj.DisplayInteractUI();
							if (this.InteractUI.GetComponent<Text>().text != "")
							{
								this.uiHand.SetActive(true);
							}
							else
							{
								this.uiHand.SetActive(false);
							}
							this.uiTimer.enabled = true;
							this.uiFill.enabled = true;
							float time = timedInteractObj.GetInteractTimer(this.player);
							float timer = 0f;
							while (Input.GetKey(this.Sholder.interact))
							{
								if (time == 15.1f && timer == 0f)
								{
									hitInfo.collider.gameObject.GetComponent<ChestInteract>().cnc.LockPick(true);
								}
								else if (time == 6.1f && timer == 0f)
								{
									ChestInteract1 component = hitInfo.collider.gameObject.GetComponent<ChestInteract1>();
									component.cnc.LockPick(true, component.id);
								}
								else if (time == 17.1f && timer == 0f)
								{
									hitInfo.collider.gameObject.GetComponent<PortcullisController>().pnc.LockPick(true);
								}
								RaycastHit raycastHit;
								if (!Physics.Raycast(new Ray(this.InteractorSource.position, this.InteractorSource.forward), out raycastHit, this.InteractRange, this.layers) || !(hitInfo.collider.gameObject == raycastHit.collider.gameObject))
								{
									break;
								}
								timer += Time.deltaTime;
								this.uiFill.fillAmount = Mathf.Lerp(0f, 1f, timer / time);
								if (timer > time)
								{
									if (time == 15.1f)
									{
										hitInfo.collider.gameObject.GetComponent<ChestInteract>().cnc.LockPick(false);
									}
									else if (time == 6.1f)
									{
										ChestInteract1 component2 = hitInfo.collider.gameObject.GetComponent<ChestInteract1>();
										component2.cnc.LockPick(false, component2.id);
									}
									else if (time == 17.1f)
									{
										hitInfo.collider.gameObject.GetComponent<PortcullisController>().pnc.LockPick(false);
									}
									timedInteractObj.Interact(base.gameObject.transform.parent.gameObject);
									this.InteractUI.gameObject.SetActive(false);
									break;
								}
								yield return null;
							}
							if (time == 15.1f && timer <= time)
							{
								hitInfo.collider.gameObject.GetComponent<ChestInteract>().cnc.LockPick(false);
							}
							else if (time == 6.1f && timer <= time)
							{
								ChestInteract1 component3 = hitInfo.collider.gameObject.GetComponent<ChestInteract1>();
								component3.cnc.LockPick(false, component3.id);
							}
							else if (time == 17.1f && timer <= time)
							{
								hitInfo.collider.gameObject.GetComponent<PortcullisController>().pnc.LockPick(false);
							}
							this.uiFill.fillAmount = 0f;
							this.uiTimer.enabled = false;
							this.uiFill.enabled = false;
							yield return new WaitForSeconds(0.2f);
						}
						else if (this.InteractUI.gameObject.activeSelf)
						{
							this.InteractUI.gameObject.SetActive(false);
						}
						timedInteractObj = null;
					}
				}
				else if (this.InteractUI != null && this.InteractUI.gameObject.activeSelf)
				{
					this.InteractUI.gameObject.SetActive(false);
				}
			}
			else if (this.InteractUI != null && this.InteractUI.gameObject.activeSelf)
			{
				this.InteractUI.gameObject.SetActive(false);
			}
			yield return null;
			hitInfo = default(RaycastHit);
		}
		yield break;
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x00029794 File Offset: 0x00027994
	public void resetmap()
	{
		for (int i = 0; i < this.mapcanvas.transform.childCount; i++)
		{
			GameObject gameObject = this.mapcanvas.transform.GetChild(i).gameObject;
			if (!gameObject.CompareTag("map") && !gameObject.CompareTag("dontdelete"))
			{
				Object.Destroy(gameObject);
			}
		}
	}

	// Token: 0x040005C2 RID: 1474
	public Transform InteractorSource;

	// Token: 0x040005C3 RID: 1475
	public float InteractRange;

	// Token: 0x040005C4 RID: 1476
	public Text InteractUI;

	// Token: 0x040005C5 RID: 1477
	public Text InteractUI2;

	// Token: 0x040005C6 RID: 1478
	public GameObject uiHand;

	// Token: 0x040005C7 RID: 1479
	public LayerMask layers;

	// Token: 0x040005C8 RID: 1480
	public Image uiTimer;

	// Token: 0x040005C9 RID: 1481
	public Image uiFill;

	// Token: 0x040005CA RID: 1482
	private SettingsHolder Sholder;

	// Token: 0x040005CB RID: 1483
	public GameObject map;

	// Token: 0x040005CC RID: 1484
	public Animator mapani;

	// Token: 0x040005CD RID: 1485
	private bool hasPlayedshite = true;

	// Token: 0x040005CE RID: 1486
	public GameObject mapcanvas;

	// Token: 0x040005CF RID: 1487
	public Material[] levelNumberMats;

	// Token: 0x040005D0 RID: 1488
	public DecalProjector lvlnum;

	// Token: 0x040005D1 RID: 1489
	public CanvasGroup lvluptext;

	// Token: 0x040005D2 RID: 1490
	public TextMeshProUGUI lvltextactual;

	// Token: 0x040005D3 RID: 1491
	public TextMeshProUGUI lvltextactual2;

	// Token: 0x040005D4 RID: 1492
	private GameObject player;

	// Token: 0x040005D5 RID: 1493
	private float waitqueue;
}
