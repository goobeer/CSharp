namespace Goobeer.DB.Command
{
    public class CommandInvoker: ICommandInvoker
    {
        public IDatabaseCommand Command { get; set; }

        public void RunCommand()
        {
            Command.Execute();
        }
    }
}
