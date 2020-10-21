using base_app_common;
using base_app_service.Services;
using System.Threading.Tasks;

namespace base_app_service
{
    public interface IServiceManager
    {
        ServiceContext serviceContext { get; }
                

        OrganizationService Organization_Service { get; }

        UserService User_Service { get; }

        UserLoginService UserLogin_Service { get; }

        RoleService Role_Service { get; }

        UserRoleService UserRole_Service { get; }

        RefreshTokenService RefreshToken_Service { get; }

        Task CommitAsync();
    }
}
