using System;
using System.Text.RegularExpressions;

namespace Goobeer.DB.Context.R
{
    public static class RQueryHelper
    {
        #region 分页
        //分页
        //sqlserver >=2005 中，row_number
        //聚合操作
        /// <summary>
        /// sqlserver2005 分页,使用row_number
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static string RenderPageSqlServer2005(string sql,string orderString, uint pageNum, uint pageSize)
        {
            if (string.IsNullOrEmpty(orderString))
            {
                throw new NotSupportedException("分页排序字段不能为空");
            }
            Regex regField = new Regex("select (.*?) from", RegexOptions.IgnoreCase);
            var match = regField.Match(sql);
            
            string fields = match.Groups[1].Value;

            Regex regOrder = new Regex(string.Format(" order by {0}", orderString));
            sql = sql.Replace(regOrder.Match(sql).Value, string.Empty);
            sql = sql.Insert(sql.IndexOf("select ", StringComparison.OrdinalIgnoreCase) + 7, string.Format("row_number() over(order by {0}) row_number,", orderString));

            sql = string.Format("select {3} from ({0}) t where row_number> {1} and row_number<= {2}", sql, (pageNum - 1) * pageSize, pageNum * pageSize, fields);
            return sql;
        }

        /// <summary>
        /// sqlserver 2012 分页,使用 [order by expression offset offset_number rows fetch next count rows] 分页
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static string RenderPageSqlServer2012(string sql, string orderString, uint pageNum, uint pageSize)
        {
            if (string.IsNullOrEmpty(orderString))
            {
                throw new NotSupportedException("分页排序字段不能为空");
            }
            sql = string.Format("{0} {1}", sql, string.Format("offset {0} rows fetch next {1} rows only", pageNum * pageSize, pageSize));
            return sql;
        }

        /// <summary>
        /// mysql 分页,使用 limit m,n
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static string RenderPageMysql(string sql, string orderString, uint pageNum, uint pageSize)
        {
            sql = string.Format("{0} limit {1},{2}", sql, pageNum * pageSize, pageSize);
            return sql;
        }

        /// <summary>
        /// oracle分页 ,使用 row_number
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static string RenderPageOracle(string sql,string orderString, uint pageNum, uint pageSize)
        {
            return RenderPageSqlServer2005(sql, orderString, pageNum, pageSize);
        }

        public static string RenderPageSqlite(string sql, string orderString, uint pageNum, uint pageSize)
        {
            sql = string.Format("{0} limit {1} offset {2}", sql, pageSize, pageNum * pageSize);
            return sql;
        } 
        #endregion
    }
}
