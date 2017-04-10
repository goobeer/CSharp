using Goobeer.Entity;
using Goobeer.Spider.ContentItem;
using Goobeer.Spider.Filter;
using Goobeer.Spider.PoolContainer;
using Goobeer.Spider.Request;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Goobeer.Spider
{
    /// <summary>
    /// 过滤操作(delegate 不能并行操作) 影响效率
    /// </summary>
    /// <param name="srcUri">源地址</param>
    /// <param name="content">源内容</param>
    public delegate void FilterEventHandler(string srcUri, string content);

    /// <summary>
    /// 过滤事件参数
    /// </summary>
    public class FilterEventArgs : EventArgs
    {
        private readonly List<string> Data=new List<string>();

        public FilterEventArgs(List<string> data)
        {
            Data.AddRange(data);
        }
    }

    /// <summary>
    /// 网络爬虫
    /// </summary>
    public class GoobeerSpider
    {
        private EventWaitHandle readWaitHandler { get; set; }

        private EventWaitHandle writeWaitHandler { get; set; }

        public ObjectPool<HttpPageRequest> SpiderPool { get; }

        private IFilterStrategy[] Filter { get; }

        public ConcurrentQueue<WebDocument> Queue { get; set; }

        public Func<string> GetUnVisitedUri { get; set; }

        //数据容器，后续处理

        public GoobeerSpider(int spiderCount,params IFilterStrategy[] filter)
        {
            //初始化爬虫池
            SpiderPool = new ObjectPool<HttpPageRequest>(spiderCount);
            //初始化过滤器
            Filter = filter;

            readWaitHandler = new AutoResetEvent(false);
            writeWaitHandler = new AutoResetEvent(false);
        }

        public async Task InitUrlSeed(string uri)
        {
            HttpPageRequest httpReqest = SpiderPool.Take();
            httpReqest.ContinueRequest(uri, null);
            var document =  httpReqest.HttpResponse().Result;
            Queue.Enqueue(document);
            SpiderPool.Add(httpReqest);
            await DoFilter(document);

            UriProductor();
            UriCustomer();
        }

        /// <summary>
        /// 生产者(获得document)
        /// </summary>
        /// <param name="uri"></param>
        public async Task UriProductor()
        {
            string uri = GetUnVisitedUri();
            while (!string.IsNullOrEmpty(uri))
            {
                uri = uri.Split('\t')[0];
                HttpPageRequest httpReqest = SpiderPool.Take();
                httpReqest.ContinueRequest(uri, null);
                var result = await httpReqest.HttpResponse();
                if (result!=null)
                {
                    Queue.Enqueue(result);
                }
                SpiderPool.Add(httpReqest);
                readWaitHandler.Set();
                writeWaitHandler.WaitOne();
                uri = GetUnVisitedUri();
            }
        }

        /// <summary>
        /// Uri消费者(过滤内容，存储Uri)
        /// </summary>
        public async Task UriCustomer()
        {
            WebDocument document = null;
            while (readWaitHandler.WaitOne() && Queue.TryDequeue(out document))
            {
                if (document!=null)
                {
                    await DoFilter(document);
                }
                writeWaitHandler.Set();
            }
        }

        public async Task DoFilter(WebDocument document)
        {
            await Task.Factory.StartNew(()=> {
                Parallel.ForEach(Filter, ifs => {
                    var filterData = ifs.DoFilter(document);
                });
            });
        }
    }
    
    /// <summary>
    /// Http请求谓词
    /// </summary>
    public enum HttpMethod
    {
        GET, POST
    }
}
