using System;

namespace Goobeer.DB.DbBase
{
    /// <summary>
    /// 条件之间的 运算符
    /// </summary>
    [Flags]
    public enum ConditionOperator
    {
        /// <summary>
        /// 用于 update,insert 操作
        /// </summary>
        None = 0,
        And = 1,
        Or = 2,//and、or 都是双元运算符
    }
}
