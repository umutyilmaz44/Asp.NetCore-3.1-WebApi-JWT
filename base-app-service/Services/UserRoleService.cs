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
    public class UserRoleService : BaseService, IBaseService<UserRole, UserRoleBo>
    {
        public UserRoleService(ServiceContext serviceContext, IServiceManager serviceManager) : base(serviceContext, serviceManager)
        {
        }

        public async Task<ServiceResult<UserRoleBo>> CreateAsync(UserRoleBo bo)
        {
            if (bo == null)
                return new ServiceResult<UserRoleBo>(null, false, "UserRole info is empty!");

            try
            {
                UserRole entity;
                if (bo.Id > 0)
                {
                    entity = await repositoryManager.UserRoleRepository.GetByIDAsync(bo.Id);
                    if (entity != null)
                        return new ServiceResult<UserRoleBo>(null, false, "UserRole already exist!");
                    else
                        bo.Id = 0;
                }

                entity = mapper.Map<UserRole>(bo);

                await repositoryManager.UserRoleRepository.InsertAsync(entity);
                await repositoryManager.CommitAsync();
                bo = mapper.Map<UserRoleBo>(entity);

                return new ServiceResult<UserRoleBo>(bo, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<UserRoleBo>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult> DeleteAsync(long id)
        {
            if (id <= 0)
                return new ServiceResult(false, "Id is empty!");

            try
            {
                await repositoryManager.UserRoleRepository.DeleteAsync(id);
                await repositoryManager.CommitAsync();
                return new ServiceResult(true);
            }
            catch (Exception ex)
            {
                return new ServiceResult(false, ex.Message);
            }
        }

        public async Task<ServiceResult> DeleteByUserIdAsync(long userid)
        {
            if (userid <= 0)
                return new ServiceResult(false, "userid is empty!");

            try
            {
                IEnumerable<UserRole> userRoles = await repositoryManager.UserRoleRepository.GetAsync(filter: (x => x.UserId == userid));
                if (userRoles != null && userRoles.Count() > 0) 
                {
                    foreach (var userRole in userRoles)
                    {
                        await repositoryManager.UserRoleRepository.DeleteAsync(userRole.Id);
                    }
                    await repositoryManager.CommitAsync();
                }                

                return new ServiceResult(true);
            }
            catch (Exception ex)
            {
                return new ServiceResult(false, ex.Message);
            }
        }

        public async Task<ServiceResult<IEnumerable<UserRoleBo>>> GetAsync(Expression<Func<UserRole, bool>> filter = null, Func<IQueryable<UserRole>, IOrderedQueryable<UserRole>> orderBy = null, params Expression<Func<UserRole, object>>[] includeProperties)
        {
            try
            {
                IEnumerable<UserRoleBo> data = (await repositoryManager.UserRoleRepository.GetAsync(filter, orderBy, includeProperties)).Select(x => mapper.Map<UserRoleBo>(x));
                return new ServiceResult<IEnumerable<UserRoleBo>>(data, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<IEnumerable<UserRoleBo>>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult<IEnumerable<UserRoleBo>>> FindAsync(FilterCriteria filterCriteria, Expression<Func<UserRole, bool>> predicateQuery = null)
        {
            if (filterCriteria == null)
                filterCriteria = new FilterCriteria();

            string includeProperties = filterCriteria.IncludeProperties ?? "";
            filterCriteria.PagingFilter = filterCriteria.PagingFilter ?? (new PagingFilter());
            if (filterCriteria.PagingFilter.pageNumber <= 0)
                filterCriteria.PagingFilter.pageNumber = 1;
            try
            {
                IEnumerable<UserRole> entities = await repositoryManager.UserRoleRepository.FindAsync(filterCriteria, predicateQuery);
                if (entities != null)
                {
                    IEnumerable<UserRoleBo> dtos = entities.Select(t => mapper.Map<UserRoleBo>(t));

                    return new ServiceResult<IEnumerable<UserRoleBo>>(dtos, true, pagingFilter: filterCriteria.PagingFilter);
                }
                else
                    return new ServiceResult<IEnumerable<UserRoleBo>>(null, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<IEnumerable<UserRoleBo>>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult<UserRoleBo>> GetByIdAsync(long id)
        {
            if (id <= 0)
                return new ServiceResult<UserRoleBo>(null, false, "Id is empty!");

            try
            {
                var entity = await repositoryManager.UserRoleRepository.GetByIDAsync(id);
                if (entity != null)
                {
                    UserRoleBo bo = mapper.Map<UserRoleBo>(entity);
                    return new ServiceResult<UserRoleBo>(bo, true);
                }
                else
                    return new ServiceResult<UserRoleBo>(null, false, "UserRole not found!");
            }
            catch (Exception ex)
            {
                return new ServiceResult<UserRoleBo>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult> UpdateAsync(long id, UserRoleBo bo)
        {
            if (bo == null)
                return new ServiceResult(false, "UserRoleDto is empty!");

            try
            {
                UserRole entity;

                if (bo.Id > 0 && id == bo.Id)
                {
                    entity = await repositoryManager.UserRoleRepository.GetByIDAsync(id);
                    entity.RoleId = bo.RoleId;
                    entity.UserId = bo.UserId;

                    //entity = mapper.Map<UserRole>(bo);
                    await repositoryManager.UserRoleRepository.UpdateAsync(id, entity);
                    await repositoryManager.CommitAsync();
                    if (entity == null)
                        return new ServiceResult(false, "UserRole not found!");
                }
                else
                {
                    return new ServiceResult(false, "UserRole Id is missing!");
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
