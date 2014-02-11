using System.Collections.Generic;
using TradingCore.Model;

namespace TradingCore.Repository
{
    public interface IReadOnlyRespository<out TEntity> where TEntity : class, IEntity
    {
        TEntity Get();
        IEnumerable<TEntity> GetAll();
    }
}
