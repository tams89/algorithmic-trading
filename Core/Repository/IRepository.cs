using Core.Model;

namespace Core.Repository
{
    public interface IRepository<TEntity> : IReadOnlyRespository<TEntity> where TEntity : EntityBase, new()
    {
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
