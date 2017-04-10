using Goobeer.DB.SqlCauseHelper.Condition;
using Goobeer.DB.SqlCauseHelper.Operator;
using System.Data.Common;
using System;
using System.Collections.Generic;

namespace Goobeer.DB.SqlCauseHelper.ISql
{
    public interface ISelect: IWhere
    {
        #region 生成 单表 ISelect
        /// <summary>
        /// ISelect
        /// </summary>
        /// <param name="selectFields">要检索的字段名称集合(可为空,若为空:则检索实体的全部属性)</param>
        /// <returns>ISelect</returns>
        ISelect BuildSelect<E>(DbCommand cmd, params string[] selectFields) where E : class, new();
        #endregion

        ISelect BuildCount<E>(DbCommand cmd, string countField) where E : class, new();

        ISelect BuildGroup<E>(DbCommand cmd, string[] selectFields, string[] groupFields) where E : class, new();

        /// <summary>
        /// 2表 联合查询
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="joinTableEntity"></param>
        /// <returns></returns>
        ISelect BuildJoin<R>(DbCommand cmd, List<JoinTableEntity> joinTableEntity)
            where R : class, new();
    }

    /// <summary>
    /// 表间 联合 类型
    /// </summary>
    public enum JoinType
    {
        InnerJoin,
        LeftJoin,
        RightJoin,
        FullJoin,
        CrossJoin,
        OuterJoin
    }

    //表间 关联

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
        /// 副表
        /// </summary>
        public string TabBName { get; set; }
        public JoinType TableJoinType { get; set; }

        public string ConvertJoinType()
        {
            string joinType = TableJoinType.ToString();
            joinType = joinType.Insert(joinType.LastIndexOf("Join",StringComparison.OrdinalIgnoreCase), " ");
            return joinType;
        }
    }
}
