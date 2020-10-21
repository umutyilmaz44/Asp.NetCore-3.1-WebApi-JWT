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
    public class RefreshTokenService : BaseService, IBaseService<RefreshToken, RefreshTokenBo>
    {
        public RefreshTokenService(ServiceContext serviceContext, IServiceManager serviceManager) : base(serviceContext, serviceManager)
        {
        }

        public async Task<ServiceResult<RefreshTokenBo>> CreateAsync(RefreshTokenBo bo)
        {
            if (bo == null)
                return new ServiceResult<RefreshTokenBo>(null, false, "RefreshToken info is empty!");

            try
            {
                RefreshToken entity;
                if (bo.Id > 0)
                {
                    entity = await repositoryManager.RefreshTokenRepository.GetByIDAsync(bo.Id);
                    if (entity != null)
                        return new ServiceResult<RefreshTokenBo>(null, false, "RefreshToken already exist!");
                    else
                        bo.Id = 0;
                }

                entity = mapper.Map<RefreshToken>(bo);

                await repositoryManager.RefreshTokenRepository.InsertAsync(entity);
                await repositoryManager.CommitAsync();
                bo = mapper.Map<RefreshTokenBo>(entity);

                return new ServiceResult<RefreshTokenBo>(bo, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<RefreshTokenBo>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult> DeleteAsync(long id)
        {
            if (id == null || id <= 0)
                return new ServiceResult(false, "Id is empty!");

            try
            {
                await repositoryManager.RefreshTokenRepository.DeleteAsync(id);
                await repositoryManager.CommitAsync();
                return new ServiceResult(true);
            }
            catch (Exception ex)
            {
                return new ServiceResult(false, ex.Message);
            }
        }

        public async Task<ServiceResult<IEnumerable<RefreshTokenBo>>> GetAsync(Expression<Func<RefreshToken, bool>> filter = null, Func<IQueryable<RefreshToken>, IOrderedQueryable<RefreshToken>> orderBy = null, params Expression<Func<RefreshToken, object>>[] includeProperties)
        {
            try
            {
                IEnumerable<RefreshTokenBo> data = (await repositoryManager.RefreshTokenRepository.GetAsync(filter, orderBy, includeProperties)).Select(x => mapper.Map<RefreshTokenBo>(x));
                return new ServiceResult<IEnumerable<RefreshTokenBo>>(data, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<IEnumerable<RefreshTokenBo>>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult<IEnumerable<RefreshTokenBo>>> FindAsync(FilterCriteria filterCriteria, Expression<Func<RefreshToken, bool>> predicateQuery)
        {
            if (filterCriteria == null)
                filterCriteria = new FilterCriteria();

            string includeProperties = filterCriteria.IncludeProperties ?? "";
            filterCriteria.PagingFilter = filterCriteria.PagingFilter ?? (new PagingFilter());
            if (filterCriteria.PagingFilter.pageNumber <= 0)
                filterCriteria.PagingFilter.pageNumber = 1;
            try
            {
                IEnumerable<RefreshToken> entities = await repositoryManager.RefreshTokenRepository.FindAsync(filterCriteria, predicateQuery);
                if (entities != null)
                {
                    IEnumerable<RefreshTokenBo> dtos = entities.Select(t => mapper.Map<RefreshTokenBo>(t));

                    return new ServiceResult<IEnumerable<RefreshTokenBo>>(dtos, true, pagingFilter: filterCriteria.PagingFilter);
                }
                else
                    return new ServiceResult<IEnumerable<RefreshTokenBo>>(null, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<IEnumerable<RefreshTokenBo>>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult<RefreshTokenBo>> GetByIdAsync(long id)
        {
            if (id == null || id <= 0)
                return new ServiceResult<RefreshTokenBo>(null, false, "Id is empty!");

            try
            {
                var entity = await repositoryManager.RefreshTokenRepository.GetByIDAsync(id);
                if (entity != null)
                {
                    RefreshTokenBo bo = mapper.Map<RefreshTokenBo>(entity);
                    return new ServiceResult<RefreshTokenBo>(bo, true);
                }
                else
                    return new ServiceResult<RefreshTokenBo>(null, false, "RefreshToken not found!");
            }
            catch (Exception ex)
            {
                return new ServiceResult<RefreshTokenBo>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult> UpdateAsync(long id, RefreshTokenBo bo)
        {
            if (bo == null)
                return new ServiceResult(false, "RefreshTokenDto is empty!");

            try
            {
                RefreshToken entity;

                if (bo.Id > 0 && id == bo.Id)
                {
                    entity = await repositoryManager.RefreshTokenRepository.GetByIDAsync(id);
                    entity.ExpiryDate = bo.ExpiryDate;
                    entity.Token = bo.Token;
                    entity.UserId = bo.UserId;

                    //entity = mapper.Map<RefreshToken>(bo);
                    await repositoryManager.RefreshTokenRepository.UpdateAsync(id, entity);
                    await repositoryManager.CommitAsync();
                    if (entity == null)
                        return new ServiceResult(false, "RefreshToken not found!");
                }
                else
                {
                    return new ServiceResult(false, "RefreshToken Id is missing!");
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
