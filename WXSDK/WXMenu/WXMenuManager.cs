using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using WXSDK.Models;
using Newtonsoft.Json;

namespace WXSDK.WXMenu
{
    public class WXMenuManager
    {
        public WXResponseState Create(string accessToken,string btnJson)
        {
            WXResponseState state = null;
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}",accessToken);

            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                var data = wc.UploadString(url, btnJson);
                state = JsonConvert.DeserializeObject<WXResponseState>(data);
            }
            return state;
        }

        public string Query(string accessToken)
        {
            string menuInfo = string.Empty;
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/get?access_token={0}", accessToken);
            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                menuInfo = wc.DownloadString(url);
            }
            return menuInfo;
        }

        public WXResponseState Delete(string accessToken)
        {
            WXResponseState state = null;
            string url=string.Format("https://api.weixin.qq.com/cgi-bin/menu/delete?access_token={0}",accessToken);
            using (WebClient wc=new WebClient())
            {
                state = JsonConvert.DeserializeObject<WXResponseState>(wc.DownloadString(url));
            }
            return state;
        }

    }
}
