using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WXSDK.Models
{
    public class BundleRespMsg
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public int msg_id { get; set; }
        public int msg_data_id { get; set; }
    }
}
