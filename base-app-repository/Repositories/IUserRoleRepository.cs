using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace base_app_repository.Repositories
{
    interface IUserRoleRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {        
    }
}
