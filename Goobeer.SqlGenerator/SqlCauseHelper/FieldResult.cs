using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goobeer.DB.SqlCauseHelper
{
    public class FieldResult : ISqlResult
    {
        public StringBuilder FieldTxt { get; }
        public List<DbParameter> Params { get; set; }

        public FieldResult()
        {
            FieldTxt = new StringBuilder();
            Params = new List<DbParameter>();
        }

        public string RenderSql()
        {
            return FieldTxt.ToString();
        }
    }
}
