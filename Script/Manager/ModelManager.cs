using System;

public class ModelManager
{
	private static ModelManager instance;

	public ModelGame gameModel;
	public ModelAlert alertModel;
	public ModelUser userModel;
	public ModelRole roleModel;
	public ModelGuild guildModel;
	public ModelChat chatModel;
	public ModelCard cardModel;
	public ModelMail mailModel;
	public ModelFight fightModel;
	public ModelRank rankModel;
	public ModelGuide guideModel;
	public ModelShare shareModel;

	public ModelManager ()
	{
	}

	public static ModelManager inst
	{
		get
		{
			if (instance == null)
				instance = new ModelManager ();
			return instance;
		}
	}

	public void Register ()
	{
		gameModel = new ModelGame ();
		alertModel = new ModelAlert ();
		userModel = new ModelUser ();
		roleModel = new ModelRole ();
		guildModel = new ModelGuild ();
		chatModel = new ModelChat ();
		cardModel = new ModelCard ();
		mailModel = new ModelMail ();
		fightModel = new ModelFight ();
		rankModel = new ModelRank ();
		guideModel = new ModelGuide ();
		shareModel = new ModelShare ();
	}

	public void Unregister ()
	{
		
	}

	public void Clear ()
	{
		chatModel.Clear ();
		roleModel.clearRole ();
        roleModel.ClearFightData();
        fightModel.Clear ();
		fightModel.ClearInvite();
	}
}