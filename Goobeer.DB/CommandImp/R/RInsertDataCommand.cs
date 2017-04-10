using Goobeer.DB.CommandImp.BaseCommand;
using System.Data.Common;
using System.Linq;

namespace Goobeer.DB.CommandImp.R
{
    /// <summary>
    /// 关系型 数据 insert命令
    /// </summary>
    public class RInsertDataCommand : RBaseDataCommand
    {
        public RInsertDataCommand() { }

        public RInsertDataCommand(string connectionString, string providerName)
        {
            DBHelper = new RDBRepertory(connectionString, providerName);
        }

        public override DbCommand RenderCommandData()
        {
            var cmd = DBHelper.CreateDbCmd();
            var keys = Data.OpFields.Keys.ToArray();
            var vals = Data.OpFields.Values;
            foreach (var key in keys)
            {
                var p = cmd.CreateParameter();
                p.ParameterName = string.Format("{0}{1}", ParamPrefix, key);
                p.Value = Data.OpFields[key];
                cmd.Parameters.Add(p);
            }

            string sql = string.Format("insert into {0} ({1}) values ({2})", Data.TableName, string.Join(",", keys), string.Format("{0}{1}", ParamPrefix,string.Join("," + ParamPrefix, keys)));
            cmd.CommandText = sql;
            return cmd;
        }

        //insert 可能存在一次 插入 多个值
    }
}
