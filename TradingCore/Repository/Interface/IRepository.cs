using TradingCore.Model.Interface;

namespace TradingCore.Repository.Interface
{
    public interface IRepository<TEntity> : IReadOnlyRespository<TEntity> where TEntity : class, IEntity
    {
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
