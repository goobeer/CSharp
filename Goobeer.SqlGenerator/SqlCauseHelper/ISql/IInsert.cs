using Goobeer.DB.DataAttributeHelper;
using Goobeer.DB.SqlCauseHelper.Operator;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace Goobeer.DB.SqlCauseHelper.ISql
{
    public interface IInsert
    {
        IInsert BuildInsert<E>(DbCommand cmd, E e, SqlOperatorsBase sqlOperators)
            where E : class, new();

        IInsert BuildInsert<E>(DbCommand cmd, E e, SqlOperatorsBase sqlOperators, Func<PropertyInfo, FieldAttribute, bool> fieldFilter = null) where E : class, new();

        IInsert BuildInsert<E>(DbCommand cmd, IDictionary<string, dynamic> keyValPairs, SqlOperatorsBase sqlOperators)
            where E : class, new();

        //批量添加
        //IInsert BuildInsert<E>(List<E> entities, out List<IDbCommand> cmd, SqlOperatorsBase sqlOperators, Func<object, bool> valFilter = null)
        //    where E : class, new();
    }
}
