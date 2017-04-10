using System.Collections.Concurrent;

namespace Goobeer.Spider.PoolContainer
{
    public class ObjectPool<T> : IPoolBase<T> where T : class
    {
        protected volatile ConcurrentQueue<T> Items;

        public bool IsEmpty
        {
            get { return Items.IsEmpty; }
        }

        public ObjectPool(int maxCount)
        {
            Items=new ConcurrentQueue<T>();
        }

        /// <summary>
        /// 确保线程安全
        /// </summary>
        /// <returns></returns>
        public T Take()
        {
            T item = null;
            if (!Items.TryDequeue(out item))
            {
                item = default(T);
                Items.Enqueue(item);
            }
            return item;
        }

        public void Add(T instance)
        {
            Items.Enqueue(instance);
        }
    }
}
