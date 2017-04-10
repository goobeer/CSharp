using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WXSDK.WXMsg.ReceiveMsg.Event
{
    /// <summary>
    /// 弹出地理位置选择器的事件推送
    /// </summary>
    public class LocationSelectEventMsg : CustomeMenuClickEventMsg
    {
        public string Location_X { get; set; }
        public string Location_Y { get; set; }
        public string Scale { get; set; }
        public string Label { get; set; }
        public string Poiname { get; set; }

        public override void ParseMsg(System.Xml.XmlDocument doc)
        {
            if (doc!=null)
            {
                base.ParseMsg(doc);
                var locationXNode = doc.SelectSingleNode("/xml/SendLocationInfo/Location_X");
                if (Location_X!=null)
                {
                    Location_X = locationXNode.InnerText;
                }
                var locationYNode = doc.SelectSingleNode("/xml/SendLocationInfo/Location_Y");
                if (Location_Y!=null)
                {
                    Location_Y = locationYNode.InnerText;
                }

                var scaleNode = doc.SelectSingleNode("/xml/SendLocationInfo/Scale");
                if (scaleNode != null)
                {
                    Scale = scaleNode.InnerText;
                }
                var labelNode = doc.SelectSingleNode("/xml/SendLocationInfo/Label");
                if (labelNode != null)
                {
                    Label = labelNode.InnerText;
                }
                var poinameNode = doc.SelectSingleNode("/xml/SendLocationInfo/Poiname");
                if (poinameNode != null)
                {
                    Poiname = poinameNode.InnerText;
                }
            }
        }
    }
}
