namespace Goobeer.DB.Command
{
    public abstract class BaseDataCommand : CommandReceiver, IDataCommand
    {
        public BaseCmdData Data { get; set; }

        public ICommandReceiver Receiver { get; set; }

        public abstract int Execute();
    }
}
