using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WXSDK.Models
{
    public class WXMenu
    {
        public WXMenuType type { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string url { get; set; }
        public string media_id { get; set; }
    }

    public enum WXMenuType
    {
        click,//点击推事件
        view,//跳转URL
        scancode_push,//扫码推事件
        scancode_waitmsg,//扫码推事件且弹出“消息接收中”提示框
        pic_sysphoto,//弹出系统拍照发图
        pic_photo_or_album,//弹出拍照或者相册发图
        pic_weixin,//弹出微信相册发图器
        location_select,//弹出地理位置选择器
        media_id,//下发消息（除文本消息）
        view_limited//跳转图文消息URL
    }
}
