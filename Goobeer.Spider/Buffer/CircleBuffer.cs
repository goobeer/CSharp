using System.Collections.Generic;

namespace Goobeer.Spider.Buffer
{
    /// <summary>
    /// 环形缓冲池(供多线程使用)
    /// </summary>
    /// <typeparam name="T">要存储的类型</typeparam>
    public class CircleBuffer<T>
    {
        private int _MaxSize = 1000;

        private List<T> _Data;
        public List<T> Data
        {
            get { return _Data; }
        }

        private int _Header;
        public int Header { get { return _Header; } }

        private int _Tail;
        public int Tail { get { return _Tail; } }

        public CircleBuffer(int maxSize)
        {
            if (maxSize > 0)
            {
                _MaxSize = maxSize;
            }
            _Data = new List<T>(_MaxSize);

            _Header = _Tail = 0;
        }

        public void Write(T t)
        {
            lock (this)
            {
                if (Header != Tail)
                {
                    Data.Add(t);
                    _Header = Data.IndexOf(t);
                }
                else
                {
                    //缓冲区已满

                }
            }
        }

        public T Read()
        {
            return default(T);
        }
    }
}
