using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Goobeer.DB.DataAttributeHelper;
using Goobeer.DB.SqlCauseHelper.Operator;
using System.Linq;
using System.Data.Common;

namespace Goobeer.DB.SqlCauseHelper.Condition
{
    public class SqlConditionBuilder: ISqlConditionBuilder
    {
        public List<SqlCondition> ListConditions { get; set; }
        public List<SqlCondition> ListNonQueryFields { get; set; } 

        public SqlConditionBuilder()
        {
        }

        public SqlConditionBuilder(SqlCondition condition)
        {
            AddCondition(condition);
        }

        public SqlConditionBuilder(List<SqlCondition> condition)
        {
            AddCondition(condition);
        }
        
        #region 添加操作条件
        public ISqlConditionBuilder AddCondition(SqlCondition condition)
        {
            ListConditions = ListConditions ?? new List<SqlCondition>();
            ListConditions.Add(condition);
            return this;
        }

        public ISqlConditionBuilder AddCondition(List<SqlCondition> condition)
        {
            ListConditions = ListConditions ?? new List<SqlCondition>();
            ListConditions.AddRange(condition);
            return this;
        }
        #endregion

        #region 添加操作字段
        public ISqlConditionBuilder AddNonQueryField(SqlCondition condition)
        {
            ListNonQueryFields = ListNonQueryFields ?? new List<SqlCondition>();
            ListNonQueryFields.Add(condition);
            return this;
        }
        public ISqlConditionBuilder AddNonQueryField(List<SqlCondition> condition)
        {
            ListNonQueryFields = ListNonQueryFields ?? new List<SqlCondition>();
            ListNonQueryFields.AddRange(condition);
            return this;
        } 
        #endregion

        /// <summary>
        /// 创建 操作条件(不考虑 byte[] 类型的 条件)
        /// </summary>
        /// <param name="useParam"></param>
        /// <param name="sqlOperators"></param>
        /// <returns></returns>
        public ConditonResult BuildCondition(bool useParam, SqlOperatorsBase sqlOperators, IDictionary<PropertyInfo, FieldAttribute> idcPF,DbCommand cmd)
        {
            string paraPrefix = sqlOperators.ParaPrefix ?? string.Empty;
            ConditonResult result = new ConditonResult(useParam);

            if (ListConditions != null && ListConditions.Any())
            {
                for (int i = 0; i < ListConditions.Count; i++)
                {
                    SqlCondition tempCondition = ListConditions[i];
                    var propertyInfo = idcPF.Single(kvp => string.Compare(kvp.Value.FieldName, tempCondition.FieldName, true) == 0);

                    if (useParam)
                    {
                        //特殊值 bool 处理，性能损耗
                        List<string> lst = new List<string>();
                        string paramKey = string.Empty;
                        var v = tempCondition.FieldVal;
                        if (v is byte[] || (v as ICollection) == null)
                        {
                            paramKey = string.Format("{1}cpara{0}", i, paraPrefix);
                            
                            var param = cmd.CreateParameter();
                            param.ParameterName = paramKey;
                            if (string.Compare(propertyInfo.Key.PropertyType.Name, "boolean", true) == 0)
                            {
                                param.Value = v ? 1 : 0;
                            }
                            else
                            {
                                param.Value = v;
                            }
                            result.Params.Add(param);
                            lst.Add(paramKey);
                        }
                        else// v is ICollection
                        {
                            if (string.Compare(propertyInfo.Key.PropertyType.Name, "boolean", true) == 0)
                            {
                                var t = v as ICollection<bool>;
                                if (t != null)
                                {
                                    IEnumerator<bool> en = t.GetEnumerator();
                                    int k = 0;
                                    while (en.MoveNext())
                                    {
                                        paramKey = string.Format("{1}cpara{0}{2}", i, paraPrefix, k);
                                        var param = cmd.CreateParameter();
                                        param.ParameterName = paramKey;
                                        param.Value = en.Current ? 1 : 0;
                                        result.Params.Add(param);
                                        lst.Add(paramKey);
                                        k++;
                                    }
                                }
                            }
                            else
                            {
                                var t = v as ICollection;//TODO 性能损失
                                if (t != null)
                                {
                                    IEnumerator en = t.GetEnumerator();
                                    int k = 0;
                                    while (en.MoveNext())
                                    {
                                        paramKey = string.Format("{1}cpara{0}{2}", i, paraPrefix, k);
                                        var param = cmd.CreateParameter();
                                        param.ParameterName = paramKey;
                                        param.Value = en.Current;
                                        result.Params.Add(param);
                                        lst.Add(paramKey);
                                        k++;
                                    }
                                }
                            }
                        }
                        tempCondition.ParamName = lst;
                    }

                    result.CondtionText.AppendFormat("{1}{0}", sqlOperators.ConvertSqlOperator(tempCondition, useParam, propertyInfo), tempCondition.ConditionOperator != ConditionOperator.None ? string.Format(" {0} ", tempCondition.ConditionOperator) : string.Empty);
                }
            }
            return result;
        }

        public ConditonResult BuildCondition(bool useParam, SqlOperatorsBase sqlOperators, IDictionary<PropertyInfo, FieldAttribute> idcPF, DbCommand cmd, params ConditionComponent[] conditions)
        {
            ConditonResult result = new ConditonResult(useParam);
            string paraPrefix = sqlOperators.ParaPrefix ?? string.Empty;
            if (conditions != null && conditions.Any())
            {
                int j = 0;
                StringBuilder tempConditionTxt = new StringBuilder();
                foreach (var condition in conditions)
                {
                    tempConditionTxt.Clear();
                    for (int i = 0; i < condition.Conditions.Count; i++)
                    {
                        SqlCondition tempCondition = condition.Conditions[i];
                        var propertyInfo = idcPF.Single(kvp => string.Compare(kvp.Value.FieldName, tempCondition.FieldName, true) == 0);

                        if (useParam)
                        {
                            //特殊值 bool 处理，性能损耗
                            List<string> lst = new List<string>();
                            string paramKey = string.Empty;
                            var v = tempCondition.FieldVal;
                            if (v is byte[] || (v as ICollection) == null)
                            {
                                paramKey = string.Format("{2}cpara{0}{1}", j, i, paraPrefix);

                                var param = cmd.CreateParameter();
                                param.ParameterName = paramKey;
                                if (string.Compare(propertyInfo.Key.PropertyType.Name, "boolean", true) == 0)
                                {
                                    param.Value = v ? 1 : 0;
                                }
                                else
                                {
                                    param.Value = v;
                                }
                                result.Params.Add(param);
                                lst.Add(paramKey);
                            }
                            else// v is ICollection
                            {
                                if (string.Compare(propertyInfo.Key.PropertyType.Name, "boolean", true) == 0)
                                {
                                    var t = v as ICollection<bool>;
                                    if (t != null)
                                    {
                                        IEnumerator<bool> en = t.GetEnumerator();
                                        int k = 0;
                                        while (en.MoveNext())
                                        {
                                            paramKey = string.Format("{3}cpara{0}{1}{2}", j, i, k, paraPrefix);
                                            var param = cmd.CreateParameter();
                                            param.ParameterName = paramKey;
                                            param.Value = en.Current ? 1 : 0;
                                            result.Params.Add(param);
                                            lst.Add(paramKey);
                                            k++;
                                        }
                                    }
                                }
                                else
                                {
                                    var t = v as ICollection;//TODO 性能损失
                                    if (t != null)
                                    {
                                        IEnumerator en = t.GetEnumerator();
                                        int k = 0;
                                        while (en.MoveNext())
                                        {
                                            paramKey = string.Format("{3}cpara{0}{1}{2}", j, i, k, paraPrefix);
                                            var param = cmd.CreateParameter();
                                            param.ParameterName = paramKey;
                                            param.Value = en.Current;
                                            result.Params.Add(param);
                                            lst.Add(paramKey);
                                            k++;
                                        }
                                    }
                                }

                            }
                            tempCondition.ParamName = lst;
                        }
                        tempConditionTxt.AppendFormat("{1}{0}", sqlOperators.ConvertSqlOperator(tempCondition, useParam, propertyInfo), tempCondition.ConditionOperator != ConditionOperator.None ? string.Format(" {0} ", tempCondition.ConditionOperator) : string.Empty);
                    }
                    tempConditionTxt.Insert(0, "(").Insert(tempConditionTxt.Length, ")");
                    result.CondtionText.AppendFormat("{0}{1}", condition.ConditionOperator == ConditionOperator.None ? string.Empty : string.Format(" {0} ", condition.ConditionOperator), tempConditionTxt.ToString());
                    j++;
                }
            }

            return result;
        }

        /// <summary>
        /// 获得操作的格式化 字段范围(仅供 insert,update 使用)
        /// </summary>
        /// <param name="useParam"></param>
        /// <param name="sqlOperators"></param>
        /// <param name="fieldStrategy"></param>
        /// <returns></returns>
        public ISqlConditionBuilder BuildNonQueryField(bool useParam, SqlOperatorsBase sqlOperators, Func<SqlOperatorsBase,IDictionary<PropertyInfo,FieldAttribute>,bool, IDictionary<string, dynamic>, string> fieldStrategy, IDictionary<PropertyInfo, FieldAttribute> idcPF,DbCommand cmd)
        {
            string paraPrefix = sqlOperators.ParaPrefix ?? string.Empty;
            StringBuilder sb = new StringBuilder();
            Dictionary<string, dynamic> dics = new Dictionary<string, dynamic>();
            string paramKey = string.Empty;

            for (int i = 0; i < ListNonQueryFields.Count; i++)
            {
                SqlCondition tempCondition = ListNonQueryFields[i];

                var propertyInfo = idcPF.Single(kvp => string.Compare(kvp.Value.FieldName, tempCondition.FieldName, true) == 0);

                List<string> lst = new List<string>();
                var v = tempCondition.FieldVal;//条件原子项

                if (v is byte[])
                {
                    paramKey = string.Format("{1}fpara{0}", i, paraPrefix);
                    var param = cmd.CreateParameter();
                    param.ParameterName = paramKey;
                    param.Value = v;
                    cmd.Parameters.Add(param);
                    dics.Add(tempCondition.FieldName, paramKey);
                    lst.Add(paramKey);
                }
                else
                {
                    if (useParam && v!=null)
                    {
                        paramKey = string.Format("{1}fpara{0}", i, paraPrefix);
                        var param = cmd.CreateParameter();
                        param.ParameterName = paramKey;
                        if (string.Compare(propertyInfo.Key.PropertyType.Name, "boolean", true) == 0)
                        {
                            param.Value = v ? 1 : 0;
                        }
                        else
                        {
                            param.Value = v;
                        }
                        cmd.Parameters.Add(param);
                        dics.Add(tempCondition.FieldName, paramKey);
                        lst.Add(paramKey);
                    }
                    else
                    {
                        dics.Add(tempCondition.FieldName, v);
                    }
                }
                tempCondition.ParamName = lst;
            }

            sb.Append(fieldStrategy(sqlOperators,idcPF, useParam, dics));

            cmd.CommandText = string.Format("{0}{1}",cmd.CommandText, sb.ToString());

            return this;
        }

        public void Clear()
        {
            if (ListConditions != null && ListConditions.Count > 0)
            {
                ListConditions.Clear();
            }
            if (ListNonQueryFields != null && ListNonQueryFields.Count > 0)
            {
                ListNonQueryFields.Clear();
            }
        }
    }

    /// <summary>
    /// sql 运算符
    /// </summary>
    [Flags]
    public enum SqlOperator
    {
        None=1,//常用于insert 操作
        Equal = 2,
        Less = 4,
        More = 8,
        Like = 16,
        Between = 32,
        In = 64,
        Nullable = 128,
        NotLike = ~Like,
        NotBetween=~Between,
        NotIn = ~In,
        LessEqual = Equal | Less,
        MoreEqual = Equal | More,
        NotEqual = ~Equal,
        NotNullable=~Nullable
    }

    /// <summary>
    /// 条件之间的 运算符
    /// </summary>
    [Flags]
    public enum ConditionOperator
    {
        /// <summary>
        /// 用于 update,insert 操作
        /// </summary>
        None=1,
        And=2,
        Or=4,//and、or 都是双元运算符
        //Not=8//单元运算符，这个运算符的优先级最高
    }
}
