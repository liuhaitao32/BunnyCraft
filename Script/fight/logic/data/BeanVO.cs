using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BeanVO {
    public int itemType;
    public int power;
    public double cure;
    public double push;
    public int score;

    private static Dictionary<int, BeanVO> _map = new Dictionary<int, BeanVO>();

    public static BeanVO getVOByType(int itemType) {
        return _map[itemType];
    }

    public static void init() {
        for(int i = 0, len = 4; i < len; i++) {
            int itemType = ( i + 1 );
            Dictionary<string, object> beanConfig = (Dictionary<string, object>)ConfigConstant.powerConfig["bean" + itemType];
            _map[itemType] = new BeanVO() {
                itemType = (int)beanConfig["itemType"],
                power    = (int)beanConfig["power"],
                cure     = Convert.ToDouble(beanConfig["cure"]),
                push     = Convert.ToDouble(beanConfig["push"]),
                score    = (int)beanConfig["score"]
            };
        }
    }
}
