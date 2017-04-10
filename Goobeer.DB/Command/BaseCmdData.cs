using Goobeer.DB.DbBase;
using System;
using System.Collections.Generic;

namespace Goobeer.DB.Command
{
    /// <summary>
    /// 与命令相关的基本数据
    /// </summary>
    public class BaseCmdData : ICommandData
    {
        public DbConditionCollection DBCC { get; set; }
        public Dictionary<string, dynamic> OpFields { get; set; }
        public string TableName { get; set; }

        /// <summary>
        /// like 参数 连接
        /// </summary>
        public Func<string,string> ParamLikeConcat { get; set; }

        /// <summary>
        /// 范围参数
        /// </summary>
        public Func<string[],string> ParamRange { get; set; }

        public string ParseField(string fieldName)
        {
            if (string.IsNullOrEmpty(TableName))
            {
                return fieldName;
            }
            else
            {
                return string.Format("{0}.{1}", TableName, fieldName);
            }
        }
    }
}
