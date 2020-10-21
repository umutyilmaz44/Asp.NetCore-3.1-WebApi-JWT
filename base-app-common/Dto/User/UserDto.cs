using base_app_common.dto.role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace base_app_common.dto.user
{
    public class UserDto
    {
        public UserDto()
        {
            Role = new HashSet<RoleDto>();
        }
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public long OrganizationId { get; set; }

        public long UserTypeId { get; set; }

        public ICollection<RoleDto> Role { get; set; }
    }
}
