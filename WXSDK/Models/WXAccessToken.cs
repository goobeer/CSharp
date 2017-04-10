using System;

namespace WXSDK.Models
{
    public class WXAccessToken
    {
        public string APPID { get; set; }
        public string AppSecret { get; set; }
        public DateTime VerfiyTime { get; set; }
        public DateTime ExpiredTime { get; set; }

        public bool IsFirst { get; set; }

        public string access_token { get; set; }

        public int expires_in { get; set; }

        public int errcode { get; set; }

        public string errmsg { get; set; }
    }
}