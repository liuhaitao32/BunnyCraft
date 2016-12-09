using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ModelRank : BaseModel
{
    public ModelRank()
    {

    }
    public bool isChange = false;
    public string type=null;//
    public Dictionary<string,object> IsContainUser(List<object> value,string uid)
    {
        bool isContain = false;
        int index =0;
        Dictionary<string, object> myData=null;
        Dictionary<string, object> data = new Dictionary<string, object>();
        for(int i = 0; i < value.Count; i++)
        {
            Dictionary<string,object> item=(Dictionary<string,object>)value[i];
            if (item["uid"].ToString().Equals(uid))
            {
                myData = item;
                index = i;
                if (value.Contains(item))
                {
                    isContain = true;
                }
                else
                {
                    isContain = false;
                }
            }
        }
        data["data"] = myData;
        data["index"] = index;
        data["isContain"] = isContain;
        return data;


    }



}

