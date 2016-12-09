using System;
using System.Collections;
using System.Collections.Generic;

///被AI评估玩家 的手牌评估
public class AssessCard:BaseAssessPlayer{
	private CardGroupAction _cardGroup;

	public AssessCard(AssessPlayer assessPlayer):base(assessPlayer) 
	{
		this._cardGroup = this._assessPlayer.targetPlayer.cardGroup;
	}


	///传入需要评估的范畴和内容类型，返回评估结果
	public override double[] getTypeMark(int aspectType, int valueType) {
		MarkVO mark;
		CardData card;
		double[] marks = new double[AIConstant.MARK_COUNT];
		for (int i = 0; i < ConfigConstant.CARD_HAND_MAX; i++) {
			card = this._cardGroup.handCards [i];
			if (card != null && card.canUse) {
				mark = MarkVO.getCardMark (card.id);
				double[] tempMarks = mark.getTypeMark (aspectType, valueType);
				if (mark.isPart)
					ArrayUtils.addArray (marks, ArrayUtils.multiplyArrayValue(tempMarks,AIConstant.PART_IN_CARD_MARK_RATE));
				else
					ArrayUtils.addArray (marks, tempMarks);
			}
		}
		return marks;
	}
	///传入需要评估的内容类型，返回评估结果
	public override double getMark(int valueType) {
		MarkVO mark;
		CardData card;
		double markValue = 0;
		for (int i = 0; i < ConfigConstant.CARD_HAND_MAX; i++) {
			card = this._cardGroup.handCards [i];
			if (card != null && card.canUse) {
				mark = MarkVO.getCardMark (card.id);
				double tempMark = mark.getMark (valueType);
				if (mark.isPart) {
					markValue += tempMark * AIConstant.PART_IN_CARD_MARK_RATE;
				} else {
					markValue += tempMark;
				}
			}
		}
		return markValue;
	}
		
	///当前是否可以爆发移动
	public bool canBurstMove{
		get{
			for(int i=0;i<ConfigConstant.CARD_HAND_MAX;i++)
			{
				CardData card = this._cardGroup.handCards [i];
				if (card != null && card.canUse) {
					MarkVO mark;
					mark = MarkVO.getCardMark (card.id);
					if (!mark.isPart && mark.move > AIConstant.MARK_BURST_MOVE)
						return true;
				}
			}
			return false;
		}
	}
	///当前是否可以爆发防御
	public bool canBurstShield{
		get{
			for(int i=0;i<ConfigConstant.CARD_HAND_MAX;i++)
			{
				CardData card = this._cardGroup.handCards [i];
				if (card != null && card.canUse) {
					MarkVO mark;
					mark = MarkVO.getCardMark (card.id);
					if (!mark.isPart && mark.life > AIConstant.MARK_BURST_SHIELD)
						return true;
				}
			}
			return false;
		}
	}
}


