using base_app_repository.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace base_app_repository.Repositories
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository<RefreshToken>
    {
        public RefreshTokenRepository(BaseDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken> GetLastByUserIdAsync(long userid)
        {
            return await dbSet.Where(x => x.UserId == userid).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        }
    }
}
