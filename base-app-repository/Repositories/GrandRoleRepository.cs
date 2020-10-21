using base_app_repository.Entities;
using base_app_repository.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace base_app_repository.Repositories
{
    public class GrandRoleRepository : BaseRepository<GrandRole>, IGrandRoleRepository<GrandRole>
    {
        public GrandRoleRepository(BaseDbContext context) : base(context)
        {
        }
    }
}
