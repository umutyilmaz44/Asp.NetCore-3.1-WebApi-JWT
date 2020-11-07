using System;
using System.Collections.Generic;

namespace base_app_repository.Entities
{
    public partial class User
    {
        public User()
        {
            UserLogin = new HashSet<UserLogin>();
            UserRole = new HashSet<UserRole>();
            UserToken = new HashSet<UserToken>();
        }

        public long Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public long OrganizationId { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public long UserTypeId { get; set; }
        public bool Deleted { get; set; }

        public virtual Organization Organization { get; set; }
        public virtual UserType UserType { get; set; }
        public virtual ICollection<UserLogin> UserLogin { get; set; }
        public virtual ICollection<UserRole> UserRole { get; set; }
        public virtual ICollection<UserToken> UserToken { get; set; }
    }
}
