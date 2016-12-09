using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class LogicOperation {
    public static double operation(double source, double value, int operation) {
			switch(operation) {
                case 0:
                    source = value;
                    break;
                case 1:
					source += value;
					break;
				case 2:
					source -= value;
					break;
				case 3:
					source *= value;
					break;
				case 4:
					source /= value;
					break;
			}
			return source;
    }

    public static double grow(double source, Dictionary<string, object> formula, int level) {        
        if(formula.ContainsKey("add")) {
            double power = Convert.ToSingle(formula["power"]);
            double add = Convert.ToSingle(formula["add"]);
            source = source * Math.Pow(power, level - 1) + add * (level - 1);
        } else if(formula.ContainsKey("fixed")) {
            object[] fixeds = (object[])formula["fixed"];
            for(int i = 0, len = fixeds.Length; i < len; i++) {
                object[] value = (object[])fixeds[i];
                if(( level ) >= (int)value[0]) {
                    source = Convert.ToSingle(value[1]);
                } else {
                    break;
                }
            }
        }
        return source;
    }
}
