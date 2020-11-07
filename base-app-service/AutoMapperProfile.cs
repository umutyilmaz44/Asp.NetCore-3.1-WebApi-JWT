using AutoMapper;
using base_app_repository.Entities;
using base_app_service.Bo;
using System;

namespace base_app_service
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Grand, GrandBo>();
            CreateMap<GrandBo, Grand>();

            CreateMap<GrandRole, GrandRoleBo>();
            CreateMap<GrandRoleBo, GrandRole>();

            CreateMap<Organization, OrganizationBo>();
            CreateMap<OrganizationBo, Organization>();

            CreateMap<Page, PageBo>();
            CreateMap<PageBo, Page>();

            CreateMap<UserToken, UserTokenBo>();
            CreateMap<UserTokenBo, UserToken>();

            CreateMap<Role, RoleBo>();
            CreateMap<RoleBo, Role>();

            CreateMap<User, UserBo>();
            CreateMap<UserBo, User>();

            CreateMap<UserLogin, UserLoginBo>();
            CreateMap<UserLoginBo, UserLogin>();

            CreateMap<UserRole, UserRoleBo>();
            CreateMap<UserRoleBo, UserRole>();

            CreateMap<UserType, UserTypeBo>();
            CreateMap<UserTypeBo, UserType>();
        }        
    }
}
