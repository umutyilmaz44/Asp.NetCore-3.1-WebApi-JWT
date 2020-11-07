using System;
using System.Collections.Generic;

namespace base_app_repository.Entities
{
    public partial class UserToken
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string RefreshToken { get; set; }
        public bool IsLogout { get; set; }
        public DateTime? LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }

        public virtual User User { get; set; }
    }
}
