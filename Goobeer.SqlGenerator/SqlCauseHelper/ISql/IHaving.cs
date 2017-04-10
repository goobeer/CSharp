namespace Goobeer.DB.SqlCauseHelper.ISql
{
    public interface IHaving
    {
        IHaving BuildHaving<E>(params string[] groupFields) where E : class, new();
    }
}
