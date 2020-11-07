using AutoMapper;
using base_app_common;
using base_app_repository;
using base_app_repository.Entities;
using base_app_service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace base_app_service
{
    public class ServiceManager : IServiceManager, IDisposable
    {
        public ServiceContext serviceContext { get; internal set; }
        private RepositoryManager repositoryManager;

        public ServiceManager(BaseDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper, ILookupNormalizer lookupNormalizer)
        {
            serviceContext = new ServiceContext();
            serviceContext.AddItem("baseDbContext", context);
            serviceContext.AddItem("IHttpContextAccessor", httpContextAccessor);
            serviceContext.AddItem("IMapper", mapper);
            serviceContext.AddItem("ILookupNormalizer", lookupNormalizer);

            string userid = "";
            if (httpContextAccessor != null && httpContextAccessor.HttpContext != null && httpContextAccessor.HttpContext.User != null)
            {                
                userid = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                if(string.IsNullOrEmpty(userid))
                    userid = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }   
            serviceContext.AddItem("CurrentUserId", userid);

            string token = "";
            if (httpContextAccessor != null && httpContextAccessor.HttpContext != null && httpContextAccessor.HttpContext.Request != null)
            {                
                token = httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if(string.IsNullOrEmpty(token))
                    token = "";
            }   
            serviceContext.AddItem("Token", token);

            repositoryManager = new RepositoryManager(context);
        }

        public async Task CommitAsync()
        {
            try
            {
                await repositoryManager.CommitAsync();
            }
            catch (Exception e)
            {
                //Log.Exception(e);
                throw e;
            }
        }

        #region Disposing
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (serviceContext != null)
                    {
                        //_log.Debug("ServiceContext is being disposed");
                        serviceContext.Dispose();
                    }
                }
                disposed = true;
            }
        }
        ~ServiceManager()
        {
            Dispose(false);
        }
        #endregion

        
        #region OrganizationService
        private OrganizationService organizationService;

        OrganizationService IServiceManager.Organization_Service
        {
            get
            {
                if (this.organizationService == null)
                    organizationService = new OrganizationService(serviceContext, this);

                return organizationService;
            }
        }
        #endregion
                
        #region UserService
        private UserService userService;

        UserService IServiceManager.User_Service
        {
            get
            {
                if (this.userService == null)
                    userService = new UserService(serviceContext, this);

                return userService;
            }
        }
        #endregion

        #region UserLoginService
        private UserLoginService userLoginService;

        UserLoginService IServiceManager.UserLogin_Service
        {
            get
            {
                if (this.userLoginService == null)
                    userLoginService = new UserLoginService(serviceContext, this);

                return userLoginService;
            }
        }
        #endregion

        #region RoleService
        private RoleService roleService;

        RoleService IServiceManager.Role_Service
        {
            get
            {
                if (this.roleService == null)
                    roleService = new RoleService(serviceContext, this);

                return roleService;
            }
        }
        #endregion

        #region UserRoleService
        private UserRoleService userRoleService;

        UserRoleService IServiceManager.UserRole_Service
        {
            get
            {
                if (this.userRoleService == null)
                    userRoleService = new UserRoleService(serviceContext, this);

                return userRoleService;
            }
        }
        #endregion

        #region UserTokenService
        private UserTokenService userTokenService;

        UserTokenService IServiceManager.UserToken_Service
        {
            get
            {
                if (this.userTokenService == null)
                    userTokenService = new UserTokenService(serviceContext, this);

                return userTokenService;
            }
        }
        #endregion
    }
}
