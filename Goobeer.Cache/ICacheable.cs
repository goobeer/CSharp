namespace Goobeer.Cache
{
    /// <summary>
    /// 缓存层应该知道如何存/取数据
    /// </summary>
    public interface ICacheable
    {
        //bool ContainKey(string key);
        T Get<T>(string key);
        void Set<T>(string key,T obj,bool forceOverwrite);
        T Remove<T>(string key);
        void RemoveAll();
    }
}
