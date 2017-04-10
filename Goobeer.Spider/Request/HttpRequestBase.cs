using Goobeer.Spider.ContentItem;
using System;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Goobeer.Spider.Request
{
    /// <summary>
    /// Http 协议请求基类
    /// </summary>
    public abstract class HttpRequestBase : RequestBase
    {
        /// <summary>
        /// 设置HttpRequest 的header信息
        /// </summary>
        /// <param name="request"></param>
        protected virtual void SetRequestHeader(NameValueCollection nvcHeader)
        {
            if (nvcHeader != null)
            {
                Request.Headers.Add(nvcHeader);
            }
        }

        protected virtual WebHeaderCollection CollectResponseHeader(WebHeaderCollection headers)
        {
            WebHeaderCollection newHeader = new WebHeaderCollection();
            return newHeader;
        }

        protected void InitRequest(Uri requestUri, string reqMethod,NameValueCollection nvcHeader)
        {
            if (requestUri != null)
            {
                try
                {
                    Request = WebRequest.Create(requestUri);
                    if (WebProxy!=null)
                    {
                        Request.Proxy = WebProxy;
                    }
                    
                    if (!string.IsNullOrEmpty(reqMethod))
                    {
                        Request.Method = reqMethod;
                    }

                    SetRequestHeader(nvcHeader);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new Exception("error requestUri");
            }
        }

        protected WebResponseItem ParseResponse(WebResponse response)
        {
            if (Response != null)
            {
                Response.StoreResponse(response);
            }
            else
            {
                Response = new WebResponseItem(response);
            }
            return Response;
        }

        public async Task<WebDocument> HttpResponse()
        {
            WebDocument wd = null;
            WebResponse response = null;
            try
            {
                using (response = await Request.GetResponseAsync())
                {
                    wd = await ParseWebDocument(response);
                    wd.ParentUrlAddress = Request.RequestUri;
                    if (response.ResponseUri.Host.Length == Request.RequestUri.Host.Length && response.ResponseUri.Host == Request.RequestUri.Host)
                    {
                        var webResponseItem = ParseResponse(response);
                        string location = webResponseItem.Headers["Location"];
                        if (!string.IsNullOrEmpty(location))
                        {
                            var headers = CollectResponseHeader(response.Headers);
                            InitRequest(new Uri(ParseResponseLocation(location,Request.RequestUri)), "GET", headers);
                            wd = await HttpResponse();
                        }
                        if (string.IsNullOrEmpty(webResponseItem.CharacterSet))
                        {
                            webResponseItem.CharacterSet = wd._Charset;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                wd = new WebDocument(null) { WebException = ex, UrlAddress = Request.RequestUri, ResponseItem = ParseResponse(response) };
            }
            wd.ResponseItem.Proxy = Request.Proxy;
            return wd;
        }

        public virtual void AppendRequestHeader(Action<WebRequest> appendRequestHeader)
        {
            if (appendRequestHeader != null)
            {
                appendRequestHeader(Request);
            }
        }
        
        private async Task<WebDocument> ParseWebDocument(WebResponse response)
        {
            WebDocument wd = new WebDocument(response.ResponseUri);
            var charset = ParseCharset(response.ContentType);
            var contentEncoding = response.Headers["Content-Encoding"];
            wd._Charset = charset;

            using (var stream = response.GetResponseStream())
            {
                if (!string.IsNullOrEmpty(contentEncoding) && IsGZipCompress(contentEncoding))//GZip 压缩
                {
                    using (GZipStream gzip = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        wd = await ParseStream(wd, gzip);
                    }
                }
                else//非压缩文件
                {
                    wd = await ParseStream(wd, stream);
                }
            }

            if (string.Compare(wd.DefaultEncode.BodyName, wd._Charset, true) != 0)
            {
                wd.DefaultEncode = Encoding.GetEncoding(wd._Charset);   
            }
            return wd;
        }

        /// <summary>
        /// 解析流到 web文档
        /// </summary>
        /// <param name="wd"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        private async Task<WebDocument> ParseStream(WebDocument wd,Stream stream)
        {
            int position = 0;
            while ((position = await stream.ReadAsync(wd.Buffer, 0, wd.Capacity)) > 0)
            {
                if (!string.IsNullOrEmpty(wd._Charset))
                {
                    wd.Html.Append(Encoding.GetEncoding(wd._Charset).GetString(wd.Buffer, 0, position));
                }
                else
                {
                    if (string.IsNullOrEmpty(wd._Charset))
                    {
                        wd.Html.Append(wd.DefaultEncode.GetString(wd.Buffer, 0, position));
                        wd._Charset = ParseCharset(wd.Html.ToString());
                        if (!string.IsNullOrEmpty(wd._Charset))
                        {
                            var contents = Encoding.Convert(wd.DefaultEncode, Encoding.GetEncoding(wd._Charset), wd.DefaultEncode.GetBytes(wd.Html.ToString()));
                            wd.Html.Clear();
                            wd.Html.Append(Encoding.GetEncoding(wd._Charset).GetString(contents, 0, contents.Length));
                        }
                    }
                    else
                    {
                        wd.Html.Append(Encoding.GetEncoding(wd._Charset).GetString(wd.Buffer, 0, position));
                    }
                }
            }
            return wd;
        }

        private string ParseCharset(string content)
        {
            string charSet = string.Empty;
            if (!string.IsNullOrEmpty(content))
            {
                Regex regCharSet = new Regex("charset=['\"]?(\\w+((-?(\\d+))+)?)['\"]?", RegexOptions.IgnoreCase);
                Match match = regCharSet.Match(content);
                if (match.Success)
                {
                    charSet = match.Groups[1].Value;
                }
            }
            return charSet;
        }

        private string ParseResponseLocation(string location,Uri requestUri)
        {
            if (location.StartsWith("//"))
            {
                return string.Format("{0}:{1}", requestUri.Scheme,location);
            }
            else if(location.StartsWith("/"))
            {
                return string.Format("{0}://{1}{2}", requestUri.Scheme, requestUri.Host, location);
            }
            return location;
        }

        private bool IsGZipCompress(string content)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(content))
            {
                Regex regex = new Regex("gzip", RegexOptions.IgnoreCase);
                Match match = regex.Match(content);
                result = match.Success;
            }
            return result;
        }
    }
}
