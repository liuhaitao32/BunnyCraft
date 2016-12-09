using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;
using FairyGUI;
using LitJson;

public class MediatorUserInfo : BaseMediator {
    private ModelUser userModel;
    private ModelRole roleModel;

    //private double lon;
    //private double lat;
    //private string area;
    //private string addr;
    //private string areaAddr;
    //private string areaAddrTemp;
    private string age;
    private string ageTemp;
    private string sexTemp;
    private string sex;
    private string url;
    //private int country_tag = 0;

    //private List<string> list_addr;
    private List<string> list_age;
    private List<string> listSex;
    //private Dictionary<string, object> addrDiction;
    private Dictionary<string, object> countryData;
    //private Dictionary<string, object> otherInfo;
    private object[] loginType;
    private GTextField textName;
    private GButton changeName;
    private GTextField textUid;
    private GComboBox comboxAge;
    private GComboBox comboxAddr;
    private GComboBox comboxAddr_;
    private GTextField phone;
    private GButton changePhone;
    private GButton changeAccount;
    private GButton changePasswd;
    private GTextField textTitle;
    private GTextField textContnt;
    private GComboBox comboxSex;
    private GComboBox comboxSex_;
    private GButton maskqq;
    private GComponent child1;
    private GComponent child2;
    private GComponent head;
    private Loads ww;    
    private GGroup obj_;
    private GList loginList;
    private List<object> loginTypeList;
    private GTextField t1;
    private GTextField t7;
    private GTextField t2;
    private GTextField t3;
    private GTextField t4;
    private GTextField t5;
    private GTextField t5_;
    private GTextField t6;
    private GTextField t6_;
    private GTextField t2_;
    private GTextField t3_;
    private GTextField textName_;
    private GTextField phone_;
    private GLoader head_Icon;


    public override void Init()
    {
        base.Init();
        Create(Config.VIEW_USERINFO);
        child1 = this.GetChild("n42").asCom;
        child2 = this.GetChild("n51").asCom;
        obj_ = this.GetChild("n45").asGroup;

        t1 = this.GetChild("n2").asTextField;
        t1.text = Tools.GetMessageById("13028");
        textUid = this.GetChild("n3").asTextField;


        t2 = child1.GetChild("n8").asTextField;//name
        t2.text = Tools.GetMessageById("13027");
        t3 = child1.GetChild("n11").asTextField;//sex
        t3.text = Tools.GetMessageById("13089");
        t4 = child1.GetChild("n10").asTextField;
        t4.text = Tools.GetMessageById("13090");
        t5 = child1.GetChild("n0").asTextField;
        t5.text = Tools.GetMessageById("13091");
        t6 = child1.GetChild("n9").asTextField;//phone
        t6.text = Tools.GetMessageById("13097")+"：";

        t2_ = child2.GetChild("n1").asTextField;//name
        t2_.text = Tools.GetMessageById("13027");
        t3_ = child2.GetChild("n3").asTextField;//sex
        t3_.text = Tools.GetMessageById("13089");
        t6_ = child2.GetChild("n2").asTextField;//phone
        t6_.text = Tools.GetMessageById("13141") +"：";

        t5_ = child2.GetChild("n0").asTextField;
        t5_.text = Tools.GetMessageById("13091");
        textName_ = child2.GetChild("n9").asTextField;
        phone_ = child2.GetChild("n4").asTextField;
        bg1 = this.GetChild("n55").asImage;
        //comboxSex_ = child2.GetChild("n5").asComboBox;
        //comboxSex = child1.GetChild("n15").asComboBox;
        //listSex.Add(Tools.GetMessageById("13070"));
        //listSex.Add(Tools.GetMessageById("13071"));





        textName = child1.GetChild("n14").asTextField;
        changeName = child1.GetChild("n31").asButton;
        comboxAge = child1.GetChild("n19").asComboBox;
        comboxAddr = child1.GetChild("n13").asComboBox;
        comboxAddr.visibleItemCount = 3;
        comboxAddr_ = child2.GetChild("n13").asComboBox;
        comboxAddr_.visibleItemCount = 3;

        
        phone = child1.GetChild("n12").asTextField;



        changePhone = child1.GetChild("n20").asButton;
        changePhone.text = Tools.GetMessageById("13086");
        changePhone.onClick.Add(Change_Phone);
        changeAccount = this.GetChild("n6").asButton;
        changeAccount.text = Tools.GetMessageById("13076");
        changeAccount.onClick.Add(Change_Account);
        changePasswd = this.GetChild("n32").asButton;
        changePasswd.text = Tools.GetMessageById("13083");
        changePasswd.onClick.Add(Change_Pass);

        textTitle = this.GetChild("n33").asTextField;
        textTitle.text = Tools.GetMessageById("13058");
        textContnt = this.GetChild("n5").asTextField;
        textContnt.text = Tools.GetMessageById("13059");
        maskqq = GetChild("mask2").asButton;
        maskqq.onClick.Add(ChangeUname);
        maskPhon = GetChild("mask1").asButton;
        maskPhon.onClick.Add(ChangeUname);
        head =GetChild("n8").asCom;
        loginList=GetChild("n11").asList;
        InitDate();
        this.AddGlobalListener(MainEvent.ROLE_UPDATE, ViewUpdate);
    }
    private void InitDate()
    {
        userModel = ModelManager.inst.userModel;
        roleModel = ModelManager.inst.roleModel;
        url = (string)DataManager.inst.systemSimple["gps_url"];
        //lon = Convert.ToDouble(roleModel.longitude);
        //lat = Convert.ToDouble(roleModel.latitude);
        //addrDiction = (Dictionary<string, object>)DataManager.inst.systemSimple["area_config"];
        countryData = (Dictionary<string, object>)DataManager.inst.systemSimple["society_location"];
        loginType = (object[])(DataManager.inst.systemSimple["login_icon"]);
        //loginTypeList = Tools.ConvertDicToList(loginType,"name");
        //Tools.Sort(loginTypeList, new string[] { "name:int:0" });

        //list_addr = new List<string>();
        list_age = new List<string>();
        //area = userModel.area;
        //addr = userModel.addr;

        //userModel.tel = "1";
        if (Tools.IsNullEmpty(userModel.tel))
        {
            obj_.visible = true;
            Dictionary<string, object> headIcon = (Dictionary<string, object>)userModel.head;
            head_Icon=head.GetChild("n0").asCom.GetChild("n0").asLoader;
//            head_Icon.url = ModelUser.GetHeadUrl(headIcon["use"].ToString());
			Tools.SetLoaderButtonUrl(null,ModelUser.GetHeadUrl(headIcon["use"].ToString()),head_Icon);
            head.GetChild("n2").asTextField.text = userModel.lv.ToString();
            loginList.itemRenderer = OnrenderLogin;
            loginList.numItems = loginType.Length;
        }
        else
        {
            bg1.visible = true;
            obj_.visible = false;
            t1.visible = true;
            textUid.visible = true;
            textUid.text = userModel.uid.ToString();

		    type_bing = Tools.GetUserTel (userModel.tel) [0];
			if (type_bing == Ex_Local.LOGIN_TYPE_QQ || type_bing == Ex_Local.LOGIN_TYPE_WEIXIN)
            {
                InitSex(1);
                //changeAccount.x = 377;
                //changeAccount.y = 456;
                child2.visible = true;
                //SetListAddr(comboxAddr_);
                if (userModel.uname != null)
                {
                    textName_.text = userModel.uname.ToString();

                }
                else
                {
                    textName_.text = userModel.uid.ToString();
                }
                switch (type_bing)
                {
                    case Ex_Local.LOGIN_TYPE_QQ:
                        phone_.text = Tools.GetMessageById("33204");
                        break;
                    case Ex_Local.LOGIN_TYPE_WEIXIN:
                        phone_.text = Tools.GetMessageById("33205");
                        break;
                }
                
                maskqq.visible = true;

            }
            else
            {
                //changeAccount.y = 456;
                //changeAccount.x = 200;
                child1.visible = true;
                changePasswd.visible = true;
                //SetListAddr(comboxAddr);
                //InitAge();
                InitSex(2);
                string tel = Tools.GetUserTel(userModel.tel)[1];
                if (!Tools.IsNullEmpty(tel))
                {
                    string old = tel.Substring(3, 4);
                    tel = tel.Replace(old, "****");
                    phone.text = tel;
                }
                if (userModel.uname != null)
                {
                    textName.text = userModel.uname.ToString();

                }
                else
                {
                    textName.text = userModel.uid.ToString();
                }
                maskPhon.visible = true;

            }
        }
    }

    //private void SetListAddr(GComboBox comBox)
    //{
    //    if (addrDiction != null&& !Tools.IsNullEmpty(area))
    //    {
    //        string aa = roleModel.GetAddrCfg(addrDiction, "", area);
    //        if (Tools.IsNullEmpty(addr))
    //        {
    //            areaAddr = aa;
    //        }
    //        else
    //        {
    //            areaAddr = aa + "." + addr;
    //        }
    //        list_addr.Add(areaAddr);
    //    }
    //    if (!PhoneManager.inst.IsOpenGps && addrDiction != null)
    //    {
    //        //ViewManager.inst.ShowText(Tools.GetMessageById("13038"));
    //        foreach (KeyValuePair<string, object> v in addrDiction)
    //        {
    //            list_addr.Add((string)v.Value);
    //        }
    //        InitAddr(comBox);
    //    }
    //    else
    //    {
    //        ww = LoaderManager.inst.Load(url + lat + "," + lon, (object w) =>
    //        {
    //            WWW www = (WWW)w;
    //            string a = www.text.Replace("\n", "");
    //            a = a.Replace("\\", "");
    //            LitJson.JsonData jd = LitJson.JsonMapper.ToObject(a);
    //            LitJson.JsonData result = jd["result"];
    //            string addr1 = (string)result["formatted_address"];
    //            //list_addr.Add(addr1);
    //            LitJson.JsonData addressComponent = result["addressComponent"];
    //            string country = (string)addressComponent["country"];
    //            string city = (string)addressComponent["city"];
    //            country_tag = Tools.GetCountry(country, countryData);

    //            LitJson.JsonData pois = result["pois"];
               
    //            if (Tools.IsNullEmpty(city))
    //            {
    //                foreach (KeyValuePair<string, object> v in addrDiction)
    //                {
    //                    list_addr.Add((string)v.Value);
    //                }
    //            }
    //            else
    //            {
    //                foreach (LitJson.JsonData v in pois)
    //                {
    //                    string name_ = (string)v["name"];
    //                    list_addr.Add(city + "." + name_);
    //                }
    //            }

    //            InitAddr(comBox);
    //        });
    //    }
    //}
	private string Type_Bingding;
    private GButton maskPhon;

    private void OnrenderLogin(int index, GObject item)
    {
        GComponent obj=item.asCom;
        GLoader icon=obj.GetChild("n2").asLoader;
        object[] value=(object[])loginType[index];
        //object[] data=(object[])value[value["name"].ToString()];
        icon.url = Tools.GetResourceUrl("Icon:"+ value[1].ToString());
        item.onClick.Add(()=> {
            switch (value[2].ToString())
            {
                case "phone":
				Type_Bingding = Ex_Local.LOGIN_TYPE_TEL;
                    ViewManager.inst.ShowView<MediatorRegister>();
                    break;
                case "weixin"://微信
                    //userModel.tel = "wx|1204";
                    //InitDate();
                    Bingding(Ex_Local.LOGIN_TYPE_WEIXIN);
                    break;
                case "weibo"://微博
                    break;
                case "qq"://qq

					Bingding(Ex_Local.LOGIN_TYPE_QQ);
                    break;

            }
        });
    }

	private void Bingding(string type)
    {
		Type_Bingding = type;
		if(type == Ex_Local.LOGIN_TYPE_QQ){
			PlatForm.inst.GetSdk ().Call (Ex_Local.ERROR_NO_QQ, null, (object a) => {
				ViewManager.inst.ShowAlert (Tools.GetMessageById ("33207"));
			});
			PlatForm.inst.GetSdk ().Call (Ex_Local.CALL_LOGIN_QQ_HEAD,null,Headimg_getQQ);//QQ 先拿头像再绑定
//			QQ_auth_code("F9DB32C8E692C5F636A2A9AE4CD964B0");
		}
		else if(type == Ex_Local.LOGIN_TYPE_WEIXIN){
			PlatForm.inst.GetSdk ().Call (Ex_Local.ERROR_NO_WX, null, (object a) => {
				ViewManager.inst.ShowAlert (Tools.GetMessageById ("33208"));
			});
			PlatForm.inst.GetSdk ().Call (Ex_Local.CALL_LOGIN_WEIXIN_CODE, null, WX_auth_code);//Weixin 先绑定再拿头像
//			WX_auth_code("021BDUMw0XIMan1yVKQw0r5TMw0BDUM9");
		}
		PlatForm.inst.GetSdk().Login(type,true);//强制 授权登录
    }
	private void WX_auth_code(object re){
		string code = (string)re;
		string param = "code=" + code;
		NetHttp.inst.Send(NetBase.HTTP_REGIST_WX, param, (VoHttp v) =>{

			Debug.Log(v.data);
			Dictionary<string,object> data = (Dictionary<string,object>)v.data;
			Dictionary<string,object> pf = (Dictionary<string,object>)data["data"];
//			[openid]       	o9zsEwc8Xql8WP6QSSeB3fZC2bqs                                                                         	System.Collections.Generic.KeyValuePair<string,object>
//			[scope]        	snsapi_userinfo                                                                                      	System.Collections.Generic.KeyValuePair<string,object>
//			[access_token] 	qVJEDFZiSD-PpenZl9ERev9j3a7lTY_gjs-SNExNFM-LP9GoyK1yQFhaJ3G4Om9VaeqS6TWjO_qCZxi1PQkbw_dVQwXiLNWpCqF9…	System.Collections.Generic.KeyValuePair<string,object>
//			[unionid]      	oucfGvrYR6vmnj5W-T23qwpCrPTE                                                                         	System.Collections.Generic.KeyValuePair<string,object>
//			[expires_in]   	7200                                                                                                 	System.Collections.Generic.KeyValuePair<string,object>
//			[refresh_token]	rdq-ofChFq5wXEzB2r-3u4cBOBFt9pXrwaF__Yj5r2DctS2J_zKyjmiw5xeVxY0HtLbE1RGUjP13f4vwvN9CatnphXVTlc_gbkCp…	System.Collections.Generic.KeyValuePair<string,object>
			userModel.UpdateData(v.data);
            InitDate();
            Set_LocalData(pf,(Dictionary<string,object>)data["user"]);
		});
	}
	private void QQ_auth_code(object re){
		string code = (string)re;
		string param = "code=" + code;
		NetHttp.inst.Send(NetBase.HTTP_REGIST_QQ, param, (VoHttp v) =>{

			Debug.Log(v.data);
			Dictionary<string,object> data = (Dictionary<string,object>)v.data;
			Dictionary<string,object> pf = (Dictionary<string,object>)data["data"];

			userModel.UpdateData(v.data);
            InitDate();
			Set_LocalData(pf,(Dictionary<string,object>)data["user"]);
		});
	}
	private void Set_LocalData(Dictionary<string,object> pf,Dictionary <string,object>user){
		LocalStore.SetLocal (LocalStore.LOCAL_TYPE, Type_Bingding);
		LocalStore.SetLocal (LocalStore.LOCAL_PWD, (string)user ["pwd"]);
		if(Type_Bingding == Ex_Local.LOGIN_TYPE_QQ){
			LocalStore.Set_QQ_Info(userModel.uid,LocalStore.GetLocal(LocalStore.QQ_OPENID),LocalStore.GetLocal(LocalStore.QQ_TOKEN),LocalStore.GetLocal(LocalStore.QQ_DATE));

//			LocalStore.DelUids(userModel.uid);
			LocalStore.SetUids (userModel.uid, userModel.uname, (string)user ["pwd"], Type_Bingding, LocalStore.GetLocal(LocalStore.OTHER_HEADIMG));

			Update_headimg (headstr);
		}
		else if(Type_Bingding == Ex_Local.LOGIN_TYPE_WEIXIN){
			LocalStore.Set_WX_Info (userModel.uid, (string)pf ["openid"], (string)pf ["access_token"], (string)pf ["refresh_token"]);

			PlatForm.inst.GetSdk ().Call (Ex_Local.CALL_AUTH_HEAD_GET_WEIXIN, null, Headimg_getWX);//微信绑定之后才能拿到头像

			PlatForm.inst.GetSdk ().WX_auth_info ("");//微信 找头像
		}
	}
	private string headstr = "";
    private string type_bing;
    private GButton to_man;
    private GButton to_wman;
    private GButton to_man_;
    private GButton to_wman_;
    private GImage bg1;

    private void Headimg_getQQ(object re){
//		if (Type_Bingding == Ex_Local.LOGIN_TYPE_WEIXIN) {
//			Update_headimg ((string)re);
//		}
		headstr = (string)re;
		QQ_auth_code (LocalStore.GetLocal (LocalStore.QQ_TOKEN));
	}
	private void Headimg_getWX(object re){
//		LocalStore.DelUids(userModel.uid);
		LocalStore.SetUids (userModel.uid, userModel.uname, LocalStore.GetLocal (LocalStore.LOCAL_PWD), Type_Bingding, LocalStore.GetLocal(LocalStore.OTHER_HEADIMG));

		Update_headimg ((string)re);
	}
	private void Update_headimg(string img){
        //

        //lht这里需要判断平台的 账号是否绑定过了，是否能用平台账号绑定
//        string param = "";
//        switch (Type_Bingding) {
//            case Ex_Local.LOGIN_TYPE_QQ:
//                param = "openid=" + LocalStore.QQ_OPENID;
//                param += "|token=" + LocalStore.QQ_TOKEN;
//                param += "|data=" + LocalStore.QQ_DATE;
//                break;
//            case Ex_Local.LOGIN_TYPE_WEIXIN:
//                param = "openid=" + LocalStore.WX_OPENID;
//                param += "|token=" + LocalStore.WX_TOKEN;
//                param += "|data=" + "";
//                break;
//        }
//        NetHttp.inst.Send(NetBase.HTTP_REGIST, param, (VoHttp v) =>
//        {
//            Debug.Log(v.data);
//            if ((string)v.data != string.Empty)
//            {
                ViewManager.inst.ShowText(Tools.GetMessageById("13124"));
                //LocalStore.SetLocal(LocalStore.LOCAL_PWD, input_password.text);
                //LocalStore.SetLocal(LocalStore.LOCAL_TEL, input_phone.text);
                //userModel.tel = input_phone.text;
                //otherInfo["tel"] = input_phone.text;
                //LocalStore.DelUids(userModel.uid);
                //LocalStore.SetUids(userModel.uname, input_password.text, Ex_Local.LOGIN_TYPE_UNAME);
                //Dictionary<string, object> dc = new Dictionary<string, object>();
                //dc.Add("value", "");
                //dc.Add("tag", "account");
                //DispatchGlobalEvent(new MainEvent(MainEvent.ROLE_UPDATE, dc));
                //ViewManager.inst.CloseView(this);

                Dictionary<string, object> data = new Dictionary<string, object>();
                data["head_key"] = img;
                NetHttp.inst.Send(NetBase.HTTP_CHOOSE_HEAD, data, (VoHttp vo) =>
                {
//                    userModel.type_login = Type_Bingding;
                    userModel.UpdateData(vo.data);
                    //
//                    LocalStore.SetLocal(LocalStore.LOCAL_TYPE, userModel.type_login);
                    //
                    Dictionary<string, object> headIcon = (Dictionary<string, object>)userModel.head;
                    head_Icon = head.GetChild("n0").asCom.GetChild("n0").asLoader;
//                    head_Icon.url = ModelUser.GetHeadUrl(headIcon["use"].ToString());
					Tools.SetLoaderButtonUrl(null,ModelUser.GetHeadUrl(headIcon["use"].ToString()),head_Icon);

					LocalStore.SetLocal(LocalStore.OTHER_HEADIMG+userModel.uid,LocalStore.GetLocal(LocalStore.OTHER_HEADIMG));
                });

//            }
//        });

        //
        
	}


    private void ViewUpdate(MainEvent e)
    {
        Dictionary<string, object> obj1 = (Dictionary<string, object>)e.data;
        string tag = (string)obj1["tag"];
        if (tag.Equals("chphone"))
        {
			phone.text = Tools.GetUserTel(userModel.tel)[1];
        }
        if (tag.Equals("account"))
        {
            InitDate();
         
        }
        if (tag.Equals("uname"))
        {
            if (type_bing.Equals(Ex_Local.LOGIN_TYPE_TEL))
            {
                textName.text = userModel.uname;
            }
            else
            {
                textName_.text = userModel.uname;
            }
            
            
        }
        if (tag.Equals("photo"))
        {
            
            //Tools.SetLoaderButtonUrl(head_Icon, ModelUser.GetHeadUrl(userModel.));
        }
    }
    //private void InitAge()
    //{

    //    for (int i = 1; i <= 100; i++)
    //    {
    //        list_age.Add(i + "");
    //    }

    //    list_age = new List<string>();
    //    for (int i = 1; i <= 100; i++)
    //    {
    //        list_age.Add(i + "");
    //    }
    //    comboxAge.items = list_age.ToArray();
    //    if (userModel.age != 1)
    //    {
    //        comboxAge.text = userModel.age.ToString();
    //    }
    //    age = comboxAge.text;
    //    ageTemp = comboxAge.text;
    //    comboxAge.onChanged.Add(() =>
    //    {
    //        ageTemp = comboxAge.text;
    //    });
    //}

    private void InitSex(int type)
    {
        if (type == 1)
        {
            to_man = child2.GetChild("n5").asButton;
            to_man.text = Tools.GetMessageById("13070");
            to_man.changeStateOnClick = true;
            to_wman = child2.GetChild("n44").asButton;
            to_wman.text = Tools.GetMessageById("13071");
            to_wman.changeStateOnClick = true;
        }
        else
        {

            to_man = child1.GetChild("n15").asButton;
            to_man.text = Tools.GetMessageById("13070");
            to_man.changeStateOnClick = true;
            to_wman = child1.GetChild("n56").asButton;
            to_wman.text = Tools.GetMessageById("13071");
            to_wman.changeStateOnClick = true;
        }

        sex = userModel.sex;
        to_man.selected = (userModel.GetSex==0);
		to_wman.selected = (userModel.GetSex == 1);
        sexTemp = userModel.sex;

        //        if (Tools.IsNullEmpty(userModel.sex))
        //        {
        //            sexTemp = "m";
        //            sex = "";
        //            //comboxSex.text = listSex[1];
        //            to_man.selected = true;
        //            to_wman.selected = false;
        //        }
        //        else
        //        {
        //            sex = userModel.sex.ToString();
        //            if (userModel.sex.ToString().Equals("f"))
        //            {
        //                //comboxSex.text = listSex[0];
        //
        //                to_man.selected = true;
        //                to_wman.selected = false;
        //                sexTemp = "f";
        //                sex = "f";
        //            }
        //            if (userModel.sex.ToString().Equals("m"))
        //            {
        //                to_wman.selected = true;
        //                to_man.selected = false;
        //                sex = "m";
        //            }
        //        }
        to_man.onChanged.Add(OnManChanged);
        to_wman.onChanged.Add(OnWmanChanged);
    }
    private void OnWmanChanged(EventContext context)
    {
        Debug.Log(context.sender);
        bool isWman = to_wman.selected;
        if (isWman == true)
        {
            to_wman.selected = true;
            to_man.selected = false;
            sexTemp = "f";
        }
        else
        {
            to_man.selected = true;
            to_wman.selected = false;
            sexTemp = "m";

        }
    }

    private void OnManChanged(EventContext context)
    {
        bool isMan = to_man.selected;
        if (isMan == true)
        {
            to_man.selected = true;
            to_wman.selected = false;
            sexTemp = "m";
        }
        else
        {
            to_wman.selected = true;
            to_man.selected = false;
            sexTemp = "f";
        }
    }

//    private void InitAddr(GComboBox comBox)
//    {

//        if (list_addr.Count > 0)
//        {
////            Debug.Log(list_addr.Count);
////            Debug.Log(comBox);
//            comBox.items = list_addr.ToArray();
//            comBox.selectedIndex = 0;
//            areaAddr = comBox.text;
//            areaAddrTemp = comBox.text;
//            comBox.onChanged.Add(() =>
//            {
//                areaAddrTemp = comBox.text;
//                if (areaAddrTemp.IndexOf('.') == -1)
//                {
//                    area = areaAddrTemp;
//                    addr = "";
//                }
//                else {
//                    string[] str = areaAddrTemp.Split('.');
//                    area = str[0];
//                    addr = str[1];
//                }
//            });
//        }
    //}

    private void Save()
    {
		Dictionary<string, object> dic = new Dictionary<string, object>();
        dic["age"] = 1;
        dic["sex"] = sexTemp;
        dic["country"] = userModel.country;
        
        NetHttp.inst.Send(NetBase.HTTP_CHANGE_USERINFO, dic, (VoHttp v) =>
        {
            if ((bool)v.data)
            {
                userModel.sex = dic["sex"].ToString();
                userModel.age = (int)dic["age"];
                roleModel.otherInfo["sex"] = dic["sex"].ToString();
                roleModel.otherInfo["age"] = (int)dic["age"];
                Dictionary<string, object> dc = new Dictionary<string, object>();
                dc.Add("value", "");
                dc.Add("tag", "sex");
                DispatchGlobalEvent(new MainEvent(MainEvent.ROLE_UPDATE, dc));

            }
        });
    }

    private void ChangeUname()
    {
		ViewManager.inst.ShowView<MediatorChangeName>();
    }

    private void Change_Account()
    {
		ViewManager.inst.ShowView<MediatorChangeAccount>();
    }

    private void Change_Pass()
    {

		ViewManager.inst.ShowView<MediatorChangePassword>();
    }

    private void Change_Phone()
    {
		ViewManager.inst.ShowView<MediatorRemovePhone>();
    }

    

    private int GetListKey(List<string> list, string value)
    {
        int key = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Equals(value))
            {
                key = i;
            }
        }
        return key;
    }

    private string GetDictionaryKey(Dictionary<string, object> dc, string value)
    {
        string key = "";
        foreach (KeyValuePair<string, object> str in dc)
        {
            if (str.Value.Equals(value))
            {
                key = str.Key;
            }
        }
        return key;
    }
    public override void Clear()
    {
        if ( !Tools.IsNullEmpty(ageTemp) || !Tools.IsNullEmpty(sexTemp))
        {

			if ((age!=null && !age.Equals(ageTemp)) || !sex.Equals(sexTemp))
            {

                Save();
            }
        }
        RemoveGlobalListener(MainEvent.ROLE_UPDATE, ViewUpdate);
        if (ww != null)
            Tools.Clear(ww);
    }

}
