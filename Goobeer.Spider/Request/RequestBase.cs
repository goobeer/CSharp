using Goobeer.Spider.ContentItem;
using System;
using System.Net;

namespace Goobeer.Spider.Request
{
    /// <summary>
    /// web请求基类
    /// </summary>
    public abstract class RequestBase
    {
        /// <summary>
        /// 文本类型的HttpRequest
        /// </summary>
        public WebRequest Request { get; set; }

        public IWebProxy WebProxy { get; set; }

        public WebResponseItem Response { get; set; }
    }
}
