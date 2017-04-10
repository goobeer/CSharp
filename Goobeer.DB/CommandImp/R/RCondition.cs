using Goobeer.DB.Command;
using Goobeer.DB.DbBase;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Goobeer.DB.CommandImp.R
{
    /// <summary>
    /// 不同的 数据库 condition 表现方式 不一样
    /// 1、字符串连接 方式
    /// </summary>
    public static class RCondition
    {
        public static string RenderConditon(DbCommand cmd,string paraPrefix,BaseCmdData conditionData)
        {
            StringBuilder condition = new StringBuilder();

            if (conditionData!=null && conditionData.DBCC != null && (conditionData.DBCC.Conditions != null || conditionData.DBCC.ConditionCombiners != null))
            {
                if (conditionData.DBCC.Conditions!=null)
                {
                    if (conditionData.DBCC.Conditions.L != null && conditionData.DBCC.Conditions.R != null)
                    {
                        var leftCondt = conditionData.DBCC.Conditions.L;
                        var leftc = string.Format("{0} ", ParseCondition(cmd, paraPrefix, conditionData, leftCondt, "0", conditionData.DBCC.Conditions.NextConditionOperator));

                        var rightCondt = conditionData.DBCC.Conditions.R;
                        StringBuilder sbRightc = new StringBuilder("(");
                        for (int i = 0; i < rightCondt.Count; i++)
                        {
                            sbRightc.AppendFormat(" {0}", ParseCondition(cmd, paraPrefix, conditionData, rightCondt[i], i + "1", rightCondt[i].NextConditionOperator));
                        }
                        sbRightc.Append(")");

                        condition.AppendFormat("{0}{1}", leftc, sbRightc.ToString());
                    }
                    else if (conditionData.DBCC.Conditions.L != null)
                    {
                        var leftCondt = conditionData.DBCC.Conditions.L;
                        var leftc = string.Format("{0} ", ParseCondition(cmd, paraPrefix, conditionData, leftCondt, "0", conditionData.DBCC.Conditions.NextConditionOperator));
                        condition.AppendFormat("{0}", leftc);
                    }
                    else if (conditionData.DBCC.Conditions.R != null)
                    {
                        var rightCondt = conditionData.DBCC.Conditions.R;
                        condition.Append("(");
                        for (int i = 0; i < rightCondt.Count; i++)
                        {
                            condition.AppendFormat(" {0}", ParseCondition(cmd, paraPrefix, conditionData, rightCondt[i], i + "1", rightCondt[i].NextConditionOperator));
                        }
                        condition.Append(")");
                    }
                }
                else
                {
                    for (int i = 0; i < conditionData.DBCC.ConditionCombiners.Count; i++)
                    {
                        var item = conditionData.DBCC.ConditionCombiners[i];

                        if (item.L != null && item.R != null)
                        {
                            var leftCondt = item.L;
                            var leftc = string.Format("{0} ", ParseCondition(cmd, paraPrefix, conditionData, leftCondt, i+"00", item.NextConditionOperator));

                            var rightCondt = item.R;
                            StringBuilder sbRightc = new StringBuilder("(");
                            for (int j = 0; j < rightCondt.Count; j++)
                            {
                                sbRightc.AppendFormat(" {0}", ParseCondition(cmd, paraPrefix, conditionData, rightCondt[j], i+j + "1", rightCondt[j].NextConditionOperator));
                            }
                            sbRightc.Append(")");

                            condition.AppendFormat("{0}{1}", leftc, sbRightc.ToString());
                        }
                        else if (item.L != null)
                        {
                            var leftCondt = item.L;
                            var leftc = string.Format("{0} ", ParseCondition(cmd, paraPrefix, conditionData, leftCondt, i+"0", item.NextConditionOperator));
                            condition.AppendFormat("{0}", leftc);
                        }
                        else if (item.R != null)
                        {
                            var rightCondt = item.R;
                            StringBuilder sbRightc = new StringBuilder("(");
                            for (int j = 0; j < rightCondt.Count; j++)
                            {
                                sbRightc.AppendFormat(" {0}", ParseCondition(cmd, paraPrefix, conditionData, rightCondt[j], i+j + "1", rightCondt[j].NextConditionOperator));
                            }
                            sbRightc.Append(")");

                            condition.AppendFormat("{0}", sbRightc.ToString());
                        }
                    }
                }

                if (conditionData.DBCC.Conditions.ConditionOperator != ConditionOperator.None)
                {
                    condition.AppendFormat(" {0}", conditionData.DBCC.Conditions.ConditionOperator.ToString());
                }
            }
            return condition.ToString();
        }

        private static string ParseCondition(DbCommand cmd, string paraPrefix, BaseCmdData conditionData,DbCondition conditions,string index,ConditionOperator co)
        {
            StringBuilder result = new StringBuilder();
            var leftCondt = conditions;
            if (leftCondt.FieldVal is byte[] || (leftCondt.FieldVal as ICollection) == null)//单值
            {
                var p = cmd.CreateParameter();
                p.ParameterName = string.Format("{0}{1}{2}", paraPrefix, leftCondt.FieldName,index);
                p.Value = leftCondt.FieldVal;
                cmd.Parameters.Add(p);
                result.AppendFormat("{0} {1}", RenderConditionOperator(conditionData, leftCondt.FieldName, leftCondt.DataOperator, p.ParameterName), co == ConditionOperator.None ? string.Empty : co.ToString());
            }
            else//多值
            {
                var t = leftCondt.FieldVal as ICollection<bool>;
                if (t != null)
                {
                    IEnumerator<bool> en = t.GetEnumerator();
                    List<string> param = new List<string>();
                    int k = 0;
                    while (en.MoveNext())
                    {
                        string paramKey = string.Format("{0}{1}{2}{3}", paraPrefix, leftCondt.FieldName,index, k);
                        var p = cmd.CreateParameter();
                        p.ParameterName = paramKey;
                        p.Value = en.Current ? 1 : 0;
                        cmd.Parameters.Add(p);
                        k++;
                        param.Add(paramKey);
                    }
                    result.AppendFormat(" {0} {1}", RenderConditionOperator(conditionData, leftCondt.FieldName, leftCondt.DataOperator, param.ToArray()), leftCondt.NextConditionOperator == ConditionOperator.None ? string.Empty : leftCondt.NextConditionOperator.ToString());
                }
                else
                {
                    var t1 = leftCondt.FieldVal as ICollection;//TODO 性能损失
                    List<string> param = new List<string>();
                    if (t1 != null)
                    {
                        IEnumerator en = t1.GetEnumerator();
                        int k = 0;
                        while (en.MoveNext())
                        {
                            string paramKey = string.Format("{0}{1}{2}{3}", paraPrefix, leftCondt.FieldName,index, k);
                            var p = cmd.CreateParameter();
                            p.ParameterName = paramKey;
                            p.Value = en.Current;
                            cmd.Parameters.Add(p);
                            k++;
                            param.Add(paramKey);
                        }
                        result.AppendFormat(" {0} {1}", RenderConditionOperator(conditionData, leftCondt.FieldName, leftCondt.DataOperator, param.ToArray()), leftCondt.NextConditionOperator==ConditionOperator.None ? string.Empty : leftCondt.NextConditionOperator.ToString());
                    }
                }
            }
            return result.ToString();
        }
        
        public static string RenderConditionOperator(BaseCmdData conditionData, string fieldName, DataOperator op, params string[] param)
        {
            string condition = string.Empty;
            fieldName = conditionData.ParseField(fieldName);
            switch (op)
            {
                case DataOperator.None:
                    break;
                case DataOperator.Equal:
                case DataOperator.NotEqual:
                    condition = string.Format("{0}{1}{2}", fieldName, op == DataOperator.Equal ? "=" : "!=", param[0]);
                    break;
                case DataOperator.Less:
                case DataOperator.LessEqual:
                    condition = string.Format("{0}{1}{2}", fieldName, op == DataOperator.Less ? "<" : "<=", param[0]);
                    break;
                case DataOperator.More:
                case DataOperator.MoreEqual:
                    condition = string.Format("{0}{1}{2}", fieldName, op == DataOperator.More ? ">" : ">=", param[0]);
                    break;
                case DataOperator.Like:
                case DataOperator.NotLike:
                    condition = string.Format("{0} {1} {2}", fieldName, op == DataOperator.Like ? "like" : "not like", conditionData.ParamLikeConcat(param[0]));
                    break;
                case DataOperator.Between:
                case DataOperator.NotBetween://边界条件
                    condition = string.Format("{0} {1} {2} and {3}", fieldName, op == DataOperator.Between ? "between" : "not between", param[0], param[1]);
                    break;
                case DataOperator.In:
                case DataOperator.NotIn://范围条件
                    condition = string.Format("{0} {1} {2}", fieldName, op == DataOperator.In ? "in" : "not in", conditionData.ParamRange(param));
                    break;
                case DataOperator.Nullable:
                case DataOperator.NotNullable:
                    condition = string.Format("{0} {1}", fieldName, op == DataOperator.Nullable ? "is null" : "is not null");
                    break;
                default:
                    break;
            }
            return condition;
        }

        #region like 连接
        /// <summary>
        /// 数据库 like 相关操作下 字符串 连接 操作
        /// 1、mssql  :+
        /// 2、oracle & sqlite :||
        /// 3、mysql  :concat(paras,...)
        /// </summary>
        /// <returns></returns>
        public static string MsSqlLikeStrConcat(string param)
        {
            return string.Format("'%'+{0}+'%'", param);
        }

        public static string MySqlLikeStrConcat(string param)
        {
            return string.Format("concat('%',{0},'%')", param);
        }

        public static string OracleLikeStrConcat(string param)
        {
            return string.Format("'%'||{0}||'%'", param);
        }

        public static string SqliteLikeStrConcat(string param)
        {
            return string.Format("'%'||{0}||'%'", param);
        }
        #endregion

        /// <summary>
        /// 范围参数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string ParamRange(string[] param)
        {
            return string.Format("({0})", string.Join(",", param));
        }
    }
}
