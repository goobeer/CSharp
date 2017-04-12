using Goobeer.DB.DataAttributeHelper;

namespace Goobeer.Entity
{
    public class Visited
    {
        [Field(AutoCreate = true, IsPK = true)]
        public int ID { get; set; }
        public string ParentUriAddress { get; set; }
        public string UriAddress { get; set; }
        public string Charset { get; set; }
        public string Content { get; set; }
    }
}
