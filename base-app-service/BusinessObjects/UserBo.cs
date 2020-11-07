using base_app_common.dto.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace base_app_service.Bo
{
    public class UserBo
    {
        public UserBo()
        {
            UserToken = new HashSet<UserTokenBo>();
            UserRole = new HashSet<UserRoleBo>();
            UserLogin = new HashSet<UserLoginBo>();
        }

        public long Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public long OrganizationId { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public long UserTypeId { get; set; }
        
        public virtual OrganizationBo Organization { get; set; }
        [JsonIgnore]
        public virtual ICollection<UserLoginBo> UserLogin { get; set; }
        [JsonIgnore]
        public ICollection<UserTokenBo> UserToken { get; set; }
        [JsonIgnore]
        public ICollection<UserRoleBo> UserRole { get; set; }

        public virtual UserTypeBo UserType { get; set; }

        public static UserDto ConvertToDto(UserBo bo)
        {
            if (bo == null)
                return null;

            UserDto dto = new UserDto();
            dto.Id = bo.Id;
            dto.FirstName = bo.FirstName;
            dto.MiddleName = bo.MiddleName;
            dto.LastName = bo.LastName;
            dto.EmailAddress = bo.EmailAddress;
            dto.Password = bo.Password;
            dto.OrganizationId = bo.OrganizationId;
            dto.UserTypeId = bo.UserTypeId;

            for (int i = 0; i < bo.UserRole.Count; i++)
            {
                if (bo.UserRole.ElementAt(i).Role != null)
                    dto.Role.Add(RoleBo.ConvertToDto(bo.UserRole.ElementAt(i).Role));
            }

            return dto;
        }

        public static UserBo ConvertToBusinessObject(UserDto dto)
        {
            if (dto == null)
                return null;

            UserBo bo = new UserBo();
            bo.Id = dto.Id;
            bo.FirstName = dto.FirstName;
            bo.MiddleName = dto.MiddleName;
            bo.LastName = dto.LastName;
            bo.EmailAddress = dto.EmailAddress;
            bo.Password = dto.Password;
            bo.OrganizationId = dto.OrganizationId;
            bo.UserTypeId = dto.UserTypeId;

            return bo;
        }

        public static TokenResponseDto ConvertToTokenResponseDto(UserBo bo)
        {
            TokenResponseDto tokenUserDto = new TokenResponseDto();
            tokenUserDto.Id = bo.Id;
            tokenUserDto.FirstName = bo.FirstName;
            tokenUserDto.MiddleName = bo.MiddleName;
            tokenUserDto.LastName = bo.LastName;
            tokenUserDto.EmailAddress = bo.EmailAddress;
            tokenUserDto.OrganizationId = bo.OrganizationId;
            tokenUserDto.LastLoginTime = bo.LastLoginTime;
            tokenUserDto.UserTypeId = bo.UserTypeId;

            return tokenUserDto;
        }
    }
}
