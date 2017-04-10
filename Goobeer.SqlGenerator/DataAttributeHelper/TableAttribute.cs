using System;
using System.Collections.Generic;
using System.Reflection;

namespace Goobeer.DB.DataAttributeHelper
{
    /// <summary>
    /// 表 属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute:Attribute
    {
        /// <summary>
        /// 数据库 名称
        /// </summary>
        public string DBName { get; set; }

        /// <summary>
        /// 数据表名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 字段 集合
        /// </summary>
        public static IDictionary<string, IDictionary<TableAttribute, IDictionary<PropertyInfo, FieldAttribute>>> Fields { get; set; }
        
        public TableAttribute()
        { }

        public TableAttribute(string tableName)
        {
            TableName = tableName;
        }

        public TableAttribute(string tableName, string dbName)
            : this(tableName)
        {
            DBName = dbName;
        }
    }
}
