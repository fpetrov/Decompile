using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Delegating;
using FishNet.Serializing;
using FishNet.Transporting;
using UnityEngine;

namespace DrawPixels_Done
{
	// Token: 0x0200023F RID: 575
	public class DrawPixels : NetworkBehaviour
	{
		// Token: 0x1700025F RID: 607
		// (get) Token: 0x060016B7 RID: 5815 RVA: 0x0005EA58 File Offset: 0x0005CC58
		// (set) Token: 0x060016B8 RID: 5816 RVA: 0x0005EA60 File Offset: 0x0005CC60
		public DrawPixels Instance { get; private set; }

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x060016B9 RID: 5817 RVA: 0x0005EA6C File Offset: 0x0005CC6C
		// (remove) Token: 0x060016BA RID: 5818 RVA: 0x0005EAA4 File Offset: 0x0005CCA4
		public event EventHandler OnColorChanged;

		// Token: 0x060016BB RID: 5819 RVA: 0x0005EADC File Offset: 0x0005CCDC
		public virtual void Awake()
		{
			this.NetworkInitialize___Early();
			this.Awake_UserLogic_DrawPixels_Done.DrawPixels_Assembly-CSharp.dll();
			this.NetworkInitialize__Late();
		}

		// Token: 0x060016BC RID: 5820 RVA: 0x0005EAFB File Offset: 0x0005CCFB
		public void StartEditing()
		{
			this.isBeingEdited = true;
			base.StartCoroutine(this.DrawPix());
		}

		// Token: 0x060016BD RID: 5821 RVA: 0x0005EB11 File Offset: 0x0005CD11
		private IEnumerator DrawPix()
		{
			while (this.isBeingEdited)
			{
				if (Input.GetMouseButton(0))
				{
					Vector3 mouseWorldPosition = DrawPixels.GetMouseWorldPosition();
					int cursorSizeInt = this.GetCursorSizeInt();
					for (int i = 0; i < cursorSizeInt; i++)
					{
						for (int j = 0; j < cursorSizeInt; j++)
						{
							this.gridWorldPosition = mouseWorldPosition + new Vector3((float)i, (float)j) * this.cellSize;
							if (this.grid.GetGridObject(this.gridWorldPosition) != null)
							{
								this.SyncPix(this.colorUV, this.gridWorldPosition);
							}
						}
					}
					RaycastHit raycastHit;
					if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit, 999f, this.drawlayer))
					{
						this.colorUV = raycastHit.textureCoord;
						EventHandler onColorChanged = this.OnColorChanged;
						if (onColorChanged != null)
						{
							onColorChanged(this, EventArgs.Empty);
						}
					}
				}
				yield return new WaitForSeconds(0.025f);
			}
			yield break;
		}

		// Token: 0x060016BE RID: 5822 RVA: 0x0005EB20 File Offset: 0x0005CD20
		[ServerRpc(RequireOwnership = false)]
		private void SyncPix(Vector2 CUV, Vector3 GridPos)
		{
			this.RpcWriter___Server_SyncPix_4162922948(CUV, GridPos);
		}

		// Token: 0x060016BF RID: 5823 RVA: 0x0005EB30 File Offset: 0x0005CD30
		[ObserversRpc]
		private void ObsSyncPix(Vector2 CUV, Vector3 GridPos)
		{
			this.RpcWriter___Observers_ObsSyncPix_4162922948(CUV, GridPos);
		}

		// Token: 0x060016C0 RID: 5824 RVA: 0x0005EB4C File Offset: 0x0005CD4C
		public static Vector3 GetMouseWorldPosition()
		{
			Vector3 mouseWorldPositionWithZ = DrawPixels.GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
			mouseWorldPositionWithZ.z = 0f;
			return mouseWorldPositionWithZ;
		}

		// Token: 0x060016C1 RID: 5825 RVA: 0x0001684E File Offset: 0x00014A4E
		public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
		{
			return worldCamera.ScreenToWorldPoint(screenPosition);
		}

		// Token: 0x060016C2 RID: 5826 RVA: 0x0005EB76 File Offset: 0x0005CD76
		public Grid<DrawPixels.PixelGridObject> GetGrid()
		{
			return this.grid;
		}

		// Token: 0x060016C3 RID: 5827 RVA: 0x0005EB7E File Offset: 0x0005CD7E
		public Vector2 GetColorUV()
		{
			return this.colorUV;
		}

		// Token: 0x060016C4 RID: 5828 RVA: 0x0005EB86 File Offset: 0x0005CD86
		public void SetCursorSize(DrawPixels.CursorSize cursorSize)
		{
			this.cursorSize = cursorSize;
		}

		// Token: 0x060016C5 RID: 5829 RVA: 0x0005EB90 File Offset: 0x0005CD90
		private int GetCursorSizeInt()
		{
			switch (this.cursorSize)
			{
			case DrawPixels.CursorSize.Small:
				return 1;
			default:
				return 3;
			case DrawPixels.CursorSize.Large:
				return 7;
			}
		}

		// Token: 0x060016C6 RID: 5830 RVA: 0x0005EBBC File Offset: 0x0005CDBC
		public void SaveImage(Action<Texture2D> onSaveImage)
		{
			Texture2D texture2D = new Texture2D(this.grid.GetWidth(), this.grid.GetHeight(), TextureFormat.ARGB32, false);
			texture2D.filterMode = FilterMode.Point;
			for (int i = 0; i < this.grid.GetWidth(); i++)
			{
				for (int j = 0; j < this.grid.GetHeight(); j++)
				{
					Vector2 vector = this.grid.GetGridObject(i, j).GetColorUV();
					vector.x *= (float)this.colorsTexture.width;
					vector.y *= (float)this.colorsTexture.height;
					texture2D.SetPixel(i, j, this.colorsTexture.GetPixel((int)vector.x, (int)vector.y));
				}
			}
			texture2D.Apply();
			onSaveImage(texture2D);
		}

		// Token: 0x060016C7 RID: 5831 RVA: 0x0005EC8C File Offset: 0x0005CE8C
		public void SaveFlagPermanently()
		{
			string text = this.SerializeGrid();
			PlayerPrefs.SetString("flagGrid", text);
			PlayerPrefs.Save();
		}

		// Token: 0x060016C8 RID: 5832 RVA: 0x0005ECB0 File Offset: 0x0005CEB0
		public string SerializeGrid()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < this.grid.GetWidth(); i++)
			{
				for (int j = 0; j < this.grid.GetHeight(); j++)
				{
					Vector2 vector = this.grid.GetGridObject(i, j).GetColorUV();
					list.Add(string.Format("{0:F2}:{1:F2}", vector.x, vector.y));
				}
			}
			return string.Join(",", list);
		}

		// Token: 0x060016C9 RID: 5833 RVA: 0x0005ED33 File Offset: 0x0005CF33
		public void loadflag()
		{
			if (PlayerPrefs.HasKey("flagGrid"))
			{
				this.ServerLoadFlag(PlayerPrefs.GetString("flagGrid"));
			}
		}

		// Token: 0x060016CA RID: 5834 RVA: 0x0005ED51 File Offset: 0x0005CF51
		[ServerRpc(RequireOwnership = false)]
		private void ServerLoadFlag(string flagdata)
		{
			this.RpcWriter___Server_ServerLoadFlag_3615296227(flagdata);
		}

		// Token: 0x060016CB RID: 5835 RVA: 0x0005ED5D File Offset: 0x0005CF5D
		[ObserversRpc]
		private void ObsLoadFlag(string flagdata)
		{
			this.RpcWriter___Observers_ObsLoadFlag_3615296227(flagdata);
		}

		// Token: 0x060016CC RID: 5836 RVA: 0x0005ED6C File Offset: 0x0005CF6C
		public void DeserializeGrid(string serialized)
		{
			string[] array = serialized.Split(',', StringSplitOptions.None);
			int width = this.grid.GetWidth();
			int height = this.grid.GetHeight();
			int num = 0;
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					if (num >= array.Length)
					{
						return;
					}
					string[] array2 = array[num].Split(':', StringSplitOptions.None);
					float num2 = float.Parse(array2[0]);
					float num3 = float.Parse(array2[1]);
					this.grid.GetGridObject(i, j).SetColorUV(new Vector2(num2, num3));
					num++;
				}
			}
		}

		// Token: 0x060016CE RID: 5838 RVA: 0x0005EE18 File Offset: 0x0005D018
		public virtual void NetworkInitialize___Early()
		{
			if (this.NetworkInitialize___EarlyDrawPixels_Done.DrawPixelsAssembly-CSharp.dll_Excuted)
			{
				return;
			}
			this.NetworkInitialize___EarlyDrawPixels_Done.DrawPixelsAssembly-CSharp.dll_Excuted = true;
			base.RegisterServerRpc(0U, new ServerRpcDelegate(this.RpcReader___Server_SyncPix_4162922948));
			base.RegisterObserversRpc(1U, new ClientRpcDelegate(this.RpcReader___Observers_ObsSyncPix_4162922948));
			base.RegisterServerRpc(2U, new ServerRpcDelegate(this.RpcReader___Server_ServerLoadFlag_3615296227));
			base.RegisterObserversRpc(3U, new ClientRpcDelegate(this.RpcReader___Observers_ObsLoadFlag_3615296227));
		}

		// Token: 0x060016CF RID: 5839 RVA: 0x0005EE92 File Offset: 0x0005D092
		public virtual void NetworkInitialize__Late()
		{
			if (this.NetworkInitialize__LateDrawPixels_Done.DrawPixelsAssembly-CSharp.dll_Excuted)
			{
				return;
			}
			this.NetworkInitialize__LateDrawPixels_Done.DrawPixelsAssembly-CSharp.dll_Excuted = true;
		}

		// Token: 0x060016D0 RID: 5840 RVA: 0x0005EEA5 File Offset: 0x0005D0A5
		public override void NetworkInitializeIfDisabled()
		{
			this.NetworkInitialize___Early();
			this.NetworkInitialize__Late();
		}

		// Token: 0x060016D1 RID: 5841 RVA: 0x0005EEB4 File Offset: 0x0005D0B4
		private void RpcWriter___Server_SyncPix_4162922948(Vector2 CUV, Vector3 GridPos)
		{
			if (!base.IsClientInitialized)
			{
				NetworkManager networkManager = base.NetworkManager;
				networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
				return;
			}
			Channel channel = Channel.Reliable;
			PooledWriter pooledWriter = WriterPool.Retrieve();
			pooledWriter.WriteVector2(CUV);
			pooledWriter.WriteVector3(GridPos);
			base.SendServerRpc(0U, pooledWriter, channel, DataOrderType.Default);
			pooledWriter.Store();
		}

		// Token: 0x060016D2 RID: 5842 RVA: 0x0005EF33 File Offset: 0x0005D133
		private void RpcLogic___SyncPix_4162922948(Vector2 CUV, Vector3 GridPos)
		{
			this.ObsSyncPix(CUV, GridPos);
		}

		// Token: 0x060016D3 RID: 5843 RVA: 0x0005EF40 File Offset: 0x0005D140
		private void RpcReader___Server_SyncPix_4162922948(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
		{
			Vector2 vector = PooledReader0.ReadVector2();
			Vector3 vector2 = PooledReader0.ReadVector3();
			if (!base.IsServerInitialized)
			{
				return;
			}
			this.RpcLogic___SyncPix_4162922948(vector, vector2);
		}

		// Token: 0x060016D4 RID: 5844 RVA: 0x0005EF84 File Offset: 0x0005D184
		private void RpcWriter___Observers_ObsSyncPix_4162922948(Vector2 CUV, Vector3 GridPos)
		{
			if (!base.IsServerInitialized)
			{
				NetworkManager networkManager = base.NetworkManager;
				networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
				return;
			}
			Channel channel = Channel.Reliable;
			PooledWriter pooledWriter = WriterPool.Retrieve();
			pooledWriter.WriteVector2(CUV);
			pooledWriter.WriteVector3(GridPos);
			base.SendObserversRpc(1U, pooledWriter, channel, DataOrderType.Default, false, false, false);
			pooledWriter.Store();
		}

		// Token: 0x060016D5 RID: 5845 RVA: 0x0005F014 File Offset: 0x0005D214
		private void RpcLogic___ObsSyncPix_4162922948(Vector2 CUV, Vector3 GridPos)
		{
			DrawPixels.PixelGridObject gridObject = this.grid.GetGridObject(GridPos);
			if (gridObject != null)
			{
				gridObject.SetColorUV(CUV);
			}
		}

		// Token: 0x060016D6 RID: 5846 RVA: 0x0005F038 File Offset: 0x0005D238
		private void RpcReader___Observers_ObsSyncPix_4162922948(PooledReader PooledReader0, Channel channel)
		{
			Vector2 vector = PooledReader0.ReadVector2();
			Vector3 vector2 = PooledReader0.ReadVector3();
			if (!base.IsClientInitialized)
			{
				return;
			}
			this.RpcLogic___ObsSyncPix_4162922948(vector, vector2);
		}

		// Token: 0x060016D7 RID: 5847 RVA: 0x0005F07C File Offset: 0x0005D27C
		private void RpcWriter___Server_ServerLoadFlag_3615296227(string flagdata)
		{
			if (!base.IsClientInitialized)
			{
				NetworkManager networkManager = base.NetworkManager;
				networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
				return;
			}
			Channel channel = Channel.Reliable;
			PooledWriter pooledWriter = WriterPool.Retrieve();
			pooledWriter.WriteString(flagdata);
			base.SendServerRpc(2U, pooledWriter, channel, DataOrderType.Default);
			pooledWriter.Store();
		}

		// Token: 0x060016D8 RID: 5848 RVA: 0x0005F0EE File Offset: 0x0005D2EE
		private void RpcLogic___ServerLoadFlag_3615296227(string flagdata)
		{
			if (!Regex.IsMatch(flagdata, "^[0-9:\\.,\\-]+$"))
			{
				return;
			}
			this.ObsLoadFlag(flagdata);
		}

		// Token: 0x060016D9 RID: 5849 RVA: 0x0005F108 File Offset: 0x0005D308
		private void RpcReader___Server_ServerLoadFlag_3615296227(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
		{
			string text = PooledReader0.ReadString();
			if (!base.IsServerInitialized)
			{
				return;
			}
			this.RpcLogic___ServerLoadFlag_3615296227(text);
		}

		// Token: 0x060016DA RID: 5850 RVA: 0x0005F13C File Offset: 0x0005D33C
		private void RpcWriter___Observers_ObsLoadFlag_3615296227(string flagdata)
		{
			if (!base.IsServerInitialized)
			{
				NetworkManager networkManager = base.NetworkManager;
				networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
				return;
			}
			Channel channel = Channel.Reliable;
			PooledWriter pooledWriter = WriterPool.Retrieve();
			pooledWriter.WriteString(flagdata);
			base.SendObserversRpc(3U, pooledWriter, channel, DataOrderType.Default, false, false, false);
			pooledWriter.Store();
		}

		// Token: 0x060016DB RID: 5851 RVA: 0x0005F1BD File Offset: 0x0005D3BD
		private void RpcLogic___ObsLoadFlag_3615296227(string flagdata)
		{
			this.DeserializeGrid(flagdata);
		}

		// Token: 0x060016DC RID: 5852 RVA: 0x0005F1C8 File Offset: 0x0005D3C8
		private void RpcReader___Observers_ObsLoadFlag_3615296227(PooledReader PooledReader0, Channel channel)
		{
			string text = PooledReader0.ReadString();
			if (!base.IsClientInitialized)
			{
				return;
			}
			this.RpcLogic___ObsLoadFlag_3615296227(text);
		}

		// Token: 0x060016DD RID: 5853 RVA: 0x0005F1FC File Offset: 0x0005D3FC
		private void Awake_UserLogic_DrawPixels_Done.DrawPixels_Assembly-CSharp.dll()
		{
			this.Instance = this;
			this.grid = new Grid<DrawPixels.PixelGridObject>(100, 66, this.cellSize, Vector3.zero, (Grid<DrawPixels.PixelGridObject> g, int x, int y) => new DrawPixels.PixelGridObject(g, x, y));
			this.colorUV = new Vector2(0f, 0f);
			this.cursorSize = DrawPixels.CursorSize.Small;
		}

		// Token: 0x04000D13 RID: 3347
		public LayerMask drawlayer;

		// Token: 0x04000D16 RID: 3350
		public bool isBeingEdited;

		// Token: 0x04000D17 RID: 3351
		[SerializeField]
		private Texture2D colorsTexture;

		// Token: 0x04000D18 RID: 3352
		private Grid<DrawPixels.PixelGridObject> grid;

		// Token: 0x04000D19 RID: 3353
		private float cellSize = 1f;

		// Token: 0x04000D1A RID: 3354
		private Vector2 colorUV;

		// Token: 0x04000D1B RID: 3355
		private DrawPixels.CursorSize cursorSize;

		// Token: 0x04000D1C RID: 3356
		private Vector3 gridWorldPosition;

		// Token: 0x04000D1D RID: 3357
		private bool NetworkInitialize___EarlyDrawPixels_Done.DrawPixelsAssembly-CSharp.dll_Excuted;

		// Token: 0x04000D1E RID: 3358
		private bool NetworkInitialize__LateDrawPixels_Done.DrawPixelsAssembly-CSharp.dll_Excuted;

		// Token: 0x02000240 RID: 576
		public enum CursorSize
		{
			// Token: 0x04000D20 RID: 3360
			Small,
			// Token: 0x04000D21 RID: 3361
			Medium,
			// Token: 0x04000D22 RID: 3362
			Large
		}

		// Token: 0x02000241 RID: 577
		public class PixelGridObject
		{
			// Token: 0x060016DE RID: 5854 RVA: 0x0005F265 File Offset: 0x0005D465
			public PixelGridObject(Grid<DrawPixels.PixelGridObject> grid, int x, int y)
			{
				this.grid = grid;
				this.x = x;
				this.y = y;
			}

			// Token: 0x060016DF RID: 5855 RVA: 0x0005F282 File Offset: 0x0005D482
			public void SetColorUV(Vector2 colorUV)
			{
				this.colorUV = colorUV;
				this.TriggerGridObjectChanged();
			}

			// Token: 0x060016E0 RID: 5856 RVA: 0x0005F291 File Offset: 0x0005D491
			public Vector2 GetColorUV()
			{
				return this.colorUV;
			}

			// Token: 0x060016E1 RID: 5857 RVA: 0x0005F299 File Offset: 0x0005D499
			private void TriggerGridObjectChanged()
			{
				this.grid.TriggerGridObjectChanged(this.x, this.y);
			}

			// Token: 0x060016E2 RID: 5858 RVA: 0x0005F2B2 File Offset: 0x0005D4B2
			public override string ToString()
			{
				return this.colorUV.x.ToString();
			}

			// Token: 0x04000D23 RID: 3363
			private Grid<DrawPixels.PixelGridObject> grid;

			// Token: 0x04000D24 RID: 3364
			private int x;

			// Token: 0x04000D25 RID: 3365
			private int y;

			// Token: 0x04000D26 RID: 3366
			private Vector2 colorUV;
		}
	}
}
