using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TestValue : MonoBehaviour {

    public static string value = "10";

    public static bool test1 = false;
    public static bool test2 = false;
    public static bool test3 = false;
    public static bool test4 = false;
    public static bool test5 = false;
    public static bool test6 = false;
    public static bool test7 = false;
    public static bool test8 = false;
    public static bool test9 = false;

    void Start() {
        Button btn = Tools.FindChild<Button>(this.gameObject, "btn");
        L_Input input = Tools.FindChild<L_Input>(this.gameObject, "input");
        input.text = value.ToString();
        btn.onClick.AddListener(() => {
            value = input.text;
            FightMain.instance.changeTest();
        });
    }

	// Update is called once per frame
}
