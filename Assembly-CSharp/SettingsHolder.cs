using System;
using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// Token: 0x0200017A RID: 378
public class SettingsHolder : MonoBehaviour
{
	// Token: 0x17000184 RID: 388
	// (get) Token: 0x06000F53 RID: 3923 RVA: 0x0003E28B File Offset: 0x0003C48B
	// (set) Token: 0x06000F54 RID: 3924 RVA: 0x0003E292 File Offset: 0x0003C492
	public static SettingsHolder Instance { get; private set; }

	// Token: 0x06000F55 RID: 3925 RVA: 0x0003E29A File Offset: 0x0003C49A
	public void SetVoiceModelPath(string path)
	{
		PlayerPrefs.SetString("path", path);
		this.VoiceModelPath = path;
	}

	// Token: 0x06000F56 RID: 3926 RVA: 0x0003E2B0 File Offset: 0x0003C4B0
	public void RestoreDefaults()
	{
		Application.targetFrameRate = -1;
		this.leftmouse = KeyCode.Mouse0;
		this.w2 = KeyCode.W;
		this.a2 = KeyCode.A;
		this.s2 = KeyCode.S;
		this.d2 = KeyCode.D;
		this.one = KeyCode.Alpha1;
		this.two = KeyCode.Alpha2;
		this.three = KeyCode.Alpha3;
		this.four = KeyCode.Alpha4;
		this.drop = KeyCode.G;
		this.interact = KeyCode.E;
		this.sprint = KeyCode.LeftShift;
		this.jump = KeyCode.Space;
		this.Crouch = KeyCode.LeftControl;
		this.map = KeyCode.Q;
		this.recall = KeyCode.B;
		this.ptt = KeyCode.V;
		this.item1 = KeyCode.Mouse3;
		this.item2 = KeyCode.R;
		this.item3 = KeyCode.F;
		this.item4 = KeyCode.C;
		this.item5 = KeyCode.X;
		this.useAltMoveControls = false;
		this.sense = 1f;
		this.SettingsCanvas.transform.Find("volume").GetComponent<Slider>().value = 0f;
		this.mixer.SetFloat("volume", 0f);
		this.mixer2.SetFloat("volume", 0f);
		if (PlayerPrefs.HasKey("framerate"))
		{
			PlayerPrefs.DeleteKey("framerate");
		}
		if (PlayerPrefs.HasKey("volume"))
		{
			PlayerPrefs.DeleteKey("volume");
		}
		if (PlayerPrefs.HasKey("item1"))
		{
			PlayerPrefs.DeleteKey("item1");
		}
		if (PlayerPrefs.HasKey("item2"))
		{
			PlayerPrefs.DeleteKey("item2");
		}
		if (PlayerPrefs.HasKey("item3"))
		{
			PlayerPrefs.DeleteKey("item3");
		}
		if (PlayerPrefs.HasKey("item4"))
		{
			PlayerPrefs.DeleteKey("item4");
		}
		if (PlayerPrefs.HasKey("item5"))
		{
			PlayerPrefs.DeleteKey("item5");
		}
		if (PlayerPrefs.HasKey("leftmouse"))
		{
			PlayerPrefs.DeleteKey("leftmouse");
		}
		if (PlayerPrefs.HasKey("w2"))
		{
			PlayerPrefs.DeleteKey("w2");
		}
		if (PlayerPrefs.HasKey("a2"))
		{
			PlayerPrefs.DeleteKey("a2");
		}
		if (PlayerPrefs.HasKey("s2"))
		{
			PlayerPrefs.DeleteKey("s2");
		}
		if (PlayerPrefs.HasKey("d2"))
		{
			PlayerPrefs.DeleteKey("d2");
		}
		if (PlayerPrefs.HasKey("one"))
		{
			PlayerPrefs.DeleteKey("one");
		}
		if (PlayerPrefs.HasKey("two"))
		{
			PlayerPrefs.DeleteKey("two");
		}
		if (PlayerPrefs.HasKey("three"))
		{
			PlayerPrefs.DeleteKey("three");
		}
		if (PlayerPrefs.HasKey("four"))
		{
			PlayerPrefs.DeleteKey("four");
		}
		if (PlayerPrefs.HasKey("drop"))
		{
			PlayerPrefs.DeleteKey("drop");
		}
		if (PlayerPrefs.HasKey("interact"))
		{
			PlayerPrefs.DeleteKey("interact");
		}
		if (PlayerPrefs.HasKey("sprint"))
		{
			PlayerPrefs.DeleteKey("sprint");
		}
		if (PlayerPrefs.HasKey("jump"))
		{
			PlayerPrefs.DeleteKey("jump");
		}
		if (PlayerPrefs.HasKey("Crouch"))
		{
			PlayerPrefs.DeleteKey("Crouch");
		}
		if (PlayerPrefs.HasKey("map"))
		{
			PlayerPrefs.DeleteKey("map");
		}
		if (PlayerPrefs.HasKey("recall"))
		{
			PlayerPrefs.DeleteKey("recall");
		}
		if (PlayerPrefs.HasKey("ptt"))
		{
			PlayerPrefs.DeleteKey("ptt");
		}
		if (PlayerPrefs.HasKey("sense"))
		{
			PlayerPrefs.DeleteKey("sense");
		}
		PlayerPrefs.Save();
	}

	// Token: 0x06000F57 RID: 3927 RVA: 0x0003E60C File Offset: 0x0003C80C
	public void setkey(string kts)
	{
		base.StartCoroutine(this.checkforkeypress(kts));
	}

	// Token: 0x06000F58 RID: 3928 RVA: 0x0003E61C File Offset: 0x0003C81C
	private IEnumerator checkforkeypress(string keytoset)
	{
		bool hasbeenset = false;
		while (!hasbeenset)
		{
			if (Input.anyKeyDown)
			{
				foreach (object obj in Enum.GetValues(typeof(KeyCode)))
				{
					KeyCode keyCode = (KeyCode)obj;
					if (Input.GetKeyDown(keyCode))
					{
						PlayerPrefs.SetInt(keytoset, (int)keyCode);
						this.SetVariableByName(this, keytoset, keyCode);
						hasbeenset = true;
						this.SettingsCanvas.transform.Find(keytoset).GetChild(0).GetComponent<Text>()
							.text = keyCode.ToString();
						this.SettingsCanvas.transform.Find(keytoset).GetChild(1).GetComponent<Text>()
							.text = keyCode.ToString();
						break;
					}
				}
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000F59 RID: 3929 RVA: 0x0003E634 File Offset: 0x0003C834
	public void LoadKeybinds()
	{
		if (PlayerPrefs.HasKey("item1"))
		{
			KeyCode @int = (KeyCode)PlayerPrefs.GetInt("item1");
			this.item1 = @int;
			this.SettingsCanvas.transform.Find("item1").GetChild(0).GetComponent<Text>()
				.text = @int.ToString();
			this.SettingsCanvas.transform.Find("item1").GetChild(1).GetComponent<Text>()
				.text = @int.ToString();
		}
		if (PlayerPrefs.HasKey("item2"))
		{
			KeyCode int2 = (KeyCode)PlayerPrefs.GetInt("item2");
			this.item2 = int2;
			this.SettingsCanvas.transform.Find("item2").GetChild(0).GetComponent<Text>()
				.text = int2.ToString();
			this.SettingsCanvas.transform.Find("item2").GetChild(1).GetComponent<Text>()
				.text = int2.ToString();
		}
		if (PlayerPrefs.HasKey("item3"))
		{
			KeyCode int3 = (KeyCode)PlayerPrefs.GetInt("item3");
			this.item3 = int3;
			this.SettingsCanvas.transform.Find("item3").GetChild(0).GetComponent<Text>()
				.text = int3.ToString();
			this.SettingsCanvas.transform.Find("item3").GetChild(1).GetComponent<Text>()
				.text = int3.ToString();
		}
		if (PlayerPrefs.HasKey("item4"))
		{
			KeyCode int4 = (KeyCode)PlayerPrefs.GetInt("item4");
			this.item4 = int4;
			this.SettingsCanvas.transform.Find("item4").GetChild(0).GetComponent<Text>()
				.text = int4.ToString();
			this.SettingsCanvas.transform.Find("item4").GetChild(1).GetComponent<Text>()
				.text = int4.ToString();
		}
		if (PlayerPrefs.HasKey("item5"))
		{
			KeyCode int5 = (KeyCode)PlayerPrefs.GetInt("item5");
			this.item5 = int5;
			this.SettingsCanvas.transform.Find("item5").GetChild(0).GetComponent<Text>()
				.text = int5.ToString();
			this.SettingsCanvas.transform.Find("item5").GetChild(1).GetComponent<Text>()
				.text = int5.ToString();
		}
		if (PlayerPrefs.HasKey("leftmouse"))
		{
			KeyCode int6 = (KeyCode)PlayerPrefs.GetInt("leftmouse");
			this.leftmouse = int6;
			this.SettingsCanvas.transform.Find("leftmouse").GetChild(0).GetComponent<Text>()
				.text = int6.ToString();
			this.SettingsCanvas.transform.Find("leftmouse").GetChild(1).GetComponent<Text>()
				.text = int6.ToString();
		}
		if (PlayerPrefs.HasKey("w2"))
		{
			KeyCode int7 = (KeyCode)PlayerPrefs.GetInt("w2");
			this.w2 = int7;
			this.SettingsCanvas.transform.Find("w2").GetChild(0).GetComponent<Text>()
				.text = int7.ToString();
			this.SettingsCanvas.transform.Find("w2").GetChild(1).GetComponent<Text>()
				.text = int7.ToString();
		}
		if (PlayerPrefs.HasKey("a2"))
		{
			KeyCode int8 = (KeyCode)PlayerPrefs.GetInt("a2");
			this.a2 = int8;
			this.SettingsCanvas.transform.Find("a2").GetChild(0).GetComponent<Text>()
				.text = int8.ToString();
			this.SettingsCanvas.transform.Find("a2").GetChild(1).GetComponent<Text>()
				.text = int8.ToString();
		}
		if (PlayerPrefs.HasKey("s2"))
		{
			KeyCode int9 = (KeyCode)PlayerPrefs.GetInt("s2");
			this.s2 = int9;
			this.SettingsCanvas.transform.Find("s2").GetChild(0).GetComponent<Text>()
				.text = int9.ToString();
			this.SettingsCanvas.transform.Find("s2").GetChild(1).GetComponent<Text>()
				.text = int9.ToString();
		}
		if (PlayerPrefs.HasKey("d2"))
		{
			KeyCode int10 = (KeyCode)PlayerPrefs.GetInt("d2");
			this.d2 = int10;
			this.SettingsCanvas.transform.Find("d2").GetChild(0).GetComponent<Text>()
				.text = int10.ToString();
			this.SettingsCanvas.transform.Find("d2").GetChild(1).GetComponent<Text>()
				.text = int10.ToString();
		}
		if (PlayerPrefs.HasKey("one"))
		{
			KeyCode int11 = (KeyCode)PlayerPrefs.GetInt("one");
			this.one = int11;
			this.SettingsCanvas.transform.Find("one").GetChild(0).GetComponent<Text>()
				.text = int11.ToString();
			this.SettingsCanvas.transform.Find("one").GetChild(1).GetComponent<Text>()
				.text = int11.ToString();
		}
		if (PlayerPrefs.HasKey("two"))
		{
			KeyCode int12 = (KeyCode)PlayerPrefs.GetInt("two");
			this.two = int12;
			this.SettingsCanvas.transform.Find("two").GetChild(0).GetComponent<Text>()
				.text = int12.ToString();
			this.SettingsCanvas.transform.Find("two").GetChild(1).GetComponent<Text>()
				.text = int12.ToString();
		}
		if (PlayerPrefs.HasKey("three"))
		{
			KeyCode int13 = (KeyCode)PlayerPrefs.GetInt("three");
			this.three = int13;
			this.SettingsCanvas.transform.Find("three").GetChild(0).GetComponent<Text>()
				.text = int13.ToString();
			this.SettingsCanvas.transform.Find("three").GetChild(1).GetComponent<Text>()
				.text = int13.ToString();
		}
		if (PlayerPrefs.HasKey("four"))
		{
			KeyCode int14 = (KeyCode)PlayerPrefs.GetInt("four");
			this.four = int14;
			this.SettingsCanvas.transform.Find("four").GetChild(0).GetComponent<Text>()
				.text = int14.ToString();
			this.SettingsCanvas.transform.Find("four").GetChild(1).GetComponent<Text>()
				.text = int14.ToString();
		}
		if (PlayerPrefs.HasKey("drop"))
		{
			KeyCode int15 = (KeyCode)PlayerPrefs.GetInt("drop");
			this.drop = int15;
			this.SettingsCanvas.transform.Find("drop").GetChild(0).GetComponent<Text>()
				.text = int15.ToString();
			this.SettingsCanvas.transform.Find("drop").GetChild(1).GetComponent<Text>()
				.text = int15.ToString();
		}
		if (PlayerPrefs.HasKey("interact"))
		{
			KeyCode int16 = (KeyCode)PlayerPrefs.GetInt("interact");
			this.interact = int16;
			this.SettingsCanvas.transform.Find("interact").GetChild(0).GetComponent<Text>()
				.text = int16.ToString();
			this.SettingsCanvas.transform.Find("interact").GetChild(1).GetComponent<Text>()
				.text = int16.ToString();
		}
		if (PlayerPrefs.HasKey("sprint"))
		{
			KeyCode int17 = (KeyCode)PlayerPrefs.GetInt("sprint");
			this.sprint = int17;
			this.SettingsCanvas.transform.Find("sprint").GetChild(0).GetComponent<Text>()
				.text = int17.ToString();
			this.SettingsCanvas.transform.Find("sprint").GetChild(1).GetComponent<Text>()
				.text = int17.ToString();
		}
		if (PlayerPrefs.HasKey("jump"))
		{
			KeyCode int18 = (KeyCode)PlayerPrefs.GetInt("jump");
			this.jump = int18;
			this.SettingsCanvas.transform.Find("jump").GetChild(0).GetComponent<Text>()
				.text = int18.ToString();
			this.SettingsCanvas.transform.Find("jump").GetChild(1).GetComponent<Text>()
				.text = int18.ToString();
		}
		if (PlayerPrefs.HasKey("Crouch"))
		{
			KeyCode int19 = (KeyCode)PlayerPrefs.GetInt("Crouch");
			this.Crouch = int19;
			this.SettingsCanvas.transform.Find("Crouch").GetChild(0).GetComponent<Text>()
				.text = int19.ToString();
			this.SettingsCanvas.transform.Find("Crouch").GetChild(1).GetComponent<Text>()
				.text = int19.ToString();
		}
		if (PlayerPrefs.HasKey("map"))
		{
			KeyCode int20 = (KeyCode)PlayerPrefs.GetInt("map");
			this.map = int20;
			this.SettingsCanvas.transform.Find("map").GetChild(0).GetComponent<Text>()
				.text = int20.ToString();
			this.SettingsCanvas.transform.Find("map").GetChild(1).GetComponent<Text>()
				.text = int20.ToString();
		}
		if (PlayerPrefs.HasKey("recall"))
		{
			KeyCode int21 = (KeyCode)PlayerPrefs.GetInt("recall");
			this.recall = int21;
			this.SettingsCanvas.transform.Find("recall").GetChild(0).GetComponent<Text>()
				.text = int21.ToString();
			this.SettingsCanvas.transform.Find("recall").GetChild(1).GetComponent<Text>()
				.text = int21.ToString();
		}
		if (PlayerPrefs.HasKey("ptt"))
		{
			KeyCode int22 = (KeyCode)PlayerPrefs.GetInt("ptt");
			this.ptt = int22;
			this.SettingsCanvas.transform.Find("ptt").GetChild(0).GetComponent<Text>()
				.text = int22.ToString();
			this.SettingsCanvas.transform.Find("ptt").GetChild(1).GetComponent<Text>()
				.text = int22.ToString();
		}
		if (PlayerPrefs.HasKey("sense"))
		{
			float @float = PlayerPrefs.GetFloat("sense");
			this.sense = @float;
			this.SettingsCanvas.transform.Find("sense").GetComponent<TextMeshProUGUI>().text = @float.ToString();
		}
		if (PlayerPrefs.HasKey("volume"))
		{
			float float2 = PlayerPrefs.GetFloat("volume");
			this.SettingsCanvas.transform.Find("volume").GetComponent<Slider>().value = float2;
			this.mixer.SetFloat("volume", float2);
			this.mixer2.SetFloat("volume", float2);
		}
		if (PlayerPrefs.HasKey("path"))
		{
			this.VoiceModelPath = PlayerPrefs.GetString("path");
		}
		if (PlayerPrefs.HasKey("framerate"))
		{
			Application.targetFrameRate = PlayerPrefs.GetInt("framerate");
		}
	}

	// Token: 0x06000F5A RID: 3930 RVA: 0x0003F26D File Offset: 0x0003D46D
	public void setsense(float val)
	{
		PlayerPrefs.SetFloat("sense", val);
		this.sense = val;
	}

	// Token: 0x06000F5B RID: 3931 RVA: 0x0003F281 File Offset: 0x0003D481
	public void setvolume(float val)
	{
		PlayerPrefs.SetFloat("volume", val);
		this.mixer.SetFloat("volume", val);
		this.mixer2.SetFloat("volume", val);
	}

	// Token: 0x06000F5C RID: 3932 RVA: 0x0003F2B2 File Offset: 0x0003D4B2
	public void togglePtt()
	{
		this.usePtt = !this.usePtt;
	}

	// Token: 0x06000F5D RID: 3933 RVA: 0x0003F2C4 File Offset: 0x0003D4C4
	private void SetVariableByName(object target, string variableName, object value)
	{
		FieldInfo field = target.GetType().GetField(variableName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (field != null)
		{
			field.SetValue(target, value);
		}
	}

	// Token: 0x06000F5E RID: 3934 RVA: 0x0003F2F1 File Offset: 0x0003D4F1
	public void SetFramerateCap(int val)
	{
		Application.targetFrameRate = val;
		PlayerPrefs.SetInt("framerate", val);
	}

	// Token: 0x06000F5F RID: 3935 RVA: 0x0003F304 File Offset: 0x0003D504
	private void Awake()
	{
		SettingsHolder.Instance = this;
	}

	// Token: 0x06000F60 RID: 3936 RVA: 0x0003F30C File Offset: 0x0003D50C
	private void Start()
	{
		base.StartCoroutine(this.FindSettingsButtons());
	}

	// Token: 0x06000F61 RID: 3937 RVA: 0x0003F31B File Offset: 0x0003D51B
	public void SetSettings(GameObject obj)
	{
		this.SettingsCanvas = obj;
	}

	// Token: 0x06000F62 RID: 3938 RVA: 0x0003F324 File Offset: 0x0003D524
	private IEnumerator FindSettingsButtons()
	{
		while (this.SettingsCanvas == null)
		{
			yield return null;
		}
		this.SettingsCanvas.transform.Find("60").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.SetFramerateCap(60);
		});
		this.SettingsCanvas.transform.Find("90").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.SetFramerateCap(90);
		});
		this.SettingsCanvas.transform.Find("144").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.SetFramerateCap(144);
		});
		this.SettingsCanvas.transform.Find("fs").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.ChangeScreenMode(FullScreenMode.ExclusiveFullScreen);
		});
		this.SettingsCanvas.transform.Find("windowed").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.ChangeScreenMode(FullScreenMode.Windowed);
		});
		this.SettingsCanvas.transform.Find("Borderless").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.ChangeScreenMode(FullScreenMode.FullScreenWindow);
		});
		this.SettingsCanvas.transform.Find("w2").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.setkey("w2");
		});
		this.SettingsCanvas.transform.Find("a2").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.setkey("a2");
		});
		this.SettingsCanvas.transform.Find("s2").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.setkey("s2");
		});
		this.SettingsCanvas.transform.Find("d2").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.setkey("d2");
		});
		this.useAltMoveControls = true;
		this.SettingsCanvas.transform.Find("one").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.setkey("one");
		});
		this.SettingsCanvas.transform.Find("two").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.setkey("two");
		});
		this.SettingsCanvas.transform.Find("three").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.setkey("three");
		});
		this.SettingsCanvas.transform.Find("four").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.setkey("four");
		});
		this.SettingsCanvas.transform.Find("drop").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.setkey("drop");
		});
		this.SettingsCanvas.transform.Find("interact").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.setkey("interact");
		});
		this.SettingsCanvas.transform.Find("sprint").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.setkey("sprint");
		});
		this.SettingsCanvas.transform.Find("Crouch").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.setkey("Crouch");
		});
		this.SettingsCanvas.transform.Find("leftmouse").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.setkey("leftmouse");
		});
		this.SettingsCanvas.transform.Find("jump").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.setkey("jump");
		});
		this.SettingsCanvas.transform.Find("map").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.setkey("map");
		});
		this.SettingsCanvas.transform.Find("recall").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.setkey("recall");
		});
		this.SettingsCanvas.transform.Find("Slider").GetComponent<Slider>().onValueChanged.AddListener(delegate(float val)
		{
			SettingsHolder.Instance.setsense(val);
		});
		this.SettingsCanvas.transform.Find("volume").GetComponent<Slider>().onValueChanged.AddListener(delegate(float val)
		{
			SettingsHolder.Instance.setvolume(val);
		});
		this.SettingsCanvas.transform.Find("ptt").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.setkey("ptt");
		});
		Transform pttButton = this.SettingsCanvas.transform.Find("enableptt");
		pttButton.GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.togglePtt();
		});
		pttButton.GetComponent<Button>().onClick.AddListener(delegate
		{
			pttButton.GetComponent<showcheckmark>().ToggleCheck(this.usePtt);
		});
		this.SettingsCanvas.transform.Find("reset").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.RestoreDefaults();
		});
		this.SettingsCanvas.transform.Find("item1").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.setkey("item1");
		});
		this.SettingsCanvas.transform.Find("item2").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.setkey("item2");
		});
		this.SettingsCanvas.transform.Find("item3").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.setkey("item3");
		});
		this.SettingsCanvas.transform.Find("item4").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.setkey("item4");
		});
		this.SettingsCanvas.transform.Find("item5").GetComponent<Button>().onClick.AddListener(delegate
		{
			SettingsHolder.Instance.setkey("item5");
		});
		yield break;
	}

	// Token: 0x06000F63 RID: 3939 RVA: 0x0003F333 File Offset: 0x0003D533
	public void ChangeScreenMode(FullScreenMode screenmode)
	{
		Screen.fullScreenMode = screenmode;
		Debug.Log("top g");
	}

	// Token: 0x040008C8 RID: 2248
	public KeyCode leftmouse;

	// Token: 0x040008C9 RID: 2249
	public KeyCode w;

	// Token: 0x040008CA RID: 2250
	public KeyCode a;

	// Token: 0x040008CB RID: 2251
	public KeyCode s;

	// Token: 0x040008CC RID: 2252
	public KeyCode d;

	// Token: 0x040008CD RID: 2253
	public KeyCode w2;

	// Token: 0x040008CE RID: 2254
	public KeyCode a2;

	// Token: 0x040008CF RID: 2255
	public KeyCode s2;

	// Token: 0x040008D0 RID: 2256
	public KeyCode d2;

	// Token: 0x040008D1 RID: 2257
	public KeyCode one;

	// Token: 0x040008D2 RID: 2258
	public KeyCode two;

	// Token: 0x040008D3 RID: 2259
	public KeyCode three;

	// Token: 0x040008D4 RID: 2260
	public KeyCode four;

	// Token: 0x040008D5 RID: 2261
	public KeyCode drop;

	// Token: 0x040008D6 RID: 2262
	public KeyCode interact;

	// Token: 0x040008D7 RID: 2263
	public KeyCode sprint;

	// Token: 0x040008D8 RID: 2264
	public KeyCode jump;

	// Token: 0x040008D9 RID: 2265
	public KeyCode Crouch;

	// Token: 0x040008DA RID: 2266
	public KeyCode map;

	// Token: 0x040008DB RID: 2267
	public KeyCode recall;

	// Token: 0x040008DC RID: 2268
	public KeyCode ptt;

	// Token: 0x040008DD RID: 2269
	public KeyCode item1;

	// Token: 0x040008DE RID: 2270
	public KeyCode item2;

	// Token: 0x040008DF RID: 2271
	public KeyCode item3;

	// Token: 0x040008E0 RID: 2272
	public KeyCode item4;

	// Token: 0x040008E1 RID: 2273
	public KeyCode item5;

	// Token: 0x040008E2 RID: 2274
	public float sense = 1f;

	// Token: 0x040008E3 RID: 2275
	public string VoiceModelPath = "LanguageModels/vosk-model-small-en-us-0.15";

	// Token: 0x040008E4 RID: 2276
	private int frameRateCap;

	// Token: 0x040008E5 RID: 2277
	public bool usePtt;

	// Token: 0x040008E6 RID: 2278
	public bool useAltMoveControls;

	// Token: 0x040008E7 RID: 2279
	private GameObject SettingsCanvas;

	// Token: 0x040008E8 RID: 2280
	public AudioMixer mixer;

	// Token: 0x040008E9 RID: 2281
	public AudioMixer mixer2;
}
