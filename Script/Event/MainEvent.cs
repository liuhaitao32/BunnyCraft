using System;

public class MainEvent
{
	public const string GAME_START = "game_start";
	public const string GAME_CLOSE = "game_close";
	public const string RESTART_GAME = "restart_game";
	public const string RELOGIN_GAME = "relogin_game";
	public const string CLOSE_MASK = "close_mask";
    public const string SHOW_USER = "show_user";
	public const string SHOW_FIGHTDETAIL = "show_fightdetail";
	public const string SOCKET_ERROR = "socket_error";
	public const string SOCKET_CLOSE = "socket_close";
	public const string SOCKET_ERROR_DISCONNECT = "socket_error_disconnect";
	public const string UPDATE_LOG = "update_log";
	//uid
	public const string ROLE_UPDATE = "role_update";
	public const string ROLE_POSITION = "role_position";
	public const string PHOTO_SEND = "photo_send";
	//用户数据更新
	public const string USER_UPDATE = "user_update";
	public const string USER_UPDATE_UI = "user_update_ui";
	public const string CARD_CHANGE = "card_change";

	public const string EXPLORE_UNLOCK = "explore_unlock";
	public const string EXPLORE_GIFT = "explore_gift";
	public const string MAIN_EXPLORE = "main_explore";

	public const string RED_GIFT = "red_gift";
	public const string DAY_BOX_BUY = "day_box_buy";
	public const string DAY_BOX_FRUSH = "day_box_frush";

	public const string CHANGE_GUILD_ICON = "change_guild_icon";
	public const string CHANGE_GUILD_NAME = "change_guild_name";
	public const string GUILD_ESC = "guild_esc";
	public const string EVENTSHARE = "event_share";
	public const string WEIXIN_UPDATE = "weixint_update";
	public const string ATTENTION_UPDATE = "attention_update";
	public const string GIFT_UPDATE = "gift_update";
	public const string UPDATA_USER = "updata_user";
	public const string JUMP_COINGOLDEXPGET = "jump_coingoldexpget";
	public const string JUMP_ELSCORE = "jump_elscore";
	public const string JUMP_OVER = "jump_over";
	public const string COLOR_CHANGE = "color_change";
	public const string JUMPEL_OVER = "jumpel_over";
	public const string CARD_LEVELUP = "card_levelup";
	public const string LEVEL_UP_USER = "level_up_user";
	//
	public const string SHARE_DATA_EVENT = "share_data_event";

	//fight
	public const string FIGHT_RESULT = "fight_result";
	public const string START_FIGHT = "start_fight";
	public const string CLOSE_FIGHT = "close_fight";
	public const string FIGHT_DATA_END = "fight_data_end";

	//red
	public const string RED_UPDATE = "red_update";
	public const string RED_CHATUPDATE = "red_chatupdate";
    //socket dispatch
    public const string CHAT_SEND = "chat_send";
	public const string CHAT_APPLEJOINGUILD = "chat_applejoinguild";
	public const string CHAT_GUILDJOIN = "chat_guildjoin";
	public const string CHAT_GUILDMODIFY = "chat_guildmodify";
	public const string CHAT_SENDGUILDREDBAG = "chat_sendguildredbag";
	public const string CHAT_REQUIREGUILDSUPPORT = "chat_requireguildsupport";
	public const string CHAT_SENDGUILDSUPPORT = "chat_sendguildsupport";
	public const string CHAT_SENDREQUESTCARD = "chat_sendrequestcard";
	public const string CHAT_SENDREDBAG = "chat_sendredbag";
	public const string CHAT_FIGHTSHARE = "chat_fightshare";
	public const string CHAT_ISSENDREDBAG = "chat_issendredbag";
	public const string GONGGAO_CHANGE = "gonggao_change";
	public const string GUILDICON_CHANGE = "guildIcon_change";
	public const string GUILDDIZHI_CHANGE = "guildDizhi_change";

	public const string FIGHT_INIT_COMPLETE = "fightInitComplete";
	public const string FIGHT_QUIT = "fight_quit";
	public const string FIGHT_ING = "fight_ing";

	public const string MICRO_ADD = "micro_add";

	public const string VIEWCOLOR = "view_olor";
	//
	public const string GUIDE_UPDATE_OK = "guide_update_ok";
	public const string GUIDE_UPDATE_OVER = "guide_update_over";

	public object target;
	public string name;
	public object data;

	public MainEvent (string name, object data = null)
	{
		this.name = name;
		this.data = data;
	}

}