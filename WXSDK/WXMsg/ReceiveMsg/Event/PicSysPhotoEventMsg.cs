using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WXSDK.WXMsg.ReceiveMsg.Event
{
    /// <summary>
    /// 弹出系统拍照发图的事件推送
    /// </summary>
    public class PicSysPhotoEventMsg : CustomeMenuClickEventMsg
    {
        public string SendPicsCount { get; set; }
        public List<string> PicMd5Sums { get; set; }

        public override void ParseMsg(System.Xml.XmlDocument doc)
        {
            if (doc!=null)
            {
                base.ParseMsg(doc);

                var countNode = doc.SelectSingleNode("/xml/SendPicsInfo/Count");
                if (countNode!=null)
                {
                    SendPicsCount = countNode.InnerText;
                }

                var picListNodes = doc.SelectNodes("/xml/SendPicsInfo/PicList/item/PicMd5Sum");
                if (picListNodes!=null)
                {
                    PicMd5Sums = new List<string>();
                    foreach (System.Xml.XmlNode item in picListNodes)
                    {
                        PicMd5Sums.Add(item.InnerText);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 弹出拍照或者相册发图的事件推送
    /// </summary>
    public class PicPhotoOrAlbumEventMsg:PicSysPhotoEventMsg
    {

    }

    /// <summary>
    /// 弹出微信相册发图器的事件推送
    /// </summary>
    public class PicWeixinEventMsg:PicSysPhotoEventMsg
    {

    }
}
