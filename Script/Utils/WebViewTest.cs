using UnityEngine;
using System.Collections;

class WebViewCallbackTest : Kogarasi.WebView.IWebViewCallback
{
	
	public void onLoadStart( string url )
	{
		Debug.Log( "call onLoadStart : " + url );
	}
	public void onLoadFinish( string url )
	{
		Debug.Log( "call onLoadFinish : " + url );
	}
	public void onLoadFail( string url )
	{
		Debug.Log( "call onLoadFail : " + url );
	}
}

public class WebViewTest : MonoBehaviour
{

	WebViewCallbackTest m_callback;
	void Awake(){
//		RemoteLog.Instance.Start ("192.168.1.34",2010);
	}
	// Use this for initialization
	void Start () {

		m_callback = new WebViewCallbackTest();

		WebViewBehavior webview = GetComponent<WebViewBehavior>();
	
		if( webview != null )
		{
			webview.LoadURL( "https://www.baidu.com" );
			webview.SetVisibility( true );
			webview.setCallback( m_callback );
		}
	}
}
