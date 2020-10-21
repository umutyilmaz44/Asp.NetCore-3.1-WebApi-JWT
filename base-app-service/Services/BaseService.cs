using AutoMapper;
using base_app_common;
using base_app_repository;
using base_app_repository.Entities;
using Microsoft.AspNetCore.Identity;

namespace base_app_service.Services
{
    public class BaseService
    {
        protected readonly IMapper mapper;
        protected readonly BaseDbContext context;
        protected readonly ServiceContext serviceContext;
        protected readonly RepositoryManager repositoryManager;
        protected readonly UpperInvariantLookupNormalizer normalizer;
        protected readonly IServiceManager serviceManager;

        public BaseService(ServiceContext serviceContext, IServiceManager serviceManager)
        {
            this.serviceContext = serviceContext;
            this.mapper = serviceContext.GetItem<IMapper>("IMapper");
            this.context = serviceContext.GetItem<BaseDbContext>("baseDbContext");
            this.repositoryManager = new RepositoryManager(this.context);
            this.normalizer = new UpperInvariantLookupNormalizer();
            this.serviceManager = serviceManager;
        }
    }
}
