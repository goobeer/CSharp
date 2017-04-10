namespace Goobeer.DB.SqlCauseHelper.ISql
{
    public interface IGroup
    {
        IGroup BuildHaving<E>(params string[] groupFields) where E : class, new();
    }
}
