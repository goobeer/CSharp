using Goobeer.DB.DataAttributeHelper;
using System;

namespace Goobeer.Entity.Base
{
    public class EntityBase
    {
        [Field(AutoCreate = true, IsPK = true)]
        public Guid ID { get; set; }

        public string Name { get; set; }
    }
}
