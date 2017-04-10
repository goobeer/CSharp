using System.Threading.Tasks;

namespace Goobeer.DB.Command
{
    /// <summary>
    /// 命令请求者
    /// </summary>
    public class CommandInvoker: ICommandInvoker
    {
        public IDataCommand Command { get; set; }

        public int RunCommand()
        {
            return Command.Execute();
        }
    }
}
