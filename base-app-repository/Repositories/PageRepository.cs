using base_app_repository.Entities;

namespace base_app_repository.Repositories
{
    public class PageRepository : BaseRepository<Page>, IPageRepository<Page>
    {
        public PageRepository(BaseDbContext context) : base(context)
        {
        }
    }
}
