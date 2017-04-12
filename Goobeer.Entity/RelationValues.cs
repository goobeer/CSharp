using Goobeer.DB.DataAttributeHelper;
using System;

namespace Goobeer.Entity
{
    [Table(TableName = "[O.RelationValues]")]
    public class RelationValues
    {
        [Field(AutoCreate = true, IsPK = true)]
        public Guid ID { get; set; }
        public Guid RelationID { get; set; }
        public string RelationValue { get; set; }
    }
}
