using System.Collections.Generic;

namespace Goobeer.DB.DbBase
{
    public class DbConditionCollection
    {
        /// <summary>
        /// 复杂条件 合成器
        /// </summary>
        public List<DbConditionCombiner> ConditionCombiners { get; set; }

        /// <summary>
        /// 简单条件 合成器
        /// </summary>
        public DbConditionCombiner Conditions { get; set; }

        public void AddCondition(DbCondition condition, ConditionOperator op)
        {
            Conditions = Conditions ?? new DbConditionCombiner() { R = new List<DbCondition>(), ConditionOperator = op };
            Conditions.R.Add(condition);
        }

        public void AddConditionCombiner(DbConditionCombiner conditionCombiner)
        {
            ConditionCombiners = ConditionCombiners ?? new List<DbConditionCombiner>();
            ConditionCombiners.Add(conditionCombiner);
        }
    }
}
