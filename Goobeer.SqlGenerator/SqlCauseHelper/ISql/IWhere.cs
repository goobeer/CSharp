using Goobeer.DB.SqlCauseHelper.Condition;
using Goobeer.DB.SqlCauseHelper.Operator;
using System.Data.Common;

namespace Goobeer.DB.SqlCauseHelper.ISql
{
    public interface IWhere
    {
        IWhere BuildWhere<E>(DbCommand cmd, bool useParam, ISqlConditionBuilder conditon, SqlOperatorsBase sqlOperators) where E : class, new();

        IWhere BuildWhere<E>(DbCommand cmd, bool useParam, ISqlConditionBuilder conditon, SqlOperatorsBase sqlOperators, params ConditionComponent[] conditions) where E : class, new();

        IWhere BuildOrder<E>(DbCommand cmd, OrderCriteria oc) where E : class, new();
    }
}
