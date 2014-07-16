using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using Core.Model;
using Core.Utilities;
using DapperExtensions;

namespace Core.Repository
{
    /// <summary>
    /// Abstract Generic Repository.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : EntityBase, new()
    {
        /// <summary>
        /// Database connection.
        /// </summary>
        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(Configuration.AlgoTraderDbConStr);
            }
        }

        /// <summary>
        /// Get a single entity by its identifier.
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Single entity</returns>
        public virtual TEntity Get(Guid id)
        {
            TEntity entity;

            using (var cn = Connection)
            {
                cn.Open();
                entity = cn.Get<TEntity>(id);
            }

            return entity;
        }

        /// <summary>
        /// Get entities which satisfy the predicate.
        /// </summary>
        /// <param name="memberExpression">The property to predicate with</param>
        /// <param name="memberValue">The value that the predicated property should equal</param>
        /// <returns>Collection of satisfied entities</returns>
        public IEnumerable<TEntity> GetBy(Expression<Func<TEntity, object>> memberExpression, dynamic memberValue)
        {
            ExpressionHelpers<TEntity>.CheckMemberExpression(memberExpression, memberValue);

            IEnumerable<TEntity> ticks;

            using (var cn = Connection)
            {
                cn.Open();
                IFieldPredicate predicate = Predicates.Field<TEntity>(memberExpression, Operator.Eq, memberValue);
                ticks = cn.GetList<TEntity>(predicate);
            }

            return ticks;
        }

        /// <summary>
        /// Get collection of all of a type of entity.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetAll()
        {
            IEnumerable<TEntity> ticks;

            using (var cn = Connection)
            {
                cn.Open();
                ticks = cn.GetList<TEntity>();
            }

            return ticks;
        }

        /// <summary>
        /// Insert a single entity.
        /// </summary>
        /// <param name="entity"></param>
        public void Insert(TEntity entity)
        {
            using (var cn = Connection)
            {
                cn.Open();
                cn.Insert(entity);
            }
        }

        /// <summary>
        /// Update a single entity.
        /// </summary>
        /// <param name="entity"></param>
        public void Update(TEntity entity)
        {
            using (var cn = Connection)
            {
                cn.Open();
                cn.Update(entity);
            }
        }

        /// <summary>
        /// Physically delete a single entity.
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(TEntity entity)
        {
            using (var cn = Connection)
            {
                cn.Open();
                cn.Delete(entity);
            }
        }
    }
}
