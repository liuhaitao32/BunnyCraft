using System;
using UnityEngine;
using System.Collections;

public class PhoneManager
{
	private static PhoneManager instance;
	//	private float viborateX = 0f;
//	private float viborateY = 0f;
	private Action fun;
	private string viborateId;
	private string keyboardId;
    public bool IsOpenGps=false;

	public PhoneManager ()
	{
	}

	public static PhoneManager inst
	{
		get
		{
			if (instance == null)
				instance = new PhoneManager ();
			return instance;
		}
	}
	public Texture2D Base64ToTexter2d (string Base64STR, int w = 200, int h = 200)
	{
		Texture2D pic = new Texture2D(w, h);
		byte[] data = System.Convert.FromBase64String(Base64STR);
		pic.LoadImage(data);
		return pic;
	}
	public Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
	{
		Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);

//		float incX = (1.0f / (float)targetWidth);
//		float incY = (1.0f / (float)targetHeight);

		for (int i = 0; i < result.height; ++i)
		{
			for (int j = 0; j < result.width; ++j)
			{
				Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
				result.SetPixel(j, i, newColor);
			}
		}

		result.Apply();
		return result;
	}
	//截屏
	public void ScreenImage (string name, int size = 0)
	{
		string path = Application.dataPath + "/" + name + ".jpg";
		Application.CaptureScreenshot (path, size);
		Log.debug (path);
	}

	public void CaptureScreen (string name, Rect rect, Vector2 v2)
	{  
		Texture2D t2 = new Texture2D ((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);  
		t2.ReadPixels (rect, 0, 0);  
		t2.Apply ();
		if (v2 != Vector2.zero)
		{
			if (t2.width >= v2.x && t2.height >= v2.y)
				t2 = Tools.ScaleTexture (t2, (int)v2.x, (int)v2.y);
		}
		Tools.SaveImage (name, t2);
	}

	public Texture2D CaptureCamera (string name, Camera camera, Rect rect,Rect rect2D,bool save = false)
	{  		
		RenderTexture old = RenderTexture.active;
		RenderTexture rt = new RenderTexture ((int)rect.width, (int)rect.height, 0);
		camera.targetTexture = rt;  
		camera.Render ();
		// 激活这个rt, 并从中中读取像素  
		RenderTexture.active = rt;  
		Texture2D screenShot = new Texture2D ((int)rect2D.width, (int)rect2D.height, TextureFormat.RGB24, false);  
//		screenShot.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
		screenShot.ReadPixels (rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素  
		screenShot.Apply ();  
		// 重置相关参数，以使用camera继续在屏幕上显示
		camera.targetTexture = null;  
		RenderTexture.active = old; // JC: added to avoid errors  
		GameObject.Destroy (rt);  
		GameObject.DestroyObject (camera.gameObject);
		// 最后将这些纹理数据，成一个png图片文件  
		if(save){
			byte[] bytes = screenShot.EncodeToJPG ();
			string filename = Application.dataPath + "/" + name+".jpg";  //persistentDataPath
			System.IO.File.WriteAllBytes (filename, bytes);  
		}
//		Log.debug (string.Format ("截屏了一张照片: {0}", filename));  
		return screenShot;
	}
	public void SaveTexture2D(Texture2D t,string name){
		string str = Application.persistentDataPath + "/" + name + ".jpg";
		///读取屏幕像素点
//		yield return new WaitForEndOfFrame();
//		Texture2D ts= "你的图片";
		byte[] byt = t.EncodeToJPG();
		System.IO.File.WriteAllBytes(str,byt);
	}
	public Texture2D GetTexture2d (Texture2D source, Texture2D target, Vector2 pos)
	{
		Texture2D tt = new Texture2D (source.width, source.height);
		tt.SetPixels32 (0, 0, source.width, source.height, source.GetPixels32 (), 0);
		tt.Apply ();

		Color32[] colors = target.GetPixels32 ();
		tt.SetPixels32 ((int)pos.x, (int)pos.y, target.width, target.height, colors, 0);
		tt.Apply ();

		return tt;
	}
	public Texture2D DrawRect(int w,int h,int dx,int dy,int dw,int dh,float a = 0f){
		Texture2D tt = new Texture2D (w, h);
		Color c1 = new Color (0.3f, 0.3f, 0.3f, 0.5f);
		Color c2 = new Color (1f, 1f, 1f, a);
		for (int i = 0; i < w; i++) {
			for (int j = 0; j < h; j++) {
				if(i>=dx && j>=dy && i<=(dx+dw) && j<= (dy+dh)){
					tt.SetPixel (i, j, c2);
				}
				else{
					tt.SetPixel (i, j, c1);
				}
			}
		}
		tt.Apply ();
		return tt;
	}
	public Texture2D DrawCircular(int w,int h,int dx,int dy,int r,int rr,float a = 0f){
		Texture2D tt = new Texture2D (w, h);
		Color c1 = new Color (0.3f, 0.3f, 0.3f, 0.7f);
		Color c2 = new Color (0.3f, 0.3f, 0.3f, 0f);
		Vector3 p = new Vector3 (dx, dy);
		Color c3;
		float adds = 0.5f / rr;
		float dd = 0;

		for (int i = 0; i < w; i++) {
			for (int j = 0; j < h; j++) {
				if (i >= (dx- r) && j >= (dy-r) && i <= (dx + r) && j <= (dy + r)) {
					dd = Vector3.Distance (new Vector3 (i, j), p);
					if (dd <= r) {
						tt.SetPixel (i, j, c2);
						for (int n = rr; n >= 0; n--) {
							if (dd > (r - n)) {
								c3 = tt.GetPixel (i, j);
								if (c3.a < 0.5f) {
									c3.a += adds;
								} else {
									c3.a = 0.5f;
								}
								tt.SetPixel (i, j, c3);
							}
						}
					} else {
						tt.SetPixel (i, j, c1);
					}
				}
				else{
					tt.SetPixel (i, j, c1);
				}
			}
		}
		tt.Apply ();
		return tt;
	}
	//震动
	public void Vibrate (Action fun)
	{
//		this.fun = fun;
//		viborateId = RenderManager.Instance ().AddTimeUpdate ((float time) =>
//		{
//			float y = Input.acceleration.y;
//			float curY = y - viborateY;
//			viborateY = y;
//			if (curY > 0.8f)
//			{						
//				RenderManager.Instance ().RemoveTimeUpdate (viborateId);
//				#if UNITY_ANDROID
//				Handheld.Vibrate ();
//				#endif
//
//				#if UNITY_IPHONE
//				Handheld.Vibrate ();
//				#endif
//				if (this.fun != null)
//					this.fun ();
//			}	
//		}, 0.5f);
	}

	public void Keyboard ()
	{
//		keyboardId = RenderManager.Instance ().AddTimeUpdate ((float time) =>
//		{
//			//截屏
//			if (Input.GetKey (KeyCode.F5))
//			{
//				this.ScreenImage ("test.png");
//			}
//			else if (Input.GetKey (KeyCode.F6))
//			{
//				#if UNITY_ANDROID
//				Handheld.Vibrate ();
//				#endif
//
//				#if UNITY_IPHONE
//				Handheld.Vibrate ();
//				#endif
//			}
//		}, 0.1f);
	}

	public void GetGps (Location.LocationEvent fun)
	{
		Location.Get ().Load (fun);
	}

}

public class Location:MonoBehaviour
{
	public delegate void LocationEvent (string la, string lo);

	private string latitude;
	private string longitude;
	private int time = 10;
	private LocationEvent onEvent;

	private static Location instance;
	private static GameObject go;

	public Location ()
	{		
	}

	public static Location Get ()
	{
		go = new GameObject ("Location");
		instance = go.AddComponent<Location> ();
		return instance;
	}

	public void Load (LocationEvent fun)
	{
		this.onEvent = fun;
		this.StartCoroutine (LoadGps ());
	}

	public void Clear ()
	{
		Tools.Clear (go);
	}

	private IEnumerator LoadGps ()
	{ 
		// LocationService.isEnabledByUser 用户设置里的定位服务是否启用  
		if (!Input.location.isEnabledByUser)
        {
            PhoneManager.inst.IsOpenGps = false;
            yield return false;

        }
        // LocationService.Start() 启动位置服务的更新,最后一个位置坐标会被使用  
        Input.location.Start (10.0f, 10.0f);  

		while (Input.location.status == LocationServiceStatus.Initializing && time > 0)
		{  
			// 暂停协同程序的执行(1秒)  
			yield return new WaitForSeconds (1);  
			time--;  
		}  

		if (time < 1)
        {
            PhoneManager.inst.IsOpenGps = false;
            yield return false;

        }

		if (Input.location.status == LocationServiceStatus.Failed)
		{
            PhoneManager.inst.IsOpenGps = false;
            yield return false;  
			if (onEvent != null)
				onEvent ("0", "0");		
		}
		else
		{
            if(Input.location.status != LocationServiceStatus.Stopped)
            {
                PhoneManager.inst.IsOpenGps = true;
                latitude = Input.location.lastData.latitude + "";
                longitude = Input.location.lastData.longitude + "";
                yield return null;
                PhoneManager.inst.IsOpenGps = true;
                if (onEvent != null)
                    onEvent(latitude, longitude);
            }
		}  
		Input.location.Stop ();
		this.Clear ();
	}

}