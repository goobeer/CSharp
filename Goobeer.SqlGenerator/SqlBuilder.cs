using Goobeer.DB.SqlCauseHelper;
using Goobeer.DB.DataAttributeHelper;
using Goobeer.DB.ReflectionHelper;
using Goobeer.DB.SqlCauseHelper.Condition;
using Goobeer.DB.SqlCauseHelper.ISql;
using Goobeer.DB.SqlCauseHelper.Operator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Goobeer.DB
{
    /// <summary>
    /// sqlCause helper
    /// </summary>
    /// <typeparam name="E">Entity</typeparam>
    public class SqlBuilder : ISqlCommand
    {
        #region MyRegion
        /// <summary>
        /// 缓存 获得实体属性值的 Delegate
        /// </summary>
        private static volatile Dictionary<string, Delegate> PropDelegate = new Dictionary<string, Delegate>();

        private string UpdateFieldStrategy(SqlOperatorsBase sqlOperators, IDictionary<PropertyInfo, FieldAttribute> idcPF, bool useParam, IDictionary<string, dynamic> vals)
        {
            return string.Join(",", vals.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value == null ? "null" : useParam ? kvp.Value : sqlOperators.FormateCondition(idcPF.Single(k => string.Compare(k.Value.FieldName, kvp.Key, true) == 0), new SqlCondition() { FieldVal = kvp.Value }))));
        }

        private string InsertFieldStrategy(SqlOperatorsBase sqlOperators, IDictionary<PropertyInfo, FieldAttribute> idcPF, bool useParam, IDictionary<string, dynamic> vals)
        {
            return string.Format("({0}) values ({1})", string.Join(",", vals.Keys), string.Join(",", vals.Select(kvp => kvp.Value == null ? "null" : useParam ? kvp.Value : sqlOperators.FormateCondition(idcPF.Single(k => string.Compare(k.Value.FieldName, kvp.Key, true) == 0), new SqlCondition() { FieldVal = kvp.Value }))));
        }

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

        /// <summary>
        /// 获取字段+值集合
        /// </summary>
        /// <param name="e">实体</param>
        /// <param name="tableAttr">TableAttribute</param>
        /// <param name="fieldFilter"></param>
        /// <returns>字段+值集合</returns>
        private IDictionary<string, dynamic> GetFieldWithVal<E>(E e, out TableAttribute tableAttr, params Func<PropertyInfo, FieldAttribute, bool>[] fieldFilter) where E : class, new()
        {
            IDictionary<string, dynamic> vals = null;
            Type eType = typeof(E);
            if (TableAttribute.Fields != null && TableAttribute.Fields.ContainsKey(eType.FullName))
            {
                tableAttr = TableAttribute.Fields[eType.FullName].Keys.Single();
                vals = new Dictionary<string, dynamic>();
                var idcPF = TableAttribute.Fields[eType.FullName].Values.Single();

                foreach (var item in idcPF.Keys)
                {
                    if (fieldFilter != null)
                    {
                        bool flag = false;
                        foreach (var filter in fieldFilter)
                        {
                            if (filter != null && filter(item, idcPF[item]))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            continue;
                        }
                    }

                    var key = idcPF[item].FieldName;
                    string keyDelegate = string.Format("{0}-{1}", eType.FullName, item.Name);
                    Delegate getPropDelegate = null;
                    if (PropDelegate.ContainsKey(keyDelegate))
                    {
                        getPropDelegate = PropDelegate[keyDelegate];
                    }
                    else
                    {
                        getPropDelegate = EntityReflection<E>.CreateGetDelegate(e, item);
                        //System.CodeDom.
                        PropDelegate.Add(keyDelegate, getPropDelegate);
                    }
                    var val = getPropDelegate.DynamicInvoke(e);

                    vals.Add(key, val);
                }
            }
            else
            {
                IDictionary<PropertyInfo, FieldAttribute> idcs = GetFields<E>(out tableAttr);
                if (idcs.Count > 0)
                {
                    vals = GetFieldWithVal(e, out tableAttr, fieldFilter);
                }
                else
                {
                    throw new NotSupportedException("该实体类没有遵循默认规范。");
                }
            }
            return vals;
        }

        public virtual SqlOperatorsBase GetSqlOperators()
        {
            throw new NotImplementedException();
        } 
        #endregion

        public IWhere BuildWhere<E>(DbCommand cmd, bool useParam, ISqlConditionBuilder conditon, SqlOperatorsBase sqlOperators) where E : class, new()
        {
            TableAttribute tableAttr = null;
            IDictionary<PropertyInfo, FieldAttribute> idcPF = GetFields<E>(out tableAttr);
            var condtionResult = conditon.BuildCondition(useParam, sqlOperators, idcPF, cmd);
            if (conditon.ListConditions!=null && conditon.ListConditions.Any())
            {
                cmd.CommandText = string.Format("{0} where {1}", cmd.CommandText, condtionResult.CondtionText.ToString());
            }
            
            if (useParam)
            {
                cmd.Parameters.AddRange(condtionResult.Params.ToArray());
            }
            return this;
        }

        public IWhere BuildWhere<E>(DbCommand cmd, bool useParam, ISqlConditionBuilder conditon, SqlOperatorsBase sqlOperators,params ConditionComponent[] conditions) where E : class, new()
        {
            TableAttribute tableAttr = null;
            IDictionary<PropertyInfo, FieldAttribute> idcPF = GetFields<E>(out tableAttr);
            var condtionResult = conditon.BuildCondition(useParam, sqlOperators, idcPF, cmd,conditions);
            if (conditions != null && conditions.Any())
            {
                cmd.CommandText = string.Format("{0} where {1}", cmd.CommandText, condtionResult.CondtionText.ToString());
            }

            if (useParam)
            {
                cmd.Parameters.AddRange(condtionResult.Params.ToArray());
            }
            return this;
        }

        #region 生成select sql
        /// <summary>
        /// 获得select sql
        /// </summary>
        /// <param name="fields">要检索的字段名称集合(可为空,若为空:则检索实体的全部属性)</param>
        /// <returns>select sql</returns>
        public ISelect BuildSelect<E>(DbCommand cmd, params string[] fields) where E:class,new()
        {
            TableAttribute tableAttr = null;
            IDictionary<PropertyInfo, FieldAttribute> idcPF = GetFields<E>(out tableAttr);

            ICollection<FieldAttribute> listFieldAttributes = idcPF.Values;

            cmd.CommandText = string.Format("select {0} from {1}{2}", (fields != null && fields.Length > 0) ? string.Join(",", fields) : string.Join(",", listFieldAttributes.Select(fa => fa.FieldName).ToArray()), tableAttr.DBName ?? string.Empty, tableAttr.TableName);
            return this;
        }
        
        public ISelect BuildCount<E>(DbCommand cmd, string countField) where E : class, new()
        {
            return BuildSelect<E>(cmd, string.Format("count({0})", countField));
        }

        public ISelect BuildGroup<E>(DbCommand cmd, string[] selectFields, string[] groupFields) where E : class, new()
        {
            BuildSelect<E>(cmd, selectFields);
            cmd.CommandText = string.Format("{0} group by {1}", cmd.CommandText, string.Join(",", groupFields));
            return this;
        }

        public IWhere BuildOrder<E>(DbCommand cmd, OrderCriteria oc) where E : class, new()
        {
            StringBuilder sb = new StringBuilder("order by ");
            int count = oc.Orders.Count;
            if (count == 0)
            {
                throw new NotSupportedException();
            }
            else if (count==1)
            {
                var item = oc.Orders.First();
                sb.AppendFormat("{0} {1}", item.Key, item.Value);
            }
            else
            {
                for (int i = 0; i < oc.Orders.Count; i++)
                {
                    var key = oc.Orders.Keys.ElementAt(i);
                    if (i != oc.Orders.Count - 1)
                    {
                        sb.AppendFormat("{0} {1},", key, oc.Orders[key]);
                    }
                    else
                    {
                        sb.AppendFormat("{0} {1}", key, oc.Orders[key]);
                    }
                }
            }

            cmd.CommandText = string.Format("{0} {1}",cmd.CommandText,sb.ToString());

            return this;
        }

        /// <summary>
        /// 获得select sql
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="selectFields">要检索的字段名称集合(可为空,若为空:则检索实体的全部属性)</param>
        /// <param name="useParam">是否使用参数化查询</param>
        /// <param name="conditon">检索条件</param>
        /// <param name="sqlOperators">条件操作格式</param>
        /// <param name="cmd">select command</param>
        /// <returns>select语句</returns>
        public IWhere BuildWhere<E>(DbCommand cmd, string[] selectFields, bool useParam, ISqlConditionBuilder conditon, SqlOperatorsBase sqlOperators)
            where E:class,new()
        {
            return BuildSelect<E>(cmd, selectFields).BuildWhere<E>(cmd, useParam, conditon, sqlOperators);
        }

        public IWhere BuildWhere<E>(DbCommand cmd, string[] selectFields, bool useParam, ISqlConditionBuilder conditon, SqlOperatorsBase sqlOperators, params ConditionComponent[] conditions)
            where E : class, new()
        {
            return BuildSelect<E>(cmd, selectFields).BuildWhere<E>(cmd, useParam, conditon, sqlOperators,conditions);
        }

        public IWhere BuildCount<E>(DbCommand cmd, string countField, bool useParam, ISqlConditionBuilder conditon, SqlOperatorsBase sqlOperators) where E : class, new()
        {
            return BuildCount<E>(cmd, countField).BuildWhere<E>(cmd, useParam, conditon, sqlOperators);
        }

        public ISelect BuildJoin<R>(DbCommand cmd, List<JoinTableEntity> joinTableEntity) where R : class, new()
        {
            JoinResult result = new JoinResult() { JoinTableEntity = joinTableEntity };
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("select {0}",result.RenderSql());
            cmd.CommandText = sb.ToString();
            return this;
        }

        #endregion

        #region 分页
        public virtual ISelect Take(int count,int offset)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 生成insert语句
        public IInsert BuildInsert<E>(DbCommand cmd, E e, SqlOperatorsBase sqlOperators)
            where E : class, new()
        {
            return BuildInsert<E>(cmd, e, sqlOperators, null);
        }
        
        public IInsert BuildInsert<E>(DbCommand cmd, E e, SqlOperatorsBase sqlOperators, Func<PropertyInfo, FieldAttribute, bool> fieldFilter = null)
            where E : class, new()
        {
            if (e == null) return null;

            TableAttribute tableAttr = null;

            Func<PropertyInfo, FieldAttribute, bool> acFilter = (pi, fa) => { return fa.AutoCreate; };

            IDictionary<PropertyInfo, FieldAttribute> idcPF = GetFields<E>(out tableAttr);

            IDictionary<string, dynamic> fieldWithVals = GetFieldWithVal(e, out tableAttr, fieldFilter, acFilter);

            if (fieldWithVals.Count == 0) return null;

            SqlConditionBuilder conditionBuilder = new SqlConditionBuilder();
            foreach (string fieldName in fieldWithVals.Keys)
            {
                SqlCondition condition = new SqlCondition() { FieldName = fieldName, FieldVal = fieldWithVals[fieldName], SqlOperation = SqlOperator.None, ConditionOperator = ConditionOperator.None };
                conditionBuilder.AddNonQueryField(condition);
            }

            conditionBuilder.BuildNonQueryField(true, sqlOperators, InsertFieldStrategy, idcPF, cmd);

            cmd.CommandText = string.Format("insert into {0}{1} {2}", tableAttr.DBName ?? string.Empty, tableAttr.TableName, cmd.CommandText);

            return this;
        }

        /// <summary>
        /// 单条记录插入
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="keyValPairs"></param>
        /// <param name="useParam"></param>
        /// <param name="cmd"></param>
        /// <param name="sqlOperators"></param>
        /// <returns></returns>
        public IInsert BuildInsert<E>(DbCommand cmd, IDictionary<string, dynamic> keyValPairs, SqlOperatorsBase sqlOperators)
            where E : class, new()
        {
            if (keyValPairs == null || keyValPairs.Count == 0)
            {
                throw new Exception("传入参数不能为空，且必须有值。");
            }

            TableAttribute tableAttr = null;
            IDictionary<PropertyInfo, FieldAttribute> idcPF = GetFields<E>(out tableAttr);

            var errorField = keyValPairs.Where(kvp => idcPF.Values.All(fa => string.Compare(fa.FieldName, kvp.Key, StringComparison.OrdinalIgnoreCase) != 0));

            if (errorField.Any())
            {
                throw new Exception(string.Format("传入的字段[{0}]不存在相应的实体{1}中。", string.Join(",", errorField.Select(ef => ef.Key)), typeof(E)));
            }

            SqlConditionBuilder conditionBuilder = new SqlConditionBuilder();
            foreach (string fieldName in keyValPairs.Keys)
            {
                SqlCondition condition = new SqlCondition() { FieldName = fieldName, FieldVal = keyValPairs[fieldName], ConditionOperator = ConditionOperator.None, SqlOperation = SqlOperator.None };
                conditionBuilder.AddNonQueryField(condition);
            }

            conditionBuilder.BuildNonQueryField(true, sqlOperators, InsertFieldStrategy, idcPF,cmd);
            cmd.CommandText = string.Format("insert into {0}{1} {2}", tableAttr.DBName ?? string.Empty, tableAttr.TableName, cmd.CommandText);
            return this;
        }
        #endregion

        #region 生成delete 语句
        /// <summary>
        /// 生成单个实体的 delete sql,依据主键删除
        /// </summary>
        /// <param name="e">实体</param>
        /// <param name="sqlOperators"></param>
        /// <param name="useParam"></param>
        /// <returns>IDelete</returns>
        public IDelete BuildDelete<E>(DbCommand cmd, E e, SqlOperatorsBase sqlOperators, bool useParam = true)
            where E : class, new()
        {
            TableAttribute tableAttr = null;
            IDictionary<PropertyInfo, FieldAttribute> idcPF = GetFields<E>(out tableAttr);

            IDictionary<string, dynamic> idcVals = GetFieldWithVal(e, out tableAttr, null);

            var pk = idcPF.SingleOrDefault(kvp => kvp.Value.IsPK);

            var pkVal = idcVals.SingleOrDefault(kvp => string.Compare(kvp.Key, pk.Key.Name, true) == 0);
            if (string.IsNullOrEmpty(pkVal.Key))
            {
                throw new Exception("当前实体类缺少主键!");
            }

            SqlConditionBuilder conditionBuilder = new SqlConditionBuilder();

            SqlCondition condition = new SqlCondition() { FieldName = pkVal.Key, FieldVal = pkVal.Value, ConditionOperator = ConditionOperator.And, SqlOperation = SqlOperator.Equal };

            conditionBuilder.AddCondition(condition);
            return BuildDelete<E>(cmd, conditionBuilder, sqlOperators,useParam);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditon">要操作的条件集合</param>
        /// <param name="sqlOperators">sql 运算符转换</param>
        /// <param name="useParam">是否使用参数化操作</param>
        /// <returns>delete sql</returns>
        public IDelete BuildDelete<E>(DbCommand cmd, SqlConditionBuilder conditon, SqlOperatorsBase sqlOperators, bool useParam = true)
            where E : class, new()
        {
            StringBuilder sb = new StringBuilder();
            TableAttribute tableAttr = null;
            IDictionary<PropertyInfo, FieldAttribute> idcPF = GetFields<E>(out tableAttr);
            cmd.CommandText = string.Format("delete from {0}{1}", tableAttr.DBName ?? string.Empty, tableAttr.TableName);

            BuildWhere<E>(cmd, useParam, conditon, sqlOperators);
            return this;
        }
        #endregion

        #region 生成update 语句
        /// <summary>
        /// 根据主键更新实体
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="e"></param>
        /// <param name="cmd"></param>
        /// <param name="sqlOperators"></param>
        /// <returns></returns>
        public IUpdate BuildUpdate<E>(DbCommand cmd, E e, SqlOperatorsBase sqlOperators)
            where E : class, new()
        {
            return BuildUpdate<E>(cmd, e, true, sqlOperators,null);
        }

        /// <summary>
        /// 根据主键更新实体,对值进行过滤
        /// </summary>
        /// <param name="e">实体</param>
        /// <param name="valFilter">值过滤操作</param>
        /// <returns>update sql</returns>
        public IUpdate BuildUpdate<E>(DbCommand cmd, E e, bool useParam, SqlOperatorsBase sqlOperators, Func<PropertyInfo, FieldAttribute, bool> fieldFilter = null)
            where E : class, new()
        {
            TableAttribute tableAttr = null;
            IDictionary<PropertyInfo, FieldAttribute> idcPF = GetFields<E>(out tableAttr);
            KeyValuePair<PropertyInfo, FieldAttribute> kvpPK = idcPF.Single(item => item.Value.IsPK);
            if (kvpPK.Key == null)
            {
                throw new Exception("没有为实体设置主键。");
            }

            Func<PropertyInfo, FieldAttribute, bool> pkFilter = (pi, fa) => fa.AutoCreate && !fa.IsPK;

            IDictionary<string, dynamic> idcs = GetFieldWithVal(e, out tableAttr, fieldFilter,pkFilter);
            KeyValuePair<string, dynamic> pk = idcs.Single(idc => string.Compare(idc.Key, kvpPK.Value.FieldName, true) == 0);
            if (idcs.Contains(pk))
            {
                idcs.Remove(pk);
            }
            cmd.CommandText=string.Format("update {0}{1} set ", tableAttr.DBName ?? string.Empty, tableAttr.TableName);

            SqlConditionBuilder conditonBuilder = new SqlConditionBuilder();
            SqlCondition conditionPK = new SqlCondition() { FieldName = pk.Key, FieldVal = pk.Value, ConditionOperator = ConditionOperator.And, SqlOperation = SqlOperator.Equal };
            conditonBuilder.AddCondition(conditionPK);
            
            foreach (var item in idcs)
            {
                SqlCondition condition = new SqlCondition() { FieldName = item.Key, FieldVal = item.Value, ConditionOperator = ConditionOperator.None, SqlOperation = SqlOperator.None };
                conditonBuilder.AddNonQueryField(condition);
            }

            conditonBuilder.BuildNonQueryField(useParam, sqlOperators, UpdateFieldStrategy, idcPF,cmd);

            BuildWhere<E>(cmd, useParam, conditonBuilder, sqlOperators);
            return this;
        }

        /// <summary>
        /// IUpdate
        /// </summary>
        /// <param name="idcs">要更新的键值对集合</param>
        /// <param name="useParam">是否使用参数化操作</param>
        /// <param name="conditonBuilder">条件集合</param>
        /// <param name="sqlOperators">sql操作符转换操作</param>
        /// <returns>IUpdate</returns>
        public IUpdate BuildUpdate<E>(DbCommand cmd, IDictionary<string, dynamic> idcs, bool useParam, ISqlConditionBuilder conditonBuilder, SqlOperatorsBase sqlOperators)
            where E : class, new()
        {
            if (idcs == null || idcs.Count == 0)
            {
                throw new Exception("更新的值集合非法。");
            }
            TableAttribute tableAttr = null;
            IDictionary<PropertyInfo, FieldAttribute> idcPF = GetFields<E>(out tableAttr);
            cmd.CommandText = string.Format("update {0}{1} set ", tableAttr.DBName ?? string.Empty, tableAttr.TableName);

            foreach (var item in idcs)
            {
                SqlCondition condition = new SqlCondition() { FieldName = item.Key, FieldVal = item.Value, ConditionOperator = ConditionOperator.None, SqlOperation = SqlOperator.None };
                conditonBuilder.AddNonQueryField(condition);
            }

            conditonBuilder.BuildNonQueryField(useParam, sqlOperators, UpdateFieldStrategy, idcPF, cmd);
            BuildWhere<E>(cmd, useParam, conditonBuilder, sqlOperators);

            return this;
        }
        #endregion

        public ICreate BuildCreate<E>(E e) where E : class ,new()
        {
            throw new NotImplementedException();
        }

        //多表操作(表间关系)

        //数据库中 自定义函数、存储过程
    }
}