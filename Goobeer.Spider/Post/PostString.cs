using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace Goobeer.Spider.Post
{
    /// <summary>
    /// post string
    /// </summary>
    public class PostString : PostDataBase, IPostDataStrategy
    {
        public IDictionary<string,dynamic> FormDataDic { get; set; }

        public PostString(IDictionary<string,dynamic> formDataDic)
        {
            FormDataDic = formDataDic;
        }

        public void AppendPostHeader(WebRequest request)
        {
            request.ContentType = "application/x-www-form-urlencoded";
            List<string> list = new List<string>();
            foreach (string key in FormDataDic.Keys)
            {
                list.Add(string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(FormDataDic[key].ToString())));
            }
            string postDataStr = string.Join("&", list);
            request.ContentLength = Encoding.Default.GetByteCount(postDataStr);
            if (request.RequestUri.Scheme.StartsWith("https",StringComparison.OrdinalIgnoreCase))
            {
                request.PreAuthenticate = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback((obj, certificate, chain, sslPolicyErrors) => { return true; });
            }
            
            Stream stream = request.GetRequestStream();
            StreamWriter sw = new StreamWriter(stream, Encoding.Default);
            sw.Write(postDataStr);
            sw.Close();
        }
    }
}
