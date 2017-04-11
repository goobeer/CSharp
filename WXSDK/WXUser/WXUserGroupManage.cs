using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WXSDK.WXUser
{
    public class WXUserGroupManage
    {
        public void Create(string accessToken,string groupName)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/groups/create?access_token={0}", accessToken);
            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                var resp = wc.UploadString(url, string.Format("{1}\"group\":{1}\"name\":\"{0}\"{2}{2}",groupName,"{","}"));

            }
        }
    }
}
