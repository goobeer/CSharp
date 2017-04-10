using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WXSDK.Models;

namespace WXSDK
{
    public class WXMain
    {
        static object obj = new object();

        public bool CheckSignature(string token, string signature, string timestamp, string nonce)
        {
            var arr = new string[] { token, timestamp, nonce }.OrderBy(s => s);
            var sha1Provider = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            var sha1Data = sha1Provider.ComputeHash(System.Text.Encoding.Default.GetBytes(string.Join(string.Empty, arr)));
            StringBuilder sb = new StringBuilder();
            foreach (var item in sha1Data)
            {
                sb.AppendFormat("{0:x2}", item);
            }
            return sb.ToString() == signature;
        }

        public WXAccessToken GetWXAccessToken(string appid, string appsecret, Func<string, WXAccessToken> accessTokenCacheFunc)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", appid, appsecret);

            WXAccessToken wxAccessToken = null;
            lock (obj)
            {
                wxAccessToken = accessTokenCacheFunc(appid);
                var now = DateTime.Now;
                if (!(wxAccessToken != null && wxAccessToken.VerfiyTime <= now && wxAccessToken.ExpiredTime >= now))
                {
                    bool isFirst = wxAccessToken == null;
                    using (System.Net.WebClient client = new System.Net.WebClient())
                    {
                        try
                        {
                            var data = client.DownloadString(url);
                            var newWxAccessToken = JsonConvert.DeserializeObject<WXAccessToken>(data);
                            newWxAccessToken.APPID = appid;
                            newWxAccessToken.AppSecret = appsecret;
                            newWxAccessToken.VerfiyTime = DateTime.Now;
                            newWxAccessToken.ExpiredTime = newWxAccessToken.VerfiyTime.AddSeconds(newWxAccessToken.expires_in);
                            wxAccessToken = newWxAccessToken;
                            wxAccessToken.IsFirst = isFirst;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }
            return wxAccessToken;
        }
    }

    public static class DateTimeExtension
    {
        public static readonly DateTime ComputerYear = DateTime.Parse("1970/1/1");

        public static int DateParseInt(this DateTime date)
        {
            return (int)(date - DateTimeExtension.ComputerYear).TotalSeconds;
        }

        public static DateTime IntParseDate(this int timeStamp)
        {
            return DateTimeExtension.ComputerYear.AddSeconds(timeStamp);
        }
    }
}
