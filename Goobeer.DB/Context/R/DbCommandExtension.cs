using Goobeer.Security;
using System.Data.Common;
using System.Text;

namespace Goobeer.DB.Context.R
{
    public static class DbCommandExtension
    {
        public static string GenerateHash(this DbCommand cmd)
        {
            StringBuilder sb = new StringBuilder(cmd.CommandText);
            sb.AppendFormat("{0}_{1}", cmd.Connection.ConnectionString, cmd.CommandText);

            foreach (DbParameter item in cmd.Parameters)
            {
                sb.AppendFormat("{0}{1}", item.ParameterName, item.Value);
            }
            return HashEncryptHelper.HashEncrypt(HashType.MD5, sb.ToString());
        }
    }
}
