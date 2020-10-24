namespace base_app_webapi.Models
{
    public static class GrandFilterType
    {
        public const string Or = "Or";
        public const string And = "And";
    }

    public static class GrandPermission
    {
        // Endpoint
        public const string EndpointPermission = "EndpointPermission";
    }

    public static class Grands
    {   
        // Organization
        public const string OrganizationCreate = "1";
        public const string OrganizationRead = "2";
        public const string OrganizationUpdate = "3";
        public const string OrganizationDelete = "4";
        // User
        public const string UserCreate = "5";
        public const string UserRead = "6";
        public const string UserUpdate = "7";
        public const string UserDelete = "8";
        // Role
        public const string RoleCreate = "9";
        public const string RoleRead = "10";
        public const string RoleUpdate = "11";
        public const string RoleDelete = "12";
        // UserLogin
        public const string UserLoginCreate = "13";
        public const string UserLoginRead = "14";
        public const string UserLoginUpdate = "15";
        public const string UserLoginDelete = "16";
        // RefreshToken
        public const string RefreshTokenCreate = "17";
        public const string RefreshTokenRead = "18";
        public const string RefreshTokenUpdate = "19";
        public const string RefreshTokenDelete = "20";        
        // Page
        public const string PageCreate = "21";
        public const string PageRead = "22";
        public const string PageUpdate = "23";
        public const string PageDelete = "24";        
    }


}