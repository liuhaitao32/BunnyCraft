using System;
using System.Collections.Generic;

public interface IDecorate
{
	/**
	 * 装饰属性。
	 * @param	personProperty
	 * @return
	 */
	void decortation(Dictionary<string, double> property);



	/**
	 * 获取当前应用的人身上
	 */
	PersonEntity applyTarget { get;}

    FightEntity owner { get; }

    Dictionary<string, object> data { get; }

    void finish();

    void update();

    string buffType { get; }

    int operation { get; }

    double value { get; }

    bool isFinish { get; }

}

