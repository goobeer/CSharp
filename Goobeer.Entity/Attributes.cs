using Goobeer.Entity.Base;
using Goobeer.DB.DataAttributeHelper;
using System;

namespace Goobeer.Entity
{
    [Table(TableName = "[O.Attributes]")]
    public class Attributes:EntityBase
    {
        public Guid CID { get; set; }
        public Guid AttrTypeID { get; set; }
        public string Val { get; set; }
        public bool IsUsed { get; set; }
        public int LifeDepth { get; set; }
    }
    
}
