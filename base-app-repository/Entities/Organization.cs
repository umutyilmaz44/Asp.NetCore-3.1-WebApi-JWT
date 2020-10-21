using System;
using System.Collections.Generic;

namespace base_app_repository.Entities
{
    public partial class Organization
    {
        public Organization()
        {
            User = new HashSet<User>();
        }

        public long Id { get; set; }
        public long? ParentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime RecordDate { get; set; }

        public virtual ICollection<User> User { get; set; }
    }
}
