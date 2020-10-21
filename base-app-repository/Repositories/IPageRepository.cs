using System;
using System.Collections.Generic;
using System.Text;

namespace base_app_repository.Repositories
{
    interface IPageRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
    }
}
