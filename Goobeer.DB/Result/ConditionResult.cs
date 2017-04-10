using Goobeer.DB.DbBase;
using System.Collections.Generic;

namespace Goobeer.DB.Result
{
    public abstract class ConditionResult : IConditionResult
    {
        public Dictionary<string,dynamic> ConditionParams { get; set; }

        public abstract string RenderConditon(DbCondition conditon, ConditionOperator op);
    }
}
