using base_app_common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace base_app_service.Services
{
    public interface IBaseService<TEntity, TBo> where TBo : class
    {
        Task<ServiceResult<IEnumerable<TBo>>> GetAsync(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties);

        Task<ServiceResult<TBo>> GetByIdAsync(long id);
        Task<ServiceResult<TBo>> CreateAsync(TBo dto);
        Task<ServiceResult> UpdateAsync(long id, TBo dto);
        Task<ServiceResult> DeleteAsync(long id);

        //Task<ServiceResult<IEnumerable<TBo>>> FindAsync(FilterCriteria filterCriteria);
        Task<ServiceResult<IEnumerable<TBo>>> FindAsync(FilterCriteria filterCriteria, Expression<Func<TEntity, bool>> predicateQuery = null);
    }
}
