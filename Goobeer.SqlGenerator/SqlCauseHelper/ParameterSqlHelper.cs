using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Goobeer.DB.SqlCauseHelper
{
    /// <summary>
    /// 参数化sql转普通sql
    /// </summary>
    /// <typeparam name="T">parameter类型</typeparam>
    /// <typeparam name="C">相应的parameter集合</typeparam>
    public class ParameterSqlHelper<T, C>
        where T : DbParameter, new()
        where C : IDataParameterCollection, new()
    {
        /// <summary>
        /// 获得参数化sql的原生sql语句
        /// </summary>
        /// <param name="paraCmd">参数化的DbCommand</param>
        /// <param name="convertSql">进行参数化的sql转普通sql操作</param>
        /// <returns>还原后的sql语句</returns>
        public string GetParameterSqlSentence(IDbCommand paraCmd, Func<string, C, string> convertSql)
        {
            if (paraCmd.CommandType != CommandType.Text)
            {
                throw new Exception("参数只能为普通sql语句");
            }

            string paraSql = paraCmd.CommandText;
            C paraCol = new C();
            foreach (IDataParameter item in paraCmd.Parameters)
            {
                T t = new T() { ParameterName = item.ParameterName, Value = item.Value };
                paraCol.Add(t);
            }
            if (convertSql == null)
            {
                convertSql = ConvertParaSql;
                paraSql = convertSql(paraSql, paraCol);
            }
            else
            {
                paraSql = convertSql(paraSql, paraCol);
            }

            return paraSql;
        }

        /// <summary>
        /// 转换参数化sql语句为普通的sql语句(解耦操作)
        /// </summary>
        /// <param name="paraSql">源sql语句</param>
        /// <param name="paraCol">参数集合</param>
        /// <returns>经过转换的sql语句</returns>
        private string ConvertParaSql(string paraSql, C paraCol)
        {
            bool useEscape = paraSql.ToLower().IndexOf("like") != -1;
            OralceSqlParameterHelper osph = new OralceSqlParameterHelper();
            List<SqlParameterRule> paraRule = new List<SqlParameterRule>();
            //准备规则 ' 优先级最高，注意次序
            paraRule.Add(new SqlParameterRule(new string[] { "'" }.ToList(), null, "'"));
            paraRule.Add(new SqlParameterRule(new string[] { "&", "--" }.ToList(), null, "||"));
            paraRule.Add(new SqlParameterRule(new string[] { "_", "%" }.ToList(), @"\", "escape"));

            osph.ConvertParameter(paraCol, paraRule, useEscape);

            foreach (IDbDataParameter item in paraCol)
            {
                paraSql = paraSql.Replace(item.ParameterName, item.Value == null ? "" : string.Format("'{0}'", item.Value.ToString()));
            }

            var res = paraRule.Where(s => s.OperationCode == 1 && s.Operation != null && s.Operation.ToLower() == "escape").FirstOrDefault();
            if (res != null)
            {
                paraSql += string.Format(" escape '{0}'", res.ParaDst);
            }
            return paraSql;
        }
    }

    /// <summary>
    /// 参数化sql转义操作接口
    /// </summary>
    public interface ISqlParameter
    {
        /// <summary>
        /// 把参数化的sql语句转为普通的sql语句(有别于参数化的sql语句)
        /// </summary>
        /// <param name="paras">sql参数集合</param>
        /// <param name="paraRule">参数转义规则</param>
        /// <param name="useEscape">是否使用转义操作</param>
        void ConvertParameter(IDataParameterCollection paras, List<SqlParameterRule> paraRule, bool useEscape);
    }

    /// <summary>
    /// Oralce 参数化转义
    /// </summary>
    public class OralceSqlParameterHelper : ISqlParameter
    {
        public void ConvertParameter(IDataParameterCollection paras, List<SqlParameterRule> paraRule, bool useEscape)
        {
            for (int i = 0; i < paras.Count; i++)
            {
                IDataParameter para = (IDataParameter)paras[i];
                List<SqlParameterRule> res = ((from r in paraRule
                                               where para.Value != null && (
                                                   from ss in r.ParaSrc
                                                   where para.Value.ToString().IndexOf(ss) != -1
                                                   select ss
                                               ).Count() > 0
                                               select r).Cast<SqlParameterRule>().ToList());
                if (para.Value != null && res != null)
                {
                    for (int j = 0; j < res.Count; j++)
                    {
                        SqlParameterRule spr = res[j];
                        foreach (string item in spr.ParaSrc)
                        {
                            if (!string.IsNullOrEmpty(spr.Operation))
                            {
                                string temp = spr.Operation.ToLower();
                                if (temp == "escape" && useEscape)
                                {
                                    para.Value = para.Value.ToString().Replace(item, string.Format("{0}{1}", spr.ParaDst, item));
                                    spr.OperationCode = 1;
                                }
                                else if (temp == "||")
                                {
                                    para.Value = para.Value.ToString().Replace(item, string.Format("'||'{0}'||'", item));
                                }
                                else if (temp == "'")
                                {
                                    para.Value = para.Value.ToString().Replace(item, string.Format("'{0}", item));
                                }
                                else
                                {
                                    //未知的操作符
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// sql参数化替换规则
    /// </summary>
    public class SqlParameterRule
    {
        #region 定义变量
        /// <summary>
        /// 用来标识 是否使用 escape 进行转义(当为1时，需要使用escape转义)
        /// </summary>
        public int OperationCode;

        /// <summary>
        /// 存储数据库中的 特殊字符集
        /// </summary>
        public List<string> ParaSrc { get; set; }

        /// <summary>
        /// 转义字符
        /// </summary>
        public string ParaDst { get; set; }

        /// <summary>
        /// 具体转义操作符
        /// </summary>
        public string Operation { get; set; }
        #endregion

        public SqlParameterRule() { }

        public SqlParameterRule(List<string> paraSrc, string paraDst, string operation)
        {
            this.ParaSrc = paraSrc;
            this.ParaDst = paraDst;
            this.Operation = operation;
        }
    }
}
