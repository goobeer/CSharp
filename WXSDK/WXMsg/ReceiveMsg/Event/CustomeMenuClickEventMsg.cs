using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WXSDK.WXMsg.ReceiveMsg.Event
{
    /// <summary>
    /// 点击菜单拉取消息时的事件推送
    /// </summary>
    public class CustomeMenuClickEventMsg:WXRecvMsgBase
    {
        public string Event { get; set; }
        public string EventKey { get; set; }

        public override void ParseMsg(System.Xml.XmlDocument doc)
        {
            if (doc!=null)
            {
                base.ParseMsg(doc);
                var eventNode = doc.SelectSingleNode("/xml/Event");
                if (eventNode!=null)
                {
                    Event = eventNode.InnerText;
                }
                var eventKeyNode = doc.SelectSingleNode("/xml/EventKey");
                if (eventKeyNode!=null)
                {
                    EventKey = eventKeyNode.InnerText;
                }
            }
        }
    }

    /// <summary>
    /// 点击菜单跳转链接时的事件推送
    /// </summary>
    public class CustomeMenuViewEventMsg : CustomeMenuClickEventMsg
    {
        public string MenuId { get; set; }

        public override void ParseMsg(System.Xml.XmlDocument doc)
        {
            if (doc!=null)
            {
                base.ParseMsg(doc);
                var menuIDNode = doc.SelectSingleNode("/xml/MenuId");
                if (menuIDNode!=null)
                {
                    MenuId = menuIDNode.InnerText;
                }
            }
        }
    }
}
