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
    public class UserTokenService : BaseService, IBaseService<UserToken, UserTokenBo>
    {
        public UserTokenService(ServiceContext serviceContext, IServiceManager serviceManager) : base(serviceContext, serviceManager)
        {
        }

        public async Task<ServiceResult<UserTokenBo>> CreateAsync(UserTokenBo bo)
        {
            if (bo == null)
                return new ServiceResult<UserTokenBo>(null, false, "UserToken info is empty!");

            try
            {
                UserToken entity;
                if (bo.Id > 0)
                {
                    entity = await repositoryManager.UserTokenRepository.GetByIDAsync(bo.Id);
                    if (entity != null)
                        return new ServiceResult<UserTokenBo>(null, false, "UserToken already exist!");
                    else
                        bo.Id = 0;
                }

                entity = mapper.Map<UserToken>(bo);

                await repositoryManager.UserTokenRepository.InsertAsync(entity);
                await repositoryManager.CommitAsync();
                bo = mapper.Map<UserTokenBo>(entity);

                return new ServiceResult<UserTokenBo>(bo, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<UserTokenBo>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult> DeleteAsync(long id)
        {
            if (id <= 0)
                return new ServiceResult(false, "Id is empty!");

            try
            {
                await repositoryManager.UserTokenRepository.DeleteAsync(id);
                await repositoryManager.CommitAsync();
                return new ServiceResult(true);
            }
            catch (Exception ex)
            {
                return new ServiceResult(false, ex.Message);
            }
        }

        public async Task<ServiceResult<IEnumerable<UserTokenBo>>> GetAsync(Expression<Func<UserToken, bool>> filter = null, Func<IQueryable<UserToken>, IOrderedQueryable<UserToken>> orderBy = null, params Expression<Func<UserToken, object>>[] includeProperties)
        {
            try
            {
                IEnumerable<UserTokenBo> data = (await repositoryManager.UserTokenRepository.GetAsync(filter, orderBy, includeProperties)).Select(x => mapper.Map<UserTokenBo>(x));
                return new ServiceResult<IEnumerable<UserTokenBo>>(data, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<IEnumerable<UserTokenBo>>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult<IEnumerable<UserTokenBo>>> FindAsync(FilterCriteria filterCriteria, Expression<Func<UserToken, bool>> predicateQuery)
        {
            if (filterCriteria == null)
                filterCriteria = new FilterCriteria();

            string includeProperties = filterCriteria.IncludeProperties ?? "";
            filterCriteria.PagingFilter = filterCriteria.PagingFilter ?? (new PagingFilter());
            if (filterCriteria.PagingFilter.pageNumber <= 0)
                filterCriteria.PagingFilter.pageNumber = 1;
            try
            {
                IEnumerable<UserToken> entities = await repositoryManager.UserTokenRepository.FindAsync(filterCriteria, predicateQuery);
                if (entities != null)
                {
                    IEnumerable<UserTokenBo> dtos = entities.Select(t => mapper.Map<UserTokenBo>(t));

                    return new ServiceResult<IEnumerable<UserTokenBo>>(dtos, true, pagingFilter: filterCriteria.PagingFilter);
                }
                else
                    return new ServiceResult<IEnumerable<UserTokenBo>>(null, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<IEnumerable<UserTokenBo>>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult<UserTokenBo>> GetByIdAsync(long id)
        {
            if (id <= 0)
                return new ServiceResult<UserTokenBo>(null, false, "Id is empty!");

            try
            {
                var entity = await repositoryManager.UserTokenRepository.GetByIDAsync(id);
                if (entity != null)
                {
                    UserTokenBo bo = mapper.Map<UserTokenBo>(entity);
                    return new ServiceResult<UserTokenBo>(bo, true);
                }
                else
                    return new ServiceResult<UserTokenBo>(null, false, "UserToken not found!");
            }
            catch (Exception ex)
            {
                return new ServiceResult<UserTokenBo>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult> UpdateAsync(long id, UserTokenBo bo)
        {
            if (bo == null)
                return new ServiceResult(false, "UserTokenDto is empty!");

            try
            {
                UserToken entity;

                if (bo.Id > 0 && id == bo.Id)
                {
                    entity = await repositoryManager.UserTokenRepository.GetByIDAsync(id);
                    entity.ExpiryDate = bo.ExpiryDate;
                    entity.AccessToken = bo.AccessToken;
                    entity.RefreshToken = bo.RefreshToken;
                    entity.LoginTime = bo.LoginTime;
                    entity.LogoutTime = bo.LogoutTime;
                    entity.UserId = bo.UserId;
                    entity.IsLogout = bo.IsLogout;

                    //entity = mapper.Map<UserToken>(bo);
                    await repositoryManager.UserTokenRepository.UpdateAsync(id, entity);
                    await repositoryManager.CommitAsync();
                    if (entity == null)
                        return new ServiceResult(false, "UserToken not found!");
                }
                else
                {
                    return new ServiceResult(false, "UserToken Id is missing!");
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
