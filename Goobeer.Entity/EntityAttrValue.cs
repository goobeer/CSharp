using Goobeer.DB.DataAttributeHelper;
using System;

namespace Goobeer.Entity
{
    [Table(TableName = "[O.EntityAttrValues]")]
    public class EntityAttrValues
    {
        [Field(AutoCreate = true, IsPK = true)]
        public Guid ID { get; set; }
        public Guid AID { get; set; }
        public Guid EID { get; set; }
        public string AttrVal { get; set; }
    }
}
