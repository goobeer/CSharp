using Memcached.ClientLibrary;

namespace Goobeer.Cache
{
    public class MemcacheCache : ICacheable
    {
        SockIOPool _sockIOPoll { get; }
        MemcachedClient _mc = new MemcachedClient();

        public MemcacheCache(SockIOPool sockIOPoll,MemcachedClient mc)
        {
            _sockIOPoll = sockIOPoll;
            _sockIOPoll.Initialize();
            _mc = mc;
        }

        public T Get<T>(string key)
        {
            T result = default(T);
            if (_mc.KeyExists(key))
            {
                result = (T)_mc.Get(key);
            }

            return result;
        }

        public T Remove<T>(string key)
        {
            var result = Get<T>(key);
            _mc.Delete(key);
            return result;
        }

        public void RemoveAll()
        {
            _mc.FlushAll();
        }

        public void Set<T>(string key, T obj, bool forceOverwrite)
        {
            if (forceOverwrite)
            {
                _mc.Set(key, obj);
            }
            else
            {
                _mc.Add(key, obj);
            }
        }
    }
}
