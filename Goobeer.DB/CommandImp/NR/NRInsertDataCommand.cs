using Goobeer.DB.CommandImp.BaseCommand;
using System;
using System.Threading.Tasks;

namespace Goobeer.DB.CommandImp
{
    /// <summary>
    /// insert data 命令
    /// </summary>
    public class NRInsertDataCommand : NRBaseDataCommand
    {
        public override int Execute()
        {
            return ExecuteCommand();
        }

        public override int ExecuteCommand()
        {
            return 0;
        }

        //数据库适配只能发生在 该层
    }
}
