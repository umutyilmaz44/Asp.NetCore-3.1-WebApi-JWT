using base_app_repository.Entities;

namespace base_app_repository.Repositories
{
    public class OrganizationRepository : BaseRepository<Organization>, IOrganizationRepository<Organization>
    {
        public OrganizationRepository(BaseDbContext context) : base(context)
        {
        }
    }
}
