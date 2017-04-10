using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WXSDK.Models;
using Newtonsoft.Json;

namespace WXSDK.WXMsg.KFMsg
{
    public class KFAccountManage
    {
        public WXResponseState Add(string accessToken,WXKFAccount account)
        {
            WXResponseState state = null;
            string url = string.Format("https://api.weixin.qq.com/customservice/kfaccount/add?access_token={0}", accessToken);
            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                state = JsonConvert.DeserializeObject<WXResponseState>(wc.UploadString(url, JsonConvert.SerializeObject(accessToken)));
            }
            return state;
        }

        public WXResponseState Update(string accessToken, WXKFAccount account)
        {
            WXResponseState state = null;
            string url = string.Format("https://api.weixin.qq.com/customservice/kfaccount/update?access_token={0}", accessToken);
            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                state = JsonConvert.DeserializeObject<WXResponseState>(wc.UploadString(url, JsonConvert.SerializeObject(accessToken)));
            }
            return state;
        }

        public WXResponseState Del(string accessToken, WXKFAccount account)
        {
            WXResponseState state = null;
            string url = string.Format("https://api.weixin.qq.com/customservice/kfaccount/del?access_token={0}", accessToken);
            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                state = JsonConvert.DeserializeObject<WXResponseState>(wc.UploadString(url, JsonConvert.SerializeObject(accessToken)));
            }
            return state;
        }

        public string GetAllKf(string accessToken)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/customservice/getkflist?access_token={0}", accessToken);
            string kfs = string.Empty;
            using (WebClient wc = new WebClient() { Encoding=Encoding.UTF8})
            {
                kfs = wc.DownloadString(url);
            }
            return kfs;
        }

        /// <summary>
        /// 头像图片文件必须是jpg格式，推荐使用640*640大小的图片以达到最佳效果
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="kfAccount"></param>
        public WXResponseState SetKFHeader(string accessToken,string kfAccount,string fileName)
        {
            string url = string.Format("http://api.weixin.qq.com/customservice/kfaccount/uploadheadimg?access_token={0}&kf_account={1}", accessToken,kfAccount);
            WXResponseState state = null;
            using (WebClient wc = new WebClient() { Encoding=Encoding.UTF8})
            {
                var stateStr = Encoding.UTF8.GetString(wc.UploadFile(url, fileName));
                state = JsonConvert.DeserializeObject<WXResponseState>(stateStr);
            }
            return state;
        }
    }
}
