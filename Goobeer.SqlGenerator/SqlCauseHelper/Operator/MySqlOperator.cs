using Goobeer.DB.DataAttributeHelper;
using Goobeer.DB.SqlCauseHelper.Condition;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Goobeer.DB.SqlCauseHelper.Operator
{
    public class MySqlOperator : SqlOperatorsBase
    {
        public MySqlOperator()
        {
            this.ParaPrefix = "?";
        }

        public override string ConvertSqlOperator(SqlCondition sqlCondition, bool useParam,KeyValuePair<PropertyInfo, FieldAttribute> propertyInfo)
        {
            string condition = string.Empty;
            condition = base.ConvertSqlOperator(sqlCondition, useParam, propertyInfo);
            if (string.IsNullOrEmpty(condition))
            {
                object obj = sqlCondition.FieldVal;
                SqlOperator operation = sqlCondition.SqlOperation;
                switch (operation)
                {
                    case SqlOperator.NotLike:
                    case SqlOperator.Like://字段-值之间的关系1对1
                        condition = string.Format("{0} {2}like CONCAT('%',{1},'%')", sqlCondition.FieldName, useParam ? sqlCondition.ParamName.FirstOrDefault() : FormateCondition(propertyInfo, sqlCondition), operation == SqlOperator.Like ? string.Empty : "not ");
                        break;
                    case SqlOperator.NotBetween:
                    case SqlOperator.Between://字段-值之间的关系1对2
                        var ic = obj as ICollection;
                        if (!(ic != null && ic.Count >= 2))
                        {
                            throw new Exception("参数值有误,请检查参数值。");
                        }
                        IEnumerator en = ic.GetEnumerator();
                        en.MoveNext();
                        var firstVal = en.Current;
                        en.MoveNext();
                        var secondVal = en.Current;

                        string codt1 = string.Empty;
                        string codt2 = string.Empty;
                        string[] codt = FormateCondition(propertyInfo, sqlCondition, true).ToString().Split(',');
                        codt1 = codt.First();
                        codt2 = codt.Last();
                        //if (useParam)
                        //{
                        //    GetO2TCondition(propertyInfo, sqlCondition, ref codt1, ref codt2);
                        //}

                        condition = string.Format("{0} {3}between {1} and {2}", sqlCondition.FieldName, useParam ? sqlCondition.ParamName.FirstOrDefault() : codt1, useParam ? sqlCondition.ParamName.LastOrDefault() : codt2, operation == SqlOperator.Between ? string.Empty : "not ");
                        break;
                    case SqlOperator.NotIn:
                    case SqlOperator.In://字段-值之间的关系1对多
                        ic = obj as ICollection;
                        if (!(ic != null && ic.Count >= 1))
                        {
                            throw new Exception("参数值有误,请检查参数值。");
                        }
                        en = ic.GetEnumerator();
                        List<object> objVals = new List<object>();
                        while (en.MoveNext())
                        {
                            objVals.Add(en.Current);
                        }
                        condition = string.Format("{0} {2}in ({1})", sqlCondition.FieldName, useParam ? string.Join(",", sqlCondition.ParamName) : FormateCondition(propertyInfo, sqlCondition,true), operation == SqlOperator.In ? string.Empty : "not ");
                        break;
                    default:
                        break;
                }
            }

            return condition;
        }
    }
}
