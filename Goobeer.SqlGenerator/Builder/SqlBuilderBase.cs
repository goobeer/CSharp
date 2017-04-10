using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goobeer.Cache;
using Goobeer.DB.DataAttributeHelper;
using System.Reflection;

namespace Goobeer.DB.Builder
{
    public abstract class SqlBuilderBase
    {
        protected ICacheable CacheRepertory { get; set; }

        protected abstract void InitCache(ICacheable cacheRepertory);

        /// <summary>
        /// 获得字段信息(映射到数据库的字段和实体属性信息)
        /// </summary>
        /// <param name="tableName">要带出的表名</param>
        /// <returns>实体的 属性+字段 集合</returns>
        public IDictionary<PropertyInfo, FieldAttribute> GetFields<E>(out TableAttribute tableAttr) where E : class, new()
        {
            IDictionary<PropertyInfo, FieldAttribute> idcPF = null;
            Type type = typeof(E);
            TableAttribute.Fields = TableAttribute.Fields ?? new Dictionary<string, IDictionary<TableAttribute, IDictionary<PropertyInfo, FieldAttribute>>>();

            if (!TableAttribute.Fields.ContainsKey(type.FullName))
            {
                TableAttribute tabAttributes = type.GetCustomAttributes(typeof(TableAttribute), true).Cast<TableAttribute>().SingleOrDefault();
                idcPF = new Dictionary<PropertyInfo, FieldAttribute>();

                if (tabAttributes != null)
                {
                    tabAttributes.TableName = tabAttributes.TableName ?? type.Name;
                }
                else
                {
                    tabAttributes = new TableAttribute(type.Name);
                }

                IEnumerable<PropertyInfo> properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty).Where(pi => (pi.GetCustomAttribute<FieldAttribute>(true) == null && pi.GetCustomAttribute<FieldIgnoreAttribute>(true) == null) || pi.GetCustomAttribute<FieldAttribute>(true) != null || pi.GetCustomAttribute<FieldIgnoreAttribute>(true) == null);

                foreach (PropertyInfo item in properties)
                {
                    FieldAttribute attribute = item.GetCustomAttribute(typeof(FieldAttribute), true) as FieldAttribute;

                    if (attribute != null)
                    {
                        attribute.FieldName = attribute.FieldName ?? item.Name;
                    }
                    else
                    {
                        attribute = new FieldAttribute(item.Name);
                    }
                    idcPF.Add(item, attribute);
                }

                Dictionary<TableAttribute, IDictionary<PropertyInfo, FieldAttribute>> dic = new Dictionary<TableAttribute, IDictionary<PropertyInfo, FieldAttribute>>();
                dic.Add(tabAttributes, idcPF);
                TableAttribute.Fields.Add(type.FullName, dic);
                tableAttr = tabAttributes;
            }
            else
            {
                idcPF = TableAttribute.Fields[type.FullName].Values.First();
                tableAttr = TableAttribute.Fields[type.FullName].Keys.First();
            }

            return idcPF;
        }

    }
}
