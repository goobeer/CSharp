using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Goobeer.DataAttributeHelper;
using Goobeer.SqlCauseHelper.Condition;

namespace Goobeer.SqlCauseHelper.Operator
{
    /// <summary>
    /// sql条件运算
    /// </summary>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="P"></typeparam>
    public abstract class SqlOperators<C,P>
        where C : class,IDbCommand, new()
        where P : class,IDataParameter, new()
    {
        public string ParaPrefix { get; set; }

        /// <summary>
        /// sql 算数运算与逻辑运算
        /// </summary>
        /// <param name="sqlCondition">sql条件</param>
        /// <param name="useParam">是否使用参数化操作</param>
        /// <returns>转化后的sql 条件语句</returns>
        public virtual string ConvertSqlOperator(SqlCondition sqlCondition, bool useParam, IDictionary<PropertyInfo, FieldAttribute> idcPF)
        {
            string condition = string.Empty;
            SqlOperator operation = sqlCondition.SqlOperation;
            switch (operation)
            {
                //TODO 对参数值 前的 引号 做精细化控制
                case SqlOperator.Equal://字段-值之间的关系1对1
                    condition = string.Format("{0}={1}", sqlCondition.FieldName, useParam ? sqlCondition.ParamName.FirstOrDefault() :(idcPF.Any(kvp => kvp.Key.PropertyType.IsValueType && string.Compare(kvp.Value.FieldName, sqlCondition.FieldName, System.StringComparison.OrdinalIgnoreCase)==0)?sqlCondition.FieldVal: string.Format("'{0}'", sqlCondition.FieldVal)));
                    break;
                case SqlOperator.Less://字段-值之间的关系1对1
                    condition = string.Format("{0}<{1}", sqlCondition.FieldName, useParam ? sqlCondition.ParamName.FirstOrDefault() : (idcPF.Any(kvp => kvp.Key.PropertyType.IsValueType && string.Compare(kvp.Value.FieldName, sqlCondition.FieldName, System.StringComparison.OrdinalIgnoreCase) == 0) ? sqlCondition.FieldVal : string.Format("'{0}'", sqlCondition.FieldVal)));
                    break;
                case SqlOperator.More://字段-值之间的关系1对1
                    condition = string.Format("{0}>{1}", sqlCondition.FieldName, useParam ? sqlCondition.ParamName.FirstOrDefault() : (idcPF.Any(kvp => kvp.Key.PropertyType.IsValueType && string.Compare(kvp.Value.FieldName, sqlCondition.FieldName, System.StringComparison.OrdinalIgnoreCase) == 0) ? sqlCondition.FieldVal : string.Format("'{0}'", sqlCondition.FieldVal)));
                    break;
                case SqlOperator.LessEqual:////字段-值之间的关系1对1
                    condition = string.Format("{0}<={1}", sqlCondition.FieldName, useParam ? sqlCondition.ParamName.FirstOrDefault() : (idcPF.Any(kvp => kvp.Key.PropertyType.IsValueType && string.Compare(kvp.Value.FieldName, sqlCondition.FieldName, System.StringComparison.OrdinalIgnoreCase) == 0) ? sqlCondition.FieldVal : string.Format("'{0}'", sqlCondition.FieldVal)));
                    break;
                case SqlOperator.MoreEqual://字段-值之间的关系1对1
                    condition = string.Format("{0}>={1}", sqlCondition.FieldName, useParam ? sqlCondition.ParamName.FirstOrDefault() : (idcPF.Any(kvp => kvp.Key.PropertyType.IsValueType && string.Compare(kvp.Value.FieldName, sqlCondition.FieldName, System.StringComparison.OrdinalIgnoreCase) == 0) ? sqlCondition.FieldVal : string.Format("'{0}'", sqlCondition.FieldVal)));
                    break;
                case SqlOperator.NotEqual://字段-值之间的关系1对1
                    condition = string.Format("{0}!={1}", sqlCondition.FieldName, useParam ? sqlCondition.ParamName.FirstOrDefault() : (idcPF.Any(kvp => kvp.Key.PropertyType.IsValueType && string.Compare(kvp.Value.FieldName, sqlCondition.FieldName, System.StringComparison.OrdinalIgnoreCase) == 0) ? sqlCondition.FieldVal : string.Format("'{0}'", sqlCondition.FieldVal)));
                    break;
                case SqlOperator.None:
                default:
                    break;
            }
            return condition;
        }
    }
}
