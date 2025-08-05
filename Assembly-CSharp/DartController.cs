using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x02000047 RID: 71
public class DartController : MonoBehaviour
{
	// Token: 0x06000317 RID: 791 RVA: 0x0000D6A2 File Offset: 0x0000B8A2
	public void Setup(Vector3 shootdir)
	{
		base.GetComponent<Rigidbody>().AddForce(shootdir * this.startVelocity, ForceMode.VelocityChange);
	}

	// Token: 0x06000318 RID: 792 RVA: 0x0000D6BC File Offset: 0x0000B8BC
	private IEnumerator HitSomething()
	{
		this.asauce.pitch = Random.Range(0.8f, 1.2f);
		this.asauce.PlayOneShot(this.dartDestroyed[Random.Range(0, this.dartDestroyed.Length)]);
		this.col.enabled = false;
		this.rendererer.SetActive(false);
		float timer = 0f;
		while (timer < 1f)
		{
			this.hdld.intensity = Mathf.Lerp(0f, 130000f, timer);
			this.hdld.range = Mathf.Lerp(1f, 10f, timer);
			timer += Time.deltaTime * 7f;
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x06000319 RID: 793 RVA: 0x0000D6CC File Offset: 0x0000B8CC
	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Ignorable") && !other.CompareTag("hex") && !this.collided)
		{
			this.collided = true;
			base.GetComponent<Rigidbody>().isKinematic = true;
			GetPlayerGameobject getPlayerGameobject;
			GetShadowWizardController getShadowWizardController;
			if (other.TryGetComponent<GetPlayerGameobject>(out getPlayerGameobject))
			{
				this.dgc.applydogshit(getPlayerGameobject.player);
			}
			else if (other.TryGetComponent<GetShadowWizardController>(out getShadowWizardController))
			{
				this.dgc.applydogshitai(getShadowWizardController.ShadowWizardAI);
			}
			else if (other.CompareTag("hitable"))
			{
				other.transform.GetComponent<IInteractableNetworkObj>().NetInteract();
			}
			base.StartCoroutine(this.HitSomething());
		}
	}

	// Token: 0x0400018D RID: 397
	private bool collided;

	// Token: 0x0400018E RID: 398
	public DartGunController dgc;

	// Token: 0x0400018F RID: 399
	public AudioSource asauce;

	// Token: 0x04000190 RID: 400
	public AudioClip[] dartDestroyed;

	// Token: 0x04000191 RID: 401
	public Collider col;

	// Token: 0x04000192 RID: 402
	public GameObject rendererer;

	// Token: 0x04000193 RID: 403
	public HDAdditionalLightData hdld;

	// Token: 0x04000194 RID: 404
	public float startVelocity;
}
