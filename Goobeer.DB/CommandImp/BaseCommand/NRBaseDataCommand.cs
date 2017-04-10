using Goobeer.DB.Command;
using System;
using System.Threading.Tasks;

namespace Goobeer.DB.CommandImp.BaseCommand
{
    /// <summary>
    /// 非关系型 数据命令 基类
    /// </summary>
    public abstract class NRBaseDataCommand:BaseDataCommand
    {
        public override int Execute()
        {
            return ExecuteCommand();
        }

        public override int ExecuteCommand()
        {
            throw new NotImplementedException();
        }
    }
}
