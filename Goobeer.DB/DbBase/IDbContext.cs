using Goobeer.Cache;

namespace Goobeer.DB.DbBase
{
    public interface IDbContext
    {
        ICacheable Cache { get; set; }
    }
}
