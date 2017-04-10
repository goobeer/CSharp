using Goobeer.DB.SqlCauseHelper;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System;

namespace Goobeer.DB.SqlCauseHelper.Condition
{
    public class ConditonResult: ISqlResult
    {
        public StringBuilder CondtionText { get; set; }
        public List<DbParameter> Params { get; set; }

        public ConditonResult(bool useParam)
        {
            CondtionText = new StringBuilder();
            if (useParam)
            {
                Params = new List<DbParameter>();
            }
        }

        public string RenderSql()
        {
            return CondtionText.ToString();
        }
    }
}
