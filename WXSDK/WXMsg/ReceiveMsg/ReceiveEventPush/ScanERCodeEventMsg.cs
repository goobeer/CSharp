using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WXSDK.WXMsg.ReceiveMsg.ReceiveEventPush
{
    /// <summary>
    /// 用户已关注时的事件推送
    /// </summary>
    public class ScanERCodeEventMsg:WXRecvMsgBase
    {
        public string Event { get; set; }
        public string EventKey { get; set; }
        public string Ticket { get; set; }

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
                var eventKeyNode = doc.SelectSingleNode("/xml/EventKey");
                if (eventKeyNode != null)
                {
                    EventKey = eventKeyNode.InnerText;
                }
                var ticketNode = doc.SelectSingleNode("/xml/Ticket");
                if (ticketNode!=null)
                {
                    Ticket = ticketNode.InnerText;
                }
            }
        }
    }

    /// <summary>
    /// 用户未关注时，进行关注后的事件推送
    /// </summary>
    public class UnScanERCodeEventMsg:ScanERCodeEventMsg
    {

    }
}
