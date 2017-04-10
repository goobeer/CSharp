using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WXSDK.WXMsg.ReceiveMsg.Event
{
    /// <summary>
    /// 扫码推事件的事件推送
    /// </summary>
    public class ScanCodePushEventMsg : CustomeMenuClickEventMsg
    {
        public string ScanType { get; set; }
        public string ScanResult { get; set; }

        public override void ParseMsg(System.Xml.XmlDocument doc)
        {
            if (doc!=null)
            {
                base.ParseMsg(doc);
                var scanTypeNode = doc.SelectSingleNode("/xml/ScanCodeInfo/ScanType");
                if (scanTypeNode!=null)
                {
                    ScanType = scanTypeNode.InnerText;
                }
                var scanResultNode = doc.SelectSingleNode("/xml/ScanCodeInfo/ScanResult");
                if (scanResultNode!=null)
                {
                    ScanResult = scanResultNode.InnerText;
                }
            }
        }
    }

    /// <summary>
    /// 扫码推事件且弹出“消息接收中”提示框的事件推送
    /// </summary>
    public class ScanCodeWaitmsgEventMsg:ScanCodePushEventMsg
    {
    }
}
