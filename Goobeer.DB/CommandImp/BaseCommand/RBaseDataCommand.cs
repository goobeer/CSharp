using Goobeer.DB.Command;
using Goobeer.DB.DbBase;
using System.Data.Common;

namespace Goobeer.DB.CommandImp.BaseCommand
{
    /// <summary>
    /// 关系型 数据命令 基类
    /// </summary>
    public abstract class RBaseDataCommand : BaseDataCommand
    {
        /// <summary>
        /// 参数化操作前缀
        /// </summary>
        public string ParamPrefix { get; set; }

        public RDBRepertory DBHelper { get; set; }

        public override int Execute()
        {
            return ExecuteCommand();
        }

        public override int ExecuteCommand()
        {
            return DBHelper.ExecuteNonQuery(RenderCommandData());
        }

        public abstract DbCommand RenderCommandData();

        public string RenderConditionOperator(string fieldName, DataOperator op, params string[] param)
        {
            string condition = string.Empty;
            switch (op)
            {
                case DataOperator.None:
                    break;
                case DataOperator.Equal:
                case DataOperator.NotEqual:
                    condition = string.Format("{0}{1}{2}", fieldName, op == DataOperator.Equal ? "=" : "!=", param);
                    break;
                case DataOperator.Less:
                case DataOperator.LessEqual:
                    condition = string.Format("{0}{1}{2}", fieldName, op == DataOperator.Less ? "<" : "<=", param);
                    break;
                case DataOperator.More:
                case DataOperator.MoreEqual:
                    condition = string.Format("{0}{1}{2}", fieldName, op == DataOperator.More ? ">" : ">=", param);
                    break;
                case DataOperator.Like:
                case DataOperator.NotLike:
                    condition = string.Format("{0} {1} {2}", fieldName, op == DataOperator.Like ? "like" : "not like", Data.ParamLikeConcat(param[0]));
                    break;
                case DataOperator.Between:
                case DataOperator.NotBetween://边界条件
                    condition = string.Format("{0} {1} {2} and {3}", fieldName, op == DataOperator.Between ? "between" : "not between", param[0], param[1]);
                    break;
                case DataOperator.In:
                case DataOperator.NotIn://范围条件
                    condition = string.Format("{0} {1} {2}", fieldName, op == DataOperator.In ? "in" : "not in", Data.ParamRange(param));
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
    }
}
