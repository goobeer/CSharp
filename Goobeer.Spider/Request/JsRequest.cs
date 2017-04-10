using System.Net;

namespace Goobeer.Spider.Request
{
    public class JsRequest:HttpRequestBase
    {

        public JsRequest(string url, HttpMethod method)
        {
            //base.InitRequest(url, method);
        }

        public JsRequest(string url, HttpMethod method, CookieContainer cookieContainer)
            : this(url, method)
        {
            //this.Request.CookieContainer = cookieContainer;
        }

        public void AppendRequestHeader(WebRequest request)
        {
            //request.Accept = "*/*";
            //request.ProtocolVersion = HttpVersion.Version11;
        }
    }
}
