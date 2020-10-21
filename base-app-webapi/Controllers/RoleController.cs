using base_app_common;
using base_app_common.dto.role;
using base_app_service;
using base_app_service.Bo;
using base_app_webapi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace base_app_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleController : BaseController
    {
        public RoleController(IServiceManager serviceManager, ILogger<BaseController> logger) : base(serviceManager, logger)
        {
        }

        // GET: api/Users/5
        [HttpGet("Get/{id}")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.RoleRead)]
        public async Task<ActionResult<RoleDto>> Get(long id)
        {
            RoleBo bo = null;
            ServiceResult<RoleBo> result = await serviceManager.Role_Service.GetByIdAsync(id);
            if (result.Success)
            {
                bo = result.Data;
            }
            else
            {
                //Log(result.Error, LogLevel.Error, this.ControllerContext.RouteData.Values);
            }

            if (bo == null)
            {
                return NotFound();
            }

            return RoleBo.ConvertToDto(bo);
        }

        // POST: api/Users
        [HttpPost("Create")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.RoleCreate)]
        public async Task<ActionResult<RoleDto>> Post([FromBody] RoleDto dto)
        {
            RoleBo bo = RoleBo.ConvertToBusinessObject(dto);
            ServiceResult<RoleBo> result = await serviceManager.Role_Service.CreateAsync(bo);
            if (result.Success)
            {
                bo = result.Data;

                await serviceManager.CommitAsync();
            }
            else
            {
                return BadRequest(result.Error);
            }

            if (bo == null)
            {
                return NotFound();
            }

            return RoleBo.ConvertToDto(bo);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("Update/{id}")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.RoleUpdate)]
        public async Task<IActionResult> Put(long id, RoleDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }
            try
            {
                RoleBo bo = RoleBo.ConvertToBusinessObject(dto);
                ServiceResult serviceResult = await serviceManager.Role_Service.UpdateAsync(id, bo);
                if (serviceResult.Success)
                {
                    await serviceManager.CommitAsync();

                    return Ok();
                }
                else
                {
                    return BadRequest(serviceResult.Error);
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message, LogLevel.Error, this.ControllerContext.RouteData.Values);
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("Delete/{id}")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.RoleDelete)]
        public async Task<ActionResult> Delete(long id)
        {
            ServiceResult serviceResult = await serviceManager.Role_Service.DeleteAsync(id);
            if (serviceResult.Success)
            {
                return Ok();
            }
            else
            {
                Log(serviceResult.Error, LogLevel.Error, this.ControllerContext.RouteData.Values);
                return BadRequest(serviceResult.Error);
            }
        }
    }
}