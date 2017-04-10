using Goobeer.DB.SqlCauseHelper.Condition;
using Goobeer.DB.SqlCauseHelper.Operator;
using System.Data.Common;

namespace Goobeer.DB.SqlCauseHelper.ISql
{
    public interface IDelete
    {
        IDelete BuildDelete<E>(DbCommand cmd, E e, SqlOperatorsBase sqlOperators, bool useParam = true) where E : class, new();

        IDelete BuildDelete<E>(DbCommand cmd, SqlConditionBuilder conditon, SqlOperatorsBase sqlOperators, bool useParam = true)
            where E : class, new();
    }
}
