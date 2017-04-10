using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WXSDK.AllSend
{
    /// <summary>
    /// 群发
    /// </summary>
    public class SendAll
    {
        /// <summary>
        /// 上传图文消息内的图片获取URL:本接口所上传的图片不占用公众号的素材库中图片数量的5000个的限制。图片仅支持jpg/png格式，大小必须在1MB以下
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="fileName"></param>
        public void UploadPic(string accessToken,string fileName)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/media/uploadimg?access_token={0}", accessToken);
            using (WebClient client=new WebClient())
            {
                var res = client.UploadFile(url, fileName);
            }
        }
    }
}
