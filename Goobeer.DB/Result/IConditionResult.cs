using Goobeer.DB.DbBase;

namespace Goobeer.DB.Result
{
    public interface IConditionResult
    {
        string RenderConditon(DbCondition conditon, ConditionOperator op);
    }
}
