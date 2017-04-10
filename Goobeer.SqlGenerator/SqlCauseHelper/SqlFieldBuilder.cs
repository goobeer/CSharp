using Goobeer.DB.SqlCauseHelper;
using Goobeer.DB.DataAttributeHelper;
using Goobeer.DB.SqlCauseHelper.Condition;
using Goobeer.DB.SqlCauseHelper.Operator;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Goobeer.DB.SqlCauseHelper
{
    public class SqlFieldBuilder
    {
        public Dictionary<string,dynamic> KVP { get; }

        public SqlFieldBuilder()
        {
            KVP = new Dictionary<string, dynamic>();
        }

        #region 添加操作字段
        public SqlFieldBuilder AddField(string key,dynamic val)
        {
            KVP.Add(key, val);
            return this;
        }

        #endregion

        /// <summary>
        /// 获得操作的格式化 字段范围(仅供 insert,update 使用)
        /// </summary>
        /// <param name="useParam"></param>
        /// <param name="sqlOperators"></param>
        /// <param name="fieldStrategy"></param>
        /// <returns></returns>
        public ISqlResult BuildField(bool useParam, SqlOperatorsBase sqlOperators, Func<SqlOperatorsBase, IDictionary<PropertyInfo, FieldAttribute>, bool, IDictionary<string, dynamic>, string> fieldStrategy, IDictionary<PropertyInfo, FieldAttribute> idcPF, DbCommand cmd)
        {
            var result = new FieldResult();

            string paraPrefix = sqlOperators.ParaPrefix ?? string.Empty;
            string paramKey = string.Empty;

            for (int i = 0; i < KVP.Count; i++)
            {
                var item = KVP.ElementAt(i);

                var propertyInfo = idcPF.Single(kvp => string.Compare(kvp.Value.FieldName, item.Key, true) == 0);

                List<string> lst = new List<string>();
                var v = item.Value;//条件原子项

                if (v is byte[])
                {
                    paramKey = string.Format("{1}fpara{0}", i, paraPrefix);
                    var param = cmd.CreateParameter();
                    param.ParameterName = paramKey;
                    param.Value = v;
                    result.Params.Add(param);
                    lst.Add(paramKey);
                }
                else
                {
                    if (useParam && v != null)
                    {
                        paramKey = string.Format("{1}fpara{0}", i, paraPrefix);
                        var param = cmd.CreateParameter();
                        param.ParameterName = paramKey;
                        if (string.Compare(propertyInfo.Key.PropertyType.Name, "boolean", true) == 0)
                        {
                            param.Value = v ? 1 : 0;
                        }
                        else
                        {
                            param.Value = v;
                        }
                        result.Params.Add(param);
                        lst.Add(paramKey);
                    }
                }
            }

            result.FieldTxt.Append(fieldStrategy(sqlOperators, idcPF, useParam, KVP));

            return result;
        }
    }
}
