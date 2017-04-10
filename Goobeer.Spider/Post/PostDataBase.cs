using System;
using System.Text;
using System.Web.Security;

namespace Goobeer.Spider.Post
{
    /// <summary>
    /// post数据基类
    /// </summary>
    public abstract class PostDataBase
    {
        private static readonly string _flag = string.Format("{0}{1}", "goobeer", DateTime.Now.ToLocalTime());
        protected static readonly string _itemHeaderTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";

        /// <summary>
        /// post数据boundary最前边
        /// </summary>
        /// <param name="boundary"></param>
        /// <returns></returns>
        protected string BeforePostData(string boundary, ref byte[] data)
        {
            string beginBoundary = string.Format("\r\n--{0}\r\n", boundary);
            byte[] beginData = Encoding.ASCII.GetBytes(beginBoundary);
            data = beginData;
            return beginBoundary;
        }

        /// <summary>
        /// post数据boundary最后边
        /// </summary>
        /// <param name="boundary"></param>
        protected void AfterPostData(string boundary, ref byte[] data)
        {
            string endBoundary = string.Format("\r\n--{0}--\r\n", boundary);
            byte[] endData = Encoding.ASCII.GetBytes(endBoundary);
            data = data.Combine(endData);
        }

        /// <summary>
        /// post数据中的原子项头部
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fileName"></param>
        /// <param name="mimeType">文件的MimeType,当为空的时候，说明传输的是普通的文本数据</param>
        protected void AppendItemHeader(string key, string fileName, string mimeType, ref byte[] data)
        {
            string itemHeaderTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string itemHeader = string.Empty;
            if (string.IsNullOrEmpty(mimeType))
            {
                itemHeaderTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
            }
            itemHeader = string.Format(itemHeaderTemplate, key, fileName, mimeType);
            byte[] temp = Encoding.ASCII.GetBytes(itemHeader);
            data = data.Combine(temp);
        }

        protected string Boundary { get { return GetBoundary(); } }

        private string GetBoundary()
        {
            return Membership.CreateUser("zs", _flag).GetPassword();
            //return FormsAuthentication.HashPasswordForStoringInConfigFile(_flag, System.Web.Configuration.FormsAuthPasswordFormat.MD5.ToString());
        }
    }

    public static class Ext
    {
        public static byte[] Combine(this byte[] data1,byte[] data2)
        {
            byte[] result = new byte[data1.Length+data2.Length];
            data1.CopyTo(result, 0);
            data2.CopyTo(result, data1.Length);
            return result;
        }
    }
}
