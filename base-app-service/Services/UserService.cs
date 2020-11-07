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
    public class UserService : BaseService, IBaseService<User, UserBo>
    {
        protected readonly PasswordHasher<UserBo> passwordHasher;

        public UserService(ServiceContext serviceContext, IServiceManager serviceManager) : base(serviceContext, serviceManager)
        {
            passwordHasher = new PasswordHasher<UserBo>();
        }

        public async Task<ServiceResult<UserBo>> CreateAsync(UserBo bo)
        {
            if (bo == null)
                return new ServiceResult<UserBo>(null, false, "User info is empty!");

            try
            {
                User entity;
                if (bo.Id > 0)
                {
                    entity = await repositoryManager.UserRepository.GetByIDAsync(bo.Id);
                    if (entity != null)
                        return new ServiceResult<UserBo>(null, false, "User already exist!");
                    else
                        bo.Id = 0;
                }
                if (!string.IsNullOrEmpty(bo.Password))
                    bo.Password = passwordHasher.HashPassword(bo, bo.Password);

                bo.FirstName = normalizer.NormalizeName(bo.FirstName);
                bo.MiddleName = normalizer.NormalizeName(bo.MiddleName);
                bo.LastName = normalizer.NormalizeName(bo.LastName);                
                bo.EmailAddress = normalizer.NormalizeEmail(bo.EmailAddress);

                entity = await repositoryManager.UserRepository.GetByEmailAddressAsync(bo.EmailAddress);
                if (entity != null)
                    return new ServiceResult<UserBo>(null, false, "Email address already exist!");

                entity = mapper.Map<User>(bo);
                entity.UserLogin = null;
                entity.Organization = null;
                entity.UserToken = null;
                entity.UserRole = null;
                entity.UserType = null;                

                await repositoryManager.UserRepository.InsertAsync(entity);
                await repositoryManager.CommitAsync();

                bo = mapper.Map<UserBo>(entity);
                bo.Organization = null;
                bo.Password = null;
                return new ServiceResult<UserBo>(bo, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<UserBo>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult> DeleteAsync(long id)
        {
            if (id <= 0)
                return new ServiceResult(false, "Id is empty!");

            try
            {
                User user = await repositoryManager.UserRepository.GetByIDAsync(id);                
                if(user == null)
                    return new ServiceResult(false, "User not found!");

                if (user.UserToken.Count == 1 && user.UserToken.FirstOrDefault() == null)
                    user.UserToken = new HashSet<UserToken>();

                user.Deleted = true;
                await repositoryManager.UserRepository.UpdateAsync(id, user);
                await repositoryManager.CommitAsync();

                return new ServiceResult(true);
            }
            catch (Exception ex)
            {
                return new ServiceResult(false, ex.Message);
            }
        }

        public async Task<ServiceResult<IEnumerable<UserBo>>> GetAsync(Expression<Func<User, bool>> filter = null, Func<IQueryable<User>, IOrderedQueryable<User>> orderBy = null, params Expression<Func<User, object>>[] includeProperties)
        {
            try
            {
                IEnumerable<UserBo> data = (await repositoryManager.UserRepository.GetAsync(filter, orderBy, includeProperties)).Select(x => mapper.Map<UserBo>(x));
                foreach (var item in data)
                {
                    item.Password = "";
                }
                return new ServiceResult<IEnumerable<UserBo>>(data, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<IEnumerable<UserBo>>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult<IEnumerable<UserBo>>> FindAsync(FilterCriteria filterCriteria, Expression<Func<User, bool>> predicateQuery=null)
        {
            if (filterCriteria == null)
                filterCriteria = new FilterCriteria();

            string includeProperties = filterCriteria.IncludeProperties ?? "";
            filterCriteria.PagingFilter = filterCriteria.PagingFilter ?? (new PagingFilter());
            if (filterCriteria.PagingFilter.pageNumber <= 0)
                filterCriteria.PagingFilter.pageNumber = 1;
            try
            {
                IEnumerable<User> entities = await repositoryManager.UserRepository.FindAsync(filterCriteria, predicateQuery);
                if (entities != null)
                {
                    IEnumerable<UserBo> dtos = entities.Select(t => mapper.Map<UserBo>(t));
                    foreach (var item in dtos)
                    {
                        item.Password = "";
                    }
                    return new ServiceResult<IEnumerable<UserBo>>(dtos, true, pagingFilter: filterCriteria.PagingFilter);
                }
                else
                    return new ServiceResult<IEnumerable<UserBo>>(null, true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<IEnumerable<UserBo>>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult<UserBo>> GetByIdAsync(long id)
        {
            if (id <= 0)
                return new ServiceResult<UserBo>(null, false, "Id is empty!");

            try
            {
                var entity = await repositoryManager.UserRepository.GetByIDAsync(id);
                if (entity != null)
                {
                    entity.Password = "";
                    var UserToken = await repositoryManager.UserTokenRepository.GetLastByUserIdAsync(id);
                    entity.UserToken = new List<UserToken>() { UserToken };
                    entity.Password = "";
                    UserBo bo = mapper.Map<UserBo>(entity);
                    return new ServiceResult<UserBo>(bo, true);
                }
                else
                    return new ServiceResult<UserBo>(null, false, "User not found!");
            }
            catch (Exception ex)
            {
                return new ServiceResult<UserBo>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult<UserBo>> GetByEmailAddressAsync(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
                return new ServiceResult<UserBo>(null, false, "Email address is empty!");

            try
            {
                var entity = await repositoryManager.UserRepository.GetByEmailAddressAsync(emailAddress);
                if (entity != null)
                {
                    entity.Password = "";
                    UserBo bo = mapper.Map<UserBo>(entity);
                    return new ServiceResult<UserBo>(bo, true);
                }
                else
                    return new ServiceResult<UserBo>(null, false, "User not found!");
            }
            catch (Exception ex)
            {
                return new ServiceResult<UserBo>(null, false, ex.Message);
            }
        }

        public async Task<ServiceResult> UpdateAsync(long id, UserBo bo)
        {
            if (bo == null)
                return new ServiceResult(false, "UserDto is empty!");

            try
            {
                User entity;

                if (bo.Id > 0 && id == bo.Id)
                {
                    bo.FirstName = normalizer.NormalizeName(bo.FirstName);
                    bo.MiddleName = normalizer.NormalizeName(bo.MiddleName);
                    bo.LastName = normalizer.NormalizeName(bo.LastName);
                    bo.EmailAddress = normalizer.NormalizeEmail(bo.EmailAddress);
                    if (!string.IsNullOrEmpty(bo.Password))
                        bo.Password = passwordHasher.HashPassword(bo, bo.Password);

                    entity = await repositoryManager.UserRepository.GetByIDAsync(id);
                    entity.FirstName = bo.FirstName;
                    entity.MiddleName = bo.MiddleName;
                    entity.LastName = bo.LastName;
                    entity.EmailAddress = bo.EmailAddress;
                    entity.UserTypeId = bo.UserTypeId;
                    entity.OrganizationId = bo.OrganizationId;
                    if (!string.IsNullOrEmpty(bo.Password))
                        entity.Password = bo.Password;
                    if (entity.UserToken.Count == 1 && entity.UserToken.FirstOrDefault() == null)
                        entity.UserToken = new HashSet<UserToken>();

                    await repositoryManager.UserRepository.UpdateAsync(id, entity);
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

        public async Task<ServiceResult<IEnumerable<UserBo>>> GetListAsync(long organizationId, FilterCriteria filterCriteria = null)
        {
            IEnumerable<UserBo> listBo = null;

            ServiceResult<IEnumerable<UserBo>> resultList = null;
            OrganizationBo dto = null;
            List<OrganizationBo> orgDtos;
            CultureInfo enCulture = new CultureInfo("en-US");

            ServiceResult<OrganizationBo> result = await serviceManager.Organization_Service.GetByIdAsync(organizationId);
            if (!result.Success || result.Data == null)
            {
                return new ServiceResult<IEnumerable<UserBo>>(null, false, "Organization Not Found!");
            }
            dto = result.Data;

            ServiceResult<List<OrganizationBo>> orgResults = await serviceManager.Organization_Service.GetOrganizationHierarchicaly(dto);
            if (!orgResults.Success || orgResults.Data == null || orgResults.Data.Count == 0)
            {
                return new ServiceResult<IEnumerable<UserBo>>(null, false, "Organizations Not Found!");
            }
            orgDtos = orgResults.Data;
            List<long> orgIds = orgDtos.Select(x => x.Id).ToList();
            if (filterCriteria == null)
                filterCriteria = new FilterCriteria();

            var query = PredicateBuilder.True<User>();
            query = query.And(x => x.Deleted == false);

            foreach (DictonaryFilter item in filterCriteria.DictonaryBasedFilter)
            {
                switch (item.Key.ToLower(enCulture))
                {
                    case "first_name":
                        switch (item.OperandType)
                        {
                            case OperandType.Equal:
                                query = query.And(x => x.FirstName == item.Data);
                                break;
                            case OperandType.Like:
                                query = query.And(x => EF.Functions.Like(x.FirstName.ToLower(), "%" + item.Data.ToLower() + "%"));
                                break;
                        }
                        break;
                    case "middle_name":
                        switch (item.OperandType)
                        {
                            case OperandType.Equal:
                                query = query.And(x => x.MiddleName == item.Data);
                                break;
                            case OperandType.Like:
                                query = query.And(x => EF.Functions.Like(x.MiddleName.ToLower(), "%" + item.Data.ToLower() + "%"));
                                break;
                        }
                        break;
                    case "last_name":
                        switch (item.OperandType)
                        {
                            case OperandType.Equal:
                                query = query.And(x => x.LastName == item.Data);
                                break;
                            case OperandType.Like:
                                query = query.And(x => EF.Functions.Like(x.LastName.ToLower(), "%" + item.Data.ToLower() + "%"));
                                break;
                        }
                        break;
                    case "email_address":
                        switch (item.OperandType)
                        {
                            case OperandType.Equal:
                                query = query.And(x => x.EmailAddress == item.Data);
                                break;
                            case OperandType.Like:
                                query = query.And(x => EF.Functions.Like(x.EmailAddress.ToLower(), "%" + item.Data.ToLower() + "%"));
                                break;
                        }
                        break;
                    case "user_type_id":
                        {
                            long userTypeId = 0;
                            if (!long.TryParse(item.Data, out userTypeId))
                                return new ServiceResult<IEnumerable<UserBo>>(null, false, "UserTypeId Not Available!");
                            switch (item.OperandType)
                            {
                                case OperandType.Equal:
                                    query = query.And(x => x.UserTypeId == userTypeId);
                                    break;
                                case OperandType.NotEqual:
                                    query = query.And(x => x.UserTypeId != userTypeId);
                                    break;
                                case OperandType.Like:
                                    query = query.And(x => EF.Functions.Like(x.UserTypeId.ToString(), "%" + item.Data.ToLower() + "%"));
                                    break;
                            }
                        }
                        break;
                }
            }

            query = query.And(x => orgIds.Contains(x.OrganizationId));

            filterCriteria.IncludeProperties = "Device,Device.Rtumod";
            resultList = await serviceManager.User_Service.FindAsync(filterCriteria, query);

            if (resultList.Success)
            {
                listBo = resultList.Data;
            }
            else
            {
                return new ServiceResult<IEnumerable<UserBo>>(null, false, resultList.Error);
            }

            if (listBo == null)
            {
                return new ServiceResult<IEnumerable<UserBo>>(null, false, "Not Found!");
            }

            return new ServiceResult<IEnumerable<UserBo>>(listBo, true, "");
        }

    }
}
