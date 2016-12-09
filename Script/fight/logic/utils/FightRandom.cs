using System;
public class FightRandom {
	static readonly long NUM_1 = 239641;
	static readonly long NUM_2 = 6700417;
	static readonly long NUM_3 = 9991;
	static readonly double MOD = 1000;

	public long seed = 82684;
	public int seedNum = 0;
	public long seedAI = 82684;
	public int seedAINum = 0;

	///返回战斗逻辑所需的下一个double随机数，0~1之间，Biggo修改
	public double getRandom() {
		seed = (seed * NUM_3 + NUM_2) % NUM_1;
		seedNum++;
		return seed % MOD / MOD;
	}
		
	///返回战斗AI 所需的下一个double随机数，0~1之间，Biggo新增
	public double getRandomAI() {
		seedAI = (seedAI * NUM_3 + NUM_2) % NUM_1;
		seedAINum++;
		return seedAI % MOD / MOD;
	}
}