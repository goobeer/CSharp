using Goobeer.DataAttributeHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goobeer.Entity
{
    public class EntityBase
    {
        [Field(AutoCreate = true, IsPK = true)]
        public Guid ID { get; set; }
        [Field]
        public string Name { get; set; }
    }
}
