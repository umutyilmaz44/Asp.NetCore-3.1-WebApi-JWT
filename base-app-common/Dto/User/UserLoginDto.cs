using System;
using System.Collections.Generic;
using System.Text;

namespace base_app_common.dto.user
{
    public class UserLoginDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime LoginTime { get; set; }

        public virtual UserDto User { get; set; }
    }
}
