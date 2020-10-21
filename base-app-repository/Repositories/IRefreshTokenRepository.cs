using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace base_app_repository.Repositories
{
    interface IRefreshTokenRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetLastByUserIdAsync(long userid);
    }
}
