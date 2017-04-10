using Goobeer.DB.Command;

namespace Goobeer.DB.Result
{
    /// <summary>
    /// 命令结果
    /// </summary>
    public interface ICommandResult
    {
        /// <summary>
        /// 命令结果展现
        /// </summary>
        /// <returns></returns>
        string RenderCommandResult(BaseCmdData data);
    }
}
