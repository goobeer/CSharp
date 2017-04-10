using Goobeer.DB.Command;
using Goobeer.DB.DbBase;

namespace Goobeer.DB.Context.BaseContext
{
    /// <summary>
    /// 命令上下文(update,insert,delete)
    /// </summary>
    public abstract class CommandContext : ICommandContext
    {
        public BaseDataCommand CmdReceiver { get; set; }
        public CommandInvoker CmdInvoker { get; set; }

        public abstract int Command();
    }
}
