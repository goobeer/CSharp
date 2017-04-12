using Goobeer.DB.DataAttributeHelper;
using Goobeer.Entity.Base;
using System;

namespace Goobeer.Entity
{
    [Table(TableName = "[O.Class]")]
    public class Classes:OrderBase
    {
        public Guid PID { get; set; }
    }
}
