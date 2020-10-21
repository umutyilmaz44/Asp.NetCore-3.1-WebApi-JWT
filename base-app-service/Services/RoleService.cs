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
    public class RoleService : BaseService, IBaseService<Role, RoleBo>
    {
        public RoleService(ServiceContext serviceContext, IServiceManager serviceManager) : base(serviceContext, serviceManager)
        {
        }

        public async Task<ServiceResult<RoleBo>> CreateAsync(RoleBo bo)
        {
            if (bo == null)
                return new ServiceResult<RoleBo>(null, false, "Role info is empty!");

            try
            {
                Role entity;
                if (bo.Id > 0)
                {
                    entity = await repositoryManager.RoleRepository.GetByIDAsync(bo.Id);
                    if (entity != null)
                        return new ServiceResult<RoleBo>(null, false, "Role already exist!");
                    else
                        bo.Id = 0;
                }

                entity = mapper.Map<Role>(bo);

                await repositoryManager.RoleRepository.InsertAsync(entity);
                await repositoryManager.CommitAsync();
                bo = mapper.Map<RoleBo>(entity);

                return new ServiceResult<RoleBo>(bo, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<RoleBo>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult> DeleteAsync(long id)
        {
            if (id == null || id <= 0)
                return new ServiceResult(false, "Id is empty!");

            try
            {
                await repositoryManager.RoleRepository.DeleteAsync(id);
                await repositoryManager.CommitAsync();
                return new ServiceResult(true);
            }
            catch (Exception ex)
            {
                return new ServiceResult(false, ex.Message);
            }
        }

        public async Task<ServiceResult<IEnumerable<RoleBo>>> GetAsync(Expression<Func<Role, bool>> filter = null, Func<IQueryable<Role>, IOrderedQueryable<Role>> orderBy = null, params Expression<Func<Role, object>>[] includeProperties)
        {
            try
            {
                IEnumerable<RoleBo> data = (await repositoryManager.RoleRepository.GetAsync(filter, orderBy, includeProperties)).Select(x => mapper.Map<RoleBo>(x));
                return new ServiceResult<IEnumerable<RoleBo>>(data, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<IEnumerable<RoleBo>>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult<IEnumerable<RoleBo>>> FindAsync(FilterCriteria filterCriteria, Expression<Func<Role, bool>> predicateQuery)
        {
            if (filterCriteria == null)
                filterCriteria = new FilterCriteria();

            string includeProperties = filterCriteria.IncludeProperties ?? "";
            filterCriteria.PagingFilter = filterCriteria.PagingFilter ?? (new PagingFilter());
            if (filterCriteria.PagingFilter.pageNumber <= 0)
                filterCriteria.PagingFilter.pageNumber = 1;
            try
            {
                IEnumerable<Role> entities = await repositoryManager.RoleRepository.FindAsync(filterCriteria, predicateQuery);
                if (entities != null)
                {
                    IEnumerable<RoleBo> dtos = entities.Select(t => mapper.Map<RoleBo>(t));

                    return new ServiceResult<IEnumerable<RoleBo>>(dtos, true, pagingFilter: filterCriteria.PagingFilter);
                }
                else
                    return new ServiceResult<IEnumerable<RoleBo>>(null, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<IEnumerable<RoleBo>>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult<RoleBo>> GetByIdAsync(long id)
        {
            if (id == null || id <= 0)
                return new ServiceResult<RoleBo>(null, false, "Id is empty!");

            try
            {
                var entity = await repositoryManager.RoleRepository.GetByIDAsync(id);
                if (entity != null)
                {
                    RoleBo bo = mapper.Map<RoleBo>(entity);
                    return new ServiceResult<RoleBo>(bo, true);
                }
                else
                    return new ServiceResult<RoleBo>(null, false, "Role not found!");
            }
            catch (Exception ex)
            {
                return new ServiceResult<RoleBo>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult> UpdateAsync(long id, RoleBo bo)
        {
            if (bo == null)
                return new ServiceResult(false, "RoleDto is empty!");

            try
            {
                Role entity;

                if (bo.Id > 0 && id == bo.Id)
                {
                    entity = await repositoryManager.RoleRepository.GetByIDAsync(id);
                    entity.Desc = bo.Desc;
                    entity.RoleName = bo.RoleName;                    

                    //entity = mapper.Map<Role>(bo);
                    await repositoryManager.RoleRepository.UpdateAsync(id, entity);
                    await repositoryManager.CommitAsync();
                    if (entity == null)
                        return new ServiceResult(false, "Role not found!");
                }
                else
                {
                    return new ServiceResult(false, "Role Id is missing!");
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
