using Goobeer.Cache;
using Goobeer.DB.SqlCauseHelper;
using Goobeer.DB.DataAttributeHelper;
using Goobeer.DB.SqlCauseHelper.Condition;
using Goobeer.DB.SqlCauseHelper.ISql;
using Goobeer.DB.SqlCauseHelper.Operator;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Goobeer.DB.Builder
{
    public class SelectBuilder: SqlBuilderBase
    {
        protected override void InitCache(ICacheable cacheRepertory)
        {
            CacheRepertory = cacheRepertory;
        }

        /// <summary>
        /// 单表查询
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="fields"></param>
        public void BuildSelect<E>(DbCommand cmd, bool useParam, ISqlConditionBuilder conditon, SqlOperatorsBase sqlOperators, params string[] fields) where E : class, new()
        {
            TableAttribute tableAttr = null;
            IDictionary<PropertyInfo, FieldAttribute> idcPF = GetFields<E>(out tableAttr);

            ICollection<FieldAttribute> listFieldAttributes = idcPF.Values;

            cmd.CommandText = string.Format("select {0} from {1}{2}", (fields != null && fields.Length > 0) ? string.Join(",", fields) : string.Join(",", listFieldAttributes.Select(fa => fa.FieldName).ToArray()), tableAttr.DBName ?? string.Empty, tableAttr.TableName);

            if (conditon!=null && conditon.ListConditions!=null && conditon.ListConditions.Any())
            {
                var condtionResult = conditon.BuildCondition(useParam, sqlOperators, idcPF, cmd);

                cmd.CommandText = string.Format("{0} where {1}", cmd.CommandText, condtionResult.CondtionText.ToString());

            if (useParam)
            {
                cmd.Parameters.AddRange(condtionResult.Params.ToArray());
            }

        }
        }

        /// <summary>
        /// 多表查询
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="joinTableEntity"></param>
        public void BuildJoin<R>(DbCommand cmd, List<JoinTableEntity> joinTableEntity) where R : class, new()
        {
            JoinResult result = new JoinResult() { JoinTableEntity = joinTableEntity };
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("select {0}", result.RenderSql());
            cmd.CommandText = sb.ToString();
        }
    }
}
