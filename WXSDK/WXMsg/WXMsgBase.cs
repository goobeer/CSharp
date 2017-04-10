using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WXSDK.WXMsg
{
    public abstract class WXMsgBase
    {
        public string ToUserName { get; set; }
        public string FromUserName { get; set; }
        public UInt64 CreateTime { get; set; }
        public string MsgType { get; set; }
    }
}
