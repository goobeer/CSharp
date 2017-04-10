using Goobeer.DB.DbBase;
using System.Collections.Generic;

namespace Goobeer.DB.Result
{
    public class SqlServConditionResult : ConditionResult
    {
        private readonly string _prefix = "@";
        private static string _paramFormate = "@p{0}";

        public SqlServConditionResult()
        {
            ConditionParams = new Dictionary<string, dynamic>();
        }

        public override string RenderConditon(DbCondition conditon, ConditionOperator op)
        {
            string conditonSql = string.Empty;
            string key = string.Format(_paramFormate, ConditionParams.Count);
            ConditionParams.Add(key, conditon.FieldVal);

            switch (conditon.DataOperator)
            {
                case DataOperator.None:
                    break;
                case DataOperator.Equal:
                case DataOperator.NotEqual:
                    conditonSql = string.Format("{0}{1}{2} {3} ", conditon.FieldName, conditon.DataOperator == DataOperator.Equal ? "=" : "!=", key, op.ToString());
                    break;
                case DataOperator.Less:
                case DataOperator.LessEqual:
                    conditonSql = string.Format("{0}{1}{2} {3} ", conditon.FieldName, conditon.DataOperator == DataOperator.Less ? "<" : "<=", key, op.ToString());
                    break;
                case DataOperator.More:
                case DataOperator.MoreEqual:
                    conditonSql = string.Format("{0}{1}{2} {3} ", conditon.FieldName, conditon.DataOperator == DataOperator.More ? ">" : ">=", key, op.ToString());
                    break;
                case DataOperator.Like:
                case DataOperator.NotLike:
                    conditonSql = string.Format("{0} {1} '%'+{2}+'%' {3} ", conditon.FieldName, conditon.DataOperator == DataOperator.More ? "like" : "not like", key, op.ToString());
                    break;
                case DataOperator.Between:
                case DataOperator.NotBetween:
                    throw new System.Exception("这个不方便,多值");
                    conditonSql = string.Format("{0}{1}{2} {3} ", conditon.FieldName, conditon.DataOperator == DataOperator.More ? ">" : ">=", key, op.ToString());
                    break;
                case DataOperator.In:
                    break;
                case DataOperator.Nullable:
                    break;
                case DataOperator.NotIn:
                    break;
                case DataOperator.NotNullable:
                    break;
                default:
                    break;
            }
            return conditonSql;
        }
    }
}
