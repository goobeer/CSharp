using Goobeer.DB.DataAttributeHelper;
using Goobeer.DB.SqlCauseHelper.Condition;
using Goobeer.DB.SqlCauseHelper.Operator;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace Goobeer.DB.SqlCauseHelper.ISql
{
    public interface IUpdate
    {
        IUpdate BuildUpdate<E>(DbCommand cmd, E e, SqlOperatorsBase sqlOperators)
            where E : class, new();

        IUpdate BuildUpdate<E>(DbCommand cmd, E e, bool useParam, SqlOperatorsBase sqlOperators, Func<PropertyInfo, FieldAttribute, bool> fieldFilter = null) where E : class, new();

        IUpdate BuildUpdate<E>(DbCommand cmd, IDictionary<string, dynamic> idcs, bool useParam, ISqlConditionBuilder conditonBuilder, SqlOperatorsBase sqlOperators)
            where E : class, new();

    }
}
