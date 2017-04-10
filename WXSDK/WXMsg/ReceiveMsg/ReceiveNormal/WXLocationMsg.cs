using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WXSDK.WXMsg.ReceiveMsg.ReceiveNormal
{
    public class WXLocationMsg:WXRecvMsgBase
    {
        public string Location_X { get; set; }
        public string Location_Y { get; set; }
        public string Scale { get; set; }
        public string Label { get; set; }

        public override void ParseMsg(XmlDocument doc)
        {
            if (doc!=null)
            {
                base.ParseMsg(doc);
                var locXNode = doc.SelectSingleNode("/xml/Location_X");
                if (locXNode!=null)
                {
                    Location_X = locXNode.InnerText;
                }
                var locYNode = doc.SelectSingleNode("/xml/Location_Y");
                if (locYNode != null)
                {
                    Location_Y = locYNode.InnerText;
                }

                var scaleNode = doc.SelectSingleNode("/xml/Scale");
                if (scaleNode != null)
                {
                    Scale = scaleNode.InnerText;
                }

                var labelNode = doc.SelectSingleNode("/xml/Label");
                if (labelNode != null)
                {
                    Label = labelNode.InnerText;
                }
            }
        }
    }
}
