using base_app_common;
using base_app_common.dto.refreshtoken;
using base_app_service;
using base_app_service.Bo;
using base_app_webapi.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace base_app_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [AuthorizeExt]
    public class UserTokenController : BaseController
    {
        public UserTokenController(IServiceManager serviceManager, ILogger<BaseController> logger) : base(serviceManager, logger)
        {
        }

        // GET: api/Users/5
        [HttpGet("Get/{id}")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.UserTokenRead)]
        public async Task<GenericResponse<UserTokenDto>> Get(long id)
        {
            UserTokenBo bo = null;
            ServiceResult<UserTokenBo> result = await serviceManager.UserToken_Service.GetByIdAsync(id);
            if (result.Success)
            {
                bo = result.Data;
            }
            else
            {
                Log(result.Error, LogLevel.Error, this.ControllerContext.RouteData.Values);
            }

            if (bo == null)
            {
                return GenericResponse<UserTokenDto>.Error(ResultType.Error, "User Not Found!", "RT_G_01", StatusCodes.Status404NotFound);
            }

            return GenericResponse<UserTokenDto>.Ok(UserTokenBo.ConvertToDto(bo));
        }

       // POST: api/Users
       [HttpPost("Create")]
       [GrandAuthorize(GrandPermission.EndpointPermission, Grands.UserTokenCreate)]
        public async Task<GenericResponse<UserTokenDto>> Post([FromBody] UserTokenDto dto)
        {
            UserTokenBo bo = UserTokenBo.ConvertToBusinessObject(dto);
            ServiceResult<UserTokenBo> result = await serviceManager.UserToken_Service.CreateAsync(bo);
            if (result.Success)
            {
                bo = result.Data;

                await serviceManager.CommitAsync();
            }
            else
            {
                return GenericResponse<UserTokenDto>.Error(ResultType.Error, result.Error, "RT_PST_01", StatusCodes.Status500InternalServerError);
            }

            if (bo == null)
            {
                return GenericResponse<UserTokenDto>.Error(ResultType.Error, "NOt Found!", "RT_PST_02", StatusCodes.Status404NotFound);
            }

            return GenericResponse<UserTokenDto>.Ok(UserTokenBo.ConvertToDto(bo));
        }

        /// PUT: api/Users/5
        ///  To protect from overposting attacks, please enable the specific properties you want to bind to, for
        /// more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("Update/{id}")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.UserTokenUpdate)]
        public async Task<GenericResponse> Put(long id, UserTokenDto dto)
        {
            if (id != dto.Id)
            {
                return GenericResponse.Error(ResultType.Error, "Ids are mismatch!", "RT_PT_01", StatusCodes.Status500InternalServerError);
            }
            try
            {
                UserTokenBo bo = UserTokenBo.ConvertToBusinessObject(dto);
                ServiceResult serviceResult = await serviceManager.UserToken_Service.UpdateAsync(id, bo);
                if (serviceResult.Success)
                {
                    await serviceManager.CommitAsync();

                    return GenericResponse.Ok();
                }
                else
                {
                    return GenericResponse.Error(ResultType.Error, serviceResult.Error, "RT_PT_02", StatusCodes.Status500InternalServerError);
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message, LogLevel.Error, this.ControllerContext.RouteData.Values);
                return GenericResponse.Error(ResultType.Error, ex.Message, "RT_PT_03", StatusCodes.Status500InternalServerError);
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("Delete/{id}")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.UserTokenDelete)]
        public async Task<GenericResponse> Delete(long id)
        {
            ServiceResult serviceResult = await serviceManager.UserToken_Service.DeleteAsync(id);
            if (serviceResult.Success)
            {
                return GenericResponse.Ok();
            }
            else
            {
                Log(serviceResult.Error, LogLevel.Error, this.ControllerContext.RouteData.Values);
                return GenericResponse.Error(ResultType.Error, serviceResult.Error, "RT_DLT_01", StatusCodes.Status500InternalServerError);
            }
        }
    }
}