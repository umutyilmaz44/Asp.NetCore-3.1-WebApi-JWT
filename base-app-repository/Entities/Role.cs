using System;
using System.Collections.Generic;

namespace base_app_repository.Entities
{
    public partial class Role
    {
        public Role()
        {
            GrandRole = new HashSet<GrandRole>();
            UserRole = new HashSet<UserRole>();
        }

        public long Id { get; set; }
        public string RoleName { get; set; }
        public string Desc { get; set; }

        public virtual ICollection<GrandRole> GrandRole { get; set; }
        public virtual ICollection<UserRole> UserRole { get; set; }
    }
}
