using System.Web;

namespace Goobeer.Cache
{
    /// <summary>
    /// HttpRuntime.Cache
    /// </summary>
    public class WebCache : ICacheable
    {
        private System.Web.Caching.Cache CacheRepertory { get { return HttpRuntime.Cache; } }

        private static object state = new object();

        public T Get<T>(string key)
        {
            var cacheObj = CacheRepertory.Get(key);
            return (T)cacheObj;
        }

        public void Set<T>(string key, T obj, bool forceOverwrite)
        {
            if (forceOverwrite)
            {
                CacheRepertory.Insert(key, obj);
            }
            else
            {
                CacheRepertory.Add(key, obj, null, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
            }
        }

        public T Remove<T>(string key)
        {
            var cacheObj = CacheRepertory.Remove(key);
            return (T)cacheObj;
        }

        public void RemoveAll()
        {
            var cacheData = CacheRepertory.GetEnumerator();

            while (cacheData.MoveNext())
            {
                CacheRepertory.Remove(cacheData.Key.ToString());
            }
        }
    }
}
