namespace base_app_service.Bo
{
    public class GrandRoleBo
    {
        public long Id { get; set; }
        public long RoleId { get; set; }
        public long GrandId { get; set; }
        
        public virtual GrandBo Grand { get; set; }
        
        public virtual RoleBo Role { get; set; }
    }
}
