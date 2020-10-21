using base_app_common.dto.role;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace base_app_common.dto.grand
{
    public class GrandRoleDto
    {
        public long Id { get; set; }
        public long RoleId { get; set; }
        public long ActionTypeId { get; set; }
        
        public virtual GrandDto Grand { get; set; }
        
        public virtual RoleDto Role { get; set; }
    }
}
