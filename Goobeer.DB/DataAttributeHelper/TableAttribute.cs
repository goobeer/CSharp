using System;

namespace Goobeer.DB.DataAttributeHelper
{
    /// <summary>
    /// 表 属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute:Attribute
    {
        /// <summary>
        /// 数据表名称
        /// </summary>
        public string TableName { get; set; }
        
        public TableAttribute()
        { }

        public TableAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
