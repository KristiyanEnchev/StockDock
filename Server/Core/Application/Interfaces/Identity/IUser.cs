namespace Application.Interfaces.Identity
{
    public interface IUser
    {
        string? Id { get; }
        string? Email { get; }
    }
}