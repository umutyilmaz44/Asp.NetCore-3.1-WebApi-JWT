using base_app_repository.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace base_app_repository.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository<User>
    {
        public UserRepository(BaseDbContext context) : base(context)
        {
        }

        public async Task<User> GetByEmailAddressAsync(string emailAddress)
        {
            return await dbSet.FirstOrDefaultAsync(x => x.EmailAddress == emailAddress && !x.Deleted);
        }
    }
}
