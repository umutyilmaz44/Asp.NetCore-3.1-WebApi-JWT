using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using base_app_common;
using base_app_service.Bo;
using base_app_repository.Entities;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace base_app_service.Services
{
    public class UserLoginService : BaseService, IBaseService<UserLogin, UserLoginBo>
    {
        protected readonly PasswordHasher<UserBo> passwordHasher;

        public UserLoginService(ServiceContext serviceContext, IServiceManager serviceManager) : base(serviceContext, serviceManager)
        {
            passwordHasher = new PasswordHasher<UserBo>();
        }

        public async Task<ServiceResult<UserLoginBo>> CreateAsync(UserLoginBo bo)
        {
            if (bo == null)
                return new ServiceResult<UserLoginBo>(null, false, "UserLogin info is empty!");

            try
            {
                UserLogin entity;
                if (bo.Id > 0)
                {
                    entity = await repositoryManager.UserLoginRepository.GetByIDAsync(bo.Id);
                    if (entity != null)
                        return new ServiceResult<UserLoginBo>(null, false, "UserLogin already exist!");
                    else
                        bo.Id = 0;
                }

                entity = mapper.Map<UserLogin>(bo);
                entity.LoginTime = DateTime.Now;       

                await repositoryManager.UserLoginRepository.InsertAsync(entity);
                await repositoryManager.CommitAsync();

                bo = mapper.Map<UserLoginBo>(entity);
                return new ServiceResult<UserLoginBo>(bo, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<UserLoginBo>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult> DeleteAsync(long id)
        {
            if (id == null || id <= 0)
                return new ServiceResult(false, "Id is empty!");

            try
            {
                UserLogin user = await repositoryManager.UserLoginRepository.GetByIDAsync(id);                
                if(user == null)
                    return new ServiceResult(false, "UserLogin not found!");

                await repositoryManager.UserLoginRepository.UpdateAsync(id, user);
                await repositoryManager.CommitAsync();

                return new ServiceResult(true);
            }
            catch (Exception ex)
            {
                return new ServiceResult(false, ex.Message);
            }
        }

        public async Task<ServiceResult<IEnumerable<UserLoginBo>>> GetAsync(Expression<Func<UserLogin, bool>> filter = null, Func<IQueryable<UserLogin>, IOrderedQueryable<UserLogin>> orderBy = null, params Expression<Func<UserLogin, object>>[] includeProperties)
        {
            try
            {
                IEnumerable<UserLoginBo> data = (await repositoryManager.UserLoginRepository.GetAsync(filter, orderBy, includeProperties)).Select(x => mapper.Map<UserLoginBo>(x));
                
                return new ServiceResult<IEnumerable<UserLoginBo>>(data, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<IEnumerable<UserLoginBo>>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult<IEnumerable<UserLoginBo>>> FindAsync(FilterCriteria filterCriteria, Expression<Func<UserLogin, bool>> predicateQuery=null)
        {
            if (filterCriteria == null)
                filterCriteria = new FilterCriteria();

            string includeProperties = filterCriteria.IncludeProperties ?? "";
            filterCriteria.PagingFilter = filterCriteria.PagingFilter ?? (new PagingFilter());
            if (filterCriteria.PagingFilter.pageNumber <= 0)
                filterCriteria.PagingFilter.pageNumber = 1;
            try
            {
                IEnumerable<UserLogin> entities = await repositoryManager.UserLoginRepository.FindAsync(filterCriteria, predicateQuery);
                if (entities != null)
                {
                    IEnumerable<UserLoginBo> dtos = entities.Select(t => mapper.Map<UserLoginBo>(t));
                    return new ServiceResult<IEnumerable<UserLoginBo>>(dtos, true, pagingFilter: filterCriteria.PagingFilter);
                }
                else
                    return new ServiceResult<IEnumerable<UserLoginBo>>(null, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<IEnumerable<UserLoginBo>>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult<UserLoginBo>> GetByIdAsync(long id)
        {
            if (id == null || id <= 0)
                return new ServiceResult<UserLoginBo>(null, false, "Id is empty!");

            try
            {
                var entity = await repositoryManager.UserRepository.GetByIDAsync(id);
                if (entity != null)
                {
                    entity.Password = "";
                    var refreshToken = await repositoryManager.RefreshTokenRepository.GetLastByUserIdAsync(id);
                    entity.RefreshToken = new List<RefreshToken>() { refreshToken };
                    entity.Password = "";
                    UserLoginBo bo = mapper.Map<UserLoginBo>(entity);
                    return new ServiceResult<UserLoginBo>(bo, true);
                }
                else
                    return new ServiceResult<UserLoginBo>(null, false, "UserLogin not found!");
            }
            catch (Exception ex)
            {
                return new ServiceResult<UserLoginBo>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult> UpdateAsync(long id, UserLoginBo bo)
        {
            if (bo == null)
                return new ServiceResult(false, "UserDto is empty!");

            try
            {
                UserLogin entity;

                if (bo.Id > 0 && id == bo.Id)
                {
                    entity = await repositoryManager.UserLoginRepository.GetByIDAsync(id);
                    entity.UserId = bo.UserId;
                    entity.LoginTime = bo.LoginTime;

                    await repositoryManager.UserLoginRepository.UpdateAsync(id, entity);
                    await repositoryManager.CommitAsync();
                    if (entity == null)
                        return new ServiceResult(false, "User not found!");
                }
                else
                {
                    return new ServiceResult(false, "User Id is missing!");
                }

                return new ServiceResult(true);
            }
            catch (Exception ex)
            {
                return new ServiceResult(false, ex.Message);
            }
        }

        public async Task<ServiceResult<IEnumerable<UserLoginBo>>> GetListAsync(long organizationId, FilterCriteria filterCriteria = null)
        {
            IEnumerable<UserLoginBo> listBo = null;

            ServiceResult<IEnumerable<UserLoginBo>> resultList = null;
            OrganizationBo dto = null;
            List<OrganizationBo> orgDtos;
            CultureInfo enCulture = new CultureInfo("en-US");

            ServiceResult<OrganizationBo> result = await serviceManager.Organization_Service.GetByIdAsync(organizationId);
            if (!result.Success || result.Data == null)
            {
                return new ServiceResult<IEnumerable<UserLoginBo>>(null, false, "Organization Not Found!");
            }
            dto = result.Data;

            ServiceResult<List<OrganizationBo>> orgResults = await serviceManager.Organization_Service.GetOrganizationHierarchicaly(dto);
            if (!orgResults.Success || orgResults.Data == null || orgResults.Data.Count == 0)
            {
                return new ServiceResult<IEnumerable<UserLoginBo>>(null, false, "Organizations Not Found!");
            }
            orgDtos = orgResults.Data;
            List<long> orgIds = orgDtos.Select(x => x.Id).ToList();
            if (filterCriteria == null)
                filterCriteria = new FilterCriteria();

            var query = PredicateBuilder.True<UserLogin>();
            
            foreach (DictonaryFilter item in filterCriteria.DictonaryBasedFilter)
            {
                switch (item.Key.ToLower(enCulture))
                {                    
                    case "user_id":
                        {
                            long userId = 0;
                            if (!long.TryParse(item.Data, out userId))
                                return new ServiceResult<IEnumerable<UserLoginBo>>(null, false, "UserId Not Available!");
                            switch (item.OperandType)
                            {
                                case OperandType.Equal:
                                    query = query.And(x => x.UserId == userId);
                                    break;
                                case OperandType.NotEqual:
                                    query = query.And(x => x.UserId != userId);
                                    break;
                                case OperandType.Like:
                                    query = query.And(x => EF.Functions.Like(x.UserId.ToString(), "%" + item.Data.ToLower() + "%"));
                                    break;
                            }
                        }
                        break;
                }
            }

            query = query.And(x => orgIds.Contains(x.User.OrganizationId));

            filterCriteria.IncludeProperties = "User";
            resultList = await FindAsync(filterCriteria, query);

            if (resultList.Success)
            {
                listBo = resultList.Data;
            }
            else
            {
                return new ServiceResult<IEnumerable<UserLoginBo>>(null, false, resultList.Error);
            }

            if (listBo == null)
            {
                return new ServiceResult<IEnumerable<UserLoginBo>>(null, false, "Not Found!");
            }

            return new ServiceResult<IEnumerable<UserLoginBo>>(listBo, true, "");
        }

    }
}
