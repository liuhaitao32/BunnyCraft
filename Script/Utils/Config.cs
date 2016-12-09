using System;
using System.Collections;
using System.Collections.Generic;
public class Config
{

	//	public const string PACKAGE_PATH = "Assets/Bundle/";
	//	public const int APP_VERSION = 4;

	//fight
	public const string LOSE = "lose";
	public const string WIN = "win";

	//reg
	public const string REG_NUMORABC = "[a-zA-Z0-9]";

	//asset
	public const string ASSET_CARD = "card";
	public const string ASSET_COIN = "coin";
	public const string ASSET_GOLD = "gold";
	public const string ASSET_EXP = "exp";
	//	public const string ASSET_ACE = "ace";
	public const string ASSET_RANKSCORE = "rank_score";
	public const string ASSET_ELSCORE = "el_score";
	public const string ASSET_REDBAGCOIN = "redbag_coin";
	public const string ASSET_BODY = "body";
	//宝箱
	public const string ASSET_AWARD = "award";
	//	public const string ASSET_RANKSCORE = "rankscore";

	public const string ASSET_URL_COIN = "Image2:n_icon_zs1";
	public const string ASSET_URL_GOLD = "Image2:n_icon_xm1";
	public const string ASSET_URL_EXP = "Image2:n_icon_jingyan";
	public const string ASSET_URL_RANKSCORE = "Image2:n_icon_jf1";
	public const string ASSET_URL_ELSCORE = "Image2:n_icon_jf1";
	public const string ASSET_URL_REDBAGCOIN = "Image2:n_icon_hongbaoquan";
	public const string ASSET_URL_AWARD = "icon_baoxiang";
	public const string ASSET_URL_CARD = "Image2:n_icon_kp1";
	//	public const string ASSET_URL_ACE = "Image:icon_chengjiujinyan";

	//Color
	public const uint COLOR_RED = 0xff0000;

	//Sound
	public const string SOUND_BTN = "Base:btn";
	public const string SOUND_MAIN = "Base:main";
	public const string SOUND_LOGIN = "Base:login";
    public const string SOUND_MISSIONREWARD = "Base:missionreward";
    public const string SOUND_LIKE = "Base:like";
    public const string SOUND_CARDLVUP = "Base:CardLvUp";
    public const string SOUND_SCIENCELVUP = "Base:scienceLvUp";
    public const string SOUND_SHOWCARD = "Base:ShowCard";
    public const string SOUND_OPENBOX = "Base:OpenBox";

    //tip
    public const string TOUCH_COMGOLD = "ComGold";
	public const string TOUCH_COMCOIN = "ComCoin";
	public const string TOUCH_COMEXP = "ComExp";
	public const string TOUCH_COMELSCORE = "ComElScore";
	public const string TOUCH_PROGRESSBAR = "GProgressBar";
	public const string TOUCH_RANKSCORE = "GComponent";
    
	//effect
	public const string EFFECT_GIFT = "Gift/gift";
	public const string EFFECT_BUILD01 = "Build01/build01";
	public const string EFFECT_GETBOX = "Getbox/getbox";
	public const string EFFECT_BOX100 = "Box100/box100";
	public const string EFFECT_BOX001 = "Box001/box001";
	public const string EFFECT_BOX002 = "Box002/box002";
	public const string EFFECT_BOX003 = "Box003/box003";
	public const string EFFECT_BOX004 = "Box004/box004";
	public const string EFFECT_BOX005 = "Box005/box005";
	public const string EFFECT_BOX006 = "Box006/box006";
	public const string EFFECT_BOX007 = "Box007/box007";
	public const string EFFECT_BOX101 = "Box101/box101";
	public const string EFFECT_RPBOX001 = "Rpbox001/rpbox001";
	public const string EFFECT_RPBOX002 = "Rpbox002/rpbox002";
	public const string EFFECT_WIN = "Win/win";
	public const string EFFECT_NO1 = "No1/no1";
	public const string EFFECT_NO2 = "No2/no2";
	public const string EFFECT_NO3 = "No3/no3";
	public const string EFFECT_FINISH = "Finish/finish";
	public const string EFFECT_LIGHT = "Card_light/card_light";
	public const string EFFECT_LOSE = "Lose/lose";
	//	public const string EFFECT_RPBOX003 = "Rpbox001/box008";
	public const string EFFECT_EGG103 = "Egg103/egg103";
	public const string EFFECT_EGG105 = "Egg105/egg105";
	public const string EFFECT_COUNTDOWN = "Countdown/countdown";
	public const string EFFECT_ELV_UP = "Elv_up/elv_up";
	public const string EFFECT_BORN = "Born/born";
	public const string EFFECT_HAPPY = "Happy/happy";
	public const string EFFECT_BACKLIGHT = "Backlight/backlight";
	public const string EFFECT_GET_LOW = "Getlow/getlow";
	public const string EFFECT_BG1 = "Background/obj01";
	public const string EFFECT_BG2 = "Background/obj02";
	public const string EFFECT_BG3 = "Background/obj03";
	public const string EFFECT_BG4 = "Background/obj04";
    public const string EFFECT_HONGBAO = "N_icon_hongbao/n_icon_hongbao";
    public const string EFFECT_LOADING = "Icon_loading/icon_loading";
    public const string EFFECT_CARDLVUP = "Card_lvup/card_lvup";

    public const string EFFECT_DENGJI = "Dengji4/dengji4";
	public const string EFFECT_K04 = "K04/k04";
	public const string EFFECT_MAILBOX = "Mail_box/mail_box";
	public const string EFFECT_HAND = "Hand/hand";
	public const string EFFECT_UNLOCKSKIN = "Unlockskin/unlockskin";
	public const string EFFECT_BIGEARTH = "Bigearth/bigearth";

	public const string SHADER_WHITE = "Shaders/WhiteShader";
	public const string SHADER_COLORE = "Shaders/DiscolorShader";
	//UI
	public const string COM_MASK = "Base:ComMask";
	public const string COM_ALERT = "Base:ComAlert";
	public const string COM_ALERTLIKE = "Base:ComAlertLike";
	public const string COM_TEXT = "Base:ComText";
	public const string COM_NUMERIC = "Base:ComNumeric";
	public const string COM_CARD = "Base:ComCard";
	public const string COM_REWARD = "Base:ComReward";
	public const string COM_ICON = "Base:ComIcon";
	public const string COM_LOAD = "Base:ComLoad";
	public const string COM_LAN = "Base:ComLan";
	public const string COM_BIGICON = "Base:ComBigIcon";
	public const string COM_BINDBIGICON = "Base:BindBigIcon";
	public const string COM_PROGRESSSIDE = "Base:ProgressSide";
	public const string COM_PROGRESSSIDE1= "Base:ProgressSide1";
	public const string COM_EFFECTICON = "Base:ComEffectIcon";
	public const string COM_GOLDCOINEXP = "Base:ComCoinGoldExp";
	public const string COM_GUIDE = "Base:ComGuide";
	public const string COM_RED1 = "Base:ComRed1";
	public const string COM_RED2 = "Base:ComRed2";
	public const string COM_TIP = "Base:ComTip";
	public const string COM_MESSAGE = "Base:ComMessage";
	public const string COM_SCENEMASK = "Base:ComSceneMask";
	public const string COM_TITLE = "Base:ComTitle";
	public const string COM_POPTEXT = "Base:ComPopText";
	public const string COM_HURT = "Base:ComHurt";


	//Scene
	public const string SCENE_FRIENDROOT = "ViewFriend:SceneFriendRoot";
	public const string SCENE_ROLEROOT = "ViewRoleInfo:SceneRoleRoot";
	public const string SCENE_MAIN = "ViewMain:SceneMain";
	public const string SCENE_SHIP = "ViewShip:SceneShip";
	public const string SCENE_SHOP = "ViewShop:SceneShop";
	public const string SCENE_GUILDMAIN = "ViewGuild:SceneGuildMain";
	
	public const string SCENE_FIGTHTBOX = "ViewStatement:SceneFightBoxStatement";
	public const string SCENE_FIGTHTDATA = "ViewStatement:SceneFightDataStatement";
	public const string SCENE_FIGTHTDATA2 = "ViewStatement:SceneFightDataStatement2";
	public const string SCENE_FIGTHTUSERDATA = "ViewStatement:SceneUserDataStatemnet";
	public const string SCENE_RANKROOT = "ViewRanking:SceneRankRoot";
    public const string SCENE_SHARE = "ViewFriend:SceneShare";
    public const string SCENE_MAIL = "ViewMail:SceneMail";
    //View
    public const string VIEW_CHAT = "ViewChat:ViewMain";
	public const string VIEW_REDPACKAGE = "ViewRedPackage:ViewRedPackage";
	public const string VIEW_ROBRED = "ViewRedPackage:ViewRodRed";
	public const string VIEW_GIVERED = "ViewRedPackage:ViewGiveRed";
	public const string VIEW_LOGINCARD = "ViewMain:ViewLoginCard";
	public const string VIEW_AD = "ViewMain:ViewAd";
	public const string VIEW_ROLELEFT = "ViewRoleInfo:ViewRoleLeft";
	public const string VIEW_ROLEIDEX = "ViewRoleInfo:ViewRoleIndex";
	public const string VIEW_ROLEPHOTO = "ViewRoleInfo:ViewRolePhoto";
	public const string VIEW_ROLELOVE = "ViewRoleInfo:ViewRoleLove";
	public const string VIEW_ROLEPHOTOHEAD = "ViewRoleInfo:ViewRolePhotoHead";
	public const string VIEW_ROLEGUESTBOOK = "ViewRoleInfo:ViewRoleGuestBook";
	public const string VIEW_USERINFO = "ViewRoleInfo:ViewUserInfo";
	public const string VIEW_REGISTER = "ViewRoleInfo:ViewRegister";
	public const string VIEW_CHANGEPHONE = "ViewRoleInfo:ViewChangePhone";
	public const string VIEW_REMOVEPHONE = "ViewRoleInfo:ViewRemovePhone";
	public const string ITEM_CHATROLEINFO = "ViewRoleInfo:ItemChat0";
	public const string VIEW_CHANGEPASSWORD = "ViewRoleInfo:ViewChangePassword";
	public const string VIEW_CHANGEACCOUNT = "ViewRoleInfo:ViewChangeAccount";
	public const string VIEW_CHANGEACCOUNT2 = "ViewRoleInfo:ViewChangeAccount2";
	public const string VIEW_CHANGEACCOUNT1 = "ViewRoleInfo:ViewChangeAccount1";
    public const string VIEW_ROLEMICRO = "ViewRoleInfo:ViewMicro";
    public const string VIEW_CHANGEACCOUNTICON = "ViewRoleInfo:ViewChangeAccountIcon";
    public const string VIEW_ROLEFIGHT = "ViewRoleInfo:ViewRoleFight";
	public const string VIEW_ROLERECORD = "ViewRoleInfo:ViewRoleRecord";
	public const string VIEW_ROLEVOYAGE = "ViewRoleInfo:ViewRoleVoyage";
	public const string VIEW_ROLEDECK = "ViewRoleInfo:ViewDeck";
	public const string VIEW_GAMESET = "ViewRoleInfo:ViewGameSet";
	public const string VIEW_CHANGENAME = "ViewRoleInfo:ViewChangeName";
	public const string VIEW_EXPLORE = "ViewExplore:ViewExplore";
	public const string VIEW_RED1 = "ViewChat:ViewRed1";
	public const string VIEW_RED2 = "ViewChat:ViewRed2";
	public const string VIEW_BOXITEM = "ViewExplore:ViewBoxItem";
	public const string VIEW_GUILDCHANGEICON = "ViewGuild:ViewGuildChangeIcon";
	public const string VIEW_GUILDCREATE = "ViewGuild:ViewCreateGuild";
	public const string VIEW_GUILDCHANGENAME = "ViewGuild:ViewChangeGuildName";
	public const string VIEW_GUILDSETUP = "ViewGuild:ViewGuildSetUp";
	public const string VIEW_GUILDLIST = "ViewGuild:ViewGuildList";
	public const string VIEW_GUILDINFO = "ViewGuild:ViewGuildInfo";
	public const string VIEW_GUILDSEARCH = "ViewGuild:ViewGuildSearch";
	public const string VIEW_MEMBERSET = "ViewGuild:MemberSet";
	public const string VIEW_GUILDINLIST = "ViewGuild:ViewGuildInList";
	public const string VIEW_GUILDMEMBER = "ViewGuild:ViewGuildMember";

	public const string VIEW_ITEMCADRGET = "ViewShop:ItemCardGet";
	public const string VIEW_ITEMPAY = "ViewShop:ItemPay";
	public const string VIEW_ITEMCHANGECOIN = "ViewShop:ItemChangeCoin";
	public const string VIEW_GETCARDBOX = "ViewShop:ViewGetCardBox";
	public const string VIEW_GETDAYCARDBOX = "ViewShop:ViewGetDayCardBox";
	public const string VIEW_GIFTSHOW = "ViewShop:ViewGiftShow";
	
	public const string VIEW_ATTENTION = "ViewFriend:ViewAttention";
	public const string VIEW_SEARCH = "ViewFriend:ViewSearch";
	public const string VIEW_NEAR = "ViewFriend:ViewNear";
	public const string VIEW_SHAREBTN = "ViewFriend:ViewShareBtn";
	public const string VIEW_TASTE = "ViewFriend:ViewTaste";
	public const string VIEW_SEARCHROOT = "ViewFriend:ViewSearchRoot";
	public const string VIEW_GOLDBUY = "ViewShop:ViewGoldBuy";

	public const string VIEW_GETWIN = "ViewFriend:ViewGetWin";

	public const string VIEW_ITEMSHIP = "ViewShip:ItemShip";
	public const string VIEW_COMSHIPINFO = "ViewShip:ComShipInfo";
	public const string VIEW_ITEMCOLOR = "ViewShip:ViewColor";
	public const string VIEW_REQUEST = "ViewChat:ViewRequest";
	public const string VIEW_ITEMSHIPINFO = "ViewShip:ViewShipInfo";
	public const string VIEW_ITEMSHIPINFO2 = "ViewShip:ViewShipInfo2";
	public const string VIEW_LEVELUP = "ViewShip:ViewLevelUp";
	public const string VIEW_LEVELUSERUP = "ViewShip:ViewUserLevelUp";

	public const string VIEW_NOTICE = "ViewMail:ViewNotice";
	public const string VIEW_GIFT = "ViewMail:ViewGift";
	public const string VIEW_CUSTOM = "ViewMail:ViewCustom";
	public const string VIEW_ADDGIFT = "ViewMail:ViewAddGift";
	public const string VIEW_WEINXINAC = "ViewActivation:ViewWeiXinAc";
	public const string VIEW_ACTIVATION = "ViewActivation:ViewWeiXinAc2";
	public const string VIEW_EFFORT = "ViewEffort:ViewEffort";
	public const string VIEW_EFFORTXX = "ViewEffort:ViewEffortXX";
	public const string VIEW_EXPLOREBOX = "ViewEffort:ViewExploreBox";
	public const string VIEW_CHANGENAMEGUIDE = "ViewMain:ViewChangeName";
	public const string VIEW_FIGTHTDATASHOWMATCH = "ViewStatement:ViewFightDataShowMatch";
	public const string VIEW_FIGTHTDATASHOWRANK = "ViewStatement:ViewFightDataShowRank";


	public const string VIEW_RANK1 = "ViewRanking:ViewRank";
	public const string VIEW_RANK2 = "ViewRanking:ViewRank2";
	
 
	public const string VIEW_FIGTHTDATASHOWMEMBER = "ViewStatement:ViewFightDataShowMember";
	public const string VIEW_FIGTHTDATASHARE = "ViewStatement:ViewFightDataShare";

	//fight
	public const string VIEW_TEAMMATCH = "ViewFight:ViewTeamMatch";
	public const string VIEW_INVITE = "ViewFight:ViewInvite";
	public const string VIEW_FIGHTWORLD = "ViewFight:ViewFightWorld";
	public const string VIEW_FREEMATCH = "ViewFight:ViewFreeMatch";
	public const string VIEW_LOADFIGHT = "ViewFight:ViewLoadFight";
	public const string VIEW_MICRO = "ViewFight:ViewMicro";
	public const string VIEW_TIP = "ViewFight:ViewTip";
	public const string VIEW_INVITEALERT = "ViewFight:ViewInviteAlert";
	public const string VIEW_FIGHTSEASON = "ViewFight:ViewFightSeason";

	//item
	public const string RED_PEOPLE = "ViewRedPackage:RedPeople";
	public const string ITEMEFFORT = "ViewEffort:RanderEffort";
	public const string RED_GIFT = "ViewRedPackage:ItemGift";
	public const string ITEM_CHAT = "ViewChat:ItemChat0";
	public const string ITEM_GUILD = "ViewChat:ItemGuild";
	public const string ITEM_TEXT = "ViewChat:ItemText";
	public const string ITEM_RED1 = "ViewChat:ItemRed1";
	public const string ITEM_RED2 = "ViewChat:ItemRed2";
	public const string ITEM_REQUEST = "ViewChat:ItemRequest";
	public const string ITEM_SHARE = "ViewChat:ItemShare";

	//open
	public const string UNLOCK_HEAD = "0";
	public const string UNLOCK_ASSET = "1";
	public const string UNLOCK_CODE = "2";
	public const string UNLOCK_MAIL = "3";
	public const string UNLOCK_RANK = "4";
	public const string UNLOCK_REDBAG = "5";
	public const string UNLOCK_EFFORT = "6";
	public const string UNLOCK_EXPLORE = "7";
	public const string UNLOCK_CARD = "8";
	public const string UNLOCK_FRIEND = "9";
	public const string UNLOCK_TEAMMATCH = "10";
	public const string UNLOCK_MATCH = "11";
	public const string UNLOCK_FREEMATCH1 = "12";
	public const string UNLOCK_FREEMATCH2 = "13";
	public const string UNLOCK_GUILD = "14";
	public const string UNLOCK_SHOP = "15";
	public const string UNLOCK_MAIL_REWARD = "16";
	public const string UNLOCK_MALL_HELP = "17";
	public const string UNLOCK_SHARD = "18";
	public const string UNLOCK_PAY = "19";
	public const string UNLOCK_GUIDE = "20";
	public const string UNLOCK_NOTICE = "21";
	public const string UNLOCK_TIP = "22";
	public const string UNLOCK_CARDGROUP = "23";
	public const string UNLOCK_WEICHAT = "24"; 
	public const string UNLOCK_FIGHTSHARE = "25"; 
	public const string UNLOCK_PAY_RED = "26"; 
	public const string UNLOCK_PAY_MICRO = "27";
    //
    public static Dictionary<string,string> OffLineTxt = GetValues();

	public static Dictionary<string,string> GetValues(){
		OffLineTxt = new Dictionary<string, string> ();
		OffLineTxt ["cn_14056"] = "确定";
		OffLineTxt ["cn_14057"] = "取消";
		OffLineTxt ["cn_14004"] = "当前网络状态不好，请重新连接";
		//
		OffLineTxt ["en_14056"] = "Ok";
		OffLineTxt ["en_14057"] = "Cancel";
		OffLineTxt ["en_14004"] = "Network can not connect";
		return OffLineTxt;
	}

    public Config ()
	{
	}
		
}
