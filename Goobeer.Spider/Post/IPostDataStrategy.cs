using System;
using System.Net;

namespace Goobeer.Spider.Post
{
    /// <summary>
    /// PostData策略
    /// </summary>
    public interface IPostDataStrategy
    {
        void AppendPostHeader(WebRequest request);
    }
}
