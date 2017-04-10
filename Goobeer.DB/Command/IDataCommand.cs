namespace Goobeer.DB.Command
{
    /// <summary>
    /// 数据命令
    /// </summary>
    public interface IDataCommand
    {
        /// <summary>
        /// 命令实现者
        /// </summary>
        ICommandReceiver Receiver { get; set; }

        #region 操作字段和条件
        BaseCmdData Data {get;set;} 
        #endregion

        int Execute();
    }
}
