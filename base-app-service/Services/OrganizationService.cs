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
    public class OrganizationService : BaseService, IBaseService<Organization, OrganizationBo>
    {
        public OrganizationService(ServiceContext serviceContext, IServiceManager serviceManager) : base(serviceContext, serviceManager)
        {
        }

        public async Task<ServiceResult<OrganizationBo>> CreateAsync(OrganizationBo bo)
        {
            if (bo == null)
                return new ServiceResult<OrganizationBo>(null, false, "Organization info is empty!");

            try
            {
                Organization entity;
                if (bo.Id > 0)
                {
                    entity = await repositoryManager.OrganizationRepository.GetByIDAsync(bo.Id);
                    if (entity != null)
                        return new ServiceResult<OrganizationBo>(null, false, "Organization already exist!");
                    else
                        bo.Id = 0;
                }

                entity = mapper.Map<Organization>(bo);

                await repositoryManager.OrganizationRepository.InsertAsync(entity);
                await repositoryManager.CommitAsync();
                bo = mapper.Map<OrganizationBo>(entity);

                return new ServiceResult<OrganizationBo>(bo, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<OrganizationBo>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult> DeleteAsync(long id)
        {
            if (id == null || id <= 0)
                return new ServiceResult(false, "Id is empty!");

            try
            {
                await repositoryManager.OrganizationRepository.DeleteAsync(id);
                repositoryManager.CommitAsync();

                return new ServiceResult(true);
            }
            catch (Exception ex)
            {
                return new ServiceResult(false, ex.Message);
            }
        }

        public async Task<ServiceResult<IEnumerable<OrganizationBo>>> GetAsync(Expression<Func<Organization, bool>> filter = null, Func<IQueryable<Organization>, IOrderedQueryable<Organization>> orderBy = null, params Expression<Func<Organization, object>>[] includeProperties)
        {
            try
            {
                IEnumerable<OrganizationBo> data = (await repositoryManager.OrganizationRepository.GetAsync(filter, orderBy, includeProperties)).Select(x => mapper.Map<OrganizationBo>(x));
                return new ServiceResult<IEnumerable<OrganizationBo>>(data, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<IEnumerable<OrganizationBo>>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult<IEnumerable<OrganizationBo>>> FindAsync(FilterCriteria filterCriteria, Expression<Func<Organization, bool>> predicateQuery)
        {
            if (filterCriteria == null)
                filterCriteria = new FilterCriteria();

            string includeProperties = filterCriteria.IncludeProperties ?? "";
            filterCriteria.PagingFilter = filterCriteria.PagingFilter ?? (new PagingFilter());
            if (filterCriteria.PagingFilter.pageNumber <= 0)
                filterCriteria.PagingFilter.pageNumber = 1;
            try
            {
                IEnumerable<Organization> entities = await repositoryManager.OrganizationRepository.FindAsync(filterCriteria);
                if (entities != null)
                {
                    IEnumerable<OrganizationBo> dtos = entities.Select(t => mapper.Map<OrganizationBo>(t));

                    return new ServiceResult<IEnumerable<OrganizationBo>>(dtos, true, pagingFilter: filterCriteria.PagingFilter);
                }
                else
                    return new ServiceResult<IEnumerable<OrganizationBo>>(null, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<IEnumerable<OrganizationBo>>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult<OrganizationBo>> GetByIdAsync(long id)
        {
            if (id == null || id <= 0)
                return new ServiceResult<OrganizationBo>(null, false, "Id is empty!");

            try
            {
                var entity = await repositoryManager.OrganizationRepository.GetByIDAsync(id);
                if (entity != null)
                {
                    OrganizationBo bo = mapper.Map<OrganizationBo>(entity);
                    return new ServiceResult<OrganizationBo>(bo, true);
                }
                else
                    return new ServiceResult<OrganizationBo>(null, false, "Organization not found!");
            }
            catch (Exception ex)
            {
                return new ServiceResult<OrganizationBo>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult> UpdateAsync(long id, OrganizationBo bo)
        {
            if (bo == null)
                return new ServiceResult(false, "Organization is empty!");

            try
            {
                Organization entity;

                if (bo.Id > 0 && id == bo.Id)
                {
                    entity = await repositoryManager.OrganizationRepository.GetByIDAsync(id);
                    entity.Description = bo.Description;
                    entity.ParentId = bo.ParentId;
                    entity.Title = bo.Title;

                    //entity = mapper.Map<Organization>(bo);
                    await repositoryManager.OrganizationRepository.UpdateAsync(id, entity);
                    await repositoryManager.CommitAsync();
                    //entity = await repositoryManager.OrganizationRepository.GetByIDAsync(bo.Id);
                    if (entity == null)
                        return new ServiceResult(false, "Organization not found!");
                }
                else
                {
                    return new ServiceResult(false, "Organization Id is missing!");
                }

                return new ServiceResult(true);
            }
            catch (Exception ex)
            {
                return new ServiceResult(false, ex.Message);
            }
        }

        public async Task<ServiceResult<List<OrganizationBo>>> GetOrganizationHierarchicaly(OrganizationBo organizationDto)
        {
            ServiceResult<List<OrganizationBo>> serviceResult = new ServiceResult<List<OrganizationBo>>(new List<OrganizationBo>(), true, "");
            if (!serviceResult.Data.Any(x => x.Id == organizationDto.Id))
                serviceResult.Data.Add(organizationDto);

            ServiceResult<IEnumerable<OrganizationBo>> childResultList = await GetAsync(filter: (x => x.ParentId == organizationDto.Id));
            if (childResultList.Success && childResultList.Data != null)
            {
                List<OrganizationBo> childList = childResultList.Data.ToList();
                for (int i = 0; i < childList.Count; i++)
                {
                    ServiceResult<List<OrganizationBo>> result = await GetOrganizationHierarchicaly(childList[i]);
                    if (result.Success && result.Data != null)
                        serviceResult.Data.AddRange(result.Data);
                }
            }

            return serviceResult;
        }

        public async Task<ServiceResult<IEnumerable<OrganizationBo>>> GetHierarchicalyByOrganizationIdAsync(long organizationId)
        {
            OrganizationBo bo = null;
            ServiceResult<OrganizationBo> result = await serviceManager.Organization_Service.GetByIdAsync(organizationId);
            if (!result.Success || result.Data == null)
            {
                return new ServiceResult<IEnumerable<OrganizationBo>>(null, false, "Organization Not Found!");
            }
            bo = result.Data;

            ServiceResult<List<OrganizationBo>> orgResults = await GetOrganizationHierarchicaly(bo);
            if (!orgResults.Success || orgResults.Data == null || orgResults.Data.Count == 0)
            {
                return new ServiceResult<IEnumerable<OrganizationBo>>(null, false, "Organizations Not Found!");
            }
            List<OrganizationBo> orgDtos = orgResults.Data;

            return new ServiceResult<IEnumerable<OrganizationBo>>(orgDtos, true, "");
        }
    }
}
