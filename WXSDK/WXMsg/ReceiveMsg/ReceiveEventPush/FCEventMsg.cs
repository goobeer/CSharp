using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WXSDK.WXMsg.ReceiveMsg.ReceiveEventPush
{
    /// <summary>
    /// 关注/取消关注事件
    /// </summary>
    public class FCEventMsg:WXRecvMsgBase
    {
        public string Event { get; set; }

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
            }
        }
    }
}
