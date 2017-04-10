using Goobeer.Spider.Request;
using Goobeer.Spider.Post;
using System;
using System.Net;

namespace Goobeer.Spider.Request
{
    /// <summary>
    /// 文本http请求类型的header
    /// </summary>
    public class TextRequestHeader : RequestBase
    {
        //public TextRequestHeader(string url, HttpMethod method)
        //{
        //    base.InitRequestHeader(url, method);
        //}

        //public TextRequestHeader(string url, HttpMethod method, CookieContainer cookieContainer = null)
        //    : this(url, method)
        //{
        //    Request.CookieContainer = cookieContainer;
        //}

        //public override void AppendRequestHeader(System.Net.HttpWebRequest request)
        //{
        //    request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        //    request.ProtocolVersion = HttpVersion.Version11;
        //}
    }
}
