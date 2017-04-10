using Goobeer.DB.DataAttributeHelper;
using Goobeer.DB.SqlCauseHelper.Operator;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace Goobeer.DB.SqlCauseHelper.Condition
{
    public interface ISqlConditionBuilder
    {
        List<SqlCondition> ListConditions { get; set; }
        List<SqlCondition> ListNonQueryFields { get; set; }

        ISqlConditionBuilder AddCondition(SqlCondition condition);

        ISqlConditionBuilder AddCondition(List<SqlCondition> condition);

        ISqlConditionBuilder AddNonQueryField(SqlCondition condition);

        ISqlConditionBuilder AddNonQueryField(List<SqlCondition> condition);

        ConditonResult BuildCondition(bool useParam, SqlOperatorsBase sqlOperators, IDictionary<PropertyInfo, FieldAttribute> idcPF,DbCommand cmd);

        ConditonResult BuildCondition(bool useParam, SqlOperatorsBase sqlOperators, IDictionary<PropertyInfo, FieldAttribute> idcPF, DbCommand cmd, params ConditionComponent[] conditions);

        ISqlConditionBuilder BuildNonQueryField(bool useParam, SqlOperatorsBase sqlOperators, Func<SqlOperatorsBase, IDictionary<PropertyInfo, FieldAttribute>, bool, IDictionary<string, dynamic>, string> fieldStrategy, IDictionary<PropertyInfo, FieldAttribute> idcPF, DbCommand cmd);

        void Clear();
    }
}
