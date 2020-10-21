using base_app_repository.Entities;

namespace base_app_repository.Repositories
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository<Role>
    {
        public RoleRepository(BaseDbContext context) : base(context)
        {
        }
    }
}
