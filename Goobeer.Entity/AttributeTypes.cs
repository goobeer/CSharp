using Goobeer.Entity.Base;
using Goobeer.DB.DataAttributeHelper;

namespace Goobeer.Entity
{
    [Table(TableName = "[O.AttributeTypes]")]
    public class AttributeTypes:OrderBase
    {
        public string AttrReg { get; set; }
    }
}
