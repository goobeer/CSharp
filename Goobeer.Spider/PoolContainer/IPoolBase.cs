namespace Goobeer.Spider.PoolContainer
{
    public interface IPoolBase<T> where T:class
    {
        T Take();
        void Add(T instance);
    }    
}
