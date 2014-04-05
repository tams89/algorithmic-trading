using Core.Model.Interface;

namespace Core.Repository.Interface
{
    public interface IRepository<TEntity> : IReadOnlyRespository<TEntity> where TEntity : EntityBase, new()
    {
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
