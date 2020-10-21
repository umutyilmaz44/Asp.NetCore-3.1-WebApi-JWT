using base_app_common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace base_app_repository.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAsync(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           params Expression<Func<TEntity, object>>[] includeProperties);

        Task<TEntity> GetByIDAsync(object id);

        Task InsertAsync(TEntity entity);

        Task DeleteAsync(object id);

        Task DeleteAsync(TEntity entityToDelete);

        Task UpdateAsync(object id, TEntity entityToUpdate);

        Task<IEnumerable<TEntity>> FindAsync(FilterCriteria filterCriteria, Expression<Func<TEntity, bool>> predicateQuery = null);

        Task<IEnumerable<TEntity>> FindAsync(
                PagingFilter paging, 
                Expression<Func<TEntity, bool>> filter = null, 
                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                params Expression<Func<TEntity, object>>[] includeProperties);

        Task<IEnumerable<TEntity>> GetWithRawSqlAsync(
                PagingFilter paging, 
                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
                string rawQuery,
                object[] parameters,
                params Expression<Func<TEntity, object>>[] includeProperties);
    }
}
