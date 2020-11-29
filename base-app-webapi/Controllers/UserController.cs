using AutoMapper;
using base_app_common;
using base_app_common.dto.refreshtoken;
using base_app_common.dto.user;
using base_app_repository.Entities;
using base_app_service;
using base_app_service.Bo;
using base_app_webapi.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace base_app_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [AuthorizeExt]
    public class UserController : BaseController
    {
        private readonly JWTSettings jwtSettings;
        private readonly IMailer mailer;
        public UserController(IServiceManager serviceManager, IOptions<JWTSettings> jwtSettings, ILogger<BaseController> logger, IMailer mailer) 
                    : base(serviceManager, logger)
        {
            this.jwtSettings = jwtSettings.Value;
            this.mailer = mailer;
        }

        #region Authentication   

        // POST: api/User/GetToken/
        [HttpPost("GetToken")]
        [AllowAnonymous]
        public async Task<GenericResponse<TokenResponseDto>> GetToken([FromBody] TokenDto tokenDto)
        {
            UserBo user = null;            
            ServiceResult<IEnumerable<UserBo>> result;

            FilterCriteria filterCriteria = new FilterCriteria();
            filterCriteria.QueryFilter = "EmailAddress = \"" + tokenDto.EmailAddress + "\"";
            filterCriteria.IncludeProperties = "UserType,UserRole,UserRole.Role,UserRole.Role.GrandRole";
            result = await serviceManager.User_Service.FindAsync(filterCriteria);

            if (result.Success)
            {
                user = result.Data.FirstOrDefault();
            }
            else
            {
                Log(result.Error, LogLevel.Error, this.ControllerContext.RouteData.Values);
            }

            if (user == null)
            {                
                return GenericResponse<TokenResponseDto>.Error(ResultType.Error, "Not Found!", "U_GT_01", StatusCodes.Status404NotFound);
            }          
            
            Microsoft.AspNetCore.Identity.PasswordVerificationResult verificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, tokenDto.Password);
            if (verificationResult == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
            {
                return GenericResponse<TokenResponseDto>.Error(ResultType.Error, "Password verification failed!", "U_GT_02", StatusCodes.Status404NotFound);
            }                       

            ServiceResult<TokenResponseDto> userTokenResult = await GetTokenResponseAsync(user);
            if(!userTokenResult.Success)
            {
                return GenericResponse<TokenResponseDto>.Error(ResultType.Error, userTokenResult.Error, "U_GT_03", StatusCodes.Status500InternalServerError);
            }
            
            return GenericResponse<TokenResponseDto>.Ok((userTokenResult.Data));
        }

        // POST: api/User/RefreshToken/
        [HttpPost("RefreshToken")]
        public async Task<GenericResponse<TokenResponseDto>> RefreshToken(string refresh_token)
        {
            //ServiceResult<UserBo> userResult = await GetUserFromAccessToken(refreshRequestDto.Access_Token);
            if(currentUserId <= 0)
            {
                return GenericResponse<TokenResponseDto>.Error(ResultType.Error, "User not found into the token!", "U_RT_01", StatusCodes.Status404NotFound);
            }

            string access_token = serviceManager.serviceContext.Items["Token"].ToString();
            if(string.IsNullOrEmpty(access_token))
            {
                return GenericResponse<TokenResponseDto>.Error(ResultType.Error, "Token not found into the request!", "U_RT_02", StatusCodes.Status404NotFound);
            }

            ServiceResult validationResult = await ValidateUserToken(currentUserId, access_token, refresh_token);
            if (!validationResult.Success)
            {
                return GenericResponse<TokenResponseDto>.Error(ResultType.Error, validationResult.Error, "U_RT_03", StatusCodes.Status404NotFound);
            }
            
            UserBo user = null;
            FilterCriteria filterCriteria = new FilterCriteria();
            filterCriteria.QueryFilter = "Id = " + this.currentUserId;
            filterCriteria.IncludeProperties = "UserType,UserRole,UserRole.Role,UserRole.Role.GrandRole";
            ServiceResult<IEnumerable<UserBo>> userResult = await serviceManager.User_Service.FindAsync(filterCriteria);
            if (!userResult.Success || userResult.Data == null)
            {
                return GenericResponse<TokenResponseDto>.Error(ResultType.Error, "Logged user not found!", "U_RT_04", StatusCodes.Status404NotFound);
            }
            else
            {
                user = userResult.Data.FirstOrDefault();
            }

            ServiceResult<TokenResponseDto> userTokenResult = await GetTokenResponseAsync(user);
            if(!userTokenResult.Success)
            {
                return GenericResponse<TokenResponseDto>.Error(ResultType.Error, userTokenResult.Error, "U_RT_05", StatusCodes.Status500InternalServerError);
            }

            return GenericResponse<TokenResponseDto>.Ok(userTokenResult.Data);
        }

        // POST: api/User/Logout/
        [HttpPut("Logout")]
        public async Task<GenericResponse> Logout()
        {
            try{
                Claim claim = null;
                long userTokenId=0;
                string access_token = "";
                IHttpContextAccessor httpContextAccessor = (IHttpContextAccessor)serviceManager.serviceContext.Items["IHttpContextAccessor"];
                if(httpContextAccessor != null && httpContextAccessor.HttpContext != null && httpContextAccessor.HttpContext.User != null)
                {
                    claim = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "utid");
                    if(claim != null)
                    {
                        if(!long.TryParse(claim.Value, out userTokenId))
                            userTokenId=0;
                    }
                }

                if(userTokenId == 0)
                {
                    access_token = serviceManager.serviceContext.Items["Token"].ToString();
                    if(string.IsNullOrEmpty(access_token))
                    {
                        return GenericResponse.Error(ResultType.Error, "Token not found into the request!", "U_LO_01", StatusCodes.Status404NotFound);
                    }
                }
                                
                UserTokenBo userTokenBo = null;
                ServiceResult<IEnumerable<UserTokenBo>> result = null;
                if(userTokenId > 0)
                {
                    result = await serviceManager.UserToken_Service.GetAsync(
                                    filter: (rt => rt.Id == userTokenId && rt.UserId == this.currentUserId), 
                                    orderBy: (rt => rt.OrderByDescending(x => x.ExpiryDate)));
                }
                else
                {
                    result = await serviceManager.UserToken_Service.GetAsync(
                                    filter: (rt => rt.AccessToken == access_token && rt.UserId == this.currentUserId), 
                                    orderBy: (rt => rt.OrderByDescending(x => x.ExpiryDate)));
                }
                if(!result.Success)
                {
                    return GenericResponse.Error(ResultType.Error, "User Token Not Found!", "U_LO_02", StatusCodes.Status404NotFound);
                }

                userTokenBo = result.Data.FirstOrDefault();
                userTokenBo.LogoutTime=DateTime.Now;
                userTokenBo.IsLogout = true;
                await serviceManager.UserToken_Service.UpdateAsync(userTokenBo.Id, userTokenBo);

                return GenericResponse.Ok();   
            }
            catch(Exception ex) {
                return GenericResponse.Error(ResultType.Error, ex.Message, "U_LO_03", StatusCodes.Status500InternalServerError);
            }
        }

        private async Task<ServiceResult<TokenResponseDto>> GetTokenResponseAsync(UserBo user)
        {
            string accessToken="";                                         
            string refreshToken = GenerateRefreshToken();                            
            ServiceResult<TokenResponseDto> response = null;

            UserTokenBo userTokenBo = GenerateUserToken(user);
            userTokenBo.RefreshToken = refreshToken;
            userTokenBo.AccessToken = "";

            ServiceResult<UserTokenBo> userTokenResult = await serviceManager.UserToken_Service.CreateAsync(userTokenBo);
            if(!userTokenResult.Success)
            {
                response = new ServiceResult<TokenResponseDto>(null, false, "User Token Create Failed!");
                return response;
            }
            userTokenBo = userTokenResult.Data;
            
            try
            {
                //sign your token here here..   
                accessToken = GenerateAccessToken(userTokenBo.Id, user);    
            }
            catch(Exception ex)
            {
                response = new ServiceResult<TokenResponseDto>(null, false, "Token Create Failed! " + (ex.Message) );
                return response;
            }
            userTokenBo.AccessToken = accessToken;
            await serviceManager.UserToken_Service.UpdateAsync(userTokenBo.Id, userTokenBo);
            await serviceManager.UserLogin_Service.CreateAsync(new UserLoginBo() { UserId = user.Id, LoginTime=DateTime.UtcNow });

            TokenResponseDto tokenResponseDto = UserBo.ConvertToTokenResponseDto(user);              
            tokenResponseDto.AccessToken = accessToken;
            tokenResponseDto.RefreshToken = refreshToken; 
            
            response = new ServiceResult<TokenResponseDto>(tokenResponseDto, true, "");
            return response;
        }

        private async Task<ServiceResult> ValidateUserToken(long userid, string access_token, string refresh_token)
        {
            ServiceResult response = new ServiceResult(false,"");
            ServiceResult<IEnumerable<UserTokenBo>> userTokenResult = await serviceManager.UserToken_Service.GetAsync(
                                                                                    filter: (x => x.UserId == userid && x.RefreshToken == refresh_token),
                                                                                    orderBy: (x => x.OrderByDescending(x => x.ExpiryDate)));
            if(!userTokenResult.Success || userTokenResult.Data == null || userTokenResult.Data.FirstOrDefault() == null)
            {
                response = new ServiceResult(false, "Refresh token not found!");
                return response;
            }            
            UserTokenBo userTokenBo = userTokenResult.Data.FirstOrDefault();
            if(userTokenBo.ExpiryDate < DateTime.UtcNow)
            {
                response = new ServiceResult(false, "Refresh token expired!");
                return response;
            }
            if(userTokenBo.IsLogout)
            {
                response = new ServiceResult(false, "Refresh token logouted!");
                return response;
            }
            if(userTokenBo.AccessToken != access_token)
            {
                response = new ServiceResult(false, "Access token mismatch!");
                return response;
            }

            response = new ServiceResult(true, "");
            return response;
        }

        private UserTokenBo GenerateUserToken(UserBo userBo)
        {
            UserTokenBo userToken = new UserTokenBo();
            userToken.UserId = userBo.Id;            
            userToken.LoginTime=DateTime.UtcNow;
            // Token Life Time Setting            
            int tokenLifeTimeSec = (userBo.UserType != null && userBo.UserType.TokenLifeTime > 0) ? userBo.UserType.TokenLifeTime : 60;
            DateTime dtimeTokenLife = DateTime.UtcNow.AddSeconds(tokenLifeTimeSec);
            userToken.ExpiryDate = dtimeTokenLife;

            return userToken;
        }

        private string GenerateAccessToken(long userTokenId, UserBo userDto)
        {
            /// NOTICE
            /// Token a iligli kullanıcının id değeri ve yetkisinde olan action id leri ekleniyor 
            /// bunun haricinden yeni değerlerin eklenmesi token boyutunu arttıracağından dolayı 
            /// yeni değerlerin eklenmemesi gerekmektedir
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("utid", Convert.ToString(userTokenId)));
            claims.Add(new Claim(ClaimTypes.Name, Convert.ToString(userDto.Id)));

            string grandIds = "";
            ICollection<UserRoleBo> userRoleList = userDto.UserRole;
            foreach (var userRole in userRoleList)
            {
                foreach (var grandRole in userRole.Role.GrandRole)
                {
                    grandIds += grandRole.GrandId + ",";
                }
            }
            grandIds = grandIds.Trim(',');
            claims.Add(new Claim(GrandPermission.EndpointPermission, grandIds));

            // Token Life Time Setting            
            int tokenLifeTimeSec = (userDto.UserType != null && userDto.UserType.TokenLifeTime > 0) ? userDto.UserType.TokenLifeTime : 60;
            DateTime dtimeTokenLife = DateTime.UtcNow.AddSeconds(tokenLifeTimeSec);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = dtimeTokenLife,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                EncryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256)
                //new X509EncryptingCredentials(new X509Certificate2(key))
            };
            SecurityToken token = null;
            try
            {
                token = tokenHandler.CreateToken(tokenDescriptor);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            string refreshToekn="";
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                refreshToekn = Convert.ToBase64String(randomNumber);
            }

            return refreshToekn;
        }
        private async Task<ServiceResult<UserBo>> GetUserFromAccessToken(string accessToken)
        {
            ServiceResult<UserBo> response = new ServiceResult<UserBo>(null, false, "");

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    TokenDecryptionKey = new SymmetricSecurityKey(key)
                };

                SecurityToken securityToken;
                var principle = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);

                JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

                if (jwtSecurityToken != null && 
                    jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.Aes128KW, StringComparison.InvariantCultureIgnoreCase))
                {
                    var userIdObj = principle.FindFirst(ClaimTypes.Name)?.Value;
                    long userId;
                    if (!long.TryParse(userIdObj, out userId))
                    {
                        response = new ServiceResult<UserBo>(null, false, "Token UserId not available!");
                        return response;
                    }
                        

                    UserBo userDto = null;                    
                    FilterCriteria filterCriteria = new FilterCriteria();
                    filterCriteria.QueryFilter = "Id = " + userId;
                    filterCriteria.IncludeProperties = "UserType,UserRole,UserRole.Role,UserRole.Role.GrandRole";
                    ServiceResult<IEnumerable<UserBo>> resultList = await serviceManager.User_Service.FindAsync(filterCriteria);
                    if (resultList.Success && resultList.Data != null && resultList.Data.FirstOrDefault() != null){
                        userDto = resultList.Data.FirstOrDefault();
                        response = new ServiceResult<UserBo>(userDto, true, "");
                    }
                    else
                        response = new ServiceResult<UserBo>(null, false, "Token User not found!");
                }
                else{
                    response = new ServiceResult<UserBo>(null, false, "JwtSecurityToken Error!");
                }
            }
            catch (Exception ex)
            {
                response = new ServiceResult<UserBo>(null, false, "Token User finding error!");
                Log(ex.Message, LogLevel.Error, this.ControllerContext.RouteData.Values);
            }

            return response;
        }
        #endregion

        // GET: api/Users/5
        [HttpGet("Get/{id}")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.UserRead)]
        public async Task<GenericResponse<UserDto>> Get(long id)
        {
            UserBo userBo = null;
            FilterCriteria filterCriteria = new FilterCriteria();
            filterCriteria.QueryFilter = "Id = " + id;
            filterCriteria.IncludeProperties = "UserRole,UserRole.Role,UserRole.Role.GrandRole,UserRole.Role.GrandRole.Grand";
            ServiceResult<IEnumerable<UserBo>> result = await serviceManager.User_Service.FindAsync(filterCriteria);
            if (result.Success)
            {
                userBo = result.Data.FirstOrDefault();
                if (userBo == null)
                {
                    return GenericResponse<UserDto>.Error(ResultType.Error, "Not Found!", "U_G_01", StatusCodes.Status404NotFound);
                }                    
                else
                {
                    // Yetki kontrolü yapılıyor
                    ServiceResult<bool> resultAutorized = await GetAutorizedUserStatusById(userBo);
                    if (!resultAutorized.Success || !resultAutorized.Data)
                    {
                        return GenericResponse<UserDto>.Error(ResultType.Error, "Not Autorized Access!", "U_G_02", StatusCodes.Status203NonAuthoritative);
                    }
                }

                UserDto userDto = UserBo.ConvertToDto(userBo);
                userDto.Password = "";

                return GenericResponse<UserDto>.Ok(userDto);
            }
            else
            {
                Log(result.Error, LogLevel.Error, this.ControllerContext.RouteData.Values);
                return GenericResponse<UserDto>.Error(ResultType.Error, result.Error, "U_G_03", StatusCodes.Status500InternalServerError);
            }
        }

        // POST: api/Users
        [HttpPost("Create")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.UserCreate)]
        public async Task<GenericResponse<UserDto>> Post([FromBody] UserDto dto)
        {
            UserBo bo = UserBo.ConvertToBusinessObject(dto);

            // Yetki kontrolü yapılıyor
            ServiceResult<bool> resultAutorized = await GetAutorizedUserStatusById(bo);
            if (!resultAutorized.Success || !resultAutorized.Data)
            {
                return GenericResponse<UserDto>.Error(ResultType.Error, "Not Autorized Access!", "U_PST_01", StatusCodes.Status203NonAuthoritative);
            }                

            ServiceResult<UserBo> result = await serviceManager.User_Service.CreateAsync(bo);
            if (result.Success)
            {
                bo = result.Data;
                dto.Id = bo.Id;

                if (dto.Role != null && dto.Role.Count > 0)
                {
                    UserRoleBo userRoleBo;
                    foreach (var role in dto.Role)
                    {
                        userRoleBo = new UserRoleBo() { UserId = dto.Id, RoleId = role.Id };
                        await serviceManager.UserRole_Service.CreateAsync(userRoleBo);
                    }
                }

                await serviceManager.CommitAsync();
            }
            else
            {
                return GenericResponse<UserDto>.Error(ResultType.Error, result.Error, "U_PST_02", StatusCodes.Status500InternalServerError);
            }
            
            return GenericResponse<UserDto>.Ok(dto);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("Update/{id}")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.UserUpdate)]
        public async Task<GenericResponse> Put(long id, UserDto dto)
        {
            if (id != dto.Id)
            {
                return GenericResponse.Error(ResultType.Error, "Ids are mismatch!", "U_PT_01", StatusCodes.Status500InternalServerError);
            }
            try
            {
                UserBo bo = UserBo.ConvertToBusinessObject(dto);

                // Yetki kontrolü yapılıyor
                ServiceResult<bool> resultAutorized = await GetAutorizedUserStatusById(bo);
                if (!resultAutorized.Success || !resultAutorized.Data)
                {
                    return GenericResponse.Error(ResultType.Error, "Not Autorized Access!", "U_PT_02", StatusCodes.Status203NonAuthoritative);
                }

                ServiceResult serviceResult = await serviceManager.User_Service.UpdateAsync(id, bo);
                if (serviceResult.Success)
                {
                    if (dto.Role != null && dto.Role.Count > 0)
                    {
                        await serviceManager.UserRole_Service.DeleteByUserIdAsync(dto.Id);

                        UserRoleBo userRole;
                        foreach (var role in dto.Role)
                        {
                            userRole = new UserRoleBo() { UserId = dto.Id, RoleId = role.Id };
                            await serviceManager.UserRole_Service.CreateAsync(userRole);
                        }
                    }

                    await serviceManager.CommitAsync();

                    return GenericResponse.Ok();
                }
                else
                {
                    return GenericResponse.Error(ResultType.Error, serviceResult.Error, "U_PT_03", StatusCodes.Status500InternalServerError);
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Log(ex.Message, LogLevel.Error, this.ControllerContext.RouteData.Values);

                bool exist = await UserExists(id);
                if (!exist)
                {
                    return GenericResponse.Error(ResultType.Error, "Not Found!", "U_PT_04", StatusCodes.Status404NotFound);
                }
                else
                {
                    return GenericResponse.Error(ResultType.Error, ex.Message, "U_PT_05", StatusCodes.Status500InternalServerError);
                }
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("Delete/{id}")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.UserDelete)]
        public async Task<GenericResponse> Delete(long id)
        {
            UserBo userDto = null;
            ServiceResult<UserBo> result = await serviceManager.User_Service.GetByIdAsync(id);
            if (result.Success)
            {
                userDto = result.Data;

                // Yetki kontrolü yapılıyor
                ServiceResult<bool> resultAutorized = await GetAutorizedUserStatusById(userDto);
                if (!resultAutorized.Success || !resultAutorized.Data)
                {
                    return GenericResponse.Error(ResultType.Error, "Not Autorized Access!", "U_DLT_01", StatusCodes.Status203NonAuthoritative);
                }
            }
            else
            {
                return GenericResponse.Error(ResultType.Error, "Not Found!", "U_DLT_02", StatusCodes.Status404NotFound);
            }

            ServiceResult serviceResult = await serviceManager.User_Service.DeleteAsync(id);
            if (serviceResult.Success)
            {
                return GenericResponse.Ok();
            }
            else
            {
                Log(serviceResult.Error, LogLevel.Error, this.ControllerContext.RouteData.Values);

                return GenericResponse.Error(ResultType.Error, serviceResult.Error, "U_DLT_03", StatusCodes.Status500InternalServerError);
            }
        }

        // GET: api/Users/5
        [HttpPost("GetList")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.UserRead)]
        public async Task<GenericResponse<IEnumerable<UserDto>>> GetList([FromBody] FilterCriteria filterCriteria)
        {
            IEnumerable<UserBo> listBo = null;
            IEnumerable<UserDto> listDto = null;
            ServiceResult<IEnumerable<UserBo>> resultList;
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
                    return GenericResponse<IEnumerable<UserDto>>.Error(ResultType.Error, "Not Autorized Access!", "U_GL_01", StatusCodes.Status203NonAuthoritative);
                }
            }
            else
            {
                ServiceResult<UserBo> userDtoResult = await GetCurrentUser();
                if (!userDtoResult.Success || userDtoResult.Data == null)
                {
                    return GenericResponse<IEnumerable<UserDto>>.Error(ResultType.Error, "User Not Found!", "U_GL_02", StatusCodes.Status404NotFound);
                }

                organizationId = userDtoResult.Data.OrganizationId;
            }

            resultList = await serviceManager.User_Service.GetListAsync(organizationId, filterCriteria);
            if (!resultList.Success || resultList.Data == null)
            {
                return GenericResponse<IEnumerable<UserDto>>.Error(ResultType.Error, resultList.Error, "U_GL_03", StatusCodes.Status500InternalServerError);
            }
            
            listBo = resultList.Data;
            listDto = listBo.Select(x => UserBo.ConvertToDto(x)).ToList();

            return GenericResponse<IEnumerable<UserDto>>.Ok(listDto); 
        }

        private async Task<bool> UserExists(long id)
        {
            ServiceResult<UserBo> serviceResult = await serviceManager.User_Service.GetByIdAsync(id);
            return serviceResult.Success && serviceResult.Data != null;
        }
    }
}