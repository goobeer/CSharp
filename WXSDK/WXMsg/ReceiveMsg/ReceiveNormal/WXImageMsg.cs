using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WXSDK.WXMsg.ReceiveMsg.ReceiveNormal
{
    public class WXImageMsg:WXRecvMsgBase
    {
        public string PicUrl { get; set; }
        public string MediaId { get; set; }

        public override void ParseMsg(XmlDocument doc)
        {
            if (doc!=null)
            {
                base.ParseMsg(doc);
                var picUrlNode = doc.SelectSingleNode("/xml/PicUrl");
                if (picUrlNode!=null)
                {
                    PicUrl = picUrlNode.InnerText;
                }
                var mediaIdNode = doc.SelectSingleNode("/xml/MediaId");
                if (mediaIdNode != null)
                {
                    MediaId = mediaIdNode.InnerText;
                }
            }
        }

        public string SendMsg()
        {
            string xmlStr = string.Empty;
            if (!(!string.IsNullOrEmpty(ToUserName) && !string.IsNullOrEmpty(FromUserName) && !string.IsNullOrEmpty(MediaId)))
            {
                throw new ArgumentException();
            }
            xmlStr = string.Format(@"<xml>
<ToUserName><![CDATA[{0}]]></ToUserName>
<FromUserName><![CDATA[{1}]]></FromUserName>
<CreateTime>{2}</CreateTime>
<MsgType><![CDATA[image]]></MsgType>
<Image>
<MediaId><![CDATA[{3}]]></MediaId>
</Image>
</xml>", ToUserName, FromUserName, CreateTime, MsgType, MediaId);
            return xmlStr;
        }
    }
}
