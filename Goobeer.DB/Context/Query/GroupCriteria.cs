using System.Collections.Generic;

namespace Goobeer.DB.Context.Query
{
    public class GroupCriteria
    {
        /// <summary>
        /// 分组字段 集合
        /// </summary>
        public List<string> GroupFields { get; set; }

        /// <summary>
        /// 合计条件
        /// </summary>
        private List<string> HavingCondition { get; set; }

        public GroupCriteria()
        {
            GroupFields = new List<string>();
        }

        public GroupCriteria(params string[] groupFields):this()
        {
            AddGroupField(groupFields);
        }

        public void AddGroupField(params string[] groupFields)
        {
            GroupFields.AddRange(groupFields);
        }

        public GroupCriteria Having(params string[] havingCondition)
        {
            HavingCondition.AddRange(havingCondition);
            return this;
        }
    }
}
