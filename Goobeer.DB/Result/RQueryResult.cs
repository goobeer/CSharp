using Goobeer.DB.DbBase;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using Goobeer.DB.Command;

namespace Goobeer.DB.Result
{
    /// <summary>
    /// (关系数据库)单表、多表查询
    /// </summary>
    public abstract class RQueryResult: ICommandResult
    {
        public ConditionResult ConditionResult { get; set; }

        #region 单表查询
        public List<string> QueryFields { get; set; }

        public string TabName { get; set; }

        public DbConditionCollection DBCC { get; set; }
        #endregion

        #region 多表查询
        public List<JoinTableEntity> JoinTableEntity { get; set; }

        public IDictionary<string, DbConditionCollection> JoinTableEntityConditions { get; set; }
        #endregion

        public string RenderCommandResult(BaseCmdData data)
        {
            StringBuilder result = new StringBuilder();
            
            return result.ToString();
        }
    }
}
