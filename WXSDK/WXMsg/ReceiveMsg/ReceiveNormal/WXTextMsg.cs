using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WXSDK.WXMsg.ReceiveMsg.ReceiveNormal
{
    public class WXTextMsg : WXRecvMsgBase
    {
        public string Content { get; set; }

        public List<TxtWithImgItem> Articles { get; set; }
        public override void ParseMsg(XmlDocument doc)
        {
            if (doc!=null)
            {
                base.ParseMsg(doc);
                var contentNode = doc.SelectSingleNode("/xml/Content");
                if (contentNode!=null)
                {
                    Content = contentNode.InnerText;
                }
            }
        }

        public string SendMsg()
        {
            string xmlStr = string.Empty;
            if (!(!string.IsNullOrEmpty(ToUserName) && !string.IsNullOrEmpty(FromUserName) && !string.IsNullOrEmpty(Content)))
            {
                throw new ArgumentException();
            }
            xmlStr = string.Format(@"<xml>
<ToUserName><![CDATA[{0}]]></ToUserName>
<FromUserName><![CDATA[{1}]]></FromUserName>
<CreateTime>{2}</CreateTime>
<MsgType><![CDATA[text]]></MsgType>
<Content><![CDATA[{3}]]></Content>
</xml>", ToUserName, FromUserName, CreateTime,Content);
            return xmlStr;
        }

        public string SendTxtWithImg()
        {
            if (!(!string.IsNullOrEmpty(ToUserName) && !string.IsNullOrEmpty(FromUserName) && Articles != null && Articles.Any()))
            {
                throw new ArgumentException();
            }
            string xmlStr = string.Empty;
            string xmlTpl = @"<xml>
<ToUserName><![CDATA[{0}]]></ToUserName>
<FromUserName><![CDATA[{1}]]></FromUserName>
<CreateTime>{2}</CreateTime>
<MsgType><![CDATA[news]]></MsgType>
<ArticleCount>{3}</ArticleCount>
<Articles>
{4}
</Articles>
</xml>";
            string articleItemTpl = @"<item>
<Title><![CDATA[{0}]]></Title> 
<Description><![CDATA[{1}]]></Description>
<PicUrl><![CDATA[{2}]]></PicUrl>
<Url><![CDATA[{3}]]></Url>
</item>";
            StringBuilder sbArticles = new StringBuilder();

            foreach (var item in Articles)
            {
                sbArticles.AppendFormat(articleItemTpl, item.Title, item.Description, item.PicUrl, item.Url);
            }
            xmlStr = string.Format(xmlTpl, ToUserName, FromUserName, CreateTime, Articles.Count, sbArticles.ToString());
            return xmlStr;
        }
    }

    public class TxtWithImgItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string PicUrl { get; set; }
        public string Url { get; set; }
    }
}
