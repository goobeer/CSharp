using Goobeer.DB.DataAttributeHelper;
using Goobeer.DB.SqlCauseHelper.Condition;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Goobeer.DB.SqlCauseHelper.Operator
{
    public class SQLiteOperator:SqlOperatorsBase
    {
        public SQLiteOperator()
        {
            ParaPrefix = "@";
        }

        public override string ConvertSqlOperator(SqlCondition sqlCondition,bool useParam,KeyValuePair<PropertyInfo, FieldAttribute> propertyInfo)
        {
            string condition = string.Empty;
            condition = base.ConvertSqlOperator(sqlCondition, useParam, propertyInfo);
            if (string.IsNullOrEmpty(condition))
            {
                SqlOperator operation = sqlCondition.SqlOperation;
                switch (operation)
                {
                    case SqlOperator.NotLike:
                    case SqlOperator.Like:
                        condition = string.Format("{0} {2}like '%'||{1}||'%'", sqlCondition.FieldName, FormateCondition(propertyInfo, sqlCondition), operation == SqlOperator.Like ? string.Empty : "not ");
                        break;
                    case SqlOperator.NotBetween:
                    case SqlOperator.Between:
                        string codt1 = string.Empty;
                        string codt2 = string.Empty;
                        string[] codt = FormateCondition(propertyInfo, sqlCondition, true).ToString().Split(',');
                        codt1 = codt.First();
                        codt2 = codt.Last();
                        //if (useParam)
                        //{
                        //    FormateCondition(propertyInfo, sqlCondition, true);
                        //    GetO2TCondition(propertyInfo, sqlCondition, ref codt1, ref codt2);
                        //}
                        condition = string.Format("{0} {3}between {1} and {2}", sqlCondition.FieldName, useParam ? sqlCondition.ParamName.FirstOrDefault() : codt1, useParam ? sqlCondition.ParamName.LastOrDefault() : codt2, operation == SqlOperator.Between ? string.Empty : "not ");
                        break;
                    case SqlOperator.NotIn:
                    case SqlOperator.In:
                        condition = string.Format("{0} {2}in {1}", sqlCondition.FieldName, useParam ? string.Join(",", sqlCondition.ParamName) : FormateCondition(propertyInfo, sqlCondition,true), operation == SqlOperator.In ? string.Empty : "not ");
                        break;
                    default:
                        break;
                }
            }
            return condition;
        }
    }
}
