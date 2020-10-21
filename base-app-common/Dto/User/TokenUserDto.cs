using System;
using System.Collections.Generic;
using System.Text;

namespace base_app_common.dto.user
{
    public class TokenUserDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public long OrganizationId { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public long UserTypeId { get; set; }

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

    }
}
