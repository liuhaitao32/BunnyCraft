using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

    public class ModelMail:BaseModel
    {
        public ModelMail()
        {

        }

    public bool Get_RedMail()
    {
        string version= LocalStore.GetLocal(LocalStore.NOTICE_VERSION);
        if (ModelManager.inst.userModel.notice_version.Equals(version))
            return false;
        else
        {
           
            return true;
        }
            
    }

    }

