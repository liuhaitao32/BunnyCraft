using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class MediatorSystem:FightViewBase
{
	private string timer;

    public static MediatorSystem instance;

	public Text text;

    public Button btn;


	void Awake() {
        instance = this;
        this.InvokeRepeating("clearMax", 1f, 5f);
        this.btn.onClick.AddListener(()=> {
            this.text.gameObject.SetActive(!this.text.gameObject.activeSelf);
            LogMessage.instance.gameObject.SetActive(this.text.gameObject.activeSelf);
        });
    }

    void clearMax() {
        foreach(String key in times.Keys) {
            Dictionary<string, object> dic = times[key];
            dic["maxTime"] = 0;
            dic["test"] = null;
            dic["intervalTime"] = 0;
            dic["runTotal"] = 0;
            dic["runCount"] = 0;
        }
    }

    void Update() {
        showLabel();
    }
         

	void Time_Tick (float time)
	{
//		    MediatorSystem.timeStart ("sss");
		//this.transform.SetSiblingIndex (player.childCount - 1);
//			MediatorSystem.getRunTime ("sss");
	}


	public static Dictionary <String, Dictionary<String,object>> times = new Dictionary<String, Dictionary<String,object>> ();

	public static void timeStart (string name = "")
	{
		//return;
		if (name == "")
		{
			name = "System";
		}
		if (times.ContainsKey (name) == false)
		{
			Dictionary<String,object> obj = new Dictionary<String,object> ();
			obj.Add ("start", 0);
			obj.Add ("runTime", 0);
			obj.Add ("maxTime", 0);
            obj.Add("test", null);
            obj.Add("runTotal", 0);
            obj.Add("runCount", 0);
            obj.Add ("intervalTime", DateTime.Now.Ticks / 10000);
			times.Add (name, obj);
		}
		times [name] ["start"] = DateTime.Now.Ticks / 10000;

    }

    public static string label = "";

	public static string log2 = "";

	public static Dictionary<string, object> dic = new Dictionary<string, object>();

	public static void log (string key, object value){
        //return;
        dic [key] = value;
		log2 = "";
		foreach (string name in dic.Keys) {
			log2 += name + ":" + dic [name] + "\n";
		}
		//instance.text.text = log2;
	}


	public static long getRunTime (string key = "", object c = null)
	{
		//return 0;
		if (key == "")
		{
			key = "System";
		}
		long time = DateTime.Now.Ticks / 10000;
		Dictionary<string, object> dic = times [key];
		dic["runTime"] = time - Convert.ToInt64(dic ["start"]);
        int runTime = Convert.ToInt32(dic["runTime"]);
        int maxTime = Convert.ToInt32(dic["maxTime"]);
        dic["runTotal"] = Convert.ToInt32(dic["runTotal"]) + runTime;
        dic["runCount"] = (int)dic["runCount"] + 1;
        if(runTime > maxTime) {
            dic["maxTime"] = runTime;
            dic["test"] = c;
        }

		return runTime;

	}

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera) {
        return false;
    }

    public static void showLabel(){
        //return;
        if(!instance.text.gameObject.activeSelf) return;
		label = "";
		foreach (String key in times.Keys)
		{
			Dictionary<string, object> dic = times [key];
            string cc = dic["test"] == null ? "" : dic["test"].ToString();
            //Debug.Log((Convert.ToInt32(dic["runCount"]) == 0) + "   " + dic["runCount"]);
            int avg = Convert.ToInt32(dic["runCount"]) == 0 ? 0 : Convert.ToInt32(dic["runTotal"]) / Convert.ToInt32(dic["runCount"]);
            label += key + ":" + dic ["runTime"] + "ms\t(max:\t" + dic ["maxTime"] + ")\t(avg:\t" + avg  +  ")  " + cc + "\n";
		}
		instance.text.text = label + log2;
	}
}

