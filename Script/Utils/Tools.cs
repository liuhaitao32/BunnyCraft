using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using FairyGUI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO.Compression;
using SevenZip.Sdk;
using FairyGUI.Utils;
using DG.Tweening;
using System.Security.Cryptography;
using System.Text;


public class Tools
{
    private static  DateTime timerEnd;
    private static DateTime timerBegin;
    private static int season_last_;
    private static  int season_protect;
    private static int season_settle;

    public Tools ()
	{
       
	}
	public static string UrlEncode(string str)
	{
		StringBuilder sb = new StringBuilder();
		byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
		for (int i = 0; i < byStr.Length; i++)
		{
			sb.Append(@"%" + Convert.ToString(byStr[i], 16));
		}

		return (sb.ToString());
	}
	//Ui:xxxx
	public static GObject GetComponent (string name)
	{
		string[] names = name.Split (':');
		return UIPackage.CreateObject (names [0], names [1]);
	}

	public static object GetUIPackageObject (string name)
	{
		string[] names = name.Split (':');
		return UIPackage.GetItemAsset (names [0], names [1]);
	}

	public static long GetSystemMillisecond ()
	{
		return ModelManager.inst.gameModel.time.Ticks / 10000;
	}

	public static string GetResourceUrl (string name)
	{
		string[] names = name.Split (':');
		string url = "";
		if(names!=null && names.Length>1){
			url = UIPackage.GetItemURL (names [0], names [1]);
		}
		if (IsNullEmpty (url)) {
			url = UIConfig.loaderErrorSign;
		}
//        Log.debug("Loader Url Change - vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv" + name + "|" + url);
        return url;
	}

	public static Vector2 GetStageScale ()
	{
		return GRoot.inst.scale;
	}
    public static bool IsNewDay(DateTime time)
    {
        string timeClick = string.Format("{0:s}", time).Split('T')[0];
        string now = string.Format("{0:s}", ModelManager.inst.gameModel.time).Split('T')[0];
        if (!timeClick.Equals(now))
        {
            //新的一天
            return true;
        }
        else
        {
            return false;

        }
    }
    public static GameObject FindChild2 (string path, GameObject go = null)
	{
		string[] paths = path.Split ('/');

		for (int i = 0, len = paths.Length; i < len; i++)
		{
			go = go == null ? GameObject.Find (paths [i]) : go.transform.FindChild (paths [i]).gameObject;
		}
		return go;
	}
    public static void UpdataHeadTime()
    {
        if (ModelManager.inst.roleModel.phototTimer.Ticks != 0)
        {
            long log = ModelManager.inst.gameModel.time.Ticks / 10000 - (ModelManager.inst.roleModel.phototTimer.Ticks) / 10000;
            int max = (int)DataManager.inst.systemSimple["time_Interval"];
            int min = Convert.ToInt32(log);
            if (min > max)
            {
                NetHttp.inst.Send(NetBase.HTTP_UPDATE_PHOTO, "", (VoHttp vo) =>
                {
                    ModelManager.inst.userModel.UpdateData(vo.data);
                    ModelManager.inst.roleModel.phototTimer = ModelManager.inst.gameModel.time;
                });
            }
        }
        else
        {
            NetHttp.inst.Send(NetBase.HTTP_UPDATE_PHOTO, "", (VoHttp vo) =>
            {
                ModelManager.inst.userModel.UpdateData(vo.data);
                ModelManager.inst.roleModel.phototTimer = ModelManager.inst.gameModel.time;

            });
        }
    }
	public static T FindChild<T> (GameObject go, string name)
	{
		return go.transform.FindChild (name).GetComponent<T> ();
	}

	public static GameObject FindChild (GameObject go, string path)
	{
		return go.transform.Find (path).gameObject;
	}

	public static void RemoveGraphObject (GGraph graph)
	{
		if (graph.displayObject != null)
			graph.displayObject.Dispose ();
	}

	public static int GetRandom (int min = 0, int max = 999999)
	{
		return UnityEngine.Random.Range (min, max);
	}

	public static string GetDateTimeString ()
	{
		return ModelManager.inst.gameModel.time.ToString ("HH-mm-ss");
	}
    public static int GetCountry(string country, Dictionary<string, object> countryData)
    {
        int country_tag = 0;
        if (countryData != null)
        {
            List<object> list = Tools.ConvertDicToList(countryData, "name");
            for (int i = 0; i < list.Count; i++)
            {
                Dictionary<string, object> dd = (Dictionary<string, object>)list[i];
                string str = dd["name"].ToString();
                object[] obj = (object[])dd[str];
                string c = Tools.GetMessageById((string)obj[1]);
                if (country.Equals(c))
                {
                    country_tag = i;
                }
            }
        }
        return country_tag;

    }
    //loaderbutton set url  =  resource or weburl
	public static void SetLoaderButtonUrl (GButton loader, string url, GLoader myLoader = null,UnityEngine.UI.Image image = null, Action<object> onComplete = null)
	{
        if(url == "") return;
		string h01 = "Icon:h01";
		GLoader ld = null;
		if (image == null) {
			ld = (myLoader == null) ? loader.GetChild ("n0").asLoader : myLoader;
			ld.url = "";
		} else {
			ld = new GLoader ();
		}
//		ld.url = Tools.GetResourceUrl ("");
//		return;
        //		ld.name = url;
        if (url.IndexOf("http") > -1)
        {
            string bitmap = LocalStore.GetLocal(LocalStore.HEADIMG + url);
            if (!Tools.IsNullEmpty(bitmap) && bitmap.Length > 0)
            {
//                Log.debug("----------temp--------" + url);
				if (image == null&&!ld.image.isDisposed) {
					ld.texture = new NTexture (PhoneManager.inst.Base64ToTexter2d (bitmap, 100, 100));
				} else {
                    //TimerManager.inst.Add(5f, 1, (float ff) => {
                        image.sprite = GetSprite(PhoneManager.inst.Base64ToTexter2d(bitmap, 100, 100));
                    //});
                    if(null != onComplete) {
                        onComplete(image.sprite);
                    }
					ld.Dispose ();
					ld.RemoveFromParent();
				}
            }
            else
            {
                //				if (ld.myExternalLoadSuccessName != url) {
                //					ld.myExternalLoadSuccessName = url;
                //				}
                //				Debug.LogError ("SetLoaderButtonUrl http start "+url);
                //				ld.loadComplete = (NTexture texture, string urlid) =>
                //				{
                //					Debug.LogError ("SetLoaderButtonUrl -- onExternalLoadSuccess--" + urlid);
                //					if (urlid.IndexOf ("http") > -1)
                //					{
                //						LocalStore.SetLocal (LocalStore.HEADIMG + urlid, Convert.ToBase64String ((texture.nativeTexture as Texture2D).EncodeToPNG ()));
                //					}
                //				};
                //
                LoaderManager.inst.Load(url, (object w) =>
                {
                    Dictionary<string, object> ww = (Dictionary<string, object>)w;
                    byte[] topImage = (byte[])ww["texture"];

                    string urlid = (string)ww["url"];
                    GLoader selfLoader = (GLoader)ww["loader"];
                    if (w != null && w != null)
                    {
                        string base64 = Convert.ToBase64String(topImage);
                        if (urlid.IndexOf("http") > -1)
                        {
							LocalStore.SetLocal(LocalStore.HEADIMG + urlid, base64);
                        }

						if (selfLoader != null)// && selfLoader.parent!=null
                        {
							if(!selfLoader.image.isDisposed){
                            	selfLoader.texture = new NTexture(PhoneManager.inst.Base64ToTexter2d(base64, 100, 100));
							}
							else{
//									ld.texture = new NTexture(PhoneManager.inst.Base64ToTexter2d(base64, 100, 100));
							}
                        }
						if (image == null) {
	
						}
						else{
							image.sprite = GetSprite (PhoneManager.inst.Base64ToTexter2d (base64, 100, 100));
							selfLoader.Dispose ();
							selfLoader.RemoveFromParent();
						}

                    }
                }, (object xx) =>
                {
					if (image == null) {
						ld.url = Tools.GetResourceUrl(h01);
					}
					else{
						image.sprite = GetSprite(h01);
                        if(null != onComplete) {
                            onComplete(image.sprite);
                        }
						ld.Dispose ();
						ld.RemoveFromParent();
                    }
                }, ld);
                //				ld.url = url;
            }
        }
        else
        {
			if (image == null) {
				ld.url = url;
			} else {
				
				if (url == "") {
					image.sprite = GetSprite (h01);
				} else {
					image.sprite = GetSprite (url);
				}
                if(null != onComplete) {
                    onComplete(image.sprite);
                }
				ld.Dispose ();
				ld.RemoveFromParent();
            }
        }
    }

    //检测头像是否是合格的照片
    public static bool checkHead()
    {
        bool isPass = true;
        ModelUser userModel=ModelManager.inst.userModel;
        Dictionary<string,object> head= userModel.head;
        string useHead=head["use"].ToString();
        foreach (KeyValuePair<string, object> str in userModel.head)
        {
            if (!str.Key.Equals("use"))
            {
                if (useHead.Equals(str.Key))
                {
                    Dictionary<string, object> dc1 = (Dictionary<string, object>)str.Value;
                    if ((int)dc1["status"] == -1)
                    {
                        isPass = false;
                    }
                }
            }
        }
        return isPass;
    }
    public static string[] miro(int timer)
    {
        string[] timeArr = new string[4];
        timeArr[0] = TextForm((timer / 3600000).ToString());
        timeArr[1] = TextForm(((timer / 60000) % 60).ToString());
        timeArr[2] = TextForm(((timer / 1000) % 60).ToString());
        timeArr[3] = TextForm((timer % 1000).ToString());
        return timeArr;
    }
    public static string TextForm(string text)
    {
        string t = "";
        if (text.Length == 1)
        {
            t = "0" + text;
        }
        else if (text.Length > 2)
        {
            t = text.Substring(0, 2);
        }
        else
        {
            t = text;
        }
        return t;
    }

    //获得resource资源
    public static T GetAsset<T> (string name)where T:UnityEngine.Object
	{
		return LoaderManager.inst.Load<T> (name);
	}

	//初始化后的
	public static GameObject GetPrefab (string name)
	{
		GameObject go = GetAsset<GameObject> (name);
		if (go != null)
			return GameObject.Instantiate (go);
		return null;
	}

	public static void Clear (object go)
	{
		if (go == null)
			return;
		if (go is GameObject)
			GameObject.Destroy (go as GameObject);
		else if (go is Loads)
			(go as Loads).Clear ();
		else if (go is Tweener)
			(go as Tweener).Kill ();
		else
		{
			Log.debug ("No This Object Clear Function");
		}
	}

	public static object Clone (object data)
	{		
		BinaryFormatter Formatter = new BinaryFormatter (null, new System.Runtime.Serialization.StreamingContext (System.Runtime.Serialization.StreamingContextStates.Clone));
		MemoryStream stream = new MemoryStream ();
		Formatter.Serialize (stream, data);
		stream.Position = 0;
		object clone = Formatter.Deserialize (stream);
		stream.Close ();
		return clone;
	}

	public static Sprite GetSprite (Texture t2)
	{		
		return Sprite.Create ((Texture2D)t2, new Rect (0, 0, t2.width, t2.height), new Vector2 (0.5f, 0.5f));
	}

	public static Sprite GetSprite (string name)
	{		
		NTexture nt = GetUIPackageObject (name) as NTexture;
		Rect r = new Rect ();
		r.x = nt.uvRect.x * nt.nativeTexture.width;
		r.y = nt.uvRect.y * nt.nativeTexture.height;
		r.width = nt.width;//nt.uvRect.width * nt.width;
		r.height = nt.height;//nt.uvRect.height * nt.height;
//		Debug.LogError(r.width + " | " +nt.nativeTexture.width);
		return Sprite.Create ((Texture2D)nt.nativeTexture, r, new Vector2 (0.5f, 0.5f));

//		return Sprite.Create((Texture2D)nt.nativeTexture,new Rect(0,0,nt.width,nt.height),new Vector2(0f,0f));

	}

	public static bool IsNullEmpty (object value)
	{
		string msg = (string)value;
		if (msg == "" || msg == null)
			return true;		
		return false;
	}

	public static Texture2D ScaleTexture (Texture2D source, int targetWidth, int targetHeight)
	{
		Texture2D result = new Texture2D (targetWidth, targetHeight, source.format, false);

//		float incX = (1.0f / (float)targetWidth);
//		float incY = (1.0f / (float)targetHeight);

		for (int i = 0; i < result.height; ++i)
		{
			for (int j = 0; j < result.width; ++j)
			{
				Color newColor = source.GetPixelBilinear ((float)j / (float)result.width, (float)i / (float)result.height);
				result.SetPixel (j, i, newColor);
			}
		}
		result.Apply ();
		return result;
	}

	public static ColorFilter AddColorFilter (GObject item)
	{
		ColorFilter cf;
		if (item.filter != null)
			cf = item.filter as ColorFilter;
		else
		{
			cf = new ColorFilter ();
			item.filter = cf;
		}
		return cf;
	}

	public static void RemoveColorFilter (GObject item)
	{
		item.filter = null;
	}

	public static void SaveImage (string name, Texture2D t2)
	{
		byte[] bytes = t2.EncodeToJPG ();  
		string filename = Application.dataPath + "/" + name + ".jpg";
		System.IO.File.WriteAllBytes (filename, bytes);
//		Log.debug (filename);
	}

	public static Color GetColor (string color)
	{
		if (color.StartsWith ("#"))
			color = color.Replace ("#", "");
		int v = int.Parse (color, System.Globalization.NumberStyles.HexNumber);
		float r = Convert.ToByte ((v >> 16) & 255);
		float g = Convert.ToByte ((v >> 8) & 255);
		float b = Convert.ToByte ((v >> 0) & 255);
		return new Color (r / 255, g / 255, b / 255, 1);
	}

	public static int GetColorToRgb (Color color)
	{
		int value = (int)color.r << 16;
		value += (int)color.g << 8;
		value += (int)color.b;
		return value;
	}
	public static string GetMessageById (string id, object[] value = null)
	{
//		if (DataManager.inst.returnMsg == null)
//			return null;		
		Dictionary<string,object> data = DataManager.inst.returnMsg;
		if (data == null) {
			data = new Dictionary<string, object> ();
		}
		string msg = null;
		if (data.ContainsKey (id)) {
			msg = (string)DataManager.inst.returnMsg [id];
		}
		if (msg == null) {
			if (!DataManager.inst.ConfigWasGet) {
				if (Config.OffLineTxt.ContainsKey (PlatForm.inst.language + "_"+id)) {
					msg = Config.OffLineTxt [PlatForm.inst.language + "_" + id];
				} else {
					return null;
				}
			} else {
				return null;
			}

		}
		if (value != null && value.Length > 0)
			return string.Format (msg, value);
		else
			return msg;	
	}

    public static void GetResourceUrlForMVP(GLoader mvp,string type)
    {
        switch (type)
        {
            case "mvp":
                //if (PlatForm.inst.language.Equals("cn"))
                //{
                //    mvp.url = GetResourceUrl("Image2:n_icon_sj5_");
                //}
                //else
                //{
                //    mvp.url = GetResourceUrl("Image2:n_icon_sj5");
                //}
                mvp.url = GetResourceUrl("Image2:n_icon_paiming0");
                break;
            case "win":
                if (PlatForm.inst.language.Equals("cn"))
                {
                    mvp.url = GetResourceUrl("Image2:n_icon_win_");
                }
                else
                {
                    mvp.url = GetResourceUrl("Image2:n_icon_win");
                }
                break;
            case "lose":
                if (PlatForm.inst.language.Equals("cn"))
                {
                    mvp.url = GetResourceUrl("Image2:n_icon_lose_");
                }
                else
                {
                    mvp.url = GetResourceUrl("Image2:n_icon_lose");
                }
                break;

        }

        

        
    }

    //aaaaaa[0]bbbbb[/0]xxxxx{0}yyyyy{1}zzzzz
    public static string GetMessageColor (string value, string[] color)
	{
		int len = color.Length;
		string msg = value;
		for (int i = 0; i < len; i++)
		{
			msg = msg.Replace ("[" + i + "]", "[color=#" + color [i] + "]");
			msg = msg.Replace ("[/" + i + "]", "[/color]");
		}			
		return msg;
	}

	//获得当前时间秒数
	public static long GetSystemSecond ()
	{		
		return ModelManager.inst.gameModel.time.Ticks / 10000000;
	}

	public static long GetSystemTicks ()
	{		
		return ModelManager.inst.gameModel.time.Ticks;
	}

	//type=0  hh:mm:ss type=1 dd天hh小时mm分ss秒 type=2 dd天hh小时 type=3 dd天 type=4 dd小时   type=5 dd分 type=6 dd秒  type=7 dd天hh小时mm分
	public static string TimeFormat (long tick, int type = 0, bool ifSecond = true)
	{
		if (tick < 0)
			return "";
		DateTime dt = new DateTime (tick);
		if (type == 0)
		{
			return dt.ToString ("HH:mm:ss");
		}
		else if (type == 2)
		{
			double t = Convert.ToDouble (tick);
			t = t / 10000 / 1000;
			double day = Math.Floor (t / 86400);
			double hours = Math.Floor (t / 3600 % 24);
			string msg = "";
			if (day > 0)
			{
				msg += day + Tools.GetMessageById ("14008");
			}
			if (hours != 0 || day != 0)
			{
				msg += hours.ToString ();
				msg += Tools.GetMessageById ("14009");
			}
			return msg;
		}
		else if (type == 3)
		{
			double t = Convert.ToDouble (tick);
			t = t / 10000 / 1000;
			double day = Math.Floor (t / 86400);
			string msg = "";
			if (day > 0)
			{
				msg += day + Tools.GetMessageById ("14008");
			}
			return msg;
		}
		else if (type == 4)
		{
			double t = Convert.ToDouble (tick);
			t = t / 10000 / 1000;
			double hours = Math.Floor (t / 3600 % 24);
			string msg = "";
			if (hours != 0)
			{
				msg += hours.ToString ();
				msg += Tools.GetMessageById ("14009");
			}
			return msg;
		}
		else if (type == 5)
		{
			double t = Convert.ToDouble (tick);
			t = t / 10000 / 1000;
			double minutes = Math.Floor (t / 60 % 60);

			string msg = "";
			if (minutes != 0)
			{
//				msg += minutes < 10 ? minutes.ToString () : minutes.ToString ();
				msg += minutes.ToString ();
				msg += Tools.GetMessageById ("14010");
			}
			return msg;
		}
		else if (type == 6)
		{
			double t = Convert.ToDouble (tick);
			t = t / 10000 / 1000;
			double second = Math.Floor (t % 60);

			string msg = "";
			if (second != 0)
			{
				if (ifSecond)
				{
//					msg += second < 10 ? second.ToString () : second.ToString ();
					msg += second.ToString ();
					msg += Tools.GetMessageById ("14011");
				}
			}
			return msg;
		}else if (type == 7)
		{
			double t = Convert.ToDouble (tick);
			t = t / 10000 / 1000;
			double day = Math.Floor (t / 86400);
			double hours = Math.Floor (t / 3600 % 24);
			double minutes = Math.Floor (t / 60 % 60);
			double second = Math.Floor (t % 60);

			string msg = "";
			if (day > 0)
			{
				msg += day + Tools.GetMessageById ("14008");
			}
			if (hours != 0 || day != 0)
			{
				msg += hours.ToString ();
				msg += Tools.GetMessageById ("14009");
			}
			if (minutes != 0 || day != 0 || hours != 0)
			{
				//				msg += minutes < 10 ? "0" + minutes.ToString () : minutes.ToString ();
				msg += minutes.ToString ();
				msg += Tools.GetMessageById ("14010");
			}
			return msg;
		}
        else if (type == 8)
        {
            double t = Convert.ToDouble(tick);
            t = t / 10000 / 1000;


            double second = Math.Floor(t % 60);
            double second1= Math.Floor(t % 60 % 60);
            double second2= Math.Floor(t % 60 % 60 % 60);

            string msg = "";
            if (second != 0 || second1 != 0 || second2 != 0)
            {
                //				msg += minutes < 10 ? "0" + minutes.ToString () : minutes.ToString ();
                msg += second + "："+ second1+"：" +"："+ second2;
            }

            return msg;
        }
        else
		{					
			double t = Convert.ToDouble (tick);	
			t = t / 10000 / 1000;
			double day = Math.Floor (t / 86400);
			double hours = Math.Floor (t / 3600 % 24);
			double minutes = Math.Floor (t / 60 % 60);
			double second = Math.Floor (t % 60);

			string msg = "";
			if (day > 0)
			{
				msg += day + Tools.GetMessageById ("14008");
			}
			if (hours != 0 || day != 0)
			{
				msg += hours.ToString ();
				msg += Tools.GetMessageById ("14009");
			}
			if (minutes != 0 || day != 0 || hours != 0)
			{
//				msg += minutes < 10 ? "0" + minutes.ToString () : minutes.ToString ();
				msg += minutes.ToString ();
				msg += Tools.GetMessageById ("14010");
			}
			if (second != 0 || day != 0 || hours != 0 || minutes != 0)
			{
				if (ifSecond)
				{
//					msg += second < 10 ? "0" + second.ToString () : second.ToString ();
					msg += second.ToString ();
					msg += Tools.GetMessageById ("14011");
				}
			}
			return msg;
		}
	}
	//设置名次的样式

	public static void StartSetValue (GComponent gc, string rank, string myRank = "", string myUid = "-")
	{
		GLoader icon = gc.GetChild ("n1").asLoader;
		GTextField text = gc.GetChild ("n4").asTextField;
		
		switch (rank)
		{
		case "1":
			icon.url = Tools.GetResourceUrl ("Image2:n_icon_paiming1");
                text.text = "";
			//Tools.SetTextFieldStrokeAndShadow (text, "#9b5c04", new Vector2 (0, 0));
			break;
		case "2":
			icon.url = Tools.GetResourceUrl ("Image2:n_icon_paiming2");
                text.text = "";
                //Tools.SetTextFieldStrokeAndShadow (text, "#4b4b4b", new Vector2 (0, 0));
                break;
		case "3":
			icon.url = Tools.GetResourceUrl ("Image2:n_icon_paiming3");
                text.text = "";
                //Tools.SetTextFieldStrokeAndShadow (text, "#853c1d", new Vector2 (0, 0));
                break;
		default:
                //if (rank.Equals (Tools.GetMessageById ("24223")))
                //{
                //             //Tools.GetResourceUrlForMVP(icon);
                //                 //icon.url = Tools.GetResourceUrl ("Image2:n_icon_paiming0");
                //	//Tools.SetTextFieldStrokeAndShadow (text, "#46149f", new Vector2 (0, 0));
                //}
                //else
                //{

                //}
                if (myUid.Equals(ModelManager.inst.userModel.uid))
                {
                    text.text = myRank;
                }
                else
                {
                    text.text = rank;
                }
                icon.url = Tools.GetResourceUrl("Image2:n_icon_paiming4");
//                Tools.SetTextFieldStrokeAndShadow(text, "#426600", new Vector2(0, 0));

                break;
		}
	}
	public static string StartValueTxt(int rank){
		return rank > 3 ? rank + "" : "";
	}
	public static string TimeFormat2 (long tick, int type = 0)
	{
		double t = Convert.ToDouble (tick);	
		t = t / 10000 / 1000;
		double hours = Math.Floor (t / 3600);
		double minutes = Math.Floor (t / 60 % 60);
		double second = Math.Floor (t % 60);

		string msg = "";
		if (type == 0)
		{
			if (hours != 0)
			{
				msg += hours.ToString ();
				msg += Tools.GetMessageById ("14009");

//				msg += minutes < 10 ? "0" + minutes.ToString () : minutes.ToString ();
				msg += minutes.ToString ();
				msg += Tools.GetMessageById ("14010");
			}
			else if (minutes != 0)
			{
//				msg += minutes < 10 ? "0" + minutes.ToString () : minutes.ToString ();
				msg += minutes.ToString ();
				msg += Tools.GetMessageById ("14010");

//				msg += second < 10 ? "0" + second.ToString () : second.ToString ();
				msg += second.ToString ();
				msg += Tools.GetMessageById ("14011");
			}
			else
			{
//				msg += second < 10 ? "0" + second.ToString () : second.ToString ();
				msg += second.ToString ();
				msg += Tools.GetMessageById ("14011");
			}
		}
		else if (type == 1)
		{
			if (hours != 0)
			{
				msg += hours.ToString ();
				msg += Tools.GetMessageById ("14009");
			}
			if (minutes != 0 && hours == 0)
			{
				msg += minutes.ToString ();
//				msg += minutes < 10 ? "0" + minutes.ToString () : minutes.ToString ();
				msg += Tools.GetMessageById ("14010");
			}
			if (second != 0 && minutes == 0 && hours == 0)
			{
				msg += second.ToString ();
//				msg += second < 10 ? "0" + second.ToString () : second.ToString ();
				msg += Tools.GetMessageById ("14011");
			}
		}
		else if (type == 2)
		{
			if (hours != 0)
			{
				msg += hours.ToString ();
				msg += ":";

				msg += minutes.ToString ();
//				msg += minutes < 10 ? "0" + minutes.ToString () : minutes.ToString ();
			}
			else if (minutes != 0)
			{
				msg += minutes.ToString ();
//				msg += minutes < 10 ? "0" + minutes.ToString () : minutes.ToString ();
				msg += ":";

				msg += second.ToString ();
//				msg += second < 10 ? "0" + second.ToString () : second.ToString ();
			}
			else
			{
//				msg += second < 10 ? "0" + second.ToString () : second.ToString ();
				msg += second.ToString ();
			}
			if (msg == "0")
			{
				msg = "";
			}
		}
		return msg;
	}

	public static List<object> ConvertDicToList (Dictionary<string,object> value, string keyName = "dataid")
	{
		List<object> list = new List<object> ();
		Dictionary<string,object> d;
		foreach (string n in value.Keys)
		{
			if (value [n] is IDictionary)
			{
				d = (Dictionary<string,object>)value [n];
				d [keyName] = n; 
				list.Add (d);
			}
			else
			{
				d = new Dictionary<string, object> ();
				d [keyName] = n;
				d [n] = value [n];
				list.Add (d);
			}
		}
		return list;
	}

	public static void Sort (object[] list, string[] names)
	{
		List<object> data = new List<object> (list);
		Tools.Sort (data, names);
	}

	//names =[age:int:0,name:string:0,time:datetime:1] type=0升 1降
	public static void Sort (List<object> list, string[] names)
	{
		list.Sort ((object a, object b) =>
		{
			if (a == null && b == null)
				return 0;
			if (a != null && b == null)
				return -1;
			if (a == null && b != null)
				return 1;
			Dictionary<string,object> aa = (Dictionary<string,object>)a;
			Dictionary<string,object> bb = (Dictionary<string,object>)b;
			string[] msg;
			string type;
			string k;
			string t;
			long xi;
			long yi;
			string xs;
			string ys;
			int result = 0;
			for (int i = 0; i < names.Length; i++)
			{
				msg = names [i].Split (':');
				k = msg [0];
				t = msg [1];
				type = msg [2];
				if (t == "int")
				{
					xi = Convert.ToInt32 (aa [k]);
					yi = Convert.ToInt32 (bb [k]);
					result = xi.CompareTo (yi);
				}
				else if (t == "string")
				{
					xs = aa [k].ToString ();
					ys = bb [k].ToString ();
					result = xs.CompareTo (ys);
				}
				else if (t == "datetime")
				{
					xi = ((DateTime)aa [k]).Ticks;
					yi = ((DateTime)bb [k]).Ticks;
					result = xi.CompareTo (yi);
				}
				else
				{
					throw new UnityException ("Call Sort Function Args Wrong");
				}
				if (result == 0)
					continue;
				else
				{
					if (type == "0" || result == 0)
						return result;
					else
						return result == 1 ? -1 : 1;
				}
			}
			return result;
		});
	}

	public static string GetIconMcName (string type)
	{
		string msg = "";
		switch (type)
		{
		case Config.ASSET_COIN:
			msg = "Flash:Mc_Gold";
			break;
		case Config.ASSET_GOLD:
			msg = "Flash:Mc_Coin";
			break;
		case Config.ASSET_EXP:
			msg = "Flash:Mc_Exp";
			break;
		case Config.ASSET_REDBAGCOIN:
			msg = "Flash:Mc_RedFly";
			break;
		case Config.ASSET_CARD:
			msg = "Flash:Mc_Card";
			break;
		case Config.ASSET_ELSCORE:
			msg = "Flash:Mc_ElScore";
			break;
		}
		return msg;
	}

	public static string GetIconUrl (string type)
	{		
		string msg = "";
		switch (type)
		{
		case Config.ASSET_COIN:
			msg = Config.ASSET_URL_COIN;
			break;
		case Config.ASSET_GOLD:
			msg = Config.ASSET_URL_GOLD;
			break;
		case Config.ASSET_EXP:
			msg = Config.ASSET_URL_EXP;
			break;
		case Config.ASSET_RANKSCORE:
			msg = Config.ASSET_URL_RANKSCORE;
			break;
		case Config.ASSET_ELSCORE:
			msg = Config.ASSET_URL_ELSCORE;
			break;
		case Config.ASSET_REDBAGCOIN:
			msg = Config.ASSET_URL_REDBAGCOIN;
			break;
		case Config.ASSET_CARD:
			msg = Config.ASSET_URL_CARD;
			break;
		default:
			msg = type;
			break;
//		case Config.ASSET_ACE:
//			return Config.ASSET_URL_ACE;
//		case Config.ASSET_SCORE:
//			return Config.ASSET_URL_SCORE;
		}	
		if (msg == type)
			return msg;
		else
			return Tools.GetResourceUrl (msg);
	}

	public static string GetBodyName (string id)
	{
		return Tools.GetMessageById (((Dictionary<string,object>)DataManager.inst.body [id]) ["name"].ToString ());
	}

	public static string GetIconName (string type)
	{
		string msg = "";
		switch (type)
		{
		case Config.ASSET_COIN:
			msg = Tools.GetMessageById ("14014");
			break;
		case Config.ASSET_GOLD:
			msg = Tools.GetMessageById ("14013");
			break;
		case Config.ASSET_EXP:
			msg = Tools.GetMessageById ("14018");
			break;
		case Config.ASSET_RANKSCORE:
			msg = Tools.GetMessageById ("14019");
			break;
		case Config.ASSET_ELSCORE:
			msg = Tools.GetMessageById ("14020");
			break;
		case Config.ASSET_REDBAGCOIN:
			msg = Tools.GetMessageById ("14021");
			break;
		case Config.ASSET_AWARD:
			msg = Tools.GetMessageById ("14022");
			break;
		case Config.ASSET_CARD:
			msg = Tools.GetMessageById ("14030");
			break;
		}
		return msg;
	}

	public static string GetEffortName (int effort_lv)
	{
		string msg = "";
		switch (effort_lv)
		{
		case 1:
			msg = Tools.GetMessageById ("23050");
			break;
		case 2:
			msg = Tools.GetMessageById ("23051");
			break;
		case 3:
			msg = Tools.GetMessageById ("23052");
			break;
		case 4:
			msg = Tools.GetMessageById ("23053");
			break;
		case 5:
			msg = Tools.GetMessageById ("23054");
			break;
		case 6:
			msg = Tools.GetMessageById ("23055");
			break;
		case 7:
			msg = Tools.GetMessageById ("23056");
			break;
		case 8:
			msg = Tools.GetMessageById ("23057");
			break;
		}
		return msg;
	}

	public static string GetExploreBoxID (string id)
	{
		string msg = "";
		switch (id)
		{
		case "box001":
			msg = Config.EFFECT_BOX001;
			break;
		case "box002":
			msg = Config.EFFECT_BOX002;
			break;
		case "box003":
			msg = Config.EFFECT_BOX003;
			break;
		case "box004":
			msg = Config.EFFECT_BOX004;
			break;
		case "box005":
			msg = Config.EFFECT_BOX005;
			break;
		case "box006":
			msg = Config.EFFECT_BOX006;
			break;
		case "box007":
			msg = Config.EFFECT_BOX007;
			break;
		case "box100":
			msg = Config.EFFECT_BOX100;
			break;
		case "box101":
			msg = Config.EFFECT_BOX101;
			break;
		case "rpbox001":
			msg = Config.EFFECT_RPBOX001;
			break;
		case "rpbox002":
			msg = Config.EFFECT_RPBOX002;
			break;
//		case "rpbox003":
//			msg = Config.EFFECT_BOX008;
//			break;
		}
		return msg;
	}

	public static string GetEggName (string id)
	{
		return "Egg" + id + "/egg" + id;
	}

	public static string GetEffortBuildID (string id)
	{
		return "Build" + id + "/build" + id;
	}

	public static string GetFightRewardID (string id)
	{
		return "Bag" + id + "/bag" + id;
	}

	public static string GetSexUrl (object sex)
	{
		if (sex == null)
			return Tools.GetResourceUrl ("Image:icon_woman");	
		if (sex.ToString () == "f")
			return Tools.GetResourceUrl ("Image:icon_woman");
		return Tools.GetResourceUrl ("Image:icon_man"); 

	}

	public static int[] GetStringLength (string value,int length)
	{
        int num = 0;
        float count = 0;
        int[] countArr = new int[2];
        for (int i = 0; i < value.Length; i++)
		{
            num += 1;
			if ((int)value [i] > 127)
            {
                count += 1f;
            }
            else
            {
                count += 0.5f;

            }

            if (count >= length)
                break;

        }

        countArr[0] = Convert.ToInt32(Math.Floor(count));
        countArr[1] = num;

        return countArr;
	}

	public static string GetStringByLength (string value, int length)
	{
		string msg = value;
		int[] len = GetStringLength (value, length);
        if((int)len[0]>= length)
        {
            msg = msg.Substring(0, (int)len[1]);
        }
        
		return msg;
	}

	public static List<object> GetReward (object data)
	{
		List<object> ld = new List<object> ();
		Dictionary<string,object> da = (Dictionary<string,object>)data;
		Dictionary<string,object> d;
		Dictionary<string,object> b;
		string paixu = "paixu";
		Dictionary<string,object> pxCfg = (Dictionary<string,object>)DataManager.inst.systemSimple ["sort_gift"];
		Dictionary<string,object> cardCfg = DataManager.inst.card;
		foreach (string n in da.Keys)
		{
			if (n == Config.ASSET_CARD)
			{
				d = (Dictionary<string,object>)da [n];
				foreach (string m in d.Keys)
				{
					b = new Dictionary<string, object> ();
					b [m] = d [m].ToString ();
					int rarity = (int)((Dictionary<string,object>)cardCfg [m]) ["rarity"];
					b [paixu] = (int)((object[])pxCfg [n]) [rarity] * (int)d [m] + pxCfg.Count;
					ld.Add (b);
				}
			}
			else if (n == Config.ASSET_EXP || n == Config.ASSET_COIN || n == Config.ASSET_GOLD || n == Config.ASSET_ELSCORE || n == Config.ASSET_RANKSCORE || n == Config.ASSET_REDBAGCOIN)
			{
				d = new Dictionary<string, object> ();
				d [n] = da [n].ToString ();
				d [paixu] = pxCfg [n];
				ld.Add (d);
			}
			else if (n == Config.ASSET_BODY)
			{
				object[] o = (object[])da [n];
				for (int i = 0; i < o.Length; i++)
				{
					d = new Dictionary<string, object> ();
					d.Add (o [i].ToString (), 1);
					d [paixu] = -1;
					ld.Add (d);
				}
			}
			else if (n == Config.ASSET_AWARD)
			{
				d = (Dictionary<string,object>)da [n];
				object[] o;
				foreach (string m in d.Keys)
				{
					o = (object[])d [m];
					b = new Dictionary<string, object> ();
					b [n] = new object[]{ o [0], o [1], m };
					b [paixu] = -2;
					ld.Add (b);
				}
			}
		}
		Tools.Sort (ld, new string[]{ "paixu:int:0" });
		foreach (object str in ld)
		{
			Dictionary<string,object> dat = (Dictionary<string,object>)str;
			dat.Remove ("paixu");
		}
		return ld;
	}

	////////////////////////////////////////////////////////////////
	//计算一上下限的两个数字
	public static float[] NumSection (object[] arr, int lv)
	{
		float min = Convert.ToSingle (arr [0]) + (lv - 1f) * Convert.ToSingle (arr [2]) / 2f;
		float max = Convert.ToSingle (arr [1]) + (lv - 1f) * Convert.ToSingle (arr [3]);
		return new float[]{ min, max };
	}

	public static float NumSectionOne (object[] arr, int lv, bool isMax = true)
	{
		if (arr.GetLength (0) < 2)
			return (int)arr [0];
		float max = 0;
		if (isMax)
		{
			max = Convert.ToSingle (arr [0]) + (lv - 1f) * Convert.ToSingle (arr [1]);
		}
		else
		{
			max = Convert.ToSingle (arr [0]) + (lv - 1f) * Convert.ToSingle (arr [1]) / 2f;
		}
		return max;
	}

	//dic arr - skill[0].step[100].name
	public static object Analysis (object source, string value)
	{
		Regex r = new Regex ("^(\\w+)\\[(\\d+)\\]$");
		Match m;
		string[] msg = value.Split ('.');
		object[] o;
		Dictionary<string,object> d;
		object temp = source;
		for (int i = 0; i < msg.Length; i++)
		{
			if (r.IsMatch (msg [i]))
			{
				m = r.Match (msg [i]);
				d = (Dictionary<string,object>)temp;
				temp = d [m.Groups [1].ToString ()];

				o = (object[])temp;	
				temp = o [Convert.ToInt32 (m.Groups [2].ToString ())];
			}
			else
			{
				d = (Dictionary<string,object>)temp;
				temp = d [msg [i].ToString ()];
			}
		}
		return temp;
	}

	//秒 返回消费钻石
	public static int GetCoinByTime (long sceonds)
	{
		float time_coin = (int)DataManager.inst.systemSimple ["time_coin"];
		double s = Math.Ceiling (sceonds / time_coin);
		return Convert.ToInt32 (s);
	}

	public static List<object> GetSubListPage (List<object> list, int skip, int pageSize)
	{
		if (list == null)
		{
			return null;
		}
		int startIndex = skip * pageSize;
		int endIndex = startIndex + pageSize;

		if (startIndex > list.Count)
			return null;
		if (endIndex > list.Count)
		{
			endIndex = list.Count;
			return list.GetRange (startIndex, endIndex - startIndex);
		}
		else
		{
			return list.GetRange (startIndex, pageSize);
		}
	}

	public static String GetByteSize (double size)
	{
		String[] units = new String[] { "B", "KB", "MB", "GB", "TB", "PB" };
		double mod = 1024.0;
		int i = 0;
		while (size >= mod)
		{
			size /= mod;
			i++;
		}
		return Math.Round (size) + units [i];
	}

	//	public static byte[] CompressNSpeex (byte[] inputBytes)
	//	{
	//		SpeexEncoder encoder = new SpeexEncoder (BandMode.Wide);
	//		// convert to short
	//		short[] data = new short[inputBytes.Length / 2];
	//		Buffer.BlockCopy (inputBytes, 0, data, 0, data.Length);
	//		byte[] encodedData = new byte[inputBytes.Length];
	//		// note: the number of samples per frame must be a multiple of encoder.FrameSize
	//		int encodedBytes = encoder.Encode (data, 0, data.Length, encodedData, 0, encodedData.Length);
	//
	//		if (encodedBytes != 0)
	//		{
	//			// todo: do something with the encoded data
	//		}
	////		BitConverter.
	//		return encodedData;//BitConverter.GetBytes (encodedBytes);
	//	}
	//
	//	public static byte[] DecompressNSpeex (byte[] encodedData)
	//	{
	//		SpeexDecoder decoder = new SpeexDecoder (BandMode.Wide);
	//		short[] decodedFrame = new short[1024]; // should be the same number of samples as on the capturing side
	//		decoder.Decode (encodedData, 0, encodedData.Length, decodedFrame, 0, false);
	//
	//		byte[] decodeData = new byte[decodedFrame.Length * 2];
	//		Buffer.BlockCopy (decodedFrame, 0, decodeData, 0, decodeData.Length);
	//		return decodeData;
	//		// todo: do something with the decoded data
	//	}

	public static byte[] Compress (byte[] inputBytes)
	{    
		CoderPropId[] propIDs = {
			CoderPropId.DictionarySize,
			CoderPropId.PosStateBits,
			CoderPropId.LitContextBits,
			CoderPropId.LitPosBits,
			CoderPropId.Algorithm,
			CoderPropId.NumFastBytes,
			CoderPropId.MatchFinder,
			CoderPropId.EndMarker
		};
		object[] properties = {
			1 << 22,
			2,
			3,
			0,
			2,
			256,
			"bt4",
			false
		};
			
		SevenZip.Sdk.Compression.Lzma.Encoder encoder = new SevenZip.Sdk.Compression.Lzma.Encoder ();
		encoder.SetCoderProperties (propIDs, properties);

		MemoryStream inStream = new MemoryStream (inputBytes);
		MemoryStream outStream = new MemoryStream ();

		encoder.WriteCoderProperties (outStream);
		long streamSize = inStream.Length;
		for (int i = 0; i < 8; i++)
			outStream.WriteByte ((byte)(streamSize >> (8 * i)));
		encoder.Code (inStream, outStream, -1, -1, null);
		return outStream.ToArray ();
	}

	public static byte[] Decompress (byte[] inputBytes)
	{
		MemoryStream inStream = new MemoryStream (inputBytes);
		SevenZip.Sdk.Compression.Lzma.Decoder decoder = new SevenZip.Sdk.Compression.Lzma.Decoder ();
		inStream.Seek (0, 0);
		MemoryStream outStream = new MemoryStream ();
			
		long outSize;
		var lzmAproperties = new byte[5];
		if (inStream.Read (lzmAproperties, 0, 5) != 5)
		{
			throw new Exception ("Decompress A");
		}
		outSize = 0;
		for (int i = 0; i < 8; i++)
		{
			int b = inStream.ReadByte ();
			if (b < 0)
			{
				throw new Exception ("Decompress B");
			}
			outSize |= ((long)(byte)b) << (i << 3);
		}
		decoder.SetDecoderProperties (lzmAproperties);
		decoder.Code (inStream, outStream, inStream.Length - inStream.Position, outSize, null);
		return outStream.ToArray ();
	}

	//#ff0000
	public static void SetTextFieldStrokeAndShadow (GTextField g, string color, Vector2 shadow, int stroke = 1)
	{		
		g.stroke = stroke;
		g.strokeColor = ToolSet.ConvertFromHtmlColor (color);
		if (shadow != null) {
			g.shadowOffset = shadow;
		} else {
			g.shadowOffset = new Vector2 (0, 0);
		}
	}
    public static void SetButtonBgAndColor(GButton btn,string bg,string str,string fontSize, string color_fons, string color,int strok=2)
    {

        btn.GetChild("n1").asLoader.url = Tools.GetResourceUrl("Image2:"+bg);
        GTextField text=btn.GetChild("title").asTextField;
        if (!IsNullEmpty(fontSize))
        {
            text.textFormat.size = Convert.ToInt32(fontSize);
        }
        text.color = ToolSet.ConvertFromHtmlColor("#" + color_fons);
        Tools.SetTextFieldStrokeAndShadow(text, color, new Vector2(0, 0), strok);
        text.text = Tools.GetMessageById(str);
    }

	public static void SetRootTabTitle (GTextField text_1, string str_1, int fontSize, string color_fons, string color, Vector2 shadow, int stroke)
	{
		text_1.textFormat.size = fontSize;
		text_1.textFormat = text_1.textFormat;
		string str = "";
//		if (text_1.text.IndexOf ("[/") > -1) {
//			str = text_1.text;
//		} else {
//			str = "[0]" + text_1.text + "[/0]";
//		}
//		text_1.text = Tools.GetMessageColor (str, new string[] { color_fons });
		text_1.color = ToolSet.ConvertFromHtmlColor ("#"+color_fons);
		Tools.SetTextFieldStrokeAndShadow (text_1, color, shadow, stroke);
	}
	public static void SetRootTabTitleStrokeColor (GTextField g,string color,int stroke,string txt= ""){
		if (txt != "") {
			g.text = txt;
		}
		g.stroke = stroke;
		g.strokeColor = ToolSet.ConvertFromHtmlColor (color);
	}
	public static string GetConfigFile (string path)
	{
		string msg = "";
		if (!File.Exists (path))
			return msg;
		StreamReader sr = new StreamReader (path);
		msg = sr.ReadToEnd ();
		sr.Close ();
		return msg;
	}

	public static void DataTimeFormat (GTextField g, DateTime time, int type)//0.精确到秒
	{
		long t = ModelManager.inst.gameModel.time.Ticks - time.Ticks;
		switch (type)
		{
		case 0:
			if (t >= ModelManager.inst.roleModel.Oneday)
			{
				g.text = Tools.TimeFormat (t, 3) + Tools.GetMessageById ("31072");
			}
			else if (t >= ModelManager.inst.roleModel.OneHour)
			{
				g.text = Tools.TimeFormat (t, 4) + Tools.GetMessageById ("31072");
			}
			else if (t >= ModelManager.inst.roleModel.OneMili)
			{
				g.text = Tools.TimeFormat (t, 5) + Tools.GetMessageById ("31072");
			}
			else
			{
				//g.text = "1" + Tools.GetMessageById ("14011") + Tools.GetMessageById ("31072");
				g.text = Tools.GetMessageById ("31080");
			}
			break;
		}
	}
    //设置文本颜色 55/8888
    public static string SetFontColor(string str1,string str2,string[] arrColor)
    {
        if (arrColor == null)
        {
            arrColor = new string[] { "E78723", "666666"};
        }
        string str=string.Format("{0}/{1}", str1, str1);
        string[] arr=str.Split('/');
        string a="[0]" + arr[0] + "[/0]";
        string b="[1]" + "/"+arr[1] + "[/1]";
        return Tools.GetMessageColor(a+b, arrColor);
    }



    public static string StrReplace (string str)
	{
		return str.Replace ("\t", "").Replace ("\r", "");
	}

	public static float offectSetX (float x1138,int i = 2)
	{
//		Debug.LogError (GRoot.inst.scale);
		return (x1138 - (1138 - ModelManager.inst.gameModel.width) / i);//*GRoot.inst.scale.x;
	}

	public static string getRankGroup (int rankScore)
	{
		Dictionary<string,object> rankGroup = (Dictionary<string, object>)DataManager.inst.match ["rank_group_show"];
		string group = "";
		foreach (object data in rankGroup.Values)
		{
			object[] value = (object[])data;
			object[] dis = (object[])value [0];
			int min = (int)dis [0];
			int max = (int)dis [1];
			if (rankScore >= min && rankScore <= max)
			{
				group = Tools.GetMessageById (value [4].ToString ());
			}
		}
		return group;
	}

	public static string ConvertDoubleToInt (string str)//获取小数点后的几位数字
	{
		int count = str.LastIndexOf (".");
		int num_ = Convert.ToInt32 (str.Substring (count + 1, 2));
		return num_.ToString ();
		
	}

	public static void ClearTweenList (List<Tweener> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			list [i].Kill ();
		}
		list.Clear ();
	}
   


    public static Tweener SetMoveValue (int value, float time, GProgressBar pro, float exp, float maxExp)
	{
		
		GComponent par = pro.GetChild ("bar").asCom;
		GTextField txt = pro.GetChild ("title").asTextField;
		float _max = pro.max;
		float end;
		if (_max != -1)
			end = (int)(par.width * (value / _max > 1 ? 1 : value / _max));
		else
			end = (int)(par.width * (value / 100 > 1 ? 1 : value / 100));

		Tweener tween = null;
//		if (value != 0) {
		tween = DOTween.To (() => par.width, x => par.width = x, end, time / 2).OnComplete (() =>
		{
			pro.value = value;
//				Debug.LogError (value + " B|| " + pro.value + " || " + pro.max+" || "+exp+" || "+maxExp);
		});
//		} else {
//			pro.value = exp;
//			pro.max = maxExp;
//		}
		if (_max != -1)
		{
			txt.text = (int)value + "/" + _max;//(exp - maxExp + value)
//			Debug.LogError (value + " A|| " + pro.value + " || " + pro.max + " || " + exp + " || " + maxExp);
		}
		else
		{
			txt.text = (int)value + "%";
//			Debug.LogError ("SetMoveValue _max == -1");
		}

		return tween;
	}

	public static string GetNearDistance (object di)
	{
        string color = "666666";
		int distance = Convert.ToInt32 (di);
		string dis = "";
		if (distance >= 0 && distance < 100)
		{
			dis = Tools.GetMessageColor ("[0]" + 100 + Tools.GetMessageById ("19954") + Tools.GetMessageById ("19968") + "[/0]", new string[] { color });
		}
		else if (distance >= 100 && distance < 1000)
		{
			dis = Tools.GetMessageColor ("[0]" + distance + Tools.GetMessageById ("19954") + "[/0]", new string[] { color });
		}
		else if (distance >= 1000 && distance < 100000)
		{
			dis = Tools.GetMessageColor ("[0]" + distance / 1000 + Tools.GetMessageById ("19967") + "[/0]", new string[] { color });
		}
		else
		{
			dis = Tools.GetMessageColor ("[0]" + 100 + Tools.GetMessageById ("19967") + Tools.GetMessageById ("19969") + "[/0]", new string[] { color });
		}
		return dis;
	}
	public static float[] GetRGB(float mm,float cc,ref float[] rgbArr ,float[] nullcolor){
		bool rnull = (nullcolor[0]==1);
		bool gnull = (nullcolor[1]==1);
		bool bnull = (nullcolor[2]==1);

		float min = rgbArr [6];

		if(rgbArr[0]==0f && rgbArr[1]==0f && rgbArr[2]==0f){
			rgbArr[3]+=cc;
			if(rgbArr[3]>=mm){
				rgbArr[0] = 1f;
			}
		}
		else if(rgbArr[0] == 1f && rgbArr[1]==0f && rgbArr[2]==0f){
			rgbArr[4]+=cc;
			if(rgbArr[4]>=mm){
				rgbArr [1] = 1f;
			}
		}
		else if(rgbArr[0] == 1f && rgbArr[1]==1f && rgbArr[2]==0f){
			rgbArr[3]-=cc;
			if(rgbArr[3]<=min){
				rgbArr[0] = 0f;
			}
		}
		else if(rgbArr[0]==0f && rgbArr[1]==1f && rgbArr[2]==0f){
			rgbArr[5]+=cc;
			if(rgbArr[5]>=mm){
				rgbArr [2] = 1f;
			}
		}
		else if(rgbArr[0]==0f && rgbArr[1]==1f && rgbArr[2]==1f){
//			if (bnull) {
//				rgbArr[3]+=cc;
//				rgbArr[4]-=cc;
//				if(rgbArr[3]>=mm && rgbArr[4]<min){
//					rgbArr [0] = 1f;
//					rgbArr [1] = 0f;
//				}
//			}
			if (rnull && bnull) {
//				rgbArr[3]+=cc;
//				rgbArr[4]-=cc;
				rgbArr[5]-=cc;
				if (rgbArr [5] <= min) {
					rgbArr [5] = min;
					rgbArr[3]+=cc;
					if (rgbArr [3] >= mm) {
						rgbArr [0] = 1f;
						rgbArr [2] = 0f;
					}
				}
			}
			else {
				rgbArr [4] -= cc;
				if (rgbArr [4] <= min) {
					rgbArr [1] = 0f;
				}
			}
		}
		else if(rgbArr[0]==0f && rgbArr[1]==0f && rgbArr[2]==1f){
			rgbArr[3]+=cc;
			if(rgbArr[3]>=mm){
				rgbArr[0] = 1f;
			}
		}
		else if(rgbArr[0] == 1f && rgbArr[1]==0f && rgbArr[2]==1f){
			if (rnull) {
				rgbArr[4]+=cc;
				rgbArr[5]-=cc;
				if(rgbArr[4]>=mm && rgbArr[5]<min){
					rgbArr [1] = 1f;
					rgbArr [2] = 0f;
				}
			} else {
				rgbArr [5] -= cc;
				if (rgbArr [5] <= min) {
					rgbArr [2] = 0f;
				}
			}
		}

		return rgbArr;
	}

	public static float[] GetRGB(float mm,float cc,ref float ri,ref float gi,ref float bi,ref bool rb,ref bool gb,ref bool bb){

		if(!rb && !gb && !bb){
			ri+=cc;
			if(ri>=mm){
				rb = true;
			}
		}
		else if(rb && !gb && !bb){
			gi+=cc;
			if(gi>=mm){
				gb = true;
			}
		}
		else if(rb && gb && !bb){
			ri-=cc;
			if(ri<=0){
				rb = false;
			}
		}
		else if(!rb && gb && !bb){
			bi+=cc;
			if(bi>=mm){
				bb = true;
			}
		}
		else if(!rb && gb && bb){
			gi-=cc;
			if(gi<=0){
				gb = false;
			}
		}
		else if(!rb && !gb && bb){
			ri+=cc;
			if(ri>=mm){
				rb = true;
			}
		}
		else if(rb && !gb && bb){
			bi-=cc;
			if(bi<=0){
				bb = false;
			}
		}

		return new float[]{ ri / mm, gi / mm, bi / mm };
	}

	public static string MD5 (string value)
	{
		byte[] data = Encoding.UTF8.GetBytes (value);
		MD5 md5 = new MD5CryptoServiceProvider ();
		byte[] outBytes = md5.ComputeHash (data);
			 
		string msg = "";
		for (int i = 0; i < outBytes.Length; i++)
		{
			msg += outBytes [i].ToString ("x2");
		}
		return msg;
	}
	public static string[] GetUserTel(string str){
		return str.Split (new string[]{ "|" },StringSplitOptions.None);
	}
    public static void setRankData()
    {
        timerEnd = (DateTime)ModelManager.inst.userModel.season["end_time"];
        timerBegin = (DateTime)ModelManager.inst.userModel.season["start_time"];
        season_last_ = (int)DataManager.inst.systemSimple["season_last"];
        season_protect = (int)DataManager.inst.systemSimple["season_protect"];
        season_settle = (int)DataManager.inst.systemSimple["season_settle_time"];
    }
    public static bool[] RankTimer(int type = 0, GTextField text1 = null, GTextField text2 = null)//1.fight  2.rank
    {

        //Debug.Log(ModelManager.inst.gameModel.time);
//        Debug.Log(timerBegin);
//        Debug.Log(timerEnd);
        bool[] bools = new bool[] { };

        if (!ModelManager.inst.rankModel.isChange)
        {

            long rankTimer = ModelManager.inst.gameModel.time.Ticks - timerBegin.Ticks;
            long t1 = timerEnd.Ticks - timerBegin.Ticks - rankTimer;
            if (t1 > 0)
            {
                bools = Tools.SetRankTimerData(type, rankTimer, t1, text1, text2);
            }
            else
            {
                ModelRole.rankTempData = null;
                ModelRole.loveTempData = null;
                ModelRole.killTempData = null;
                ModelRole.mvpTempData = null;
                ModelManager.inst.userModel.season["start_time"] = ModelManager.inst.userModel.season["end_time"];
                ModelManager.inst.userModel.season["end_time"] = new DateTime(((DateTime)ModelManager.inst.userModel.season["start_time"]).Ticks + season_last_ * ModelManager.inst.roleModel.OneMili);
                ModelManager.inst.rankModel.isChange = true;

//                Debug.LogError("start"+ModelManager.inst.userModel.season["start_time"]+"***********   end"+ ModelManager.inst.userModel.season["end_time"]);
            }
        }
        else
        {
            timerBegin = (DateTime)ModelManager.inst.userModel.season["start_time"];
            timerEnd = (DateTime)ModelManager.inst.userModel.season["end_time"];
            long season_last = (long)season_last_ * ModelManager.inst.roleModel.OneMili;
            long rankTimer = ModelManager.inst.gameModel.time.Ticks - timerBegin.Ticks;
            long t1 = season_last - rankTimer;
            if (t1 > 0)
            {
                bools = Tools.SetRankTimerData(type, rankTimer, t1, text1, text2);
            }
            else
            {
                ModelRole.rankTempData = null;
                ModelRole.loveTempData = null;
                ModelRole.killTempData = null;
                ModelRole.mvpTempData = null;
                ModelManager.inst.userModel.season["start_time"] = ModelManager.inst.userModel.season["end_time"];
                ModelManager.inst.userModel.season["end_time"] = new DateTime(((DateTime)ModelManager.inst.userModel.season["start_time"]).Ticks + season_last_ * ModelManager.inst.roleModel.OneMili);
            }
        }
        return bools;
    }

    private  static bool[] SetRankTimerData(int type, long rankTimer, long t1, GTextField text1 = null, GTextField text2 = null)
    {
        bool[] bools = new bool[2];
        bool is_season_settle = false;
        bool is_season_protect = false;
        long oneMili = ModelManager.inst.roleModel.OneMili;
        switch (type)
        {
            case 0:
                if (rankTimer < season_settle * oneMili)
                {
                    is_season_settle = true;
                }
                else if (t1 < season_protect * oneMili)
                {
                    is_season_protect = true;
                }
                else
                {
                }
                break;
            case 1:
                string color1 = "#666666";
                string color2 = "#E07E03";
                if (rankTimer < season_settle * oneMili)
                {
                    is_season_settle = true;
                    string text_3 = Tools.TimeFormat(season_settle* oneMili - rankTimer, 1);
                    string text_1 = "[color=" + color1 + "+][size=15]" + Tools.GetMessageById("13151") + "[/size][/color]";
                    string test_2 = "[color=" + color2 + "+][size=18]" + text_3 + "[/size][/color]";
                    text1.text = text_1 + test_2;
                }
                else if (t1 < season_protect * oneMili)
                {
                    is_season_protect = true;
                    string text_3 = Tools.TimeFormat(t1, 1);

                    string text_1 = "[color=" + color1 + "+][size=15]" + Tools.GetMessageById("13152") + "[/size][/color]";
                    string test_2 = "[color=" + color2 + "+][size=18]" + text_3 + "[/size][/color]";
                    text1.text = text_1 + test_2;
                }
                else
                {
                    //text1.text = Tools.GetMessageById("31078");
                    string text_3 = Tools.TimeFormat(t1 - season_protect * oneMili, 1);
                    string text_1 = "[color=" + color1 + "+][size=15]" + Tools.GetMessageById("31078") + "[/size][/color]";
                    string test_2 = "[color=" + color2 + "+][size=18]" + text_3 + "[/size][/color]";
                    text1.text = text_1 + test_2;
                }
                break;
            case 2:
                if (rankTimer < season_settle * oneMili)
                {
                    is_season_settle = true;
                    string text_3 = Tools.TimeFormat(season_settle * oneMili - rankTimer, 1);
                    text1.text = Tools.GetMessageById("13151") + text_3;
                }
                else if (t1 < season_protect * oneMili)
                {
                    is_season_protect = true;
                    text1.text = Tools.GetMessageById("13152") + Tools.TimeFormat(t1, 1);
                }
                else
                {
                    string text_3 = Tools.TimeFormat(t1 - season_protect * oneMili, 1);
                    text1.text = Tools.GetMessageById("31078") + text_3;
                }
                break;
        }
        bools[0] = is_season_settle;
        bools[1] = is_season_protect;
        return bools;
    }

    public static void FullCard(Dictionary<string,object>data,int gold,string dataname= "gifts_dict") {
        Dictionary<string, object> gifts = (Dictionary<string, object>)( ( (Dictionary<string, object>)data )[dataname] );
        Dictionary<string, object> userData = (Dictionary<string, object>)( ( (Dictionary<string, object>)data )["user"] );
        if(gifts.ContainsKey("gold")) {
            int gift_gold = Convert.ToInt32(gifts["gold"]);
            int gold2 = Convert.ToInt32(userData["gold"]);
            if(gold + gift_gold != gold2) {
                ModelManager.inst.userModel.isShowText = true;
            }
        }
       
    }

}

public class Log
{
	public static List<string> list = new List<string> ();

	public Log ()
	{
	}

	public static void debug (object msg)
	{
		#if UNITY_STANDALONE_WIN  || UNITY_EDITOR_WIN  
		if (msg is string)
		{
			Debug.Log (msg);
			list.Add (msg.ToString ());
			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.UPDATE_LOG));
		}
		else if (msg is object[])
		{
			object[] o = msg as object[];
			string m = "";
			foreach (object n in o)
			{
				m += n.ToString () + ":";
			}
			Debug.Log (m.Substring (0, m.Length - 1));
		}
		else
		{
			Debug.Log (msg.ToString ());
		}
		#endif
	}

}