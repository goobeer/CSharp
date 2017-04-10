using Goobeer.DB.CommandImp.R;
using Goobeer.DB.Context.BaseContext;
using Goobeer.DB.Context.Query;
using Goobeer.DB.DataAttributeHelper;
using Goobeer.DB.ReflectionHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Goobeer.DB.Context.R
{
    /// <summary>
    /// 关系型 数据库查询上下文
    /// </summary>
    public class RBaseQueryContext : QueryContext
    {
        /// <summary>
        /// 参数化操作前缀
        /// </summary>
        public string ParamPrefix { get; set; }

        public RDBRepertory DBHelper { get; set; }
        
        /// <summary>
        /// 分页
        /// </summary>
        public Func<string,string,uint,uint,string> Pagination { get; set; }

        protected string OrderString { get; set; }
        protected string GroupString { get; set; }

        public RBaseQueryContext() { }

        public RBaseQueryContext(string connectionString, string providerName)
        {
            DBHelper = new RDBRepertory(connectionString, providerName);
        }
        
        #region 查询
        #region 多表查询

        public override int MultTabCountQuery<E>()
        {
            int result = 0;
            var cmd = ParseMult<E>(true);
            
            result = (int)DBHelper.ExecuteScalar(cmd);
            return result;
        }

        public override List<E> MultTabQuery<E>(uint pageNum, uint pageSize)
        {
            ClearState();
            List<E> result = null;
            var cmd = ParseMult<E>();
            cmd.CommandText = Pagination(cmd.CommandText, OrderString, pageNum, pageSize);
            result = DBHelper.Select<E>(cmd);
            return result;
        }

        public override List<E> MultTabQuery<E>()
        {
            ClearState();
            List<E> result = null;
            var cmd = ParseMult<E>();
            result = DBHelper.Select<E>(cmd);
            return result;
        }
        #endregion

        #region 单表查询

        public override int SingleTabCountQuery<E>()
        {
            ClearState();
            int result = 0;
            var cmd = ParseSingle<E>(true);
            result = (int)DBHelper.ExecuteScalar(cmd);
            return result;
        }

        public override List<E> SingleTabQuery<E>()
        {
            ClearState();
            List<E> result = null;
            var cmd = ParseSingle<E>();
            var hashKey = cmd.GenerateHash();
            //result = Cache.Get<List<E>>(hashKey);
            //if (result==null)
            //{
            //    result = ConvertT2E<E>(DBHelper.Select(cmd));
            //    Cache.Set(hashKey, result,false);
            //}

            result = DBHelper.Select<E>(cmd);
            return result;
        }

        public override List<E> SingleTabQuery<E>(uint pageNum, uint pageSize)
        {
            ClearState();
            List<E> result = null;
            var cmd = ParseSingle<E>();
            cmd.CommandText = Pagination(cmd.CommandText, OrderString, pageNum, pageSize);
            result = DBHelper.Select<E>(cmd);
            return result;
        }
        #endregion

        /// <summary>
        /// 复杂查询
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public List<E> BasicTabQuerys<E>(DbCommand cmd) where E : class, new()
        {
            List<E> result = null;
            result = DBHelper.Select<E>(cmd);
            return result;
        }
        #endregion

        protected virtual void ClearState()
        {
            OrderString = string.Empty;
            GroupString = string.Empty;
        }

        private DbCommand ParseSingle<E>(bool useCount=false) where E : class, new()
        {
            var cmd = DBHelper.CreateDbCmd();
            StringBuilder sb = new StringBuilder();

            if (SingleQueryTableEntity==null)
            {
                SingleQueryTableEntity = new QueryTableEntity() { TabName = EntityReflection<E>.GetTableAttr().TableName };
            }

            SingleQueryTableEntity = SingleQueryTableEntity ?? new QueryTableEntity();

            if (!(SingleQueryTableEntity.SelectFields != null && SingleQueryTableEntity.SelectFields.Any()) && !useCount)
            {
                SingleQueryTableEntity.SelectFields = EntityReflection<E>.GetFields().Values.Select(fa => fa.FieldName).ToList();
            }

            sb.AppendFormat("select {0} from {1}", useCount ? "count(1)" : string.Join(",", SingleQueryTableEntity.SelectFields), SingleQueryTableEntity.TabName);
            if (QueryPredicates != null && QueryPredicates.Any())
            {
                var qp = QueryPredicates.First();
                string condition = RCondition.RenderConditon(cmd, ParamPrefix, qp.Data);
                if (!string.IsNullOrEmpty(condition))
                {
                    sb.AppendFormat(" where {0}", condition);
                }

                string group = ParseGroup(qp.GC, string.Empty);

                if (!string.IsNullOrEmpty(group))
                {
                    GroupString = group;
                    sb.AppendFormat(" group by {0}", group);
                }

                var order = ParseOrder(qp.OC, string.Empty);

                if (!string.IsNullOrEmpty(order))
                {
                    OrderString = order;
                    sb.AppendFormat(" order by {0}", order);
                }
            }
            cmd.CommandText = sb.ToString();
            return cmd;
        }

        private DbCommand ParseMult<E>(bool useCount=false) where E : class, new()
        {
            var cmd = DBHelper.CreateDbCmd();
            StringBuilder sb = new StringBuilder();

            Dictionary<string, List<string>> dicField = new Dictionary<string, List<string>>();

            string keyMaster = MultQueryTableEntity.Master.AliasTabName ?? MultQueryTableEntity.Master.TabName;
            if (MultQueryTableEntity.Master.SelectFields != null && MultQueryTableEntity.Master.SelectFields.Any())
            {
                dicField.Add(keyMaster, MultQueryTableEntity.Master.SelectFields);
            }

            StringBuilder sbjoin = new StringBuilder();
            StringBuilder sbCondition = new StringBuilder();
            StringBuilder sbGroup = new StringBuilder();
            StringBuilder sbOrder = new StringBuilder();

            string masterCondition = string.Empty;
            if (MultQueryTableEntity.Master.QP != null)
            {
                masterCondition = RCondition.RenderConditon(cmd, ParamPrefix, MultQueryTableEntity.Master.QP.Data);
                string group = ParseGroup(MultQueryTableEntity.Master.QP.GC, keyMaster);
                if (!string.IsNullOrEmpty(group))
                {
                    sbGroup.Append(group);
                }
                string order = ParseOrder(MultQueryTableEntity.Master.QP.OC, keyMaster);
                if (!string.IsNullOrEmpty(order))
                {
                    sbOrder.Append(order);
                }
            }

            if (!string.IsNullOrEmpty(masterCondition))
            {
                sbCondition.Append(masterCondition);
            }
            if (string.IsNullOrEmpty(MultQueryTableEntity.Master.AliasTabName))
            {
                sbjoin.Append(keyMaster);
            }
            else
            {
                sbjoin.AppendFormat("{0} {1}", MultQueryTableEntity.Master.TabName, keyMaster);
            }
            var directSlaveTabs = MultQueryTableEntity.SlaveJoinTable.Where(jqtec => (jqtec.L.TabName == MultQueryTableEntity.Master.TabName && jqtec.L.AliasTabName == MultQueryTableEntity.Master.AliasTabName) || (jqtec.R.TabName == MultQueryTableEntity.Master.TabName && jqtec.R.AliasTabName == MultQueryTableEntity.Master.AliasTabName));

            var notDirectSlaveTabs = MultQueryTableEntity.SlaveJoinTable.Where(jqtec => (!(jqtec.L.TabName == MultQueryTableEntity.Master.TabName && jqtec.L.AliasTabName == MultQueryTableEntity.Master.AliasTabName) || (jqtec.R.TabName == MultQueryTableEntity.Master.TabName && jqtec.R.AliasTabName == MultQueryTableEntity.Master.AliasTabName)));

            #region parse join
            foreach (var item in directSlaveTabs)
            {
                string keyL = item.L.AliasTabName ?? item.L.TabName;
                string keyR = item.R.AliasTabName ?? item.R.TabName;

                if (!dicField.ContainsKey(keyL))
                {
                    if (item.L.SelectFields != null && item.L.SelectFields.Any())
                    {
                        dicField.Add(keyL, item.L.SelectFields);
                    }
                }

                if (!dicField.ContainsKey(keyR))
                {
                    if (item.R.SelectFields != null && item.R.SelectFields.Any())
                    {
                        dicField.Add(keyR, item.R.SelectFields);
                    }
                }
                string joinPartTabName = string.Empty;
                string condition = string.Empty;
                string group = string.Empty;
                string order = string.Empty;
                if (item.L.AliasTabName == keyMaster || item.L.TabName == keyMaster)
                {
                    if (!string.IsNullOrEmpty(item.R.AliasTabName))
                    {
                        joinPartTabName = string.Format("{0} {1}", item.R.TabName, item.R.AliasTabName);
                    }
                    else
                    {
                        joinPartTabName = item.R.TabName;
                    }
                    if (item.R.QP != null)
                    {
                        condition = RCondition.RenderConditon(cmd, ParamPrefix, item.R.QP.Data);
                        group = ParseGroup(item.R.QP.GC, keyR);
                        order = ParseOrder(item.R.QP.OC, keyR);
                    }
                }
                else if (item.R.AliasTabName == keyMaster || item.R.TabName == keyMaster)
                {
                    if (!string.IsNullOrEmpty(item.L.AliasTabName))
                    {
                        joinPartTabName = string.Format("{0} {1}", item.L.TabName, item.L.AliasTabName);
                    }
                    else
                    {
                        joinPartTabName = item.L.TabName;
                    }
                    if (item.L.QP != null)
                    {
                        condition = RCondition.RenderConditon(cmd, ParamPrefix, item.L.QP.Data);
                        group = ParseGroup(item.L.QP.GC, keyL);
                        order = ParseOrder(item.L.QP.OC, keyL);
                    }
                }
                sbjoin.AppendFormat(" {0} {1} on {2}.{3}={4}.{5}", item.ConvertJoinType(), joinPartTabName, keyL, item.LJoinField, keyR, item.RJoinField);
                if (!string.IsNullOrEmpty(condition))
                {
                    sbCondition.AppendFormat(" {0}", condition);
                }
                if (!string.IsNullOrEmpty(group))
                {
                    sbGroup.AppendFormat(" {1}{0}", group, sbGroup.Length > 0 ? "," : string.Empty);
                }
                if (!string.IsNullOrEmpty(order))
                {
                    sbOrder.AppendFormat(" {1}{0}", order, sbOrder.Length > 0 ? "," : string.Empty);
                }
            }

            foreach (var item in notDirectSlaveTabs)
            {
                string keyL = item.L.AliasTabName ?? item.L.TabName;
                string keyR = item.R.AliasTabName ?? item.R.TabName;

                if (!dicField.ContainsKey(keyL))
                {
                    if (item.L.SelectFields != null && item.L.SelectFields.Any())
                    {
                        dicField.Add(keyL, item.L.SelectFields);
                    }
                }

                if (!dicField.ContainsKey(keyR))
                {
                    if (item.R.SelectFields != null && item.R.SelectFields.Any())
                    {
                        dicField.Add(keyR, item.R.SelectFields);
                    }
                }
                string joinPartTabName = string.Empty;
                string condition = string.Empty;
                string group = string.Empty;
                string order = string.Empty;
                //非直接相关的 关联 表，不与 左侧的表 相关联 就与 右侧的表相关联
                bool isLeft = directSlaveTabs.Any(e => (e.L.AliasTabName == item.L.AliasTabName && e.L.TabName == item.L.TabName) || (e.R.AliasTabName == item.L.AliasTabName && e.R.TabName == item.L.TabName));

                if (isLeft)
                {
                    if (!string.IsNullOrEmpty(item.R.AliasTabName))
                    {
                        joinPartTabName = string.Format("{0} {1}", item.R.TabName, item.R.AliasTabName);
                    }
                    else
                    {
                        joinPartTabName = item.R.TabName;
                    }
                    if (item.R.QP != null)
                    {
                        condition = RCondition.RenderConditon(cmd, ParamPrefix, item.R.QP.Data);
                        group = ParseGroup(item.R.QP.GC, keyR);
                        order = ParseOrder(item.R.QP.OC, keyR);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.L.AliasTabName))
                    {
                        joinPartTabName = string.Format("{0} {1}", item.L.TabName, item.L.AliasTabName);
                    }
                    else
                    {
                        joinPartTabName = item.L.TabName;
                    }
                    if (item.L.QP != null)
                    {
                        condition = RCondition.RenderConditon(cmd, ParamPrefix, item.L.QP.Data);
                        group = ParseGroup(item.L.QP.GC, keyL);
                        order = ParseOrder(item.L.QP.OC, keyL);
                    }
                }
                sbjoin.AppendFormat(" {0} {1} on {2}.{3}={4}.{5}", item.ConvertJoinType(), joinPartTabName, keyL, item.LJoinField, keyR, item.RJoinField);
                if (string.IsNullOrEmpty(condition))
                {
                    sbCondition.AppendFormat(" {0}", condition);
                }

                if (!string.IsNullOrEmpty(group))
                {
                    sbGroup.AppendFormat(" {1}{0}", group, sbGroup.Length > 0 ? "," : string.Empty);
                }
                if (!string.IsNullOrEmpty(order))
                {
                    sbOrder.AppendFormat(" {1}{0}", order, sbOrder.Length > 0 ? "," : string.Empty);
                }
            }
            #endregion

            sb.Append("select ");
            #region fields
            if (!useCount)
            {
                int i = 0;
                foreach (var item in dicField)
                {
                    if (i != dicField.Count - 1)
                    {
                        sb.AppendFormat("{0}.{1},", item.Key, string.Join(string.Format(",{0}.", item.Key), item.Value.Distinct()));
                    }
                    else
                    {
                        sb.AppendFormat("{0}.{1}", item.Key, string.Join(string.Format(",{0}.", item.Key), item.Value.Distinct()));
                    }
                    i++;
                }
            }
            else
            {
                sb.Append("count(1)");
            }
            #endregion
            sb.AppendFormat(" from {0}", sbjoin.ToString());
            if (sbCondition.Length > 0)
            {
                sb.AppendFormat(" where {0}", sbCondition.ToString());
            }

            if (sbGroup.Length > 0)
            {
                GroupString = sbGroup.ToString();
                sb.AppendFormat(" group by {0}", GroupString);
            }

            if (sbOrder.Length > 0)
            {
                OrderString = sbOrder.ToString();
                sb.AppendFormat(" order by {0}", OrderString);
            }

            cmd.CommandText = sb.ToString();
            return cmd;
        }

        private List<E> ConvertT2E<E>(DataTable table)
            where E : class, new()
        {
            List<E> list = new List<E>();
            IDictionary<PropertyInfo, FieldAttribute> idcs = EntityReflection<E>.GetFields();

            foreach (DataRow row in table.Rows)
            {
                E e = new E();
                foreach (PropertyInfo pi in idcs.Keys)
                {
                    var fieldName = idcs[pi].FieldName;
                    //var r = row.Field(fieldName);
                    //TODO 待优化
                    if (table.Columns.Contains(fieldName) && row[fieldName] != DBNull.Value)
                    {
                        pi.SetValue(e, Convert.ChangeType(row[fieldName], pi.PropertyType));
                    }
                }
                list.Add(e);
            }
            return list;
        }

        private string ParseGroup(List<GroupCriteria> gcs, string tableName)
        {
            StringBuilder sb = new StringBuilder();
            if (gcs != null && gcs.Any())
            {
                for (int i = 0; i < gcs.Count; i++)
                {
                    var item = gcs[i];
                    if (!(item.GroupFields != null && item.GroupFields.Any()))
                    {
                        continue;
                    }
                    if (i != gcs.Count - 1)
                    {
                        if (string.IsNullOrEmpty(tableName))
                        {
                            sb.AppendFormat("{0},", string.Join(",", item.GroupFields));
                        }
                        else
                        {
                            sb.AppendFormat("{0},", string.Format("{0}.{1}", tableName, string.Join(string.Format(",{0}.", tableName), item.GroupFields)));
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(tableName))
                        {
                            sb.AppendFormat("{0}", string.Join(",", item.GroupFields));
                        }
                        else
                        {
                            sb.AppendFormat("{0}", string.Format("{0}.{1}", tableName, string.Join(string.Format(",{0}.", tableName), item.GroupFields)));
                        }
                    }
                }
            }
            return null;
        }

        private string ParseOrder(List<OrderCriteria> ocs, string tableName)
        {
            StringBuilder sb = new StringBuilder();
            if (ocs != null && ocs.Any())
            {
                for (int i = 0; i < ocs.Count; i++)
                {
                    var item = ocs[i];
                    if (!(item.Orders != null && item.Orders.Any()))
                    {
                        continue;
                    }
                    if (i != ocs.Count - 1)
                    {
                        int j = 0;
                        foreach (var kvp in item.Orders)
                        {
                            if (j != item.Orders.Count - 1)
                            {
                                sb.AppendFormat("{0} {1},", string.IsNullOrEmpty(tableName) ? kvp.Key : string.Format("{0}.{1}", tableName, kvp.Key), kvp.Value);
                            }
                            else
                            {
                                sb.AppendFormat("{0} {1}", string.IsNullOrEmpty(tableName) ? kvp.Key : string.Format("{0}.{1}", tableName, kvp.Key), kvp.Value);
                            }
                            j++;
                        }
                    }
                    else
                    {
                        int j = 0;
                        foreach (var kvp in item.Orders)
                        {
                            if (j != item.Orders.Count - 1)
                            {
                                sb.AppendFormat("{0} {1},", string.IsNullOrEmpty(tableName) ? kvp.Key : string.Format("{0}.{1}", tableName, kvp.Key), kvp.Value);
                            }
                            else
                            {
                                sb.AppendFormat("{0} {1}", string.IsNullOrEmpty(tableName) ? kvp.Key : string.Format("{0}.{1}", tableName, kvp.Key), kvp.Value);
                            }
                            j++;
                        }
                    }
                }
            }
            return sb.ToString();
        }
    }
}
