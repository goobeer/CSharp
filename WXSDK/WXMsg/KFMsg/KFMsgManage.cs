using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WXSDK.WXMsg.KFMsg
{
    public class KFMsgManage
    {
        static readonly string UrlTpl = "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={0}";

        public void SendTxtMsg(string accessToken, string openid, string content)
        {
            string txtMsgTpl = string.Format("{\"touser\":\"{0}\",\"msgtype\":\"text\",\"text\":{\"content\":\"{1}\"}}", openid, content);
            string url = string.Format(UrlTpl, accessToken);
            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                wc.UploadString(url, txtMsgTpl);
            }
        }

        public void SendImgMsg(string accessToken, string openid, string mediaID)
        {
            string imgMsgTpl = string.Format("{\"touser\":\"{0}\",\"msgtype\":\"image\",\"image\":{\"media_id\":\"{1}\"}}", openid, mediaID);
            string url = string.Format(UrlTpl, accessToken);
            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                wc.UploadString(url, imgMsgTpl);
            }
        }

        public void SendVoiceMsg(string accessToken, string openid, string mediaID)
        {
            string voiceMsgTpl = string.Format("{\"touser\":\"{0}\",\"msgtype\":\"voice\",\"voice\":{\"media_id\":\"{1}\"}}", openid, mediaID);
            string url = string.Format(UrlTpl, accessToken);
            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                wc.UploadString(url, voiceMsgTpl);
            }
        }

        public void SendVideoMsg(string accessToken, string openid, string mediaID,string tmediaID,string title,string desc)
        {
            string voiceMsgTpl = string.Format("{\"touser\":\"{0}\",\"msgtype\":\"video\",\"video\":{\"media_id\":\"{1}\",\"thumb_media_id\":\"MEDIA_ID\",\"title\":\"{2}\",\"description\":\"{3}\"}}", openid, mediaID, tmediaID, title, desc);
            string url = string.Format(UrlTpl, accessToken);
            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                wc.UploadString(url, voiceMsgTpl);
            }
        }

        public void SendMusicMsg(string accessToken, string openid, string title, string desc, string mscUrl, string hqUrl,string tmID)
        {
            string voiceMsgTpl = string.Format("{\"touser\":\"{0}\",\"msgtype\":\"music\",\"music\":{\"title\":\"{1}\",\"description\":\"{2}\",\"musicurl\":\"{3}\",\"hqmusicurl\":\"{4}\",\"thumb_media_id\":\"{5}\"}}", openid, title, desc, mscUrl, hqUrl, tmID);
            string url = string.Format(UrlTpl, accessToken);
            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                wc.UploadString(url, voiceMsgTpl);
            }
        }

        public void SendOuterTxtWithImg(string accessToken,string opendid,List<KFArticle> articles)
        {
            string url = string.Format(UrlTpl, accessToken);
            string articleItemTpl = "{\"title\":\"{0}\",\"description\":\"{1}\",\"url\":\"{2}\",\"picurl\":\"{3}\"}";
            List<string> articleItems = new List<string>();
            foreach (var item in articles)
            {
                articleItems.Add(string.Format(articleItemTpl, item.Title, item.Desc, item.Url, item.PicUrl));
            }
            string tpl = "{\"touser\":\"{0}\",\"msgtype\":\"news\",\"news\":{\"articles\": [{1}]}}";
            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                wc.UploadString(url,string.Format(tpl,opendid,string.Join(",",articleItems)));
            }
        }

        public void SendInnerTxtWithImg(string accessToken,string openid,string mediaID)
        {
            string url = string.Format(UrlTpl, accessToken);
            string tpl = "{\"touser\":\"{0}\",\"msgtype\":\"mpnews\",\"mpnews\":{\"media_id\":\"{1}\"}}";
            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                wc.UploadString(url, string.Format(tpl, openid, mediaID));
            }
        }

        //TODO 客服 发送卡券

    }

    public class KFArticle
    {
        public string Title { get; set; }
        public string Desc { get; set; }
        public string Url { get; set; }
        public string PicUrl { get; set; }
    }
}
