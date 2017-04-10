using System;
using System.Collections.Generic;

namespace Goobeer.DB.Context.Query
{
    public class JoinQueryTableEntityCombiner
    {
        public QueryTableEntity L { get; set; }
        public QueryTableEntity R { get; set; }
        public string LJoinField { get; set; }
        public string RJoinField { get; set; }
        public JoinType TableJoinType { get; set; }
        public string ConvertJoinType()
        {
            string joinType = TableJoinType.ToString();
            joinType = joinType.Insert(joinType.LastIndexOf("Join", StringComparison.OrdinalIgnoreCase), " ");
            return joinType;
        }
    }

    public class JoinQueryTableEntityCompact
    {
        public QueryTableEntity Master { get; set; }
        public List<JoinQueryTableEntityCombiner> SlaveJoinTable { get; set; }
    }

    public enum JoinType
    {
        None,
        InnerJoin,
        LeftJoin,
        RightJoin,
        FullJoin,
        CrossJoin,
        OuterJoin
    }
}
