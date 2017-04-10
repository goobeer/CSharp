using Goobeer.DB.SqlCauseHelper.ISql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Goobeer.DB.SqlCauseHelper
{
    public class JoinResult : ISqlResult
    {
        public StringBuilder JoinTxt { get; set; }
        public List<JoinTableEntity> JoinTableEntity { get; set; }
        public Dictionary<string,string> TableNames { get; set; }

        public JoinResult()
        {
            JoinTxt = new StringBuilder();
            TableNames = new Dictionary<string, string>();
        }

        public string RenderSql()
        {
            StringBuilder sbJoin = new StringBuilder();
            for (int i = 0; i < JoinTableEntity.Count; i++)
            {
                var item = JoinTableEntity[i];
                string tableAName = string.Format("a_{0}", i);
                string tableBName = string.Format("b_{0}", i);

                var aName = TableNames.Values.FirstOrDefault(k => string.Compare(item.TabAName, k, true) == 0);

                var bName = TableNames.Values.FirstOrDefault(k => string.Compare(item.TabBName, k, true) == 0);

                if (string.IsNullOrEmpty(aName))
                {
                    TableNames.Add(tableAName, item.TabAName);
                }
                else
                {
                    var kvp = TableNames.First(k => string.Compare(k.Value, item.TabAName) == 0);
                    tableAName = kvp.Key;
                }

                if (string.IsNullOrEmpty(bName))
                {
                    TableNames.Add(tableBName, item.TabBName);
                }
                else
                {
                    var kvp = TableNames.First(k => string.Compare(k.Value, item.TabBName) == 0);
                    tableBName = kvp.Key;
                }

                if (item.SelectAFields != null && item.SelectAFields.Any())
                {
                    if (JoinTxt.Length > 0 && !JoinTxt.ToString().EndsWith(","))
                    {
                        JoinTxt.Append(",");
                    }

                    int acount = item.SelectAFields.Count();
                    for (int j = 0; j < acount; j++)
                    {
                        if (j != acount - 1)
                        {
                            JoinTxt.AppendFormat("{0}.{1},", tableAName, item.SelectAFields[j]);
                        }
                        else
                        {
                            JoinTxt.AppendFormat("{0}.{1}", tableAName, item.SelectAFields[j]);
                        }
                    }
                }

                if (item.SelectBFields != null && item.SelectBFields.Any())
                {
                    if (JoinTxt.Length > 0 && !JoinTxt.ToString().EndsWith(","))
                    {
                        JoinTxt.Append(",");
                    }

                    int bcount = item.SelectBFields.Count();
                    for (int j = 0; j < bcount; j++)
                    {
                        if (j != bcount - 1)
                        {
                            JoinTxt.AppendFormat("{0}.{1},", tableBName, item.SelectBFields[j]);
                        }
                        else
                        {
                            JoinTxt.AppendFormat("{0}.{1}", tableBName, item.SelectBFields[j]);
                        }
                    }
                }

                if (i == 0)
                {
                    sbJoin.AppendFormat(" {0} {1} {2} {3} {4} on {1}.{5}={4}.{6} ", item.TabAName, tableAName, item.ConvertJoinType(), item.TabBName, tableBName, item.OnFieldAName, item.OnFieldBName);
                }
                else
                {
                    var tabKVP = TableNames.Where(k => string.Compare(k.Value, item.TabAName, true) == 0 || string.Compare(k.Value, item.TabBName, true) == 0).FirstOrDefault();
                    if (string.IsNullOrEmpty(tabKVP.Key))
                    {
                        throw new Exception("联合查询表顺序异常!");
                    }

                    if (string.Compare(tabKVP.Value, item.TabAName, true) == 0)
                    {
                        sbJoin.AppendFormat("{0} {1} {2} on {2}.{3}={4}.{5} ", item.ConvertJoinType(), item.TabBName, tableBName, item.OnFieldBName, tabKVP.Key, item.OnFieldAName);
                    }
                    else
                    {
                        sbJoin.AppendFormat("{0} {1} {2} on {2}.{3}={4}.{5} ", item.ConvertJoinType(), item.TabAName, tableAName, item.OnFieldAName, tabKVP.Key, item.OnFieldBName);
                    }
                }
            }

            JoinTxt.AppendFormat(" from {0}", sbJoin.ToString());

            return JoinTxt.ToString();
        }
    }
}
