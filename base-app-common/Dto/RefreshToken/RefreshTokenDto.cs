using base_app_common.dto.user;
using System;
using System.Collections.Generic;
using System.Text;

namespace base_app_common.dto.refreshtoken
{
    public class RefreshTokenDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }

        public virtual UserDto User { get; set; }
    }
}
