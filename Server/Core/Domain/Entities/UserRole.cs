namespace Domain.Entities
{
    public class UserRole : BaseIdentityRoleAuditableEntity
    {
        public string? Description { get; set; }

        public UserRole(string name, string? description = null) : base(name)
        {
            Description = description;
        }
    }
}