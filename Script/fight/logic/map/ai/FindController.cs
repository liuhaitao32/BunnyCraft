using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

///发现控制器
public class FindController:BaseAIPlayer {
	public List<PlayerEntity> findEnemyPlayerList;
	public List<PersonEntity> findEnemyPersonList;
	public List<PlayerEntity> findAllyPlayerList;
	public List<PersonEntity> findAllyPersonList;
	public List<BulletEntity> findBulletList;
	public List<BeanEntity> findBeanList;

	///发现目标的雷达半径形状
	private GeomBase _radarShape;

	///发现目标的检测CD
	public double findCD;

	public FindController (AIPlayer aiPlayer):base(aiPlayer) 
	{
		this.init ();
	}
	public void init () {
		this.findEnemyPlayerList = new List<PlayerEntity>();
		this.findEnemyPersonList = new List<PersonEntity>();
		this.findAllyPlayerList = new List<PlayerEntity>();
		this.findAllyPersonList = new List<PersonEntity>();
		this.findBulletList = new List<BulletEntity>();
		this.findBeanList = new List<BeanEntity>();

//		Collision.createGeom(this, new object[] { (int)this.getProperty(ConfigConstant.PROPERTY_ATTRACT) 
		this._radarShape = Collision.createGeom(this._aiPlayer.player, ( new object[] { 1 } ));
		this.reset ();
	}
	///复活后重开
	public void reset(){
		this.resetFindCD (0.1);
	}
	///随机下一个搜索CD
	private void resetFindCD(double rate = 1){
		this.findCD = AIConstant.FIND_CD / (this._aiPlayer.IQ + this.getRandom()*0.5) * rate;
	}
	public void update(){
		this.findCD -= ConfigConstant.MAP_ACT_TIME_S;
		if (this.findCD <= 0)
			this.checkFind ();
	}
	///查找附近的各个目标
	public void checkFind(){
		this.findEnemyPlayerList.Clear();
		this.findEnemyPersonList.Clear();
		this.findAllyPlayerList.Clear();
		this.findAllyPersonList.Clear();
		this.findBulletList.Clear();
		this.findBeanList.Clear();
		this._radarShape.radius = (int)this._aiPlayer.player.getProperty (ConfigConstant.PROPERTY_RADAR);
		this._radarShape.position.copy (this._aiPlayer.player.position);

        //this._map.getFightEntitysByRange(this._radarShape,new List<int> { ConfigConstant.ENTITY_PLAYER, ConfigConstant.ENTITY_CALL}, null, -1);
        //TODO:这里可能以后要添加取出来一次格子 然后用这个格子找对应的entity；
        this.findEnemyPlayerList = this._map.findPlayer(this._radarShape, 1, -1, false);
        this.findEnemyPersonList = this._map.getPerson(this._radarShape, 1, -1, true, false);
		if (this._map.mapData.isTeam) {
            this.findAllyPlayerList = this._map.findPlayer(this._radarShape, -1, 0, false);
            this.findAllyPersonList = this._map.getPerson(this._radarShape, -1, 0, true, false);
		}
        this.findBulletList = this._map.getFightEntitysByRange(this._radarShape, new List<int> { ConfigConstant.ENTITY_BULLET }, FindEntityManager.getEnemyTeamFilter, 0, false).ConvertAll<BulletEntity>((e) => { return e as BulletEntity; });
        this.findBeanList = this._map.getFightEntitysByRange(this._radarShape, new List<int> { ConfigConstant.ENTITY_LOOP_BEAN, ConfigConstant.ENTITY_PRICE_BEAN }, null, 0, false).ConvertAll<BeanEntity>((e) => { return e as BeanEntity; });

		PlayerEntity player;
		int i,len;
		len = this.findEnemyPlayerList.Count;
		for (i = 0; i < len; i++) {
			player = this.findEnemyPlayerList [i];
			//对这个敌对玩家积累仇恨
			this._aiPlayer.assessController.findEnemyPlayer (player.uid);
		}
		len = this.findAllyPlayerList.Count;
		for (i = 0; i < len; i++) {
			player = this.findAllyPlayerList [i];
			this._aiPlayer.assessController.findAllyPlayer (player.uid);
		}

		this.resetFindCD();
		//重新索敌后，决策CD剧减，AI越高，CD减少越明显
		this._aiPlayer.tacticsController.tacticsCD *= 0.9-this._aiPlayer.IQ*0.8;
	}
}
