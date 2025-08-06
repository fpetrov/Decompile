using System;
using FishNet.Object;
using MagusToolkit;
using UnityEngine;

namespace MageArenaManager
{
	// Token: 0x0200001D RID: 29
	public class NoClip : MonoBehaviour
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00005756 File Offset: 0x00003956
		private float flySpeed
		{
			get
			{
				return ConfigManager.FlySpeed.Value;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00005762 File Offset: 0x00003962
		private float boostSpeed
		{
			get
			{
				return this.flySpeed + 10f;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00005770 File Offset: 0x00003970
		private PlayerMovement Player
		{
			get
			{
				return Globals.LocalPlayer;
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00005777 File Offset: 0x00003977
		private void Start()
		{
			NoClip.Instance = this;
			base.enabled = true;
			if (this.Player != null)
			{
				this.rb = this.Player.GetComponent<Rigidbody>();
				this.netObj = this.Player.GetComponent<NetworkObject>();
			}
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000057B8 File Offset: 0x000039B8
		private void Update()
		{
			if (this.Player == null || !ConfigManager.NoClip.Value)
			{
				if (this.flyEnabled)
				{
					this.ToggleFly(false);
				}
				return;
			}
			if (!this.flyEnabled)
			{
				this.ToggleFly(true);
			}
			this.HandleFlyMovement();
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00005804 File Offset: 0x00003A04
		private void HandleFlyMovement()
		{
			float num = (Input.GetKey(304) ? this.boostSpeed : this.flySpeed);
			Vector3 vector = Vector3.zero;
			if (Input.GetKey(119))
			{
				vector += Camera.main.transform.forward;
			}
			if (Input.GetKey(115))
			{
				vector -= Camera.main.transform.forward;
			}
			if (Input.GetKey(97))
			{
				vector -= Camera.main.transform.right;
			}
			if (Input.GetKey(100))
			{
				vector += Camera.main.transform.right;
			}
			if (Input.GetKey(32))
			{
				vector += Vector3.up;
			}
			if (Input.GetKey(306))
			{
				vector += Vector3.down;
			}
			if (vector.magnitude > 1f)
			{
				vector.Normalize();
			}
			Vector3 vector2 = this.Player.transform.position + vector * num * Time.deltaTime;
			this.Player.transform.position = vector2;
			if (this.netObj != null && this.netObj.IsOwner)
			{
				this.netObj.transform.position = vector2;
			}
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00005954 File Offset: 0x00003B54
		public void ToggleFly(bool enable)
		{
			this.flyEnabled = enable;
			if (this.Player == null)
			{
				return;
			}
			this.Player.canMove = !this.flyEnabled;
			if (this.rb == null)
			{
				this.rb = this.Player.GetComponent<Rigidbody>();
			}
			if (this.rb != null)
			{
				this.rb.useGravity = !this.flyEnabled;
				this.rb.isKinematic = this.flyEnabled;
				if (this.flyEnabled)
				{
					this.rb.velocity = Vector3.zero;
				}
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000059F8 File Offset: 0x00003BF8
		private void OnDestroy()
		{
			if (this.Player != null)
			{
				CharacterController component = this.Player.GetComponent<CharacterController>();
				if (component != null)
				{
					component.enabled = true;
					component.detectCollisions = true;
				}
				if (this.rb == null)
				{
					this.rb = this.Player.GetComponent<Rigidbody>();
				}
				if (this.rb != null)
				{
					this.rb.useGravity = true;
					this.rb.isKinematic = false;
					this.rb.velocity = Vector3.zero;
				}
				this.Player.canMove = true;
				this.Player.transform.position += Vector3.up * 0.1f;
			}
		}

		// Token: 0x04000053 RID: 83
		public static NoClip Instance;

		// Token: 0x04000054 RID: 84
		private Rigidbody rb;

		// Token: 0x04000055 RID: 85
		private NetworkObject netObj;

		// Token: 0x04000056 RID: 86
		private bool flyEnabled;
	}
}
