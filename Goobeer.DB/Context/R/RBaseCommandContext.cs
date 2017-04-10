using Goobeer.DB.Context.BaseContext;

namespace Goobeer.DB.Context.R
{
    /// <summary>
    /// 关系型 数据库 命令上下文
    /// </summary>
    public class RBaseCommandContext : CommandContext
    {
        public RBaseCommandContext()
        {

        }

        public override int Command()
        {
            CmdInvoker.Command = CmdReceiver;
            return CmdInvoker.RunCommand();
        }
    }
}
