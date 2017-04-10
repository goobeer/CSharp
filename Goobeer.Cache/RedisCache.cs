using ServiceStack.Redis;

namespace Goobeer.Cache
{
    public class RedisCache : ICacheable
    {
        RedisClient _redisClient { get; }

        public RedisCache( RedisClient client)
        {
            _redisClient = client;
        }


        public T Get<T>(string key)
        {
            return _redisClient.Get<T>(key);
        }

        public T Remove<T>(string key)
        {
            var result = Get<T>(key);
            _redisClient.Remove(key);
            return result;
        }

        public void RemoveAll()
        {
            _redisClient.FlushAll();
        }

        public void Set<T>(string key, T obj, bool forceOverwrite)
        {
            if (forceOverwrite)
            {
                _redisClient.Replace(key, obj);
            }
            else
            {
                _redisClient.Add(key, obj);
            }
        }
    }
}
