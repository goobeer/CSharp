using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace Goobeer.Spider.Post
{
    /// <summary>
    /// post文件
    /// </summary>
    public class PostFile:PostDataBase,IPostDataStrategy
    {
        /// <summary>
        /// post普通文件
        /// </summary>
        public Dictionary<string, string> Files { get; set; }

        public Dictionary<string, string> Txts { get; set; }

        private static XmlDocument _XmlMimeType;
        private XmlDocument XmlMimeType
        {
            get
            {
                if (_XmlMimeType == null)
                {
                    _XmlMimeType = new XmlDocument();
                    _XmlMimeType.Load("MimeType.xml");
                }
                return _XmlMimeType;
            }
            set
            {
                _XmlMimeType = value;
            }
        }

        public PostFile(Dictionary<string, string> files, Dictionary<string, string> txts)
        {
            Files = files;
            Txts = txts;
        }

        public void AppendPostHeader(WebRequest request)
        {
            string boundary = string.Format("----{0}", Boundary);
            request.ContentType = string.Format("multipart/form-data;boundary={0}", boundary);
            byte[] buffer = new byte[0];
            string beginBoundary = BeforePostData(boundary, ref buffer);
            string mimeType = string.Empty;
            string fileName = string.Empty;
            int len = 0;

            if (Txts != null)
            {
                len = Txts.Keys.Count;
                if (Files.Count > 0 && len > 0)
                {
                    buffer = buffer.Combine(Encoding.ASCII.GetBytes(beginBoundary));
                }
                foreach (string key in Txts.Keys)
                {
                    len--;
                    AppendItemHeader(key, Txts[key], string.Empty, ref buffer);
                    if (len > 0)
                    {
                        buffer = buffer.Combine(Encoding.ASCII.GetBytes(beginBoundary));
                    }
                }
            }

            if (Files != null)
            {
                len = Files.Keys.Count;
                foreach (string key in Files.Keys)
                {
                    len--;
                    FileInfo fi = new FileInfo(Files[key]);
                    try
                    {
                        mimeType = XmlMimeType.SelectSingleNode(string.Format("/file/item[@ext='{0}']", fi.Extension)).Attributes["mimeType"].Value;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    if (string.IsNullOrEmpty(mimeType))
                    {
                        mimeType = "application/octet-stream";
                    }
                    AppendItemHeader(key, fi.Name, mimeType, ref buffer);
                    buffer = buffer.Combine(File.ReadAllBytes(Files[key]));
                    if (len > 0)
                    {
                        buffer = buffer.Combine(Encoding.ASCII.GetBytes(beginBoundary));
                    }
                }
            }
            
            AfterPostData(boundary, ref buffer);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
        }
    }
}
