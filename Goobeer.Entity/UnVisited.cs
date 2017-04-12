using Goobeer.DB.DataAttributeHelper;
using System;

namespace Goobeer.Entity
{
    public class UnVisited
    {
        [Field(AutoCreate = true, IsPK = true)]
        public Int64 ID { get; set; }
        public string UriAddress { get; set; }
        public string ParentUriAddress { get; set; }

        public bool IsRelated { get; set; }
    }
}
