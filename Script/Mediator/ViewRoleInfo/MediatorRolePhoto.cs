using UnityEngine;
using System.Collections;
using System;
using FairyGUI;
using System.Collections.Generic;
using DG.Tweening;

public class MediatorRolePhoto : BaseMediator
{
    private GLoader image_photo;
    private GTextField content;
    private GButton del;
    private GImage image_;
    private GObject image_photo_;
    private GImage bg;
    private GList list;
    private ModelRole roleModel;
    private ModelUser userModel;
    private List<Loads> listPhoto;
    private List<object> listData;
    private List<object> tempListData;
    private Dictionary<string, object> otherInfo;
    private GObject tempTag;
    private static int HEAD_WH = 512;
    private const int HEAD_WH_small = 100;
    private const string HEAD_WH_NAME = "_hd";
    private const int OUT_TIME = 500;
    private float moveTime = 6f;
    private float startY = 0;
    private float endY = -70;
    private float distanceTime = 0.1f;
    //private float moveTimeY;
    private Tweener t1;
    private Tweener t2;
    private int upload_max;
    private Tweener t3;
    private FunQueue initQueue;
    private float tempTouchStart;

    //	private bool isBegin = false;

    public override void Init()
    {
        this.Create(Config.VIEW_ROLEPHOTO);
#if UNITY_IOS
		HEAD_WH = 256;
#endif
        initQueue = new FunQueue();
        FindeObject();
        InitDate();
    }

    private void FindeObject()
    {
        Dictionary<string, object> dc = (Dictionary<string, object>)DataManager.inst.systemSimple["picture_config"];
        upload_max = (int)dc["upload_max"];
        list = this.GetChild("n2").asList;
        image_photo_ = this.GetChild("n0");
        image_photo = image_photo_.asCom.GetChild("n0").asLoader;
        image_photo_.touchable = false;
        content = this.GetChild("n5").asTextField;
        del = this.GetChild("n3").asButton;
        del.visible = false;
        image_ = this.GetChild("n4").asImage;
        bg = this.GetChild("n51").asImage;
		del.onClick.Add (() => {
			if (StatusCurr == 1) {
				ViewManager.inst.ShowAlert (Tools.GetMessageById ("13161"), DelPhoto, true);
			} else {
				DelPhoto (1);
			}
		});
        MoveStart();
        image_photo_.onTouchBegin.Add(touchBegin);
        image_photo_.onTouchEnd.Add(touchEnd);
        image_photo_.onRollOut.Add(touchEnd);

    }

	private void DelPhoto(int v){
		if (v <= 0) {
			return;
		}
		Dictionary<string, object> data = new Dictionary<string, object>();
		data["head_key"] = image_photo.name;
		NetHttp.inst.Send (NetBase.HTTP_DEL_PHPTO, data, (VoHttp vo) => {
			userModel.UpdateData (vo.data);
			GetMyData ();
			if (listData.Count != 0)
				list.ScrollToView (0);
		});
	}

    private void MoveStart()
    {
        Time_Move(0);
        TimerManager.inst.Add(2 * moveTime + distanceTime * 2 + 1, 0, Time_Move);
    }

    private void touchEnd(EventContext context)
    {
        TimerManager.inst.Remove(Time1);
        TimerManager.inst.Remove(Time_Move);
        if (t1 != null)
            t1.Kill();
        if (t2 != null)
            t2.Kill();
        if (t3 != null)
            t3.Kill();
        float temp = context.inputEvent.y;

        //moveTime = 1f;
        TimerManager.inst.Add(1f, 1, (float a) =>
        {
//            Debug.Log("AAAAAAAAAAAA" + tempTouchStart + "SSSSS" + temp + "vvvvvvv" + image_photo.position.y);
            if (tempTouchStart > temp)
            {
                if (image_photo.position.y > -15)
                {
                    initQueue.Init(new List<Action> {
                    Move1,
                    Distance,
                    Move2,
                    Distance,
                    MoveStart
                });
                }
                else
                {
                    //image_photo_.asCom.scrollPane.posY = -70;
                    initQueue.Init(new List<Action> {
                    Move2,
                    Distance,
                    MoveStart
                });
                }
            }
            else
            {
                //image_photo_.asCom.scrollPane.posY = -70;
                //Debug.Log("BBBBBBBBBBB" + tempTouchStart + "SSSSS" + temp);
                //initQueue.Init(new List<Action> {
                //    Move2,
                //    Distance,
                //    MoveStart
                //});
                if (image_photo.position.y < -20)
                {
                    initQueue.Init(new List<Action> {
                    Move2,
                    Distance,
                    MoveStart
                });

                }
                else
                {
                    initQueue.Init(new List<Action> {
                    MoveStart
                });
                }
            }
        });
    }

    private void touchBegin(EventContext context)
    {
        tempTouchStart = context.inputEvent.x;
//        Debug.Log(context);
        TimerManager.inst.Remove(Time1);
        TimerManager.inst.Remove(Time_Move);
        if (t1 != null)
            t1.Kill();
        if (t2 != null)
            t2.Kill();
        if (t3 != null)
            t3.Kill();
    }

    private void Time_Move(float obj)
    {


        initQueue.Init(new List<Action> {
                    Move1,
                    Distance,
                    Move2,
                    Distance
                });
    }

    private void Move1()
    {
        t1 = image_photo.TweenMoveY(endY, moveTime);
        t1.OnComplete(() =>
        {
            moveTime = 6f;
            this.initQueue.Next();
        });
    }
    private void Move2()
    {
        t2 = image_photo.TweenMoveY(startY, moveTime);
        t2.OnComplete(() =>
        {
            moveTime = 6f;
            this.initQueue.Next();
        });
    }
    private void Distance()
    {
        TimerManager.inst.Add(distanceTime, 1, Time1);
    }

    private void Time1(float obj)
    {
        this.initQueue.Next();
    }



    private void InitDate()
    {
        roleModel = ModelManager.inst.roleModel;
        userModel = ModelManager.inst.userModel;
        otherInfo = roleModel.otherInfo;
        list.itemRenderer = ListRender;
        //list.SetVirtual ();
        if (otherInfo["uid"].ToString().Equals(userModel.uid))
        {
            NetHttp.inst.Send(NetBase.HTTP_UPDATE_PHOTO, "", (VoHttp vo) =>
           {
               userModel.UpdateData(vo.data);
           });
            GetMyData();
            int max = (int)DataManager.inst.systemSimple["time_Interval"];
            TimerManager.inst.Add(max * 1000L, 0, Time_Tick);
        }
        else
        {
            del.visible = false;
            tempListData = new List<object>();
            Dictionary<string, object> listTmp = (Dictionary<string, object>)Tools.Clone(roleModel.otherInfo["head"]);
            listTmp.Remove("use");
            listData = Tools.ConvertDicToList(listTmp, "name");
            Tools.Sort(listData, new string[] { "time:datetime:1" });
            //  得到数据
            foreach (object v in listData)
            {
                int status = (int)((Dictionary<string, object>)v)["status"];
                if (status == 1)
                {
                    tempListData.Add(v);
                }
            }
            listData.Clear();
            foreach (Dictionary<string, object> v in tempListData)
            {
                listData.Add(v);
            }

            //设置数据
            list.numItems = listData.Count;
            if (listData.Count == 0)
            {
                if (otherInfo["uid"].ToString().Equals(userModel.uid))
                {
                    content.text = Tools.GetMessageById("13149");
                }
                else
                {
                    content.text = Tools.GetMessageById("13063");
                }

            }
        }

    }

    private void GetMyData()
    {
        Dictionary<string, object> listTmp = (Dictionary<string, object>)Tools.Clone(userModel.head);
        listTmp.Remove("use");
        listData = Tools.ConvertDicToList(listTmp, "name");
        Tools.Sort(listData, new string[] { "time:datetime:1" });
        //设置数据
        if (listData.Count < upload_max)
        {
            listData.Add(GetTempData(0));
        }
        list.numItems = listData.Count;
    }

    private void ListRender(int index, GObject item)
    {
        GTextField item_status = item.asCom.GetChild("n2").asTextField;
        item_status.visible = true;
        GButton image = item.asCom.GetChild("n1").asButton;
        GImage image_bg = item.asCom.GetChild("n3").asImage;
        GImage image_Add = item.asCom.GetChild("n8").asImage;
        GImage bg2 = item.asCom.GetChild("n9").asImage;
        bg2.visible = false;
        Dictionary<string, object> data = (Dictionary<string, object>)listData[index];
        string name = (string)data["name"];
        int status = (int)data["status"];
        image.visible = true;
        if (data.ContainsKey("isAdd"))
        {
            bool isAdd = (bool)data["isAdd"];
            if (isAdd)
            {
                image_Add.visible = true;
            }
            else
            {
                image_Add.visible = false;
            }
        }
        else
        {
            image_Add.visible = false;
        }
        item.RemoveEventListeners();
        item.onClick.Add(() =>
       {
           if (image_Add.visible)
           {
               SubPhoto();
           }
           else
           {

               UpdatePhotoHead(item, bg2, name, status, index);

           }

       });

        if (index == 0)
        {
            UpdatePhotoHead(item, bg2, name, status, index);
        }

        if (name != "")
        {
            image_bg.visible = false;
            Tools.SetLoaderButtonUrl(image, ModelUser.GetHeadUrl(name));
            CheckStatus(status, item_status);
        }
        else
        {
            image_bg.visible = true;
            image.GetChild("n0").asLoader.url = "";
            image.visible = false;
            item_status.text = "";
        }

    }

    private Dictionary<string, object> GetTempData(int i)
    {
        Dictionary<string, object> d = new Dictionary<string, object>();
        d["name"] = "";
        d["status"] = 10;
        if (i == 0)
        {
            d["isAdd"] = true;
        }
        else
        {
            d["isAdd"] = false;

        }
        return d;
    }

    private void Time_Tick(float data)
    {
        NetHttp.inst.Send(NetBase.HTTP_UPDATE_PHOTO, "", (VoHttp vo) =>
       {
           userModel.UpdateData(vo.data);
           roleModel.phototTimer = ModelManager.inst.gameModel.time;
           GetMyData();
       });

    }

    private void sdk_callback(object str)
    {
        roleModel.uids.Clear();
        Dictionary<string, object> data = new Dictionary<string, object>();
        //
        string hd = (string)str;
        Texture2D tHD = PhoneManager.inst.Base64ToTexter2d(hd, HEAD_WH, HEAD_WH);
        data["head_file" + HEAD_WH_NAME] = (string)str;
        //缩小再存储
        Texture2D small = PhoneManager.inst.ScaleTexture(tHD, HEAD_WH_small, HEAD_WH_small);
		data["head_file"] = System.Convert.ToBase64String(small.EncodeToJPG());
        //
		NetHttp.inst.Send (NetBase.HTTP_UPLOAD, data, (VoHttp vo) => {
			content.visible = true;
			del.visible = true;
			image_.visible = false;
			userModel.UpdateData (vo.data);
			GetMyData ();
			Change_currHeadimg (((Dictionary<string, object>)listData [0]) ["name"] + "");

		}, null, OUT_TIME);
    }
	private void Change_currHeadimg(string name){
		string nowImg = userModel.head ["use"].ToString ();
		if (nowImg == "h01") {
			Dictionary<string, object> sendData = new Dictionary<string, object>();
			sendData ["head_key"] = name;
			NetHttp.inst.Send (NetBase.HTTP_CHOOSE_HEAD, sendData, (VoHttp vo) => {
				userModel.UpdateData (vo.data);
			});
		}
	}
    private string smallNameToHDname(string smallName)
    {
        //		string[] arr
        //		Debug.LogError(smallName);
        string hdName = smallName + HEAD_WH_NAME;//smallName.Replace(".png",HEAD_WH_NAME+".png");
                                                 //		Debug.LogError(hdName);
        return hdName;
    }

    private void test()
    {
        Texture2D _tex = (Texture2D)Resources.Load("1");
        Dictionary<string, object> data = new Dictionary<string, object>();
		data["head_file" + HEAD_WH_NAME] = System.Convert.ToBase64String(_tex.EncodeToJPG());
        //缩小再存储
        Texture2D small = PhoneManager.inst.ScaleTexture(_tex, HEAD_WH_small, HEAD_WH_small);
        data["head_file"] = System.Convert.ToBase64String(small.EncodeToJPG());

		NetHttp.inst.Send (NetBase.HTTP_UPLOAD, data, (VoHttp vo) => {
			content.visible = true;
			del.visible = true;
			image_.visible = false;
			userModel.UpdateData (vo.data);
			GetMyData ();
			Change_currHeadimg (((Dictionary<string, object>)listData [0]) ["name"] + "");
		}, null, OUT_TIME);
    }

    private void UpdatePhotoHead(string name)
    {
        //		image_photo.url = ModelUser.GetHeadUrl (smallNameToHDname (name));
        //		Tools.SetLoaderButtonUrl (null, ModelUser.GetHeadUrl (smallNameToHDname (name)), image_photo);
        //		image_photo.name = name;
        //		image_photo_.touchable = true;
    }
	private int StatusCurr;
    private void UpdatePhotoHead(GObject item, GImage g2, string name, int status_, int index)
    {
        content.visible = true;
        if (name == "")
        {
            bg.visible = true;
            image_photo.visible = false;
            del.visible = false;
            image_photo_.touchable = false;
            image_.visible = true;
        }
        else
        {
            if (tempTag != null)
            {
                //tempTag.asCom.GetChild ("n6").asImage.visible = true;
                tempTag.asCom.GetChild("n9").asImage.visible = false;
                tempTag = null;
            }
            g2.visible = true;
            tempTag = item;
            //			image_photo.url = ModelUser.GetHeadUrl (smallNameToHDname (name));
            Tools.SetLoaderButtonUrl(null, ModelUser.GetHeadUrl(smallNameToHDname(name)), image_photo);
            image_photo.visible = true;
            image_photo.name = name;
            del.visible = true;
            image_photo_.touchable = true;
            image_.visible = false;
            if (!otherInfo["uid"].ToString().Equals(userModel.uid))
                del.visible = false;
        }
		StatusCurr = status_;
        CheckStatus(status_, content);
    }
    private void CheckStatus(int status_, GTextField content)
    {
        switch (status_)
        {
            case 0:
                content.text = Tools.GetMessageColor("[0]" + Tools.GetMessageById("13030") + "[/0]", new string[] { "FDFAA9" });//审核中
                break;
            case 1:
                content.text = "";
                break;
            case -1:
                content.text = Tools.GetMessageColor("[0]" + Tools.GetMessageById("13031") + "[/0]", new string[] { "FFC7C3" });//未通过
                break;
            default:
                if (otherInfo["uid"].ToString().Equals(userModel.uid))
                {
                    content.text = Tools.GetMessageById("13149");
                }
                else
                {
                    content.text = Tools.GetMessageById("13063");
                }
                break;
        }
    }
    public override void Clear()
    {
        base.Clear();
        TimerManager.inst.Remove(Time_Tick);
        TimerManager.inst.Remove(Time_Move);
    }

    private void SubPhoto()
    {

        if (listData.Count > upload_max)
        {
            ViewManager.inst.ShowText(Tools.GetMessageById("10017"));
        }
        else
        {

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX || UNITY_EDITOR// UNITY_EDITOR_OSX || UNITY_EDITOR || UNITY_EDITOR_64 || (!UNITY_ANDROID && !UNITY_IOS)
            test();
			return;
#endif

#if UNITY_ANDROID || UNITY_IOS
			string wh = HEAD_WH.ToString ();
			PlatForm.inst.GetSdk ().Call (Ex_Local.CALL_OPENPHOTO, new string[] { wh, wh }, sdk_callback);

#endif
        }
    }
}
