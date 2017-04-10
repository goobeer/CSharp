using System;
using System.Collections.Specialized;
using System.Net;

namespace Goobeer.Spider.Request
{
    /// <summary>
    /// http协议 页面请求
    /// </summary>
    public class HttpPageRequest : HttpRequestBase
    {
        public HttpPageRequest()
        { }

        public HttpPageRequest(Uri requestUri, NameValueCollection nvcHeader = null, string reqMethod = "GET")
        {
            ContinueRequest(requestUri, nvcHeader, reqMethod);
        }

        public void ContinueRequest(Uri requestUri, NameValueCollection nvcHeader, string reqMethod = "GET")
        {
            InitRequest(requestUri, reqMethod, nvcHeader);
            SetRequestHeader(null);
            AddCookies(CollectResponseCookie());
        }

        public void ContinueRequest(string uriString, NameValueCollection nvcHeader, string reqMethod = "GET")
        {
            Uri requestUri = new Uri(uriString);
            ContinueRequest(requestUri, nvcHeader, reqMethod);
        }

        public void AddCookies(CookieCollection cc)
        {
            if (cc != null && cc.Count>0)
            {
                var httpWebRequest = (HttpWebRequest)Request;
                var cookieContainer = new CookieContainer(cc.Count);
                cookieContainer.Add(cc);
                httpWebRequest.CookieContainer = cookieContainer;
            }
        }

        protected CookieCollection CollectResponseCookie()
        {
            CookieCollection cookies = null;
            if (Response != null && Request != null && Request.RequestUri.Host.Length == Response.ResponseUri.Host.Length && Request.RequestUri.Host == Response.ResponseUri.Host)
            {
                cookies = new CookieCollection();
                cookies.Add(Response.Cookies);
                foreach (var key in Response.Headers.AllKeys)
                {
                    if (string.Compare(key, "Set-Cookie",true)==0)
                    {
                        var cookieStr = Response.Headers[key];
                        if (!string.IsNullOrEmpty(cookieStr))
                        {
                            var cookie = ParseCookie(Response.Headers[key]);
                            if (string.IsNullOrEmpty(cookie.Domain))
                            {
                                cookie.Domain = Response.ResponseUri.Host;
                            }
                            cookies.Add(cookie);
                        }
                    }
                }
            }
            return cookies;
        }

        protected void SetDefaultHttpRequestHeader(HttpWebRequest request)
        {
            request.KeepAlive = true;
            request.Host = Request.RequestUri.Host;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2062.58 Safari/537.36";
            Request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate,sdch");
        }

        protected virtual void SetRequestHeader(NameValueCollection nvcHeader)
        {
            var httpWebRequest = (HttpWebRequest)Request;
            httpWebRequest.ProtocolVersion = HttpVersion.Version11;
            SetDefaultHttpRequestHeader(httpWebRequest);
            httpWebRequest.AllowAutoRedirect = false;
            httpWebRequest.ServicePoint.Expect100Continue = true;
            base.SetRequestHeader(nvcHeader);
        }

        private Cookie ParseCookie(string cookieStr)
        {
            Cookie cookie = new Cookie();
            var data = cookieStr.Split(new string[] {",","; " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in data)
            {
                var itemKV = item.Split('=');
                if (itemKV.Length == 2)
                {
                    var k=itemKV[0].ToLower();
                    var v = itemKV[1];

                    switch (k)
                    {
                        case "commenturi":
                            cookie.CommentUri = new Uri(v);
                            break;
                        case "comment":
                            cookie.Comment = v;
                            break;
                        case "domain":
                            cookie.Domain = v;
                            break;
                        case "expires":
                            cookie.Expires = DateTime.Parse(v);
                            break;
                        case "path":
                            cookie.Path = v;
                            break;
                        case "port":
                            cookie.Port = v;
                            break;
                        case "discard":
                            cookie.Discard = bool.Parse(v);
                            break;
                        case "expired":
                            cookie.Expired = bool.Parse(v);
                            break;
                        case "httponly":
                            cookie.HttpOnly = bool.Parse(v);
                            break;
                        case "secure":
                            cookie.Secure = bool.Parse(v);
                            break;
                        case "version":
                            cookie.Version=int.Parse(v);
                            break;
                        case "TimeStamp":
                            break;
                        default:
                            cookie.Name = itemKV[0];
                            cookie.Value = v;
                            break;
                    }
                }
                else
                {
                    var k = itemKV[0].ToLower();
                    switch (k)
                    {
                        case "discard":
                            cookie.Discard = true;
                            break;
                        case "expired":
                            cookie.Expired = true;
                            break;
                        case "httponly":
                            cookie.HttpOnly = true;
                            break;
                        case "secure":
                            cookie.Secure = true;
                            break;
                        default:
                            break;
                    }
                }
            }
            return cookie;
        }
    }
}
