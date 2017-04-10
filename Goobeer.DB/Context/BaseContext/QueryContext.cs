using Goobeer.DB.Context.Query;
using Goobeer.DB.DbBase;
using System.Collections.Generic;
using Goobeer.Cache;

namespace Goobeer.DB.Context.BaseContext
{
    public abstract class QueryContext : IQueryContext
    {
        public ICacheable Cache { get; set; }

        /// <summary>
        /// 单表查询
        /// </summary>
        public QueryTableEntity SingleQueryTableEntity { get; set; }

        /// <summary>
        /// 多表查询
        /// </summary>
        public JoinQueryTableEntityCompact MultQueryTableEntity { get; set; }

        /// <summary>
        /// 查询 谓词 集合
        /// </summary>
        public List<QueryPredicate> QueryPredicates { get; set; }

        public abstract List<E> MultTabQuery<E>() where E : class, new();

        public abstract List<E> SingleTabQuery<E>() where E : class, new();

        public abstract List<E> SingleTabQuery<E>(uint pageNum, uint pageSize) where E : class, new();

        public abstract List<E> MultTabQuery<E>(uint pageNum, uint pageSize) where E : class, new();

        public abstract int SingleTabCountQuery<E>() where E : class, new();

        public abstract int MultTabCountQuery<E>() where E : class, new();
    }
}
