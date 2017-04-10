using WXSDK.WXMsg.ReceiveMsg;
using WXSDK.WXMsg.ReceiveMsg.Event;
using WXSDK.WXMsg.ReceiveMsg.ReceiveNormal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WXSDK.WXMsg.ReceiveMsg.ReceiveEventPush;

namespace WXSDK.WXMsg
{
    public class WXMsgFactory
    {
        public WXRecvMsgBase CreateRecvMsg(XmlDocument doc)
        {
            WXRecvMsgBase msgBase = null;
            if (doc!=null)
            {
                var msgType = doc.SelectSingleNode("/xml/MsgType");
                if (msgType!=null && !string.IsNullOrEmpty(msgType.InnerText))
                {
                    var msgTypeText = msgType.InnerText.ToLower();
                    switch (msgTypeText)
                    {
                        case "text":
                            msgBase = new WXTextMsg();
                            break;
                        case "image":
                            msgBase = new WXImageMsg();
                            break;
                        case "voice":
                            msgBase = new WXVoiceMsg();
                            break;
                        case "video":
                            msgBase = new WXVoiceMsg();
                            break;
                        case "shortvideo":
                            msgBase = new WXShortVideoMsg();
                            break;
                        case "location":
                            msgBase = new WXLocationMsg();
                            break;
                        case "link":
                            msgBase = new WXLocationMsg();
                            break;
                        case "event":
                            var eventNode = doc.SelectSingleNode("/xml/Event");
                            if (eventNode!=null)
                            {
                                var eventVal=eventNode.InnerText.ToLower();
                                switch (eventVal)
                                {
                                    case "subscribe":
                                    case "unsubscribe":
                                        var eventKeyNode = doc.SelectSingleNode("/xml/EventKey");
                                        var ticketNode = doc.SelectSingleNode("/xml/Ticket");
                                        if (eventKeyNode!=null && ticketNode!=null)
                                        {
                                            msgBase = new UnScanERCodeEventMsg();
                                        }
                                        else
                                        {
                                            msgBase = new FCEventMsg();
                                        }
                                        break;
                                    case "scan":
                                        msgBase = new ScanERCodeEventMsg();
                                        break;
                                    case "location":
                                        msgBase = new ReportLocEventMsg();
                                        break;
                                    case "click":
                                        msgBase = new CustomeMenuClickEventMsg();
                                        break;
                                    case "view":
                                        msgBase = new CustomeMenuViewEventMsg();
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    if (msgBase!=null)
                    {
                        msgBase.ParseMsg(doc);
                    }
                }
            }
            return msgBase;
        }
    }
}
