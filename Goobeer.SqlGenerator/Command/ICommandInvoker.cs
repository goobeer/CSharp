namespace Goobeer.DB.Command
{
    /// <summary>
    /// 命令请求者
    /// </summary>
    public interface ICommandInvoker
    {
        void RunCommand();
    }
}
