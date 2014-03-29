using TradingCore.Model.Interface;

namespace TradingCore.Repository.Interface
{
    public interface IRepository<TEntity> : IReadOnlyRespository<TEntity> where TEntity : EntityBase, new()
    {
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
