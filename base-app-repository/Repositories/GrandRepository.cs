using base_app_repository.Entities;
using base_app_repository.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace base_app_repository.Repositories
{
    public class GrandRepository : BaseRepository<Grand>, IGrandRepository<Grand>
    {
        public GrandRepository(BaseDbContext context) : base(context)
        {
        }
    }
}
