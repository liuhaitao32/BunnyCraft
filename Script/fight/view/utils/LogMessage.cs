using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LogMessage : MonoBehaviour {

    public Text text;

    public static LogMessage instance;

    void Awake() { instance = this; }

	// Use this for initialization
	void Start () {
        Application.RegisterLogCallback(this.unityLogHandler);
    }
    


    public void unityLogHandler(string logString, string stackTrace, LogType type) {
        if(type == LogType.Exception) {
            this.text.text += logString + "\n" + stackTrace;
        }
        
    }

    // Update is called once per frame
    void Update () {
	    if(Input.GetKey(KeyCode.Z)) {
            text.text = "";
        }
	}
}
