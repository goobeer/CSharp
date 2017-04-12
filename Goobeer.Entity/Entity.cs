using Goobeer.DB.DataAttributeHelper;
using Goobeer.Entity.Base;
using System;

namespace Goobeer.Entity
{
    [Table(TableName="[O.Entity]")]
    public class Entity:OrderBase
    {
        public Guid CID { get; set; }

        public DateTime DTime { get; set; }
    }
}
