using System.Collections.Generic;

namespace Goobeer.DB.SqlCauseHelper.Condition
{
    /// <summary>
    /// sql操作条件
    /// </summary>
    public class SqlCondition
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 参数化名称
        /// </summary>
        public List<string> ParamName { get; set; } 

        /// <summary>
        /// 字段值
        /// </summary>
        public dynamic FieldVal { get; set; }

        /// <summary>
        /// 操作条件
        /// </summary>
        public SqlOperator SqlOperation { get; set; }
        
        /// <summary>
        /// 和sql中的上一个条件之间的 运算符
        /// </summary>
        public ConditionOperator ConditionOperator { get; set; }
    }

    public class ConditionComponent
    {
        public List<SqlCondition> Conditions { get; set; }
        public ConditionOperator ConditionOperator { get; set; }
    }
}
