using System;

namespace Goobeer.DB.Result
{
    public class JoinTableEntity
    {
        public string[] SelectAFields { get; set; }
        public string[] SelectBFields { get; set; }
        public string OnFieldAName { get; set; }
        public string OnFieldBName { get; set; }

        /// <summary>
        /// 主表
        /// </summary>
        public string TabAName { get; set; }

        /// <summary>
        /// 主表别名
        /// </summary>
        public string AliasTabAName { get; set; }

        /// <summary>
        /// 副表
        /// </summary>
        public string TabBName { get; set; }

        /// <summary>
        /// 副表别名
        /// </summary>
        public string AliasTabBName { get; set; }

        public JoinType TableJoinType { get; set; }

        public string ConvertJoinType()
        {
            string joinType = TableJoinType.ToString();
            joinType = joinType.Insert(joinType.LastIndexOf("Join", StringComparison.OrdinalIgnoreCase), " ");
            return joinType;
        }
    }

    public enum JoinType
    {
        InnerJoin,
        LeftJoin,
        RightJoin,
        FullJoin,
        CrossJoin,
        OuterJoin
    }
}
