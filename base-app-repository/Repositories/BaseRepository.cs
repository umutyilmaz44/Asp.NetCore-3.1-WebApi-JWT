using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Query;
using System.Reflection;
using base_app_repository.Entities;
using base_app_common;

namespace base_app_repository.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        internal BaseDbContext context;
        internal DbSet<TEntity> dbSet;

        public BaseRepository(BaseDbContext appDbContext)
        {
            this.context = appDbContext;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return await query.ToListAsync();
            }
        }
        public virtual async Task<TEntity> GetByIDAsync(object id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual async Task InsertAsync(TEntity entity)
        {
            await dbSet.AddAsync(entity);
        }
        public virtual async Task DeleteAsync(object id)
        {
            TEntity entityToDelete = await dbSet.FindAsync(id);
            DeleteAsync(entityToDelete);
        }
        public virtual async Task DeleteAsync(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }
        public virtual async Task UpdateAsync(object id, TEntity entityToUpdate)
        {
            TEntity entity = await GetByIDAsync(id);
            CopyEntity(entityToUpdate, entity);
            context.Entry(entity).State = EntityState.Modified;

            //if (context.Entry(entityToUpdate).State == EntityState.Detached)
            //{
            //    dbSet.Attach(entityToUpdate);
            //    context.Entry(entityToUpdate).State = EntityState.Modified;
            //}
            //else
            //{
            //    TEntity entity = await GetByIDAsync(id);
            //    CopyEntity(entityToUpdate, entity);
            //    context.Entry(entity).State = EntityState.Modified;
            //}
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(FilterCriteria filterCriteria, Expression<Func<TEntity, bool>> predicateQuery = null)
        {
            IQueryable<TEntity> query = dbSet;

            string includeProperties = (filterCriteria != null) ? filterCriteria.IncludeProperties ?? "" : "";
            PagingFilter paging = filterCriteria.PagingFilter ?? (new PagingFilter());

            if(predicateQuery != null)
            {
                query = query.Where(predicateQuery);
            }
            else if (!string.IsNullOrEmpty(filterCriteria.QueryFilter) && !string.IsNullOrWhiteSpace(filterCriteria.QueryFilter))
            {
                query = query.Where(filterCriteria.QueryFilter);
            }

            if (!string.IsNullOrEmpty(includeProperties) && !string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }
            if (paging.pageNumber <= 0)
                paging.pageNumber = 1;

            if (paging.pageSize <= 0)
                paging.pageSize = 20;

            int totalNumberOfRecords;
            try
            {
                totalNumberOfRecords = await query.CountAsync();
            }
            catch(Exception ex) { totalNumberOfRecords = 0; }

            var mod = totalNumberOfRecords % paging.pageSize;
            var totalPageCount = (totalNumberOfRecords / paging.pageSize) + (mod == 0 ? 0 : 1);

            paging.pageCount = totalPageCount;
            paging.totalCount = totalNumberOfRecords;

            if (!string.IsNullOrEmpty(filterCriteria.OrderFilter))
                query = query.OrderBy(filterCriteria.OrderFilter);

            //string queryText = query.ToSql();

            return await query.Skip(paging.skipAmount).Take(paging.pageSize).ToListAsync().ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(
            PagingFilter paging,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = dbSet;

            if (paging == null)
                paging = new PagingFilter();

            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (paging.pageNumber <= 0)
                paging.pageNumber = 1;

            if (paging.pageSize <= 0)
                paging.pageSize = 20;

            int totalNumberOfRecords;
            try
            {
                totalNumberOfRecords = await query.CountAsync();
            }
            catch (Exception ex) { totalNumberOfRecords = 0; }

            var mod = totalNumberOfRecords % paging.pageSize;
            var totalPageCount = (totalNumberOfRecords / paging.pageSize) + (mod == 0 ? 0 : 1);

            paging.pageCount = totalPageCount;
            paging.totalCount = totalNumberOfRecords;

            if (orderBy != null)
            {
                return await orderBy(query.Skip(paging.skipAmount).Take(paging.pageSize)).ToListAsync().ConfigureAwait(false);
            }
            else
            {
                return await query.Skip(paging.skipAmount).Take(paging.pageSize).ToListAsync().ConfigureAwait(false);
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetWithRawSqlAsync(
            PagingFilter paging,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            string rawQuery,
            object[] parameters,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = dbSet;

            if (!string.IsNullOrEmpty(rawQuery))
            {
                query = dbSet.FromSqlRaw(rawQuery, parameters);
            }
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (paging.pageNumber <= 0)
                paging.pageNumber = 1;

            if (paging.pageSize <= 0)
                paging.pageSize = 20;

            int totalNumberOfRecords;
            try
            {
                totalNumberOfRecords = await query.CountAsync();
            }
            catch (Exception ex) { totalNumberOfRecords = 0; }

            var mod = totalNumberOfRecords % paging.pageSize;
            var totalPageCount = (totalNumberOfRecords / paging.pageSize) + (mod == 0 ? 0 : 1);

            paging.pageCount = totalPageCount;
            paging.totalCount = totalNumberOfRecords;

            if (orderBy != null)
            {
                return await orderBy(query.Skip(paging.skipAmount).Take(paging.pageSize)).ToListAsync().ConfigureAwait(false);
            }
            else
            {
                return await query.Skip(paging.skipAmount).Take(paging.pageSize).ToListAsync().ConfigureAwait(false);
            }
        }

        public virtual void CopyEntity(TEntity src, TEntity target)
        {
            var fromProperties = src.GetType().GetProperties();
            var toProperties = target.GetType().GetProperties();

            foreach (var fromProperty in fromProperties)
            {
                foreach (var toProperty in toProperties)
                {
                    if (fromProperty.Name == toProperty.Name && fromProperty.PropertyType == toProperty.PropertyType)
                    {
                        toProperty.SetValue(target, fromProperty.GetValue(src));
                        break;
                    }
                }
            }
        }


    }

    public static class Utils
    {
        public static string ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            var enumerator = query.Provider.Execute<IEnumerable<TEntity>>(query.Expression).GetEnumerator();
            var relationalCommandCache = enumerator.Private("_relationalCommandCache");
            var selectExpression = relationalCommandCache.Private<SelectExpression>("_selectExpression");
            var factory = relationalCommandCache.Private<IQuerySqlGeneratorFactory>("_querySqlGeneratorFactory");

            var sqlGenerator = factory.Create();
            var command = sqlGenerator.GetCommand(selectExpression);

            string sql = command.CommandText;
            return sql;
        }

        private static object Private(this object obj, string privateField) => obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
        private static T Private<T>(this object obj, string privateField) => (T)obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
    }
}