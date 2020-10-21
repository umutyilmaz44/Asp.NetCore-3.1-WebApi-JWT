using base_app_common.dto.role;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace base_app_common.dto.user
{
    public class UserRoleDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long RoleId { get; set; }
        
        public virtual RoleDto Role { get; set; }
        
        public virtual UserDto User { get; set; }
    }
}
