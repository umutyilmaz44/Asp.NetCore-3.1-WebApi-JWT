using base_app_common.dto.user;
using System;
using System.Collections.Generic;
using System.Text;

namespace base_app_common.dto.refreshtoken
{
    public class UserTokenDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string RefreshToken { get; set; }
        public bool IsLogout { get; set; }
        public DateTime? LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }

        public virtual UserDto User { get; set; }
    }
}
