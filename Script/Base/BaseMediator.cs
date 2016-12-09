using System;
using UnityEngine;
using FairyGUI;
using System.Collections.Generic;
using DG.Tweening;

public class BaseMediator : IBaseClass
{
    private Dictionary<string, List<Action<MainEvent>>> listen;
    public GComponent group;
	public GComponent view;
    private GTextField title;
    public GButton btn_Close;
    public bool isAutoClose = true;

    public string[] barsNum = null;
    public GObject bg;
    public ComTabEffect tab;
    public Controller tabC1;
    public Controller tabC2;
    public int unlockVisFalseNum = 0;
    public GComponent tabLeft;
    public string[] tabLeftNames;
    public object[] btnArr = null;
    public int listCssNum = 0;//list黑白相间的数量

    //
    public static GObject emptyBg;
    private DateTime timerEnd;
    private DateTime timerBegin;
    private int season_last_;
    private int season_protect;
    private int season_settle;
	public string uiName = "";
	public Action<float> jump1;
    public BaseMediator()
    {
        this.Init();
    }

    public void Create(string name, bool isAuto = false, string title = null)
    {
		uiName = name;
        listen = new Dictionary<string, List<Action<MainEvent>>>();
        string[] names = name.Split(':');
        view = UIPackage.CreateObject(names[0], names[1]).asCom;
        group = new GComponent();
        if (isAuto)
        {
            view.width = GRoot.inst.width;
            view.height = GRoot.inst.height;
        }

//		view.onRemovedFromStage.Add(() =>
//		{
//				Debug.LogError("onRemovedFromStage  " + uiName);
//			this.ClearListener();
//			this.Clear();
//		});
		view.onRemovedFromStage.Add(Clear);
        //		if (titleText != null)
        //		{
        //title = Tools.GetComponent (Config.COM_TITLE).asCom;
        //group.AddChild (title);
        //group.AddChild (view);

        //title.width = view.width - 5;
        //title.x = 2;
        //title.y = 0;
        //view.y += title.height + 5;

        //btn_Close = title.GetChild ("n2").asButton;
        //btn_Close.onClick.Add (this.Close);

        //this.text = isAddTitle;

        //		}
        //		else
        //		{
        group.AddChild(view);
        //		}
        group.width = view.width;
        //		if (titleText != null)
        //			group.height = view.height + title.height + 5;
        //		else
        group.height = view.height;
        //		group.fairyBatching = true;
        group.SetPivot(0.5f, 0.5f, false);
        group.Center();
        //		Debug.LogError ("GRoot.inst.fairyBatching :: " + GRoot.inst.fairyBatching.ToString());
        this.CheckTitle(title);
    }

    private void CheckTitle(string text)
    {
        GComponent p;
        GObject bg = this.GetChild("bg");
        if (bg == null)
            return;
        p = bg.asCom;
        if (p == null)
            return;
        GObject close = p.GetChild("close");
        if (close != null)
            (close.asButton).onClick.Add(this.Close);
        GObject t = p.GetChild("title");
        if (t != null)
        {
            title = t.asTextField;
            title.text = text;
        }
        //		this.SetChildIndex (title, this.numChildren - 1);
        //		GComponent obj = this.GetChild ("title").asCom;
        //		title = obj.GetChild ("n2").asTextField;
        //		title.text = text;
        //		GButton close = obj.GetChild ("n3").asButton;
        //		close.onClick.Add (this.Close);
    }


    public void InitTopBar(string[] topBars = null)
    {
        this.barsNum = topBars;
        this.bg = this.GetChild("bg");
        this.tab = this.GetChild("barClip") as ComTabEffect;

        GButton bars = null;
        GTextField text;
        int num = 0;
        for (var i = 0; i < this.barsNum.Length; i++)
        {
            bars = this.GetChild("bar" + i).asButton;
            //			bars.enabled = false;
            bars.y = this.bg.y + 4;
            if (this.barsNum[i] == "")
            {
                bars.visible = false;
                num += 1;
            }
            else
            {
                bars.visible = true;
            }
            text = bars.GetChild("title").asTextField;
            text.text = this.barsNum[i];
        }
        bars.GetChild("right").visible = false;
        Check_TabAlgin(this.barsNum.Length, num);
    }
    public void OnTabChange()
    {
        if (tab != null && tabC1 != null)
        {

            tab.SetIndex(this.GetChild("bar" + tabC1.selectedIndex));
            GTextField text;
            GButton bars = null;
            for (var i = 0; i < this.barsNum.Length; i++)
            {
                bars = this.GetChild("bar" + i).asButton;
                text = bars.GetChild("title").asTextField;
                //				text.textFormat.size = 12;
                //				text.textFormat = text.textFormat;
                if (tabC1.selectedIndex == i)
                {
                    bars.GetChild("right").visible = false;
                    Tools.SetRootTabTitle(text, text.text, 30, "ffffff", "#e77e65", new Vector2(0, 0), 1);
                }
                else
                {
                    Tools.SetRootTabTitle(text, text.text, 24, "848484", "", new Vector2(0, 0), 0);
                    bars.GetChild("right").visible = true;
                }
            }
            bars.GetChild("right").visible = false;

        }
    }
    //	public void Check_lockTop(string[] data){
    //		ModelManager.inst.userModel.GetUnlcok (Config.UNLOCK_PAY, n7);
    //		GButton bars = null;
    //		int num = 0;
    //		foreach(var i in data){
    //			bars = this.GetChild (i.Key).asButton;
    //			if (i.Value == "false") {
    //				bars.visible = false;
    //			} else {
    //				ModelManager.inst.userModel.GetUnlcok (i.Value, bars);
    //			}
    //			if (bars.visible == false) {
    //				num += 1;
    //			}
    //		}
    //		if (data != null && data.Length > 0) {
    //			num = data.Length;
    //			for (int i = 0; i < num; i++) {
    //				this.GetChild (data [i]).visible = false;
    //			}
    //		} else {
    //			num = 0;
    //		}
    //		Check_bar_algin (this.barsNum.Length, num);
    //
    //	}
    private void Check_TabAlgin(int alginNum, int falseNum = 0)
    {
        unlockVisFalseNum = falseNum;
        GButton bars = null;
        int lastNum = (alginNum - falseNum);
        float l = lastNum * 190;
        float left = this.bg.x + (this.bg.width - l) * 0.5f;
        int visFalseNum = 0;

        string firstName = "";
        for (int i = 0; i < alginNum; i++)
        {
            bars = this.GetChild("bar" + i).asButton;
            if (bars.visible == false)
            {
                visFalseNum += 1;
            }
            else
            {
                if (firstName == "")
                {
                    firstName = "bar" + i;
                }
            }
            bars.x = left + bars.width * (i - visFalseNum);
        }
        bars = this.GetChild(firstName).asButton;
        this.tab.x = bars.x;
        this.tab.y = bars.y;
        this.tab.SetCount(lastNum, 0);
        if (lastNum <= 1)
        {
            this.tab.visible = false;
        }
    }

    public void Check_TabLock(Dictionary<string, string> data, GComponent parent)
    {
        GButton bars = null;
        //		int num = 0;
        foreach (var i in data)
        {
            if (parent != null)
            {
                bars = parent.GetChild(i.Key).asButton;
            }
            else
            {
                bars = this.GetChild(i.Key).asButton;
            }
            if (i.Value == "false")
            {
                bars.visible = false;
            }
            else
            {
                ModelManager.inst.userModel.GetUnlcok(i.Value, bars);
            }
            //			if (bars.visible == false)
            //			{
            //				num += 1;
            //			}
        }
    }

    public void Init_LeftTab(string[] names, string tabName)
    {
        tabLeftNames = names;
        tabLeft = this.GetChild(tabName).asCom;
        tabC2 = tabLeft.GetController("c1");

        GButton btn;
        GTextField text;
        //		Log.debug (tabLeft.name);
        for (int i = 0; i < names.Length; i++)
        {
            //			Log.debug (tabLeft.GetChildAt (i).name);
            btn = tabLeft.GetChild("bar" + i).asButton;
            //			btn.enabled = false;
            text = btn.GetChild("title").asTextField;
            text.text = names[i];
        }
    }

    public void OnTabLeftChange()
    {
        if (tabLeft != null && tabC2 != null)
        {
            GButton btn;
            GTextField text;
            for (int i = 0; i < tabLeftNames.Length; i++)
            {
                btn = tabLeft.GetChild("bar" + i).asButton;
                text = btn.GetChild("title").asTextField;
                if (tabC2.selectedIndex == i)
                {
                    Tools.SetRootTabTitleStrokeColor(text, "#d27069", 2);
                }
                else
                {
                    Tools.SetRootTabTitleStrokeColor(text, "#339AB1", 2);
                }
            }
            //			if (tabC1!=null) {
            //				tabC1.selectedIndex = 0;
            //			}
        }
    }

    public void SetButtonData(GButton[] obj, string[] bgArr, string[] textArr, string[] fontColor, string[] color)
    {
        object[] arr = new object[5];
        arr[0] = obj;
        arr[1] = bgArr;
        arr[2] = textArr;
        arr[3] = fontColor;
        arr[4] = color;
        btnArr = arr;
    }

    public void ButtonChange()
    {
        GButton[] arr = (GButton[])btnArr[0];
        for (int i = 0; i < arr.Length; i++)
        {
            GButton btn = (GButton)arr[i];
            GLoader bg = btn.asCom.GetChild("n1").asLoader;
            GTextField gtext = btn.asCom.GetChild("title").asTextField;
            string[] bgArr = (string[])btnArr[1];
            string[] textArr = (string[])btnArr[2];
            string[] fontColor = (string[])btnArr[3];
            string[] color = (string[])btnArr[4];
            string str2 = "";

            if (btn.GetController("c1").selectedIndex == 0)
            {
                bg.url = Tools.GetResourceUrl(bgArr[0].ToString());
                string str1 = "[0]" + textArr[i * 4] + "[/0]";
                str2 = Tools.GetMessageColor(str1, new string[] { fontColor[0] });
                Tools.SetTextFieldStrokeAndShadow(gtext, color[0], new Vector2(1, 2), 1);
            }
            else if (btn.GetController("c1").selectedIndex == 1)
            {
                bg.url = Tools.GetResourceUrl(bgArr[1]);
                string str1 = "[0]" + textArr[i * 4 + 1] + "[/0]";
                str2 = Tools.GetMessageColor(str1, new string[] { fontColor[1] });
                Tools.SetTextFieldStrokeAndShadow(gtext, color[1], new Vector2(1, 2), 1);
            }
            else if (btn.GetController("c1").selectedIndex == 2)
            {
                bg.url = Tools.GetResourceUrl(bgArr[1]);
                string str1 = "[0]" + textArr[i * 4 + 2] + "[/0]";
                str2 = Tools.GetMessageColor(str1, new string[] { fontColor[1] });
                Tools.SetTextFieldStrokeAndShadow(gtext, color[1], new Vector2(1, 2), 1);
            }
            else
            {
                bg.url = Tools.GetResourceUrl(bgArr[0]);
                string str1 = "[0]" + textArr[i * 4 + 3] + "[/0]";
                str2 = Tools.GetMessageColor(str1, new string[] { fontColor[0] });
                Tools.SetTextFieldStrokeAndShadow(gtext, color[0], new Vector2(1, 2), 1);
            }
            btn.text = str2;
        }
    }

    public void CheckListNum(EventContext v)
    {
        int value = (int)v.data;
        if (value > 0)
        {
            if (emptyBg != null)
            {
                emptyBg.visible = true;
            }
        }
        else
        {
            if (emptyBg != null)
            {
                emptyBg.visible = false;
            }
        }
    }

    public string text
    {
        set
        {
            if (title != null)
                title.text = value;
        }
        get
        {
            if (title == null)
                return "";
            else
                return title.text;
        }
    }

    public GObject GetChild(string name)
    {
        return view.GetChild(name);
    }

    public Controller GetController(string name)
    {
        return view.GetController(name);
    }

    public float x
    {
        set { group.x = value; }
        get { return group.x; }
    }

    public float y
    {
        set { group.y = value; }
        get { return group.y; }
    }

    public float width
    {
        set
        {
            group.width = value;
            view.width = value;
        }
        get { return group.width; }
    }

    public float height
    {
        set
        {
            view.height = value;
            if (title != null)
                group.height = view.height + title.height + 5;
            else
                group.height = view.height;
        }
        get { return group.height; }
    }

    public float titleHeight
    {
        //		set
        //		{
        //			view.height = value;
        //			if (title != null)
        //				group.height = view.height + title.height + 5;
        //			else
        //				group.height = view.height;
        //		}
        get
        {
            if (title != null)
            {
                return title.height + 5;
            }
            return 0;
        }
    }

    public bool closeEnable
    {
        set { btn_Close.enabled = value; }
        get { return btn_Close.enabled; }
    }

    public void AddChild(GObject child)
    {
        view.AddChild(child);
    }

    public void AddChildAt(GObject child, int index)
    {
        view.AddChildAt(child, index);
    }

    public void RemoveChild(GObject child, bool dispose = true)
    {
        view.RemoveChild(child, dispose);
    }

    public void SetChildIndex(GObject child, int index)
    {
        view.SetChildIndex(child, index);
    }

    public Tweener TweenMove(Vector2 endValue, float duration)
    {
        return group.TweenMove(endValue, duration);
    }

    public Tweener TweenMoveX(float endValue, float duration)
    {
        return group.TweenMoveX(endValue, duration);
    }

    public Tweener TweenMoveY(float endValue, float duration)
    {
        return group.TweenMoveY(endValue, duration);
    }

    public Tweener TweenScale(Vector2 endValue, float duration)
    {
        return group.TweenScale(endValue, duration);
    }

    public Tweener TweenScaleX(float endValue, float duration)
    {
        return group.TweenScaleX(endValue, duration);
    }

    public Tweener TweenScaleY(float endValue, float duration)
    {
        return group.TweenScaleY(endValue, duration);
    }

    public Tweener TweenResize(Vector2 endValue, float duration)
    {
        return group.TweenResize(endValue, duration);
    }

    public Tweener TweenFade(float endValue, float duration)
    {
        return group.TweenFade(endValue, duration);
    }

    public Tweener TweenRotate(float endValue, float duration)
    {
        return group.TweenRotate(endValue, duration);
    }

    public bool touchable
    {
        set { group.touchable = value; }
        get { return group.touchable; }
    }

    public int numChildren
    {
        //		set{ view.numChildren }
        get { return view.numChildren; }
    }

    public ScrollPane scrollPane
    {
        //		set{ view.scrollPane = value; }
        get { return view.scrollPane; }
    }

    public GComponent parent
    {
        //		set{ group.parent = value; }
        get { return group.parent; }
    }

    public bool visible
    {
        set { group.visible = value; }
        get { return group.visible; }
    }

    public virtual void Close()
    {
        if (!ModelRole.isCloseMask)
        {
            if (ModelRole.isUploadOver == true)
            {
                ModelRole.isUploadOver = false;
                ViewManager.inst.ShowText(Tools.GetMessageById("13165"));
            }
            ViewManager.inst.CloseView(this);
        }
        else
        {
            ViewManager.inst.ShowText(Tools.GetMessageById("13164"));
        }
    }

    public virtual void Init()
    {

    }

    public virtual void Clear()
    {
//		Debug.LogError("onRemovedFromStage  " + uiName);
		ClearListener();
		if (view != null) {
			view.onRemovedFromStage.Remove (Clear);
		}
        view.Dispose();
        group.Dispose();
		if (title != null) {
			title.Dispose ();
		}
        if (ModelRole.isUploadOver == true)
        {
            ModelRole.isUploadOver = false;
            ViewManager.inst.ShowText(Tools.GetMessageById("13165"));
        }
    }

    public void AddGlobalListener(string name, Action<MainEvent> fun)
    {
        if (DispatchManager.inst.HasEventListener(name, fun))
            return;
        if (!listen.ContainsKey(name))
            listen[name] = new List<Action<MainEvent>>();
        listen[name].Add(fun);
        DispatchManager.inst.Register(name, fun);
    }

    public void RemoveGlobalListener(string name, Action<MainEvent> fun)
    {
        if (listen.ContainsKey(name))
        {
            listen[name].Remove(fun);
            if (listen[name].Count == 0)
                listen.Remove(name);
        }
        DispatchManager.inst.Unregister(name, fun);
    }

    public void DispatchGlobalEvent(MainEvent e)
    {
        DispatchManager.inst.Dispatch(e);
    }

    public void DispatchGlobalEventWith(string name, object data = null)
    {
        MainEvent me = new MainEvent(name, data);
        DispatchManager.inst.Dispatch(me);
    }

    public void ClearListener()
    {
        foreach (string n in listen.Keys)
        {
            foreach (Action<MainEvent> fun in listen[n])
            {
                DispatchManager.inst.Unregister(n, fun);
            }
        }
        listen.Clear();
    }

    public void SetListCSS(GList list, object[] roleRecord, int num, bool isTouch)
    {
        //   list.numItems = 0;
        listCssNum = num;
        if (isTouch)
        {
            if (roleRecord.Length > num - 1)
            {
                list.scrollPane.touchEffect = true;
            }
            else
            {
                list.scrollPane.touchEffect = false;
            }
        }
        if (roleRecord.Length <= num && roleRecord.Length > 0)
        {
            list.numItems = num;
        }
        else
        {
            list.numItems = roleRecord.Length;
        }
    }
   
    public bool SetListCSS(GObject item, object[] roleRecord, int index)
    {
        bool isVisible = true;
        GGroup itemObj = item.asCom.GetChild("item").asGroup;
        GImage itemBg = item.asCom.GetChild("itemBg").asImage;
        itemBg.visible = true;
        itemObj.visible = true;
        if (index % 2 == 0)
        {
            itemBg.visible = false;
        }
        if (roleRecord.Length <= listCssNum && roleRecord.Length > 0)
        {
            if (index > roleRecord.Length - 1)
            {
                isVisible = false;
                itemObj.visible = false;

            }
        }
        if (!isVisible)
        {
            item.touchable = false;
        }
        return isVisible;
    }
}