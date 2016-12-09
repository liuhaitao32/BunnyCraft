using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;
using FairyGUI;

public class MediatorGameSet : BaseMediator
{
	private GComboBox gCombox;
	private GButton music_close;
	private GButton music_open;
	private GButton sound_close;
	private GButton sound_open;
	private GButton voice_close;
	private GButton voice_open;
	private GButton request_close;
	private GButton request_open;
	private object[] language;
	private List<object[]> arr;
	private List<string> arrValue;
	public bool isVoice;
	public bool isInvite;
	public bool isAttention;
	public bool isDreamAttetion;
    private GButton attention_close;
	private GButton music;
	private GButton sound;
	private GButton voice;
	private GButton request;
	private GButton attention;
	private ModelUser userModel;
	private ModelRole roleModel;
    private GButton dream_attention;

    public override void Init ()
	{
		base.Init ();
		Create (Config.VIEW_GAMESET);
		userModel = ModelManager.inst.userModel;
		roleModel = ModelManager.inst.roleModel;
		this.isAttention = (bool)userModel.records ["follow_sign"];
		this.isInvite = (bool)userModel.records ["invite_sign"];//+		["follow_sign"]	false	System.Collections.DictionaryEntry
        this.isVoice = (bool)userModel.records["chat_voice_sign"];
        this.isDreamAttetion = (bool)userModel.records["request_sign"];//求关注
        //this.isDreamAttetion = true;
        FindObject ();
		InitData ();
	}

	private void InitData ()
	{
		language = (object[])DataManager.inst.systemSimple ["language"];
		arr = new List<object[]> ();
		arrValue = new List<string> ();
		foreach (object[] v in language)
		{
			arr.Add (v);
		}
		foreach (object[] v in arr)
		{
			arrValue.Add (v [1].ToString ());
		}
		if (PlatForm.inst.language != "")
		{
			foreach (object[] v in arr)
			{
				if (v [0].ToString ().Equals (PlatForm.inst.language))
				{
					gCombox.selectedIndex = arr.IndexOf (v);
				}
			}
		}
		gCombox.items = arrValue.ToArray ();
	}

	private void FindObject ()
	{
		this.GetChild ("n2").text = Tools.GetMessageById ("13092");
		this.GetChild ("n1").text = Tools.GetMessageById ("13093");
		this.GetChild ("n5").text = Tools.GetMessageById ("13094");
		this.GetChild ("n4").text = Tools.GetMessageById ("13095");
		this.GetChild ("n3").text = Tools.GetMessageById ("13096");
		this.GetChild ("n15").text = Tools.GetMessageById ("13109");
		this.GetChild ("n62").text = Tools.GetMessageById ("13147")+"：";
        string start = Tools.GetMessageById ("13116");
		string close = Tools.GetMessageById ("13117");
		gCombox = this.GetChild ("n14").asComboBox;
		gCombox.onChanged.Add (() =>
		{
			foreach (object[] v in arr)
			{
				if (v [1].ToString ().Equals (gCombox.text))
				{
					PlatForm.inst.SwitchLanguage (v [0].ToString ());
					roleModel.uids.Clear ();
				}
                   
			}
            
		});
        //音乐
		music = this.GetChild ("n56").asButton;
		//music.GetChild ("n4").asTextField.text = start;
		//music.GetChild ("n5").asTextField.text = close;
		if (SoundManager.inst.isMusic)
			music.selected = true;
		else
			music.selected = false;
        music.onClick.Add (() =>
		{
            if(music.selected) {
                SoundManager.inst.SetMusic(true);
                userModel.isBGM = true;
            } else {
                SoundManager.inst.SetMusic(false);
                userModel.isBGM = false;
            }
		});

        //音效
		sound = this.GetChild ("n57").asButton;
		//sound.GetChild ("n4").asTextField.text = start;
		//sound.GetChild ("n5").asTextField.text = close;
//		if (SoundManager.inst.isSound)
			sound.selected = SoundManager.inst.isSound;
//		else
//			sound.selected = false;

		sound.onClick.Add (() =>
		{
            if(sound.selected) {
                SoundManager.inst.SetSound(true);
                userModel.isSound = true;
            } else {
                SoundManager.inst.SetSound(false);
                userModel.isSound = false;
            }
        });
        //声音
		voice = this.GetChild ("n58").asButton;
		//voice.GetChild ("n4").asTextField.text = start;
		//voice.GetChild ("n5").asTextField.text = close;
		if (isVoice)
			voice.selected = true;
		else
			voice.selected = false;

		voice.onClick.Add (() =>
		{
			if (voice.selected)
				SetVoice (true);
			else
				SetVoice (false);
		});

        //邀请
		request = this.GetChild ("n59").asButton;
		//request.GetChild ("n4").asTextField.text = start;
		//request.GetChild ("n5").asTextField.text = close;
		if (isInvite)
			request.selected = true;
		else
			request.selected = false;

		request.onClick.Add (() =>
		{
			if (request.selected)
				SetInvite (true);
			else
				SetInvite (false);
		});


        //关注
		attention = this.GetChild ("n60").asButton;
		//attention.GetChild ("n4").asTextField.text = start;
		//attention.GetChild ("n5").asTextField.text = Tools.GetMessageById ("13074");
        
		if (isAttention)
			attention.selected = true;
		else
			attention.selected = false;

		attention.onClick.Add (() =>
		{
			if (attention.selected)
				SetAttention (true);
			else
				SetAttention (false);
		});

        //求关注
        dream_attention = this.GetChild("n63").asButton;
        //attention.GetChild ("n4").asTextField.text = start;
        //attention.GetChild ("n5").asTextField.text = Tools.GetMessageById ("13074");

        if (isDreamAttetion)
            dream_attention.selected = true;
        else
            dream_attention.selected = false;

        dream_attention.onClick.Add(() =>
        {
            if (dream_attention.selected)
                SetDreamAttention(true);
            else
                SetDreamAttention(false);
        });
    }

    

    private GTextField GetText (string id)
	{
		return this.GetChild (id).asCom.GetChild ("n1").asTextField;
	}

	private GButton GetCheck (string id)
	{
		return this.GetChild (id).asCom.GetChild ("n0").asButton;
	}

	public void SetVoice (bool value)
	{
        Dictionary<string, object> dc = new Dictionary<string, object>();
        dc["follow_sign"] = null;
        dc["invite_sign"] = null;
        dc["chat_voice_sign"] = value;
        dc["request_sign"] = null;
        NetHttp.inst.Send(NetBase.HTTP_SETFOLLOW, dc, (VoHttp vo) =>
        {
            if ((bool)vo.data == true)
            {
                userModel.records["chat_voice_sign"] = value;
            }
        });
    }

	public void SetInvite (bool value)
	{
		Dictionary<string, object> dc = new Dictionary<string, object> ();
		dc ["follow_sign"] = null;
		dc ["invite_sign"] = value;
        dc["chat_voice_sign"] = null;
        dc["request_sign"] = null;

        NetHttp.inst.Send (NetBase.HTTP_SETFOLLOW, dc, (VoHttp vo) =>
		{
			if ((bool)vo.data == true)
			{
				userModel.records ["invite_sign"] = value;
			}
		});

	}

	public void SetAttention (bool value)
	{
		Dictionary<string, object> dc = new Dictionary<string, object> ();
		dc ["follow_sign"] = value;
		dc ["invite_sign"] = null;
        dc["chat_voice_sign"] = null;
        dc["request_sign"] = null; 

        NetHttp.inst.Send (NetBase.HTTP_SETFOLLOW, dc, (VoHttp vo) =>
		{
			if ((bool)vo.data == true)
			{
				userModel.records ["follow_sign"] = value;
			}
		});

        
	}
    private void SetDreamAttention(bool value)
    {
        Dictionary<string, object> dc = new Dictionary<string, object>();
        dc["follow_sign"] = null;
        dc["invite_sign"] = null;
        dc["chat_voice_sign"] = null;
        dc["request_sign"] = value;

        NetHttp.inst.Send(NetBase.HTTP_SETFOLLOW, dc, (VoHttp vo) =>
        {
            if ((bool)vo.data == true)
            {
                userModel.records["request_sign"] = value;
            }
        });
    }
}
