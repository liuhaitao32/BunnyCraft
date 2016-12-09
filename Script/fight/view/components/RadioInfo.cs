using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///击杀播报数据结构
public class RadioInfo {
	public ClientPlayerEntity player1;
	public ClientPlayerEntity player2;
	public string info;
	public float lightningScale;
	///最短停留时间
	public int timeMin;
	public float scale;
	public Color color;
	///语音
	public string voice;
	///我主语语音
	public string voiceIDo;
	///我宾语语音
	public string voiceDoMe;
	///队友主语语音
	public string voiceAllyDo;
	///敌人主语语音
	public string voiceEnermyDo;


    public RadioInfo(string info, Color color) {
        this.info = info;
        this.color = color;
    }

    public RadioInfo (string type)
	{	
		Dictionary<string,object> dic =  (Dictionary<string,object>)RadioConfig.othersConfig [type];
		initData (dic);
	}
	public RadioInfo (ClientPlayerEntity player1, ClientPlayerEntity player2)
	{	
		this.player1 = player1;
		this.player2 = player2;
		initKill ();
	}
    public RadioInfo(ClientPlayerEntity player, string type) {
        this.player1 = player;
        Dictionary<string, object> dic = (Dictionary<string, object>)RadioConfig.othersConfig[type];
        initData(dic);
    }

    ///玩家1 击杀 玩家2，判别应该使用哪个字典对象
    public void initKill()
	{	
		Dictionary<string,object> dic = null;
		//判断杀人提示的优先级
		if (FightMain.instance.selection.mapData.isTeam && FightMain.instance.selection.killPlayerNum == 1) {
			dic = RadioConfig.firstBlood;
		} else if (player1.fightResult.comboKillNum >= RadioConfig.COMBO_MIN) {
			if (RadioConfig.comboConfig.ContainsKey (player1.fightResult.comboKillNum))
				dic = (Dictionary<string,object>)RadioConfig.comboConfig [player1.fightResult.comboKillNum];
			else
				dic = (Dictionary<string,object>)RadioConfig.comboConfig [RadioConfig.COMBO_MAX];
		} else if (player1.fightResult.currLifeKillNum >= RadioConfig.LIFE_MIN) {
			if (RadioConfig.lifeConfig.ContainsKey (player1.fightResult.currLifeKillNum))
				dic = (Dictionary<string,object>)RadioConfig.lifeConfig [player1.fightResult.currLifeKillNum];
			else
				dic = (Dictionary<string,object>)RadioConfig.lifeConfig [RadioConfig.LIFE_MAX];
		} else if (player2.fightResult.currLifeKillNum >= RadioConfig.LIFE_MIN) {
			dic = RadioConfig.shutDown;
		} else {
			dic = RadioConfig.kill;
		}

		initData (dic);
	}
	public void initData(Dictionary<string,object> dic)
	{	
		//if(TestConfig.isEnglish)
		//	this.info = (string)dic ["infoEn"];
		//else 
			this.info = (string)dic ["info"];
		this.lightningScale = dic.ContainsKey("lightning")?(float)dic ["lightning"]:0f;
		this.scale = (float)dic ["time"];
		this.color = dic.ContainsKey("color")?MaterialUtils.stringToColor((string)dic ["color"]):Color.yellow;
		this.timeMin = (int)(scale*RadioConfig.RADIO_TIME_MIN);

		this.voice = dic.ContainsKey("voice")?(string)dic ["voice"]:null;
		this.voiceIDo = dic.ContainsKey("voiceIDo")?(string)dic ["voiceIDo"]:this.voice;
		this.voiceDoMe = dic.ContainsKey("voiceDoMe")?(string)dic ["voiceDoMe"]:this.voice;
		this.voiceAllyDo = dic.ContainsKey("voiceAllyDo")?(string)dic ["voiceAllyDo"]:this.voice;
		this.voiceEnermyDo = dic.ContainsKey("voiceEnermyDo")?(string)dic ["voiceEnermyDo"]:this.voice;
	}
}
