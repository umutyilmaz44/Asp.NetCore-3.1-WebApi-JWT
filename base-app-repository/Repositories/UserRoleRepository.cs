using base_app_repository.Entities;

namespace base_app_repository.Repositories
{
    public class UserRoleRepository : BaseRepository<UserRole>, IUserRoleRepository<UserRole>
    {
        public UserRoleRepository(BaseDbContext context) : base(context)
        {
        }
    }
}
