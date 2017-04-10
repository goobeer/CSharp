using System;
using System.Text;

namespace Goobeer.Spider.ContentItem
{
    /// <summary>
    /// web文档
    /// </summary>
    public class WebDocument
    {
        private int _Capacity = 4 * 1024;
        public int Capacity {
            get { return _Capacity; }
            set
            {
                if (value<=0 || value>=int.MaxValue)
                {
                    throw new Exception("Capacity Error");
                }
                _Capacity = value;
            }
        }

        private byte[] _Buffer;
        public byte[] Buffer
        {
            get
            {
                if (_Buffer == null)
                {
                    _Buffer = new byte[Capacity];
                }
                return _Buffer;
            }
            set
            {
                _Buffer = new byte[Capacity];
            }
        }

        public Exception WebException { get; set; }
        public Uri ParentUrlAddress { get; set; }

        public Uri UrlAddress { get; set; }

        public StringBuilder Html { get; set; }

        private Encoding _defaultEncode = Encoding.UTF8;

        /// <summary>
        /// 默认的编码格式
        /// </summary>
        public Encoding DefaultEncode
        {
            get { return _defaultEncode; }
            set
            {
                _defaultEncode = value;
            }
        }

        public string _Charset;

        public WebResponseItem ResponseItem { get; set; }

        public WebDocument(Uri urlAddress)
        {
            UrlAddress = urlAddress;
            Html = new StringBuilder();
        }

        public void ClearState()
        {
            Html = Html.Clear();
            _defaultEncode = Encoding.UTF8;
            UrlAddress = null;
            _Charset = null;
        }
    }
}
