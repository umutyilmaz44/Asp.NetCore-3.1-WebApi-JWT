using base_app_repository.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace base_app_repository.Repositories
{
    interface IGrandRoleRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {        
    }
}
