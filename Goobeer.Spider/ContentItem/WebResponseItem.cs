using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Goobeer.Spider.ContentItem
{
    public class WebResponseItem
    {
        public WebHeaderCollection Headers { get; set; }
        public long ContentLength { get; set; }
        public string CharacterSet { get; set; }
        public bool IsFromCache { get; set; }
        public bool IsMutuallyAuthenticated { get; set; }
        public bool SupportsHeaders { get; set; }
        public Uri ResponseUri { get; set; }
        public CookieCollection Cookies { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Server { get; set; }

        public IWebProxy Proxy { get; set; }

        public WebResponseItem(WebResponse response)
        {
            StoreResponse(response);
        }

        public void StoreResponse(WebResponse response)
        {
            if (response != null)
            {
                try
                {
                    var rp = (HttpWebResponse)response;
                    ContentLength = rp.ContentLength;
                    SupportsHeaders = rp.SupportsHeaders;
                    CharacterSet = rp.CharacterSet;
                    Cookies = rp.Cookies;
                    Headers = rp.Headers;
                    IsFromCache = rp.IsFromCache;
                    IsMutuallyAuthenticated = rp.IsMutuallyAuthenticated;
                    ResponseUri = rp.ResponseUri;
                    StatusCode = rp.StatusCode;
                    Server = rp.Server;
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
