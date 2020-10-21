using base_app_common.dto.user;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace base_app_service.Bo
{
    public class UserRoleBo
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long RoleId { get; set; }
        
        public virtual RoleBo Role { get; set; }
        
        public virtual UserBo User { get; set; }

        public static UserRoleBo ConvertToBusinessObject(UserRoleDto dto)
        {
            if (dto == null)
                return null;

            UserRoleBo bo = new UserRoleBo();
            bo.Id = dto.Id;
            bo.Role = RoleBo.ConvertToBusinessObject(dto.Role);
            bo.RoleId = dto.RoleId;
            bo.User = UserBo.ConvertToBusinessObject(dto.User);
            bo.UserId = dto.UserId;
            
            return bo;
        }

        public static UserRoleDto ConvertToDto(UserRoleBo bo)
        {
            if (bo == null)
                return null;

            UserRoleDto dto = new UserRoleDto();
            dto.Id = bo.Id;
            dto.Role = RoleBo.ConvertToDto(bo.Role);
            dto.RoleId = bo.RoleId;
            dto.User = UserBo.ConvertToDto(bo.User);
            dto.UserId = bo.UserId;

            return dto;
        }
    }
}
