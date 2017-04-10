using Goobeer.DB.SqlCauseHelper.Operator;

namespace Goobeer.DB
{
    public class SqlServerBuilder:SqlBuilder
    {
        SqlServerOperator SqlOperator = new SqlServerOperator();
        public override SqlOperatorsBase GetSqlOperators()
        {
            return SqlOperator;
        }
    }
}
