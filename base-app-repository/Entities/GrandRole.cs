namespace base_app_repository.Entities
{
    public partial class GrandRole
    {
        public long Id { get; set; }
        public long RoleId { get; set; }
        public long GrandId { get; set; }

        public virtual Grand Grand { get; set; }
        public virtual Role Role { get; set; }
    }
}
