using Goobeer.DB.Command;
using System.Collections.Generic;

namespace Goobeer.DB.Context.Query
{
    /// <summary>
    /// 查询谓词
    /// </summary>
    public class QueryPredicate
    {
        public BaseCmdData Data { get; set; }
        public List<GroupCriteria> GC { get; set; }
        public List<OrderCriteria> OC { get; set; }
    }
}
