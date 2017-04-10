using System.Data.Common;

namespace Goobeer.DB.Command
{
    public abstract class DbCommandBase
    {
        public DbCommand Cmd { get; set; }

        public abstract void DecorateCommand();
    }
}
