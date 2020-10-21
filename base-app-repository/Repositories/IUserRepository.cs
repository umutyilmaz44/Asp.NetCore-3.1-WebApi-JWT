using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace base_app_repository.Repositories
{
    interface IUserRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByEmailAddressAsync(string emailAddress);
    }
}
