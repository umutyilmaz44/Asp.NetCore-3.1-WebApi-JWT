using System;
using System.Collections.Generic;

namespace base_app_repository.Entities
{
    public partial class UserLogin
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime LoginTime { get; set; }

        public virtual User User { get; set; }

    }
}
