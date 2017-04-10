using System;
using System.Net;

namespace Goobeer.Spider.Request
{
    /// <summary>
    /// http 请求
    /// </summary>
    public abstract class RequestHeader
    {
        /// <summary>
        /// 文本类型的HttpRequest
        /// </summary>
        public HttpWebRequest Request { get; set; }

        public abstract void AppendRequestHeader(HttpWebRequest request);

        /// <summary>
        /// 设置HttpRequest公共的header信息
        /// </summary>
        /// <param name="request"></param>
        private void HttpRequestCommonAppend(HttpWebRequest request)
        {
            request.KeepAlive = true;
            request.AllowAutoRedirect = true;
            request.Headers.Add("Accept-Encoding: gzip, deflate, sdch");
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2062.58 Safari/537.36";
            request.ServicePoint.Expect100Continue = true;
        }

        /// <summary>
        /// 设置 HttpRequest请求头信息
        /// </summary>
        /// <param name="request"></param>
        protected virtual void SetRequestHeader(HttpWebRequest request)
        {
            this.HttpRequestCommonAppend(request);
            this.AppendRequestHeader(request);
        }

        #region 初始化HttpRequest头信息
        protected void InitRequestHeader(string url, HttpMethod method)
        {
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    Request = (HttpWebRequest)HttpWebRequest.Create(url);
                    Request.Method = method.ToString();
                    this.SetRequestHeader(Request);
                }
                catch (Exception ex)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine(string.Format("异常的url地址:{0},异常信息:{1}", url, ex.Message));
                    Console.BackgroundColor = ConsoleColor.Black;
                }
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("异常的url地址:{0}", url));
                Console.BackgroundColor = ConsoleColor.Black;
            }
        }
        #endregion
    }
}
