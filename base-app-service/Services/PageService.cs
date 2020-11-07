using base_app_common;
using base_app_repository.Entities;
using base_app_service.Bo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace base_app_service.Services
{
    public class PageService : BaseService, IBaseService<Page, PageBo>
    {
        public PageService(ServiceContext serviceContext, IServiceManager serviceManager) : base(serviceContext, serviceManager)
        {
        }

        public async Task<ServiceResult<PageBo>> CreateAsync(PageBo bo)
        {
            if (bo == null)
                return new ServiceResult<PageBo>(null, false, "Page info is empty!");

            try
            {
                Page entity;
                if (bo.Id > 0)
                {
                    entity = await repositoryManager.PageRepository.GetByIDAsync(bo.Id);
                    if (entity != null)
                        return new ServiceResult<PageBo>(null, false, "Page already exist!");
                    else
                        bo.Id = 0;
                }

                entity = mapper.Map<Page>(bo);

                await repositoryManager.PageRepository.InsertAsync(entity);
                await repositoryManager.CommitAsync();
                bo = mapper.Map<PageBo>(entity);

                return new ServiceResult<PageBo>(bo, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<PageBo>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult> DeleteAsync(long id)
        {
            if (id <= 0)
                return new ServiceResult(false, "Id is empty!");

            try
            {
                await repositoryManager.PageRepository.DeleteAsync(id);
                await repositoryManager.CommitAsync();
                return new ServiceResult(true);
            }
            catch (Exception ex)
            {
                return new ServiceResult(false, ex.Message);
            }
        }

        public async Task<ServiceResult<IEnumerable<PageBo>>> GetAsync(Expression<Func<Page, bool>> filter = null, Func<IQueryable<Page>, IOrderedQueryable<Page>> orderBy = null, params Expression<Func<Page, object>>[] includeProperties)
        {
            try
            {
                IEnumerable<PageBo> data = (await repositoryManager.PageRepository.GetAsync(filter, orderBy, includeProperties)).Select(x => mapper.Map<PageBo>(x));
                return new ServiceResult<IEnumerable<PageBo>>(data, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<IEnumerable<PageBo>>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult<IEnumerable<PageBo>>> FindAsync(FilterCriteria filterCriteria, Expression<Func<Page, bool>> predicateQuery)
        {
            if (filterCriteria == null)
                filterCriteria = new FilterCriteria();

            string includeProperties = filterCriteria.IncludeProperties ?? "";
            filterCriteria.PagingFilter = filterCriteria.PagingFilter ?? (new PagingFilter());
            if (filterCriteria.PagingFilter.pageNumber <= 0)
                filterCriteria.PagingFilter.pageNumber = 1;
            try
            {
                IEnumerable<Page> entities = await repositoryManager.PageRepository.FindAsync(filterCriteria, predicateQuery);
                if (entities != null)
                {
                    IEnumerable<PageBo> dtos = entities.Select(t => mapper.Map<PageBo>(t));

                    return new ServiceResult<IEnumerable<PageBo>>(dtos, true, pagingFilter: filterCriteria.PagingFilter);
                }
                else
                    return new ServiceResult<IEnumerable<PageBo>>(null, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<IEnumerable<PageBo>>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult<PageBo>> GetByIdAsync(long id)
        {
            if (id <= 0)
                return new ServiceResult<PageBo>(null, false, "Id is empty!");

            try
            {
                var entity = await repositoryManager.PageRepository.GetByIDAsync(id);
                if (entity != null)
                {
                    PageBo bo = mapper.Map<PageBo>(entity);
                    return new ServiceResult<PageBo>(bo, true);
                }
                else
                    return new ServiceResult<PageBo>(null, false, "Page not found!");
            }
            catch (Exception ex)
            {
                return new ServiceResult<PageBo>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult> UpdateAsync(long id, PageBo bo)
        {
            if (bo == null)
                return new ServiceResult(false, "PageDto is empty!");

            try
            {
                Page entity;

                if (bo.Id > 0 && id == bo.Id)
                {
                    entity = await repositoryManager.PageRepository.GetByIDAsync(id);                    
                    entity.NaviagteUrl = bo.NaviagteUrl;
                    entity.PageName = bo.PageName;

                    //entity = mapper.Map<Page>(bo);
                    await repositoryManager.PageRepository.UpdateAsync(id, entity);
                    await repositoryManager.CommitAsync();
                    if (entity == null)
                        return new ServiceResult(false, "Page not found!");
                }
                else
                {
                    return new ServiceResult(false, "Page Id is missing!");
                }

                return new ServiceResult(true);
            }
            catch (Exception ex)
            {
                return new ServiceResult(false, ex.Message);
            }
        }
    }
}
