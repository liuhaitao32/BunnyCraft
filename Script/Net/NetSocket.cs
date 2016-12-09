using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using FairyGUI;
using System.Threading;
using UnityEngine;
using System.Net.NetworkInformation;

public class NetSocket
{
	public Action onConnect;

	private static NetSocket instance;
	private Socket socket;

	private string ip;
	private int port;
	private bool onlyHost;
	private List<VoPackage> package;
	private VoPackage current;
	private byte[] bytes;

	private int isConnectStatus = 0;
	private bool pingSend = true;
	//	private Thread listen;

	public string ping = "true";
	private int pingTime = 0;
	public bool socketLogin = false;
	private int connectNum = 2;
	public bool isInstNet = false;//是 单例 
	//	private int pingMax = 5;

	//0game 1fight
	private int type = 0;

	//回调
	private Dictionary<string,SocketListener> call;
    public static int id = 0;
    private int _id = 0;
	public NetSocket ()
	{
		this.isInstNet = false;
        this._id = (id+=10);
		isConnectStatus = 0;
		bytes = new byte[4];
		package = new List<VoPackage> ();
		call = new Dictionary<string, SocketListener> ();
	}

	public static NetSocket inst
	{
		get
		{
			if (instance == null) {
				instance = new NetSocket ();
				instance.isInstNet = true;
			}
			return instance;
		}
	}

	private AddressFamily GetV4OrV6 ()
	{  
		#if UNITY_ANDROID
		return AddressFamily.InterNetwork;
		#endif
		bool v4 = false;
		bool v6 = false;
		int v4num = 0;
		int v6num = 0;

		//Debug.LogError ("GetV4OrV6--1");
//		IPAddress[] a = Dns.GetHostAddresses ((;
//		foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces()) {
////			if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
////				netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet) {
//				foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses) {
//					if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork) {
////						var ipAddress = addrInfo.Address;
//						v4 = true;
//						v4num++;
//						// use ipAddress as needed ...
//					}
//					else if(addrInfo.Address.AddressFamily == AddressFamily.InterNetworkV6){
////						var ipAddress = addrInfo.Address;
//						v6 = true;
//						v6num++;
//						// use ipAddress as needed ...
//					}
//				}
////			}  
//		}
		//

		//获取说有网卡信息
		NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
		//Debug.LogError ("GetV4OrV6--2");
		foreach (NetworkInterface adapter in nics)
		{
			//判断是否为以太网卡
			//Wireless80211         无线网卡    Ppp     宽带连接
			//Ethernet              以太网卡   
			//Debug.LogError ("GetV4OrV6--3");
			if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
			{
				//Debug.LogError ("GetV4OrV6--4");
				//获取以太网卡网络接口信息
				IPInterfaceProperties ip = adapter.GetIPProperties();
				//获取单播地址集
				//Debug.LogError ("GetV4OrV6--5");
				UnicastIPAddressInformationCollection ipCollection = ip.UnicastAddresses;
				//Debug.LogError ("GetV4OrV6--6");
				foreach (UnicastIPAddressInformation ipadd in ipCollection)
				{
					//Debug.LogError ("GetV4OrV6--7");
					//InterNetwork    IPV4地址      InterNetworkV6        IPV6地址
					//Max            MAX 位址
					if (ipadd.Address.AddressFamily == AddressFamily.InterNetwork){
						v4 = true;
						//Debug.LogError ("GetV4OrV6--8");
						//判断是否为ipv4
//						label1.Text = ipadd.Address.ToString();//获取ip
					}
					else if(ipadd.Address.AddressFamily == AddressFamily.InterNetworkV6){
						v6 = true;
						//Debug.LogError ("GetV4OrV6--9");
					}
				}
			}
		}
		//
//		NetworkInterfaceType.
//		string str = Dns.GetHostName ();
//		IPAddress[] arrIP = Dns.GetHostAddresses (str);
//		foreach (IPAddress ip in arrIP)
//		{  
//			if (AddressFamily.InterNetwork.Equals (ip.AddressFamily))
//			{  
////				Debug.LogError ("is ip 4");
//				v4 = true;
//			}
//			else if (AddressFamily.InterNetworkV6.Equals (ip.AddressFamily))
//			{  
////				Debug.LogError ("is ip 6");
//				v6 = true;
//			}  
//		}  
//		Debug.LogError (v6num+" | "+v4num);
		//Debug.LogError ("GetV4OrV6--10");
		if (v4 && v6)
			return AddressFamily.InterNetwork;
		if (v4)
			return AddressFamily.InterNetwork;
		if (v6)
			return AddressFamily.InterNetworkV6;
		return AddressFamily.InterNetwork;
	}

	public string GetIPByHostName (string hostname)
	{
		try
		{
			IPAddress ip;
			if (IPAddress.TryParse (hostname, out ip))
				return ip.ToString ();
			else
				return Dns.GetHostEntry (hostname).AddressList [0].ToString ();
		}
		catch (Exception)
		{
			throw new Exception ("IP Address Error");
		}
	}

	public void Start (int type, string ip, int port,int cNum = 2)
	{
		this.connectNum = cNum;
		TimerManager.inst.Remove (Check_connect);
		TimerManager.inst.Add (1f, 0, Check_connect);
		string[] ipArr = ip.Split (new string[]{ "." }, StringSplitOptions.None);
		this.onlyHost = false;
		if (ipArr.Length == 4) {
			if (Convert.ToInt32 (ipArr [ipArr.Length - 1]) > 0) {
				
			} else {
				
			}
		} else {
			this.onlyHost = true;
		}
//		this.onlyHost = _onlyHost;
//		Debug.LogError(" ip is :: "+ip + " is host " +this.onlyHost.ToString()+" : "+port);
		this.type = type;
		this.ip = ip;//this.onlyHost ? ip : GetIPByHostName (ip);
		this.port = port;
//		Debug.LogError ("Socket Start -->> " + ip + " : " + port);
		try
		{			
			isConnectStatus = 0;
//			AddressFamily af = this.GetV4OrV6 ();
//			if (af == AddressFamily.InterNetwork)
//				this.ip = PlatForm.inst.SERVER4;
//			else
//				this.ip = PlatForm.inst.SERVER6;
//			this.port = PlatForm.inst.PORT;

			socket = new Socket (this.GetV4OrV6 (), SocketType.Stream, ProtocolType.Tcp);
			socket.NoDelay = true;
			//Debug.LogError ("GetV4OrV6--11");
			IAsyncResult ar;
			if (onlyHost)
			{
				//Debug.LogError ("GetV4OrV6--11-1");
				ar = socket.BeginConnect (this.ip, port, new AsyncCallback (Socket_Connect), socket);
//				Debug.LogError("onlyHost "+this.ip+":"+port);
			}
			else
			{
				//Debug.LogError ("GetV4OrV6--11-2");
				IPEndPoint ipe = new IPEndPoint (IPAddress.Parse (this.ip), this.port);
//				Debug.LogError ("GetV4OrV6--11-2");
				ar = socket.BeginConnect (ipe, new AsyncCallback (Socket_Connect), socket);
			}
		}
		catch (Exception e)
		{
			Interlocked.Exchange (ref isConnectStatus, -1);
//			Debug.LogError (e.Message);
		}
	}

	private void Check_Ping ()
	{
//		if (pingSend)
//		{
		pingSend = true;
		Dictionary<string,object> data = new Dictionary<string, object> ();
		data ["name"] = "ping_ok";
		this.Send (NetBase.SOCKET_PING, data);
//		Log.debug ("ping - send");
		TimerManager.inst.Remove (Socket_Ping);
		TimerManager.inst.Add (1f, 0, Socket_Ping);
//		}
	}
	private void Check_connect(float time){
		if (isConnectStatus < 0) {
			if (connectNum > 0) {
				--connectNum;
				ReConnect ();
			} else {
				DispatchManager.inst.Dispatch (new MainEvent (MainEvent.SOCKET_ERROR_DISCONNECT));
			}
		}
	}
	private void Socket_Ping (float time)
	{		
		pingTime++;
		if (pingTime > PlatForm.inst.timeout)
		{
			pingTime = 0;
			TimerManager.inst.Remove (Socket_Ping);
			if (pingSend)
			{
				pingSend = false;
				ping = "false";

//				NetSocket.inst.RemoveListener (NetBase.SOCKET_PING);
				ReConnect ();
//				DispatchManager.inst.Dispatch (new MainEvent (MainEvent.SOCKET_CLOSE));
//				ViewManager.inst.ShowAlert (Tools.GetMessageById ("14004") + " [TCP]", ReLogin, false, false);
			}
			else
			{
				Check_Ping ();
			}
		}
	}

	public bool IsConnected ()
	{
		if (socket != null && socket.Connected && socketLogin)
			return true;		
		return false;
	}

	private void ReLogin (int index)
	{
		DispatchManager.inst.Dispatch (new MainEvent (MainEvent.RELOGIN_GAME));
	}

	//	public void CheckIsConnect ()
	//	{
	//		if (isConnectStatus == 0)
	//		{
	//		}
	//		else if (isConnectStatus == -1)
	//		{
	//			DispatchManager.inst.Dispatch (new MainEvent (MainEvent.SOCKET_ERROR));
	//			isConnectStatus = 0;
	//		}
	//	}

	public void Close ()
	{
		socketLogin = false;
        TimerManager.inst.Remove(Check_connect);
        TimerManager.inst.Remove (Socket_Ping);
		if (this.type == 0)
		{
			NetSocket.inst.RemoveListener (NetBase.SOCKET_PING);
			TimerManager.inst.Remove (Socket_Ping);
		}
		TimerManager.inst.Remove (Socket_Check);
		isConnectStatus = 0;
		if (socket == null)
			return;
//		socket.Shutdown (SocketShutdown.Both);
//		socket.Disconnect (true);
		socket.Close ();

//		Log.debug ("Socket Close");

//		DispatchManager.inst.Dispatch (new MainEvent (MainEvent.SOCKET_CLOSE));
	}

	public void ReConnect (int index = 0)
	{
		Debug.LogError ("ReConnect isInstNet " + isInstNet);
		Close ();
		if (isInstNet) {
			NetSocket.inst.onConnect = () => {
				NetSocket.inst.onConnect = null;
				Socket_Login ();
				//			Log.debug ("Socket Connect");
			};
		}
		Start (this.type, this.ip, this.port, this.connectNum);
	}
	private void Socket_Login(){
		Dictionary<string, object> data = new Dictionary<string, object> ();
		data ["uid"] = ModelManager.inst.userModel.uid;
		data ["sessionid"] = ModelManager.inst.userModel.sessionid;

		NetSocket.inst.RemoveListener (NetBase.SOCKET_LOGIN);
		NetSocket.inst.AddListener (NetBase.SOCKET_LOGIN,SOCKET_LOGIN_re);
		NetSocket.inst.Send (NetBase.SOCKET_LOGIN, data);
	}
	private void SOCKET_LOGIN_re(VoSocket vo){
		NetSocket.inst.socketLogin = true;
		NetSocket.inst.RemoveListener (NetBase.SOCKET_LOGIN);
	}
	private void Socket_Connect (IAsyncResult ar)
	{		
		try
		{
			Interlocked.Exchange (ref isConnectStatus, 1);
			socket.EndConnect (ar);
//			Debug.LogError ("Socket_Connect -->> " + ar.AsyncState);
			socket.BeginReceive (bytes, 0, VoPackage.HEAD_LEN, SocketFlags.None, new AsyncCallback (Socket_Head), current);

			if (onConnect != null)
				onConnect ();
		}
		catch (Exception e)
		{
//			Debug.LogError (e.Message.ToString ());
			Interlocked.Exchange (ref isConnectStatus, -1);
			return;
		}
		finally
		{
			TimerManager.inst.AddUpdate (Socket_Check);
			if (this.type == 0)
			{				
				ping = "true";
				NetSocket.inst.AddListener (NetBase.SOCKET_PING, (VoSocket vo) =>
				{
					Dictionary<string,object> data = (Dictionary<string,object>)vo.data;
					if (data != null)// && data ["name"].ToString () == "ping_ok"
					{
						ping = "true";
						pingSend = false;
//						Log.debug ("ping - " + ping);
					}
//						Debug.Log("ping back");
				});
				if (isConnectStatus > 0) {
					Check_Ping ();
				}
			}
		}
	}

	private void Socket_Head (IAsyncResult ar)
	{
		try{
//			Debug.LogError ("Socket_Head -->> " + ar.AsyncState);
			int len = socket.EndReceive (ar);
			if (len < 1)
				return;
			if (bytes.Length < VoPackage.HEAD_LEN)
			{
				socket.BeginReceive (bytes, bytes.Length, VoPackage.HEAD_LEN - bytes.Length, SocketFlags.None, new AsyncCallback (Socket_Head), current);
			}
			else
			{
				int bodyLen = System.BitConverter.ToInt32 (bytes, 0);
				current = new VoPackage (bodyLen);
				socket.BeginReceive (current.body, current.readLen, current.bodyLen - current.readLen, SocketFlags.None, new AsyncCallback (Socket_Body), socket);
			}
		}catch(Exception ee){
			Interlocked.Exchange (ref isConnectStatus, -1);
//			Debug.LogError ("Socket_Head -->> " + ee.Message);
		}
	}

	private void Socket_Body (IAsyncResult ar)
	{
		try{
//			Debug.LogError ("Socket_Body -->> " + ar.AsyncState);
			int len = socket.EndReceive (ar);
			if (len < 1)
				return;
			current.readLen += len;
			if (current.readLen < current.bodyLen)
			{
				socket.BeginReceive (current.body, current.readLen, current.bodyLen - current.readLen, SocketFlags.None, new AsyncCallback (Socket_Body), socket);
			}
			else
			{
	            lock (package) {
	                package.Add(current);
	                socket.BeginReceive(bytes, 0, VoPackage.HEAD_LEN, SocketFlags.None, new AsyncCallback(Socket_Head), current);
	            }
				
			}
		}catch(Exception ee){
			Interlocked.Exchange (ref isConnectStatus, -1);
//			Debug.LogError ("Socket_Body -->> " + ee.Message);
		}
	}

    public int index = 0;

    public int count = 0;

    public int maxCount = 0;


    public long tick = 0;

    public long tick2 = 0;

    public static bool test = false;

    private void Socket_Check(float time) {
        if(test) {
            if(package.Count >= 2) test = false;
        }
        if(test) return;
        index++;
//		Debug.LogError ("Socket_Check -->> :: " + package.Count);
        if(package.Count >= 1) {
            count = package.Count;
            maxCount = Math.Max(count, maxCount);
//            long offset = ( System.DateTime.Now.Ticks - tick ) / 10000;

            //			if (0 != tick) {
            //				MediatorSystem.log("socketUpdate", offset);
            //
            //				if (offset >= 150) {
            //					MediatorSystem.log("socket大", offset);
            //				}
            //
            //				if (offset <= 50) {
            //					MediatorSystem.log("socket小", offset);
            //				}
            //
            //
            //			}

            tick = System.DateTime.Now.Ticks;
        }
//		Debug.LogError ("Socket_Check");
        MediatorSystem.log("count", package.Count + "   " + count + "    " + maxCount);
        while(package.Count != 0) {
            VoPackage vo = null;
            lock (package) {
                vo = package[0];
                package.Remove(vo);

            }
            if(vo.isClear) {
//                LogMessage.instance.text.text += "此vo已经被清除过了！";
                continue;
            }
            VoSocket v = new VoSocket();
//            MediatorSystem.timeStart("SockettoDatas");
            v.toDatas(vo.body);
//            MediatorSystem.getRunTime("SockettoDatas");
//			Debug.LogError ("re -->> :: " + v.method);
//			if (v.method == NetBase.SOCKET_LOGIN) {
//				Check_Ping ();
//			}
//            if(v.method != NetBase.SOCKET_PING && v.method != "sync")
//                Log.debug("SocketReceive[" + this.ip + "]:" + JsonUtility.ToJson(v));
            if(!vo.isClear) {
                if(call.ContainsKey(v.method)) {
//					Debug.LogError (v.method);
                    SocketListener sl = call[v.method];
                    //try {
//                    MediatorSystem.timeStart("execSocketData");
                    sl.Excute(v);
//                    MediatorSystem.getRunTime("execSocketData");
                    //} catch(Exception e) {
                    //    Debug.Log(e.HelpLink);
                    //}

                }
            } else {
//                Log.debug("Socket Data Is Clear");
            }
            vo.Clear();
        }
        if(index % 500 == 0) {
            maxCount = 0;
            tick2 = 0;
        }
    }

    //	public void Send (string method, string data, char chars = '|')
    //	{
    //		if (socket == null || !socket.Connected)
    //		{
    //			Log.debug ("Socket is Closed");
    //			return;
    //		}
    //		VoSocket vo = new VoSocket ();
    //		byte[] body = vo.ToBytes (method, data, chars);
    //		byte[] head = System.BitConverter.GetBytes (body.Length);
    //		byte[] d = new byte[VoPackage.HEAD_LEN + body.Length];
    //		Array.Copy (head, 0, d, 0, head.Length);
    //		Array.Copy (body, 0, d, VoPackage.HEAD_LEN, body.Length);
    //		Log.debug ("SocketSend:" + method);
    //		socket.Send (d, SocketFlags.None);
    //	}

    public void Send (string method, Dictionary<string,object> data)
	{
//		Debug.LogError ("Send -->> : "+method);
		if (socket == null || !socket.Connected)
		{
//			Debug.LogError ("Socket error ->> :" + socket.Connected);
//			Log.debug ("Socket is Closed");
//			ViewManager.inst.ShowAlert (Tools.GetMessageById ("14004"), ReConnect, false);
			return;		
		}
		try
		{
			VoSocket vo = new VoSocket ();
			byte[] body = vo.ToBytes (method, data);
			byte[] head = System.BitConverter.GetBytes (body.Length);
			byte[] d = new byte[VoPackage.HEAD_LEN + body.Length];
			Array.Copy (head, 0, d, 0, head.Length);
			Array.Copy (body, 0, d, VoPackage.HEAD_LEN, body.Length);
//			if (vo.method != NetBase.SOCKET_PING && this.type != 1)
//				Log.debug ("SocketSend[" + this.ip + "]:" + JsonUtility.ToJson (vo));
			socket.Send (d, SocketFlags.None);
		}
		catch (Exception e)
		{
//			Debug.LogError (e.Message.ToString ());
			Interlocked.Exchange (ref isConnectStatus, -1);
			return;
		}
	}

	public void AddListener (string method, Action<VoSocket> fun)
	{
		if (call.ContainsKey (method))
			return;
		SocketListener sl = new SocketListener ();
		sl.onEvent += fun;
		call.Add (method, sl);
	}

	public void RemoveListener (string method)
	{
		if (call.ContainsKey (method))
			call.Remove (method);
	}

	public void RemoveListeners ()
	{
		call.Clear ();
	}

	//特定删除
	public void RemoveListenersMethod ()
	{
		this.RemoveListener (NetBase.SOCKET_GETINVITE);
		this.RemoveListener (NetBase.SOCKET_FREEGETINVITE);
	}
}

public class SocketListener
{
	//	public delegate void SocketEvent (VoSocket vo);
	//	public SocketEvent onEvent;
	public Action<VoSocket> onEvent;

	public void Excute (VoSocket vo)
	{
		if (onEvent != null)
		{
			onEvent (vo);
		}
	}
}