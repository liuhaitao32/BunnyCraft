using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

///战术管理器。在战略下会考虑若干条战术，战术是一种计划。根据主要目标确定战术后，会根据周边其他情况对战术评分，存在多种战术执行最佳评分的战术
public class TacticsController:BaseAIPlayer {
	protected List<BaseTactic> _tacticList;
	protected List<BaseTactic> _cardTacticList;
	protected List<BaseTactic> _moveTacticList;


	///重定位战术CD
	public double tacticsCD;
	///检测使用卡牌CD
	public double cardCD;


	///躲避的目标，如果该目标仍然阻挡，就继续躲避
	public FightEntity dodgeTarget;

	/// 上一次发现的战术对象列表，如果上次发现了，这次必定发现并重视
	public List<FightEntity> lastTacticTargetList;

	/// 当前决策次数
	private int tacticsCount;

	/// 统一策略后最终目标位置
	public Vector2D moveAimPosition;
	/// 统一策略后最终目标移动控速，默认1
	public double moveAimSpeedScale;
	/// 统一策略后最终目标朝向角度
	public double moveAimAngle;

	///摇杆比例
	public Vector2D steeringV2d;
	/// 上一次摇杆控速
	public double lastSpeedScale;
	/// 上一次摇杆角度
	public double lastAngle;
	/// 上一次摇杆角速度
	public double lastAngleSpeed;

	///模拟玩家操作摇杆的体力，左右摆动，加速减速都消耗体力，当体力不足时变化率将很低
	public double steeringHp;

	///当前位置到预期位置的距离(临时变量，每时间片刷新一次)
	public Vector2D moveDeltaV2d;
	///得到预期到达（距离）目标位置需要的时间片数(临时变量，每时间片刷新一次)
	public double moveTimes;


	public TacticsController(AIPlayer aiPlayer):base(aiPlayer) 
	{
		this.init ();
	}
	public void init(){
		this._tacticList = new List<BaseTactic>();
		this._cardTacticList = new List<BaseTactic>();
		this._moveTacticList = new List<BaseTactic>();

		this.lastSpeedScale = 0;
		this.lastAngle = this._aiPlayer.player.angle;
		this.lastTacticTargetList = new List<FightEntity>();
		this.moveAimPosition = Vector2D.createVector ();
		this.moveDeltaV2d = Vector2D.createVector ();
		this.steeringV2d = Vector2D.createVector ();
		this.reset ();
	}
	///复活后重开
	public void reset(){
		this.tacticsCount = 0;
		this.moveAimPosition.copy(this._aiPlayer.player.position);
		this.steeringV2d.zero ();
		this.moveAimAngle = this._aiPlayer.player.angle;
		this.moveAimSpeedScale = 0;

		this.steeringHp = this._aiPlayer.IQ;
		this.lastAngleSpeed = 0;
		this.dodgeTarget = null;

		//遗忘上一次考虑过的战术
		this.lastTacticTargetList.Clear ();

		this.resetTacticsCD(0.1);
		this.resetCardCD(0.1);
	}
	///随机下一个决策CD
	private void resetTacticsCD(double rate = 1){
		this.tacticsCD = AIConstant.TACTICS_CD / (this._aiPlayer.IQ + this.getRandom()*0.5) * rate;
	}
	///随机下一个出卡CD
	private void resetCardCD(double rate = 1){
		this.cardCD = AIConstant.CARD_CD / (this._aiPlayer.IQ + this.getRandom()) * rate;
	}
		
	public void update(){
		//恢复摇杆体力
		this.steeringHp = Math2.range (this.steeringHp + 0.1*(this._aiPlayer.IQ+1+this._aiPlayer.moodController.nervous), 1+this._aiPlayer.IQ,0);
		this.updateMoveTimes ();
		this.speedUpCD ();

		this.tacticsCD -= ConfigConstant.MAP_ACT_TIME_S;
		if (this.tacticsCD <= 0)
			this.checkTactics ();

		//发出摇杆指令
		this.sendSteeringV2d ();

		this.cardCD -= ConfigConstant.MAP_ACT_TIME_S;
		if (this.cardCD <= 0)
			this.checkUseCard ();


		//以下为视图更新
		this._aiPlayer.showMoveAimPosition ();
	}

	///计算移动目标和当前位置距离，造成需要移动的时间片数
	public void updateMoveTimes(){
//		this.moveAimPosition.setXY (this._aiPlayer.player.position.x - 1500, 4500);//;this.getRandomRange(5000,7000));

		this.moveDeltaV2d.copy(Collision.realPosition(this._aiPlayer.player.position,this.moveAimPosition,this._map.mapData).deltaPos);
		double length = this.moveDeltaV2d.length;
		double currentSpeed = Math.Max (0.1, this._aiPlayer.selfAssess.currentSpeedMax * this.moveAimSpeedScale);
		this.moveTimes = Math.Max (1, (length - AIConstant.ARRIVE_RADIUS) / currentSpeed);
	}
	///距离目标很近时加速下个发现和决策
	public void speedUpCD(){
		if(this.moveTimes < 10)
			this.tacticsCD *= this.moveTimes/10;
	}

	///重新决策，定义新的移动点和出牌策略
	public void checkTactics(){
		this.resetTacticsCD();
		this.tacticsCount++;

		this._tacticList.Clear ();
		this._cardTacticList.Clear ();
		this._moveTacticList.Clear ();
		//判断条件，满足条件的战术，都压进去
		this.addTactics();

		Vector2D playerPosition = this._aiPlayer.player.position;

		//排序思考优先级较高的若干条
		this._tacticList.Sort((x, y) => (-x.thinkPriority.CompareTo(y.thinkPriority)));

		int i, len;
		BaseTactic tactic;
		len = Math.Min (10, this._tacticList.Count);
		for (i = 0; i < len; i++) {
			//只思考这些方案
			tactic = this._tacticList[i];
			tactic.thinkTactic ();
			this._cardTacticList.Add (tactic);
			this._moveTacticList.Add (tactic);
			//这些方案中有目标的都记录下来，下次必定会发现，并且重要性提升
			FightEntity target = tactic.target;
			if (null != target)
				this.lastTacticTargetList.Add (target);
		}

		//排序优先级，出牌优先级较高的，之后再判断出牌
		this._cardTacticList.Sort((x, y) => (-x.cardPriority.CompareTo(y.cardPriority)));
		//移动优先级较高的，直接判定移动
		this._moveTacticList.Sort((x, y) => (-x.movePriority.CompareTo(y.movePriority)));

		len = this._moveTacticList.Count;
		if (len > 0) {
			tactic = this._moveTacticList [0];
			this.moveAimPosition.copy(tactic.moveAimPosition);
			this.moveAimAngle = tactic.moveAimAngle;
			this.moveAimSpeedScale = tactic.moveAimSpeedScale;
		} else {
			this.moveAimPosition.copy(playerPosition);
			this.moveAimAngle = this._aiPlayer.player.angle;
			this.moveAimSpeedScale = 0;
		}

		//是否可以贴边行进
		bool canMoveEdge = false;
		if (len > 0) {
			tactic = this._moveTacticList [0];
			int type = tactic.type;
			switch(type)
			{
				case AIConstant.PLAYER_TACTICS_TYPE:
					//积极追杀可以贴边
					PlayerTactic playerTactic = (PlayerTactic)tactic;
					canMoveEdge = playerTactic.subType == PlayerTactic.IN_LINE;
					break;
				case AIConstant.BEAN_TACTICS_TYPE:
				case AIConstant.RADISH_TACTICS_TYPE:
					//为了萝卜,为了吃道具可以贴边
					canMoveEdge = true;
					break;
			}
		}

		//不能贴边的策略
		if (!canMoveEdge) {
			//如果最终的移动方案导致贴边，则以当前位置向目标向转90度作为目标点
			if (this.moveAimPosition.y > this._map.mapData.height -ConfigConstant.MAP_GRID_SIZE || this.moveAimPosition.y < ConfigConstant.MAP_GRID_SIZE) {
				Vector2D deltaV2d = Collision.realPosition (this._aiPlayer.player.position, this.moveAimPosition,this._map.mapData).deltaPos;
				if (this.moveAimPosition.y < ConfigConstant.MAP_GRID_SIZE) {
					//要出上边
					if (deltaV2d.x >= 0) {
						//向右
						deltaV2d.angle += Math2.PIHalf;
					} else {
						//向左
						deltaV2d.angle -= Math2.PIHalf;
					}

				} else {
					//要出下边
					if (deltaV2d.x <= 0) {
						//向左
						deltaV2d.angle += Math2.PIHalf;
					} else {
						//向右
						deltaV2d.angle -= Math2.PIHalf;
					}
				}
				this.moveAimPosition.copy (playerPosition);
				this.moveAimPosition.add (deltaV2d);
				//随机偏移
				this.offsetMoveAim(300);
			}
		}


		//测试，都定位到某朵云上
		//		moveAimPosition = Scene.instance.GetBarrierMapPosition().offsetRadiusNew(50);
		//测试，都定位到右侧固定纬度
		//		moveAimPosition = new Vector2D(ownerPlayer.logicPosition.x + 5000, 4000);


		//临时打印，最终屏蔽
		string log;
		if (len > 0) {
			tactic = this._moveTacticList [0];
			int type = tactic.type;
			if (type == AIConstant.PLAYER_TACTICS_TYPE) {
				PlayerTactic playerTactic = (PlayerTactic)tactic;
				log = string.Format ("{0} {1}", playerTactic.subType, this.tacticsCount % 10);
			} else {
				//乱来战术
				log = string.Format ("{0} {1}", tactic.GetType ().ToString (), this.tacticsCount % 10);
			}
			//显示摇杆体力
//			log = string.Format ("{0:F2}", this.steeringHp);
		} else {
			log = "没有移动目标";
		}

		this.updateMoveTimes ();
//		this._aiPlayer.showAILog(this.moveAimPosition.ToString ());
		this._aiPlayer.setTestLog (log);
		this._aiPlayer.showMoveAimPosition ();
	}

	///判断条件，满足条件的战术，都压进去
	public void addTactics(){
		//判断条件，满足条件的战术，都压进去
		double important;
		int i, len;
		double distance;
		//自身中了控制

		//ai级别
		double IQ = this._aiPlayer.IQ;
		//对自己的评估
		AssessPlayer selfAssess = this._aiPlayer.selfAssess;
		FindController findController = this._aiPlayer.findController;

		//自身血量低，回复生命类
//		important = (AIConstant.HP_PER_MIDDLE - this._aiPlayer.selfAssess.hpPer)/AIConstant.HP_PER_MIDDLE * IQ;
//		if (important>0) {
//			CheckAddTactic (new DyingTactic (ai, null, important));
//		}
		//自身短暂未来会出现在的位置，子弹、玩家、道具与之判断距离和重要性时都依照此位置判断
		Vector2D ownerForecastPosition = selfAssess.getForecastPosition(AIConstant.FORECAST_TIME * (0.5+IQ));

		if (this._map.mapData.isTeam) {
			//如果抢萝卜模式，之后再实现AI

		}


		//应对附近有子弹，只考虑最近4个,中低级AI几乎不用
//		if (this.getRandom() < IQ*0.8) {
//			len = findController.findBulletList.Count;
//			for (i = 0; i < len; i++) {
//				BulletEntity bullet = findController.findBulletList [i];
//				if (bullet.alived) {
//					distance = Collision.realPosition(ownerForecastPosition,bullet.position,this._map.mapData).deltaPos.length;
////					important = Math.Min (Math.Min ((AIConfig.AI_FIND_RADIUS - distance) / 1500, 1), 1) * AIValue;
//					important = Math.Min (Math.Min ((2500- distance) / 1500, 1) * (0.9 + bullet.atk / this._aiPlayer.player.hp), 1) * (IQ+0.2);
//
//					checkAddTactic (new BulletTactic (this._aiPlayer, bullet, important));
//				}
//			}
//		}
		//应对附近有敌人玩家
		if (this.getRandom() < IQ * 2) {
			len = findController.findEnemyPlayerList.Count;
			for (i = 0; i < len; i++) {
				PlayerEntity player = findController.findEnemyPlayerList [i];
				if (player.alived) {

					distance = Collision.realPosition (ownerForecastPosition, player.position, this._map.mapData).deltaPos.length;
					important = Math.Min ((2500 - distance) / 1500, 1) * (IQ * 0.5 + 0.5);

					if (this._map.mapData.isChaos) {
						//如果是乱斗前3名玩家，有更高可能性被针对
						int scoreIndex = this._map.getSortPlayer(1).IndexOf (player);
						if (scoreIndex < 3) {
							important += (3 - scoreIndex) * 0.25 * IQ;
						}
					}
					//如果血很少，很容易被针对
					double hpPer = selfAssess.hpPer;
					if (hpPer < AIConstant.HP_PER_NEAR_MAX) {
						important += (AIConstant.HP_PER_NEAR_MAX - hpPer) * 0.8 * IQ;
					}
					//仇恨列表中的玩家，针对性大大提升
					AssessPlayer assessPlayer = this._aiPlayer.assessController.getAssessPlayer (player.uid);
					important += assessPlayer.anger * 0.6 * (IQ + 0.5);

					this.checkAddTactic (new PlayerTactic (this._aiPlayer, player, important));
				}
			}
		}

		//应对附近有多个道具，低级AI几乎不用
		if (this.getRandom() < IQ * 1.4) {
			len = findController.findBeanList.Count;

			if (len > 0) {
				//确定玩家对于能量和生命的需求
				double powerImportant = selfAssess.needPowerCardCount * 0.34;
				double scoreImportant = 1;
				double hpImportant = 0.05 + (1 - selfAssess.hpPer);
				//每种类型的道具只考虑最近的一个
				Dictionary<int, BeanEntity> beans = new Dictionary<int, BeanEntity> ();

				//如果数量特别庞大，以格子内的总价值集中地作为特殊目标策略进行思考
				for (i = 0; i < len; i++) {
					BeanEntity bean = findController.findBeanList [i];
					if (bean.canUse) {
						if (beans.ContainsKey (bean.itemType))
							continue;
						beans.Add (bean.itemType,bean);

						distance = Collision.realPosition(ownerForecastPosition,bean.position,this._map.mapData).deltaPos.length;
//						Debug.Log ("distance  "+distance);

						//该道具吻合玩家需求的程度
						important = bean.beanVO.power * 0.001 * powerImportant  * (IQ+0.5);
						important += bean.beanVO.score * 0.0001 * scoreImportant  * (IQ+1);
						important += bean.beanVO.cure * hpImportant * 30 * (IQ+1);

						//根据距离修正，但如果没有需求，就基本不重要
						important *= Math.Min ((2500 - distance) / 1500, 1);
						if (important > AIConstant.MESS_IMPORTANT) {
							this.checkAddTactic (new BeanTactic (this._aiPlayer, bean, important));
						}
					}
				}
			}
		}

		//如果有装备冷却完毕，立即使用，低级AI几乎不用
		if (this.getRandom() < IQ * 1) {
			important = IQ + 0.1;
			this.checkAddTactic (new PartTactic (this._aiPlayer, null, important));
		}

		//无条件的乱走乱扔牌战术
		important = AIConstant.MESS_IMPORTANT;
		addTactic (new MessTactic (this._aiPlayer, null, important));

		//遗忘上一次考虑过的战术
		this.lastTacticTargetList.Clear ();
	}

	/// 判断是否能够加入该策略
	public bool checkAddTactic(BaseTactic tactic){
		bool b = false;
		FightEntity target = tactic.target;
		if(null != target)
		{
			if (this.lastTacticTargetList.IndexOf (target) >= 0) {
				//上次重视过的目标，这次必顶发现且更加重视
				b = true;
				tactic.cardPriority *= 1.2;
				tactic.movePriority *= 1.2;
			}
		}
		if (!b && tactic.checkDiscover ()) {
			b = tactic.checkDiscover ();
		}
		if (b)
			this.addTactic (tactic);
		return b;
	}
	public void addTactic(BaseTactic tactic){
		this._tacticList.Add (tactic);
	}
	///判断当前状况是否使用卡牌
	public void checkUseCard(){
		int i, len;
		BaseTactic tactic;
		double[] fitnessArray1 = new double[ConfigConstant.CARD_HAND_MAX];
		double[] fitnessArray2;
		double priority = 0;
		len = Math.Min (3, this._cardTacticList.Count);
		//在所有出牌相关的策略里，找到最适合的那张牌打出
		for (i = 0; i < len; i++) {
			tactic = this._cardTacticList[i];
			if (tactic.cardPriority <= 0)
				break;
			priority += tactic.cardPriority;
			fitnessArray2 = tactic.getCardFitnessArray ();
			ArrayUtils.multiplyArrayValue(fitnessArray2,tactic.cardPriority);
			ArrayUtils.addArray (fitnessArray1, fitnessArray2);
		}
		//合并优先级后的每张牌适应度，找到最适合的那张打出（如果都没到达临界值，则不打出）
		ArrayUtils.multiplyArrayValue(fitnessArray1,1/priority);
		double fitness;
		double fitnessMax = AIConstant.MARK_USE_CARD;
		int fitnessIndex = -1;
		for (i = 0; i < ConfigConstant.CARD_HAND_MAX; i++) {
			fitness = fitnessArray1 [i];
			if (fitness > fitnessMax) {
				fitnessMax = fitness;
				fitnessIndex = i;
			}
		}
		if (fitnessIndex >= 0) {
			this._aiPlayer.sendUseCard (fitnessIndex);
			this.resetCardCD ();
		}
	}


	///微调摇杆绕开障碍，发出摇杆指令
	public void sendSteeringV2d(){
		//本次摇杆力
		this.steeringV2d.copy(this.moveDeltaV2d);
//		Debug.Log (this.moveDeltaV2d);
//		this.steeringV2d.normalize ();
//		return steeringV2d;

		//本次目标速率和目标角度
		double currAimSpeedScale = this.moveAimSpeedScale;
		double currAimAngle = this.steeringV2d.angle;

		if (this.moveTimes < 2) {
			if (double.MinValue != this.moveAimAngle) {
				//移动末期，剧烈原地转向(如果有目标角度)
				currAimSpeedScale = 0.001;
				currAimAngle = this.moveAimAngle;
			}
		} else {
			//非移动末期，如果移动前方有友军、障碍等，侧向滑动摇杆躲避
			if (null == this.dodgeTarget && this.getRandom() * 2 < this._aiPlayer.IQ) {
				//尝试躲避，先检查最近需要躲避的单位是什么
				this.dodgeTarget = this.getDodgeTarget(currAimAngle);
			}
			if (null != this.dodgeTarget) {
				double angleDodge = this.getDodgeAngle(currAimAngle);
				if (angleDodge != double.MinValue) {
					currAimSpeedScale = 1;
					currAimAngle = angleDodge;
				} else {
					//躲避目标已脱离，本次不躲避，下次重新找
					this.dodgeTarget = null;
				}
			}
		}

		//模拟  玩家操作摇杆的缓动过程,  频繁的左右切换方向，加减速非常消耗体力

		//当前角度
		double currAngle;
		//当前角速度
		double currAngleSpeed;
	
		//最大角度加速度
		double angleAccSpeedMax = 0.4 + 0.6 * this.steeringHp;
		double angleSpeedRate = 0.15 + 0.1 * this.steeringHp;

		currAngleSpeed = Math2.range (Math2.deltaAngle (this.lastAngle, currAimAngle) * angleSpeedRate,  this.lastAngleSpeed + angleAccSpeedMax ,this.lastAngleSpeed - angleAccSpeedMax);
		currAngle = (this.lastAngle + currAngleSpeed + Math2.PI2) % Math2.PI2;

		//转角加速度非常消耗体力
		useSteeringHp( Math.Abs (this.lastAngleSpeed - currAngleSpeed) * 0.1);
		this.steeringV2d.angle = currAngle;

		this.lastAngle = currAngle;
		this.lastAngleSpeed = currAngleSpeed;

		//加减速
		double speedRate;
		double deltaSpeedRate = currAimSpeedScale - this.lastSpeedScale;
		if (deltaSpeedRate >= 0) {
			//加速很快
			speedRate = 0.3 + 0.2 * this.steeringHp;
		} else {
			//减速很慢，消耗体力
			speedRate = 0.01 + 0.1 * this.steeringHp;
			useSteeringHp( Math.Abs (deltaSpeedRate* speedRate*0.1));
		}
		this.lastSpeedScale += deltaSpeedRate * speedRate;

		this.steeringV2d.length = this.lastSpeedScale;
		//发出摇杆指令
		this._aiPlayer.sendMove (this.steeringV2d);
	}
	///得到某个单位向指定方向移动，首个需要躲避的前方友军或障碍物
	public FightEntity getDodgeTarget(double aimAngle){
		Vector2D aimDeltaV2d = Vector2D.createVector2 ( AIConstant.DODGE_BARRIER_RADIUS, aimAngle);
		new GeomCircle (null, int.MaxValue);
		GeomBase geom = new GeomCircle (null, int.MaxValue);
		geom.parseData (new object[] { this._aiPlayer.player.shape.radius +AIConstant.DODGE_BARRIER_RADIUS});
		geom.position.copy (aimDeltaV2d.add(_aiPlayer.player.position));
		geom.applyEntity = false;
//		Collision.createGeom(this._aiPlayer.player, ( new object[] { this._aiPlayer.player.shape.radius} ));
//		geom.position.copy (aimDeltaV2d.add(_aiPlayer.player.position));
		aimDeltaV2d.clear ();

//		List<FightEntity> fightList = this.map.getFightEntitysByRange (geom,new List<int> { ConfigConstant.ENTITY_PLAYER},FightEntity.getPureAllyTeamFilter,-1);
		List<BarrierEntity> barrierList = this._map.getFightEntitysByRange (geom,new List<int> { ConfigConstant.ENTITY_BARRIER},null,0).ConvertAll<BarrierEntity>((e) => { return e as BarrierEntity; });
		if (barrierList.Count > 0)
			return barrierList [0];
		else 
			return null;
	}
	///朝指定方向移动，如果需要躲避该目标，则返回躲避角度
	public double getDodgeAngle (double aimAngle) {
		if (!this.dodgeTarget.alived)
			return float.MinValue;
		Vector2D aimDeltaV2d = Vector2D.createVector2 ( AIConstant.DODGE_BARRIER_RADIUS, aimAngle);
		if (
			Collision.realPosition (aimDeltaV2d.add(this._aiPlayer.player.position),this.dodgeTarget.position,this._map.mapData).deltaPos.length >
			this._aiPlayer.player.shape.radius+this.dodgeTarget.shape.radius+AIConstant.DODGE_BARRIER_RADIUS
		) {
			//如果已经不阻挡道路，就不躲避了
			return double.MinValue;
		}
		aimDeltaV2d.clear ();	

		Vector2D deltaV2d = Collision.realPosition (this._aiPlayer.player.position, this.dodgeTarget.position,this._map.mapData).deltaPos;
		double deltaAngle = Math2.deltaAngle(aimAngle,deltaV2d.angle);
		//如果目标方向与碰撞向量夹角>90，不予理睬
		double absDeltaAngle = Math.Abs (deltaAngle);
		if (absDeltaAngle >= Math2.PIHalf) {
			return double.MinValue;
		}

		//增加垂直于向左或向右的方向
		double tempAngle = Math2.PIHalf;
		deltaAngle += deltaAngle <= 0 ? tempAngle : -tempAngle;
		return aimAngle + deltaAngle;
	}


	///消耗体力
	public void useSteeringHp(double value){
		this.steeringHp = Math.Max (this.steeringHp - value, 0);
	}

	///设置目标点，随机偏移不超过半径
	public void offsetMoveAim(double radius){
		radius = this.getRandom () * radius;
		double angle = this.getRandom () * Math2.PI2;
		Vector2D v2d = Vector2D.createVector2 (radius, angle);
		this.moveAimPosition.add (v2d);
		v2d.clear ();
	}
}
