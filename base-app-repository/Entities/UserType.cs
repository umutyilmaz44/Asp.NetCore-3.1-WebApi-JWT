using System;
using System.Collections.Generic;

namespace base_app_repository.Entities
{
    public partial class UserType
    {
        public UserType()
        {
            User = new HashSet<User>();
        }

        public long Id { get; set; }
        public string TypeName { get; set; }
        public string TypeDescription { get; set; }
        public int TokenLifeTime { get; set; }

        public virtual ICollection<User> User { get; set; }
    }
}
