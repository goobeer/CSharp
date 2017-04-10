using Goobeer.DB.DataAttributeHelper;
using Goobeer.DB.SqlCauseHelper.Condition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Goobeer.DB.SqlCauseHelper.Operator
{
    /// <summary>
    /// sql条件运算
    /// </summary>
    /// <typeparam name="C"></typeparam>
    public abstract class SqlOperatorsBase
    {
        public string ParaPrefix { get; set; }

        /// <summary>
        /// 解析条件值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <param name="isSurrounded"></param>
        /// <param name="isMany"></param>
        /// <returns></returns>
        public dynamic ParseCdtVals<T>(dynamic val,bool isSurrounded,bool isMany)
        {
            dynamic condition = null;
            if (val==null)
            {
                return null;
            }
            if (isMany)
            {
                var ic = val as ICollection<T>;
                if (!(ic != null && ic.Count >= 1))
                {
                    throw new Exception("参数值有误,请检查参数值。");
                }
                
                if (isSurrounded)
                {
                    condition = string.Format("'{0}'", string.Join<T>("','", ic));
                }
                else
                {
                    if (string.Compare(typeof(T).Name, "boolean", true)==0)
                    {
                        var temp = string.Empty;
                        for (int i = 0; i < ic.Count; i++)
                        {
                            var item = ic.ElementAt(i);
                            if (i != ic.Count - 1)
                            {
                                temp += string.Format("{0}，", string.Compare("true", item.ToString(), true) == 0 ? 1 : 0);
                            }
                            else
                            {
                                temp += string.Format("{0}", string.Compare("true", item.ToString(), true) == 0 ? 1 : 0);
                            }
                        }
                        condition = temp;
                    }
                    else
                    {
                        condition = string.Join<T>(",", ic);
                    }
                }
            }
            else
            {
                if (isSurrounded)
                {
                    condition = string.Format("'{0}'",val);
                }
                else
                {
                    if (string.Compare(typeof(T).Name, "boolean", true) == 0)
                    {
                        condition = val ? 1 : 0;
                    }
                    else
                    {
                        condition = val;
                    }
                }
            }
            return condition;
        }

        /// <summary>
        /// 格式化条件值
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="sqlCondition"></param>
        /// <param name="isMany"></param>
        /// <returns></returns>
        public dynamic FormateCondition(KeyValuePair<PropertyInfo, FieldAttribute> propertyInfo, SqlCondition sqlCondition, bool isMany = false)
        {
            dynamic condition = null;
            var propertyType = propertyInfo.Key.PropertyType.Name.ToLower();
            switch (propertyType)
            {
                case "guid":
                    condition = ParseCdtVals<Guid>(sqlCondition.FieldVal, true, isMany);
                    break;
                case "boolean":
                    condition = ParseCdtVals<bool>(sqlCondition.FieldVal, false, isMany);
                    break;
                case "int16":
                    condition = ParseCdtVals<Int16>(sqlCondition.FieldVal, false, isMany);
                    break;
                case "int32":
                    condition = ParseCdtVals<int>(sqlCondition.FieldVal, false, isMany);
                    break;
                case "int64":
                    condition = ParseCdtVals<Int64>(sqlCondition.FieldVal, false, isMany);
                    break;
                case "uint16":
                    condition = ParseCdtVals<UInt16>(sqlCondition.FieldVal, false, isMany);
                    break;
                case "uint32":
                    condition = ParseCdtVals<UInt32>(sqlCondition.FieldVal, false, isMany);
                    break;
                case "uint64":
                    condition = ParseCdtVals<UInt64>(sqlCondition.FieldVal, false, isMany);
                    break;
                case "double":
                    condition = ParseCdtVals<double>(sqlCondition.FieldVal, false, isMany);
                    break;
                case "decimal":
                    condition = ParseCdtVals<decimal>(sqlCondition.FieldVal, false, isMany);
                    break;
                default:
                    condition = ParseCdtVals<string>(sqlCondition.FieldVal, true, isMany);
                    break;
            }
            return condition;
        }

        /// <summary>
        /// sql 算数运算与逻辑运算
        /// </summary>
        /// <param name="sqlCondition">sql条件</param>
        /// <param name="useParam">是否使用参数化操作</param>
        /// <param name="propertyInfo">实体属性信息</param>
        /// <returns>转化后的sql 条件语句</returns>
        public virtual string ConvertSqlOperator(SqlCondition sqlCondition, bool useParam, KeyValuePair<PropertyInfo, FieldAttribute> propertyInfo)
        {
            string condition = string.Empty;
            SqlOperator operation = sqlCondition.SqlOperation;
            switch (operation)
            {
                case SqlOperator.Equal://字段-值之间的关系1对1
                case SqlOperator.NotEqual:
                    if (sqlCondition.FieldVal==null)
                    {
                        condition = string.Format("{0} is null", sqlCondition.FieldName);
                    }
                    else
                    {
                        condition = string.Format("{0}{2}={1}", sqlCondition.FieldName, useParam ? sqlCondition.ParamName.Single() : FormateCondition(propertyInfo, sqlCondition), operation == SqlOperator.NotEqual ? "!" : string.Empty);
                    }
                    break;
                case SqlOperator.Less://字段-值之间的关系1对1
                case SqlOperator.More:
                    if (sqlCondition.FieldVal == null)
                    {
                        condition = string.Format("{0} is null", sqlCondition.FieldName);
                    }
                    else
                    {
                        condition = string.Format("{0}{2}{1}", sqlCondition.FieldName, useParam ? sqlCondition.ParamName.Single() : FormateCondition(propertyInfo, sqlCondition), operation == SqlOperator.Less ? "<" : ">");
                    }
                    break;
                case SqlOperator.LessEqual:////字段-值之间的关系1对1
                case SqlOperator.MoreEqual:
                    if (sqlCondition.FieldVal == null)
                    {
                        condition = string.Format("{0} is null", sqlCondition.FieldName);
                    }
                    else
                    {
                        condition = string.Format("{0}{2}={1}", sqlCondition.FieldName, useParam ? sqlCondition.ParamName.Single() : FormateCondition(propertyInfo, sqlCondition), operation == SqlOperator.Less ? "<" : ">");
                    }
                    break;
                case SqlOperator.Nullable://字段-值之间的关系1对1
                case SqlOperator.NotNullable:
                    condition = string.Format("{0} is{1} null", sqlCondition.FieldName, operation == SqlOperator.NotNullable ? " not" : string.Empty);
                    break;
                case SqlOperator.None:
                default:
                    break;
            }
            return condition;
        }
    }
}
