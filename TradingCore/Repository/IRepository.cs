using TradingCore.Model;

namespace TradingCore.Repository
{
    public interface IRepository<TEntity> : IReadOnlyRespository<TEntity> where TEntity : class, IEntity
    {
        bool Insert(TEntity entity);
        bool Update(TEntity entity);
        bool Delete(TEntity entity);
    }
}
