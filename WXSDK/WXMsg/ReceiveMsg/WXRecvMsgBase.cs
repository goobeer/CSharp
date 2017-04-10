using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WXSDK.WXMsg.ReceiveMsg
{
    public abstract class WXRecvMsgBase:WXMsgBase
    {
        public Int64 MsgId { get; set; }

        public virtual void ParseMsg(XmlDocument doc)
        {
            if (doc != null)
            {
                var toUserNameNode = doc.SelectSingleNode("/xml/ToUserName");
                if (toUserNameNode != null)
                {
                    ToUserName = toUserNameNode.InnerText;
                }

                var fromUserNameNode = doc.SelectSingleNode("/xml/FromUserName");
                if (fromUserNameNode != null)
                {
                    FromUserName = fromUserNameNode.InnerText;
                }

                var CreateTimeNode = doc.SelectSingleNode("/xml/CreateTime");
                if (CreateTimeNode != null)
                {
                    CreateTime = UInt64.Parse(CreateTimeNode.InnerText);
                }
                var msgIDNode = doc.SelectSingleNode("/xml/MsgId");
                if (msgIDNode != null)
                {
                    MsgId = Int64.Parse(msgIDNode.InnerText);
                }
            }
        }
    }
}
