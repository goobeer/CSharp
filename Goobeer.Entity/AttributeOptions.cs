using Goobeer.Entity.Base;
using Goobeer.DB.DataAttributeHelper;
using System;

namespace Goobeer.Entity
{
    [Table(TableName = "[O.AttributeOptions]")]
    public class AttributeOptions:OrderBase
    {
        public Guid AttrID { get; set; }
        public string Val { get; set; }
    }
}
