namespace Goobeer.DB.Command
{
    /// <summary>
    /// 命令实现者
    /// </summary>
    public abstract class CommandReceiver : ICommandReceiver
    {
        public abstract int ExecuteCommand();
    }
}
