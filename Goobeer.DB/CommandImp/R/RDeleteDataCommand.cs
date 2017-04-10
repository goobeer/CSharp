using Goobeer.DB.CommandImp.BaseCommand;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Goobeer.DB.CommandImp.R
{
    public class RDeleteDataCommand : RBaseDataCommand
    {
        public RDeleteDataCommand() { }

        public RDeleteDataCommand(string connectionString, string providerName)
        {
            DBHelper = new RDBRepertory(connectionString, providerName);
        }

        public override DbCommand RenderCommandData()
        {
            var cmd = DBHelper.CreateDbCmd();
            var keys = Data.OpFields.Keys.ToArray();
            var vals = Data.OpFields.Values;
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("delete from {0} ", Data.TableName);
            for (int i = 0; i < keys.Length; i++)
            {
                var p = cmd.CreateParameter();
                p.ParameterName = string.Format("{0}{1}", ParamPrefix, keys[i]);
                p.Value = Data.OpFields[keys[i]];
                cmd.Parameters.Add(p);
                if (i < keys.Length - 1)
                {
                    sql.AppendFormat("{0}={1}{0},", keys[i], ParamPrefix);
                }
                else
                {
                    sql.AppendFormat("{0}={1}{0} ", keys[i], ParamPrefix);
                }
            }

            //解析条件
            string condition = RCondition.RenderConditon(cmd, ParamPrefix, Data);
            if (!string.IsNullOrEmpty(condition))
            {
                sql.AppendFormat("where {0}", condition);
            }

            cmd.CommandText = sql.ToString();
            return cmd;
        }
    }
}
