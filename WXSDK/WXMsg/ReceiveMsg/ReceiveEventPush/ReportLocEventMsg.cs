using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WXSDK.WXMsg.ReceiveMsg.ReceiveEventPush
{
    /// <summary>
    /// 上报地理位置事件
    /// </summary>
    public class ReportLocEventMsg:WXRecvMsgBase
    {
        public string Event { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Precision { get; set; }

        public override void ParseMsg(System.Xml.XmlDocument doc)
        {
            if (doc!=null)
            {
                base.ParseMsg(doc);
                var eventNode = doc.SelectSingleNode("/xml/Event");
                if (eventNode != null)
                {
                    Event = eventNode.InnerText;
                }
                var latitudeNode = doc.SelectSingleNode("/xml/Latitude");
                if (latitudeNode!=null)
                {
                    Latitude = latitudeNode.InnerText;
                }
                var longitudeNode = doc.SelectSingleNode("/xml/Longitude");
                if (longitudeNode!=null)
                {
                    Longitude = longitudeNode.InnerText;
                }
                var precisionNode = doc.SelectSingleNode("/xml/Precision");
                if (precisionNode!=null)
                {
                    Precision = precisionNode.InnerText;
                }
            }
        }
    }
}
