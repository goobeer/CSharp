using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WXSDK.Models
{
    public enum BundleReqMsgType
    {
        /// <summary>
        /// 图文消息
        /// </summary>
        mpnews,
        /// <summary>
        /// 文本
        /// </summary>
        text,
        /// <summary>
        /// 语音
        /// </summary>
        voice,
        /// <summary>
        /// 图片
        /// </summary>
        image,
        /// <summary>
        /// 视频
        /// </summary>
        mpvideo,
        /// <summary>
        /// 卡券消息
        /// </summary>
        wxcard,
    }
}
