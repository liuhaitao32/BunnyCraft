using System;
using UnityEngine;
using System.Collections.Generic;

public class NetBase
{
	//socket
	public const string SOCKET_LOGIN = "login";
	public const string SOCKET_CHAT = "chat_guild";
	public const string SOCKET_APPLEJOINGUILD = "apply_join_guild";
	public const string SOCKET_GUILDJOIN = "guild_join";
	public const string SOCKET_GUILDMODIFY = "guild_modify";
	public const string SOCKET_GUILDEXIT = "guild_exit";
	public const string SOCKET_SENDGUILDREDBAG = "send_guild_redbag";
	public const string SOCKET_REQUIREGUILDSUPPORT = "require_guild_support";
	public const string SOCKET_SENDGUILDSUPPORT = "send_guild_support";
	public const string SOCKET_STATUS = "get_users_status";
	public const string SOCKET_MAILSTATUS = "get_mail_status";
	public const string SOCKET_NOTICE = "notice";
	public const string SOCKET_PING = "ping";
	public const string SOCKET_SHOWMSG = "send_msg";
	public const string SOCKET_GRABGUILDREDBAG = "grab_guild_redbag";
	public const string SOCKET_GUILDFIGHTSHARE = "guild_fight_share";

	public const string SOCKET_GETRESUILT = "get_result_match";
	public const string SOCKET_GETRESUILTPUSH = "get_result_match_push";
	public const string SOCKET_GETRESUILTTEAM = "get_result_match_team";
	public const string SOCKET_GETRESUILTTEAMPUSH = "get_result_match_team_push";
	public const string SOCKET_GETRESULTFREEMATCH = "get_result_free_match";
	public const string SOCKET_GETRESULTFREEMATCHPUSH = "get_result_free_match_push";
    //fight 排位
	public const string SOCKET_NEWTEAM = "new_team";
	public const string SOCKET_KILLTEAM = "kill_team";
	public const string SOCKET_KILLTEAMPUSH = "kill_team_push";
	public const string SOCKET_GETINVITEUSERS = "get_invite_users";
	public const string SOCKET_STARTMATCH = "start_match";
	public const string SOCKET_STARTMATCHPUSH = "start_match_push";
	public const string SOCKET_CANCELMATCH = "cancel_match";
	public const string SOCKET_CANCELMATCHPUSH = "cancel_match_push";
	public const string SOCKET_SENDINVITE = "send_invite";
	public const string SOCKET_TEAMMATCH = "team_match";
	public const string SOCKET_QUITTEAM = "quit_team";
	public const string SOCKET_QUITTEAMPUSH = "quit_team_push";
	public const string SOCKET_GETINVITE = "get_invite";
	public const string SOCKET_ACCEPTINVITE = "accept_invite";
	public const string SOCKET_ACCEPTINVITEOTHER = "accept_invite_other";
	//匹配
	public const string SOCKET_MATCHJOIN = "match_join";
	public const string SOCKET_MATCHQUIT = "match_quit";
	public const string SOCKET_GETRESULTMATCH = "get_result_match";
	//自定义
	public const string SOCKET_NEWROOM = "new_room";
	public const string SOCKET_FREESENDINVITE = "free_send_invite";
	public const string SOCKET_FREEGETINVITE = "free_get_invite";
	public const string SOCKET_FREEACCEPTINVITE = "free_accept_invite";
	public const string SOCKET_ADDUSER = "add_user";
	public const string SOCKET_QUIT = "quit";
	public const string SOCKET_FREEADDBOT = "free_add_bot";
	public const string SOCKET_REMOVEBOT = "remove_bot";
	public const string SOCKET_REMOVEUSER = "remove_user";
	public const string SOCKET_CHANGEMODE = "change_mode";
	public const string SOCKET_CHANGEMODEPUSH = "change_mode_push";
	public const string SOCKET_CHANGEPOS = "change_pos";
	public const string SOCKET_CHANGEPOSPUSH = "change_pos_push";
	public const string SOCKET_KILLROOM = "kill_room";
	public const string SOCKET_KILLROOMPUSH = "kill_room_push";
	public const string SOCKET_FREESTARTMATCH = "free_start_match";
	public const string SOCKET_FREESTARTMATCHPUSH = "free_start_match_push";
	public const string SOCKET_FREECHAT = "free_chat";
	public const string SOCKET_FREECHATPUSH = "free_chat_push";
	public const string SOCKET_MATCHTEAMCHAT = "match_team_chat";
	public const string SOCKET_MATCHTEAMCHATPUSH = "match_team_chat_push";

	//http
	public const string HTTP_CONFIG = "sys.get_config";
	public const string HTTP_CHANGELAN = "user.change_lan";
	public const string HTTP_LOGIN = "user.login";
	public const string HTTP_TEST_COIN = "user.test_coin";
	public const string HTTP_DOGUIDE = "user.do_guide";
	//notice_key
	public const string HTTP_DEL_NOTICE = "user.del_notice";
	//chat
	public const string HTTP_CHATS = "user.guild_chat_init_list";
	public const string HTTP_CHAT = "user.chat_guild";
	public const string HTTP_REQUIREGUILDSUPPORT = "user.require_guild_support";
	public const string HTPP_SENDGUILDSUPPORT = "user.send_guild_support";

	public const string HTTP_VALIDATE = "user.validate_sign";
	public const string HTTP_REGIST = "user.tel_bind";
	public const string HTTP_REGIST_WX = "user.wx_bind";//微信绑定
	public const string HTTP_REGIST_QQ = "user.qq_bind";//QQ绑定
	public const string HTTP_REMOVETEL = "user.unbind_tel_num";
	public const string HTTP_GETBACKSIGN = "user.get_back_sign";
	public const string HTTP_CHANGEGETBACKPWD = "user.change_getback_pwd";
	public const string HTTP_CHANGEPWD = "user.change_pwd";
	public const string HTTP_UPLOAD = "user.upload_head";
	public const string HTTP_CHANGE_NAME = "user.change_uname";
	public const string HTTP_CHANGE_USERINFO = "user.change_userinfo";
	public const string HTTP_GET_REDBAG = "user.get_redbag";
	public const string HTTP_SEND_REDBAG = "user.send_redbag";
	public const string HTTP_UPDATE_PHOTO = "user.check_head";
	public const string HTTP_CHOOSE_HEAD = "user.choose_head";
	public const string HTTP_DEL_PHPTO = "user.delete_head";
	public const string HTTP_GET_REDBAG_MSG = "user.get_redbag_msg";
	public const string HTTP_FOLLOW = "user.friend_follow";
	public const string HTTP_UNFOLLOW = "user.friend_unfollow";
	public const string HTTP_GETFRIEND = "user.get_friend_follow";
	public const string HTTP_SHIELDING = "user.friend_black";
	public const string HTTP_CANCLESHIELDING = "user.friend_unblack";
	public const string HTTP_REFRESH_DAILY_BOX = "user.refresh_daily_box";
	public const string HTTP_ADDMSG = "user.add_msg_board";
	public const string HTTP_GETMSG = "user.get_msg_board";
	public const string HTTP_FRIENDLIKE = "user.friend_like";
	public const string HTTP_GETFRIENDLISK = "user.get_friend_like";
	public const string HTTP_FUSERGET = "user.user_info";
	public const string HTTP_GET_DAILY_BOX = "user.get_daily_box";
	public const string HTTP_GET_RANDOM_BOX = "user.get_random_box";
	public const string HTTP_ALTER_CARD_GROUP_INDEX = "user.alter_card_group_index";
	public const string HTTP_COIN_BUY_GOLD = "user.coin_buy_gold";
	public const string HTTP_ALTER_CARD_GROUP = "user.alter_card_group";
	public const string HTTP_UNLOCK_EXPLORE = "user.unlock_explore";
	public const string HTTP_OPEN_EXPLORE = "user.open_explore";
	public const string HTTP_REFRESH_EXPLORE = "user.refresh_explore";
	public const string HTTP_CARD_LVUP = "user.card_lvup";
	public const string HTTP_GETBALCKLIST = "user.get_friend_black";
	public const string HTTP_GETATTENTION = "user.get_friend_follow";
	public const string HTTP_GETFANS = "user.get_friend_fans";
	public const string HTTP_REDBAG = "user.get_friend_redbag";
	public const string HTTP_CHANGE_STORY = "user.change_story";
	public const string HTTP_GETNEAR = "user.get_friend_near";
	public const string HTTP_GETTASTE = "user.get_friend_interest";
	public const string HTTP_SENDGUILDREDBAG = "user.send_guild_redbag";
	public const string HTTP_GRABGUILDREDBAG = "user.grab_guild_redbag";
	public const string HTTP_GETEFFORT = "user.get_effort";
	public const string HTTP_GETEFFORTREWARD = "user.get_effort_reward";
	public const string HTTP_READ_CARD = "user.read_card";
	public const string HTTP_SETFOLLOW = "user.set_follow_sign";
	public const string HTTP_GET_SEASON_DATA = "user.get_season_data";//检查赛季是否结束
    //感兴趣的人
	public const string HTTP_SEAT = "user.refresh_seat";
	public const string HTTP_GETFRIENS = "user.find_user";
	public const string HTTP_USE_BODY = "user.use_body";
	public const string HTTP_BUY_BODY = "user.buy_body";
	public const string HTTP_GUILD_CREATE = "user.guild_create";
	public const string HTTP_GUILD_INDEX = "user.guild_index";
	public const string HTTP_GUILD_LOCATION_RANK = "user.guild_location_rank";
	public const string HTTP_GUILD_INFO = "user.guild_info";
	public const string HTTP_GUILD_OVER = "user.guild_over";
	public const string HTTP_GUILD_APPLY_MEMBER = "user.guild_apply_member";
	public const string HTTP_GUILD_CANCEL_MEMBER = "user.guild_cancel_member";
	public const string HTTP_GUILD_MODIFY = "user.guild_modify";
	public const string HTTP_GUILD_EXIT = "user.guild_exit";
	public const string HTTP_GUILD_FIND = "user.guild_find";
	public const string HTTP_GUILD_SETUP_CHANGE = "user.guild_setup_change";
	public const string HTTP_GUILD_JOIN_SETUP = "user.guild_join_setup";
	public const string HTTP_GUILD_RANK = "user.guild_rank";
	public const string HTTP_GUILD_RECOMMEND = "user.guild_recommend";
	public const string HTTP_GUILD_FILTER = "user.guild_filter";
	public const string HTTP_GETNOTICE = "user.get_notice";
	public const string HTTP_GUILD_JOIN = "user.guild_join";
	public const string HTTP_GIFT = "user.get_gift_msg";
	public const string HTTP_ISOPEN = "user.read_gift_msg";
	public const string HTTP_ADDGIFT = "user.accept_gift_msg";
	public const string HTTP_GET_FIGHT_REWARD = "user.get_fight_reward";
	public const string HTTP_GET_RANK_REWARD = "user.get_rank_reward";
	public const string HTTP_ACTIVATION = "user.get_reward_by_code";
	public const string HTTP_ACTIVATIONAC = "user.get_weixin_gift";
	public const string HTTP_RANK_LIKE = "user.get_user_rank";
	public const string HTTP_SHARE = "user.share_success";
	public const string HTTP_GETWIN = "user.invite_back";
	public const string HTTP_SEND_BUG_MSG = "user.send_bug_msg";
	public const string HTTP_GET_BUG_MSG = "user.get_bug_msg";
	public const string HTTP_GET_FIGHT_SEASON_DATA = "user.get_fight_season_data";
	public const string HTTP_GET_FIGHT_LOGS = "user.get_fight_logs";
	public const string HTTP_GET_FIGHT_DATA = "user.get_fight_data";
    
	public const string HTTP_GET_HISTORY_SEASONS = "user.get_history_seasons";
	public const string HTTP_SHAREFIGHT= "user.share_fight";

    public NetBase ()
	{     
	}

}