using System;

namespace Goobeer.DB.DbBase
{
    /// <summary>
    /// 数据运算符
    /// </summary>
    [Flags]
    public enum DataOperator
    {
        None = 1,//常用于insert 操作
        Equal = 2,
        Less = 4,
        More = 8,
        Like = 16,
        Between = 32,
        In = 64,
        Nullable = 128,
        NotLike = ~Like,
        NotBetween = ~Between,
        NotIn = ~In,
        LessEqual = Equal | Less,
        MoreEqual = Equal | More,
        NotEqual = ~Equal,
        NotNullable = ~Nullable
    }
}
