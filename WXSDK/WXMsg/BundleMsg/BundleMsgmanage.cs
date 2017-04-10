using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WXSDK.Models;

namespace WXSDK.WXMsg.BundleMsg
{
    /// <summary>
    /// 群发消息 
    /// </summary>
    public class BundleMsgmanage
    {
        /// <summary>
        /// 上传图文消息内的图片获取URL
        /// post-key:media
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public WXUploadFileRespMsg UploadImg(string accessToken, string fileName)
        {
            WXUploadFileRespMsg msg = null;
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/media/uploadimg?access_token={0}", accessToken);

            //TODO 读取 相应文件 文件流
            string responseMsg = string.Empty;
            JsonConvert.DeserializeObject<WXSDK.Models.WXUploadFileRespMsg>(responseMsg);
            return msg;
        }

        /// <summary>
        /// 上传图文消息素材
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="articles"></param>
        /// <returns></returns>
        public WXUploadArticleRespMsg UploadNews(string accessToken, List<WXUploadArticleItem> articles)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/media/uploadnews?access_token={0}", accessToken);

            WXUploadArticleRespMsg msg = null;
            using (WebClient wc = new WebClient() { Encoding=Encoding.UTF8})
            {
                var resp = wc.UploadString(url, JsonConvert.SerializeObject(articles));
                resp = string.Format("{1}\"articles\":{0}{2}", resp, "{", "}");
                msg = JsonConvert.DeserializeObject<WXUploadArticleRespMsg>(resp);
            }
            return msg;
        }

        /// <summary>
        /// 根据分组进行群发
        /// </summary>
        public BundleRespMsg BundleMsgByGroupID(string accessToken,string groupID,string data,BundleReqMsgType msgType)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/message/mass/sendall?access_token={0}", accessToken);

            BundleRespMsg msg = null;
            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                var resp = wc.UploadString(url,FormateBundleReqMsg(groupID,data,msgType));
                msg = JsonConvert.DeserializeObject<BundleRespMsg>(resp);
            }
            return msg;
        }

        /// <summary>
        /// 根据OpenID列表群发【订阅号不可用，服务号认证后可用】
        /// </summary>
        public void BundleMsgByOpenIDList(string accessToken)
        {

        }

        private string FormateBundleReqMsg(string groupID, string msgData, BundleReqMsgType msgType)
        {
            string data = string.Empty;
            string tpl = "{\"filter\":{\"is_to_all\":false,\"group_id\":{0}},\"{2}\":{\"{3}\":\"{1}\"},\"msgtype\":\"{2}\"}";
            switch (msgType)
            {
                case BundleReqMsgType.mpnews:
                case BundleReqMsgType.voice:
                case BundleReqMsgType.image:
                case BundleReqMsgType.mpvideo:
                    data = string.Format(tpl, groupID, data, msgType, "media_id");
                    break;
                case BundleReqMsgType.text:
                    data = string.Format(tpl, groupID, data, msgType, "content");
                    break;
                case BundleReqMsgType.wxcard:
                    data = string.Format(tpl, groupID, data, msgType, "card_id");
                    break;
                default:
                    break;
            }
            return data;
        }
    }
}
