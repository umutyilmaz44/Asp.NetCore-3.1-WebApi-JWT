using base_app_repository.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace base_app_repository.Repositories
{
    public class UserLoginRepository : BaseRepository<UserLogin>, IUserLoginRepository<UserLogin>
    {
        public UserLoginRepository(BaseDbContext context) : base(context)
        {
        }
    }
}
