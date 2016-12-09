using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class PlayerData {

    public static PlayerData instance = new PlayerData();

    public List<Dictionary<string, object>> cardGroup = new List<Dictionary<string, object>>();

    public string uid = "";

    public string shipId = "000";
    //[users]:uid lv effor_lv name head["use"] cards[id, lv];
    public Dictionary<string, object> data = new Dictionary<string, object>() {
        { "match_server",new object[2] { PlatForm.inst.SERVER4, 7777 } },
        { "room_id", 3},
        { "room_key", ""},
        { "mode", ConfigConstant.MODE_CHAOS},
    };

    public int rank;
    

    public PlayerData() {

    }
}