using Domain.Entities.Base;

namespace Domain.Entities.Identity
{
    public class User : BaseIdentityAuditableEntity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}