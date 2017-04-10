using Goobeer.DB.Context.BaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goobeer.DB.Context.NR
{
    /// <summary>
    /// 非关系型 数据库查询上下文
    /// </summary>
    public class NRBaseQueryContext : QueryContext
    {
        public override int MultTabCountQuery<E>()
        {
            throw new NotImplementedException();
        }

        public override List<E> MultTabQuery<E>()
        {
            throw new NotImplementedException();
        }

        public override List<E> MultTabQuery<E>(uint pageNum, uint pageSize)
        {
            throw new NotImplementedException();
        }

        public override int SingleTabCountQuery<E>()
        {
            throw new NotImplementedException();
        }

        public override List<E> SingleTabQuery<E>()
        {
            throw new NotImplementedException();
        }

        public override List<E> SingleTabQuery<E>(uint pageNum, uint pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
