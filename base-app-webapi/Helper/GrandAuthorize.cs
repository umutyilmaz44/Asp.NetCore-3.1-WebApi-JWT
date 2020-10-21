using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;

namespace base_app_webapi.Models
{
    public class GrandAuthorizeAttribute : TypeFilterAttribute
    {
        public GrandAuthorizeAttribute(string GrandType, string GrandValue, string filterType = GrandFilterType.And) : base(typeof(GrandAuthorizeFilter))
        {
            Arguments = new object[] { new Claim(GrandType, GrandValue, filterType) };
        }
    }

    public class GrandAuthorizeFilter : IAuthorizationFilter
    {
        readonly Claim _Claim;

        public GrandAuthorizeFilter(Claim claim)
        {
            _Claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                IActionResult actionResult = null;
                switch (_Claim.Type)
                {
                    case GrandPermission.EndpointPermission:
                        Claim grandPermissions = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == _Claim.Type);
                        if (grandPermissions != null)
                        {
                            string[] userActionIds = grandPermissions.Value.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            string[] requirementActionIds = _Claim.Value.Split(',', StringSplitOptions.RemoveEmptyEntries);

                            switch (_Claim.ValueType)
                            {
                                case GrandFilterType.Or:
                                    {
                                        bool exist = false;
                                        for (int i = 0; i < requirementActionIds.Length; i++)
                                        {
                                            if (userActionIds.Contains(requirementActionIds[i]))
                                            {
                                                exist = true;
                                                break;
                                            }
                                        }

                                        if (!exist)
                                            actionResult = new ForbidResult();
                                    }
                                    break;
                                case GrandFilterType.And:
                                    {
                                        for (int i = 0; i < requirementActionIds.Length; i++)
                                        {
                                            if (!userActionIds.Contains(requirementActionIds[i]))
                                            {
                                                actionResult = new ForbidResult();
                                                break;
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                }
                if (actionResult != null)
                    context.Result = actionResult;
            }
            catch (Exception ex)
            {

            }
        }
    }
}