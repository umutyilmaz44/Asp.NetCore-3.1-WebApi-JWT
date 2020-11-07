using base_app_repository.Entities;
using base_app_repository.Repositories;
using System;
using System.Threading.Tasks;

namespace base_app_repository
{
    public class RepositoryManager : IDisposable
    {
        public BaseDbContext context { get; internal set; }

        public RepositoryManager()
        {
            this.context = new BaseDbContext();
        }

        public RepositoryManager(BaseDbContext baseDbContext)
        {
            this.context = baseDbContext;
        }

        private OrganizationRepository organizationRepository;
        public OrganizationRepository OrganizationRepository
        {
            get
            {
                this.organizationRepository = this.organizationRepository ?? new OrganizationRepository(context);
                return this.organizationRepository;
            }
        }

        private UserRepository userRepository;
        public UserRepository UserRepository
        {
            get
            {
                this.userRepository = this.userRepository ?? new UserRepository(context);
                return this.userRepository;
            }
        }

        private UserLoginRepository userLoginRepository;
        public UserLoginRepository UserLoginRepository
        {
            get
            {
                this.userLoginRepository = this.userLoginRepository ?? new UserLoginRepository(context);
                return this.userLoginRepository;
            }
        }

        private UserRoleRepository userRoleRepository;
        public UserRoleRepository UserRoleRepository
        {
            get
            {
                this.userRoleRepository = this.userRoleRepository ?? new UserRoleRepository(context);
                return this.userRoleRepository;
            }
        }

        private UserTokenRepository userTokenRepository;
        public UserTokenRepository UserTokenRepository
        {
            get
            {
                this.userTokenRepository = this.userTokenRepository ?? new UserTokenRepository(context);
                return this.userTokenRepository;
            }
        }

        private PageRepository pageRepository;
        public PageRepository PageRepository
        {
            get
            {
                this.pageRepository = this.pageRepository ?? new PageRepository(context);
                return this.pageRepository;
            }
        }

        private GrandRepository pageRoleRepository;
        public GrandRepository PageRoleRepository
        {
            get
            {
                this.pageRoleRepository = this.pageRoleRepository ?? new GrandRepository(context);
                return this.pageRoleRepository;
            }
        }

        private RoleRepository roleRepository;
        public RoleRepository RoleRepository
        {
            get
            {
                this.roleRepository = this.roleRepository ?? new RoleRepository(context);
                return this.roleRepository;
            }
        }


        public async Task CommitAsync()
        {
            await context.SaveChangesAsync();
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
