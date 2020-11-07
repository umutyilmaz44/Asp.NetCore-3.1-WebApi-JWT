using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;
using base_app_service;
using base_app_common;
using System.Collections.Generic;
using base_app_service.Bo;
using Microsoft.AspNetCore.Http;

namespace base_app_webapi.Helper
{
    public class AuthorizeExtAttribute : TypeFilterAttribute
    {
        public AuthorizeExtAttribute() : base(typeof(AuthorizeExtFilter))
        {
        }
    }

    public class AuthorizeExtFilter : IAuthorizationFilter
    {
        readonly IServiceManager serviceManager;

        public AuthorizeExtFilter(IServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                // it isn't needed to set unauthorized result 
                // as the base class already requires the user to be authenticated
                // this also makes redirect to a login page work properly
                // context.Result = new UnauthorizedResult();
                return;
            }

            try
            {
                
                long userTokenId=0;
                Claim claim = user.Claims.FirstOrDefault(x => x.Type == "utid");
                if(claim == null || !long.TryParse(claim.Value, out userTokenId))
                {
                    context.Result = new JsonResult(new { message = "User Token Id Not Found!" }) { StatusCode = StatusCodes.Status401Unauthorized };                    
                    return;
                }
                                
                UserTokenBo userTokenBo = null;
                ServiceResult<UserTokenBo> result = serviceManager.UserToken_Service.GetByIdAsync(userTokenId).Result;
                if(!result.Success)
                {
                    context.Result = new JsonResult(new { message = "User Token Not Found!" }) { StatusCode = StatusCodes.Status401Unauthorized };                    
                    return;
                }
                userTokenBo = result.Data;
                if(userTokenBo.IsLogout)
                {
                    context.Result = new JsonResult(new { message = "Token Expired!" }) { StatusCode = StatusCodes.Status401Unauthorized };                    
                    return;
                }              
            }
            catch (Exception ex)
            {
                context.Result = new JsonResult(new { message = ex.Message }) { StatusCode = StatusCodes.Status500InternalServerError };                    
                return;
            }   
        }
    }
}