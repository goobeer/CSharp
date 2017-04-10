using System.Collections;

namespace Goobeer.Spider.StorageFactory
{
    public interface IStorage<T> where T : ICollection
    {
        void Write(T t);
    }
}
