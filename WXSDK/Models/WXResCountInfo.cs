using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WXSDK.Models
{
    public class WXResCountInfo
    {
        public int voice_count { get; set; }
        public int video_count { get; set; }
        public int image_count { get; set; }
        public int news_count { get; set; }
    }
}
