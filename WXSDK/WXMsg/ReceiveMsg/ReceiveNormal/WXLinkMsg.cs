using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WXSDK.WXMsg.ReceiveMsg.ReceiveNormal
{
    public class WXLinkMsg:WXRecvMsgBase
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }

        public override void ParseMsg(XmlDocument doc)
        {
            if (doc!=null)
            {
                base.ParseMsg(doc);
                var titleNode = doc.SelectSingleNode("/xml/Title");
                if (titleNode!=null)
                {
                    Title = titleNode.InnerText;
                }

                var descNode = doc.SelectSingleNode("/xml/Description");
                if (descNode!=null)
                {
                    Description = descNode.InnerText;
                }

                var urlNode = doc.SelectSingleNode("/xml/Url");
                if (urlNode!=null)
                {
                    Url = urlNode.InnerText;
                }
            }
        }
    }
}
