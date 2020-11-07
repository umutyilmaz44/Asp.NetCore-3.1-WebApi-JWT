using base_app_common;
using base_app_common.dto.organization;
using base_app_service;
using base_app_service.Bo;
using base_app_webapi.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace base_app_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [AuthorizeExt]
    public class OrganizationController : BaseController
    {
        public OrganizationController(IServiceManager serviceManager, ILogger<BaseController> logger) : base(serviceManager, logger)
        {
        }

        // GET: api/Users/5
        [HttpGet("Get/{id}")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.OrganizationRead)]
        public async Task<GenericResponse<OrganizationDto>> Get(long id)
        {
            // Yetki kontrolü yapılıyor
            ServiceResult<bool> resultAutorized = await GetAutorizedOrganizationStatusById(id);
            if (!resultAutorized.Success || !resultAutorized.Data)
            {                
                return GenericResponse<OrganizationDto>.Error(ResultType.Error, "Not Autorized Access!", "O_G_01", StatusCodes.Status203NonAuthoritative);
            }

            OrganizationBo bo = null;
            ServiceResult<OrganizationBo> result = await serviceManager.Organization_Service.GetByIdAsync(id);
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
                return GenericResponse<OrganizationDto>.Error(ResultType.Error, "User Not Found!", "O_G_02", StatusCodes.Status404NotFound);
            }

            return GenericResponse<OrganizationDto>.Ok(OrganizationBo.ConvertToDto(bo));
        }

        // POST: api/Users
        [HttpPost("Create")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.OrganizationCreate)]
        public async Task<GenericResponse<OrganizationDto>> Post([FromBody] OrganizationDto dto)
        {
            if (dto.ParentId.HasValue && dto.ParentId > 0)
            {
                // Yetki kontrolü yapılıyor
                ServiceResult<bool> resultAutorized = await GetAutorizedOrganizationStatusById(dto.ParentId.Value);
                if (!resultAutorized.Success || !resultAutorized.Data)
                {
                    return GenericResponse<OrganizationDto>.Error(ResultType.Error, "Not Autorized Access!", "O_PST_01", StatusCodes.Status203NonAuthoritative);
                }                    
            }
            else
            {
                return GenericResponse<OrganizationDto>.Error(ResultType.Error, "Parent organization identifer not found!", "O_PST_02", StatusCodes.Status404NotFound);
            }

            OrganizationBo bo = OrganizationBo.ConvertToBusinessObject(dto);
            ServiceResult<OrganizationBo> result = await serviceManager.Organization_Service.CreateAsync(bo);
            if (result.Success)
            {
                bo = result.Data;

                await serviceManager.CommitAsync();

                return GenericResponse<OrganizationDto>.Ok(OrganizationBo.ConvertToDto(bo));
            }
            else
            {
                return GenericResponse<OrganizationDto>.Error(ResultType.Error, "Parent organization identifer not found!", "O_PST_03", StatusCodes.Status500InternalServerError);
            }
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("Update/{id}")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.OrganizationUpdate)]
        public async Task<GenericResponse> Put(long id, OrganizationDto dto)
        {                         
            if (id != dto.Id)
            {
                return GenericResponse.Error(ResultType.Error, "Ids are mismatch!", "O_PT_01", StatusCodes.Status500InternalServerError);
            }
            try
            {
                // Yetki kontrolü yapılıyor
                ServiceResult<bool> resultAutorized = await GetAutorizedOrganizationStatusById(dto.Id);
                if (!resultAutorized.Success || !resultAutorized.Data)
                {
                    return GenericResponse.Error(ResultType.Error, "Not Autorized Access!", "O_PT_02", StatusCodes.Status203NonAuthoritative);
                }
                OrganizationBo bo = OrganizationBo.ConvertToBusinessObject(dto);
                ServiceResult serviceResult = await serviceManager.Organization_Service.UpdateAsync(id, bo);
                if (serviceResult.Success)
                {
                    await serviceManager.CommitAsync();

                    return GenericResponse.Ok();
                }
                else
                {
                    return GenericResponse.Error(ResultType.Error, serviceResult.Error, "O_PT_03", StatusCodes.Status500InternalServerError);
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message, LogLevel.Error, this.ControllerContext.RouteData.Values);
                return GenericResponse.Error(ResultType.Error, ex.Message, "O_PT_04", StatusCodes.Status500InternalServerError);
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("Delete/{id}")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.OrganizationDelete)]
        public async Task<GenericResponse> Delete(long id)
        {
            // Yetki kontrolü yapılıyor
            ServiceResult<bool> resultAutorized = await GetAutorizedOrganizationStatusById(id);
            if (!resultAutorized.Success || !resultAutorized.Data)
            {
                return GenericResponse.Error(ResultType.Error, "Not Autorized Access!", "O_DLT_01", StatusCodes.Status203NonAuthoritative);
            }
            ServiceResult serviceResult = await serviceManager.Organization_Service.DeleteAsync(id);
            if (serviceResult.Success)
            {
                return GenericResponse.Ok();
            }
            else
            {
                Log(serviceResult.Error, LogLevel.Error, this.ControllerContext.RouteData.Values);
                return GenericResponse.Error(ResultType.Error, serviceResult.Error, "O_DLT_02", StatusCodes.Status500InternalServerError);
            }
        }

        // GET: api/Users/5
        [HttpPost("GetList")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.OrganizationRead)]
        public async Task<GenericResponse<IEnumerable<OrganizationDto>>> GetList([FromBody] FilterCriteria filterCriteria)
        {
            IEnumerable<OrganizationBo> listBo = null;
            ServiceResult<IEnumerable<OrganizationBo>> resultList;
            CultureInfo enCulture = new CultureInfo("en-US");

            long organizationId = 0;

            if (filterCriteria == null)
                filterCriteria = new FilterCriteria();

            DictonaryFilter dictonaryFilter = filterCriteria.DictonaryBasedFilter.FirstOrDefault(x => x.Key == "organizationId");
            if (dictonaryFilter == null || !long.TryParse(dictonaryFilter.Data, out organizationId))
            {
                organizationId = 0;
            }

            if (organizationId > 0)
            {
                // Yetki kontrolü yapılıyor
                ServiceResult<bool> resultAutorized = await GetAutorizedOrganizationStatusById(organizationId);
                if (!resultAutorized.Success || !resultAutorized.Data)
                {
                    return GenericResponse<IEnumerable<OrganizationDto>>.Error(ResultType.Error, "Not Autorized Access!", "O_GL_01", StatusCodes.Status203NonAuthoritative);
                }
            }
            else
            {
                ServiceResult<UserBo> userDtoResult = await GetCurrentUser();
                if (!userDtoResult.Success || userDtoResult.Data == null)
                {
                    return GenericResponse<IEnumerable<OrganizationDto>>.Error(ResultType.Error, "User Not Found!", "O_GL_02", StatusCodes.Status404NotFound);
                }
                organizationId = userDtoResult.Data.OrganizationId;
            }

            resultList = await serviceManager.Organization_Service.GetHierarchicalyByOrganizationIdAsync(organizationId);
            if (!resultList.Success || resultList.Data == null)
            {
                return GenericResponse<IEnumerable<OrganizationDto>>.Error(ResultType.Error, resultList.Error, "O_GL_03", StatusCodes.Status500InternalServerError);
            }
            listBo = resultList.Data;

            foreach (DictonaryFilter item in filterCriteria.DictonaryBasedFilter)
            {
                switch (item.Key.ToLower(enCulture))
                {
                    case "title":
                        switch (item.OperandType)
                        {
                            case OperandType.Equal:
                                listBo = listBo.Where(x => x.Title.ToLower() == item.Data.ToLower());
                                break;
                            case OperandType.NotEqual:
                                listBo = listBo.Where(x => x.Title.ToLower() != item.Data.ToLower());
                                break;
                            case OperandType.Like:
                                listBo = listBo.Where(x => x.Title.ToLower().Contains(item.Data.ToLower()));
                                break;
                        }
                        break;
                    case "description":
                        switch (item.OperandType)
                        {
                            case OperandType.Equal:
                                listBo = listBo.Where(x => x.Description.ToLower() == item.Data.ToLower());
                                break;
                            case OperandType.NotEqual:
                                listBo = listBo.Where(x => x.Description.ToLower() != item.Data.ToLower());
                                break;
                            case OperandType.Like:
                                listBo = listBo.Where(x => x.Description.ToLower().Contains(item.Data.ToLower()));
                                break;
                        }
                        break;
                }
            }

            IEnumerable<OrganizationDto> listDto = listBo.Select(x => OrganizationBo.ConvertToDto(x));
            return GenericResponse<IEnumerable<OrganizationDto>>.Ok(listDto);
        }
    }
}