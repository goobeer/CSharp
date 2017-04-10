using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goobeer.Spider.StorageFactory
{
    /// <summary>
    /// 文本存储
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FileStorage<T> : IStorage<T> where T : ICollection
    {
        private string fileName;
        public FileStorage(string fileName)
        {
            this.fileName = fileName;
        }

        public void Write(T t)
        {
            List<string> list = t.Cast<string>().ToList();
            for (int i = list.Count - 1000; i >= 0 || (i % 1000 < 0 && 1000 + i > 0); i -= 1000)
            {
                if (i>=0)
                {
                    File.AppendAllLines(fileName, list.Take(1000), Encoding.Default);
                    list.RemoveRange(0, 1000);
                }
                else
                {
                    File.AppendAllLines(fileName, list, Encoding.Default);
                    list.Clear();
                }
            }
        }
    }
}
