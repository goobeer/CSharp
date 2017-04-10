using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WXSDK.WXMsg.ReceiveMsg.ReceiveNormal
{
    public class WXVideoMsg:WXRecvMsgBase
    {
        public string MediaId { get; set; }
        public string ThumbMediaId { get; set; }

        #region 回复消息字段
        public string Title { get; set; }
        public string Description { get; set; }
        public string MusicUrl { get; set; }
        public string HQMusicUrl { get; set; }
        #endregion

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

                var thumbMediaIDNode = doc.SelectSingleNode("/xml/ThumbMediaId");
                if (thumbMediaIDNode!=null)
                {
                    ThumbMediaId = thumbMediaIDNode.InnerText;
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
<MsgType><![CDATA[video]]></MsgType>
<Video>
<MediaId><![CDATA[{3}]]></MediaId>
<Title><![CDATA[{4}]]></Title>
<Description><![CDATA[{5}]]></Description>
</Video> 
</xml>", ToUserName, FromUserName, CreateTime, MediaId, Title, Description);
            return xmlStr;
        }

        public string SendMusicMsg()
        {
            if (!(!string.IsNullOrEmpty(ToUserName) && !string.IsNullOrEmpty(FromUserName)))
            {
                throw new ArgumentException();
            }

            string xmlStr = string.Empty;
            xmlStr = string.Format(@"<xml>
<ToUserName><![CDATA[{0}]]></ToUserName>
<FromUserName><![CDATA[{1}]]></FromUserName>
<CreateTime>{2}</CreateTime>
<MsgType><![CDATA[music]]></MsgType>
<Music>
<Title><![CDATA[{3}]]></Title>
<Description><![CDATA[{4}]]></Description>
<MusicUrl><![CDATA[{5}]]></MusicUrl>
<HQMusicUrl><![CDATA[{6}]]></HQMusicUrl>
<ThumbMediaId><![CDATA[{7}]]></ThumbMediaId>
</Music>
</xml>", ToUserName, FromUserName, CreateTime, Title, Description, MusicUrl, HQMusicUrl, ThumbMediaId);
            return xmlStr;
        }
    }

    public class WXShortVideoMsg : WXVideoMsg
    {

    }
}
