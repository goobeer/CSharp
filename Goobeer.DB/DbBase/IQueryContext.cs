using System.Collections.Generic;

namespace Goobeer.DB.DbBase
{
    public interface IQueryContext
    {
        List<E> SingleTabQuery<E>() where E : class, new();
        int SingleTabCountQuery<E>() where E : class, new();
        List<E> SingleTabQuery<E>(uint pageNum,uint pageSize) where E : class, new();
        List<E> MultTabQuery<E>() where E : class, new();
        int MultTabCountQuery<E>() where E : class, new();
        List<E> MultTabQuery<E>(uint pageNum, uint pageSize) where E : class, new();
    }
}
