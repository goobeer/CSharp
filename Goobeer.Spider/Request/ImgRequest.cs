using System.Net;

namespace Goobeer.Spider.Request
{
    public class ImgRequest:HttpRequestBase
    {
        public ImgRequest(string url, HttpMethod method)
        {
            //base.InitRequest(url, method);
        }

        public ImgRequest(string url, HttpMethod method, CookieContainer cookieContainer)
            : this(url, method)
        {
            //this.Request.CookieContainer = cookieContainer;
        }

        public void AppendRequestHeader(WebRequest request)
        {
            //this.Request.Accept = "image/png,image/*;q=0.8,*/*;q=0.5";
            //Request.ProtocolVersion = HttpVersion.Version11;
        }
    }
}
