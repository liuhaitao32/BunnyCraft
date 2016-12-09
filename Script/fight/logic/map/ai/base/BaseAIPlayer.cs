using System;

///包含单个AI玩家引用的基类
public class BaseAIPlayer :BaseAI {
	///AI玩家
	protected AIPlayer _aiPlayer;
	public AIPlayer aiPlayer{get{return _aiPlayer;}}

	public BaseAIPlayer(AIPlayer aiPlayer):base(aiPlayer.map) 
	{
		this._aiPlayer = aiPlayer;
	}
}


