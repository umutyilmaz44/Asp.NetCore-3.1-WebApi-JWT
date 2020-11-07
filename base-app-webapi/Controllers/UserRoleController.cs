using base_app_common;
using base_app_common.dto.user;
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
    public class UserRoleController : BaseController
    {
        public UserRoleController(IServiceManager serviceManager, ILogger<BaseController> logger) : base(serviceManager, logger)
        {
        }

        // GET: api/Users/5
        [HttpGet("Get/{id}")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.UserRead)]
        public async Task<GenericResponse<UserRoleDto>> Get(long id)
        {
            UserRoleBo bo = null;
            ServiceResult<UserRoleBo> result = await serviceManager.UserRole_Service.GetByIdAsync(id);
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
                return GenericResponse<UserRoleDto>.Error(ResultType.Error, "Not Found!", "UR_G_01", StatusCodes.Status404NotFound);
            }

            return GenericResponse<UserRoleDto>.Ok(UserRoleBo.ConvertToDto(bo));
        }

        // POST: api/Users
        [HttpPost("Create")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.UserCreate)]
        public async Task<GenericResponse<UserRoleDto>> Post([FromBody] UserRoleDto dto)
        {            
            UserRoleBo bo = UserRoleBo.ConvertToBusinessObject(dto);
            ServiceResult<UserRoleBo> result = await serviceManager.UserRole_Service.CreateAsync(bo);
            if (result.Success)
            {
                bo = result.Data;

                await serviceManager.CommitAsync();
            }
            else
            {
                return GenericResponse<UserRoleDto>.Error(ResultType.Error, result.Error, "UR_PST_01", StatusCodes.Status500InternalServerError);
            }

            if (bo == null)
            {
                return GenericResponse<UserRoleDto>.Error(ResultType.Error, "Not Found!", "UR_PST_02", StatusCodes.Status404NotFound);
            }
            
            return GenericResponse<UserRoleDto>.Ok(UserRoleBo.ConvertToDto(bo));
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("Update/{id}")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.UserUpdate)]
        public async Task<GenericResponse> Put(long id, UserRoleDto dto)
        {
            if (id != dto.Id)
            {
                return GenericResponse.Error(ResultType.Error, "Ids are mismatch!", "UR_PT_01", StatusCodes.Status500InternalServerError);
            }
            try
            {
                UserRoleBo bo = UserRoleBo.ConvertToBusinessObject(dto);
                ServiceResult serviceResult = await serviceManager.UserRole_Service.UpdateAsync(id, bo);
                if (serviceResult.Success)
                {
                    await serviceManager.CommitAsync();

                    return GenericResponse.Ok();
                }
                else
                {
                    return GenericResponse.Error(ResultType.Error, serviceResult.Error, "UR_PT_02", StatusCodes.Status500InternalServerError);
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message, LogLevel.Error, this.ControllerContext.RouteData.Values);
                return GenericResponse.Error(ResultType.Error, ex.Message, "UR_PT_03", StatusCodes.Status500InternalServerError);
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("Delete/{id}")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.UserDelete)]
        public async Task<GenericResponse> Delete(long id)
        {
            ServiceResult serviceResult = await serviceManager.UserRole_Service.DeleteAsync(id);
            if (serviceResult.Success)
            {
                return GenericResponse.Ok();
            }
            else
            {
                Log(serviceResult.Error, LogLevel.Error, this.ControllerContext.RouteData.Values);
                return GenericResponse.Error(ResultType.Error, serviceResult.Error, "UR_DLT_01", StatusCodes.Status500InternalServerError);
            }
        }
    }
}