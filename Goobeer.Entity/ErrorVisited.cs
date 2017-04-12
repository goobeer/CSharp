using Goobeer.DB.DataAttributeHelper;

namespace Goobeer.Entity
{
    public class ErrorVisited
    {
        [Field(AutoCreate = true, IsPK = true)]
        public int ID { get; set; }
        public string ParentUriAddress { get; set; }
        public string UriAddress { get; set; }

        public string WebHeaders { get; set; }
    }
}
