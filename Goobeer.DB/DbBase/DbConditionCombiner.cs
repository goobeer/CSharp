using System.Collections.Generic;

namespace Goobeer.DB.DbBase
{
    /// <summary>
    /// 条件合成器
    /// </summary>
    public class DbConditionCombiner
    {
        /// <summary>
        /// 左条件
        /// </summary>
        public DbCondition L { get; set; }

        /// <summary>
        /// 右条件(若为单表检索,则条件集合存储在R上)
        /// </summary>
        public List<DbCondition> R { get; set; }

        /// <summary>
        /// 组合条件间关系
        /// </summary>
        public ConditionOperator ConditionOperator { get; set; }

        /// <summary>
        /// 与 下一个 组合 条件之间的关系
        /// </summary>
        public ConditionOperator NextConditionOperator { get; set; }
    }
}
