using Goobeer.Entity.Base;
using Goobeer.DB.DataAttributeHelper;
using System;

namespace Goobeer.Entity
{
    [Table(TableName = "[O.ClassAttrValues]")]
    public class ClassAttrValues:OrderBase
    {
        public Guid AID { get; set; }
        public Guid CID { get; set; }
        public string CValue { get; set; }
        public string Desc { get; set; }
    }
}
