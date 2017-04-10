namespace Goobeer.DB.Command
{
    /// <summary>
    /// 命令实现者
    /// </summary>
    public interface ICommandReceiver
    {
        /// <summary>
        /// 命令实现者 执行命令
        /// </summary>
        /// <returns></returns>
        int ExecuteCommand();
    }
}
