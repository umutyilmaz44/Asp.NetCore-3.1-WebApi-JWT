using base_app_common;
using base_app_common.dto.organization;
using base_app_service;
using base_app_service.Bo;
using base_app_webapi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace base_app_webapi.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    //[Authorize]
    //public class OrganizationController : ControllerBase
    //{
    //    public OrganizationController(IServiceManager serviceManager, ILogger logger) //: base(serviceManager, logger)
    //    {
    //    }

    //    // GET: api/Users/5
    //    [HttpGet("Get/{id}")]
    //    [GrandAuthorize(GrandPermission.EndpointPermission, Grands.OrganizationRead)]
    //    public async Task<ActionResult<OrganizationDto>> Get(long id)
    //    {
    //        // Yetki kontrolü yapılıyor
    //        ServiceResult<bool> resultAutorized = await GetAutorizedOrganizationStatusById(id);
    //        if (!resultAutorized.Success || !resultAutorized.Data)
    //            return BadRequest("Not Autorized Access!");

    //        OrganizationBo bo = null;
    //        ServiceResult<OrganizationBo> result = await serviceManager.Organization_Service.GetByIdAsync(id);
    //        if (result.Success)
    //        {
    //            bo = result.Data;
    //        }
    //        else
    //        {
    //            Log(result.Error, LogLevel.Error, this.ControllerContext.RouteData.Values);
    //        }

    //        if (bo == null)
    //        {
    //            return NotFound();
    //        }

    //        return OrganizationBo.ConvertToDto(bo);
    //    }

    //    // POST: api/Users
    //    [HttpPost("Create")]
    //    [GrandAuthorize(GrandPermission.EndpointPermission, Grands.OrganizationCreate)]
    //    public async Task<ActionResult<OrganizationDto>> Post([FromBody] OrganizationDto dto)
    //    {
    //        if (dto.ParentId.HasValue && dto.ParentId > 0)
    //        {
    //            // Yetki kontrolü yapılıyor
    //            ServiceResult<bool> resultAutorized = await GetAutorizedOrganizationStatusById(dto.ParentId.Value);
    //            if (!resultAutorized.Success || !resultAutorized.Data)
    //                return BadRequest("Not Autorized Access!");
    //        }
    //        else
    //        {
    //            return BadRequest("Parent organization identifer not found!");
    //        }

    //        OrganizationBo bo = OrganizationBo.ConvertToBusinessObject(dto);
    //        ServiceResult<OrganizationBo> result = await serviceManager.Organization_Service.CreateAsync(bo);
    //        if (result.Success)
    //        {
    //            bo = result.Data;

    //            await serviceManager.CommitAsync();

    //            return OrganizationBo.ConvertToDto(bo);
    //        }
    //        else
    //        {
    //            return BadRequest(result.Error);
    //        }

    //        return BadRequest();
    //    }

    //    // PUT: api/Users/5
    //    // To protect from overposting attacks, please enable the specific properties you want to bind to, for
    //    // more details see https://aka.ms/RazorPagesCRUD.
    //    [HttpPut("Update/{id}")]
    //    [GrandAuthorize(GrandPermission.EndpointPermission, Grands.OrganizationUpdate)]
    //    public async Task<IActionResult> Put(long id, OrganizationDto dto)
    //    {
    //        if (id != dto.Id)
    //        {
    //            return BadRequest();
    //        }
    //        try
    //        {
    //            // Yetki kontrolü yapılıyor
    //            ServiceResult<bool> resultAutorized = await GetAutorizedOrganizationStatusById(dto.Id);
    //            if (!resultAutorized.Success || !resultAutorized.Data)
    //                return BadRequest("Not Autorized Access!");

    //            OrganizationBo bo = OrganizationBo.ConvertToBusinessObject(dto);
    //            ServiceResult serviceResult = await serviceManager.Organization_Service.UpdateAsync(id, bo);
    //            if (serviceResult.Success)
    //            {
    //                await serviceManager.CommitAsync();

    //                return Ok();
    //            }
    //            else
    //            {
    //                return BadRequest(serviceResult.Error);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Log(ex.Message, LogLevel.Error, this.ControllerContext.RouteData.Values);
    //            return BadRequest(ex.Message);
    //        }
    //    }

    //    // DELETE: api/Users/5
    //    [HttpDelete("Delete/{id}")]
    //    [GrandAuthorize(GrandPermission.EndpointPermission, Grands.OrganizationDelete)]
    //    public async Task<ActionResult> Delete(long id)
    //    {
    //        // Yetki kontrolü yapılıyor
    //        ServiceResult<bool> resultAutorized = await GetAutorizedOrganizationStatusById(id);
    //        if (!resultAutorized.Success || !resultAutorized.Data)
    //            return BadRequest("Not Autorized Access!");

    //        ServiceResult serviceResult = await serviceManager.Organization_Service.DeleteAsync(id);
    //        if (serviceResult.Success)
    //        {
    //            return Ok();
    //        }
    //        else
    //        {
    //            Log(serviceResult.Error, LogLevel.Error, this.ControllerContext.RouteData.Values);
    //            return BadRequest(serviceResult.Error);
    //        }
    //    }

    //    // GET: api/Users/5
    //    [HttpGet("GetList")]
    //    [GrandAuthorize(GrandPermission.EndpointPermission, Grands.OrganizationRead)]
    //    public async Task<ActionResult<IEnumerable<OrganizationDto>>> GetList(long organizationId)
    //    {
    //        IEnumerable<OrganizationBo> listBo = null;
    //        ServiceResult<IEnumerable<OrganizationBo>> resultList;

    //        if (organizationId > 0)
    //        {
    //            // Yetki kontrolü yapılıyor
    //            ServiceResult<bool> resultAutorized = await GetAutorizedOrganizationStatusById(organizationId);
    //            if (!resultAutorized.Success || !resultAutorized.Data)
    //                return BadRequest("Not Autorized Access!");
    //        }
    //        else
    //        {
    //            ServiceResult<UserBo> userDtoResult = await GetCurrentUser();
    //            if (!userDtoResult.Success || userDtoResult.Data == null)
    //                return BadRequest("User Not Found!");

    //            organizationId = userDtoResult.Data.OrganizationId;
    //        }

    //        resultList = await serviceManager.Organization_Service.GetHierarchicalyByOrganizationIdAsync(organizationId);
    //        if (!resultList.Success || resultList.Data == null)
    //        {
    //            return NotFound(resultList.Error);
    //        }
    //        listBo = resultList.Data;
    //        IEnumerable<OrganizationDto> listDto = listBo.Select(x => OrganizationBo.ConvertToDto(x));
    //        return Ok(listDto);
    //    }
    //}
}