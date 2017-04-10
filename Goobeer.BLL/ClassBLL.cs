using Goobeer.DataLogic;
using System;
using System.Collections.Generic;

namespace Goobeer.BLL
{
    public class ClassBLL : BaseBLL
    {
        ClassDL CDal = new ClassDL();

        public ClassBLL()
        {
            
        }

        public List<Entity.Classes> GetClasses(Guid pid)
        {
            List<Entity.Classes> list = null;
            list = CDal.GetClasses(pid);
            return list;
        }

        public List<Entity.Entity> GetEntity(Guid cid)
        {
            return CDal.GetEntity(cid);
        }
    }
}
