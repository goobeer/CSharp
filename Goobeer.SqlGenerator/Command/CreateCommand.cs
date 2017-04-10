using System;
using System.Data.Common;

namespace Goobeer.DB.Command
{
    public class CreateCommand : IDatabaseCommand
    {
        public DbCommand Cmd
        {
            get;
        }

        public CreateCommand(DbCommand cmd)
        {
            Cmd = cmd;
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
