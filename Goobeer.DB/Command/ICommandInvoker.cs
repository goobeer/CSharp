using System.Threading.Tasks;

namespace Goobeer.DB.Command
{
    /// <summary>
    /// 命令请求者
    /// </summary>
    public interface ICommandInvoker
    {
        /// <summary>
        /// 请求执行命令
        /// </summary>
        int RunCommand();
    }
}
