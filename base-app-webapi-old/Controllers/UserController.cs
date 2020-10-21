using AutoMapper;
using base_app_common;
using base_app_common.dto.refreshtoken;
using base_app_common.dto.user;
using base_app_service;
using base_app_service.Bo;
using base_app_webapi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
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
    public class UserController : ControllerBase
    {
        private readonly JWTSettings jwtSettings;
        public UserController(IServiceManager serviceManager, IOptions<JWTSettings> jwtSettings, ILogger logger) //: base(serviceManager, logger)
        {
            //this.jwtSettings = jwtSettings.Value;
        }

        #region Authentication   

        // POST: api/Authenticate/
        [HttpPost("GetToken")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenUserDto>> GetToken([FromBody] TokenDto tokenDto)
        {
            return NotFound("User Not Found!");

            UserBo user = null;
            TokenUserDto tokenUserDto = null;
            ServiceResult<IEnumerable<UserBo>> result;

            FilterCriteria filterCriteria = new FilterCriteria();
            filterCriteria.QueryFilter = "EmailAddress = \"" + tokenDto.EmailAddress + "\"";
            filterCriteria.IncludeProperties = "UserType,UserRole,UserRole.Role,UserRole.Role.ActionTypeRole";
            //result = await serviceManager.User_Service.FindAsync(filterCriteria);            

            //if (result.Success)
            //{
            //    user = result.Data.FirstOrDefault();
            //}
            //else
            {
                //Log(result.Error, LogLevel.Error, this.ControllerContext.RouteData.Values);
            }

            if (user == null)
            {
                return NotFound("User Not Found!");
            }
            //user = await _context.User.Include(u => u.UserRole).ThenInclude(ur => ur.Role)
            //                            .Where(u => u.EmailAddress == userDto.EmailAddress).FirstOrDefaultAsync();

            //if (user != null)
            //{
            //    Microsoft.AspNetCore.Identity.PasswordVerificationResult verificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, tokenDto.Password);
            //    if (verificationResult != Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success)
            //        return NotFound("Password verification failed!");
            //}

            if (user != null)
            {
                tokenUserDto = UserBo.ConvertToTokenUserDto(user);

                RefreshTokenBo refreshTokenDto = GenerateRefreshToken();
                user.RefreshToken.Add(refreshTokenDto);
                //await serviceManager.CommitAsync();

                tokenUserDto.RefreshToken = refreshTokenDto.Token;
            }

            //sign your token here here..            
            tokenUserDto.AccessToken = GenerateAccessToken(user);
            return tokenUserDto;
        }

        // GET: api/Authenticate/
        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenUserDto>> RefreshToken([FromBody] RefreshRequestDto refreshRequestDto)
        {
            UserBo userBo = await GetUserFromAccessToken(refreshRequestDto.Access_Token);

            bool validation = await ValidateRefreshToken(userBo, refreshRequestDto.Refresh_Token);
            if (userBo != null && validation)
            {
                TokenUserDto tokenUserDto = UserBo.ConvertToTokenUserDto(userBo);
                tokenUserDto.AccessToken = GenerateAccessToken(userBo);

                return tokenUserDto;
            }

            return null;
        }

        private async Task<bool> ValidateRefreshToken(UserBo userDto, string refreshToken)
        {
            RefreshTokenBo refreshTokenDto = null;
            //ServiceResult<IEnumerable<RefreshTokenBo>> result = await serviceManager.RefreshToken_Service.GetAsync(filter: (rt => rt.Token == refreshToken), orderBy: (rt => rt.OrderBy(x => x.ExpiryDate)));
            //if (result.Success)
            //{
            //    refreshTokenDto = result.Data.FirstOrDefault();
            //}
            //RefreshToken refreshTokenUser = _context.RefreshToken.Where(rt => rt.Token == refreshToken)
            //                                    .OrderByDescending(rt => rt.ExpiryDate)
            //                                    .FirstOrDefault();

            if (refreshTokenDto == null)
            {
                return false;
            }

            if (refreshTokenDto != null && refreshTokenDto.UserId == userDto.Id && refreshTokenDto.ExpiryDate > DateTime.UtcNow)
            {
                return true;
            }

            return false;
        }

        private RefreshTokenBo GenerateRefreshToken()
        {
            RefreshTokenBo refreshToken = new RefreshTokenBo();

            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                refreshToken.Token = Convert.ToBase64String(randomNumber);
            }
            refreshToken.ExpiryDate = DateTime.UtcNow.AddMonths(6);

            return refreshToken;
        }

        private string GenerateAccessToken(UserBo userDto)
        {
            /// NOTICE
            /// Token a iligli kullanıcının id değeri ve yetkisinde olan action id leri ekleniyor 
            /// bunun haricinden yeni değerlerin eklenmesi token boyutunu arttıracağından dolayı 
            /// yeni değerlerin eklenmemesi gerekmektedir
            List<Claim> claims = new List<Claim>();
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
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<UserBo> GetUserFromAccessToken(string accessToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                SecurityToken securityToken;
                var principle = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);

                JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

                if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    var userIdObj = principle.FindFirst(ClaimTypes.Name)?.Value;
                    long userId;
                    if (!long.TryParse(userIdObj, out userId))
                        return new UserBo();

                    UserBo userDto = null;
                    ServiceResult<UserBo> result;
                    FilterCriteria filterCriteria = new FilterCriteria();
                    filterCriteria.QueryFilter = "Id = " + userId;
                    filterCriteria.IncludeProperties = "UserType,UserRole,UserRole.Role,UserRole.Role.GrandRole";
                    //ServiceResult<IEnumerable<UserBo>> resultList = await serviceManager.User_Service.FindAsync(filterCriteria);
                    //if (resultList.Success && resultList.Data != null && resultList.Data.FirstOrDefault() != null)
                    //    userDto = resultList.Data.FirstOrDefault();
                    //else
                        userDto = new UserBo();

                    return userDto;

                    //ServiceResult<UserDto> result = await serviceManager.User_Service.GetByIdAsync(userId);
                    //if (result.Success)
                    //    return result.Data;
                    //else
                    //    return new UserDto();
                    //User user = await _context.User.Include(u => u.UserRole).ThenInclude(ur => ur.Role)
                    //                    .Where(u => u.Id == Convert.ToInt32(userId)).FirstOrDefaultAsync();

                    //UserDto userDto = mapper.Map<UserDto>(user);
                    //return userDto;

                }
            }
            catch (Exception ex)
            {
                //Log(ex.Message, LogLevel.Error, this.ControllerContext.RouteData.Values);

                return new UserBo();
            }

            return new UserBo();
        }
        #endregion

        // GET: api/Users/5
        [HttpGet("Get/{id}")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.UserRead)]
        public async Task<ActionResult<UserDto>> Get(long id)
        {
            return NotFound();

            UserBo userBo = null;
            FilterCriteria filterCriteria = new FilterCriteria();
            filterCriteria.QueryFilter = "Id = " + id;
            filterCriteria.IncludeProperties = "UserRole,UserRole.Role,UserRole.Role.ActionTypeRole,UserRole.Role.ActionTypeRole.ActionType";
            //ServiceResult<IEnumerable<UserBo>> result = await serviceManager.User_Service.FindAsync(filterCriteria);
            //if (result.Success)
            //{
            //    userBo = result.Data.FirstOrDefault();
            //    if (userBo == null)
            //        return NotFound();
            //    else
            //    {
            //        // Yetki kontrolü yapılıyor
            //        ServiceResult<bool> resultAutorized = await GetAutorizedUserStatusById(userBo);
            //        if (!resultAutorized.Success || !resultAutorized.Data)
            //            return BadRequest("Not Autorized Access!");
            //    }

            //    UserDto userDto = UserBo.ConvertToDto(userBo);

            //    return userDto;
            //}
            //else
            //{
                //Log(result.Error, LogLevel.Error, this.ControllerContext.RouteData.Values);
                //return BadRequest(result.Error);
            //}

            //var user = await _context.User.Include(u => u.UserRole).ThenInclude(ur => ur.Role)
            //                                .Where(u => u.Id == id)
            //                                .FirstOrDefaultAsync();
        }

        // POST: api/Users
        [HttpPost("Create")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.UserCreate)]
        public async Task<ActionResult<UserDto>> Post([FromBody] UserDto dto)
        {
            UserBo bo = UserBo.ConvertToBusinessObject(dto);

            //// Yetki kontrolü yapılıyor
            //ServiceResult<bool> resultAutorized = await GetAutorizedUserStatusById(bo);
            //if (!resultAutorized.Success || !resultAutorized.Data)
            //    return BadRequest("Not Autorized Access!");

            //ServiceResult<UserBo> result = await serviceManager.User_Service.CreateAsync(bo);
            //if (result.Success)
            //{
            //    bo = result.Data;
            //    dto.Id = bo.Id;

            //    if (dto.Role != null && dto.Role.Count > 0)
            //    {
            //        UserRoleBo userRoleBo;
            //        foreach (var role in dto.Role)
            //        {
            //            userRoleBo = new UserRoleBo() { UserId = dto.Id, RoleId = role.Id };
            //            await serviceManager.UserRole_Service.CreateAsync(userRoleBo);
            //        }
            //    }

            //    await serviceManager.CommitAsync();
            //}
            //else
            //{
            //    return BadRequest(result.Error);
            //}

            return dto;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("Update/{id}")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.UserUpdate)]
        public async Task<IActionResult> Put(long id, UserDto dto)
        {
            return NotFound();

            if (id != dto.Id)
            {
                return BadRequest();
            }
            try
            {
                UserBo bo = UserBo.ConvertToBusinessObject(dto);

                //// Yetki kontrolü yapılıyor
                //ServiceResult<bool> resultAutorized = await GetAutorizedUserStatusById(bo);
                //if (!resultAutorized.Success || !resultAutorized.Data)
                //    return BadRequest("Not Autorized Access!");

                //ServiceResult serviceResult = await serviceManager.User_Service.UpdateAsync(id, bo);
                //if (serviceResult.Success)
                //{
                //    if (dto.Role != null && dto.Role.Count > 0)
                //    {
                //        await serviceManager.UserRole_Service.DeleteByUserIdAsync(dto.Id);

                //        UserRoleBo userRole;
                //        foreach (var role in dto.Role)
                //        {
                //            userRole = new UserRoleBo() { UserId = dto.Id, RoleId = role.Id };
                //            await serviceManager.UserRole_Service.CreateAsync(userRole);
                //        }
                //    }

                //    await serviceManager.CommitAsync();

                //    return Ok();
                //}
                //else
                //{
                //    return BadRequest(serviceResult.Error);
                //}
            }
            catch (DbUpdateConcurrencyException ex)
            {
                //Log(ex.Message, LogLevel.Error, this.ControllerContext.RouteData.Values);

                bool exist = await UserExists(id);
                if (!exist)
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("Delete/{id}")]
        [GrandAuthorize(GrandPermission.EndpointPermission, Grands.UserDelete)]
        public async Task<ActionResult> Delete(long id)
        {
            UserBo userDto = null;
            //ServiceResult<UserBo> result = await serviceManager.User_Service.GetByIdAsync(id);
            //if (result.Success)
            //{
            //    userDto = result.Data;

            //    // Yetki kontrolü yapılıyor
            //    ServiceResult<bool> resultAutorized = await GetAutorizedUserStatusById(userDto);
            //    if (!resultAutorized.Success || !resultAutorized.Data)
            //        return BadRequest("Not Autorized Access!");
            //}
            //else
            {
                return BadRequest("Not Found!");
            }

            //ServiceResult serviceResult = await serviceManager.User_Service.DeleteAsync(id);
            //if (serviceResult.Success)
            {
                return Ok();
            }
            //else
            //{
            //    //Log(serviceResult.Error, LogLevel.Error, this.ControllerContext.RouteData.Values);
            //    return BadRequest(serviceResult.Error);
            //}
        }

        private async Task<bool> UserExists(long id)
        {
            //ServiceResult<UserBo> serviceResult = await serviceManager.User_Service.GetByIdAsync(id);
            //return serviceResult.Success && serviceResult.Data != null;
            return false;
        }
    }
}