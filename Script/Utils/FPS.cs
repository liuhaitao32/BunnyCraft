using UnityEngine;
using System.Collections;

public class FPS : MonoBehaviour
{
	private long mFrameCount = 0;
	private long mLastFrameTime = 0;
	static long mLastFps = 0;

	void Awake ()
	{
		Application.targetFrameRate = 30;
        style.fontSize = 30;
	}

	void Start ()
	{
	
	}

	void Update ()
	{
		UpdateTick ();  
	}

	void OnGUI ()
	{  
		DrawFps ();  
	}

	private void DrawFps ()
	{  
		if (mLastFps > 28)
		{  
			style.normal.textColor = new Color (0, 1, 0);  
		}
		else if (mLastFps > 20)
		{
            style.normal.textColor = new Color (1, 1, 0);  
		}
		else
		{
            style.normal.textColor = new Color (1.0f, 0, 0);  
		}
        GUI.Label (new Rect (0, 0, 350, 40), "fps: " + mLastFps /*+ " " + PlatForm.inst.wifi + PlatForm.inst.HTTP*/, style);//s + " ms:" + Time.deltaTime);  
	}

    GUIStyle style = new GUIStyle();
    private void UpdateTick ()
	{  
		if (true)
		{  
			mFrameCount++;  
			long nCurTime = TickToMilliSec (System.DateTime.Now.Ticks);  
			if (mLastFrameTime == 0)
			{  
				mLastFrameTime = TickToMilliSec (System.DateTime.Now.Ticks);  
			}  

			if ((nCurTime - mLastFrameTime) >= 1000)
			{  
				long fps = (long)(mFrameCount * 1.0f / ((nCurTime - mLastFrameTime) / 1000.0f));  
				mLastFps = fps;  
				mFrameCount = 0;  
				mLastFrameTime = nCurTime;
			}  
		}  
	}

	public static long TickToMilliSec (long tick)
	{  
		return tick / (10 * 1000);  
	}
}
