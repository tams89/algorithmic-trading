using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TradingCore.Model;

namespace TradingCore.Repository
{
    public interface IReadOnlyRespository<TEntity> where TEntity : class, IEntity
    {
        TEntity Get();
        IEnumerable<TEntity> GetBy(Expression<Func<TEntity, bool>> member);
        IEnumerable<TEntity> GetAll();
    }
}
