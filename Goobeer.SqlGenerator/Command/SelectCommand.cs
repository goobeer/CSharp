namespace Goobeer.DB.Command
{
    /// <summary>
    /// 为什么 部门作为 一个服务?
    /// </summary>
    public class SelectCommand : DbCommandBase, IDatabaseCommand
    {
        //select cmd 包含 检索字段、检索条件
        public void Execute()
        {
            DecorateCommand();
        }

        public override void DecorateCommand()
        {
            
        }
    }
}
