using System;
using FairyGUI;
using DG.Tweening;

public class ComProgressBar:BaseCom
{
	// x y 下间距 右间距
	//绿色
	public const string BAR1 = "Image:bg_bar0,Image:bg_bar1,0,0,0,0";
	//黄色
	public const string BAR2 = "Image:icon_jindutiao,Image:icon_jifen2,5,3,5,9";
	//蓝色
	public const string BAR3 = "Image:icon_dengji3,Image:icon_dengji1,0,0,0,0";
	//Effect绿色
	public const string BAR4 = "Image2:n_icon_jindu1,Image2:n_icon_jindu2,8,5,10,20";
	//Effort蓝色
	public const string BAR5 = "Image2:n_bg_shuxing1,Image2:n_bg_shuxing2,0,0,0,0";
	//Card绿色
	public const string BAR6 = "Image:icon_dengji3,Image:icon_dengji2,0,0,0,0";
	//紫色
	public const string BAR7 = "Image:icon_zst2,Image:icon_zst,2,2,2,2";
	//main绿色
	public const string BAR8 = "Image:icon_jindutiao,Image:icon_jifen,5,2,7,8";
	//explore蓝色
	public const string BAR9 = "Image:icon_jindutiao,Image:icon_jifen3,7,5,5,8";
	//音量
	public const string BAR10 = "Image:icon_yinliang2,Image:icon_yinliang1,0,0,0,0";
	//
	//new image2

	//main绿色
	public const string BAR11 = "Image2:n_icon_shengji3,Image2:n_icon_shengji2,0,0,0,0";
    //倒计时
	public const string BAR12 = "Image2:n_bg_luzhishijian1,Image2:n_bg_luzhishijian2,0,0,0,0";

    private GLoader bg;
	private GComponent par;
    //private GLoader side;
    public GLoader side;
	public GGraph graph;
    // private GTextField txt;
    public GTextField txt;
	private float _value = -1;
	private float _max = -1;
	//	private bool isSkin = false;
	private string _skin;
	public float offsetY = 0;

	public ComProgressBar ()
	{
	}

	public override void ConstructFromXML (FairyGUI.Utils.XML xml)
	{
		base.ConstructFromXML (xml);

		bg = this.GetChild ("n0").asLoader;
		txt = this.GetChild ("n1").asTextField;
		par = Tools.GetComponent (Config.COM_PROGRESSSIDE).asCom;
		side = par.GetChild ("n0").asLoader;
		graph = par.GetChild ("n1").asGraph;

		txt.touchable = false;
		par.touchable = false;
		side.touchable = false;
		graph.touchable = false;
		this.graph.width = 0;

		this.AddChildAt (par, 1);
		txt.text = "";
	}

	public void update ()
	{		
		string[] names = _skin.Split (',');
		this.bg.url = Tools.GetResourceUrl (names [0]);
		this.side.url = Tools.GetResourceUrl (names [1]);
		par.x = Convert.ToInt32 (names [2]);
		par.y = Convert.ToInt32 (names [3]);
		//
		par.width = this.viewWidth - Convert.ToInt32 (names [4]) - Convert.ToInt32 (names [2]);
		par.height = this.viewHeight - Convert.ToInt32 (names [5]) - Convert.ToInt32 (names [3]);
//		txt.x = Convert.ToInt32 (names [2]) * 2;
//		txt.width = par.width - Convert.ToInt32 (names [4]) * 2 - Convert.ToInt32 (names [2]);
//		if (!isSkin)
//		{			
		this.side.width = par.width;
		this.side.height = par.height;
		this.graph.width = 0;
		this.graph.height = par.height;
//		txt.y = side.y - side.height / 2 + 12;
//			isSkin = true;
//		}

		if (_value != -1)
		{
			if (_max != -1)
				this.graph.width = par.width * (_value / _max > 1 ? 1 : _value / _max);
			else
				this.graph.width = par.width * (_value / 100 > 1 ? 1 : _value / 100);
		}
	}

	public GLoader GetBg ()
	{
		return this.bg;
	}

	public GLoader GetBar ()
	{
		return this.side;
	}

	public void SetTextSize (int size)
	{
		TextFormat tf = this.txt.textFormat;
		tf.size = size;
		this.txt.textFormat = tf;
	}

	public void SetTextVisible (bool value)
	{
		this.txt.visible = value;
	}

	public string skin
	{
		get{ return _skin; }
		set
		{
			if (_skin == value)
				return;
			_skin = value;
			update ();
		}
	}

	public float value
	{
		get{ return _value; }
		set
		{
			_value = value;
			if (_max != -1)
				this.graph.width = (int)(par.width * (_value / _max > 1 ? 1 : _value / _max));
			else
				this.graph.width = (int)(par.width * (_value / 100 > 1 ? 1 : _value / 100));
			if (txt != null) {
				if (max != -1)
					txt.text = (int)_value + "/" + _max;
				else
					txt.text = (int)_value + "%";

//				txt.y = (this.height - txt.textHeight) / 2 - side.height / 2 + offsetY + 2;
			}
		}
	}

	public void SetMoveValue (int value, float time)
	{
		_value = value;
		float end;
		if (_max != -1)
			end = (int)(par.width * (_value / _max > 1 ? 1 : _value / _max));
		else
			end = (int)(par.width * (_value / 100 > 1 ? 1 : _value / 100));

		DOTween.To (() => this.graph.width, x => this.graph.width = x, end, time / 2);

		if (max != -1)
			txt.text = (int)_value + "/" + _max;
		else
			txt.text = (int)_value + "%";

//        txt.y = (this.height - txt.textHeight) / 2 - side.height / 2 + offsetY + 2;
        
    }

	public float max
	{
		get{ return _max; }
		set
		{
			_max = value;
			this.graph.width = (int)(par.width * (_value / _max > 1 ? 1 : _value / _max));
			txt.text = (int)_value + "/" + _max;

//            txt.y = (this.height - txt.textHeight) / 2 - side.height / 2 + offsetY +2;
           
        }
	}

	public override string text
	{
		get{ return txt.text; }
		set{ txt.text = value; }
	}

	public override void Clear ()
	{
		base.Clear ();
	}
}