using System;
using System.Collections.Generic;

namespace base_app_repository.Entities
{
    public partial class RefreshToken
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }

        public virtual User User { get; set; }
    }
}
