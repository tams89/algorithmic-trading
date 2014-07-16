using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Core.Model;

namespace Core.Repository
{
    public interface IReadOnlyRespository<TEntity> where TEntity : EntityBase, new()
    {
        TEntity Get(Guid id);
        IEnumerable<TEntity> GetBy(Expression<Func<TEntity, object>> memberExpression, dynamic memberValue);
        IEnumerable<TEntity> GetAll();
    }
}
