namespace Domain.Interfaces
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        string? DeletedBy { get; set; }
        DateTimeOffset? DeletedDate { get; set; }
    }
}