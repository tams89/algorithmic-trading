using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TradingCore.Model.Interface;

namespace TradingCore.Repository.Interface
{
    public interface IReadOnlyRespository<TEntity> where TEntity : class, IEntity
    {
        TEntity Get(Guid id);
        IEnumerable<TEntity> GetBy(Expression<Func<TEntity, object>> memberExpression, dynamic memberValue);
        IEnumerable<TEntity> GetAll();
    }
}
