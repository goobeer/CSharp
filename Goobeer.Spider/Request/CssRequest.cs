using System.Net;

namespace Goobeer.Spider.Request
{
    public class CssRequest:HttpRequestBase
    {
        public CssRequest(string url, HttpMethod method)
        {
            //base.InitRequest(url, method);
        }

        public CssRequest(string url, HttpMethod method, CookieContainer cookieContainer)
            : this(url, method)
        {
            //this.Request.CookieContainer = cookieContainer;
        }


        public void AppendRequestHeader(WebRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}
