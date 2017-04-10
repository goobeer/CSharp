using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WXSDK.WXMsg.ReceiveMsg.ReceiveNormal
{
    public class WXVoiceMsg : WXRecvMsgBase
    {
        public string MediaId { get; set; }
        public string Format { get; set; }

        public override void ParseMsg(XmlDocument doc)
        {
            if (doc!=null)
            {
                base.ParseMsg(doc);
                var mediaIDNode = doc.SelectSingleNode("/xml/MediaId");
                if (mediaIDNode!=null)
                {
                    MediaId = mediaIDNode.InnerText;
                }
                var formateNode = doc.SelectSingleNode("/xml/Format");
                if (formateNode!=null)
                {
                    Format = formateNode.InnerText;
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
<MsgType><![CDATA[voice]]></MsgType>
<Voice>
<MediaId><![CDATA[{3}]]></MediaId>
</Voice>
</xml>", ToUserName, FromUserName, CreateTime, MediaId);
            return xmlStr;
        }
    }
}
