using AutoMapper;
using base_app_common;
using base_app_common.dto.user;
using base_app_repository.Entities;
using base_app_service;
using base_app_service.Bo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace base_app_webapi.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected readonly long currentUserId;
        protected readonly ILogger<BaseController> logger;
        protected readonly IMapper mapper;
        protected readonly IServiceManager serviceManager;
        protected readonly UpperInvariantLookupNormalizer normalizer;
        protected readonly PasswordHasher<UserBo> passwordHasher;
        protected BaseController(IServiceManager serviceManager, ILogger<BaseController> logger)
        {
            this.logger = logger;
            this.serviceManager = serviceManager;
            this.mapper = this.serviceManager.serviceContext.GetItem<IMapper>("IMapper");
            this.normalizer = this.serviceManager.serviceContext.GetItem<UpperInvariantLookupNormalizer>("ILookupNormalizer");
            string userId = this.serviceManager.serviceContext.GetItem<string>("CurrentUserId");
            if (!long.TryParse(userId, out currentUserId))
                this.currentUserId = -1;

            this.passwordHasher = new PasswordHasher<UserBo>();
        }

        protected void Log(string message, Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.AspNetCore.Routing.RouteValueDictionary routeValueDictionary)
        {
            string actionName = routeValueDictionary.FirstOrDefault(x => x.Key == "action").Value.ToString();
            string controllerName = routeValueDictionary.FirstOrDefault(x => x.Key == "controller").Value.ToString();

            Log(message, logLevel, controllerName, actionName);
        }

        protected void Log(string message, Microsoft.Extensions.Logging.LogLevel logLevel, string controllerName, string actionName)
        {
            switch (logLevel)
            {
                case Microsoft.Extensions.Logging.LogLevel.Error:
                    logger.LogError(string.Format("{0,-30}{1,-30}{2}", controllerName, actionName, message));
                    break;
                case Microsoft.Extensions.Logging.LogLevel.Debug:
                    logger.LogDebug(string.Format("{0,-30}{1,-30}{2}", controllerName, actionName, message));
                    break;
                case Microsoft.Extensions.Logging.LogLevel.Warning:
                    logger.LogWarning(string.Format("{0,-30}{1,-30}{2}", controllerName, actionName, message));
                    break;
                default:
                case Microsoft.Extensions.Logging.LogLevel.Information:
                    logger.LogInformation(string.Format("{0,-30}{1,-30}{2}", controllerName, actionName, message));
                    break;
            }
        }

        protected async Task<ServiceResult<UserBo>> GetCurrentUser()
        {
            ServiceResult<UserBo> result = null;
            if (this.currentUserId > 0)
            {
                ServiceResult<IEnumerable<UserBo>> resultList = await serviceManager.User_Service.GetAsync(filter: (x => x.Id == this.currentUserId), includeProperties: (x => x.Organization));
                if (!resultList.Success || resultList.Data == null)
                    result = new ServiceResult<UserBo>(null, false, "Logged user not found!");
                else
                {
                    UserBo userDto = resultList.Data.FirstOrDefault();
                    result = new ServiceResult<UserBo>(userDto, true, "");
                }
            }
            else
                result = new ServiceResult<UserBo>(null, false, "Logged user identity not found!");

            return result;
        }

        protected async Task<ServiceResult<List<OrganizationBo>>> GetAutorizedOrganizationList()
        {
            ServiceResult<List<OrganizationBo>> result;
            if (this.currentUserId > 0)
            {
                ServiceResult<IEnumerable<UserBo>> resultList = await serviceManager.User_Service.GetAsync(filter: (x => x.Id == this.currentUserId), includeProperties: (x => x.Organization));
                if (!resultList.Success || resultList.Data == null)
                    result = new ServiceResult<List<OrganizationBo>>(null, false, "Logged user not found!");
                else
                {
                    UserBo userDto = resultList.Data.FirstOrDefault();
                    result = await serviceManager.Organization_Service.GetOrganizationHierarchicaly(userDto.Organization);
                }
            }
            else
                result = new ServiceResult<List<OrganizationBo>>(null, false, "Logged user identity not found!");

            return result;
        }

        protected async Task<ServiceResult<bool>> GetAutorizedOrganizationStatusById(long organizationId)
        {
            ServiceResult<bool> result;
            ServiceResult<List<OrganizationBo>> resultList = await GetAutorizedOrganizationList();
            if (!resultList.Success || resultList.Data == null)
                result = new ServiceResult<bool>(false, true, resultList.Error);
            else
            {
                List<OrganizationBo> organizationlist = resultList.Data;
                OrganizationBo dto = organizationlist.FirstOrDefault(x => x.Id == organizationId);
                if (dto != null)
                    result = new ServiceResult<bool>(true, true, "");
                else
                    result = new ServiceResult<bool>(false, true, "");
            }

            return result;
        }

        protected async Task<ServiceResult<bool>> GetAutorizedUserStatusById(UserBo userDto)
        {
            return await GetAutorizedUserStatusById(userDto.OrganizationId);
        }

        protected async Task<ServiceResult<bool>> GetAutorizedUserStatusById(long organizationid)
        {
            ServiceResult<bool> result;

            ServiceResult<List<OrganizationBo>> resultOrganizationList = await GetAutorizedOrganizationList();
            if (!resultOrganizationList.Success || resultOrganizationList.Data == null)
                result = new ServiceResult<bool>(false, true, resultOrganizationList.Error);
            else
            {
                List<OrganizationBo> organizationDtos = resultOrganizationList.Data;
                List<long> organizationIds = organizationDtos.Select(x => x.Id).ToList();

                if (organizationIds.Contains(organizationid))
                    result = new ServiceResult<bool>(true, true, "");
                else
                    result = new ServiceResult<bool>(false, false, "Not autorized!");
            }

            return result;
        }
    }
}
