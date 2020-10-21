using System;
using System.Collections.Generic;

namespace base_app_repository.Entities
{
    public partial class Grand
    {
        public Grand()
        {
            GrandRole = new HashSet<GrandRole>();
        }

        public long Id { get; set; }
        public string GrandName { get; set; }

        public virtual ICollection<GrandRole> GrandRole { get; set; }
    }
}
